using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

using MapLib.Tiles;
using MapLib.Sources.MemCache;
using MapLib.Sources.DiskCache;
using DbgLib;

namespace MapLib.Service
{
  /// <summary>
  /// Singleton:
  /// 
  /// Scheduler for asynch requests of any kind
  ///  Abort only when the owning Component is Disposed !!
  /// 
  /// Exposes:
  ///   TileLoading facilities
  ///   --
  /// 
  /// </summary>
  internal sealed class RequestScheduler
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // Singleton Pattern
    public static RequestScheduler Instance => lazy.Value;
    private static readonly Lazy<RequestScheduler> lazy = new Lazy<RequestScheduler>( ( ) => new RequestScheduler( ) );

    // Worker threads
    // Mem Cache retrieval has a severe penalty for Queued times and processing when more threads are involved (reason unknown)
    // ?? ConcurrentDictionary access ?? 
    private const int NUM_OF_THREAD_TILE = 8;  // 20231124 Redesign to throttle only HTTP calls
    private const int NUM_OF_THREAD_CACHE = 4;  // Disk Caching may use more..

    // interval of the PingEvent
    private readonly TimeSpan c_pingInterval = new TimeSpan( 0, 0, 0, 5 ); // 5sec

    // the dispatcher
    private Thread _dispatcherTask = null;
    private bool _abortDispatcher = false;

    // TileLoaderJob dispatching
    private ConcurrentQueue<LoaderJobWrapper> _tileLoaderJobQueue;
    private TileWorkflowCat _tileworkflowCatalog;

    // DiskCacheJob dispatching
    private ConcurrentQueue<DiskCacheJob> _diskCacheJobQueue;

    // cache providers
    private DiskSource _diskSource;
    private MemorySource _memorySource;

    /// <summary>
    /// A scheduler Ping even triggered every .. sec 
    ///  - not this is a separate thread that pings
    /// </summary>
    public event EventHandler PingEvent;
    private void OnPingEvent( )
    {
      _tileLoaderService.WorkflowCleanup( );
      PingEvent?.Invoke( this, EventArgs.Empty );
    }

    /// <summary>
    /// True if the Scheduler is running
    /// </summary>
    public bool IsAlive => _dispatcherTask.IsAlive;

    /// <summary>
    /// Get: Memory Cache Status
    /// </summary>
    public bool ServiceStatus_MemoryCache => _memorySource.ProviderEnabled;
    /// <summary>
    /// Get: Disk Cache Status
    /// </summary>
    public bool ServiceStatus_DiskCache => _diskSource.ProviderEnabled;
    /// <summary>
    /// Get: Provider Source Status
    /// </summary>
    public bool ServiceStatus_Providers => Sources.Providers.MapProviderBase.Enabled;


    /// <summary>
    /// Get: Internal Access to the MemorySource
    /// </summary>
    public MemorySource MemorySource => _memorySource;

    /// <summary>
    /// Get: Internal Access to the DiskSource
    /// </summary>
    public DiskSource DiskSource => _diskSource;



    // cTor: Hidden
    private RequestScheduler( )
    {
      // the dispatcher
      _dispatcherTask = new Thread( this.RunDispatcher ) {
        IsBackground = true // set to background so it will not prevent the App from shutting down
      };

      // TileLoaderJob dispatching
      _tileworkflowCatalog = new TileWorkflowCat( );
      _tileLoaderJobQueue = new ConcurrentQueue<LoaderJobWrapper>( );

      // DiskCacheJob dispatching
      _diskCacheJobQueue = new ConcurrentQueue<DiskCacheJob>( );

      // Create Sources
      _diskSource = new DiskSource( );
      _memorySource = new MemorySource( );

      // finally let's go
      _dispatcherTask.Start( );
    }

    /// <summary>
    /// Sets the abort status for the listener and waits for the exit
    /// </summary>
    public void Abort( )
    {
      _abortDispatcher = true; // atomic - no sync needed
      if (_dispatcherTask != null && _dispatcherTask.IsAlive) {
        _dispatcherTask.Join( ); // waits for the threads to terminate and finally for the ListenerTask itself
      }
      _dispatcherTask = null;

      // Remove all pending jobs in the pool
      while (_tileLoaderJobQueue.Count > 0) {
        _tileLoaderJobQueue.TryDequeue( out LoaderJobWrapper _ );
      }
      while (_diskCacheJobQueue.Count > 0) {
        _diskCacheJobQueue.TryDequeue( out DiskCacheJob _ );
      }

    }

    /// <summary>
    /// Set the Service status for the Memory Cache
    /// </summary>
    /// <param name="enabled">Service status</param>
    public void SetServiceStatus_MemoryCache( bool enabled )
    {
      _memorySource.ProviderEnabled = enabled;
    }

    /// <summary>
    /// Set the Service status for the Disk Cache
    /// </summary>
    /// <param name="enabled">Service status</param>
    public void SetServiceStatus_DiskCache( bool enabled )
    {
      _diskSource.ProviderEnabled = enabled;
    }

    /// <summary>
    /// Set the Service status for the Map Providers
    /// </summary>
    /// <param name="enabled">Service status</param>
    public void SetServiceStatus_MapProvider( bool enabled )
    {
      Sources.Providers.MapProviderBase.SetServiceStatus( enabled );
    }


    #region TileLoader Access

    private TileLoaderService _tileLoaderService;

    /// <summary>
    /// Only JobNumbers > than this number are processed
    /// </summary>
    public int JobNumberLimit {
      get => _jobNumberLimit;
      set {
        _jobNumberLimit = value;
        _tileLoaderService.UpdateJobNumberLimit( value );
      }
    }
    private int _jobNumberLimit = 0;

    /// <summary>
    /// Grants access to the Catalog of received Tiles
    /// -> USE FullKey to retrieve images
    /// </summary>
    public TileWorkflowCat TileWorkflowCatalog => _tileworkflowCatalog;

    /// <summary>
    /// Returns a Job Wrapper with the currently available Services loaded
    /// </summary>
    /// <returns>A LoaderJobWrapper or null</returns>
    public LoaderJobWrapper GetJobWrapper( TileLoaderJob tileLoaderJob )
    {
      if (tileLoaderJob.JobNumber > JobNumberLimit) {
        // only if not already discarded by JobNumber

        var jobWrapper = new LoaderJobWrapper( tileLoaderJob );
        // add our Sources in FIFO Manner (provider is added already..)
        if (_diskSource.ProviderEnabled) jobWrapper.AddSource( _diskSource );
        if (_memorySource.ProviderEnabled) jobWrapper.AddSource( _memorySource );
        return jobWrapper;
      }

      return null;
    }

    /// <summary>
    /// Add one TileLoader job
    /// </summary>
    /// <param name="tileLoaderJob">A TileLoader job</param>
    public void Add_TileLoaderJob( TileLoaderJob tileLoaderJob )
    {
      var jobWrapper = GetJobWrapper( tileLoaderJob );
      if (jobWrapper != null) {
        // send for processing
        _tileLoaderJobQueue.Enqueue( jobWrapper );
      }
    }

    #endregion

    #region DiskCache Access

    private DiskCacheService _diskCacheService;

    /// <summary>
    /// Add one DiskCache job
    /// </summary>
    /// <param name="diskCacheJob">A DiskCache job</param>
    public void Add_DiskCacheJob( DiskCacheJob diskCacheJob )
    {
      // send for processing
      _diskCacheJobQueue.Enqueue( diskCacheJob );
    }

    #endregion

    /// <summary>
    /// Master Dispatcher Task Routine 
    /// </summary>
    private void RunDispatcher( )
    {
      // trigger for run once at startup jobs
      bool runOnce = true;
      DateTime lastPing = DateTime.MinValue;

      // Client Task to handle client requests
      _tileLoaderService = new TileLoaderService( _tileLoaderJobQueue, _tileworkflowCatalog );
      _tileLoaderService.StartThreads( ); // start the threads

      _diskCacheService = new DiskCacheService( _diskCacheJobQueue );
      _diskCacheService.StartThreads( ); // start the threads

      // Task to maintain the Cache Size of the Memory Cache
      try {
        // checks if we have to abort
        while (!_abortDispatcher) {
          if (runOnce) {
            // run once asynch chores could be added here
            _diskSource.MaintainCacheSize( );
            // not any longer
            runOnce = false;
          }

          // other asynch chores could be added here
          _memorySource.MaintainCacheSize( ); // will cleanup if needed

          // .. 
          // wait a while 
          Thread.Sleep( 500 ); // runs at a rel. slow pace

          // issue pings at an interval
          if (DateTime.Now > (lastPing + c_pingInterval)) {
            OnPingEvent( );
            lastPing = DateTime.Now;
          }

        }// loop

      }
      catch (Exception ex) {
        LOG.Error( "RequestScheduler.StartDispatcher", ex, "Error - task loop run into an Exception" );
      }

      LOG.Info( "RequestScheduler.StartDispatcher", $"Aborted on command" );

      // Stop client requests handling
      _tileLoaderService.Stop( ); _tileLoaderService = null;
      _diskCacheService.Stop( ); _diskCacheService = null;
    }


    #region Class TileLoaderService

    /// <summary>
    /// Creates a number of services that will handle the jobs waiting in the queue
    /// </summary>
    private class TileLoaderService
    {
      private const int c_TimeoutIdle = 200; // task wait while in idle
      private const int c_TimeoutBusy = 5;   // task wait while busy

      // pending Loader Jobs
      private ConcurrentQueue<LoaderJobWrapper> _jobQueue;
      // return loaded tiles here for the Workflow
      private TileWorkflowCat _tileWorkflowCatalog;
      // Abort flag
      private bool _continueProcess = false;
      // Worker threads
      private Thread[] _threadTasks = new Thread[NUM_OF_THREAD_TILE];

      private int _jobNumberLimit = 0;

      /// <summary>
      /// Update the JobNumber Limit to a new value
      /// </summary>
      /// <param name="limit"></param>
      public void UpdateJobNumberLimit( int limit ) => _jobNumberLimit = limit;

      /// <summary>
      /// cTor: Start the Service with some args
      /// </summary>
      /// <param name="jobQueue">The input Job Queue</param>
      /// <param name="tileWorkflowCatalog">The output Image Bag</param>
      public TileLoaderService( ConcurrentQueue<LoaderJobWrapper> jobQueue, TileWorkflowCat tileWorkflowCatalog )
      {
        _jobQueue = jobQueue;
        _tileWorkflowCatalog = tileWorkflowCatalog;
      }

      /// <summary>
      /// Cleanup the WorkflowCat from obsolete items
      /// </summary>
      public void WorkflowCleanup( )
      {
        // nothing to be done anymore for now
        if (_jobQueue.IsEmpty) {
          var wfKey = _tileWorkflowCatalog.Keys.FirstOrDefault( );
          while (!string.IsNullOrEmpty( wfKey )) {
            try {
              // in case someone removed it just...
              if (_tileWorkflowCatalog[wfKey].Job.IsCancelled) {
                if (_tileWorkflowCatalog.TryRemove( wfKey, out TileWorkflow workflow )) {
                  workflow.MapImage?.Dispose( );
                }
              }
            }
            catch { }
            // next
            wfKey = _tileWorkflowCatalog.Keys.FirstOrDefault( );
          }
        }
      }

      /// <summary>
      /// Start the Worker threads
      /// </summary>
      public void StartThreads( )
      {
        _continueProcess = true;
        _jobNumberLimit = 0;

        // Start threads to handle Loader Task
        for (int i = 0; i < _threadTasks.Length; i++) {
          _threadTasks[i] = new Thread( this.TileLoader_Process ) {
            IsBackground = true // set to background so it will not prevent the App from shutting down
          };
          _threadTasks[i].Start( i );
        }
      }

      /// <summary>
      /// Thread Loop - Gets a job from the pool and handles it
      /// </summary>
      private void TileLoader_Process( object taskID_int )
      {
        int taskID = (int)taskID_int;
        int timeout_ms = c_TimeoutIdle;
        bool success = false;
        LoaderJobWrapper job = null;

        // catch Stop signal here
        while (_continueProcess) {
          // do the processing after wait - we've seen that after TryAdd - the value in the List was not available for processing straight away ??
          // reset processing
          job = null;
          success = false;

          if (_jobQueue.TryDequeue( out job )) {
            // Debug.WriteLine( $"{DateTime.Now.Ticks} TileLoaderService.TileLoader_Process[{taskID}]: Job started ({job.MapImageID.FullKey}) - {job.TileLoaderJob.MapImageID}" );

            if (job.TileLoaderJob.JobNumber > _jobNumberLimit) {
              // only if not discarded by JobNumber

              _threadTasks[taskID].IsBackground = false;
              job.TileLoaderJob.StartExec( ); // signal job started execution

              // get the first source to start the retrieval process
              //    GetTileImage()  dives down in the source list until served or end of list
              var service = job.GetNextSource( );
              if (service != null) {
                // synchronous ..
                MapImage img = service.GetTileImage( job );
                if (img != null) {
                  // Push the Image into the Workflow
                  success = _tileWorkflowCatalog.TryAdd( job.MapImageID.FullKey, new TileWorkflow( job.TileLoaderJob, img ) );
                  //   Debug.WriteLine( $"{DateTime.Now.Ticks} TileLoaderService.TileLoader_Process[{taskID}]: Add image ({job.MapImageID.FullKey}) Success? {success}" );
                  if (success) {
                    // MUST be visible in the Catalog in order to further process the image
                    while (!_tileWorkflowCatalog.ContainsKey( job.MapImageID.FullKey )) {
                      Thread.SpinWait( 10 );
                      //         Debug.WriteLine( $"{DateTime.Now.Ticks} TileLoaderService.TileLoader_Process[{taskID}]: SpinWaited ..." );
                    }
                  }
                }
                else {
                  //       Debug.WriteLine( $"{DateTime.Now.Ticks} TileLoaderService.TileLoader_Process[{taskID}]: Error - image could not be retrieved ({job.MapImageID.ZxyKey})" );
                }
              }
              // process the job in the next loop
              // let the job handle the outcome
              job.TileLoaderJob.ProcessTile( success, taskID );

            }
            else {
              // DEBUG outdated job
              ;
            }
            // pace as Busy
            timeout_ms = c_TimeoutBusy;
          }
          else {
            // make sure it is not some garbage value from TryDequeue..
            job = null;

            // no further job right now - slow down
            timeout_ms = c_TimeoutIdle;
            _threadTasks[taskID].IsBackground = true;
          }
          // wait a while
          Thread.Sleep( timeout_ms );
        }
      }

      /// <summary>
      /// Shut the Service, its processes and the jobs pending
      /// </summary>
      public void Stop( )
      {
        if (!_continueProcess) return; // alreday called

        _continueProcess = false;

        // Shut server threads - clients go back in the pool
        for (int i = 0; i < _threadTasks.Length; i++) {
          if (_threadTasks[i] != null && _threadTasks[i].IsAlive) {
            _threadTasks[i].Join( ); // waits for the threads to terminate
          }
        }
      }

    }

    #endregion

    #region Class DiskCacheService

    /// <summary>
    /// Creates a number of services that will handle the jobs waiting in the queue
    /// </summary>
    private class DiskCacheService
    {
      private const int c_TimeoutIdle = 200; // task wait while in idle
      private const int c_TimeoutBusy = 5;   // task wait while busy

      // pending Loader Jobs
      private ConcurrentQueue<DiskCacheJob> _jobQueue;
      // Abort flag
      private bool _continueProcess = false;
      // Worker threads
      private Thread[] _threadTasks = new Thread[NUM_OF_THREAD_CACHE];

      /// <summary>
      /// cTor: Start the Service with some args
      /// </summary>
      /// <param name="jobQueue">The input Job Queue</param>
      public DiskCacheService( ConcurrentQueue<DiskCacheJob> jobQueue )
      {
        _jobQueue = jobQueue;
      }

      /// <summary>
      /// Start the Worker threads
      /// </summary>
      public void StartThreads( )
      {
        _continueProcess = true;
        // Start threads to handle Loader Task
        for (int i = 0; i < _threadTasks.Length; i++) {
          _threadTasks[i] = new Thread( this.DiskCache_Process ) {
            IsBackground = true // set to background so it will not prevent the App from shutting down
          };
          _threadTasks[i].Start( i );
        }
      }

      /// <summary>
      /// Thread Loop - Gets a job from the pool and handles it
      /// </summary>
      private void DiskCache_Process( object taskID_int )
      {
        int taskID = (int)taskID_int;
        int timeout_ms = c_TimeoutIdle;

        // catch Stop signal here
        while (_continueProcess) {

          if (_jobQueue.TryDequeue( out DiskCacheJob job )) {
            // this shall never fail (e.g. data is becoming unavilable, img was disposed...)
            _threadTasks[taskID].IsBackground = false;
            try {
              if (!job.CacheProvider.PutImageToCache( job.Data, job.MapImageID.MapProvider, job.MapImageID.TileXY, job.MapImageID.ZoomLevel )) {
                LOG.Error( "DiskCacheService.DiskCache_Process", $"Could not Cache ({job.MapImageID.ZxyKey})" );
              }
            }
            catch (Exception ex) {
              LOG.Error( "DiskCacheService.DiskCache_Process", ex, $"Exception in chache Job ({job.MapImageID.ZxyKey})" );
            }
            // pace as Busy
            timeout_ms = c_TimeoutBusy;
          }
          else {
            // no further job right now - slow down
            timeout_ms = c_TimeoutIdle;
            _threadTasks[taskID].IsBackground = true;
          }
          // wait a while
          Thread.Sleep( timeout_ms );
        }
      }

      /// <summary>
      /// Shut the Service, its processes and the jobs pending
      /// </summary>
      public void Stop( )
      {
        if (!_continueProcess) return; // alreday called

        _continueProcess = false;

        // Shut server threads - clients go back in the pool
        for (int i = 0; i < _threadTasks.Length; i++) {
          if (_threadTasks[i] != null && _threadTasks[i].IsAlive) {
            _threadTasks[i].Join( ); // waits for the threads to terminate
          }
        }
      }

    }

    #endregion
  }
}

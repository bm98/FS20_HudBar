using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MapLib.Service;

namespace MapLib.Sources.DiskCache
{
  /// <summary>
  /// A Disk Cache Provider
  /// </summary>
  internal class DiskSource : IImgSource
  {
    // max #tiles to retain in cache (about 25kB per Tile - depends on the source image format)
    private const int c_maxRecNumber = 64_000 / 25; // ~64MB -> 2560 tiles
    // #days to retain cached records
    private const int c_retentionDays = 100;

    private LiteDBCache _cache = new LiteDBCache( );

    #region IImgSource interface implementation

    /// <summary>
    /// Whether or not the Source is Enabled
    /// </summary>
    public bool ProviderEnabled { get; set; } = true;

    /// <summary>
    /// Try get the image from the local source or propagate
    ///  -- Note: This is a synchronous call to a DiskCache and will eventually return or timeout
    /// </summary>
    /// <param name="jobWrapper">A JobWrapper</param>
    /// <returns>A MapImage or null</returns>
    public MapImage GetTileImage( LoaderJobWrapper jobWrapper )
    {
      var imageSought = jobWrapper.MapImageID;
      MapImage mapImage = null;
      if (ProviderEnabled) {
        mapImage = _cache.GetImageFromCache( imageSought.MapProvider, imageSought.TileXY, imageSought.ZoomLevel );
      }
      if (mapImage != null) {
//        Debug.WriteLine( $"DiskSource.GetTileImage: Served from DISK CACHE - {imageSought.FullKey}" );
        return mapImage;
      }
      else {
        // let the next do the job and follow up if an image is delivered
        var service = jobWrapper.GetNextSource( );
        mapImage = service?.GetTileImage( jobWrapper );
        if (mapImage != null && !mapImage.IsFailedImage) {
          // handle a returned image from the previous sources
          DiskCacheJob cacheJob = new DiskCacheJob( mapImage.MapImageID, _cache, mapImage.DataStream.GetBuffer( ) );
          RequestScheduler.Instance.Add_DiskCacheJob( cacheJob );
        }
        return mapImage;
      }
    }

    #endregion

    /// <summary>
    /// Get: the disk cache location (folder path)
    /// </summary>
    public string DiskCacheLocation => _cache.CacheLocation;

    /// <summary>
    /// Set the folder path for the DiskCache
    /// default is the App Dir
    /// </summary>
    /// <param name="folderPath">A folder path</param>
    public void SetDiskCacheLocation( string folderPath )
    {
      _cache.CacheLocation = folderPath;
    }

    /// <summary>
    /// Method to prevent the cache from overrun
    /// </summary>
    public void MaintainCacheSize( )
    {
      DateTime oldDate = DateTime.Now - new TimeSpan( c_retentionDays, 0, 0, 0 );
      Debug.WriteLine( $"DiskSource.MaintainCacheSize: started, {c_retentionDays} days retention " );

      foreach (MapProvider provider in Enum.GetValues( typeof( MapProvider ) )) {
        int deletedRecords = _cache.DeleteByMaxRecNumber( c_maxRecNumber, provider );
        Debug.WriteLine( $"DiskSource.MaintainCacheSize: deleted {deletedRecords} records from {provider}" );
      }
      // cleanup old stuff if there is still any..
      foreach (MapProvider provider in Enum.GetValues( typeof( MapProvider ) )) {
        int deletedRecords = _cache.DeleteOlderThan( oldDate, provider );
        Debug.WriteLine( $"DiskSource.MaintainCacheSize: deleted {deletedRecords} records from {provider}" );
      }

    }

  }
}


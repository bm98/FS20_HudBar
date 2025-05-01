using System;

namespace MapLib.Sources.MemCache
{
  /// <summary>
  /// Caches One MapImage
  /// </summary>
  internal class MemCacheItem : ICacheItem // implies IDisposable
  {
    public string Key { get; set; } = "";
    public MapImage MapImage { get; set; } = null;
    public DateTime TimeStamp { get; set; } = new DateTime( 0 );
    public MapProvider MapProvider { get; set; } = MapProvider.DummyProvider;

    public MemCacheItem( MapImage mapImage )
    {
      if (mapImage != null) {
        Key = mapImage.MapImageID.FullKey;
        MapImage = mapImage.Clone( ); // we hold a copy in case the owner Disposes the argument
        TimeStamp = DateTime.Now;
        MapProvider = Tools.MapProviderFromKey( Key );
      }
    }

    #region DISPOSE

    private bool disposedValue;

    protected virtual void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
          MapImage?.Dispose( );
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~MemCacheItem()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose( )
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose( disposing: true );
      GC.SuppressFinalize( this );
    }

    #endregion
  }
}

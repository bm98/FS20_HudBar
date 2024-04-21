using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MapLib.Service;

namespace MapLib.Sources.MemCache
{
  /// <summary>
  /// A Memory Cache Provider
  /// </summary>
  internal class MemorySource : IImgSource
  {
    // instance of our cache
    //    private MemCacheItemCat _memCache = new MemCacheItemCat( );
    private CacheV2 _memCache = new CacheV2( );


    #region IImgSource interface implementation

    /// <summary>
    /// Whether or not the Source is Enabled
    /// </summary>
    public bool ProviderEnabled { get; set; } = true;

    /// <summary>
    /// Try get the image from the local source or propagate
    ///  -- Note: This is a synchronous call to a MemoryCache and will eventually return or timeout
    /// </summary>
    /// <param name="jobWrapper">A JobWrapper</param>
    /// <returns>A MapImage or null</returns>
    public MapImage GetTileImage( LoaderJobWrapper jobWrapper )
    {
      var imageSought = jobWrapper.MapImageID;
      MapImage mapImage = null;
      if (ProviderEnabled) {
        if (_memCache.TryGetValue( jobWrapper.MapImageID.FullKey, out ICacheItem item )) {
          var mci = item as MemCacheItem;
          // need a copy here
          mapImage = MapImage.FromArray( mci.MapImage.DataStream.GetBuffer( ), mci.MapImage.MapImageID );
        }
        // MemCacheItemCat Version below
        //  mapImage = _memCache.Retrieve( jobWrapper.MapImageID.FullKey );
      }
      if (mapImage != null) {
        //        Debug.WriteLine( $"MemorySource.GetTileImage: Served from MEMORY CACHE - {imageSought.FullKey}" );
        mapImage.ImageSource = ImgSource.MemCache;
        return mapImage;
      }
      else {
        // let the next do the job and follow up if an image is delivered
        var service = jobWrapper.GetNextSource( );
        mapImage = service?.GetTileImage( jobWrapper );
        if (mapImage != null && mapImage.IsValid && !mapImage.IsFailedImage) {
          // put into this cache or leave it alone
          var mItem = new MemCacheItem( mapImage );
          _memCache.TryAdd( mItem.Key, mItem ); // don't care about the return value here
          MaintainCacheSize( );
          // MemCacheItemCat Version below
          // _memCache.Add( mapImage );
        }
        return mapImage;
      }
    }

    #endregion

    /// <summary>
    /// Method to prevent the cache from overrun
    /// </summary>
    public void MaintainCacheSize( )
    {
      _memCache.MaintainCacheSize( );
    }

  }
}

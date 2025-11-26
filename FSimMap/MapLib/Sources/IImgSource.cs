using MapLib.Service;
using System.Threading.Tasks;

namespace MapLib.Sources
{
  /// <summary>
  /// Any Image source must implement this method
  /// </summary>
  internal interface IImgSource
  {
    /// <summary>
    /// Whether or not the Source is Enabled
    /// </summary>
   bool ProviderEnabled { get; set; }

    /// <summary>
    /// Sources a MapImage
    /// </summary>
    /// <param name="jobWrapper">The JobWrapper</param>
    /// <returns>A MapImage or null</returns>
    MapImage GetTileImage( LoaderJobWrapper jobWrapper );

    /// <summary>
    /// Sources a MapImage
    /// </summary>
    /// <param name="jobWrapper">The JobWrapper</param>
    /// <returns>A MapImage or null</returns>
    Task<MapImage> GetTileImage_Asynch( LoaderJobWrapper jobWrapper );

    /// <summary>
    /// Method to prevent the cache from overrun
    /// </summary>
    void MaintainCacheSize( );
  }
}

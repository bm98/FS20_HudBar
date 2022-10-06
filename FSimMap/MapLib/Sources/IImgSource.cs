using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MapLib.Service;

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
    /// Method to prevent the cache from overrun
    /// </summary>
    void MaintainCacheSize( );
  }
}

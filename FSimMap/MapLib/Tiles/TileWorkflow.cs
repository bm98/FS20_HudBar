using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Tiles
{
  internal class TileWorkflow
  {
    /// <summary>
    /// The Job requesting an image
    /// </summary>
    public TileLoaderJob Job { get; protected set; }
    /// <summary>
    /// The Image attached
    /// </summary>
    public MapImage MapImage {get; protected set;}

    /// <summary>
    /// cTor:
    /// </summary>
    public TileWorkflow(TileLoaderJob job, MapImage mapImage )
    {
      Job = job;
      MapImage = mapImage;
    }

  }
}

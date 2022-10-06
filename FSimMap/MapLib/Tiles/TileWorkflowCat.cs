using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Tiles
{
  /// <summary>
  /// Catalog where the received images are pushed
  /// for further processing
  /// 
  /// Thread aware
  /// </summary>
  internal class TileWorkflowCat : ConcurrentDictionary<string, MapImage>
  {
  }
}

using System.Collections.Concurrent;

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

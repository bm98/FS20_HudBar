using System.Collections.Concurrent;

namespace MapLib.Tiles
{
  /// <summary>
  /// Type of Catalog to track tiles while loading
  /// </summary>
  internal class TrackingCat : ConcurrentDictionary<string, int>
  {
  }
}

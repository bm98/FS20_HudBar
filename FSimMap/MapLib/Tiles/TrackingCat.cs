using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Tiles
{
  /// <summary>
  /// Type of Catalog to track tiles while loading
  /// </summary>
  internal class TrackingCat : ConcurrentDictionary<string, int>
  {
  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Sources.MemCache
{
  /// <summary>
  /// Minimum Properties a CacheItem must provide
  /// IDisposable is added to support such items which are Disposed in Cleanup procedures
  /// 
  /// </summary>
  internal interface ICacheItem : IDisposable
  {
    /// <summary>
    /// A timestamp will be set by the cache when the item is cached
    /// and allows to Clean the Cache with older than qualifier
    /// </summary>
    DateTime TimeStamp { get; set; }
  }
}

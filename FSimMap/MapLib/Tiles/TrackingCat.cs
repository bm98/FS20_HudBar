using System.Collections.Generic;
using System.Collections.Concurrent;

namespace MapLib.Tiles
{
  /// <summary>
  /// Type of Catalog to track tiles while loading
  ///  Key:  TrackKey    of (MapImageID, _version)
  ///  Value: JobNumber
  /// </summary>
  internal class TrackingCat : ConcurrentDictionary<string, int>
  {

    /// <summary>
    /// Remove entries upto and including jobNumberLimit
    /// </summary>
    /// <param name="jobNumberLimit">The limit number to remove</param>
    public void RemoveObsoleteJobs( int jobNumberLimit )
    {
      List<string> removeKeys = new List<string>( );
      foreach (var kv in this) {
        if (kv.Value > jobNumberLimit) continue;
        removeKeys.Add( kv.Key );
      }

      foreach (var k in removeKeys) {
        this.TryRemove( k, out int _ );
      }
    }

  }
}

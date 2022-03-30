using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using DbgLib;

namespace FS20_HudBar
{
  /// <summary>
  /// Debug Start Check
  /// </summary>
  internal static class DebugStart
  {
    /// <summary>
    /// Checks if a Debug Start File exists, and enables Debug Out if so
    /// </summary>
    public static void CheckForDebugStart( )
    {
      if ( File.Exists(Path.Combine( ".","HB_DEBUG.txt" ) )) {
        Dbg.Instance.EnableDebug( );
        Dbg.Instance.DumpAudioProps( ); // need to know the Audio props for HudBar
      }

    }
  }
}

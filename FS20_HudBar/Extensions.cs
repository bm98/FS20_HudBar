using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar
{
  class Extensions
  {
  }

  /// <summary>
  /// Clears a Control Collection and disposes the items when asked for
  /// </summary>
  public static class ExtensionMethods
  {

    /// <summary>
    /// Clear a Control Collection with optional Dispose
    /// </summary>
    /// <param name="controls">The ControlCollection</param>
    /// <param name="dispose">Set true to call dispose for each item</param>
    public static void Clear( this Control.ControlCollection controls, bool dispose )
    {
      for ( int ix = controls.Count - 1; ix >= 0; --ix ) {
        var tmpObj = controls[ix];
        controls.RemoveAt( ix );
        if ( dispose && !tmpObj.IsDisposed ) tmpObj.Dispose( );
      }
    }

  }

}

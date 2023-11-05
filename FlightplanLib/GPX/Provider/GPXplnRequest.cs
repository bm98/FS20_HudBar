using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.GPX.Provider
{
  /// <summary>
  /// Form and issue a FlightPlan Request 
  /// 
  /// </summary>
  internal class GPXplnRequest
  {
    public static string ResponseRaw { get; set; } = "";

    /// <summary>
    /// Async Retrieve a GPX XML record
    /// </summary>
    /// <param name="fileName">The fully qualified filename</param>
    /// <returns>An XML Document</returns>
    public static async Task<string> GetDocument( string fileName )
    {
      //GET
      try {
        using (var sr = new StreamReader( fileName )) {
          ResponseRaw = await sr.ReadToEndAsync( );
        }
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch (Exception ex) {
#pragma warning restore CS0168 // Variable is declared but never used
        ResponseRaw = "";
      }

      return ResponseRaw;
    }
  }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.MSFSFlt.Provider
{
  /// <summary>
  /// Form and issue a FlightPlan Request 
  /// 
  /// </summary>
  internal class FltRequest
  {
    public static string ResponseRaw { get; set; } = "";

    /// <summary>
    /// Async Retrieve a PLN XML record
    /// </summary>
    /// <param name="fileName">The fully qualified filename</param>
    /// <returns>An XML Document</returns>
    public static async Task<string> GetDocument( string fileName )
    {
      //GET
      byte[] byt;
      try {
        using (var ts = File.Open( fileName, FileMode.Open, FileAccess.Read, FileShare.Read )) {
          if (ts.Length < 10) {
            // bail out on very short files
            ResponseRaw = "";
          } 
          else {
            byt = new byte[ts.Length];
            await ts.ReadAsync( byt, 0, byt.Length );
            // check if UTF8 (with BOM) or not (we save our debug file with BOM to load it here again)
            bool utf8 = true;
            var bom = Encoding.UTF8.GetPreamble( );
            for (int b = 0; b < bom.Length; b++) { utf8 &= byt[b] == bom[b]; } // check BOM bytes
            if (utf8) {
              ResponseRaw = Encoding.UTF8.GetString( byt ); // it is a save file and already UTF8, leave it alone
            }
            else {
              var encoder = Encoding.GetEncoding( "iso-8859-1" ); // FLT INI File have this encoding, convert to UTF8
              ResponseRaw = Encoding.UTF8.GetString( Encoding.Convert( encoder, Encoding.UTF8, byt ) );
            }
          }
        }
      }
#pragma warning disable CS0168 // Variable is declared but never used
      catch (Exception ex) {
#pragma warning restore CS0168 // Variable is declared but never used
        ResponseRaw = "";
      }
      finally {
        byt = new byte[0]; // help the GC
      }

      return ResponseRaw;
    }
  }
}

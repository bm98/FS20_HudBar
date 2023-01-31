using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.MSFSFlt.FLTDEC
{
  /// <summary>
  /// A MSFS FLT Document Decoder
  /// </summary>
  public class FltDecoder
  {
    /// <summary>
    /// Decode a JSON Flight Plan string 
    /// returns a filled OFP or an empty one if something failed
    /// </summary>
    /// <param name="jsonString">The flight plan as JSON string</param>
    /// <returns>An OFP</returns>
    public static FLT FromString( string jsonString )
    {
      var ofp = Formatter.FromIniString<FLT>( jsonString );
      if (ofp != null) {
        return ofp;
      }
      return new FLT( );
    }

    /// <summary>
    /// Decode a JSON Flight Plan from a File 
    /// returns a filled OFP or an empty one if something failed
    /// </summary>
    /// <param name="jsonFile">The flight plan as JSON filename</param>
    /// <returns>An OFP</returns>
    public static FLT FromFile( string jsonFile )
    {
      var ofp = Formatter.FromIniFile<FLT>( jsonFile );
      if (ofp != null) {
        return ofp;
      }
      return new FLT( );
    }

  }
}

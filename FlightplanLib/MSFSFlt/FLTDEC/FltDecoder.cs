using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dNetBm98.IniLib;

namespace FlightplanLib.MSFSFlt.FLTDEC
{
  /// <summary>
  /// A MSFS FLT Document Decoder
  /// </summary>
  public class FltDecoder
  {
    /// <summary>
    /// Decode a FLT Ini Flight Plan string 
    /// returns a filled OFP or an empty one if something failed
    /// </summary>
    /// <param name="fltIniString">The flight plan as Flt Ini string</param>
    /// <returns>An OFP</returns>
    public static FLT FromString( string fltIniString )
    {
      var ofp = IniSerializer.FromIniString<FLT>( fltIniString );
      if (ofp != null) {
        return ofp;
      }
      return new FLT( );
    }

    /// <summary>
    /// Decode a FLT Ini Flight Plan from a File 
    /// returns a filled OFP or an empty one if something failed
    /// </summary>
    /// <param name="fltIniFile">The flight plan as FLT Ini filename</param>
    /// <returns>An OFP</returns>
    public static FLT FromFile( string fltIniFile )
    {
      // MSFS FLT encoding is iso-8859-1 , don't unquote while decoding
      var ofp = IniSerializer.FromIniFile<FLT>( fltIniFile, false, MSiniFile.IniEncoding.iso_8859_1 );
      if (ofp != null) {
        return ofp;
      }
      return new FLT( );
    }

  }
}

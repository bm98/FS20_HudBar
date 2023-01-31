using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.MSFSPln.PLNDEC
{
  /// <summary>
  /// A MSFS PLN Document Decoder
  /// </summary>
  public static class PlnDecoder
  {
    /// <summary>
    /// Decode a JSON Flight Plan string 
    /// returns a filled OFP or an empty one if something failed
    /// </summary>
    /// <param name="jsonString">The flight plan as JSON string</param>
    /// <returns>An OFP</returns>
    public static PLN FromString( string jsonString )
    {
      var ofp = Formatter.FromXmlString<PLN>( jsonString );
      if (ofp != null) {
        return ofp;
      }
      return new PLN( );
    }

    /// <summary>
    /// Decode a JSON Flight Plan from a File 
    /// returns a filled OFP or an empty one if something failed
    /// </summary>
    /// <param name="jsonFile">The flight plan as JSON filename</param>
    /// <returns>An OFP</returns>
    public static PLN FromFile( string jsonFile )
    {
      var ofp = Formatter.FromXmlFile<PLN>( jsonFile );
      if (ofp != null) {
        return ofp;
      }
      return new PLN( );
    }

  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.MSFSPln.PLNDEC
{
  /// <summary>
  /// A LNM GPX Document Decoder
  /// </summary>
  public static class PlnDecoder
  {
    /// <summary>
    /// Decode a XML Flight Plan string 
    /// returns a filled OFP or an empty one if something failed
    /// </summary>
    /// <param name="xmlString">The flight plan as XML string</param>
    /// <returns>An OFP</returns>
    public static PLN FromString( string xmlString )
    {
      var ofp = Formatter.FromXmlString<PLN>( xmlString );
      if (ofp != null) {
        return ofp;
      }
      return new PLN( );
    }

    /// <summary>
    /// Decode a XML Flight Plan from a File 
    /// returns a filled OFP or an empty one if something failed
    /// </summary>
    /// <param name="jsonFile">The flight plan as XML filename</param>
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

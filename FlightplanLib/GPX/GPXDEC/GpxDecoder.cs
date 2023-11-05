using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.GPX.GPXDEC
{
  /// <summary>
  /// A MSFS GPX Document Decoder
  /// </summary>
  public static class GpxDecoder
  {
    /// <summary>
    /// Decode a GPX Flight Plan string 
    /// returns a filled OFP or an empty one if something failed
    /// </summary>
    /// <param name="jsonString">The flight plan as GPX string</param>
    /// <returns>An OFP</returns>
    public static GPX FromString( string jsonString )
    {
      var ofp = Formatter.FromXmlString<GPX>( jsonString );
      if (ofp != null) {
        return ofp;
      }
      return new GPX( );
    }

    /// <summary>
    /// Decode a GPX Flight Plan from a File 
    /// returns a filled OFP or an empty one if something failed
    /// </summary>
    /// <param name="jsonFile">The flight plan as GPX filename</param>
    /// <returns>An OFP</returns>
    public static GPX FromFile( string jsonFile )
    {
      var ofp = Formatter.FromXmlFile<GPX>( jsonFile );
      if (ofp != null) {
        return ofp;
      }
      return new GPX( );
    }

  }
}

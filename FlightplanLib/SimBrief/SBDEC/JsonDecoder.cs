using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.SimBrief.SBDEC
{
  /// <summary>
  /// A SimBrief JSON Document Decoder
  /// </summary>
  public static class JsonDecoder
  {
    /// <summary>
    /// Decode a JSON Flight Plan string 
    /// returns a filled OFP or an empty one if something failed
    /// </summary>
    /// <param name="jsonString">The flight plan as JSON string</param>
    /// <returns>An OFP</returns>
    public static OFP FromString( string jsonString )
    {
      var ofp = Formatter.FromJsonString<OFP>( jsonString );
      if (ofp != null) {
        return ofp;
      }
      return new OFP( );
    }

    /// <summary>
    /// Decode a JSON Flight Plan from a File 
    /// returns a filled OFP or an empty one if something failed
    /// </summary>
    /// <param name="jsonFile">The flight plan as JSON filename</param>
    /// <returns>An OFP</returns>
    public static OFP FromFile( string jsonFile )
    {
      var ofp = Formatter.FromJsonFile<OFP>( jsonFile );
      if (ofp != null) {
        return ofp;
      }
      return new OFP( );
    }

  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using FlightplanLib.Routes;

namespace FlightplanLib.LNM.LNMDEC
{
  /// <summary>
  /// A LNM PLN Document Decoder
  /// </summary>
  public static class LnmPlnDecoder
  {
    /// <summary>
    /// Decode a LNM Flight Plan string 
    /// returns a filled OFP or an empty one if something failed
    /// </summary>
    /// <param name="plnString">The flight plan as LNM Plan  string</param>
    /// <returns>An OFP</returns>
    public static RouteCapture RouteFromString( string plnString )
    {
      var ofp = Formatter.FromXmlString<LNM>( plnString );
      if (ofp != null) {
        // go via Route String 
        StringBuilder err = new StringBuilder( );
        RouteDecoder decoder = new RouteDecoder( ofp.AsRouteString, err );
        return decoder.Route;
      }
      return new RouteCapture( ); // will be invalid..
    }

    /// <summary>
    /// Decode a LNM Flight Plan string 
    /// returns a filled OFP or an empty one if something failed
    /// </summary>
    /// <param name="plnString">The flight plan as LNM Plan  string</param>
    /// <returns>An OFP</returns>
    public static LNM FromString( string plnString )
    {
      var ofp = Formatter.FromXmlString<LNM>( plnString );
      if (ofp != null) {
        return ofp;
      }
      return new LNM( ); // will be invalid..
    }

  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

using FlightplanLib.Routes;

namespace FlightplanLib.RTE.RTEDEC
{
  /// <summary>
  /// A LNM GPX Document Decoder
  /// </summary>
  public static class PlnDecoder
  {
    /// <summary>
    /// Decode a Route Flight Plan string 
    /// returns a filled OFP or an empty one if something failed
    /// </summary>
    /// <param name="rteString">The flight plan as Route string</param>
    /// <returns>An OFP</returns>
    public static RouteCapture FromString( string rteString )
    {
      StringBuilder err = new StringBuilder( );
      RouteDecoder decoder = new RouteDecoder( rteString, err );
      return decoder.Route;
    }

  }
}

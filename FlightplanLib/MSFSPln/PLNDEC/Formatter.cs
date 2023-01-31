using CoordLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace FlightplanLib.MSFSPln.PLNDEC
{
  /// <summary>
  /// Generic De-Serializer helpers
  /// </summary>
  internal class Formatter
  {
    #region STATIC TOOLS

    /// <summary>
    /// Returns the value in the string or NaN
    /// </summary>
    /// <param name="valueS">A value String</param>
    /// <returns>The value or NaN</returns>
    internal static double GetValue( string valueS )
    {
      if (double.TryParse( valueS, out double result )) return result;
      return double.NaN;
    }

    /// <summary>
    /// Convert a WorldPosition (LLA) to a LatLon
    /// </summary>
    /// <param name="worldPosition">An LLA string</param>
    /// <returns>A LatLon</returns>
    internal static LatLon ToLatLon( string worldPosition )
    {
      LatLon ll = LatLon.Empty;
      if (LLA.TryParseLLA( worldPosition, out var lat, out var lon, out var alt )) {
        ll = new LatLon( lat, lon, alt );
      }
      else {
        Console.WriteLine( $"X_ATCWaypoint ERROR ToLatLon failed - pattern does not match\n{worldPosition}" );
      }
      return ll;
    }

    /// <summary>
    /// The B21 Soaring engine uses tagged names for the Task Management
    ///  [*]Name+Elev[|MaxAlt[/MinAlt]][xRadius]
    ///  1st * - Start of Task
    ///  2nd * - End of Task
    ///  Name  - the WP name
    ///  +     - Separator
    ///  Elev  - Waypoint Elevation  [ft}
    ///  |MaxAlt - Max alt of the gate [ft}
    ///  /MinAlt - Min alt of the gate [ft}
    ///  xRadius - Radius of the gate [meters]
    /// </summary>
    /// <param name="b21name">Possibly a B21 Task Waypoint name</param>
    /// <returns>A string</returns>
    internal static string CleanB21SoaringName( string b21name )
    {
      Match match = c_wpB21.Match( b21name );
      if (match.Success) {
        if (match.Groups["name"].Success) {
          return match.Groups["name"].Value;
        }
      }
      // seems a regular name
      return b21name;
    }
    private readonly static Regex c_wpB21 =
      new Regex( @"^(?<start_end>\*)?(?<name>([^\+])*)(?<elevation>(\+|-)\d{1,5})(?<maxAlt>\|\d{1,5})?(?<minAlt>\/\d{1,5})?(?<radius>x\d{1,5})?" );

    /// <summary>
    /// Get the decorations from the Waypoint ID
    /// </summary>
    /// <param name="b21name">Possibly a B21 Task Waypoint name</param>
    /// <returns>A string</returns>
    internal static string GetB21SoaringDecoration( string b21name )
    {
      // decoration see above
      Match match = c_wpB21.Match( b21name );
      if (match.Success) {
        var deco = "";
        if (match.Groups["start_end"].Success) { deco += "* "; }
        if (match.Groups["elevation"].Success) { deco += $"{match.Groups["elevation"].Value} ft "; }
        if (match.Groups["minAlt"].Success) { deco += $"(min {match.Groups["minAlt"].Value}) "; }
        if (match.Groups["maxAlt"].Success) { deco += $"(max {match.Groups["maxAlt"].Value}) "; }
        if (match.Groups["radius"].Success) { deco += $"(rad {match.Groups["radius"].Value} m)"; }
        return deco;
      }
      // seems a regular name
      return "";
    }

    #endregion


#pragma warning disable CS0168 // Variable is declared but never used

    /// <summary>
    /// Reads from the open stream one entry
    /// </summary>
    /// <param name="xStream">An open stream at position</param>
    /// <returns>A Controller obj or null for errors</returns>
    public static T FromXmlStream<T>( Stream xStream )
    {
      try {
        var xmlSerializer = new XmlSerializer( typeof( T ) );
        xmlSerializer.UnknownNode += XmlSerializer_UnknownNode;
        xmlSerializer.UnknownAttribute += XmlSerializer_UnknownAttribute;
        xmlSerializer.UnknownElement += XmlSerializer_UnknownElement;
        object objResponse = xmlSerializer.Deserialize( xStream );
        var xmlResults = (T)objResponse;
        xStream.Flush( );
        return xmlResults;
      }
      catch (Exception e) {
        return default( T );
      }
    }

    private static void XmlSerializer_UnknownElement( object sender, XmlElementEventArgs e )
    {
      Console.WriteLine( $"Unknown Element at {e.LineNumber}: {e.Element.Name}" );
    }

    private static void XmlSerializer_UnknownAttribute( object sender, XmlAttributeEventArgs e )
    {
      Console.WriteLine( $"Unknown Attribute at {e.LineNumber}: {e.Attr.Name}" );
    }

    private static void XmlSerializer_UnknownNode( object sender, XmlNodeEventArgs e )
    {
      Console.WriteLine( $"Unknown Node at {e.LineNumber}: {e.Name}" );
    }

    /// <summary>
    /// Reads from the supplied string
    /// </summary>
    /// <param name="jString">A Xml formatted string</param>
    /// <returns>A Controller obj or null for errors</returns>
    public static T FromXmlString<T>( string jString )
    {
      try {
        T xmlResults = default;
        using (var ms = new MemoryStream( Encoding.UTF8.GetBytes( jString ) )) {
          xmlResults = FromXmlStream<T>( ms );
        }
        return xmlResults;
      }
      catch (Exception e) {
        return default;
      }
    }


    /// <summary>
    /// Reads from a file one entry
    /// Tries to aquire a shared Read Access and blocks for max 100ms while doing so
    /// </summary>
    /// <param name="xFilename">The Xml Filename</param>
    /// <returns>A Controller obj or null for errors</returns>
    public static T FromXmlFile<T>( string xFilename )
    {
      T retVal = default( T );
      if (!File.Exists( xFilename )) {
        return retVal;
      }

      int retries = 10; // 100ms worst case
      while (retries-- > 0) {
        try {
          using (var ts = File.Open( xFilename, FileMode.Open, FileAccess.Read, FileShare.Read )) {
            retVal = FromXmlStream<T>( ts );
          }
          return retVal;
        }
        catch (IOException ioex) {
          // retry after a short wait
          Thread.Sleep( 10 ); // allow the others fileIO to be completed
        }
        catch (Exception ex) {
          // not an IO exception - just fail
          return retVal;
        }
      }

      return retVal;
    }

    /// <summary>
    /// Write to the open stream one entry
    /// </summary>
    /// <param name="data">A datafile object to write</param>
    /// <param name="xStream">An open stream at position</param>
    /// <returns>True if successfull</returns>
    public static bool ToXmlStream<T>( T data, Stream xStream )
    {
      try {
        var xmlSerializer = new XmlSerializer( typeof( T ) );
        xmlSerializer.Serialize( xStream, data );
        return true;
      }
      catch (Exception e) {
        return false; // bails at data==null or formatting issues
      }
    }

    /// <summary>
    /// Write to a file one entry
    /// Tries to aquire an exclusive Write Access and blocks for max 100ms while doing so
    /// </summary>
    /// <param name="data">A datafile object to write</param>
    /// <param name="xFilename">The Xml Filename</param>
    /// <returns>True if successfull</returns>
    public static bool ToXmlFile<T>( T data, string xFilename )
    {
      bool retVal = false;

      int retries = 10; // 100ms worst case
      while (retries-- > 0) {
        try {
          using (var ts = File.Open( xFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None )) {
            retVal = ToXmlStream( data, ts );
          }
          return retVal;
        }
        catch (IOException ioex) {
          // retry after a short wait
          Thread.Sleep( 10 ); // allow the others fileIO to be completed
        }
        catch (Exception ex) {
          // not an IO exception - just fail
          return retVal;
        }
      }
      return retVal;
    }


#pragma warning restore CS0168 // Variable is declared but never used
  }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using CoordLib.MercatorTiles;

namespace MapLib
{
  /// <summary>
  /// Widely used static Methods
  /// </summary>
  internal class Tools
  {
    // Max Dimenstion for a Zoom Level (index)
    private static int[] c_maxDim = new int[24];

    static Tools( )
    {
      // precalc max Dimensions for each zoom level
      for (int i = 0; i < c_maxDim.Length; i++) c_maxDim[i] = (int)Math.Pow( 2, i );
    }

    /// <summary>
    /// Returns a MemoryStream from an InputStream
    /// </summary>
    /// <param name="inputStream">An Input Stream</param>
    /// <param name="rewindStream">Rewind the InputString before retuning</param>
    /// <returns>A MemoryStream at Pos 0</returns>
    public static MemoryStream CopyStream( Stream inputStream, bool rewindStream )
    {
      const int readSize = 32 * 1024;
      var buffer = new byte[readSize];
      var mstream = new MemoryStream( );

      // bulk copy
      if (inputStream.CanSeek) inputStream.Seek( 0, SeekOrigin.Begin );

      int count;
      while ((count = inputStream.Read( buffer, 0, readSize )) > 0) {
        mstream.Write( buffer, 0, count );
      }

      if (rewindStream) {
        if (inputStream.CanSeek) inputStream.Seek( 0, SeekOrigin.Begin );
      }

      mstream.Seek( 0, SeekOrigin.Begin );
      return mstream;
    }

    /// <summary>
    /// Get: a position index based 'randomized' to access not the same server all the time
    ///  Returns 0...(max-1)
    /// </summary>
    /// <param name="tileXY">A TileXY</param>
    /// <param name="max">Max numbers to return</param>
    /// <returns>A number</returns>
    public static int GetServerNum( TileXY tileXY, int max )
    {
      return (int)(tileXY.X + 2 * tileXY.Y) % max;
    }

    // TILE KEYs 

    /// <summary>
    /// Returns the XY Tile Key for the arguments
    /// (Xx¦Yy)
    /// </summary>
    /// <param name="x">X tile coord</param>
    /// <param name="y">Y Tile coord</param>
    /// <returns>A Key string or an empty string on error</returns>
    public static string ToXYKey( int x, int y )
    {
      if ((x < 0) || (y < 0)) return "";

      // MASTER CONVERSION
      return $"X{x}¦Y{y}";
    }
    /// <summary>
    /// Returns the XY Tile Key for the arguments
    /// (Xx¦Yy)
    /// </summary>
    /// <param name="tileXY">A TileXY</param>
    /// <returns>A Key string or an empty string on error</returns>
    public static string ToXYKey( TileXY tileXY )
    {
      return ToXYKey( tileXY.X, tileXY.Y );
    }

    /// <summary>
    /// Returns the fully qualified ZXY Key for the arguments
    /// (Zz¦Xx¦Yy)
    /// </summary>
    /// <param name="x">Tile X coord</param>
    /// <param name="y">Tile Y coord</param>
    /// <param name="zoom">A Zoom level</param>
    /// <returns>A Key string or an empty string on error</returns>
    public static string ToZxyKey( int x, int y, ushort zoom )
    {
      var q = ToXYKey( x, y );
      // MASTER CONVERSION
      return string.IsNullOrEmpty( q ) ? "" : $"Z{zoom}¦{q}";
    }
    /// <summary>
    /// Returns the fully qualified ZXY Key for the arguments
    /// (Zz¦Xx¦Yy)
    /// </summary>
    /// <param name="tileXY">A TileXY</param>
    /// <param name="zoom">A Zoom level</param>
    /// <returns>A Key string or an empty string on error</returns>
    public static string ToZxyKey( TileXY tileXY, ushort zoom )
    {
      return ToZxyKey( tileXY.X, tileXY.Y, zoom );
    }
    /// <summary>
    /// Returns the fully qualified ZXY Key for the arguments
    /// (Zz¦Xx¦Yy)
    /// </summary>
    /// <param name="mapImageID">A MapImageID</param>
    /// <returns>A Key string or an empty string on error</returns>
    public static string ToZxyKey( MapImageID mapImageID )
    {
      return ToZxyKey( mapImageID.TileXY, mapImageID.ZoomLevel );
    }

    // IMAGE ID KEYs 

    /// <summary>
    /// Returns the fully qualified Image Key for the arguments
    /// (provider¦Zz¦Xx¦Yy)
    /// </summary>
    /// <param name="mapProvider">A map provider</param>
    /// <param name="x">Tile x coord</param>
    /// <param name="y">Tile y coord</param>
    /// <param name="zoom">Zoom level</param>
    /// <returns>A Key string or an empty string on error</returns>
    public static string ToFullKey( MapProvider mapProvider, int x, int y, ushort zoom )
    {
      var q = ToZxyKey( x, y, zoom );
      // MASTER CONVERSION
      return string.IsNullOrEmpty( q ) ? "" : $"{mapProvider}¦{q}";
    }

    /// <summary>
    /// Returns the fully qualified Image Key for the arguments
    /// (provider¦Zz¦Xx¦Yy)
    /// </summary>
    /// <param name="mapProvider">A map provider</param>
    /// <param name="tileXY">Tile XY coord</param>
    /// <param name="zoom">Zoom level</param>
    /// <returns>A Key string or an empty string on error</returns>
    public static string ToFullKey( MapProvider mapProvider, TileXY tileXY, ushort zoom )
    {
      return ToFullKey( mapProvider, tileXY.X, tileXY.Y, zoom );
    }

    /// <summary>
    /// Returns the fully qualified Image Key for the arguments
    /// (provider¦Zz¦Xx¦Yy)
    /// </summary>
    /// <param name="mapImageID">A MapImageID</param>
    /// <returns>A Key string or an empty string on error</returns>
    public static string ToFullKey( MapImageID mapImageID )
    {
      return ToFullKey( mapImageID.MapProvider, mapImageID.TileXY, mapImageID.ZoomLevel );
    }

    // TRACKING KEYs 

    /// <summary>
    /// Returns the fully qualified Image Tracking Key for the arguments
    /// (provider¦Zz¦Xx¦Yy)
    /// </summary>
    /// <param name="mapProvider">A map provider</param>
    /// <param name="x">Tile x coord</param>
    /// <param name="y">Tile y coord</param>
    /// <param name="zoom">Zoom level</param>
    /// <param name="version">A Tracking Version number</param>
    /// <returns>A Key string or an empty string on error</returns>
    public static string ToTrackKey( MapProvider mapProvider, int x, int y, ushort zoom, int version )
    {
      var q = ToFullKey( mapProvider, x, y, zoom );
      // MASTER CONVERSION
      return string.IsNullOrEmpty( q ) ? "" : $"{q}¦{version}";
    }

    /// <summary>
    /// Returns the fully qualified Image Tracking Key for the arguments
    /// (provider¦Zz¦Xx¦Yy)
    /// </summary>
    /// <param name="mapProvider">A map provider</param>
    /// <param name="tileXY">Tile XY coord</param>
    /// <param name="zoom">Zoom level</param>
    /// <param name="version">A Tracking Version number</param>
    /// <returns>A Key string or an empty string on error</returns>
    public static string ToTrackKey( MapProvider mapProvider, TileXY tileXY, ushort zoom, int version )
    {
      return ToTrackKey( mapProvider, tileXY.X, tileXY.Y, zoom, version );
    }

    /// <summary>
    /// Returns the fully qualified Image Tracking Key for the arguments
    /// (provider¦Zz¦Xx¦Yy)
    /// </summary>
    /// <param name="mapImageID">A MapImageID</param>
    /// <param name="version">A Tracking Version number</param>
    /// <returns>A Key string or an empty string on error</returns>
    public static string ToTrackKey( MapImageID mapImageID, int version )
    {
      return ToTrackKey( mapImageID.MapProvider, mapImageID.TileXY, mapImageID.ZoomLevel, version );
    }

    //  "PROVIDER_NAME¦Zzz¦Xxxxxx¦Yyyyyy|n.."
    private static Regex _rxFullKey = new Regex( @"^(?<p>\w+?)¦Z(?<z>\d{1,2})¦X(?<x>\d{1,5})¦Y(?<y>\d{1,5})(¦(?<n>\d+))?", RegexOptions.Compiled );
    /// <summary>
    /// Returns a Provider String from a FullKey
    /// </summary>
    /// <param name="fullKey">A FullKey</param>
    /// <returns>The Provider String or an empty string on error</returns>
    public static string ProviderFromKey( string fullKey )
    {
      Match match = _rxFullKey.Match( fullKey );
      if (match.Success) {
        return match.Groups["p"].Value;
      }
      return "";
    }

    /// <summary>
    /// Returns a MapProvider from a FullKey
    /// </summary>
    /// <param name="fullKey">A FullKey</param>
    /// <returns>The MapProvider or DummyProvider on error</returns>
    public static MapProvider MapProviderFromKey( string fullKey )
    {
      string p = ProviderFromKey( fullKey );
      if (string.IsNullOrWhiteSpace( p )) return MapProvider.DummyProvider;
      if (Enum.TryParse( p, true, out MapProvider mapProvider )) return mapProvider;

      return MapProvider.DummyProvider;
    }

    /// <summary>
    /// Returns the MapProvider from it's string representation
    /// </summary>
    /// <param name="mapProviderString">A string of the MapProvider Enum</param>
    /// <returns>A MapProvider (DummyProvider on error)</returns>
    public static MapProvider MapProviderFromString( string mapProviderString )
    {
      if (Enum.TryParse( mapProviderString, true, out MapProvider mapProvider )) return mapProvider;
      return MapProvider.DummyProvider;
    }

    /// <summary>
    /// Increments the TileXY.X by 1 
    ///   wraps around
    /// </summary>
    /// <param name="tileXY">A tileXY</param>
    /// <param name="zoomLevel">A Map ZoomLevel</param>
    /// <returns>A TileXY</returns>
    public static TileXY TileXY_IncX( TileXY tileXY, ushort zoomLevel )
    {
      // sanity bail out
      if (zoomLevel >= c_maxDim.Length) return tileXY;

      var v = (tileXY.X + 1) % c_maxDim[zoomLevel];
      return new TileXY( v, tileXY.Y );
    }

    /// <summary>
    /// Decrements the TileXY.X by 1 
    ///   wraps around
    /// </summary>
    /// <param name="tileXY">A tileXY</param>
    /// <param name="zoomLevel">A Map ZoomLevel</param>
    /// <returns>A TileXY</returns>
    public static TileXY TileXY_DecX( TileXY tileXY, ushort zoomLevel )
    {
      // sanity bail out
      if (zoomLevel >= c_maxDim.Length) return tileXY;

      var v = (tileXY.X - 1 + c_maxDim[zoomLevel]) % c_maxDim[zoomLevel];
      return new TileXY( v, tileXY.Y );
    }

    /// <summary>
    /// Increments the TileXY.Y by 1 
    ///   wraps around
    /// </summary>
    /// <param name="tileXY">A tileXY</param>
    /// <param name="zoomLevel">A Map ZoomLevel</param>
    /// <returns>A TileXY</returns>
    public static TileXY TileXY_IncY( TileXY tileXY, ushort zoomLevel )
    {
      // sanity bail out
      if (zoomLevel >= c_maxDim.Length) return tileXY;

      var v = (tileXY.Y + 1) % c_maxDim[zoomLevel];
      return new TileXY( tileXY.X, v );
    }

    /// <summary>
    /// Decrements the TileXY.Y by 1 
    ///   wraps around
    /// </summary>
    /// <param name="tileXY">A tileXY</param>
    /// <param name="zoomLevel">A Map ZoomLevel</param>
    /// <returns>A TileXY</returns>
    public static TileXY TileXY_DecY( TileXY tileXY, ushort zoomLevel )
    {
      // sanity bail out
      if (zoomLevel >= c_maxDim.Length) return tileXY;

      var v = (tileXY.Y - 1 + c_maxDim[zoomLevel]) % c_maxDim[zoomLevel];
      return new TileXY( tileXY.X, v );
    }

  }
}

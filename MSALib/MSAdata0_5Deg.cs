using System;
using System.Runtime.InteropServices;

using NLog;
using CoordLib;
using System.IO;

namespace MSALib
{
  internal class MSAdata0_5Deg
  {
    private const int c_degMult = 2;   // resolution is 1/ N degree

    // NLog setup
    private static readonly Logger LOG = LogManager.GetCurrentClassLogger( );

    // in degree resolution points 
    private const int c_latMax = (180 / 2) * c_degMult;
    private const int c_latMin = -(180 / 2) * c_degMult - (-1);

    private const int c_lonMin = -(360 / 2) * c_degMult;
    private const int c_lonMax = (360 / 2) * c_degMult - 1;

    private const string c_msaFileName = @".\MSAdata0_5Deg.dat";

    // latitude node 
    public static int LatIndex( double deg )
    {
      // sanity 
      deg = Geo.Wrap90( deg );

      // int deg in resolution points
      int ideg = (int)Math.Ceiling( deg * c_degMult );

      if (ideg > c_latMax) ideg = c_latMax;
      if (ideg < c_latMin) ideg = c_latMin;

      int node = -(ideg - c_latMax);
      return node;
    }

    // longitude node 
    public static int LonIndex( double deg )
    {
      // sanity 
      deg = Geo.Wrap180( deg );

      // int deg in resolution points
      int ideg = (int)Math.Floor( deg * c_degMult );

      if (ideg < c_lonMin) ideg = c_lonMin;
      if (ideg > c_lonMax) ideg = c_lonMax;

      int node = (ideg - c_lonMin);
      return node;
    }

    /// <summary>
    /// ctor: Create class from data returned by the Reader
    /// </summary>
    /// <param name="reader">A binary data reader for this type of data</param>
    public MSAdata0_5Deg( )
    {
      LOG.Info( $"Loading MSA Data" );

      // sanity
      System.Diagnostics.Trace.Assert( Marshal.SizeOf( typeof( MyRecord ) ) == RecordLength, "Record size does not match!(" + Marshal.SizeOf( typeof( MyRecord ) ).ToString( ) + ")" );


      // file in the App dir takes precedence over the embdedded resource file
      if (File.Exists( c_msaFileName )) {
        LOG.Info( $"Using Datafile <{c_msaFileName}>" );
        using (var reader = new RecReader( c_msaFileName ) { BigEndianMode = false }) {
          if (reader.IsOpen( )) {
            try {
              m_item = RecReader.ByteToType<MyRecord>( reader.TheReader );
              m_itemValid = m_item.IsValid;
            }
            catch (Exception ex) {
              // for any disruption... stop and return invalid
              LOG.Error( ex, $"Invalid Record - ignored" );
              m_itemValid = false;
            }
          }
        }
      }
      else {
        LOG.Info( $"Using Embedded Resource" );
        using (Stream msaStream = System.Reflection.Assembly.GetExecutingAssembly( ).GetManifestResourceStream( @"MSALib.MSAdata0_5Deg.dat" )) {
          using (var reader = new RecReader( msaStream, DateTime.Now ) { BigEndianMode = false }) {
            if (reader.IsOpen( )) {
              try {
                m_item = RecReader.ByteToType<MyRecord>( reader.TheReader );
                m_itemValid = m_item.IsValid;
              }
              catch (Exception ex) {
                // for any disruption... stop and return invalid
                LOG.Error( ex, $"Invalid Record - ignored" );
                m_itemValid = false;
              }
            }
            else {
              LOG.Error( $"Embedded Resource cannot be read" );
            }
          }
        }
      }
    }


    /// <summary>
    /// True if the item is valid
    /// </summary>
    public bool isValid => m_itemValid;


    /// <summary>
    /// The MSA value of a coord point
    /// </summary>
    /// <param name="lat">A Lat</param>
    /// <param name="lon">A Lon</param>
    /// <returns>An altitude in ft</returns>
    public int MSAof_ft( double lat, double lon ) => isValid ? m_item.ValueByIndex( LonIndex( lon ), LatIndex( lat ) ) * 100 : 30_000;

    /// <summary>
    /// The MSA value of a coord point
    /// </summary>
    /// <param name="latLon">A LatLon</param>
    /// <returns>An altitude in ft</returns>
    public int MSAof_ft( LatLon latLon ) => isValid ? m_item.ValueByIndex( LonIndex( latLon.Lon ), LatIndex( latLon.Lat ) ) * 100 : 30_000;


    #region DataRecord

    private MyRecord m_item;
    private bool m_itemValid = false;

    private const int RecordLength = (360 * c_degMult) * (180 * c_degMult) * 2; // sîze is Int16


    [StructLayout( LayoutKind.Sequential, Pack = 1, CharSet = CharSet.Ansi )] //, Size = RecordLength
    private struct MyRecord
    {
      public bool IsValid => true;

      // returns the datapoint (ft / 100)
      public int ValueByIndex( int lonI, int latI ) => data_[latI * (360 * c_degMult) + lonI];

      [MarshalAs( UnmanagedType.ByValArray, SizeConst = (360 * c_degMult) * (180 * c_degMult) )]
      private short[] data_; // 
    }

    #endregion

  }

}


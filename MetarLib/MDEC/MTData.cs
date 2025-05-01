using System.Collections.Generic;
using System.Linq;

namespace MetarLib.MDEC
{
  /// <summary>
  /// The Message Type 
  /// </summary>
  public enum MsgType
  {
    /// <summary>
    /// A METAR Message
    /// </summary>
    METAR = 0,
    /// <summary>
    /// A SPECI Message (metar special report)
    /// </summary>
    SPECI,
    /// <summary>
    /// A TAF Message
    /// </summary>
    TAF,
    // others
  }

  /// <summary>
  /// A Message Modifer 
  /// </summary>
  public enum MsgModifier
  {
    /// <summary>
    /// No modifier
    /// </summary>
    NONE=0,
    /// <summary>
    /// TAF Amendment
    /// </summary>
    AMD,
    /// <summary>
    /// TAF Correction
    /// </summary>
    COR,
  }

  /// <summary>
  /// METAR / TAF Data record
  /// </summary>
  public class MTData
  {
    /// <summary>
    /// The Type of the message
    /// </summary>
    public MsgType MsgType { get; set; } = MsgType.METAR;
    /// <summary>
    /// The Modifier of the message
    /// </summary>
    public MsgModifier MsgModifier { get; set; } = MsgModifier.NONE;
    /// <summary>
    /// Flag if the message was cancelled
    /// </summary>
    public bool CancelFlag { get; set; } = false;

    /// <summary>
    /// The Raw Message string
    /// </summary>
    public string RAW { get; set; } = "";
    /// <summary>
    /// Station record
    /// </summary>
    public M_Station Station { get; set; } = new M_Station( );
    /// <summary>
    /// Observation Date/Time record
    /// </summary>
    public M_ObsTime ObsTime { get; set; } = new M_ObsTime( );
    /// <summary>
    /// TAF Forecast Period record
    /// </summary>
    public T_Period TafPeriod { get; set; } = new T_Period( );
    /// <summary>
    /// Modifier record
    /// </summary>
    public M_Modifier Modifier { get; set; } = new M_Modifier( );
    /// <summary>
    /// Wind Record
    /// </summary>
    public M_Wind Wind { get; set; } = new M_Wind( );
    /// <summary>
    /// Visibility record
    /// </summary>
    public M_Visibility Visibility { get; set; } = new M_Visibility( );
    /// <summary>
    /// Runway Visibility records
    /// </summary>
    public List<M_RunwayVR> RunwayVRs { get; set; } = new List<M_RunwayVR>( );
    /// <summary>
    /// Present Weather records
    /// </summary>
    public List<M_Weather> Weather { get; set; } = new List<M_Weather>( );
    /// <summary>
    /// Sky Condition records
    /// </summary>
    public List<M_SkyCondition> SkyConditions { get; set; } = new List<M_SkyCondition>( );
    /// <summary>
    /// Temperature / Dewpoint record
    /// </summary>
    public M_Temp Temperature { get; set; } = new M_Temp( );
    /// <summary>
    /// Altimeter setting (pressure) record
    /// </summary>
    public M_Pressure Altimeter { get; set; } = new M_Pressure( );


    // Remarks Automated
    // Remarks Maint.

    /// <summary>
    /// The Flight Category
    /// </summary>
    public M_Category FlightCategory { get; set; } = new M_Category( );

    /// <summary>
    /// TAF Forecast Records
    /// </summary>
    public List<T_Forecast> Forecasts { get; set; } = new List<T_Forecast>( );


    /// <summary>
    /// Readable Content
    /// </summary>
    public string Pretty =>
      MsgType == MsgType.METAR ? MetarString( ) :
      MsgType == MsgType.SPECI ? MetarString( ) :
      MsgType == MsgType.TAF ? TafString( ) :
      "";

    /// <summary>
    /// Pretty Print METAR Data
    /// </summary>
    /// <returns>A CRFL formatted string</returns>
    private string MetarString( )
    {
      string ret = "";

      string t = Station.Pretty; if ( string.IsNullOrEmpty( t ) ) t = "NO STATION";
      ret += $"{t}";
      t = ObsTime.Pretty; if ( string.IsNullOrEmpty( t ) ) t = "NO OBS TIME";
      ret += $" @ {t}";
      t = ( MsgModifier == MsgModifier.COR ) ? "Correction " + t : "";
      ret += $" - {t}\n";
      ret += ( MsgType == MsgType.SPECI ) ? "Special Meteo Report\n" : "";

      t = Altimeter.Pretty; if ( string.IsNullOrEmpty( t ) ) t = "no P-Data";
      ret += $"{t}\n";

      t = Temperature.Pretty; if ( string.IsNullOrEmpty( t ) ) t = "no Temp Data";
      ret += $"{t}\n";

      t = Wind.Pretty; if ( string.IsNullOrEmpty( t ) ) t = "no Wind Data";
      ret += $"{t}\n";
      t = Visibility.Pretty; if ( string.IsNullOrEmpty( t ) ) t = "no Vis Data";
      ret += $"{t}\n";

      foreach ( var sk in RunwayVRs ) {
        ret += $"{sk.Pretty}\n";
      }

      foreach ( var sk in SkyConditions ) {
        ret += $"{sk.Pretty}\n";
      }

      foreach ( var sk in Weather ) {
        ret += $"{sk.Pretty}\n";
      }

      ret += $"{FlightCategory.Pretty}\n";

      return ret;
    }

    /// <summary>
    /// Pretty Print TAF Data
    /// </summary>
    /// <returns>A CRFL formatted string</returns>
    private string TafString( )
    {
      string ret = $"";
      string t = $"Forecast: {Station.Pretty}"; if ( string.IsNullOrEmpty( t ) ) t = "NO STATION";
      t = ( MsgModifier == MsgModifier.AMD ) ? "Amendement " + t : t;
      t = ( MsgModifier == MsgModifier.COR ) ? "Correction " + t : t;
      ret += $"{t}";
      ret += $" @ {ObsTime.Pretty}\n";
      //ret += $"{TafPeriod.Pretty}";

      foreach ( var fc in Forecasts ) {
        ret += $"{fc.Pretty}\n";
        ret += !string.IsNullOrEmpty( fc.Wind.Pretty ) ? $"{fc.Wind.Pretty}\n" : "";
        ret += !string.IsNullOrEmpty( fc.Visibility.Pretty ) ? $"{fc.Visibility.Pretty}\n" : "";

        foreach ( var sk in fc.SkyConditions ) {
          ret += $"{sk.Pretty}\n";
        }

        foreach ( var sk in fc.Weather ) {
          ret += $"{sk.Pretty}\n";
        }
        ret += $"{fc.FlightCategory.Pretty}\n";
      }

      return ret;
    }

    // Static Decoder

    /// <summary>
    /// Decode a METAR Message
    /// </summary>
    /// <param name="raw">The raw message starting with the StationID</param>
    /// <param name="mdata">The MData record to fill</param>
    private static void DecodeMetar( string raw, MTData mdata )
    {
      raw = M_StationDecoder.Decode( raw, mdata.Station );
      if ( !mdata.Station.Valid ) return; // ERROR must have

      raw = M_ObsTimeDecoder.Decode( raw, mdata.ObsTime );
      if ( !mdata.ObsTime.Valid ) return; // ERROR must have

      raw = M_ModifierDecoder.Decode( raw, mdata.Modifier );
      if ( mdata.Modifier.IsNil ) {
        // cancelled - forget the rest and return
        return;
      }

      raw = M_WindDecoder.Decode( raw, mdata.Wind );
      raw = M_WindDecoder.Decode( raw, mdata.Wind ); // optional variable part, will capture if there

      raw = M_VisibilityDecoder.Decode( raw, mdata.Visibility );

      while ( M_RunwayVRDecoder.IsMatch( raw ) )// optional multiple
      {
        raw = M_RunwayVRDecoder.Decode( raw, mdata.RunwayVRs );
      };

      while ( M_WeatherDecoder.IsMatch( raw ) )// optional multiple
      {
        raw = M_WeatherDecoder.Decode( raw, mdata.Weather );
      }

      while ( M_SkyConditionDecoder.IsMatch( raw ) )// optional multiple
      {
        raw = M_SkyConditionDecoder.Decode( raw, mdata.SkyConditions );
      };

      raw = M_TempDecoder.Decode( raw, mdata.Temperature );
      raw = M_PressureDecoder.Decode( raw, mdata.Altimeter );

      // RMKs 

      // Eval Category 
      mdata.FlightCategory = M_CategoryDecoder.Decode( mdata );
    }


    /// <summary>
    /// Decode a TAF Message
    /// </summary>
    /// <param name="raw">The raw message starting with the StationID</param>
    /// <param name="mdata">The MData record to fill</param>
    private static void DecodeTaf( string raw, MTData mdata )
    {
      raw = M_StationDecoder.Decode( raw, mdata.Station );
      if ( !mdata.Station.Valid ) return; // ERROR must have

      raw = M_ObsTimeDecoder.Decode( raw, mdata.ObsTime );
      if ( !mdata.ObsTime.Valid ) return; // ERROR must have

      raw = T_PeriodDecoder.Decode( raw, mdata.TafPeriod );
      if ( !mdata.TafPeriod.Valid ) return; // ERROR must have

      if ( T_CancelDecoder.IsMatch( raw ) ) {
        // cancelled - collect the flag, forget the rest and return
        raw = T_CancelDecoder.Decode( raw, mdata );
        return;
      }

      // add the basic FC record here as we go and process further parts
      mdata.Forecasts.Add( new T_Forecast( ) { Valid = true, From = mdata.TafPeriod.From, To = mdata.TafPeriod.To, IsTimeSpan = true } );

      raw = M_WindDecoder.Decode( raw, mdata.Forecasts.First( ).Wind );
      raw = M_WindDecoder.Decode( raw, mdata.Forecasts.First( ).Wind ); // optional variable part, will capture if there

      raw = M_VisibilityDecoder.Decode( raw, mdata.Forecasts.First( ).Visibility );

      while ( M_WeatherDecoder.IsMatch( raw ) )// optional multiple
      {
        raw = M_WeatherDecoder.Decode( raw, mdata.Forecasts.First( ).Weather );
      }

      while ( M_SkyConditionDecoder.IsMatch( raw ) )// optional multiple
      {
        raw = M_SkyConditionDecoder.Decode( raw, mdata.Forecasts.First( ).SkyConditions );
      };

      while ( T_TempMinMaxDecoder.IsMatch( raw ) )// optional multiple
      {
        raw = T_TempMinMaxDecoder.Decode( raw, mdata.Forecasts.First( ).TempMinMax );
      };
      // Eval Category 
      mdata.Forecasts.First( ).FlightCategory = M_CategoryDecoder.Decode( mdata.Forecasts.First( ) );

      // We should get into the additional forecast records now..
      while ( T_ForecastDecoder.IsMatch( raw ) ) {
        raw = T_ForecastDecoder.Decode( raw, mdata.Forecasts );
        // collect the allowed forecast records
        raw = M_WindDecoder.Decode( raw, mdata.Forecasts.Last( ).Wind );
        raw = M_WindDecoder.Decode( raw, mdata.Forecasts.Last( ).Wind ); // optional variable part, will capture if there

        raw = M_VisibilityDecoder.Decode( raw, mdata.Forecasts.Last( ).Visibility );

        while ( M_WeatherDecoder.IsMatch( raw ) )// optional multiple
        {
          raw = M_WeatherDecoder.Decode( raw, mdata.Forecasts.Last( ).Weather );
        }

        while ( M_SkyConditionDecoder.IsMatch( raw ) )// optional multiple
        {
          raw = M_SkyConditionDecoder.Decode( raw, mdata.Forecasts.Last( ).SkyConditions );
        };
        // Eval Category 
        mdata.Forecasts.Last( ).FlightCategory = M_CategoryDecoder.Decode( mdata.Forecasts.Last( ) );
      }

    }

    /// <summary>
    /// Decode a METAR/TAF message
    /// </summary>
    /// <param name="msg">The message</param>
    /// <returns>A filled MTData object</returns>
    public static MTData Decode( string msg )
    {
      var mdata = new MTData { RAW = msg }; // save reference
      if ( string.IsNullOrWhiteSpace( msg ) ) return mdata;

      string raw = msg.Replace("\n","") + " "; // remove CRLF and we need a space at the end...

      raw = M_MsgTypeDecoder.Decode( raw, mdata ); // defaults to METAR if not tagged with a Msg Type

      if ( mdata.MsgType == MsgType.METAR ) {
        DecodeMetar( raw, mdata );
      }
      else if ( mdata.MsgType == MsgType.SPECI ) {
        DecodeMetar( raw, mdata );
      }
      else if ( mdata.MsgType == MsgType.TAF ) {
        DecodeTaf( raw, mdata );
      }

      return mdata;
    }


  }
}

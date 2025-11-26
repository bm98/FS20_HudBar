using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using CoordLib;

namespace MetarLib.JMDEC
{
  /// <summary>
  /// One JSON Metar Entry updated 20251113
  /// </summary>
  [DataContract]
  internal class J_MetarEntry
  {
    #region Example

    /*
          [
          {
            "icaoId": "KMCI",
            "receiptTime": "2025-11-13T13:56:38.056Z",
            "obsTime": 1763041980,
            "reportTime": "2025-11-13T14:00:00.000Z",
            "temp": 10,
            "dewp": 2.8,
            "wdir": 140,
            "wspd": 9,
            "visib": "10+",
            "altim": 1020.7,
            "slp": 1020.3,
            "qcField": 12,
            "metarType": "METAR",
            "rawOb": "METAR KMCI 131353Z 14009KT 10SM BKN210 10/03 A3014 RMK AO2 SLP203 T01000028 $",
            "lat": 39.2975,
            "lon": -94.7309,
            "elev": 308,
            "name": "Kansas City Intl, MO, US",
            "cover": "BKN",
            "clouds": [
              {
                "cover": "BKN",
                "base": 21000
              }
            ],
            "fltCat": "VFR"
          },
          {
            "icaoId": "KMCI",
            "receiptTime": "2025-11-13T12:56:36.787Z",
            "obsTime": 1763038380,
            "reportTime": "2025-11-13T13:00:00.000Z",
            "temp": 8.9,
            "dewp": 2.8,
            "wdir": 130,
            "wspd": 8,
            "visib": "10+",
            "altim": 1020.7,
            "slp": 1020.5,
            "qcField": 12,
            "metarType": "METAR",
            "rawOb": "METAR KMCI 131253Z 13008KT 10SM BKN240 09/03 A3014 RMK AO2 SLP205 T00890028 $",
            "lat": 39.2975,
            "lon": -94.7309,
            "elev": 308,
            "name": "Kansas City Intl, MO, US",
            "cover": "BKN",
            "clouds": [
              {
                "cover": "BKN",
                "base": 24000
              }
            ],
            "fltCat": "VFR"
          }
        ]     
     */
    #endregion


    // Station Info
    /// <summary>
    /// ICAO identifier -Default ""
    /// </summary>
    [DataMember( Name = "icaoId", IsRequired = true )] // "icaoId": "KMCI",
    public string IcaoID { get; set; } = "";

    /// <summary>
    /// Latitude of site in degrees - Default 0
    /// </summary>
    [DataMember( Name = "lat", IsRequired = true )] // "lat": 39.2975,
    public double Lat { get; set; } = 0;

    /// <summary>
    /// Longitude of site in degrees - Default 0
    /// </summary>
    [DataMember( Name = "lon", IsRequired = true )] // "lon": -94.7309,
    public double Lon { get; set; } = 0;

    /// <summary>
    /// Elevation of site in meters - Default 0
    /// </summary>
    [DataMember( Name = "elev", IsRequired = true )] // "elev": 308,
    public int Elev_m { get; set; } = 0;

    /// <summary>
    /// Full name of the site - Default ""
    /// </summary>
    [DataMember( Name = "name", IsRequired = false )] // "name": "Kansas City Intl, MO, US",
    public string StationName { get; set; } = "";

    // Metar Base Info
    /// <summary>
    /// The time of the report (yyyy-mm-dd hh:mm:ss.sssZ) - Default ""
    /// </summary>
    [DataMember( Name = "reportTime", IsRequired = true )] // "reportTime": "2025-11-13T14:00:00.000Z",
    public string ReportTime_UTCS { get; set; } = "";

    /// <summary>
    /// Type of encoding - Allowed values "METAR" "SPECI" "SYNOP" "BUOY" "CMAN" - Default ""
    /// </summary>
    [DataMember( Name = "metarType", IsRequired = false )] // "metarType": "METAR",
    public string MetarType { get; set; } = ""; // METAR and what else...

    /// <summary>
    /// Raw text of observation - Default ""
    /// </summary>
    [DataMember( Name = "rawOb", IsRequired = false )] // "rawOb": "METAR KMCI 131353Z 14009KT 10SM BKN210 10/03 A3014 RMK AO2 SLP203 T01000028 $",
    public string RawString { get; set; } = "";


    // fields <null> values are common throughout 
    /// <summary>
    /// Flight category restriction - Default ""
    /// </summary>
    [DataMember( Name = "fltCat", IsRequired = false )] // "fltCat": "VFR"
    public string FlightCat { get; set; } = "";

    /// <summary>
    /// Temperature in Celsius -     Default 0
    /// </summary>
    [DataMember( Name = "temp", IsRequired = false )] // "temp": 10,
    public float? Temp_C { get; set; } = 0f;

    /// <summary>
    /// Dewpoint temperature in Celsius - Default null
    /// </summary>
    [DataMember( Name = "dewp", IsRequired = false )] // "dewp": 2.8,
    public float? Dewpoint_CS { get; set; } = null;

    /// <summary>
    /// Wind direction in degrees or "VRB" for variable winds - (integer | string) Default null
    /// </summary>
    [DataMember( Name = "wdir", IsRequired = false )] // "wdir": 140,
    public string WindDir_degS { get; set; } = "";  // number or 'VRB' or ??

    /// <summary>
    /// Wind speed in knots - Default null
    /// </summary>
    [DataMember( Name = "wspd", IsRequired = false )] // "wspd": 9,
    public float? WindSpeed_ktS { get; set; } = null;

    /// <summary>
    /// Wind gusts in knots - Default null
    /// </summary>
    [DataMember( Name = "wgst", IsRequired = false )] // 
    public float? WindGust_ktS { get; set; } = null;

    /// <summary>
    /// Visibility in statute miles, 10+ is greater than 10 sm - (number | string) Default null
    /// </summary>
    [DataMember( Name = "visib", IsRequired = false )] // "visib": "10+",
    public string Visibility_statMilesS { get; set; } = "";  // 10+, 6+ ..

    /// <summary>
    /// Altimeter setting in hectopascals - Default null
    /// </summary>
    [DataMember( Name = "altim", IsRequired = false )] // "altim": 1020.7,
    public float? Baro_hpa { get; set; } = null;

    /// <summary>
    /// Sea level pressure in hectopascals - Default null
    /// </summary>
    [DataMember( Name = "slp", IsRequired = false )] // "slp": 1020.3,
    public float? SealevelPressure_hpa { get; set; } = null;

    /// <summary>
    /// Encoded present weather string - Default null
    /// </summary>
    [DataMember( Name = "wxString", IsRequired = false )] // 
    public string WxString { get; set; } = "";  // seen '-RA'

    /// <summary>
    /// Maximum temperature over last 6 hours in Celsius - Default null
    /// </summary>
    [DataMember( Name = "maxT", IsRequired = false )]
    public float? MaxT_C { get; set; } = null;

    /// <summary>
    /// Minimum temperature over last 6 hours in Celsius - Default null
    /// </summary>
    [DataMember( Name = "minT", IsRequired = false )]
    public float? MinT_C { get; set; } = null;

    /// <summary>
    /// Maximum temperature over last 24 hours in Celsius - Default null
    /// </summary>
    [DataMember( Name = "maxT24", IsRequired = false )] // 
    public float? MaxT24_C { get; set; } = null; // ??

    /// <summary>
    /// Minimum temperature over last 24 hours in Celsius - Default null
    /// </summary>
    [DataMember( Name = "minT24", IsRequired = false )] // 
    public float? MinT24_C { get; set; } = null; // ??

    /// <summary>
    /// Precipitation over last hour in inches - Default null
    /// </summary>
    [DataMember( Name = "precip", IsRequired = false )] // 
    public float? Precipitation1h_in { get; set; } = null; // ??

    /// <summary>
    /// Precipitation over last 3 hours in inches - Default null
    /// </summary>
    [DataMember( Name = "pcp3hr", IsRequired = false )]
    public float? Precipitation3h_in { get; set; } = null;

    /// <summary>
    /// Precipitation over last 6 hours in inches - Default null
    /// </summary>
    [DataMember( Name = "pcp6hr", IsRequired = false )]
    public float? Precipitation6h_in { get; set; } = null;

    /// <summary>
    /// Precipitation over last 24 hours in inches - Default null
    /// </summary>
    [DataMember( Name = "pcp24hr", IsRequired = false )]
    public float? Precipitation24h_in { get; set; } = null;

    /// <summary>
    /// Snow depth in inches - Defaultt null
    /// </summary>
    [DataMember( Name = "snow", IsRequired = false )]
    public float? Snow_in { get; set; } = null;

    /// <summary>
    /// Vertical visibility in feet - Default null
    /// </summary>
    [DataMember( Name = "vertVis", IsRequired = false )]
    public float? VerticalVisibility_ft { get; set; } = null;

    /// <summary>
    /// List of Cloud bases object Default[]
    /// </summary>
    [DataMember( Name = "clouds", IsRequired = false )]
    public List<J_Clouds> CloudsList { get; set; } = new List<J_Clouds>( );


    // NON JSON

    [IgnoreDataMember]
    public LatLon LatLonElev_m => new LatLon( Lat, Lon, Elev_m );


    // non JSON

    /// <summary>
    /// Wind Direction in either deg or NaN (may be VRB code in WindDir_degS)
    /// </summary>
    [IgnoreDataMember]
    public float WindDir_deg {
      get {
        if (float.TryParse( WindDir_degS, out float fValue )) return fValue;
        return float.NaN;
      }
    }

    /// <summary>
    /// The time of the report (yyyy-mm-dd hh:mm:ss.sssZ) - Default ""
    /// </summary>
    [IgnoreDataMember]
    public DateTime ReportTime_UTC {
      get {
        if (DateTime.TryParse( ReportTime_UTCS, out DateTime dtValue )) return dtValue;
        return DateTime.MinValue;
      }
    }

  }
}

using CoordLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MetarLib.JMDEC
{
  /// <summary>
  /// One Metar Entry
  /// </summary>
  [DataContract]
  internal class J_MetarEntry
  {
    // Station Info
    [DataMember( Name = "icaoId", IsRequired = true )]
    public string IcaoID { get; set; } = "";

    [DataMember( Name = "lat", IsRequired = true )]
    public double Lat { get; set; } = 0;
    [DataMember( Name = "lon", IsRequired = true )]
    public double Lon { get; set; } = 0;
    [DataMember( Name = "elev", IsRequired = true )]
    public double Elev_m { get; set; } = 0;

    [DataMember( Name = "name", IsRequired = false )]
    public string StationName { get; set; } = "";

    // Metar Base Info
    [DataMember( Name = "reportTime", IsRequired = true )]
    public DateTime ReportTime_UTC { get; set; } = DateTime.Now;

    [DataMember( Name = "metarType", IsRequired = false )]
    public string MetarType { get; set; } = ""; // METAR and what else...

    [DataMember( Name = "rawOb", IsRequired = true )]
    public string RawString { get; set; } = "";


    // fields <null> values are common throughout 
    [DataMember( Name = "temp", IsRequired = true )]
    public float? Temp_C { get; set; } = 0f;

    [DataMember( Name = "dewp", IsRequired = true )]
    public float? Dewpoint_C { get; set; } = 0f;

    [DataMember( Name = "wdir", IsRequired = true )]
    public string WindDir_deg { get; set; } = "";  // number or 'VRB' or ??

    [DataMember( Name = "wspd", IsRequired = true )]
    public float? WindSpeed_kt { get; set; } = 0f;

    [DataMember( Name = "wgst", IsRequired = true )]
    public float? WindGust_kt { get; set; } = 0f;

    [DataMember( Name = "visib", IsRequired = true )]
    public string Visibility_statMiles { get; set; } = "";  // 10+, 6+ ..

    [DataMember( Name = "altim", IsRequired = true )]
    public float? Baro_hpa { get; set; } = 0f;

    [DataMember( Name = "slp", IsRequired = false )]
    public float? SealevelPressure_hpa { get; set; } = 0f;

    [DataMember( Name = "wxString", IsRequired = false )]
    public string WxString { get; set; } = "";  // seen '-RA'

    [DataMember( Name = "maxT", IsRequired = false )]
    public float? MaxT_C { get; set; } = 0f; // ??

    [DataMember( Name = "minT", IsRequired = false )]
    public float? MinT_C { get; set; } = 0f; // ??

    [DataMember( Name = "maxT24", IsRequired = false )]
    public float? MaxT24_C { get; set; } = 0f; // ??

    [DataMember( Name = "minT24", IsRequired = false )]
    public float? MinT24_C { get; set; } = 0f; // ??

    [DataMember( Name = "precip", IsRequired = false )]
    public float? Precipitation { get; set; } = 0f; // ??

    [DataMember( Name = "pcp3hr", IsRequired = false )]
    public float? Precipitation3h { get; set; } = 0f; // ??

    [DataMember( Name = "pcp6hr", IsRequired = false )]
    public float? Precipitation6h { get; set; } = 0f; // ??

    [DataMember( Name = "pcp24hr", IsRequired = false )]
    public float? Precipitation24h { get; set; } = 0f; // ??

    [DataMember( Name = "snow", IsRequired = false )]
    public float? Snow { get; set; } = 0f; // ??

    [DataMember( Name = "vertVis", IsRequired = false )]
    public float? VerticalVisibility { get; set; } = 0f; // ??

    [DataMember( Name = "clouds", IsRequired = false )]
    public List<J_Clouds> Cloudse { get; set; } = new List<J_Clouds>( );


    // NON JSON

    [IgnoreDataMember]
    public LatLon LatLonElev_m=> new LatLon(Lat, Lon, Elev_m);


  }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FS20_HudBar.Bar;

using SC = SimConnectClient;
using MetarLib;
using CoordLib;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// An internal METAR class with helpers
  /// </summary>
  internal class HudMetar : Metar
  {
    public string MText { get; private set; } = "";
    public string StationText { get; private set; } = "";

    public bool HasNewData { get; private set; } = false;
    public Color ConditionColor { get; private set; } = Color.White;

    /// <summary>
    /// Returns the text and set it as read
    /// </summary>
    /// <returns></returns>
    public string Read( )
    {
      HasNewData = false;
      return MText;
    }

    /// <summary>
    /// Clear the contained data to defaults
    /// </summary>
    public void Clear( )
    {
      MText = "";
      ConditionColor = HudBar.c_ActBG;
      HasNewData = true; // clear current
    }

    /// <summary>
    /// Update from METAR Callback data
    /// </summary>
    /// <param name="metarDatas"></param>
    public void Update( MetarTafDataList metarDatas )
    {
      var closest = metarDatas.GetClosest( SC.SimConnectClient.Instance.AircraftModule.Lat, SC.SimConnectClient.Instance.AircraftModule.Lon );
      if ( closest.Valid ) {
        MText = closest.Pretty;
        StationText = $"{closest.Data.Station.StationID}";
        if ( !float.IsNaN( closest.Distance_nm ) )
          StationText += $" {closest.Distance_nm:##0.0} nm @{closest.Bearing_deg:000}°";

        HasNewData = true;
        ConditionColor =
          ( closest.Data.FlightCategory.FlightCategoryColor == "green" ) ? HudBar.c_MetG :
          ( closest.Data.FlightCategory.FlightCategoryColor == "blue" ) ? HudBar.c_MetB :
          ( closest.Data.FlightCategory.FlightCategoryColor == "red" ) ? HudBar.c_MetR :
          ( closest.Data.FlightCategory.FlightCategoryColor == "magenta" ) ? HudBar.c_MetM :
          ( closest.Data.FlightCategory.FlightCategoryColor == "black" ) ? HudBar.c_MetK :  // SUB ILS
          ( closest.Data.FlightCategory.FlightCategoryColor == "white" ) ? HudBar.c_MetR : HudBar.c_ActBG; // unknown
      }
      else {
        MText = "";
        StationText = "n.a.";
        HasNewData = true;
        ConditionColor = HudBar.c_ActBG;
      }

    }
  }

}

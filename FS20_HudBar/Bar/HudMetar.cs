using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FS20_HudBar.Bar;

using SC = SimConnectClient;
using static FSimClientIF.Sim;
using MetarLib;
using FSimClientIF.Modules;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// An internal METAR class with helpers
  /// </summary>
  internal class HudMetar : Metar
  {
    // SimVar access
    private readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;

    public string MText { get; private set; } = "";
    public string StationText { get; private set; } = "";

    public bool HasNewData { get; private set; } = false;
    public GUI.GUI_Colors.ColorType ConditionColor { get; private set; } = GUI.GUI_Colors.ColorType.cActBG;

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
      ConditionColor = GUI.GUI_Colors.ColorType.cActBG;
      HasNewData = true; // clear current
    }

    /// <summary>
    /// Update from METAR Callback data
    /// </summary>
    /// <param name="metarDatas"></param>
    public void Update( MetarTafDataList metarDatas )
    {
      var closest = metarDatas.GetClosest( SV.Get<double>( SItem.dGS_Acft_Lat ), SV.Get<double>( SItem.dGS_Acft_Lon ) );
      if (closest.Valid) {
        MText = closest.Pretty;
        StationText = $"{closest.Data.Station.StationID}";
        if (!float.IsNaN( closest.Distance_nm ))
          StationText += $" {closest.Distance_nm:##0.0} nm @{closest.Bearing_deg:000}°";

        HasNewData = true;
        ConditionColor =
          (closest.Data.FlightCategory.FlightCategoryColor == "green") ? GUI.GUI_Colors.ColorType.cMetG :
          (closest.Data.FlightCategory.FlightCategoryColor == "blue") ? GUI.GUI_Colors.ColorType.cMetB :
          (closest.Data.FlightCategory.FlightCategoryColor == "red") ? GUI.GUI_Colors.ColorType.cMetR :
          (closest.Data.FlightCategory.FlightCategoryColor == "magenta") ? GUI.GUI_Colors.ColorType.cMetM :
          (closest.Data.FlightCategory.FlightCategoryColor == "black") ? GUI.GUI_Colors.ColorType.cMetK :  // SUB ILS
          (closest.Data.FlightCategory.FlightCategoryColor == "white") ? GUI.GUI_Colors.ColorType.cMetR : GUI.GUI_Colors.ColorType.cActBG; // unknown
      }
      else {
        MText = "";
        StationText = "n.a.";
        HasNewData = true;
        ConditionColor = GUI.GUI_Colors.ColorType.cActBG;
      }

    }
  }

}

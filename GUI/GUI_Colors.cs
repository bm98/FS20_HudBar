using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// Container to have all colors used in one place
  /// </summary>
  class GUI_Colors
  {
    // Item Colors Foreground
    public static readonly Color c_Info = Color.WhiteSmoke;   // Info Text (basically white)
    public static readonly Color c_OK = Color.LimeGreen;      // OK Text
    public static readonly Color c_AP = Color.LimeGreen;      // Autopilot, NAV (green)
    public static readonly Color c_Gps = Color.Fuchsia;       // GPS (magenta)
    public static readonly Color c_Set = Color.Cyan;          // Set Values (cyan)
    public static readonly Color c_RA = Color.Orange;         // Radio Alt
    public static readonly Color c_Est = Color.Plum;          // Estimates 
    // those are set in the data receiver part in Main (here to have all in one place)
    public static readonly Color c_SubZero = Color.DeepSkyBlue;  // Temp sub zero
    public static readonly Color c_SRATE = Color.Goldenrod;   // SimRate != 1

    // Background
    public static readonly Color c_BG = Color.Transparent;    // regular background for all controls
    public static readonly Color c_WinBG = Color.FromArgb(0,1,12); // Window background (slight blueish)
    public static readonly Color c_ActBG = Color.FromArgb(0,0,75); // Actionable Items Background (dark blue)
    public static readonly Color c_ActPressed = Color.Indigo;      // Actionable Items Background - pressed
    public static readonly Color c_LiveBG = Color.DarkGreen;  // Live Items Background
    public static readonly Color c_FBCol =  Color.PaleGreen;  // FlowBreak Background in Configuration

    
    public static readonly Color c_MetG = Color.ForestGreen;  // METAR Green
    public static readonly Color c_MetB = Color.Blue;         // METAR Blue
    public static readonly Color c_MetR = Color.Crimson;      // METAR Red
    public static readonly Color c_MetM = Color.DarkViolet;   // METAR Magenta
    public static readonly Color c_MetK = Color.DarkOrange;   // METAR Black (SUB ILS)

  }
}

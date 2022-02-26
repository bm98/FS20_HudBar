using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// Container to have all colors used in one place
  /// </summary>
  static class GUI_Colors
  {
    // Color Enum for items
    public enum ColorType
    {
      // FOREGROUND

      /// <summary>
      /// Item Label color
      /// </summary>
      cLabel=0,
      /// <summary>
      /// Information item color
      /// </summary>
      cInfo,
      /// <summary>
      /// OK content color
      /// </summary>
      cOK,
      /// <summary>
      /// Warning content color
      /// </summary>
      cWarn,
      /// <summary>
      /// Alerting content color
      /// </summary>
      cAlert,
      /// <summary>
      /// Autopilot Active color
      /// </summary>
      cAP,
      /// <summary>
      /// NAV Active color
      /// </summary>
      cNav,
      /// <summary>
      /// GPS items color
      /// </summary>
      cGps,
      /// <summary>
      /// Autopilot Setting color
      /// </summary>
      cSet,
      /// <summary>
      /// Radio Altitude color
      /// </summary>
      cRA,
      /// <summary>
      /// Estimate Items color
      /// </summary>
      cEst,
      /// <summary>
      /// Average Items color
      /// </summary>
      cAvg,
      /// <summary>
      /// OAT Freezing color
      /// </summary>
      cSubZero,
      /// <summary>
      /// SimRate not nominal color
      /// </summary>
      cSRATE,
      /// <summary>
      /// Stepping color (Flaps etc)
      /// </summary>
      cStep,
      /// <summary>
      /// Inverse readout color (black)
      /// </summary>
      cInverse,

      // BACKGROUND
      /// <summary>
      /// Default Background (Transparent)
      /// </summary>
      cBG,
      /// <summary>
      /// Actionable Items Background (dark blue)
      /// </summary>
      cActBG,
      /// <summary>
      /// Live Items Background (e.g. when something is temporary active)
      /// </summary>
      cLiveBG,
      /// <summary>
      /// Warning Items Background (e.g. when something is to be fixed)
      /// </summary>
      cWarnBG,
      /// <summary>
      /// Alert Items Background (e.g. when something is to be fixed immediately)
      /// </summary>
      cAlertBG,

      // METAR BACKGROUND
      /// <summary>
      /// METAR Green
      /// </summary>
      cMetG,
      /// <summary>
      /// METAR Blue
      /// </summary>
      cMetB,
      /// <summary>
      /// METAR Red
      /// </summary>
      cMetR,
      /// <summary>
      /// METAR Magenta
      /// </summary>
      cMetM,
      /// <summary>
      /// METAR Black (SUB ILS)
      /// </summary>
      cMetK,

      // Separator Back
      cDivBG1, // type 1 darker blueish
      cDivBG2, // type 2 brighter yellowish

      // Color Scale e.g. Wind
      cScale0,  // lowest
      cScale1,
      cScale2,
      cScale3,
      cScale4,
      cScale5,  // highest
    }

    /// <summary>
    /// Color Sets to choose from
    /// </summary>
    public enum ColorSet
    {
      BrightSet=0,
      DimmedSet,
      DarkSet,
    }

    private static Color Dimm( Color color, float percent = 70f )
    {
      percent *= 0.01f;
      return Color.FromArgb( color.A,
        (int)( color.R * percent ), (int)( color.G * percent ), (int)( color.B * percent ) );
    }

    // colors assuming a darker background; fore colors are on the brighter side 
    //  Transparency <=40%
    //  Transparency > 40% during night flight (again darker background)
    private static readonly Dictionary<ColorType, Color> c_brightColors = new Dictionary<ColorType, Color>() {
      { ColorType.cLabel, Color.Silver },
      { ColorType.cInfo, Color.WhiteSmoke },
      { ColorType.cOK, Color.LimeGreen },
      { ColorType.cWarn, Color.Gold },
      { ColorType.cAlert, Color.OrangeRed },
      { ColorType.cAP, Color.LimeGreen },
      { ColorType.cNav, Color.LimeGreen },
      { ColorType.cGps, Color.Fuchsia },
      { ColorType.cSet, Color.Cyan },
      { ColorType.cRA, Color.Orange },
      { ColorType.cEst, Color.Plum },
      { ColorType.cAvg, Dimm( Color.Yellow )}, // toned down yellow
      { ColorType.cSubZero, Color.DeepSkyBlue },
      { ColorType.cSRATE, Color.Goldenrod },
      { ColorType.cStep, Color.LightBlue },
      { ColorType.cInverse, Color.Black },

      { ColorType.cBG, Color.Transparent },
      { ColorType.cActBG, Color.FromArgb(0,0,75) },
      { ColorType.cLiveBG, Color.DarkGreen },
      { ColorType.cWarnBG, Color.DarkGoldenrod },
      { ColorType.cAlertBG, Color.Firebrick },

      { ColorType.cMetG, Color.ForestGreen }, // METAR Green
      { ColorType.cMetB, Color.Blue }, // METAR Blue
      { ColorType.cMetR, Color.Crimson },// METAR Red
      { ColorType.cMetM, Color.DarkViolet },// METAR Magenta
      { ColorType.cMetK, Color.DarkOrange },// METAR Black (SUB ILS)

      { ColorType.cDivBG1, Color.FromArgb(46,56,107) },// Separator Background Blueish
      { ColorType.cDivBG2, Color.FromArgb(217,216,160) },// Separator Background Yellowish bright

      { ColorType.cScale0, Color.MintCream }, // Color Scale light greenish .. Magenta
      { ColorType.cScale1, Color.LawnGreen }, // Color Scale light greenish .. Magenta
      { ColorType.cScale2, Color.Gold }, // Color Scale light greenish .. Magenta
      { ColorType.cScale3, Color.Orange }, // Color Scale light greenish .. Magenta
      { ColorType.cScale4, Color.Red }, // Color Scale light greenish .. Magenta
      { ColorType.cScale5, Color.Magenta }, // Color Scale light greenish .. Magenta

      //{ ColorType.cDivBG, Dimm( Color.LightSteelBlue, 50) },// Separator Background
      //{ ColorType.cDivBG, Color.FromArgb(79,83,43) },// Separator Background Yellowish
      //{ ColorType.cDivBG, Color.FromArgb(130,142,193) },// Separator Background Blueish bright
    };

    // dimmed foreground colors; based on Bright Colors
    //  Transparency <=40% during night flight 
    private static readonly Dictionary<ColorType, Color> c_dimColors = new Dictionary<ColorType, Color>() {
      { ColorType.cLabel, Dimm( c_brightColors[ColorType.cLabel],85f )},
      { ColorType.cInfo, Dimm( c_brightColors[ColorType.cInfo] )},
      { ColorType.cOK, Dimm( c_brightColors[ColorType.cOK] )},
      { ColorType.cWarn, Dimm( c_brightColors[ColorType.cWarn] )},
      { ColorType.cAlert, Dimm( c_brightColors[ColorType.cAlert] )},
      { ColorType.cAP, Dimm( c_brightColors[ColorType.cAP] )},
      { ColorType.cNav, Dimm( c_brightColors[ColorType.cAP] )},
      { ColorType.cGps, Dimm( c_brightColors[ColorType.cGps] )},
      { ColorType.cSet, Dimm( c_brightColors[ColorType.cSet] )},
      { ColorType.cRA, Dimm( c_brightColors[ColorType.cRA] )},
      { ColorType.cEst, Dimm( c_brightColors[ColorType.cEst] )},
      { ColorType.cAvg, Dimm( c_brightColors[ColorType.cAvg] )},
      { ColorType.cSubZero, Dimm( c_brightColors[ColorType.cSubZero] )},
      { ColorType.cSRATE, Dimm( c_brightColors[ColorType.cSRATE] )},
      { ColorType.cStep, Color.LightBlue },
      { ColorType.cInverse, Color.Black },

      { ColorType.cBG, Color.Transparent },
      { ColorType.cActBG, Color.FromArgb(0,0,75) },
      { ColorType.cLiveBG, Color.DarkGreen },
      { ColorType.cWarnBG, Color.DarkGoldenrod },
      { ColorType.cAlertBG, Color.Firebrick },

      { ColorType.cMetG, Color.ForestGreen }, // METAR Green
      { ColorType.cMetB, Color.Blue }, // METAR Blue
      { ColorType.cMetR, Color.Crimson },// METAR Red
      { ColorType.cMetM, Color.DarkViolet },// METAR Magenta
      { ColorType.cMetK, Color.DarkOrange },// METAR Black (SUB ILS)
      { ColorType.cDivBG1, Dimm( c_brightColors[ColorType.cDivBG1] ) },// Separator Background
      { ColorType.cDivBG2, Dimm( c_brightColors[ColorType.cDivBG2] ) },// Separator Background

      { ColorType.cScale0, Dimm( c_brightColors[ColorType.cScale0] ) },// Color Scale
      { ColorType.cScale1, Dimm( c_brightColors[ColorType.cScale1] ) },// Color Scale
      { ColorType.cScale2, Dimm( c_brightColors[ColorType.cScale2] ) },// Color Scale
      { ColorType.cScale3, Dimm( c_brightColors[ColorType.cScale3] ) },// Color Scale
      { ColorType.cScale4, Dimm( c_brightColors[ColorType.cScale4] ) },// Color Scale
      { ColorType.cScale5, Dimm( c_brightColors[ColorType.cScale5] ) },// Color Scale

    };

    // colors assuming a brighter background; fore colors cannot be white or blue
    //  if Transparency > 40% during day flight
    private static readonly Dictionary<ColorType, Color> c_darkColors = new Dictionary<ColorType, Color>() {
      { ColorType.cLabel, Color.Black },
      { ColorType.cInfo, Color.Black },
      { ColorType.cOK, Color.DarkGreen },
      { ColorType.cWarn, Color.FromArgb(214, 181, 17) }, // orange/yellow
      { ColorType.cAlert,Color.FromArgb(214, 17, 17) }, // red
      { ColorType.cAP, Color.FromArgb(17, 184, 64) }, // green
      { ColorType.cNav, Color.FromArgb(17, 184, 64) }, // green
      { ColorType.cGps, Color.FromArgb(143, 26, 135) },// Color.DarkMagenta
      { ColorType.cSet, Color.DarkBlue },
      { ColorType.cRA, Color.FromArgb(156, 88, 0) }, // orange
      { ColorType.cEst, Color.DarkOrchid },
      { ColorType.cAvg, Color.FromArgb(90, 90, 0) }, // dark Yellow
      { ColorType.cSubZero, Color.DarkBlue },
      { ColorType.cSRATE, Color.Orange },
      { ColorType.cStep, Color.DeepSkyBlue },
      { ColorType.cInverse, Color.WhiteSmoke },

      { ColorType.cBG, Color.Transparent },
      { ColorType.cActBG, Color.FromArgb(124, 214, 252) }, // mid blue
      { ColorType.cLiveBG, Color.DarkGreen },
      { ColorType.cWarnBG, Color.DarkGoldenrod },
      { ColorType.cAlertBG, Color.Firebrick },

      { ColorType.cMetG, Color.ForestGreen }, // METAR Green
      { ColorType.cMetB, Color.Blue }, // METAR Blue
      { ColorType.cMetR, Color.Crimson },// METAR Red
      { ColorType.cMetM, Color.DarkViolet },// METAR Magenta
      { ColorType.cMetK, Color.DarkOrange },// METAR Black (SUB ILS)

      { ColorType.cDivBG1, Dimm(Color.DeepSkyBlue, 50) },// Separator Background type 1
      { ColorType.cDivBG2, Color.FromArgb(217,216,160) },// Separator Background Yellowish bright

      { ColorType.cScale0, Dimm( c_dimColors[ColorType.cScale0] ) },// Color Scale
      { ColorType.cScale1, Dimm( c_dimColors[ColorType.cScale1] ) },// Color Scale
      { ColorType.cScale2, Dimm( c_dimColors[ColorType.cScale2] ) },// Color Scale
      { ColorType.cScale3, Dimm( c_dimColors[ColorType.cScale3] ) },// Color Scale
      { ColorType.cScale4, Dimm( c_dimColors[ColorType.cScale4] ) },// Color Scale
      { ColorType.cScale5, Dimm( c_dimColors[ColorType.cScale5] ) },// Color Scale

    };


    // Lookup for colors
    private static readonly Dictionary<ColorSet, Dictionary<ColorType, Color>> c_colors = new Dictionary<ColorSet, Dictionary<ColorType, Color>>()
    {
      { ColorSet.BrightSet, c_brightColors },
      { ColorSet.DimmedSet, c_dimColors },
      { ColorSet.DarkSet, c_darkColors },
    };

    // manage dynamic color updates for the registered Interfaces
    private static List<IColorType> m_registeredCtrls = new List<IColorType>();

    /// <summary>
    /// Update the colors of the registered controls
    /// </summary>
    private static void UpdateRegisteredItems( )
    {
      foreach ( var ci in m_registeredCtrls ) {
        ci.UpdateColor( );
      }
    }

    private static ColorSet m_currentColorSet = ColorSet.BrightSet;

    /// <summary>
    /// Get;Set the ForeGround Colors to use 
    /// </summary>
    public static ColorSet Colorset {
      get => m_currentColorSet;
      set {
        m_currentColorSet = value;
        UpdateRegisteredItems( );

      }
    }

    /// <summary>
    /// Convert an Int to a colorset, checking the value
    /// </summary>
    /// <param name="csValue">A numeric value to convert</param>
    /// <returns>The corresponding ColorSet (or Bright if the value is undef)</returns>
    public static ColorSet ToColorSet( int csValue )
    {
      if ( Enum.IsDefined( typeof( ColorSet ), csValue ) ) {
        return (ColorSet)csValue;
      }
      return ColorSet.BrightSet;
    }


    /// <summary>
    /// Changes the returned colors to the next colorset
    /// Rotating through and back to the first
    /// </summary>
    public static void NextColorset( )
    {
      int next = (int)Colorset + 1;
      next = ( next > (int)ColorSet.DarkSet ) ? 0 : next; // rotate
      Colorset = (ColorSet)next;
    }

    /// <summary>
    /// Returns the current color of Type
    /// </summary>
    /// <param name="colorType">A ColorType</param>
    /// <returns>A Color</returns>
    public static Color ItemColor( ColorType colorType )
    {
      return c_colors[Colorset][colorType];
    }

    /// <summary>
    /// Clear the registered Interfaces
    /// </summary>
    public static void ClearRegistry( )
    {
      m_registeredCtrls.Clear( );
    }

    /// <summary>
    /// Register an Interface for color set updates
    /// </summary>
    /// <param name="ctrlInterface">The Control to register</param>
    public static void Register( IColorType ctrlInterface )
    {
      // don't bother with duplicate registration
      try {
        m_registeredCtrls.Add( ctrlInterface );
      }
      catch { }
    }

    public static void Unregister( IColorType ctrlInterface )
    {
      // don't bother with unknown registration
      try {
        m_registeredCtrls.Remove( ctrlInterface );
      }
      catch { }
    }

    // Background items with static colors
    public static readonly Color c_WinBG = Color.FromArgb(0,1,12); // Window background (slight blueish)
    public static readonly Color c_ActPressed = Color.Indigo;      // Actionable Items Background - pressed

    public static readonly Color c_FBCol =  Color.PaleGreen;   // FlowBreak Background in Configuration
    public static readonly Color c_DB1Col =  Color.LightSkyBlue;  // DivBreak Type 1 Background in Configuration
    public static readonly Color c_DB2Col =  Color.Khaki;  // DivBreak Type 2 Background in Configuration
    public static readonly Color c_NBCol =  Color.WhiteSmoke;  // NoneBreak Background in Configuration

  }
}

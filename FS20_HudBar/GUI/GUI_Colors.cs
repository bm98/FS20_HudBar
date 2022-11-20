using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FS20_HudBar.GUI.GUI_Colors;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// Container to have all colors used in one place
  /// </summary>
  public static class GUI_Colors
  {
    // Color Enum for items
    public enum ColorType
    {
      // TEXT COLORS for Labels and Values - configurable
      /// <summary>
      /// All and ONLY Item Labels (Whiteish)
      /// </summary>
      cTxLabel = 0,
      /// <summary>
      /// Label Armed color (Cyan)
      /// </summary>
      cTxLabelArmed,
      /// <summary>
      /// Info Values (White)
      /// </summary>
      cTxInfo,
      /// <summary>
      /// Inverse readout color (black)
      /// </summary>
      cTxInfoInverse,
      /// <summary>
      /// Dimmed Values (Whiteish)
      /// </summary>
      cTxDim,
      /// <summary>
      /// Active Values / status Green (Green)
      /// </summary>
      cTxActive,
      /// <summary>
      /// Warn Values / status Yellow (Yellow)
      /// </summary>
      cTxWarn,
      /// <summary>
      /// Alert Values / status Red (Red)
      /// </summary>
      cTxAlert,
      /// <summary>
      /// Autopilot Active Labels (Green)
      /// </summary>
      cTxAPActive,
      /// <summary>
      /// NAV Values (Green)
      /// </summary>
      cTxNav,
      /// <summary>
      /// GPS Values (Magenta)
      /// </summary>
      cTxGps,
      /// <summary>
      /// Settings/Armed Values (Cyan)
      /// </summary>
      cTxSet,
      /// <summary>
      /// Calculated Estimates Values (Dim Magenta)
      /// </summary>
      cTxEst,
      /// <summary>
      /// Radio Alt Values (Amber)
      /// </summary>
      cTxRA,
      /// <summary>
      /// Calculated Average Values ()
      /// </summary>
      cTxAvg,
      /// <summary>
      /// Temp <= 0°C Values (Blue)
      /// </summary>
      cTxSubZero,

      /// <summary>
      /// Opaque Background (Black)
      /// </summary>
      cOpaqueBG,

      // FOREGROUND OTHER THAN Labels/Values

      /// <summary>
      /// Information item color
      /// </summary>
      cInfo = 100, // Gap this one - not configurable from here onwards
      /// <summary>
      /// OK content color
      /// </summary>
      cOK,
      /// <summary>
      /// Alternate OK items color
      /// </summary>
      cAltOK,
      /// <summary>
      /// Warning content color
      /// </summary>
      cWarn,
      /// <summary>
      /// Alerting content color
      /// </summary>
      cAlert,
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
      /// <summary>
      /// SimRate Background not nominal color
      /// </summary>
      cSimRateWarnBG,

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

      /// <summary>
      /// Actionable VALUE Background
      /// </summary>
      cValBG, // type 1 dark blueish

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
      BrightSet = 0,
      DimmedSet,
      InverseSet,
    }

    // The ColorTypes that can be configured
    private static readonly List<ColorType> c_configColTypes = new List<ColorType> {
      ColorType.cTxLabel,
      ColorType.cTxLabelArmed,
      ColorType.cTxInfo,
      ColorType.cTxInfoInverse,
      ColorType.cTxDim,
      ColorType.cTxActive,
      ColorType.cTxAlert,
      ColorType.cTxWarn,
      ColorType.cTxAPActive,
      ColorType.cTxSet,
      ColorType.cTxNav,
      ColorType.cTxGps,
      ColorType.cTxEst,
      ColorType.cTxRA,
      ColorType.cTxAvg,
      ColorType.cTxSubZero,
      ColorType.cOpaqueBG,
    };


    // Dimms a given color
    private static Color Dimm( Color color, float percent = 70f )
    {
      percent *= 0.01f;
      return Color.FromArgb( color.A,
        (int)(color.R * percent), (int)(color.G * percent), (int)(color.B * percent) );
    }

    // DEFAULT
    //  colors assuming a darker background; fore colors are on the brighter side 
    //  Transparency <=40%
    //  Transparency > 40% during night flight (again darker background)
    private static readonly GUI_ColorSet c_brightColors = new GUI_ColorSet( ) {
      // Text Colors
      { ColorType.cTxLabel, Color.Silver },
      { ColorType.cTxLabelArmed, Color.Cyan },
      { ColorType.cTxInfo, Color.WhiteSmoke },
      { ColorType.cTxInfoInverse, Color.Black },
      { ColorType.cTxDim, Color.Silver },
      { ColorType.cTxActive, Color.LimeGreen },
      { ColorType.cTxWarn, Color.Gold },
      { ColorType.cTxAlert, Color.OrangeRed },
      { ColorType.cTxAPActive, Color.LimeGreen },
      { ColorType.cTxNav, Color.LimeGreen },
      { ColorType.cTxGps, Color.Fuchsia },
      { ColorType.cTxSet, Color.Cyan },
      { ColorType.cTxRA, Color.Orange },
      { ColorType.cTxEst, Color.Plum },
      { ColorType.cTxAvg, Dimm( Color.Yellow )}, // toned down yellow
      { ColorType.cTxSubZero, Color.DeepSkyBlue },
      { ColorType.cOpaqueBG,Color.FromArgb( 0, 1, 12 ) }, // Window background (slight blueish)
      //
      { ColorType.cInfo, Color.WhiteSmoke },
      { ColorType.cOK, Color.LimeGreen },
      { ColorType.cWarn, Color.Gold },
      { ColorType.cAlert, Color.OrangeRed },
      { ColorType.cAltOK, Color.Fuchsia },
      { ColorType.cStep, Color.LightBlue },
      { ColorType.cInverse, Color.Black },

      { ColorType.cBG, Color.Transparent },
      { ColorType.cActBG, Color.FromArgb(0,0,75) },
      { ColorType.cLiveBG, Color.DarkGreen },
      { ColorType.cWarnBG, Color.DarkGoldenrod },
      { ColorType.cAlertBG, Color.Firebrick },
      { ColorType.cSimRateWarnBG, Color.Goldenrod },

      { ColorType.cMetG, Color.ForestGreen }, // METAR Green
      { ColorType.cMetB, Color.Blue }, // METAR Blue
      { ColorType.cMetR, Color.Crimson },// METAR Red
      { ColorType.cMetM, Color.DarkViolet },// METAR Magenta
      { ColorType.cMetK, Color.DarkOrange },// METAR Black (SUB ILS)

      { ColorType.cDivBG1, Color.FromArgb(46,56,107) },// Separator Background Blueish
      { ColorType.cDivBG2, Color.FromArgb(217,216,160) },// Separator Background Yellowish bright

      { ColorType.cValBG, Color.FromArgb(16,26,67) },// Clickable Value Background, blueish dark

      { ColorType.cScale0, Color.MintCream }, // Color Scale light greenish .. Magenta
      { ColorType.cScale1, Color.LawnGreen },
      { ColorType.cScale2, Color.Gold },
      { ColorType.cScale3, Color.Orange },
      { ColorType.cScale4, Color.Red },
      { ColorType.cScale5, Color.Magenta },

      //{ ColorType.cDivBG, Dimm( Color.LightSteelBlue, 50) },// Separator Background
      //{ ColorType.cDivBG, Color.FromArgb(79,83,43) },// Separator Background Yellowish
      //{ ColorType.cDivBG, Color.FromArgb(130,142,193) },// Separator Background Blueish bright
    };

    // DEFAULT
    // dimmed foreground colors; based on Bright Colors
    //  Transparency <=40% during night flight 
    private static readonly GUI_ColorSet c_dimColors = new GUI_ColorSet( ) {
      // Text Colors
      { ColorType.cTxLabel, Dimm( c_brightColors[ColorType.cTxLabel],85f )},
      { ColorType.cTxLabelArmed, Dimm( c_brightColors[ColorType.cTxLabelArmed] )},
      { ColorType.cTxInfo, Dimm( c_brightColors[ColorType.cTxInfo] )},
      { ColorType.cTxInfoInverse, Color.Black },
      { ColorType.cTxDim, Dimm( c_brightColors[ColorType.cTxDim] )},
      { ColorType.cTxActive, Dimm( c_brightColors[ColorType.cTxActive] )},
      { ColorType.cTxWarn, Dimm( c_brightColors[ColorType.cTxWarn] )},
      { ColorType.cTxAlert, Dimm( c_brightColors[ColorType.cTxAlert] )},
      { ColorType.cTxAPActive, Dimm( c_brightColors[ColorType.cTxAPActive] )},
      { ColorType.cTxNav, Dimm( c_brightColors[ColorType.cTxNav] )},
      { ColorType.cTxGps, Dimm( c_brightColors[ColorType.cTxGps] )},
      { ColorType.cTxSet, Dimm( c_brightColors[ColorType.cTxSet] )},
      { ColorType.cTxRA, Dimm( c_brightColors[ColorType.cTxRA] )},
      { ColorType.cTxEst, Dimm( c_brightColors[ColorType.cTxEst] )},
      { ColorType.cTxAvg, Dimm( c_brightColors[ColorType.cTxAvg] )},
      { ColorType.cTxSubZero, Dimm( c_brightColors[ColorType.cTxSubZero] )},
      { ColorType.cOpaqueBG, Color.FromArgb( 0, 1, 12 ) }, // Window background (slight blueish)
      //
      { ColorType.cInfo, Dimm( c_brightColors[ColorType.cInfo] )},
      { ColorType.cOK, Dimm( c_brightColors[ColorType.cOK] )},
      { ColorType.cWarn, Dimm( c_brightColors[ColorType.cWarn] )},
      { ColorType.cAlert, Dimm( c_brightColors[ColorType.cAlert] )},
      { ColorType.cAltOK, Dimm( c_brightColors[ColorType.cAltOK] )},
      { ColorType.cSimRateWarnBG, Dimm( c_brightColors[ColorType.cSimRateWarnBG] )},
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

      { ColorType.cValBG, Dimm( c_brightColors[ColorType.cValBG] ) },// Clickable Value Background

      { ColorType.cScale0, Dimm( c_brightColors[ColorType.cScale0] ) },// Color Scale
      { ColorType.cScale1, Dimm( c_brightColors[ColorType.cScale1] ) },// Color Scale
      { ColorType.cScale2, Dimm( c_brightColors[ColorType.cScale2] ) },// Color Scale
      { ColorType.cScale3, Dimm( c_brightColors[ColorType.cScale3] ) },// Color Scale
      { ColorType.cScale4, Dimm( c_brightColors[ColorType.cScale4] ) },// Color Scale
      { ColorType.cScale5, Dimm( c_brightColors[ColorType.cScale5] ) },// Color Scale

    };

    // DEFAULT
    // colors assuming a brighter background; fore colors cannot be white or blue
    //  if Transparency > 40% during day flight
    private static readonly GUI_ColorSet c_inverseColors = new GUI_ColorSet( ) {
      // Text Colors
      { ColorType.cTxLabel, Color.Black },
      { ColorType.cTxLabelArmed, Color.DarkBlue },
      { ColorType.cTxInfo, Color.Black },
      { ColorType.cTxInfoInverse, Color.WhiteSmoke },
      { ColorType.cTxDim, Color.Black },
      { ColorType.cTxActive, Color.DarkGreen },
      { ColorType.cTxWarn, Color.FromArgb(214, 181, 17) }, // orange/yellow
      { ColorType.cTxAlert,Color.FromArgb(214, 17, 17) }, // red
      { ColorType.cTxAPActive, Color.FromArgb(17, 184, 64) }, // green
      { ColorType.cTxNav, Color.FromArgb(17, 184, 64) }, // green
      { ColorType.cTxGps, Color.FromArgb(143, 26, 135) },// Color.DarkMagenta
      { ColorType.cTxSet, Color.DarkBlue },
      { ColorType.cTxRA, Color.FromArgb(156, 88, 0) }, // orange
      { ColorType.cTxEst, Color.DarkOrchid },
      { ColorType.cTxAvg, Color.FromArgb(90, 90, 0) }, // dark Yellow
      { ColorType.cTxSubZero, Color.DarkBlue },
      { ColorType.cOpaqueBG,Color.Honeydew }, // Window background (bright blueish)
      //
      { ColorType.cInfo, Color.Black },
      { ColorType.cOK, Color.DarkGreen },
      { ColorType.cWarn, Color.FromArgb(214, 181, 17) }, // orange/yellow
      { ColorType.cAlert,Color.FromArgb(214, 17, 17) }, // red
      { ColorType.cAltOK, Color.FromArgb(143, 26, 135) },// Color.DarkMagenta
      { ColorType.cSimRateWarnBG, Color.Orange },
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

      { ColorType.cValBG, Dimm(Color.FromArgb(124, 214, 252) ) },// Clickable Value Background

      { ColorType.cScale0, Dimm( c_dimColors[ColorType.cScale0] ) },// Color Scale
      { ColorType.cScale1, Dimm( c_dimColors[ColorType.cScale1] ) },// Color Scale
      { ColorType.cScale2, Dimm( c_dimColors[ColorType.cScale2] ) },// Color Scale
      { ColorType.cScale3, Dimm( c_dimColors[ColorType.cScale3] ) },// Color Scale
      { ColorType.cScale4, Dimm( c_dimColors[ColorType.cScale4] ) },// Color Scale
      { ColorType.cScale5, Dimm( c_dimColors[ColorType.cScale5] ) },// Color Scale

    };

    // Lookup for colors - contains the actual colors for each set
    private static Dictionary<ColorSet, GUI_ColorSet> _colors = new Dictionary<ColorSet, GUI_ColorSet>( )
    {
      { ColorSet.BrightSet, c_brightColors },
      { ColorSet.DimmedSet, c_dimColors },
      { ColorSet.InverseSet, c_inverseColors },
    };

    /// <summary>
    /// cTor: static (init defaults)
    /// </summary>
    static GUI_Colors( )
    {
      ResetDefaultColorSets( );
    }

    /// <summary>
    /// The List of configurable colors
    /// </summary>
    public static List<ColorType> ConfigurableColorList => c_configColTypes;

    /// <summary>
    /// Reverts to the default color sets
    /// </summary>
    public static void ResetDefaultColorSets( )
    {
      _colors[ColorSet.BrightSet] = c_brightColors.Clone( ); // must use a copy
      _colors[ColorSet.DimmedSet] = c_dimColors.Clone( );
      _colors[ColorSet.InverseSet] = c_inverseColors.Clone( );
      UpdateRegisteredItems( );
    }

    /// <summary>
    /// Returns a Copy of a Default ColorSet
    /// </summary>
    /// <param name="colorSet"></param>
    /// <returns></returns>
    public static GUI_ColorSet GetDefaultColorSet( ColorSet colorSet )
    {
      if (colorSet == ColorSet.BrightSet) return c_brightColors.Clone( );
      if (colorSet == ColorSet.DimmedSet) return c_dimColors.Clone( );
      if (colorSet == ColorSet.InverseSet) return c_inverseColors.Clone( );
        return null;
    }

    /// <summary>
    /// Returns a Copy of a ColorSet
    /// </summary>
    /// <param name="colorSet"></param>
    /// <returns></returns>
    public static GUI_ColorSet GetColorSet( ColorSet colorSet )
    {
      if (_colors.TryGetValue( colorSet, out var colorCat )) {
        return colorCat.Clone( );
      }
      else {
        return null;
      }
    }

    /// <summary>
    /// Update the current Colors for a Set from the Catalog argument
    /// </summary>
    /// <param name="colorSet">ColorSet to update</param>
    /// <param name="colorCat">Update Values</param>
    public static void UpdateColorSet( ColorSet colorSet, GUI_ColorSet colorCat )
    {
      foreach (var c in ConfigurableColorList) {
        if (colorCat.TryGetValue( c, out var color )) {
          _colors[colorSet][c] = color;
        }
      }
    }

    // manage dynamic color updates for the registered Interfaces
    private static List<IColorType> m_registeredCtrls = new List<IColorType>( );

    /// <summary>
    /// Update the colors of the registered controls
    /// </summary>
    private static void UpdateRegisteredItems( )
    {
      foreach (var ci in m_registeredCtrls) {
        ci.UpdateColor( );
      }
    }

    // the selected Color Set
    private static ColorSet _currentColorSet = ColorSet.BrightSet;

    /// <summary>
    /// Get;Set the Color Set to use 
    /// </summary>
    public static ColorSet Colorset {
      get => _currentColorSet;
      set {
        _currentColorSet = value;
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
      if (Enum.IsDefined( typeof( ColorSet ), csValue )) {
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
      next = (next > (int)ColorSet.InverseSet) ? 0 : next; // rotate
      Colorset = (ColorSet)next;
    }

    /// <summary>
    /// Returns the current color of Type for the selected Color Set
    /// </summary>
    /// <param name="colorType">A ColorType</param>
    /// <returns>A Color</returns>
    public static Color ItemColor( ColorType colorType )
    {
      return _colors[Colorset][colorType];
    }

    /// <summary>
    /// Returns the current color of Type for the Color Set argument
    /// </summary>
    /// <param name="colorType">A ColorType</param>
    /// <returns>A Color</returns>
    public static Color ItemColor( ColorType colorType, GUI_ColorSet colorSet )
    {
      if ( colorSet.TryGetValue(colorType, out var color )) {
        return color;
      }
      return Color.Pink; // Error Color
    }

    /// <summary>
    /// Clear the registered Color Handler Interfaces
    /// </summary>
    public static void ClearRegistry( )
    {
      m_registeredCtrls.Clear( );
    }

    /// <summary>
    /// Register a Color Handler Interface for color set updates
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

    /// <summary>
    /// Unregister a Color Handler Interface
    /// </summary>
    /// <param name="ctrlInterface">The Control to un-register</param>
    public static void Unregister( IColorType ctrlInterface )
    {
      // don't bother with unknown registration
      try {
        m_registeredCtrls.Remove( ctrlInterface );
      }
      catch { }
    }

    /// <summary>
    /// Returns the Config String for a ColorSet (only configurable Elements)
    /// </summary>
    /// <param name="colorCat">The Color Catalog</param>
    /// <returns>A Config String</returns>
    public static string AsConfigString( GUI_ColorSet colorCat )
    {
      // format:
      // ColorTypeNumber¬R¬G¬B¦{ColorTypeNumber¬R¬G¬B¦}
      string cfg = "";

      foreach (var c in ConfigurableColorList) {
        if (colorCat.TryGetValue( c, out var color )) {
          cfg += $"{(int)c}¬{color.R}¬{color.G}¬{color.B}¦";
        }
        else {
          cfg += $"{(int)c},{Color.Pink.R},{Color.Pink.G},{Color.Pink.B}¦";
        }
      }
      return cfg;
    }


    // "EVal,R,G,B"
    private static Regex c_rxCol = new Regex( @"(?<num>\d+)¬(?<r>\d+)¬(?<g>\d+)¬(?<b>\d+)", RegexOptions.Compiled );

    /// <summary>
    /// Returns the ColorSet from a Config String (only configurable Elements)
    /// </summary>
    /// <param name="configString">A Config String</param>
    /// <returns>The Color Catalog</returns>
    public static GUI_ColorSet FromConfigString( string configString )
    {
      GUI_ColorSet colorCat = new GUI_ColorSet( );
      string[] e = configString.Split( new char[] { '¦' }, StringSplitOptions.RemoveEmptyEntries );
      foreach (var colItem in e) {
        Match match = c_rxCol.Match( colItem );
        if (match.Success) {
          int eVal = int.Parse( match.Groups["num"].Value );
          if (Enum.IsDefined( typeof( ColorType ), eVal )) {
            if (ConfigurableColorList.Contains( (ColorType)eVal )) {
              int r = int.Parse( match.Groups["r"].Value );
              int g = int.Parse( match.Groups["g"].Value );
              int b = int.Parse( match.Groups["b"].Value );
              colorCat.Add( (ColorType)eVal, Color.FromArgb( r, g, b ) );
            }
            else {
              ; // Color cannot be configured ??
            }
          }
          else {
            ; // Color Enum not known ??
          }
        }
        else {
          ; // format Error ??
        }
      }
      return colorCat;
    }

    // Legacy - Background items with static colors
    public static readonly Color c_WinBG = Color.FromArgb( 0, 1, 12 ); // Window background (slight blueish)
    public static readonly Color c_ActPressed = Color.Indigo;      // Actionable Items Background - pressed

    public static readonly Color c_FBCol = Color.PaleGreen;   // FlowBreak Background in Configuration
    public static readonly Color c_DB1Col = Color.LightSkyBlue;  // DivBreak Type 1 Background in Configuration
    public static readonly Color c_DB2Col = Color.Khaki;  // DivBreak Type 2 Background in Configuration
    public static readonly Color c_NBCol = Color.WhiteSmoke;  // NoneBreak Background in Configuration

  }
}

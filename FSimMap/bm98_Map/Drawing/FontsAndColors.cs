using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// Prefabs of Resource using Drawing Items 
  /// 
  /// --> NEVER Dispose them as they are widely used while the App is running
  /// </summary>
  internal static class FontsAndColors
  {
    //    public readonly static Brush BrushDebug = Brushes.DarkGray;
    public readonly static Brush BrushDebug = null;
    // **************

    public readonly static Color ColInfo = Color.FromArgb( 216, 225, 244 );    // whiteish
    public readonly static Color ColInfoDim = Color.FromArgb( 138, 145, 162 );
    public readonly static Color ColInfoDarker = Color.FromArgb( 22, 22, 22 );

    public readonly static Color ColRange = Color.DeepSkyBlue;
    public readonly static Color ColRangeTrue = Color.Blue;
    public readonly static Color ColTRange = Color.Lime; // Target Range color
    public readonly static Color ColScale = Color.Orchid;//Color.Magenta; //
    public readonly static Color ColRoute = Color.Magenta;
    public readonly static Color ColRouteAwy = Color.OrangeRed;  // Airways
    public readonly static Color ColRouteSid = Color.BlueViolet; // and SID & STAR
    public readonly static Color ColRouteApr = Color.DeepPink; // Approach
    public readonly static Color ColRouteMApr = Color.BlueViolet; // missed Apr

    public readonly static Color ColRwBorder = Color.FromArgb( 0, 166, 81 );    // green
    public readonly static Color ColRwPavement = Color.FromArgb( 45, 44, 44 );  // dark grey
    public readonly static Color ColRwBorderWater = Color.Aqua;    // bright Blue
    public readonly static Color ColRwPavementWater = Color.DarkBlue;  // dark blue
    public readonly static Color ColRwNumber = Color.FromArgb( 237, 28, 36 );  // red

    public readonly static Color ColNavAid = Color.Black; // Coral
    public readonly static Color ColNavAidWyp = Color.DarkMagenta;
    public readonly static Color ColNavAidApt = Color.DarkBlue;
    public readonly static Color ColAptRange = Color.RoyalBlue;
    public readonly static Color ColVfrMain = Color.RoyalBlue; // in and left hand color
    public readonly static Color ColVfrAlt = Color.Black; // out and right hand color
    public readonly static Color ColVfrHeading = Color.DarkBlue;

    public readonly static Color ColAcftOutline = Color.Black;
    public readonly static Color ColAcftWind = Color.MediumBlue;


    public readonly static Pen PenInfo = new Pen( ColInfo );
    public readonly static Pen PenInfo2 = new Pen( ColInfo, 2f );
    public readonly static Pen PenInfo4 = new Pen( ColInfo, 4f );

    public readonly static Pen PenRoute = new Pen( ColRoute, 3f );
    public readonly static Pen PenRouteAwy = new Pen( ColRouteAwy, 3f );
    public readonly static Pen PenRouteSid = new Pen( ColRouteSid, 3f );
    public readonly static Pen PenRouteApt = new Pen( ColRoute ) { Width = 2, DashPattern = new float[] { 5.0F, 10.0F, 2.0F, 10.0F } }; // to rwy/apt

    public readonly static Pen PenRouteApr = new Pen( ColRouteApr, 3f );
    public readonly static Pen PenRouteMApr = new Pen( ColRouteMApr){ Width = 3, DashPattern = new float[] { 5.0F, 10.0F, 5.0F, 10.0F } }; // missed apr

  public readonly static Pen PenRange3 = new Pen( ColRange, 3f );
    public readonly static Pen PenRange5 = new Pen( ColRangeTrue, 5f );
    public readonly static Pen PenTRange3 = new Pen( ColTRange, 3f );
    public readonly static Pen PenScale1 = new Pen( ColScale, 1f );
    public readonly static Pen PenScale2 = new Pen( ColScale, 2f );
    public readonly static Pen PenScale3 = new Pen( ColScale, 3f );
    public readonly static Pen PenRwNumber = new Pen( ColRwNumber );
    public readonly static Pen PenAptRange = new Pen( ColAptRange, 2f );
    public readonly static Pen PenHeadingLine = new Pen( ColAptRange, 3f );
    public readonly static Pen PenVfrNoDeco = new Pen( ColVfrMain ) { Width = 2, DashStyle = DashStyle.DashDot }; // dim
    public readonly static Pen PenVfrMain = new Pen( ColVfrMain, 3f );
    public readonly static Pen PenVfrAlt = new Pen( ColVfrAlt, 3f );

    public readonly static Pen PenAcftOutline = new Pen( ColAcftOutline, 1f );
    public readonly static Pen PenAcftWind = new Pen( ColAcftWind, 1f );


    public readonly static Brush BrushInfo = new SolidBrush( ColInfo );
    public readonly static Brush BrushInfoDim = new SolidBrush( ColInfoDim );
    public readonly static Brush BrushInfoDarker = new SolidBrush( ColInfoDarker );

    public readonly static Brush BrushScale = new SolidBrush( ColScale );

    public readonly static Brush BrushRwBorder = new SolidBrush( ColRwBorder );
    public readonly static Brush BrushRwPavement = new SolidBrush( ColRwPavement );
    public readonly static Brush BrushRwBorderWater = new SolidBrush( ColRwBorderWater );
    public readonly static Brush BrushRwPavementWater = new SolidBrush( ColRwPavementWater );
    public readonly static Brush BrushRwNumber = new SolidBrush( ColRwNumber );

    public readonly static Brush BrushNavAid = new SolidBrush( ColNavAid );
    public readonly static Brush BrushNavAidWyp = new SolidBrush( ColNavAidWyp );
    public readonly static Brush BrushNavAidApt = new SolidBrush( ColNavAidApt );

    public readonly static Brush BrushAptRange = new SolidBrush( ColAptRange );

    public readonly static Brush BrushVFRHeading = new SolidBrush( ColVfrHeading );

    public readonly static Brush BrushAcftWind = new SolidBrush( ColAcftWind );

    // fonts for numbers
    //    private readonly static string TextFont = "Bahnschrift"; // has equaly spaced digits
    private readonly static string TextFont = "Consolas";

    public readonly static Font FtLarger = new Font( TextFont, 24f, FontStyle.Bold );
    public readonly static Font FtLarge = new Font( TextFont, 18f, FontStyle.Bold );
    public readonly static Font FtMid = new Font( TextFont, 16f, FontStyle.Bold );
    public readonly static Font FtSmall = new Font( TextFont, 14f, FontStyle.Regular );
    public readonly static Font FtSmaller = new Font( TextFont, 10f, FontStyle.Regular );
    public readonly static Font FtSmallest = new Font( TextFont, 8f, FontStyle.Regular );

  }
}

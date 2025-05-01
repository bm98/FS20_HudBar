using System.Drawing;
using System.Drawing.Drawing2D;

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

    // generic
    public readonly static Color ColInfo = Color.FromArgb( 216, 225, 244 );    // whiteish
    public readonly static Pen PenInfo = new Pen( ColInfo );
    public readonly static Pen PenInfo2 = new Pen( ColInfo, 2f );
    public readonly static Pen PenInfo4 = new Pen( ColInfo, 4f );
    public readonly static Brush BrushInfo = new SolidBrush( ColInfo );

    public readonly static Color ColInfoDim = Color.FromArgb( 138, 145, 162 );
    public readonly static Brush BrushInfoDim = new SolidBrush( ColInfoDim );

    public readonly static Color ColInfoDarker = Color.FromArgb( 22, 22, 22 );
    public readonly static Brush BrushInfoDarker = new SolidBrush( ColInfoDarker );

    // runways
    public readonly static Color ColRwNumber = Color.FromArgb( 237, 28, 36 );  // red
    public readonly static Pen PenRwNumber = new Pen( ColRwNumber );
    public readonly static Brush BrushRwNumber = new SolidBrush( ColRwNumber );

    public readonly static Color ColRwBorder = Color.FromArgb( 0, 166, 81 );    // green
    public readonly static Brush BrushRwBorder = new SolidBrush( ColRwBorder );
    public readonly static Color ColHpBorder = Color.FromArgb( 81, 81, 0 );    // yellow
    public readonly static Brush BrushHpBorder = new SolidBrush( ColHpBorder );

    public readonly static Color ColRwPavement = Color.FromArgb( 45, 44, 44 );  // dark grey
    public readonly static Brush BrushRwPavement = new SolidBrush( ColRwPavement );
    public readonly static Color ColHpPavement = Color.FromArgb( 145, 144, 144 );  // light grey
    public readonly static Brush BrushHpPavement = new SolidBrush( ColHpPavement );

    public readonly static Color ColRwBorderWater = Color.Aqua;    // bright Blue
    public readonly static Brush BrushRwBorderWater = new SolidBrush( ColRwBorderWater );

    public readonly static Color ColRwPavementWater = Color.DarkBlue;  // dark blue
    public readonly static Brush BrushRwPavementWater = new SolidBrush( ColRwPavementWater );

    // airport VFR marks
    public readonly static Color ColVfrMain = Color.RoyalBlue; // in and left hand color
    public readonly static Pen PenVfrNoDeco = new Pen( ColVfrMain ) { Width = 2, DashStyle = DashStyle.DashDot }; // dim
    public readonly static Pen PenVfrMain = new Pen( ColVfrMain, 3f );

    public readonly static Color ColVfrAlt = Color.Black; // out and right hand color
    public readonly static Pen PenVfrAlt = new Pen( ColVfrAlt, 3f );

    public readonly static Color ColVfrHeading = Color.DarkBlue;
    public readonly static Brush BrushVFRHeading = new SolidBrush( ColVfrHeading );


    // range rings
    public readonly static Color ColAptRange = Color.RoyalBlue;
    public readonly static Pen PenAptRange = new Pen( ColAptRange, 2f );
    public readonly static Pen PenHeadingLine = new Pen( ColAptRange, 3f );
    public readonly static Brush BrushAptRange = new SolidBrush( ColAptRange );

    // user aircraft
    public readonly static Color ColAcftOutline = Color.Black;
    public readonly static Pen PenAcftOutline = new Pen( ColAcftOutline, 1f );

    public readonly static Color ColAcftWind = Color.MediumBlue;
    public readonly static Pen PenAcftWind = new Pen( ColAcftWind, 1f );
    public readonly static Brush BrushAcftWind = new SolidBrush( ColAcftWind );

    // user aircraft RADAR
    public readonly static Color ColAcftOutlineRdr = Color.Green;
    public readonly static Pen PenAcftOutlineRdr = new Pen( ColAcftOutlineRdr, 1f );

    public readonly static Color ColAcftWindRdr = Color.PowderBlue;
    public readonly static Pen PenAcftWindRdr = new Pen( ColAcftWindRdr, 1f );
    public readonly static Brush BrushAcftWindRdr = new SolidBrush( ColAcftWindRdr );


    // user acft target range
    public readonly static Color ColRange = Color.DeepSkyBlue;
    public readonly static Pen PenRange3 = new Pen( ColRange, 3f );

    public readonly static Color ColRangeTrue = Color.Blue;
    public readonly static Pen PenRange5 = new Pen( ColRangeTrue, 5f );

    public readonly static Color ColTRange = Color.Lime; // Target Range color
    public readonly static Pen PenTRange3 = new Pen( ColTRange, 3f );

    // AI aircraft MAP
    public readonly static Color ColAcftAiOutline = Color.FromArgb( 0, 0, 96 ); // dark blue
    public readonly static Pen PenAcftAiOutline = new Pen( ColAcftAiOutline, 2f );

    public readonly static Color ColAcftAiText = Color.FromArgb( 0, 0, 96 );// dark blue
    public readonly static Brush BrushAcftAiText = new SolidBrush( ColAcftAiText );

    public readonly static Color ColAcftAiTextBG = Color.FromArgb( 96, 255, 255, 255 ); // some transparency to make it visible on dark BG
    public readonly static Brush BrushAcftAiTextBG = new SolidBrush( ColAcftAiTextBG );

    // AI aircraft RADAR
    public readonly static Color ColAcftAiOutlineRdr = Color.LimeGreen;
    public readonly static Pen PenAcftAiOutlineRdr = new Pen( ColAcftAiOutlineRdr, 2f );

    public readonly static Color ColAcftAiTextRdr = Color.LimeGreen;
    public readonly static Brush BrushAcftAiTextRdr = new SolidBrush( ColAcftAiTextRdr );

    public readonly static Color ColAcftAiTextBGRdr = Color.FromArgb( 96, 255, 255, 255 ); // some transparency to make it visible on dark BG
    public readonly static Brush BrushAcftAiTextBGRdr = null; // don't use new SolidBrush( ColAcftAiTextBGRdr );


    // navaids
    public readonly static Color ColNavAid = Color.Black; // Coral
    public readonly static Brush BrushNavAid = new SolidBrush( ColNavAid );

    public readonly static Color ColNavAidWyp = Color.DarkMagenta;
    public readonly static Brush BrushNavAidWyp = new SolidBrush( ColNavAidWyp );

    public readonly static Color ColNavAidApt = Color.DarkBlue;
    public readonly static Brush BrushNavAidApt = new SolidBrush( ColNavAidApt );

    // navaids RADAR
    public readonly static Color ColNavAidRdr = Color.LightSteelBlue;
    public readonly static Brush BrushNavAidRdr = new SolidBrush( ColNavAidRdr );

    public readonly static Color ColNavAidWypRdr = Color.MediumOrchid;
    public readonly static Brush BrushNavAidWypRdr = new SolidBrush( ColNavAidWypRdr );

    public readonly static Color ColNavAidAptRdr = Color.LightSkyBlue;
    public readonly static Brush BrushNavAidAptRdr = new SolidBrush( ColNavAidAptRdr );

    // routes
    public readonly static Color ColRoute = Color.Magenta;
    public readonly static Pen PenRoute = new Pen( ColRoute, 3f );
    public readonly static Pen PenRouteApt = new Pen( ColRoute ) { Width = 2, DashPattern = new float[] { 5.0F, 10.0F, 2.0F, 10.0F } }; // to rwy/apt

    public readonly static Color ColRouteAwy = Color.OrangeRed;  // Airways
    public readonly static Pen PenRouteAwy = new Pen( ColRouteAwy, 3f );

    public readonly static Color ColRouteSid = Color.BlueViolet; // and SID & STAR
    public readonly static Pen PenRouteSid = new Pen( ColRouteSid, 3f );

    public readonly static Color ColRouteApr = Color.DeepPink; // Approach
    public readonly static Pen PenRouteApr = new Pen( ColRouteApr, 3f );

    public readonly static Color ColRouteMApr = Color.BlueViolet; // missed Apr
    public readonly static Pen PenRouteMApr = new Pen( ColRouteMApr ) { Width = 3, DashPattern = new float[] { 5.0F, 10.0F, 5.0F, 10.0F } }; // missed apr

    // scale and grid
    public readonly static Color ColScale = Color.Orchid;//Color.Magenta; //
    public readonly static Pen PenScale1 = new Pen( ColScale, 1f );
    public readonly static Pen PenScale2 = new Pen( ColScale, 2f );
    public readonly static Pen PenScale3 = new Pen( ColScale, 3f );
    public readonly static Brush BrushScale = new SolidBrush( ColScale );

    // compass rose  RADAR
    public readonly static Color ColRoseRdr = Color.LightSlateGray;
    public readonly static Pen PenRoseRdr = new Pen( ColRoseRdr, 0.5f );
    public readonly static Brush BrushRoseRdr = new SolidBrush( ColRoseRdr );


    // fonts for numbers
    //    private readonly static string TextFont = "Bahnschrift"; // has equaly spaced digits
    private readonly static string TextFont = "Consolas";

    public readonly static Font FtLargest = new Font( TextFont, 32f, FontStyle.Bold );
    public readonly static Font FtLarger = new Font( TextFont, 24f, FontStyle.Bold );
    public readonly static Font FtLarge = new Font( TextFont, 18f, FontStyle.Bold );
    public readonly static Font FtMid = new Font( TextFont, 16f, FontStyle.Bold );
    public readonly static Font FtSmall = new Font( TextFont, 14f, FontStyle.Regular );
    public readonly static Font FtSmaller = new Font( TextFont, 10f, FontStyle.Regular );
    public readonly static Font FtSmallest = new Font( TextFont, 8f, FontStyle.Regular );

  }
}

using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;

namespace bm98_VProfile
{
  internal static class FontsAndColors
  {
    //    public readonly static Brush BrushDebug = Brushes.DarkGray;
    public readonly static Brush BrushDebug = null;
    // **************

    public readonly static Color ColAlarm = Color.FromArgb(237,28,36);  // red
    public readonly static Color ColWarn = Color.FromArgb(255,242,0);   // yellow
    public readonly static Color ColGps = Color.FromArgb(236,0,140);    // magenta
    public readonly static Color ColGpsDim = Color.FromArgb(170,0,87);
    public readonly static Color ColNav = Color.FromArgb(0,166,81);     // green
    public readonly static Color ColNavDim = Color.FromArgb(0,100,20);
    public readonly static Color ColRef = Color.FromArgb(0,174,239);    // cyan
    public readonly static Color ColRefDim = Color.FromArgb(0,94,153);
    public readonly static Color ColInfo = Color.FromArgb(216,225,244);    // whiteish
    public readonly static Color ColInfoDim = Color.FromArgb(138,145,162);
    public readonly static Color ColInfoDark = Color.FromArgb(45,44,44);
    public readonly static Color ColInfoDarker = Color.FromArgb(22,22,22);
    public readonly static Color ColAP = Color.FromArgb(26,183,0);     // green brighter
    public readonly static Color ColAParmed = Color.FromArgb(144,153,143);  // whiteish
    public readonly static Color ColTransp = Color.FromArgb(0,1,1,1);  // transparent
    public readonly static Color ColSimrate = Color.Yellow; 

    public readonly static Pen PenInfo = new Pen(ColInfo);
    public readonly static Pen PenInfo2 = new Pen( ColInfo, 2f );
    public readonly static Pen PenInfo4 = new Pen(ColInfo, 4f);

    public readonly static Pen PenNav2 = new Pen( ColNav, 2f );

    public readonly static Brush BrushAlarm = new SolidBrush(ColAlarm);
    public readonly static Brush BrushWarn = new SolidBrush(ColWarn);
    public readonly static Brush BrushGps = new SolidBrush(ColGps);
    public readonly static Brush BrushGpsDim = new SolidBrush(ColGpsDim);
    public readonly static Brush BrushNav = new SolidBrush(ColNav);
    public readonly static Brush BrushNavDim = new SolidBrush(ColNavDim);
    public readonly static Brush BrushRef = new SolidBrush(ColRef);
    public readonly static Brush BrushRefDim = new SolidBrush(ColRefDim);
    public readonly static Brush BrushInfo = new SolidBrush(ColInfo);
    public readonly static Brush BrushInfoDim = new SolidBrush(ColInfoDim);
    public readonly static Brush BrushInfoDark = new SolidBrush(ColInfoDark);
    public readonly static Brush BrushInfoDarker = new SolidBrush(ColInfoDarker);
    public readonly static Brush BrushAP = new SolidBrush(ColAP);
    public readonly static Brush BrushAParmed = new SolidBrush(ColAParmed);
    public readonly static Brush BrushTransp = new SolidBrush(ColTransp);
    public readonly static Brush BrushSimrate =new SolidBrush(ColSimrate);


    //private readonly static string TapeFont = "Share Tech Mono";
    private readonly static string TapeFont = "Arial";

    public readonly static Font FtTape = new Font( TapeFont, 18f );                // Tape label numbers
    public readonly static Font FtTapeSmall = new Font( TapeFont, 16f );           // Tape label numbers small

    //private readonly static string TextFont = "Lucida Sans";
    private readonly static string TextFont = "Consolas";

    public readonly static Font FtLarger = new Font( TextFont, 24f, FontStyle.Bold );   // For numbers, sources large
    public readonly static Font FtLarge = new Font( TextFont, 20f, FontStyle.Bold );   // For numbers, sources large
    public readonly static Font FtMid = new Font( TextFont, 18f, FontStyle.Bold );     // For numbers, sources mid
    public readonly static Font FtSmall = new Font( TextFont, 14f );                   // For numbers, sources small

    public readonly static Font FtNumLarger;              // For numbers
    public readonly static Font FtNumLarge;              // For numbers
    public readonly static Font FtNumMid;              // For numbers
    public readonly static Font FtNumSmall;              // For numbers
    public readonly static Font FtNumSmaller;              // For numbers

    // Store embedded Fonts here
    private static PrivateFontCollection m_privateFonts = new PrivateFontCollection( );
    private static FontFamily m_numCondensed;
    private static FontFamily m_numCompressed;

    static FontsAndColors( )
    {
      try {
        //  Font embedding Ref: https://web.archive.org/web/20141224204810/http://bobpowell.net/embedfonts.aspx
        // Load a rather condensed embedded font 
        Stream fontStream =  Assembly.GetExecutingAssembly().GetManifestResourceStream(@"bm98_VProfile.Fonts.florencesans-sc.cond.ttf");
        byte[] fontdata = new byte[fontStream.Length];
        fontStream.Read( fontdata, 0, (int)fontStream.Length );
        fontStream.Close( );
        unsafe {
          fixed ( byte* pFontData = fontdata ) {
            m_privateFonts.AddMemoryFont( (IntPtr)pFontData, fontdata.Length );
          }
        }

        fontStream = Assembly.GetExecutingAssembly( ).GetManifestResourceStream( @"bm98_VProfile.Fonts.florencesans-sc.comp-bold.ttf" );
        fontdata = new byte[fontStream.Length];
        fontStream.Read( fontdata, 0, (int)fontStream.Length );
        fontStream.Close( );
        unsafe {
          fixed ( byte* pFontData = fontdata ) {
            m_privateFonts.AddMemoryFont( (IntPtr)pFontData, fontdata.Length );
          }
        }
        // set new font if we were successful
        m_numCompressed = m_privateFonts.Families[0]; // last added
        m_numCondensed = m_privateFonts.Families[1];

        FtNumLarger = new Font( m_numCondensed, 24f, FontStyle.Bold );              // For numbers
        FtNumLarge = new Font( m_numCondensed, 20f, FontStyle.Bold );              // For numbers
        FtNumMid = new Font( m_numCondensed, 17f, FontStyle.Bold );              // For numbers
        FtNumSmall = new Font( m_numCondensed, 14f );
        FtNumSmaller = new Font( m_numCompressed, 16f );
      }
      catch {
        ; // DEBUG STOP ONLY
      }

    }
  }
}

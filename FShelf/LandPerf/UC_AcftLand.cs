using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FSimClientIF.Modules;


namespace FShelf.LandPerf
{
  public partial class UC_AcftLand : UserControl
  {
    private StringFormat _stringFormat { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
    private StringFormat _stringFormatText { get; set; } = new StringFormat( ) { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Center };

    private PerfTracker.PerfData _lPerf = new PerfTracker.PerfData( ) {
      TdCount = 1,
      LandingPerf = new LandingPerfData(
        3600, // zulu sec
        220f, // vs
        2.8f, -0.8f, 0.7f, //pby
        260f, 1.23f, // hdg, G
        259f, 262f, 72f,  // gtrk, gtrkMag, gs
        74f, -0.5f,  // ias, slip
        330f, 5f, //wind dir,speed
        float.NaN, float.NaN,
        "D_CALL",
        "SR22T Working Title"
      ),
      AirportIdent = "LSZF",
      RunwayIdent = "RW26",
      RunwayBearing_deg = 260,
      RunwayLength_m = 718,
      RunwayWidth_m = 20,
      TdDisplacement_m = 1,
      TdDistance_m = 133.9f
    };


    /// <summary>
    /// Acft Landing sketch
    /// </summary>
    public UC_AcftLand( )
    {
      InitializeComponent( );
    }

    private void UC_AcftLand_Load( object sender, EventArgs e )
    {

    }

    // paint the item 
    private void UC_AcftLand_Paint( object sender, PaintEventArgs e )
    {

      var li = new LandingImage( _lPerf, e.ClipRectangle.Width, e.ClipRectangle.Height );
      Image image = li.AsImage( );

      //create our own custom brush to fill the background with the image
      using (TextureBrush iBrush = new TextureBrush( new Bitmap( image ) )) {
        e.Graphics.FillRectangle( iBrush, new Rectangle( new Point( 0, 0 ), image.Size ) );
      }
      image?.Dispose( );

#if DEBUG
      li = new LandingImage( _lPerf, 0, 0 ); // native
      image = li.AsImage( );
      image.Save( @".\LandingImage.jpg", ImageFormat.Jpeg );
      image?.Dispose( );
#endif

    }

    /// <summary>
    /// Set a Landing performance obj to draw it
    /// </summary>
    /// <param name="lPerf">A Landing Performance record</param>
    public void DrawLanding( PerfTracker.PerfData lPerf )
    {
      _lPerf = lPerf;
      this.Refresh( );
    }

  }
}

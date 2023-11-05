using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using dNetBm98;

namespace FShelf.Energy
{
  /// <summary>
  /// Display an Energy Table from a set of 3
  /// Add the current E-State derived from AltMSL and TAS
  /// 
  /// </summary>
  public partial class UC_ETable : UserControl
  {
    // current values
    private float _prev_tas_kt = 0;
    private float _prev_altMsl_ft = 0;
    private double _prev_t_sec = 0;

    // current values
    private float _tas_kt = 0;
    private float _altMsl_ft = 0;
    private double _t_sec = 0;

    // derivatives
    private float _accel_ktPsec = 0;
    private float _vs_ftPsec = 0;

    // estimates (in 6 sec)
    private float _est_t_sec = 6;
    private float _est_tas_kt = 0;
    private float _est_altMsl_ft = 0;

    // limits
    private int _stallSpeed_kt = 0;
    private float _stallSpeedTAS_kt = 0;
    private int _altGround_ft = 0;

    private object _calcLock = new object( );

    // temp drawing area
    private Bitmap _drawImage = null;

    // current scale (image layout)
    private int _scaleImage = 0; // we have 3 of them

    // Set the scale from the current values
    private void SelectScale( )
    {
      int nextScale = _scaleImage;

      if (_scaleImage == 0) {
        // 0..250kt ; 0..10'000ft
        // going Upwards
        if ((_tas_kt > 240f) || (_altMsl_ft > 10_000f)) {
          nextScale = 1;
        }
      }
      else if (_scaleImage == 1) {
        // 0..450kt ; 0..30'000ft
        // going Upwards
        if (_altMsl_ft > 28_000f) {
          nextScale = 2;
        }
        // going Downwards
        else if ((_altMsl_ft < 10_000f) && (_tas_kt < 240f)) {
          nextScale = 0;
        }
      }
      else {
        // 0..450kt ; 20'000..50'000ft
        // going Downwards
        if (_altMsl_ft < 26_000f) {
          nextScale = 1;
        }
      }
      // switch the bg image
      if (_scaleImage != nextScale) {
        _scaleImage = nextScale;
      }
    }

    // do all calculations 
    private void CalculateValues( )
    {
      double dT = 0; float dTas = 0; float dAlt = 0;
      lock (_calcLock) {
        dT = _t_sec - _prev_t_sec;
        dTas = _tas_kt - _prev_tas_kt;
        dAlt = _altMsl_ft - _prev_altMsl_ft;
      }

      _accel_ktPsec = 0;
      _vs_ftPsec = 0;
      if (dT > 0) {
        _accel_ktPsec = (float)XMath.Clip( dTas / dT, -30, 30 ); // 
        _vs_ftPsec = (float)XMath.Clip( dAlt / dT, -150, 150 ); // +- 9000fpm should be enough...
      }

      // Calc Estimates for a defined future
      _est_tas_kt = _tas_kt + (_est_t_sec * _accel_ktPsec);
      _est_altMsl_ft = _altMsl_ft + (_est_t_sec * _vs_ftPsec);

      // recalc stall speed at this height from IAS (treated as CAS)
      _stallSpeedTAS_kt = (float)Units.TAS_From_CAS( _stallSpeed_kt, _altMsl_ft );
    }

    // update the input vars
    private void UpdateValues( float tas_kt, float altMsl_ft, double sim_time_sec )
    {
      lock (_calcLock) {
        // carry prev
        _prev_t_sec = _t_sec;
        _prev_tas_kt = _tas_kt;
        _prev_altMsl_ft = _altMsl_ft;
        // set current
        _t_sec = sim_time_sec;
        _tas_kt = tas_kt;
        _altMsl_ft = altMsl_ft;
      }
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_ETable( )
    {
      InitializeComponent( );

      this.ResizeRedraw = true;
      this.BackgroundImage = null;


      _scaleImage = 0;
      _drawImage = new Bitmap( Properties.Resources.e_table_00 );
    }

    // UC load event
    private void UC_ETable_Load( object sender, EventArgs e )
    {
    }

    /// <summary>
    /// Get;Set: Stall Speed kt (in IAS)
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Stall Speed IAS kt" ), Category( "Data" )]
    public float StallSpeed_kt {
      get => _stallSpeed_kt;
      set {
        int sspeed = (int)Math.Round( value );
        if (sspeed == _stallSpeed_kt) return; // already there..

        _stallSpeed_kt = sspeed;
        this.Refresh( );
      }
    }

    /// <summary>
    /// Get;Set: Ground Elevation kt
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Ground Elevation ft" ), Category( "Data" )]
    public float GroundElevation_ft {
      get => _altGround_ft;
      set {
        int agnd = (int)Math.Round( value );
        if (agnd == _altGround_ft) return; // already there..

        _altGround_ft = agnd;
        this.Refresh( );
      }
    }

    /// <summary>
    /// Get;Set: TAS kt
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "TAS kt" ), Category( "Data" )]
    public float TAS_kt {
      get => _tas_kt;
      set {
        UpdateValues( value, _altMsl_ft, _t_sec + 1 ); // fake calc for testing
        SelectScale( );
        this.Refresh( );
      }
    }
    /// <summary>
    /// Get;Set: ALT ft msl
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "ALT ft msl" ), Category( "Data" )]
    public float ALTmsl_ft {
      get => _altMsl_ft;
      set {
        UpdateValues( _tas_kt, value, _t_sec + 1 ); // fake calc for testing
        SelectScale( );
        this.Refresh( );
      }
    }

    /// <summary>
    /// Get;Set: Est time 1..60 [sec]
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Estimate time [sec]" ), Category( "Data" )]
    public float Est_Time_s {
      get => _est_t_sec;
      set {
        if (value < 1) return; // sanity
        if (value > 60) return; // sanity

        _est_t_sec = value;
        SelectScale( );
        this.Refresh( );
      }
    }

    /// <summary>
    /// Set the TAS and ALT for the aircrafts E Display
    /// </summary>
    /// <param name="tas_kt">TAS [kt]</param>
    /// <param name="altMsl_ft">ALT msl [ft]</param>
    /// <param name="sim_time_sec">Sim Time [sec]</param>
    public void SetValues( float tas_kt, float altMsl_ft, double sim_time_sec )
    {
      UpdateValues( tas_kt, altMsl_ft, sim_time_sec );
      SelectScale( );
      this.Refresh( );
    }

    // Table Image 
    // Size    = 580x615
    // Loc 0   = ( 58/583)
    // Loc max = (556/ 14)

    // Native diagram dimensions
    private const float dWidth = 556f - 58f;
    private const float dHeight = 583f - 14f;
    // Translate to move the 0 Pt to bottom left
    private const float shiftX = -58;
    private const float shiftY = -14;
    // Scale to make TAS matching original Pixels
    // scaleImage==0:  0..250kt ; 0..10'000ft
    // scaleImage==1:  0..450kt ; 0..30'000ft
    // scaleImage==2:  0..450kt ; 20'000..50'000ft
    private readonly float[] scaleX = new float[] { dWidth / 250f, dWidth / 450f, dWidth / 600f };
    private readonly float[] scaleY = new float[] { dHeight / 10_000f, dHeight / 30_000f, dHeight / 30_000f };
    private readonly float[] offsX = new float[] { 0f, 0f, 0f };
    private readonly float[] offsY = new float[] { 0f, 0f, 20_000f };
    private readonly float[] maxY = new float[] { 10_000f, 30_000f, 30_000f };

    // UC Paint Event
    private void UC_ETable_Paint( object sender, PaintEventArgs e )
    {
      // calc if we need to draw
      CalculateValues( );

      Graphics dg = Graphics.FromImage( _drawImage );
      var state = dg.Save( );
      dg.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
      switch (_scaleImage) {
        case 0:
          dg.DrawImage( Properties.Resources.e_table_00,
          new Rectangle( new Point( 0, 0 ), _drawImage.Size ),
          new Rectangle( new Point( 0, 0 ), _drawImage.Size ), GraphicsUnit.Pixel );
          break;
        case 1:
          dg.DrawImage( Properties.Resources.e_table_01,
          new Rectangle( new Point( 0, 0 ), _drawImage.Size ),
          new Rectangle( new Point( 0, 0 ), _drawImage.Size ), GraphicsUnit.Pixel );
          break;
        case 2:
          dg.DrawImage( Properties.Resources.e_table_02,
          new Rectangle( new Point( 0, 0 ), _drawImage.Size ),
          new Rectangle( new Point( 0, 0 ), _drawImage.Size ), GraphicsUnit.Pixel );
          break;
        default:
          dg.DrawImage( Properties.Resources.e_table_01,
         new Rectangle( new Point( 0, 0 ), _drawImage.Size ),
         new Rectangle( new Point( 0, 0 ), _drawImage.Size ), GraphicsUnit.Pixel );
          break;
      }
      dg.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;

      // Move the table image to have the table Y axis Max at drawing Zero
      // X is 0..max of used table TAS
      // Y is max..min of used table ALT

      dg.TranslateTransform( -shiftX, -shiftY );

      // draw limits

      // Stall line
      float x = (_stallSpeedTAS_kt - offsX[_scaleImage]) * scaleX[_scaleImage];
      dg.DrawLine( Pens.Red, x, 0, x, dHeight + 10 );

      // Ground Line
      // use inverted direction for Y
      float y = (maxY[_scaleImage] - (_altGround_ft - offsY[_scaleImage])) * scaleY[_scaleImage];
      dg.DrawLine( Pens.Brown, -10, y, dWidth, y );
      // cross
      //      dg.DrawLine( Pens.Brown, x - 10, y, x + 10, y );
      //      dg.DrawLine( Pens.Red, x, y - 10, x, y + 10 );


      // draw estimates before actual (maintain actual over estimate)
      x = (_est_tas_kt - offsX[_scaleImage]) * scaleX[_scaleImage];
      y = (maxY[_scaleImage] - (_est_altMsl_ft - offsY[_scaleImage])) * scaleY[_scaleImage];
      dg.TranslateTransform( x, y, System.Drawing.Drawing2D.MatrixOrder.Append );

      dg.FillEllipse( Brushes.Magenta, -6, -6, 12, 12 );
      //dg.DrawEllipse( Pens.Magenta, -9, -9, 18, 18 );

      dg.TranslateTransform( -x, -y, System.Drawing.Drawing2D.MatrixOrder.Append );

      // draw actual
      x = (_tas_kt - offsX[_scaleImage]) * scaleX[_scaleImage];
      y = (maxY[_scaleImage] - (_altMsl_ft - offsY[_scaleImage])) * scaleY[_scaleImage];
      dg.TranslateTransform( x, y, System.Drawing.Drawing2D.MatrixOrder.Append );

      dg.FillEllipse( Brushes.ForestGreen, -9, -9, 18, 18 );
      dg.FillEllipse( Brushes.Chartreuse, -3, -3, 6, 6 );
      //      dg.DrawEllipse( Pens.Lime, -9, -9, 18, 18 );

      // XY Guides on border
      dg.TranslateTransform( -x, -y, System.Drawing.Drawing2D.MatrixOrder.Append );
      dg.DrawLine( Pens.ForestGreen, x, dHeight, x, dHeight + 20 );
      dg.DrawLine( Pens.ForestGreen, -30, y, 0, y );

      //dg.Restore( state );
      // finish
      dg.Dispose( );

      // draw the original table scaled onto the canvas
      Graphics g = e.Graphics;
      g.DrawImage( _drawImage, this.ClientRectangle, new Rectangle( new Point( 0, 0 ), _drawImage.Size ), GraphicsUnit.Pixel );


    }
  }
}

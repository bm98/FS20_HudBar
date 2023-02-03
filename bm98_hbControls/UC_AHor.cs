using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bm98_hbControls
{
  /// <summary>
  /// User Control: Artificial Horizont (part of ESI)  
  /// </summary>
  public partial class UC_AHor : UserControl
  {
    private readonly Size _minSize = new Size( 20, 10);
    private readonly float _aspect = 2; // Height*_aspect => Width

    /// <summary>
    /// Properties
    /// </summary>
    private float _pPitch = 0;
    private float _pBank = 0;

    private float _pMin = -100;
    private float _pMax = 100;
    private bool _pAutoSizeHeight = false;
    private bool _pAutoSizeWidth = false;

    private const float c_penSize = 2;

    private Pen _pen = new Pen(Color.Green, c_penSize);
    private Size _border = new Size( -2, 0); // internal border for the scale drawn rectangle
    float _uScale = 1;

    private PointF _drawCenter = new PointF(0,0);

    /// <summary>
    /// Pitch Angle
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Pitch Angle" ), Category( "Data" )]
    public float PitchAngle {
      get => _pPitch;
      set {
        _pPitch = value;
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Bank Angle
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Bank Angle" ), Category( "Data" )]
    public float BankAngle {
      get => _pBank;
      set {
        _pBank = value;
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Minimum Scale Point
    /// </summary>
    [DefaultValue( -100 )]
    [Description( "Minimum Scale Point" ), Category( "Data" )]
    public float Minimum {
      get => _pMin;
      set {
        if ( value >= _pMax ) throw new ArgumentException( "Property value is invalid" );
        _pMin = value;
        RecalcPrimitives( );
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Maximum Scale Point
    /// </summary>
    [DefaultValue( 100 )]
    [Description( "Maximum Scale Point" ), Category( "Data" )]
    public float Maximum {
      get => _pMax;
      set {
        if ( value <= _pMin ) throw new ArgumentException( "Property value is invalid" );
        _pMax = value;
        RecalcPrimitives( );
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// AutoSize the Height of the Control
    /// </summary>
    [DefaultValue( false )]
    [Description( "AutoSize the Height of the Control" ), Category( "Layout" )]
    public bool AutoSizeHeight {
      get => _pAutoSizeHeight;
      set {
        _pAutoSizeHeight = value;
        UC_AHor_Resize( this, null );
      }
    }

    /// <summary>
    /// AutoSize the Width of the Control
    /// </summary>
    [DefaultValue( false )]
    [Description( "AutoSize the Width of the Control" ), Category( "Layout" )]
    public bool AutoSizeWidth {
      get => _pAutoSizeWidth;
      set {
        _pAutoSizeWidth = value;
        UC_AHor_Resize( this, null );
      }
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_AHor( )
    {
      InitializeComponent( );
      // calc desired aspect from our _minSize Const
      _aspect = _minSize.Width / _minSize.Height; // Height*_aspect => Width
      RecalcPrimitives( );
      // get setting colors
      _pen?.Dispose( );
      _pen = new Pen( this.ForeColor, c_penSize );
    }

    /// <summary>
    /// Get;Set; Foreground Color
    /// </summary>
    public override Color ForeColor {
      get => base.ForeColor;
      set {
        if ( value == base.ForeColor ) return; // already set
        base.ForeColor = value;
        _pen?.Dispose( );
        _pen = new Pen( value, c_penSize );
        this.Invalidate( this.ClientRectangle );
      }
    }

    private void UC_AHor_Paint( object sender, PaintEventArgs e )
    {
      if ( _pen == null ) return;
      if ( !this.Enabled ) return;

      var g = e.Graphics;

      var saved = g.Save();
      g.SmoothingMode = SmoothingMode.HighQuality;

      var rect = this.ClientRectangle;
      rect.Inflate( _border );

      float pitch = -PitchAngle; // The Pitch is Inversed for scale movement
      var XC = rect.Width/2;
      var YC = rect.Height/2-1;

      // Set world transform of graphics object to translate.
      g.TranslateTransform( -XC, -YC );
      g.RotateTransform( BankAngle, MatrixOrder.Append ); // turns along its master BANK Angle
      g.TranslateTransform( rect.X + XC, rect.Y + YC, MatrixOrder.Append );

      // Background Pitch Shift
      g.TranslateTransform( 0, pitch * _uScale );

      Brush gradSky = new LinearGradientBrush(new Rectangle( 0, YC, rect.Width, -rect.Height * 5), Color.RoyalBlue, Color.Navy,  LinearGradientMode.Vertical);
      Brush gradGround = new LinearGradientBrush(new Rectangle( 0, YC, rect.Width, rect.Height * 5), Color.Peru, Color.Maroon,  LinearGradientMode.Vertical);

      g.FillRectangle( gradSky, -rect.Width * 2, -rect.Height * 4, rect.Width * 5, rect.Height * 4.5f );
      g.FillRectangle( gradGround, -rect.Width * 2, YC, rect.Width * 5, rect.Height * 4.5f );
      // scale lines
      var lx = XC - rect.Width / 5 / _aspect;
      var rx = XC + rect.Width / 5 / _aspect;
      g.DrawLine( Pens.LightPink, lx, YC - rect.Height, rx, YC - rect.Height );
      g.DrawLine( Pens.LightPink, lx + 5, YC - rect.Height * 3 / 4, rx - 5, YC - rect.Height * 3 / 4 );
      g.DrawLine( Pens.White, lx, YC - rect.Height / 2, rx, YC - rect.Height / 2 );
      g.DrawLine( Pens.White, lx + 5, YC - rect.Height / 4, rx - 5, YC - rect.Height / 4 );

      g.DrawLine( Pens.White, lx, YC, rx, YC );

      g.DrawLine( Pens.White, lx + 5, YC + rect.Height / 4, rx - 5, YC + rect.Height / 4 );
      g.DrawLine( Pens.White, lx, YC + rect.Height / 2, rx, YC + rect.Height / 2 );
      g.DrawLine( Pens.LightPink, lx + 5, YC + rect.Height * 3 / 4, rx - 5, YC + rect.Height * 3 / 4 );
      g.DrawLine( Pens.LightPink, lx, YC + rect.Height, rx, YC + rect.Height );

      gradSky.Dispose( );
      gradGround.Dispose( );

      // Get coords back to rect
      g.Restore( saved );

      // center cross
      g.DrawLine( Pens.Yellow, rect.Left + rect.Width / 2 - 3, rect.Top + rect.Height / 2, rect.Left + rect.Width / 2 + 3, rect.Top + rect.Height / 2 );
      g.DrawLine( Pens.Yellow, rect.Left + rect.Width / 2, rect.Top + rect.Height / 2 - 3, rect.Left + rect.Width / 2, rect.Top + rect.Height / 2 + 3 );

      g.DrawLine( Pens.Yellow, rect.Left, rect.Top + rect.Height / 2, rect.Left + rect.Width / 4, rect.Top + rect.Height / 2 );
      g.DrawLine( Pens.Yellow, rect.Right, rect.Top + rect.Height / 2, rect.Right - rect.Width / 4, rect.Top + rect.Height / 2 );

      if ( PitchAngle < Minimum ) {
        //out of sight upper
        // Draw two lines on top
        g.DrawLine( _pen, rect.Left, rect.Top + 3, rect.Right, rect.Top + 3 );
        g.DrawLine( _pen, rect.Left, rect.Top + 7, rect.Right, rect.Top + 7 );
      }
      else if ( PitchAngle > Maximum ) {
        g.DrawLine( _pen, rect.Left, rect.Bottom - 3, rect.Right, rect.Bottom - 3 );
        g.DrawLine( _pen, rect.Left, rect.Bottom - 7, rect.Right, rect.Bottom - 7 );
      }

    }

    private void UC_AHor_Resize( object sender, EventArgs e )
    {
      if ( AutoSizeHeight && AutoSizeWidth ) { this.Size = _minSize; }
      else if ( AutoSizeHeight ) { this.Height = (int)( this.Width / _aspect ); }
      else if ( AutoSizeWidth ) { this.Width = (int)( this.Height * _aspect ); }
      RecalcPrimitives( );
    }

    private void UC_AHor_ClientSizeChanged( object sender, EventArgs e )
    {
      RecalcPrimitives( );
    }

    // recalculate our internal primitives (avoiding cal for every paint)
    private void RecalcPrimitives( )
    {
      // Precalc things that will not change often
      var rect = this.ClientRectangle;
      // for scaling we reduce the size
      rect.Inflate( _border );
      _uScale = rect.Height / ( Maximum - Minimum );
      _drawCenter = new PointF( rect.Left + rect.Width / 2.0f, rect.Top + rect.Height / 2.0f );
    }

  }
}

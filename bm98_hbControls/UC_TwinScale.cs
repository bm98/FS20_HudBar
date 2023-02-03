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
  /// Provides a twin scale for one range of Values and two variables
  ///  The Control will draw a scale back in a neutral color
  ///  
  ///  Properties should be 
  ///   DispValue (float)
  ///   Min (float) defaults to -10
  ///   Max (float) defaults to +10
  ///   ForeColor of the Needle (Color)
  ///   ForeColor of the Needle (Color)
  ///   BackColor of the control (Color)
  ///   AutoSizeHorizontal flag (bool)
  ///   AutoSizeVertical flag (bool)
  ///   Enabled flag (bool)
  /// 
  /// </summary>
  public partial class UC_TwinScale : UserControl
  {
    private readonly Size _minSize = new Size( 30, 10);
    private readonly float _aspect = 3; // Height*_aspect => Width
    /// <summary>
    /// Properties
    /// </summary>
    private Damper _pDamperUpper = new Damper(0, 1);
    private Damper _pDamperLower = new Damper(0, 1);
    private float _pAlertValue = 0;
    private bool _pAlertEnabled = true;

    private float _pMin = -100;
    private float _pMax = 100;
    private bool _pAutoSizeHeight = false;
    private bool _pAutoSizeWidth = false;
    private Color _pLowColor = Color.Magenta;
    private Color _pAlertColor = Color.Red;

    private const float c_penSize = 3;

    private Pen _penUpper = new Pen(Color.Green, c_penSize);
    private Pen _penLower = new Pen(Color.Magenta, c_penSize);
    private Pen _penAlert = new Pen(Color.Red, 1);
    private readonly Brush _bScale = new SolidBrush(Color.FromArgb(96,32,32,32));

    private Size _border = new Size( -2, 0); // internal border for the scale drawn rectangle
    private Point[] _scaleBackPoly = new Point[4];
    float _uScale = 1;


    /// <summary>
    /// Display Value Upper Scale
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Display Value of the Upper scale" ), Category( "Data" )]
    public float ValueUpper {
      get => _pDamperUpper.Avg;
      set {
        _pDamperUpper.Sample( value );
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Display Value Lower Scale
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Display Value of the Lower scale" ), Category( "Data" )]
    public float ValueLower {
      get => _pDamperLower.Avg;
      set {
        _pDamperLower.Sample( value );
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Needle Dampening Factor
    /// The factor defines the 1/ ratio a new value will contribute to the out value
    /// </summary>
    [DefaultValue( 1 )]
    [Description( "Needle Dampening Factor" ), Category( "Data" )]
    public ushort Dampening {
      get => _pDamperUpper.Factor;
      set {
        if ( value == _pDamperUpper.Factor ) return; // already set
        _pDamperUpper = new Damper( _pDamperUpper.Avg, value );
        _pDamperLower = new Damper( _pDamperLower.Avg, value );
      }
    }

    /// <summary>
    /// Alert Value
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Alert Value of the Control" ), Category( "Appearance" )]
    public float AlertValue {
      get => _pAlertValue;
      set {
        if ( value < _pMin ) throw new ArgumentException( "Property value is invalid" );
        if ( value > _pMax ) throw new ArgumentException( "Property value is invalid" );
        _pAlertValue = value;
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Alert Value
    /// </summary>
    [DefaultValue( true )]
    [Description( "Wether to use the Alert Value" ), Category( "Appearance" )]
    public bool AlertEnabled {
      get => _pAlertEnabled;
      set {
        _pAlertEnabled = value;
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
    /// Get;Set; Foreground Color for the Lower Range
    /// Note this is reset to the Complementary Color when setting the ForeColor property
    /// </summary>
    [Description( "Foreground Color for the Lower Range" ), Category( "Appearance" )]
    public Color ForeColor_LRange {
      get => _pLowColor;
      set {
        if ( value == _pLowColor ) return; // already set
        _pLowColor = value;
        _penLower?.Dispose( );
        _penLower = new Pen( _pLowColor, c_penSize );
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Get;Set; Alert Color
    /// </summary>
    [Description( "Alert Color " ), Category( "Appearance" )]
    public Color ForeColor_Alert {
      get => _pAlertColor;
      set {
        if ( value == _pAlertColor ) return; // already set
        _pAlertColor = value;
        _penAlert?.Dispose( );
        _penAlert = new Pen( _pAlertColor, 1 );
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
        UC_Scale_Resize( this, null );
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
        UC_Scale_Resize( this, null );
      }
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_TwinScale( )
    {
      InitializeComponent( );
      // calc desired aspect from our _minSize Const
      _aspect = _minSize.Width / _minSize.Height; // Height*_aspect => Width
      RecalcPrimitives( );
      // get setting colors
      _penUpper?.Dispose( );
      _penUpper = new Pen( this.ForeColor, c_penSize );
      _penLower?.Dispose( );
      _penLower = new Pen( this.ForeColor_LRange, c_penSize );
      _penAlert?.Dispose( );
      _penAlert = new Pen( this.ForeColor_Alert, 1 );
    }

    /// <summary>
    /// Get;Set; Foreground Color Upper Range
    /// </summary>
    public override Color ForeColor {
      get => base.ForeColor;
      set {
        if ( value == base.ForeColor ) return; // already set
        base.ForeColor = value;
        _penUpper?.Dispose( );
        _penUpper = new Pen( value, c_penSize );
        this.Invalidate( this.ClientRectangle );
      }
    }

    private void UC_Scale_Paint( object sender, PaintEventArgs e )
    {
      if ( _penUpper == null ) return;
      if ( !this.Enabled ) return;

      var g = e.Graphics;

      var saved = g.Save();
      g.SmoothingMode = SmoothingMode.HighQuality;

      var rect = this.ClientRectangle;
      rect.Inflate( _border );

      g.DrawLine( Pens.Black, rect.Left, rect.Top + rect.Height / 2, rect.Right, rect.Top + rect.Height / 2 ); // a center line

      // Upper Scale
      var value = Math.Max( Math.Min(ValueUpper, Maximum), Minimum); // Limit Input to Range
      float nX =Math.Abs( _uScale * (value-Minimum));
      g.DrawLine( _penUpper, rect.Left + nX, rect.Top, rect.Left + nX, rect.Top + rect.Height / 2 );

      // Lower Scale
      value = Math.Max( Math.Min( ValueLower, Maximum ), Minimum ); // Limit Input to Range
      nX = Math.Abs( _uScale * ( value - Minimum ) );
      g.DrawLine( _penLower, rect.Left + nX, rect.Bottom, rect.Left + nX, rect.Bottom - rect.Height / 2 );

      // Alert Line gets over the ValueLine (else one does not see it)
      if ( AlertEnabled ) {
        nX = Math.Abs( _uScale * ( AlertValue - Minimum ) );
        g.DrawLine( _penAlert, rect.X + nX, rect.Top, rect.X + nX, rect.Bottom );
      }
      // this will cover the Line
      g.FillPolygon( _bScale, _scaleBackPoly );

      g.Restore( saved );
    }

    // Capture Resizing of the Control
    // Maintain the AutoSizing demands
    private void UC_Scale_Resize( object sender, EventArgs e )
    {
      if ( AutoSizeHeight && AutoSizeWidth ) { this.Size = _minSize; }
      else if ( AutoSizeHeight ) { this.Height = (int)( this.Width / _aspect ); }
      else if ( AutoSizeWidth ) { this.Width = (int)( this.Height * _aspect ); }
      RecalcPrimitives( );
    }

    private void UC_Scale_ClientSizeChanged( object sender, EventArgs e )
    {
      RecalcPrimitives( );
    }

    // recalculate our internal primitives (avoiding cal for every paint)
    private void RecalcPrimitives( )
    {
      // Precalc things that will not change often
      var rect = this.ClientRectangle;
      var l = 0;
      _scaleBackPoly = new Point[]{ new Point( rect.Left+l, rect.Bottom-rect.Height/2 ),
                                    new Point( rect.Left+l, rect.Bottom ) ,
                                    new Point( rect.Right, rect.Bottom-rect.Height/2 ) ,
                                    new Point( rect.Left+l, rect.Bottom-rect.Height/2 ),
                                    new Point( rect.Right, rect.Top ),
                                    new Point( rect.Left+l, rect.Top ),
                                    new Point( rect.Left+l, rect.Bottom-rect.Height/2 ) };
      // for scaling we reduce the size
      rect.Inflate( _border );
      _uScale = rect.Width / ( Maximum - Minimum );
    }
  }
}


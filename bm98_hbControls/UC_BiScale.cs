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
  /// Provides a scale for Pos and Neg Values
  ///  the Needle will go from left (Mid) to right  (Min; Max) 
  ///  pos values are shown in the upper half, neg values in the lower half 
  ///  Preferebly pos and neg values have different colors (Auto contrast??? then use only ForeColor for Pos and neg is found out by algorithm)
  ///  The Control will draw a scale back in a neutral color
  ///  
  ///  Properties should be 
  ///   DispValue (float)
  ///   Min (float) defaults to -10
  ///   Mid (float) defaults to   0
  ///   Max (float) defaults to +10
  ///   ForeColor of the Needle (Color)
  ///   BackColor of the control (Color)
  ///   AutoSizeHorizontal flag (bool)
  ///   AutoSizeVertical flag (bool)
  ///   Enabled flag (bool)
  ///   
  /// 
  /// </summary>
  public partial class UC_BiScale : UserControl
  {
    private readonly Size _minSize = new Size( 30, 10);
    private readonly float _aspect = 3; // Height*_aspect => Width
    /// <summary>
    /// Properties
    /// </summary>
    private Damper _pDamper = new Damper(0, 1);
    private float _pMin = -100;
    private float _pMid = 0;
    private float _pMax = 100;
    private bool _pAutoSizeHeight = false;
    private bool _pAutoSizeWidth = false;
    private Color _pLowColor = Color.Magenta;

    private const float c_penSize = 3;

    private Pen _penUpper = new Pen(Color.Green, c_penSize);
    private Pen _penLower = new Pen(Color.Magenta, c_penSize);
    private readonly Brush _bScale = new SolidBrush(Color.FromArgb(96,32,32,32));

    private Size _border = new Size( -2, 0); // internal border for the scale drawn rectangle
    private Point[] _scaleBackPoly = new Point[4];
    float _uScale = 1;
    float _lScale = 1;

    /// <summary>
    /// Display Value
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Display Value of the Control" ), Category( "Data" )]
    public float Value {
      get => _pDamper.Avg;
      set {
        _pDamper.Sample( value );
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
      get => _pDamper.Factor;
      set {
        if ( value == _pDamper.Factor ) return; // already set
        _pDamper = new Damper( _pDamper.Avg, value );
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
        if ( value >= _pMid ) throw new ArgumentException( "Property value is invalid" );
        _pMin = value;
        RecalcPrimitives( );
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Mid Scale Point
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Mid Scale Point" ), Category( "Data" )]
    public float Middle {
      get => _pMid;
      set {
        if ( value <= _pMin ) throw new ArgumentException( "Property value is invalid" );
        if ( value >= _pMax ) throw new ArgumentException( "Property value is invalid" );
        _pMid = value;
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
        if ( value <= _pMid ) throw new ArgumentException( "Property value is invalid" );
        _pMax = value;
        RecalcPrimitives( );
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Get;Set; Foreground Color for the Lower Range
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

    public UC_BiScale( )
    {
      InitializeComponent( );
      // calc desired aspect from our _minSize Const
      _aspect = _minSize.Width / _minSize.Height; // Height*_aspect => Width
      RecalcPrimitives( );
      // get setting colors
      _penUpper?.Dispose( );
      _penUpper = new Pen( this.ForeColor, c_penSize );
      // get a complementary color for the under Range
      _pLowColor = ComplementaryColor( this.ForeColor );
      _penLower?.Dispose( );
      _penLower = new Pen( _pLowColor, c_penSize );

    }

    /// <summary>
    /// Get;Set; Foreground Color
    /// </summary>
    public override Color ForeColor {
      get => base.ForeColor;
      set {
        if ( value == base.ForeColor ) return; // already set
        base.ForeColor = value;
        _penUpper?.Dispose( );
        _penUpper = new Pen( value, c_penSize );
        // get a complementary color for the under Range
        _pLowColor = ComplementaryColor( value );
        _penLower?.Dispose( );
        _penLower = new Pen( _pLowColor, c_penSize );
        this.Invalidate( this.ClientRectangle );
      }
    }

    private void UC_Scale_Paint( object sender, PaintEventArgs e )
    {
      if ( _penUpper == null ) return;
      if ( _penLower == null ) return;
      if ( !this.Enabled ) return;

      var g = e.Graphics;

      var saved = g.Save();
      g.SmoothingMode = SmoothingMode.HighQuality;

      var rect = this.ClientRectangle;
      rect.Inflate( _border );

      var value = Math.Max( Math.Min(Value, Maximum), Minimum); // Limit Input to Range

      //g.DrawLine( Pens.Black, rect.Left, rect.Top + rect.Height / 2, rect.Right, rect.Top + rect.Height / 2 ); // a center line
      // Needle
      if ( value >= Middle ) {
        // Upper Scale
        float nX =Math.Abs( _uScale * (value - Middle));
        g.DrawLine( _penUpper, rect.X + nX, rect.Top, rect.X + nX, rect.Top + rect.Height / 2 );
      }
      else {
        // lower Scale
        float nX = Math.Abs( _lScale * (Middle - value));
        g.DrawLine( _penLower, rect.X + nX, rect.Bottom, rect.X + nX, rect.Bottom - rect.Height / 2 );
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
      // scale back arrow like point is left back is right (full height)
      /*
      _scaleBackPoly = new Point[]{ new Point( rect.X, rect.Y+rect.Height/2 ) ,
                                    new Point( rect.Right, rect.Top ),
                                    new Point( rect.Right, rect.Bottom ),
                                    new Point( rect.X, rect.Y+rect.Height/2 ) };
      */
      var l = 0;
      _scaleBackPoly = new Point[]{ new Point( rect.Left+l, rect.Top+rect.Height/2 ) ,
                                    new Point( rect.Left+l, rect.Top ),
                                    new Point( rect.Right, rect.Top ),
                                    new Point( rect.Left-l-7, rect.Top+rect.Height/2 ) ,//back left
                                    new Point( rect.Right, rect.Bottom ),
                                    new Point( rect.Left+l, rect.Bottom ),
                                    new Point( rect.Left+l, rect.Top+rect.Height/2 ) };
      // for scaling we reduce the size
      rect.Inflate( _border );
      _uScale = rect.Width / ( Maximum - Middle );
      _lScale = rect.Width / ( Middle - Minimum );
    }

    private Color ComplementaryColor( Color color )
    {
      return Color.FromArgb( Color.White.ToArgb( ) & 0xFFFFFF - color.ToArgb( ) & 0xFFFFFF );
    }

  }
}

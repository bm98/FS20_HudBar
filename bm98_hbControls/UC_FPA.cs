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
  /// FPS Display
  ///   when in range, draws a arrow head shape with tip at Value
  ///   out of range, draws two hor. lines at the out of range end (top or bottom)
  /// </summary>
  public partial class UC_FPA : UserControl
  {
    private readonly Size _minSize = new Size( 20, 10);
    private readonly float _aspect = 2; // Height*_aspect => Width

    /// <summary>
    /// Properties
    /// </summary>
    private Damper _pDamperVert = new Damper(0, 1);
    private Damper _pDamperHor = new Damper(0, 1);

    private float _pMinV = -100;
    private float _pMaxV = 100;
    private float _pMinH= -100;
    private float _pMaxH = 100;
    private bool _pAutoSizeHeight = false;
    private bool _pAutoSizeWidth = false;
    private Color _pOutOfRangeColor = Color.Gold;

    private const float c_penSize = 2;

    private Pen _pen = new Pen(Color.Green, c_penSize);
    private Pen _penOutOfRange = new Pen(Color.Gold, c_penSize);
    private Size _border = new Size( -2, -2); // internal border for the scale drawn rectangle
    private float _vScale = 1;
    private float _hScale = 1;

    /// <summary>
    /// Vertical Angle
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Vertical Angle" ), Category( "Data" )]
    public float VerticalAngle {
      get => _pDamperVert.Avg;
      set {
        _pDamperVert.Sample( value );
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Horizontal Angle
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Horizontal Angle" ), Category( "Data" )]
    public float HorizontalAngle {
      get => _pDamperHor.Avg;
      set {
        _pDamperHor.Sample( value );
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
      get => _pDamperVert.Factor;
      set {
        if ( value == _pDamperVert.Factor ) return; // already set
        _pDamperVert = new Damper( _pDamperVert.Avg, value );
        _pDamperHor = new Damper( _pDamperHor.Avg, value );
      }
    }

    /// <summary>
    /// Minimum Scale Point Vertical
    /// </summary>
    [DefaultValue( -100 )]
    [Description( "Minimum Scale Point Vertical" ), Category( "Data" )]
    public float MinimumVer {
      get => _pMinV;
      set {
        if ( value >= _pMaxV ) throw new ArgumentException( "Property value is invalid" );
        _pMinV = value;
        RecalcPrimitives( );
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Maximum Scale Point Vertical
    /// </summary>
    [DefaultValue( 100 )]
    [Description( "Maximum Scale Point Vertical" ), Category( "Data" )]
    public float MaximumVer {
      get => _pMaxV;
      set {
        if ( value <= _pMinV ) throw new ArgumentException( "Property value is invalid" );
        _pMaxV = value;
        RecalcPrimitives( );
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Minimum Scale Point Horizontal
    /// </summary>
    [DefaultValue( -100 )]
    [Description( "Minimum Scale Point Horizontal" ), Category( "Data" )]
    public float MinimumHor {
      get => _pMinH;
      set {
        if ( value >= _pMaxH ) throw new ArgumentException( "Property value is invalid" );
        _pMinH = value;
        RecalcPrimitives( );
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Maximum Scale Point Horizontal
    /// </summary>
    [DefaultValue( 100 )]
    [Description( "Maximum Scale Point Horizontal" ), Category( "Data" )]
    public float MaximumHor {
      get => _pMaxH;
      set {
        if ( value <= _pMinH ) throw new ArgumentException( "Property value is invalid" );
        _pMaxH = value;
        RecalcPrimitives( );
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Get;Set; OutOfRange Color
    /// </summary>
    [Description( "OutOfRange Color " ), Category( "Appearance" )]
    public Color ForeColor_OutOfRange {
      get => _pOutOfRangeColor;
      set {
        if ( value == _pOutOfRangeColor ) return; // already set
        _pOutOfRangeColor = value;
        _penOutOfRange?.Dispose( );
        _penOutOfRange = new Pen( _pOutOfRangeColor, c_penSize );
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
        UC_FPA_Resize( this, null );
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
        UC_FPA_Resize( this, null );
      }
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_FPA( )
    {
      InitializeComponent( );
      // calc desired aspect from our _minSize Const
      _aspect = _minSize.Width / _minSize.Height; // Height*_aspect => Width
      _pDamperVert = new Damper( 0, _pDamperVert.Factor );
      _pDamperHor = new Damper( 0, _pDamperHor.Factor );
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


    private void UC_FPA_Paint( object sender, PaintEventArgs e )
    {
      if ( _pen == null ) return;
      if ( !this.Enabled ) return;

      var g = e.Graphics;

      var saved = g.Save();
      g.SmoothingMode = SmoothingMode.HighQuality;

      var rect = this.ClientRectangle;
      g.TranslateTransform( rect.Left + rect.Width / 2, rect.Top + rect.Height / 2 );
      // center is now at 0/0
      // guide Rect (half size around the center)
      g.DrawRectangle( Pens.DimGray, -rect.Width / _aspect / 4, -rect.Height / 4, rect.Width / _aspect / 2, rect.Height / 2 );

      // Hor Guides
      // - 1/2 guides
      g.DrawLine( Pens.Gray, -rect.Width / _aspect / 2f, -rect.Height / 2, -rect.Width / _aspect / 2f, -rect.Height / 2 + 3 );
      g.DrawLine( Pens.Gray, -rect.Width / _aspect / 2f, 3, -rect.Width / _aspect / 2f, -3 );
      g.DrawLine( Pens.Gray, -rect.Width / _aspect / 2f, rect.Height / 2, -rect.Width / _aspect / 2f, rect.Height / 2 - 3 );
      
      // Center guides
      g.DrawLine( Pens.Gray, 0, -rect.Height / 2, 0, -rect.Height / 2 + 3 );
      g.DrawLine( Pens.Gray, 0, rect.Height / 2, 0, rect.Height / 2 - 3 );
      // + 1/2 guides
      g.DrawLine( Pens.Gray, rect.Width / _aspect / 2f, -rect.Height / 2, rect.Width / _aspect / 2f, -rect.Height / 2 + 3 );
      g.DrawLine( Pens.Gray, rect.Width / _aspect / 2f, 3, rect.Width / _aspect / 2f, -3 );
      g.DrawLine( Pens.Gray, rect.Width / _aspect / 2f, rect.Height / 2, rect.Width / _aspect / 2f, rect.Height / 2 - 3 );
      
      // Vertical Center guides 
      g.DrawLine( Pens.Gray, -rect.Width / 2, 0, -rect.Width / 2 + 3, 0 );
      g.DrawLine( Pens.Gray, -rect.Width / _aspect / 2f - 3, 0, -rect.Width / _aspect / 2f + 3, 0 );
      g.DrawLine( Pens.Gray, rect.Width / _aspect / 2f - 3, 0, rect.Width / _aspect / 2f + 3, 0 );
      g.DrawLine( Pens.Gray, rect.Width / 2, 0, rect.Width / 2 - 3, 0 );

      // center cross
      g.DrawLine( Pens.Gray, -3, 0, 3, 0 );
      g.DrawLine( Pens.Gray, 0, -3, 0, 3 );

      // use Size Only !! X/Y was shifted in Transform above
      rect.Inflate( _border );

      // Value
      float nX = rect.Width/2 - _hScale * HorizontalAngle; // Mid is in the center
      float nY = rect.Height/2 - _vScale * VerticalAngle; // Mid is in the center

      PointF[] aPts = new PointF[3]{ new PointF(-rect.Width / _aspect / 4, rect.Height / 4), new PointF(0,0), new PointF(rect.Width / _aspect / 4,rect.Height/4)};

      // 4x both out of range all combinations
      if ( ( nX < 0 ) && ( nY < 0 ) ) {
        //out of sight left and upper
        g.TranslateTransform( -rect.Width / 2, -rect.Height / 2 );
        g.RotateTransform( -45, MatrixOrder.Prepend );
        g.DrawLines( _penOutOfRange, aPts );
      }
      else if ( ( nX > rect.Width ) && ( nY > rect.Height ) ) {
        //out of sight right and lower
        g.TranslateTransform( rect.Width / 2, rect.Height / 2 );
        g.RotateTransform( 135, MatrixOrder.Prepend );
        g.DrawLines( _penOutOfRange, aPts );
      }
      else if ( ( nX < 0 ) && ( nY > rect.Height ) ) {
        //out of sight left and lower
        g.TranslateTransform( -rect.Width / 2, rect.Height / 2 );
        g.RotateTransform( -135, MatrixOrder.Prepend );
        g.DrawLines( _penOutOfRange, aPts );
      }
      else if ( ( nX > rect.Width ) && ( nY < 0 ) ) {
        //out of sight right and upper
        g.TranslateTransform( rect.Width / 2, -rect.Height / 2 );
        g.RotateTransform( 45, MatrixOrder.Prepend );
        g.DrawLines( _penOutOfRange, aPts );
      }
      // 4x one out of range
      else if ( nX < 0 ) {
        //out of sight left only
        g.TranslateTransform( -rect.Width / 2, nY - rect.Height / 2 );
        g.RotateTransform( -90, MatrixOrder.Prepend );
        g.DrawLines( _penOutOfRange, aPts );
      }
      else if ( nX > rect.Width ) {
        //out of sight right only
        g.TranslateTransform( rect.Width / 2, nY - rect.Height / 2 );
        g.RotateTransform( 90, MatrixOrder.Prepend );
        g.DrawLines( _penOutOfRange, aPts );
      }
      else if ( nY < 0 ) {
        //out of sight upper only
        //g.RotateTransform( 90, MatrixOrder.Prepend );
        g.TranslateTransform( nX - rect.Width / 2, -rect.Height / 2 );
        g.DrawLines( _penOutOfRange, aPts );
      }
      else if ( nY > rect.Height ) {
        //out of sight lower only
        g.TranslateTransform( nX - rect.Width / 2, rect.Height / 2 );
        g.RotateTransform( 180, MatrixOrder.Prepend );
        g.DrawLines( _penOutOfRange, aPts );
      }
      // draw arrowhead
      else {
        g.TranslateTransform( nX - rect.Width / 2, nY - rect.Height / 2 );
        g.DrawLines( _pen, aPts );
      }

      g.Restore( saved );
    }

    private void UC_FPA_ClientSizeChanged( object sender, EventArgs e )
    {
      RecalcPrimitives( );
    }

    private void UC_FPA_Resize( object sender, EventArgs e )
    {
      if ( AutoSizeHeight && AutoSizeWidth ) { this.Size = _minSize; }
      else if ( AutoSizeHeight ) { this.Height = (int)( this.Width / _aspect ); }
      else if ( AutoSizeWidth ) { this.Width = (int)( this.Height * _aspect ); }
    }

    // recalculate our internal primitives (avoiding cal for every paint)
    private void RecalcPrimitives( )
    {
      // Precalc things that will not change often
      var rect = this.ClientRectangle;
      // for scaling we reduce the size
      rect.Inflate( _border );
      _hScale = -rect.Width / ( MaximumHor - MinimumHor );
      _vScale = rect.Height / ( MaximumVer - MinimumVer );
    }

  }
}

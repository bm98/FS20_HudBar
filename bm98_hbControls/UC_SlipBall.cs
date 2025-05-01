using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using bm98_hbControls.Properties;

namespace bm98_hbControls
{
  /// <summary>
  /// SlipBall and TurnRate Display
  /// </summary>
  public partial class UC_SlipBall : UserControl
  {
    private readonly Size _minSize = new Size( 30, 10 );
    private readonly float _aspect = 3; // Height*_aspect => Width

    /// <summary>
    /// Properties
    /// </summary>
    private Damper _pDamperBall = new Damper( 0, 5, 0 );
    // scale
    private float _pMinHor = -100;
    private float _pMaxHor = 100;
    // ball size % of Height
    private int _ballSize_prct = 50;
    // turnrate %
    private int _turnRate_prct = 0;
    // turnrate rotation for Standard TurnRate  (angle to rot the graph) * 100
    private float _turnRate_stdRot_deg100 = 0;

    private bool _pAutoSizeHeight = false;
    private bool _pAutoSizeWidth = false;
    private bool _inop = false;

    // drawing primitives, calculated when resizing
    private int _d4th = 30 / 4; // 1/4 width
    private int _dGuideRect = 30 / 12; // guide rect half width
    private int _dhHhalf = 10 / 2; // half height
    private int _dwHhalf = 30 / 2; // half width
    private Size _border = new Size( -2, -2 ); // internal border for the scale drawn rectangle
    private float _lScale = 1;

    private Brush _brush = new SolidBrush( Color.White );

    /// <summary>
    /// Ball Position min..max range NaN to INOP
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Ball Position" ), Category( "Data" )]
    public float BallPosition {
      get => (int)_pDamperBall.Avg;
      set {
        if (float.IsNaN( value )) {
          this.Inop = true;
        }
        else {
          this.Inop = false;
          _pDamperBall.Sample( value );
        }
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Ball Size 1..100% (of graph height)
    /// </summary>
    [DefaultValue( 50 )]
    [Description( "Ball Size %" ), Category( "Data" )]
    public int BallSize_prct {
      get => _ballSize_prct;
      set {
        if (value == _ballSize_prct) return; // already

        _ballSize_prct = (value < 1) ? 1 : (value > 100) ? 100 : value; // limit 1..100
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Current Turnrate% of selected Std Turnrate 0..+-500% (negative is Left Bank)
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Turnrate % of selected StdTurnrate" ), Category( "Data" )]
    public int Turnrate_prct {
      get => _turnRate_prct;
      set {
        if (value == _turnRate_prct) return; // already
        // limit +-500
        _turnRate_prct = value;
        _turnRate_prct = Math.Min( _turnRate_prct, 500 );
        _turnRate_prct = Math.Max( _turnRate_prct, -500 );
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
      get => _pDamperBall.Factor;
      set {
        if (value == _pDamperBall.Factor) return; // already set
        _pDamperBall = new Damper( 0, value, 0 );
      }
    }

    /// <summary>
    /// Minimum Scale Point Horizontal
    /// </summary>
    [DefaultValue( -100 )]
    [Description( "Minimum Scale Point Horizontal" ), Category( "Data" )]
    public float MinimumHor {
      get => _pMinHor;
      set {
        if (value >= _pMaxHor) throw new ArgumentException( "Property value is invalid" );
        _pMinHor = value;
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
      get => _pMaxHor;
      set {
        if (value <= _pMinHor) throw new ArgumentException( "Property value is invalid" );
        _pMaxHor = value;
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
        UC_SlipBall_Resize( this, null );
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
        UC_SlipBall_Resize( this, null );
      }
    }

    /// <summary>
    /// Inop flag
    /// </summary>
    [DefaultValue( false )]
    [Description( "Set the control as Inop" ), Category( "Appearance" )]
    public bool Inop {
      get => _inop;
      set {
        _inop = value;
        this.Refresh( );
      }
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_SlipBall( )
    {
      InitializeComponent( );
      // calc desired aspect from our _minSize Const
      _aspect = _minSize.Width / _minSize.Height; // Height*_aspect => Width
      _pDamperBall = new Damper( 0, _pDamperBall.Factor, 0 );
      RecalcPrimitives( );
      // get setting colors
      _brush?.Dispose( );
      _brush = new SolidBrush( this.ForeColor );
    }

    /// <summary>
    /// Get;Set; Foreground Color
    /// </summary>
    public override Color ForeColor {
      get => base.ForeColor;
      set {
        if (value == base.ForeColor) return; // already set
        base.ForeColor = value;
        _brush?.Dispose( );
        _brush = new SolidBrush( value );
        this.Invalidate( this.ClientRectangle );
      }
    }

    private void UC_SlipBall_Resize( object sender, EventArgs e )
    {
      if (AutoSizeHeight && AutoSizeWidth) { this.Size = _minSize; }
      else if (AutoSizeHeight) { this.Height = (int)(this.Width / _aspect); }
      else if (AutoSizeWidth) { this.Width = (int)(this.Height * _aspect); }
      RecalcPrimitives( );
    }

    private void UC_SlipBall_Paint( object sender, PaintEventArgs e )
    {
      if (_brush == null) return;
      if (!this.Enabled) return;

      var g = e.Graphics;

      var saved = g.Save( );
      g.SmoothingMode = SmoothingMode.HighQuality;

      var rect = this.ClientRectangle;
      if (_inop) {
        g.DrawImage( Resources.Inop, rect );
      }
      else {
        rect.Inflate( _border );
        // width 0/0 -> center square width/aspect, height

        float lPos = (BallPosition - MinimumHor) * _lScale * _aspect; // ball center in rect area
        int bSize2 = (int)(rect.Height * (_ballSize_prct / 200f));
        // strange drawing an odd size draws an even size circle... - so make it even(to have it odd drawn)
        int bSize = bSize2 * 2;
        // half of ball size to the left
        int ballDrawOffset = rect.Left - bSize2;

        // vert center 
        g.TranslateTransform( 0, rect.Top + (int)Math.Ceiling( rect.Height / 2f ) );

        g.FillEllipse( _brush, (int)lPos + ballDrawOffset, -bSize2, bSize, bSize );

        // center of the drawing area is now  0/0
        g.TranslateTransform( rect.Left + (int)Math.Floor( rect.Width / 2f ), 0 );

        // decorations

        // Vert Guides
        // - 1/4 guides
        g.DrawLine( Pens.Gray, -_d4th, -_dhHhalf, -_d4th, -_dhHhalf + 3 );// vert top
        g.DrawLine( Pens.Gray, -_d4th, 3, -_d4th, -3 ); // vert center
        g.DrawLine( Pens.Gray, -_d4th, _dhHhalf, -_d4th, _dhHhalf - 3 ); // vert bottom

        // Center guides
        g.DrawLine( Pens.Gray, 0, -_dhHhalf, 0, -_dhHhalf + 3 ); // vert top
        g.DrawLine( Pens.Gray, 0, _dhHhalf, 0, _dhHhalf - 3 ); // vert bottom

        // + 1/4 guides
        g.DrawLine( Pens.Gray, _d4th, -_dhHhalf, _d4th, -_dhHhalf + 3 );// vert top
        g.DrawLine( Pens.Gray, _d4th, 3, _d4th, -3 ); // vert center
        g.DrawLine( Pens.Gray, _d4th, _dhHhalf, _d4th, _dhHhalf - 3 ); // vert bottom

        // center cross
        g.DrawLine( Pens.Gray, -3, 0, 3, 0 );
        g.DrawLine( Pens.Gray, 0, -3, 0, 3 );

        // vertical ball limits
       // g.DrawRectangle( Pens.DeepSkyBlue, -_dGuideRect, -rect.Height / 2, _dGuideRect * 2, rect.Height );


        // turn profile, rot to 100% turn = 
        g.RotateTransform( _turnRate_stdRot_deg100 * _turnRate_prct );

        // overlay the guide Rect (around the center)
        g.DrawRectangle( Pens.Gray, -_dGuideRect, -rect.Height / 2, _dGuideRect * 2, rect.Height );

        // draw bank from left to right
        g.DrawLine( Pens.LightCyan, -_dwHhalf, 0, _dwHhalf, 0 );

      }
      g.Restore( saved );
    }

    // recalculate our internal primitives (avoiding cal for every paint)
    private void RecalcPrimitives( )
    {
      // Precalc things that will not change often
      var rect = this.ClientRectangle;
      // for scaling we reduce the size
      rect.Inflate( _border );
      _lScale = rect.Width / 3f / (MaximumHor - MinimumHor);
      _dhHhalf = rect.Height / 2;
      _dwHhalf = rect.Width / 2;
      _d4th = rect.Width / 4;
      _dGuideRect = rect.Width / 12;
      // std Rate angle pointing from center to the lower R or L corner
      // /100 so we dont need to div the arg by 100 each time when using it
      _turnRate_stdRot_deg100 = (float)(Math.Atan( (rect.Height / 2.0) / (rect.Width / 2.0) ) / Math.PI * 180.0 / 100.0);
    }

  }
}


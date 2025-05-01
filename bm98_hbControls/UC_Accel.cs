using bm98_hbControls.Properties;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace bm98_hbControls
{
  /// <summary>
  /// Acceleration Display
  /// </summary>
  public partial class UC_Accel : UserControl
  {
    private readonly Size _minSize = new Size( 30, 10 );
    private readonly float _aspect = 3; // Height*_aspect => Width

    /// <summary>
    /// Properties
    /// </summary>
    private Damper _pDamperVert = new Damper( 5, 3 );
    private Damper _pDamperLon = new Damper( 5, 3 );

    private float _pMinV = -100;
    private float _pMaxV = 100;
    private float _pMinL = -100;
    private float _pMaxL = 100;
    private bool _pAutoSizeHeight = false;
    private bool _pAutoSizeWidth = false;
    private Color _pOutOfRangeColor = Color.Gold;
    private bool _inop = false;

    private const float c_penSize = 2;

    private Pen _pen = new Pen( Color.Green, c_penSize );
    private Pen _penOutOfRange = new Pen( Color.Gold, c_penSize );
    private Size _border = new Size( -2, -2 ); // internal border for the scale drawn rectangle
    private float _vScale = 1;
    private float _lScale = 1;

    /// <summary>
    /// Vertical Accel (pos = up)
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Vertical Acceleration" ), Category( "Data" )]
    public float VerticalAccel {
      get => _pDamperVert.Avg;
      set {
        if (float.IsNaN( value )) {
          this.Inop = true;
        }
        else {
          this.Inop = false;
          _pDamperVert.Sample( value );
        }
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Longitudinal Accel (pos = increase)
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Longitudinal Acceleration" ), Category( "Data" )]
    public float LongitudinalAccel {
      get => _pDamperLon.Avg;
      set {
        if (float.IsNaN( value )) {
          this.Inop = true;
        }
        else {
          this.Inop = false;
          _pDamperLon.Sample( value );
        }
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
        if (value == _pDamperVert.Factor) return; // already set
        _pDamperVert = new Damper( value, 3 );
        _pDamperLon = new Damper( value, 3 );
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
        if (value >= _pMaxV) throw new ArgumentException( "Property value is invalid" );
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
        if (value <= _pMinV) throw new ArgumentException( "Property value is invalid" );
        _pMaxV = value;
        RecalcPrimitives( );
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Minimum Scale Point Longitudinal
    /// </summary>
    [DefaultValue( -100 )]
    [Description( "Minimum Scale Point Longitudinal" ), Category( "Data" )]
    public float MinimumLon {
      get => _pMinL;
      set {
        if (value >= _pMaxL) throw new ArgumentException( "Property value is invalid" );
        _pMinL = value;
        RecalcPrimitives( );
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Maximum Scale Point Longitudinal
    /// </summary>
    [DefaultValue( 100 )]
    [Description( "Maximum Scale Point Longitudinal" ), Category( "Data" )]
    public float MaximumLon {
      get => _pMaxL;
      set {
        if (value <= _pMinL) throw new ArgumentException( "Property value is invalid" );
        _pMaxL = value;
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
        if (value == _pOutOfRangeColor) return; // already set
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
        UC_Acccel_Resize( this, null );
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
        UC_Acccel_Resize( this, null );
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
    public UC_Accel( )
    {
      InitializeComponent( );
      // calc desired aspect from our _minSize Const
      _aspect = _minSize.Width / _minSize.Height; // Height*_aspect => Width
      _pDamperVert = new Damper( _pDamperVert.Factor, 3 );
      _pDamperLon = new Damper( _pDamperLon.Factor, 3 );
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
        if (value == base.ForeColor) return; // already set
        base.ForeColor = value;
        _pen?.Dispose( );
        _pen = new Pen( value, c_penSize );
        this.Invalidate( this.ClientRectangle );
      }
    }

    private void UC_Acccel_Resize( object sender, EventArgs e )
    {
      if (AutoSizeHeight && AutoSizeWidth) { this.Size = _minSize; }
      else if (AutoSizeHeight) { this.Height = (int)(this.Width / _aspect); }
      else if (AutoSizeWidth) { this.Width = (int)(this.Height * _aspect); }
      RecalcPrimitives( );
    }

    private void UC_Acccel_Paint( object sender, PaintEventArgs e )
    {
      if (_pen == null) return;
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
        // fill accel
        // width 0/0 -> center square width/aspect, height
        // width min -> full size square width, height
        // width max -> zero size square, height
        float lWidth = rect.Width/2f - (LongitudinalAccel - MinimumLon) * _lScale;
        float vCenter = (VerticalAccel - MinimumVer) * _vScale; //min=top, max=bottom
        if (lWidth > 1) {
          g.FillRectangle( Brushes.DodgerBlue, rect.Left + (rect.Width - lWidth) / 2f, rect.Top, lWidth, vCenter );
          g.FillRectangle( Brushes.Peru, rect.Left + (rect.Width - lWidth) / 2f, rect.Top + vCenter, lWidth, rect.Height - vCenter );
        }

        // decorations
        g.TranslateTransform( rect.Left + rect.Width / 2, rect.Top + rect.Height / 2 );
        // center is now at 0/0

        // Vert Guides
        // - 1/6 guides
        g.DrawLine( Pens.Gray, -rect.Width / _aspect / 2f, -rect.Height / 2, -rect.Width / _aspect / 2f, -rect.Height / 2 + 3 );// vert top
        g.DrawLine( Pens.Gray, -rect.Width / _aspect / 2f, 3, -rect.Width / _aspect / 2f, -3 ); // vert center
        g.DrawLine( Pens.Gray, -rect.Width / _aspect / 2f, rect.Height / 2, -rect.Width / _aspect / 2f, rect.Height / 2 - 3 ); // vert bottom

        // Center guides
        g.DrawLine( Pens.Gray, 0, -rect.Height / 2, 0, -rect.Height / 2 + 3 ); // vert top
        g.DrawLine( Pens.Gray, 0, rect.Height / 2, 0, rect.Height / 2 - 3 ); // vert bottom

        // + 1/6 guides
        g.DrawLine( Pens.Gray, rect.Width / _aspect / 2f, -rect.Height / 2, rect.Width / _aspect / 2f, -rect.Height / 2 + 3 );// vert top
        g.DrawLine( Pens.Gray, rect.Width / _aspect / 2f, 3, rect.Width / _aspect / 2f, -3 ); // vert center
        g.DrawLine( Pens.Gray, rect.Width / _aspect / 2f, rect.Height / 2, rect.Width / _aspect / 2f, rect.Height / 2 - 3 ); // vert bottom

        // Hor Center guides 
        g.DrawLine( Pens.Gray, -rect.Width / 2, 0, -rect.Width / 2 + 3, 0 ); // hor left
        g.DrawLine( Pens.Gray, -rect.Width / _aspect / 2f - 3, 0, -rect.Width / _aspect / 2f + 3, 0 ); // hor -1/6
        g.DrawLine( Pens.Gray, rect.Width / _aspect / 2f - 3, 0, rect.Width / _aspect / 2f + 3, 0 ); // hor +1/6
        g.DrawLine( Pens.Gray, rect.Width / 2, 0, rect.Width / 2 - 3, 0 ); // hor right

        // center cross
        g.DrawLine( Pens.Gray, -3, 0, 3, 0 );
        g.DrawLine( Pens.Gray, 0, -3, 0, 3 );

        // overlay the guide Rect (around the center)
        g.DrawRectangle( Pens.White, -rect.Width / _aspect / 2, -rect.Height / 2, rect.Width / _aspect, rect.Height );

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
      _lScale = rect.Width/3f / (MaximumLon - MinimumLon);
      _vScale = rect.Height / (MaximumVer - MinimumVer);
    }

  }
}

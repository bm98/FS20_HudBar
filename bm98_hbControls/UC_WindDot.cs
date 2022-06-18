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
  /// User Control to draw a Wind Up/Down Direction Dot
  /// i.e. the dot should represent the wind strength
  ///  Dot Size and Colot is controlled by
  ///   Wind Strength
  ///   
  ///  Properties should be 
  ///   WindSpeed [m/sec] (float)
  ///   UpWindColor of the dot when Positive (Color)
  ///   DownWindColor of the dot when Negative (Color)
  ///   BackColor of the control (Color)
  ///   AutoSizeHorizontal flag (bool)
  ///   AutoSizeVertical flag (bool)
  ///   Enabled flag (bool)
  ///   
  /// </summary>
  public partial class UC_WindDot : UserControl
  {
    private readonly Size _minSize = new Size( 10, 10 );
    private readonly float _maxSpeed = 3.0f; // 3 m/s ~5.8 kt
    private readonly HatchStyle _hatch = HatchStyle.Percent75;

    /// <summary>
    /// Properties
    /// </summary>
    private float _pWindSpeed = 0;
    private bool _pAutoSizeHeight = false;
    private bool _pAutoSizeWidth = false;
    private Color _upWindColor = Color.Green;
    private Color _downWindColor = Color.DarkOrange;

    private Brush _upWindBrush;
    private Brush _downWindBrush;

    /// <summary>
    /// Wind Direction From [true deg]
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Wind Speed [m/sec] positive is up" ), Category( "Data" )]
    public float WindSpeed {
      get => _pWindSpeed;
      set {
        _pWindSpeed = value;
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
        UC_WindDot_Resize( this, null );
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
        UC_WindDot_Resize( this, null );
      }
    }

    /// <summary>
    /// Get;Set; UpWind Color
    /// </summary>
    [DefaultValue( false )]
    [Description( "Dot Color for Up-Wind" ), Category( "Appearance" )]
    public Color UpWindColor {
      get => _upWindColor;
      set {
        if (value == _upWindColor) return; // already set
        _upWindColor = value;
        _upWindBrush?.Dispose( );
        _upWindBrush = new HatchBrush( _hatch, _upWindColor );
      }
    }
    /// <summary>
    /// Get;Set; Foreground Color
    /// </summary>
    [DefaultValue( false )]
    [Description( "Dot Color for Down-Wind" ), Category( "Appearance" )]
    public Color DownWindColor {
      get => _downWindColor;
      set {
        if (value == _downWindColor) return; // already set
        _downWindColor = value;
        _downWindBrush?.Dispose( );
        _downWindBrush = new HatchBrush( _hatch, _downWindColor );
      }
    }

    /*
     * Notes:
     *  The UC is sized Square based on AutoSizeV and AutoSizeH
     *  It cannot shrink under MinSize which is fixed at 8x8
     */
    /// <summary>
    /// cTor:
    /// </summary>
    public UC_WindDot( )
    {
      InitializeComponent( );

      _upWindBrush = new HatchBrush( _hatch, _upWindColor );
      _downWindBrush = new HatchBrush( _hatch, _downWindColor );
    }


    // Capture Paint Event - draw stuff
    private void UC_WindDot_Paint( object sender, PaintEventArgs e )
    {
      if (_upWindBrush == null) return;
      if (_downWindBrush == null) return;
      if (!this.Enabled) return;

      var g = e.Graphics;

      var saved = g.Save( );
      g.SmoothingMode = SmoothingMode.HighQuality;

      // Set world transform of graphics object to translate.
      var rect = this.ClientRectangle;
      var shift = new Size( (rect.X + rect.Width / 2), (rect.Y + rect.Height / 2) );
      g.TranslateTransform( shift.Width, shift.Height, MatrixOrder.Append );

      float radius = rect.Width * (Math.Min( Math.Abs( _pWindSpeed ), _maxSpeed ) / _maxSpeed) / 2f;
      if (radius < 0.5f) return;

      if (_pWindSpeed >= 0) {
        g.FillEllipse( _upWindBrush, -radius, -radius, 2f * radius, 2f * radius );
      }
      else {
        g.FillEllipse( _downWindBrush, -radius, -radius, 2f * radius, 2f * radius );
      }

      g.Restore( saved );
    }


    // Capture Resizing of the Control
    // Maintain the AutoSizing demands
    private void UC_WindDot_Resize( object sender, EventArgs e )
    {
      if (AutoSizeHeight && AutoSizeWidth) { this.Size = _minSize; }
      else if (AutoSizeHeight) { this.Height = this.Width; }
      else if (AutoSizeWidth) { this.Width = this.Height; }
    }

    private void UC_WindDot_ClientSizeChanged( object sender, EventArgs e )
    {
      this.Invalidate( this.ClientRectangle );
    }

    // recalculate our internal primitives (avoiding cal for every paint)
  }
}

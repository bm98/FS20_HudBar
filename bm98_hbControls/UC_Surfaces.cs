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
  /// Display Control Surfaces
  /// 
  /// Rudder, Elevator, Ailerons
  /// 
  /// </summary>
  public partial class UC_Surfaces : UserControl
  {
    private readonly Size _minSize = new Size(30, 10);
    private readonly float _aspect = 3; // Height*_aspect => Width

    /// <summary>
    /// Properties
    /// </summary>
    private float _pRudder = 0;
    private float _pElevator = 0;
    private float _pAileron = 0;

    private bool _pAutoSizeHeight = false;
    private bool _pAutoSizeWidth = false;

    private const float c_penSize = 3;

    private Color _pREColor = Color.Lime;
    private Color _pAileronColor = Color.Aqua;

    private Pen _penRE = new Pen(Color.Pink, c_penSize);
    private Pen _penAileron = new Pen(Color.Pink, c_penSize);
    private readonly Brush _bScale = new SolidBrush(Color.FromArgb(96, 32, 32, 32));

    private Size _border = new Size(-2, 0); // internal border for the scale drawn rectangle
    private Point[] _scaleBackPoly = new Point[4];
    float _x1Scale = 1f;
    float _x2Scale = 1f;
    float _yScale = 1f;

    /// <summary>
    /// Display Value Rudder 0..+-1 [%/100] -1 Full Left
    /// </summary>
    [DefaultValue(0)]
    [Description("Display Value of the Rudder"), Category("Data")]
    public float ValueRudder
    {
      get => _pRudder;
      set
      {
        _pRudder = value;
        this.Invalidate(this.ClientRectangle);
      }
    }
    /// <summary>
    /// Display Value Elevator 0..+-1 [%/100] -1 Full Down
    /// </summary>
    [DefaultValue(0)]
    [Description("Display Value of the Elevator"), Category("Data")]
    public float ValueElevator
    {
      get => _pElevator;
      set
      {
        _pElevator = value;
        this.Invalidate(this.ClientRectangle);
      }
    }
    /// <summary>
    /// Display Value Aileron 0..+-1 [%/100] -1 Full Left
    /// </summary>
    [DefaultValue(0)]
    [Description("Display Value of the Aileron"), Category("Data")]
    public float ValueAileron
    {
      get => _pAileron;
      set
      {
        _pAileron = value;
        this.Invalidate(this.ClientRectangle);
      }
    }

    /// <summary>
    /// Get;Set; Foreground Color for the Rudder/Elevator Cross
    /// </summary>
    [Description("Foreground Color for the Rudder/Elevator Cross"), Category("Appearance")]
    public Color ForeColor_RE
    {
      get => _pREColor;
      set
      {
        if (value == _pREColor) return; // already set
        _pREColor = value;
        _penRE?.Dispose();
        _penRE = new Pen(_pREColor, c_penSize);
        this.Invalidate(this.ClientRectangle);
      }
    }

    /// <summary>
    /// Get;Set; Foreground Color for the Aileron
    /// </summary>
    [Description("Foreground Color for the Aileron"), Category("Appearance")]
    public Color ForeColor_Aileron
    {
      get => _pAileronColor;
      set
      {
        if (value == _pAileronColor) return; // already set
        _pAileronColor = value;
        _penAileron?.Dispose();
        _penAileron = new Pen(_pAileronColor, c_penSize);
        this.Invalidate(this.ClientRectangle);
      }
    }

    /// <summary>
    /// AutoSize the Height of the Control
    /// </summary>
    [DefaultValue(false)]
    [Description("AutoSize the Height of the Control"), Category("Layout")]
    public bool AutoSizeHeight
    {
      get => _pAutoSizeHeight;
      set
      {
        _pAutoSizeHeight = value;
        UC_Scale_Resize(this, null);
      }
    }

    /// <summary>
    /// AutoSize the Width of the Control
    /// </summary>
    [DefaultValue(false)]
    [Description("AutoSize the Width of the Control"), Category("Layout")]
    public bool AutoSizeWidth
    {
      get => _pAutoSizeWidth;
      set
      {
        _pAutoSizeWidth = value;
        UC_Scale_Resize(this, null);
      }
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_Surfaces( )
    {
      InitializeComponent();
      // calc desired aspect from our _minSize Const
      _aspect = _minSize.Width / _minSize.Height; // Height*_aspect => Width
      RecalcPrimitives();
      // get setting colors
      _penRE?.Dispose();
      _penRE = new Pen(this._pREColor, c_penSize);
      _penAileron?.Dispose();
      _penAileron = new Pen(this._pAileronColor, c_penSize);
    }

    /// <summary>
    /// Get;Set; Foreground Color RE and Aileron
    /// </summary>
    public override Color ForeColor
    {
      get => base.ForeColor;
      set
      {
        if (value == base.ForeColor) return; // already set

        base.ForeColor = value;

        /*
        _pREColor = value;
        _pAileronColor = value;

        _penRE?.Dispose();
        _penRE = new Pen(this._pREColor, c_penSize);

        _penAileron?.Dispose();
        _penAileron = new Pen(this._pAileronColor, c_penSize);
        */
        this.Invalidate(this.ClientRectangle);
      }
    }

    private void UC_Scale_Paint( object sender, PaintEventArgs e )
    {
      if (!this.Enabled) return;

      var g = e.Graphics;

      var saved = g.Save();
      g.SmoothingMode = SmoothingMode.HighQuality;

      var rect = this.ClientRectangle;
      rect.Inflate(_border);

      // HV Cross Dimm
      g.DrawLine(Pens.DimGray, rect.Left, rect.Top + rect.Height / 2, rect.Left + rect.Width / 3f * 2, rect.Top + rect.Height / 2); // Cross Hor
      g.DrawLine(Pens.DimGray, rect.Left + rect.Width / 3f * 2 / 2, rect.Top, rect.Left + rect.Width / 3f * 2 / 2, rect.Bottom); // Cross Ver

      // RE Cross
      var valueX = Math.Max(Math.Min(ValueRudder, 1), -1); // Limit Input to Range
      var valueY = Math.Max(Math.Min(ValueElevator, 1), -1); // Limit Input to Range
      float nX1 = _x1Scale * (valueX + 1f);
      float nY = _yScale * (-valueY + 1f);
      g.DrawLine(_penRE, rect.Left + nX1 - c_penSize * 2, nY, rect.Left + nX1 + c_penSize * 2, nY); // Cross Hor
      g.DrawLine(_penRE, rect.Left + nX1, rect.Top + nY - c_penSize * 2, rect.Left + nX1, rect.Top + nY + c_penSize * 2); // Cross Ver

      // Aileron
      var valueA = Math.Max(Math.Min(ValueAileron, 1), -1); // Limit Input to Range
      nY = _x2Scale * -(valueA - 1f);
      g.DrawLine(_penAileron, rect.Left + rect.Width / 3f * 2, nY, rect.Right, rect.Height - nY); // Bar

      // a Hor Divider
      g.DrawLine(Pens.Gray, rect.Left + rect.Width / 3f * 2, rect.Top, rect.Left + rect.Width / 3 * 2, rect.Bottom);

      g.Restore(saved);
    }

    // Capture Resizing of the Control
    // Maintain the AutoSizing demands
    private void UC_Scale_Resize( object sender, EventArgs e )
    {
      if (AutoSizeHeight && AutoSizeWidth) { this.Size = _minSize; }
      else if (AutoSizeHeight) { this.Height = (int)(this.Width / _aspect); }
      else if (AutoSizeWidth) { this.Width = (int)(this.Height * _aspect); }
      RecalcPrimitives();
    }

    private void UC_Scale_ClientSizeChanged( object sender, EventArgs e )
    {
      RecalcPrimitives();
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
      rect.Inflate(_border);
      _x1Scale = (rect.Width / _aspect * 2) / 2f; // first 2/3rd is RE
      _x2Scale = (rect.Width / _aspect) / 2f; // second 1/3rd is Aileron
      _yScale = rect.Height / 2f;
    }

  }
}

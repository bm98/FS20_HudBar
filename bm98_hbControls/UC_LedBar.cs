using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using bm98_hbControls.Properties;


namespace bm98_hbControls
{
  /// <summary>
  /// Displays a VU type horizontal LED bar with selectable number of elements
  /// Display value is % 0..100 given by I
  /// </summary>
  public partial class UC_LedBar : UserControl
  {
    private readonly Size _minSize = new Size( 20, 10 );
    private readonly float _aspect = 2; // Height*_aspect => Width

    // max supported for now 
    private const int c_maxElements = 20;

    // number of elements
    private int _nElements = 5;

    // the Bar value
    private int _barPrct = 0;
    // % per element ( 100 / #elements)
    private int _elStep = 0;

    // true when HoldMax mode is on
    private bool _holdMax = false;
    // true when holding the max element  (must display then)
    private bool _holdingMaxElement = false;

    private bool _state = false;
    // 0 based start index of a range
    private int _elY = 1;
    private int _elR = 2;

    private Image _imageON = Resources.LED_BarElement_on; // one Element (width / #elements)
    private Image _imageOFF = Resources.LED_BarElement_off;
    private readonly RectangleF _srcRect;

    private RectangleF[] _elRect = new RectangleF[c_maxElements];
    private ImageAttributes[] _iaRefs = new ImageAttributes[c_maxElements]; // holding current disp property

    private static readonly ImageAttributes _imageAttributes_off = new ImageAttributes( ); // Dark
    private static readonly ImageAttributes _imageAttributes_idle = new ImageAttributes( ); // Blueish (dimm)
    private static readonly ImageAttributes _imageAttributesG = new ImageAttributes( ); // Green
    private static readonly ImageAttributes _imageAttributesY = new ImageAttributes( ); // Amber
    private static readonly ImageAttributes _imageAttributesR = new ImageAttributes( ); // Red

    private bool _pAutoSizeHeight = false;
    private bool _pAutoSizeWidth = false;

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_LedBar( )
    {
      InitializeComponent( );

      this.BackgroundImage = null;
      this.ForeColor = Color.White;
      _srcRect = new RectangleF( new Point( 0, 0 ), _imageON.Size );

      RecalcElementIndex( _nElements );
      RecalcElementState( _barPrct );

      // Setup the coloring for the BAR elements
      // recolor components (base image is pure white with greyscale glow)
      Color col = Color.FromArgb( 32, 32, 32 );
      float a = col.A / 255f;
      float r = col.R / 255f;
      float g = col.G / 255f;
      float b = col.B / 255f;

      float[][] colorMatrixElements = {
         new float[] {r,  0,  0,  0, 0},     // red scaling factor
         new float[] {0,  g,  0,  0, 0},     // green scaling factor
         new float[] {0,  0,  b,  0, 0},     // blue scaling factor
         new float[] {0,  0,  0,  a, 0},     // alpha scaling factor
         new float[] {0 , 0 , 0,  0, 1}};    // three additives R G B
      _imageAttributes_off.SetColorMatrix( new ColorMatrix( colorMatrixElements ), ColorMatrixFlag.Default, ColorAdjustType.Bitmap );

      // Range Idle (Dimm Blueish)
      col = Color.MidnightBlue;
      a = col.A / 255f;
      r = col.R / 255f;
      g = col.G / 255f;
      b = col.B / 255f;

      float[][] colorMatrixElementsIdle = {
         new float[] {r,  0,  0,  0, 0},     // red scaling factor
         new float[] {0,  g,  0,  0, 0},     // green scaling factor
         new float[] {0,  0,  b,  0, 0},     // blue scaling factor
         new float[] {0,  0,  0,  a, 0},     // alpha scaling factor
         new float[] {0 , 0 , 0,  0, 1}};    // three additives R G B
      _imageAttributes_idle.SetColorMatrix( new ColorMatrix( colorMatrixElementsIdle ), ColorMatrixFlag.Default, ColorAdjustType.Bitmap );


      // Range 1 (Green)
      col = Color.LimeGreen;
      a = col.A / 255f;
      r = col.R / 255f;
      g = col.G / 255f;
      b = col.B / 255f;

      float[][] colorMatrixElementsR1 = {
         new float[] {r,  0,  0,  0, 0},     // red scaling factor
         new float[] {0,  g,  0,  0, 0},     // green scaling factor
         new float[] {0,  0,  b,  0, 0},     // blue scaling factor
         new float[] {0,  0,  0,  a, 0},     // alpha scaling factor
         new float[] {0 , 0 , 0,  0, 1}};    // three additives R G B
      _imageAttributesG.SetColorMatrix( new ColorMatrix( colorMatrixElementsR1 ), ColorMatrixFlag.Default, ColorAdjustType.Bitmap );

      // Range 2 (Amber)
      col = Color.Gold;
      a = col.A / 255f;
      r = col.R / 255f;
      g = col.G / 255f;
      b = col.B / 255f;

      float[][] colorMatrixElementsR2 = {
         new float[] {r,  0,  0,  0, 0},     // red scaling factor
         new float[] {0,  g,  0,  0, 0},     // green scaling factor
         new float[] {0,  0,  b,  0, 0},     // blue scaling factor
         new float[] {0,  0,  0,  a, 0},     // alpha scaling factor
         new float[] {0 , 0 , 0,  0, 1}};    // three additives R G B
      _imageAttributesY.SetColorMatrix( new ColorMatrix( colorMatrixElementsR2 ), ColorMatrixFlag.Default, ColorAdjustType.Bitmap );

      // Range 3 (Redish)
      col = Color.OrangeRed;
      a = col.A / 255f;
      r = col.R / 255f;
      g = col.G / 255f;
      b = col.B / 255f;

      float[][] colorMatrixElementsR3 = {
         new float[] {r,  0,  0,  0, 0},     // red scaling factor
         new float[] {0,  g,  0,  0, 0},     // green scaling factor
         new float[] {0,  0,  b,  0, 0},     // blue scaling factor
         new float[] {0,  0,  0,  a, 0},     // alpha scaling factor
         new float[] {0 , 0 , 0,  0, 1}};    // three additives R G B
      _imageAttributesR.SetColorMatrix( new ColorMatrix( colorMatrixElementsR3 ), ColorMatrixFlag.Default, ColorAdjustType.Bitmap );

    }

    private void UC_LedBar_Load( object sender, EventArgs e )
    {
      RecalcPrimitives( );
    }

    /// <summary>
    /// LED State of the control
    /// </summary>
    [Description( "Illumination state" ), Category( "Behavior" )]
    public bool OnState {
      get => _state;
      set {
        if (value == _state) return; // already there

        _state = value;
        this.Refresh( );
      }
    }

    /// <summary>
    /// Number of bar elements 3..9
    /// </summary>
    [Description( "Number of elements" ), Category( "Appearance" )]
    public int BarElements {
      get => _nElements;
      set {
        if (value == _nElements) return;

        _nElements = (value < 3) ? 3 : (value > c_maxElements) ? c_maxElements : value; // clamp 3...c_maxElements
        // need to recalc and redraw all
        RecalcPrimitives( );
        RecalcElementIndex( _nElements );
        RecalcElementState( _barPrct );
        this.Refresh( );
      }
    }


    /// <summary>
    /// Set the Bar Value 0..100 [%]
    /// </summary>
    [Description( "% Value of the LED Bar" ), Category( "Data" )]
    public int BarPrct {
      get => this._barPrct;
      set {
        if (value == _barPrct) return;

        _barPrct = (value < 0) ? 0 : (value > 100) ? 100 : value; // clamp 0...100

        RecalcElementState( _barPrct );
        this.Refresh( );
      }
    }

    /// <summary>
    /// When true the max element will remain illuminated when it was hit once
    /// (switch off by issue ClearMax())
    /// else the elements are illuminated as long as the value persists
    /// </summary>
    [Description( "Holding Max Value" ), Category( "Behavior" )]
    public bool HoldMax {
      get => _holdMax;
      set {
        if (value == _holdMax) return;

        _holdMax = value;
        if (!HoldMax) { ClearMax( ); }
        this.Refresh( );
      }
    }

    /// <summary>
    /// Clear the Max Hold once - if the value is still 1 it will reappear immediately...
    /// </summary>
    public void ClearMax( )
    {
      _holdingMaxElement = false;
      RecalcElementState( _barPrct );
      this.Refresh( );
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
        UC_LedBar_Resize( this, null );
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
        UC_LedBar_Resize( this, null );
      }
    }



    private void UC_LedBar_Paint( object sender, PaintEventArgs e )
    {
      Graphics g = e.Graphics;

      var save = g.BeginContainer( );
      g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
      g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

      if (_state) {
        // based on Value 0...1 
        // 0= left most bar, dimm visible
        // >0..<1 = left to right except right most illuminate according to value
        // 1= illuminate 

        for (int i = 0; i < _nElements; i++) {
          g.DrawImage( _imageON, AsPointF_Array( _elRect[i] ), _srcRect, GraphicsUnit.Pixel, _iaRefs[i] );
        }
      }
      else {
        // OFF
        for (int i = 0; i < _nElements; i++) {
          g.DrawImage( _imageOFF, AsPointF_Array( _elRect[i] ), _srcRect, GraphicsUnit.Pixel, _imageAttributes_off );
        }
      }
      g.EndContainer( save );

    }

    // recalculate the drawing xy's 
    private PointF[] AsPointF_Array( RectangleF rect )
    {
      // returm upper left, upper right, lower left Pts as array
      return new PointF[3] { new PointF( rect.Left, rect.Top ), new PointF( rect.Right + 0.5f, rect.Top ), new PointF( rect.Left, rect.Bottom ) };
    }

    // recalculate the bar element rectangles
    private void RecalcPrimitives( )
    {
      // bar has N elements which should fit into the client rectangle
      float el_width = this.ClientRectangle.Width / (float)_nElements;

      float x = 0;
      for (int i = 0; i < _nElements; i++, x += el_width) {
        _elRect[i] = new RectangleF( x, 0, el_width, this.ClientRectangle.Height );
      }
    }

    // recalculate the bar element color
    private void RecalcElementState( int value )
    {
      for (int i = 0; i < _nElements; i++) _iaRefs[i] = _imageAttributes_off;
      // 0..1
      if (value <= 0) {
        // all off but the lowest which is idle
        _iaRefs[0] = _imageAttributes_idle;
      }
      else if (value >= 100) {
        // trigger hold only at 100
        if (_holdMax) {
          _holdingMaxElement = true;
        }

        // all on
        for (int i = 0; i < _nElements; i++) {
          if (i >= _elR) {
            _iaRefs[i] = _imageAttributesR;
          }
          else if (i >= _elY) {
            _iaRefs[i] = _imageAttributesY;
          }
          else {
            _iaRefs[i] = _imageAttributesG;
          }
        }
      }
      else {
        // a value between >0 and <100, cannot illuminate Max indicator
        for (int i = 0; i < (_nElements - 1); i++) {
          if (value > (i * _elStep)) {
            // hit element
            if (i >= _elY) {
              _iaRefs[i] = _imageAttributesY;
            }
            else {
              _iaRefs[i] = _imageAttributesG;
            }
          }
        }
      }

      // maintain Max indicator
      if (_holdingMaxElement) {
        _iaRefs[_nElements - 1] = _imageAttributesR;
      }
    }

    // recalculate the bar element start index of G,Y,R
    private void RecalcElementIndex( int nElements )
    {
      // % per element ex the Max indicator
      _elStep = (int)Math.Round( 99.0 / (nElements - 1) );

      // G starts always at 0
      // R is always the topmost
      _elR = nElements - 1;

      switch (nElements) {         // 01234567890123456789
        case 3: _elY = 1; break;   // GYR
        case 4: _elY = 2; break;   // GGYR
        case 5: _elY = 3; break;   // GGGYR
        case 6: _elY = 3; break;   // GGGYYR
        case 7: _elY = 4; break;   // GGGGYYR
        case 8: _elY = 4; break;   // GGGGYYYR
        case 9: _elY = 5; break;   // GGGGGYYYR
        case 10: _elY = 6; break;  // GGGGGGYYYR
        case 11: _elY = 7; break;  // GGGGGGGYYYR
        case 12: _elY = 8; break;  // GGGGGGGGYYYR
        case 13: _elY = 9; break;  // GGGGGGGGGYYYR
        case 14: _elY = 10; break; // GGGGGGGGGGYYYR
        case 15: _elY = 11; break; // GGGGGGGGGGGYYYR
        case 16: _elY = 12; break; // GGGGGGGGGGGGYYYR
        case 17: _elY = 13; break; // GGGGGGGGGGGGGYYYR
        case 18: _elY = 14; break; // GGGGGGGGGGGGGGYYYR
        case 19: _elY = 15; break; // GGGGGGGGGGGGGGGYYYR
        case 20: _elY = 16; break; // GGGGGGGGGGGGGGGGYYYR

        default: _elY = 1; break;  // GYR
      }
    }

    private void UC_LedBar_Resize( object sender, EventArgs e )
    {
      if (AutoSizeHeight && AutoSizeWidth) { this.Size = _minSize; }
      else if (AutoSizeHeight) { this.Height = (int)(this.Width / _aspect); }
      else if (AutoSizeWidth) { this.Width = (int)(this.Height * _aspect); }

      RecalcPrimitives( );
    }

    private void UC_LedBar_ClientSizeChanged( object sender, EventArgs e )
    {
      this.Invalidate( this.ClientRectangle );
    }

  }
}

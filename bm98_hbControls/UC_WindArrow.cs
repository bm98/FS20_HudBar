using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace bm98_hbControls
{
  /// <summary>
  /// User Control to draw a Wind Direction Arrow with regards to the heading an object has
  /// i.e. the arrow should represent the wind direction seen from the face of said object (e.g. aircraft)
  ///  Arrow direction is controlled by
  ///   Wind direction (deg where the wind blows from)
  ///   Heading (deg face heading of an object that is facing the wind)
  ///   
  ///  Properties should be 
  ///   WindDirection (float)
  ///   Heading (float)
  ///   ForeColor of the arrow (Color)
  ///   BackColor of the control (Color)
  ///   AutoSizeHorizontal flag (bool)
  ///   AutoSizeVertical flag (bool)
  ///   Enabled flag (bool)
  ///   
  /// </summary>
  public partial class UC_WindArrow : UserControl
  {
    private readonly Size _minSize = new Size(10,10);

    /// <summary>
    /// Properties
    /// </summary>
    private int _pHeadingTo_deg = 0;
    private int _pDirectionFrom_deg =0;
    private bool _pAutoSizeHeight = false;
    private bool _pAutoSizeWidth = false;

    private Pen _pen = new Pen(Color.White);
    private int _penSize = 1;
    private int _arrowLenHalf = 20;
    private int _arrowWidth = 4;
    private int _arrowHeight = 5;

    /// <summary>
    /// Wind Direction From [true deg]
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Wind Direction From [true deg]" ), Category( "Data" )]
    public int DirectionFrom {
      get => _pDirectionFrom_deg;
      set {
        _pDirectionFrom_deg = value;
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Object Heading To [true deg]
    /// </summary>
    [DefaultValue( 0 )]
    [Description( "Object Heading To [true deg]" ), Category( "Data" )]
    public int Heading {
      get => _pHeadingTo_deg;
      set {
        _pHeadingTo_deg = value;
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
        UC_WindArrow_Resize( this, null );
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
        UC_WindArrow_Resize( this, null );
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
    public UC_WindArrow( )
    {
      InitializeComponent( );

      RecalcPrimitives( );
    }

    /// <summary>
    /// Get;Set; Foreground Colort
    /// </summary>
    public override Color ForeColor {
      get => base.ForeColor;
      set {
        if ( value == base.ForeColor ) return; // already set
        base.ForeColor = value;
        _pen?.Dispose( );
        _pen = new Pen( value, _penSize );
      }
    }

    // Capture Paint Event - draw stuff
    private void UC_WindArrow_Paint( object sender, PaintEventArgs e )
    {
      if ( _pen == null ) return;
      if ( !this.Enabled ) return;

      var g = e.Graphics;

      var saved = g.Save();
      g.SmoothingMode = SmoothingMode.HighQuality;

      // Set world transform of graphics object to translate.
      var rect = this.ClientRectangle;
      var shift = new Size( ( rect.X + rect.Width / 2 ), ( rect.Y + rect.Height / 2 ) );
      g.RotateTransform( ( ( DirectionFrom - Heading ) + 180 ) % 360, MatrixOrder.Append );
      g.TranslateTransform( shift.Width, shift.Height, MatrixOrder.Append );

      g.DrawLine( _pen, 0, _arrowLenHalf + 1, 0, -_arrowLenHalf );
      g.DrawLine( _pen, 0 - _arrowWidth, -_arrowLenHalf + _arrowHeight, 0, -_arrowLenHalf );
      g.DrawLine( _pen, 0 + _arrowWidth, -_arrowLenHalf + _arrowHeight, 0, -_arrowLenHalf );

      g.Restore( saved );
    }


    // Capture Resizing of the Control
    // Maintain the AutoSizing demands
    private void UC_WindArrow_Resize( object sender, EventArgs e )
    {
      if ( AutoSizeHeight && AutoSizeWidth ) { this.Size = _minSize; }
      else if ( AutoSizeHeight ) { this.Height = this.Width; }
      else if ( AutoSizeWidth ) { this.Width = this.Height; }
    }

    private void UC_WindArrow_ClientSizeChanged( object sender, EventArgs e )
    {
      RecalcPrimitives( );
      this.Invalidate( this.ClientRectangle );
    }

    // recalculate our internal primitives (avoiding cal for every paint)
    private void RecalcPrimitives( )
    {
      // calc the Arrow dimensions
      var shorter = (this.ClientRectangle.Height<this.ClientRectangle.Width)? this.ClientRectangle.Height:this.ClientRectangle.Width;
      _arrowLenHalf = ( shorter - 2 ) / 2; // 2px border
      _arrowHeight = (int)( _arrowLenHalf / 1.5 );
      _arrowWidth = (int)( _arrowLenHalf / 2.0 );
      // line size depends on the size of the arrow
      var pSize = 1;
      if ( _arrowLenHalf > 25 ) {
        pSize = 5;
      }
      else if ( _arrowLenHalf > 10 ) {
        pSize = 3;
      }
      if ( pSize!= _penSize ) {
        _penSize = pSize;
        // create a new Pen
        _pen.Dispose( );
        _pen = new Pen( this.ForeColor, _penSize );
      }
    }


  }
}

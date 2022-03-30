using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bm98_hbControls
{
  /// <summary>
  /// A User Label Control
  ///  it allows to AutoSize on either Width or Height 
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
  public partial class UC_Label : UserControl
  {
    private readonly Size _minSize = new Size(10,10);

    // some shorts to eval the Alignment  (x & c_xy)>0  gets the H or V StringAlignment
    private readonly ContentAlignment c_hLeft = ContentAlignment.TopLeft | ContentAlignment.MiddleLeft | ContentAlignment.BottomLeft;
    private readonly ContentAlignment c_hCenter = ContentAlignment.TopCenter | ContentAlignment.MiddleCenter | ContentAlignment.BottomCenter;
    //private readonly ContentAlignment c_hRight = ContentAlignment.TopRight | ContentAlignment.MiddleRight | ContentAlignment.BottomRight;

    private readonly ContentAlignment c_vTop = ContentAlignment.TopLeft | ContentAlignment.TopCenter | ContentAlignment.TopRight;
    private readonly ContentAlignment c_vMiddle = ContentAlignment.MiddleLeft | ContentAlignment.MiddleCenter | ContentAlignment.MiddleRight;
    //private readonly ContentAlignment c_vBottom = ContentAlignment.BottomLeft | ContentAlignment.BottomCenter | ContentAlignment.BottomRight;

    // Hor Alignment
    private StringAlignment HAlignment( ContentAlignment ca )
    {
      return ( ( ca & c_hLeft ) > 0 ) ? StringAlignment.Near : ( ( ca & c_hCenter ) > 0 ) ? StringAlignment.Center : StringAlignment.Far;
    }
    // Ver Alignment
    private StringAlignment VAlignment( ContentAlignment ca )
    {
      return ( ( ca & c_vTop ) > 0 ) ? StringAlignment.Near : ( ( ca & c_vMiddle ) > 0 ) ? StringAlignment.Center : StringAlignment.Far;
    }


    /// <summary>
    /// Properties
    /// </summary>
    private string _pText = "";
    private ContentAlignment _pTextAlignment = ContentAlignment.MiddleCenter;


    private Brush _txtBrush = new SolidBrush ( Color.Black );
    private StringFormat _txtFormat = new StringFormat( StringFormatFlags.NoWrap );
    private Size _txtSize = new Size();

    // Evaluate the new Text Size
    private void SetTextSize( )
    {
      using ( var g = this.CreateGraphics( ) ) {
        var s = g.MeasureString( this.Text, this.Font );
        s += new Size( 3, 3 ); // leave some room for the text
        _txtSize = s.ToSize( );
      }
    }



    /// <summary>
    /// Get; Set; Text property of this Label
    /// </summary>
    [Browsable( true )]
    [DesignerSerializationVisibility( DesignerSerializationVisibility.Visible )]
    [EditorBrowsable( EditorBrowsableState.Always )]
    [DefaultValue( "UC_Label" )]
    [Description( "Text property of this Label" ), Category( "Appearance" )]
    public override string Text {
      get => _pText;
      set {
        _pText = value;
        SetTextSize( );
        UC_Label_Resize( this, null );
      }
    }

    /// <summary>
    /// Gets or sets the alignment of text in the label.
    /// </summary>
    [DefaultValue( ContentAlignment.TopLeft )]
    [Description( "Gets or sets the alignment of text in the label." ), Category( "Appearance" )]
    public ContentAlignment TextAlign {
      get => _pTextAlignment;
      set {
        _pTextAlignment = value;
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// Get;Set; Foreground Color
    /// </summary>
    public override Color ForeColor {
      get => base.ForeColor;
      set {
        if ( value == base.ForeColor ) return; // already set
        base.ForeColor = value;
        _txtBrush.Dispose( );
        _txtBrush = new SolidBrush( this.ForeColor );
        this.Invalidate( this.ClientRectangle );
      }
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_Label( )
    {
      InitializeComponent( );

      _txtFormat.Trimming = StringTrimming.None;

      _txtBrush.Dispose( );
      _txtBrush = new SolidBrush( this.ForeColor );
      SetTextSize( );
      UC_Label_Resize( this, null );

    }

    // Improved text rendering as per:
    // https://stackoverflow.com/questions/2609520/how-to-make-text-labels-smooth
    private TextRenderingHint _hint = TextRenderingHint.SystemDefault;
    public TextRenderingHint TextRenderingHint {
      get { return this._hint; }
      set { this._hint = value; }
    }


    private void UC_Label_AutoSizeChanged( object sender, EventArgs e )
    {
      if ( this.AutoSize ) {
        this.Size = _txtSize;
      }
      else {
        //this.Size = _minSize;
      }
      UC_Label_Resize( this, null );
    }

    // Capture Resizing of the Control
    // Maintain the AutoSizing demands
    private void UC_Label_Resize( object sender, EventArgs e )
    {
      if ( AutoSize ) { this.Size = _txtSize; }
      this.Invalidate( this.ClientRectangle );
    }

    // Paint Text with given Attributes
    private void UC_Label_Paint( object sender, PaintEventArgs e )
    {
      var g = e.Graphics;
      var saved = g.Save();
      g.SmoothingMode = SmoothingMode.HighQuality;
      g.TextRenderingHint = TextRenderingHint;
      var rect = this.ClientRectangle;

      // setup the Text Alignment within the Rectangle
      _txtFormat.Alignment = HAlignment( _pTextAlignment );
      _txtFormat.LineAlignment = VAlignment( _pTextAlignment );

      g.DrawString( this.Text, this.Font, _txtBrush, rect, _txtFormat );

      g.Restore( saved );
    }

    private void UC_Label_ForeColorChanged( object sender, EventArgs e )
    {
      _txtBrush.Dispose( );
      _txtBrush = new SolidBrush( this.ForeColor );
      this.Invalidate( this.ClientRectangle );
    }

    private void UC_Label_FontChanged( object sender, EventArgs e )
    {
      SetTextSize( );
      UC_Label_Resize( this, null );
    }

    private void UC_Label_ClientSizeChanged( object sender, EventArgs e )
    {
      this.Invalidate( this.ClientRectangle );
    }
  }
}

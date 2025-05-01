using System.Drawing;


namespace bm98_Map.Drawing.DispItems
{
  /// <summary>
  /// BaseClass to implement items to draw
  /// supports sub items and a Key to manipulate from the DisplayList
  /// SubItems are drawn OnTop of this Item
  /// provide some basic features for drawing
  /// 
  /// </summary>
  internal abstract class DisplayItem : IDrawing
  {
    /// <summary>
    /// Apply a Transform to turn the graphics around the point by an angle
    ///  The Graphics translation is not changed by this action
    /// </summary>
    /// <param name="g">Graphics context</param>
    /// <param name="point">Turn point</param>
    /// <param name="angleDeg">Turn angle</param>
    internal static void TurnAroundPoint(Graphics g, Point point, float angleDeg )
    {
      if ( angleDeg!=0 ) {
        g.TranslateTransform( point.X, point.Y );
        g.RotateTransform( angleDeg );
        g.TranslateTransform( -point.X, -point.Y );
      }
    }

    /// <summary>
    /// Access the SubItem List (a DisplayList)
    /// Manipulate elements via this property only
    /// </summary>
    public DisplayList SubItemList { get; } = new DisplayList( );

    /// <summary>
    /// A key to access this item in the DisplayList
    /// </summary>
    public int Key { get; set; } = -1;

    /// <summary>
    /// Wether or not the item should be drawn
    /// </summary>
    public bool Active { get; set; } = true;

    //  Basic Drawing properties one can use

    /// <summary>
    /// The outer bounds for the item
    /// </summary>
    public Rectangle Rectangle { get; set; } = new Rectangle( );
    /// <summary>
    /// A Pen to use for lines
    /// </summary>
    public Pen Pen { get; set; } = Pens.Pink; // default Pen (alarm color if not set)
    /// <summary>
    /// A Brush to use for Draw Fills
    /// </summary>
    public Brush FillBrush { get; set; } = null; // default Brush (alarm color if not set)

    /// <summary>
    /// A Brush to use for Draw Fills Decorations
    /// </summary>
    public Brush FillBrushAlt { get; set; } = null; // default Brush (alarm color if not set)

    // Text 

    /// <summary>
    /// A Brush to use for Text Writing
    /// </summary>
    public Brush TextBrush { get; set; } = Brushes.Pink; // default Brush (alarm color if not set)
    /// <summary>
    /// A Pen to use for Text Box Frame Rect
    /// </summary>
    public Pen TextRectPen { get; set; } = Pens.Pink; // default Pen (alarm color if not set)
    /// <summary>
    /// A Brush to use for Text Background Rect
    /// </summary>
    public Brush TextRectFillBrush { get; set; } = Brushes.Pink; // default Brush (alarm color if not set)
    /// <summary>
    /// A font to use for text
    /// </summary>
    public Font Font { get; set; } = SystemFonts.DialogFont;
    /// <summary>
    /// A string to use for text
    /// </summary>
    public string String { get; set; } = "undef";

    // Coordinate Inputs

    /// <summary>
    /// A Coordinate Point
    /// </summary>
    public CoordLib.LatLon CoordPoint { get; set; } = new CoordLib.LatLon( );

    /// <summary>
    /// cTor: empty
    /// </summary>
    public DisplayItem( ) { }

    /// <summary>
    /// cTor: copy from
    ///  we copy refs and do not create new object other than the subitem list
    /// </summary>
    /// <param name="other">The object to create this from</param>
    public DisplayItem( DisplayItem other )
    {
      this.Key = other.Key;
      this.FillBrush = other.FillBrush;
      this.Rectangle = other.Rectangle;
      this.Pen = other.Pen;
      this.TextRectPen = other.TextRectPen;
      this.TextRectFillBrush = other.TextRectFillBrush;
      this.TextBrush = other.TextBrush;
      this.Font = other.Font;
      this.String = other.String;
      this.SubItemList.AddSubitems( other.SubItemList );
    }

    /// <summary>
    /// Draw all of this item
    /// </summary>
    /// <param name="g">Graphics Context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    public virtual void Paint( Graphics g, IVPortPaint vpRef )
    {
      PaintThis( g, vpRef ); // paint this item
      SubItemList.Paint( g, vpRef ); // paint all below this item
    }

    /// <summary>
    /// Paint this item
    /// Override in the implementation as needed (base does not do anything)
    /// </summary>
    /// <param name="g">Graphics Context</param>
    /// <param name="vpRef">Viewport access for paint events</param>
    protected virtual void PaintThis( Graphics g, IVPortPaint vpRef ) { }

  }
}

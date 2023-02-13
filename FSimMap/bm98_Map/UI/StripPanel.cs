using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;

using static dNetBm98.XColor;

namespace bm98_Map.UI
{
  /// <summary>
  /// A FlowLayoutPanel supporting Label Strips and handling thereof
  ///   The panel left position must be set by the owner using the Y Property
  ///   The panel gets the min. width from the max width argument (fixed widht)
  ///   Height is set according to items and starts to scroll when the max height is reached
  ///   The panel is stacked on the bottom Y coord when commited
  ///   A single strip item is selectable if set when adding and will get a frame when selected
  ///   Clicking the title will unselect all strips and raise an ItemClicked event
  ///   Clicking a strip will unselect all and select the strip and raise an ItemClicked event
  ///   Clicing an empty area of the panel will raise an EmptyClicked Event
  ///   Use InitUpdate, AddItem, CommitUpdate to update with layout disabled (less flicker)
  ///   Use ClearItems to remove all but the title strip
  ///   Add a SubTitle when needed (never selectable)
  /// </summary>
  internal class StripPanel : FlowLayoutPanel
  {
    // default colors
    private readonly Color _foreCol = Color.LightYellow;
    private readonly Color _foreColTitle = Color.NavajoWhite;
    private readonly Color _backCol = Color.FromArgb( 99, 85, 60 ); // brown

    // The title label - first one, stays permanent
    private StripLabel _lTitle;
    // zebra colors for panel stripes
    private Color _f1; // forecolor 
    private Color _f2;
    private Color _b1; // backcolor
    private Color _b2;

    /// <summary>
    /// Fired when the user clicks a clickable item
    ///  Sender is the StripLabel which was selected
    /// </summary>
    [Category( "Action" )]
    [Description( "Fires when a strip item is selected" )]
    public event EventHandler<EventArgs> ItemSelected;
    private void OnItemSelected( object label, EventArgs e )
    {
      ItemSelected?.Invoke( label, e );
    }

    /// <summary>
    /// Fired when the user clicks a clickable item
    ///  Sender is the Label which was clicked
    /// </summary>
    [Category( "Action" )]
    [Description( "Fires when a strip item is clicked" )]
    public event EventHandler<EventArgs> ItemClicked;
    private void OnItemClicked( object label, EventArgs e )
    {
      ItemClicked?.Invoke( label, e );
    }

    /// <summary>
    /// Fired when the user clicks an empty area
    ///  Sender is the Panel
    /// </summary>
    [Category( "Action" )]
    [Description( "Fires when an empty area is clicked" )]
    public event EventHandler<EventArgs> EmptyClicked;
    private void OnEmptyClicked( object label, EventArgs e )
    {
      EmptyClicked?.Invoke( label, e );
    }

    /// <summary>
    /// Visible property, also raises the control to front when visible
    /// </summary>
    public new bool Visible {
      get => base.Visible;
      set {
        base.Visible = value;
        if (value) { this.BringToFront( ); }
      }
    }

    // base init for all ctors. (set base colors, font before calling)
    private void InitPanel( Size maxSize, string title )
    {
      base.ForeColor = _foreCol;
      base.BackColor = _backCol;
      base.Anchor = AnchorStyles.Bottom | AnchorStyles.Left; // resize anchor is bottom
      base.AutoScroll = true;
      base.FlowDirection = FlowDirection.TopDown;
      base.AutoSize = false; // will be set true after loading
      base.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      base.WrapContents = false;
      base.MaximumSize = maxSize;
      base.MinimumSize = new Size( maxSize.Width, 10 ); // take the width
      base.Size = base.MinimumSize;
      base.Visible = false; // initially invisible
      // init zebra colors
      _f1 = this.ForeColor;
      _f2 = _f1.Dimmed( 10 );
      _b1 = this.BackColor.Dimmed( 20 );
      _b2 = _b1.Dimmed( 30 );
      // Add title item
      _lTitle = new StripLabel( ) {
        ForeColor = _foreColTitle,
        Text = title,
      };
      base.Controls.Add( _lTitle );
      // handlers
      _lTitle.Click += Strip_Clicked;
      _lTitle.Selected += Strip_Selected;

      base.Click += StripPanel_Clicked;
    }

    /// <summary>
    /// cTor: Define max size and initial title, Base Colors and Font
    /// </summary>
    /// <param name="maxSize">Max Size of the panel</param>
    /// <param name="title">Initial title strip content</param>
    /// <param name="back">Background color</param>
    /// <param name="fore">Font color for strip content</param>
    /// <param name="foreTitle">Font color for the titles</param>
    /// <param name="font">Strip font</param>
    public StripPanel( Size maxSize, string title, Color back, Color fore, Color foreTitle, Font font )
    {
      base.Font = new Font( font.FontFamily, font.Size, font.Style );
      _backCol = back;
      _foreCol = fore;
      _foreColTitle = foreTitle;
      InitPanel( maxSize, title );
    }

    /// <summary>
    /// cTor: Define max size and initial title
    /// </summary>
    /// <param name="maxSize">Max Size of the panel</param>
    /// <param name="title">Initial title strip content</param>
    public StripPanel( Size maxSize, string title )
    {
      // use default font and colors
      base.Font = new Font( "Consolas", 11.25f, FontStyle.Bold );
      InitPanel( maxSize, title );
    }


    /// <summary>
    /// Get;Set: Content of the title strip of the panel
    /// </summary>
    public string Title { get { return _lTitle.Text; } set { _lTitle.Text = value; } }

    /// <summary>
    /// Add a SubTitle (will be treated as item and removed when cleared), not selectable
    /// </summary>
    /// <param name="content">The subtitle content</param>
    public void AddSubTitle( string content )
    {
      var l = new StripLabel( ) {
        ForeColor = _foreColTitle,
        Text = content,
      };
      base.Controls.Add( l );
    }

    /// <summary>
    /// Add one Item to the panel
    /// </summary>
    /// <param name="content">The content string</param>
    /// <param name="tag">A tag</param>
    /// <param name="selectable">True if a Click is expected</param>
    public void AddItem( string content, object tag, bool selectable )
    {
      var num = base.Controls.Count;

      var l = new StripLabel( ) {
        Text = content,
        Tag = tag,
        AutoEllipsis = true,
        Selectable = selectable,
        // use zebra colors
        ForeColor = (num % 2 == 0) ? _f1 : _f2,
        BackColor = (num % 2 == 0) ? _b1 : _b2,
      };
      if (selectable) {
        l.Click += Strip_Clicked;
        l.Selected += Strip_Selected;
      }
      base.Controls.Add( l );
      // if there is a clickable one - set the title clickable too (cursor)
      if (selectable) { _lTitle.Cursor = Cursors.Hand; }
    }


    /// <summary>
    /// Clear content items
    /// </summary>
    public void ClearItems( )
    {
      // clear all controls from the FLP but the title
      while (base.Controls.Count > 1) {
        var cx = base.Controls[1] as StripLabel;
        base.Controls.Remove( cx );
        cx.Click -= Strip_Clicked;
        cx.Selected -= Strip_Selected;
        cx.Dispose( );
      }
    }

    // temp store while mass updating
    private bool _visSafe = false;

    /// <summary>
    /// Initialize a mass update of the contents
    /// </summary>
    public void InitUpdate( )
    {
      _visSafe = base.Visible;
      base.SuspendLayout( );
      base.AutoSize = false;
    }

    /// <summary>
    /// Commit and refresh the updates
    /// </summary>
    /// <param name="bottomY">The bottom Y coord of the placement</param>
    public void CommitUpdate( int bottomY )
    {
      // prepare to show
      base.AutoSize = true;
      base.ResumeLayout( );
      base.Top = bottomY - base.Height - 5;
      base.Visible = _visSafe;
    }

    // manages the selected frame of an FLP, selected=null deselects all
    private void LabelFLP_SetSelected( StripLabel selected )
    {
      // clear any but never fail
      try {
        for (int i = 1; i < base.Controls.Count; i++) { (base.Controls[i] as StripLabel).ItemSelected = false; }
        if (selected == null) return; // unselect all
        // select one
        selected.ItemSelected = true;
      }
      catch { }
    }

    // a strip item was selected
    private void Strip_Selected( object sender, EventArgs e )
    {
      OnItemSelected( sender, e );
    }

    // a strip label was clicked
    private void Strip_Clicked( object sender, EventArgs e )
    {
      if (sender.Equals( _lTitle )) {
        LabelFLP_SetSelected( null );
      }
      else {
        LabelFLP_SetSelected( sender as StripLabel );
      }
      OnItemClicked( sender, e );
    }

    // panel clicked
    private void StripPanel_Clicked( object sender, EventArgs e )
    {
      OnEmptyClicked( this, e );
    }




  }
}

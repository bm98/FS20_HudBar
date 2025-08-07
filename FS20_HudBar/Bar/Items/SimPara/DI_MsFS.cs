using System;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using static FS20_HudBar.GUI.GUI_Colors;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using FShelf.LandPerf;


namespace FS20_HudBar.Bar.Items
{
  class DI_MsFS : DispItem
  {
    // module Ref
    private readonly ImageToolTip _tTip = new ImageToolTip( );

    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.MSFS;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "MSFS";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "MSFS Status";

    private readonly B_Base _label;

    // dt to next review and act on mouse
    private DateTime _nextReview = DateTime.MinValue;

    public DI_MsFS( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      //TText = "Click to change the text appearance\nSteps through Bright, Dim, Dark ";
      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.Ad;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );

      _label.ButtonClicked += _label_ButtonClicked;
      // map mouse events to the outer container (this)
      _label.MouseEnter += ( object sender, EventArgs e ) => { this.OnMouseEnter( e ); };
      _label.MouseLeave += ( object sender, EventArgs e ) => { this.OnMouseLeave( e ); };
      _label.MouseHover += ( object sender, EventArgs e ) => { this.OnMouseHover( e ); };

      // popup image handling
      _tTip.SetToolTip( _label, " " ); // must provide some content, else it will not popup
      _tTip.ShowAlways = true;
      _tTip.InitialDelay = 1500; // default 1000
      _tTip.AutoPopDelay = 30 * 1000; // default 5000
      //_tTip.ReshowDelay = 500; // default 500

    }

    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      base.UnregisterDataSource( );

      _tTip.RemoveAll( );
      _tTip.Dispose( );
      if (_label.Tag is Bitmap) {
        (_label.Tag as Bitmap)?.Dispose( );
      }
    }

    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      NextColorset( ); // MSFS, rotate colorset

      // save as setting
      AppSettingsV2.Instance.Appearance = (int)Colorset;
      AppSettingsV2.Instance.Save( );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
    }


    // Custom Tooltip to show the Landing Perf Image
    // The Image shall be in the Tip providing Control.Tag property
    private class ImageToolTip : ToolTip
    {
      // cTor
      public ImageToolTip( )
      {
        this.OwnerDraw = true;
        this.Popup += new PopupEventHandler( this.OnPopup );
        this.Draw += new DrawToolTipEventHandler( this.OnDraw );
      }

      // called before poping up, derive the required size of the popup
      private void OnPopup( object sender, PopupEventArgs e ) // use this event to set the size of the tool tip
      {
        // sanity
        // expecting the calling Control has the image as Tag property (or null...)
        Control parent = e.AssociatedControl;
        if (parent == null) return;

        // use only the first when Bounces occur
        var limage = new LandingImage( PerfTracker.Instance.InitialTD );

        // prep for the next image (must dispose previous images)
        lock (parent) {
          if (parent.Tag is Bitmap) {
            (parent.Tag as Bitmap)?.Dispose( );
          }
          parent.Tag = limage.AsImage( ); // set popup source
          e.ToolTipSize = (parent.Tag as Image).Size;
        }
      }

      // called to draw the the content
      private void OnDraw( object sender, DrawToolTipEventArgs e ) // use this to customzie the tool tip
      {
        // sanity
        if (!(sender is ToolTip)) return;
        // expecting the calling Control has the image as Tag property (or null...)
        Control parent = e.AssociatedControl;
        if (parent == null) return;
        if (parent.Tag == null) return;
        if (!(parent.Tag is Image)) return;

        // as the calling label may be anywhere and at a location where the popup image is going byond the screen
        // we move the popup image to the top left of the current screen

        var tTip = sender as ToolTip;
        var hWnd = (IntPtr)tTip.GetType( ).GetProperty( "Handle", BindingFlags.NonPublic | BindingFlags.Instance ).GetValue( tTip );

        // get the screen where the Control is currently shown
        var screen = Screen.FromHandle( hWnd );
        // top left corner of the working area
        MoveWindow( hWnd, screen.WorkingArea.X + 2, screen.WorkingArea.Y + 2, e.Bounds.Width, e.Bounds.Height, false );

        lock (parent) {
          Image image = parent.Tag as Image;
          // image.Save( @".\TEST.png" );  // DEBUG ONLY

          //create our own custom brush to fill the background with the image
          using (TextureBrush iBrush = new TextureBrush( new Bitmap( image ) )) {
            e.Graphics.FillRectangle( iBrush, e.Bounds );
          }
        }
      }

      // Win32 call to move a window to a location
      [DllImport( "User32.dll" )]
      static extern bool MoveWindow( IntPtr h, int x, int y, int width, int height, bool redraw );

    }

  }
}

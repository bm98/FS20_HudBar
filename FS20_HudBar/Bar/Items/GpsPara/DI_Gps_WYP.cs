using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;
using FShelf.LandPerf;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using Windows.Networking.Sockets;

namespace FS20_HudBar.Bar.Items
{
  class DI_Gps_WYP : DispItem
  {
    // module Ref
    private readonly GPS_ToolTip _tTip = new GPS_ToolTip( );
    private const string c_GPS_Label = "GPS";
    private const string c_PWYP_Label = "PWYP";
    private const string c_NWYP_Label = "NWYP";

    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.GPS_WYP;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "≡GPS≡";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "GPS Waypoints";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;

    public DI_Gps_WYP( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.GPS_PWYP;
      _label = new L_Text( lblProto ) { Name = c_GPS_Label, Text = Short }; this.AddItem( _label );
      _value1 = new V_ICAO( value2Proto ) { Name = c_PWYP_Label, ItemForeColor = cTxGps };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.GPS_NWYP;
      _value2 = new V_ICAO_L( value2Proto ) { Name = c_NWYP_Label, ItemForeColor = cTxGps };
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      AddObserver( Desc, 1, OnDataArrival ); // once per sec is enough

      // tooltip - must provide some content, else it will not popup
      _tTip.SetToolTip( _label, " " ); // GPS Label
      _label.Cursor = Cursors.PanEast;
      _tTip.SetToolTip( _value2, " " ); // Next WYP
      _value2.Cursor = Cursors.PanEast;

    }

    // format an empty wyp as null -> ____ readout
    private string WypLabel( string wypName ) => string.IsNullOrWhiteSpace( wypName ) ? null : wypName;


    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        // using the Flightplan tracker content
        string pWyp = HudBar.FlightPlanRef.Tracker.PrevRoutePointID;
        string nWyp = HudBar.FlightPlanRef.Tracker.NextRoutePointID;

        _value1.Text = SV.Get<bool>( SItem.bG_Gps_DirectTo_tracking ) ? "Ð→" : WypLabel( pWyp );
        _value2.Text = WypLabel( nWyp );
      }
    }

    // Disconnect from updates and dispose ToolTip
    protected override void UnregisterDataSource( )
    {
      base.UnregisterDataSource( );

      _tTip.RemoveAll( );
      _tTip.Dispose( );
    }

    #region FLplan ToolTip

    /// <summary>
    /// Make a monospace tooltip
    /// 
    /// Extended to work with all kind of Fonts
    /// We actually measure the string to be drawn and apply that size later 
    /// when the TTip needs to be drawn
    /// </summary>
    private class GPS_ToolTip : ToolTip_Base
    {
      // Font used throughout for this type of tooltip
      // a mono font to have the columns neatly displayed
      private readonly Font c_ttFont = new Font( "Consolas", 9f, FontStyle.Regular );

      // Extend Size to have some padding around the measured box
      private readonly SizeF c_extend = new SizeF( 20, 15 );

      // TT Context for dynamic text - collected when the popup is required
      private struct TTContext
      {
        // TT Caption to be drawn
        public string Caption;
        // Pre Measured Size for proper font scaling of the popup
        public Size Size;
        // Create a Context with data
        public TTContext( string cont, Size size )
        {
          Caption = cont;
          Size = size;
        }
      }

      //maintain the TTContext for all controls to apply the proper sizing and caption when about to be drawn
      private Dictionary<Control, TTContext> _ttCat = new Dictionary<Control, TTContext>( );

      /// <summary>
      /// cTor:
      /// </summary>
      public GPS_ToolTip( )
      {
        // Set up the delays for the ToolTip.
        AutoPopDelay = 30_000; // looong

        this.OwnerDraw = true;
        this.IsBalloon = false;
        // interact and overwrite some behavior
        this.Draw += new DrawToolTipEventHandler( OnDraw );
        this.Popup += TT_Popup;
      }

      /// <summary>
      /// cTor:
      /// </summary>
      public GPS_ToolTip( System.ComponentModel.IContainer Cont )
      {
        this.OwnerDraw = true;
        this.Draw += new DrawToolTipEventHandler( OnDraw );
        this.Popup += TT_Popup;
      }

      /// <summary>
      /// Resets the Draw Size list an removes old entries
      /// Beware, call it only when none of the controls will be needed ever again.
      /// </summary>
      public void ResetDrawList( )
      {
        _ttCat.Clear( );
      }


      /// <summary>
      /// Called to set the tooltip caption 
      ///  we reimplement it to derive the bounding size of the TTip for later use
      /// </summary>
      /// <param name="control">The control to attach the TTip</param>
      /// <param name="caption">The string to show as TTip Caption</param>
      new public void SetToolTip( Control control, string caption )
      {
        // Sanity checks - as we don't check when calling this one
        if (control == null) return;
        if (control.IsDisposed) return;

        // maintain an entry for all TTips per control
        if (!_ttCat.ContainsKey( control )) {
          _ttCat.Add( control, CreateContextFor( control, caption ) );
        }
        else {
          _ttCat[control] = CreateContextFor( control, caption );
        }
        base.SetToolTip( control, caption );
      }


      // the boundary box should be set here via ToolTip PopUp event to the measured size of the text to draw
      // also use the caption in the catalog - which is dynamically set when the popup is requested
      private void OnDraw( object sender, DrawToolTipEventArgs e )
      {
        string caption = " ";
        // shall never fail due to not found control - should really not..
        try {
          caption = _ttCat[e.AssociatedControl].Caption;
        }
        catch { }

        DrawToolTipEventArgs newArgs = new DrawToolTipEventArgs( e.Graphics,
                  e.AssociatedWindow, e.AssociatedControl, e.Bounds, caption,
                  this.BackColor, this.ForeColor, c_ttFont );
        newArgs.DrawBackground( );
        newArgs.DrawBorder( );
        newArgs.DrawText( TextFormatFlags.TextBoxControl | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix );
      }


      // called on PopUp BEFORE the OnDraw Call
      // get the caption and the size of it for the draw call
      private void TT_Popup( object sender, PopupEventArgs e )
      {
        // shall never fail due to not found control - should really not..
        try {
          // Get required text when popping up
          if (e.AssociatedControl.Name == c_GPS_Label) {
            var ctx = CreateContextFor( e.AssociatedControl, HudBar.FlightPlanRef.RemainingPlan( ) );
            _ttCat[e.AssociatedControl] = ctx;
          }
          else if (e.AssociatedControl.Name == c_NWYP_Label) {
            var ctx = CreateContextFor( e.AssociatedControl, HudBar.FlightPlanRef.Tracker.NextRoutePoint.PrettyDetailed( ) );
            _ttCat[e.AssociatedControl] = ctx;
          }
          else {
            // ??
          }
          e.ToolTipSize = _ttCat[e.AssociatedControl].Size; // assign measured size
        }
        catch { }
      }

      // create a TTContext for this control and caption
      private TTContext CreateContextFor( Control control, string caption )
      {
        // measure the string to be drawn and store it per control
        Size size = new Size( 100, 100 );
        using (var g = control.CreateGraphics( )) {
          var s = g.MeasureString( caption, c_ttFont );
          s += c_extend; // inflate the measured bound
          size = s.ToSize( );
        }
        return new TTContext( caption, size );
      }

    }

    #endregion

  }
}

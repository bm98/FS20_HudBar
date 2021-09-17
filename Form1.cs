﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CoordLib;
using MetarLib;
using FS20_HudBar.Bar;

using SC = SimConnectClient;

namespace FS20_HudBar
{
  public partial class frmMain : Form
  {
    // Handle of the Primary Screen to attach bar and tile
    private readonly Screen m_mainScreen;

    // Form opacity when not Fully opaque (slightly transparent)
    private const float c_opacity = 0.85f;

    // The profiles
    private List<CProfile> m_profiles = new List<CProfile>();
    private int m_selProfile = 0;

    //private SC.Input.InputHandler FSInput;  // Receive commands from FSim -  not yet used
    private readonly frmConfig CFG = new frmConfig( ); // Configuration Dialog
    // need to stop processing while reconfiguring the bar
    private bool m_initDone = false;

    // Auto ETrim
    private const int c_aETrimTime = 20_000; // msec  AutoETrim active time when clicked
    private int m_aETrimTimer = 0;           // switch AET off when expired (<=0)


    /// <summary>
    /// Checks if a rectangle is visible on any screen
    /// </summary>
    /// <param name="formRect"></param>
    /// <returns>True if visible</returns>
    private static bool IsOnScreen( Rectangle formRect )
    {
      formRect.Inflate( -20, -20 ); // have to make it a bit smaller as the rectangle can be slightly out of screen
      Screen[] screens = Screen.AllScreens;
      foreach ( Screen screen in screens ) {
        if ( screen.WorkingArea.Contains( formRect ) ) {
          return true;
        }
      }
      return false;
    }

    /// <summary>
    /// Main Form Init
    /// </summary>
    public frmMain( )
    {
      InitializeComponent( );

      // Load all from AppSettings
      AppSettings.Instance.Reload( );

      m_profiles.Add( new CProfile( 1, AppSettings.Instance.Profile_1_Name,
                                       AppSettings.Instance.Profile_1, AppSettings.Instance.FlowBreak_1, AppSettings.Instance.Sequence_1,
                                       AppSettings.Instance.Profile_1_FontSize, AppSettings.Instance.Profile_1_Placement,
                                       AppSettings.Instance.Profile_1_Kind, AppSettings.Instance.Profile_1_Location,
                                       AppSettings.Instance.Profile_1_Condensed ) );

      m_profiles.Add( new CProfile( 2, AppSettings.Instance.Profile_2_Name,
                                       AppSettings.Instance.Profile_2, AppSettings.Instance.FlowBreak_2, AppSettings.Instance.Sequence_2,
                                       AppSettings.Instance.Profile_2_FontSize, AppSettings.Instance.Profile_2_Placement,
                                       AppSettings.Instance.Profile_2_Kind, AppSettings.Instance.Profile_2_Location,
                                       AppSettings.Instance.Profile_2_Condensed ) );

      m_profiles.Add( new CProfile( 3, AppSettings.Instance.Profile_3_Name,
                                       AppSettings.Instance.Profile_3, AppSettings.Instance.FlowBreak_3, AppSettings.Instance.Sequence_3,
                                       AppSettings.Instance.Profile_3_FontSize, AppSettings.Instance.Profile_3_Placement,
                                       AppSettings.Instance.Profile_3_Kind, AppSettings.Instance.Profile_3_Location,
                                       AppSettings.Instance.Profile_3_Condensed ) );

      m_profiles.Add( new CProfile( 4, AppSettings.Instance.Profile_4_Name,
                                       AppSettings.Instance.Profile_4, AppSettings.Instance.FlowBreak_4, AppSettings.Instance.Sequence_4,
                                       AppSettings.Instance.Profile_4_FontSize, AppSettings.Instance.Profile_4_Placement,
                                       AppSettings.Instance.Profile_4_Kind, AppSettings.Instance.Profile_4_Location,
                                       AppSettings.Instance.Profile_4_Condensed ) );

      m_profiles.Add( new CProfile( 5, AppSettings.Instance.Profile_5_Name,
                                       AppSettings.Instance.Profile_5, AppSettings.Instance.FlowBreak_5, AppSettings.Instance.Sequence_5,
                                       AppSettings.Instance.Profile_5_FontSize, AppSettings.Instance.Profile_5_Placement,
                                       AppSettings.Instance.Profile_5_Kind, AppSettings.Instance.Profile_5_Location,
                                       AppSettings.Instance.Profile_5_Condensed ) );

      m_selProfile = AppSettings.Instance.SelProfile;
      mSelProfile.Text = m_profiles[m_selProfile].PName;


      // ShowUnits and Opacity are set via HUD in InitGUI

      // Find and hold the Primary Screen
      Screen[] screens = Screen.AllScreens;
      m_mainScreen = screens[0];
      // now get the Primary one
      foreach ( Screen screen in screens ) {
        if ( screen.Primary ) {
          m_mainScreen = screen;
        }
      }
    }


    private void frmMain_Load( object sender, EventArgs e )
    {
      // prepare the GUI 

      // The FlowPanel in Design is not docked - do it here
      flp.Dock = DockStyle.Fill;
      // flp.BorderStyle = BorderStyle.FixedSingle; // DEBUG to see where the FLPanel is
      flp.WrapContents = true; // Needs to wrap around
      // Window Props
      this.FormBorderStyle = FormBorderStyle.None; // no frame etc. to begin with
      this.TopMost = true; // make sure we float on top

      // Get the controls
      InitGUI( );

      // attach a Callback for the SimClient
      SC.SimConnectClient.Instance.DataArrived += Instance_DataArrived;
      SC.SimConnectClient.Instance.FlightPlanModule.Enabled = false; // start disabled, will be re-checked in InitGUI

      // Pacer to connect and may be other chores
      timer1.Interval = 5000; // try to connect in 5 sec intervals
      timer1.Enabled = true;
    }

    // fired when the Window Location has changed; also when starting the prog
    // Take care to capture only real user relocations
    private void frmMain_LocationChanged( object sender, EventArgs e )
    {
      if ( !m_initDone ) return; // bail out if in Init
      if ( HUD.Kind != GUI.Kind.Window ) return; // can only handle Window here

      HUD.Profile.UpdateLocation( this.Location );
      // store new location per profile
      switch ( m_selProfile ) {
        case 0: AppSettings.Instance.Profile_1_Location = this.Location; break;
        case 1: AppSettings.Instance.Profile_2_Location = this.Location; break;
        case 2: AppSettings.Instance.Profile_3_Location = this.Location; break;
        case 3: AppSettings.Instance.Profile_4_Location = this.Location; break;
        case 4: AppSettings.Instance.Profile_5_Location = this.Location; break;
        default: AppSettings.Instance.Profile_1_Location = this.Location; break;
      }
      AppSettings.Instance.Save( );
    }

    // Fired when about to Close
    private void frmMain_FormClosing( object sender, FormClosingEventArgs e )
    {
      // Save all Settings
      AppSettings.Instance.SelProfile = m_selProfile;
      AppSettings.Instance.Save( );
      // stop connecting tries
      timer1.Enabled = false;

      // disconnect from Sim if needed
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        SC.SimConnectClient.Instance.Disconnect( );
      }
    }

    // Menu Exit event
    private void mExit_Click( object sender, EventArgs e )
    {
      this.Close( ); // just call the main Close
    }

    // Menu Config Event
    private void mConfig_Click( object sender, EventArgs e )
    {
      // don't handle timer while in Config
      timer1.Enabled = false;
      // Config must use the current environment 
      CFG.HudBarRef = HUD;
      CFG.ProfilesRef = m_profiles;
      CFG.SelectedProfile = m_selProfile;

      // Show and see if the user Accepts the changes
      if ( CFG.ShowDialog( this ) == DialogResult.OK ) {
        // Save all configuration properties
        AppSettings.Instance.ShowUnits = HUD.ShowUnits;
        AppSettings.Instance.Opaque = HUD.OpaqueBackground;
        AppSettings.Instance.FltAutoSave = HUD.FltAutoSave;

        AppSettings.Instance.SelProfile = m_selProfile;

        AppSettings.Instance.Profile_1_Name = m_profiles[0].PName;
        AppSettings.Instance.Profile_1 = m_profiles[0].ProfileString( );
        AppSettings.Instance.FlowBreak_1 = m_profiles[0].FlowBreakString( );
        AppSettings.Instance.Sequence_1 = m_profiles[0].ItemPosString( );
        AppSettings.Instance.Profile_1_FontSize = (int)m_profiles[0].FontSize;
        AppSettings.Instance.Profile_1_Placement = (int)m_profiles[0].Placement;
        AppSettings.Instance.Profile_1_Kind = (int)m_profiles[0].Kind;
        AppSettings.Instance.Profile_1_Condensed = m_profiles[0].Condensed;

        AppSettings.Instance.Profile_2_Name = m_profiles[1].PName;
        AppSettings.Instance.Profile_2 = m_profiles[1].ProfileString( );
        AppSettings.Instance.FlowBreak_2 = m_profiles[1].FlowBreakString( );
        AppSettings.Instance.Sequence_2 = m_profiles[1].ItemPosString( );
        AppSettings.Instance.Profile_2_FontSize = (int)m_profiles[1].FontSize;
        AppSettings.Instance.Profile_2_Placement = (int)m_profiles[1].Placement;
        AppSettings.Instance.Profile_2_Kind = (int)m_profiles[1].Kind;
        AppSettings.Instance.Profile_2_Condensed = m_profiles[1].Condensed;

        AppSettings.Instance.Profile_3_Name = m_profiles[2].PName;
        AppSettings.Instance.Profile_3 = m_profiles[2].ProfileString( );
        AppSettings.Instance.FlowBreak_3 = m_profiles[2].FlowBreakString( );
        AppSettings.Instance.Sequence_3 = m_profiles[2].ItemPosString( );
        AppSettings.Instance.Profile_3_FontSize = (int)m_profiles[2].FontSize;
        AppSettings.Instance.Profile_3_Placement = (int)m_profiles[2].Placement;
        AppSettings.Instance.Profile_3_Kind = (int)m_profiles[2].Kind;
        AppSettings.Instance.Profile_3_Condensed = m_profiles[2].Condensed;

        AppSettings.Instance.Profile_4_Name = m_profiles[3].PName;
        AppSettings.Instance.Profile_4 = m_profiles[3].ProfileString( );
        AppSettings.Instance.FlowBreak_4 = m_profiles[3].FlowBreakString( );
        AppSettings.Instance.Sequence_4 = m_profiles[3].ItemPosString( );
        AppSettings.Instance.Profile_4_FontSize = (int)m_profiles[3].FontSize;
        AppSettings.Instance.Profile_4_Placement = (int)m_profiles[3].Placement;
        AppSettings.Instance.Profile_4_Kind = (int)m_profiles[3].Kind;
        AppSettings.Instance.Profile_4_Condensed = m_profiles[3].Condensed;

        AppSettings.Instance.Profile_5_Name = m_profiles[4].PName;
        AppSettings.Instance.Profile_5 = m_profiles[4].ProfileString( );
        AppSettings.Instance.FlowBreak_5 = m_profiles[4].FlowBreakString( );
        AppSettings.Instance.Sequence_5 = m_profiles[4].ItemPosString( );
        AppSettings.Instance.Profile_5_FontSize = (int)m_profiles[4].FontSize;
        AppSettings.Instance.Profile_5_Placement = (int)m_profiles[4].Placement;
        AppSettings.Instance.Profile_5_Kind = (int)m_profiles[4].Kind;
        AppSettings.Instance.Profile_5_Condensed = m_profiles[4].Condensed;

        AppSettings.Instance.Save( );

        // Restart the GUI 
        InitGUI( ); // redraw changes
      }

      // reset out float above others each time we redo the GUI, could get lost when using Config
      this.TopMost = true;
      // pacer is finally back
      timer1.Enabled = true;
    }

    #region Profile Selectors

    // Menu Profile Selections 1..5
    private void mP1_Click( object sender, EventArgs e )
    {
      m_selProfile = 0;
      InitGUI( );
    }

    private void mP2_Click( object sender, EventArgs e )
    {
      m_selProfile = 1;
      InitGUI( );
    }

    private void mP3_Click( object sender, EventArgs e )
    {
      m_selProfile = 2;
      InitGUI( );
    }

    private void mP4_Click( object sender, EventArgs e )
    {
      m_selProfile = 3;
      InitGUI( );
    }

    private void mP5_Click( object sender, EventArgs e )
    {
      m_selProfile = 4;
      InitGUI( );
    }

    #endregion

    #region Mouse handlers for moving the Tile around

    private bool m_moving = false;
    private Point m_moveOffset = new Point(0,0);

    private void frmMain_MouseDown( object sender, MouseEventArgs e )
    {
      if ( !m_initDone ) return; // bail out if in Init
      if ( HUD.Kind != GUI.Kind.Tile ) return; // can only move Tile kind around here

      m_moving = true;
      m_moveOffset = e.Location;
    }

    private void frmMain_MouseMove( object sender, MouseEventArgs e )
    {
      if ( !m_initDone ) return; // bail out if in Init
      if ( HUD.Kind != GUI.Kind.Tile ) return; // can only move Tile kind around here
      if ( !m_moving ) return;

      switch ( HUD.Placement ) {
        case GUI.Placement.Bottom:
          this.Location = new Point( this.Location.X + e.X - m_moveOffset.X, this.Location.Y );
          break;
        case GUI.Placement.Left:
          this.Location = new Point( this.Location.X, this.Location.Y + e.Y - m_moveOffset.Y );
          break;
        case GUI.Placement.Right:
          this.Location = new Point( this.Location.X, this.Location.Y + e.Y - m_moveOffset.Y );
          break;
        case GUI.Placement.Top:
          this.Location = new Point( this.Location.X + e.X - m_moveOffset.X, this.Location.Y );
          break;
        default: break;
      }
    }

    private void frmMain_MouseUp( object sender, MouseEventArgs e )
    {
      if ( !m_initDone ) return; // bail out if in Init
      if ( HUD.Kind != GUI.Kind.Tile ) return; // can only move Tile kind around here
      if ( !m_moving ) return;

      m_moving = false;
      HUD.Profile.UpdateLocation( this.Location );
      // store new location per profile
      switch ( m_selProfile ) {
        case 0: AppSettings.Instance.Profile_1_Location = this.Location; break;
        case 1: AppSettings.Instance.Profile_2_Location = this.Location; break;
        case 2: AppSettings.Instance.Profile_3_Location = this.Location; break;
        case 3: AppSettings.Instance.Profile_4_Location = this.Location; break;
        case 4: AppSettings.Instance.Profile_5_Location = this.Location; break;
        default: AppSettings.Instance.Profile_1_Location = this.Location; break;
      }
      AppSettings.Instance.Save( );

    }

    #endregion

    #region Callback Handlers

    // fired from Sim for new Data
    private void Instance_DataArrived( object sender, FSimClientIF.ClientDataArrivedEventArgs e )
    {
      m_awaitingEvent = false; // confirm we've got events
      UpdateGUI( e.DataRefName );
    }

    // Receive commands from FSim -  not yet used
    /*
    private void FSInput_InputArrived( object sender, SC.Input.FSInputEventArgs e )
    {
      // FSInput should be valid when this event fires..
      if ( SC.SimConnectClient.Instance.IsConnected && ( e.ActionName == FSInput.Inputname ) ) {
        // DO SOMETHING HERE
      }
    }
    */

    #endregion

    #region BarEvent Handler

    /// <summary>
    /// Handle the BAR click events here
    /// </summary>
    private void FrmMain_ButtonClicked( object sender, GUI.ClickedEventArgs e )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return; // no action when not connected

      switch ( e.Item ) {
        case VItem.AP:
          SC.SimConnectClient.Instance.AP_G1000Module.Master_toggle( );
          break;
        case VItem.AP_ALT:
          SC.SimConnectClient.Instance.AP_G1000Module.ALT_hold = true; // toggles independent of the set value
          break;
        case VItem.AP_APR:
          SC.SimConnectClient.Instance.AP_G1000Module.APR_hold = true; // toggles independent of the set value
          break;
        case VItem.AP_FLC:
          SC.SimConnectClient.Instance.AP_G1000Module.FLC_active = true; // toggles independent of the set value
          break;
        case VItem.AP_HDG:
          SC.SimConnectClient.Instance.AP_G1000Module.HDG_hold = true; // toggles independent of the set value
          break;
        case VItem.AP_NAV:
          SC.SimConnectClient.Instance.AP_G1000Module.NAV_hold = true; // toggles independent of the set value
          break;
        case VItem.AP_VS:
          SC.SimConnectClient.Instance.AP_G1000Module.VS_hold = true; // toggles independent of the set value
          break;
        case VItem.AP_BC:
          SC.SimConnectClient.Instance.AP_G1000Module.BC_hold = true; // toggles independent of the set value
          break;
        case VItem.AP_YD:
          SC.SimConnectClient.Instance.AP_G1000Module.YD_toggle( ); // toggles
          break;
        case VItem.AP_LVL:
          SC.SimConnectClient.Instance.AP_G1000Module.LVL_toggle( ); // toggles
          break;
        case VItem.ETrim:
          SC.SimConnectClient.Instance.AircraftModule.PitchTrim_prct = 0; // Set 0
          break;
        case VItem.RTrim:
          SC.SimConnectClient.Instance.AircraftModule.RudderTrim_prct = 0; // Set 0
          break;
        case VItem.ATrim:
          SC.SimConnectClient.Instance.AircraftModule.AileronTrim_prct = 0; // Set 0
          break;
        case VItem.A_ETRIM:
          SC.SimConnectClient.Instance.AutoETrimModule.Enabled = !SC.SimConnectClient.Instance.AutoETrimModule.Enabled; // toggles
          m_aETrimTimer = c_aETrimTime;
          break;
        case VItem.BARO_HPA:
          SC.SimConnectClient.Instance.AircraftModule.AltimeterSetting = true; // one shot trigger to sync Baro
          break;
        case VItem.BARO_InHg:
          SC.SimConnectClient.Instance.AircraftModule.AltimeterSetting = true; // one shot trigger to sync Baro
          break;
        // Enroute Reset
        case VItem.ENR_WP:
          WPTracker.InitFlight( );
          break;

        // METAR Airport - post a request
        case VItem.ATC_APT:
          HudBar.MetarApt.Clear( );
          if ( AirportMgr.IsAvailable ) {
            if ( AirportMgr.Location != null )
              HudBar.MetarApt.PostMETAR_Request( AirportMgr.AirportICAO, AirportMgr.Location ); // station rec with Location
            else
              HudBar.MetarApt.PostMETAR_Request( AirportMgr.AirportICAO ); // station rec
          }
          break;
        // METAR Location - post a request
        case VItem.METAR:
          HudBar.MetarLoc.Clear( );
          HudBar.MetarLoc.PostMETAR_Request( SC.SimConnectClient.Instance.AircraftModule.Lat,
                                      SC.SimConnectClient.Instance.AircraftModule.Lon,
                                      SC.SimConnectClient.Instance.GpsModule.GTRK ); // from current pos along the current track
          break;

        // (Re-)Start Meters
        case VItem.M_Elapsed1:
          HudBar.CPMeter1.Start( new LatLon( SC.SimConnectClient.Instance.AircraftModule.Lat, SC.SimConnectClient.Instance.AircraftModule.Lon ),
                            SC.SimConnectClient.Instance.AircraftModule.SimTime_zulu_sec );
          break;
        case VItem.M_Elapsed2:
          HudBar.CPMeter2.Start( new LatLon( SC.SimConnectClient.Instance.AircraftModule.Lat, SC.SimConnectClient.Instance.AircraftModule.Lon ),
                            SC.SimConnectClient.Instance.AircraftModule.SimTime_zulu_sec );
          break;
        case VItem.M_Elapsed3:
          HudBar.CPMeter3.Start( new LatLon( SC.SimConnectClient.Instance.AircraftModule.Lat, SC.SimConnectClient.Instance.AircraftModule.Lon ),
                            SC.SimConnectClient.Instance.AircraftModule.SimTime_zulu_sec );
          break;

        default: break; // nothing 
      }
    }

    #endregion

    #region GUI

    private HudBar HUD = null;

    // initialize the form, the labels and default values
    // sequence defines appearance
    private void InitGUI( )
    {
      timer1.Enabled = false; // stop asynch events
      m_initDone = false; // stop updating values while reconfiguring
      this.Visible = false; // hide, else we see all kind of shaping

      // reread after config change
      SC.SimConnectClient.Instance.FlightPlanModule.Enabled = AppSettings.Instance.FltAutoSave;

      AirportMgr.Reset( );
      WPTracker.Reset( );


      // Update profile selection items
      mP1.Text = m_profiles[0].PName;
      mP2.Text = m_profiles[1].PName;
      mP3.Text = m_profiles[2].PName;
      mP4.Text = m_profiles[3].PName;
      mP5.Text = m_profiles[4].PName;
      mSelProfile.Text = m_profiles[m_selProfile].PName;

      // start from scratch
      HUD = new HudBar( lblProto, valueProto, value2Proto, signProto,
                          AppSettings.Instance.ShowUnits, AppSettings.Instance.Opaque, AppSettings.Instance.FltAutoSave,
                          m_profiles[m_selProfile] );

      // prepare to create the content as bar or tile (may be switch to Window later if needed)
      this.FormBorderStyle = FormBorderStyle.None; // no frame etc.
      // Prepare FLPanel to load controls
      flp.Controls.Clear( ); // reload
      // release dock to allow the bar to autosize
      flp.Dock = DockStyle.None;
      flp.AutoSize = true;
      // can move a tile kind profile (but not a bar)
      flp.Cursor = HUD.Profile.Kind == GUI.Kind.Tile ? Cursors.SizeAll : this.Cursor;

      // attach it to the PRIMARY screen (we assume the FS is run on the primary anyway...)
      // preliminary  windows full width/height
      switch ( HUD.Placement ) {
        case GUI.Placement.Bottom:
          this.Width = m_mainScreen.Bounds.Width;
          this.Height = 40; //  any will do as we rescale it below
          this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y + m_mainScreen.Bounds.Height - this.Height );
          flp.FlowDirection = FlowDirection.LeftToRight;
          break;
        case GUI.Placement.Left:
          this.Height = m_mainScreen.Bounds.Height;
          this.Width = 200; //  any will do as we rescale it below
          this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y );
          flp.FlowDirection = FlowDirection.TopDown;
          break;
        case GUI.Placement.Right:
          this.Height = m_mainScreen.Bounds.Height;
          this.Width = 200; //  any will do as we rescale it below
          this.Location = new Point( m_mainScreen.Bounds.X + m_mainScreen.Bounds.Width - this.Width, m_mainScreen.Bounds.Y );
          flp.FlowDirection = FlowDirection.TopDown;
          break;
        case GUI.Placement.Top:
          this.Width = m_mainScreen.Bounds.Width;
          this.Height = 40; //  any will do as we rescale it below
          this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y );
          flp.FlowDirection = FlowDirection.LeftToRight;
          break;
        default:
          // Bottom
          this.Width = m_mainScreen.Bounds.Width;
          this.Height = 40; //  any will do as we rescale it below
          this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y + m_mainScreen.Bounds.Height - this.Height );
          flp.FlowDirection = FlowDirection.LeftToRight;
          break;
      }
      // set form opacity from settings (use the const value for slight transparency)
      this.Opacity = HUD.OpaqueBackground ? 1 : c_opacity;
      //      this.Opacity = 0.1;
      flp.BackColor = Color.FromArgb( 255, 0, 0, 1 );

      // Walk all DispItems and add the ones to be shown
      int maxHeight = 0;
      int maxWidth = 0;
      GUI.DispItem prevDi = null;
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        // using the enum index only to count from 0..max items
        var key = HUD.Profile.ItemKeyFromPos( (int)i);
        // The DispItem is a FlowPanel containing the Label and maybe some Values
        var di = HUD.DispItem( key ); 
        if ( di != null && HUD.ShowItem( key ) ) {
          // if the item is checked, i.e. to be shown only
          if ( di.Controls.Count > 0 ) {
            // add it to the Main FlowPanel when we have to show something
            flp.Controls.Add( di );

            // the flowbreak causes the tagged item to be on the same line and then to break for the next one
            // Not so intuitive for the user - so we mark the one that goes on the next line but need to attach the FB then to the prev one
            if ( HUD.Profile.BreakItem( key ) && prevDi != null ) {
              // We set the FlowBreak to the item before the marked one
              flp.SetFlowBreak( prevDi, true );
            }
            // collect max dimensions derived from each DispItem while loading the panel (loading also layouts them)
            int h = di.Top+di.Height;
            maxHeight = ( h > maxHeight ) ? h : maxHeight;
            int w = di.Left+di.Width;
            maxWidth = ( w > maxWidth ) ? w : maxWidth;

            prevDi = di; // store for FlowBreak attachment for valid and visible ones if the next one is tagged
          }
        }
        else {
          if ( di != null ) di.Visible = false;
        }
      }

      // attach mouse click handlers to Button Type Labels
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        var di = HUD.DispItem( i );
        if ( di != null ) {
          var l = di.Label;
          if ( l is GUI.B_Base ) {
            ( l as GUI.B_Base ).ButtonClicked += FrmMain_ButtonClicked;
          }
        }
      }

      // post proc - allocate the needed height/width/location
      // reduce width/ height for Tiles or Windows
      // A window is essentially a tile with border and will later be positioned at the last stored location
      switch ( HUD.Placement ) {
        case GUI.Placement.Bottom:
          this.Height = maxHeight + 5;
          if ( ( HUD.Profile.Kind == GUI.Kind.Tile ) || ( HUD.Profile.Kind == GUI.Kind.Window ) ) {
            this.Width = flp.Width + 5;
            this.Location = new Point( HUD.Profile.Location.X, m_mainScreen.Bounds.Y + m_mainScreen.Bounds.Height - this.Height );
          }
          else { // Bar
            this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y + m_mainScreen.Bounds.Height - this.Height );
          }
          break;

        case GUI.Placement.Left:
          this.Width = maxWidth + 10;
          if ( ( HUD.Profile.Kind == GUI.Kind.Tile ) || ( HUD.Profile.Kind == GUI.Kind.Window ) ) {
            this.Height = flp.Height + 10;
            this.Location = new Point( m_mainScreen.Bounds.X, HUD.Profile.Location.Y );
          }
          else { // Bar
            this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y );
          }
          break;

        case GUI.Placement.Right:
          this.Width = maxWidth + 10;
          if ( ( HUD.Profile.Kind == GUI.Kind.Tile ) || ( HUD.Profile.Kind == GUI.Kind.Window ) ) {
            this.Height = flp.Height + 10;
            this.Location = new Point( m_mainScreen.Bounds.X + m_mainScreen.Bounds.Width - this.Width, HUD.Profile.Location.Y );
          }
          else { // Bar
            this.Location = new Point( m_mainScreen.Bounds.X + m_mainScreen.Bounds.Width - this.Width, m_mainScreen.Bounds.Y );
          }
          break;

        case GUI.Placement.Top:
          this.Height = maxHeight + 5;
          if ( ( HUD.Profile.Kind == GUI.Kind.Tile ) || ( HUD.Profile.Kind == GUI.Kind.Window ) ) {
            this.Width = flp.Width + 5;
            this.Location = new Point( HUD.Profile.Location.X, m_mainScreen.Bounds.Y );
          }
          else { // Bar
            this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y );
          }
          break;

        default:
          // Bottom
          this.Height = maxHeight + 5;
          this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y + m_mainScreen.Bounds.Height - this.Height );
          break;
      }
      // after sizing the Window - re-dock the FLPanel for full Fill
      flp.Dock = DockStyle.Fill;

      // handle Window Style HUDs
      if ( HUD.Kind == GUI.Kind.Window ) {
        this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        // We take the last user location to reposition the window / above it was bound to the edge of the main screen (Tile kind)
        // avoid invisible windows from odd stored locations
        if ( IsOnScreen( new Rectangle( HUD.Profile.Location, this.Size ) ) ) {
          this.Location = HUD.Profile.Location;
        }
        // A Window is still TopMost - don't know if this is a good idea, we shall see...
      }

      // Color the MSFS Label it if connected
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        HUD.DispItem( LItem.MSFS ).Label.ForeColor = Color.LimeGreen;
        HUD.DispItem( LItem.MSFS ).Label.BackColor = flp.BackColor;
        HUD.Value( VItem.Ad ).Text = "";
      }
      else {
        HUD.DispItem( LItem.MSFS ).Label.ForeColor = HudBar.c_Info;
        HUD.DispItem( LItem.MSFS ).Label.BackColor = Color.Red;
        HUD.Value( VItem.Ad ).Text = ""; // = "NO SIM" - don't add a text - it makes the bars jumping due to this change in layout
      }

      this.Visible = true; // Unhide when finished
      m_initDone = true;
      timer1.Enabled = true; // and enable pacer
    }


    /// <summary>
    /// Update the GUI values from Sim
    ///  In general GUI elements are only updated when checked and visible
    ///  Trackers and Meters are maintained independent of the View state (another profile may use them..)
    /// </summary>
    private void UpdateGUI( string dataRefName )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return; // sanity..
      if ( !m_initDone ) return; // cannot access items at this time

      HUD.UpdateGUI( dataRefName );
    }

    #endregion

    // Monitor the Sim Event Handler after Connection
    private bool m_awaitingEvent = true; // cleared in the Sim Event Handler
    private int m_scGracePeriod = -1;    // grace period count down

    /// <summary>
    /// Toggle the connection
    /// </summary>
    private void SimConnect( )
    {
      HUD.Value( VItem.Ad ).Text = ""; // reset in case it had an error message
      HUD.DispItem( LItem.MSFS ).Label.ForeColor = HudBar.c_Info;
      HUD.DispItem( LItem.MSFS ).Label.BackColor = HudBar.c_BG;

      if ( SC.SimConnectClient.Instance.IsConnected ) {
        // Disconnect from Input and SimConnect
        // FSInput.InputArrived -= FSInput_InputArrived; // Receive commands from FSim -  not yet used
        //FltMgr.Disable( );
        SC.SimConnectClient.Instance.FlightPlanModule.Enabled = false;
        SC.SimConnectClient.Instance.Disconnect( );
      }
      else {
        // setup the event monitor before connecting (will be handled in the Timer Event)
        m_awaitingEvent = true;
        m_scGracePeriod = 3; // about 3*5 secs to get an event
        // try to connect
        if ( SC.SimConnectClient.Instance.Connect( ) ) {
          HUD.DispItem( LItem.MSFS ).Label.ForeColor = Color.LimeGreen;
          // init the SimClient by pulling one item, so it registers the module, else the callback is not initiated
          _ = SC.SimConnectClient.Instance.AircraftModule.AcftConfigFile;
          // Receive commands from FSim -  not yet used
          /*
          FSInput = SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_01 ); // use first input
          FSInput.InputArrived += FSInput_InputArrived;
          */
          SC.SimConnectClient.Instance.FlightPlanModule.Enabled = AppSettings.Instance.FltAutoSave;
          //FltMgr.Enable( );
        }
        else {
          HUD.DispItem( LItem.MSFS ).Label.BackColor = Color.Red;
          //HUD.Value( GItem.Ad ).Text = SC.SimConnectClient.Instance.ErrorList.FirstOrDefault( ); // error message
          //HUD.Value( VItem.Ad ).Text = "NO SIM";
        }
      }

    }


    /// <summary>
    /// Try every interval to connect - and if connected.. do in Sim chores
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timer1_Tick( object sender, EventArgs e )
    {
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        // Kick AutoEtrim Module if needed
        if ( m_aETrimTimer <= 0 ) {
          // stop module
          SC.SimConnectClient.Instance.AutoETrimModule.Enabled = false;
        }
        else {
          m_aETrimTimer -= timer1.Interval; // dec timer for the AutoTrim lifetime
        }

        // handle the situation where Sim is connected but could not hookup to events
        // Happens when HudBar is running when the Sim is starting only.
        // Sometimes the Connection is made but was not hooking up to the event handling
        // Disconnect and try to reconnect 
        if ( m_awaitingEvent ) {
          // No events seen so far
          if ( m_scGracePeriod <= 0 ) {
            // grace period is expired !
            Console.WriteLine( "HudBar: Did not receive an Event - Restarting Connection" );
            SimConnect( ); // Disconnect if we don't receive Events even the Sim is connected
          }
          m_scGracePeriod--;
        }

      }
      else {
        // If not connected try again
        SimConnect( );
      }
    }

  }
}

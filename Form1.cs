﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;

namespace FS20_HudBar
{
  public partial class frmMain : Form
  {

    private List<CProfile> m_profiles = new List<CProfile>();
    private int m_selProfile = 0;
    private bool m_initDone = false; // need to stop processing while reconfiguring the bar

    private const int c_aETrimTime = 20_000; // 20 sec
    private int m_aETrimTimer = 0; // switch AET off when expired (<=0)

    private const float c_opacity = 0.85f;  // Form opacity when not Fully opaque (slightly transparent)

    //private SC.Input.InputHandler FSInput;  // Receive commands from FSim -  not yet used
    private frmConfig CFG = new frmConfig( );

    Screen m_mainScreen;

    private CPointMeter m_cpMeter1 = new CPointMeter();
    private CPointMeter m_cpMeter2 = new CPointMeter();
    private CPointMeter m_cpMeter3 = new CPointMeter();

    public frmMain( )
    {
      InitializeComponent( );

      AppSettings.Instance.Reload( );

      m_profiles.Add( new CProfile( 1, AppSettings.Instance.Profile_1_Name,
                                       AppSettings.Instance.Profile_1, AppSettings.Instance.FlowBreak_1, AppSettings.Instance.Sequence_1,
                                       AppSettings.Instance.Profile_1_FontSize, AppSettings.Instance.Profile_1_Placement,
                                       AppSettings.Instance.Profile_1_Kind, AppSettings.Instance.Profile_1_Location,
                                       AppSettings.Instance.Profile_1_Condensed ) );

      m_profiles.Add( new CProfile( 2, AppSettings.Instance.Profile_2_Name,
                                       AppSettings.Instance.Profile_2, AppSettings.Instance.FlowBreak_2, AppSettings.Instance.Sequence_3,
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
      flp.Dock = DockStyle.Fill;
      // flp.BorderStyle = BorderStyle.FixedSingle; // DEBUG
      flp.WrapContents = true;
      this.FormBorderStyle = FormBorderStyle.None; // no frame etc.
      this.TopMost = true; // make sure we float on top

      // Get the controls
      InitGUI( );

      // attach a Callback for the SimClient
      SC.SimConnectClient.Instance.DataArrived += Instance_DataArrived;

      timer1.Interval = 5000; // try to connect in 5 sec intervals
      timer1.Enabled = true;
    }

    private void frmMain_FormClosing( object sender, FormClosingEventArgs e )
    {
      AppSettings.Instance.SelProfile = m_selProfile;
      AppSettings.Instance.Save( );
      // stop connecting tries
      timer1.Enabled = false;

      // disconnect if needed
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        SC.SimConnectClient.Instance.Disconnect( );
      }
    }


    private void mExit_Click( object sender, EventArgs e )
    {
      this.Close( );
    }


    private void mConfig_Click( object sender, EventArgs e )
    {
      timer1.Enabled = false; // don't handle timer while in Config

      CFG.HudBarRef = HUD;
      CFG.ProfilesRef = m_profiles;
      CFG.SelectedProfile = m_selProfile;

      if ( CFG.ShowDialog( this ) == DialogResult.OK ) {

        // Save all configuration properties
        AppSettings.Instance.ShowUnits = HUD.ShowUnits;
        AppSettings.Instance.Opaque = HUD.OpaqueBackground;

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

        InitGUI( ); // redraw changes
      }

      this.TopMost = true; // reset out float above others each time we redo the GUI, could get lost when using Config

      timer1.Enabled = true;
    }

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

    private bool m_moving = false;
    private Point m_moveOffset = new Point(0,0);

    private void frmMain_MouseDown( object sender, MouseEventArgs e )
    {
      if ( !m_initDone ) return; // bail out if in Init - the splitter is moved during some init and resize events on it's own..
      if ( HUD.Kind == GUI.Kind.Bar ) return; // cannot move the bar

      m_moving = true;
      m_moveOffset = e.Location;
    }

    private void frmMain_MouseMove( object sender, MouseEventArgs e )
    {
      if ( !m_initDone ) return; // bail out if in Init - the splitter is moved during some init and resize events on it's own..
      if ( HUD.Kind == GUI.Kind.Bar ) return; // cannot move the bar
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
      if ( !m_initDone ) return; // bail out if in Init - the splitter is moved during some init and resize events on it's own..
      if ( HUD.Kind == GUI.Kind.Bar ) return; // cannot move the bar
      if ( !m_moving ) return;

      m_moving = false;
      PROFILE.UpdateLocation( this.Location );
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

    #region Callback Handlers

    // fired from Sim for new Data
    private void Instance_DataArrived( object sender, FSimClientIF.ClientDataArrivedEventArgs e )
    {
      UpdateGUI( );
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

    #region GUI

    private HudBar HUD = null;
    private CProfile PROFILE = null;

    // initialize the form, the labels and default values
    // sequence defines appearance
    private void InitGUI( )
    {
      m_initDone = false; // stop updating values while reconfiguring
      this.Visible = false; // hide, else we see all kind of shaping

      // Update profile selection items
      mP1.Text = m_profiles[0].PName;
      mP2.Text = m_profiles[1].PName;
      mP3.Text = m_profiles[2].PName;
      mP4.Text = m_profiles[3].PName;
      mP5.Text = m_profiles[4].PName;
      mSelProfile.Text = m_profiles[m_selProfile].PName;

      // current profile
      PROFILE = m_profiles[m_selProfile];

      // start from scratch
      HUD = new HudBar( lblProto, valueProto, value2Proto, signProto,
                          AppSettings.Instance.ShowUnits,
                          AppSettings.Instance.Opaque,
                          PROFILE.FontSize,
                          PROFILE.Placement,
                          PROFILE.Kind,
                          PROFILE.Condensed );

      // Prepare FLPanel to load controls
      flp.Controls.Clear( ); // reload
      // release dock to allow the bar to autosize
      flp.Dock = DockStyle.None;
      flp.AutoSize = true;
      // can move a tile kind profile (but not a bar)
      flp.Cursor = PROFILE.Kind == GUI.Kind.Tile ? Cursors.SizeAll : this.Cursor;

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

      // load flowLayout 
      int maxHeight = 0;
      int maxWidth = 0;
      GUI.DispItem prevDi = null;
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        // use the enum index only to count from 0..max items
        var key = PROFILE.ItemKeyFromPos( (int)i);
        var di = HUD.DispItem( key );
        if ( PROFILE.ShowItem( key ) ) {
          if ( di != null && di.Controls.Count > 0 ) {
            // add when we have to show something
            flp.Controls.Add( di );

            // the flowbreak causes the tagged item to be on the same line and then to break for the next one
            // Not so intuitive for the user - so we mark the one that goes on the next line but need to attach the FB then to the prev one
            if ( PROFILE.BreakItem( key ) && prevDi != null ) {
              // We set the FlowBreak to the item before the marked one
              flp.SetFlowBreak( prevDi, true );
            }
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

      // attach handlers
      foreach ( VItem i in Enum.GetValues( typeof( VItem ) ) ) {
        Control l = HUD.LabelControl( i );
        if ( l != null ) {
          if ( l is GUI.B_Base ) {
            ( l as GUI.B_Base ).ButtonClicked += FrmMain_ButtonClicked;
          }
        }
        l = HUD.ValueControl( i );
        if ( l != null ) {
          if ( l is GUI.B_Base ) {
            ( l as GUI.B_Base ).ButtonClicked += FrmMain_ButtonClicked;
          }
        }
      }

      // post proc - allocate the needed height/width/location
      // reduce width/ height for Tiles
      switch ( HUD.Placement ) {
        case GUI.Placement.Bottom:
          this.Height = maxHeight + 5;
          if ( PROFILE.Kind == GUI.Kind.Tile ) {
            this.Width = flp.Width + 5;
            this.Location = new Point( PROFILE.Location.X, m_mainScreen.Bounds.Y + m_mainScreen.Bounds.Height - this.Height );
          }
          else {
            this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y + m_mainScreen.Bounds.Height - this.Height );
          }
          break;

        case GUI.Placement.Left:
          this.Width = maxWidth + 5;
          if ( PROFILE.Kind == GUI.Kind.Tile ) {
            this.Height = flp.Height + 10;
            this.Location = new Point( m_mainScreen.Bounds.X, PROFILE.Location.Y );
          }
          else {
            this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y );
          }
          break;

        case GUI.Placement.Right:
          this.Width = maxWidth + 5;
          if ( PROFILE.Kind == GUI.Kind.Tile ) {
            this.Height = flp.Height + 10;
            this.Location = new Point( m_mainScreen.Bounds.X + m_mainScreen.Bounds.Width - this.Width, PROFILE.Location.Y );
          }
          else {
            this.Location = new Point( m_mainScreen.Bounds.X + m_mainScreen.Bounds.Width - this.Width, m_mainScreen.Bounds.Y );
          }
          break;

        case GUI.Placement.Top:
          this.Height = maxHeight + 5;
          if ( PROFILE.Kind == GUI.Kind.Tile ) {
            this.Width = flp.Width + 5;
            this.Location = new Point( PROFILE.Location.X, m_mainScreen.Bounds.Y );
          }
          else {
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

      // Color it if connected
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        HUD.LabelControl( VItem.Ad ).ForeColor = Color.LimeGreen;
        HUD.LabelControl( VItem.Ad ).BackColor = flp.BackColor;
        HUD.Value( VItem.Ad ).Text = "";
      }
      else {
        HUD.LabelControl( VItem.Ad ).ForeColor = HUD.c_Info;
        HUD.LabelControl( VItem.Ad ).BackColor = Color.Red;
        HUD.Value( VItem.Ad ).Text = "NO SIM";
      }

      this.Visible = true; // Unhide when finished
      m_initDone = true;
    }

    /// <summary>
    /// Handle the BAR click events here
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FrmMain_ButtonClicked( object sender, GUI.ClickedEventArgs e )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return; // no action

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
          SC.SimConnectClient.Instance.AircraftModule.AltimeterSetting = true;
          break;
        case VItem.BARO_InHg:
          SC.SimConnectClient.Instance.AircraftModule.AltimeterSetting = true;
          break;
        // Start Meters
        case VItem.M_Elapsed1:
          m_cpMeter1.Start( SC.SimConnectClient.Instance.AircraftModule.Lat, SC.SimConnectClient.Instance.AircraftModule.Lon,
                            SC.SimConnectClient.Instance.AircraftModule.SimTime_zulu_sec );
          break;
        case VItem.M_Elapsed2:
          m_cpMeter2.Start( SC.SimConnectClient.Instance.AircraftModule.Lat, SC.SimConnectClient.Instance.AircraftModule.Lon,
                            SC.SimConnectClient.Instance.AircraftModule.SimTime_zulu_sec );
          break;
        case VItem.M_Elapsed3:
          m_cpMeter3.Start( SC.SimConnectClient.Instance.AircraftModule.Lat, SC.SimConnectClient.Instance.AircraftModule.Lon,
                            SC.SimConnectClient.Instance.AircraftModule.SimTime_zulu_sec );
          break;

        default: break; // nothing 
      }
    }

    /// <summary>
    /// Update the GUI values from Sim
    /// </summary>
    private void UpdateGUI( )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return; // sanity..
      if ( !m_initDone ) return; // cannot access items at this time

      int numEngines = SC.SimConnectClient.Instance.EngineModule.NumEngines;

      // we do this one by one..

      // SimRate
      HUD.Value( VItem.SimRate ).Value = SC.SimConnectClient.Instance.AircraftModule.SimRate_rate;
      HUD.ValueControl( VItem.SimRate ).ForeColor = ( SC.SimConnectClient.Instance.AircraftModule.SimRate_rate != 1.0f ) ? HUD.c_BG : HUD.c_Info;
      HUD.ValueControl( VItem.SimRate ).BackColor = ( SC.SimConnectClient.Instance.AircraftModule.SimRate_rate != 1.0f ) ? HUD.c_SRATE : HUD.c_BG;

      HUD.Value( VItem.ACFT_ID ).Text = SC.SimConnectClient.Instance.AircraftModule.AcftID;

      // TRIMS
      // Auto ETrim
      HUD.LabelControl( VItem.A_ETRIM ).BackColor = SC.SimConnectClient.Instance.AutoETrimModule.Enabled ? HUD.c_AP : HUD.c_ActBG;
      HUD.Value( VItem.A_ETRIM ).Value = SC.SimConnectClient.Instance.AircraftModule.PitchTrim_prct;
      // Regular Trim
      HUD.Value( VItem.ETrim ).Value = SC.SimConnectClient.Instance.AircraftModule.PitchTrim_prct;
      HUD.Value( VItem.RTrim ).Value = SC.SimConnectClient.Instance.AircraftModule.RudderTrim_prct;
      HUD.Value( VItem.ATrim ).Value = SC.SimConnectClient.Instance.AircraftModule.AileronTrim_prct;
      // OAT
      HUD.Value( VItem.OAT ).Value = SC.SimConnectClient.Instance.AircraftModule.OutsideTemperature_degC;
      HUD.ValueControl( VItem.OAT ).ForeColor = ( SC.SimConnectClient.Instance.AircraftModule.OutsideTemperature_degC < 0 ) ? HUD.c_SubZero : HUD.c_Info; // icing conditions
      // BARO
      HUD.Value( VItem.BARO_HPA ).Value = SC.SimConnectClient.Instance.AircraftModule.AltimeterSetting_mbar;
      HUD.Value( VItem.BARO_InHg ).Value = SC.SimConnectClient.Instance.AircraftModule.AltimeterSetting_inHg;
      // Wind
      HUD.Value( VItem.WIND_SPEED ).Value = SC.SimConnectClient.Instance.AircraftModule.WindSpeed_kt;
      HUD.Value( VItem.WIND_DIR ).Value = SC.SimConnectClient.Instance.AircraftModule.WindDirection_deg;
      HUD.Value( VItem.WIND_LAT ).Value = SC.SimConnectClient.Instance.AircraftModule.Wind_AcftX_kt;
      HUD.Value( VItem.WIND_LON ).Value = SC.SimConnectClient.Instance.AircraftModule.Wind_AcftZ_kt;
      // Aoa
      HUD.Value( VItem.AOA ).Value = SC.SimConnectClient.Instance.AircraftModule.AngleOfAttack_deg;
      // Gear, Brakes, Flaps
      if ( SC.SimConnectClient.Instance.AircraftModule.IsGearRetractable ) {
        HUD.Value( VItem.Gear ).Step =
          SC.SimConnectClient.Instance.AircraftModule.GearPosition == FSimClientIF.GearPosition.Down ? GUI.Steps.Down :
          SC.SimConnectClient.Instance.AircraftModule.GearPosition == FSimClientIF.GearPosition.Up ? GUI.Steps.Up : GUI.Steps.Unk;
      }

      HUD.Value( VItem.Brakes ).Step = SC.SimConnectClient.Instance.AircraftModule.Parkbrake_on ? GUI.Steps.On : GUI.Steps.Off;

      if ( SC.SimConnectClient.Instance.AircraftModule.Flaps == FSimClientIF.CmdMode.Up ) {
        HUD.Value( VItem.Flaps ).Step = GUI.Steps.Up;
      }
      else if ( SC.SimConnectClient.Instance.AircraftModule.Flaps == FSimClientIF.CmdMode.Down ) {
        HUD.Value( VItem.Flaps ).Step = GUI.Steps.Down;
      }
      else if ( SC.SimConnectClient.Instance.AircraftModule.Flaps == FSimClientIF.CmdMode.Pos1 ) {
        HUD.Value( VItem.Flaps ).Step = GUI.Steps.P1;
      }
      else if ( SC.SimConnectClient.Instance.AircraftModule.Flaps == FSimClientIF.CmdMode.Pos2 ) {
        HUD.Value( VItem.Flaps ).Step = GUI.Steps.P2;
      }
      else if ( SC.SimConnectClient.Instance.AircraftModule.Flaps == FSimClientIF.CmdMode.Pos3 ) {
        HUD.Value( VItem.Flaps ).Step = GUI.Steps.P3;
      }
      // Consolidated lights (RA colored for Taxi and/or Landing lights on)
      int lightsInt = 0;
      HUD.ValueControl( VItem.Lights ).ForeColor = HUD.c_Info;
      if ( SC.SimConnectClient.Instance.AircraftModule.Lights_Beacon ) lightsInt &= (int)GUI.V_Lights.Lights.Beacon;
      if ( SC.SimConnectClient.Instance.AircraftModule.Lights_Nav ) lightsInt &= (int)GUI.V_Lights.Lights.Nav;
      if ( SC.SimConnectClient.Instance.AircraftModule.Lights_Strobe ) lightsInt &= (int)GUI.V_Lights.Lights.Strobe;
      if ( SC.SimConnectClient.Instance.AircraftModule.Lights_Taxi ) {
        lightsInt &= (int)GUI.V_Lights.Lights.Taxi;
        HUD.ValueControl( VItem.Lights ).ForeColor = HUD.c_RA;
      }
      if ( SC.SimConnectClient.Instance.AircraftModule.Lights_Landing ) {
        lightsInt &= (int)GUI.V_Lights.Lights.Landing;
        HUD.ValueControl( VItem.Lights ).ForeColor = HUD.c_RA;
      }
      HUD.Value( VItem.Lights ).IntValue = lightsInt;

      // TORQ, PRPM, ERPM, ITT, EGT, MAN, FFLOW
      HUD.Value( VItem.E1_TORQP ).Value = SC.SimConnectClient.Instance.EngineModule.Turbine1_Torque_prct / 100;  // needs to be 0..1
      HUD.Value( VItem.E2_TORQP ).Value = SC.SimConnectClient.Instance.EngineModule.Turbine2_Torque_prct / 100;  // needs to be 0..1
      HUD.ValueControl( VItem.E2_TORQP ).Visible = ( numEngines > 1 );

      HUD.Value( VItem.E1_TORQ ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_Torque_ft_lbs;
      HUD.Value( VItem.E2_TORQ ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_Torque_ft_lbs;
      HUD.ValueControl( VItem.E2_TORQ ).Visible = ( numEngines > 1 );

      HUD.Value( VItem.P1_RPM ).Value = SC.SimConnectClient.Instance.EngineModule.Propeller1_rpm;
      HUD.Value( VItem.P2_RPM ).Value = SC.SimConnectClient.Instance.EngineModule.Propeller2_rpm;
      HUD.ValueControl( VItem.P2_RPM ).Visible = ( numEngines > 1 );

      HUD.Value( VItem.E1_RPM ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_rpm;
      HUD.Value( VItem.E2_RPM ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_rpm;
      HUD.ValueControl( VItem.E2_RPM ).Visible = ( numEngines > 1 );

      HUD.Value( VItem.E1_N1 ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_N1_prct / 100;  // needs to be 0..1
      HUD.Value( VItem.E2_N1 ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_N1_prct / 100;  // needs to be 0..1
      HUD.ValueControl( VItem.E2_N1 ).Visible = ( numEngines > 1 );

      HUD.Value( VItem.E1_ITT ).Value = SC.SimConnectClient.Instance.EngineModule.Turbine1_itt;
      HUD.Value( VItem.E2_ITT ).Value = SC.SimConnectClient.Instance.EngineModule.Turbine2_itt;
      HUD.ValueControl( VItem.E2_ITT ).Visible = ( numEngines > 1 );

      HUD.Value( VItem.E1_EGT ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_egt;
      HUD.Value( VItem.E2_EGT ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_egt;
      HUD.ValueControl( VItem.E2_EGT ).Visible = ( numEngines > 1 );

      HUD.Value( VItem.E1_MAN ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1MAN_inhg;
      HUD.Value( VItem.E2_MAN ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2MAN_inhg;
      HUD.ValueControl( VItem.E2_MAN ).Visible = ( numEngines > 1 );

      HUD.Value( VItem.E1_FFlow_pph ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_FuelFlow_lbPh;
      HUD.Value( VItem.E2_FFlow_pph ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_FuelFlow_lbPh;
      HUD.ValueControl( VItem.E2_FFlow_pph ).Visible = ( numEngines > 1 );

      HUD.Value( VItem.E1_FFlow_gph ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_FuelFlow_galPh;
      HUD.Value( VItem.E2_FFlow_gph ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_FuelFlow_galPh;
      HUD.ValueControl( VItem.E2_FFlow_gph ).Visible = ( numEngines > 1 );

      HUD.Value( VItem.Fuel_Left ).Value = SC.SimConnectClient.Instance.AircraftModule.FuelQuantityLeft_gal;
      HUD.Value( VItem.Fuel_Right ).Value = SC.SimConnectClient.Instance.AircraftModule.FuelQuantityRight_gal;
      HUD.Value( VItem.Fuel_Total ).Value = SC.SimConnectClient.Instance.AircraftModule.FuelQuantityTotal_gal;

      // GPS (nulled when no flightplan is active)
      if ( SC.SimConnectClient.Instance.GpsModule.IsGpsFlightplan_active ) {
        HUD.Value( VItem.GPS_PWYP ).Text = SC.SimConnectClient.Instance.GpsModule.WYP_prev;
        HUD.Value( VItem.GPS_NWYP ).Text = SC.SimConnectClient.Instance.GpsModule.WYP_next;
        HUD.Value( VItem.GPS_DIST ).Value = SC.SimConnectClient.Instance.GpsModule.WYP_dist;
        HUD.Value( VItem.GPS_ETE ).Value = SC.SimConnectClient.Instance.GpsModule.WYP_ete;
        HUD.Value( VItem.GPS_TRK ).Value = SC.SimConnectClient.Instance.GpsModule.GTRK;
        HUD.Value( VItem.GPS_BRGm ).Value = SC.SimConnectClient.Instance.GpsModule.BRG;
        HUD.Value( VItem.GPS_DTRK ).Value = SC.SimConnectClient.Instance.GpsModule.DTK;
        HUD.Value( VItem.GPS_XTK ).Value = SC.SimConnectClient.Instance.GpsModule.GpsWaypointCrossTRK_nm;
        HUD.Value( VItem.GPS_GS ).Value = SC.SimConnectClient.Instance.AircraftModule.Groundspeed_kt;
        float tgtAlt = SC.SimConnectClient.Instance.GpsModule.WYP_alt;
        HUD.Value( VItem.GPS_ALT ).Value = tgtAlt;
        // Estimates use WYP ALT if >0 (there is no distinction if a WYP ALT is given - it is 0 if not)
        Color estCol = HUD.c_Est;
        if ( tgtAlt == 0 ) {
          // use Set Alt if WYP ALT is zero (see comment above)
          tgtAlt = SC.SimConnectClient.Instance.AP_G1000Module.ALT_setting_ft;
          estCol = HUD.c_Set;
        }
        // Update Estimate Calculation with Acf data
        Estimates.UpdateValues(
          SC.SimConnectClient.Instance.AircraftModule.Groundspeed_kt,
          SC.SimConnectClient.Instance.AircraftModule.AltMsl_ft,
          SC.SimConnectClient.Instance.AircraftModule.VS_ftPmin
        );

        HUD.Value( VItem.EST_VS ).Value = Estimates.VSToTgt_AtAltitude( tgtAlt, SC.SimConnectClient.Instance.GpsModule.WYP_dist );
        HUD.ValueControl( VItem.EST_VS ).ForeColor = estCol;
        HUD.Value( VItem.EST_ALT ).Value = Estimates.AltitudeAtTgt( SC.SimConnectClient.Instance.GpsModule.WYP_dist );
        HUD.ValueControl( VItem.EST_ALT ).ForeColor = estCol;
      }
      else {
        HUD.Value( VItem.GPS_PWYP ).Text = "_____";
        HUD.Value( VItem.GPS_NWYP ).Text = "_____";
        HUD.Value( VItem.GPS_DIST ).Value = null;
        HUD.Value( VItem.GPS_ETE ).Value = null;
        HUD.Value( VItem.GPS_TRK ).Value = null;
        HUD.Value( VItem.GPS_BRGm ).Value = null;
        HUD.Value( VItem.GPS_DTRK ).Value = null;
        HUD.Value( VItem.GPS_XTK ).Value = null;
        HUD.Value( VItem.GPS_GS ).Value = null;
        HUD.Value( VItem.EST_VS ).Value = null; // cannot if we don't have a WYP to aim at
        HUD.Value( VItem.EST_ALT ).Value = null; // cannot if we don't have a WYP to aim at
      }

      // Autopilot
      HUD.LabelControl( VItem.AP ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.AP_mode == FSimClientIF.APMode.On ? HUD.c_AP : HUD.c_Info;

      HUD.LabelControl( VItem.AP_HDG ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.HDG_hold ? HUD.c_Set : HUD.c_Info;
      HUD.Value( VItem.AP_HDGset ).Value = SC.SimConnectClient.Instance.AP_G1000Module.HDG_setting_degm;

      HUD.LabelControl( VItem.AP_ALT ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.ALT_hold ? HUD.c_Set : HUD.c_Info;
      HUD.Value( VItem.AP_ALTset ).Value = SC.SimConnectClient.Instance.AP_G1000Module.ALT_setting_ft;

      HUD.LabelControl( VItem.AP_VS ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.VS_hold ? HUD.c_Set : HUD.c_Info;
      HUD.Value( VItem.AP_VSset ).Value = SC.SimConnectClient.Instance.AP_G1000Module.VS_setting_fpm;

      HUD.LabelControl( VItem.AP_FLC ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.FLC_active ? HUD.c_Set : HUD.c_Info;
      HUD.Value( VItem.AP_FLCset ).Value = SC.SimConnectClient.Instance.AP_G1000Module.IAS_setting_kt;

      HUD.LabelControl( VItem.AP_NAV ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.NAV_hold ? HUD.c_AP : HUD.c_Info;
      HUD.LabelControl( VItem.AP_NAVgps ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.GPS_active ? HUD.c_Gps : HUD.c_Info;
      HUD.LabelControl( VItem.AP_APR ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.APR_hold ? HUD.c_AP : HUD.c_Info;
      HUD.LabelControl( VItem.AP_GS ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.GS_active ? HUD.c_AP : HUD.c_Info;

      // Aircraft Data
      HUD.Value( VItem.HDG ).Value = SC.SimConnectClient.Instance.AircraftModule.HDG_mag_degm;
      HUD.Value( VItem.ALT ).Value = SC.SimConnectClient.Instance.AircraftModule.AltMsl_ft;
      if ( SC.SimConnectClient.Instance.AircraftModule.AltAoG_ft < 1000 ) {
        HUD.Value( VItem.RA ).Value = SC.SimConnectClient.Instance.AircraftModule.AltAoG_ft;
      }
      else {
        HUD.Value( VItem.RA ).Value = null;
      }
      HUD.Value( VItem.IAS ).Value = SC.SimConnectClient.Instance.AircraftModule.IAS_kt;
      HUD.Value( VItem.TAS ).Value = SC.SimConnectClient.Instance.AircraftModule.TAS_kt;
      HUD.Value( VItem.MACH ).Value = SC.SimConnectClient.Instance.AircraftModule.Machspeed_mach;
      HUD.Value( VItem.VS ).Value = SC.SimConnectClient.Instance.AircraftModule.VS_ftPmin;

      // Eval Meters
      m_cpMeter1.Lapse( SC.SimConnectClient.Instance.AircraftModule.Lat, SC.SimConnectClient.Instance.AircraftModule.Lon,
                            SC.SimConnectClient.Instance.AircraftModule.SimTime_zulu_sec );
      m_cpMeter2.Lapse( SC.SimConnectClient.Instance.AircraftModule.Lat, SC.SimConnectClient.Instance.AircraftModule.Lon,
                            SC.SimConnectClient.Instance.AircraftModule.SimTime_zulu_sec );
      m_cpMeter3.Lapse( SC.SimConnectClient.Instance.AircraftModule.Lat, SC.SimConnectClient.Instance.AircraftModule.Lon,
                            SC.SimConnectClient.Instance.AircraftModule.SimTime_zulu_sec );
      HUD.Value( VItem.M_Elapsed1 ).Value = m_cpMeter1.Duration;
      HUD.Value( VItem.M_Dist1 ).Value = (float)m_cpMeter1.Distance;
      HUD.Value( VItem.M_Elapsed2 ).Value = m_cpMeter2.Duration;
      HUD.Value( VItem.M_Dist2 ).Value = (float)m_cpMeter2.Distance;
      HUD.Value( VItem.M_Elapsed3 ).Value = m_cpMeter3.Duration;
      HUD.Value( VItem.M_Dist3 ).Value = (float)m_cpMeter3.Distance;
      HUD.LabelControl( VItem.M_Elapsed1 ).BackColor = m_cpMeter1.Started ? HUD.c_AP : HUD.c_ActBG;
      HUD.LabelControl( VItem.M_Elapsed2 ).BackColor = m_cpMeter2.Started ? HUD.c_AP : HUD.c_ActBG;
      HUD.LabelControl( VItem.M_Elapsed3 ).BackColor = m_cpMeter3.Started ? HUD.c_AP : HUD.c_ActBG;
    }


    #endregion

    /// <summary>
    /// Toggle the connection
    /// </summary>
    private void SimConnect( )
    {
      HUD.Value( VItem.Ad ).Text = ""; // reset in case it had an error message
      HUD.LabelControl( VItem.Ad ).ForeColor = HUD.c_Info;
      HUD.LabelControl( VItem.Ad ).BackColor = HUD.c_BG;

      if ( SC.SimConnectClient.Instance.IsConnected ) {
        // Disconnect from Input and SimConnect
        // FSInput.InputArrived -= FSInput_InputArrived; // Receive commands from FSim -  not yet used
        SC.SimConnectClient.Instance.Disconnect( );
      }
      else {
        // try to connect
        if ( SC.SimConnectClient.Instance.Connect( ) ) {
          HUD.LabelControl( VItem.Ad ).ForeColor = Color.LimeGreen;
          // init the SimClient by pulling one item, so it registers the module, else the callback is not initiated
          _ = SC.SimConnectClient.Instance.AircraftModule.AcftConfigFile;
          // Receive commands from FSim -  not yet used
          /*
          FSInput = SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_01 ); // use first input
          FSInput.InputArrived += FSInput_InputArrived;
          */
        }
        else {
          HUD.LabelControl( VItem.Ad ).BackColor = Color.Red;
          //HUD.Value( GItem.Ad ).Text = SC.SimConnectClient.Instance.ErrorList.FirstOrDefault( ); // error message
          HUD.Value( VItem.Ad ).Text = "NO SIM";
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
      }
      else {
        // If not connected try again
        SimConnect( );
      }
    }

  }
}

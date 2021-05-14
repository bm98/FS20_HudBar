using System;
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
    private bool initDone = false; // need to stop processing while reconfiguring the bar

    private const int c_aETrimTime = 20_000; // 20 sec
    private int m_aETrimTimer = 0; // switch AET off when expired (<=0)

    private const float c_opacity = 0.85f;  // Form opacity when not Fully opaque (slightly transparent)

    private frmConfig CFG = new frmConfig( );


    public frmMain( )
    {
      InitializeComponent( );

      AppSettings.Instance.Reload( );

      m_profiles.Add( new CProfile( AppSettings.Instance.Profile_1_Name, AppSettings.Instance.Profile_1 ) );
      m_profiles.Add( new CProfile( AppSettings.Instance.Profile_2_Name, AppSettings.Instance.Profile_2 ) );
      m_profiles.Add( new CProfile( AppSettings.Instance.Profile_3_Name, AppSettings.Instance.Profile_3 ) );
      m_profiles.Add( new CProfile( AppSettings.Instance.Profile_4_Name, AppSettings.Instance.Profile_4 ) );
      m_profiles.Add( new CProfile( AppSettings.Instance.Profile_5_Name, AppSettings.Instance.Profile_5 ) );

      m_selProfile = AppSettings.Instance.SelProfile;
      // ShowUnits and Opacity are set via HUD in InitGUI

    }

    private void frmMain_Load( object sender, EventArgs e )
    {
      // prepare the GUI
      flp.Dock = DockStyle.Fill;
      this.FormBorderStyle = FormBorderStyle.None; // no frame etc.
      this.TopMost = true; // make sure we float on top

      // attach it to the bottom of the PRIMARY screen (we assume the FS is run on the primary anyway...)
      // full width
      Screen[] screens = Screen.AllScreens;
      Screen mainScreen = screens[0];
      // now get the Primary one
      foreach ( Screen screen in screens ) {
        if ( screen.Primary ) {
          mainScreen = screen;
        }
      }
      this.Width = mainScreen.Bounds.Width;
      // this.Height = ??   need to derive from DPI etc ??? we take it from the current GUI design for the moment
      this.Location = new Point( mainScreen.Bounds.X, mainScreen.Bounds.Y + mainScreen.Bounds.Height - this.Height );

      // Get the controls
      InitGUI( );

      // attach a Callback for the SimClient
      SC.SimConnectClient.Instance.DataArrived += Instance_DataArrived;

      timer1.Interval = 5000; // try to connect in 5 sec intervals
      timer1.Enabled = true;
    }

    private void frmMain_FormClosing( object sender, FormClosingEventArgs e )
    {
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
      if ( CFG.ShowDialog( this ) == DialogResult.OK ) {

        // Save all configuration properties
        AppSettings.Instance.ShowUnits = HUD.ShowUnits;
        AppSettings.Instance.Opaque = HUD.OpaqueBackground;
        AppSettings.Instance.FontSize = (int)HUD.FontSize;

        AppSettings.Instance.SelProfile = m_selProfile;
        AppSettings.Instance.Profile_1_Name = m_profiles[0].PName;
        AppSettings.Instance.Profile_1 = m_profiles[0].ProfileString( );
        AppSettings.Instance.Profile_2_Name = m_profiles[1].PName;
        AppSettings.Instance.Profile_2 = m_profiles[1].ProfileString( );
        AppSettings.Instance.Profile_3_Name = m_profiles[2].PName;
        AppSettings.Instance.Profile_3 = m_profiles[2].ProfileString( );
        AppSettings.Instance.Profile_4_Name = m_profiles[3].PName;
        AppSettings.Instance.Profile_4 = m_profiles[3].ProfileString( );
        AppSettings.Instance.Profile_5_Name = m_profiles[4].PName;
        AppSettings.Instance.Profile_5 = m_profiles[4].ProfileString( );
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


    #region Callback Handlers

    private void Instance_DataArrived( object sender, FSimClientIF.ClientDataArrivedEventArgs e )
    {
      UpdateGUI( );
    }

    #endregion


    #region GUI

    private HudBar HUD = null;
    private CProfile PROFILE = null;

    // initialize the labels and default values
    // sequence defines appearance
    private void InitGUI( )
    {
      initDone = false; // stop updating values while reconfiguring

      // Update profile selection items
      mP1.Text = m_profiles[0].PName;
      mP2.Text = m_profiles[1].PName;
      mP3.Text = m_profiles[2].PName;
      mP4.Text = m_profiles[3].PName;
      mP5.Text = m_profiles[4].PName;


      // start from scratch
      HUD = new HudBar( lblProto, valueProto, value2Proto, signProto,
                          AppSettings.Instance.ShowUnits, 
                          AppSettings.Instance.Opaque, 
                          (GUI.FontSize)AppSettings.Instance.FontSize );
      // set form opacity from settings (use the const value for slight transparency)
      this.Opacity = HUD.OpaqueBackground ? 1 : c_opacity;

      PROFILE = m_profiles[m_selProfile];

      flp.Controls.Clear( ); // reload

      // load flowLayout from Config Profile
      foreach ( GItem i in Enum.GetValues( typeof( GItem ) ) ) {
        if ( i == GItem.ITEM_COUNT ) continue; // murks..

        if ( PROFILE.ShowItem( i ) ) {
          Control l = HUD.LabelControl( i );
          if ( l != null ) {
            if ( l is GUI.B_Base ) {
              ( l as GUI.B_Base ).ButtonClicked += FrmMain_ButtonClicked;
            }
            if ( l != null ) flp.Controls.Add( l );
          }
          l = HUD.ValueControl( i );
          if ( l != null ) {
            if ( l is GUI.B_Base ) {
              ( l as GUI.B_Base ).ButtonClicked += FrmMain_ButtonClicked;
            }
            flp.Controls.Add( l );
          }
        }
      }
      if ( SC.SimConnectClient.Instance.IsConnected )
        flp.Controls[0].ForeColor = Color.LimeGreen;

      initDone = true;
    }

    /// <summary>
    /// Handle the click events here
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void FrmMain_ButtonClicked( object sender, GUI.ClickedEventArgs e )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return; // no action

      switch ( e.Item ) {
        case GItem.AP:
          SC.SimConnectClient.Instance.AP_G1000Module.Master_toggle( );
          break;
        case GItem.AP_ALT:
          SC.SimConnectClient.Instance.AP_G1000Module.ALT_hold = true; // toggles independet of the set value
          break;
        case GItem.AP_APR:
          SC.SimConnectClient.Instance.AP_G1000Module.APR_hold = true; // toggles independet of the set value
          break;
        case GItem.AP_FLC:
          SC.SimConnectClient.Instance.AP_G1000Module.FLC_active = true; // toggles independet of the set value
          break;
        case GItem.AP_HDG:
          SC.SimConnectClient.Instance.AP_G1000Module.HDG_hold = true; // toggles independet of the set value
          break;
        case GItem.AP_NAV:
          SC.SimConnectClient.Instance.AP_G1000Module.NAV_hold = true; // toggles independet of the set value
          break;
        case GItem.AP_VS:
          SC.SimConnectClient.Instance.AP_G1000Module.VS_hold = true; // toggles independet of the set value
          break;
        case GItem.ETrim:
          SC.SimConnectClient.Instance.AutoETrimModule.Enabled = !SC.SimConnectClient.Instance.AutoETrimModule.Enabled; // toggles
          m_aETrimTimer = c_aETrimTime;
          break;
        case GItem.BARO_HPA:
          SC.SimConnectClient.Instance.AircraftModule.AltimeterSetting = true;
          break;
        case GItem.BARO_InHg:
          SC.SimConnectClient.Instance.AircraftModule.AltimeterSetting=true;
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
      if ( !initDone ) return; // cannot access items at this time

      // we do this one by one..

      // TRIMS
      HUD.LabelControl( GItem.ETrim ).BackColor = SC.SimConnectClient.Instance.AutoETrimModule.Enabled ? HUD.c_AP : this.BackColor;
      HUD.Value( GItem.ETrim ).Value = SC.SimConnectClient.Instance.AircraftModule.PitchTrim_prct;
      HUD.Value( GItem.RTrim ).Value = SC.SimConnectClient.Instance.AircraftModule.RudderTrim_prct;
      HUD.Value( GItem.ATrim ).Value = SC.SimConnectClient.Instance.AircraftModule.AileronTrim_prct;
      // OAT, BARO
      HUD.Value( GItem.OAT ).Value = SC.SimConnectClient.Instance.AircraftModule.OutsideTemperature_degC;
      HUD.ValueControl( GItem.OAT ).ForeColor = ( SC.SimConnectClient.Instance.AircraftModule.OutsideTemperature_degC < 0 ) ? HUD.c_SubZero : HUD.c_Info;

      HUD.Value( GItem.BARO_HPA ).Value = SC.SimConnectClient.Instance.AircraftModule.AltimeterSetting_inHg;
      HUD.Value( GItem.BARO_InHg ).Value = SC.SimConnectClient.Instance.AircraftModule.AltimeterSetting_mbar;

      // Gear, Brakes, Flaps
      if ( SC.SimConnectClient.Instance.AircraftModule.IsGearRetractable ) {
        HUD.Value( GItem.Gear ).Step =
          SC.SimConnectClient.Instance.AircraftModule.GearPosition == FSimClientIF.GearPosition.Down ? GUI.Steps.Down :
          SC.SimConnectClient.Instance.AircraftModule.GearPosition == FSimClientIF.GearPosition.Up ? GUI.Steps.Up : GUI.Steps.Unk;
      }

      HUD.Value( GItem.Brakes ).Step = SC.SimConnectClient.Instance.AircraftModule.Parkbrake_on ? GUI.Steps.On : GUI.Steps.Off;

      if ( SC.SimConnectClient.Instance.AircraftModule.Flaps == FSimClientIF.CmdMode.Up ) {
        HUD.Value( GItem.Flaps ).Step = GUI.Steps.Up;
      }
      else if ( SC.SimConnectClient.Instance.AircraftModule.Flaps == FSimClientIF.CmdMode.Down ) {
        HUD.Value( GItem.Flaps ).Step = GUI.Steps.Down;
      }
      else if ( SC.SimConnectClient.Instance.AircraftModule.Flaps == FSimClientIF.CmdMode.Pos1 ) {
        HUD.Value( GItem.Flaps ).Step = GUI.Steps.P1;
      }
      else if ( SC.SimConnectClient.Instance.AircraftModule.Flaps == FSimClientIF.CmdMode.Pos2 ) {
        HUD.Value( GItem.Flaps ).Step = GUI.Steps.P2;
      }
      else if ( SC.SimConnectClient.Instance.AircraftModule.Flaps == FSimClientIF.CmdMode.Pos3 ) {
        HUD.Value( GItem.Flaps ).Step = GUI.Steps.P3;
      }

      // TORQ, PRPM, ERPM, FFLOW
      HUD.Value( GItem.E1_TORQP ).Value = SC.SimConnectClient.Instance.EngineModule.Turbine1_Torque_prct / 100;  // needs to be 0..1
      HUD.Value( GItem.E2_TORQP ).Value = SC.SimConnectClient.Instance.EngineModule.Turbine2_Torque_prct / 100;  // needs to be 0..1
      HUD.Value( GItem.E1_TORQ ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_Torque_ft_lbs;
      HUD.Value( GItem.E2_TORQ ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_Torque_ft_lbs;

      HUD.Value( GItem.P1_RPM ).Value = SC.SimConnectClient.Instance.EngineModule.Propeller1_rpm;
      HUD.Value( GItem.P2_RPM ).Value = SC.SimConnectClient.Instance.EngineModule.Propeller2_rpm;
      HUD.Value( GItem.E1_RPM ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_rpm;
      HUD.Value( GItem.E2_RPM ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_rpm;
      HUD.Value( GItem.E1_N1 ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_N1_prct / 100;  // needs to be 0..1
      HUD.Value( GItem.E2_N1 ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_N1_prct / 100;  // needs to be 0..1

      HUD.Value( GItem.E1_ITT ).Value = SC.SimConnectClient.Instance.EngineModule.Turbine1_itt;
      HUD.Value( GItem.E2_ITT ).Value = SC.SimConnectClient.Instance.EngineModule.Turbine2_itt;
      HUD.Value( GItem.E1_EGT ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_egt;
      HUD.Value( GItem.E2_EGT ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_egt;

      HUD.Value( GItem.E1_FFlow ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_FuelFlow_lbPh;
      HUD.Value( GItem.E2_FFlow ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_FuelFlow_lbPh;

      // GPS
      if ( SC.SimConnectClient.Instance.GpsModule.IsGpsFlightplan_active ) {
        HUD.Value( GItem.GPS_PWYP ).Text = SC.SimConnectClient.Instance.GpsModule.WYP_prev;
        HUD.Value( GItem.GPS_NWYP ).Text = SC.SimConnectClient.Instance.GpsModule.WYP_next;
        HUD.Value( GItem.GPS_DIST ).Value = SC.SimConnectClient.Instance.GpsModule.WYP_dist;
        HUD.Value( GItem.GPS_ETE ).Value = SC.SimConnectClient.Instance.GpsModule.WYP_ete;
        HUD.Value( GItem.GPS_TRK ).Value = SC.SimConnectClient.Instance.GpsModule.GTRK;
        HUD.Value( GItem.GPS_GS ).Value = SC.SimConnectClient.Instance.AircraftModule.Groundspeed_kt;
      }
      else {
        HUD.Value( GItem.GPS_PWYP ).Text = "_____";
        HUD.Value( GItem.GPS_NWYP ).Text = "_____";
        HUD.Value( GItem.GPS_DIST ).Value = null;
        HUD.Value( GItem.GPS_ETE ).Value = null;
        HUD.Value( GItem.GPS_TRK ).Value = null;
        HUD.Value( GItem.GPS_GS ).Value = null;
      }

      // Autopilot
      HUD.LabelControl( GItem.AP ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.AP_mode == FSimClientIF.APMode.On ? HUD.c_AP : HUD.c_Info;

      HUD.LabelControl( GItem.AP_HDG ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.HDG_hold ? HUD.c_Set : HUD.c_Info;
      HUD.Value( GItem.AP_HDGset ).Value = SC.SimConnectClient.Instance.AP_G1000Module.HDG_setting_degm;

      HUD.LabelControl( GItem.AP_ALT ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.ALT_hold ? HUD.c_Set : HUD.c_Info;
      HUD.Value( GItem.AP_ALTset ).Value = SC.SimConnectClient.Instance.AP_G1000Module.ALT_setting_ft;

      HUD.LabelControl( GItem.AP_VS ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.VS_hold ? HUD.c_Set : HUD.c_Info;
      HUD.Value( GItem.AP_VSset ).Value = SC.SimConnectClient.Instance.AP_G1000Module.VS_setting_fpm;

      HUD.LabelControl( GItem.AP_FLC ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.FLC_active ? HUD.c_Set : HUD.c_Info;
      HUD.Value( GItem.AP_FLCset ).Value = SC.SimConnectClient.Instance.AP_G1000Module.IAS_setting_kt;

      HUD.LabelControl( GItem.AP_NAV ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.NAV_hold ? HUD.c_AP : HUD.c_Info;
      HUD.LabelControl( GItem.AP_NAVgps ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.GPS_active ? HUD.c_Gps : HUD.c_Info;
      HUD.LabelControl( GItem.AP_APR ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.APR_hold ? HUD.c_AP : HUD.c_Info;
      HUD.LabelControl( GItem.AP_GS ).ForeColor = SC.SimConnectClient.Instance.AP_G1000Module.GS_active ? HUD.c_AP : HUD.c_Info;

      // Aircraft Data
      HUD.Value( GItem.HDG ).Value = SC.SimConnectClient.Instance.AircraftModule.HDG_mag_degm;
      HUD.Value( GItem.ALT ).Value = SC.SimConnectClient.Instance.AircraftModule.AltMsl_ft;
      if ( SC.SimConnectClient.Instance.AircraftModule.AltAoG_ft < 1000 ) {
        HUD.Value( GItem.RA ).Value = SC.SimConnectClient.Instance.AircraftModule.AltAoG_ft;
      }
      else {
        HUD.Value( GItem.RA ).Value = null;
      }
      HUD.Value( GItem.IAS ).Value = SC.SimConnectClient.Instance.AircraftModule.IAS_kt;
      HUD.Value( GItem.VS ).Value = SC.SimConnectClient.Instance.AircraftModule.VS_ftPmin;
    }


    #endregion

    /// <summary>
    /// Toggle the connection
    /// </summary>
    private void SimConnect( )
    {
      HUD.Value( GItem.Ad ).Text = ""; // reset in case it had an error message
      flp.Controls[0].ForeColor = this.ForeColor;
      flp.Controls[0].BackColor = this.BackColor;
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        // Disconnect from Input and SimConnect
        SC.SimConnectClient.Instance.Disconnect( );
      }
      else {
        // try to connect
        if ( SC.SimConnectClient.Instance.Connect( ) ) {
          flp.Controls[0].ForeColor = Color.LimeGreen;
          _ = SC.SimConnectClient.Instance.AircraftModule.AcftConfigFile; // init by pulling one item, so it registers the module
        }
        else {
          flp.Controls[0].BackColor = Color.Red;
          HUD.Value( GItem.Ad ).Text = SC.SimConnectClient.Instance.ErrorList.FirstOrDefault( ); // error message
        }
      }

    }

    /// <summary>
    /// Try every intervall to connect - and if connected.. do in Sim chores
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
          m_aETrimTimer -= timer1.Interval; // dec timer
        }

        return; // already connected
      }

      // If not connected try again
      SimConnect( );
    }



  }
}

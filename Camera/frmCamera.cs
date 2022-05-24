using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using FSimClientIF;
using SC = SimConnectClient;

namespace FS20_HudBar.Camera
{
  public partial class frmCamera : Form
  {
    private Dictionary<CameraSetting, ModeButton> _btMode = new Dictionary<CameraSetting, ModeButton>( );
    private List<Button> _btIndex = new List<Button>( ); // Cam Index
    private List<Button> _btSlot = new List<Button>( ); // Starred Slots
    private List<Button> _btSlotFolder = new List<Button>( ); // Slot Folders

    private int _observerID = 0;

    // View Colors
    private readonly Color c_vPassive = Color.SteelBlue;
    private readonly Color c_vActive = Color.DarkTurquoise;
    // SlotFolder Colors
    private readonly Color c_sfPassive = Color.Olive;
    private readonly Color c_sfActive = Color.Gold;


    // tracking stuff
    private int _actIndex = 0;
    private int _prevIndex = 0;
    private CameraSetting _actSetting = CameraSetting.NONE;
    private CameraSetting _prevSetting = CameraSetting.NONE;

    private bool _slotSaving = false;


    public frmCamera( )
    {
      InitializeComponent( );

      // Load Lists and Catalogs
      _btMode = new Dictionary<CameraSetting, ModeButton>( ) {
        { CameraSetting.NONE, new ModeButton(CameraSetting.NONE, new Button() /*a dummy one*/, SwitchCamera ) },
        { CameraSetting.Cockpit_Pilot,new ModeButton( CameraSetting.Cockpit_Pilot, btPilotView, SwitchCamera )},
        { CameraSetting.Cockpit_Instrument,new ModeButton(CameraSetting.Cockpit_Instrument, btInstrumentView, SwitchCamera ) },
        { CameraSetting.Cockpit_Quick, new ModeButton(CameraSetting.Cockpit_Quick,btCockpitQuick, SwitchCamera ) },
        { CameraSetting.Ext_Default,new ModeButton(CameraSetting.Ext_Default, btExternal, SwitchCamera ) },
        { CameraSetting.Ext_Quick,new ModeButton(CameraSetting.Ext_Quick, btExternalQuick, SwitchCamera )},
        { CameraSetting.ShCase_Drone,new ModeButton(CameraSetting.ShCase_Drone, btDrone, SwitchCamera )},
        { CameraSetting.ShCase_Fixed,new ModeButton( CameraSetting.ShCase_Fixed, btShowcase, SwitchCamera) },
      };
      _btIndex = new List<Button> { btIndex01, btIndex02, btIndex03, btIndex04, btIndex05, btIndex06, btIndex07, btIndex08,
                                    btIndex09, btIndex10, btIndex11, btIndex12, btIndex13, btIndex14, btIndex15, btIndex16,
                                    btIndex17, btIndex18, btIndex19, btIndex20, btIndex21, btIndex22, btIndex23, btIndex24 };
      _btSlot = new List<Button> { btSlot01, btSlot02, btSlot03, btSlot04, btSlot05, btSlot06, btSlot07, btSlot08,
                                    btSlot09, btSlot10 };
      _btSlotFolder = new List<Button> { btSlotFolderA, btSlotFolderB, btSlotFolderC, btSlotFolderD, btSlotFolderE, btSlotFolderF };

      // Starred Slots init
      SlotCat.InitSlotFolders( _btSlot, SwitchCamera );
      // get save confirmation callback
      foreach (var slotFolder in SlotCat.SlotFolders) {
        slotFolder.SlotSaved += _slots_SlotSaved;
      }
    }


    private void frmCamera_Load( object sender, EventArgs e )
    {
      // Init GUI

      // reset button colors
      foreach (var b in _btIndex) { b.BackColor = c_vPassive; }
      foreach (var b in _btMode) { b.Value.Button.BackColor = c_vPassive; }
      foreach (var b in _btSlotFolder) { b.ForeColor = c_sfPassive; }

      // Load AppSettings A..F - clumsy but the settings are by SlotFolder and not all in one
      SlotCat.SlotFolders[0].AppSettingString = AppSettings.Instance.CameraSlotFolder0;
      SlotCat.SlotFolders[1].AppSettingString = AppSettings.Instance.CameraSlotFolder1;
      SlotCat.SlotFolders[2].AppSettingString = AppSettings.Instance.CameraSlotFolder2;
      SlotCat.SlotFolders[3].AppSettingString = AppSettings.Instance.CameraSlotFolder3;
      SlotCat.SlotFolders[4].AppSettingString = AppSettings.Instance.CameraSlotFolder4;
      SlotCat.SlotFolders[5].AppSettingString = AppSettings.Instance.CameraSlotFolder5;
      // Enable SlotFolder A
      SlotCat.SetActiveSlotFolder( 0 );
      _btSlotFolder[SlotCat.CurrentSlotFolder.SlotFolderNo].ForeColor = c_sfActive;

      lblSaveStar.Visible = false;
    }


    private void frmCamera_VisibleChanged( object sender, EventArgs e )
    {
      if (this.Visible) {
        // register DataUpdates
        if (SC.SimConnectClient.Instance.IsConnected)
          _observerID = SC.SimConnectClient.Instance.CameraModule.AddObserver( "HUDBAR_CAMERA_FORM", DataUpdate );
      }
    }

    private void frmCamera_FormClosing( object sender, FormClosingEventArgs e )
    {
      // UnRegister DataUpdates
      if (SC.SimConnectClient.Instance.IsConnected && (_observerID >= 0))
        SC.SimConnectClient.Instance.CameraModule.RemoveObserver( _observerID );

      if (this.Visible && (this.WindowState == FormWindowState.Normal)) {
        AppSettings.Instance.CameraLocation = this.Location;
        AppSettings.Instance.CameraSize = this.Size;
      }

      if (e.CloseReason == CloseReason.UserClosing) {
        // we don't close if the User clicks the X Box, only Hide; else it will not maintain the content throughout
        e.Cancel = true;
        this.Hide( );
      }

    }

    /// <summary>
    /// Handle Data Arrival...
    /// </summary>
    /// <param name="refName"></param>
    private void DataUpdate( string refName )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      // Index Buttons (_prevIndex SHALL never be out of range...)
      _btIndex[_prevIndex].BackColor = c_vPassive;
      if (SC.SimConnectClient.Instance.CameraModule.ActCamView_index < _btIndex.Count) {
        _prevIndex = _actIndex;
        _actIndex = (int)SC.SimConnectClient.Instance.CameraModule.ActCamView_index;
        _btIndex[_actIndex].BackColor = c_vActive;
      }
      // ViewMode Buttons
      _btMode[_prevSetting].Button.BackColor = c_vPassive;
      if (SC.SimConnectClient.Instance.CameraModule.CameraSetting != CameraSetting.NONE) {
        _prevSetting = _actSetting;
        _actSetting = SC.SimConnectClient.Instance.CameraModule.CameraSetting;
        _btMode[_actSetting].Button.BackColor = c_vActive;
      }
    }

    // Handle the Slot Saved Event from the SlotCat
    private void _slots_SlotSaved( object sender, EventArgs e )
    {
      // Save Settings A..F
      AppSettings.Instance.CameraSlotFolder0 = SlotCat.SlotFolders[0].AppSettingString;
      AppSettings.Instance.CameraSlotFolder1 = SlotCat.SlotFolders[1].AppSettingString;
      AppSettings.Instance.CameraSlotFolder2 = SlotCat.SlotFolders[2].AppSettingString;
      AppSettings.Instance.CameraSlotFolder3 = SlotCat.SlotFolders[3].AppSettingString;
      AppSettings.Instance.CameraSlotFolder4 = SlotCat.SlotFolders[4].AppSettingString;
      AppSettings.Instance.CameraSlotFolder5 = SlotCat.SlotFolders[5].AppSettingString;

      lblSaveStar.Visible = false;
      _slotSaving = false;
    }

    // Save Slot from current settings
    private void btSaveToSlot_Click( object sender, EventArgs e )
    {
      if (_slotSaving) {
        // click when active will cancel the save
        lblSaveStar.Visible = false;
        _slotSaving = false;
        SlotCat.CurrentSlotFolder.CancelSlotSave( );
      }
      else {
        // sanity
        if (!SC.SimConnectClient.Instance.IsConnected) return;

        // click when inactive will trigger the save
        _slotSaving = true;
        lblSaveStar.Visible = true;
        SlotCat.CurrentSlotFolder.ExpectSlotSave( );
      }
    }

    /// <summary>
    /// The MAIN switch the Camera View Method
    /// </summary>
    /// <param name="setting">The Cam Setting</param>
    /// <param name="viewIndex">The CamView Index</param>
    private void SwitchCamera( CameraSetting setting, uint viewIndex )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      _btMode[setting].ViewIndex = viewIndex; // save preset default
      if (SC.SimConnectClient.Instance.CameraModule.CameraSetting == setting) {
        // already in requested View, set index
        // - it does not always change if the view is switched to the same and then the index is set ??? Asobo ???
        SC.SimConnectClient.Instance.CameraModule.ActCamView_index = viewIndex;
      }
      else {
        SC.SimConnectClient.Instance.CameraModule.CameraSetting = setting;
        SC.SimConnectClient.Instance.CameraModule.ActCamView_index = viewIndex;
      }
    }


    // Handle Reset View Click
    private void btResetView_Click( object sender, EventArgs e )
    {
      SC.SimConnectClient.Instance.CameraModule.ActCam_Reset( );
    }


    // Handle any ViewIndex Button click
    private void btIndex_Click( object sender, EventArgs e )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (!(sender is Button)) return;

      var bt = sender as Button;
      var index = _btIndex.FindIndex( x => x.Name == bt.Name );
      if (index >= 0) {
        // Cam Index Selectors
        SC.SimConnectClient.Instance.CameraModule.ActCamView_index = (uint)index;
        _btMode[SC.SimConnectClient.Instance.CameraModule.CameraSetting].ViewIndex = (uint)index; // save the last selected for later use

      }
      else {
        // Pilot View Position Selectors
        uint idx = 0;
        if (bt.Name == "btIndexPilot") {
          idx = (uint)PilotCamPosition.Pilot;
        }
        else if (bt.Name == "btIndexClose") {
          idx = (uint)PilotCamPosition.Close;
        }
        else if (bt.Name == "btIndexLand") {
          idx = (uint)PilotCamPosition.Landing;
        }
        else if (bt.Name == "btIndexCoPilot") {
          idx = (uint)PilotCamPosition.Copilot;
        }
        // switch
        SwitchCamera( CameraSetting.Cockpit_Pilot, idx );
      }
    }

    // Make a SlotFolder current
    private void btSlotFolder_Click( object sender, EventArgs e )
    {
      if (!(sender is Button)) return;

      var bt = sender as Button;
      var index = _btSlotFolder.FindIndex( x => x.Name == bt.Name );
      if (index >= 0) {
        _btSlotFolder[SlotCat.CurrentSlotFolder.SlotFolderNo].ForeColor = c_sfPassive;
        SlotCat.SetActiveSlotFolder( (uint)index );
        _btSlotFolder[SlotCat.CurrentSlotFolder.SlotFolderNo].ForeColor = c_sfActive;
      }
    }
  }
}

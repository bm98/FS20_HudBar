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

    private int _observerID = -1;// -1 triggers first update from Sim,

    // View Colors
    private readonly Color c_vPassive = Color.SteelBlue;
    private readonly Color c_vActive = Color.DarkTurquoise;
    private readonly Color c_vPassiveText = Color.CadetBlue;
    private readonly Color c_vActiveText = Color.Black;
    // SlotFolder Colors
    private readonly Color c_sfPassive = Color.Olive;
    private readonly Color c_sfActive = Color.Gold;
    // SmartCam Colors 
    private readonly Color c_scActiveText = Color.Gold;
    private readonly Color c_scPassiveText = Color.Gainsboro;



    // tracking stuff
    private bool _updating = false;

    private int _actIndex = 0;
    private int _prevIndex = 0;
    private CameraSetting _actSetting = CameraSetting.NONE;
    private CameraSetting _prevSetting = CameraSetting.NONE;

    private const int c_triggerUpdate = 3; // N tries to update values after the Sim has changed them, sometimes it needs persistence...
    private int _droneValueUpdate = 0; // trigger delayed update to Sim
    private int _droneMovement = -1; // -1 triggers first update from Sim, else only when moving the slider
    private int _droneRotation = -1; // -1 triggers first update from Sim, else only when moving the slider


    private int _camUpdate = 3; // N tries to update values after the Sim has changed them, sometimes it needs persistence...
    private CameraSetting _camSetting = CameraSetting.NONE;
    private uint _camIndex = 0;

    private UInt64 _smartTargetHash = 0; // triggers update of the Smart Target List if changed

    private uint _maxIndexEnabled = 0; // track the enabled Index Buttons and change only when needed to avoid flicker

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
        { CameraSetting.Cockpit_Custom, new ModeButton(CameraSetting.Cockpit_Custom,btCustomView, SwitchCamera ) },
        { CameraSetting.Ext_Default,new ModeButton(CameraSetting.Ext_Default, btExternal, SwitchCamera ) },
        { CameraSetting.Ext_Quick,new ModeButton(CameraSetting.Ext_Quick, btExternalQuick, SwitchCamera )},
        { CameraSetting.ShCase_Drone,new ModeButton(CameraSetting.ShCase_Drone, btDrone, SwitchCamera )},
        { CameraSetting.ShCase_Fixed,new ModeButton( CameraSetting.ShCase_Fixed, btShowcase, SwitchCamera) },
      };
      _btIndex = new List<Button> { btIndex01, btIndex02, btIndex03, btIndex04, btIndex05, btIndex06, btIndex07, btIndex08,
                                    btIndex09, btIndex10, btIndex11, btIndex12, btIndex13, btIndex14, btIndex15, btIndex16, btIndex17, btIndex18,
                                    btIndex19, btIndex20, btIndex21, btIndex22, btIndex23, btIndex24, btIndex25, btIndex26, btIndex27, btIndex28  };
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
        if (SC.SimConnectClient.Instance.IsConnected && _observerID < 0) {
          _observerID = SC.SimConnectClient.Instance.CameraModule.AddObserver( "HUDBAR_CAMERA_FORM", DataUpdate );
          timer1.Enabled = true;
        }
      }
    }

    private void frmCamera_FormClosing( object sender, FormClosingEventArgs e )
    {
      timer1.Enabled = false;

      // UnRegister DataUpdates
      if (SC.SimConnectClient.Instance.IsConnected && (_observerID >= 0))
        SC.SimConnectClient.Instance.CameraModule.RemoveObserver( _observerID );
      _observerID = -1;

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

      _updating = true;

      var cam = SC.SimConnectClient.Instance.CameraModule;

      // Index Buttons (_prevIndex SHALL never be out of range...)
      _btIndex[_prevIndex].BackColor = c_vPassive;
      if (cam.ActCamView_index < _btIndex.Count) {
        _prevIndex = _actIndex;
        _actIndex = (int)cam.ActCamView_index;
        _btIndex[_actIndex].BackColor = c_vActive;
      }
      SetIndexEnabled( cam.ActCam_View_index_max( cam.ActCam_viewType ) );

      // ViewMode Buttons
      _btMode[_prevSetting].Button.BackColor = c_vPassive;
      if (cam.CameraSetting != CameraSetting.NONE) {
        _prevSetting = _actSetting;
        _actSetting = cam.CameraSetting;
        _btMode[_actSetting].Button.BackColor = c_vActive;
      }

      // Drone 
      cbxDroneLock.CheckState = cam.DroneCam_locked ? CheckState.Checked : CheckState.Unchecked;
      cbxDroneFollow.CheckState = cam.DroneCam_follow ? CheckState.Checked : CheckState.Unchecked;
      tbMove.Value = (int)cam.DroneCam_movespeed;
      tbRotate.Value = (int)cam.DroneCam_rotspeed;
      if (_droneMovement < 2) _droneMovement = (int)cam.DroneCam_movespeed; // init or don't stay at 0
      if (_droneRotation < 2) _droneRotation = (int)cam.DroneCam_rotspeed; // init or don't stay at 0
      if ( cam.CameraSetting== CameraSetting.ShCase_Drone) {
        pnlDroneSlider.BackColor = pnlDrone.BackColor;
        cbxDroneFollow.Enabled = true;
        cbxDroneLock.Enabled = true;
      }
      else {
        pnlDroneSlider.BackColor = this.BackColor; // Window Back
        cbxDroneFollow.Enabled = false;
        cbxDroneLock.Enabled = false;
      }

      // SmartTarget 
      if (cam.SmartCam_active) {
        txSmartTargetType.Text = $"{cam.SmarCamTarget_selected_type}";
        txSmartTargetName.Text = cam.SmarCamTarget_selected_name;
        txSmartTargetType.ForeColor = c_scActiveText;
        txSmartTargetName.ForeColor = c_scActiveText;
        lbxSmartTargets.ForeColor = c_scActiveText;
      }
      else {
        txSmartTargetType.Text = "---";
        txSmartTargetName.Text = "---";
        txSmartTargetType.ForeColor = c_scPassiveText;
        txSmartTargetName.ForeColor = c_scPassiveText;
        lbxSmartTargets.ForeColor = c_scPassiveText;
      }

      PopulateSmartTargets( cam ); // new targets to show

      _updating = false;
    }

    // Knuth hash
    private static UInt64 CalculateHash( string read )
    {
      UInt64 hashedValue = 3074457345618258791ul;
      for (int i = 0; i < read.Length; i++) {
        hashedValue += read[i];
        hashedValue *= 3074457345618258799ul;
      }
      return hashedValue;
    }

    // Updates the SmartCam Target list if needed (something has changed)
    private void PopulateSmartTargets( FSimClientIF.Modules.ICamera cam )
    {
      // Build the list
      var list = new List<string>( );
      for (uint i = 0; i < 10; i++) {
        var s = cam.SmartCamTarget_name( i ); // can be null
        list.Add( string.IsNullOrEmpty( s ) ? "---" : $"{cam.TargetTypeClass(cam.SmartCamTarget_type(i))}-{s}" );
      }
      // if changed - load the list into the ListBox
      var tgtHash = CalculateHash( string.Concat( list ) );
      if (tgtHash != _smartTargetHash) {
        lbxSmartTargets.Items.Clear( );
        foreach (var s in list) lbxSmartTargets.Items.Add( s );
        _smartTargetHash = tgtHash;
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


    // Handle Reset View Click
    private void btResetView_Click( object sender, EventArgs e )
    {
      SC.SimConnectClient.Instance.CameraModule.ActCam_Reset( );
      // restore Drone speeds
      _droneValueUpdate = c_triggerUpdate;
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

    private void lbxSmartTargets_SelectedIndexChanged( object sender, EventArgs e )
    {
      // sanity
      if (_updating) return;
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (SC.SimConnectClient.Instance.CameraModule.CameraSetting == CameraSetting.ShCase_Drone) return;
      if (SC.SimConnectClient.Instance.CameraModule.CameraSetting == CameraSetting.ShCase_Fixed) return;

      int selected = lbxSmartTargets.SelectedIndex;
      if ((selected >= 0) && (selected < 10)) {
        if (!SC.SimConnectClient.Instance.CameraModule.SmartCam_active)
          SC.SimConnectClient.Instance.CameraModule.SmartCam_active = true;

        SC.SimConnectClient.Instance.CameraModule.SmartCamTarget_selected = (uint)selected;
      }
    }

    private void cbxDroneLock_CheckedChanged( object sender, EventArgs e )
    {
      if (_updating) return;
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SC.SimConnectClient.Instance.CameraModule.DroneCam_locked = cbxDroneLock.Checked;
    }

    private void cbxDroneFollow_CheckedChanged( object sender, EventArgs e )
    {
      if (_updating) return;
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SC.SimConnectClient.Instance.CameraModule.DroneCam_follow = cbxDroneFollow.Checked;
    }

    private void tbMove_ValueChanged( object sender, EventArgs e )
    {
      if (_updating) return;
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      _droneMovement = tbMove.Value;
      SC.SimConnectClient.Instance.CameraModule.DroneCam_movespeed = tbMove.Value;
    }

    private void tbRotate_ValueChanged( object sender, EventArgs e )
    {
      if (_updating) return;
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      _droneRotation = tbRotate.Value;
      SC.SimConnectClient.Instance.CameraModule.DroneCam_rotspeed = tbRotate.Value;
    }

    // more murks.. 
    // sometimes a delayed SimConnect Call is needed to set values that the Sim changes within one Trigger cycle
    private void timer1_Tick( object sender, EventArgs e )
    {
      if (_updating) return;
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      if (_droneValueUpdate > 0) {
        _droneValueUpdate--;
        // restore Drone speeds if in Drone Mode
        if (SC.SimConnectClient.Instance.CameraModule.CameraSetting == CameraSetting.ShCase_Drone) {
          SC.SimConnectClient.Instance.CameraModule.DroneCam_movespeed = _droneMovement;
          SC.SimConnectClient.Instance.CameraModule.DroneCam_rotspeed = _droneRotation;
        }
      }
      if (_camUpdate > 0) {
        _camUpdate--;
        SwitchCamera_low( _camSetting, _camIndex );
      }
    }


    /// <summary>
    /// The MAIN switch the Camera View Method
    /// </summary>
    /// <param name="setting">The Cam Setting</param>
    /// <param name="viewIndex">The CamView Index</param>
    private void SwitchCamera_low( CameraSetting setting, uint viewIndex )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      var cam = SC.SimConnectClient.Instance.CameraModule;

      _btMode[setting].ViewIndex = viewIndex; // save preset default
      if (SC.SimConnectClient.Instance.CameraModule.CameraSetting != setting) {
        // change if not already there
        cam.SmartCam_active = false;
        SC.SimConnectClient.Instance.CameraModule.CameraSetting = setting;
      }
      // Set the ViewIndex
      if (setting != CameraSetting.Cockpit_Custom) {
        // custom cams don't have a ViewIndex
        SC.SimConnectClient.Instance.CameraModule.ActCamView_index = viewIndex;
      }
      // restore Drone speeds
      _droneValueUpdate = c_triggerUpdate;

    }
    /// <summary>
    /// The MAIN switch the Camera View Method
    /// </summary>
    /// <param name="setting">The Cam Setting</param>
    /// <param name="viewIndex">The CamView Index</param>
    private void SwitchCamera( CameraSetting setting, uint viewIndex )
    {
      _camSetting = setting;
      _camIndex = viewIndex;
      _camUpdate = c_triggerUpdate;
      SwitchCamera_low( setting, viewIndex );
    }

    // set the Index buttons 1..N enabled and the rest disabled
    private void SetIndexEnabled( uint maxIndex )
    {
      if (maxIndex == _maxIndexEnabled) return; // already there

      for (int i = 0; i < _btIndex.Count; i++) {
        _btIndex[i].Cursor = (i < maxIndex)? Cursors.Hand: Cursors.Default;
        _btIndex[i].ForeColor = (i < maxIndex) ? c_vActiveText : c_vPassiveText;
      }
      _maxIndexEnabled = maxIndex;
    }

  }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using FSimClientIF;
using SC = SimConnectClient;

namespace FCamControl
{
  /// <summary>
  /// Camera Control Form
  /// </summary>
  public partial class frmCamera : Form
  {
    private Dictionary<CameraSetting, ModeButton> _btMode = new Dictionary<CameraSetting, ModeButton>( );
    private List<Button> _btIndex = new List<Button>( ); // Cam Index
    private List<Button> _btSlot = new List<Button>( ); // Starred Slots
    private List<Button> _btSlotFolder = new List<Button>( ); // Slot Folders

    private string _observerName = "CAM_FORM";
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

    private ToolTip _tooltip = new ToolTip( );

    private const int c_triggerUpdate = 3; // N tries to update values after the Sim has changed them, sometimes it needs persistence...
    private int _droneValueUpdate = 0; // trigger delayed update to Sim
    private int _droneMovement = 4;  // initial values as the sim has them
    private int _droneRotation = 50; // initial values as the sim has them


    private int _camUpdate = 0; // trigger delayed update to Sim
    private CameraSetting _camSetting = CameraSetting.NONE;
    private uint _camIndex = 0;

    private UInt64 _smartTargetHash = 0; // triggers update of the Smart Target List if changed
    private int _prevSmartTarget = 99;

    private uint _maxIndexEnabled = 0; // track the enabled Index Buttons and change only when needed to avoid flicker

    private bool _slotSaving = false;

    // track the last known live location in order to save the proper one
    private Point _lastLiveLocation;

    /// <summary>
    /// Set true to run in standalone mode
    /// </summary>
    public bool Standalone { get; private set; } = false;

    #region AppSettingUpdate

    // Needed only once to update the AppSettings concept
    /// <summary>
    /// AppSettings Update from .Net to SettingsLib
    /// </summary>
    /// <param name="camSettings">The old camsettings</param>
    public void UpdateSettings( string[] camSettings )
    {
      int idx = 0;
      if (camSettings.Length > idx) if (AppSettings.Instance.CameraSlotFolder0 == "") AppSettings.Instance.CameraSlotFolder0 = camSettings[idx]; idx++;
      if (camSettings.Length > idx) if (AppSettings.Instance.CameraSlotFolder1 == "") AppSettings.Instance.CameraSlotFolder1 = camSettings[idx]; idx++;
      if (camSettings.Length > idx) if (AppSettings.Instance.CameraSlotFolder2 == "") AppSettings.Instance.CameraSlotFolder2 = camSettings[idx]; idx++;
      if (camSettings.Length > idx) if (AppSettings.Instance.CameraSlotFolder3 == "") AppSettings.Instance.CameraSlotFolder3 = camSettings[idx]; idx++;
      if (camSettings.Length > idx) if (AppSettings.Instance.CameraSlotFolder4 == "") AppSettings.Instance.CameraSlotFolder4 = camSettings[idx]; idx++;
      if (camSettings.Length > idx) if (AppSettings.Instance.CameraSlotFolder5 == "") AppSettings.Instance.CameraSlotFolder5 = camSettings[idx]; idx++;
      AppSettings.Instance.Save( );
    }

    #endregion

    /// <summary>
    /// Checks if a Point is visible on any screen
    /// </summary>
    /// <param name="point">The Location to check</param>
    /// <returns>True if visible</returns>
    private static bool IsOnScreen( Point point )
    {
      Screen[] screens = Screen.AllScreens;
      foreach (Screen screen in screens) {
        if (screen.WorkingArea.Contains( point )) {
          return true;
        }
      }
      return false;
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
        list.Add( string.IsNullOrEmpty( s ) ? "---" : $"{cam.TargetTypeClass( cam.SmartCamTarget_type( i ) )}-{s}" );
      }
      // if changed - load the list into the ListBox
      var tgtHash = CalculateHash( string.Concat( list ) );
      if (tgtHash != _smartTargetHash) {
        lbxSmartTargets.Items.Clear( );
        foreach (var s in list) lbxSmartTargets.Items.Add( s );
        _smartTargetHash = tgtHash;
      }
    }


    #region Form 

    // FORM
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="instance">An instance name (use "" as default)</param>
    /// <param name="standalone">Standalone flag (defaults to false)</param>
    public frmCamera( string instance, bool standalone = false )
    {
      // the first thing to do
      Standalone = standalone;
      AppSettings.InitInstance( Folders.SettingsFile, instance );
      // ---------------

      InitializeComponent( );

      this.ShowInTaskbar = true;
      this.MinimizeBox = true;

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

      // use another WindowFrame
      if (Standalone) {
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MinimizeBox = true;
        this.MaximizeBox = false;
        this.ControlBox = true;

        SC.SimConnectClient.Instance.DataArrived += Instance_DataArrived;
      }

      _tooltip.SetToolTip( btPilotView, "Cockpit View with previous selection" );
      _tooltip.SetToolTip( btIndexPilot, "Cockpit - Pilot View" );
      _tooltip.SetToolTip( btIndexClose, "Cockpit - Closer Instrument View" );
      _tooltip.SetToolTip( btIndexLand, "Cockpit - Landing View" );
      _tooltip.SetToolTip( btIndexCoPilot, "Cockpit - CoPilot View" );

      _tooltip.SetToolTip( btInstrumentView, "Instrument Views - use Index buttons" );
      _tooltip.SetToolTip( btShowcase, "Showcase Views - use Index buttons" );
      _tooltip.SetToolTip( btCustomView, "Custom Views - cannot yet use Index buttons" );

      _tooltip.SetToolTip( btCockpitQuick, "Cockpit Quick Views - Index 1..8" );
      _tooltip.SetToolTip( btExternalQuick, "External Quick Views - Index 1..8" );

      _tooltip.SetToolTip( btExternal, "External Free View - use mouse pan" );
      _tooltip.SetToolTip( btDrone, "Drone View - use drone controls" );
      _tooltip.SetToolTip( cbxDroneFollow, "Drone follows the target" );
      _tooltip.SetToolTip( cbxDroneLock, "Drone locks to the target" );
      _tooltip.SetToolTip( tbMove, "Drone movement speed" );
      _tooltip.SetToolTip( tbRotate, "Drone rotation speed" );

      _tooltip.SetToolTip( btResetView, "Reset the view" );
      _tooltip.SetToolTip( btSaveToSlot, "Save the current view to a slot" );

      _tooltip.SetToolTip( lbxSmartTargets, "Smart targets - click to view, click again to cancel" );

      Location = AppSettings.Instance.CameraLocation;
    }


    // form is loaded to get visible
    private void frmCamera_Load( object sender, EventArgs e )
    {
      // Init GUI
      this.Location = AppSettings.Instance.CameraLocation;
      if (!IsOnScreen( Location )) {
        Location = new Point( 20, 20 );
      }
      _lastLiveLocation = Location;

      // standalone connection status line
      lblSimConnected.BackColor = Color.Transparent;

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

      // standalone handling
      if (Standalone) {
        // File Access Check
        if (DbgLib.Dbg.Instance.AccessCheck( Folders.UserFilePath ) != DbgLib.AccessCheckResult.Success) {
          string msg = $"MyDocuments Folder Access Check Failed:\n{DbgLib.Dbg.Instance.AccessCheckResult}\n\n{DbgLib.Dbg.Instance.AccessCheckMessage}";
          MessageBox.Show( msg, "Access Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Error );
        }
      }

      // Pacer interval 
      timer1.Interval = 200;
    }

    // form got attention
    private void frmCamera_Activated( object sender, EventArgs e )
    {
      this.TopMost = true;
      timer1.Enabled = true;
      // register DataUpdates if in shared mode and if not yet done 
      if (!Standalone && SC.SimConnectClient.Instance.IsConnected && (_observerID < 0)) {
        _observerID = SC.SimConnectClient.Instance.CameraModule.AddObserver( _observerName, OnDataArrival );
      }
    }

    // track last known location while visible
    private void frmCamera_LocationChanged( object sender, EventArgs e )
    {
      if (this.Visible)
        _lastLiveLocation = this.Location;
    }

    // about to close the form
    private void frmCamera_FormClosing( object sender, FormClosingEventArgs e )
    {
      this.TopMost = false;
      timer1.Enabled = false;

      // UnRegister DataUpdates
      if (SC.SimConnectClient.Instance.IsConnected && (_observerID >= 0))
        SC.SimConnectClient.Instance.CameraModule.RemoveObserver( _observerID );
      _observerID = -1;

      // save last known good form location
      if (this.Visible && (this.WindowState == FormWindowState.Normal)) {
        AppSettings.Instance.CameraLocation = this.Location;
      }
      else {
        AppSettings.Instance.CameraLocation = _lastLiveLocation;
      }
      AppSettings.Instance.Save( );

      if (Standalone) {
        // don't cancel if standalone (else how to close it..)
        this.WindowState = FormWindowState.Minimized;
      }
      else {
        if (e.CloseReason == CloseReason.UserClosing) {
          // we don't close if the User clicks the X Box, only Hide; else it will not maintain the content throughout
          e.Cancel = true;
          this.Hide( );
        }
      }
    }

    #endregion

    /// <summary>
    /// Timer Event
    /// </summary>
    private void timer1_Tick( object sender, EventArgs e )
    {
      // Call SimConnect if needed and Standalone
      if (Standalone && --_simConnectTrigger <= 0) {
        SimConnectPacer( );
      }

      // prevent concurrency issues with control settings
      if (_updating) return;
      // not when not connected
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      // more murks.. 
      // sometimes a delayed SimConnect Call is needed to set values that the Sim changes within one Trigger cycle

      // retries as the Sim sometimes just ignores the first command when switching cams 
      // or switches but goes to it's own default state (which is not ours...)
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
    /// Handle Data Arrival from Sim
    /// </summary>
    /// <param name="refName">Data Reference Name that called the update</param>
    private void OnDataArrival( string refName )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (!this.Visible) return;  // no need to update while not visible

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
      // rot and move have their own Sim behavior - will reset at times
      // trying to preserve the user settings here
      // there is User Slider input and SimGui input which is either user triggered or by the Sim when switching views
      // _droneXY is restored when switching back to DroneView _droneXY is set via Slider or here
      // Rot Mov == 0 is problematic as nothing moves and is likely overlooked
      tbMove.Value = (int)cam.DroneCam_movespeed;
      tbRotate.Value = (int)cam.DroneCam_rotspeed;
      if (_droneMovement < 2) _droneMovement = (int)cam.DroneCam_movespeed; // init or don't stay at 0
      if (_droneRotation < 2) _droneRotation = (int)cam.DroneCam_rotspeed; // init or don't stay at 0
      if (cam.CameraSetting == CameraSetting.ShCase_Drone) {
        lblDroneMove.ForeColor = (_droneMovement < 2) ? Color.Orange : pnlDroneSlider.ForeColor;
        lblDroneRot.ForeColor = (_droneRotation < 2) ? Color.Orange : pnlDroneSlider.ForeColor;
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


    #region GUI Event Handlers

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
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

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
        if (selected != _prevSmartTarget) {
          if (!SC.SimConnectClient.Instance.CameraModule.SmartCam_active)
            SC.SimConnectClient.Instance.CameraModule.SmartCam_active = true;

          SC.SimConnectClient.Instance.CameraModule.SmartCamTarget_selected = (uint)selected;
          _prevSmartTarget = selected;
        }
        else {
          SC.SimConnectClient.Instance.CameraModule.SmartCam_active = false;
          _prevSmartTarget = 99;
        }
      }
    }

    private void cbxDroneLock_CheckedChanged( object sender, EventArgs e )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (_updating) return;

      SC.SimConnectClient.Instance.CameraModule.DroneCam_locked = cbxDroneLock.Checked;
    }

    private void cbxDroneFollow_CheckedChanged( object sender, EventArgs e )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (_updating) return;

      SC.SimConnectClient.Instance.CameraModule.DroneCam_follow = cbxDroneFollow.Checked;
    }

    private void tbMove_ValueChanged( object sender, EventArgs e )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (_updating) return;

      _droneMovement = tbMove.Value;
      SC.SimConnectClient.Instance.CameraModule.DroneCam_movespeed = _droneMovement;
    }

    private void tbRotate_ValueChanged( object sender, EventArgs e )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (_updating) return;

      _droneRotation = tbRotate.Value;
      SC.SimConnectClient.Instance.CameraModule.DroneCam_rotspeed = _droneRotation;
    }

    #endregion

    #region Cam Switcher

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
        _btIndex[i].Cursor = (i < maxIndex) ? Cursors.Hand : Cursors.Default;
        _btIndex[i].ForeColor = (i < maxIndex) ? c_vActiveText : c_vPassiveText;
      }
      _maxIndexEnabled = maxIndex;
    }

    #endregion


    #region SimConnectClient chores

    // Monitor the Sim Event Handler after Connection
    private bool m_awaitingEvent = true; // cleared in the Sim Event Handler
    private int m_scGracePeriod = -1;    // grace period count down
    private int _simConnectTrigger = 0; //  count down to call the SimConnect Pacer

    /// <summary>
    /// fired from Sim for new Data
    /// Callback from SimConnect client signalling data arrival
    ///  Appart from subscriptions this is calles on a regular pace 
    /// </summary>
    private void Instance_DataArrived( object sender, FSimClientIF.ClientDataArrivedEventArgs e )
    {
      m_awaitingEvent = false; // confirm we've got data events
    }

    /// <summary>
    /// Toggle the connection
    /// if not connected: Try to connect and setup facilities
    /// if connected:     Disconnect facilities and shut 
    /// </summary>
    private void SimConnect( )
    {
      // only needed in a standalone environment
      if (!Standalone) return;

      //LOG.Log( $"SimConnect: Start" );
      lblSimConnected.BackColor = Color.Transparent;
      if (SC.SimConnectClient.Instance.IsConnected) {
        // Disconnect from Input and SimConnect
        //        SetupInGameHook( false );

        // Unregister DataUpdates if not done 
        if (_observerID >= 0) {
          SC.SimConnectClient.Instance.CameraModule.RemoveObserver( _observerID );
          _observerID = -1;
        }
        SC.SimConnectClient.Instance.Disconnect( );
        lblSimConnected.BackColor = Color.DarkRed;
        //        LOG.Log( $"SimConnect: Disconnected now" );
      }

      else {
        // setup the event monitor before connecting (will be handled in the Timer Event)
        m_awaitingEvent = true;
        m_scGracePeriod = 3; // about 3*5 secs to get an event

        // try to connect
        if (SC.SimConnectClient.Instance.Connect( )) {

          //        HUD.DispItem( LItem.MSFS ).Label.ForeColor = Color.LimeGreen;
          lblSimConnected.BackColor = Color.MediumPurple;
          // enable game hooks if newly connected and desired
          //          SetupInGameHook( AppSettings.Instance.InGameHook );
          // Set Engines 
          //          LOG.Log( $"SimConnect: Connected now" );
        }
        else {
          // connect failed - will be retried through the pacer
          //          HUD.DispItem( LItem.MSFS ).Label.BackColor = Color.Red;
          lblSimConnected.BackColor = Color.DarkRed;
          //          LOG.Log( $"SimConnect: Could not connect" );
        }
      }

    }

    /// <summary>
    /// SimConnect chores on a timer, mostly reconnecting and monitoring the connection status
    /// Intender to be called about every 5 seconds
    /// </summary>
    private void SimConnectPacer( )
    {
      if (SC.SimConnectClient.Instance.IsConnected) {
        // handle the situation where Sim is connected but could not hookup to events
        // Happens when HudBar is running when the Sim is starting only.
        // Sometimes the Connection is made but was not hooking up to the event handling
        // Disconnect and try to reconnect 
        if (m_awaitingEvent || SC.SimConnectClient.Instance.CameraModule.Camera_state <= 0) {
          // No events seen so far
          // init the SimClient by pulling one item, so it registers the module, else the callback is not initiated
          _ = SC.SimConnectClient.Instance.CameraModule.Camera_state;

          if (m_scGracePeriod <= 0) {
            // grace period is expired !
            //            LOG.Log( "SimConnectPacer: Did not receive an Event for 5sec - Restarting Connection" );
            SimConnect( ); // Disconnect if we don't receive Events even the Sim is connected
          }
          m_scGracePeriod--;
        }
        else {
          lblSimConnected.BackColor = Color.DarkGreen;
          // register DataUpdates if not done 
          if (SC.SimConnectClient.Instance.IsConnected && _observerID < 0) {
            _observerID = SC.SimConnectClient.Instance.CameraModule.AddObserver( _observerName, OnDataArrival );
          }
        }
      }
      else {
        // If not connected try again
        SimConnect( );
      }

      // reset calling interval
      _simConnectTrigger = 5000 / timer1.Interval;
    }

    #endregion
  }
}

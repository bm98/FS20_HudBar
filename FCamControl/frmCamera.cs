using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Numerics;
using dNetBm98.Win;
using bm98_hbFolders;
using FSimClientIF;
using FSimClientIF.Modules;
using SC = SimConnectClient;
using static FSimClientIF.Sim;
using SimConnectClientAdapter;
using FS20_CamControl;
using System.Threading;

namespace FCamControl
{
  /// <summary>
  /// Camera Control Form
  /// </summary>
  public partial class frmCamera : Form
  {
    // this forms size
    private Size c_FormSize = new Size( 457, 684 );
    private Point c_pnlBaseLocation = new Point( 3, 340 );
    private Point c_pnlDroneLocation = new Point( 3, 191 );
    private Point c_pnl6DofLocation = new Point( 3, 191 );

    // SimConnect Client Adapter (used only to establish the connection and handle the Online color label)
    private SCClient SCAdapter;

    // attach the property module - this does not depend on the connection established or not
    private readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;

    // handle the GUI elements with indexed access using the items below
    private Dictionary<CameraSetting, ModeButton> _btMode = new Dictionary<CameraSetting, ModeButton>( );
    private List<Button> _btIndex = new List<Button>( ); // Cam Index
    private List<Button> _btSlot = new List<Button>( ); // Starred Slots
    private List<Button> _btSlotFolder = new List<Button>( ); // Slot Folders

    // SimVar Observer items
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
    private float _droneMovement = 4;  // initial values as the sim has them
    private float _droneRotation = 50; // initial values as the sim has them
    private float _droneZoom = 50; // initial values as the sim has them

    // requested cam settings
    private int _camUpdateRequested = 0; // trigger delayed update to Sim
    private CameraSetting _camSettingRequested = CameraSetting.NONE;
    private int _camIndexRequested = 0;

    // Custom Cams
    private CustomCamController _customCamController = null;
    private MSFS_KeyCat _msfs_Keys = null;

    // 6DOF items
    private static readonly Vector3 c_cam6dDefaultPos = new Vector3( ) { X = 20f, Y = 20f, Z = 20f };
    private Vector3 _cam6dPositionRequested = c_cam6dDefaultPos;
    private Vector3 _cam6dGimbalRequested = new Vector3( );

    // Smart target items
    private UInt64 _smartTargetHash = 0; // triggers update of the Smart Target List if changed
    private int _prevSmartTarget = 99;

    // track the enabled Index Buttons and change only when needed to avoid flicker
    private int _maxIndexEnabled = 0;

    // flag when Slot Saving is active
    private bool _slotSaving = false;

    // track the last known live location in order to save the proper one
    private Point _lastLiveLocation;

    // invoker for ready signal
    private WinFormInvoker _flyByInvoker;

    /// <summary>
    /// Set true  when run in standalone mode
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


    // true when the cam CANNOT be used
    private bool Inop( )
    {
      // use only if in flight or pause
      return
        !(
        (SC.SimConnectClient.Instance.ClientState == MSFS_State.InFlight)
      || (SC.SimConnectClient.Instance.ClientState == MSFS_State.ActivePause)
      );
    }

    // Updates the SmartCam Target list if needed (something has changed)
    private void PopulateSmartTargets( )
    {
      // Build the list
      var list = new List<string>( );
      for (int i = 0; i < 10; i++) {
        // set the Query for the N.th item
        SV.Set( SItem.iS_Cam_Smart_targetQuery_index, i );
        var ttype = SV.Get<CameraTargetType>( SItem.cttG_Cam_Smart_targettype_of );
        var s = SV.Get<string>( SItem.sG_Cam_Smart_targetname_of ); // can be null
        // new query value for the class
        SV.Set( SItem.iS_Cam_Smart_targetQuery_index, (int)ttype );
        var ttClass = SV.Get<string>( SItem.sG_Cam_Smart_targettypeClass_of );
        // comlpete the entry
        list.Add( string.IsNullOrEmpty( s ) ? "---" : $"{ttClass}-{s}" );
      }
      // if changed - load the list into the ListBox
      var tgtHash = dNetBm98.Utilities.KnuthHash( string.Concat( list ) );
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

      // Init the Folders Utility with our AppSettings File
      Folders.InitStorage( "FCamAppSettings.json" );

      AppSettings.InitInstance( Folders.SettingsFile, instance );
      // ---------------

      InitializeComponent( );

      // set this form size from Dev Size
      this.Size = c_FormSize;

      this.ShowInTaskbar = true;
      this.MinimizeBox = true;

      pnlBase.Visible = true;
      pnlBase.Location = c_pnlBaseLocation;

      pnl6Dof.Visible = false;
      pnl6Dof.Location = c_pnl6DofLocation;

      pnlDrone.Visible = false;
      pnlDrone.Location = c_pnlDroneLocation;

      // Keys and controllers
      _msfs_Keys = new MSFS_KeyCat( );
      _msfs_Keys.FromConfigString( AppSettings.Instance.MSFS_KeyConfiguration ); // will load defaults and then settings
      _customCamController = new CustomCamController( _msfs_Keys );

      _flyByInvoker = new WinFormInvoker( btFlyByFire );


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
        { CameraSetting.Cam_6DOF,new ModeButton( CameraSetting.Cam_6DOF,bt6DOF, SwitchCamera) },
      };
      _btIndex = new List<Button> { btIndex01, btIndex02, btIndex03, btIndex04, btIndex05, btIndex06, btIndex07, btIndex08,
                                    btIndex09, btIndex10, btIndex11, btIndex12, btIndex13, btIndex14, btIndex15, btIndex16, btIndex17, btIndex18,
                                    btIndex19, btIndex20, btIndex21, btIndex22, btIndex23, btIndex24, btIndex25, btIndex26, btIndex27, btIndex28  };
      _btSlot = new List<Button> { btSlot01, btSlot02, btSlot03, btSlot04, btSlot05, btSlot06, btSlot07, btSlot08,
                                    btSlot09, btSlot10 };
      _btSlotFolder = new List<Button> { btSlotFolderA, btSlotFolderB, btSlotFolderC, btSlotFolderD, btSlotFolderE, btSlotFolderF, btSlotFolderG, btSlotFolderH };

      // Starred Slots init
      SlotCat.InitSlotFolders( _btSlot, SwitchCamera );
      // get save confirmation callback
      foreach (var slotFolder in SlotCat.SlotFolders) {
        slotFolder.SlotSaved += _slots_SlotSaved;
      }

      // attach 6DOF panel
      uc6Entry.ValueChanged += Uc6Entry_ValueChanged;

      // Handle the Standalone version
      if (Standalone) {
        // use another WindowFrame
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MinimizeBox = true;
        this.MaximizeBox = false;
        this.ControlBox = true;
        // Connection to SimConnect Client
        SCAdapter = new SCClient( );
        SCAdapter.Connected += SCAdapter_Connected;
        SCAdapter.Establishing += SCAdapter_Establishing;
        SCAdapter.Disconnected += SCAdapter_Disconnected;
        SCAdapter.SC_Label = lblSimConnected;
      }

      _tooltip.SetToolTip( btPilotView, "Cockpit View with previous selection" );
      _tooltip.SetToolTip( btIndexPilot, "Cockpit - Pilot View" );
      _tooltip.SetToolTip( btIndexClose, "Cockpit - Closer Instrument View" );
      _tooltip.SetToolTip( btIndexLand, "Cockpit - Landing View" );
      _tooltip.SetToolTip( btIndexCoPilot, "Cockpit - CoPilot View" );

      _tooltip.SetToolTip( btInstrumentView, "Instrument Views - use Index buttons" );
      _tooltip.SetToolTip( btShowcase, "Showcase Views - use Index buttons" );
      _tooltip.SetToolTip( btCustomView, "Custom Views - use Index buttons 1..10" );

      _tooltip.SetToolTip( btCockpitQuick, "Cockpit Quick Views - Index 1..8" );
      _tooltip.SetToolTip( btExternalQuick, "External Quick Views - Index 1..8" );

      _tooltip.SetToolTip( btExternal, "External Free View - use mouse pan" );
      _tooltip.SetToolTip( btDrone, "Drone View - use drone controls" );
      _tooltip.SetToolTip( cbxDroneFollow, "Drone follows the target" );
      _tooltip.SetToolTip( cbxDroneLock, "Drone locks to the target" );
      _tooltip.SetToolTip( tbMove, "Drone movement speed" );
      _tooltip.SetToolTip( tbRotate, "Drone rotation speed" );
      _tooltip.SetToolTip( tbZoom, "Drone zoom (field of view)" );
      _tooltip.SetToolTip( btDroneRecover, "Drone - Recover default position" );

      _tooltip.SetToolTip( btFlyByDroneReset, "FlyBy - Reset position" );
      _tooltip.SetToolTip( btFlyByFollow, "FlyBy - Follow aircraft" );
      _tooltip.SetToolTip( btFlyByPrep, "FlyBy - Go into cam position" );
      _tooltip.SetToolTip( btFlyByFire, "FlyBy - Release and fly by" );
      _tooltip.SetToolTip( cbAbove, "FlyBy - Set position above" );
      _tooltip.SetToolTip( cbBelow, "FlyBy - Set position below" );
      _tooltip.SetToolTip( cbLeftSide, "FlyBy - Set position left of aircraft, else right" );

      _tooltip.SetToolTip( bt6DOF, "6DOF View - numeric controls" );

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
      if (!dNetBm98.Utilities.IsOnScreen( Location )) {
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
      SlotCat.SlotFolders[6].AppSettingString = AppSettings.Instance.CameraSlotFolder6;
      SlotCat.SlotFolders[7].AppSettingString = AppSettings.Instance.CameraSlotFolder7;
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
        // activate connection
        SCAdapter.Connect( );
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
        _observerID = SV.AddObserver( _observerName, 2, OnDataArrival );
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
        SV.RemoveObserver( _observerID );
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
        SCAdapter.Disconnect( );
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
      // inhibit when not in flight
      pnlMain.Enabled = !Inop( );
      lblInop.Visible = Inop( );

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
        if (SV.Get<CameraSetting>( SItem.csetGS_Cam_Actual_setting ) == CameraSetting.ShCase_Drone) {
          SV.Set( SItem.fGS_Cam_Drone_movespeed, _droneMovement );
          SV.Set( SItem.fGS_Cam_Drone_rotspeed, _droneRotation );
          SV.Set( SItem.fGS_Cam_Drone_zoomlevel, _droneZoom );
        }
      }
      // sometimes the cam needs to be enforced - so try again if needed
      if (_camUpdateRequested > 0) {
        _camUpdateRequested--;
        SwitchCamera_low( _camSettingRequested, _camIndexRequested, _cam6dPositionRequested, _cam6dGimbalRequested, false );
      }
    }

    // Sometimes the Drone cam does not reset to behind the acft
    // This should fix it...
    private void RecoverDroneResetPosition( )
    {
      // Switch to Ext Quick View if not there 
      if (_actSetting != CameraSetting.Ext_Default) {
        SwitchCamera( CameraSetting.Ext_Default, 0 );
      }
      // and Reset this Cam
      btResetView_Click( this, EventArgs.Empty );
      // and to previous Setting if not there
      if (_prevSetting != CameraSetting.Ext_Default) {
        SwitchCamera( _prevSetting, _prevIndex );
      }
    }


    // set all slots to passive
    private void RecoverButtonSlots( )
    {
      foreach (var bt in _btIndex) {
        bt.BackColor = c_vPassive;
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

      // ViewMode Buttons
      CameraSetting curSetting = SV.Get<CameraSetting>( SItem.csetGS_Cam_Actual_setting );
      _btMode[_prevSetting].Button.BackColor = c_vPassive;
      if (curSetting != CameraSetting.NONE) {
        _prevSetting = _actSetting;
        _actSetting = curSetting;
        _btMode[_actSetting].Button.BackColor = c_vActive;
      }
      // _actSetting is valid now

      // Index Buttons (_prevIndex SHALL never be out of range...)
      int curViewIndex = SV.Get<int>( SItem.iGS_Cam_Viewindex );
      if ((_prevIndex < 0) || (_prevIndex >= _btIndex.Count)) {
        RecoverButtonSlots( );
      }
      else {
        _btIndex[_prevIndex].BackColor = c_vPassive;
      }

      if (curViewIndex < _btIndex.Count) {
        _prevIndex = _actIndex;
        _actIndex = curViewIndex;

        if (_actSetting == CameraSetting.Cockpit_Custom) {
          // custom views don't set the view Index
          _actIndex = _customCamController.LastSlotIndex; // valid while setting it from here...
        }
        else {
          _customCamController.ClearLastSlotIndex( ); // must clear when no longer in custom mode
        }
        // finally illuminate the selected one
        _btIndex[_actIndex].BackColor = c_vActive;
      }
      // _actIndex is valid now

      // get the max index for the views available
      var vt = SV.Get<CameraViewType>( SItem.cvtG_Cam_Viewtype );
      switch (vt) {
        case CameraViewType.Unknown_default: SetIndexEnabled( SV.Get<int>( SItem.iG_Cam_Viewindex_max_default ) ); break;
        case CameraViewType.PilotView: SetIndexEnabled( SV.Get<int>( SItem.iG_Cam_Viewindex_max_pilot ) ); break;
        case CameraViewType.InstrumentView: SetIndexEnabled( SV.Get<int>( SItem.iG_Cam_Viewindex_max_instrument ) ); break;
        case CameraViewType.Quickview: SetIndexEnabled( SV.Get<int>( SItem.iG_Cam_Viewindex_max_quick ) ); break;
        case CameraViewType.Quickview_External: SetIndexEnabled( SV.Get<int>( SItem.iG_Cam_Viewindex_max_external ) ); break;
        case CameraViewType.OtherView: SetIndexEnabled( SV.Get<int>( SItem.iG_Cam_Viewindex_max_other ) ); break;
        default: SetIndexEnabled( SV.Get<int>( SItem.iG_Cam_Viewindex_max_default ) ); break;
      }

      // Drone mode states
      cbxDroneLock.CheckState = SV.Get<bool>( SItem.bGS_Cam_Drone_locked ) ? CheckState.Checked : CheckState.Unchecked;
      cbxDroneFollow.CheckState = SV.Get<bool>( SItem.bGS_Cam_Drone_follow ) ? CheckState.Checked : CheckState.Unchecked;
      // rot and move have their own Sim behavior - will reset at times
      // trying to preserve the user settings here
      // there is User Slider input and SimGui input which is either user triggered or by the Sim when switching views
      // _droneXY is restored when switching back to DroneView _droneXY is set via Slider or here
      // Rot Mov == 0 is problematic as nothing moves and is likely overlooked

      // Drone sliders
      if (!_mouseDown) {
        // prevent changes while changing the sliders
        tbMove.Value = (int)SV.Get<float>( SItem.fGS_Cam_Drone_movespeed );
        tbRotate.Value = (int)SV.Get<float>( SItem.fGS_Cam_Drone_rotspeed );
        tbZoom.Value = (int)SV.Get<float>( SItem.fGS_Cam_Drone_zoomlevel );
        if (_droneMovement < 2) _droneMovement = tbMove.Value; // init or don't stay at 0
        if (_droneRotation < 2) _droneRotation = tbRotate.Value; // init or don't stay at 0
      }
      // illluminate 'no move' settings of the sliders
      if (_actSetting == CameraSetting.ShCase_Drone) {
        lblDroneMove.ForeColor = (_droneMovement < 2) ? Color.Orange : pnlDroneSlider.ForeColor;
        lblDroneRot.ForeColor = (_droneRotation < 2) ? Color.Orange : pnlDroneSlider.ForeColor;
      }

      // handle Custom Camera setting
      if (_actSetting == CameraSetting.Cockpit_Custom) {
        SetIndexEnabled( 10 );
      }

      // SmartTarget, illuminate the selected one
      if (SV.Get<bool>( SItem.bGS_Cam_Smart_active )) {
        txSmartTargetType.Text = $"{SV.Get<CameraTargetType>( SItem.cttG_Cam_Smart_targettype )}";
        txSmartTargetName.Text = SV.Get<string>( SItem.sG_Cam_Smart_targetname_selected );
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
      PopulateSmartTargets( ); // new targets to show

      // base Panel
      pnlBase.Visible = !((_actSetting == CameraSetting.Cam_6DOF) || (_actSetting == CameraSetting.ShCase_Drone));
      // 6D cam Panel
      pnl6Dof.Visible = (_actSetting == CameraSetting.Cam_6DOF);
      // Drone/FlyBy Panel
      pnlDrone.Visible = (_actSetting == CameraSetting.ShCase_Drone);

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
      AppSettings.Instance.CameraSlotFolder6 = SlotCat.SlotFolders[6].AppSettingString;
      AppSettings.Instance.CameraSlotFolder7 = SlotCat.SlotFolders[7].AppSettingString;
      AppSettings.Instance.Save( );

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
        SlotCat.CurrentSlotFolder.ExpectSlotSave( uc6Entry.Position, uc6Entry.Gimbal );
      }
    }


    // Handle Reset View Click
    private void btResetView_Click( object sender, EventArgs e )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set( SItem.S_Cam_Actual_reset, true );
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
        if (_actSetting == CameraSetting.Cockpit_Custom) {
          _customCamController.SendSlot( index ); // slots are 0..9, aka CustomSLot 1..9,0
        }
        else {
          // Cam Index Selectors
          SV.Set( SItem.iGS_Cam_Viewindex, index );
        }
        _btMode[_actSetting].ViewIndex = index; // save the last selected per setting for later use

      }
      else {
        // Pilot View Position Selectors
        int idx = 0;
        if (bt.Name == "btIndexPilot") {
          idx = (int)PilotCamPosition.Pilot;
        }
        else if (bt.Name == "btIndexClose") {
          idx = (int)PilotCamPosition.Close;
        }
        else if (bt.Name == "btIndexLand") {
          idx = (int)PilotCamPosition.Landing;
        }
        else if (bt.Name == "btIndexCoPilot") {
          idx = (int)PilotCamPosition.Copilot;
        }
        // switch
        SwitchCamera( CameraSetting.Cockpit_Pilot, idx, new Vector3( ), new Vector3( ) ); // not DOF
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
      if (SV.Get<CameraSetting>( SItem.csetGS_Cam_Actual_setting ) == CameraSetting.ShCase_Drone) return;
      if (SV.Get<CameraSetting>( SItem.csetGS_Cam_Actual_setting ) == CameraSetting.ShCase_Fixed) return;
      if (SV.Get<CameraSetting>( SItem.csetGS_Cam_Actual_setting ) == CameraSetting.Cam_6DOF) return;

      int selected = lbxSmartTargets.SelectedIndex;
      if ((selected >= 0) && (selected < 10)) {
        if (selected != _prevSmartTarget) {
          if (!SV.Get<bool>( SItem.bGS_Cam_Smart_active ))
            SV.Set( SItem.bGS_Cam_Smart_active, true );

          SV.Set( SItem.iGS_Cam_Smart_targetindex_selected, selected );
          _prevSmartTarget = selected;
        }
        else {
          DisableSmartTarget( );
        }
      }
    }

    // disables the smart targeting, True if it was needed
    private bool DisableSmartTarget( )
    {
      if (SV.Get<bool>( SItem.bGS_Cam_Smart_active )) {
        SV.Set( SItem.bGS_Cam_Smart_active, false );
        _prevSmartTarget = 99;
        return true;
      }
      return false;
    }

    // try recover the drone default position
    private void btDroneRecover_Click( object sender, EventArgs e )
    {
      // this is called from Drone view only (button is not avail elsewhere)
      RecoverDroneResetPosition( );
    }

    private void cbxDroneLock_CheckedChanged( object sender, EventArgs e )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (_updating) return;

      SV.Set( SItem.bGS_Cam_Drone_locked, cbxDroneLock.Checked );
    }

    private void cbxDroneFollow_CheckedChanged( object sender, EventArgs e )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (_updating) return;

      SV.Set( SItem.bGS_Cam_Drone_follow, cbxDroneFollow.Checked );
    }

    private void tbMove_ValueChanged( object sender, EventArgs e )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (_updating) return;

      _droneMovement = tbMove.Value;
      SV.Set( SItem.fGS_Cam_Drone_movespeed, _droneMovement );
    }

    private void tbRotate_ValueChanged( object sender, EventArgs e )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (_updating) return;

      _droneRotation = tbRotate.Value;
      SV.Set( SItem.fGS_Cam_Drone_rotspeed, _droneRotation );
    }

    private void tbZoom_ValueChanged( object sender, EventArgs e )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;
      if (_updating) return;

      _droneZoom = tbZoom.Value;
      SV.Set( SItem.fGS_Cam_Drone_zoomlevel, _droneZoom );
    }


    private bool _mouseDown = false;
    private void tbMove_MouseDown( object sender, MouseEventArgs e )
    {
      _mouseDown = true;
    }

    private void tbMove_MouseUp( object sender, MouseEventArgs e )
    {
      _mouseDown = false;
    }

    private void tbRotate_MouseDown( object sender, MouseEventArgs e )
    {
      _mouseDown = true;
    }

    private void tbRotate_MouseUp( object sender, MouseEventArgs e )
    {
      _mouseDown = false;
    }

    private void tbZoom_MouseDown( object sender, MouseEventArgs e )
    {
      _mouseDown = true;
    }

    private void tbZoom_MouseUp( object sender, MouseEventArgs e )
    {
      _mouseDown = false;
    }

    #endregion

    #region Cam Switcher

    /// <summary>
    /// The MAIN switch the Camera View Method
    /// </summary>
    /// <param name="setting">The Cam Setting</param>
    /// <param name="viewIndex">The CamView Index</param>
    /// <param name="position">6DOF Position</param>
    /// <param name="gimbal">6DOF Gimbal</param>
    /// <param name="initial">Set false when retrying, initial must be true</param>
    private void SwitchCamera_low( CameraSetting setting, int viewIndex, Vector3 position, Vector3 gimbal, bool initial = true )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      _btMode[setting].ViewIndex = viewIndex; // save preset default
      if (SV.Get<CameraSetting>( SItem.csetGS_Cam_Actual_setting ) != setting) {
        // change if not already there

        // NOTE: all patches and recoveries are only performed on the initial call, not retries

        // need to disable smart if it was active
        if (initial && DisableSmartTarget( )) {
          // nasty wait until the Sim has settled the disable
          // otherwise the Cam Reset position is messed up when leaving the SmartMode without settling
          Thread.Sleep( 250 );
        }

        if (initial && (setting == CameraSetting.ShCase_Drone)) {
          // to recover the Drone reset pos, Switch to Ext View if not there 
          if (_actSetting != CameraSetting.Ext_Default) {
            SV.Set( SItem.csetGS_Cam_Actual_setting, CameraSetting.Ext_Default );
          }
          // and Reset this Cam
          SV.Set( SItem.S_Cam_Actual_reset, true );
          // another nasty wait until the Sim has settled the reset
          Thread.Sleep( 250 );
        }

        // now switch to the new cam
        SV.Set( SItem.csetGS_Cam_Actual_setting, setting );
      }

      // Set the ViewIndex
      if (setting == CameraSetting.Cockpit_Custom) {
        // custom cam needs help via keyboard
        if (initial) {
          _customCamController.SendSlot( viewIndex );
        }
      }
      else {
        // others can use the API
        SV.Set( SItem.iGS_Cam_Viewindex, viewIndex );
      }

      // Set the 6DOF
      if (initial && (setting == CameraSetting.Cam_6DOF)) {
        Set6DOFcam( position, gimbal );
        uc6Entry.Position = position;
        uc6Entry.Gimbal = gimbal;
      }

      // restore Drone speeds
      _droneValueUpdate = c_triggerUpdate;

      // the GUI may change once we read the effective cam setting via DataArrival
    }

    /// <summary>
    /// The MAIN switch the Camera View Method to set 6DOF
    /// </summary>
    /// <param name="setting">The Cam Setting</param>
    /// <param name="viewIndex">The CamView Index</param>
    /// <param name="position">6DOF Position</param>
    /// <param name="gimbal">6DOF Gimbal</param>
    private void SwitchCamera( CameraSetting setting, int viewIndex, Vector3 position, Vector3 gimbal )
    {
      // set requested cam props
      _camSettingRequested = setting;
      _camIndexRequested = viewIndex;
      if (setting == CameraSetting.Cam_6DOF) {
        _cam6dPositionRequested = position;
        _cam6dGimbalRequested = gimbal;
      }
      _camUpdateRequested = (setting != CameraSetting.Cockpit_Custom) ? c_triggerUpdate : 0; // no retrigger for CustomCam
      // switch cam
      SwitchCamera_low( _camSettingRequested, _camIndexRequested, _cam6dPositionRequested, _cam6dGimbalRequested );
    }

    /// <summary>
    /// The MAIN switch the Camera View Method
    /// </summary>
    /// <param name="setting">The Cam Setting</param>
    /// <param name="viewIndex">The CamView Index</param>
    private void SwitchCamera( CameraSetting setting, int viewIndex )
    {
      // set requested cam props
      _camSettingRequested = setting;
      _camIndexRequested = viewIndex;
      _camUpdateRequested = (setting != CameraSetting.Cockpit_Custom) ? c_triggerUpdate : 0; // no retrigger for CustomCam
      // switch cam
      SwitchCamera_low( _camSettingRequested, _camIndexRequested, _cam6dPositionRequested, _cam6dGimbalRequested );
    }

    // set the Index buttons 1..N enabled and the rest disabled
    private void SetIndexEnabled( int maxIndex )
    {
      if (maxIndex == _maxIndexEnabled) return; // already there

      for (int i = 0; i < _btIndex.Count; i++) {
        _btIndex[i].Cursor = (i < maxIndex) ? Cursors.Hand : Cursors.Default;
        _btIndex[i].ForeColor = (i < maxIndex) ? c_vActiveText : c_vPassiveText;
      }
      _maxIndexEnabled = maxIndex;
    }

    #endregion

    #region 6DOF 

    // update the sim Cam
    private void Set6DOFcam( Vector3 pos, Vector3 gimbal )
    {
      if (_actSetting == CameraSetting.Cam_6DOF) {
        SV.Set( SItem.v3GS_Cam_6DOF_xyz, pos );
        SV.Set( SItem.v3GS_Cam_6DOF_pbh, gimbal );
        SV.Set( SItem.bS_Cam_6DOF_set, true );
      }
    }

    // data entry
    private void Uc6Entry_ValueChanged( object sender, EventArgs e )
    {
      Set6DOFcam( uc6Entry.Position, uc6Entry.Gimbal );
    }

    #endregion

    #region FlyBy  / TODO CLEANUP once it works a bit

    // controller to perform flyby's
    private FlyByController _flyByController;

    private void btFlyByPrep_Click( object sender, EventArgs e )
    {
      btFlyByFire.Visible = false;
      lblFlyByCountdown.Visible = true;
      lblFlyByCountdown.Text = "...";
      lblFlyByFailed.Visible = false;

      if (_flyByController == null) {
        _flyByController = new FlyByController( _msfs_Keys );
        _flyByController.FlyByReadyProgress += _flyBy_FlyByReady;
      }
      float dist = FlyByController.DistFromPercent( tbFlyByDist.Value );
      if (!_flyByController.Prepare( dist, cbLeftSide.Checked, cbAbove.Checked, cbBelow.Checked )) {
        // prepare failed
        lblFlyByFailed.Visible = true;
      }
    }

    // called from controller when ready to fire
    private void _flyBy_FlyByReady( object sender, FlyByControllerEventArgs e )
    {
      _flyByInvoker.HandleEvent( ( ) => {
        btFlyByFire.Visible = e.Ready;
        lblFlyByCountdown.Visible = !e.Ready;
        lblFlyByCountdown.Text = $"DON'T CLICK\nwait.. {e.Remaining_ms / 1000f:#0.0}";
      } );

    }

    private void btFlyByFire_Click( object sender, EventArgs e )
    {
      if (_flyByController == null) return;

      _flyByController.Fire( );
      btFlyByFire.Visible = false;
      lblFlyByCountdown.Visible = false;
    }

    private void btDroneReset_Click( object sender, EventArgs e )
    {
      // essentially only reset
      btResetView_Click( sender, e );
    }

    // set Follow mode
    private void btFollow_Click( object sender, EventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set<bool>( SItem.bGS_Cam_Drone_follow, true );
    }

    private void tbFlyByDist_ValueChanged( object sender, EventArgs e )
    {
    }

    private void tbFlyByDist_Scroll( object sender, EventArgs e )
    {
      // reset checked when used
      rbStage0.Checked = false;
      rbStage1.Checked = false;
      rbStage2.Checked = false;
      rbStage3.Checked = false;
    }

    private void tbFlyByDist_MouseDown( object sender, MouseEventArgs e )
    {
      _mouseDown = true;
    }

    private void tbFlyByDist_MouseUp( object sender, MouseEventArgs e )
    {
      _mouseDown = false;
    }

    private void cbAbove_CheckedChanged( object sender, EventArgs e )
    {
      if (cbAbove.Checked) {
        cbBelow.Checked = false;
      }
    }

    private void cbBelow_CheckedChanged( object sender, EventArgs e )
    {
      if (cbBelow.Checked) {
        cbAbove.Checked = false;
      }
    }

    private void rbStage0_CheckedChanged( object sender, EventArgs e )
    {
      if (rbStage0.Checked) {
        tbFlyByDist.Value = FlyByController.DistToPercent( FlyByController.DistOfStage_nm( 0 ) );
      }
    }

    private void rbStage1_CheckedChanged( object sender, EventArgs e )
    {
      if (rbStage1.Checked) {
        tbFlyByDist.Value = FlyByController.DistToPercent( FlyByController.DistOfStage_nm( 1 ) );
      }
    }

    private void rbStage2_CheckedChanged( object sender, EventArgs e )
    {
      if (rbStage2.Checked) {
        tbFlyByDist.Value = FlyByController.DistToPercent( FlyByController.DistOfStage_nm( 2 ) );
      }
    }

    private void rbStage3_CheckedChanged( object sender, EventArgs e )
    {
      if (rbStage3.Checked) {
        tbFlyByDist.Value = FlyByController.DistToPercent( FlyByController.DistOfStage_nm( 3 ) );
      }
    }

    #endregion


    #region SimConnectClient chores

    // establishing event
    private void SCAdapter_Establishing( object sender, EventArgs e )
    {
    }

    // connect event
    private void SCAdapter_Connected( object sender, EventArgs e )
    {
      // register DataUpdates if not done 
      if (SC.SimConnectClient.Instance.IsConnected && _observerID < 0) {
        _observerID = SV.AddObserver( _observerName, 2, OnDataArrival );
      }
    }

    // disconnect event
    private void SCAdapter_Disconnected( object sender, EventArgs e )
    {
      if (_observerID >= 0) {
        SV.RemoveObserver( _observerID );
        _observerID = -1;
      }
    }

    #endregion

    #region Key Configs

    private frmKeyConfig _keyConfig;

    private void btConfig_Click( object sender, EventArgs e )
    {
      _keyConfig = new frmKeyConfig( );
      // load current keys
      _keyConfig.LoadKeyDict( _msfs_Keys );

      if (_keyConfig.ShowDialog( this ) == DialogResult.OK) {
        // accept keys
        _msfs_Keys = new MSFS_KeyCat( _keyConfig.MSFS_Keys );
        // Reload controllers
        _customCamController?.LoadSlotsFromCatalog( _msfs_Keys );
        _flyByController?.ReloadKeyCatalog( _msfs_Keys );

        // Save Config
        AppSettings.Instance.MSFS_KeyConfiguration = _msfs_Keys.ToConfigString( );
        AppSettings.Instance.Save( );
      }

      _keyConfig.Dispose( );
    }

    private void lblSimConnected_DoubleClick( object sender, EventArgs e )
    {
      btConfig_Click( sender, e );
    }

    #endregion

  }
}

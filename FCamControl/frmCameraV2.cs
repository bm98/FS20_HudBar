using System;
using System.Drawing;
using System.Windows.Forms;

using bm98_hbFolders;

using FSimClientIF;

using SimConnectClientAdapter;

namespace FCamControl
{
  public partial class frmCameraV2 : Form
  {
    // this forms size
    private readonly Size c_FormSize = new Size( 666, 429 );
    private readonly Point c_pnlCamButtonsLocation = new Point( 3, 8 ); // Cam Button Panel
    private readonly Point c_pnlSlotsLocation = new Point( 3, 67 );  // Slots Panel
    private readonly Point c_pnlCtrlLocation = new Point( 3, 158 ); // controls are here

    // colors used from matching color scheme
    internal static readonly Color c_CamPanelBColor = Color.FromArgb( 54, 52, 50 ); // panel background
    internal static readonly Color c_CamBtUnselBColor = Color.FromArgb( 108, 105, 104 ); // unselected button Backcolor
    internal static readonly Color c_CamBtSelBColor = Color.FromArgb( 38, 197, 83 ); // selected button Backcolor

    internal static readonly Color c_SlotBackColor = Color.FromArgb( 62, 59, 67 ); // panel background
    internal static readonly Color c_SlotButtonBColor = Color.FromArgb( 39, 37, 36 ); // Slot Button Backcolor
    internal static readonly Color c_SlotButtonUnselFColor = Color.FromArgb( 23, 119, 80 ); // Slot Button unselected ForeColor
    internal static readonly Color c_SlotButtonSelFColor = Color.FromArgb( 34, 178, 120 ); // Slot Button selected ForeColor

    internal static readonly Color c_SlotFolderBColor = Color.FromArgb( 39, 37, 36 ); // button background
    internal static readonly Color c_SlotFolderUnselFColor = Color.SaddleBrown; // Slot Folder Button unselected ForeColor
    internal static readonly Color c_SlotFolderSelFColor = Color.Gold; // Slot Folder Button selected ForeColor

    internal static readonly Color c_CtrlBColor = Color.FromArgb( 39, 37, 36 ); // panel background
    internal static readonly Color c_CtrlUnselBColor = Color.FromArgb( 108, 105, 104 ); // Ctrl buttons unselected backcolor
    internal static readonly Color c_CtrlSelBColor = Color.FromArgb( 29, 148, 108 );  // Ctrl buttons selected backcolor

    internal static readonly Color c_CtrlUnselFColor = Color.Black; // Ctrl buttons unselected forecolor
    internal static readonly Color c_CtrlUnselFColorDim = Color.Gray; // Ctrl buttons unselected forecolor Dimmed
    internal static readonly Color c_CtrlSelFColor = Color.Black;  // Ctrl buttons selected forecolor

    private readonly ToolTip _tooltip = new ToolTip( );

    // SimConnect Client Adapter (used only to establish the connection and handle the Online color label)
    private readonly SCClient SCAdapter;

    // Panels
    private readonly UC_SlotsPanel _slotsPanel;
    private readonly UC_ViewsPanel _viewsPanel;
    private readonly UC_DronePanel _dronePanel;
    private readonly UC_6DOFPanel _6DOFPanel;

    // Camera Obj
    private readonly Camera _camera;
    // track current
    private CameraMode _currentCam = CameraMode.NONE;
    private int _currentCamSlot = -1;

    // track GUI update and don't mix writes up..
    private bool _updatingGUI = false;

    // MSFS Keymap
    private MSFS_KeyCat _msfs_Keys = null;
    // Key config Dialog
    private frmKeyConfig _keyConfig;

    // track the last known live location in order to save the proper one
    private Point _lastLiveLocation;

    // button handlers
    private ButtonHandler _btHCamera = null; // cam selectors

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
    /// Set true  when run in standalone mode
    /// </summary>
    public bool Standalone { get; private set; } = false;


    // true when the cam CANNOT be used
    private bool Inop( )
    {
#if DEBUG
      return false;
#else
      // use only if in flight or pause
      return
        !(
        (SC.SimConnectClient.Instance.ClientState == MSFS_State.InFlight)
      || (SC.SimConnectClient.Instance.ClientState == MSFS_State.ActivePause)
      );
#endif

    }

    private void timer1_Tick( object sender, EventArgs e )
    {
      // register DataUpdates if in shared mode and if not yet done 
      if (!Standalone) {
        if ((_camera != null) && _camera.ConnectSim( )) {
          // newly connected
        }
      }

      // Check Inop State only
      // inhibit when not in flight
      pnlCamButtons.Enabled = !Inop( );
      _slotsPanel.Enabled = !Inop( );
      _viewsPanel.Enabled = !Inop( );
      _dronePanel.Enabled = !Inop( );
      _6DOFPanel.Enabled = !Inop( );

      lblInop.Visible = Inop( );
    }

    #region Form 

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="instance">An instance name (use "" as default)</param>
    /// <param name="standalone">Standalone flag (defaults to false)</param>
    public frmCameraV2( string instance, bool standalone = false )
    {
      // the first thing to do
      Standalone = standalone;

      // Init the Folders Utility with our AppSettings File
      Folders.InitStorage( "FCamAppSettings.json" );

      AppSettings.InitInstance( Folders.SettingsFile, instance );
      // ---------------

      InitializeComponent( );

      // add disposed handler
      this.Disposed += FrmCameraV2_Disposed;

      // setup from design layout
      this.Size = c_FormSize;
      pnlCamButtons.Location = c_pnlCamButtonsLocation;

      // Keys and controllers
      _msfs_Keys = new MSFS_KeyCat( );
      _msfs_Keys.FromConfigString( AppSettings.Instance.MSFS_KeyConfiguration ); // will load defaults and then settings

      // load the camera
      _camera = new Camera( this, _msfs_Keys );
      _camera.CameraAPI.StateChanged +=
        ( object sender, State.StateTransitionEventArgs e ) => {
          this.Invoke( (MethodInvoker)delegate { CameraAPI_StateChanged( e ); } );
        };
      _camera.CameraAPI.DataArrived +=
        ( object sender, EventArgs e ) => {
          this.Invoke( (MethodInvoker)delegate { CameraAPI_DataArrived( ); } );
        };

      // Camera Buttons - act as RadioButtons
      _btHCamera = new ButtonHandler( true ) {
        BColor = c_CamBtUnselBColor,
        FColor = Color.Black,
        ActBColor = c_CamBtSelBColor,
        ActFColor = Color.Black
      };

      // Slots Panel 
      _slotsPanel = new UC_SlotsPanel( _camera ) {
        Visible = false,
        Location = c_pnlSlotsLocation
      };
      this.Controls.Add( _slotsPanel );

      // Views Panel 
      _viewsPanel = new UC_ViewsPanel( _camera, _btHCamera ) {
        Visible = false,
        Location = c_pnlCtrlLocation
      };
      this.Controls.Add( _viewsPanel );

      // Drone Panel 
      _dronePanel = new UC_DronePanel( _camera, _msfs_Keys ) {
        Visible = false,
        Location = c_pnlCtrlLocation
      };
      this.Controls.Add( _dronePanel );

      // Drone Panel 
      _6DOFPanel = new UC_6DOFPanel( _camera ) {
        Visible = false,
        Location = c_pnlCtrlLocation
      };
      this.Controls.Add( _6DOFPanel );

      // start Panels
      _slotsPanel.Visible = true;
      _viewsPanel.Visible = true;
      //_dronePanel.Visible = true; // DEBUG ONLY
      //_6DOFPanel.Visible = true; // DEBUG ONLY



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

      _tooltip.SetToolTip( btPilotView, "Cockpit - Pilot View" );
      _tooltip.SetToolTip( btCloseView, "Cockpit - Closer Instrument View" );
      _tooltip.SetToolTip( btLandView, "Cockpit - Landing View" );
      _tooltip.SetToolTip( btCoPilotView, "Cockpit - CoPilot View" );

      _tooltip.SetToolTip( btInstrumentIndexed, "Instrument Views - use Index buttons" );
      _tooltip.SetToolTip( btExternalIndexed, "Showcase Views - use Index buttons" );
      _tooltip.SetToolTip( btCustomCamera, "Custom Views - use Index buttons 1..10" );

      _tooltip.SetToolTip( btInstrumentQuick, "Cockpit Quick Views - Index 1..8" );
      _tooltip.SetToolTip( btExternalQuick, "External Quick Views - Index 1..8" );

      _tooltip.SetToolTip( btExternalFree, "External Free View - use mouse pan" );
      _tooltip.SetToolTip( btDrone, "Drone View - use drone controls" );

      _tooltip.SetToolTip( btDOF6, "6DOF View - numeric controls" );

      _tooltip.SetToolTip( btResetView, "Reset the view" );

      // Inop Checker
      timer1.Interval = 1000;

    }

    // form loads
    private void frmCameraV2_Load( object sender, EventArgs e )
    {
      // Init GUI
      Location = new Point( 20, 20 );
      // init with the proposed location from profile (check within a virtual box)
      if (dNetBm98.Utilities.IsOnScreen( AppSettings.Instance.CameraLocation, new Size( 100, 100 ) )) {
        this.Location = AppSettings.Instance.CameraLocation;
      }
      _lastLiveLocation = Location;

      // standalone connection status line
      lblSimConnected.BackColor = Color.Transparent;

      int i = 0;
      // Add Camera View buttons
      // Pilot Cam Selectors
      i = _btHCamera.AddButton( btPilotView, CamSelection_Action, CameraMode.PilotView );
      _btHCamera.ButtonFromSlot( i ).LastViewIndex = (int)PilotCamPosition.Pilot; // must remain fixed 
      i = _btHCamera.AddButton( btCloseView, CamSelection_Action, CameraMode.CloseView );
      _btHCamera.ButtonFromSlot( i ).LastViewIndex = (int)PilotCamPosition.Close; // must remain fixed 
      i = _btHCamera.AddButton( btLandView, CamSelection_Action, CameraMode.LandingView );
      _btHCamera.ButtonFromSlot( i ).LastViewIndex = (int)PilotCamPosition.Landing; // must remain fixed 
      i = _btHCamera.AddButton( btCoPilotView, CamSelection_Action, CameraMode.CoPilotView );
      _btHCamera.ButtonFromSlot( i ).LastViewIndex = (int)PilotCamPosition.Copilot; // must remain fixed
      // further Cam Selectors
      i = _btHCamera.AddButton( btCustomCamera, CamSelection_Action, CameraMode.CustomCamera ); _btHCamera.ButtonFromSlot( i ).LastViewIndex = 0;

      i = _btHCamera.AddButton( btInstrumentQuick, CamSelection_Action, CameraMode.InstrumentQuick ); _btHCamera.ButtonFromSlot( i ).LastViewIndex = 0;
      i = _btHCamera.AddButton( btInstrumentIndexed, CamSelection_Action, CameraMode.InstrumentIndexed ); _btHCamera.ButtonFromSlot( i ).LastViewIndex = 0;

      i = _btHCamera.AddButton( btExternalQuick, CamSelection_Action, CameraMode.ExternalQuick ); _btHCamera.ButtonFromSlot( i ).LastViewIndex = 0;
      i = _btHCamera.AddButton( btExternalIndexed, CamSelection_Action, CameraMode.ExternalIndexed ); _btHCamera.ButtonFromSlot( i ).LastViewIndex = 0;
      i = _btHCamera.AddButton( btExternalFree, CamSelection_Action, CameraMode.ExternalFree );//no ViewIndex

      i = _btHCamera.AddButton( btDrone, CamSelection_Action, CameraMode.Drone ); _btHCamera.ButtonFromSlot( i ).LastViewIndex = -1;//no ViewIndex
      i = _btHCamera.AddButton( btDOF6, CamSelection_Action, CameraMode.DOF6 ); _btHCamera.ButtonFromSlot( i ).LastViewIndex = -1;//no ViewIndex
      _btHCamera.DeactivateAll( );

      // standalone handling
      if (Standalone) {
        // File Access Check
        if (DbgLib.Dbg.Instance.AccessCheck( Folders.UserFilePath ) != DbgLib.AccessCheckResult.Success) {
          string msg = $"MyDocuments Folder Access Check Failed:\n{DbgLib.Dbg.Instance.AccessCheckResult}\n\n{DbgLib.Dbg.Instance.AccessCheckMessage}";
          MessageBox.Show( msg, "Access Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Error );
        }
        // activate connection, will cause SCAdapter events to finalize the connection
        SCAdapter.Connect( );
      }
      else {
        // connect via HudBar
        _camera?.ConnectSim( );
      }

      // start inop checker
      timer1.Start( );
    }

    // act when getting visible
    private void frmCameraV2_VisibleChanged( object sender, EventArgs e )
    {
      if (this.Visible) {
        // after hide, make sure we are live again
        this.TopMost = true;
        timer1.Enabled = true;
      }
    }

    // form is about to close
    private void frmCameraV2_FormClosing( object sender, FormClosingEventArgs e )
    {
      // no longer live
      this.TopMost = false;
      timer1.Enabled = false;

      // UnRegister DataUpdates
      _camera?.DisconnectSim( );

      // save last known good form location and size
      if (this.Visible && this.WindowState == FormWindowState.Normal) {
        AppSettings.Instance.CameraLocation = this.Location;
      }
      else {
        AppSettings.Instance.CameraLocation = _lastLiveLocation;
      }
      //--
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

    private void FrmCameraV2_Disposed( object sender, EventArgs e )
    {
      // local Dispose
      this.Controls.Remove( _slotsPanel ); _slotsPanel?.Dispose( );
      this.Controls.Remove( _viewsPanel ); _viewsPanel?.Dispose( );
      this.Controls.Remove( _dronePanel ); _dronePanel?.Dispose( );
      this.Controls.Remove( _6DOFPanel ); _6DOFPanel?.Dispose( );
      _camera?.Dispose( );
    }

    #endregion

    // camera changed
    private void CameraAPI_StateChanged( State.StateTransitionEventArgs e )
    {
      // runs on this. thread
    }

    // camera data arrived
    private void CameraAPI_DataArrived( )
    {
      // runs on this. thread
      UpdateGUI( );

      if (_slotsPanel.Visible) _slotsPanel.UpdateGUI( );
      if (_viewsPanel.Visible) _viewsPanel.UpdateGUI( );
      if (_dronePanel.Visible) _dronePanel.UpdateGUI( );
      if (_6DOFPanel.Visible) _6DOFPanel.UpdateGUI( );
    }

    // update this GUI
    private void UpdateGUI( )
    {
      _updatingGUI = true;

      var newCam = _camera.CameraAPI.CurrentCamMode;
      if (_currentCam != newCam) {
        // cam has changed

        HandledButton newButton = _btHCamera.ButtonFromCamSetting( newCam );
        if (newButton != null) {
          _btHCamera.ActivateButton( newButton.Slot ); // to maintain RadioButton behavior
          _currentCamSlot = newButton.Slot;
        }
        else {
          ; // ERROR
          _currentCamSlot = -1;
        }
        _currentCam = newCam;

        // panels
        _dronePanel.Visible = (newCam == CameraMode.Drone);
        _6DOFPanel.Visible = (newCam == CameraMode.DOF6);
        _viewsPanel.Visible = !(_dronePanel.Visible || _6DOFPanel.Visible);
      }

      _updatingGUI = false;
    }

    #region Generic Camera Actions

    // Handle Reset View Click
    private void btResetView_Click( object sender, EventArgs e )
    {
      _camera.CameraAPI.CamRequestAPI.RequestResetCamera( );
      //_camController?.RequestRecoverDroneSpeeds( );
    }

    // Handler for Camera Selector buttons
    private void CamSelection_Action( HandledButton sender )
    {
      // will be called with the button instance as sender
      if (sender.CamMode == CameraMode.NONE) return; // cannot switch to this cam..

      // direct change request
      _camera.CameraAPI.CamRequestAPI.RequestSwitchCamera( sender.CamMode, sender.LastViewIndex );
    }

    #endregion

    #region Key Configs

    // Use the Key Config Dialog
    private void btConfig_Click( object sender, EventArgs e )
    {
      _keyConfig = new frmKeyConfig( );
      // load current keys
      _keyConfig.LoadKeyDict( _msfs_Keys );

      if (_keyConfig.ShowDialog( this ) == DialogResult.OK) {
        // accept keys
        _msfs_Keys = new MSFS_KeyCat( _keyConfig.MSFS_Keys );
        // Reload controllers
        _camera.ReloadKeyCatalog( _msfs_Keys );
        // _flyByController?.ReloadKeyCatalog( _msfs_Keys );

        // _customCamController?.ReloadKeyCatalog( _msfs_Keys );

        // Save Config
        AppSettings.Instance.MSFS_KeyConfiguration = _msfs_Keys.ToConfigString( );
        AppSettings.Instance.Save( );
      }

      _keyConfig.Dispose( );
    }

    private void mnuKeyConfig_Click( object sender, EventArgs e )
    {
      btConfig_Click( sender, e );
    }

    // Offline cheat to use the Key Config
    private void lblSimConnected_DoubleClick( object sender, EventArgs e )
    {
      btConfig_Click( sender, e );
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
      _camera.ConnectSim( );
    }

    // disconnect event
    private void SCAdapter_Disconnected( object sender, EventArgs e )
    {
      _camera.DisconnectSim( );
    }

    #endregion


  }
}

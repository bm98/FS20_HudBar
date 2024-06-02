using dNetBm98.Win;

using FS20_CamControl;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FCamControl
{
  /// <summary>
  /// Implements the Drone Control Panel
  /// </summary>
  internal partial class UC_DronePanel : UserControl
  {

    private readonly ToolTip _tooltip = new ToolTip( );
    private ButtonHandler _btHDrone = null; // Drone Panel buttons

    // Camera Obj
    private readonly Camera _camera;

    // track slider movement - to avoid concurrent slider updates from user and Sim
    private bool _mouseDown = false;
    // track GUI update and don't mix writes up..
    private bool _updatingGUI = false;
    // toggle control input has no get from the Sim
    private bool _togCtrlState = false;

    // invoker for ready signal
    private WinFormInvoker _flyByInvoker;

    // controller to perform flyby's
    private FlyByController _flyByController;
    private int _flyByTimeMeter;

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_DronePanel( Camera camera, MSFS_KeyCat msfs_KeyCat )
    {
      if (msfs_KeyCat is null) {
        throw new ArgumentNullException( nameof( msfs_KeyCat ) );
      }

      _camera = camera ?? throw new ArgumentNullException( nameof( camera ) );

      InitializeComponent( );

      _flyByInvoker = new WinFormInvoker( btDrone_FlyByFire );
      _flyByController = new FlyByController( msfs_KeyCat, _camera );
      _flyByController.FlyByReadyProgress += _flyBy_FlyByReady;

      // Drone Panel Buttons -- colors from Control Area
      _btHDrone = new ButtonHandler( true ) {
        BColor = frmCameraV2.c_CtrlUnselBColor,
        FColor = Color.Black,
        ActBColor = frmCameraV2.c_CtrlSelBColor,
        ActFColor = Color.Black,
      };

      _tooltip.SetToolTip( btDrone_TogControls, "Toggle control input" );
      _tooltip.SetToolTip( btDrone_CamFollow, "Drone follows the target" );
      _tooltip.SetToolTip( btDrone_CamLock, "Drone locks to the target" );
      _tooltip.SetToolTip( tbMove, "Drone movement speed" );
      _tooltip.SetToolTip( tbRotate, "Drone rotation speed" );
      _tooltip.SetToolTip( tbZoom, "Drone zoom (field of view)" );

      _tooltip.SetToolTip( btDrone_FlyByReset, "FlyBy - Reset position" );
      //      _tooltip.SetToolTip( btFlyByFollow, "FlyBy - Follow aircraft" );
      _tooltip.SetToolTip( btDrone_FlyByInit, "FlyBy - Go to manual cam position" );
      _tooltip.SetToolTip( btDrone_FlyByPrep, "FlyBy - Go to selected cam position" );
      _tooltip.SetToolTip( btDrone_FlyByFire, "FlyBy - Release and fly by" );
      _tooltip.SetToolTip( btDrone_FromAbove, "FlyBy - Set position above" );
      _tooltip.SetToolTip( btDrone_FromBelow, "FlyBy - Set position below" );
      _tooltip.SetToolTip( btDrone_FromLeft, "FlyBy - Set position left of aircraft" );
      _tooltip.SetToolTip( btDrone_FromRight, "FlyBy - Set position right of aircraft" );

    }

    private void UC_DronePanel_Load( object sender, EventArgs e )
    {
      // Add Drone Panel buttons
      _btHDrone.AddButton( btDrone_CamLock, DroneButton_Action ); _btHDrone.AddButton( btDrone_CamFollow, DroneButton_Action );
      // flyby controls
      _btHDrone.AddButton( btDrone_FromAbove, ToggleDirection ); _btHDrone.AddButton( btDrone_FromBelow, ToggleDirection );
      _btHDrone.AddButton( btDrone_FromLeft, ToggleDirection ); _btHDrone.AddButton( btDrone_FromRight, ToggleDirection );
      _btHDrone.AddButton( btDrone_FlyByInit, BtFlyByInit_Action ); _btHDrone.AddButton( btDrone_FlyByPrep, BtFlyByPrep_Action );
      _btHDrone.AddButton( btDrone_FlyByFire, BtFlyByFire_Action );
      _btHDrone.AddButton( btDrone_TogControls, BtTogControls_Action );
    }

    /// <summary>
    /// Reload the KeyCatalog after changes
    /// </summary>
    /// <param name="msfsKeyCatalog">The Catalog to load</param>
    public void ReloadKeyCatalog( MSFS_KeyCat msfsKeyCatalog )
    {
      _flyByController?.ReloadKeyCatalog( msfsKeyCatalog );
    }

    /// <summary>
    /// Update the GUI from Data
    /// </summary>
    public void UpdateGUI( )
    {
      _updatingGUI = true;

      var camValues = _camera.CameraAPI.CamValueAPI;

      if (!_mouseDown) {
        tbMove.Value = camValues.DroneMoveSpeed;
        tbRotate.Value = camValues.DroneRotSpeed;
        tbZoom.Value = camValues.ZoomLevel;
      }
      btDrone_CamLock.Checked = camValues.DroneLock;
      btDrone_CamFollow.Checked = camValues.DroneFollow;

      _updatingGUI = false;
    }

    #region Drone

    // handles Drone Buttons
    private void DroneButton_Action( HandledButton sender )
    {
      if (_updatingGUI) return;

      // toggles from current
      if (sender.Button.Equals( btDrone_CamLock )) {
        _camera.CameraAPI.CamRequestAPI.RequestDroneLock( !_camera.CameraAPI.CamValueAPI.DroneLock );
        sender.Activate( _camera.CameraAPI.CamValueAPI.DroneLock );
      }
      else if (sender.Button.Equals( btDrone_CamFollow )) {
        _camera.CameraAPI.CamRequestAPI.RequestDroneFollow( !_camera.CameraAPI.CamValueAPI.DroneFollow );
        sender.Activate( _camera.CameraAPI.CamValueAPI.DroneFollow );
      }
    }

    // toggle this button checked state
    private void ToggleDirection( HandledButton sender )
    {
      sender.Activate( !sender.Active );
    }

    private void tbMove_ValueChanged( object sender, EventArgs e )
    {
      if (_updatingGUI) return;

      _camera.CameraAPI.CamRequestAPI.RequestDroneMoveSpeed( tbMove.Value );
    }

    private void tbRotate_ValueChanged( object sender, EventArgs e )
    {
      if (_updatingGUI) return;

      _camera.CameraAPI.CamRequestAPI.RequestDroneRotSpeed( tbRotate.Value );
    }

    // Zoom is used also for indexed cams
    private void tbZoom_ValueChanged( object sender, EventArgs e )
    {
      if (_updatingGUI) return;

      _camera.CameraAPI.CamRequestAPI.RequestZoomLevel( tbZoom.Value );
    }

    // mouse up/down of sliders
    private void Slider_MouseDown( object sender, MouseEventArgs e ) => _mouseDown = true;
    private void Slider_MouseUp( object sender, MouseEventArgs e ) => _mouseDown = false;

    #endregion

    #region FlyBy  / TODO CLEANUP once it works a bit


    // FlyBy Position Init 
    private void BtFlyByInit_Action( HandledButton sender )
    {
      btDrone_FlyByInit.Enabled = false;
      btDrone_FlyByInit.Checked = true;

      btDrone_FlyByFire.Visible = false;
      btDrone_FlyByFire.Checked = true;

      lblFlyByCountdown.Visible = true;
      lblFlyByCountdown.Text = "...";
      lblFlyByFailed.Visible = false;

      _flyByTimeMeter = -1; // init
      float dist = 0.05f;
      if (!_flyByController.Prepare( dist, false, false, false, false )) {
        // prepare failed
        lblFlyByFailed.Visible = true;
        btDrone_FlyByInit.Checked = false;
      }

      btDrone_FlyByInit.Enabled = true;
    }


    // FlyBy Position at distance
    private void BtFlyByPrep_Action( HandledButton sender )
    {
      btDrone_FlyByPrep.Enabled = false;
      btDrone_FlyByPrep.Checked = true;

      btDrone_FlyByFire.Visible = false;
      btDrone_FlyByFire.Checked = true;

      lblFlyByCountdown.Visible = true;
      lblFlyByCountdown.Text = "...";
      lblFlyByFailed.Visible = false;

      _flyByTimeMeter = -1; // init
      float dist = FlyByController.DistFromPercent( tbFlyByDist.Value );
      if (!_flyByController.Prepare( dist,
        btDrone_FromRight.Checked, btDrone_FromLeft.Checked,
        btDrone_FromAbove.Checked, btDrone_FromBelow.Checked )) {
        // prepare failed
        lblFlyByFailed.Visible = true;
        btDrone_FlyByPrep.Checked = false;
      }

      btDrone_FlyByPrep.Enabled = true;
    }

    // called from controller when ready to fire
    private void _flyBy_FlyByReady( object sender, FlyByControllerEventArgs e )
    {
      if (_flyByTimeMeter < 0) {
        _flyByTimeMeter = e.Remaining_ms;
      }

      // out of thread event, must invoke on the GUI element
      _flyByInvoker.HandleEvent( ( ) => {
        btDrone_FlyByFire.Visible = e.Ready;
        lblFlyByCountdown.Visible = !e.Ready;

        // reset all if done
        if (e.Ready) {
          btDrone_FlyByInit.Checked = false;
          btDrone_FlyByPrep.Checked = false;
        }
        // count down text
        string task = (_flyByTimeMeter - e.Remaining_ms) < 2500 ? "Cam Reset" : "Goto Pos";
        lblFlyByCountdown.Text = $"{task}\nDON'T CLICK\nwait.. {e.Remaining_ms / 1000f:#0.0}";
      } );

    }

    // Handle the click to view FlyBy
    private void BtFlyByFire_Action( HandledButton sender )
    {
      _flyByController?.Fire( );
      btDrone_FlyByFire.Visible = false;
      lblFlyByCountdown.Visible = false;
    }

    // reset drone cam 
    private void btDrone_FlyByReset_Click( object sender, EventArgs e )
    {
      _camera.CameraAPI.CamRequestAPI.RequestResetCamera( );
    }

    // toggle Controls
    private void BtTogControls_Action( HandledButton sender )
    {
      _camera.CameraAPI.CamRequestAPI.RequestDroneToggleControls( );
      _togCtrlState = !_togCtrlState; // toggle
      sender.Activate( _togCtrlState ); // update color state
    }

    // unckeck radios 
    private void tbFlyByDist_Scroll( object sender, EventArgs e )
    {
      // reset checked when used
      rbStage0.Checked = false;
      rbStage1.Checked = false;
      rbStage2.Checked = false;
      rbStage3.Checked = false;
    }

    // radio behavior
    private void btDrone_FromAbove_CheckedChanged( object sender, EventArgs e )
    {
      if (btDrone_FromAbove.Checked) {
        btDrone_FromBelow.Checked = false;
      }
    }

    // radio behavior
    private void btDrone_FromBelow_CheckedChanged( object sender, EventArgs e )
    {
      if (btDrone_FromBelow.Checked) {
        btDrone_FromAbove.Checked = false;
      }
    }

    // radio behavior
    private void btDrone_FromLeft_CheckedChanged( object sender, EventArgs e )
    {
      if (btDrone_FromLeft.Checked) {
        btDrone_FromRight.Checked = false;
      }
    }

    // radio behavior
    private void btDrone_FromRight_CheckedChanged( object sender, EventArgs e )
    {
      if (btDrone_FromRight.Checked) {
        btDrone_FromLeft.Checked = false;
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

  }
}

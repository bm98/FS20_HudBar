using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FCamControl
{
  /// <summary>
  /// Implements the Views Panel
  /// </summary>
  [DefaultEvent( "ValueChanged" )]
  public partial class UC_6DOFPanel : UserControl
  {
    private static readonly Vector3 c_cam6dDefaultPos = new Vector3( ) { X = 20f, Y = 20f, Z = 20f };

    private readonly ToolTip _tooltip = new ToolTip( );

    // Camera Obj (Ref)
    private readonly Camera _camera;

    private bool _camLocked = false;

    // track GUI update and don't mix writes up..
#pragma warning disable CS0414 // Remove unread private members
    private bool _updatingGUI = false;
#pragma warning restore CS0414 // Remove unread private members

    private readonly Color c_UnselColor = Color.FromArgb( 39, 37, 36 );
    private readonly Color c_SelColor = Color.FromArgb( 29, 148, 108 );

    private float _x = 0;
    private float _y = 0;
    private float _z = 0;

    private float _p = 0;
    private float _b = 0;
    private float _h = 0;


    // true while updating the numeric controls, don't trigger while updating
    private bool _updatingNumericCtrls = false;

    // avoid interfering with editing during GUI updates
    private const int c_editTimerStart = 15; // number of gui cycles to wait for GUI updates 
    private int _editingTimer = -1;

    /// <summary>
    /// Event triggered when a User Input has changed a value
    /// </summary>
    [Description( "A Value has changed" ), Category( "Action" )]
    public event EventHandler<EventArgs> ValueChanged;
    private void OnValueChanged( )
    {
      if (_updatingNumericCtrls) return; // not while updating from the code
      ValueChanged?.Invoke( this, new EventArgs( ) );

      _camera?.CameraAPI.CamRequestAPI.Request6DofPosition( Position, Gimbal );

    }

    /// <summary>
    /// cTor: V1
    /// </summary>
    internal UC_6DOFPanel( )
    {
      _camera = null;

      InitializeComponent( );

      btCamLock.Checked = true;
    }


    /// <summary >
    /// cTor: V2
    /// </summary>
    internal UC_6DOFPanel( Camera camera )
    {
      _camera = camera;

      InitializeComponent( );

      _tooltip.SetToolTip( btViewOut, "Look down left, or right side" );
      _tooltip.SetToolTip( btCamLock, "Lock the camera to the aircraft" );
      _tooltip.SetToolTip( btResetView, "Reset the camera view" );

      // cam lock is true on init
      _camLocked = true;
      btCamLock.Checked = _camLocked;
    }

    private void UC_6DOFPanel_Load( object sender, EventArgs e )
    {
      btResetView_Click( sender, e );
    }

    /// <summary>
    /// Update the GUI from Data
    /// </summary>
    public void UpdateGUI( )
    {
      if (_updatingNumericCtrls) return;
      if (_editingTimer-- > 0) return;

      _updatingGUI = true;

      var camValues = _camera.CameraAPI.CamValueAPI;

      Position = camValues.Cam6DofPosition;
      Gimbal = camValues.Cam6DofGimbal;
      CameraLocked = camValues.Cam6DofLocked;

      _updatingGUI = false;
    }

    /// <summary>
    /// Get;Set: Wether or not the camera is locked to the aircraft when changing values
    /// </summary>
    public bool CameraLocked {
      get => _camLocked;
      set {
        if (value == _camLocked) return; // already

        _camLocked = value;
        btCamLock.Checked = _camLocked;
        if (_camLocked) {
          // apply when locked
          AcftView( );
          OnValueChanged( );
        }
      }
    }


    // update a NumericControl with a value and don't trigger the ValueChanged event of the control while doing this
    private float UpdateNum( NumericUpDown num, float value )
    {
      decimal v = (decimal)Math.Round( value, 1 );

      if (v != num.Value) {
        // must change
        _updatingNumericCtrls = true;

        num.Value = (v > num.Maximum) ? num.Maximum
                     : (v < num.Minimum) ? num.Minimum
                     : v;

        _updatingNumericCtrls = false;
      }

      return (float)num.Value;
    }

    /// <summary>
    /// Get;Set: The Cam Position Component, this will not trigger a camera update
    /// </summary>
    public Vector3 Position {
      get => new Vector3( _x, _y, _z );
      set {
        UpdateNum( numX, value.X );
        UpdateNum( numY, value.Y );
        UpdateNum( numZ, value.Z );
      }
    }
    /// <summary>
    /// Get;Set: The Cam Gimbal Component, this will not trigger a camera update
    /// </summary>
    public Vector3 Gimbal {
      get => new Vector3( _p, _b, _h );
      set {
        UpdateNum( numP, value.X );
        UpdateNum( numB, value.Y );
        UpdateNum( numH, value.Z );
      }
    }

    // control value changes, may trigger cam updates when the user changes the numbers
    private void numX_ValueChanged( object sender, EventArgs e )
    {
      _x = UpdateNum( numX, (float)numX.Value );
      if (_camLocked) { AcftView( ); }
      OnValueChanged( );
    }

    // control value changes, may trigger cam updates when the user changes the numbers
    private void numY_ValueChanged( object sender, EventArgs e )
    {
      _y = UpdateNum( numY, (float)numY.Value );
      if (_camLocked) { AcftView( ); }
      OnValueChanged( );
    }

    // control value changes, may trigger cam updates when the user changes the numbers
    private void numZ_ValueChanged( object sender, EventArgs e )
    {
      _z = UpdateNum( numZ, (float)numZ.Value );
      if (_camLocked) { AcftView( ); }
      OnValueChanged( );
    }

    // control value changes, may trigger cam updates when the user changes the numbers
    private void numP_ValueChanged( object sender, EventArgs e )
    {
      _p = UpdateNum( numP, (float)numP.Value );
      OnValueChanged( );
    }

    // control value changes, may trigger cam updates when the user changes the numbers
    private void numB_ValueChanged( object sender, EventArgs e )
    {
      _b = UpdateNum( numB, (float)numB.Value );
      OnValueChanged( );
    }

    // control value changes, may trigger cam updates when the user changes the numbers
    private void numH_ValueChanged( object sender, EventArgs e )
    {
      _h = UpdateNum( numH, (float)numH.Value );
      OnValueChanged( );
    }

    // generic NumericControl Key down (editing) capture
    private void num_KeyDown( object sender, KeyEventArgs e )
    {
      _editingTimer = c_editTimerStart; // restart while typing
    }

    // generic NumericControl Leave (editing) capture
    private void num_Leave( object sender, EventArgs e )
    {
      _editingTimer = -1; // disable timer
    }

    // calculate the Gimbal to look at the aircraft (0/0/0) position without updating the cam at this moment
    private void AcftView( )
    {
      // calc view back to 0/0/0 from current XYZ
      var hdg = (float)(Math.Acos( _z / Math.Sqrt( _x * _x + _z * _z ) ) * (_x < 0 ? -1 : 1) * 180f / Math.PI);
      if (float.IsNaN( hdg )) {
        // catch where ACos cannot be evaluated and gets NaN
        hdg = _z < 0 ? 180 : 0;
      }
      UpdateNum( numH, (hdg >= 0) ? (hdg - 180f) : (hdg + 180f) );

      var pitch = (float)(Math.Acos( _y / Math.Sqrt( _x * _x + _y * _y + _z * _z ) ) * 180f / Math.PI) - 90f; // pitch angle 0 is in plane (not north up)
      if (float.IsNaN( pitch )) {
        // catch where ACos cannot be evaluated and gets NaN
        pitch = -90f; // pitch angle 0 is in plane (not north up)
      }
      UpdateNum( numP, pitch );
    }

    // calculate a look outside and down from the current position
    // causes the cam lock to release (else we cannot look outside..)
    private void btViewOut_Click( object sender, EventArgs e )
    {
      _camera.CameraAPI.CamRequestAPI.Request6DofCameraLock( false );

      if (_x > 0) {
        // Right side
        UpdateNum( numH, 90 );
      }
      else {
        // Left side
        UpdateNum( numH, -90 );
      }
      UpdateNum( numP, -15 );

      // trigger camera update
      OnValueChanged( );
    }



    // toggle on click
    private void btCamLock_Click( object sender, EventArgs e )
    {
      _camera.CameraAPI.CamRequestAPI.Request6DofCameraLock( !_camLocked );
    }

    // reset to somewhere outside
    private void btResetView_Click( object sender, EventArgs e )
    {
      _camera.CameraAPI.CamRequestAPI.Request6DofCameraLock( true );
      Position = c_cam6dDefaultPos;
      Gimbal = new Vector3( );
      // calc the locked view already
      AcftView( );

      // trigger camera update
      OnValueChanged( );
    }

  }
}

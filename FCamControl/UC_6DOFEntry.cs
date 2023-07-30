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
  /// Entry Panel for 6DOF Cam
  /// </summary>
  [DefaultEvent( "ValueChanged" )]
  public partial class UC_6DOFEntry : UserControl
  {

    private float _x = 0;
    private float _y = 0;
    private float _z = 0;

    private float _p = 0;
    private float _b = 0;
    private float _h = 0;

    // true while updating the numeric controls
    private bool _updating = false;

    /// <summary>
    /// Event triggered when a User Input has changed a value
    /// </summary>
    [Description( "A Value has changed" ), Category( "Action" )]
    public event EventHandler<EventArgs> ValueChanged;
    private void OnValueChanged( )
    {
      if (_updating) return; // not while updating from the code
      ValueChanged?.Invoke( this, new EventArgs( ) );
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_6DOFEntry( )
    {
      InitializeComponent( );
    }

    // update a number control
    private float UpdateNum( NumericUpDown num, float value )
    {
      decimal v = (decimal)Math.Round( value, 1 );
      _updating = true;
      num.Value = (v > num.Maximum) ? num.Maximum
                   : (v < num.Minimum) ? num.Minimum
                   : v;
      _updating = false;

      return (float)num.Value;
    }

    /// <summary>
    /// Returns the Cam Position Component
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
    /// Returns the Cam Gimbal Component
    /// </summary>
    public Vector3 Gimbal {
      get => new Vector3( _p, _b, _h );
      set {
        UpdateNum( numP, value.X );
        UpdateNum( numB, value.Y );
        UpdateNum( numH, value.Z );
      }
    }

    private void numX_ValueChanged( object sender, EventArgs e )
    {
      _x = UpdateNum( numX, (float)numX.Value );
      if (cbxLockAcftView.Checked) { AcftView( ); }
      OnValueChanged( );
    }

    private void numY_ValueChanged( object sender, EventArgs e )
    {
      _y = UpdateNum( numY, (float)numY.Value );
      if (cbxLockAcftView.Checked) { AcftView( ); }
      OnValueChanged( );
    }

    private void numZ_ValueChanged( object sender, EventArgs e )
    {
      _z = UpdateNum( numZ, (float)numZ.Value );
      if (cbxLockAcftView.Checked) { AcftView( ); }
      OnValueChanged( );
    }

    private void numP_ValueChanged( object sender, EventArgs e )
    {
      _p = UpdateNum( numP, (float)numP.Value );
      OnValueChanged( );
    }

    private void numB_ValueChanged( object sender, EventArgs e )
    {
      _b = UpdateNum( numB, (float)numB.Value );
      OnValueChanged( );
    }

    private void numH_ValueChanged( object sender, EventArgs e )
    {
      _h = UpdateNum( numH, (float)numH.Value );
      OnValueChanged( );
    }

    private void AcftView( )
    {
      // calc view back to 0/0/0 from current XYZ

      var hdg = (float)(Math.Acos( _z / Math.Sqrt( _x * _x + _z * _z ) ) * (_x < 0 ? -1 : 1) * 180f / Math.PI);
      UpdateNum( numH, (hdg > 0) ? (hdg - 180f) : (hdg + 180f) );

      var r = Math.Sqrt( _x * _x + _y * _y + _z * _z );
      float pitch = (float)(Math.Acos( _y / r ) * 180f / Math.PI) - 90f; // pitch angle 0 is in plane (not north up)
      UpdateNum( numP, -pitch );
    }

    // look outside
    private void btViewOut_Click( object sender, EventArgs e )
    {
      if (_x > 0) {
        // Right side
        UpdateNum( numH, 90 );
      }
      else {
        // Left side
        UpdateNum( numH, -90 );
      }
      UpdateNum( numP, 15 );
    }


  }
}

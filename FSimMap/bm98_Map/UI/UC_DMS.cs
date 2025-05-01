using System;
using System.ComponentModel;
using System.Windows.Forms;

using CoordLib;

namespace bm98_Map.UI
{
  /// <summary>
  /// DMS Field
  /// </summary>
  public partial class UC_DMS : UserControl
  {
    private double _dms = 0;
    private bool _isLat = true;
    private bool _readonly = false;

    // format text fields
    private void FormatDMS( )
    {
      var dms = Dms.ToDMSarray( _dms, _isLat );
      txDeg.Text = dms[0];
      txMin.Text = dms[1];
      txSec.Text = dms[2];
    }

    // capture input
    private void CaptureDMS( )
    {
      double val = Dms.ParseDMS( $"{txDeg.Text}° {txMin.Text}' {txSec.Text}\"" );
      _dms = double.IsNaN( val ) ? _dms : val;
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_DMS( )
    {
      InitializeComponent( );
    }

    private void UC_DMS_Load( object sender, EventArgs e )
    {
      FormatDMS( );
    }


    /// <summary>
    /// Get;Set: True to show a Latitude, else a Longitude
    /// </summary>
    [Category( "Data" )]
    [Description( "True when the controls shows a Latitude" )]
    public bool IsLat {
      get => _isLat;
      set {
        if (value == _isLat) return;
        _isLat = value;
        FormatDMS( );
      }
    }

    /// <summary>
    /// Get;Set: True to show a Latitude, else a Longitude
    /// </summary>
    [Category( "Behavior" )]
    [Description( "True when the controls shows a Latitude" )]
    public bool ReadOnly {
      get => _readonly;
      set {
        _readonly = value;
        txDeg.ReadOnly = _readonly;
        txMin.ReadOnly = _readonly;
        txSec.ReadOnly = _readonly;
      }
    }

    /// <summary>
    /// DMS Value
    /// </summary>
    public double Value {
      get {
        CaptureDMS( );
        return _dms;
      }
      set {
        _dms = value;
        FormatDMS( );
      }
    }

  }
}

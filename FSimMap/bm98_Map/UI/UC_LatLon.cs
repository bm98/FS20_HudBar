using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CoordLib;

using dNetBm98.Logging;

namespace bm98_Map.UI
{
  /// <summary>
  /// To handle Lat/Lon Alt values
  /// </summary>
  public partial class UC_LatLon : UserControl
  {
    // true if Deg, else DMS
    private bool _degMode = true;

    private double _lat = 0;
    private double _lon = 0;

    // Update Lat and Lon according to DEG setting
    private void UpdateLatLon( )
    {
      txLat.Visible = _degMode;
      txLon.Visible = _degMode;
      dmsLat.Visible = !_degMode;
      dmsLon.Visible = !_degMode;

      if (_degMode) {
        txLat.Text = $"{_lat:#0.00000}";
        txLon.Text = $"{_lon:#0.00000}";
      }
      else {
        dmsLat.Value = _lat;
        dmsLon.Value = _lon;
      }
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_LatLon( )
    {
      InitializeComponent( );
    }

    private void UC_LatLonAlt_Load( object sender, EventArgs e )
    {
      flpLon.Top = lblLon.Top - 3;
      flpLon.Left = flpLat.Left;
      UpdateLatLon( );
      SetMSA( 0 );
    }

    /// <summary>
    /// True for DEG mode, false for DMS
    /// </summary>
    public bool DegMode {
      get => _degMode;
      set {
        if (_degMode != value) {
          if (value) {
            rbDEG.Checked = true;
          }
          else {
            rbDMS.Checked = true;
          }
        }
      }
    }

    /// <summary>
    /// Latitude [°]
    ///  returns values captured on Set Button pressed
    /// </summary>
    public double Lat {
      get => _lat;
      set {
        _lat = value;
        UpdateLatLon( );
      }
    }
    /// <summary>
    /// Longitude [°]
    ///  returns values captured on Set Button pressed
    /// </summary>
    public double Lon {
      get => _lon;
      set {
        _lon = value;
        UpdateLatLon( );
      }
    }

    /// <summary>
    /// Set the MSA altitude
    /// </summary>
    /// <param name="msa">Altitude ft</param>
    public void SetMSA( int msa )
    {
      if (msa == 0) {
        lblMsaFt1000.Text = $"na";
        lblMsaFt100.Text = $" ";
      }
      else {
        lblMsaFt1000.Text = $"{msa / 1000:#0}";
        lblMsaFt100.Text = $"{(msa % 1000) / 100:0}";
      }
    }

    #region Event handling

    private void rb_CheckedChanged( object sender, EventArgs e )
    {
      _degMode = rbDEG.Checked;
      UpdateLatLon( );
    }

    #endregion



  }
}

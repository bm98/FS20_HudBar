using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.Config
{
  /// <summary>
  /// Form to enter Departure and Arrival Airport ICAOs
  /// </summary>
  public partial class frmApt : Form
  {
    public frmApt( )
    {
      InitializeComponent( );

      // must be empty to start with
      txDep.Text = ""; 
      txArr.Text = "";
    }

    /// <summary>
    /// Departure Apt ICAO
    /// </summary>
    public string DepAptICAO { get; set; } = "";
    /// <summary>
    /// Destination Apt ICAO
    /// </summary>
    public string ArrAptICAO { get; set; } = "";


    private void frmApt_Load( object sender, EventArgs e )
    {
      txArr.Text = ArrAptICAO;
    }

    private void frmApt_FormClosing( object sender, FormClosingEventArgs e )
    {
      this.Hide( );
    }

    private void btAccept_Click( object sender, EventArgs e )
    {
      var s = txDep.Text.TrimStart();
      if (s.Length > 4) s = s.Substring( 0, 4 ); // limit for sanity
      DepAptICAO = s.TrimEnd( ).ToUpperInvariant( );

      s = txArr.Text.TrimStart( );
      if (s.Length > 4) s = s.Substring( 0, 4 ); // limit for sanity
      ArrAptICAO = s.TrimEnd( ).ToUpperInvariant( );
    }

    private void btClear_Click( object sender, EventArgs e )
    {
      txDep.Text = "";
      txArr.Text = "";
    }

  }
}

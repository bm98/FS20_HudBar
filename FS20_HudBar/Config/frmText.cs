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
  public partial class frmText : Form
  {
    public frmText( )
    {
      InitializeComponent( );
    }

    private void frmText_Load( object sender, EventArgs e )
    {
      txFree.Text = AppSettingsV2.Instance.FreeText;
    }

    private void frmText_FormClosing( object sender, FormClosingEventArgs e )
    {
      this.Hide( );
    }

    private void btAccept_Click( object sender, EventArgs e )
    {
      var s = txFree.Text;
      if ( s.Length > 60 ) s = s.Substring( 0, 60 ); // limit for sanity
      AppSettingsV2.Instance.FreeText = s.TrimEnd();
    }
  }
}

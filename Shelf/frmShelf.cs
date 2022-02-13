using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.Shelf
{
  public partial class frmShelf : Form
  {

    /// <summary>
    /// Set the Shelf Folder to use
    /// </summary>
    /// <param name="fName">A directory name</param>
    /// <returns>True if successful</returns>
    public bool SetShelfFolder( string fName )
    {
      try {
        aShelf.SetShelfFolder( fName );
        AppSettings.Instance.ShelfFolder = fName;
        return true;
      }
      catch ( Exception ) {
        return false;
      }
    }

    public frmShelf( )
    {
      InitializeComponent( );
    }

    private void frmShelf_Load( object sender, EventArgs e )
    {
      ;
    }

    private void frmShelf_FormClosing( object sender, FormClosingEventArgs e )
    {
      if ( this.Visible && this.WindowState == FormWindowState.Normal ) {
        AppSettings.Instance.ShelfLocation = this.Location;
        AppSettings.Instance.ShelfSize = this.Size;
      }

      if ( e.CloseReason == CloseReason.UserClosing ) {
        // we don't close if the User clicks the X Box, only Hide; else it will not maintain the content throughout
        e.Cancel = true;
        this.Hide( );
      }
    }

    // the Shelf gets TopMost while visible
    private void frmShelf_VisibleChanged( object sender, EventArgs e )
    {
      if ( this.Visible ) {
        this.TopMost = true;
      }
      else {
        this.TopMost = false;
      }
    }

  }
}

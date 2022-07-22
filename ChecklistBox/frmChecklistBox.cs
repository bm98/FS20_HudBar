using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.ChecklistBox
{
  public partial class frmChecklistBox : Form
  {
    private const string c_savePath = "MSFS_HudBarSave"; // Folder Used for FLT saves as well
    private string m_savePath = "";

    public frmChecklistBox( )
    {
      // must be set before Controls are loaded

      InitializeComponent( );

      m_savePath = Path.Combine( Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments ), c_savePath );
      if (!Directory.Exists( m_savePath )) {
        // shall not fail to create a folder
        try {
          Directory.CreateDirectory( m_savePath );
        }
        catch {
          m_savePath = "NONE"; // will be catched later on and save to AppDir
        }
      }
      chklistBox.UserDir = m_savePath;

    }

    private void frmChecklistBox_Load( object sender, EventArgs e )
    {
    }

    private void frmChecklistBox_FormClosing( object sender, FormClosingEventArgs e )
    {
      if (this.Visible && (this.WindowState == FormWindowState.Normal)) {
        AppSettings.Instance.ChecklistBoxLocation = this.Location;
        AppSettings.Instance.Save( );
      }

      if (e.CloseReason == CloseReason.UserClosing) {
        // we don't close if the User clicks the X Box, only Hide; else it will not maintain the content throughout
        e.Cancel = true;
        this.Hide( );
      }
    }

    // user selected Hide in the Box Menu
    private void chklistBox_HideClicked( object sender, EventArgs e )
    {
      this.Hide( );
    }


    #region Mouse handlers for moving the Box around

    private bool m_moving = false;
    private Point m_moveOffset = new Point( 0, 0 );

    private void frmChecklistBox_MouseDown( object sender, MouseEventArgs e )
    {
      if (e.Button != MouseButtons.Left) return;
      m_moving = true;
      m_moveOffset = e.Location;
    }

    private void frmChecklistBox_MouseMove( object sender, MouseEventArgs e )
    {
      if (!m_moving) return;

      this.Location = new Point( this.Location.X + e.X - m_moveOffset.X, this.Location.Y + e.Y - m_moveOffset.Y );
    }

    private void frmChecklistBox_MouseUp( object sender, MouseEventArgs e )
    {
      if (m_moving) {
        AppSettings.Instance.ChecklistBoxLocation = this.Location;
        AppSettings.Instance.Save( );
        m_moving = false;
      }
    }

    #endregion

  }
}

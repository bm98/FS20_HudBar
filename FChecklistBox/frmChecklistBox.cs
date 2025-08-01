﻿using System;
using System.Drawing;
using System.Windows.Forms;

using bm98_hbFolders;

namespace FChecklistBox
{
  /// <summary>
  /// Checklist Box Form
  /// </summary>
  public partial class frmChecklistBox : Form
  {
    // track the last known live location in order to save the proper one
    private Point _lastLiveLocation;

    /// <summary>
    /// Set true to run in standalone mode
    /// </summary>
    public bool Standalone { get; private set; } = false;

    // FORM
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="instance">An instance name (use "" as default)</param>
    /// <param name="standalone">Standalone flag (defaults to false)</param>
    public frmChecklistBox( string instance, bool standalone = false )
    {
      // the first thing to do
      Standalone = standalone;

      // Init the Folders Utility with our AppSettings File
      Folders.InitStorage( "FCBoxAppSettings.json" );


      AppSettings.InitInstance( Folders.SettingsFile, instance );
      // ---------------

      InitializeComponent( );

      this.ShowInTaskbar = true;

      // use another WindowFrame
      if (Standalone) {
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MinimizeBox = true;
        this.MaximizeBox = false;
        this.ControlBox = true;
      }

      chklistBox.UserDir = Folders.UserFilePath; // main HudBar location
    }

    // form is loaded to get visible
    private void frmChecklistBox_Load( object sender, EventArgs e )
    {
      // Init GUI
      Location = new Point( 20, 20 );
      // init with the proposed location from profile (check within a virtual box)
      if (dNetBm98.Utilities.IsOnScreen( AppSettings.Instance.ChecklistBoxLocation, new Size( 100, 100 ) )) {
        Location = AppSettings.Instance.ChecklistBoxLocation;
      }
      _lastLiveLocation = Location;

      // load the Config Dialog props
      chklistBox.ConfigLocation = AppSettings.Instance.ConfigLocation;
      chklistBox.ConfigSize = AppSettings.Instance.ConfigSize;

      // standalone handling
      if (Standalone) {
        // File Access Check
        if (DbgLib.Dbg.Instance.AccessCheck( Folders.UserFilePath ) != DbgLib.AccessCheckResult.Success) {
          string msg = $"MyDocuments Folder Access Check Failed:\n{DbgLib.Dbg.Instance.AccessCheckResult}\n\n{DbgLib.Dbg.Instance.AccessCheckMessage}";
          MessageBox.Show( msg, "Access Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Error );
        }
      }

    }

    // about to close the form
    private void frmChecklistBox_FormClosing( object sender, FormClosingEventArgs e )
    {
      this.TopMost = false;

      if (this.Visible && (this.WindowState == FormWindowState.Normal)) {
        AppSettings.Instance.ChecklistBoxLocation = this.Location;
      }
      else {
        AppSettings.Instance.ChecklistBoxLocation = _lastLiveLocation;
      }
      // save the Config Dialog props
      AppSettings.Instance.ConfigLocation = chklistBox.ConfigLocation;
      AppSettings.Instance.ConfigSize = chklistBox.ConfigSize;

      AppSettings.Instance.Save( );

      if (Standalone) {
        // don't cancel if standalone (else how to close it..)
        this.WindowState = FormWindowState.Minimized;
      }
      else {
        if (e.CloseReason == CloseReason.UserClosing) {
          // we don't close if the User clicks the X Box, only Hide; else it will not maintain the content throughout
          e.Cancel = true;
          this.Hide( );
        }
      }
    }

    // user selected Hide in the Box Menu
    private void chklistBox_HideClicked( object sender, EventArgs e )
    {
      this.TopMost = false;

      if (Standalone) {
        // don't hide if standalone
        this.WindowState = FormWindowState.Minimized;
      }
      else {
        this.Hide( );
      }
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

    private void frmChecklistBox_VisibleChanged( object sender, EventArgs e )
    {
    }

    private void frmChecklistBox_Activated( object sender, EventArgs e )
    {
      this.TopMost = true;
    }

    private void frmChecklistBox_LocationChanged( object sender, EventArgs e )
    {
      if (this.Visible)
        _lastLiveLocation = this.Location;
    }
  }
}

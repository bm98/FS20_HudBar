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

namespace bm98_Checklist
{
  /// <summary>
  /// Configuration Dialog
  /// </summary>
  public partial class frmConfig : Form
  {
    #region STATIC

    /// <summary>
    /// Get; Set: The UserDir where the config file should be found (or not...)
    /// Note the Dir shall exist - will not be created here
    /// </summary>
    public static string UserDir { get; set; } = "";
    private static bool _userDirValid = false;

    /// <summary>
    /// The filename for the Config File 
    /// </summary>
    public const string c_CheckListFile = "CheckLists.json";

    // returns the proper userdir path
    /// <summary>
    /// Returns the ConfigFile Path and Name
    /// </summary>
    /// <returns>Configfile</returns>
    public static string ConfigFileName( )
    {
      _userDirValid = Directory.Exists( UserDir );
      if (_userDirValid) return Path.Combine( UserDir, c_CheckListFile );

      // UserDir not found - this would be in the AppDir ...
      return c_CheckListFile;
    }

    // The one and only Font Manager
    internal static FontProvider FontManager = new FontProvider( );

    #endregion


    // Limit for practical reasons
    private const int c_MaxNumberOfChecklists = 20;

    // the default checklist with some values
    private Json.Checklist _defaultChecklist = null;

    private void PopupateCheckSize( )
    {
      cbxCheckSize.Items.Clear( );
      cbxCheckSize.Items.Add( "Medium" );
      cbxCheckSize.Items.Add( "Small" );
      cbxCheckSize.Items.Add( "Large" );
    }

    private void PopulateCheckColor( )
    {
      cbxCheckColor.Items.Clear( );
      cbxCheckColor.Items.Add( "Blue" );
      cbxCheckColor.Items.Add( "Red" );
      cbxCheckColor.Items.Add( "Green" );
      cbxCheckColor.Items.Add( "Yellow" );
      cbxCheckColor.Items.Add( "White" );
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public frmConfig( )
    {
      InitializeComponent( );

      PopulateCheckColor( );
      PopupateCheckSize( );
    }

    private void frmConfig_Load( object sender, EventArgs e )
    {
      string cFilename = ConfigFileName( );

      // get the default checklist from the control
      _defaultChecklist = new UC_CheckPage( ).GetChecklist( );

      tabCfg.TabPages.Clear( );

      // try to load the config; returns null on error
      Json.ChecklistCat cfg = Json.Formatter.FromJsonFile<Json.ChecklistCat>( cFilename );
      if (cfg == null) {
        // No or wrong File - start with an empty single Page Config
        // Add a first Page
        tabCfg.TabPages.Add( "Page 1" );
        var cp = new UC_CheckPage( ) { Name = "ucP0", Dock = DockStyle.Fill, AutoScroll = true };

        tabCfg.TabPages[0].Controls.Add( cp );
        tabCfg.SelectedIndex = 0;
        cbxCheckColor.SelectedIndex = 0; // Blue
        cbxCheckSize.SelectedIndex = 0;  // Medium
      }
      else {
        Json.ChecklistCat.VersionUp( cfg );
        // load from File
        foreach (var checklist in cfg.Checklists) {
          AddNewTab( checklist );
        }
        chkOrientation.Checked = cfg.Horizontal;
        cbxCheckColor.SelectedIndex = (cfg.CheckColor >= 0) ? cfg.CheckColor : (int)SwitchColor.Blue;
        cbxCheckSize.SelectedIndex = (cfg.CheckSize >= 0) ? cfg.CheckSize : (int)CheckSize.SizeMedium;
      }
      tabCfg.SelectedIndex = 0;
    }

    // Add a Tab from the given Checklist
    private void AddNewTab( Json.Checklist checklist )
    {
      int newTabIndex = tabCfg.TabPages.Count;
      tabCfg.TabPages.Add( $"Page_{newTabIndex + 1}", "Change Below" );
      TabPage tab = tabCfg.TabPages[newTabIndex];
      var cp = new UC_CheckPage( ) { Name = $"ucP_{newTabIndex}", Dock = DockStyle.Fill, AutoScroll = true };
      tab.Controls.Add( cp );
      // add handlers
      cp.CopyClicked += Cp_CopyClicked;
      cp.DeleteClicked += Cp_DeleteClicked;
      cp.HeaderChanged += Cp_HeaderChanged;
      cp.ReloadButtonFonts( );
      cp.LoadChecklist( checklist );
      tab.Text = checklist.Name;
      // select it
      tabCfg.SelectTab( tabCfg.TabCount - 1 );
    }

    private void DeleteCurrentTab( )
    {
      if (tabCfg.TabCount < 2) return; // cannot delete the last tab 

      TabPage tab = tabCfg.SelectedTab;
      tabCfg.TabPages.Remove( tab );
      if (tab.Controls[0] is UC_CheckPage) {
        var cp = tab.Controls[0] as UC_CheckPage;
        cp.CopyClicked -= Cp_CopyClicked;
        cp.DeleteClicked -= Cp_DeleteClicked;
        cp.HeaderChanged -= Cp_HeaderChanged;
      }
      // rename pages
      for (int i = 0; i < tabCfg.TabPages.Count; i++) {
        tab = tabCfg.TabPages[i];
        tab.Name = $"Page_{i + 1}";
      }
      tabCfg.SelectTab( 0 );
    }


    // Add Button
    private void btAdd_Click( object sender, EventArgs e )
    {
      if (tabCfg.TabPages.Count >= c_MaxNumberOfChecklists) return; // already full

      AddNewTab( _defaultChecklist );
    }

    // Copy Button
    private void Cp_CopyClicked( object sender, EventArgs e )
    {
      if (tabCfg.TabPages.Count >= c_MaxNumberOfChecklists) return; // already full

      var tab = tabCfg.SelectedTab;
      if (tab.Controls[0] is UC_CheckPage) {
        var cp = tab.Controls[0] as UC_CheckPage;
        var checklist = cp.GetChecklist( );
        AddNewTab( checklist );
      }
    }

    // Delete Button
    private void Cp_DeleteClicked( object sender, EventArgs e )
    {
      DeleteCurrentTab( );
    }

    // Checklist Name changed
    private void Cp_HeaderChanged( object sender, EventArgs e )
    {
      var tab = tabCfg.SelectedTab;
      if (tab.Controls[0] is UC_CheckPage) {
        var cp = tab.Controls[0] as UC_CheckPage;
        tab.Text = cp.Header;
      }
    }

    // Font Button was clicked
    private void btFont_Click( object sender, EventArgs e )
    {
      fntDlg.Font = frmConfig.FontManager.GetFont( "DEFAULT" );
      if (fntDlg.ShowDialog( this ) == DialogResult.OK) {
        // register user font
        frmConfig.FontManager.SetUserFont( fntDlg.Font );
        // reload fonts
        foreach (TabPage tab in tabCfg.TabPages) {
          if (tab.Controls[0] is UC_CheckPage) {
            var cp = tab.Controls[0] as UC_CheckPage;
            cp.ReloadButtonFonts( );
          }
        }
      }
    }

    // Checkbox Size changed
    private void cbxCheckSize_SelectedIndexChanged( object sender, EventArgs e )
    {
      foreach (TabPage tab in tabCfg.TabPages) {
        if (tab.Controls[0] is UC_CheckPage) {
          var cp = tab.Controls[0] as UC_CheckPage;
          cp.SetBoxSize( (CheckSize)cbxCheckSize.SelectedIndex );
        }
      }
    }

    private void btAccept_Click( object sender, EventArgs e )
    {
      //Save Config
      Json.ChecklistCat cfg = new Json.ChecklistCat {
        Horizontal = chkOrientation.Checked,
        CheckColor = cbxCheckColor.SelectedIndex,
        CheckSize = cbxCheckSize.SelectedIndex,
        UserFont = frmConfig.FontManager.GetUserConfigString( ),
        LayoutVersion = Json.ChecklistCat.LAYOUT_VERSION,
      };
      foreach (TabPage tab in tabCfg.TabPages) {
        if (tab.Controls[0] is UC_CheckPage) {
          var cp = tab.Controls[0] as UC_CheckPage;
          var checklist = cp.GetChecklist( );
          cfg.Checklists.Add( checklist );
        }
      }
      if (!Json.Formatter.ToJsonFile( cfg, ConfigFileName( ) )) {
        Console.WriteLine( $"Checklist.Config: Writing the Configfile failed" );
      }

      this.DialogResult = DialogResult.OK;
      this.Hide( );
    }

    private void btCancel_Click( object sender, EventArgs e )
    {
      // Discard Config
      this.DialogResult = DialogResult.Cancel;
      this.Hide( );
    }
  }
}

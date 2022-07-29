using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace bm98_Checklist
{
  /// <summary>
  /// The Main UserControl for the Checklist Box
  /// </summary>
  public partial class UC_Checklist : UserControl
  {
    private readonly Color c_AllDoneColor = Color.FromArgb( 0, 48, 0 );


    private readonly Color c_CheckText = Color.LightYellow;
    private readonly Color c_CheckTextDone = Color.Pink; // will be changed in cTor..

    private frmConfig CFG;

    // Current Config
    private Json.ChecklistCat _currentCatalog = null;
    // The Checklist in use
    private Json.Checklist _currentChecklist = null;
    // The selected Phase
    private Json.CheckPhase _currentPhase = null;

    /// <summary>
    /// Hide was selected from the Menu
    /// </summary>
    public event EventHandler HideClicked;
    private void OnHideClicked( )
    {
      HideClicked?.Invoke( this, new EventArgs( ) );
    }

    /// <summary>
    /// Get; Set: The UserDir where the config file should be found (or not...)
    /// Note the Dir shall exist - will not be created here
    /// </summary>
    [Description( "The UserDir where the config file should be found" ), Category( "Data" )]
    public string UserDir {
      get => frmConfig.UserDir;
      set {
        frmConfig.UserDir = value; // a bit ugly but it is maintained in the config Dialog
      }
    }

    // Create the Selection Menu for all Checklists
    private void PopulateChecklists( )
    {
      if (_currentCatalog == null) return; // ??

      mChecklist.DropDownItems.Clear( );
      foreach (var checklist in _currentCatalog.Checklists) {
        var tsi = mChecklist.DropDownItems.Add( checklist.Name );
        tsi.Tag = mChecklist.DropDownItems.Count - 1; // Index: 0..N-1
        tsi.Click += mCList_Click;
      }
    }

    // Create the Phase Menu for a Checklistst
    private void PopulatePhases( )
    {
      // clear all from the first separator upwards and before the ListName (item 0)
      int tss = ctxMenu.Items.IndexOf( tssPhase );
      for (int i = tss - 1; i > 0; i--) {
        var item = ctxMenu.Items[i];
        ctxMenu.Items.RemoveAt( i );
        item.Dispose( );
      }

      if (_currentChecklist == null) return; // ??

      // insert enabled Phases
      int phaseIndex = 0;
      int insertIndex = 1; // after the ListName
      foreach (var phase in _currentChecklist.Phases) {
        if (phase.Enabled) {
          var tsi = new ToolStripMenuItem( phase.Name );
          tsi.Click += mChk_Click;
          tsi.Tag = phaseIndex;
          ctxMenu.Items.Insert( insertIndex++, tsi );
        }
        phaseIndex++; // 
      }
      // assign the used name
      lblChecklist.Text = _currentChecklist.Name;
      mListName.Text = _currentChecklist.Name;
    }

    // Create the Checks Items for the current Phase
    private void PopulateChecks( )
    {
      flp.SuspendLayout( );
      while (flp.Controls.Count > 0) {
        var item = flp.Controls[0];
        flp.Controls.RemoveAt( 0 );
        item.Dispose( );
      }
      if (_currentPhase == null) return; // when all Phases are disabled ...

      int checkIndex = 0;
      int insertIndex = 0;
      foreach (var check in _currentPhase.Checks) {
        if (!string.IsNullOrWhiteSpace( check )) {
          var uc = new UC_PushButtonLEDTop( ) {
            Name = $"Check_{checkIndex + 1}",
            Tag = checkIndex,
            ButtonText = check,
            OnState = false,
            ForeColor = c_CheckText,
            PushOffColor = (SwitchColor)_currentCatalog.CheckColor,
            PushOnColor = SwitchColor.Dark,
            Size = Helper.CheckBoxSizes[_currentCatalog.CheckSize],
            Font = frmConfig.FontManager.GetFont( "DEFAULT" ),
          };
          flp.Controls.Add( uc );
          uc.PushbuttonPressed += Uc_PushbuttonPressed;
          insertIndex++;
        }
        checkIndex++;
      }
      lblPhase.Font = frmConfig.FontManager.GetFont( "PHASE" );
      lblPhase.Text = _currentPhase.Name;
      CheckAllDone( );
      flp.ResumeLayout( );
    }


    // Load a checklist
    private void LoadConfigFile( )
    {
      string cFilename = frmConfig.ConfigFileName( );
      // try to load the config; returns null on error
      Json.ChecklistCat cfg = Json.Formatter.FromJsonFile<Json.ChecklistCat>( cFilename );
      if (cfg == null) {
        // no valid Config File.. create a dummy one for a start
        _currentCatalog = new Json.ChecklistCat {
          UserFont = "",
          CheckSize = (int)CheckSize.SizeMedium,
          CheckColor = (int)SwitchColor.Blue,
          LayoutVersion = Json.ChecklistCat.LAYOUT_VERSION,
        };
        _currentCatalog.Checklists.Add( new Json.Checklist { Name = "USE CONFIGURE" } );
        _currentCatalog.Checklists[0].Phases.Add( new Json.CheckPhase( ) { Name = "EMPTY", Enabled = true, Checks = new List<string>( ) { "DUMMY" } } );
      }
      else {
        Json.ChecklistCat.VersionUp( cfg );
        //Load Config
        _currentCatalog = cfg;
      }
      // load font if needed
      if (!string.IsNullOrWhiteSpace( _currentCatalog.UserFont )) {
        frmConfig.FontManager.SetUserConfigString( _currentCatalog.UserFont );
      }
      // fill GUI
      FlowDirection newDirection = (_currentCatalog.Horizontal) ? FlowDirection.LeftToRight : FlowDirection.TopDown;
      if (flp.FlowDirection != newDirection) {
        // assign only if needed
        flp.FlowDirection = newDirection;
      }
//      flp.FlowDirection = (_currentCatalog.Horizontal) ? FlowDirection.LeftToRight : FlowDirection.TopDown;
      PopulateChecklists( );
      _currentChecklist = _currentCatalog.Checklists[0];
      PopulatePhases( );
      _currentPhase = _currentChecklist.Phases.FirstOrDefault( x => x.Enabled ); // only set enabled ones
      PopulateChecks( );
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_Checklist( )
    {
      InitializeComponent( );

      c_CheckTextDone = Color.FromArgb( c_CheckText.R - 75, c_CheckText.G - 75, c_CheckText.B - 75 ); // Dimm

      // register fonts on init
      frmConfig.FontManager.RegisterFont( "DEFAULT", ucPbReference.Font ); // base font
      frmConfig.FontManager.RegisterFont( "PHASE", lblPhase.Font );

    }

    private void UC_Checklist_Load( object sender, EventArgs e )
    {
      LoadConfigFile( );
    }

    // A checklist was clicked
    private void mCList_Click( object sender, EventArgs e )
    {
      if (!(sender is ToolStripMenuItem)) return;
      var tsi = sender as ToolStripMenuItem;
      int checklistIndex = (tsi.Tag is int @tag) ? @tag : -1;
      if (checklistIndex < 0) return;
      // load the checklist with index and make the first phase visible
      _currentChecklist = _currentCatalog.Checklists[checklistIndex];
      PopulatePhases( );
      _currentPhase = _currentChecklist.Phases.FirstOrDefault( x => x.Enabled ); // only set enabled ones
      PopulateChecks( );
    }

    // A Phase Item was clicked
    private void mChk_Click( object sender, EventArgs e )
    {
      if (!(sender is ToolStripMenuItem)) return;
      var tsi = sender as ToolStripMenuItem;
      int phaseIndex = (tsi.Tag is int @tag) ? @tag : -1;
      if (phaseIndex < 0) return;
      // load Phase with index
      _currentPhase = _currentChecklist.Phases[phaseIndex];
      PopulateChecks( );
    }

    // Configuration was clicked
    private void mConfig_Click( object sender, EventArgs e )
    {
      CFG = new frmConfig( );

      if (CFG.ShowDialog( this ) == DialogResult.OK) {
        // User Font may have changed

        // reload
        LoadConfigFile( );
      }
    }

    // Reset was clicked
    private void mReset_Click( object sender, EventArgs e )
    {
      // set all current checks unchecked
      foreach (var obj in flp.Controls) {
        if (obj is UC_PushButtonLEDTop) {
          UC_PushButtonLEDTop bt = obj as UC_PushButtonLEDTop;
          bt.OnState = false;
          bt.ForeColor = c_CheckText;
        }
      }
      CheckAllDone( );
    }

    // Hide was clicked
    private void mHide_Click( object sender, EventArgs e )
    {
      OnHideClicked( );
    }

    // A Checkbutton was pressed
    private void Uc_PushbuttonPressed( object sender, MouseEventArgs e )
    {
      if (!(sender is UC_PushButtonLEDTop)) return; // Programm error - just don't fail..
      if (e.Button != MouseButtons.Left) return; // Must use left button

      UC_PushButtonLEDTop bt = sender as UC_PushButtonLEDTop;
      bt.OnState = !bt.OnState; // toggle
      bt.ForeColor = bt.OnState ? c_CheckTextDone : c_CheckText;
      CheckAllDone( );
    }

    // this will turn the lights green when all are checked
    private void CheckAllDone( )
    {
      // set all current checks unchecked
      var allDone = true;
      foreach (var obj in flp.Controls) {
        if (obj is UC_PushButtonLEDTop) {
          var bt = (obj as UC_PushButtonLEDTop);
          allDone &= bt.OnState;
        }
      }
      // turns the panel background green if allDone
      flp.BackColor = allDone ? c_AllDoneColor : Color.Transparent;
      /*
      foreach (var obj in flp.Controls) {
        if (obj is UC_PushButtonLEDTop) {
          var bt = (obj as UC_PushButtonLEDTop);
          bt.PushOnColor = (allDone) ? SwitchColor.Green : SwitchColor.White;
        }
      }
      */
    }

    #region propagate Mouse Events ?? don't know why they are not bubbled up ??
    private void propagate_MouseDown( object sender, MouseEventArgs e )
    {
      this.OnMouseDown( e );
    }

    private void propagate_MouseEnter( object sender, EventArgs e )
    {
      this.OnMouseEnter( e );
    }

    private void propagate_MouseMove( object sender, MouseEventArgs e )
    {
      this.OnMouseMove( e );
    }

    private void propagate_MouseUp( object sender, MouseEventArgs e )
    {
      this.OnMouseUp( e );
    }

    private void propagate_MouseLeave( object sender, EventArgs e )
    {
      this.OnMouseLeave( e );
    }

    private void propagate_MouseHover( object sender, EventArgs e )
    {
      this.OnMouseHover( e );
    }
    #endregion
  }
}

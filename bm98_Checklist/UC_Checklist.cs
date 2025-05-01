using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

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

    private readonly UC_PushButton _btNext;

    // Current Config
    private Json.ChecklistCat _currentCatalog = null;
    // The Checklist in use
    private Json.Checklist _currentChecklist = null;
    // The selected Phase
    private Json.CheckPhase _currentPhase = null;

    // store the selected phase to apply the checkmarks
    private ToolStripMenuItem _checkedChecklist = new ToolStripMenuItem( );
    // store the selected phase to apply the checkmarks
    private ToolStripMenuItem _checkedPhase = new ToolStripMenuItem( );

    /// <summary>
    /// Hide was selected from the Menu
    /// </summary>
    [Description( "Hide was selected from the Menu" ), Category( "Action" )]
    public event EventHandler<EventArgs> HideClicked;
    private void OnHideClicked( )
    {
      HideClicked?.Invoke( this, new EventArgs( ) );
    }

    /// <summary>
    /// NEXT button was clicked
    /// </summary>
    [Description( "NEXT button was clicked" ), Category( "Action" )]
    public event EventHandler<EventArgs> NextPhaseClicked;
    private void OnNextPhaseClicked( )
    {
      NextPhaseClicked?.Invoke( this, new EventArgs( ) );
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

    /// <summary>
    /// Last known Config Location
    /// </summary>
    public Point ConfigLocation { get; set; } = new Point( 10, 10 );
    /// <summary>
    /// Last known Config Size
    /// </summary>
    public Size ConfigSize { get; set; } = new Size( );


    // Create the Selection Menu for all Checklists
    private void PopulateChecklists( )
    {
      if (_currentCatalog == null) return; // ??

      int checklistIndex = 0; // Index: 0..N-1
      mChecklist.DropDownItems.Clear( );
      foreach (var checklist in _currentCatalog.Checklists) {
        ToolStripMenuItem tsi = new ToolStripMenuItem( checklist.Name );
        tsi.Click += mCList_Click;
        tsi.Tag = checklistIndex;
        // on load check the first phase as it is the default start
        if (checklistIndex == 0) {
          tsi.Checked = true;
          _checkedChecklist = tsi; // remember
        }
        mChecklist.DropDownItems.Add( tsi );
        checklistIndex++;
      }
    }

    // select the Checklist of the menu item, loads the Phase and Checks
    // base SelecteChecklist() method - does all 
    private void SelectChecklist( ToolStripMenuItem tsi )
    {
      int checklistIndex = (tsi.Tag is int @tag) ? @tag : -1;
      if (checklistIndex < 0) return;

      // maintain checked state
      _checkedChecklist.Checked = false;
      tsi.Checked = true;
      _checkedChecklist = tsi;
      // select checklist and load
      _currentChecklist = _currentCatalog.Checklists[checklistIndex];
      PopulatePhases( );
      _currentPhase = _currentChecklist.Phases.FirstOrDefault( x => x.Enabled ); // only set enabled ones
      PopulateChecks( );
    }

    // select the Checklist with index (bail out on invalid indexes)
    private void SelectChecklist( int checklistIndex )
    {
      // sanity
      if (checklistIndex < 0) return;
      if (checklistIndex >= mChecklist.DropDownItems.Count) return;
      if (checklistIndex >= _currentCatalog.Checklists.Count) return; // should really not..

      var mi = mChecklist.DropDownItems[checklistIndex];
      if (mi is ToolStripMenuItem tsi) {
        SelectChecklist( tsi );
      }
    }

    // select the Checklist with name or the first if not found
    private void SelectChecklist( string checklistName )
    {
      foreach (var mi in mChecklist.DropDownItems) {
        if (mi is ToolStripMenuItem tsi) {
          if (tsi.Text == checklistName) {
            SelectChecklist( tsi );
            return;
          }
        }
      }
      // not found...
      SelectChecklist( 0 );
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
          // on load check the first phase as it is the default start
          if (phaseIndex == 0) {
            tsi.Checked = true;
            _checkedPhase = tsi; // remember
          }
          ctxMenu.Items.Insert( insertIndex++, tsi );
        }
        phaseIndex++; // 
      }
      // assign the used name
      lblChecklist.Text = _currentChecklist.Name;
      mListName.Text = _currentChecklist.Name;
    }

    // select the Phase of the menu item
    // base SelectPhase() method - does all 
    private void SelectPhase( ToolStripMenuItem tsi )
    {
      int phaseIndex = (tsi.Tag is int @tag) ? @tag : -1;
      if (phaseIndex < 0) return;
      // load Phase with index
      _checkedPhase.Checked = false;
      tsi.Checked = true;
      _checkedPhase = tsi; // remember last checked

      _currentPhase = _currentChecklist.Phases[phaseIndex];
      PopulateChecks( );
    }

    // select the next phase if there is any
    private void SelectNextPhase( )
    {
      var cur = ctxMenu.Items.IndexOf( _checkedPhase );
      if (cur == -1) return;
      // next (no check as there is always that next
      if (ctxMenu.Items[cur + 1] is ToolStripMenuItem tsi) {
        SelectPhase( tsi );
      };
    }

    // Create the Checks Items for the current Phase
    private void PopulateChecks( )
    {
      flp.Visible = false;
      flp.SuspendLayout( );
      while (flp.Controls.Count > 0) {
        var item = flp.Controls[0];
        flp.Controls.RemoveAt( 0 );
        // dispose all but our NEXT button
        if (!item.Equals( _btNext )) {
          item.Dispose( );
        }
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
      // add the NEXT button at the end
      flp.Controls.Add( _btNext );
      _btNext.Size = Helper.CheckBoxSizes[_currentCatalog.CheckSize];
      _btNext.Visible = true;

      flp.ResumeLayout( );
      flp.Visible = true;
    }


    // Load a checklists from ConfigFile
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

      // Reload and try to select the previously selected one
      string lastChecklistName = _checkedChecklist.Text;
      PopulateChecklists( );
      // try to select the previous one if it still exists
      SelectChecklist( lastChecklistName );
    }


    /// <summary>
    /// cTor:
    /// </summary>
    public UC_Checklist( )
    {
      InitializeComponent( );

      // hold the Next Button Ref
      _btNext = btNEXT_DONT_REMOVE; // take from Designer
      _btNext.PushbuttonPressed += _btNext_PushbuttonPressed;
      _btNext.ForeColor = c_CheckText;

      c_CheckTextDone = Color.FromArgb( c_CheckText.R - 75, c_CheckText.G - 75, c_CheckText.B - 75 ); // Dimm

      // register fonts on init
      frmConfig.FontManager.RegisterFont( "DEFAULT", ucPbReference.Font ); // base font
      frmConfig.FontManager.RegisterFont( "PHASE", lblPhase.Font );
    }

    // load event
    private void UC_Checklist_Load( object sender, EventArgs e )
    {
      LoadConfigFile( );
    }

    // A checklist was clicked
    private void mCList_Click( object sender, EventArgs e )
    {
      if (!(sender is ToolStripMenuItem)) return;
      var tsi = sender as ToolStripMenuItem;
      SelectChecklist( tsi );
    }


    // A Phase Item was clicked
    private void mChk_Click( object sender, EventArgs e )
    {
      if (!(sender is ToolStripMenuItem)) return;
      var tsi = sender as ToolStripMenuItem;
      SelectPhase( tsi );
    }

    // NEXT button clicked
    private void _btNext_PushbuttonPressed( object sender, MouseEventArgs e )
    {
      SelectNextPhase( );
      OnNextPhaseClicked( );
    }

    // Configuration was clicked
    private void mConfig_Click( object sender, EventArgs e )
    {
      CFG = new frmConfig( );
      CFG.LastLocation = ConfigLocation;
      CFG.LastSize = ConfigSize;

      if (CFG.ShowDialog( this ) == DialogResult.OK) {
        // User Font may have changed
        // reload
        LoadConfigFile( );

      }
      // save Config for Settings
      ConfigLocation = CFG.Location;
      ConfigSize = CFG.Size;

      CFG.Dispose( );
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
      if (e.Button != MouseButtons.Left) return; // Must use left button, right is context menu

      UC_PushButtonLEDTop bt = sender as UC_PushButtonLEDTop;
      bt.OnState = !bt.OnState; // toggle
      bt.ForeColor = bt.OnState ? c_CheckTextDone : c_CheckText;
      CheckAllDone( );
    }

    // turns the panel background green if allDone
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
    // this will turn the lights green when all are checked
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

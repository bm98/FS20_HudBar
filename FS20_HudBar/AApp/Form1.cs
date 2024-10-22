using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DbgLib;

using SC = SimConnectClient;
using SimConnectClientAdapter;
using static FS20_HudBar.GUI.GUI_Colors;
using static FSimClientIF.Sim;

using FS20_HudBar.GUI.Templates.Base;
using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.Bar.Items;
using FS20_HudBar.Bar;
using FS20_HudBar.Config;
using System.IO;
using FS20_HudBar.GUI;
using FSimClientIF.Modules;
using bm98_hbFolders;
using FShelf;
using FSimClientIF;

namespace FS20_HudBar
{
  /// <summary>
  /// As for the WinForms environment there is no way to have a Form to be translucent but maintain
  /// the GUI elements in full brightness
  /// 
  /// As a workaround we use 2 synched forms where one carries the GUI elements
  /// and the other acts as Main form and has opacity set to whatever is choosen.
  /// 
  /// This Form is the Owner of the GUI Form.
  /// It will not have any elements shown 
  /// The Opacity property will get as a translucent background of both.
  /// This Form needs to manage the Movement and Sizing of the GUI form.
  /// 
  /// The Form is the one managed by the user (moved) and set top of Z-Order
  /// It will Own the GUI Form and as per doc the Owned Form will never be behind the Owner and therefore
  /// the Translucent background should stay second to the GUI
  /// 
  /// </summary>
  public partial class frmMain : Form, IColorType
  {
    #region STATIC
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );
    #endregion

    // SimConnect Client Adapter
    private SCClient SCAdapter;

    // Handle of the Primary Screen to attach bar and tile
    private readonly Screen m_mainScreen;
    // the screen the bar is attached to
    private Screen m_barScreen;
    // the screen number of the above
    private int m_barScreenNumber = 0;

    // This will be the GUI form
    private frmGui m_frmGui;
    private DispPanel flp;

    // A HudBar standard ToolTip for the Button Helpers
    private ToolTip_Base m_toolTip = new ToolTip_Base( );

    // The configuration 
    private Configuration m_config = null;

    // The profiles
    //    private List<CProfile> m_profiles = new List<CProfile>( );
    //    private int m_selProfile = 0;
    private ToolStripMenuItem[] m_profileMenu; // enable array access for the MenuItems

    // Handles the RawInput from HID Keyboards
    Win.HotkeyController _keyHook;

    // MSFS Input handlers
    private Dictionary<Hotkeys, SC.SimEvents.SimEventAdapter> _fsInputCat = new Dictionary<Hotkeys, SC.SimEvents.SimEventAdapter>( );

    // SimVar access
    private readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;

    // The Flightbag
    private FShelf.frmShelf _Shelf;

    // Camera Selector
    private FCamControl.frmCameraV2 _Camera;

    // Checklist Box
    private FChecklistBox.frmChecklistBox _ChecklistBox;

    // Configuration Dialog
    //    private readonly frmConfig CFG = new frmConfig( );
    private readonly frmConfigV2 CFG = new frmConfigV2( ); // 20240223

    // need to stop processing while reconfiguring the bar
    private bool m_initDone = false;

    #region IColorType Interface
    public void UpdateColor( )
    {
      this.BackColor = c_WinBG; // start with the default for non Opaque

      if (m_config.UsedProfile.Opacity >= 0.999) {
        // full opaque is configurable
        this.BackColor = GUI_Colors.ItemColor( ColorType.cOpaqueBG );
      }
    }

    #endregion 

    #region Settings Concept Update 

    // needed for transition to new AppSettings concept
    private void UpdateCamSettings( )
    {
      if (!Program.AppSettingsV1Available) return; // V1 is not available, bail out

      if (AppSettings.Instance.CameraSlotFolder0 != "UPGRADED") {
        string[] settings = new string[6] {
          AppSettings.Instance.CameraSlotFolder0,
          AppSettings.Instance.CameraSlotFolder1,
          AppSettings.Instance.CameraSlotFolder2,
          AppSettings.Instance.CameraSlotFolder3,
          AppSettings.Instance.CameraSlotFolder4,
          AppSettings.Instance.CameraSlotFolder5
          };
        _Camera.UpdateSettings( settings );
        AppSettings.Instance.CameraSlotFolder0 = "UPGRADED";
        AppSettings.Instance.Save( );
      }
    }

    private void UpdateShelfSettings( )
    {
      if (!Program.AppSettingsV1Available) return; // V1 is not available, bail out

      if (AppSettings.Instance.ShelfFolder != "UPGRADED") {
        string settings = AppSettings.Instance.ShelfFolder;
        _Shelf.UpdateSettings( settings );
        AppSettings.Instance.ShelfFolder = "UPGRADED";
        AppSettings.Instance.Save( );
      }
    }

    private void UpdateCheckListBoxSettings( )
    {
      // no settings to update
      //  has it's own config file already
    }

    #endregion

    #region Synch GUI Forms

    /// <summary>
    /// Ensure a Tile to be in Bounds of the Main Screen
    ///  Bounds means that at least a part of the Tile remains on the main screen to be catched by the Mouse
    ///  The allowance is 1/2 of the bar Width or Height
    /// </summary>
    /// <param name="curLoc">Current Location</param>
    /// <param name="size">The Size of the Tile</param>
    /// <returns>Proposed new Location to be within the rules</returns>
    private Point TileBoundLocation( Point curLoc, Size size )
    {
      Point newL = curLoc;

      // Tiles are bound to a border of the main screen
      switch (m_config.UsedProfile.Placement) {
        case GUI.Placement.TopStack:
        case GUI.Placement.Top:
          newL.X = (newL.X > m_barScreen.Bounds.Right - size.Width / 2) ? m_barScreen.Bounds.Right - size.Width / 2 : newL.X; // catch right side out of bounds
          newL.X = (newL.X < m_barScreen.Bounds.Left - size.Width / 2) ? m_barScreen.Bounds.Left - size.Width / 2 : newL.X; // catch left side out of bounds, wins if the width is > screen.Width
          break;
        case GUI.Placement.Bottom:
          newL.X = (newL.X > m_barScreen.Bounds.Right - size.Width / 2) ? m_barScreen.Bounds.Right - size.Width / 2 : newL.X; // catch right side out of bounds
          newL.X = (newL.X < m_barScreen.Bounds.Left - size.Width / 2) ? m_barScreen.Bounds.Left - size.Width / 2 : newL.X; // catch left side out of bounds, wins if the width is > screen.Width
          break;
        case GUI.Placement.Left:
          newL.Y = (newL.Y > m_barScreen.Bounds.Bottom - size.Height / 2) ? m_barScreen.Bounds.Bottom - size.Height / 2 : newL.Y; // catch top side out of bounds
          newL.Y = (newL.Y < m_barScreen.Bounds.Top - size.Height / 2) ? m_barScreen.Bounds.Top - size.Height / 2 : newL.Y; // catch top side out of bounds, wins if the width is > screen.Height
          break;
        case GUI.Placement.Right:
          newL.Y = (newL.Y > m_barScreen.Bounds.Bottom - size.Height / 2) ? m_barScreen.Bounds.Bottom - size.Height / 2 : newL.Y; // catch top side out of bounds
          newL.Y = (newL.Y < m_barScreen.Bounds.Top - size.Height / 2) ? m_barScreen.Bounds.Top - size.Height / 2 : newL.Y; // catch top side out of bounds, wins if the width is > screen.Height
          break;
        default: break;
      }

      return newL;
    }


    /// <summary>
    /// Calc and return the Size of the Bar
    /// </summary>
    /// <returns>A Size</returns>
    private Size BarSize( )
    {
      Size newS = m_frmGui.PreferredSize;

      // get the window manager decoration borders from the Main Window
      Rectangle screenClientRect = base.RectangleToScreen( base.ClientRectangle );
      int leftBorderWidth = screenClientRect.Left - base.Left;
      int rightBorderWidth = base.Right - screenClientRect.Right;
      int topBorderHeight = screenClientRect.Top - base.Top;
      int bottomBorderHeight = base.Bottom - screenClientRect.Bottom;
      // define the needed Window Size from the AutoSized GUI Form
      // AutoSize on our Main Form does not catch the the GUI changes as they are not contained in the Form
      // The GUI Form and it's FlowPanel is only overlaid over the Main Form
      newS.Width += leftBorderWidth + rightBorderWidth;
      newS.Height += topBorderHeight + bottomBorderHeight;
      // Adjust for Bar which is stretching the screen in vert or hor direction and is only on the Main Screen 
      if (m_config.UsedProfile.Kind == GUI.Kind.Bar) {
        switch (m_config.UsedProfile.Placement) {
          case GUI.Placement.TopStack:
          case GUI.Placement.Top: newS.Width = m_barScreen.Bounds.Width; break;
          case GUI.Placement.Bottom: newS.Width = m_barScreen.Bounds.Width; break;
          case GUI.Placement.Left: newS.Height = m_barScreen.Bounds.Height; break;
          case GUI.Placement.Right: newS.Height = m_barScreen.Bounds.Height; break;
          default: break; // program error...
        }
      }

      return newS;
    }

    /// <summary>
    /// Calc and return the Location of the Bar
    /// </summary>
    /// <param name="curLoc">Current Location</param>
    /// <param name="size">The proposed BarSize</param>
    /// <returns>A Point</returns>
    private Point BarLocation( Point curLoc, Size size )
    {
      Point newL = curLoc;

      // location is managed when it is a Bar or Tile, else it is movable and we return the current Location
      // Bar is on the Main Screen Border aligned full width or height
      // Tile follows the preferred border but the dimension is the same as a Borderless Window
      switch (m_config.UsedProfile.Placement) {
        case GUI.Placement.TopStack:
        case GUI.Placement.Top:
          if (m_config.UsedProfile.Kind == GUI.Kind.Bar) newL = new Point( m_barScreen.Bounds.X, m_barScreen.Bounds.Y );
          else if (m_config.UsedProfile.Kind == GUI.Kind.Tile) {
            newL.Y = m_barScreen.Bounds.Y;
            newL = TileBoundLocation( newL, size );
          }
          break;
        case GUI.Placement.Bottom:
          if (m_config.UsedProfile.Kind == GUI.Kind.Bar) newL = new Point( m_barScreen.Bounds.X, m_barScreen.Bounds.Y + m_barScreen.Bounds.Height - size.Height );
          else if (m_config.UsedProfile.Kind == GUI.Kind.Tile) {
            newL.Y = m_barScreen.Bounds.Y + m_barScreen.Bounds.Height - size.Height;
            newL = TileBoundLocation( newL, size );
          }
          break;
        case GUI.Placement.Left:
          if (m_config.UsedProfile.Kind == GUI.Kind.Bar) newL = new Point( m_barScreen.Bounds.X, m_barScreen.Bounds.Y );
          else if (m_config.UsedProfile.Kind == GUI.Kind.Tile) {
            newL.X = m_barScreen.Bounds.X;
            newL = TileBoundLocation( newL, size );
          }
          break;
        case GUI.Placement.Right:
          if (m_config.UsedProfile.Kind == GUI.Kind.Bar) newL = new Point( m_barScreen.Bounds.X + m_barScreen.Bounds.Width - size.Width, m_barScreen.Bounds.Y );
          else if (m_config.UsedProfile.Kind == GUI.Kind.Tile) {
            newL.X = m_barScreen.Bounds.X + m_barScreen.Bounds.Width - size.Width;
            newL = TileBoundLocation( newL, size );
          }
          break;
        default:
          break;
      }

      return newL;
    }

    /// <summary>
    /// Returns a Rectangle containing the Bar Form
    /// Takes care of the Kind and Position taken from the HUD
    /// </summary>
    /// <returns>A Rectangle</returns>
    private Rectangle BarRectangle( )
    {
      Size newS = BarSize( );
      Point newL = BarLocation( this.Location, newS );

      return new Rectangle( newL, newS );
    }

    // synch the two forms for Size
    private void SynchGUISize( )
    {
      // Adding controls to the FlowPanel is enclosed in a   _inLayout = true ...  = false bracket
      // While adding controls to the FlowPanel there is some Resizing we don't want to catch
      // else it looks weird and takes more time than needed
      if (!_inLayout) {
        Rectangle newR = BarRectangle( );
        // set This Size and Loc if a change is needed
        if ((this.Width != newR.Width) || (this.Height != newR.Height)) {
          this.Size = newR.Size;
        }
        if ((this.Left != newR.Left) || (this.Top != newR.Top)) {
          this.Location = newR.Location;
        }
      }
    }

    // synch the GUI Form to overlay the Main Form
    private void SynchGUILocation( )
    {
      // in Windowed mode we need to get it from the client rect
      m_frmGui.Location = this.PointToScreen( this.ClientRectangle.Location );
    }

    // synch the two forms for Size and Location
    private void SynchGUI( )
    {
      SynchGUISize( );
      SynchGUILocation( );
    }

    // synch the two forms for Visible state
    private void SynchGUIVisible( bool visible )
    {
      this.Visible = visible;
      m_frmGui.Visible = visible;
    }

    #endregion

    #region InGame Hooks

    // Hook into the InGame Keyboard Events 
    private void SetupInGameHook( bool enabled )
    {
      // sanity checks
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      if (enabled) {
        if (_fsInputCat.Count <= 0) {
          // reinitiate the hooks
          _fsInputCat.Add( Hotkeys.Show_Hide, SC.SimConnectClient.Instance.AppInputAdapter( SC.SimEvents.AppInputE.FST_01 ) );
          _fsInputCat[Hotkeys.Show_Hide].AppInputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hotkeys.FlightBag, SC.SimConnectClient.Instance.AppInputAdapter( SC.SimEvents.AppInputE.FST_07 ) );
          _fsInputCat[Hotkeys.FlightBag].AppInputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hotkeys.Camera, SC.SimConnectClient.Instance.AppInputAdapter( SC.SimEvents.AppInputE.FST_08 ) );
          _fsInputCat[Hotkeys.Camera].AppInputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hotkeys.ChecklistBox, SC.SimConnectClient.Instance.AppInputAdapter( SC.SimEvents.AppInputE.FST_09 ) );
          _fsInputCat[Hotkeys.ChecklistBox].AppInputArrived += FSInput_InputArrived;

          // ONLY the first 5 have SimKey Hooks (6..10 do not have this hotkey)
          _fsInputCat.Add( Hotkeys.Profile_1, SC.SimConnectClient.Instance.AppInputAdapter( SC.SimEvents.AppInputE.FST_02 ) );
          _fsInputCat[Hotkeys.Profile_1].AppInputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hotkeys.Profile_2, SC.SimConnectClient.Instance.AppInputAdapter( SC.SimEvents.AppInputE.FST_03 ) );
          _fsInputCat[Hotkeys.Profile_2].AppInputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hotkeys.Profile_3, SC.SimConnectClient.Instance.AppInputAdapter( SC.SimEvents.AppInputE.FST_04 ) );
          _fsInputCat[Hotkeys.Profile_3].AppInputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hotkeys.Profile_4, SC.SimConnectClient.Instance.AppInputAdapter( SC.SimEvents.AppInputE.FST_05 ) );
          _fsInputCat[Hotkeys.Profile_4].AppInputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hotkeys.Profile_5, SC.SimConnectClient.Instance.AppInputAdapter( SC.SimEvents.AppInputE.FST_06 ) );
          _fsInputCat[Hotkeys.Profile_5].AppInputArrived += FSInput_InputArrived;
        }
      }
      else {
        // disable all callbacks
        foreach (var fi in _fsInputCat) {
          // this shall never fail..
          try {
            fi.Value.AppInputArrived -= FSInput_InputArrived;
          }
          catch { }
        }
        // remove all cat entries
        _fsInputCat.Clear( );
      }
    }

    // Receive commands from FSim
    private void FSInput_InputArrived( object sender, SC.SimEvents.AppInputEventArgs e )
    {
      // sanity checks
      if (!SC.SimConnectClient.Instance.IsConnected) return; // catch odd cases of disruption
      if (_fsInputCat.Count <= 0) return;

      // _fsInputCat should be valid when this event fires..
      if (e.ActionName == _fsInputCat[Hotkeys.Show_Hide].AppInput) SynchGUIVisible( !this.Visible );
      else if (e.ActionName == _fsInputCat[Hotkeys.FlightBag].AppInput) mShelf_Click( null, new EventArgs( ) );
      else if (e.ActionName == _fsInputCat[Hotkeys.Camera].AppInput) mCamera_Click( null, new EventArgs( ) );
      else if (e.ActionName == _fsInputCat[Hotkeys.ChecklistBox].AppInput) mChecklistBox_Click( null, new EventArgs( ) );
      else if (e.ActionName == _fsInputCat[Hotkeys.Profile_1].AppInput) mP1_Click( null, new EventArgs( ) );
      else if (e.ActionName == _fsInputCat[Hotkeys.Profile_2].AppInput) mP2_Click( null, new EventArgs( ) );
      else if (e.ActionName == _fsInputCat[Hotkeys.Profile_3].AppInput) mP3_Click( null, new EventArgs( ) );
      else if (e.ActionName == _fsInputCat[Hotkeys.Profile_4].AppInput) mP4_Click( null, new EventArgs( ) );
      else if (e.ActionName == _fsInputCat[Hotkeys.Profile_5].AppInput) mP5_Click( null, new EventArgs( ) );
    }

    /// <summary>
    /// returns a crlf formatted string of the hooks and their MSFS Key items
    /// </summary>
    /// <returns></returns>
    internal string InGameHints( )
    {
      string ret = "";
      foreach (var hi in _fsInputCat) {
        ret += $"{hi.Key} -> {hi.Value.SimEventName}\n";
      }
      if (string.IsNullOrEmpty( ret )) {
        ret = "There are no InGame Hooks active";
      }
      else {
        ret = "InGame Hooks active - use the commands listed below\n" + ret;
      }
      return ret;
    }

    #endregion

    #region Keyboard Hooks

    // Enable/Disable Keyboard interaction 
    private void SetupKeyboardHook( bool install, bool enabled )
    {
      if (install) {
        if (HUD == null) return; // no HUD so far - ignore this one
        if (_keyHook == null) {
          _keyHook = new Win.HotkeyController( Handle );
        }
        // disable momentarily
        _keyHook.KeyHandling( false );
        // reload the bindings
        _keyHook.RemoveAllKeys( );

        if (enabled) {
          if (HUD.Hotkeys.ContainsKey( Hotkeys.Show_Hide )) _keyHook.AddKey( HUD.Hotkeys[Hotkeys.Show_Hide], Hotkeys.Show_Hide.ToString( ), OnHookKey );
          if (HUD.Hotkeys.ContainsKey( Hotkeys.FlightBag )) _keyHook.AddKey( HUD.Hotkeys[Hotkeys.FlightBag], Hotkeys.FlightBag.ToString( ), OnHookKey );
          if (HUD.Hotkeys.ContainsKey( Hotkeys.Camera )) _keyHook.AddKey( HUD.Hotkeys[Hotkeys.Camera], Hotkeys.Camera.ToString( ), OnHookKey );
          if (HUD.Hotkeys.ContainsKey( Hotkeys.ChecklistBox )) _keyHook.AddKey( HUD.Hotkeys[Hotkeys.ChecklistBox], Hotkeys.ChecklistBox.ToString( ), OnHookKey );
          // enable always
          if (HUD.Hotkeys.ContainsKey( Hotkeys.MoveBarToOtherWindow )) _keyHook.AddKey( HUD.Hotkeys[Hotkeys.MoveBarToOtherWindow], Hotkeys.MoveBarToOtherWindow.ToString( ), OnHookKey );
          // profile switchers
          for (int p = 0; p < CProfile.c_numProfiles; p++) {
            if (HUD.Hotkeys.ContainsKey( (Hotkeys)p )) _keyHook.AddKey( HUD.Hotkeys[(Hotkeys)p], ((Hotkeys)p).ToString( ), OnHookKey );
          }
        }

        // enable handling
        _keyHook.KeyHandling( true );
      }
      else {
        // de-install - we cannot unhook the RawInputLib / not currently used anyway 
        _keyHook?.KeyHandling( false );
        _keyHook?.RemoveAllKeys( );
        _keyHook?.Dispose( );
        _keyHook = null;
      }
    }

    // Callback - handles the Keyboard Hooks
    private void OnHookKey( string tag )
    {
      if (tag == Hotkeys.Show_Hide.ToString( )) SynchGUIVisible( !this.Visible );
      else if (tag == Hotkeys.FlightBag.ToString( )) mShelf_Click( null, new EventArgs( ) );
      else if (tag == Hotkeys.Camera.ToString( )) mCamera_Click( null, new EventArgs( ) );
      else if (tag == Hotkeys.ChecklistBox.ToString( )) mChecklistBox_Click( null, new EventArgs( ) );
      else if (tag == Hotkeys.MoveBarToOtherWindow.ToString( )) MoveBarToNextScreen( );
      else {
        // check if a profile was triggered
        for (int p = 0; p < CProfile.c_numProfiles; p++) {
          if (tag == ((Hotkeys)p).ToString( )) {
            // switch Profile - this may take too long for some Profiles if they contain a lot of items
            // Windows may unhook in such cases (see MS doc rsp. code in HookController)
            m_config.SetProfile( p );
            InitGUI( );
            return;
          }
        }
      }
    }

    #endregion

    private readonly string c_facDBmsg
      = "The Facility Database could not be found!\n\nPlease visit the QuickGuide, head for 'DataLoader' and proceed accordingly"
      + "\n\nDo you want to check next startup again - click YES\n"
      + "\nClicking NO will no longer check for the Database to exist.";
    private void CheckFacilityDB( )
    {
      if (!File.Exists( Folders.GenAptDBFile )) {
        if (AppSettingsV2.Instance.OmitDBCheck) { return; }
        if (MessageBox.Show( c_facDBmsg, "Facility Database Missing", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation ) == DialogResult.No) {
          AppSettingsV2.Instance.OmitDBCheck = true;
          AppSettingsV2.Instance.Save( );
        }
      }
    }


    /// <summary>
    /// Main Form Init
    /// </summary>
    public frmMain( )
    {
      InitializeComponent( );

      // Load all from AppSettings
      var AS = AppSettingsV2.Instance;

      AS.Reload( );
      LOG.Info( "frmMain", "Load Configuration" );
      m_config = Configuration.GetFromSettings( );

      // last from Setting
      m_config.SetProfile( AS.SelProfile );
      mSelProfile.Text = m_config.UsedProfileName;
      //      m_selProfile = AS.SelProfile;


      // collect the Menus for the profiles
      m_profileMenu = new ToolStripMenuItem[] { mP1, mP2, mP3, mP4, mP5, mP6, mP7, mP8, mP9, mP10 };

      LOG.Info( "frmMain", "Load Colors" );
      Colorset = ToColorSet( AS.Appearance );

      mAltMetric.CheckState = AS.Altitude_Metric ? CheckState.Checked : CheckState.Unchecked;
      mDistMetric.CheckState = AS.Distance_Metric ? CheckState.Checked : CheckState.Unchecked;
      mShowUnits.CheckState = AS.ShowUnits ? CheckState.Checked : CheckState.Unchecked;

      // Setup the screen the bar/tile is attached to
      // bugfix- set m_barScreen before it is used....
      m_barScreenNumber = AS.ScreenNumber;
      m_barScreen = null;
      // Find and hold the Primary Screen
      Screen[] screens = Screen.AllScreens;
      m_mainScreen = screens[0];
      // now get the Primary one and the stored one
      for (int scIndex = 0; scIndex < screens.Length; scIndex++) {
        if (screens[scIndex].Primary) {
          m_mainScreen = screens[scIndex];
        }
        if (scIndex == m_barScreenNumber) {
          m_barScreen = screens[scIndex];
        }
      }
      // did not find the stored screen ?? disconnected, changed ??
      if (m_barScreen == null) {
        m_barScreen = m_mainScreen; // to main
      }

      // ShowUnits and Opacity are set via HUD in InitGUI
      LOG.Info( "frmMain", "Load GUI Forms" );
      m_frmGui = new frmGui {
        AutoSize = true // Allow the GUIform to AutoSize (the one containing the FlowPanel)
      };
      m_frmGui.Resize += M_frmGui_Resize;
      m_frmGui.SizeChanged += M_frmGui_SizeChanged;

      this.AddOwnedForm( m_frmGui ); // we Own the GuiForm for management
                                     // Create a DispList 
      flp = new DispPanel( ) { Name = "flp" };
      m_frmGui.Controls.Add( flp );
      m_frmGui.Show( );
      // And Overlay the two Forms
      SynchGUI( );

#if DEBUG
      // DEBUG TESTS
      //flp.BackColor = Color.DarkGray; // show extent of the FlowPanel
#endif

      // SimConnect
      LOG.Info( "frmMain", "Load SimConnectAdapter" );
      SCAdapter = new SCClient( );
      SCAdapter.Connected += SCAdapter_Connected;
      SCAdapter.Establishing += SCAdapter_Establishing;
      SCAdapter.Disconnected += SCAdapter_Disconnected;

      // Setup the Shelf
      LOG.Info( "frmMain", "Load Shelf" );
      _Shelf = new FShelf.frmShelf( Program.Instance );
      _Shelf.FlightPlanLoadedByUser += M_shelf_FlightPlanLoadedByUser; // attach FPlan loading event
      UpdateShelfSettings( ); // needed for transition to new AppSettings concept

      // Setup the Camera
      LOG.Info( "frmMain", "Load Camera" );
      _Camera = new FCamControl.frmCameraV2( Program.Instance );
      UpdateCamSettings( ); // needed for transition to new AppSettings concept

      // Setup the Checklist Box
      LOG.Info( "frmMain", "Load Checklist Box" );
      _ChecklistBox = new FChecklistBox.frmChecklistBox( Program.Instance );
      UpdateCheckListBoxSettings( ); // needed for transition to new AppSettings concept

      LOG.Info( "frmMain", "Init Form Done" );
    }

    private void frmMain_Load( object sender, EventArgs e )
    {
      LOG.Info( "frmMain_Load", "Start" );
      // The FlowPanel in Design is not docked - do it here
      flp.Dock = DockStyle.Fill;
      flp.WrapContents = true; // Needs to wrap around for the FlowBreaks
      flp.AutoSize = true; // The FlowPanel is AutoSize too 

      // attach mouse handlers to move the Bar/Tile around
      flp.MouseDown += frmMain_MouseDown;
      flp.MouseUp += frmMain_MouseUp;
      flp.MouseMove += frmMain_MouseMove;

      // Main Window Props - major ones - the rest will be set in InitGUI()
      this.FormBorderStyle = FormBorderStyle.None; // no frame etc. to begin with - this may change during InitGUI()
      this.TopMost = true; // make sure we float on top
      UpdateColor( ); // Set the Background color of this form

      // File Access Check
      if (Dbg.Instance.AccessCheck( Folders.UserFilePath ) != AccessCheckResult.Success) {
        string msg = $"MyDocuments Folder Access Check Failed:\n{Dbg.Instance.AccessCheckResult}\n\n{Dbg.Instance.AccessCheckMessage}";
        MessageBox.Show( msg, "Access Check Failed", MessageBoxButtons.OK, MessageBoxIcon.Error );
      }
      CheckFacilityDB( );

      // attach a Callback for the SimClient
      SC.SimConnectClient.Instance.DataArrived += Instance_DataArrived;
      // Layout may need to update when the Aircraft changes (due to Engine Count)
      SC.SimConnectClient.Instance.AircraftChange += Instance_AircraftChange;

      // Get the controls the first time from Config
      InitGUI( );
      _Shelf.FlightPlanRef.Tracker.ResetWYP( ); // TODO check if needed

      LOG.Info( "frmMain_Load", "Connect to SimConnectAdapter" );
      // SimConnect
      SCAdapter.Connect( );
      // Init Landing Performance Tracker
      _ = FShelf.LandPerf.PerfTracker.Instance;

      // Pacer to connect and other repetitive chores
      timer1.Interval = 5000; // try to connect in 5 sec intervals
      timer1.Enabled = true;

      LOG.Info( "frmMain_Load", "End" );
    }

    // Layout may need to update when the Aircraft changes (due to Engine Count)
    private void Instance_AircraftChange( object sender, EventArgs e )
    {
      SynchGUISize( );
    }


    // fired when the Window Location has changed; also when starting the prog
    // Take care to capture only real user relocations
    private void frmMain_LocationChanged( object sender, EventArgs e )
    {
      if (!m_initDone) return; // bail out if in Init
      if (this.WindowState != FormWindowState.Normal) return;   // can only handle the normal Window State here
                                                                // can only handle Windows here, Bar and Tile is tied to the screen border
      if (!(HUD.Kind == GUI.Kind.Window || HUD.Kind == GUI.Kind.WindowBL)) return;

      m_config.UsedProfile.UpdateLocation( this.Location );
      // store new location per profile
      switch (m_config.CurrentProfile) {
        case DProfile.Profile_1: AppSettingsV2.Instance.Profile_1_Location = this.Location; break;
        case DProfile.Profile_2: AppSettingsV2.Instance.Profile_2_Location = this.Location; break;
        case DProfile.Profile_3: AppSettingsV2.Instance.Profile_3_Location = this.Location; break;
        case DProfile.Profile_4: AppSettingsV2.Instance.Profile_4_Location = this.Location; break;
        case DProfile.Profile_5: AppSettingsV2.Instance.Profile_5_Location = this.Location; break;
        case DProfile.Profile_6: AppSettingsV2.Instance.Profile_6_Location = this.Location; break;
        case DProfile.Profile_7: AppSettingsV2.Instance.Profile_7_Location = this.Location; break;
        case DProfile.Profile_8: AppSettingsV2.Instance.Profile_8_Location = this.Location; break;
        case DProfile.Profile_9: AppSettingsV2.Instance.Profile_9_Location = this.Location; break;
        case DProfile.Profile_10: AppSettingsV2.Instance.Profile_10_Location = this.Location; break;
        default: AppSettingsV2.Instance.Profile_1_Location = this.Location; break;
      }
      AppSettingsV2.Instance.Save( );
    }


    // Capture Resizing of the GUI Form (where the controls are loaded)
    private void M_frmGui_SizeChanged( object sender, EventArgs e )
    {
      SynchGUISize( );
    }

    // Capture Resizing of the GUI Form (where the controls are loaded)
    private void M_frmGui_Resize( object sender, EventArgs e )
    {
      //        SynchGUISize( );
    }

    // Capture and synch Movements of the Main Window
    private void frmMain_Move( object sender, EventArgs e )
    {
      SynchGUILocation( );
    }

    // Fired when about to Close, Cleanup
    private void frmMain_FormClosing( object sender, FormClosingEventArgs e )
    {
      LOG.Info( "frmMain_FormClosing", "Start" );

      // Properly close to handle the cleanup
      _Camera?.Close( );
      _ChecklistBox?.Close( );
      _Shelf?.Close( );

      // Save all Settings
      Configuration.SaveToSettings( m_config );
      // stop connecting tries
      timer1.Enabled = false;

      // Cleanup Sounds / Loops
      PingLib.Sounds.RemoveTempSounds( ); // Sounds and Loops share the same cleanup - need only one to call

      // Unhook Hotkeys
      SetupKeyboardHook( false, false );
      SetupInGameHook( false );

      // disconnect from Sim if needed
      if (SC.SimConnectClient.Instance.IsConnected) {
        // finalize a Recording if one is pending
        if (SC.SimConnectClient.Instance.FlightLogModule.LogMode != FSimClientIF.FlightLogMode.Off) {
          SC.SimConnectClient.Instance.FlightLogModule.LogMode = FSimClientIF.FlightLogMode.Off;
        }
        // closure
        SCAdapter.Disconnect( );// close the connection
                                //        SC.SimConnectClient.Instance.Disconnect( );
      }
      LOG.Info( "frmMain_FormClosing", "End" );

    }

    // Menu Exit event
    private void mExit_Click( object sender, EventArgs e )
    {
      this.Close( ); // just call the main Close
    }

    // Timer Event
    private void timer1_Tick( object sender, EventArgs e )
    {
      // SimConnect stuff
      SimConnectPacer( );

      //DEBUG
      SynchGUISize( ); // first time align the size

    }

    // fired when the user has loaded a flightplan
    private void M_shelf_FlightPlanLoadedByUser( object sender, EventArgs e )
    {
      // update from shelf
      HudBar.UpdateFlightplanReference( _Shelf.FlightPlanRef );
    }


    #region Units Menu

    // Update if the user has changed it
    private void mAltMetric_CheckedChanged( object sender, EventArgs e )
    {
      AppSettingsV2.Instance.Altitude_Metric = mAltMetric.Checked;
      AppSettingsV2.Instance.Save( );
      HUD?.SetAltitudeMetric( AppSettingsV2.Instance.Altitude_Metric );
    }

    // Update if the user has changed it
    private void mDistMetric_CheckedChanged( object sender, EventArgs e )
    {
      AppSettingsV2.Instance.Distance_Metric = mDistMetric.Checked;
      AppSettingsV2.Instance.Save( );
      HUD?.SetDistanceMetric( AppSettingsV2.Instance.Distance_Metric );
    }

    // Update if the user has changed it
    private void mShowUnits_CheckedChanged( object sender, EventArgs e )
    {
      AppSettingsV2.Instance.ShowUnits = mShowUnits.Checked;
      AppSettingsV2.Instance.Save( );
      HUD?.SetShowUnits( AppSettingsV2.Instance.ShowUnits );
    }

    #endregion

    #region Config Menu

    // Menu Config Event
    private void mConfig_Click( object sender, EventArgs e )
    {
      LOG.Info( $"mConfig_Click", "Start" );

      // don't handle timer while in Config
      timer1.Enabled = false;
      // hide the Shelf while in config
      _Shelf.Hide( );

      // Kill any Pings in Config - will restablish after getting back
      var muted = HudBar.PingLoop.Mute;
      HudBar.PingLoop.Mute = true;

      // Config must use the current environment 
      CFG.HudBarRef = HUD;
      CFG.ConfigCopy = m_config.Clone( ); // send only a copy to work with

      // Show and see if the user Accepts the changes
      if (CFG.ShowDialog( this ) == DialogResult.OK) {
        LOG.Info( $"mConfig_Click", "Dialog OK" );
        // Get changes done while Config was open
        m_config = CFG.ConfigCopy.Clone( );
        // Then save all configuration settings
        Configuration.SaveToSettings( m_config );

        // Restart the GUI 
        InitGUI( );
      }
      // Dialog Cancelled, nothing changed
      HudBar.PingLoop.Mute = muted;

      // reset out float above others each time we redo the GUI, could get lost when using Config
      this.TopMost = true;
      // pacer is finally back
      timer1.Enabled = true;

      LOG.Info( $"mConfig_Click", "End" );
    }

    #endregion

    #region Profile Selectors

    // Menu Profile Selections 1..10
    private void mP1_Click( object sender, EventArgs e )
    {
      m_config.SetProfile( DProfile.Profile_1 );
      InitGUI( );
    }

    private void mP2_Click( object sender, EventArgs e )
    {
      m_config.SetProfile( DProfile.Profile_2 );
      InitGUI( );
    }

    private void mP3_Click( object sender, EventArgs e )
    {
      m_config.SetProfile( DProfile.Profile_3 );
      InitGUI( );
    }

    private void mP4_Click( object sender, EventArgs e )
    {
      m_config.SetProfile( DProfile.Profile_4 );
      InitGUI( );
    }

    private void mP5_Click( object sender, EventArgs e )
    {
      m_config.SetProfile( DProfile.Profile_5 );
      InitGUI( );
    }

    private void mP6_Click( object sender, EventArgs e )
    {
      m_config.SetProfile( DProfile.Profile_6 );
      InitGUI( );
    }

    private void mP7_Click( object sender, EventArgs e )
    {
      m_config.SetProfile( DProfile.Profile_7 );
      InitGUI( );
    }

    private void mP8_Click( object sender, EventArgs e )
    {
      m_config.SetProfile( DProfile.Profile_8 );
      InitGUI( );
    }

    private void mP9_Click( object sender, EventArgs e )
    {
      m_config.SetProfile( DProfile.Profile_9 );
      InitGUI( );
    }

    private void mP10_Click( object sender, EventArgs e )
    {
      m_config.SetProfile( DProfile.Profile_10 );
      InitGUI( );
    }

    #endregion

    #region Appearance Selectors

    private void maBright_Click( object sender, EventArgs e )
    {
      Colorset = ColorSet.BrightSet;
      // save as setting
      AppSettingsV2.Instance.Appearance = (int)Colorset;
      AppSettingsV2.Instance.Save( );
    }

    private void maDimm_Click( object sender, EventArgs e )
    {
      Colorset = ColorSet.DimmedSet;
      // save as setting
      AppSettingsV2.Instance.Appearance = (int)Colorset;
      AppSettingsV2.Instance.Save( );
    }

    private void maDark_Click( object sender, EventArgs e )
    {
      Colorset = ColorSet.InverseSet;
      // save as setting
      AppSettingsV2.Instance.Appearance = (int)Colorset;
      AppSettingsV2.Instance.Save( );
    }

    private void mAppearance_DropDownOpening( object sender, EventArgs e )
    {
      // set the selected item as checked
      maBright.Checked = (Colorset == ColorSet.BrightSet);
      maDimm.Checked = (Colorset == ColorSet.DimmedSet);
      maDark.Checked = (Colorset == ColorSet.InverseSet);
    }

    #endregion

    #region Shelf Selector

    private void mShelf_Click( object sender, EventArgs e )
    {
      if (_Shelf == null) return; // sanity check 

      if (_Shelf.Visible) {
        _Shelf.TopMost = false;
        _Shelf.Hide( );
      }
      else {
        _Shelf.TopMost = true;
        _Shelf.Show( );
      }
    }

    #endregion

    #region Camera Selector

    private void mCamera_Click( object sender, EventArgs e )
    {
      // sanity check 
      if (_Camera == null) return;

      if (_Camera.Visible) {
        _Camera.TopMost = false;
        _Camera.Hide( );
      }
      else {
        _Camera.TopMost = true;
        _Camera.Show( );
      }
    }

    #endregion

    #region Backup Selector

    private void mManBackup_Click( object sender, EventArgs e )
    {
      if (SC.SimConnectClient.Instance.IsConnected) {
        SC.SimConnectClient.Instance.FltFileModule.RequestFlightBackup( );
      }
    }

    #endregion

    #region Landing Selector

    private void mManSaveLanding_Click( object sender, EventArgs e )
    {
      if (SC.SimConnectClient.Instance.IsConnected) {
        FShelf.LandPerf.PerfTracker.Instance.WriteLandingImage( );
      }
    }

    #endregion

    #region Checklist Box Selector

    private void mChecklistBox_Click( object sender, EventArgs e )
    {
      // sanity check
      if (_ChecklistBox == null) return;

      if (_ChecklistBox.Visible) {
        _ChecklistBox.TopMost = false;
        _ChecklistBox.Hide( );
      }
      else {
        _ChecklistBox.TopMost = true;
        _ChecklistBox.Show( );
      }
    }

    #endregion

    #region Mouse handlers for moving the Tile around

    private bool m_moving = false;
    private Point m_moveOffset = new Point( 0, 0 );

    private void frmMain_MouseDown( object sender, MouseEventArgs e )
    {
      if (!m_initDone) return; // bail out if in Init
      if (!(HUD.Kind == GUI.Kind.Tile || HUD.Kind == GUI.Kind.WindowBL)) return; // can only move Tile kind around here

      if ((e.Button & MouseButtons.Left) > 0) {
        m_moving = true;
        m_moveOffset = e.Location;
      }
    }

    private void frmMain_MouseMove( object sender, MouseEventArgs e )
    {
      if (!m_moving) return;
      if (!m_initDone) return; // bail out if in Init
      if (!(HUD.Kind == GUI.Kind.Tile || HUD.Kind == GUI.Kind.WindowBL)) return; // can only move Tile kind around here

      if ((e.Button & MouseButtons.Left) > 0) {
        if (HUD.Kind == GUI.Kind.WindowBL) {
          // free movement
          this.Location = new Point( this.Location.X + e.X - m_moveOffset.X, this.Location.Y + e.Y - m_moveOffset.Y );
        }
        else {
          // Tiles are bound to a border of the main screen
          switch (HUD.Placement) {
            case GUI.Placement.TopStack:
            case GUI.Placement.Top:
              this.Location = new Point( this.Location.X + e.X - m_moveOffset.X, this.Location.Y );
              break;
            case GUI.Placement.Bottom:
              this.Location = new Point( this.Location.X + e.X - m_moveOffset.X, this.Location.Y );
              break;
            case GUI.Placement.Left:
              this.Location = new Point( this.Location.X, this.Location.Y + e.Y - m_moveOffset.Y );
              break;
            case GUI.Placement.Right:
              this.Location = new Point( this.Location.X, this.Location.Y + e.Y - m_moveOffset.Y );
              break;
            default: break;
          }
          this.Location = TileBoundLocation( this.Location, this.Size ); // get into Tile Bounds
        }
      }
    }

    private void frmMain_MouseUp( object sender, MouseEventArgs e )
    {
      if (!m_moving) return;
      if (!m_initDone) return; // bail out if in Init
      if (!(HUD.Kind == GUI.Kind.Tile || HUD.Kind == GUI.Kind.WindowBL)) return; // can only move Tile kind around here

      if ((e.Button & MouseButtons.Left) > 0) {
        m_moving = false;
        m_config.UsedProfile.UpdateLocation( this.Location );
        Configuration.SaveToSettings( m_config );
      }
    }

    #endregion

    #region GUI

    private HudBar HUD = null; // THE HudBar Obj
    private bool _inLayout = false; // enclose and track Loading of DispItems

    // initialize the form, the labels and default values
    private void InitGUI( )
    {
      LOG.Info( "InitGUI", "Start" );

      var AS = AppSettingsV2.Instance;

      timer1.Enabled = false; // stop asynch Timer events
      m_initDone = false; // stop updating values while reconfiguring
      SynchGUIVisible( false ); // hide, else we see all kind of shaping

      // Set FLT Backup Mode
      LOG.Info( "InitGUI", "FlightPlanModule Setup" );
      SC.SimConnectClient.Instance.FltFileModule.Enabled = (FltFileModuleMode)AS.FltAutoSaveATC == FltFileModuleMode.AutoSave;
      SC.SimConnectClient.Instance.FltFileModule.ModuleMode = (FltFileModuleMode)AS.FltAutoSaveATC;

      LOG.Info( "InitGUI", "FlightLogModule Setup" );
      SC.SimConnectClient.Instance.FlightLogModule.Enabled = AS.FRecorder;
      LOG.Info( "InitGUI", "AirportMgr Reset" );
      AirportMgr.Reset( );

      // Update profile selection item names
      LOG.Info( "InitGUI", "Update profile selection" );
      foreach (DProfile pe in Enum.GetValues( typeof( DProfile ) )) {
        int index = (int)pe;
        m_profileMenu[index].Text = m_config.Profiles[pe].PName;
        m_profileMenu[index].Checked = false;
      }
      m_profileMenu[m_config.CurrentProfileIndex].Checked = true;
      mSelProfile.Text = m_config.UsedProfileName;
      LOG.Info( "InitGUI", $"Selected profile {mSelProfile.Text}" );

      // Set the Window Title
      this.Text = (string.IsNullOrEmpty( Program.Instance ) ? "Default" : Program.Instance) + $" HudBar: {m_config.UsedProfileName}          - by bm98ch";

      // create a catalog from Settings (serialized as item strings..)
      LOG.Info( "InitGUI", "Setup Hotkeys" );
      var _hotkeycat = new WinHotkeyCat( );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Show_Hide, m_config.HKShowHide );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_1, m_config.Profiles[DProfile.Profile_1].HKProfile );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_2, m_config.Profiles[DProfile.Profile_2].HKProfile );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_3, m_config.Profiles[DProfile.Profile_3].HKProfile );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_4, m_config.Profiles[DProfile.Profile_4].HKProfile );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_5, m_config.Profiles[DProfile.Profile_5].HKProfile );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_6, m_config.Profiles[DProfile.Profile_6].HKProfile );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_7, m_config.Profiles[DProfile.Profile_7].HKProfile );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_8, m_config.Profiles[DProfile.Profile_8].HKProfile );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_9, m_config.Profiles[DProfile.Profile_9].HKProfile );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_10, m_config.Profiles[DProfile.Profile_10].HKProfile );
      _hotkeycat.MaintainHotkeyString( Hotkeys.FlightBag, m_config.HKShelf );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Camera, m_config.HKCamera );
      _hotkeycat.MaintainHotkeyString( Hotkeys.ChecklistBox, m_config.HKChecklistBox );
      _hotkeycat.MaintainHotkeyString( Hotkeys.MoveBarToOtherWindow, "RShiftKey RControlKey Cancel" ); // not to configure (RCtrl-Shift-Break)
      foreach (var hk in _hotkeycat) {
        LOG.Info( "InitGUI", $"{hk.Key} - {hk.Value.AsString}" );
      }

      // Init Colors from Config
      LOG.Info( "InitGUI", "Setup Colors" );
      GUI_Colors.SetColorSet( ColorSet.BrightSet, m_config.UsedColorSet( ColorSet.BrightSet ) );
      GUI_Colors.SetColorSet( ColorSet.DimmedSet, m_config.UsedColorSet( ColorSet.DimmedSet ) );
      GUI_Colors.SetColorSet( ColorSet.InverseSet, m_config.UsedColorSet( ColorSet.InverseSet ) );

      // Add BG Image if needed
      string bgImageName = m_config.UsedProfile.BgImageName;
      if (!string.IsNullOrEmpty( bgImageName )) {
        if (File.Exists( bgImageName )) {
          Image img = null;
          try {
            // try to load and validate that it is an image to read..
            img = Image.FromFile( bgImageName );
            this.BackgroundImage = img;
            this.BackgroundImageLayout = ImageLayout.Stretch;
          }
          catch (Exception ex) {
            LOG.Error( "InitGUI", $"Loading Background image failed with exception\n{ex}" );
          }
        }
      }
      else {
        // disable BG image
        this.BackgroundImage = null;
      }
      // set Border
      m_frmGui.Padding = m_config.UsedProfile.BgImageBorder;

      // start the HudBar from scratch
      LOG.Info( "InitGUI", "Create HudBar" );
      HUD?.Dispose( ); // MUST ..
      HUD = new HudBar( lblProto, valueProto, value2Proto, signProto, _hotkeycat, m_config );

      // reread from config (change)
      LOG.Info( "InitGUI", "Reread Config changes" );
      SetupKeyboardHook( true, AS.KeyboardHook );
      SetupInGameHook( AS.InGameHook );

      // Prepare FLPanel to load controls
      // DON'T Suspend the Layout else the calculations below will not be valid, the form is invisible and no painting is done here
      LOG.Info( "InitGUI", $"Reload FlowPanel with Kind: {m_config.UsedProfile.Kind}, Placement: {HUD.Placement}" );

      // suspend intermediate Size change Events as they are immediately obsolete
      _inLayout = true;
      // Full Reload
      flp.ClearPanel( );
      // Load visible controls into the FLP
      HUD.LoadFLPanel( flp );

      _inLayout = false;

      // post proc - reset some properties and the location of the MainWindow
      //   A window is essentially a tile with border and will later be positioned at the last stored location
      //   A Bar or Tile is following along the edges of the primary screen
      LOG.Info( "InitGUI", "Post Processing" );
      // no border for most
      this.FormBorderStyle = FormBorderStyle.None;
      if (HUD.Kind == GUI.Kind.Window) this.FormBorderStyle = FormBorderStyle.FixedToolWindow; // apply the border if needed
      // can move a tile kind profile (but not a bar or window - has it's own window border anyway)
      this.Cursor = (m_config.UsedProfile.Kind == GUI.Kind.Tile || m_config.UsedProfile.Kind == GUI.Kind.WindowBL) ? Cursors.SizeAll : Cursors.Default;
      // set form opacity from Profile
      this.Opacity = m_config.UsedProfile.Opacity;
      // use Color Handler for the Form as well
      GUI_Colors.Register( this );
      UpdateColor( ); // apply initial setting 


      // init with the proposed location from profile (check within a virtual box)
      this.Location = new Point( 0, 0 ); // safe location
      if (dNetBm98.Utilities.IsOnScreen( m_config.UsedProfile.Location, new Size( 100, 100 ) )) {
        this.Location = m_config.UsedProfile.Location;
      }

      // realign all
      SynchGUI( );
      // Unhide when finished
      SynchGUIVisible( true );

      // Color the MSFS Label it if connected
      if (SC.SimConnectClient.Instance.IsConnected) {
        HUD.DispItem( LItem.MSFS ).ColorType.ItemForeColor = ColorType.cTxActive;
        HUD.DispItem( LItem.MSFS ).ColorType.ItemBackColor = ColorType.cInverse; // not cBG so we still can select it in Transp. Mode
        // Set Engines if already connected
        flp.SetEnginesVisible( SV.Get<int>( SItem.iG_Cfg_NumberOfEngines_num ) );
      }
      else {
        HUD.DispItem( LItem.MSFS ).ColorType.ItemForeColor = ColorType.cTxInfo;
        HUD.DispItem( LItem.MSFS ).ColorType.ItemBackColor = ColorType.cAlert;
      }

      m_initDone = true;
      timer1.Enabled = true; // and enable the pacer

      LOG.Info( "InitGUI", "End" );
    }


    /// <summary>
    /// Update the GUI values from Sim
    ///  In general GUI elements are only updated when checked and visible
    ///  Trackers and Meters are maintained independent of the View state (another profile may use them..)
    /// </summary>
    private void UpdateGUI( string dataRefName )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return; // sanity..
      if (!m_initDone) return; // cannot access items at this time

      var sec = DateTimeOffset.UtcNow.ToUnixTimeSeconds( );
      // maintain the Engine Visibility
      flp.SetEnginesVisible( SV.Get<int>( SItem.iG_Cfg_NumberOfEngines_num ) );
      // It is called prematurely when the Bar connects or a flight is loaded - reset if no acft is selected
      if (string.IsNullOrEmpty( SV.Get<string>( SItem.sG_Cfg_AcftConfigFile ) )) {
        // reset as long as there is no flight active
        AirportMgr.Reset( );
        _Shelf.FlightPlanRef.Tracker.ResetWYP( );
      }
      else {
        // update FP Ref  from shelf
        HudBar.UpdateFlightplanReference( _Shelf.FlightPlanRef );
        // The Bar has it's own logic for data updates
        HUD.UpdateGUI( dataRefName, sec );
        // update map airports if there are 
        if (AirportMgr.HasChanged) {
          if (AirportMgr.IsDepAvailable)
            _Shelf.DEP_Airport = AirportMgr.DepAirportICAO;
          if (AirportMgr.IsArrAvailable)
            _Shelf.ARR_Airport = AirportMgr.ArrAirportICAO;

          AirportMgr.Read( ); // reset changed flag
        }

      }
    }

    //  moves the bar/tile to the next screen
    private void MoveBarToNextScreen( )
    {
      if (m_config.UsedProfile.Kind == GUI.Kind.Window) return; // Window is not attached to monitor
      if (m_config.UsedProfile.Kind == GUI.Kind.WindowBL) return; // Window is not attached to monitor

      Screen[] screens = Screen.AllScreens;
      m_barScreenNumber++;
      m_barScreenNumber = (m_barScreenNumber >= screens.Length) ? 0 : m_barScreenNumber; // wrap around
      m_barScreen = screens[m_barScreenNumber];
      AppSettingsV2.Instance.ScreenNumber = m_barScreenNumber;
      AppSettingsV2.Instance.Save( );
      // Restart the GUI 
      InitGUI( );
    }

    #endregion

    #region SimConnectClient chores

    ColorType IColorType.ItemForeColor { get => throw new NotImplementedException( ); set => throw new NotImplementedException( ); }
    ColorType IColorType.ItemBackColor { get => throw new NotImplementedException( ); set => throw new NotImplementedException( ); }


    // establishing event
    private void SCAdapter_Establishing( object sender, EventArgs e )
    {
      LOG.Info( "SCAdapter_Establishing", "State change received" );

      HUD.DispItem( LItem.MSFS ).ColorType.ItemForeColor = ColorType.cTxInfo;
      HUD.DispItem( LItem.MSFS ).ColorType.ItemBackColor = ColorType.cInverse;
      //signalling the connection state on the topmost Bar Item
      HUD.DispItem( LItem.MSFS ).Label.ForeColor = Color.MediumPurple;

    }

    // connect event - ASYNC CALL, GUI changes need to be invoked on this form
    private void SCAdapter_Connected( object sender, EventArgs e )
    {
      LOG.Info( "SCAdapter_Connected", "State change received" );

      HUD.DispItem( LItem.MSFS ).ColorType.ItemForeColor = ColorType.cTxInfo;
      HUD.DispItem( LItem.MSFS ).ColorType.ItemBackColor = ColorType.cInverse;
      //signalling the connection state on the topmost Bar Item
      HUD.DispItem( LItem.MSFS ).Label.ForeColor = Color.LimeGreen;

      // enable game hooks if newly connected and desired
      SetupInGameHook( AppSettingsV2.Instance.InGameHook );
      // Set Engines 
      this.Invoke( (MethodInvoker)delegate { flp.SetEnginesVisible( SV.Get<int>( SItem.iG_Cfg_NumberOfEngines_num ) ); } );

      LOG.Info( $"SCAdapter_Connected", "Connected now" );
    }

    // disconnect event
    private void SCAdapter_Disconnected( object sender, EventArgs e )
    {
      LOG.Info( "SCAdapter_Disconnected", "State change received" );

      HUD.DispItem( LItem.MSFS ).ColorType.ItemForeColor = ColorType.cTxInfo;
      HUD.DispItem( LItem.MSFS ).ColorType.ItemBackColor = ColorType.cInverse;
      //signalling the connection state on the topmost Bar Item
      HUD.DispItem( LItem.MSFS ).Label.BackColor = Color.Red;

      // Disconnect from Input and SimConnect
      SetupInGameHook( false );
      flp.SetEnginesVisible( -1 ); // reset for the next attempt      

      LOG.Info( $"SCAdapter_Disconnected", "Disconnected now" );
    }


    /// <summary>
    /// fired from Sim for new Data
    /// Callback from SimConnect client signalling data arrival
    ///  Appart from subscriptions this is calles on a regular pace 
    /// </summary>
    private void Instance_DataArrived( object sender, FSimClientIF.ClientDataArrivedEventArgs e )
    {
      if (this.InvokeRequired) {
        this.Invoke( (MethodInvoker)delegate { UpdateGUI( e.DataRefName ); } );
      }
      else {
        UpdateGUI( e.DataRefName );
      }
    }

    /// <summary>
    /// SimConnect chores on a timer, mostly reconnecting and monitoring the connection status
    /// Intended to be called about every 5 seconds
    /// </summary>
    private void SimConnectPacer( )
    {
#if DEBUG
      //flp.SetEnginesVisible( 4 ); // DEBUG ONLY - show 2x2 layout
#endif

      if (SC.SimConnectClient.Instance.IsConnected) {
        // Voice is disabled when a new HUD is created, so enable if not yet done
        // The timer is enabled after InitGUI - so this one is always 5 sec later which should avoid some of the early talking..
        HudBar.VoiceEnabled = true;
        // Check and Resize at this pace to fit the contents in case some layout changes made it not fitting anymore
        // this is due to long texts or changes in the visibility of items coming in by the SimConnect Data processing in the DI items
        SynchGUISize( );
      }
    }

    #endregion

  }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors;

using FS20_HudBar.GUI.Templates.Base;
using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.Bar.Items;
using FS20_HudBar.Bar;
using FS20_HudBar.Config;
using System.Threading;

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
  public partial class frmMain : Form
  {
    // Handle of the Primary Screen to attach bar and tile
    private readonly Screen m_mainScreen;

    // This will be the GUI form
    private frmGui m_frmGui;
    private FlowLayoutPanel flp;

    // A HudBar standard ToolTip for the Button Helpers
    private ToolTip_Base m_toolTip = new ToolTip_Base();

    // The profiles
    private List<CProfile> m_profiles = new List<CProfile>();
    private int m_selProfile = 0;
    private ToolStripMenuItem[] m_profileMenu; // enable array access for the MenuItems

    // Our interaction hooks 
    private enum Hooks
    {
      Show_Hide=0, // toggle show/hide of the bar
      Profile_1, Profile_2, Profile_3, Profile_4, Profile_5, // Profile selection
      FlightBag,
    }

    // Handles the RawInput from HID Keyboards
    Win.HotkeyController _keyHook;

    // MSFS Input handlers
    private Dictionary<Hooks, SC.Input.InputHandler> _fsInputCat = new Dictionary<Hooks, SC.Input.InputHandler>();

    // The Flightbag
    private Shelf.frmShelf m_shelf;

    // Configuration Dialog
    private readonly frmConfig CFG = new frmConfig( );

    // need to stop processing while reconfiguring the bar
    private bool m_initDone = false;

    /// <summary>
    /// Checks if a rectangle is visible on any screen
    /// </summary>
    /// <param name="formRect"></param>
    /// <returns>True if visible</returns>
    private static bool IsOnScreen( Rectangle formRect )
    {
      formRect.Inflate( -20, -20 ); // have to make it a bit smaller as the rectangle can be slightly out of screen
      Screen[] screens = Screen.AllScreens;
      foreach ( Screen screen in screens ) {
        if ( screen.WorkingArea.Contains( formRect ) ) {
          return true;
        }
      }
      return false;
    }

    #region Synch GUI Forms

    // synch the two forms for Size
    private void SynchGUISize( )
    {
      m_frmGui.Size = this.ClientSize;
    }
    // synch the two forms for Location
    private void SynchGUILocation( )
    {
      m_frmGui.Location = this.PointToScreen( this.ClientRectangle.Location ); // in Windowed mode we need to get it from the client rect
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
      if ( !SC.SimConnectClient.Instance.IsConnected ) return;

      if ( enabled ) {
        if ( _fsInputCat.Count <= 0 ) {
          // reinitiate the hooks
          _fsInputCat.Add( Hooks.Show_Hide, SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_01 ) );
          _fsInputCat[Hooks.Show_Hide].InputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hooks.Profile_1, SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_02 ) );
          _fsInputCat[Hooks.Profile_1].InputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hooks.Profile_2, SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_03 ) );
          _fsInputCat[Hooks.Profile_2].InputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hooks.Profile_3, SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_04 ) );
          _fsInputCat[Hooks.Profile_3].InputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hooks.Profile_4, SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_05 ) );
          _fsInputCat[Hooks.Profile_4].InputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hooks.Profile_5, SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_06 ) );
          _fsInputCat[Hooks.Profile_5].InputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hooks.FlightBag, SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_07 ) );
          _fsInputCat[Hooks.FlightBag].InputArrived += FSInput_InputArrived;
        }
      }
      else {
        // disable all callbacks
        foreach ( var fi in _fsInputCat ) {
          // this shall never fail..
          try {
            fi.Value.InputArrived -= FSInput_InputArrived;
          }
          catch { }
        }
        // remove all cat entries
        _fsInputCat.Clear( );
      }
    }

    // Receive commands from FSim
    private void FSInput_InputArrived( object sender, SC.Input.FSInputEventArgs e )
    {
      // sanity checks
      if ( !SC.SimConnectClient.Instance.IsConnected ) return; // catch odd cases of disruption
      if ( _fsInputCat.Count <= 0 ) return;

      // _fsInputCat should be valid when this event fires..
      if ( e.ActionName == _fsInputCat[Hooks.Show_Hide].Inputname ) SynchGUIVisible( !this.Visible );
      else if ( e.ActionName == _fsInputCat[Hooks.Profile_1].Inputname ) mP1_Click( null, new EventArgs( ) );
      else if ( e.ActionName == _fsInputCat[Hooks.Profile_2].Inputname ) mP2_Click( null, new EventArgs( ) );
      else if ( e.ActionName == _fsInputCat[Hooks.Profile_3].Inputname ) mP3_Click( null, new EventArgs( ) );
      else if ( e.ActionName == _fsInputCat[Hooks.Profile_4].Inputname ) mP4_Click( null, new EventArgs( ) );
      else if ( e.ActionName == _fsInputCat[Hooks.Profile_5].Inputname ) mP5_Click( null, new EventArgs( ) );
      else if ( e.ActionName == _fsInputCat[Hooks.FlightBag].Inputname ) mShelf_Click( null, new EventArgs( ) );
    }

    /// <summary>
    /// returns a crlf formatted string of the hooks and their MSFS Key items
    /// </summary>
    /// <returns></returns>
    internal string InGameHints( )
    {
      string ret = "";
      foreach ( var hi in _fsInputCat ) {
        ret += $"{hi.Key} -> {hi.Value.InputActionString}\n";
      }
      if ( string.IsNullOrEmpty( ret ) ) {
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
    private void SetupKeyboardHook( bool enabled )
    {
      if ( enabled ) {
        if ( HUD == null ) return; // no HUD so far - ignore this one

        if ( _keyHook == null ) {
          _keyHook = new Win.HotkeyController( Handle );
        }
        _keyHook.KeyHandling( false ); // disable momentarily
        // reload the bindings
        _keyHook.RemoveAllKeys( );
        if ( HUD.Hotkeys.ContainsKey( Hotkeys.Show_Hide ) ) _keyHook.AddKey( HUD.Hotkeys[Hotkeys.Show_Hide], Hooks.Show_Hide.ToString( ), OnHookKey );
        if ( HUD.Hotkeys.ContainsKey( Hotkeys.Profile_1 ) ) _keyHook.AddKey( HUD.Hotkeys[Hotkeys.Profile_1], Hooks.Profile_1.ToString( ), OnHookKey );
        if ( HUD.Hotkeys.ContainsKey( Hotkeys.Profile_2 ) ) _keyHook.AddKey( HUD.Hotkeys[Hotkeys.Profile_2], Hooks.Profile_2.ToString( ), OnHookKey );
        if ( HUD.Hotkeys.ContainsKey( Hotkeys.Profile_3 ) ) _keyHook.AddKey( HUD.Hotkeys[Hotkeys.Profile_3], Hooks.Profile_3.ToString( ), OnHookKey );
        if ( HUD.Hotkeys.ContainsKey( Hotkeys.Profile_4 ) ) _keyHook.AddKey( HUD.Hotkeys[Hotkeys.Profile_4], Hooks.Profile_4.ToString( ), OnHookKey );
        if ( HUD.Hotkeys.ContainsKey( Hotkeys.Profile_5 ) ) _keyHook.AddKey( HUD.Hotkeys[Hotkeys.Profile_5], Hooks.Profile_5.ToString( ), OnHookKey );
        if ( HUD.Hotkeys.ContainsKey( Hotkeys.FlightBag ) ) _keyHook.AddKey( HUD.Hotkeys[Hotkeys.FlightBag], Hooks.FlightBag.ToString( ), OnHookKey );

        _keyHook.KeyHandling( true );
      }
      else {
        // disable - we cannot unhook the RawInputLib / not currently used anyway 
        _keyHook?.KeyHandling( false );
        _keyHook?.RemoveAllKeys( );
        _keyHook?.Dispose( );
        _keyHook = null;
      }
    }

    // Callback - handles the Keyboard Hooks
    private void OnHookKey( string tag )
    {
      if ( tag == Hooks.Show_Hide.ToString( ) ) SynchGUIVisible( !this.Visible );
      else if ( tag == Hooks.Profile_1.ToString( ) ) mP1_Click( null, new EventArgs( ) );
      else if ( tag == Hooks.Profile_2.ToString( ) ) mP2_Click( null, new EventArgs( ) );
      else if ( tag == Hooks.Profile_3.ToString( ) ) mP3_Click( null, new EventArgs( ) );
      else if ( tag == Hooks.Profile_4.ToString( ) ) mP4_Click( null, new EventArgs( ) );
      else if ( tag == Hooks.Profile_5.ToString( ) ) mP5_Click( null, new EventArgs( ) );
      else if ( tag == Hooks.FlightBag.ToString( ) ) mShelf_Click( null, new EventArgs( ) );
    }

    #endregion

    /// <summary>
    /// Main Form Init
    /// </summary>
    public frmMain( )
    {
      InitializeComponent( );

      // Load all from AppSettings
      AppSettings.Instance.Reload( );

      m_profiles.Add( new CProfile( 1, AppSettings.Instance.Profile_1_Name,
                                       AppSettings.Instance.Profile_1, AppSettings.Instance.FlowBreak_1, AppSettings.Instance.Sequence_1,
                                       AppSettings.Instance.Profile_1_FontSize, AppSettings.Instance.Profile_1_Placement,
                                       AppSettings.Instance.Profile_1_Kind, AppSettings.Instance.Profile_1_Location,
                                       AppSettings.Instance.Profile_1_Condensed,
                                       AppSettings.Instance.Profile_1_Trans ) );

      m_profiles.Add( new CProfile( 2, AppSettings.Instance.Profile_2_Name,
                                       AppSettings.Instance.Profile_2, AppSettings.Instance.FlowBreak_2, AppSettings.Instance.Sequence_2,
                                       AppSettings.Instance.Profile_2_FontSize, AppSettings.Instance.Profile_2_Placement,
                                       AppSettings.Instance.Profile_2_Kind, AppSettings.Instance.Profile_2_Location,
                                       AppSettings.Instance.Profile_2_Condensed,
                                       AppSettings.Instance.Profile_2_Trans ) );

      m_profiles.Add( new CProfile( 3, AppSettings.Instance.Profile_3_Name,
                                       AppSettings.Instance.Profile_3, AppSettings.Instance.FlowBreak_3, AppSettings.Instance.Sequence_3,
                                       AppSettings.Instance.Profile_3_FontSize, AppSettings.Instance.Profile_3_Placement,
                                       AppSettings.Instance.Profile_3_Kind, AppSettings.Instance.Profile_3_Location,
                                       AppSettings.Instance.Profile_3_Condensed,
                                       AppSettings.Instance.Profile_3_Trans ) );

      m_profiles.Add( new CProfile( 4, AppSettings.Instance.Profile_4_Name,
                                       AppSettings.Instance.Profile_4, AppSettings.Instance.FlowBreak_4, AppSettings.Instance.Sequence_4,
                                       AppSettings.Instance.Profile_4_FontSize, AppSettings.Instance.Profile_4_Placement,
                                       AppSettings.Instance.Profile_4_Kind, AppSettings.Instance.Profile_4_Location,
                                       AppSettings.Instance.Profile_4_Condensed,
                                       AppSettings.Instance.Profile_4_Trans ) );

      m_profiles.Add( new CProfile( 5, AppSettings.Instance.Profile_5_Name,
                                       AppSettings.Instance.Profile_5, AppSettings.Instance.FlowBreak_5, AppSettings.Instance.Sequence_5,
                                       AppSettings.Instance.Profile_5_FontSize, AppSettings.Instance.Profile_5_Placement,
                                       AppSettings.Instance.Profile_5_Kind, AppSettings.Instance.Profile_5_Location,
                                       AppSettings.Instance.Profile_5_Condensed,
                                       AppSettings.Instance.Profile_5_Trans ) );

      m_selProfile = AppSettings.Instance.SelProfile;
      mSelProfile.Text = m_profiles[m_selProfile].PName;
      // collect the Menus for the profiles
      m_profileMenu = new ToolStripMenuItem[] { mP1, mP2, mP3, mP4, mP5 };

      Colorset = ToColorSet( AppSettings.Instance.Appearance );

      // ShowUnits and Opacity are set via HUD in InitGUI
      m_frmGui = new frmGui( );
      this.AddOwnedForm( m_frmGui );
      m_frmGui.Show( );
      SynchGUI( );
      flp = m_frmGui.flp;
      /* DEBUG ONLY
      flp.Layout += Flp_Layout;
      flp.SizeChanged += Flp_SizeChanged;
      */

      // Setup the Shelf and put it somewhere we can see it (either last location or default)
      m_shelf = new Shelf.frmShelf {
        Size = AppSettings.Instance.ShelfSize,
        Location = AppSettings.Instance.ShelfLocation
      };
      if ( !IsOnScreen( new Rectangle( m_shelf.Location, m_shelf.Size ) ) ) {
        m_shelf.Location = new Point( 100, 100 );
      }
      //  set from Saved default - if not found, set our default
      if ( !m_shelf.SetShelfFolder( AppSettings.Instance.ShelfFolder ) ) {
        m_shelf.SetShelfFolder( @".\DemoBag" ); // the one in our Deployment
      }

      // Find and hold the Primary Screen
      Screen[] screens = Screen.AllScreens;
      m_mainScreen = screens[0];
      // now get the Primary one
      foreach ( Screen screen in screens ) {
        if ( screen.Primary ) {
          m_mainScreen = screen;
        }
      }
    }

    /* DEBUG ONLY
    int lct = 0;
    int sct = 0;

    private void Flp_SizeChanged( object sender, EventArgs e )
    {
      sct++;
    }

    private void Flp_Layout( object sender, LayoutEventArgs e )
    {
      lct++;
    }
    */
    private void frmMain_Load( object sender, EventArgs e )
    {
      // prepare the GUI On Form Load

      // The FlowPanel in Design is not docked - do it here
      flp.Dock = DockStyle.Fill;
      // flp.BorderStyle = BorderStyle.FixedSingle; // DEBUG to see where the FLPanel is
      flp.WrapContents = true; // Needs to wrap around
                               // attach mouse handlers
      flp.MouseDown += frmMain_MouseDown;
      flp.MouseUp += frmMain_MouseUp;
      flp.MouseMove += frmMain_MouseMove;

      // Window Props - major ones - the rest will be set in InitGUI()
      this.FormBorderStyle = FormBorderStyle.None; // no frame etc. to begin with
      this.TopMost = true; // make sure we float on top
      this.BackColor = c_WinBG;

      // Get the controls
      InitGUI( );
      WPTracker.Reset( );

      // attach a Callback for the SimClient
      SC.SimConnectClient.Instance.DataArrived += Instance_DataArrived;
      SC.SimConnectClient.Instance.FlightPlanModule.Enabled = false; // start disabled, will be re-checked in InitGUI

      // Pacer to connect and may be other chores
      timer1.Interval = 5000; // try to connect in 5 sec intervals
      timer1.Enabled = true;
    }


    // fired when the Window Location has changed; also when starting the prog
    // Take care to capture only real user relocations
    private void frmMain_LocationChanged( object sender, EventArgs e )
    {
      if ( !m_initDone ) return; // bail out if in Init
      if ( this.WindowState != FormWindowState.Normal ) return;   // can only handle the normal Window State here
      if ( !( HUD.Kind == GUI.Kind.Window || HUD.Kind == GUI.Kind.WindowBL ) ) return; // can only handle Window here

      HUD.Profile.UpdateLocation( this.Location );
      // store new location per profile
      switch ( m_selProfile ) {
        case 0: AppSettings.Instance.Profile_1_Location = this.Location; break;
        case 1: AppSettings.Instance.Profile_2_Location = this.Location; break;
        case 2: AppSettings.Instance.Profile_3_Location = this.Location; break;
        case 3: AppSettings.Instance.Profile_4_Location = this.Location; break;
        case 4: AppSettings.Instance.Profile_5_Location = this.Location; break;
        default: AppSettings.Instance.Profile_1_Location = this.Location; break;
      }
      AppSettings.Instance.Save( );
    }

    // Get the GUI form resized too
    private void frmMain_Resize( object sender, EventArgs e )
    {
      SynchGUISize( );
    }

    // Get the GUI form moved too
    private void frmMain_Move( object sender, EventArgs e )
    {
      SynchGUILocation( );
    }

    // Fired when about to Close
    private void frmMain_FormClosing( object sender, FormClosingEventArgs e )
    {
      m_shelf?.Hide( ); // Hide the Flight Bag

      // Save all Settings
      AppSettings.Instance.SelProfile = m_selProfile;
      AppSettings.Instance.Save( );
      // stop connecting tries
      timer1.Enabled = false;

      // Unhook Hotkeys
      SetupKeyboardHook( false );
      SetupInGameHook( false );

      // disconnect from Sim if needed
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        // finalize a Recording if one is pending
        if ( SC.SimConnectClient.Instance.FlightLogModule.LogMode != FSimClientIF.FlightLogMode.Off ) {
          SC.SimConnectClient.Instance.FlightLogModule.LogMode = FSimClientIF.FlightLogMode.Off;
        }
        // closure
        SC.SimConnectClient.Instance.Disconnect( );
      }
    }

    // Menu Exit event
    private void mExit_Click( object sender, EventArgs e )
    {
      this.Close( ); // just call the main Close
    }

    #region Config Menu

    // Menu Config Event
    private void mConfig_Click( object sender, EventArgs e )
    {
      // don't handle timer while in Config
      timer1.Enabled = false;
      // hide the Shelf while in config
      m_shelf.Hide( );

      // Config must use the current environment 
      CFG.HudBarRef = HUD;
      CFG.ProfilesRef = m_profiles;
      CFG.SelectedProfile = m_selProfile;

      // Kill any Pings in Config - will restablish after getting back
      var muted = HudBar.PingLoop.Mute;
      HudBar.PingLoop.Mute = true;

      // Show and see if the user Accepts the changes
      if ( CFG.ShowDialog( this ) == DialogResult.OK ) {
        // Save all configuration properties
        AppSettings.Instance.ShowUnits = HUD.ShowUnits;
        AppSettings.Instance.FRecorder = HUD.FlightRecorder;

        AppSettings.Instance.HKShowHide = HUD.Hotkeys.HotkeyString( Hotkeys.Show_Hide );
        AppSettings.Instance.HKProfile1 = HUD.Hotkeys.HotkeyString( Hotkeys.Profile_1 );
        AppSettings.Instance.HKProfile2 = HUD.Hotkeys.HotkeyString( Hotkeys.Profile_2 );
        AppSettings.Instance.HKProfile3 = HUD.Hotkeys.HotkeyString( Hotkeys.Profile_3 );
        AppSettings.Instance.HKProfile4 = HUD.Hotkeys.HotkeyString( Hotkeys.Profile_4 );
        AppSettings.Instance.HKProfile5 = HUD.Hotkeys.HotkeyString( Hotkeys.Profile_5 );
        AppSettings.Instance.HKShelf = HUD.Hotkeys.HotkeyString( Hotkeys.FlightBag );
        AppSettings.Instance.KeyboardHook = HUD.KeyboardHook;
        AppSettings.Instance.InGameHook = HUD.InGameHook;

        AppSettings.Instance.ShelfFolder = HUD.ShelfFolder;

        AppSettings.Instance.FltAutoSaveATC = (int)HUD.FltAutoSave;
        AppSettings.Instance.VoiceName = HUD.VoiceName;

        AppSettings.Instance.SelProfile = m_selProfile;
        // All Profiles
        int pIndex = 0; // use an index avoiding copy and paste mishaps...
        AppSettings.Instance.Profile_1_Name = m_profiles[pIndex].PName;
        AppSettings.Instance.Profile_1 = m_profiles[pIndex].ProfileString( );
        AppSettings.Instance.FlowBreak_1 = m_profiles[pIndex].FlowBreakString( );
        AppSettings.Instance.Sequence_1 = m_profiles[pIndex].ItemPosString( );
        AppSettings.Instance.Profile_1_FontSize = (int)m_profiles[pIndex].FontSize;
        AppSettings.Instance.Profile_1_Placement = (int)m_profiles[pIndex].Placement;
        AppSettings.Instance.Profile_1_Kind = (int)m_profiles[pIndex].Kind;
        AppSettings.Instance.Profile_1_Condensed = m_profiles[pIndex].Condensed;
        AppSettings.Instance.Profile_1_Trans = (int)m_profiles[pIndex].Transparency;
        pIndex++;
        AppSettings.Instance.Profile_2_Name = m_profiles[pIndex].PName;
        AppSettings.Instance.Profile_2 = m_profiles[pIndex].ProfileString( );
        AppSettings.Instance.FlowBreak_2 = m_profiles[pIndex].FlowBreakString( );
        AppSettings.Instance.Sequence_2 = m_profiles[pIndex].ItemPosString( );
        AppSettings.Instance.Profile_2_FontSize = (int)m_profiles[pIndex].FontSize;
        AppSettings.Instance.Profile_2_Placement = (int)m_profiles[pIndex].Placement;
        AppSettings.Instance.Profile_2_Kind = (int)m_profiles[pIndex].Kind;
        AppSettings.Instance.Profile_2_Condensed = m_profiles[pIndex].Condensed;
        AppSettings.Instance.Profile_2_Trans = (int)m_profiles[pIndex].Transparency;
        pIndex++;
        AppSettings.Instance.Profile_3_Name = m_profiles[pIndex].PName;
        AppSettings.Instance.Profile_3 = m_profiles[pIndex].ProfileString( );
        AppSettings.Instance.FlowBreak_3 = m_profiles[pIndex].FlowBreakString( );
        AppSettings.Instance.Sequence_3 = m_profiles[pIndex].ItemPosString( );
        AppSettings.Instance.Profile_3_FontSize = (int)m_profiles[pIndex].FontSize;
        AppSettings.Instance.Profile_3_Placement = (int)m_profiles[pIndex].Placement;
        AppSettings.Instance.Profile_3_Kind = (int)m_profiles[pIndex].Kind;
        AppSettings.Instance.Profile_3_Condensed = m_profiles[pIndex].Condensed;
        AppSettings.Instance.Profile_3_Trans = (int)m_profiles[pIndex].Transparency;
        pIndex++;
        AppSettings.Instance.Profile_4_Name = m_profiles[pIndex].PName;
        AppSettings.Instance.Profile_4 = m_profiles[pIndex].ProfileString( );
        AppSettings.Instance.FlowBreak_4 = m_profiles[pIndex].FlowBreakString( );
        AppSettings.Instance.Sequence_4 = m_profiles[pIndex].ItemPosString( );
        AppSettings.Instance.Profile_4_FontSize = (int)m_profiles[pIndex].FontSize;
        AppSettings.Instance.Profile_4_Placement = (int)m_profiles[pIndex].Placement;
        AppSettings.Instance.Profile_4_Kind = (int)m_profiles[pIndex].Kind;
        AppSettings.Instance.Profile_4_Condensed = m_profiles[pIndex].Condensed;
        AppSettings.Instance.Profile_4_Trans = (int)m_profiles[pIndex].Transparency;
        pIndex++;
        AppSettings.Instance.Profile_5_Name = m_profiles[pIndex].PName;
        AppSettings.Instance.Profile_5 = m_profiles[pIndex].ProfileString( );
        AppSettings.Instance.FlowBreak_5 = m_profiles[pIndex].FlowBreakString( );
        AppSettings.Instance.Sequence_5 = m_profiles[pIndex].ItemPosString( );
        AppSettings.Instance.Profile_5_FontSize = (int)m_profiles[pIndex].FontSize;
        AppSettings.Instance.Profile_5_Placement = (int)m_profiles[pIndex].Placement;
        AppSettings.Instance.Profile_5_Kind = (int)m_profiles[pIndex].Kind;
        AppSettings.Instance.Profile_5_Condensed = m_profiles[pIndex].Condensed;
        AppSettings.Instance.Profile_5_Trans = (int)m_profiles[pIndex].Transparency;

        // Finally Save
        AppSettings.Instance.Save( );

        // Restart the GUI 
        InitGUI( ); // redraw changes
      }
      // Dialog Cancelled, nothing changed
      HudBar.PingLoop.Mute = muted;

      // reset out float above others each time we redo the GUI, could get lost when using Config
      this.TopMost = true;
      // pacer is finally back
      timer1.Enabled = true;
    }

    #endregion

    #region Profile Selectors

    // Menu Profile Selections 1..5
    private void mP1_Click( object sender, EventArgs e )
    {
      m_selProfile = 0;
      InitGUI( );
    }

    private void mP2_Click( object sender, EventArgs e )
    {
      m_selProfile = 1;
      InitGUI( );
    }

    private void mP3_Click( object sender, EventArgs e )
    {
      m_selProfile = 2;
      InitGUI( );
    }

    private void mP4_Click( object sender, EventArgs e )
    {
      m_selProfile = 3;
      InitGUI( );
    }

    private void mP5_Click( object sender, EventArgs e )
    {
      m_selProfile = 4;
      InitGUI( );
    }

    #endregion

    #region Appearance Selectors

    private void maBright_Click( object sender, EventArgs e )
    {
      Colorset = ColorSet.BrightSet;
      // save as setting
      AppSettings.Instance.Appearance = (int)Colorset;
      AppSettings.Instance.Save( );
    }

    private void maDimm_Click( object sender, EventArgs e )
    {
      Colorset = ColorSet.DimmedSet;
      // save as setting
      AppSettings.Instance.Appearance = (int)Colorset;
      AppSettings.Instance.Save( );
    }

    private void maDark_Click( object sender, EventArgs e )
    {
      Colorset = ColorSet.DarkSet;
      // save as setting
      AppSettings.Instance.Appearance = (int)Colorset;
      AppSettings.Instance.Save( );
    }

    private void mAppearance_DropDownOpening( object sender, EventArgs e )
    {
      // set the selected item as checked
      maBright.Checked = ( Colorset == ColorSet.BrightSet );
      maDimm.Checked = ( Colorset == ColorSet.DimmedSet );
      maDark.Checked = ( Colorset == ColorSet.DarkSet );
    }

    #endregion

    #region Shelf Selector
    private void mShelf_Click( object sender, EventArgs e )
    {
      if ( m_shelf == null ) return; // sanity check 

      if ( m_shelf.Visible ) {
        m_shelf.Hide( );
      }
      else {
        m_shelf.TopMost = true;
        m_shelf.Show( );
      }
    }

    #endregion

    #region Mouse handlers for moving the Tile around

    private bool m_moving = false;
    private Point m_moveOffset = new Point(0,0);

    private void frmMain_MouseDown( object sender, MouseEventArgs e )
    {
      if ( !m_initDone ) return; // bail out if in Init
      if ( !( HUD.Kind == GUI.Kind.Tile || HUD.Kind == GUI.Kind.WindowBL ) ) return; // can only move Tile kind around here

      m_moving = true;
      m_moveOffset = e.Location;
    }

    private void frmMain_MouseMove( object sender, MouseEventArgs e )
    {
      if ( !m_initDone ) return; // bail out if in Init
      if ( !( HUD.Kind == GUI.Kind.Tile || HUD.Kind == GUI.Kind.WindowBL ) ) return; // can only move Tile kind around here
      if ( !m_moving ) return;

      if ( HUD.Kind == GUI.Kind.WindowBL ) {
        // free movement
        this.Location = new Point( this.Location.X + e.X - m_moveOffset.X, this.Location.Y + e.Y - m_moveOffset.Y );
      }
      else {
        // Tiles are bound to a border
        switch ( HUD.Placement ) {
          case GUI.Placement.Bottom:
            this.Location = new Point( this.Location.X + e.X - m_moveOffset.X, this.Location.Y );
            break;
          case GUI.Placement.Left:
            this.Location = new Point( this.Location.X, this.Location.Y + e.Y - m_moveOffset.Y );
            break;
          case GUI.Placement.Right:
            this.Location = new Point( this.Location.X, this.Location.Y + e.Y - m_moveOffset.Y );
            break;
          case GUI.Placement.Top:
            this.Location = new Point( this.Location.X + e.X - m_moveOffset.X, this.Location.Y );
            break;
          default: break;
        }
      }
    }

    private void frmMain_MouseUp( object sender, MouseEventArgs e )
    {
      if ( !m_initDone ) return; // bail out if in Init
      if ( !( HUD.Kind == GUI.Kind.Tile || HUD.Kind == GUI.Kind.WindowBL ) ) return; // can only move Tile kind around here
      if ( !m_moving ) return;

      m_moving = false;
      HUD.Profile.UpdateLocation( this.Location );
      // store new location per profile
      switch ( m_selProfile ) {
        case 0: AppSettings.Instance.Profile_1_Location = this.Location; break;
        case 1: AppSettings.Instance.Profile_2_Location = this.Location; break;
        case 2: AppSettings.Instance.Profile_3_Location = this.Location; break;
        case 3: AppSettings.Instance.Profile_4_Location = this.Location; break;
        case 4: AppSettings.Instance.Profile_5_Location = this.Location; break;
        default: AppSettings.Instance.Profile_1_Location = this.Location; break;
      }
      AppSettings.Instance.Save( );

    }

    #endregion

    #region FSIm Data Callback Handler

    // fired from Sim for new Data
    private void Instance_DataArrived( object sender, FSimClientIF.ClientDataArrivedEventArgs e )
    {
      m_awaitingEvent = false; // confirm we've got events
      UpdateGUI( e.DataRefName );
    }

    #endregion

    #region GUI

    private HudBar HUD = null;

    // initialize the form, the labels and default values
    private void InitGUI( )
    {
      timer1.Enabled = false; // stop asynch events
      m_initDone = false; // stop updating values while reconfiguring
      SynchGUIVisible( false ); // hide, else we see all kind of shaping

      SC.SimConnectClient.Instance.FlightPlanModule.ModuleMode = (FSimClientIF.FlightPlanMode)AppSettings.Instance.FltAutoSaveATC;
      SC.SimConnectClient.Instance.FlightPlanModule.Enabled = ( SC.SimConnectClient.Instance.FlightPlanModule.ModuleMode != FSimClientIF.FlightPlanMode.Disabled );
      SC.SimConnectClient.Instance.FlightLogModule.Enabled = AppSettings.Instance.FRecorder;
      AirportMgr.Reset( );

      // Update profile selection items
      for ( int i = 0; i < CProfile.c_numProfiles; i++ ) {
        m_profileMenu[i].Text = m_profiles[i].PName;
        m_profileMenu[i].Checked = false;
      }
      m_profileMenu[m_selProfile].Checked = true;
      mSelProfile.Text = m_profiles[m_selProfile].PName;
      // Set the Window Title
      this.Text = ( string.IsNullOrEmpty( Program.Instance ) ? "Default" : Program.Instance ) + $" HudBar: {m_profiles[m_selProfile].PName}          - by bm98ch";

      // create a catalog from Settings (serialized as item strings..)
      var _hotkeycat = new WinHotkeyCat();
      _hotkeycat.AddHotkeyString( Hotkeys.Show_Hide, AppSettings.Instance.HKShowHide );
      _hotkeycat.AddHotkeyString( Hotkeys.Profile_1, AppSettings.Instance.HKProfile1 );
      _hotkeycat.AddHotkeyString( Hotkeys.Profile_2, AppSettings.Instance.HKProfile2 );
      _hotkeycat.AddHotkeyString( Hotkeys.Profile_3, AppSettings.Instance.HKProfile3 );
      _hotkeycat.AddHotkeyString( Hotkeys.Profile_4, AppSettings.Instance.HKProfile4 );
      _hotkeycat.AddHotkeyString( Hotkeys.Profile_5, AppSettings.Instance.HKProfile5 );
      _hotkeycat.AddHotkeyString( Hotkeys.FlightBag, AppSettings.Instance.HKShelf );

      // start from scratch
      HUD = new HudBar( lblProto, valueProto, value2Proto, signProto,
                          AppSettings.Instance.ShowUnits, AppSettings.Instance.KeyboardHook, AppSettings.Instance.InGameHook, _hotkeycat,
                          AppSettings.Instance.FltAutoSaveATC, AppSettings.Instance.ShelfFolder,
                          m_profiles[m_selProfile], AppSettings.Instance.VoiceName, AppSettings.Instance.FRecorder );

      // reread after config change
      SetupKeyboardHook( AppSettings.Instance.KeyboardHook );
      SetupInGameHook( AppSettings.Instance.InGameHook );
      m_shelf?.SetShelfFolder( AppSettings.Instance.ShelfFolder );

      // prepare to create the content as bar or tile (may be switch to Window later if needed)
      this.FormBorderStyle = FormBorderStyle.None; // no frame etc.
                                                   // Prepare FLPanel to load controls
      flp.Controls.Clear( ); // reload
      // DON'T Suspend the Layout else the calculations below will not be valid, the form is invisible and no painting is done here
      // release dock to allow the bar to autosize
      flp.Dock = DockStyle.None;
      flp.AutoSize = true;
      // can move a tile kind profile (but not a bar or window - has it's own window border anyway)
      this.Cursor = ( HUD.Profile.Kind == GUI.Kind.Tile || HUD.Profile.Kind == GUI.Kind.WindowBL ) ? Cursors.SizeAll : Cursors.Default;
      // attach it to the PRIMARY screen (we assume the FS is run on the primary anyway...)
      // preliminary  windows full width/height
      switch ( HUD.Placement ) {
        case GUI.Placement.Bottom:
          this.Width = m_mainScreen.Bounds.Width;
          this.Height = 40; //  any will do as we rescale it below
          this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y + m_mainScreen.Bounds.Height - this.Height );
          flp.FlowDirection = FlowDirection.LeftToRight;
          break;
        case GUI.Placement.Left:
          this.Height = m_mainScreen.Bounds.Height;
          this.Width = 200; //  any will do as we rescale it below
          this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y );
          flp.FlowDirection = FlowDirection.TopDown;
          break;
        case GUI.Placement.Right:
          this.Height = m_mainScreen.Bounds.Height;
          this.Width = 200; //  any will do as we rescale it below
          this.Location = new Point( m_mainScreen.Bounds.X + m_mainScreen.Bounds.Width - this.Width, m_mainScreen.Bounds.Y );
          flp.FlowDirection = FlowDirection.TopDown;
          break;
        case GUI.Placement.Top:
          this.Width = m_mainScreen.Bounds.Width;
          this.Height = 40; //  any will do as we rescale it below
          this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y );
          flp.FlowDirection = FlowDirection.LeftToRight;
          break;
        default:
          // Bottom
          this.Width = m_mainScreen.Bounds.Width;
          this.Height = 40; //  any will do as we rescale it below
          this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y + m_mainScreen.Bounds.Height - this.Height );
          flp.FlowDirection = FlowDirection.LeftToRight;
          break;
      }

      // Walk all DispItems and add the ones to be shown to the Flow Panel
      int maxHeight = 0;
      int maxWidth = 0;
      DispItem prevDi = null;
      GUI.BreakType registeredBreak = GUI.BreakType.None; ;
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        // using the enum index only to count from 0..max items
        var key = HUD.Profile.ItemKeyFromPos( (int)i);
        // The DispItem is a FlowPanel containing the Label and maybe some Values
        var di = HUD.DispItem( key );
        if ( di != null ) {
          if ( di.Controls.Count > 0 ) {
            // check and register breaks for 2nd up items if there is no break registered (FlowBreak takes priority over DivBreaks)
            // this takes breaks from not visible items too
            if ( registeredBreak == GUI.BreakType.None ) {
              // take any
              registeredBreak = HUD.Profile.BreakItem( key ) ? GUI.BreakType.FlowBreak :
                                HUD.Profile.DivItem1( key ) ? GUI.BreakType.DivBreak1 :
                                HUD.Profile.DivItem2( key ) ? GUI.BreakType.DivBreak2 : GUI.BreakType.None;
            }
            else if ( registeredBreak == GUI.BreakType.FlowBreak ) {
              // take no further
              ; // NOP
            }
            else {
              // override DivBreaks with FlowBrake only
              registeredBreak = HUD.Profile.BreakItem( key ) ? GUI.BreakType.FlowBreak : registeredBreak;
            }
          }
          // process shown items
          if ( HUD.ShowItem( key ) ) {
            // apply breaks if there are any
            if ( registeredBreak == GUI.BreakType.FlowBreak && prevDi != null ) {
              // the flowbreak causes the tagged item to be on the same line and then to break for the next one
              // Not so intuitive for the user - so we mark the one that goes on the next line but need to attach the FB then to the prev one
              flp.SetFlowBreak( prevDi, true );
              registeredBreak = GUI.BreakType.None; // reset
            }
            else if ( registeredBreak == GUI.BreakType.DivBreak1 || registeredBreak == GUI.BreakType.DivBreak2 ) {
              // separator must be set before the newly added item
              // select Color Type of the separator
              DI_Separator dSep = new DI_Separator((registeredBreak== GUI.BreakType.DivBreak2)? ColorType.cDivBG2: ColorType.cDivBG1 );
              // need some fiddling to make it fit in either direction
              if ( ( HUD.Placement == GUI.Placement.Bottom ) || ( HUD.Placement == GUI.Placement.Top ) ) {
                dSep.Dock = DockStyle.Left;// horizontal Bar
              }
              else {
                dSep.Dock = DockStyle.Top;// vertical Bar
              }
              GUI.GUI_Colors.Register( dSep ); // register for color management
              flp.Controls.Add( dSep ); // add it to the Main FlowPanel
              registeredBreak = GUI.BreakType.None; // reset
            }
            // add the item 
            flp.Controls.Add( di );
            /* Code to add tooltips to the Label Part of an item - NOT IN USE RIGHT NOW
            if ( !string.IsNullOrEmpty( di.TText ) ) {
              m_toolTip.SetToolTip( di.Label, di.TText );
            }
            */
            // collect max dimensions derived from each DispItem while loading the panel (loading also layouts them)
            int h = di.Top+di.Height;
            maxHeight = ( h > maxHeight ) ? h : maxHeight;
            int w = di.Left+di.Width;
            maxWidth = ( w > maxWidth ) ? w : maxWidth;

            prevDi = di; // store for FlowBreak attachment for valid and visible ones if the next one is tagged
          }
          // don't show
          else {
            di.Visible = false;
            // Dispose these items to get some memory back and not having invisible ones to be processed
            di.Dispose( );
          }
        }
      }

      // post proc - allocate the needed height/width/location
      // reduce width/ height for Tiles or Windows
      // A window is essentially a tile with border and will later be positioned at the last stored location
      switch ( HUD.Placement ) {
        case GUI.Placement.Bottom:
          this.Height = maxHeight + 5;
          if ( ( HUD.Profile.Kind == GUI.Kind.Tile ) || ( HUD.Profile.Kind == GUI.Kind.Window ) || ( HUD.Profile.Kind == GUI.Kind.WindowBL ) ) {
            this.Width = flp.Width + 5;
            this.Location = new Point( HUD.Profile.Location.X, m_mainScreen.Bounds.Y + m_mainScreen.Bounds.Height - this.Height );
          }
          else { // Bar
            this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y + m_mainScreen.Bounds.Height - this.Height );
          }
          break;

        case GUI.Placement.Left:
          this.Width = maxWidth + 10;
          if ( ( HUD.Profile.Kind == GUI.Kind.Tile ) || ( HUD.Profile.Kind == GUI.Kind.Window ) || ( HUD.Profile.Kind == GUI.Kind.WindowBL ) ) {
            this.Height = flp.Height + 10;
            this.Location = new Point( m_mainScreen.Bounds.X, HUD.Profile.Location.Y );
          }
          else { // Bar
            this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y );
          }
          break;

        case GUI.Placement.Right:
          this.Width = maxWidth + 10;
          if ( ( HUD.Profile.Kind == GUI.Kind.Tile ) || ( HUD.Profile.Kind == GUI.Kind.Window ) || ( HUD.Profile.Kind == GUI.Kind.WindowBL ) ) {
            this.Height = flp.Height + 10;
            this.Location = new Point( m_mainScreen.Bounds.X + m_mainScreen.Bounds.Width - this.Width, HUD.Profile.Location.Y );
          }
          else { // Bar
            this.Location = new Point( m_mainScreen.Bounds.X + m_mainScreen.Bounds.Width - this.Width, m_mainScreen.Bounds.Y );
          }
          break;

        case GUI.Placement.Top:
          this.Height = maxHeight + 5;
          if ( ( HUD.Profile.Kind == GUI.Kind.Tile ) || ( HUD.Profile.Kind == GUI.Kind.Window ) || ( HUD.Profile.Kind == GUI.Kind.WindowBL ) ) {
            this.Width = flp.Width + 5;
            this.Location = new Point( HUD.Profile.Location.X, m_mainScreen.Bounds.Y );
          }
          else { // Bar
            this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y );
          }
          break;

        default:
          // Bottom
          this.Height = maxHeight + 5;
          this.Location = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y + m_mainScreen.Bounds.Height - this.Height );
          break;
      }
      // after sizing the Window - re-dock the FLPanel for full Fill
      flp.Dock = DockStyle.Fill;

      // handle Window Style HUDs
      if ( HUD.Kind == GUI.Kind.Window ) {
        this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
        // We take the last user location to reposition the window / above it was bound to the edge of the main screen (Tile kind)
        // avoid invisible windows from odd stored locations
        if ( IsOnScreen( new Rectangle( HUD.Profile.Location, this.Size ) ) ) {
          this.Location = HUD.Profile.Location;
        }
        // A Window is still TopMost - don't know if this is a good idea, we shall see...
      }
      else if ( HUD.Kind == GUI.Kind.WindowBL ) {
        // We take the last user location to reposition the window / above it was bound to the edge of the main screen (Tile kind)
        // avoid invisible windows from odd stored locations
        if ( IsOnScreen( new Rectangle( HUD.Profile.Location, this.Size ) ) ) {
          this.Location = HUD.Profile.Location;
        }
        // A Window is still TopMost - don't know if this is a good idea, we shall see...
      }

      // set form opacity from Profile
      this.Opacity = HUD.Profile.Opacity;

      // Color the MSFS Label it if connected
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        HUD.DispItem( LItem.MSFS ).ColorType.ItemForeColor = ColorType.cOK;
        HUD.DispItem( LItem.MSFS ).ColorType.ItemBackColor = ColorType.cInverse; // not cBG so we still can select it in Transp. Mode
      }
      else {
        HUD.DispItem( LItem.MSFS ).ColorType.ItemForeColor = ColorType.cInfo;
        HUD.DispItem( LItem.MSFS ).ColorType.ItemBackColor = ColorType.cAlert;
      }

      SynchGUIVisible( true ); // Unhide when finished
      SynchGUI( );

      m_initDone = true;
      timer1.Enabled = true; // and enable pacer
    }


    /// <summary>
    /// Update the GUI values from Sim
    ///  In general GUI elements are only updated when checked and visible
    ///  Trackers and Meters are maintained independent of the View state (another profile may use them..)
    /// </summary>
    private void UpdateGUI( string dataRefName )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return; // sanity..
      if ( !m_initDone ) return; // cannot access items at this time

      HUD.UpdateGUI( dataRefName );
    }

    #endregion

    #region SimConnectClient chores

    // Monitor the Sim Event Handler after Connection
    private bool m_awaitingEvent = true; // cleared in the Sim Event Handler
    private int m_scGracePeriod = -1;    // grace period count down

    /// <summary>
    /// Toggle the connection
    /// </summary>
    private void SimConnect( )
    {
      HUD.DispItem( LItem.MSFS ).ColorType.ItemForeColor = ColorType.cInfo;
      HUD.DispItem( LItem.MSFS ).ColorType.ItemBackColor = ColorType.cInverse;

      if ( SC.SimConnectClient.Instance.IsConnected ) {
        // Disconnect from Input and SimConnect
        SetupInGameHook( false );
        SC.SimConnectClient.Instance.FlightPlanModule.Enabled = false;
        SC.SimConnectClient.Instance.Disconnect( );
      }
      else {
        // setup the event monitor before connecting (will be handled in the Timer Event)
        m_awaitingEvent = true;
        m_scGracePeriod = 3; // about 3*5 secs to get an event
                             // try to connect
        if ( SC.SimConnectClient.Instance.Connect( ) ) {
          HUD.DispItem( LItem.MSFS ).Label.ForeColor = Color.LimeGreen;
          // init the SimClient by pulling one item, so it registers the module, else the callback is not initiated
          _ = SC.SimConnectClient.Instance.HudBarModule.AcftConfigFile;
          SC.SimConnectClient.Instance.FlightPlanModule.Enabled = AppSettings.Instance.FltAutoSave;
          // enable game hooks if newly connected and desired
          SetupInGameHook( AppSettings.Instance.InGameHook );
        }
        else {
          HUD.DispItem( LItem.MSFS ).Label.BackColor = Color.Red;
        }
      }

    }

    /// <summary>
    /// Try every interval to connect - and if connected.. do in Sim chores
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void timer1_Tick( object sender, EventArgs e )
    {
      //Console.WriteLine( $"LCT count {lct,-6:###0}; SCT count {sct,-6:###0}; " );


      if ( SC.SimConnectClient.Instance.IsConnected ) {
        // handle the situation where Sim is connected but could not hookup to events
        // Happens when HudBar is running when the Sim is starting only.
        // Sometimes the Connection is made but was not hooking up to the event handling
        // Disconnect and try to reconnect 
        if ( m_awaitingEvent || SC.SimConnectClient.Instance.HudBarModule.SimRate_rate <= 0 ) {
          // No events seen so far
          if ( m_scGracePeriod <= 0 ) {
            // grace period is expired !
            Console.WriteLine( "HudBar: Did not receive an Event - Restarting Connection" );
            SimConnect( ); // Disconnect if we don't receive Events even the Sim is connected
          }
          m_scGracePeriod--;
        }
        // Voice is disabled when a new HUD is created, so enable if not yet done
        // The timer is enabled after InitGUI - so this one is always 5 sec later which should avoid the early takling..
        HUD.VoiceEnabled = true;
      }
      else {
        // If not connected try again
        SimConnect( );
      }
    }


    #endregion

  }
}

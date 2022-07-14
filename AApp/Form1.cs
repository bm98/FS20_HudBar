using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DbgLib;

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
    #region STATIC
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType);
    #endregion

    // Handle of the Primary Screen to attach bar and tile
    private readonly Screen m_mainScreen;

    // This will be the GUI form
    private frmGui m_frmGui;
    private DispPanel flp;

    // A HudBar standard ToolTip for the Button Helpers
    private ToolTip_Base m_toolTip = new ToolTip_Base();

    // The profiles
    private List<CProfile> m_profiles = new List<CProfile>();
    private int m_selProfile = 0;
    private ToolStripMenuItem[] m_profileMenu; // enable array access for the MenuItems

    // Our interaction hooks 
    private enum Hooks
    {
      Profile_1 = 0, Profile_2, Profile_3, Profile_4, Profile_5, // Profile selection
      Profile_6, Profile_7, Profile_8, Profile_9, Profile_10,
      Show_Hide, // toggle show/hide of the bar
      FlightBag,
      Camera
    }

    // Handles the RawInput from HID Keyboards
    Win.HotkeyController _keyHook;

    // MSFS Input handlers
    private Dictionary<Hotkeys, SC.Input.InputHandler> _fsInputCat = new Dictionary<Hotkeys, SC.Input.InputHandler>();

    // The Flightbag
    private Shelf.frmShelf m_shelf;

    // Camera Selector
    private Camera.frmCamera m_camera;

    // Configuration Dialog
    private readonly frmConfig CFG = new frmConfig( );

    // need to stop processing while reconfiguring the bar
    private bool m_initDone = false;

    /// <summary>
    /// Checks if a Point is visible on any screen
    /// </summary>
    /// <param name="point">The Location to check</param>
    /// <returns>True if visible</returns>
    private static bool IsOnScreen( Point point )
    {
      Screen[] screens = Screen.AllScreens;
      foreach ( Screen screen in screens ) {
        if ( screen.WorkingArea.Contains( point ) ) {
          return true;
        }
      }
      return false;
    }

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
      if ( HUD == null ) return newL; // we need the HUD to apply rules

      // Tiles are bound to a border of the main screen
      switch ( HUD.Placement ) {
        case GUI.Placement.Top:
          newL.X = ( newL.X > m_mainScreen.Bounds.Right - size.Width / 2 ) ? m_mainScreen.Bounds.Right - size.Width / 2 : newL.X; // catch right side out of bounds
          newL.X = ( newL.X < m_mainScreen.Bounds.Left - size.Width / 2 ) ? m_mainScreen.Bounds.Left - size.Width / 2 : newL.X; // catch left side out of bounds, wins if the width is > screen.Width
          break;
        case GUI.Placement.Bottom:
          newL.X = ( newL.X > m_mainScreen.Bounds.Right - size.Width / 2 ) ? m_mainScreen.Bounds.Right - size.Width / 2 : newL.X; // catch right side out of bounds
          newL.X = ( newL.X < m_mainScreen.Bounds.Left - size.Width / 2 ) ? m_mainScreen.Bounds.Left - size.Width / 2 : newL.X; // catch left side out of bounds, wins if the width is > screen.Width
          break;
        case GUI.Placement.Left:
          newL.Y = ( newL.Y > m_mainScreen.Bounds.Bottom - size.Height / 2 ) ? m_mainScreen.Bounds.Bottom - size.Height / 2 : newL.Y; // catch top side out of bounds
          newL.Y = ( newL.Y < m_mainScreen.Bounds.Top - size.Height / 2 ) ? m_mainScreen.Bounds.Top - size.Height / 2 : newL.Y; // catch top side out of bounds, wins if the width is > screen.Height
          break;
        case GUI.Placement.Right:
          newL.Y = ( newL.Y > m_mainScreen.Bounds.Bottom - size.Height / 2 ) ? m_mainScreen.Bounds.Bottom - size.Height / 2 : newL.Y; // catch top side out of bounds
          newL.Y = ( newL.Y < m_mainScreen.Bounds.Top - size.Height / 2 ) ? m_mainScreen.Bounds.Top - size.Height / 2 : newL.Y; // catch top side out of bounds, wins if the width is > screen.Height
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
      if ( HUD == null ) return newS; // we need the HUD to find the Size

      // get the window manager decoration borders from the Main Window
      Rectangle screenClientRect = base.RectangleToScreen(base.ClientRectangle);
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
      if ( HUD.Kind == GUI.Kind.Bar ) {
        switch ( HUD.Placement ) {
          case GUI.Placement.Top: newS.Width = m_mainScreen.Bounds.Width; break;
          case GUI.Placement.Bottom: newS.Width = m_mainScreen.Bounds.Width; break;
          case GUI.Placement.Left: newS.Height = m_mainScreen.Bounds.Height; break;
          case GUI.Placement.Right: newS.Height = m_mainScreen.Bounds.Height; break;
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
      if ( HUD == null ) return newL; // we need the HUD to find the Location

      // location is managed when it is a Bar or Tile, else it is movable and we return the current Location
      // Bar is on the Main Screen Border aligned full width or height
      // Tile follows the preferred border but the dimension is the same as a Borderless Window
      switch ( HUD.Placement ) {
        case GUI.Placement.Top:
          if ( HUD.Profile.Kind == GUI.Kind.Bar ) newL = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y );
          else if ( HUD.Profile.Kind == GUI.Kind.Tile ) {
            newL.Y = m_mainScreen.Bounds.Y;
            newL = TileBoundLocation( newL, size );
          }
          break;
        case GUI.Placement.Bottom:
          if ( HUD.Profile.Kind == GUI.Kind.Bar ) newL = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y + m_mainScreen.Bounds.Height - size.Height );
          else if ( HUD.Profile.Kind == GUI.Kind.Tile ) {
            newL.Y = m_mainScreen.Bounds.Y + m_mainScreen.Bounds.Height - size.Height;
            newL = TileBoundLocation( newL, size );
          }
          break;
        case GUI.Placement.Left:
          if ( HUD.Profile.Kind == GUI.Kind.Bar ) newL = new Point( m_mainScreen.Bounds.X, m_mainScreen.Bounds.Y );
          else if ( HUD.Profile.Kind == GUI.Kind.Tile ) {
            newL.X = m_mainScreen.Bounds.X;
            newL = TileBoundLocation( newL, size );
          }
          break;
        case GUI.Placement.Right:
          if ( HUD.Profile.Kind == GUI.Kind.Bar ) newL = new Point( m_mainScreen.Bounds.X + m_mainScreen.Bounds.Width - size.Width, m_mainScreen.Bounds.Y );
          else if ( HUD.Profile.Kind == GUI.Kind.Tile ) {
            newL.X = m_mainScreen.Bounds.X + m_mainScreen.Bounds.Width - size.Width;
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
      Size newS = BarSize();
      Point newL = BarLocation(this.Location, newS);

      return new Rectangle( newL, newS );
    }

    // synch the two forms for Size
    private void SynchGUISize( )
    {
      // Adding controls to the FlowPanel is enclosed in a   _inLayout = true ...  = false bracket
      // While adding controls to the FlowPanel there is some Resizing we don't want to catch
      // else it looks weird and takes more time than needed
      if ( !_inLayout ) {
        Rectangle newR = BarRectangle();
        // set This Size and Loc if a change is needed
        if ( ( this.Width != newR.Width ) || ( this.Height != newR.Height ) ) {
          this.Size = newR.Size;
        }
        if ( ( this.Left != newR.Left ) || ( this.Top != newR.Top ) ) {
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
      if ( !SC.SimConnectClient.Instance.IsConnected ) return;

      if ( enabled ) {
        if ( _fsInputCat.Count <= 0 ) {
          // reinitiate the hooks
          _fsInputCat.Add( Hotkeys.Show_Hide, SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_01 ) );
          _fsInputCat[Hotkeys.Show_Hide].InputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hotkeys.FlightBag, SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_07 ) );
          _fsInputCat[Hotkeys.FlightBag].InputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hotkeys.Camera, SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_08 ) );
          _fsInputCat[Hotkeys.Camera].InputArrived += FSInput_InputArrived;

          // ONLY the first 5 have SimKey Hooks (6..10 do not have this hotkey)
          _fsInputCat.Add( Hotkeys.Profile_1, SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_02 ) );
          _fsInputCat[Hotkeys.Profile_1].InputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hotkeys.Profile_2, SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_03 ) );
          _fsInputCat[Hotkeys.Profile_2].InputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hotkeys.Profile_3, SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_04 ) );
          _fsInputCat[Hotkeys.Profile_3].InputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hotkeys.Profile_4, SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_05 ) );
          _fsInputCat[Hotkeys.Profile_4].InputArrived += FSInput_InputArrived;
          _fsInputCat.Add( Hotkeys.Profile_5, SC.SimConnectClient.Instance.InputHandler( SC.Input.InputNameE.FST_06 ) );
          _fsInputCat[Hotkeys.Profile_5].InputArrived += FSInput_InputArrived;
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
      if ( e.ActionName == _fsInputCat[Hotkeys.Show_Hide].Inputname ) SynchGUIVisible( !this.Visible );
      else if (e.ActionName == _fsInputCat[Hotkeys.FlightBag].Inputname) mShelf_Click( null, new EventArgs( ) );
      else if (e.ActionName == _fsInputCat[Hotkeys.Camera].Inputname) mCamera_Click( null, new EventArgs( ) );
      else if ( e.ActionName == _fsInputCat[Hotkeys.Profile_1].Inputname ) mP1_Click( null, new EventArgs( ) );
      else if ( e.ActionName == _fsInputCat[Hotkeys.Profile_2].Inputname ) mP2_Click( null, new EventArgs( ) );
      else if ( e.ActionName == _fsInputCat[Hotkeys.Profile_3].Inputname ) mP3_Click( null, new EventArgs( ) );
      else if ( e.ActionName == _fsInputCat[Hotkeys.Profile_4].Inputname ) mP4_Click( null, new EventArgs( ) );
      else if ( e.ActionName == _fsInputCat[Hotkeys.Profile_5].Inputname ) mP5_Click( null, new EventArgs( ) );
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
        if (HUD.Hotkeys.ContainsKey( Hotkeys.Show_Hide )) _keyHook.AddKey( HUD.Hotkeys[Hotkeys.Show_Hide], Hotkeys.Show_Hide.ToString( ), OnHookKey );
        if (HUD.Hotkeys.ContainsKey( Hotkeys.FlightBag )) _keyHook.AddKey( HUD.Hotkeys[Hotkeys.FlightBag], Hotkeys.FlightBag.ToString( ), OnHookKey );
        if (HUD.Hotkeys.ContainsKey( Hotkeys.Camera )) _keyHook.AddKey( HUD.Hotkeys[Hotkeys.Camera], Hotkeys.Camera.ToString( ), OnHookKey );
        // profile switchers
        for (int p = 0; p < CProfile.c_numProfiles; p++) {
          if (HUD.Hotkeys.ContainsKey( (Hotkeys)p )) _keyHook.AddKey( HUD.Hotkeys[(Hotkeys)p], ((Hotkeys)p).ToString( ), OnHookKey );
        }

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
      if ( tag == Hotkeys.Show_Hide.ToString( ) ) SynchGUIVisible( !this.Visible );
      else if (tag == Hotkeys.FlightBag.ToString( )) mShelf_Click( null, new EventArgs( ) );
      else if (tag == Hotkeys.Camera.ToString( )) mCamera_Click( null, new EventArgs( ) );
      else {
        // check if a profile was triggered
        for (int p = 0; p < CProfile.c_numProfiles; p++) {
          if (tag == ((Hotkeys)p).ToString( )) {
            // switch Profile - this may take too long for some Profiles if they contain a lot of items
            // Windows may unhook in such cases (see MS doc rsp. code in HookController)
            m_selProfile = p;
            InitGUI( );
            return;
          }
        }
      }
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

      LOG.Log( "frmMain: Load Profiles" );
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

      m_profiles.Add( new CProfile( 6, AppSettings.Instance.Profile_6_Name,
                                       AppSettings.Instance.Profile_6, AppSettings.Instance.FlowBreak_6, AppSettings.Instance.Sequence_6,
                                       AppSettings.Instance.Profile_6_FontSize, AppSettings.Instance.Profile_6_Placement,
                                       AppSettings.Instance.Profile_6_Kind, AppSettings.Instance.Profile_6_Location,
                                       AppSettings.Instance.Profile_6_Condensed,
                                       AppSettings.Instance.Profile_6_Trans ) );

      m_profiles.Add( new CProfile( 7, AppSettings.Instance.Profile_7_Name,
                                       AppSettings.Instance.Profile_7, AppSettings.Instance.FlowBreak_7, AppSettings.Instance.Sequence_7,
                                       AppSettings.Instance.Profile_7_FontSize, AppSettings.Instance.Profile_7_Placement,
                                       AppSettings.Instance.Profile_7_Kind, AppSettings.Instance.Profile_7_Location,
                                       AppSettings.Instance.Profile_7_Condensed,
                                       AppSettings.Instance.Profile_7_Trans ) );

      m_profiles.Add( new CProfile( 8, AppSettings.Instance.Profile_8_Name,
                                       AppSettings.Instance.Profile_8, AppSettings.Instance.FlowBreak_8, AppSettings.Instance.Sequence_8,
                                       AppSettings.Instance.Profile_8_FontSize, AppSettings.Instance.Profile_8_Placement,
                                       AppSettings.Instance.Profile_8_Kind, AppSettings.Instance.Profile_8_Location,
                                       AppSettings.Instance.Profile_8_Condensed,
                                       AppSettings.Instance.Profile_8_Trans ) );

      m_profiles.Add( new CProfile( 9, AppSettings.Instance.Profile_9_Name,
                                       AppSettings.Instance.Profile_9, AppSettings.Instance.FlowBreak_9, AppSettings.Instance.Sequence_9,
                                       AppSettings.Instance.Profile_9_FontSize, AppSettings.Instance.Profile_9_Placement,
                                       AppSettings.Instance.Profile_9_Kind, AppSettings.Instance.Profile_9_Location,
                                       AppSettings.Instance.Profile_9_Condensed,
                                       AppSettings.Instance.Profile_9_Trans ) );

      m_profiles.Add( new CProfile( 10, AppSettings.Instance.Profile_10_Name,
                                       AppSettings.Instance.Profile_10, AppSettings.Instance.FlowBreak_10, AppSettings.Instance.Sequence_10,
                                       AppSettings.Instance.Profile_10_FontSize, AppSettings.Instance.Profile_10_Placement,
                                       AppSettings.Instance.Profile_10_Kind, AppSettings.Instance.Profile_10_Location,
                                       AppSettings.Instance.Profile_10_Condensed,
                                       AppSettings.Instance.Profile_10_Trans ) );

      m_selProfile = AppSettings.Instance.SelProfile;
      mSelProfile.Text = m_profiles[m_selProfile].PName;
      // collect the Menus for the profiles
      m_profileMenu = new ToolStripMenuItem[] { mP1, mP2, mP3, mP4, mP5, mP6, mP7, mP8, mP9, mP10 };

      LOG.Log( "frmMain: Load Colors" );
      Colorset = ToColorSet( AppSettings.Instance.Appearance );

      mAltMetric.CheckState = AppSettings.Instance.Altitude_Metric ? CheckState.Checked : CheckState.Unchecked;
      mDistMetric.CheckState = AppSettings.Instance.Distance_Metric ? CheckState.Checked: CheckState.Unchecked;
      mShowUnits.CheckState = AppSettings.Instance.ShowUnits ? CheckState.Checked : CheckState.Unchecked;

      // Use CoordLib with no blanks as separator
      CoordLib.Dms.Separator = "";

      // ShowUnits and Opacity are set via HUD in InitGUI
      LOG.Log( "frmMain: Load GUI Forms" );
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

      // Setup the Camera and put it somewhere we can see it (either last location or default)
      LOG.Log( "frmMain: Load Camera" );
      m_camera = new  Camera.frmCamera {
        //Size = AppSettings.Instance.CameraSize,  // Fixed Size
        Location = AppSettings.Instance.CameraLocation
      };
      if (!IsOnScreen( m_camera.Location )) {
        m_camera.Location = new Point( 100, 100 );// default if not visible
      }

      // Setup the Shelf and put it somewhere we can see it (either last location or default)
      LOG.Log( "frmMain: Load Shelf" );
      m_shelf = new Shelf.frmShelf {
        Size = AppSettings.Instance.ShelfSize,
        Location = AppSettings.Instance.ShelfLocation
      };
      if ( !IsOnScreen( m_shelf.Location ) ) {
        m_shelf.Location = new Point( 100, 100 );// default if not visible
      }
      //  set from Saved default - if not found, set our default
      if ( !m_shelf.SetShelfFolder( AppSettings.Instance.ShelfFolder ) ) {
        m_shelf.SetShelfFolder( @".\DemoBag" ); // the one in our Deployment
      }
      LOG.Log( $"frmMain: Shelf Folder is: {AppSettings.Instance.ShelfFolder}" );

      // Find and hold the Primary Screen
      Screen[] screens = Screen.AllScreens;
      m_mainScreen = screens[0];
      // now get the Primary one
      foreach ( Screen screen in screens ) {
        if ( screen.Primary ) {
          m_mainScreen = screen;
        }
      }

      LOG.Log( "frmMain: Init Form Done" );
    }


    private void frmMain_Load( object sender, EventArgs e )
    {
      LOG.Log( $"frmMain_Load: Start" );
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
      this.BackColor = c_WinBG;

      // Get the controls the first time from Config
      InitGUI( );
      WPTracker.Reset( );

      // attach a Callback for the SimClient
      SC.SimConnectClient.Instance.DataArrived += Instance_DataArrived;
      SC.SimConnectClient.Instance.FlightPlanModule.Enabled = false; // start disabled, will be re-checked in InitGUI
                                                                     // Layout may need to update when the Aircraft changes (due to Engine Count)
      SC.SimConnectClient.Instance.AircraftChange += Instance_AircraftChange;

      // Pacer to connect and other repetitive chores
      timer1.Interval = 5000; // try to connect in 5 sec intervals
      timer1.Enabled = true;

      LOG.Log( $"frmMain_Load: End" );
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
      if ( !m_initDone ) return; // bail out if in Init
      if ( this.WindowState != FormWindowState.Normal ) return;   // can only handle the normal Window State here
                                                                  // can only handle Windows here, Bar and Tile is tied to the screen border
      if ( !( HUD.Kind == GUI.Kind.Window || HUD.Kind == GUI.Kind.WindowBL ) ) return;

      HUD.Profile.UpdateLocation( this.Location );
      // store new location per profile
      switch ( m_selProfile ) {
        case 0: AppSettings.Instance.Profile_1_Location = this.Location; break;
        case 1: AppSettings.Instance.Profile_2_Location = this.Location; break;
        case 2: AppSettings.Instance.Profile_3_Location = this.Location; break;
        case 3: AppSettings.Instance.Profile_4_Location = this.Location; break;
        case 4: AppSettings.Instance.Profile_5_Location = this.Location; break;
        case 5: AppSettings.Instance.Profile_6_Location = this.Location; break;
        case 6: AppSettings.Instance.Profile_7_Location = this.Location; break;
        case 7: AppSettings.Instance.Profile_8_Location = this.Location; break;
        case 8: AppSettings.Instance.Profile_9_Location = this.Location; break;
        case 9: AppSettings.Instance.Profile_10_Location = this.Location; break;
        default: AppSettings.Instance.Profile_1_Location = this.Location; break;
      }
      AppSettings.Instance.Save( );
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
      LOG.Log( $"frmMain_FormClosing: Start" );

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
      LOG.Log( $"frmMain_FormClosing: End" );
    }

    // Menu Exit event
    private void mExit_Click( object sender, EventArgs e )
    {
      this.Close( ); // just call the main Close
    }

    #region Units Menu

    // Update if the user has changed it
    private void mAltMetric_CheckedChanged( object sender, EventArgs e )
    {
      AppSettings.Instance.Altitude_Metric = mAltMetric.Checked;
      AppSettings.Instance.Save( );
      HUD?.SetAltitudeMetric( AppSettings.Instance.Altitude_Metric );
    }

    // Update if the user has changed it
    private void mDistMetric_CheckedChanged( object sender, EventArgs e )
    {
      AppSettings.Instance.Distance_Metric = mDistMetric.Checked;
      AppSettings.Instance.Save( );
      HUD?.SetDistanceMetric( AppSettings.Instance.Distance_Metric );
    }

    // Update if the user has changed it
    private void mShowUnits_CheckedChanged( object sender, EventArgs e )
    {
      AppSettings.Instance.ShowUnits = mShowUnits.Checked;
      AppSettings.Instance.Save( );
      HUD?.SetShowUnits( AppSettings.Instance.ShowUnits );
    }

    #endregion


    #region Config Menu

    // Menu Config Event
    private void mConfig_Click( object sender, EventArgs e )
    {
      LOG.Log( $"mConfig_Click: Start" );

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
        LOG.Log( $"mConfig_Click: Dialog OK" );
        // Save all configuration properties
        AppSettings.Instance.FRecorder = HUD.FlightRecorder;

        AppSettings.Instance.HKShowHide = HUD.Hotkeys.HotkeyString( Hotkeys.Show_Hide );
        AppSettings.Instance.HKProfile1 = HUD.Hotkeys.HotkeyString( Hotkeys.Profile_1 );
        AppSettings.Instance.HKProfile2 = HUD.Hotkeys.HotkeyString( Hotkeys.Profile_2 );
        AppSettings.Instance.HKProfile3 = HUD.Hotkeys.HotkeyString( Hotkeys.Profile_3 );
        AppSettings.Instance.HKProfile4 = HUD.Hotkeys.HotkeyString( Hotkeys.Profile_4 );
        AppSettings.Instance.HKProfile5 = HUD.Hotkeys.HotkeyString( Hotkeys.Profile_5 );
        AppSettings.Instance.HKProfile6 = HUD.Hotkeys.HotkeyString( Hotkeys.Profile_6 );
        AppSettings.Instance.HKProfile7 = HUD.Hotkeys.HotkeyString( Hotkeys.Profile_7 );
        AppSettings.Instance.HKProfile8 = HUD.Hotkeys.HotkeyString( Hotkeys.Profile_8 );
        AppSettings.Instance.HKProfile9 = HUD.Hotkeys.HotkeyString( Hotkeys.Profile_9 );
        AppSettings.Instance.HKProfile10 = HUD.Hotkeys.HotkeyString( Hotkeys.Profile_10 );
        AppSettings.Instance.HKShelf = HUD.Hotkeys.HotkeyString( Hotkeys.FlightBag );
        AppSettings.Instance.HKCamera = HUD.Hotkeys.HotkeyString( Hotkeys.Camera );
        AppSettings.Instance.KeyboardHook = HUD.KeyboardHook;
        AppSettings.Instance.InGameHook = HUD.InGameHook;

        AppSettings.Instance.ShelfFolder = HUD.ShelfFolder;

        AppSettings.Instance.FltAutoSaveATC = (int)HUD.FltAutoSave;
        AppSettings.Instance.VoiceName = HUD.VoiceName;

        AppSettings.Instance.UserFonts = HUD.FontRef.AsConfigString( );

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
        pIndex++;
        AppSettings.Instance.Profile_6_Name = m_profiles[pIndex].PName;
        AppSettings.Instance.Profile_6 = m_profiles[pIndex].ProfileString( );
        AppSettings.Instance.FlowBreak_6 = m_profiles[pIndex].FlowBreakString( );
        AppSettings.Instance.Sequence_6 = m_profiles[pIndex].ItemPosString( );
        AppSettings.Instance.Profile_6_FontSize = (int)m_profiles[pIndex].FontSize;
        AppSettings.Instance.Profile_6_Placement = (int)m_profiles[pIndex].Placement;
        AppSettings.Instance.Profile_6_Kind = (int)m_profiles[pIndex].Kind;
        AppSettings.Instance.Profile_6_Condensed = m_profiles[pIndex].Condensed;
        AppSettings.Instance.Profile_6_Trans = (int)m_profiles[pIndex].Transparency;
        pIndex++;
        AppSettings.Instance.Profile_7_Name = m_profiles[pIndex].PName;
        AppSettings.Instance.Profile_7 = m_profiles[pIndex].ProfileString( );
        AppSettings.Instance.FlowBreak_7 = m_profiles[pIndex].FlowBreakString( );
        AppSettings.Instance.Sequence_7 = m_profiles[pIndex].ItemPosString( );
        AppSettings.Instance.Profile_7_FontSize = (int)m_profiles[pIndex].FontSize;
        AppSettings.Instance.Profile_7_Placement = (int)m_profiles[pIndex].Placement;
        AppSettings.Instance.Profile_7_Kind = (int)m_profiles[pIndex].Kind;
        AppSettings.Instance.Profile_7_Condensed = m_profiles[pIndex].Condensed;
        AppSettings.Instance.Profile_7_Trans = (int)m_profiles[pIndex].Transparency;
        pIndex++;
        AppSettings.Instance.Profile_8_Name = m_profiles[pIndex].PName;
        AppSettings.Instance.Profile_8 = m_profiles[pIndex].ProfileString( );
        AppSettings.Instance.FlowBreak_8 = m_profiles[pIndex].FlowBreakString( );
        AppSettings.Instance.Sequence_8 = m_profiles[pIndex].ItemPosString( );
        AppSettings.Instance.Profile_8_FontSize = (int)m_profiles[pIndex].FontSize;
        AppSettings.Instance.Profile_8_Placement = (int)m_profiles[pIndex].Placement;
        AppSettings.Instance.Profile_8_Kind = (int)m_profiles[pIndex].Kind;
        AppSettings.Instance.Profile_8_Condensed = m_profiles[pIndex].Condensed;
        AppSettings.Instance.Profile_8_Trans = (int)m_profiles[pIndex].Transparency;
        pIndex++;
        AppSettings.Instance.Profile_9_Name = m_profiles[pIndex].PName;
        AppSettings.Instance.Profile_9 = m_profiles[pIndex].ProfileString( );
        AppSettings.Instance.FlowBreak_9 = m_profiles[pIndex].FlowBreakString( );
        AppSettings.Instance.Sequence_9 = m_profiles[pIndex].ItemPosString( );
        AppSettings.Instance.Profile_9_FontSize = (int)m_profiles[pIndex].FontSize;
        AppSettings.Instance.Profile_9_Placement = (int)m_profiles[pIndex].Placement;
        AppSettings.Instance.Profile_9_Kind = (int)m_profiles[pIndex].Kind;
        AppSettings.Instance.Profile_9_Condensed = m_profiles[pIndex].Condensed;
        AppSettings.Instance.Profile_9_Trans = (int)m_profiles[pIndex].Transparency;
        pIndex++;
        AppSettings.Instance.Profile_10_Name = m_profiles[pIndex].PName;
        AppSettings.Instance.Profile_10 = m_profiles[pIndex].ProfileString( );
        AppSettings.Instance.FlowBreak_10 = m_profiles[pIndex].FlowBreakString( );
        AppSettings.Instance.Sequence_10 = m_profiles[pIndex].ItemPosString( );
        AppSettings.Instance.Profile_10_FontSize = (int)m_profiles[pIndex].FontSize;
        AppSettings.Instance.Profile_10_Placement = (int)m_profiles[pIndex].Placement;
        AppSettings.Instance.Profile_10_Kind = (int)m_profiles[pIndex].Kind;
        AppSettings.Instance.Profile_10_Condensed = m_profiles[pIndex].Condensed;
        AppSettings.Instance.Profile_10_Trans = (int)m_profiles[pIndex].Transparency;

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

      LOG.Log( $"mConfig_Click: End" );
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

    private void mP6_Click( object sender, EventArgs e )
    {
      m_selProfile = 5;
      InitGUI( );
    }

    private void mP7_Click( object sender, EventArgs e )
    {
      m_selProfile = 6;
      InitGUI( );
    }

    private void mP8_Click( object sender, EventArgs e )
    {
      m_selProfile = 7;
      InitGUI( );
    }

    private void mP9_Click( object sender, EventArgs e )
    {
      m_selProfile = 8;
      InitGUI( );
    }

    private void mP10_Click( object sender, EventArgs e )
    {
      m_selProfile = 9;
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
        m_shelf.TopMost = false;
        m_shelf.Hide( );
      }
      else {
        m_shelf.TopMost = true;
        m_shelf.Show( );
      }
    }

    #endregion

    #region Camera Selector
    private void mCamera_Click( object sender, EventArgs e )
    {
      if ( m_camera == null ) return; // sanity check 

      if (m_camera.Visible ) {
        m_camera.TopMost = false;
        m_camera.Hide( );
      }
      else {
        m_camera.TopMost = true;
        m_camera.Show( );
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
        // Tiles are bound to a border of the main screen
        switch ( HUD.Placement ) {
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

    private void frmMain_MouseUp( object sender, MouseEventArgs e )
    {
      if ( !m_moving ) return;
      m_moving = false;

      if ( !m_initDone ) return; // bail out if in Init
      if ( !( HUD.Kind == GUI.Kind.Tile || HUD.Kind == GUI.Kind.WindowBL ) ) return; // can only move Tile kind around here

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

    private HudBar HUD = null; // THE HudBar Obj
    private bool _inLayout = false; // enclose and track Loading of DispItems

    // initialize the form, the labels and default values
    private void InitGUI( )
    {
      LOG.Log( $"InitGUI: Start" );

      timer1.Enabled = false; // stop asynch Timer events
      m_initDone = false; // stop updating values while reconfiguring
      SynchGUIVisible( false ); // hide, else we see all kind of shaping

      LOG.Log( $"InitGUI: FlightPlanModule Setup" );
      SC.SimConnectClient.Instance.FlightPlanModule.ModuleMode = (FSimClientIF.FlightPlanMode)AppSettings.Instance.FltAutoSaveATC;
      SC.SimConnectClient.Instance.FlightPlanModule.Enabled = ( SC.SimConnectClient.Instance.FlightPlanModule.ModuleMode != FSimClientIF.FlightPlanMode.Disabled );
      SC.SimConnectClient.Instance.FlightLogModule.Enabled = AppSettings.Instance.FRecorder;
      LOG.Log( $"InitGUI: AirportMgr Reset" );
      AirportMgr.Reset( );

      // Update profile selection items
      LOG.Log( $"InitGUI: Update profile selection" );
      for ( int i = 0; i < CProfile.c_numProfiles; i++ ) {
        m_profileMenu[i].Text = m_profiles[i].PName;
        m_profileMenu[i].Checked = false;
      }
      m_profileMenu[m_selProfile].Checked = true;
      mSelProfile.Text = m_profiles[m_selProfile].PName;
      LOG.Log( $"InitGUI: Selected profile {mSelProfile.Text}" );
      // Set the Window Title
      this.Text = ( string.IsNullOrEmpty( Program.Instance ) ? "Default" : Program.Instance ) + $" HudBar: {m_profiles[m_selProfile].PName}          - by bm98ch";

      // create a catalog from Settings (serialized as item strings..)
      LOG.Log( $"InitGUI: Setup Hotkeys" );
      var _hotkeycat = new WinHotkeyCat();
      _hotkeycat.MaintainHotkeyString( Hotkeys.Show_Hide, AppSettings.Instance.HKShowHide );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_1, AppSettings.Instance.HKProfile1 );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_2, AppSettings.Instance.HKProfile2 );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_3, AppSettings.Instance.HKProfile3 );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_4, AppSettings.Instance.HKProfile4 );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_5, AppSettings.Instance.HKProfile5 );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_6, AppSettings.Instance.HKProfile6 );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_7, AppSettings.Instance.HKProfile7 );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_8, AppSettings.Instance.HKProfile8 );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_9, AppSettings.Instance.HKProfile9 );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Profile_10, AppSettings.Instance.HKProfile10 );
      _hotkeycat.MaintainHotkeyString( Hotkeys.FlightBag, AppSettings.Instance.HKShelf );
      _hotkeycat.MaintainHotkeyString( Hotkeys.Camera, AppSettings.Instance.HKCamera );
      foreach ( var hk in _hotkeycat ) {
        LOG.Log( $"InitGUI: {hk.Key} - {hk.Value.AsString}" );
      }

      // start the HudBar from scratch
      LOG.Log( $"InitGUI: Create HudBar" );
      HUD = new HudBar( lblProto, valueProto, value2Proto, signProto,
                          AppSettings.Instance.KeyboardHook, AppSettings.Instance.InGameHook, _hotkeycat,
                          AppSettings.Instance.FltAutoSaveATC, AppSettings.Instance.ShelfFolder,
                          m_profiles[m_selProfile], AppSettings.Instance.VoiceName, AppSettings.Instance.UserFonts,
                          AppSettings.Instance.FRecorder );

      // reread from config (change)
      LOG.Log( $"InitGUI: Reread Config changes" );
      SetupKeyboardHook( AppSettings.Instance.KeyboardHook );
      SetupInGameHook( AppSettings.Instance.InGameHook );
      m_shelf?.SetShelfFolder( AppSettings.Instance.ShelfFolder );

      // Prepare FLPanel to load controls
      // DON'T Suspend the Layout else the calculations below will not be valid, the form is invisible and no painting is done here
      LOG.Log( $"InitGUI: Reload FlowPanel with Kind: {HUD.Profile.Kind}, Placement: {HUD.Placement}" );

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
      LOG.Log( $"InitGUI: Post Processing" );
      // no border for most
      this.FormBorderStyle = FormBorderStyle.None;
      if ( HUD.Kind == GUI.Kind.Window ) this.FormBorderStyle = FormBorderStyle.FixedToolWindow; // apply the border if needed
      // can move a tile kind profile (but not a bar or window - has it's own window border anyway)
      this.Cursor = ( HUD.Profile.Kind == GUI.Kind.Tile || HUD.Profile.Kind == GUI.Kind.WindowBL ) ? Cursors.SizeAll : Cursors.Default;
      // set form opacity from Profile
      this.Opacity = HUD.Profile.Opacity;

      // init with the proposed location from profile
      if ( IsOnScreen( HUD.Profile.Location ) ) {
        this.Location = HUD.Profile.Location;
      }
      else {
        this.Location = new Point( 0, 0 ); // safe location
      }

      // realign all
      SynchGUI( );
      // Unhide when finished
      SynchGUIVisible( true );

      // Color the MSFS Label it if connected
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        HUD.DispItem( LItem.MSFS ).ColorType.ItemForeColor = ColorType.cOK;
        HUD.DispItem( LItem.MSFS ).ColorType.ItemBackColor = ColorType.cInverse; // not cBG so we still can select it in Transp. Mode
        // Set Engines if already connected
        flp.SetEnginesVisible( SC.SimConnectClient.Instance.HudBarModule.NumEngines );
      }
      else {
        HUD.DispItem( LItem.MSFS ).ColorType.ItemForeColor = ColorType.cInfo;
        HUD.DispItem( LItem.MSFS ).ColorType.ItemBackColor = ColorType.cAlert;
      }

      m_initDone = true;
      timer1.Enabled = true; // and enable the pacer

      LOG.Log( $"InitGUI: End" );
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

      var sec = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

      // maintain the Engine Visibility
      flp.SetEnginesVisible( SC.SimConnectClient.Instance.HudBarModule.NumEngines );
      // The Bar has it's own logic for data updates
      HUD.UpdateGUI( dataRefName, sec );
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
      LOG.Log( $"SimConnect: Start" );

      HUD.DispItem( LItem.MSFS ).ColorType.ItemForeColor = ColorType.cInfo;
      HUD.DispItem( LItem.MSFS ).ColorType.ItemBackColor = ColorType.cInverse;

      if ( SC.SimConnectClient.Instance.IsConnected ) {
        // Disconnect from Input and SimConnect
        SetupInGameHook( false );
        flp.SetEnginesVisible( -1 ); // reset for the next attempt
        SC.SimConnectClient.Instance.FlightPlanModule.Enabled = false;
        SC.SimConnectClient.Instance.Disconnect( );
        LOG.Log( $"SimConnect: Disconnected now" );
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
          // Set Engines 
          flp.SetEnginesVisible( SC.SimConnectClient.Instance.HudBarModule.NumEngines );
          LOG.Log( $"SimConnect: Connected now" );
        }
        else {
          HUD.DispItem( LItem.MSFS ).Label.BackColor = Color.Red;
          LOG.Log( $"SimConnect: Could not connect" );
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
      if ( SC.SimConnectClient.Instance.IsConnected ) {
        // handle the situation where Sim is connected but could not hookup to events
        // Happens when HudBar is running when the Sim is starting only.
        // Sometimes the Connection is made but was not hooking up to the event handling
        // Disconnect and try to reconnect 
        if ( m_awaitingEvent || SC.SimConnectClient.Instance.HudBarModule.SimRate_rate <= 0 ) {
          // No events seen so far
          if ( m_scGracePeriod <= 0 ) {
            // grace period is expired !
            LOG.Log( "timer1_Tick: Did not receive an Event for 5sec - Restarting Connection" );
            SimConnect( ); // Disconnect if we don't receive Events even the Sim is connected
          }
          m_scGracePeriod--;
        }
        // Voice is disabled when a new HUD is created, so enable if not yet done
        // The timer is enabled after InitGUI - so this one is always 5 sec later which should avoid some of the early talking..
        HUD.VoiceEnabled = true;
        // Check and Resize at this pace to fit the contents in case some layout changes made it not fitting anymore
        // this is due to long texts or changes in the visibility of items coming in by the SimConnect Data processing in the DI items
        SynchGUISize( );
      }
      else {
        // If not connected try again
        SimConnect( );
      }
    }


    #endregion

  }
}

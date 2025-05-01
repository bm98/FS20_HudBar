using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using static dNetBm98.XPoint;
using static dNetBm98.Utilities;

using FS20_HudBar.Bar.Items;
using FS20_HudBar.GUI;

using static FS20_HudBar.AppSettingsV2;
using dNetBm98;

namespace FS20_HudBar.Config
{
  /// <summary>
  /// A settings profile
  /// the checkbox, dropdown values of the user settings per profile
  /// </summary>
  class CProfile
  {
    #region Settings Support

    // ***** UPDATE WHEN SETTINGS ARE ADDED *****

    // all Profile Settings, prepended by a type tag (for human reading purposes only)
    private enum ProfileSettings
    {
      sName = 0,
      iFontSize,
      iPlacement,
      iKind,
      iTransparency,
      ptLocation,
      bCondensed,
      sProfileItems,
      sFlowBreaks,
      sSequence,
      sHotKey,
      sBGImageFile,
      paPadding,
      sFonts,
      sColorReg,
      sColorDim,
      sColorInv,
      bFrameItems,
      bBoxDivider,
    }

    // return an initialized properties catalog, mapps profile setting enum to a distinct profile 
    private static Dictionary<ProfileSettings, string> InitSettingCat( int profileNumber )
    {
      var sCat = new Dictionary<ProfileSettings, string> {
        { ProfileSettings.sName, $"Profile_{profileNumber}_Name" }, // string
        { ProfileSettings.iFontSize, $"Profile_{profileNumber}_FontSize" }, // int
        { ProfileSettings.iPlacement, $"Profile_{profileNumber}_Placement" }, // int
        { ProfileSettings.iKind, $"Profile_{profileNumber}_Kind" }, // int
        { ProfileSettings.iTransparency, $"Profile_{profileNumber}_Trans" }, // int
        { ProfileSettings.ptLocation, $"Profile_{profileNumber}_Location" }, // Point
        { ProfileSettings.bCondensed, $"Profile_{profileNumber}_Condensed" }, // bool
        { ProfileSettings.sProfileItems, $"Profile_{profileNumber}" }, // string
        { ProfileSettings.sFlowBreaks, $"FlowBreak_{profileNumber}" }, // string
        { ProfileSettings.sSequence, $"Sequence_{profileNumber}" },// string
        { ProfileSettings.sHotKey, $"HKProfile{profileNumber}" },// string
        { ProfileSettings.sBGImageFile, $"BgImageName_{profileNumber}" },// string
        { ProfileSettings.paPadding, $"BgImageArea_{profileNumber}" },// Padding
        { ProfileSettings.sFonts, $"ProfileFonts_{profileNumber}" },// string
        { ProfileSettings.sColorReg, $"ProfileColorsReg_{profileNumber}" },// string
        { ProfileSettings.sColorDim, $"ProfileColorsDim_{profileNumber}" },// string
        { ProfileSettings.sColorInv, $"ProfileColorsInv_{profileNumber}" },// string
        { ProfileSettings.bFrameItems, $"ProfileFrameItems_{profileNumber}" },// bool
        { ProfileSettings.bBoxDivider, $"ProfileBoxDivider_{profileNumber}" },// bool
      };

      return sCat;
    }


    // return a new profile from settings (with profile number 1..)
    public static CProfile GetFromSettings( int profileNumber )
    {
      // sanity
      if (profileNumber < 1) throw new ArgumentException( $"profileNumber <{profileNumber}> must be 1..{c_numProfiles}" );
      if (profileNumber > c_numProfiles) throw new ArgumentException( $"profileNumber <{profileNumber}> must be 1..{c_numProfiles}" );

      var sCat = InitSettingCat( profileNumber );

      return new CProfile(
          profileNumber,
          GetSetting<string>( sCat[ProfileSettings.sName], "" ),
          GetSetting<string>( sCat[ProfileSettings.sProfileItems], "" ),
          GetSetting<string>( sCat[ProfileSettings.sFlowBreaks], "" ),
          GetSetting<string>( sCat[ProfileSettings.sSequence], "" ),
          GetSetting<int>( sCat[ProfileSettings.iFontSize], 0 ),
          GetSetting<int>( sCat[ProfileSettings.iPlacement], 0 ),
          GetSetting<int>( sCat[ProfileSettings.iKind], 0 ),
          GetSetting<Point>( sCat[ProfileSettings.ptLocation], Point.Empty ),
          GetSetting<bool>( sCat[ProfileSettings.bCondensed], false ),
          GetSetting<int>( sCat[ProfileSettings.iTransparency], 0 ),
          GetSetting<string>( sCat[ProfileSettings.sBGImageFile], "" ),
          GetSetting<Padding>( sCat[ProfileSettings.paPadding], Padding.Empty ),
          GetSetting<string>( sCat[ProfileSettings.sFonts], "" ),
          GetSetting<string>( sCat[ProfileSettings.sColorReg], "" ),
          GetSetting<string>( sCat[ProfileSettings.sColorDim], "" ),
          GetSetting<string>( sCat[ProfileSettings.sColorInv], "" ),
          GetSetting<bool>( sCat[ProfileSettings.bFrameItems], false ),
          GetSetting<bool>( sCat[ProfileSettings.bBoxDivider], false ),
          GetSetting<string>( sCat[ProfileSettings.sHotKey], "" )
       );
    }

    /// <summary>
    /// Save the profile to Settings
    /// </summary>
    /// <param name="profile">A Profile</param>
    public static void SaveToSettings( CProfile profile )
    {
      // sanity
      if (profile == null) throw new ArgumentNullException( "profile cannot be null" );
      if (profile.ProfileNumber < 1) return; // ?? should not happen...
      if (profile.ProfileNumber > c_numProfiles) return; // ?? should not happen...

      var sCat = InitSettingCat( profile.ProfileNumber );

      SetSetting<string>( sCat[ProfileSettings.sName], profile.PName );
      SetSetting<string>( sCat[ProfileSettings.sProfileItems], profile.ProfileString( ) );
      SetSetting<string>( sCat[ProfileSettings.sFlowBreaks], profile.FlowBreakString( ) );
      SetSetting<string>( sCat[ProfileSettings.sSequence], profile.ItemPosString( ) );
      SetSetting<int>( sCat[ProfileSettings.iFontSize], (int)profile.FontSize );
      SetSetting<int>( sCat[ProfileSettings.iPlacement], (int)profile.Placement );
      SetSetting<int>( sCat[ProfileSettings.iKind], (int)profile.Kind );
      SetSetting<Point>( sCat[ProfileSettings.ptLocation], profile.Location );
      SetSetting<bool>( sCat[ProfileSettings.bCondensed], profile.Condensed );
      SetSetting<int>( sCat[ProfileSettings.iTransparency], (int)profile.Transparency );
      SetSetting<string>( sCat[ProfileSettings.sBGImageFile], profile.BgImageName );
      SetSetting<Padding>( sCat[ProfileSettings.paPadding], profile.BgImageBorder );
      SetSetting<string>( sCat[ProfileSettings.sFonts], profile.Fonts );
      SetSetting<string>( sCat[ProfileSettings.sColorReg], profile.ColorReg );
      SetSetting<string>( sCat[ProfileSettings.sColorDim], profile.ColorDim );
      SetSetting<string>( sCat[ProfileSettings.sColorInv], profile.ColorInv );
      SetSetting<bool>( sCat[ProfileSettings.bFrameItems], profile.FrameItems );
      SetSetting<bool>( sCat[ProfileSettings.bBoxDivider], profile.BoxDivider );
      SetSetting<string>( sCat[ProfileSettings.sHotKey], profile.HKProfile );
    }

    #endregion


    // *** CLASS 

    public const int c_numProfiles = 10; // Supported number of profiles

    private int _pNumber = 0; // Profile Number 1...

    // stores all item data (checked, flowbreak, position)
    private FlpItems _items = new FlpItems( );

    private GUI.FontSize _fontSize = GUI.FontSize.Regular;
    private GUI.Placement _placement = GUI.Placement.Bottom;
    private GUI.Kind _kind = GUI.Kind.Bar;
    private Point _location = new Point( 0, 0 );
    private bool _condensed = false;
    private GUI.Transparent _transparent = GUI.Transparent.T0;

    private string _bgImageName = ""; // 20240224
    private Padding _bgImageBorder = Padding.Empty; // 20240224

    private string _profileFont = ""; // 20240226
    private string _profileColorReg = ""; // 20240226
    private string _profileColorDim = ""; // 20240226
    private string _profileColorInv = ""; // 20240226
    private bool _frameItems = false; // 20241011
    private bool _boxDivider = false; // 20241011
    private string _hkProfile = "";

    /// <summary>
    /// The string divider char
    /// </summary>
    public const char Divider = ';';

    #region Break Handling Helpers

    /// <summary>
    /// The tag to indicate a flow break
    /// </summary>
    public static readonly char NoBreakTag = BreakTagFromEnum( GUI.BreakType.None );
    /// <summary>
    /// The tag to indicate a flow break
    /// </summary>
    public static readonly char FlowBreakTag = BreakTagFromEnum( GUI.BreakType.FlowBreak );
    /// <summary>
    /// The tag to indicate a div break Type 1
    /// </summary>
    public static readonly char DivBreakTag1 = BreakTagFromEnum( GUI.BreakType.DivBreak1 );
    /// <summary>
    /// The tag to indicate a div break Type 2
    /// </summary>
    public static readonly char DivBreakTag2 = BreakTagFromEnum( GUI.BreakType.DivBreak2 );

    /// <summary>
    /// Returns the Break Tag (digit character) from its Enum
    /// </summary>
    /// <param name="breakType">The BreakType Enum</param>
    /// <returns>A Break Tag character</returns>
    public static char BreakTagFromEnum( GUI.BreakType breakType )
    {
      return Convert.ToChar( Convert.ToByte( '0' ) + (byte)breakType );
    }

    /// <summary>
    /// Returns the BreakEnum from a Tag
    /// </summary>
    /// <param name="breakTag">A BreakTag</param>
    /// <returns>A Break Enum</returns>
    public static GUI.BreakType EnumFromBreakTag( char breakTag )
    {
      if (breakTag == FlowBreakTag) return GUI.BreakType.FlowBreak;
      if (breakTag == DivBreakTag1) return GUI.BreakType.DivBreak1;
      if (breakTag == DivBreakTag2) return GUI.BreakType.DivBreak2;
      return GUI.BreakType.None;
    }

    /// <summary>
    /// Returns the BreakEnum from a Tag String
    /// </summary>
    /// <param name="breakTag">A BreakTag</param>
    /// <returns>A Break Enum</returns>
    public static GUI.BreakType EnumFromBreakTagString( string breakTag )
    {
      if (breakTag == FlowBreakTag.ToString( )) return GUI.BreakType.FlowBreak;
      if (breakTag == DivBreakTag1.ToString( )) return GUI.BreakType.DivBreak1;
      if (breakTag == DivBreakTag2.ToString( )) return GUI.BreakType.DivBreak2;
      return GUI.BreakType.None;
    }

    /// <summary>
    /// Returns the Break Tag Color from its Enum
    /// </summary>
    /// <param name="breakType">The BreakType Enum</param>
    /// <returns>A Break Tag Color</returns>
    public static Color BreakColorFromEnum( GUI.BreakType breakType )
    {
      if (breakType == GUI.BreakType.FlowBreak) return GUI.GUI_Colors.c_FBCol;
      if (breakType == GUI.BreakType.DivBreak1) return GUI.GUI_Colors.c_DB1Col;
      if (breakType == GUI.BreakType.DivBreak2) return GUI.GUI_Colors.c_DB2Col;
      return GUI.GUI_Colors.c_NBCol;
    }


    #endregion

    /// <summary>
    /// Number of the Profile 1..N
    /// </summary>
    public int ProfileNumber => _pNumber;
    /// <summary>
    /// The Profile name
    /// </summary>
    public string PName { get; set; } = "Profile";
    /// <summary>
    /// The Fontsize
    /// </summary>
    public GUI.FontSize FontSize => _fontSize;
    /// <summary>
    /// The Placement of the Bar
    /// </summary>
    public GUI.Placement Placement => _placement;
    /// <summary>
    /// The Display Kind of the Bar
    /// </summary>
    public GUI.Kind Kind => _kind;
    /// <summary>
    /// The Location of the bar (last recorded)
    /// </summary>
    public Point Location => _location;
    /// <summary>
    /// Whether or not using a condensed font
    /// </summary>
    public bool Condensed => _condensed;
    /// <summary>
    /// The Transparency of the window
    /// </summary>
    public GUI.Transparent Transparency => _transparent;
    /// <summary>
    /// The Opacity of the window
    ///  just the inverse of the Transparency
    /// </summary>
    public float Opacity => 1.0f - (int)_transparent / 10f; // inverse of transparency

    /// <summary>
    /// Filename of the Background Image or empty if none to be shown
    /// </summary>
    public string BgImageName => _bgImageName;
    /// <summary>
    /// Border Area
    /// </summary>
    public Padding BgImageBorder => _bgImageBorder;

    /// <summary>
    /// True when using default fonts
    /// </summary>
    public bool UsingDefaultFonts => string.IsNullOrEmpty( _profileFont );
    /// <summary>
    /// Fonts for this Profile
    /// </summary>
    public string Fonts => _profileFont;

    /// <summary>
    /// True when using default colors
    /// </summary>
    public bool UsingDefaultColors => string.IsNullOrEmpty( _profileColorReg );
    /// <summary>
    /// Color Set regular
    /// </summary>
    public string ColorReg => _profileColorReg;
    /// <summary>
    /// Color Set dimmed
    /// </summary>
    public string ColorDim => _profileColorDim;
    /// <summary>
    /// Color Set inversed
    /// </summary>
    public string ColorInv => _profileColorInv;
    /// <summary>
    /// Frame Items flag
    /// </summary>
    public bool FrameItems => _frameItems;
    /// <summary>
    /// Box Divider Flag
    /// </summary>
    public bool BoxDivider => _boxDivider;
    /// <summary>
    /// The Profile switching hotkey as ConfigString
    /// </summary>
    public string HKProfile => _hkProfile;

    /// <summary>
    /// Update the Location of the profile
    /// </summary>
    /// <param name="location">A point</param>
    public void UpdateLocation( Point location )
    {
      _location = location;
    }


    /// <summary>
    /// cTor:  Create an empty profile with all items enabled
    /// </summary>
    public CProfile( )
    {
      _items.CreateEmptyProfile( true );
    }

    /// <summary>
    /// cTor: Copy constructor
    /// </summary>
    /// <param name="other"></param>
    public CProfile( CProfile other )
    {
      _pNumber = other._pNumber;
      LoadProfile( other.PName,
        other.ProfileString( ), other.FlowBreakString( ), other.ItemPosString( ),
        other.FontSize, other.Placement, other.Kind, other.Location, other.Condensed, other.Transparency,
        other.BgImageName, other.BgImageBorder,
        other.Fonts, other.ColorReg, other.ColorDim, other.ColorInv,
        other.FrameItems, other.BoxDivider,
        other.HKProfile );
    }

    /// <summary>
    /// cTor: Create a profile from the profile string, must match, no existing items are set to true
    /// </summary>
    /// <param name="profileName">The Name of the Profile</param>
    /// <param name="profile">A semicolon separated string of 0 or 1 (shown) </param>
    /// <param name="flowBreak">A semicolon separated string of 0 or 1 (break) </param>
    /// <param name="sequence">A semicolon separated string of numbers (display position) </param>
    /// <param name="fontSize">The FontSize Number (matches the enum)</param>
    /// <param name="placement">The Placement Number (matches the enum)</param>
    /// <param name="condensed">The Condensed Font Flag</param>
    /// <param name="transparent">The Transparency value</param>
    /// <param name="bgImageName">The filename of the background image</param>
    /// <param name="bgImageBorder">The Padding value of the background image border</param>
    /// <param name="fonts">The Font string</param>
    /// <param name="colReg">The Color string regular</param>
    /// <param name="colDim">The Color string dimmed</param>
    /// <param name="colInv">The Color string inverser</param>
    /// <param name="frameItems">Frame Items flag</param>
    /// <param name="boxDivider">Box Divider flag</param>
    /// <param name="hk">The Hotkey config string</param>
    public CProfile( int pNum, string profileName,
                     string profile, string flowBreak, string sequence,
                     int fontSize, int placement, int kind, Point location, bool condensed, int transparent,
                     string bgImageName, Padding bgImageBorder,
                     string fonts, string colReg, string colDim, string colInv,
                     bool frameItems, bool boxDivider,
                     string hk )
    {
      _pNumber = pNum;
      LoadProfile( profileName, profile, flowBreak, sequence,
                    (GUI.FontSize)fontSize, (GUI.Placement)placement, (GUI.Kind)kind,
                    location, condensed, (GUI.Transparent)transparent,
                    bgImageName, bgImageBorder,
                    fonts, colReg, colDim, colInv,
                    frameItems, boxDivider,
                    hk );
    }

    /// <summary>
    /// Update this profile from the FLPanel
    /// </summary>
    /// <param name="flp">The FLPanel</param>
    /// <param name="flpIndex">The index of the columns 0..</param>
    public void GetItemsFromFlp( FlowLayoutPanel flp, int flpIndex )
    {
      FlpHandler.UpdateFromFlp( flp, flpIndex, _items );
    }

    /// <summary>
    /// Make the current FontSize the selected one
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void LoadFontSize( ComboBox box )
    {
      box.SelectedIndex = (int)_fontSize;
    }

    /// <summary>
    /// Make the current Placement the selected one
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void LoadPlacement( ComboBox box )
    {
      box.SelectedIndex = (int)_placement;
    }

    /// <summary>
    /// Make the current Kind the selected one
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void LoadKind( ComboBox box )
    {
      box.SelectedIndex = (int)_kind;
    }

    /// <summary>
    /// Make the current Condensed the selected one
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void LoadCond( ComboBox box )
    {
      box.SelectedIndex = _condensed ? 1 : 0;
    }

    /// <summary>
    /// Make the current Transparency the selected one
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void LoadTrans( ComboBox box )
    {
      box.SelectedIndex = (int)_transparent;
    }

    /// <summary>
    /// Set the font size Setting
    /// </summary>
    /// <param name="fontSize">A valid font size</param>
    public void SetFontSize( GUI.FontSize fontSize )
    {
      _fontSize = fontSize;
    }

    /// <summary>
    /// Update this profile from the FontSize ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetFontSizeFromCombo( ComboBox box )
    {
      _fontSize = (GUI.FontSize)box.SelectedIndex;
    }

    /// <summary>
    /// Set the placement Setting
    /// </summary>
    /// <param name="placement">A valid placement</param>
    public void SetPlacement( GUI.Placement placement )
    {
      _placement = placement;
    }

    /// <summary>
    /// Update this profile from the Placement ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetPlacementFromCombo( ComboBox box )
    {
      _placement = (GUI.Placement)box.SelectedIndex;
    }

    /// <summary>
    /// Set the kind Setting
    /// </summary>
    /// <param name="kind">A valid kind</param>
    public void SetKind( GUI.Kind kind )
    {
      _kind = kind;
    }

    /// <summary>
    /// Update this profile from the Display Kind ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetKindFromCombo( ComboBox box )
    {
      _kind = (GUI.Kind)box.SelectedIndex;
    }

    /// <summary>
    /// Set the condensed Setting
    /// </summary>
    /// <param name="condensed">A condensed flag</param>
    public void SetCondensed( bool condensed )
    {
      _condensed = condensed;
    }

    /// <summary>
    /// Update this profile from the Condensed ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetCondensedFromCombo( ComboBox box )
    {
      _condensed = box.SelectedIndex == 1;
    }

    /// <summary>
    /// Set the transparent Setting
    /// </summary>
    /// <param name="transparent">A valid transparency</param>
    public void SetTransparency( GUI.Transparent transparent )
    {
      _transparent = transparent;
    }

    /// <summary>
    /// Update this profile from the Transparent ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetTransparencyFromCombo( ComboBox box )
    {
      _transparent = (GUI.Transparent)box.SelectedIndex;
    }

    /// <summary>
    /// Update this profile from BG Image values
    /// </summary>
    /// <param name="bgImageName">The BG Image filename or empty if not used</param>
    /// <param name="bgImageBorder">The BG Image border area</param>
    public void SetBgImageFromValues( string bgImageName, Padding bgImageBorder )
    {
      _bgImageName = bgImageName;
      _bgImageBorder = bgImageBorder;
    }

    /// <summary>
    /// Update this profile using Default Fonts
    /// </summary>
    public void SetDefaultFonts( )
    {
      _profileFont = "";
    }
    /// <summary>
    /// Update this profile from Font string
    /// </summary>
    /// <param name="fonts">The font string</param>
    public void SetFontsFromValues( string fonts )
    {
      _profileFont = fonts;
    }

    /// <summary>
    /// Update this profile using Default Colors
    /// </summary>
    public void SetDefaultColors( )
    {
      _profileColorReg = "";
      _profileColorDim = "";
      _profileColorInv = "";
    }
    /// <summary>
    /// Update this profile from Color strings
    /// </summary>
    /// <param name="colReg">Regular colors string</param>
    /// <param name="colDim">Dimmed colors string</param>
    /// <param name="colInv">Inversed colors string</param>
    public void SetColorsFromValues( string colReg, string colDim, string colInv )
    {
      _profileColorReg = colReg;
      _profileColorDim = colDim;
      _profileColorInv = colInv;
    }

    /// <summary>
    /// Set the FrameItems flag
    /// </summary>
    /// <param name="value">Flag</param>
    public void SetFrameItemsFromValue( bool value ) => _frameItems = value;

    /// <summary>
    /// Set the BoxDivider flag
    /// </summary>
    /// <param name="value">Flag</param>
    public void SetBoxDividerFromValue( bool value ) => _boxDivider = value;

    /// <summary>
    /// Update this profile from Hotkey string
    /// </summary>
    /// <param name="hk">Hotkey config string</param>
    public void SetHKfromValue( string hk ) => _hkProfile = hk;


    // Load the profile from stored strings (Settings)
    private void LoadProfile( string profileName, string profile, string flowBreak, string sequence,
                                GUI.FontSize fontSize, GUI.Placement placement, GUI.Kind kind,
                                Point location, bool condensed, GUI.Transparent transparent,
                                string bgImageName, Padding bgImageBorder,
                                string fonts, string colReg, string colDim, string colInv,
                                bool frameItems, bool boxDivider,
                                string hk )
    {
      // save props in Profile
      PName = profileName;
      _fontSize = fontSize;
      _placement = placement;
      _kind = kind;
      _location = location;
      _condensed = condensed;
      _transparent = transparent;
      _bgImageName = bgImageName;
      _bgImageBorder = bgImageBorder;
      _profileFont = fonts;
      _profileColorReg = colReg;
      _profileColorDim = colDim;
      _profileColorInv = colInv;
      _frameItems = frameItems;
      _boxDivider = boxDivider;
      _hkProfile = hk;

      // The Dictionaries and stored strings of it maintain the Enum Sequence
      //   even if items are relocated for display
      // If a stored string does not contain a value (too short) then a default is taken
      // i.e. we always want a complete Dictionary when leaving the method!!!

      // Get the visibility status for each item
      SetProfileString( profile );
      // Get the flow break status for each item
      SetFlowBreakString( flowBreak );
      // Get the item position - don't validate the sequence here
      SetItemPosString( sequence );
    }

    /// <summary>
    /// Returns the regular ColorSet 
    /// </summary>
    public GUI_ColorSet ColorSetReg => GUI_Colors.FromConfigString( _profileColorReg );
    /// <summary>
    /// Returns the dimmed ColorSet 
    /// </summary>
    public GUI_ColorSet ColorSetDim => GUI_Colors.FromConfigString( _profileColorDim );
    /// <summary>
    /// Returns the inverse ColorSet 
    /// </summary>
    public GUI_ColorSet ColorSetInv => GUI_Colors.FromConfigString( _profileColorInv );

    /// <summary>
    /// Returns the Show state of an item
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>True if it is shown</returns>
    public bool IsShowItem( LItem item ) => _items.CheckedFor( item );

    /// <summary>
    /// Returns the True for a FlowBreak
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>True if it breaks</returns>
    public bool IsBreakItem( LItem item ) => _items.FlowBreakFor( item ) == GUI.BreakType.FlowBreak;

    /// <summary>
    /// Returns True for a Divider/Separator Break Type 1 OR 2 OR GAP
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>True if it a separator is needed</returns>
    public bool IsDivItem( LItem item ) => (_items.FlowBreakFor( item ) == GUI.BreakType.DivBreak1)
      || (_items.FlowBreakFor( item ) == GUI.BreakType.DivBreak2);

    /// <summary>
    /// Returns True for a Divider/Separator Break Type 1
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>True if it a separator is needed</returns>
    public bool IsDivItem1( LItem item ) => _items.FlowBreakFor( item ) == GUI.BreakType.DivBreak1;

    /// <summary>
    /// Returns True for a Divider/Separator Break Type 2
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>True if it a separator is needed</returns>
    public bool IsDivItem2( LItem item ) => _items.FlowBreakFor( item ) == GUI.BreakType.DivBreak2;

    /// <summary>
    /// Returns the Items Display position
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>The display position 0..</returns>
    public int ItemPos( LItem item ) => _items.PositionFor( item );

    /// <summary>
    /// Returns the Items Key for a Display position
    /// </summary>
    /// <param name="item">A position 0..</param>
    /// <returns>The display item key</returns>
    public LItem ItemKeyFromPos( int pos ) => _items.ItemKeyFromPos( pos );

    /// <summary>
    /// Set the profile Settings
    /// </summary>
    /// <param name="profileString">A valid profile string</param>
    public void SetProfileString( string profileString ) => _items.ProfileFromString( profileString );

    /// <summary>
    /// Returns the profile Settings string 
    /// </summary>
    /// <returns>A profile string </returns>
    public string ProfileString( ) => _items.ProfileString( );

    /// <summary>
    /// Set the flowBreak Settings
    /// </summary>
    /// <param name="flowBreakString">A valid flow break string</param>
    public void SetFlowBreakString( string flowBreakString ) => _items.FlowBreaksFromString( flowBreakString );

    /// <summary>
    /// Returns the flowBreak Settings string 
    /// </summary>
    /// <returns>A flowBreak string </returns>
    public string FlowBreakString( ) => _items.FlowBreakString( );

    /// <summary>
    /// Set the item position Setting
    /// </summary>
    /// <param name="itemPosString">a valid item position string</param>
    public void SetItemPosString( string itemPosString ) => _items.PositionsFromString( itemPosString );


    /// <summary>
    /// Returns the item position Settings string 
    /// </summary>
    /// <returns>A item position string </returns>
    public string ItemPosString( ) => _items.ItemPosString( );

    /// <summary>
    /// Returns a list orderd along the item Pos, the list contains the LItem Key to be shown at this pos (0 based)
    /// Kind of Transposing the sequence list
    /// </summary>
    /// <returns></returns>
    public List<LItem> ItemPosList( ) => _items.ItemPosList( );


    #region Import / Export

    // check string, first line of a profile
    private const string c_FileMagic = "65564813-38BC-4043-A3EC-AC8DA5613032";

    /// <summary>
    /// Get this Profile as Serialized String
    /// </summary>
    /// <returns>A Profile as string</returns>
    public string GetAsSerString( )
    {
      StringBuilder sb = new StringBuilder( );

      sb.AppendLine( c_FileMagic );
      sb.AppendLine( this.PName );
      sb.AppendLine( this.ProfileString( ) );
      sb.AppendLine( this.FlowBreakString( ) );
      sb.AppendLine( this.ItemPosString( ) );
      sb.AppendLine( this.FontSize.ToString( ) );
      sb.AppendLine( this.Placement.ToString( ) );
      sb.AppendLine( this.Kind.ToString( ) );
      sb.AppendLine( this.Location.AsSerString( ) );
      sb.AppendLine( this.Condensed.ToString( ) );
      sb.AppendLine( this.Transparency.ToString( ) );
      sb.AppendLine( this.BgImageName.ToString( ) );
      sb.AppendLine( this.BgImageBorder.AsSerString( ) );
      sb.AppendLine( this.Fonts );
      sb.AppendLine( this.ColorReg );
      sb.AppendLine( this.ColorDim );
      sb.AppendLine( this.ColorInv.ToString( ) );
      sb.AppendLine( this.FrameItems.ToString( ) );
      sb.AppendLine( this.BoxDivider.ToString( ) );
      sb.AppendLine( this.HKProfile );

      return sb.ToString( );
    }

    /// <summary>
    /// Convert from a Serialized Profile String to a Profile
    /// </summary>
    /// <returns>A Profile or null</returns>
    public static CProfile GetFromSerString( string profileSerString )
    {
      try {
        string[] p = profileSerString.Replace( "\r", "" ).Split( new char[] { '\n' }, StringSplitOptions.None );

        if (p.Length < 20) return null;
        int idx = 0;
        if (p[idx++] != c_FileMagic) return null;

        string pName = p[idx++];
        string pString = p[idx++];
        string fBrkString = p[idx++];
        string iPosString = p[idx++];
        if (!Enum.TryParse<FontSize>( p[idx++], out var fSize )) { fSize = FontSize.Regular; }
        if (!Enum.TryParse<Placement>( p[idx++], out var placement )) { placement = Placement.Top; }
        if (!Enum.TryParse<Kind>( p[idx++], out var kind )) { kind = Kind.Bar; }
        Point pLoc = PointFromSerString( p[idx++] );
        Boolean.TryParse( p[idx++], out bool condensed );
        if (!Enum.TryParse<Transparent>( p[idx++], out var transparency )) { transparency = Transparent.T0; }
        string bgImageName = p[idx++];
        Padding bgImageBorder = PaddingFromSerString( p[idx++] );

        string fonts = p[idx++];
        string colReg = p[idx++];
        string colDim = p[idx++];
        string colInv = p[idx++];
        Boolean.TryParse( p[idx++], out bool frameItems );
        Boolean.TryParse( p[idx++], out bool boxDivider );
        string hkProfile = p[idx++];

        var cp = new CProfile( 0, pName, pString, fBrkString, iPosString,
          (int)fSize, (int)placement, (int)kind, pLoc, condensed, (int)transparency, bgImageName,
          bgImageBorder, fonts, colReg, colDim, colInv, frameItems, boxDivider, hkProfile );

        return cp;
      }
      catch { }
      return null;
    }

    #endregion

  }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.Bar;
using FS20_HudBar.Bar.Items;

namespace FS20_HudBar.Config
{
  /// <summary>
  /// A settings profile
  /// the checkbox, dropdown values of the user settings per profile
  /// </summary>
  class CProfile
  {
    public const int c_numProfiles = 10; // Supported number of profiles

    private int m_pNumber = 0; // Profile Number 1...

    // stores all item data (checked, flowbreak, position)
    private FlpItems m_items = new FlpItems( );


    private GUI.FontSize m_fontSize = GUI.FontSize.Regular;
    private GUI.Placement m_placement = GUI.Placement.Bottom;
    private GUI.Kind m_kind = GUI.Kind.Bar;
    private Point m_location = new Point( 0, 0 );
    private bool m_condensed = false;
    private GUI.Transparent m_transparent = GUI.Transparent.T0;

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
    /// Returns the Break Tag from its Enum
    /// </summary>
    /// <param name="breakType">The BreakType Enum</param>
    /// <returns>A Break Tag</returns>
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
    /// The Profile name
    /// </summary>
    public string PName { get; set; } = "Profile";
    /// <summary>
    /// The Fontsize
    /// </summary>
    public GUI.FontSize FontSize => m_fontSize;
    /// <summary>
    /// The Placement of the Bar
    /// </summary>
    public GUI.Placement Placement => m_placement;
    /// <summary>
    /// The Display Kind of the Bar
    /// </summary>
    public GUI.Kind Kind => m_kind;
    /// <summary>
    /// The Location of the bar (last recorded)
    /// </summary>
    public Point Location => m_location;
    /// <summary>
    /// Whether or not using a condensed font
    /// </summary>
    public bool Condensed => m_condensed;
    /// <summary>
    /// The Transparency of the window
    /// </summary>
    public GUI.Transparent Transparency => m_transparent;
    /// <summary>
    /// The Opacity of the window
    ///  just the inverse of the Transparency
    /// </summary>
    public float Opacity => 1.0f - (int)m_transparent / 10f; // inverse of transparency


    /// <summary>
    /// Update the Location of the profile
    /// </summary>
    /// <param name="location">A point</param>
    public void UpdateLocation( Point location )
    {
      m_location = location;
    }


    /// <summary>
    /// cTor:  Create an empty profile with all items enabled
    /// </summary>
    public CProfile( )
    {
      m_items.CreateEmptyProfile( true );
    }

    /// <summary>
    /// cTor: Copy constructor
    /// </summary>
    /// <param name="other"></param>
    public CProfile( CProfile other )
    {
      m_pNumber = other.m_pNumber;
      LoadProfile( other.PName,
        other.ProfileString( ), other.FlowBreakString( ), other.ItemPosString( ),
        other.FontSize, other.Placement, other.Kind, other.Location, other.Condensed, other.Transparency );
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
    public CProfile( int pNum, string profileName,
                     string profile, string flowBreak, string sequence,
                     int fontSize, int placement, int kind, Point location, bool condensed, int transparent )
    {
      m_pNumber = pNum;
      LoadProfile( profileName, profile, flowBreak, sequence,
                    (GUI.FontSize)fontSize, (GUI.Placement)placement, (GUI.Kind)kind,
                    location, condensed, (GUI.Transparent)transparent );
    }

    /// <summary>
    /// Update this profile from the FLPanel
    /// </summary>
    /// <param name="flp">The FLPanel</param>
    /// <param name="flpIndex">The index of the columns 0..</param>
    public void GetItemsFromFlp( FlowLayoutPanel flp, int flpIndex )
    {
      FlpHandler.UpdateFromFlp( flp, flpIndex, m_items );
    }

    /// <summary>
    /// Make the current FontSize the selected one
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void LoadFontSize( ComboBox box )
    {
      box.SelectedIndex = (int)m_fontSize;
    }

    /// <summary>
    /// Make the current Placement the selected one
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void LoadPlacement( ComboBox box )
    {
      box.SelectedIndex = (int)m_placement;
    }

    /// <summary>
    /// Make the current Kind the selected one
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void LoadKind( ComboBox box )
    {
      box.SelectedIndex = (int)m_kind;
    }

    /// <summary>
    /// Make the current Condensed the selected one
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void LoadCond( ComboBox box )
    {
      box.SelectedIndex = m_condensed ? 1 : 0;
    }

    /// <summary>
    /// Make the current Transparency the selected one
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void LoadTrans( ComboBox box )
    {
      box.SelectedIndex = (int)m_transparent;
    }

    /// <summary>
    /// Set the font size Setting
    /// </summary>
    /// <param name="fontSize">A valid font size</param>
    public void SetFontSize( GUI.FontSize fontSize )
    {
      m_fontSize = fontSize;
    }

    /// <summary>
    /// Update this profile from the FontSize ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetFontSizeFromCombo( ComboBox box )
    {
      m_fontSize = (GUI.FontSize)box.SelectedIndex;
    }

    /// <summary>
    /// Set the placement Setting
    /// </summary>
    /// <param name="placement">A valid placement</param>
    public void SetPlacement( GUI.Placement placement )
    {
      m_placement = placement;
    }

    /// <summary>
    /// Update this profile from the Placement ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetPlacementFromCombo( ComboBox box )
    {
      m_placement = (GUI.Placement)box.SelectedIndex;
    }

    /// <summary>
    /// Set the kind Setting
    /// </summary>
    /// <param name="kind">A valid kind</param>
    public void SetKind( GUI.Kind kind )
    {
      m_kind = kind;
    }

    /// <summary>
    /// Update this profile from the Display Kind ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetKindFromCombo( ComboBox box )
    {
      m_kind = (GUI.Kind)box.SelectedIndex;
    }

    /// <summary>
    /// Set the condensed Setting
    /// </summary>
    /// <param name="condensed">A condensed flag</param>
    public void SetCondensed( bool condensed )
    {
      m_condensed = condensed;
    }

    /// <summary>
    /// Update this profile from the Condensed ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetCondensedFromCombo( ComboBox box )
    {
      m_condensed = box.SelectedIndex == 1;
    }

    /// <summary>
    /// Set the transparent Setting
    /// </summary>
    /// <param name="transparent">A valid transparency</param>
    public void SetTransparency( GUI.Transparent transparent )
    {
      m_transparent = transparent;
    }

    /// <summary>
    /// Update this profile from the Transparent ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetTransparencyFromCombo( ComboBox box )
    {
      m_transparent = (GUI.Transparent)box.SelectedIndex;
    }

    // Load the profile from stored strings (Settings)
    private void LoadProfile( string profileName, string profile, string flowBreak, string sequence,
                                GUI.FontSize fontSize, GUI.Placement placement, GUI.Kind kind,
                                Point location, bool condensed, GUI.Transparent transparent )
    {
      // save props in Profile
      PName = profileName;
      m_fontSize = fontSize;
      m_placement = placement;
      m_kind = kind;
      m_location = location;
      m_condensed = condensed;
      m_transparent = transparent;

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
    /// Returns the Show state of an item
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>True if it is shown</returns>
    public bool IsShowItem( LItem item ) => m_items.CheckedFor( item );

    /// <summary>
    /// Returns the True for a FlowBreak
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>True if it breaks</returns>
    public bool IsBreakItem( LItem item ) => m_items.FlowBreakFor( item ) == GUI.BreakType.FlowBreak;

    /// <summary>
    /// Returns True for a Divider/Separator Break Type 1 OR 2
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>True if it a separator is needed</returns>
    public bool IsDivItem( LItem item ) => (m_items.FlowBreakFor( item ) == GUI.BreakType.DivBreak1) || (m_items.FlowBreakFor( item ) == GUI.BreakType.DivBreak2);

    /// <summary>
    /// Returns True for a Divider/Separator Break Type 1
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>True if it a separator is needed</returns>
    public bool IsDivItem1( LItem item ) => m_items.FlowBreakFor( item ) == GUI.BreakType.DivBreak1;

    /// <summary>
    /// Returns True for a Divider/Separator Break Type 2
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>True if it a separator is needed</returns>
    public bool IsDivItem2( LItem item ) => m_items.FlowBreakFor( item ) == GUI.BreakType.DivBreak2;

    /// <summary>
    /// Returns the Items Display position
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>The display position 0..</returns>
    public int ItemPos( LItem item ) => m_items.PositionFor( item );

    /// <summary>
    /// Returns the Items Key for a Display position
    /// </summary>
    /// <param name="item">A position 0..</param>
    /// <returns>The display item key</returns>
    public LItem ItemKeyFromPos( int pos ) => m_items.ItemKeyFromPos( pos );

    /// <summary>
    /// Set the profile Settings
    /// </summary>
    /// <param name="profileString">A valid profile string</param>
    public void SetProfileString( string profileString ) => m_items.ProfileFromString( profileString );

    /// <summary>
    /// Returns the profile Settings string 
    /// </summary>
    /// <returns>A profile string </returns>
    public string ProfileString( ) => m_items.ProfileString( );

    /// <summary>
    /// Set the flowBreak Settings
    /// </summary>
    /// <param name="flowBreakString">A valid flow break string</param>
    public void SetFlowBreakString( string flowBreakString ) => m_items.FlowBreaksFromString( flowBreakString );

    /// <summary>
    /// Returns the flowBreak Settings string 
    /// </summary>
    /// <returns>A flowBreak string </returns>
    public string FlowBreakString( ) => m_items.FlowBreakString( );

    /// <summary>
    /// Set the item position Setting
    /// </summary>
    /// <param name="itemPosString">a valid item position string</param>
    public void SetItemPosString( string itemPosString ) => m_items.PositionsFromString( itemPosString );


    /// <summary>
    /// Returns the item position Settings string 
    /// </summary>
    /// <returns>A item position string </returns>
    public string ItemPosString( ) => m_items.ItemPosString( );

    /// <summary>
    /// Returns a list orderd along the item Pos, the list contains the LItem Key to be shown at this pos (0 based)
    /// Kind of Transposing the sequence list
    /// </summary>
    /// <returns></returns>
    public List<LItem> ItemPosList( ) => m_items.ItemPosList( );

  }
}

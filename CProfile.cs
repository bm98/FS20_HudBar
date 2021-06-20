using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar
{


  /// <summary>
  /// A settings profile
  /// the checkbox values of the user settings
  /// </summary>
  class CProfile
  {

    private Dictionary<LItem, bool> m_profile = new Dictionary<LItem, bool>();
    private GUI.FontSize m_fontSize = GUI.FontSize.Regular;
    private GUI.Placement m_placement = GUI.Placement.Bottom;

    public string PName { get; set; } = "Profile";
    public GUI.FontSize FontSize => m_fontSize;
    public GUI.Placement Placement => m_placement;

    /// <summary>
    /// Create an empty profile with all items enabled
    /// </summary>
    public CProfile( )
    {
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        m_profile.Add( i, true );
      }
    }

    /// <summary>
    /// Create a profile from the profile string, must match, no existing items are set to true
    /// </summary>
    /// <param name="profileName">The Name of the Profile</param>
    /// <param name="profile">A semicolon separated string of 0 or 1 (shown) </param>
    /// <param name="fontSize">The FontSize Number (matches the enum)</param>
    /// <param name="placement">The Placement Number (matches the enum)</param>
    public CProfile(string profileName, string profile, int fontSize, int placement )
    {
      LoadProfile(profileName, profile, (GUI.FontSize)fontSize, (GUI.Placement)placement );
    }

    /// <summary>
    /// Load a profile Listbox with checked items from this profile
    /// </summary>
    /// <param name="box">The CListBox</param>
    /// <param name="hudBar">The HudBar</param>
    public void LoadCbx( CheckedListBox box, HudBar hudBar )
    {
      box.Items.Clear( );

      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        int idx = box.Items.Add( hudBar.CfgName( i ) );
        box.SetItemChecked( idx, m_profile[i] );
      }
    }


    /// <summary>
    /// Update this profile from the checked listbox
    /// </summary>
    /// <param name="box">The CListBox</param>
    public void GetItemsFromCbx( CheckedListBox box )
    {
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        m_profile[i] = box.GetItemChecked( (int)i );
      }
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
    /// Update this profile from the FontSize ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetFontSizeFromCombo( ComboBox box )
    {
      m_fontSize = (GUI.FontSize)box.SelectedIndex;
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
    /// Update this profile from the Placement ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetPlacementFromCombo( ComboBox box )
    {
      m_placement = (GUI.Placement)box.SelectedIndex;
    }

    /// <summary>
    /// Returns the profile string 
    /// </summary>
    /// <returns>A profile string </returns>
    public string ProfileString( )
    {
      string ret="";

      foreach ( var kv in m_profile ) {
        ret += kv.Value ? "1;" : "0;";
      }
      return ret;
    }


    public void LoadProfile(string profileName, string profile, GUI.FontSize fontSize, GUI.Placement placement )
    {
      PName = profileName;
      m_fontSize = fontSize;
      m_placement = placement;

      string[] e = profile.Split(new char[]{ ';' }, StringSplitOptions.RemoveEmptyEntries );

      m_profile.Clear( );
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        bool show = true; // default ON
        if ( e.Length > (int)i ) {
          show = e[(int)i] == "1";
        }
        m_profile.Add( i, show );
      }
    }

    /// <summary>
    /// Returns the Show state of an item
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>True if it is shown</returns>
    public bool ShowItem(LItem item )
    {
      return m_profile[item];
    }


  }
}

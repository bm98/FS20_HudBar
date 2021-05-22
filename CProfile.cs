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

    public string PName { get; set; } = "Profile";

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
    /// <param name="profile">A semicolon separated string of 0 or 1 (shown) </param>
    public CProfile(string profileName, string profile )
    {
      LoadProfile(profileName, profile );
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
    /// <param name="hudBar">The HudBar</param>
    public void GetFromCbx( CheckedListBox box, HudBar hudBar )
    {
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        m_profile[i] = box.GetItemChecked( (int)i );
      }
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


    public void LoadProfile(string profileName, string profile )
    {
      PName = profileName;
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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.Bar;
using FS20_HudBar.Bar.Items;

namespace FS20_HudBar
{
  /// <summary>
  /// A settings profile
  /// the checkbox, dropdown values of the user settings per profile
  /// </summary>
  class CProfile
  {
    #region DEFAULT PROFILES
    public enum DProfile
    {
      Profile_All,    // all items checked
      EssentialsProfile,
      CombProfile_A, // most used combustion engine profile
      CombProfile_B, // variant B
      CombProfile_C, // variant C
      TPropProfile_A,
      TPropProfile_C208B,   // has some differences to the generic one
      JetProfile,
      HeavyProfile,
    }

    public class DefaultProfile
    {
      public string Name { get; private set; } = "";
      public string Profile { get; private set; } = "";
      public string DispOrder { get; private set; } = "";
      public string FlowBreak { get; private set; } = "";

      public DefaultProfile( string name, string profile, string dispOrder, string flowBreak )
      {
        Name = name;
        Profile = profile;
        DispOrder = dispOrder;
        FlowBreak = flowBreak;
      }
      public override string ToString( ) => Name;
    }

    private static Dictionary<DProfile, DefaultProfile> _defaultProfileCat = new Dictionary<DProfile, DefaultProfile>(){
      { DProfile.Profile_All, new DefaultProfile("All Items",
        "1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;1;", // disp
        "0;1;3;4;5;71;6;7;8;9;10;20;21;14;15;22;24;16;25;26;44;47;46;53;52;48;55;56;32;35;36;38;42;57;58;59;60;61;62;64;81;82;83;2;12;49;50;51;43;39;69;74;73;11;27;28;40;75;70;33;63;65;66;80;77;79;45;54;76;34;78;41;37;67;68;29;30;31;72;17;18;19;13;23;", // order
        "0;0;0;0;0;0;0;0;0;0;0;1;0;0;0;0;0;0;0;0;1;0;0;0;0;0;0;0;1;0;0;0;0;1;0;0;0;0;0;0;0;0;0;0;1;0;0;0;0;0;1;0;0;0;0;0;0;0;0;0;0;0;0;1;1;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;") }, // flowbreak

      { DProfile.EssentialsProfile, new DefaultProfile("Essentials",
        "1;0;1;0;0;1;0;1;0;0;0;0;0;0;0;0;0;0;0;0;0;1;1;0;1;0;0;0;1;0;1;1;1;1;0;0;0;0;1;1;0;0;0;0;0;0;0;1;0;0;0;0;0;0;0;1;0;0;0;0;0;0;0;0;0;0;0;0;0;1;0;0;0;0;0;0;0;0;0;0;0;0;0;0;",
        "0;1;3;4;5;71;6;7;8;9;10;20;21;14;15;22;24;16;25;26;44;47;46;53;52;48;55;56;32;35;36;38;42;57;58;59;60;61;62;64;81;82;83;2;12;49;50;51;43;39;69;74;73;11;27;28;40;75;70;33;63;65;66;80;77;79;45;54;76;34;78;41;37;67;68;29;30;31;72;17;18;19;13;23;",
        "0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;") },

      { DProfile.CombProfile_A, new DefaultProfile("Prop Easy",
        "1;1;1;1;1;1;0;1;1;1;1;0;0;0;1;0;0;0;0;0;1;1;1;1;1;0;0;0;1;0;1;1;1;1;1;1;1;1;1;1;0;0;0;0;0;0;1;1;0;0;1;0;0;1;0;1;0;0;1;0;1;0;0;0;0;0;1;0;0;1;0;0;0;1;1;0;0;0;0;0;0;0;0;0;",
        "0;1;3;4;5;71;6;7;8;9;10;20;21;14;15;22;24;16;25;26;44;47;46;53;52;48;55;56;32;35;36;38;42;57;58;59;60;61;62;64;81;82;83;2;12;49;50;51;43;39;69;74;73;11;27;28;40;75;70;33;63;65;66;80;77;79;45;54;76;34;78;41;37;67;68;29;30;31;72;17;18;19;13;23;",
        "0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;1;0;0;0;0;0;0;0;0;0;0;0;0;1;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;") },

      { DProfile.CombProfile_B, new DefaultProfile("Prop Direct",
        "1;1;1;1;1;1;0;1;1;1;1;0;0;0;1;0;0;1;0;1;1;1;1;1;1;0;0;0;1;0;0;1;1;1;1;1;1;1;1;1;0;0;0;0;1;0;1;1;0;1;1;0;1;1;1;1;0;0;1;0;1;0;0;0;1;0;1;0;1;1;0;0;1;1;1;0;0;0;0;0;1;0;0;0;",
        "0;1;3;4;5;71;6;7;8;9;10;20;21;14;15;22;24;16;25;26;44;47;46;53;52;48;55;56;32;35;36;38;42;57;58;59;60;61;62;64;81;82;83;2;12;49;50;51;43;39;69;74;73;11;27;28;40;75;70;33;63;65;66;80;77;79;45;54;76;34;78;41;37;67;68;29;30;31;72;17;18;19;13;23;",
        "0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;1;0;0;0;0;1;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;") },

      { DProfile.CombProfile_C, new DefaultProfile("Prop Gearbox",
        "1;1;1;1;1;1;0;1;1;1;1;0;0;1;0;0;0;1;0;1;1;1;1;1;1;0;0;0;1;0;0;1;1;1;1;1;1;1;1;1;0;0;0;0;1;0;1;1;1;1;1;0;1;1;1;1;0;0;1;0;1;1;1;0;1;0;1;0;1;1;0;0;1;1;1;0;0;0;0;0;1;0;1;0;",
        "0;1;3;4;5;71;6;7;8;9;10;20;21;14;15;22;24;16;25;26;44;47;46;53;52;48;55;56;32;35;36;38;42;57;58;59;60;61;62;64;81;82;83;2;12;49;50;51;43;39;69;74;73;11;27;28;40;75;70;33;63;65;66;80;77;79;45;54;76;34;78;41;37;67;68;29;30;31;72;17;18;19;13;23;",
        "0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;1;0;0;0;0;1;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;") },

      { DProfile.TPropProfile_A, new DefaultProfile("TurboProp",
        "1;1;1;1;1;1;0;1;1;1;1;0;1;1;0;1;1;0;0;1;1;1;1;1;1;0;0;0;1;0;0;1;1;1;1;1;1;1;1;1;0;0;0;0;0;0;1;1;1;1;1;0;1;1;1;1;0;0;1;0;1;1;1;0;1;0;1;0;1;1;0;0;1;1;1;0;0;0;0;0;0;0;0;0;",
        "0;1;3;4;5;71;6;7;8;9;10;20;21;14;15;22;24;16;25;26;44;47;46;53;52;48;55;56;32;35;36;38;42;57;58;59;60;61;62;64;81;82;83;2;12;49;50;51;43;39;69;74;73;11;27;28;40;75;70;33;63;65;66;80;77;79;45;54;76;34;78;41;37;67;68;29;30;31;72;17;18;19;13;23;",
        "0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;1;0;0;0;0;1;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;") },

      { DProfile.TPropProfile_C208B, new DefaultProfile("TurboProp 208B",
        "1;1;1;1;1;1;0;1;1;1;1;1;0;1;0;1;1;0;1;0;1;1;1;1;1;0;0;0;1;0;0;1;1;1;1;1;1;1;1;1;0;0;0;0;0;0;1;1;1;1;1;0;1;1;1;1;0;0;1;0;1;1;1;0;1;0;1;0;1;1;0;0;1;1;1;0;0;0;0;0;0;0;0;0;",
        "0;1;3;4;5;71;6;7;8;9;10;20;21;14;15;22;24;16;25;26;44;47;46;53;52;48;55;56;32;35;36;38;42;57;58;59;60;61;62;64;81;82;83;2;12;49;50;51;43;39;69;74;73;11;27;28;40;75;70;33;63;65;66;80;77;79;45;54;76;34;78;41;37;67;68;29;30;31;72;17;18;19;13;23;",
        "0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;1;0;0;0;0;1;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;") },

      { DProfile.JetProfile, new DefaultProfile("Jet",
        "1;1;0;0;0;1;0;1;1;1;1;0;0;0;0;1;1;0;1;0;1;1;1;1;1;1;0;0;1;0;0;1;1;1;1;1;1;1;1;1;0;0;0;0;0;1;1;1;1;1;1;0;1;1;1;1;1;0;1;0;1;1;1;0;1;0;1;0;1;1;0;0;1;1;1;0;0;0;0;0;0;0;0;0;",
        "0;1;3;4;5;71;6;7;8;9;10;20;21;14;15;22;24;16;25;26;44;47;46;53;52;48;55;56;32;35;36;38;42;57;58;59;60;61;62;64;81;82;83;2;12;49;50;51;43;39;69;74;73;11;27;28;40;75;70;33;63;65;66;80;77;79;45;54;76;34;78;41;37;67;68;29;30;31;72;17;18;19;13;23;",
        "0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;1;0;0;0;0;0;0;0;0;0;0;0;0;1;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;") },

      { DProfile.HeavyProfile, new DefaultProfile("Heavy",
        "1;1;0;0;0;1;0;1;1;1;1;0;0;0;0;1;1;0;1;0;1;1;1;1;1;1;0;0;1;0;0;1;1;1;1;1;1;1;1;1;0;0;0;0;0;1;1;1;1;1;1;0;1;1;1;1;1;0;1;0;1;1;1;0;1;0;1;0;1;1;0;0;1;1;1;0;0;0;0;0;0;0;0;1;",
        "0;1;3;4;5;71;6;7;8;9;10;20;21;14;15;22;24;16;25;26;44;47;46;53;52;48;55;56;32;35;36;38;42;57;58;59;60;61;62;64;81;82;83;2;12;49;50;51;43;39;69;74;73;11;27;28;40;75;70;33;63;65;66;80;77;79;45;54;76;34;78;41;37;67;68;29;30;31;72;17;18;19;13;23;",
        "0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;1;0;0;0;0;0;0;0;0;0;0;0;0;1;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;0;") },
    };

    /// <summary>
    /// Return a default profile 
    /// </summary>
    /// <param name="profile">The profile ID</param>
    /// <returns>The DefaultProfile or null</returns>
    public static DefaultProfile GetDefaultProfile( DProfile profile )
    {
      if ( !_defaultProfileCat.ContainsKey( profile ) ) return null;
      return _defaultProfileCat[profile];
    }

    /// <summary>
    /// Returns a default profile from a profile name
    /// </summary>
    /// <param name="profileName">The profile Name</param>
    /// <returns>The DefaultProfile or null</returns>
    public static DefaultProfile GetDefaultProfile( string profileName )
    {
      var dp = _defaultProfileCat.Where(x=> x.Value.Name == profileName );
      if ( dp.Count() > 0 ) {
        return dp.ElementAt(0).Value;
      }
      return null;
    }

    #endregion

    private int m_pNumber = 0; // Profile Number 1...

    private Dictionary<LItem, bool> m_profile = new Dictionary<LItem, bool>();   // true to show the item
    private Dictionary<LItem, bool> m_flowBreak = new Dictionary<LItem, bool>(); // true to break the flow
    private Dictionary<LItem, int> m_sequence = new Dictionary<LItem, int>();    // display position 0...

    private GUI.FontSize m_fontSize = GUI.FontSize.Regular;
    private GUI.Placement m_placement = GUI.Placement.Bottom;
    private GUI.Kind m_kind = GUI.Kind.Bar;
    private Point m_location = new Point(0,0);
    private bool m_condensed = false;
    private GUI.Transparent m_transparent = GUI.Transparent.T0;

    public string PName { get; set; } = "Profile";
    public GUI.FontSize FontSize => m_fontSize;
    public GUI.Placement Placement => m_placement;
    public GUI.Kind Kind => m_kind;
    public Point Location => m_location;
    public bool Condensed => m_condensed;
    public GUI.Transparent Transparency => m_transparent;
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
    /// The Key or Name of the CheckBox
    /// </summary>
    /// <param name="item">An Item</param>
    /// <returns>The Key/Name</returns>
    private static string Key( int pNum, LItem item )
    {
      return $"P{pNum}_{item}";
    }
    /// <summary>
    /// The Key or Name of the CheckBox
    /// </summary>
    /// <param name="item">An Item</param>
    /// <returns>The Key/Name</returns>
    private string Key( LItem item )
    {
      return Key( m_pNumber, item );
    }


    /// <summary>
    /// Update this profile from the FLPanel
    /// </summary>
    /// <param name="flp">The FLPanel</param>
    public void GetItemsFromFlp( FlowLayoutPanel flp )
    {
      // process along the Enum sequence
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        // find the item with its Key in the layout panel - no checks - may break if not consistent
        try {
          var cb = flp.Controls[Key(i)] as CheckBox;
          int pos = flp.Controls.IndexOf(cb); // can be -1 if not found
          if ( pos >= 0 ) {
            // store along the Enum sequence
            m_sequence[i] = pos;
            m_profile[i] = cb.Checked;
            m_flowBreak[i] = (string)cb.Tag == "1";
          }
        }
        catch {
          m_sequence[i] = (int)i; m_profile[i] = true; m_flowBreak[i] = false; // dummys anyway - DEBUG STOP
        }
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
    /// Update this profile from the Placement ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetPlacementFromCombo( ComboBox box )
    {
      m_placement = (GUI.Placement)box.SelectedIndex;
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
    /// Update this profile from the Condensed ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetCondFromCombo( ComboBox box )
    {
      m_condensed = box.SelectedIndex == 1;
    }

    /// <summary>
    /// Update this profile from the Transparent ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetTramsFromCombo( ComboBox box )
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
      string[] e = profile.Split(new char[]{ ';' }, StringSplitOptions.RemoveEmptyEntries );
      m_profile.Clear( );
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        bool show = (i== LItem.MSFS)?true : false; // default OFF except the first 20210708
        if ( e.Length > (int)i ) {
          show = e[(int)i] == "1"; // found an element in the string
        }
        m_profile.Add( i, show );
      }

      // Get the flow break  status for each item
      e = flowBreak.Split( new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries );
      m_flowBreak.Clear( );
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        bool fbreak = false; // default OFF
        if ( e.Length > (int)i ) {
          fbreak = e[(int)i] == "1"; // found an element in the string
        }
        m_flowBreak.Add( i, fbreak );
      }

      // Get the item position - don't validate the sequence here
      e = sequence.Split( new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries );
      m_sequence.Clear( );
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        int idx = (int)i; // default Enum Sequence
        if ( e.Length > idx ) {
          if ( int.TryParse( e[idx], out int iPos ) ) {
            if ( iPos < m_profile.Count )
              m_sequence.Add( i, iPos ); // found an integer
            else
              m_sequence.Add( i, m_profile.Count - 1 ); // found an integer - was out of the range ??
          }
          else {
            m_sequence.Add( i, idx ); // default
          }
        }
        else {
          m_sequence.Add( i, idx ); // default
        }
      }
    }


    /// <summary>
    /// Returns the Show state of an item
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>True if it is shown</returns>
    public bool ShowItem( LItem item )
    {
      try {
        return m_profile[item];
      }
      catch {
        return false;
      }
    }

    /// <summary>
    /// Returns the FlowBreak state of an item
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>True if it breaks</returns>
    public bool BreakItem( LItem item )
    {
      try {
        return m_flowBreak[item];
      }
      catch {
        return false;
      }
    }

    /// <summary>
    /// Returns the Items Display position
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>The display position 0..</returns>
    public int ItemPos( LItem item )
    {
      try {
        return m_sequence[item];
      }
      catch {
        return 999;
      }
    }

    /// <summary>
    /// Returns the Items Key for a Display position
    /// </summary>
    /// <param name="item">A position 0..</param>
    /// <returns>The display item key</returns>
    public LItem ItemKeyFromPos( int pos )
    {
      // find the item to be shown at this position (find the pos value in m_sequence and get the item with it's Key)
      if ( m_sequence.ContainsValue( pos ) ) {
        return m_sequence.Where( x => x.Value == pos ).FirstOrDefault( ).Key;
      }
      else {
        // no such item ????
        return (LItem)999;
      }
    }

    /// <summary>
    /// Returns the profile Settings string 
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

    /// <summary>
    /// Returns the flowBreak Settings string 
    /// </summary>
    /// <returns>A flowBreak string </returns>
    public string FlowBreakString( )
    {
      string ret="";

      foreach ( var kv in m_flowBreak ) {
        ret += kv.Value ? "1;" : "0;";
      }
      return ret;
    }

    /// <summary>
    /// Returns the item position Settings string 
    /// </summary>
    /// <returns>A item position string </returns>
    public string ItemPosString( )
    {
      string ret="";

      foreach ( var kv in m_sequence ) {
        ret += $"{kv.Value};";
      }
      return ret;
    }

    /// <summary>
    /// Returns a list orderd along the item Pos, the list contains the LItem Key to be shown at this pos (0 based)
    /// Kind of Transposing the sequence list
    /// </summary>
    /// <returns></returns>
    public List<LItem> ItemPosList( )
    {
      var ret = new List<LItem>();

      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        // we use the Enum only as position index 0... max here
        int idx = (int)i;
        // find the item to be shown at this position (find the pos value in m_sequence and get the item with it's Key)
        if ( m_sequence.ContainsValue( idx ) ) {
          // the item to put as next one / Add sequentially to the layout panel
          ret.Add( ItemKeyFromPos( idx ) );
        }
        else {
          // no such item ????
          ;
        }
      }
      return ret;
    }


  }
}

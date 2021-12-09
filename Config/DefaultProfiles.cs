﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.Config
{
  /// <summary>
  /// Default Profile IDs
  /// </summary>
  public enum DProfile
  {
    Profile_1=0,
    Profile_2,
    Profile_3,
    Profile_4,
    Profile_5,    // all items checked
  }

  /// <summary>
  /// The Default Profiles
  /// </summary>
  class DefaultProfiles
  {

    // NOTE Use the CProfile.Divider, FlowBreak, NoBreak chars to split the items

    public const string DefaultProfileName = "DefaultProfiles.csv";

    /// <summary>
    /// cTor: static init of the merge database from an embedded resource
    /// </summary>
    static DefaultProfiles( )
    {
      LoadDefaultProfiles( );
    }

    // NOTE Use the CProfile.Divider chars to split the profile string

    /// <summary>
    /// Load the embedded CSV that defines the default Profiles
    /// </summary>
    private static void LoadDefaultProfiles( )
    {
      Stream csvStream;
      if ( File.Exists( DefaultProfileName ) ) {
        // user provided one
        csvStream = File.OpenRead( DefaultProfileName );
      }
      else {
        // get the built in
        csvStream = typeof( Program ).Assembly.GetManifestResourceStream( "FS20_HudBar.Config." + DefaultProfileName );
      }

      using ( var sr = new StreamReader( csvStream ) ) {
        string line = sr.ReadLine(); // Header
        line = sr.ReadLine( );
        DProfile tag = (DProfile)0; // start with the first
        // we have 4 lines per profile
        while ( line != null ) {
          string pName=line;
          string profile = sr.ReadLine().Replace(',', CProfile.Divider); // allow to read comma and semi
          string order = sr.ReadLine().Replace(',', CProfile.Divider); // allow to read comma and semi
          string flowBreak = sr.ReadLine().Replace(',', CProfile.Divider); // allow to read comma and semi
          _defaultProfileCat.Add( tag, new ProfileStore( pName, profile, order, flowBreak ) );
          // next
          line = sr.ReadLine( );
          tag++;
        }
      }
    }

    private static Dictionary<DProfile, ProfileStore> _defaultProfileCat = new Dictionary<DProfile, ProfileStore>();

    /// <summary>
    /// Add all menuitems to the dropdown menu
    /// </summary>
    /// <param name="menu">A TS MenuItem</param>
    public static void AddMenuItems( ToolStripMenuItem menu, EventHandler onClick )
    {
      for ( int i = 0; i < _defaultProfileCat.Count; i++ ) {
        menu.DropDownItems.Add( GetDefaultProfile( (DProfile)i ).Name, null, onClick );
      }
    }


    /// <summary>
    /// Return a default profile 
    /// </summary>
    /// <param name="profile">The profile ID</param>
    /// <returns>The ProfileStore or null</returns>
    public static ProfileStore GetDefaultProfile( DProfile profile )
    {
      if ( !_defaultProfileCat.ContainsKey( profile ) ) return null;
      return _defaultProfileCat[profile];
    }

    /// <summary>
    /// Returns a default profile from a profile name
    /// </summary>
    /// <param name="profileName">The profile Name</param>
    /// <returns>The ProfileStore or null</returns>
    public static ProfileStore GetDefaultProfile( string profileName )
    {
      var dp = _defaultProfileCat.Where(x=> x.Value.Name == profileName );
      if ( dp.Count( ) > 0 ) {
        return dp.First( ).Value;
      }
      return null;
    }


  }
}

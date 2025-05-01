using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace FS20_HudBar.Config
{
  /// <summary>
  /// IDs of Aircrafts where we know merging profiles for parts of the items
  /// </summary>
  public enum DAircraft
  {
    DUMMY=0, // to create an Enum of this type 
  }

  /// <summary>
  /// The Aircraft Merge profiles
  /// </summary>
  class AcftMerges
  {
    private const string c_mergeFileName = "EngineMerge.csv";

    /// <summary>
    /// cTor: static init of the merge database from an embedded resource
    /// </summary>
    static AcftMerges( )
    {
      LoadEngineFile( );
    }

    // NOTE Use the CProfile.Divider chars to split the profile string

    /// <summary>
    /// Load the embedded CSV that defines the Merges for Aircrafst (mostly Engine based items)
    /// </summary>
    private static void LoadEngineFile( )
    {
      Stream csvStream;
      if ( File.Exists( c_mergeFileName ) ) {
        // user provided one
        csvStream = File.OpenRead( c_mergeFileName );
      }
      else {
        // get the built in
        csvStream = typeof( Program ).Assembly.GetManifestResourceStream( "FS20_HudBar.Config." + c_mergeFileName );
      }
      using ( var sr = new StreamReader( csvStream ) ) {
        string line = sr.ReadLine(); // Header
        line = sr.ReadLine( );
        DAircraft tag = DAircraft.DUMMY;
        while ( line != null ) {
          // Load
          string[] e = line.Split(new char[]{CProfile.Divider, ',' } ); // allow to read commas and semi
          string profile = "";
          string acft;
          for ( int i = 0; i < e.Length; i++ ) {
            if ( e[i] == "X" ) {
              acft = e[e.Length - 1]; // last element
              if ( acft.Length > 1 ) {
                // only if we have a name (min 2chars long)
                _aircraftProfileCat.Add( new ProfileItemsStore( acft, profile, "", "" ) );
              }
              break;
            }
            else {
              profile += e[i] + CProfile.Divider;
            }
          }
          // next
          line = sr.ReadLine( );
          tag++;
        }
      }
    }

    /// <summary>
    /// The Aircraft Merging Profiles we have provided
    ///  NOTE: only the Profile item is used, the others are not merged as we don't rearrange things while merging
    /// </summary>
    private static List<ProfileItemsStore> _aircraftProfileCat = new List<ProfileItemsStore>();

    /// <summary>
    /// Add all menuitems to the dropdown menu
    /// </summary>
    /// <param name="menu">A TS MenuItem</param>
    public static void AddMenuItems( ToolStripMenuItem menu, EventHandler onClick )
    {
      for ( int i = 0; i < _aircraftProfileCat.Count; i++ ) {
        menu.DropDownItems.Add( _aircraftProfileCat[i].Name, null, onClick );
      }
    }

    /// <summary>
    /// Returns a merge profile from an aircraft name
    /// </summary>
    /// <param name="acftName">The profile Name</param>
    /// <returns>The ProfileStore or null</returns>
    public static ProfileItemsStore GetAircraftProfile( string acftName )
    {
      var dp = _aircraftProfileCat.Where(x=> x.Name == acftName );
      if ( dp.Count( ) > 0 ) {
        return dp.First( );
      }
      return null;
    }
  }
}

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DbgLib;

namespace FlightplanLib.MS
{
  /// <summary>
  /// Tool to access the MSFS folders
  /// </summary>
  public static class MsFolders
  {
    private static DbgLogger DBG = new DbgLogger( typeof( MsFolders ) );

    /// <summary>
    /// The Packages base path (i.e. where Community and Official folders are)
    /// </summary>
    private static string _packageBasePath = "";

    /// <summary>
    /// Override the internal logic to find the path
    /// (i.e. where Community and Official folders are)
    /// </summary>
    /// <param name="ipkPath">A path to MSFS Package base</param>
    /// <returns>True if it seems a legit location</returns>
    public static bool OverrideInstalledPackagesPath( string ipkPath )
    {
      if (Directory.Exists( ipkPath )) {
        // STORE
        if (Directory.Exists( Path.Combine( ipkPath, "Official", "OneStore" ) )) {
          // seems legit
          _packageBasePath = ipkPath;
          return true;
        }
        else {
          // STEAM
          if (Directory.Exists( Path.Combine( ipkPath, "Official", "Steam" ) )) {
            // seems legit
            _packageBasePath = ipkPath;
            return true;
          }
          else {
            DBG.LogError( $"OverrideInstalledPackagesPath: Official\\OneStore or Official\\Steam cannot be found in ({ipkPath})" );
          }
        }
      }
      else {
        DBG.LogError( $"OverrideInstalledPackagesPath: Given path does not exist ({ipkPath})" );
      }
      return false;
    }

    /// <summary>
    /// Reset the path with internal logic
    /// </summary>
    public static void ResetInstalledPackagesPath( )
    {
      DBG.Log( $"ResetInstalledPackagesPath: Initialize/Reset package path " );
      _packageBasePath = GetInstalledPackagesPath_Int( );// init with internal logic
    }

    /// <summary>
    /// static ctor:
    /// </summary>
    static MsFolders( )
    {
      ResetInstalledPackagesPath( );
    }

    #region Install Path

    /// <summary>
    /// Gets the Installation Path for Standard MSFS Location
    /// 
    /// C:\Users\USER\AppData\Local\Packages\Microsoft.FlightSimulator_8wekyb3d8bbwe
    /// 
    /// </summary>
    /// <returns>A path found or an empty string</returns>
    private static string GetStoreMSFSInstallPath_Int( )
    {
      var pPath = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );
      pPath = Path.Combine( pPath, "Packages", "Microsoft.FlightSimulator_8wekyb3d8bbwe" );

      if (Directory.Exists( pPath )) {
        DBG.Log( $"GetStoreMSFSInstallPath_Int: Install Path is STORE ({pPath})" );
        return pPath;
      }

      return ""; // nope
    }

    /// <summary>
    /// Gets the Installation Path for STEAM Location
    /// 
    /// C:\Users\USER\AppData\Roaming\Microsoft Flight Simulator
    /// 
    /// </summary>
    /// <returns>A path found or an empty string</returns>
    private static string GetSteamMSFSInstallPath_Int( )
    {
      var pPath = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
      pPath = Path.Combine( pPath, "Microsoft Flight Simulator" );

      if (Directory.Exists( pPath )) {
        DBG.Log( $"GetSteamMSFSInstallPath_Int: Install Path is STEAM ({pPath})" );
        return pPath;
      }

      return ""; // nope
    }

    /// <summary>
    /// MS Store Location
    /// 
    /// [InstallPath]\LocalCache\UserCfg.opt
    /// 
    /// </summary>
    /// <returns>A path or an empty string if not found</returns>
    private static string GetStoreInstalledPackagesPath_Int( )
    {
      var instPath = GetStoreMSFSInstallPath_Int( );
      if (string.IsNullOrEmpty( instPath )) {
        DBG.Log( $"GetStoreInstalledPackagesPath_Int: Cannot find STORE InstallPath" );
        return "";
      }

      string ipPath = "";
      instPath = Path.Combine( instPath, "LocalCache" );
      var optFile = Path.Combine( instPath, "UserCfg.opt" );
      if (File.Exists( optFile )) {
        using (var sr = new StreamReader( optFile )) {
          while (!sr.EndOfStream) {
            var buf = sr.ReadLine( ).Trim( );
            if (buf.StartsWith( "InstalledPackagesPath" )) {
              buf = buf.Replace( "InstalledPackagesPath", "" );
              ipPath = buf.Replace( "\"", "" ).Trim( );
            }
          }
        }
      }
      else {
        DBG.LogError( $"GetStoreInstalledPackagesPath_Int: Cannot find UserCfg.opt ({optFile})" );
      }

      return ipPath;
    }

    /// <summary>
    /// Steam Location
    /// 
    /// [InstallPath]\UserCfg.opt
    /// 
    /// </summary>
    /// <returns>A path or an empty string if not found</returns>
    private static string GetSteamInstalledPackagesPath_Int( )
    {
      var instPath = GetSteamMSFSInstallPath_Int( );
      if (string.IsNullOrEmpty( instPath )) {
        DBG.Log( $"GetSteamInstalledPackagesPath_Int: Cannot find STEAM InstallPath" );
        return "";
      }

      string ipPath = "";
      var optFile = Path.Combine( instPath, "UserCfg.opt" );
      if (File.Exists( optFile )) {
        using (var sr = new StreamReader( optFile )) {
          while (!sr.EndOfStream) {
            var buf = sr.ReadLine( ).Trim( );
            if (buf.StartsWith( "InstalledPackagesPath" )) {
              buf = buf.Replace( "InstalledPackagesPath", "" );
              ipPath = buf.Replace( "\"", "" ).Trim( );
            }
          }
        }
      }
      else {
        DBG.LogError( $"GetSteamInstalledPackagesPath_Int: Cannot find UserCfg.opt ({optFile})" );
      }

      return ipPath;
    }

    /// <summary>
    /// Get the Package Installation Base Path
    /// </summary>
    /// <returns></returns>
    private static string GetInstalledPackagesPath_Int( )
    {
      // try STORE
      var ipkPath = GetStoreInstalledPackagesPath_Int( );
      if (string.IsNullOrEmpty( ipkPath )) {
        DBG.Log( $"GetInstalledPackagesPath_Int: Not found for STORE -- try STEAM" );
        // try STEAM
        ipkPath = GetSteamInstalledPackagesPath_Int( );
      }

      if (string.IsNullOrEmpty( ipkPath )) {
        DBG.LogError( $"GetInstalledPackagesPath_Int: Cannot find any InstalledPackagesPath" );
      }
      else {
        DBG.Log( $"GetInstalledPackagesPath_Int: USING ({ipkPath})" );
      }

      return ipkPath;
    }

    /// <summary>
    /// Get the Package Installation Base Path
    /// </summary>
    /// <returns></returns>
    public static string GetInstalledPackagesPath( ) => _packageBasePath;

    #endregion

    #region Data Path

    /// <summary>
    /// Gets the Data Path for Standard MSFS Location
    /// 
    ///  C:\Users\YOURUSERNAME\AppData\Local\Packages\Microsoft.FlightSimulator_8wekyb3d8bbwe\LocalState\ ... MISSIONS\Custom\CustomFlight
    /// 
    /// </summary>
    /// <returns>A path found or an empty string</returns>
    private static string GetStoreMSFSDataPath_Int( )
    {
      var pPath = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );
      pPath = Path.Combine( pPath, "Packages", "Microsoft.FlightSimulator_8wekyb3d8bbwe", "LocalState" );

      if (Directory.Exists( pPath )) {
        DBG.Log( $"GetStoreMSFSDataPath_Int: Data Path is STORE ({pPath})" );
        return pPath;
      }

      return ""; // nope
    }

    /// <summary>
    /// Gets the Data Path for STEAM Location
    /// 
    /// C:\Users\YOURUSERNAME\AppData\Roaming\Microsoft Flight Simulator\ ... MISSIONS\Custom\CustomFlight
    /// 
    /// </summary>
    /// <returns>A path found or an empty string</returns>
    private static string GetSteamMSFSDataPath_Int( )
    {
      var pPath = Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData );
      pPath = Path.Combine( pPath, "Microsoft Flight Simulator" );

      if (Directory.Exists( pPath )) {
        DBG.Log( $"GetSteamMSFSDataPath_Int: Data Path is STEAM ({pPath})" );
        return pPath;
      }

      return ""; // nope
    }

    /// <summary>
    /// Get the Data Base Path
    /// </summary>
    /// <returns></returns>
    private static string GetDataPath_Int( )
    {
      // try STORE
      var ipkPath = GetStoreMSFSDataPath_Int( );
      if (string.IsNullOrEmpty( ipkPath )) {
        DBG.Log( $"GetDataPath_Int: Not found for STORE -- try STEAM" );
        // try STEAM
        ipkPath = GetSteamMSFSDataPath_Int( );
      }

      if (string.IsNullOrEmpty( ipkPath )) {
        DBG.LogError( $"GetDataPath_Int: Cannot find any InstalledPackagesPath" );
      }
      else {
        DBG.Log( $"GetDataPath_Int: USING ({ipkPath})" );
      }

      return ipkPath;
    }


    #endregion

    #region OneStore

    /// <summary>
    /// True if it seems an MS OneStore file
    /// </summary>
    public static bool IsMS( string filename ) => filename.Contains( @"\Official\OneStore" ) || filename.Contains( @"\Official\Steam" );

    /// <summary>
    /// True if it seems an MS Addon file
    /// </summary>
    public static bool IsMS_addon( string filename ) => filename.Contains( @"\microsoft-" );

    /// <summary>
    /// True if it seems an Asobo Addon file
    /// </summary>
    public static bool IsAsobo_addon( string filename ) => filename.Contains( @"\asobo-" );

    /// <summary>
    /// Returns the OneStore base path of the MS FS Installation
    /// 
    /// STORE:
    /// [PackagePath]\Official\OneStore
    /// 
    /// STEAM:
    /// [PackagePath]\Official\Steam
    /// 
    /// </summary>
    /// <returns>A path or an empty string</returns>
    public static string GetMSStorePath( )
    {
      string ipkPath = GetInstalledPackagesPath( );
      // if the path is empty - we could not find a source
      if (string.IsNullOrEmpty( ipkPath )) {
        DBG.LogError( $"GetMSStorePath: Cannot find any InstalledPackagesPath" );
        return ""; // nope..
      }

      // try STORE
      var osPath = Path.Combine( ipkPath, "Official", "OneStore" ); // MS files root
      if (!Directory.Exists( osPath )) {
        // try STEAM
        osPath = Path.Combine( ipkPath, "Official", "Steam" ); // Steam MS files root

        if (!Directory.Exists( osPath )) {
          DBG.LogError( $"GetMSStorePath: Cannot find any MS content path (neither OneStore nor Steam)" );
          return ""; // nope
        }
      }
      DBG.Log( $"GetMSStorePath: MS content path is ({osPath})" );

      return osPath;
    }


    /// <summary>
    /// Returns the fs-base base path of the MS FS Installation
    /// </summary>
    /// <returns>A path or an empty string</returns>
    public static string GetFsBasePath( )
    {
      string osPath = GetMSStorePath( );
      if (string.IsNullOrEmpty( osPath )) {
        DBG.LogError( $"GetFsBasePath: No MS content path found" );
        return ""; // nope..
      }

      string scPath = Path.Combine( osPath, @"fs-base" );
      if (!Directory.Exists( scPath )) {
        DBG.LogError( $"GetFsBasePath: MS content \\fs-base does not exist" );
        return ""; // nope
      }

      return scPath;
    }

    /// <summary>
    /// Returns the NAV base path of the MS FS Installation
    /// </summary>
    /// <returns>A path or an empty string</returns>
    public static string GetNavBasePath( )
    {
      string osPath = GetMSStorePath( );
      if (string.IsNullOrEmpty( osPath )) {
        DBG.LogError( $"GetNavBasePath: No MS content path found" );
        return ""; // nope..
      }

      string scPath = Path.Combine( osPath, @"fs-base-nav" );
      if (!Directory.Exists( scPath )) {
        DBG.LogError( $"GetNavBasePath: MS content \\fs-base-nav does not exist" );
        return ""; // nope
      }

      return scPath;
    }

    /// <summary>
    /// Returns the Generic Airport NAV base path of the MS FS Installation
    /// </summary>
    /// <returns>A path or an empty string</returns>
    public static string GetGenAptBasePath( )
    {
      string osPath = GetMSStorePath( );
      if (string.IsNullOrEmpty( osPath )) {
        DBG.LogError( $"GetGenAptBasePath: No MS content path found" );
        return ""; // nope..
      }

      string scPath = Path.Combine( osPath, @"fs-base-genericairports" );
      if (!Directory.Exists( scPath )) {
        DBG.LogError( $"GetGenAptBasePath: MS content \\fs-base-genericairports does not exist" );
        return ""; // nope
      }

      return scPath;
    }

    /// <summary>
    /// Returns the Scenery base path of the MS FS Installation
    /// </summary>
    /// <returns>A path or an empty string</returns>
    public static string GetSceneryBasePath( )
    {
      string osPath = GetMSStorePath( );
      if (string.IsNullOrEmpty( osPath )) {
        DBG.LogError( $"GetSceneryBasePath: No MS content path found" );
        return ""; // nope..
      }

      string scPath = Path.Combine( osPath, "fs-base", "scenery" );
      if (!Directory.Exists( scPath )) {
        DBG.LogError( $"GetSceneryBasePath: MS content \\fs-base\\scenery does not exist" );
        return ""; // nope
      }

      return scPath;
    }

    #endregion

    #region Community 

    /// <summary>
    /// True if it seems a Community file
    /// </summary>
    public static bool IsCommunity( string filename ) => filename.Contains( @"\Community" ) && !IsNavigraph( filename );

    /// <summary>
    /// Returns the Scenery base path of the MS FS Community Files Contents
    /// TAKE CARE, includes Navigraph 
    /// 
    /// STORE:
    /// [PackagePath]\Community
    /// 
    /// STEAM:
    /// [PackagePath]\Community
    /// 
    /// </summary>
    /// <returns>A path or an empty string</returns>
    public static string GetCommunityPath( )
    {
      string ipkPath = GetInstalledPackagesPath( );
      // if the path is empty - we could not find a source
      if (string.IsNullOrEmpty( ipkPath )) {
        DBG.LogError( $"GetCommunityPath: Cannot find any InstalledPackagesPath" );
        return ""; // nope..
      }

      string coPath = Path.Combine( ipkPath, @"Community" ); // Community files root
      if (!Directory.Exists( coPath )) {
        DBG.LogError( $"GetCommunityPath: \\Community path does not exist" );
        return ""; // nope
      }

      return coPath;
    }

    #endregion

    #region Navigraph

    /// <summary>
    /// True if it seems a Navigraph file
    /// </summary>
    public static bool IsNavigraph( string filename ) => filename.Contains( @"\Community\navigraph" );

    /// <summary>
    /// Returns the NAV base path of the MS FS Navigraph Files Base
    /// </summary>
    /// <returns>A path or an empty string</returns>
    public static string GetNavBasePathNaviGraph( )
    {
      string coPath = GetCommunityPath( );
      // if the path is empty - we could not find a source
      if (string.IsNullOrEmpty( coPath )) {
        DBG.LogError( $"GetNavBasePathNaviGraph: No Community path found" );
        return ""; // nope..
      }

      string naPath = Path.Combine( coPath, "navigraph-navdata-base", "scenery" );
      if (!Directory.Exists( naPath )) {
        DBG.Log( $"GetNavBasePathNaviGraph: Community \\navigraph-navdata-base\\scenery does not exist" );
        return ""; // nope
      }

      return naPath;
    }

    /// <summary>
    /// Returns the Scenery base path of the MS FS Navigraph Files Contents
    /// </summary>
    /// <returns>A path or an empty string</returns>
    public static string GetSceneryBasePathNaviGraph( )
    {
      string coPath = GetCommunityPath( );
      // if the path is empty - we could not find a source
      if (string.IsNullOrEmpty( coPath )) {
        DBG.LogError( $"GetSceneryBasePathNaviGraph: No Community path found" );
        return ""; // nope..
      }

      string naPath = Path.Combine( coPath, "navigraph-navdata", "scenery" );
      if (!Directory.Exists( naPath )) {
        DBG.Log( "GetSceneryBasePathNaviGraph: Community \\navigraph-navdata\\scenery does not exist" );
        return ""; // nope
      }

      return naPath;
    }

    #endregion

    #region Mission/Flights Folder

    /// <summary>
    /// Returns the Path to the MISSIONS folder
    /// </summary>
    /// <returns>A path or an empty string</returns>
    public static string GetMissionFolder( )
    {
      // <DataPath>\MISSIONS
      string daPath = GetDataPath_Int( );
      if (string.IsNullOrEmpty( daPath )) {
        DBG.LogError( $"GetMissionFolder: Cannot find any DataPath" );
        return "";
      }

      string miPath = Path.Combine( daPath, "MISSIONS" );
      if (Directory.Exists( miPath )) {
        return miPath;
      }
      else {
        DBG.LogError( $"GetMissionFolder: Cannot find MISSIONS path" );
        return "";
      }
    }

    /// <summary>
    /// Returns the path to the CustomFlight folder
    /// </summary>
    /// <returns>A path or an empty string</returns>
    public static string GetCustomFlightFolder( )
    {
      // <MissionsPath>\Custom\CustomFlight
      string miPath = GetMissionFolder( );
      if (string.IsNullOrEmpty( miPath )) {
        DBG.LogError( $"GetCustomFlightFolder: Cannot find any NISSIONS path" );
        return "";
      }

      string cfPath = Path.Combine( miPath, "Custom", "CustomFlight" );
      if (Directory.Exists( cfPath )) {
        return cfPath;
      }
      else {
        DBG.LogError( $"GetCustomFlightFolder: Cannot find CustomFlight path" );
        return "";
      }
    }

    /// <summary>
    /// Filename of the Custom Flight file
    /// </summary>
    public const string CustomFlightPlan = "CustomFlight.pln";


    /// <summary>
    /// Returns the CustomFlight Plan file
    /// </summary>
    /// <returns>A path or an empty string</returns>
    public static string GetCustomFlight_Plan( )
    {
      var cfPath = GetCustomFlightFolder( );
      if (string.IsNullOrEmpty( cfPath )) {
        DBG.LogError( $"GetCustomFlight_Plan: Cannot find any CustomFlight path" );
        return "";
      }

      string cfFile = Path.Combine( cfPath, CustomFlightPlan );
      if (File.Exists( cfFile )) {
        return cfFile;
      }
      else {
        DBG.LogError( $"GetCustomFlight_Plan: Cannot find any {CustomFlightPlan} file" );
        return "";
      }
    }

    #endregion

  }
}

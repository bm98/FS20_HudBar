using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;

namespace FS20_HudBar
{
  /// <summary>
  /// HudBar Apps - Folders Used
  ///  TODO - change the setting filename below
  ///  TODO - may be change the namespace
  /// </summary>
  internal static class Folders
  {
    #region HUDBAR FILE LOCATIONS
#pragma warning disable CS0168 // Variable is declared but never used
    // settings
    // @@@@@@ TODO - change to proper name
    private const string c_settingFile = "HudBarAppSettings.json";
    // @@@@@@

    // HudBar file locations in MyDocuments
    private const string c_HudBarFolder = @"MSFS_HudBarSave";
    private const string c_HudBarDbFolder = "db";
    private const string c_HudBarCacheFolder = "cache";
    private const string c_HudBarSettingsFolder = "settings";
    // MyDocuments folder
    private static readonly string c_MyDocuments = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );

    // Path to MyDocuments\ ...
    private static string _hudBarDocs = "";
    // database
    private const string c_GenAptDBNameDblite = @"fs2020genApt.dblite";
    private static string _genAptDBPath = ""; // will hold the final path
    private static string _genAptDBFile = ""; // complete path and file

    private static string _settingsPath = ""; // will hold the final path
    private static string _settingsFile = ""; // complete path and file

    // cache
    private static string _cachePath = ""; // will hold the final path

    /// <summary>
    /// Initialize all HudBar files and locations
    /// Call in cTor:
    /// </summary>
    private static void InitStorage( )
    {
      // main 
      _hudBarDocs = Path.Combine( c_MyDocuments, c_HudBarFolder );
      try {
        // make sure the database path exists - but never fail..
        if (!Directory.Exists( _hudBarDocs )) {
          Directory.CreateDirectory( _hudBarDocs );
        }
      }
      catch (Exception ex) {
        _hudBarDocs = Path.GetFullPath( @".\" ); // app Dir - at least a valid location..
      }
      // database
      _genAptDBPath = Path.Combine( _hudBarDocs, c_HudBarDbFolder );
      try {
        // make sure the database path exists - but never fail..
        if (!Directory.Exists( _genAptDBPath )) {
          Directory.CreateDirectory( _genAptDBPath );
        }
      }
      catch (Exception ex) {
        _genAptDBPath = Path.GetFullPath( @".\" ); // app Dir - at least a valid location..
      }
      _genAptDBFile = Path.Combine( _genAptDBPath, c_GenAptDBNameDblite );

      // settings
      _settingsPath = Path.Combine( _hudBarDocs, c_HudBarSettingsFolder );
      try {
        // make sure the settings path exists - but never fail..
        if (!Directory.Exists( _settingsPath )) {
          Directory.CreateDirectory( _settingsPath );
        }
      }
      catch (Exception ex) {
        _settingsPath = Path.GetFullPath( @".\" ); // app Dir - at least a valid location..
      }
      _settingsFile = Path.Combine( _settingsPath, c_settingFile );

      // caches 
      _cachePath = Path.Combine( _hudBarDocs, c_HudBarCacheFolder );
      try {
        // make sure the settings path exists - but never fail..
        if (!Directory.Exists( _cachePath )) {
          Directory.CreateDirectory( _cachePath );
        }
      }
      catch (Exception ex) {
        _cachePath = Path.GetFullPath( @".\" ); // app Dir - at least a valid location..
      }
    }

#pragma warning restore CS0168 // Variable is declared but never used
    #endregion


    static Folders( )
    {
      InitStorage( );
    }

    /// <summary>
    /// Returns the Path to save files for the User
    /// </summary>
    /// <returns>A path</returns>
    public static string UserFilePath => _hudBarDocs;
    /// <summary>
    /// Path to DB files
    /// </summary>
    public static string DBPath => _genAptDBPath;
    /// <summary>
    /// Path to Cache files
    /// </summary>
    public static string CachePath => _cachePath;
    /// <summary>
    /// Path to Settings files
    /// </summary>
    public static string SettingsPath => _settingsPath;

    /// <summary>
    /// GenAptDB File with path
    /// </summary>
    public static string GenAptDBFile => _genAptDBFile;
    /// <summary>
    /// Settings file + path 
    /// NOTE: Set the app specific Settings File Name 
    /// </summary>
    public static string SettingsFile => _settingsFile;


  }
}

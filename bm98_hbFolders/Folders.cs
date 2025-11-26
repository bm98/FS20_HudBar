using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace bm98_hbFolders
{
  /// <summary>
  /// HudBar Apps - Folders Used
  /// </summary>
  public static class Folders
  {
    #region HUDBAR FILE LOCATIONS
#pragma warning disable CS0168 // Variable is declared but never used
    // settings

    // MyDocuments folder
    private static readonly string c_MyDocuments = Environment.GetFolderPath( Environment.SpecialFolder.MyDocuments );
    // User Temp folder
    private static readonly string c_UserTemp = Path.GetTempPath( );

    // HudBar file folders in MyDocuments
    private const string c_AppName_HudBar = "HudBar";

    private const string c_HudBarFolder = "MSFS_HudBarSave";
    private const string c_HudBarDbFolder = "db";
    private const string c_HudBarCacheFolder = "cache";
    private const string c_HudBarSettingsFolder = "settings";
    private const string c_HudBarLandingsFolder = "landings";

    // Temp file folder in UserTemp
    private const string c_HudBarTemp = "HudBar";

    private static bool _fs2024selected = false;
    private static bool _genAptDB20available = false;
    private static bool _genAptDB24available = false;

    // Path to MyDocuments\ ...
    private static string _hudBarDocs = "";

    // database
    // NEW LOCATION 
    // the AppName used, as GenApt db is used by multiple apps we use a generic name
    private const string c_AppName_GenApt = "GenApt";
    // the folder to put GenApt DB in from now on
    private static string GenAptTargetFolder => bm98chAppData.Folders.DatabaseFolderFor( c_AppName_GenApt );
    private const string c_GenAptDB20NameDblite = @"fs2020genAptV2.dblite"; // FS2020
    private const string c_GenAptDB24NameDblite = @"fs2024genAptV2.dblite"; // FS2024

    private static string _genAptDB20File = ""; // complete path and file FS2020
    private static string _genAptDB24File = ""; // complete path and file FS2024

    private static string _genAptDBPath = ""; // will hold the final path

    // settings
    // NEW LOCATION 
    private static string SettingsTargetFolder => bm98chAppData.Folders.SettingsFolderFor( c_AppName_HudBar );
    private static string _settingsPath = ""; // will hold the final path
    private static string _settingsFile = ""; // complete path and file

    // landings
    private const string c_HudBarLandingsFile = "TouchDownLogV3.csv";
    private static string _landingsPath = ""; // will hold the final path
    private static string _landingsFile = ""; // complete path and file

    // cache
    // NEW LOCATION 
    private static string CacheTargetFolder => bm98chAppData.Folders.CacheFolderFor( c_AppName_HudBar );
    private static string _cachePath = ""; // will hold the final path

    // Shelf
    private static string c_AptReportSubfolder = "Airport Reports"; // where the Airport reports will go
    private static string c_FPlanPDF = "@.FlightPlan.pdf";
    private static string c_FPlanPNG = "@.FlightPlan.png";
    private static string c_FPlanJPG = "@.FlightPlan.jpg";
    private static string c_FTablePDF = "@.FlightTable.pdf";
    private static string c_FTablePNG = "@.FlightTable.png";


    // temp
    private static string _tempFiles = "";

    // flag
    private static bool _initialized = false;

    #region Migrate to NEW LOCATION

    // copy the files listed to a target folder
    private static bool CopyFolder( IEnumerable<string> srcFiles, string destFolder )
    {
      // sanity
      if (!Directory.Exists( destFolder )) return false;

      bool copyOK = true;

      try {
        foreach (var oldfile in srcFiles) {
          string newFile = Path.Combine( destFolder, Path.GetFileName( oldfile ) );
          if (!File.Exists( newFile )) {
            // cannot copy if the file exists..
            File.Copy( oldfile, newFile );
          }
        }
      }
      catch {
        copyOK = false;
      }
      return copyOK;
    }

    // migrate and move settings if required, return false when failed to copy
    private static bool MigrateSettingsTo_NEW_LOCATION( )
    {
      // Old Settings
      string oldSettingsPath = Path.Combine( _hudBarDocs, c_HudBarSettingsFolder );

      // sanity 
      if (!Directory.Exists( SettingsTargetFolder )) return false; // dst does not exist, migration failed
      if (!Directory.Exists( oldSettingsPath )) return true; // src folder does not even exist, assume migration is OK- nothing to migrate...

      bool copyAllOK = true;
      // check if the new location has some key files
      try {
        var files = Directory.EnumerateFiles( SettingsTargetFolder, "*.json", SearchOption.TopDirectoryOnly ); // json and json.N
        bool newSettingsFiles = files.Count( ) > 0;

        // migrate only if new is not available
        if (newSettingsFiles == false) {
          // copy old to new
          files = Directory.EnumerateFiles( oldSettingsPath, "*.json*", SearchOption.TopDirectoryOnly );
          copyAllOK &= CopyFolder( files, SettingsTargetFolder );
        }
      }
      catch {
        // Enumerate failure e.g. dir does not exist
        return false;
      }

      return copyAllOK;
    }

    // migrate and move cache if required
    private static bool MigrateCacheTo_NEW_LOCATION( )
    {
      string oldCachePath = Path.Combine( _hudBarDocs, c_HudBarCacheFolder );
      // sanity 
      if (!Directory.Exists( CacheTargetFolder )) return false; // dst does not exist, migration failed
      if (!Directory.Exists( oldCachePath )) return true; // src folder does not even exist, assume migration is OK- nothing to migrate...

      bool copyAllOK = true;
      // check if the new location has some key files
      try {
        var files = Directory.EnumerateFiles( SettingsTargetFolder, "*.dblite", SearchOption.TopDirectoryOnly );
        bool newCacheFiles = files.Count( ) > 0;

        if (newCacheFiles == false) {
          // Old Cache
          files = Directory.EnumerateFiles( oldCachePath, "*.dblite", SearchOption.TopDirectoryOnly );
          copyAllOK &= CopyFolder( files, CacheTargetFolder );
          // map ini
          files = Directory.EnumerateFiles( oldCachePath, "*.ini", SearchOption.TopDirectoryOnly );
          copyAllOK &= CopyFolder( files, CacheTargetFolder );
        }
      }
      catch {
        // Enumerate failure e.g. dir does not exist
        return false;
      }
      return copyAllOK;
    }

    // true if we need to migrate Settings from Old to New
    private static bool Settings_MigrationNeeded( )
    {
      // check if the new location has some key files
      try {
        var files = Directory.EnumerateFiles( SettingsTargetFolder, "*.json", SearchOption.TopDirectoryOnly );
        if (files.Count( ) > 0) return false; // seems we have already files at the new location, DONT migrate
      }
      catch { }// any error

      string oldPath = Path.Combine( _hudBarDocs, c_HudBarSettingsFolder );
      try {
        var files = Directory.EnumerateFiles( oldPath, "*.json", SearchOption.TopDirectoryOnly );
        if (files.Count( ) > 0) return true; // seems we have files at the old location, migrate
      }
      catch { }// any error, i.e. directory does not even exist


      return false; // neither files exist, DONT migrate
    }

    // true if we need to migrate Cache from Old to New
    private static bool Cache_MigrationNeeded( )
    {
      // check if the new location has some key files
      try {
        var files = Directory.EnumerateFiles( CacheTargetFolder, "*.dblite", SearchOption.TopDirectoryOnly );
        if (files.Count( ) > 0) return false; // seems we have already files at the new location, DONT migrate
      }
      catch { }// any error

      string oldPath = Path.Combine( _hudBarDocs, c_HudBarCacheFolder );
      try {
        var files = Directory.EnumerateFiles( oldPath, "*.dblite", SearchOption.TopDirectoryOnly );
        if (files.Count( ) > 0) return true; // seems we have files at the old location, migrate
      }
      catch { }// any error, i.e. directory does not even exist


      return false; // neither files exist, DONT migrate
    }

    #endregion


    /// <summary>
    /// Initialize all HudBar files and locations 
    /// </summary>
    public static void InitStorage( string appSettingsFilename )
    {
      // main MyDocuments\...
      _hudBarDocs = Path.Combine( c_MyDocuments, c_HudBarFolder );

      // do we start from scratch? need this later
      bool hudBarDocsExists = Directory.Exists( _hudBarDocs );

      try {
        // make sure the path exists - but never fail..
        if (!hudBarDocsExists) {
          Directory.CreateDirectory( _hudBarDocs );
        }
      }
      catch (Exception ex) {
        _hudBarDocs = Path.GetFullPath( @".\" ); // app Dir - at least a valid location..
      }

      // temp in <UserTEMP>
      _tempFiles = Path.Combine( c_UserTemp, c_HudBarTemp );
      try {
        // make sure the database path exists - but never fail..
        if (!Directory.Exists( _tempFiles )) {
          Directory.CreateDirectory( _tempFiles );
        }
      }
      catch (Exception ex) {
        _tempFiles = Path.GetFullPath( @".\" ); // app Dir - at least a valid location..
      }


      // settings
      bool migrationSuccessfull = true;
      if (hudBarDocsExists && Settings_MigrationNeeded( )) {
        migrationSuccessfull = MigrateSettingsTo_NEW_LOCATION( );
      }

      // prefer NEW LOCATION if Migration was successfull or not needed 
      if (migrationSuccessfull) {
        _settingsPath = SettingsTargetFolder;
      }
      else {
        // use old if migration failed
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
      }
      _settingsFile = Path.Combine( _settingsPath, appSettingsFilename );


      // caches 
      migrationSuccessfull = true;
      if (hudBarDocsExists && Cache_MigrationNeeded( )) {
        migrationSuccessfull = MigrateCacheTo_NEW_LOCATION( );
      }
      // prefer NEW LOCATION if Migration was successfull or not needed 
      if (migrationSuccessfull) {
        _cachePath = CacheTargetFolder;
      }
      else {
        // use old if migration failed
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


      // landings remain in HudBar Document folder
      _landingsPath = Path.Combine( _hudBarDocs, c_HudBarLandingsFolder );
      try {
        // make sure the settings path exists - but never fail..
        if (!Directory.Exists( _landingsPath )) {
          Directory.CreateDirectory( _landingsPath );
        }
      }
      catch (Exception ex) {
        _landingsPath = Path.GetFullPath( @".\" ); // app Dir - at least a valid location..
      }
      _landingsFile = Path.Combine( _landingsPath, c_HudBarLandingsFile );


      // GenApt database - may exist independent of HudBar, will not be migrated but used when there
      // prefer NEW LOCATION
      _genAptDBPath = GenAptTargetFolder;
      _genAptDB20File = Path.Combine( _genAptDBPath, c_GenAptDB20NameDblite );
      _genAptDB24File = Path.Combine( _genAptDBPath, c_GenAptDB24NameDblite );

      if (!(File.Exists( _genAptDB20File ) || File.Exists( _genAptDB24File ))) {
        // use old location if NEW LOCATION does not have any GenApt db file
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
        _genAptDB20File = Path.Combine( _genAptDBPath, c_GenAptDB20NameDblite );
        _genAptDB24File = Path.Combine( _genAptDBPath, c_GenAptDB24NameDblite );
      }


      CheckGenAptFiles( );
      _initialized = true;
    }

#pragma warning restore CS0168 // Variable is declared but never used
    #endregion

    // will reply the string if initialized, else throw
    private static string InitChecked( string reply )
    {
      // sanity
      if (!_initialized) throw new InvalidOperationException( "The Object must be initialized before use (bm98_hbFolders.Folders)" );

      return reply;
    }

    /// <summary>
    /// Returns the Path to save files for the User
    /// </summary>
    /// <returns>A path</returns>
    public static string UserFilePath => InitChecked( _hudBarDocs );
    /// <summary>
    /// Returns the Path to save temp files for the User
    /// </summary>
    /// <returns>A path</returns>
    public static string UserTempPath => InitChecked( _tempFiles );

    /// <summary>
    /// Path to DB files
    /// </summary>
    public static string DBPath => InitChecked( _genAptDBPath );
    /// <summary>
    /// Path to Cache files
    /// </summary>
    public static string CachePath => InitChecked( _cachePath );
    /// <summary>
    /// Path to Settings files
    /// </summary>
    public static string SettingsPath => InitChecked( _settingsPath );

    /// <summary>
    /// Path to Landings files
    /// </summary>
    public static string LandingsPath => InitChecked( _landingsPath );

    /// <summary>
    /// Path to the Landings CSV file
    /// </summary>
    public static string LandingsFile => InitChecked( _landingsFile );

    /// <summary>
    /// From Selected Version the GenAptDB File with path
    ///  Selected defaults to FS2020 if not changed
    ///  if none is avail it returns an emtpy string
    /// </summary>
    public static string GenAptDBFile {
      get {
        CheckGenAptFiles( );

        // For 2024
        if (_fs2024selected && _genAptDB24available) {
          return InitChecked( _genAptDB24File );
        }

        // 2020 and fallback from above
        if (_genAptDB20available) {
          return InitChecked( _genAptDB20File );
        }
        // Fallback for 2020 where only 2024 was found
        else if (_genAptDB24available) {
          return InitChecked( _genAptDB24File );
        }
        // nothing worked...
        return "";
      }
    }

    /// <summary>
    /// Settings file + path 
    /// NOTE: Set the app specific Settings File Name 
    /// </summary>
    public static string SettingsFile => InitChecked( _settingsFile );

    /// <summary>
    /// True if FS2024 source is used
    /// </summary>
    public static bool FS2024Used => _fs2024selected && _genAptDB24available;

    /// <summary>
    /// Select the FS Version to be used
    ///  will choose GenAptDBFile
    /// </summary>
    /// <param name="fs2024">Set true for choosing 2024</param>
    public static void SelectFSVersion( bool fs2024 ) => _fs2024selected = fs2024;

    /// <summary>
    /// Determine which GenAptDB versions are available
    /// </summary>
    public static void CheckGenAptFiles( )
    {
      _genAptDB20available = File.Exists( _genAptDB20File );
      _genAptDB24available = File.Exists( _genAptDB24File );
    }

    /// <summary>
    /// True if MSFS2020 GenApt db file is found
    /// </summary>
    public static bool HasGenApt2020 { get { CheckGenAptFiles( ); return _genAptDB20available; } }
    /// <summary>
    /// True if MSFS2024 GenApt db file is found
    /// </summary>
    public static bool HasGenApt2024 { get { CheckGenAptFiles( ); return _genAptDB24available; } }


    // SHELF items (Shelf path is in AppSettings)

    /// <summary>
    /// Airport Reports Shelf Subfolder (Shelf folder is in AppSettings)
    /// </summary>
    public static string AptReportSubfolder => c_AptReportSubfolder;

    /// <summary>
    /// Flightplan PDF Filename in Shelf (Shelf folder is in AppSettings)
    /// </summary>
    public static string FPlanPDF_FileName => c_FPlanPDF;
    /// <summary>
    /// Flightplan JPG Filename in Shelf (Shelf folder is in AppSettings)
    /// </summary>
    public static string FPlanPNG_FileName => c_FPlanPNG;
    /// <summary>
    /// Flightplan JPG Filename in Shelf (Shelf folder is in AppSettings)
    /// </summary>
    public static string FPlanJPG_FileName => c_FPlanJPG;
    /// <summary>
    /// Flightplan Table PDF Filename in Shelf (Shelf folder is in AppSettings)
    /// </summary>
    public static string FTablePDF_FileName => c_FTablePDF;
    /// <summary>
    /// Flightplan Table PNG Filename in Shelf (Shelf folder is in AppSettings)
    /// </summary>
    public static string FTablePNG_FileName => c_FTablePNG;


  }
}

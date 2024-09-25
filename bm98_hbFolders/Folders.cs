using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    private const string c_HudBarFolder = "MSFS_HudBarSave";
    private const string c_HudBarDbFolder = "db";
    private const string c_HudBarCacheFolder = "cache";
    private const string c_HudBarSettingsFolder = "settings";
    private const string c_HudBarLandingsFolder = "landings";

    // Temp file folder in UserTemp
    private const string c_HudBarTemp = "HudBar";


    // Path to MyDocuments\ ...
    private static string _hudBarDocs = "";
    // database
    private const string c_GenAptDBNameDblite = @"fs2020genAptV2.dblite"; // new use V2
    private static string _genAptDBPath = ""; // will hold the final path
    private static string _genAptDBFile = ""; // complete path and file

    // settings
    private static string _settingsPath = ""; // will hold the final path
    private static string _settingsFile = ""; // complete path and file

    // landings
    private const string c_HudBarLandingsFile = "TouchDownLogV3.csv";
    private static string _landingsPath = ""; // will hold the final path
    private static string _landingsFile = ""; // complete path and file

    // cache
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

    /// <summary>
    /// Initialize all HudBar files and locations 
    /// </summary>
    public static void InitStorage( string appSettingsFilename )
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

      // temp
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
      _settingsFile = Path.Combine( _settingsPath, appSettingsFilename );

      // landings
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
    /// GenAptDB File with path
    /// </summary>
    public static string GenAptDBFile => InitChecked( _genAptDBFile );
    /// <summary>
    /// Settings file + path 
    /// NOTE: Set the app specific Settings File Name 
    /// </summary>
    public static string SettingsFile => InitChecked( _settingsFile );


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

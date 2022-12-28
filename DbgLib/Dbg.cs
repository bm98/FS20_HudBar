using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Media.Audio;
using Windows.Media.Core;
using Windows.Media.SpeechSynthesis;

namespace DbgLib
{
  /// <summary>
  /// Results of the File IO Access check
  /// </summary>
  public enum AccessCheckResult
  {
    /// <summary>
    /// Success...
    /// </summary>
    Success=0,
    /// <summary>
    /// The check directory does not exist
    /// </summary>
    DirectoryDoesNotExist,
    /// <summary>
    /// IO Exception while enumerating files
    /// </summary>
    EnumerateFiles_IO_Exception,
    /// <summary>
    /// Access permission Exception while enumerating files
    /// </summary>
    EnumerateFiles_Access_Exception,
    /// <summary>
    /// Other Exception while enumerating files
    /// </summary>
    EnumerateFiles_Other_Exception,

    /// <summary>
    /// IO Exception while writing file
    /// </summary>
    WriteFile_IO_Exception,
    /// <summary>
    /// Access permission Exception while writing file
    /// </summary>
    WriteFile_Access_Exception,
    /// <summary>
    /// Other Exception while writing file
    /// </summary>
    WriteFile_Other_Exception,

    /// <summary>
    /// IO Exception while reading file
    /// </summary>
    ReadFile_IO_Exception,
    /// <summary>
    /// Access permission Exception while reading file
    /// </summary>
    ReadFile_Access_Exception,
    /// <summary>
    /// Other Exception while reading file
    /// </summary>
    ReadFile_Other_Exception,
    /// <summary>
    /// Error while reading file, read does not match write content
    /// </summary>
    ReadFile_ContentMismatch,

    /// <summary>
    /// IO Exception while deleting file
    /// </summary>
    DeleteFile_IO_Exception,
    /// <summary>
    /// Access permission Exception while deleting file
    /// </summary>
    DeleteFile_Access_Exception,
    /// <summary>
    /// Other Exception while deleting file
    /// </summary>
    DeleteFile_Other_Exception,

    /// <summary>
    /// Any other error encountered
    /// </summary>
    OtherError,
  }


  /// <summary>
  /// Debug Helper Object (Singleton)
  /// 
  /// USE:          sw.WriteLine( $"Debug:       {System.Reflection.Assembly.GetCallingAssembly( ).GetName( )}" );
  /// 
  /// USE:   DbgLib.Dbg.Instance.Log( $"AppVersion: {Application.ProductName} - {Application.ProductVersion}" );
  /// to log the Applications Name and Version
  /// </summary>
  public sealed class Dbg
  {
    // Singleton Pattern
    /// <summary>
    /// Library Instance
    /// </summary>
    public static Dbg Instance => lazy.Value;
    private static readonly Lazy<Dbg> lazy = new Lazy<Dbg>( ( ) => new Dbg( ) );
    private Dbg( )
    {
      _canWrite = false;
    }

    // the debug file in the current directory
    private readonly string c_logFileName = Path.Combine( ".", "DEBUG_log.txt" );

    // flag is we can/shall write
    private bool _canWrite;

    // Init the Log with some basic information
    private void InitLog( )
    {
      if (!_canWrite) return;

      try {
        using (var sw = new StreamWriter( c_logFileName, false )) {
          sw.WriteLine( TagLine( "DGBLog - Init" ) );
        }
      }
      catch (Exception e) {
        Console.WriteLine( $"ERROR: DBGLib-InitLog Cannot write logfile\n{e.Message}" );
        _canWrite = false;
        return;
      }

      using (var sw = new StreamWriter( c_logFileName, false )) {
        sw.WriteLine( TagLine( "DGBLog - Init" ) );
        sw.WriteLine( $"Debug:       {System.Reflection.Assembly.GetCallingAssembly( ).GetName( )}" );
        sw.WriteLine( $"Application: {System.Reflection.Assembly.GetEntryAssembly( ).GetName( )}" );

        CollectEnvironmentInformation( sw );
        sw.Flush( );
      }

      ListCurDir( );
    }

    // Tag a line with Timestamp
    private string TagLine( string text )
    {
      return $"{DateTime.Now:O}¦{text}";
    }

    /// <summary>
    /// The message of the last access check
    /// </summary>
    public string AccessCheckMessage { get; private set; } = "";
    /// <summary>
    /// The result of the last access check
    /// </summary>
    public AccessCheckResult AccessCheckResult { get; private set; } = AccessCheckResult.Success;

    /// <summary>
    /// Enable debugging 
    /// From now on the log will be written
    /// </summary>
    public void EnableDebug( )
    {
      _canWrite = true;
      InitLog( );
    }

    /// <summary>
    /// Returns a new Logger with a Module Name
    /// </summary>
    /// <param name="module">The Module Name</param>
    /// <returns>A Logger</returns>
    public IDbg GetLogger( string module )
    {
      return new DbgLogger( module );
    }

    /// <summary>
    /// Returns a new Logger with a Module Name
    /// </summary>
    /// <param name="type">The Executing Type</param>
    /// <returns>A Logger</returns>
    public IDbg GetLogger( Type type )
    {
      return new DbgLogger( type );
    }

    /// <summary>
    /// Returns a new Logger with a Module Name
    /// </summary>
    /// <param name="assembly">The Assembly</param>
    /// <param name="type">The Executing Type</param>
    /// <returns>A Logger</returns>
    public IDbg GetLogger( Assembly assembly, Type type )
    {
      return new DbgLogger( assembly, type );
    }

    /// <summary>
    /// Performs a number of file Access Checks for the given folder
    /// The result is returned and available as property: AccessCheckResult
    ///   the message/exception string as property: AccessCheckMessage
    /// </summary>
    /// <param name="directory">A fully qualified directory</param>
    /// <returns>Dectection message, an empty string when no problem occured</returns>
    public AccessCheckResult AccessCheck( string directory )
    {
      Log( "DbgLog", $"Performing access checks for <{directory}>" );
      AccessCheckMessage = "";
      AccessCheckResult = AccessCheckResult.Success;

      // Check folder
      if (!Directory.Exists( directory )) {
        AccessCheckResult = AccessCheckResult.DirectoryDoesNotExist;
        AccessCheckMessage = $"Directory Error: Check Directory does not exist";
        LogError( AccessCheckMessage );
        return AccessCheckResult;
      }

      // Enumerate files
      try {
        var files = Directory.GetFiles( directory );
      }
      catch (IOException ex) {
        AccessCheckResult = AccessCheckResult.EnumerateFiles_IO_Exception;
        AccessCheckMessage = $"IO Exception: Cannot enumerate files: \n{ex.Message}";
        LogError( AccessCheckMessage );
        return AccessCheckResult;
      }
      catch (UnauthorizedAccessException ex) {
        AccessCheckResult = AccessCheckResult.EnumerateFiles_Access_Exception;
        AccessCheckMessage = $"Permission Exception: Cannot enumerate files: \n{ex.Message}";
        LogError( AccessCheckMessage );
        return AccessCheckResult;
      }
      catch (Exception ex) {
        AccessCheckResult = AccessCheckResult.EnumerateFiles_Other_Exception;
        AccessCheckMessage = $"Generic Exception: Cannot enumerate files: \n{ex.Message}";
        LogError( AccessCheckMessage );
        return AccessCheckResult;
      }

      string pattern = "1234567890asdfghjkl";
      var tmpFile = Path.Combine( directory, "@@_CHECKFILE_@@.txt" );
      string buffer = "";

      // write to a file in the folder
      try {
        using (var sw = new StreamWriter( tmpFile )) {
          sw.WriteLine( pattern );
          sw.WriteLine( pattern );
          sw.WriteLine( pattern );
        }
      }
      catch (IOException ex) {
        AccessCheckResult = AccessCheckResult.WriteFile_IO_Exception;
        AccessCheckMessage = $"IO Exception: Cannot write to file: \n{ex.Message}";
        LogError( AccessCheckMessage );
        return AccessCheckResult;
      }
      catch (UnauthorizedAccessException ex) {
        AccessCheckResult = AccessCheckResult.WriteFile_Access_Exception;
        AccessCheckMessage = $"Permission Exception: Cannot enumerate files: \n{ex.Message}";
        LogError( AccessCheckMessage );
        return AccessCheckResult;
      }
      catch (Exception ex) {
        AccessCheckResult = AccessCheckResult.WriteFile_Other_Exception;
        AccessCheckMessage = $"Generic Exception: Cannot write to file: \n{ex.Message}";
        LogError( AccessCheckMessage );
        return AccessCheckResult;
      }

      // read from file
      try {
        using (var sr = new StreamReader( tmpFile )) {
          buffer = sr.ReadToEnd( );
        }
      }
      catch (IOException ex) {
        AccessCheckResult = AccessCheckResult.ReadFile_IO_Exception;
        AccessCheckMessage = $"IO Exception: Cannot read from file: \n{ex.Message}";
        LogError( AccessCheckMessage );
        return AccessCheckResult;
      }
      catch (UnauthorizedAccessException ex) {
        AccessCheckResult = AccessCheckResult.ReadFile_Access_Exception;
        AccessCheckMessage = $"Permission Exception: Cannot read from file: \n{ex.Message}";
        LogError( AccessCheckMessage );
        return AccessCheckResult;
      }
      catch (Exception ex) {
        AccessCheckResult = AccessCheckResult.ReadFile_Other_Exception;
        AccessCheckMessage = $"Generic Exception: Cannot read from file: \n{ex.Message}";
        LogError( AccessCheckMessage );
        return AccessCheckResult;
      }

      // Delete file
      try {
        File.Delete( tmpFile );
      }
      catch (IOException ex) {
        AccessCheckResult = AccessCheckResult.DeleteFile_IO_Exception;
        AccessCheckMessage = $"IO Exception: Cannot delete temp file: \n{ex.Message}";
        LogError( AccessCheckMessage );
        return AccessCheckResult;
      }
      catch (UnauthorizedAccessException ex) {
        AccessCheckResult = AccessCheckResult.DeleteFile_Access_Exception;
        AccessCheckMessage = $"Permission Exception: Cannot delete temp file: \n{ex.Message}";
        LogError( AccessCheckMessage );
        return AccessCheckResult;
      }
      catch (Exception ex) {
        AccessCheckResult = AccessCheckResult.DeleteFile_Other_Exception;
        AccessCheckMessage = $"Generic Exception: Cannot delete temp file: \n{ex.Message}";
        LogError( AccessCheckMessage );
        return AccessCheckResult;
      }

      // check read content
      if (buffer != $"{pattern}\r\n{pattern}\r\n{pattern}\r\n") {
        AccessCheckResult = AccessCheckResult.ReadFile_ContentMismatch;
        AccessCheckMessage = "Read Error: erraneous read result";
        LogError( AccessCheckMessage );
        return AccessCheckResult;
      }

      return AccessCheckResult; // seems OK
    }

    #region Logging

    /// <summary>
    /// Log a Text Item, writes immediately to the file
    /// </summary>
    /// <param name="text">Log Text</param>
    public void Log( string text )
    {
      if (!_canWrite) return;

      using (var sw = new StreamWriter( c_logFileName, true )) {
        sw.WriteLine( TagLine( text ) );
        sw.Flush( );
      }
    }

    /// <summary>
    /// Log a Text Item as Error
    /// </summary>
    /// <param name="text">Log Text</param>
    public void LogError( string text )
    {
      Log( "ERROR: " + text );
    }

    /// <summary>
    /// Log a Text Item from a Module
    /// </summary>
    /// <param name="module">The related sw part</param>
    /// <param name="text">Log Text</param>
    public void Log( string module, string text )
    {
      Log( $"{module}-{text}" );
    }

    /// <summary>
    /// Log a Text Item from a Module as Error
    /// </summary>
    /// <param name="module">The related sw part</param>
    /// <param name="text">Log Text</param>
    public void LogError( string module, string text )
    {
      LogError( $"{module}-{text}" );
    }

    /// <summary>
    /// Log a text and dump the stacktrace from the calling process
    /// </summary>
    /// <param name="module">The related sw part</param>
    /// <param name="text">Log Text</param>
    public void LogStackTrace( string module, string text )
    {
      Log( $"Stack Trace: {module}-{text}\n{Environment.StackTrace}" );
    }

    #endregion

    #region App Environment Dump

    // Collect some helpful information about the running environment
    // Note - no sensitive information shall be collected here !!
    private void CollectEnvironmentInformation( StreamWriter sw )
    {
      sw.WriteLine( "OS Information:" );
      sw.WriteLine( $"    {WinVersion.WindowsVersion( )}" );

      sw.WriteLine( $"Locale:" );
      sw.WriteLine( $"    {CultureInfo.CurrentCulture.Name}    {CultureInfo.CurrentCulture.EnglishName}" );
    }

    // Get the current directory and files in it
    private void ListCurDir( )
    {
      if (!_canWrite) return;

      using (var sw = new StreamWriter( c_logFileName, true )) {
        sw.WriteLine( $"Current Directory:\n    {Environment.CurrentDirectory}\nFiles:" );
        try {
          foreach (var file in Directory.EnumerateFiles( Environment.CurrentDirectory, "*", SearchOption.AllDirectories )) {
            sw.WriteLine( $"    {file}" );
          }
        }
        catch (Exception e) {
          sw.WriteLine( $"    ERROR enumerating files\n{e.Message}" );
        }
      }
    }

    #endregion

    #region AUDIO Dump

    /// <summary>
    /// Dump Audio Properties if needed / helpful
    /// </summary>
    public void DumpAudioProps( )
    {
      // Waits for completion of the enumeration
      ListDeviceInformation( );
      ListInstalledVoices( );
    }

    // dump the installed voices from MS
    private void ListInstalledVoices( )
    {
      if (!_canWrite) return;

      var voices = SpeechSynthesizer.AllVoices;
      using (var sw = new StreamWriter( c_logFileName, true )) {
        sw.WriteLine( $"Installed Voices:" );
        foreach (var v in voices) {
          sw.WriteLine( $" - {v.DisplayName} - {v.Language} - {v.Gender}" );
        }
      }
    }


    // Collect Audio Render Device(s) to see if any is available
    // No Asynch here - the method waits for completion of the Enum
    // don't know if this needs to be in a separate thread to not block the Main Thread where the 
    // Instance is created for??
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
    async private void ListDeviceInformation( )
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    {
      if (!_canWrite) return;

      var ret = DeviceInformation.FindAllAsync( DeviceClass.AudioRender ).AsTask( );
      ret.Wait( );
      if (ret.Status == TaskStatus.RanToCompletion) {
        using (var sw = new StreamWriter( c_logFileName, true )) {
          sw.WriteLine( "Device Information:" );

          foreach (DeviceInformation deviceInterface in ret.Result) {

            sw.WriteLine( $" - {deviceInterface.Name}" );
            foreach (var p in deviceInterface.Properties) {
              sw.WriteLine( $"     {p.Key} - {p.Value}" );
            }
          }
          sw.Flush( );
        }
      }
      else {
        LogError( $"DBGLib - ListDeviceInformation Status {ret.Status}" );
      }
    }

    #endregion

  }
}

using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;

namespace MetarLib
{
  internal static class Formatter
  {
    #region JSON Serialization

    /// <summary>
    /// Reads from the open stream one entry
    /// </summary>
    /// <param name="jStream">An open stream at position</param>
    /// <returns>A Controller obj or null for errors</returns>
    public static T FromJsonStream<T>( Stream jStream )
    {
      try {
        var jsonSerializer = new DataContractJsonSerializer( typeof( T ) );
        object objResponse = jsonSerializer.ReadObject( jStream );
        var jsonResults = (T)objResponse;
        jStream.Flush( );
        return jsonResults;
      }
      catch (Exception ex) {
        _ = ex;
        return default;
      }
    }

    /// <summary>
    /// Reads from the supplied string
    /// </summary>
    /// <param name="jString">A JSON formatted string</param>
    /// <returns>A Controller obj or null for errors</returns>
    public static T FromJsonString<T>( string jString )
    {
      try {
        T jsonResults = default;
        using (var ms = new MemoryStream( Encoding.UTF8.GetBytes( jString ) )) {
          jsonResults = FromJsonStream<T>( ms );
        }
        return jsonResults;
      }
      catch (Exception ex) {
        _ = ex;
        return default;
      }
    }


    /// <summary>
    /// Reads from a file one entry
    /// Tries to aquire a shared Read Access and blocks for max 100ms while doing so
    /// </summary>
    /// <param name="jFilename">The Json Filename</param>
    /// <returns>A Controller obj or null for errors</returns>
    public static T FromJsonFile<T>( string jFilename )
    {
      T retVal = default;
      if (!File.Exists( jFilename )) {
        return retVal;
      }

      int retries = 10; // 100ms worst case
      while (retries-- > 0) {
        try {
          using (var ts = File.Open( jFilename, FileMode.Open, FileAccess.Read, FileShare.Read )) {
            retVal = FromJsonStream<T>( ts );
          }
          return retVal;
        }
        catch (IOException ioex) {
          _ = ioex;
          // retry after a short wait
          Thread.Sleep( 10 ); // allow the others fileIO to be completed
        }
        catch (Exception ex) {
          _ = ex;
          // not an IO exception - just fail
          return retVal;
        }
      }

      return retVal;
    }

    /// <summary>
    /// Write to the open stream one entry
    /// </summary>
    /// <param name="data">A datafile object to write</param>
    /// <param name="jStream">An open stream at position</param>
    /// <returns>True if successfull</returns>
    public static bool ToJsonStream<T>( T data, Stream jStream )
    {
      try {
        var jsonSerializer = new DataContractJsonSerializer( typeof( T ) );
        jsonSerializer.WriteObject( jStream, data );
        return true;
      }
      catch (Exception ex) {
        _ = ex;
        return false; // bails at data==null or formatting issues
      }
    }

    /// <summary>
    /// Write to a file one entry
    /// Tries to aquire an exclusive Write Access and blocks for max 100ms while doing so
    /// </summary>
    /// <param name="data">A datafile object to write</param>
    /// <param name="jFilename">The Json Filename</param>
    /// <returns>True if successfull</returns>
    public static bool ToJsonFile<T>( T data, string jFilename )
    {
      bool retVal = false;

      int retries = 10; // 100ms worst case
      while (retries-- > 0) {
        try {
          using (var ts = File.Open( jFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None )) {
            retVal = ToJsonStream( data, ts );
          }
          return retVal;
        }
        catch (IOException ioex) {
          _ = ioex;
          // retry after a short wait
          Thread.Sleep( 10 ); // allow the others fileIO to be completed
        }
        catch (Exception ex) {
          _ = ex;
          // not an IO exception - just fail
          return retVal;
        }
      }
      return retVal;
    }

    #endregion


  }
}

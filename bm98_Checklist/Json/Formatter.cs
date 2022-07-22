using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.IO;

namespace bm98_Checklist.Json
{
  internal class Formatter
  {


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
#pragma warning disable CS0168 // Variable is declared but never used
      catch (Exception e) {
#pragma warning restore CS0168 // Variable is declared but never used
        return default( T );
      }
    }


    /// <summary>
    /// Reads from a file one entry
    /// </summary>
    /// <param name="jFilename">The Json Filename</param>
    /// <returns>A Controller obj or null for errors</returns>
    public static T FromJsonFile<T>( string jFilename )
    {
      T c = default( T );
      string fn = jFilename;
      if (!File.Exists( jFilename )) {
        return c;
      }
      if (File.Exists( fn )) {
        using (var ts = File.OpenRead( fn )) {
          c = FromJsonStream<T>( ts );
        }
      }
      return c;
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
#pragma warning disable CS0168 // Variable is declared but never used
      catch (Exception e) {
#pragma warning restore CS0168 // Variable is declared but never used
        return false;
      }
    }

    /// <summary>
    /// Write to a file one entry
    /// </summary>
    /// <param name="data">A datafile object to write</param>
    /// <param name="jFilename">The Json Filename</param>
    /// <returns>True if successfull</returns>
    public static bool ToJsonFile<T>( T data, string jFilename )
    {
      string fn = jFilename;
      bool c = false;
      try {
        using (var ts = File.Open( fn, FileMode.Create )) {
          c = ToJsonStream( data, ts );
        }
      }
      catch { }
      return c;
    }




  }
}

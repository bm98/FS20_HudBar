using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbgLib
{
  /// <summary>
  /// Defined the methods for Logging
  /// </summary>
  public interface IDbg
  {
    /// <summary>
    /// Log a Text Item, writes immediately to the file
    /// </summary>
    /// <param name="text">Log Text</param>
    void Log( string text );

    /// <summary>
    /// Log a Text Item as Error
    /// </summary>
    /// <param name="text">Log Text</param>
    void LogError( string text );

    /// <summary>
    /// Log a text and dump the stacktrace from the calling process
    /// </summary>
    /// <param name="text">Log Text</param>
    void LogStackTrace( string text );

  }
}

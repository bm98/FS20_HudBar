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
    void Info( string text );
    /// <summary>
    /// Log a Text Item, writes immediately to the file
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="text">Log Text</param>
    void Info( string context, string text );


    /// <summary>
    /// Log a Text Item as Debug
    /// </summary>
    /// <param name="text">Log Text</param>
    void Debug( string text );
    /// <summary>
    /// Log a Text Item as Debug
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="text">Log Text</param>
    void Debug( string context, string text );
    /// <summary>
    /// Log a Text Item as Debug with Exception
    /// </summary>
    /// <param name="ex">Exception</param>
    /// <param name="text">Log Text</param>
    void Debug( Exception ex, string text );
    /// <summary>
    /// Log a Text Item as Debug with Exception
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="ex">Exception</param>
    /// <param name="text">Log Text</param>
    void Debug( string context, Exception ex, string text );


    /// <summary>
    /// Log a Text Item as Error
    /// </summary>
    /// <param name="text">Log Text</param>
    void Error( string text );
    /// <summary>
    /// Log a Text Item as Error
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="text">Log Text</param>
    void Error( string context, string text );
    /// <summary>
    /// Log a Text Item as Error with Exception
    /// </summary>
    /// <param name="ex">Exception</param>
    /// <param name="text">Log Text</param>
    void Error( Exception ex, string text );
    /// <summary>
    /// Log a Text Item as Error with Exception
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="ex">Exception</param>
    /// <param name="text">Log Text</param>
    void Error( string context, Exception ex, string text );


    /// <summary>
    /// Log a Text Item as Trace
    /// </summary>
    /// <param name="text">Log Text</param>
    void Trace( string text );
    /// <summary>
    /// Log a Text Item as Trace
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="text">Log Text</param>
    void Trace( string context, string text );


    /// <summary>
    /// Log a text and dump the stacktrace from the calling process
    /// </summary>
    /// <param name="text">Log Text</param>
    void LogStackTrace( string text );
    /// <summary>
    /// Log a text and dump the stacktrace from the calling process
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="text">Log Text</param>
    void LogStackTrace( string context, string text );

  }
}

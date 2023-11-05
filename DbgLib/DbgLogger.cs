using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DbgLib
{
  /// <summary>
  /// A debug logger
  /// </summary>
  public class DbgLogger : IDbg
  {
    private string _assembly = "";
    private string _type = "";
    private string _module = "";

    // the composed Module Name
    private string _modName = "";


    /// <summary>
    /// cTor: Empty
    /// </summary>
    public DbgLogger( )
    {
      _module = "Generic";

      _modName = _module;
    }

    /// <summary>
    /// cTor: Module name
    /// </summary>
    public DbgLogger( string module )
    {
      _module = module;

      _modName = _module;
    }

    /// <summary>
    /// cTor: Module Prefix
    /// </summary>
    public DbgLogger( Type type )
    {
      _type = type.Name;

      _modName = $"{_type}";
    }

    /// <summary>
    /// cTor: Assembly.Module Prefix
    /// </summary>
    public DbgLogger( Assembly assembly, Type type )
    {
      _assembly = assembly.FullName;
      _type = type.Name;

      _modName = $"{assembly.GetName( ).Name}.{_type}";
    }

    /// <summary>
    /// Log a Text Item, writes immediately to the file
    /// </summary>
    /// <param name="text">Log Text</param>
    public void Log( string text )
    {
      Dbg.Instance.Log( $"({_modName})", text );
    }

    /// <summary>
    /// Log a Text Item as Error
    /// </summary>
    /// <param name="text">Log Text</param>
    public void LogError( string text )
    {
      Dbg.Instance.LogError( $"({_modName})", text );
    }

    /// <summary>
    /// Log a Text Item as Error
    /// </summary>
    /// <param name="ex">Exception</param>
    /// <param name="text">Log Text</param>
    public void LogException( Exception ex, string text )
    {
      Dbg.Instance.LogException( $"({_modName})", ex, text );
    }


    /// <summary>
    /// Log a text and dump the stacktrace from the calling process
    /// </summary>
    /// <param name="text">Log Text</param>
    public void LogStackTrace( string text )
    {
      Dbg.Instance.LogStackTrace( $"({_modName})", text );
    }

    /// <summary>
    /// Log a Text Item, writes immediately to the file
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="text">Log Text</param>
    public void Log( string context, string text )
    {
      Dbg.Instance.Log( $"({_modName}.{context})", text );
    }

    /// <summary>
    /// Log a Text Item as Error
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="text">Log Text</param>
    public void LogError( string context, string text )
    {
      Dbg.Instance.LogError( $"({_modName}.{context})", text );
    }

    /// <summary>
    /// Log a Text Item as Error
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="ex">Exception</param>
    /// <param name="text">Log Text</param>
    public void LogException( string context, Exception ex, string text )
    {
      Dbg.Instance.LogException( $"({_modName}.{context})", ex, text );
    }


    /// <summary>
    /// Log a text and dump the stacktrace from the calling process
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="text">Log Text</param>
    public void LogStackTrace( string context, string text )
    {
      Dbg.Instance.LogStackTrace( $"({_modName}.{context})", text );
    }

  }
}

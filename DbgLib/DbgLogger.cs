using System;
using System.Reflection;

using NLog;

namespace DbgLib
{
  /// <summary>
  /// A debug logger Using NLog, takes care of context
  /// </summary>
  public class DbgLogger : IDbg
  {
    private readonly Logger LOG = LogManager.GetLogger( "Generic" );

    private string _type = "";
    private string _module = "";

    // the composed Module Name
    private string _modName = "";


    /// <summary>
    /// cTor: Empty, Module name is 'Generic'
    /// </summary>
    public DbgLogger( )
    {
      _module = "Generic";

      _modName = _module;
      LOG = LogManager.GetLogger( _modName );
    }

    /// <summary>
    /// cTor: Module name
    /// </summary>
    public DbgLogger( string module )
    {
      _module = module;
      _modName = _module;
      LOG = LogManager.GetLogger( _modName );
    }

    /// <summary>
    /// cTor: Module name is Type.ToString()
    /// </summary>
    public DbgLogger( Type type )
    {
      _type = type.Name;

      _modName = $"{_type}";
      LOG = LogManager.GetLogger( _modName );
    }

    /// <summary>
    /// cTor: Assembly.Module Prefix
    /// </summary>
    public DbgLogger( Assembly assembly, Type type )
    {
      _type = type.Name;

      _modName = $"{assembly.GetName( ).Name}.{_type}";
      LOG = LogManager.GetLogger( _modName );
    }

    /// <summary>
    /// Log as INFO Text
    /// </summary>
    /// <param name="text">Info Text</param>
    public void Info( string text ) => LOG.Info( text );
    /// <summary>
    /// Log as INFO Text
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="text">Info Text</param>
    public void Info( string context, string text )
    {
      using (ScopeContext.PushNestedState( context ))
        LOG.Info( text );
    }

    /// <summary>
    /// Log as DEBUG Text
    /// </summary>
    /// <param name="text">Debug Text</param>
    public void Debug( string text ) => LOG.Debug( text );
    /// <summary>
    /// Log as DEBUG Text
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="text">Debug Text</param>
    public void Debug( string context, string text )
    {
      using (ScopeContext.PushNestedState( context ))
        LOG.Debug( text );
    }
    /// <summary>
    /// Log as DEBUG Text with Exception
    /// </summary>
    /// <param name="ex">Exception</param>
    /// <param name="text">Debug Text</param>
    public void Debug( Exception ex, string text ) => LOG.Debug( ex, text );
    /// <summary>
    /// Log as DEBUG Text with Exception
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="ex">Exception</param>
    /// <param name="text">Debug Text</param>
    public void Debug( string context, Exception ex, string text )
    {
      using (ScopeContext.PushNestedState( context ))
        LOG.Debug( ex, text );
    }


    /// <summary>
    /// Log as ERROR Text
    /// </summary>
    /// <param name="text">Error Text</param>
    public void Error( string text ) => LOG.Error( text );
    /// <summary>
    /// Log as ERROR Text
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="text">Error Text</param>
    public void Error( string context, string text )
    {
      using (ScopeContext.PushNestedState( context ))
        LOG.Error( text );
    }
    /// <summary>
    /// Log as ERROR Text with Exception
    /// </summary>
    /// <param name="ex">Exception</param>
    /// <param name="text">Error Text</param>
    public void Error( Exception ex, string text ) => LOG.Error( ex, text );
    /// <summary>
    /// Log as ERROR Text with Exception
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="ex">Exception</param>
    /// <param name="text">Error Text</param>
    public void Error( string context, Exception ex, string text )
    {
      using (ScopeContext.PushNestedState( context ))
        LOG.Error( ex, text );
    }

    /// <summary>
    /// Log as TRACE Text
    /// </summary>
    /// <param name="text">Trace Text</param>
    public void Trace( string text ) => LOG.Trace( text );
    /// <summary>
    /// Log as TRACE Text
    /// </summary>
    /// <param name="context">A context</param>
    /// <param name="text">Trace Text</param>
    public void Trace( string context, string text )
    {
      using (ScopeContext.PushNestedState( context ))
        LOG.Trace( text );
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

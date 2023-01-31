using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.MS
{
  /// <summary>
  /// attribute to hold Section Name for later processing
  /// </summary>
  [AttributeUsage( AttributeTargets.Property )]
  public class IniFileSection : Attribute
  {
    /// <summary>
    /// Const Name for the Main Section 
    /// </summary>
    public const string MainSection = "$$$_MAIN_$$$";

    // Data for properties.
    private string _name;

    /// <summary>
    /// cTor: attribute to hold Section Name for later processing
    /// </summary>
    /// <param Name="name"></param>
    public IniFileSection( string name )
    {
      _name = name;
    }
    /// <summary>
    /// cTor: empty Attribute
    /// </summary>
    public IniFileSection( ) : this( null ) { }

    /// <summary>
    /// SimVar Name of the Attribute
    /// </summary>
    public string Name { get { return _name; } }
  }

  /// <summary>
  /// attribute to hold Key Name for later processing
  /// </summary>
  [AttributeUsage( AttributeTargets.Property )]
  public class IniFileKey : Attribute
  {
    // Data for properties.
    private string _name;

    /// <summary>
    /// cTor: creates the attribute to hold Key Name for later processing
    /// </summary>
    /// <param Name="name"></param>
    public IniFileKey( string name )
    {
      _name = name;
    }
    /// <summary>
    /// cTor: empty Attribute
    /// </summary>
    public IniFileKey( ) : this( null ) { }

    // Properties. The Note and Numbers properties must be read/write, so they
    // can be used as named parameters.
    //
    /// <summary>
    /// SimVar Name of the Attribute
    /// </summary>
    public string Name { get { return _name; } }
  }


  /// <summary>
  /// Attribute definition to Ignore a Property in a Class where IniKeys and Sections are defined
  /// </summary>
  [AttributeUsage( AttributeTargets.Property )]
  public class IniFileIgnore : Attribute
  {
    /// <summary>
    /// cTor: 
    /// </summary>
    public IniFileIgnore( )
    {
    }

  }
}

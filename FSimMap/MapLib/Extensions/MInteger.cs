using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib
{
  /// <summary>
  /// Extensions for Integer types 
  /// </summary>
  public static class Integer_Extension
  {
    #region Even / Odd
    /// <summary>
    /// True for an Even number
    /// </summary>
    public static bool Even( this byte _i ) => ((_i % 2) == 0);
    /// <summary>
    /// True for an Odd number
    /// </summary>
    public static bool Odd( this byte _i ) => !_i.Even( );
    /// <summary>
    /// True for an Even number
    /// </summary>
    public static bool Even( this short _i ) => ((_i % 2) == 0);
    /// <summary>
    /// True for an Odd number
    /// </summary>
    public static bool Odd( this short _i ) => !_i.Even( );
    /// <summary>
    /// True for an Even number
    /// </summary>
    public static bool Even( this ushort _i ) => ((_i % 2) == 0);
    /// <summary>
    /// True for an Odd number
    /// </summary>
    public static bool Odd( this ushort _i ) => !_i.Even( );
    /// <summary>
    /// True for an Even number
    /// </summary>
    public static bool Even( this int _i ) => ((_i % 2) == 0);
    /// <summary>
    /// True for an Odd number
    /// </summary>
    public static bool Odd( this int _i ) => !_i.Even( );
    /// <summary>
    /// True for an Even number
    /// </summary>
    public static bool Even( this uint _i ) => ((_i % 2) == 0);
    /// <summary>
    /// True for an Odd number
    /// </summary>
    public static bool Odd( this uint _i ) => !_i.Even( );
    /// <summary>
    /// True for an Even number
    /// </summary>
    public static bool Even( this long _i ) => ((_i % 2) == 0);
    /// <summary>
    /// True for an Odd number
    /// </summary>
    public static bool Odd( this long _i ) => !_i.Even( );
    /// <summary>
    /// True for an Even number
    /// </summary>
    public static bool Even( this ulong _i ) => ((_i % 2) == 0);
    /// <summary>
    /// True for an Odd number
    /// </summary>
    public static bool Odd( this ulong _i ) => !_i.Even( );

    #endregion

  }
}

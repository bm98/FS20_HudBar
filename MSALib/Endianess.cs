using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MSALib
{
  /// <summary>
  /// Deal with endianess
  /// </summary>
  internal class Endianess
  {

    /*
    * Thank You for the Swaps !
    *    https://stackoverflow.com/questions/19560436/bitwise-endian-swap-for-various-types
    */

    private static UInt16 SwapBytes( UInt16 x )
    {
      // swap adjacent 8-bit blocks
      return (ushort)(((UInt32)x >> 8) | ((UInt32)x << 8));
    }

    private static UInt32 SwapBytes( UInt32 x )
    {
      // swap adjacent 16-bit blocks
      x = (x >> 16) | (x << 16);
      // swap adjacent 8-bit blocks
      return ((x & 0xFF00FF00) >> 8) | ((x & 0x00FF00FF) << 8);
    }

    private static UInt64 SwapBytes( UInt64 x )
    {
      // swap adjacent 32-bit blocks
      x = (x >> 32) | (x << 32);
      // swap adjacent 16-bit blocks
      x = ((x & 0xFFFF0000FFFF0000) >> 16) | ((x & 0x0000FFFF0000FFFF) << 16);
      // swap adjacent 8-bit blocks
      return ((x & 0xFF00FF00FF00FF00) >> 8) | ((x & 0x00FF00FF00FF00FF) << 8);
    }

    // Reverse Endianess by swapping works only with unsigned

    /// <summary>
    /// Returns an unsigned big endian long converted to little endian
    /// </summary>
    /// <param name="be">An unsigned BE long</param>
    /// <returns>An unsigned LE long</returns>
    public static ulong ReverseEndianess( ulong be )
    {
      return SwapBytes( be );
    }

    /// <summary>
    /// Returns an unsigned big endian int converted to little endian
    /// </summary>
    /// <param name="be">An unsigned BE int</param>
    /// <returns>An unsigned LE int</returns>
    public static uint ReverseEndianess( uint be )
    {
      return SwapBytes( be );
    }

    /// <summary>
    /// Returns an unsigned big endian short converted to little endian
    /// </summary>
    /// <param name="be">An unsigned BE short</param>
    /// <returns>An unsigned LE short</returns>
    public static ushort ReverseEndianess( ushort be )
    {
      return SwapBytes( be );
    }


    /// <summary>
    /// Returns an signed big endian long converted to little endian
    /// </summary>
    /// <param name="be">An unsigned BE long</param>
    /// <returns>An unsigned LE long</returns>
    public static long ReverseEndianess( long be )
    {
      return (long)SwapBytes( (ulong)be );
    }

    /// <summary>
    /// Returns an signed big endian int converted to little endian
    /// </summary>
    /// <param name="be">An signed BE int</param>
    /// <returns>An unsigned LE int</returns>
    public static int ReverseEndianess( int be )
    {
      return (int)SwapBytes( (uint)be );
    }

    /// <summary>
    /// Returns an signed big endian short converted to little endian
    /// </summary>
    /// <param name="be">An unsigned BE short</param>
    /// <returns>An unsigned LE short</returns>
    public static short ReverseEndianess( short be )
    {
      return (short)SwapBytes( (ushort)be );
    }


  }
}

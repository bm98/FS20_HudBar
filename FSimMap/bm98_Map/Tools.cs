using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_Map
{
  /// <summary>
  /// Widely used static Methods
  /// </summary>
  internal class Tools
  {
    /// <summary>
    /// Returns a MemoryStream from an InputStream
    /// </summary>
    /// <param name="inputStream">An Input Stream</param>
    /// <param name="rewindStream">Rewind the InputString before retuning</param>
    /// <returns>A MemoryStream at Pos 0</returns>
    public static MemoryStream CopyStream( Stream inputStream, bool rewindStream )
    {
      const int readSize = 32 * 1024;
      var buffer = new byte[readSize];
      var mstream = new MemoryStream( );

      // bulk copy
      int count;
      while ((count = inputStream.Read( buffer, 0, readSize )) > 0) {
        mstream.Write( buffer, 0, count );
      }

      if (rewindStream) {
        inputStream.Seek( 0, SeekOrigin.Begin );
      }

      mstream.Seek( 0, SeekOrigin.Begin );
      return mstream;
    }


  }
}

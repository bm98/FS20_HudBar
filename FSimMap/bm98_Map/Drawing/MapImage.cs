using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// Internal Image for proceesing Maps
  /// </summary>
  internal class MapImage : IDisposable
  {
    #region Proxy

    /// <summary>
    /// Return a MapImage from a stream
    /// </summary>
    /// <param name="stream">A stream</param>
    /// <returns>A MapImage or null</returns>
    public static MapImage FromStream( Stream stream )
    {
      try {
        var image = Image.FromStream( stream, true, false );  // use .Net built in
        if (image != null) {
          return new MapImage( image );
        }
      }
      catch (Exception ex) {
        Debug.WriteLine( "FromStream: " + ex );
      }

      return null;
    }

    /// <summary>
    /// Return a MapImage from an array of bytes
    /// </summary>
    /// <param name="data">An array of bytes</param>
    /// <returns>A MapImage or null</returns>
    public static MapImage FromArray( byte[] data )
    {
      // via memory stream from the array
      var mstream = new MemoryStream( data, 0, data.Length, false, true );
      var mapImage = FromStream( mstream );
      if (mapImage != null) {
        mstream.Position = 0; // rewind
        mapImage.DataStream = mstream; // get the memory stream as well
      }

      return mapImage;
    }

    /// <summary>
    /// Save an Image to a stream
    /// </summary>
    /// <param name="image">The MapImage</param>
    /// <param name="stream">A Stream</param>
    /// <returns>True if successful</returns>
    public static bool Save( MapImage image, Stream stream )
    {
      bool ok = true;

      if (image.Img != null) {
        // try png
        try {
          image.Img.Save( stream, ImageFormat.Png ); // use .Net built in
        }
        catch {
          // try jpeg
          try {
            stream.Seek( 0, SeekOrigin.Begin );
            image.Img.Save( stream, ImageFormat.Jpeg ); // use .Net built in
          }
          catch {
            ok = false;
          }
        }
      }
      else {
        ok = false;
      }

      return ok;
    }

    #endregion

    /// <summary>
    /// The .Net Image of this MapImage
    /// </summary>
    public Image Img { get; private set; } = null;
    /// <summary>
    /// A MemoryStream containing the Image raw data
    /// </summary>
    public MemoryStream DataStream { get; private set; } = null;
    /// <summary>
    /// Set the DataStream for this MapImage
    /// </summary>
    /// <param name="stream">The Stream containing the raw data</param>
    public void SetDataStream(MemoryStream stream )
    {
      if (stream == null) return;
     
      stream.Seek( 0, SeekOrigin.Begin );
      DataStream = stream;
    }

    /// <summary>
    /// cTor: from Image
    /// </summary>
    /// <param name="image">A .Net Image</param>
    public MapImage( Image image )
    {
      Img = image;
    }

    #region DISPOSE

    private bool disposedValue;

    protected virtual void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
          Img?.Dispose( );
          DataStream?.Dispose( );
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~MapImage()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose( )
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose( disposing: true );
      GC.SuppressFinalize( this );
    }

    #endregion 
  }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DbgLib;

namespace MapLib
{
  /// <summary>
  /// Track the source of this imate
  /// </summary>
  internal enum ImgSource{
    Unknown=0,
    MemCache,
    DiskCache,
    Provider,
    Resource, // failed images are streamed from a Resource
  }

  /// <summary>
  /// Internal Image, encapsulates an Image derived only from Streams
  /// There are no public constructors - Use the static methods to Create and get this StreamImage
  /// 
  /// Note: MS Doc says one must maintain the originating stream open while the Image is in use
  /// </summary>
  internal class StreamImage : IDisposable
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    #region StreamImage Factory

    /// <summary>
    /// Return a MapImage from a stream
    /// </summary>
    /// <param name="stream">A stream</param>
    /// <returns>A StreamImage or null</returns>
    public static StreamImage FromStream( Stream stream )
    {
      var img = new StreamImage( stream );
      return img.IsValid ? img : null;
    }

    /// <summary>
    /// Return a MapImage from an array of bytes
    /// </summary>
    /// <param name="data">An array of bytes</param>
    /// <returns>A StreamImage or null</returns>
    public static StreamImage FromArray( byte[] data )
    {
      // via memory stream from the array
      StreamImage streamImage;
      using (var mstream = new MemoryStream( data, 0, data.Length, false, true )) {
        streamImage = FromStream( mstream );
      }
      return streamImage;
    }

    /// <summary>
    /// Save an Image to a stream
    /// </summary>
    /// <param name="image">The MapImage</param>
    /// <param name="stream">A Stream</param>
    /// <returns>True if successful</returns>
    public static bool ToStream( StreamImage image, Stream stream )
    {
      return image.ToStream( stream );
    }

    #endregion

    // The Image
    protected Image _image = null;
    // The related data stream
    protected MemoryStream _dataStream = null;

    /// <summary>
    /// Where was the image sourced from
    /// </summary>
    public ImgSource ImageSource { get; set; } = ImgSource.Unknown;

    /// <summary>
    /// The .Net Image of this MapImage
    /// </summary>
    public Image Img => _image;
    /// <summary>
    /// The data stream 
    /// </summary>
    public MemoryStream DataStream => _dataStream;

    /// <summary>
    /// True if a valid Image is available
    /// </summary>
    public bool IsValid => Img != null;

    /// <summary>
    /// True if the this is a load failed image (replacement)
    /// Should not be cached ...
    /// </summary>
    public bool IsFailedImage { get; protected set; } = false;
    /// <summary>
    /// True if a retry should be done
    /// </summary>
    public bool ShouldRetry { get; protected set; } = false;

    /// <summary>
    /// cTor: is protected, 
    /// </summary>
    protected StreamImage( ) { }

    /// <summary>
    /// cTor: From Stream
    /// </summary>
    /// <param name="stream">An open stream</param>
    protected StreamImage( Stream stream )
    {
      try {
        // we maintain our own stream copy within the StreamImage
        var iStream = Tools.CopyStream( stream, true );

        var image = Image.FromStream( iStream, true, false );  // use .Net built in
        if (image != null) {
          _image = image;
          _dataStream = iStream;
        }
      }
      catch (Exception ex) {
        LOG.LogException( "StreamImage", ex, "MapImage.FromStream" );
        _image = null;
        _dataStream = null;
      }
    }

    /// <summary>
    /// Save an Image to a stream
    /// </summary>
    /// <param name="stream">A Stream</param>
    /// <returns>True if successful</returns>
    public bool ToStream( Stream stream )
    {
      if (stream == null) return false;
      if (!IsValid) return false;

      bool ok = true;

      // try png
      try {
        _image.Save( stream, ImageFormat.Png ); // use .Net built in
      }
      catch {
        // try jpeg
        try {
          stream.Seek( 0, SeekOrigin.Begin );
          _image.Save( stream, ImageFormat.Jpeg ); // use .Net built in
        }
        catch {
          ok = false;
        }
      }

      return ok;
    }


    /// <summary>
    /// Creates an exact Copy of this StreamImage
    /// </summary>
    /// <returns>A StreamImage or null</returns>
    public virtual StreamImage Clone( )
    {
      if (Img == null) return null;

      return new StreamImage( this.DataStream );
    }

    /// <summary>
    /// The Serialized Image Data sufficient to restore the Image
    /// </summary>
    /// <returns>Array of bytes or null if no such data is available</returns>
    public byte[] GetChacheData( )
    {
      if (_image == null) return new byte[0]; // no image => no data

      return _dataStream.GetBuffer( ); // should create it if it was already dropped
    }


    #region DISPOSE

    private bool disposedValue;

    protected virtual void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
          _image?.Dispose( );
          _dataStream?.Close( );
          _dataStream?.Dispose( );
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

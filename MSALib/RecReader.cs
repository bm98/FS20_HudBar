using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

using NLog;


namespace MSALib
{
  /// <summary>
  /// Implements a binary reader for BGL files
  /// </summary>
  internal class RecReader : IDisposable
  {
    // NLog setup
    private static readonly Logger LOG = LogManager.GetCurrentClassLogger( );

#if DEBUG
    public static bool TRACE = false; // dump in DEBUG only if at all - slows signicifcantly 
#else
    public static bool TRACE = false;
#endif

    // Dumps to Console
    private static void TraceSupport( long position, int size, byte[] bytes, string typeName )
    {
      StringBuilder builder = new StringBuilder( );
      builder.AppendLine( );
      builder.Append( $"{size,6:####0}  bytes: " + typeName.Replace( "+MyRecord", "" ) ).AppendLine( );
      builder.AppendFormat( "0x{0:X8}: ", position );
      foreach (byte b in bytes) {
        builder.AppendFormat( "{0:X2} ", b );
      }
      using (LOG.PushScopeNested( "Type:" + typeName.Replace( "+MyRecord", "" ) )) LOG.Trace( builder.ToString( ) );
    }

    /// <summary>
    /// Allocates and reads bytes of the size of one record 
    /// and returns the allocated bytes as structure - allowing structured access to binary data 
    /// Note: there is no error checking whatsoever here - so better make sure everything is OK
    /// </summary>
    /// <typeparam name="T">The record type to read</typeparam>
    /// <param name="reader">A binary reader</param>
    /// <param name="dump">Dump if true</param>
    /// <returns>The read record</returns>
    public static T ByteToType<T>( BinaryReader reader, bool dump = false ) where T : struct
    {
      long rPos = reader.BaseStream.Position;
      byte[] bytes = reader.ReadBytes( Marshal.SizeOf( typeof( T ) ) );
      if (TRACE) TraceSupport( rPos, Marshal.SizeOf( typeof( T ) ), bytes, typeof( T ).ToString( ) );

      GCHandle handle = GCHandle.Alloc( bytes, GCHandleType.Pinned );
      T theStructure = (T)Marshal.PtrToStructure( handle.AddrOfPinnedObject( ), typeof( T ) );
      handle.Free( );

      return theStructure;
    }

    /// <summary>
    /// Allocates and reads bytes of the size of one record 
    /// and returns the allocated bytes as structure - allowing structured access to binary data 
    /// Note: there is no error checking whatsoever here - so better make sure everything is OK
    /// </summary>
    /// <param name="reader">A binary reader</param>
    /// <param name="size">The number of bytes to read</param>
    /// <param name="dump">Dump if true</param>
    /// <returns>The read array</returns>
    public static byte[] ByteToArray( BinaryReader reader, int size, bool dump = false )
    {
      long rPos = reader.BaseStream.Position;
      byte[] bytes = reader.ReadBytes( size );
      if (TRACE) TraceSupport( rPos, size, bytes, "Raw Data" );
      return bytes;
    }

    // CLASS

    private FileStream _filestr = null;
    private MemoryStream _memstr = null;
    private BinaryReader _reader = null;

    private DateTime _fileCreatedT;  // hold the file creation time

    /// <summary>
    /// When true reading of value types will convert big to little endian
    /// </summary>
    public bool BigEndianMode { get; set; } = false;

    /// <summary>
    /// Read a byte
    /// </summary>
    public byte ReadByte( ) => TheReader.ReadByte( );
    /// <summary>
    /// Read a signed byte
    /// </summary>
    public sbyte ReadSByte( ) => TheReader.ReadSByte( );

    public ushort ReadUInt16( )
    {
      var value = TheReader.ReadUInt16( );
      return (BigEndianMode) ? Endianess.ReverseEndianess( value ) : value;
    }
    public short ReadInt16( )
    {
      var value = TheReader.ReadInt16( );
      return (BigEndianMode) ? Endianess.ReverseEndianess( value ) : value;
    }

    public uint ReadUInt32( )
    {
      var value = TheReader.ReadUInt32( );
      return (BigEndianMode) ? Endianess.ReverseEndianess( value ) : value;
    }
    public int ReadInt32( )
    {
      var value = TheReader.ReadInt32( );
      return (BigEndianMode) ? Endianess.ReverseEndianess( value ) : value;
    }

    public ulong ReadUInt64( )
    {
      var value = TheReader.ReadUInt64( );
      return (BigEndianMode) ? Endianess.ReverseEndianess( value ) : value;
    }
    public long ReadInt64( )
    {
      var value = TheReader.ReadInt64( );
      return (BigEndianMode) ? Endianess.ReverseEndianess( value ) : value;
    }


    /// <summary>
    /// ctor: 
    /// </summary>
    /// <param name="filename">The filename</param>
    public RecReader( string filename )
    {
      Open( filename );
      _fileCreatedT = File.GetCreationTimeUtc( _filestr.Name );
    }

    /// <summary>
    /// ctor: 
    /// </summary>
    /// <param name="stream">The filestream</param>
    public RecReader( Stream stream, DateTime fileCreatedT )
    {
      _fileCreatedT = fileCreatedT;
      if (stream.CanSeek) {
        stream.Seek( 0, SeekOrigin.Begin );
      }
      _reader = new BinaryReader( stream );
    }

    /// <summary>
    /// ctor: 
    /// Supports a memory stream e.g. for zipped files
    /// </summary>
    /// <param name="memStream">The MemoryStream</param>
    /// <param name="fileCreatedT">The filedate of the undelying file</param>
    public RecReader( MemoryStream memStream, DateTime fileCreatedT )
    {
      _fileCreatedT = fileCreatedT;
      _memstr = memStream;
      _memstr.Seek( 0, SeekOrigin.Begin );
      _reader = new BinaryReader( _memstr );
    }

    /// <summary>
    /// Open the file
    /// </summary>
    /// <param name="filename">The filename</param>
    /// <returns>True when successfull</returns>
    private bool Open( string filename )
    {
      if (File.Exists( filename )) {
        _filestr = File.OpenRead( filename );
        _reader = new BinaryReader( _filestr );
        return true;
      }

      return false;
    }


    /// <summary>
    /// Returns the Reader
    /// </summary>
    public BinaryReader TheReader {
      get { return _reader; }
    }

    /// <summary>
    /// Seek to the desired fileposition
    /// </summary>
    /// <param name="fileOffset">The 0 based fileoffset</param>
    public bool SeekTo( uint fileOffset )
    {
      if (_reader.BaseStream.CanSeek) {
        _reader.BaseStream.Seek( fileOffset, SeekOrigin.Begin );
        return true;
      }
      return false;
    }

    /// <summary>
    /// Seek a delta from the current position to the desired fileposition
    /// </summary>
    /// <param name="fileDelta">The 0 based Seek delta from current</param>
    public bool Seek( int fileDelta )
    {
      if (_reader.BaseStream.CanSeek) {
        _reader.BaseStream.Seek( fileDelta, SeekOrigin.Current );
        return true;
      }
      return false;
    }

    /// <summary>
    /// Returns the file status
    /// </summary>
    /// <returns>True if the file is open and the reader can read</returns>
    public bool IsOpen( )
    {
      if (_filestr != null) return _filestr.CanRead;
      else if (_memstr != null) return _memstr.CanRead;
      else return _reader.BaseStream.CanRead;
     // else return false;
    }

    public bool IsAtEnd( )
    {
      return _reader.BaseStream.Position == _reader.BaseStream.Length;
    }

    /// <summary>
    /// Returns the file creation timestamp
    /// </summary>
    public DateTime FileDateTime {
      get { return _fileCreatedT; }
      set {; }
    }

    #region Dispose

    private bool disposedValue;

    protected virtual void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
          _filestr?.Dispose( );
          _memstr?.Dispose( );
          _reader?.Dispose( );
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~RecReader()
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

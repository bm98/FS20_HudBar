using System;
using System.IO;
using System.Linq;

using CoordLib.MercatorTiles;

using DbgLib;

using LiteDB;

using MapLib.Sources.Providers;

namespace MapLib.Sources.DiskCache
{
  /// <summary>
  /// LiteDB Tile Cache implementation
  /// http://www.litedb.org/docs/
  /// 
  /// Needs:
  ///  Path to Cache Files Location
  ///  
  /// One Cache file per type 
  /// 
  /// Considerations:
  /// DBLite will use only one Index and then scan for subsequent field matches
  /// So we may want to get the x/y/z coordinates into one single ID to make use of the index
  /// 
  /// x/y values are Mercator Grid numbers that are ranging from (e.g. 2^20 x 2^20, or 1'048'576 x 1'048'576 tiles at zoom level 20)
  /// Grids are defined in x and y as 0..24 i.e. 2^25, max zoom is therefore 24 i.e. 2^5
  /// we use a GridID of 26+26+6 = 58 bits put into an Int64
  /// 
  /// The DB access is enclosed in using {} for any operation
  /// this might create substantial overhead but a DB access manager for the multiple threads running could be added later.
  /// 
  /// Needs a Timestamp to do the housekeeping
  /// 
  /// Index on GridID
  /// Index on StoreTime
  /// 
  /// </summary>
  internal class LiteDBCache
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    /// <summary>
    /// The Record to store in the Database
    /// using the DBLite Object Mapper
    /// </summary>
    private class CacheTileDbRecord
    {
      // our collection name
      public static string CollectionName = "MapLib_Tiles";

      // Id for DBLite purposes only
      [BsonId]
      public ObjectId Id { get; }

      // Unique Tile ID (the X/Y/Z composite ID)
      public Int64 GridID { get; }
      // timestamp for housekeeping
      public DateTime StoreTime { get; }
      // Tile Data Array
      public byte[] TileData { get; }

      // the following ones are for development purpose only (and can be removed eventually)
      public string SourceID { get; }
      public int X { get; }
      public int Y { get; }
      public int Z { get; }


      // Should be used by the class to create the record
      public CacheTileDbRecord( string sourceID, TileXY tileXY, int zoom, byte[] data )
      {
        Id = ObjectId.NewObjectId( );
        GridID = GID( tileXY, (ushort)zoom ); // create the Grid ID for fast access
        StoreTime = DateTime.Now;    // set timestamp on create
        TileData = data;

        SourceID = sourceID;
        X = tileXY.X;
        Y = tileXY.Y;
        Z = zoom;
      }

      // should be used by DBLite to create the obj when reading
      [BsonCtor]
      public CacheTileDbRecord( ObjectId _id, Int64 gridID, DateTime storeTime, byte[] tileData, string sourceID, int x, int y, int z )
      {
        Id = _id;
        GridID = gridID; // create the Grid ID for fast access
        StoreTime = storeTime;    // set timestamp on create
        TileData = tileData;

        // the following ones are for development purpose only (and can be removed eventually)
        SourceID = sourceID;
        X = x;
        Y = y;
        Z = z;
      }

    }

    private static readonly int DiskCacheMB = MapProviderBase.ProviderIni.DiskCacheMB;

    private readonly long c_initialSize = DiskCacheMB * 1024 * 1024; // 32..1024MB the initial Size for each DB file
    private const string c_access = "shared"; // The DBLite access prop (use this for shared access, MapLib uses threads to load tiles)
    private const string c_dbfTemplate = "MapLibCache_{$1}.dblite"; // e.g. MapLibCache_{Provider}.dblite

    private const string c_defaultCacheLocation = "."; // Default cache directory / App dir
    private string _cacheLocation = c_defaultCacheLocation; // Main cache directory

    // Create a GridID from X,Y,Z
    private static Int64 GID( TileXY tileXY, ushort zoom )
    {
      return ((((long)tileXY.X << 26) + (long)tileXY.Y) << 6) + (long)zoom;
    }

    // create the DBFileNane from the SourceType (string)
    private string ProviderCacheFile( string sourceID )
    {
      string dbf = c_dbfTemplate.Replace( "$1", sourceID );
      return Path.Combine( _cacheLocation, dbf );
    }

    private string ConnectionString( string sourceID )
    {
      return $"Filename='{ProviderCacheFile( sourceID )}';Connection={c_access};InitialSize={c_initialSize}";
    }


    /// <summary>
    /// The GMap Cache Location
    ///  for each Provider a single file is created
    /// </summary>
    public string CacheLocation {
      get => _cacheLocation;
      set {
        if (Directory.Exists( value )) {
          _cacheLocation = value;
          return;
        }
        // revert to default if the given one does not exist
        _cacheLocation = c_defaultCacheLocation;
      }
    }

    /// <summary>
    /// Send the Data[] + args into the Cache 
    /// Note: this gets called from multiple threads in parallel
    /// </summary>
    /// <param name="cacheData">Tile Data</param>
    /// <param name="mapProvider">The ID of the provider source</param>
    /// <param name="tileXY">Tile Position (X,Y)</param>
    /// <param name="zoom">The zoomLevel (Z)</param>
    /// <returns>True if successful</returns>
    public bool PutImageToCache( byte[] cacheData, MapProvider mapProvider, TileXY tileXY, ushort zoom )
    {
      if (cacheData == null) {
        LOG.Info( "DBLite-PutImageToCache", $"Tile with Null Data received - ignored" );
        return false;
      }
      // using the string representation within the LiteDB 
      string sourceID = mapProvider.ToString( );

      // this shall never fail (e.g. data is becoming unavilable, img was disposed...)
      try {
        using (var db = new LiteDatabase( ConnectionString( sourceID ) )) {
          var col = db.GetCollection<CacheTileDbRecord>( CacheTileDbRecord.CollectionName );
          //var deleted = col.DeleteMany( x => x.GridID == GID( pos, zoom )); // delete an existing one - can this happen ?? should not really
          // add the new one
          CacheTileDbRecord rec = new CacheTileDbRecord( sourceID, tileXY, (int)zoom, cacheData );
          col.Insert( rec );
          // Indexes are either created or used if they exist
          col.EnsureIndex( x => x.GridID );
          col.EnsureIndex( x => x.StoreTime );
        }
        return true;
      }
      catch (Exception ex) {
        LOG.Error( "DBLite-PutImageToCache", ex, $"Exception while caching ({Tools.ToFullKey( mapProvider, tileXY, zoom )})" );
      }
      return false;
    }

    /// <summary>
    /// Send the MapImage into the Cache
    /// </summary>
    /// <param name="mapImage">A MapImage</param>
    /// <returns>True if successful</returns>
    public bool PutImageToCache( MapImage mapImage )
    {
      byte[] cacheData = mapImage.GetChacheData( );
      return PutImageToCache( cacheData, mapImage.MapImageID.MapProvider, mapImage.MapImageID.TileXY, mapImage.MapImageID.ZoomLevel );
    }


    /// <summary>
    /// Fetch a MapImage from the Cache DB
    /// </summary>
    /// <param name="mapProvider">The ID of the provider source</param>
    /// <param name="tileXY">Tile Position (X,Y)</param>
    /// <param name="zoom">The zoomLevel (Z)</param>
    /// <returns>An Image or null</returns>
    public MapImage GetImageFromCache( MapProvider mapProvider, TileXY tileXY, ushort zoom )
    {
      // sanity
      if (mapProvider == MapProvider.DummyProvider) return null;

      // using the string representation within the LiteDB 
      string sourceID = mapProvider.ToString( );

      MapImage mapImage = null;
      // this shall never fail for whatever reason
      try {
        using (var db = new LiteDatabase( ConnectionString( sourceID ) )) {
          var col = db.GetCollection<CacheTileDbRecord>( CacheTileDbRecord.CollectionName );
          // get the first (or none), there should not be >1 with the same GID anyway
          var result = col.FindOne( x => x.GridID == GID( tileXY, zoom ) );
          if (result != null) {
            MapImageID mapImageID = new MapImageID( new TileXY( result.X, result.Y ), (ushort)result.Z, mapProvider );
            mapImage = MapImage.FromArray( result.TileData, mapImageID ); // retrieve the Tile as MapImage
          }
        }
      }
      catch (Exception ex) {
        LOG.Error( "DBLite-PutImageToCache", ex, $"Exception while retrieving ({Tools.ToFullKey( mapProvider, tileXY, zoom )})" );
        mapImage = null;
      }
      return mapImage;
    }


    /// <summary>
    /// Delete old tiles beyond a supplied date
    /// </summary>
    /// <param name="date">Tiles older than this will be deleted.</param>
    /// <param name="mapProvider">provider name</param>
    /// <returns>The number of deleted tiles.</returns>
    public int DeleteOlderThan( DateTime date, MapProvider mapProvider )
    {
      if (mapProvider == MapProvider.DummyProvider) return 0;

      string sourceID = mapProvider.ToString( ); // using the string representation here
      if (!File.Exists( ProviderCacheFile( sourceID ) )) return 0; // no such db file (this is OK .. not yet used..)

      int result = 0;
      try {
        using (var db = new LiteDatabase( ConnectionString( sourceID ) )) {
          var col = db.GetCollection<CacheTileDbRecord>( CacheTileDbRecord.CollectionName );
          result = col.DeleteMany( x => x.StoreTime < date );
        }
        LOG.Info( "DBLite-DeleteOlderThan", $"Deleted {result} records" );
      }
      catch (Exception ex) {
        LOG.Error( "DBLite-DeleteOlderThan", ex, "Failed" );
        result = 0;
      }
      return result;
    }

    /// <summary>
    /// Delete old tiles up to get back to maxRecNumber
    /// </summary>
    /// <param name="maxRecNumber">Max number of records in DB</param>
    /// <param name="mapProvider">provider name</param>
    /// <returns>The number of deleted tiles.</returns>
    public int DeleteByMaxRecNumber( long maxRecNumber, MapProvider mapProvider )
    {
      if (mapProvider == MapProvider.DummyProvider) return 0;

      string sourceID = mapProvider.ToString( ); // using the string representation here
      if (!File.Exists( ProviderCacheFile( sourceID ) )) return 0; // EXIT no such db file (this is OK .. not yet used..)

      int result = 0;
      try {
        using (var db = new LiteDatabase( ConnectionString( sourceID ) )) {
          var col = db.GetCollection<CacheTileDbRecord>( CacheTileDbRecord.CollectionName );
          int recToDelete = (int)(col.LongCount( ) - maxRecNumber);
          if (recToDelete > 0) {
            var q = col.Query( ).OrderBy( x => x.StoreTime, Query.Ascending ).Limit( recToDelete ).ToEnumerable( );
            result = col.DeleteMany( x => q.Contains( x ) );
          }
        }
        LOG.Info( "DBLite-DeleteByMaxRecNumber", $"Deleted {result} records" );
      }
      catch (Exception ex) {
        LOG.Error( "DBLite-DeleteByMaxRecNumber", ex, "Failed" );
        result = 0;
      }
      return result;
    }


  }
}

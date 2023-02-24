using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib;

using MapLib.Tiles;
using MapLib.Sources.Providers;

namespace MapLib
{
  /// <summary>
  /// Enum of known and registered providers
  /// </summary>
  public enum MapProvider
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    DummyProvider = 0,

    OSM_OpenStreetMap,
    OpenTopo,
    Stamen_Terrain,

    CB_WAC,
    CB_SEC,
    CB_TAC,
    CB_ENRA,
    CB_ENRL,
    CB_ENRH,

    ESRI_Imagery,
    ESRI_StreetMap,
    ESRI_WorldTopo,

    BING_Imagery,
    BING_ImageryLabels,
    BING_OStreetMap,

    USER_TILES_1,
    USER_TILES_2,
    USER_TILES_3,
    USER_TILES_4,
    USER_TILES_5,
    USER_TILES_6,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }


  /// <summary>
  /// Status of the loading process 
  /// </summary>
  public enum ImageLoadingStatus
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    Unknown = 0,
    Idle,
    Loading,
    LoadComplete,
    LoadFailed,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }

  /// <summary>
  /// TileMatrix Sides (bits, can be combined)
  /// </summary>
  [Flags]
  public enum TileMatrixSide
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    None = 0x00,
    Left = 0x01,
    Top = 0x02,
    Right = 0x04,
    Bottom = 0x08,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }

  /// <summary>
  /// Singleton:
  /// 
  /// Main Map Manager API 
  ///   En/Disable Sources (Memory, Disk Cache), Provider
  ///   
  /// Service Methods
  ///   Create a Map - returns a Matrix of Size
  ///   
  ///   Housekeeping - Shutdown() to stop background processing
  ///   (note cannot be restarted though..)
  /// 
  /// </summary>
  public sealed class MapManager
  {
    // Singleton Pattern
    /// <summary>
    /// The MapManager Instance
    /// </summary>
    public static MapManager Instance => lazy.Value;
    private static readonly Lazy<MapManager> lazy = new Lazy<MapManager>( ( ) => new MapManager( ) );
    private MapManager( )
    {
      _currentProvider = MapProvider.DummyProvider;
    }

    private MapProvider _currentProvider = MapProvider.OSM_OpenStreetMap;

    #region Service Status 

    /// <summary>
    /// Get: Memory Cache Status
    /// </summary>
    public bool MemCacheStatus => Service.RequestScheduler.Instance.ServiceStatus_MemoryCache;
    /// <summary>
    /// Set the Memory Cache Status
    /// </summary>
    /// <param name="enabled">True to enable, False to disable</param>
    public void SetMemCacheStatus( bool enabled ) => Service.RequestScheduler.Instance.SetServiceStatus_MemoryCache( enabled );

    /// <summary>
    /// Get: Disk Cache Status
    /// </summary>
    public bool DiskCacheStatus => Service.RequestScheduler.Instance.ServiceStatus_DiskCache;
    /// <summary>
    /// Set the Disk Cache Status
    /// </summary>
    /// <param name="enabled">True to enable, False to disable</param>
    public void SetDiskCacheStatus( bool enabled ) => Service.RequestScheduler.Instance.SetServiceStatus_DiskCache( enabled );

    /// <summary>
    /// Get: Provider Source Status
    /// </summary>
    public bool ProviderStatus => Service.RequestScheduler.Instance.ServiceStatus_Providers;
    /// <summary>
    /// Set the Provider Source Status
    /// </summary>
    /// <param name="enabled">True to enable, False to disable</param>
    public void SetProviderSource( bool enabled ) => Service.RequestScheduler.Instance.SetServiceStatus_MapProvider( enabled );

    #endregion

    /// <summary>
    /// FIRST THING TO DO !!
    /// Initialize the MapLib by providing a user path for the Provide INI file
    /// Will take preference over the default one
    /// </summary>
    /// <param name="iniPath"></param>
    public void InitMapLib( string iniPath )
    {
      MapProviderBase.InitProviderBase( iniPath );
      _currentProvider = DefaultProvider;
    }

    /// <summary>
    /// The currently active MapProvider
    /// </summary>
    public MapProvider CurrentProvider => _currentProvider;
    /// <summary>
    /// Change the Provider
    /// </summary>
    /// <param name="mapProvider">A MapProvider</param>
    public void SetNewProvider( MapProvider mapProvider ) => _currentProvider = mapProvider;

    /// <summary>
    /// The Default Provider from the INI File - may hint which one to use
    /// </summary>
    public MapProvider DefaultProvider => MapProviderBase.ProviderIni.DefaultProvider;

    /// <summary>
    /// Enabled Providers from the INI file
    /// </summary>
    public IEnumerable<MapProvider> EnabledProviders => MapProviderBase.ProviderIni.EnabledProviders;


    /// <summary>
    /// Returns the common name for a provider
    /// </summary>
    /// <param name="provider">A provider</param>
    /// <returns>The Name</returns>
    public string ProviderName(MapProvider provider )
    {
      return MapProviderBase.ProviderIni.ProviderName( provider );
    }

    /// <summary>
    /// Get: the disk cache location (folder path)
    /// </summary>
    public string DiskCacheLocation => Service.RequestScheduler.Instance.DiskSource.DiskCacheLocation;

    /// <summary>
    /// Set the folder path for the DiskCache
    /// default is the App Dir
    /// </summary>
    /// <param name="folderPath">A folder path</param>
    public void SetDiskCacheLocation( string folderPath )
    {
      Service.RequestScheduler.Instance.DiskSource.SetDiskCacheLocation( folderPath );
    }


    /// <summary>
    /// To Shutdown before closing the App
    ///  will cancel background tasks 
    ///  In general this should NOT be used as the App closing will do it anyway
    /// </summary>
    public void ShutDown( )
    {
      Service.RequestScheduler.Instance.Abort( );
    }

    // some variants to create maps

    /// <summary>
    /// Create and return a Map from Dimensions
    /// </summary>
    /// <param name="width">Number of horizontal Tiles</param>
    /// <param name="height">Number of vertical Tiles</param>
    /// <returns>A TileMatrix</returns>
    public TileMatrix CreateMap( int width, int height )
    {
      TileMatrix matrix = new TileMatrix( (uint)width, (uint)height );
      return matrix;
    }

    /// <summary>
    /// Create and return a Map from Dimensions
    /// </summary>
    /// <param name="tileSize">Tile Size</param>
    /// <returns>A TileMatrix</returns>
    public TileMatrix CreateMap( Size tileSize )
    {
      return CreateMap( tileSize.Width, tileSize.Height );
    }

    /// <summary>
    /// Create a Map with a minimum size for a Provider (TileSize matters)
    /// this will return the tile size based map that fit the constraints
    /// </summary>
    /// <param name="mapProvider">The Map Provider</param>
    /// <param name="minPixelWidth">Minimum width [pixel]</param>
    /// <param name="minPixelHeight">Minimum height [pixel]</param>
    /// <param name="borderAddon_perc">Tile size % to add to maintain the min Size and cover the area (default 50%)</param>
    /// <returns>A TileMatrix</returns>
    public TileMatrix CreateMinSizeMap( MapProvider mapProvider, int minPixelWidth, int minPixelHeight, float borderAddon_perc = 50f )
    {
      if (mapProvider == MapProvider.DummyProvider) return null;

      Size tileSize = CoordLib.MercatorTiles.Projection.TileSize;
      // add padding 
      int minWidth = minPixelWidth + (int)((borderAddon_perc / 100) * tileSize.Width);
      int minHeight = minPixelHeight + (int)((borderAddon_perc / 100) * tileSize.Height);
      // calc number of tiles now from padded Size
      int xTiles = (minWidth / tileSize.Width) + ((minWidth % tileSize.Width) > 0 ? 1 : 0); // add one for fractional tiles
      int yTiles = (minHeight / tileSize.Height) + ((minHeight % tileSize.Height) > 0 ? 1 : 0); // add one for fractional tiles

      return CreateMap( xTiles, yTiles );
    }

    /// <summary>
    /// Create a Map with a minimum size for a Provider (TileSize matters)
    /// this will return the tile size based map that fit the constraints
    /// </summary>
    /// <param name="mapProvider">The Map Provider</param>
    /// <param name="pixelSize">Minimum size [pixel]</param>
    /// <param name="borderAddon_perc">Tile size % to add to maintain the min Size and cover the area (default 50%)</param>
    /// <returns>A TileMatrix</returns>
    public TileMatrix CreateMinSizeMap( MapProvider mapProvider, Size pixelSize, float borderAddon_perc = 50f )
    {
      return CreateMinSizeMap( mapProvider, pixelSize.Width, pixelSize.Height, borderAddon_perc );
    }

  }
}

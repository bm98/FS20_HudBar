using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CoordLib.MercatorTiles;

using DbgLib;

using MapLib.Service;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// Base class for Data Providers
  /// </summary>
  internal abstract class MapProviderBase : IImgSource
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );


    #region STATIC Service Parts

    private static bool _serviceStatus = false;
    private static bool _initStatus = false;

    /// <summary>
    /// A list of registered MapProviders
    /// </summary>
    static readonly Dictionary<MapProvider, MapProviderBase> s_providerCat = new Dictionary<MapProvider, MapProviderBase>( );
    /// <summary>
    /// Returns a Provider Instance or EmptyProvider if not found
    /// </summary>
    /// <param name="provider">The Provider</param>
    /// <returns>A Provicer Instance or EmptyProvider</returns>
    public static MapProviderBase GetProviderInstance( MapProvider provider )
    {
      if (s_providerCat.TryGetValue( provider, out MapProviderBase p )) {
        return p;
      }
      return EmptyProvider.Instance;
    }

    /// <summary>
    /// Set the Enabled status for all Providers
    /// </summary>
    /// <param name="enabled">True if to be set Enabled</param>
    public static void SetServiceStatus( bool enabled )
    {
      _serviceStatus = enabled;
      foreach (var p in s_providerCat.Values) {
        p.ProviderEnabled = enabled;
      }
    }

    /// <summary>
    /// True if providers are enabled
    /// </summary>
    public static bool Enabled => _serviceStatus;

    /// <summary>
    /// Proxy for net access (NOT USED SO FAR)
    /// </summary>
    public static IWebProxy WebProxy;

    /// <summary>
    /// The web request factory
    /// </summary>
    public static Func<MapProviderBase, string, WebRequest> WebRequestFactory = null;

    /// <summary>
    /// NetworkCredential for tile http access
    /// </summary>
    public static ICredentials Credentials = null;

    /// <summary>
    /// Get; Set: User-agent HTTP header.
    /// It's pseudo-randomized to avoid blockages...
    /// </summary>
    public static string UserAgent;

    /// <summary>
    /// Timeout for connections
    /// </summary>
    public static int TimeoutMs = 60 * 1000; // 60 sec

    /// <summary>
    /// Time To Live of cache, in hours. Default: 240 (10 days).
    /// </summary>
    public static int TTLCache = 240;

    /// <summary>
    /// The Provider Ini File
    /// </summary>
    public static ProviderIni ProviderIni { get; private set; } = new ProviderIni( "" );


    static readonly string requestAccept = "*/*";
    static readonly string responseContentType = "image";
    static readonly string responseContentTypeGeneric = "application/octet-stream"; // BM Added - Tessera does not respond with image

    static readonly HttpClient _client = new HttpClient( );

    static MapProviderBase( )
    {
      WebProxy = BypassWebProxy.Instance;
      // OSM asks for a legit UserAgent ID - so provide one
      FileVersionInfo fileVersion = FileVersionInfo.GetVersionInfo( Assembly.GetAssembly( typeof( MapProviderBase ) ).Location );
      // will evaluate into "MapLib/m.n.o.p" e.g "MapLib/0.2.0.10" as of writing.
      UserAgent = string.Format( $"{fileVersion.ProductName}/{fileVersion.FileVersion}" );

      _client.DefaultRequestHeaders.Add( "User-Agent", UserAgent );
      _client.DefaultRequestHeaders.Add( "Accept", requestAccept );
      _client.Timeout = TimeSpan.FromMilliseconds( TimeoutMs );
    }

    /// <summary>
    /// Initialize the Base Provider before using Map Provider Services
    /// </summary>
    /// <param name="userPath">A path to a user folder where an INI file should be located</param>
    public static void InitProviderBase( string userPath )
    {
      ProviderIni = new ProviderIni( userPath );

      // trigger all providers to create themselves
      _ = EmptyProvider.Instance;
      // init Open Servers
      _ = OSM_OpenStreetMap.Instance;
      _ = OpenTopoProvider.Instance;
      _ = StamenTerrainProvider.Instance;
      // init ChartBundle
      _ = CB_WAC.Instance;
      _ = CB_SEC.Instance;
      _ = CB_TAC.Instance;
      _ = CB_ENRA.Instance;
      _ = CB_ENRL.Instance;
      _ = CB_ENRH.Instance;
      // init ESRI/ARCGIS
      _ = ESRI_Imagery.Instance;
      _ = ESRI_StreetMap.Instance;
      _ = ESRI_WorldTopo.Instance;
      // init Bing
      _ = Bing_Imagery.Instance;
      _ = Bing_ImageryLabels.Instance;
      _ = Bing_OSMap.Instance;
      // init user ones
      _ = UserTiles1Provider.Instance;
      _ = UserTiles2Provider.Instance;
      _ = UserTiles3Provider.Instance;
      _ = UserTiles4Provider.Instance;
      _ = UserTiles5Provider.Instance;
      _ = UserTiles6Provider.Instance;

      // initialized
      _initStatus = true;
      // default is enabled
      SetServiceStatus( true );
    }

    #endregion

    #region IImgSource interface implementation

    /// <summary>
    /// Whether or not the Source is Enabled
    /// </summary>
    public bool ProviderEnabled { get; set; } = true;

    /// <summary>
    /// Try get the image from the local source or propagate
    ///  -- Note: This is a synchronous call to a webserver and will eventually return or timeout
    /// </summary>
    /// <param name="jobWrapper">A JobWrapper</param>
    /// <returns>A MapImage or null</returns>
    public MapImage GetTileImage( LoaderJobWrapper jobWrapper )
    {
      //      Debug.WriteLine( $"MapProviderBase.GetTileImage: Got Request for: {jobWrapper.MapImageID.FullKey}" );
      if (!_initStatus) throw new InvalidOperationException( "Called before initialized! Aborted" );

      var imageSought = jobWrapper.MapImageID;
      MapImage img = GetTileImage( imageSought );
      if (img != null) {
        //        Debug.WriteLine( $"MapProviderBase.GetTileImage: Served from PROVIDER SOURCE - {imageSought.FullKey}" );
        return img;
      }
      else {
        // usually a provider is the last source .. but then we may have ideas...
        var nextSource = jobWrapper.GetNextSource( );
        img = nextSource?.GetTileImage( jobWrapper );
        if (img != null) {
          // put into this cache or leave it alone
        }
        return img;
      }
    }

    public void MaintainCacheSize( )
    {
      throw new NotImplementedException( ); // cannot maintain a cache...
    }

    #endregion

    /// <summary>
    /// cTor: add to the List
    /// </summary>
    protected MapProviderBase( MapProvider provider )
    {
      MapProvider = provider;
      s_providerCat.Add( MapProvider, this );
    }

    /// <summary>
    /// Our internal Name
    /// </summary>
    public MapProvider MapProvider { get; private set; } = MapProvider.DummyProvider;

    /// <summary>
    /// Minimum level of zoom for this Map
    /// </summary>
    public int MinZoom { get; protected set; } = 1; // LIMITED  BY POLICY

    /// <summary>
    /// Maximum level of zoom for this Map
    /// </summary>
    public int MaxZoom { get; protected set; } = 15; // LIMITED BY POLICY not server (may go down to 17)

    /// <summary>
    /// Get; Set: Referer HTTP header.
    /// </summary>
    public string RefererUrl { get; protected set; } = "";

    /// <summary>
    /// Map Provider Copyright string
    /// </summary>
    public string Copyright { get; protected set; } = "";

    /// <summary>
    /// True if tile origin at BottomLeft, WMS-C
    /// </summary>
    public bool InvertedAxisY { get; protected set; } = false;

    /// <summary>
    /// Language to capture localized versions (if available)
    /// must be a 2 letter lang code (defaults to 'en')
    /// </summary>
    public string LanguageStr { get; private set; } = "en";

    /// <summary>
    /// return the ZoomValue within the Providers limits
    /// </summary>
    /// <param name="zoom">Desired zoom level</param>
    /// <returns>Zoom level clipped within limits</returns>
    protected ushort ZoomCheck( ushort zoom ) => (ushort)Math.Min( Math.Max( zoom, MinZoom ), MaxZoom );


    #region Abstract Templates

    /// <summary>
    /// Unique provider id
    /// </summary>
    public abstract Guid Id { get; }

    /// <summary>
    /// Provider name
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// MAIN Method
    /// Get a tile image using implemented provider
    /// 
    /// TODO get it private and let the Interface do the job only
    /// 
    ///  -- Note: This is a synchronous call to a webserver and will eventually return or timeout
    ///  
    /// </summary>
    /// <param name="mapImageID">The Map Image to retrive</param>
    /// <returns>A MapImage or null</returns>
    protected abstract MapImage GetTileImage( MapImageID mapImageID );

    #endregion

    #region Overrideable Properties / Methods

    // A distinct provider would need to change those...

    /// <summary>
    /// Generic URL constructor for basic URLs which require subsitituting of {x},{y},{z}, optional {s} and {l}
    /// </summary>
    /// <param name="tileXY">A TileXY of the requested tile {x},{y}</param>
    /// <param name="zoom">A zoomlevel {z}</param>
    /// <param name="server">A server string (if supported) {s}</param>
    /// <param name="language">A language designator (if supported) {l}</param>
    /// <returns>A completed URL derived from RefererUrl</returns>
    protected virtual string MakeTileImageUrl( TileXY tileXY, ushort zoom, string server, string language )
    {
      string url = RefererUrl;
      ushort z = ZoomCheck( zoom );
      url = url.Replace( "{x}", $"{tileXY.X}" );
      url = url.Replace( "{y}", $"{tileXY.Y}" );
      url = url.Replace( "{z}", $"{z}" );

      url = url.Replace( "{s}", $"{server}" );
      url = url.Replace( "{l}", $"{language}" );

      return url;
    }

    #endregion

    #region HTTP chores

    #region Throttling

    // A Semaphore to allow limited parallel HTTP usage
    private Semaphore _httpToken = new Semaphore( 2, 2 ); // 2: may use OSM policy


    #endregion

    #region Authorization

    string _authorization = string.Empty;

    /// <summary>
    ///     http://blog.kowalczyk.info/article/at3/Forcing-basic-http-authentication-for-HttpWebReq.html
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="userPassword"></param>
    public void ForceBasicHttpAuthentication( string userName, string userPassword )
    {
      _authorization = "Basic " + Convert.ToBase64String( Encoding.UTF8.GetBytes( userName + ":" + userPassword ) );
    }

    #endregion


    /// <summary>
    /// Basic sanity check of the Server reply
    /// 
    /// Hook for Providers to analyze the response if needed
    /// </summary>
    /// <param name="cHeaders">A WebResponse object</param>
    /// <returns>True if no abnormalities are found</returns>
    protected virtual bool CheckTileImageHttpResponse( HttpContentHeaders cHeaders )
    {
      //Debug.WriteLine(response.StatusCode + "/" + response.StatusDescription + "/" + response.ContentType + " -> " + response.ResponseUri);
      return cHeaders.ContentType.MediaType.Contains( responseContentType ) || cHeaders.ContentType.MediaType.Contains( responseContentTypeGeneric );
    }

    /// <summary>
    /// Issues a Http Client request with the given URL
    /// 
    ///  -- Note: this is synchronous and waits for the Server to reply or timeout
    ///  
    /// </summary>
    /// <param name="url">An URL</param>
    /// <param name="mapImageID">The image ID to add to the image for later identification</param>
    /// <returns>A MapImage or null</returns>
    protected MapImage GetTileImageUsingHttp( string url, MapImageID mapImageID )
    {
      if (!_initStatus) throw new InvalidOperationException( "Called before initialized! Aborted" );

      // sanity
      if (string.IsNullOrWhiteSpace( url )) return null; // cannot

      MapImage mapImage = null;
      bool retry = false;

      // Query the Server and convert the replied data (if possible)
      try {
        // ask for permission to use HTTP
        //Console.WriteLine($"WaitOne for {mapImageID}" );
        _httpToken.WaitOne();
        //Console.WriteLine( $"GotOne for {mapImageID}" );

        using (var response = _client.GetAsync( url ).GetAwaiter( ).GetResult( )) {
          // check the response for common status failures
          response.EnsureSuccessStatusCode( );
          if (CheckTileImageHttpResponse( response.Content.Headers )) {
            // so far OK - collect the data
            using (var responseStream = response.Content.ReadAsStreamAsync( ).GetAwaiter( ).GetResult( )) {
              // copy the data into a memory stream
              var imageStream = Tools.CopyStream( responseStream, false );
#if DEBUG
              //              Debug.WriteLine( $"MapProviderBase.GetTileImageUsingHttpV2.Response[{imageStream.Length}  bytes]: \n    URL='{url}'" );
#endif
              // so far OK...
              if (imageStream.Length > 0) {
                mapImage = MapImage.FromStream( imageStream, mapImageID );
                imageStream.Dispose( );
              }
            }
          }
        }

      }
      catch (Exception ex) {
        if ((ex is WebException) && (ex as WebException).Status == WebExceptionStatus.Timeout) {
          LOG.Log( "MapProviderBase.GetTileImageUsingHttp", "WebExceptionStatus Timeout" );
          retry = true; // can retry on timeout
        }
        else if ((ex is WebException) && (ex as WebException).Status == WebExceptionStatus.ProtocolError) {
          var status = ((HttpWebResponse)(ex as WebException).Response).StatusCode;
          LOG.Log( "MapProviderBase.GetTileImageUsingHttp", $"HttpWebResponse Status: {status} ({(int)status})" );
          if ((status == HttpStatusCode.GatewayTimeout) || (status == HttpStatusCode.RequestTimeout)) {
            retry = true;
          }
        }
        else if ((ex is WebException) && ((int)((HttpWebResponse)(ex as WebException).Response).StatusCode == 418)) {
          // seems OSM responds with the teapot code when blocking...
          var status = ((HttpWebResponse)(ex as WebException).Response).StatusCode;
          LOG.LogError( "MapProviderBase.GetTileImageUsingHttp", $"HttpWebResponse Status: {status} ({(int)status})" );
          retry = false; // never retry
        }
        else {
          LOG.LogError( "MapProviderBase.GetTileImageUsingHttp", $"Response Exception:\n{ex}\nURL:${url}" );
          retry = false; // never retry
        }
      }
      finally {
        // free Sema
        //Console.WriteLine( $"ReleaseOne for {mapImageID}" );
        _httpToken.Release();
      }

      // add a failed Image instead of nothing
      if (mapImage == null) {
        mapImage = MapImage.FailedImage( mapImageID, retry );
      }

      return mapImage;
    }

    /// <summary>
    /// Basic sanity check of the Server reply
    /// 
    /// Hook for Providers to analyze the response if needed
    /// </summary>
    /// <param name="response">A WebResponse object</param>
    /// <returns>True if no abnormalities are found</returns>
    protected virtual bool CheckTileImageHttpResponse( WebResponse response )
    {
      //Debug.WriteLine(response.StatusCode + "/" + response.StatusDescription + "/" + response.ContentType + " -> " + response.ResponseUri);
      return response.ContentType.Contains( responseContentType ) || response.ContentType.Contains( responseContentTypeGeneric );
    }


    /// <summary>
    /// Hook for Providers to modify the WebRequest if needed
    /// </summary>
    /// <param name="request">A WebRequest</param>
    protected virtual void InitializeWebRequest( WebRequest request ) { }

    #endregion

    #region GetTileImage Proxy (TODO see if we need this here at all)

    /// <summary>
    /// Read a MapImage from a DiskFile
    /// </summary>
    /// <param name="fileName">A filename</param>
    /// <param name="mapImageID">The image ID for later identification</param>
    /// <returns>A MapImage or null</returns>
    protected MapImage GetTileImageFromFile( string fileName, MapImageID mapImageID )
    {
      if (!_initStatus) throw new InvalidOperationException( "Called before initialized! Aborted" );

      return GetTileImageFromArray( File.ReadAllBytes( fileName ), mapImageID );
    }

    /// <summary>
    /// Capture a MapImage from an array of Bytes
    ///  Hook for Providers to modify image string and returning an Image
    /// </summary>
    /// <param name="data">An array of bytes</param>
    /// <param name="mapImageID">The image ID for later identification</param>
    /// <returns>A MapImage or null</returns>
    protected virtual MapImage GetTileImageFromArray( byte[] data, MapImageID mapImageID )
    {
      if (!_initStatus) throw new InvalidOperationException( "Called before initialized! Aborted" );

      return MapImage.FromArray( data, mapImageID );
    }

    #endregion

    #region Object Instance Methods (overrides)

    /// <summary>
    /// Returns the hashcode for this instance
    /// </summary>
    /// <returns>The hashcode for this instance</returns>
    public override int GetHashCode( )
    {
      return Id.GetHashCode( );
    }

    /// <summary>
    /// Returns a value that indicates whether two instances of Guid represent the same value.
    /// </summary>
    /// <param name="obj">The object to compare with this instance.</param>
    /// <returns>true if obj is a Guid that has the same value as this instance; otherwise, false.</returns>
    public override bool Equals( object obj )
    {
      if (obj is MapProviderBase) {
        return Id.Equals( (obj as MapProviderBase).Id );
      }

      return false;
    }

    /// <summary>
    /// Converts the value of this instance to a String.
    /// </summary>
    /// <returns>The current string.</returns>
    public override string ToString( )
    {
      return Name;
    }

    #endregion
  }
}

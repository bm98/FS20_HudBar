using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

using CoordLib;
using MapLib.Service;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// Base class for Data Providers
  /// </summary>
  internal abstract class MapProviderBase : IImgSource
  {

    #region STATIC Service Parts


    private static Random Random;
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
      Random = new Random( (int)DateTime.Now.Ticks );
      UserAgent = string.Format(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:{0}.0) Gecko/20100101 Firefox/{0}.0",
            Random.Next( (DateTime.Today.Year - 2012) * 10 - 10, (DateTime.Today.Year - 2012) * 10 ) );

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

    #region Abstract Templates

    /// <summary>
    /// A content ID string - where the same content shall share the same ID
    /// </summary>
    public abstract string ContentID { get; }

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
    public abstract MapImage GetTileImage( MapImageID mapImageID );

    #endregion

    #region Overrideable Properties

    /// <summary>
    /// Our internal Name
    /// </summary>
    public virtual MapProvider MapProvider { get; private set; } = MapProvider.DummyProvider;

    /// <summary>
    /// Minimum level of zoom for this Map
    /// </summary>
    public virtual int MinZoom { get; protected set; } = 0;// A distinct provider would need to change this...

    /// <summary>
    /// Maximum level of zoom for this Map
    /// </summary>
    public virtual int? MaxZoom { get; protected set; } = 17;// A distinct provider would need to change this...

    /// <summary>
    /// Get; Set: Referer HTTP header.
    /// </summary>
    public virtual string RefererUrl { get; protected set; } = string.Empty;// A distinct provider would need to change this...

    /// <summary>
    /// Map Provider Copyright string
    /// </summary>
    public virtual string Copyright { get; protected set; } = string.Empty;// A distinct provider would need to change this...

    /// <summary>
    /// True if tile origin at BottomLeft, WMS-C
    /// </summary>
    public virtual bool InvertedAxisY { get; protected set; } = false; // A distinct provider would need to change this...

    /// <summary>
    /// Language to capture localized versions (if available)
    /// must be a 2 letter lang code (defaults to 'en')
    /// </summary>
    public virtual string LanguageStr { get; private set; } = "en";

    #endregion

    #region HTTP chores

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
          Debug.WriteLine( $"MapProviderBase.GetTileImageUsingHttp: WebExceptionStatus Timeout" );
          retry = true; // can retry on timeout
        }
        else if ((ex is WebException) && (ex as WebException).Status == WebExceptionStatus.ProtocolError) {
          var status = ((HttpWebResponse)(ex as WebException).Response).StatusCode;
          Debug.WriteLine( $"MapProviderBase.GetTileImageUsingHttp: HttpWebResponse Status: {status} ({(int)status})" );
          if ((status == HttpStatusCode.GatewayTimeout) || (status == HttpStatusCode.RequestTimeout)) {
            retry = true; // can retry on timeout
          }
        }
        else {
          Debug.WriteLine( $"MapProviderBase.GetTileImageUsingHttp: Response Exception:\n{ex}" );
        }
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

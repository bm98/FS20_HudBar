using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BingMapsRESTToolkit;
using CoordLib.MercatorTiles;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// A Bing Helper Class to provide URL Formation
  /// </summary>
  internal static class BingManager
  {
    /*
     https://docs.microsoft.com/en-us/bingmaps/rest-services/directly-accessing-the-bing-maps-tiles

    In order to access the Bing Maps tiles in a supported way you will first need to get the current tile URLs from the Bing Maps Imagery REST service 
    every time your application starts. The main purpose of this is to ensure your application is using the latest tile URLs, which helps to prevent your 
    application from breaking, and also provides a way to track the usage of the service in such a way that aligns with the Bing Maps terms of use. 
    The Bing Maps REST Imagery Medata service can be used to get the current tile URLs. If you need to use this service from .NET, see the Using the 
    REST Services with .NET document. Making a request to the imagery service like this:

     http://dev.virtualearth.net/REST/V1/Imagery/Metadata/RoadOnDemand?output=json&include=ImageryProviders&key={BingMapsKey}

    This will return a response that contains an Image URL property. This URL will look something like this:

    http://ecn.{subdomain}.tiles.virtualearth.net/tiles/r{quadkey}.jpeg?g=129&mkt={culture}&shading=hill&stl=H

    You can then replace the different parts of the URL to request each tile as needed. The imageUrlSubdomains property in the imagery metadata response 
    provides a list of valid subdomain that can be used in the tile URL. Using a different one for each tile request you can increase performance by get 
    around browser URL request limits i.e. many browsers allow up to 8 concurrent requests to the same domain. Using subdomains allows up to 8 requests 
    per subdomain. The culture value can be any value listed in the Supported Culture Codes document. The quadkey value can be calculated based on the 
    zoom level and the tile you wish to render. Information on the tile system along with some useful code can be found here:

     */

    // Helper Class contains Image URL Metadata retrieved from the Bing Resource Server
    // Can format URLs 
    public class ImMetadata
    {
      // Tile URL placeholders
      private const string c_subDomain = "{subdomain}";
      private const string c_quadKey = "{quadkey}";
      private const string c_culture = "{culture}";

      private readonly string _currentCulture = "";

      // Fields
      public ImageryType ImageryType;
      public string ImageUrl = "";
      public string[] SubDomains;
      public int MinZoom = 0;
      public int MaxZoom = 0;
      public Size TileSize = new Size( );
      public ImageryProvider[] ImageProviders;

      // cTor
      public ImMetadata( )
      {
        _currentCulture = System.Globalization.CultureInfo.CurrentCulture.Name;
      }

      // Methods

      /// <summary>
      /// The Copyright of this Dataset 
      /// </summary>
      public string Copyright {
        get {
          var copyright = "";
          foreach (var prov in ImageProviders) {
            copyright += $"{prov.Attribution}, ";
          }
          return copyright;
        }
      }

      /// <summary>
      /// Returns a Tile URL for this MetaData, language=currentCulture if sent empty, default language if not provided en-us 
      /// </summary>
      /// <param name="tileXY">A TileXY</param>
      /// <param name="validZoom">A ZoomLevel, clipped to MinMax (please...)</param>
      /// <param name="language">An optional language ID</param>
      /// <returns>The TIle url</returns>
      public string GetTileUrl( TileXY tileXY, ushort validZoom, string language )
      {
        var url = ImageUrl.Replace( c_subDomain, SubDomains[Tools.GetServerNum( tileXY, SubDomains.Length )] );
        url = url.Replace( c_quadKey, tileXY.QuadKey( validZoom ).ToString( ) ); // TileXYToQuadKey( tileXY, (int)zoomLevel ) );
        url = url.Replace( c_culture, string.IsNullOrWhiteSpace( language ) ? _currentCulture : language ); // defaults to en-us if left empty
        // remove the Key part (some may have it but does not work - or would need a session key)
        var kPos = url.LastIndexOf( "&key=" );
        if (kPos > 0) {
          url = url.Substring( 0, kPos );
        }
        return url;
      }
    } // class ImMetadata


    // ************ STATIC CLASS IMPLEMENTATION

    // Key from Ini File
    private static string BingMapsKey = MapProviderBase.ProviderIni.BingKey;
    // dict of Tile URLs etc.
    private static ConcurrentDictionary<ImageryType, ImMetadata> _metaCache = new ConcurrentDictionary<ImageryType, ImMetadata>( );
    // a default one until we get the MetaData
    public const string DefaultCopyright = "(c) Microsoft Corporation et all";

    // Makes a REST request and awaits the result to be returned
    static private async Task<Resource[]> GetResourcesFromRequest( BaseRestRequest rest_request )
    {
      //var r = ServiceManager.GetResponseAsync( rest_request ).GetAwaiter( ).GetResult( );
      var r = await ServiceManager.GetResponseAsync( rest_request );

      if (!(r != null && r.ResourceSets != null &&
          r.ResourceSets.Length > 0 &&
          r.ResourceSets[0].Resources != null &&
          r.ResourceSets[0].Resources.Length > 0))

        throw new Exception( "No results found." );

      return r.ResourceSets[0].Resources;
    }

    // Loads MetaData async - will eventually get loaded
    private async static void LoadImMetaData( ImageryType imageryType )
    {
      if (!_metaCache.ContainsKey( imageryType ) && !string.IsNullOrWhiteSpace( BingMapsKey )) {
        // not yet there but we may ask for it..
        var imgMetaRequest = new ImageryMetadataRequest( ) {
          BingMapsKey = BingMapsKey,
          Culture = "en-us",
          GetBasicInfo = false,
          ImagerySet = imageryType,
          IncludeImageryProviders = true,
          UseHTTPS = true,
        };

        try {
          var resources = await GetResourcesFromRequest( imgMetaRequest );

          foreach (var res in resources) {
            if (res is ImageryMetadata) {
              var im = (res as ImageryMetadata);
              var imMeta = new ImMetadata {
                ImageryType = imageryType,
                ImageUrl = im.ImageUrl,
                SubDomains = im.ImageUrlSubdomains,
                ImageProviders = im.ImageryProviders,
                TileSize = new Size( im.ImageWidth, im.ImageHeight ),
                MinZoom = im.ZoomMin,
                MaxZoom = im.ZoomMax
              };
              _metaCache.TryAdd( imageryType, imMeta );
            }
          }

        }
        catch {
          ;  // DEBUG STOP
        }
      }
    }


    /// <summary>
    /// Try get the Imagery Metadata for an imageryType
    /// </summary>
    /// <param name="imageryType">The Imagery Type</param>
    /// <returns>The ImMetaData  or null</returns>
    public static ImMetadata GetImMetaData( ImageryType imageryType )
    {
      // trigger loading async
      if (!_metaCache.ContainsKey( imageryType ) && !string.IsNullOrWhiteSpace( BingMapsKey )) {
        LoadImMetaData( imageryType );
      }
      // see if we got one - fails if there is no Key provided  or no info arrived from Bing
      if (_metaCache.TryGetValue( imageryType, out ImMetadata imMetadata )) {
        return imMetadata;
      }
      return null;
    }

    /// <summary>
    /// Bing specific URL creator
    /// </summary>
    /// <param name="imageryType">Kind of map to retrieve</param>
    /// <param name="tileXY">A TileXY of the requested tile</param>
    /// <param name="validZoom">A ZoomLevel, clipped to MinMax (please...)</param>
    /// <param name="language">A language designator (defaults to en-us)</param>
    /// <returns>A completed URL derived from Bing retrieved access data</returns>
    public static string MakeBingTileImageUrl( ImageryType imageryType, TileXY tileXY, ushort validZoom, string language = "en-us" )
    {
      var imMeta = GetImMetaData( imageryType );
      if (imMeta == null) return ""; // cannot...

      string url = imMeta.GetTileUrl( tileXY, validZoom, language );
      return url;
    }

  }
}

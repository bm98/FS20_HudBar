using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MSiniLib;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// Handles the Provider Ini File
  /// 
  /// ;
  /// ;
  /// DefaultProvider= PROVIDER
  /// BingKey=KEY
  /// 
  /// [PROVIDER]
  /// Enabled=TRUE
  /// Http=URL
  /// 
  /// </summary>
  internal sealed class ProviderIni
  {
    // the IniFile
    private static string c_ProviderIniFile = "MapLibProvider.ini";

    /// <summary>
    /// INI Entry of an __enabled__ Provider
    /// </summary>
    private class ProviderIniEntry
    {
      public MapProvider Provider { get; private set; } = MapProvider.DummyProvider;
      public string UrlOverride { get; private set; } = "";
      public ProviderIniEntry( MapProvider provider, string url )
      {
        Provider = provider;
        UrlOverride = url; // wether or not it will apply is decided in the Provider Class
      }
    }

    // all enabled providers
    private Dictionary<MapProvider, ProviderIniEntry> _enabledProviders = new Dictionary<MapProvider, ProviderIniEntry>( );

    /// <summary>
    /// The Provider in use (defaults to OSM on any error or if the file is not there at all)
    /// </summary>
    public MapProvider DefaultProvider { get; private set; } = MapProvider.OSM_OpenStreetMap;

    /// <summary>
    /// Returns the collection of enabled providers
    /// </summary>
    public IEnumerable<MapProvider> EnabledProviders => _enabledProviders.Keys;

    /// <summary>
    /// The Bing Key (may apply or not)
    /// </summary>
    public string BingKey { get; private set; } = "";

    /// <summary>
    /// A Provider URL template override or empty if there is no such entry
    /// </summary>
    public string ProviderHttp( MapProvider provider )
    {
      if (_enabledProviders.TryGetValue( provider, out ProviderIniEntry item )) {
        return item.UrlOverride;
      }
      return "";
    }

    /// <summary>
    /// cTor:  Reads the Ini and has properties set when created
    /// </summary>
    public ProviderIni( string usedFolder )
    {
      MSiniFile iniFile = null;
      if (Directory.Exists( usedFolder )) {
        // see if the user folder has it 
        iniFile = new MSiniFile( Path.Combine( usedFolder, c_ProviderIniFile ) );
      }
      if (iniFile == null || iniFile?.SectionCatalog.Count <= 0) {
        // Try App Dirs default one
        iniFile = new MSiniFile( c_ProviderIniFile );
      }
      if (iniFile.SectionCatalog.Count <= 0) return;

      // scan provider sections
      foreach (var sect in iniFile.SectionCatalog.Sections) {
        var provider = Tools.MapProviderFromString( sect.Name );
        if (provider == MapProvider.DummyProvider) continue; // next one 

        bool enabled = iniFile.ItemValue( sect.Name, "Enabled" ).ToLowerInvariant( ) == "true";
        string urlOverride = iniFile.ItemValue( sect.Name, "http" );
        urlOverride = urlOverride.StartsWith( "http" ) ? urlOverride : ""; // some sanity.. must start with http...

        if (enabled) {
          var entry = new ProviderIniEntry( provider, urlOverride );
          _enabledProviders.Add( provider, entry );
        }
      }
      // get provider in use or default to OSM
      var providerInUse = iniFile.ItemValue( "", "DefaultProvider" );
      if (!string.IsNullOrWhiteSpace( providerInUse )) {
        DefaultProvider = Tools.MapProviderFromString( providerInUse );
        // must be enabled
        DefaultProvider = EnabledProviders.Contains( DefaultProvider ) ? DefaultProvider : MapProvider.OSM_OpenStreetMap;
        // catch unknown Providers
        DefaultProvider = (DefaultProvider == MapProvider.DummyProvider) ? MapProvider.OSM_OpenStreetMap : DefaultProvider;
      }
      else {
        DefaultProvider = MapProvider.OSM_OpenStreetMap;
      }
      // Try Bing Key
      var bingKey = iniFile.ItemValue( "", "BingKey" );
      BingKey = (bingKey.Length < 40) ? "" : bingKey; // bing keys are long..
    }


  }
}

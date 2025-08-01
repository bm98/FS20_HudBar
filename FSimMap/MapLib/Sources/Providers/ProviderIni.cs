﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using DbgLib;

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
  /// StadiaStamenKey=KEY
  /// 
  /// DiskCacheGB=128
  /// 
  /// [PROVIDER]
  /// Enabled=true / false
  /// Name=string
  /// Http=URL
  /// 
  /// </summary>
  internal sealed class ProviderIni
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    // the IniFile
    private static string c_ProviderIniFile = "MapLibProvider.ini";

    /// <summary>
    /// INI Entry of an __enabled__ Provider
    /// </summary>
    private class ProviderIniEntry
    {
      public MapProvider Provider { get; private set; } = MapProvider.DummyProvider;
      public string UrlOverride { get; private set; } = "";
      public string Name { get; private set; } = "";
      public ProviderIniEntry( MapProvider provider, string url, string name )
      {
        Provider = provider;
        UrlOverride = url; // wether or not it will apply is decided in the Provider Class
        Name = name;
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
    /// The Stadia Key (may apply or not)
    /// </summary>
    public string StadiaStamenKey { get; private set; } = "";

    /// <summary>
    /// The Disk Cache Size in MB (default 128)
    /// </summary>
    public int DiskCacheMB { get; private set; } = 128;

    /// <summary>
    /// A Provider URL template override or empty if there is no such entry
    /// </summary>
    public string ProviderName( MapProvider provider )
    {
      if (_enabledProviders.TryGetValue( provider, out ProviderIniEntry item )) {
        return item.Name;
      }
      return $"{provider}";
    }

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
      LOG.Info( "INIT", $"cTor: usedFolder:{usedFolder}" );

      MSiniFile iniFile = null;
      if (Directory.Exists( usedFolder )) {
        // see if the user folder has it 
        iniFile = new MSiniFile( Path.Combine( usedFolder, c_ProviderIniFile ) );
      }

      if (iniFile == null || iniFile?.SectionCatalog.Count <= 0) {
        // Try App Dirs default one
        iniFile = new MSiniFile( c_ProviderIniFile );
        LOG.Info( "INIT", $"cTor: fallback MapLib INI file:{c_ProviderIniFile}" );
      }
      else {
        LOG.Info( "INIT", $"cTor: MapLib INI file:{Path.Combine( usedFolder, c_ProviderIniFile )}" );
      }

      if (iniFile.SectionCatalog.Count <= 0) {
        LOG.Error( "INIT", $"cTor: MapLib INI file is not valid" );
        return;
      }


      // scan provider sections
      foreach (var sect in iniFile.SectionCatalog.Sections) {
        var provider = Tools.MapProviderFromString( sect.Name );
        if (provider == MapProvider.DummyProvider) continue; // next one 

        bool enabled = iniFile.ItemValue( sect.Name, "Enabled" ).ToLowerInvariant( ) == "true";
        string urlOverride = iniFile.ItemValue( sect.Name, "http" );
        urlOverride = urlOverride.StartsWith( "http" ) ? urlOverride : ""; // some sanity.. must start with http...
        string name = iniFile.ItemValue( sect.Name, "Name" );

        if (enabled) {
          var entry = new ProviderIniEntry( provider, urlOverride, name.Trim( ) );
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
      // Try Stadia Key
      var stadiaKey = iniFile.ItemValue( "", "StadiaStamenKey" );
      StadiaStamenKey = (stadiaKey.Length < 30) ? "" : stadiaKey; // seems 36 (GUID)

      // Try DiskCache
      var diskCache = iniFile.ItemValue( "", "DiskCacheMB" );
      if (!string.IsNullOrEmpty( diskCache ) && int.TryParse( diskCache, out var cacheMB )) {
        DiskCacheMB = dNetBm98.XMath.Clip( cacheMB, 32, 1024 ); // valid limits
      }
    }


  }
}

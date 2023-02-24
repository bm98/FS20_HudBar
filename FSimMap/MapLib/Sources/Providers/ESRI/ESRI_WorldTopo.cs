﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoordLib.MercatorTiles;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// ESRI/ArcGIS WorldTopo Map
  /// 
  /// https://services.arcgisonline.com/arcgis/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}
  /// 
  /// V2 (not used so far)
  /// https://basemaps.arcgis.com/arcgis/rest/services/World_Basemap_v2/VectorTileServer
  /// Sources: Esri, HERE, Garmin, FAO, NOAA, USGS, © OpenStreetMap contributors, and the GIS User Community
  /// </summary>
  internal sealed class ESRI_WorldTopo : MapProviderBase
  {
    // Singleton Pattern
    public static ESRI_WorldTopo Instance => lazy.Value;
    private static readonly Lazy<ESRI_WorldTopo> lazy = new Lazy<ESRI_WorldTopo>( ( ) => new ESRI_WorldTopo( ) );
    private ESRI_WorldTopo( )
      : base( MapProvider.ESRI_WorldTopo )
    {
      RefererUrl = DefaultUrl;
      Copyright = string.Format( "Map tiles by ESRI/ArcGIS." );
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
      Name = ProviderIni.ProviderName( MapProvider );
    }

    // default URL
    private const string DefaultUrl = "https://services.arcgisonline.com/arcgis/rest/services/World_Topo_Map/MapServer/tile/{z}/{y}/{x}";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-E6CC6F319F35" );

    public override string Name { get; } = "Sources: Esri, HERE, Garmin, Intermap, INCREMENT P, GEBCO, USGS, FAO, NPS, NRCAN, GeoBase,"
                                          + "IGN, Kadaster NL, Ordnance Survey, Esri Japan, METI, Esri China (Hong Kong), © OpenStreetMap contributors, GIS User Community";

    protected override MapImage GetTileImage( MapImageID mapImageID )
    {
      if (!this.ProviderEnabled) return null;

      string url = MakeTileImageUrl( mapImageID.TileXY, mapImageID.ZoomLevel, "", "" );
      return base.GetTileImageUsingHttp( url, mapImageID );
    }

    #endregion


  }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using bm98_Map.Data;

using CoordLib;

using FSimFacilityIF;

using static bm98_Map.Drawing.FontsAndColors;
using static bm98_Map.MRH_extensions;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// Manages the DisplayLists of the airport
  /// and maintains the items and supports aircraft prperty changes
  /// </summary>
  internal class DisplayListMgr
  {
    // maximum aircraft track length
    private const int c_maxTrackPoints = 600; // see what works here

    // ref of the managed airport
    private Data.Airport _airportRef;
    // ref of the ViewPort used to show this Drawings
    private VPort2 _viewportRef;

    // wether or not showing the aircraft 
    private bool _showTrackedAcft = false;
    // wether or not showing the grid
    private bool _showMapGrid = false;
    // wether or not showing the airport Range
    private bool _showAptRange = false;
    // wether or not showing the Navaids
    private bool _showNavaids = false;
    // wether or not showing the Route
    private bool _showRoute = false;
    // wether or not showing the VFR Markings
    private bool _showVfrMarks = false;
    // wether or not showing the Airport (alternates) Marks
    private bool _showAptMarks = false;
    // selected runwayIdent 
    private string _selRunway = "";
    // selected runwayApproachIdent 
    private string _selRunwayApproach = "";

    // DisplayManager internal range handler
    private readonly MapRangeHandler _mapRangeHandler;

    /// <summary>
    /// cTor: Creates the AirportDIsplayManager (does not manage or dispose submitted items)
    /// </summary>
    /// <param name="viewport">An initialized viewport</param>
    public DisplayListMgr( VPort2 viewport )
    {
      _airportRef = null;
      _viewportRef = viewport;
      _selRunway = "";
      _mapRangeHandler = new MapRangeHandler( MapRange.Mid, SetZoomLevel );
    }

    /// <summary>
    /// Clear all Airport DispItems from the List (make it empty)
    /// </summary>
    public void ClearDispItems( )
    {
      _selRunway = "";
      _viewportRef.GProc.Drawings.Clear( ); // release all from airport and related map
    }

    /// <summary>
    /// All named Display items
    /// </summary>
    private enum DItems
    {
      FIRST_NOTUSED = PanelConst.AptBaseEnum,
      // map canvas
      APT_Canvas,
      // map grid and scale
      MAP_GRID,
      MAP_SCALE,
      // apt circles
      APT_Range,
      // navaid hook
      NAVAIDS,
      // waypoint hook
      WAYPOINTS,
      // VFR items
      VFRMARKS,
      // IFR item hook - Approaches
      IFRMARKS,
      // APT alternate items1
      APTMARKS,
      // Runways
      RUNWAYS,
      // tracking aircraft
      AIRCRAFT,
      AIRCRAFT_RANGE,
      AIRCRAFT_TRACK,
      AIRCRAFT_WIND,
      // route
      ROUTE,

    }

    /// <summary>
    /// Setup the Airport and Runway Drawing items
    /// </summary>
    public void AddDispItems( Data.Airport airport )
    {
      _airportRef = airport;

      // Items are drawn in this order i.e. first defined will be draw first 
      //  and overwritten by the later defined ones

      // Map (background)
      var canvas = new CanvasItem( ) {
        Key = (int)DItems.APT_Canvas,
        Active = true, // always
        CoordPoint = _airportRef.Coordinate,
        MapTilesRef = _viewportRef.Map,
        Font = FtLarger, // serves only debug purposes
#if DEBUG
        TileBorders = false, // @@@@@@@  DEBUG Borders
#endif
      };
      _viewportRef.GProc.Drawings.AddItem( canvas );

      // Lat Lon Grid
      var grid = new MapGridItem( ) {
        Key = (int)DItems.MAP_GRID,
        Active = _showMapGrid,
        TileMatrixRef = _viewportRef.Map,
        Pen = PenScale1,
        TextBrush = BrushScale,
        Font = FtMid,
      };
      _viewportRef.GProc.Drawings.AddItem( grid );

      // Map Scale
      var scale = new ScaleItem( ) {
        Key = (int)DItems.MAP_SCALE,
        Active = true, // allways
        Pen = PenScale3,
        Font = FtLarger,
        TextBrush = BrushScale,
      };
      _viewportRef.GProc.Drawings.AddItem( scale );

      // Airport Range
      var aptRange = new AirportRangeItem( ) {
        Key = (int)DItems.APT_Range,
        Active = _showAptRange,
        CoordPoint = _airportRef.Coordinate,
        Pen = PenAptRange,
        TextBrush = BrushAptRange,
        Font = FtSmall,
      };
      _viewportRef.GProc.Drawings.AddItem( aptRange );

      // APT Alternates Marks Hook (ManagedDrawings but not using managed items)
      // i.e. when the Hook Active property is set the whole Display Sublist will be handled the same
      var aptmarks = new ManagedHookItem( ) {
        Key = (int)DItems.APTMARKS,
        String = "Airport Marks", // only deco for debug
      };
      _viewportRef.GProc.Drawings.AddItem( aptmarks );

      // VFR Marks Hook (will get one per phys. runway)
      var vfrmarks = new ManagedHookItem( ) {
        Key = (int)DItems.VFRMARKS,
        String = "VFR Marks", // only deco for debug
      };
      _viewportRef.GProc.Drawings.AddItem( vfrmarks );

      // Navaid Hook (ManagedDrawings but not using managed items)
      var navaids = new ManagedHookItem( ) {
        Key = (int)DItems.NAVAIDS,
        String = "Navaids", // only deco for debug
      };
      _viewportRef.GProc.Drawings.AddItem( navaids );

      // Route Hook (ManagedDrawings but not using managed items)
      var route = new RouteHookItem( ) {
        Key = (int)DItems.ROUTE,
        Active = _showRoute, // visible at any Range
        String = "Route", // only deco for debug
      };
      _viewportRef.GProc.Drawings.AddItem( route );

      // Waypoint Hook (Wyps need to be managed individually)
      // The Hook is used to show/hide all Wyps (e.g. due to Range selection)
      var waypoints = new ManagedHookItem( ) {
        Key = (int)DItems.WAYPOINTS,
        String = "Waypoints", // only deco for debug
      };
      _viewportRef.GProc.Drawings.AddItem( waypoints );

      // Waypoint Hook (Wyps need to be managed individually)
      // The Hook is used to show/hide all Wyps (e.g. due to Range selection)
      var ifrmarks = new ManagedHookItem( ) {
        Key = (int)DItems.IFRMARKS,
        String = "IFR Marks", // only deco for debug
      };
      _viewportRef.GProc.Drawings.AddItem( ifrmarks );

      // Runways, always drawn (will get one per phys. runway)
      var runways = new HookItem( ) {
        Key = (int)DItems.RUNWAYS,
        String = "Runways", // only deco for debug
      };
      _viewportRef.GProc.Drawings.AddItem( runways );
      // populate the hook right now - it is static per airport
      AddRunwayDispItems( _airportRef.Runways );

      // Tracked aircraft on the Sprite Processor
      // will be added only if it does not exist (startup)
      //  else the tracked aircraft is independent from the Airport ones
      var di = _viewportRef.GProcSprite.Drawings.DispItem( (int)DItems.AIRCRAFT );
      // Aircraft Sprite
      if (di == null) {
        // Track Hook -- added first to be drawn before the icon
        var aircraftTrack = new TrackHookItem<AcftTrackItem>( c_maxTrackPoints ) {
          Key = (int)DItems.AIRCRAFT_TRACK,
          Active = false, // init false, will be shown on update if asked for
        };
        _viewportRef.GProcSprite.Drawings.AddItem( aircraftTrack );
        //aircraft.SubItemList.AddItem( aircraftTrack );

        var aircraft = new AircraftItem( Properties.Resources.aircraft_mid ) {
          Key = (int)DItems.AIRCRAFT,
          Active = _showTrackedAcft,
          CoordPoint = _airportRef.Coordinate,
          Font = FtLarger,
          TextBrush = BrushRwNumber,
          // drawn acft
          Pen = PenAcftOutline, // outline of the shape
        };
        _viewportRef.GProcSprite.Drawings.AddItem( aircraft );


        // Aircraft range arcs (sub item of Aircraft)
        var aircraftRange = new AcftRangeItem( ) {
          Key = (int)DItems.AIRCRAFT_RANGE,
          Active = false, // init false, will be shown on update if asked for
          Pen = PenRange3,
          PenTrack = PenRange5,
          TgtPen = PenTRange3,
          CoordPoint = _airportRef.Coordinate,
        };
        aircraft.SubItemList.AddItem( aircraftRange );

        // Aircraft wind arrow (sub item of Aircraft)
        var aircraftWind = new AcftWindItem( ) {
          Key = (int)DItems.AIRCRAFT_WIND,
          Active = false, // init false, will be shown on update if asked for
          Pen = PenAcftWind,
          FillBrush = BrushAcftWind,
          Font = FtSmallest,
          TextBrush = BrushAcftWind,
          CoordPoint = _airportRef.Coordinate,
        };
        aircraft.SubItemList.AddItem( aircraftWind );
      }

      ClutterManager( );
    }

    #region Runways

    // Add the Runway Drawing Items to the Hook RUNWAYS
    // Add one Item per physical runway (Start + End)
    private void AddRunwayDispItems( IEnumerable<IRunway> runways )
    {
      // all Runway items
      var rwyList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.RUNWAYS ).SubItemList;
      rwyList.Clear( );

      List<string> runwaysDone = new List<string>( ); // track the ones we have processed
      // create the needed Runway Text lines
      for (int i = 0; i < runways.Count( ); i++) {
        var rwy = runways.ElementAt( i );
        // get the the the Runway Pairs if not done
        var isWaterRwy = rwy.Surface == "WATER";
        if (!runwaysDone.Contains( rwy.Ident )) {
          var otherEnd = runways.First( x => x.Ident == rwy.OtherIdent );
          if (otherEnd != null) {
            var rwBox = new RunwayItem( ) {
              Key = GProc.DispID_Anon( ),
              Active = true, // always
              StartID = rwy.Ident,
              Start = rwy.StartCoordinate,
              EndID = otherEnd.Ident,
              End = otherEnd.StartCoordinate,
              Lenght = rwy.Length_m,
              Width = rwy.Width_m,
              IsWater = isWaterRwy,
              // Runway Fill
              FillBrushAlt = isWaterRwy ? BrushRwBorderWater : BrushRwBorder,
              FillBrush = isWaterRwy ? BrushRwPavementWater : BrushRwPavement,
              Pen = PenInfo,
              // Text
              TextBrush = BrushRwNumber,
              TextRectFillBrush = null,
              TextRectPen = PenInfo,
              Font = isWaterRwy ? FtSmall : FtMid,
            };
            runwaysDone.Add( rwy.Ident ); // track done runways
            runwaysDone.Add( otherEnd.Ident );// track done runways
            // add to displaylist
            rwyList.AddItem( rwBox );
          }
        }
      }
    }

    #endregion


    // defines visibilty of items at the current Range
    private void ClutterManager( )
    {
      // right now we set the font size for the Runway Labels here
      DisplayItem di;
      // all Runway items
      var rwyList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.RUNWAYS )?.SubItemList;
      if (rwyList != null) {
        foreach (var rwyItem in rwyList.Values) {
          di = rwyItem as Drawing.RunwayItem;
          di.Font = _mapRangeHandler.IsXFar || _mapRangeHandler.IsFarFar ? null // cannot draw Text at this resolution
            : _mapRangeHandler.IsFar || _mapRangeHandler.IsMid ? FtSmaller
            : FtMid;
        }
      }

      // disable clutter at XFar
      bool active = !_mapRangeHandler.IsXFar;
      var itemList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.NAVAIDS );
      if (itemList != null) itemList.Active = active && _showNavaids;

      itemList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.WAYPOINTS );
      if (itemList != null) itemList.Active = active && _showNavaids;

      itemList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.APTMARKS );
      if (itemList != null) itemList.Active = active && _showAptMarks;

      itemList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.VFRMARKS );
      if (itemList != null) itemList.Active = active; // the hook is only used to show no marks at XFar

      itemList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.IFRMARKS );
      if (itemList != null) itemList.Active = active; // the hook is only used to show no marks at XFar
    }

    /// <summary>
    /// Update the ZoomLevel related drawings
    /// </summary>
    /// <param name="zoomLevel">The ZoomLevel</param>
    private void SetZoomLevel( ushort zoomLevel )
    {
      ClutterManager( );
      // right now we set the font size for the Runway Labels here
      DisplayItem di;

      // Scale Item
      float pixelPerNm = 1;
      // avoid div/0 errors
      if (_viewportRef.Map.HorPixelMeasure_m != 0) {
        pixelPerNm = 1852f / _viewportRef.Map.HorPixelMeasure_m; // px/m-> px/nm conversion
      }
      di = _viewportRef.GProc.Drawings.DispItem( (int)DItems.MAP_SCALE );
      if (di != null) {
        (di as ScaleItem).PixelPerNm = pixelPerNm;
      }

    }

    #region API Show Decorations

    /// <summary>
    /// True to show the tracked aircraft, false otherwise
    /// </summary>
    public bool ShowTrackedAircraft {
      get => _showTrackedAcft;
      set {
        _showTrackedAcft = value;
        var di = _viewportRef.GProcSprite.Drawings.DispItem( (int)DItems.AIRCRAFT );
        if (di != null) di.Active = _showTrackedAcft;

        di = _viewportRef.GProcSprite.Drawings.DispItem( (int)DItems.AIRCRAFT_RANGE );
        if (di != null) di.Active = false; // will be updated when the Aircraft is update, else it's shown for a blink even if not selected

        di = _viewportRef.GProcSprite.Drawings.DispItem( (int)DItems.AIRCRAFT_WIND );
        if (di != null) di.Active = false; // will be updated when the Aircraft is update, else it's shown for a blink even if not selected

        di = _viewportRef.GProcSprite.Drawings.DispItem( (int)DItems.AIRCRAFT_TRACK );
        if (di != null) di.Active = false; // will be updated when the Aircraft is update, else it's shown for a blink even if not selected

        RenderSprite( );
        Redraw( );
      }
    }

    /// <summary>
    /// True to show the route, false otherwise
    /// </summary>
    public bool ShowRoute {
      get => _showRoute;
      set {
        _showRoute = value;
        var di = _viewportRef.GProc.Drawings.DispItem( (int)DItems.ROUTE );
        if (di == null) return;  // not (yet) defined
        di.Active = _showRoute;

        RenderStatic( );
        Redraw( );
      }
    }

    /// <summary>
    /// True to show the map grid, false otherwise
    /// </summary>
    public bool ShowMapGrid {
      get => _showMapGrid;
      set {
        _showMapGrid = value;
        var di = _viewportRef.GProc.Drawings.DispItem( (int)DItems.MAP_GRID );
        if (di == null) return; // not (yet) defined
        di.Active = _showMapGrid;

        RenderStatic( );
        Redraw( );
      }
    }

    /// <summary>
    /// True to show the airport range circles, false otherwise
    /// </summary>
    public bool ShowAiportRange {
      get => _showAptRange;
      set {
        _showAptRange = value;
        var di = _viewportRef.GProc.Drawings.DispItem( (int)DItems.APT_Range );
        if (di == null) return; // not (yet) defined
        di.Active = _showAptRange;

        RenderStatic( );
        Redraw( );
      }
    }

    /// <summary>
    /// True to show the airport range circles, false otherwise
    /// </summary>
    public bool ShowNavaids {
      get => _showNavaids;
      set {
        _showNavaids = value;
        // all Navaid items
        var navHook = _viewportRef.GProc.Drawings.DispItem( (int)DItems.NAVAIDS );
        if (navHook != null) navHook.Active = _mapRangeHandler.IsXFar ? false : _showNavaids; // avoid clutter at XFar

        // show wyp for the selected runway only
        var wypHook = _viewportRef.GProc.Drawings.DispItem( (int)DItems.WAYPOINTS );
        if (wypHook != null) wypHook.Active = _mapRangeHandler.IsXFar ? false : _showNavaids; // avoid clutter at XFar
        // per Wyp now
        RenderStatic( );
        Redraw( );
      }
    }

    /// <summary>
    /// True to show the VFR marks, false otherwise
    /// </summary>
    public bool ShowVFRMarks {
      get => _showVfrMarks;
      set {
        _showVfrMarks = value;
        // all VFR marks items
        var vfrList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.VFRMARKS )?.SubItemList;
        if (vfrList == null) return; // not (yet) defined
        foreach (var vfrItem in vfrList.Values) {
          var di = vfrItem as RwyVFRMarksItem;
          di.Active = true; // always true
          di.ShowFullDecoration = _showVfrMarks; // to draw the full painting
        }
        RenderStatic( );
        Redraw( );
      }
    }

    /// <summary>
    /// True to show the Airport marks
    /// </summary>
    public bool ShowAptMarks {
      get => _showAptMarks;
      set {
        _showAptMarks = value;
        // all VFR marks items
        var di = _viewportRef.GProc.Drawings.DispItem( (int)DItems.APTMARKS );
        if (di == null) return; // not (yet) defined
        di.Active = _mapRangeHandler.IsXFar ? false : _showAptMarks; // avoid clutter at XFar

        RenderStatic( );
        Redraw( );
      }
    }

    #endregion

    #region API VFR

    /// <summary>
    /// Set the Runway VFR Drawing Items for a corresponding pair of runway entries
    /// The first is considered as the Main one which shows the straight In approach angle
    /// and will have the Left Turn pattern in the prominent color
    /// If only one runway is provided the 'End' will have no drawings
    /// </summary>
    /// <param name="runways">A pair of runways which represent the same physical runway, null to clear all</param>
    public void SetRunwayVFRDispItems( IEnumerable<IRunway> runways )
    {
      // all VFR marks items
      var vfrList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.VFRMARKS )?.SubItemList;
      if (vfrList == null) return; // CANNOT (yet)
      vfrList.Clear( );
      if (runways == null) { return; } // unselected

      List<string> runwaysDone = new List<string>( ); // track the ones we have processed
      // create the needed Runway Text lines
      for (int i = 0; i < runways.Count( ); i++) {
        var rwy = runways.ElementAt( i );
        // get the the Runway Pairs if not done
        if (!runwaysDone.Contains( rwy.Ident )) {
          var otherEnd = runways.First( x => x.Ident == rwy.OtherIdent );
          var rwBox = new RwyVFRMarksItem( ) {
            Key = GProc.DispID_Anon( ),
            Active = true, // always, manged by the Hook for XFAR, else it's not even in the DisplayList
            ShowFullDecoration = _showVfrMarks, // to draw the full painting when VFR is selected
            StartID = rwy.Ident,
            Start = rwy.StartCoordinate,
            StartHeading_degm = rwy.Bearing_degm,
            StartRunwayIdent = rwy.Ident,
            EndID = (otherEnd == null) ? "" : otherEnd.Ident,
            End = (otherEnd == null) ? LatLon.Empty : otherEnd.StartCoordinate,
            EndHeading_degm = (otherEnd == null) ? float.NaN : otherEnd.Bearing_degm,
            EndRunwayIdent = (otherEnd == null) ? "" : otherEnd.Ident,
            Lenght = rwy.Length_m,
            Width = rwy.Width_m,
            // graphics
            Pen = PenAptRange,
            NoDecoPen = PenVfrNoDeco,
            VfrPenMain = PenVfrMain,
            VfrPenAlt = PenVfrAlt,
            FillBrush = BrushAptRange,
            // Heading Text
            TextBrush = BrushVFRHeading,
            Font = FtLarger, // Heading
            RangeFont = FtSmall, // Ring range
          };
          vfrList.AddItem( rwBox );
          runwaysDone.Add( rwy.Ident );
          if (otherEnd != null)
            runwaysDone.Add( otherEnd.Ident );
        }
      }
    }

    #endregion

    #region API MapRange

    /// <summary>
    /// Update the ZoomLevel related drawings
    /// </summary>
    /// <param name="zoomLevel">The ZoomLevel</param>
    public void SetMapZoom( ushort zoomLevel )
    {
      _mapRangeHandler.SetZoomLevel( zoomLevel );
    }

    #endregion

    #region API Aircraft

    /// <summary>
    /// Update the tracked aircraft
    /// </summary>
    /// <param name="aircraft">The internal Aircraft</param>
    public void UpdateAircraft( Data.TrackedAircraft aircraft )
    {
      // set aircraft icon on the sprite level
      var diAcft = _viewportRef.GProcSprite.Drawings.DispItem( (int)DItems.AIRCRAFT );
      if (diAcft == null) return; // CANNOT (yet)
                                  // set icon
      var acftItem = (diAcft as AircraftItem);
      acftItem.Active = _showTrackedAcft;
      acftItem.CoordPoint = aircraft.Position;
      acftItem.Heading = aircraft.TrueHeading_deg;
      acftItem.OnGround = aircraft.OnGround;

      // set range
      var di = diAcft.SubItemList.DispItem( (int)DItems.AIRCRAFT_RANGE );
      if (di == null) return; // CANNOT (yet)
      var acftRange = (di as AcftRangeItem);
      // update display of range from aircraft record
      acftRange.Active = _showTrackedAcft && aircraft.ShowAircraftRange;
      acftRange.AircraftTrackRef = aircraft;
      acftRange.CoordPoint = aircraft.Position; // default property is also set (may not be used)

      // set wind
      di = diAcft.SubItemList.DispItem( (int)DItems.AIRCRAFT_WIND );
      if (di == null) return; // CANNOT (yet)
      var acftWind = (di as AcftWindItem);
      // update display of range from aircraft record
      acftWind.Active = _showTrackedAcft && aircraft.ShowAircraftWind;
      acftWind.WindDir_deg = aircraft.WindDirection_deg;
      acftWind.String = aircraft.WindSpeedS;
      acftWind.CoordPoint = aircraft.Position; // default property is also set (may not be used)

      // set track
      di = _viewportRef.GProcSprite.Drawings.DispItem( (int)DItems.AIRCRAFT_TRACK );
      if (di == null) return; // CANNOT (yet)
      var trackHook = (di as TrackHookItem<AcftTrackItem>);
      if (aircraft.ClearAircraftTrack) {
        // must clear the track
        trackHook.Active = false; // nothing to paint anyway
        trackHook.ManagedClear( );
      }
      else {
        // update display of track from aircraft record
        trackHook.Active = _showTrackedAcft && aircraft.ShowAircraftTrack;
        // add a new tracking point from the arrived data
        var trackPoint = new AcftTrackItem( ) {
          Key = GProc.DispID_Anon( ),
          Active = true, // always true, the managed hook takes care of this
          CoordPoint = aircraft.Position,
          OnGround = aircraft.OnGround,
        };
        // add managed to the Hook, takes care of the length
        trackHook.ManagedAddItem( trackPoint );
      }

    }

    #endregion

    #region API Navaids and Alternate Apts

    private Bitmap NavaidImage( NavaidTyp navaidType )
    {
      switch (navaidType) {
        case NavaidTyp.VOR: return Properties.Resources.vor;
        case NavaidTyp.VOR_DME: return Properties.Resources.vor_dme;
        case NavaidTyp.DME: return Properties.Resources.dme;
        case NavaidTyp.NDB: return Properties.Resources.ndb;
        case NavaidTyp.NDB_HI: return Properties.Resources.ndb;
        case NavaidTyp.NDB_LO: return Properties.Resources.ndb;
        case NavaidTyp.TACVOR: return Properties.Resources.vortac;
        case NavaidTyp.TACAN: return Properties.Resources.vortac;
        case NavaidTyp.ILS_LOC: return null; // Properties.Resources.loc;
        case NavaidTyp.ILS_LOC_DME: return null; //  Properties.Resources.loc;
        case NavaidTyp.ILS_LOC_GS: return null; //  Properties.Resources.loc_gs;
        case NavaidTyp.ILS_LOC_GS_DME: return null; //  Properties.Resources.loc_gs;
        default: return Properties.Resources.vor;
      }
    }
    private Bitmap NavaidImage( UsageTyp usageType )
    {
      switch (usageType) {
        case UsageTyp.AWY: return Properties.Resources.wyp;
        case UsageTyp.APR: return Properties.Resources.wyp_apt;  // APP,STAR
        case UsageTyp.SID: return Properties.Resources.wyp_apt;  // APP,STAR
        case UsageTyp.STAR: return Properties.Resources.wyp_apt;  // APP,STAR
        case UsageTyp.HOLD: return Properties.Resources.wyp_mapr;  // MAPR
        case UsageTyp.MAPR: return Properties.Resources.wyp_mapr;  // MAPR
        default: return null;
      }
    }

    /// <summary>
    /// Set the List of Navaids for Rendering
    /// </summary>
    /// <param name="navaids">List of Navaids to show</param>
    /// <param name="fixes">List of Fixes to show</param>
    public void SetNavaidList( IList<INavaid> navaids, IList<IFix> fixes )
    {
      // clear all Navaid items
      var navList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.NAVAIDS )?.SubItemList;
      if (navList == null) return; // CANNOT (yet)
      navList.Clear( );
      // clear all WaypointCat
      var wypList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.WAYPOINTS )?.SubItemList;
      if (wypList == null) return; // CANNOT (yet)
      wypList.Clear( );
      // clear all IFR marks
      var ifrList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.IFRMARKS )?.SubItemList;
      if (ifrList == null) return; // CANNOT (yet)
      ifrList.Clear( );

      // create all NAVAIDS from the list
      foreach (var na in navaids) {
        if (na.NavaidType == NavaidTyp.Unknown) continue; // skip

        var nImage = NavaidImage( na.NavaidType );
        // calc the outbound leg CompasPoint to place the Wyp label
        var cp = "N"; // default
        var navaid = new NavaidItem( nImage ) {
          Key = GProc.DispID_Anon( ),
          Active = true, // wyps are off, others on (ManagedHook takes care)
          Pen = PenRange3,
          CoordPoint = na.Coordinate,
          OutboundLatLon = LatLon.Empty, // set the next point TODO Set from argument
          ShowOutboundTrack = false, // enable through Apr selection
          String = na.IsVOR ? $"{na.Ident}\n{na.Frequ_Hz / 1_000_000f:##0.00}"  // ICAO / mmm.nn
                 : na.IsNDB ? $"{na.Ident}\n{na.Frequ_Hz / 1_000f:###0.0}"      // ICAO / kkkk.n
                 : "",
          Font = FtLarge,
          TextBrush = BrushNavAid,
          IsNdbType = na.IsNDB,
          IsWypType = false,
          IsHoldType = false,
          RunwayIdent = "",
          RunwayApproachIdent = "",
          CompassPoint = cp,
          WypLabelRectangle = new Rectangle( ), // used later by the labelling engine
        };
        navaid.StringFormat.LineAlignment = na.IsNDB ? StringAlignment.Far : StringAlignment.Near;
        navList.AddItem( navaid );
      }

      // APPROACH FIXes
      foreach (var fix in fixes) {
        if (fix.WaypointUsage == UsageTyp.Unknown) continue; // skip

        var nImage = NavaidImage( fix.WaypointUsage );
        // for waypoints we show the IAF and FAF different, for Runway (MAPR start) nothing
        if (fix.WYP.IsRunway) { nImage = null; }
        else if (fix.IsHold) { nImage = Properties.Resources.hold; }
        else if (fix.FixInfo.StartsWith( "IF" )) { nImage = Properties.Resources.wyp_apt; }
        else if (fix.FixInfo.StartsWith( "FAF" )) { nImage = Properties.Resources.wyp_faf; }

        // calc the outbound leg CompasPoint to place the Wyp label
        var cp = "N"; // default
        if (fix.IsAnyApproach) {
          var brg = fix.WYP.Coordinate.BearingTo( fix.OutboundCoordinate );
          cp = Dms.CompassPoint( brg, 1 );
        }
        var navaid = new NavaidItem( nImage ) {
          Key = GProc.DispID_Anon( ),
          Active = false, // wyps are off, others on (ManagedHook takes care)
          Pen = fix.IsMissedApproach ? PenRouteMApr
                : fix.IsApproach ? PenRouteApr
                : PenRange3,
          CoordPoint = fix.WYP.Coordinate,
          OutboundLatLon = fix.IsAnyApproach ? fix.OutboundCoordinate : LatLon.Empty, // set the next point TODO Set from argument
          ShowOutboundTrack = false, // enable through Apr selection
          String = fix.IsHold ? $"Hold\n{fix.AltitudeLo_ft:##,##0}"   // Hold / nnnn
                    : fix.AltitudeLo_ft < 10 ? $"{fix.IdentOf}"       // ICAO
                    : $"{fix.IdentOf}\n{fix.AltitudeLo_ft:##,##0}",   // ICAO / nnnn   ft
          Font = FtMid,
          TextBrush = BrushNavAidWyp,
          IsNdbType = false,
          IsWypType = true,
          IsHoldType = fix.IsHold,
          RunwayIdent = fix.IsAnyApproach ? fix.RwyIdent : "",
          RunwayApproachIdent = fix.IsAnyApproach ? fix.ProcRef : "",
          CompassPoint = cp,
          WypLabelRectangle = new Rectangle( ), // used later by the labelling engine
        };
        navaid.StringFormat.LineAlignment = StringAlignment.Near;
        if (fix.IsAnyApproach) {
          ifrList.AddItem( navaid );
        }
        else {
          wypList.AddItem( navaid );
        }
      }
    }

    /// <summary>
    /// Layout the IFR Mark Labels
    /// </summary>
    private void LayouIfrMarkLabels( )
    {
      // all Navaid items
      var ifrList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.IFRMARKS )?.SubItemList;
      if (ifrList == null) return; // CANNOT (yet)

      // waypoints
      foreach (var navItem in ifrList.Values) {
        var di = navItem as NavaidItem;
        var aprShow = !string.IsNullOrEmpty( _selRunwayApproach ) && (di.RunwayApproachIdent == _selRunwayApproach); // show none if not selected
        di.Active = !_mapRangeHandler.IsXFar && (di.RunwayIdent == _selRunway) && aprShow; // show Wyps independent of the Navaids Button
        di.ShowOutboundTrack = (string.IsNullOrEmpty( _selRunwayApproach ) == false); // track only if an Apr is selected
      }
    }

    /// <summary>
    /// Set the WaypointCat for the selected Runway
    /// </summary>
    /// <param name="ident">Runway Ident 'nnd'</param>
    public void SetSelectedNavIdRunway( string ident )
    {
      _selRunway = ident;
      LayouIfrMarkLabels( );
    }

    /// <summary>
    /// Set the WaypointCat for the selected Runway and Approach
    /// </summary>
    /// <param name="aproachName">Approach name (RNAV, ILS ..)</param>
    public void SetSelectedNavIdRunwayApproach( string aproachName )
    {
      _selRunwayApproach = aproachName;
      LayouIfrMarkLabels( );
    }

    /// <summary>
    /// Set the List of alternate Airports for Rendering
    /// </summary>
    /// <param name="airports"></param>
    public void SetAltAirportList( List<FSimFacilityIF.IAirportDesc> airports )
    {
      // all Alt Apt items
      var aptList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.APTMARKS )?.SubItemList;
      if (aptList == null) return; // CANNOT (yet)
      aptList.Clear( );

      // make all
      foreach (var na in airports) {
        bool isSelected = na.Ident == _airportRef.ICAO;
        var altapt = new AlternateAptItem( isSelected ? Properties.Resources.airport_selected : Properties.Resources.airport ) {
          Key = GProc.DispID_Anon( ),
          Active = true, // always true, we are using the ManagedHook
          Pen = PenRange3,
          CoordPoint = na.Coordinate,
          String = $"{na.Ident}",
          Font = FtMid,
          TextBrush = BrushNavAidApt,
          RunwayIdent = "",
          WypLabelRectangle = new Rectangle( ),
        };
        altapt.StringFormat.LineAlignment = StringAlignment.Near;
        aptList.AddItem( altapt );
      }
    }

    #endregion

    #region API Route

    /// <summary>
    /// To set the Route plotted on the Map
    /// </summary>
    /// <param name="route">A route Obj</param>
    public void SetRoute( Route route )
    {
      // sanity
      if (route == null) return; // fatal programm error..
      if (route.RoutePointCat.Count < 3) return; // not enough points in route

      // all Route items
      var routeList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.ROUTE )?.SubItemList;

      if (routeList == null) return; // CANNOT (yet)
      var segMgr = (_viewportRef.GProc.Drawings.DispItem( (int)DItems.ROUTE ) as RouteHookItem).SegmentMgr;
      routeList.Clear( );

      // make all
      foreach (var rp in route.RoutePointCat) {
        var bm = rp.OutboundCoordinate.IsEmpty ? Properties.Resources.route_waypoint_end : Properties.Resources.route_waypoint;
        var rtPoint = new RoutePointItem( bm ) {
          Key = GProc.DispID_Anon( ),
          Active = true, // always true, we are using the ManagedHook
          SegmentMgr = segMgr,
          Pen = rp.OutboundApt ? PenRouteApt
                  : rp.OutboundSID ? PenRouteSid
                  : rp.OutboundSTAR ? PenRouteSid
                  : rp.OutboundAirway ? PenRouteAwy
                  : PenRoute, // select line pen, Apt has prio over Sid
          CoordPoint = rp.Coordinate,
          String = rp.IsSIDorSTAR ? $"{rp.ID}\n{rp.AltLimitS}"
                  : (rp.Coordinate.Altitude > 10) ? $"{rp.ID}\n{rp.Coordinate.Altitude:####0}" : $"{rp.ID}",
          Font = FtMid,
          TextBrush = BrushNavAidApt,
          OutboundTrack_deg = rp.OutboundTrueTrack,
          OutboundLatLon = rp.OutboundCoordinate,
          WypLabelRectangle = new Rectangle( ),
        };
        rtPoint.StringFormat.LineAlignment = StringAlignment.Near;
        routeList.AddItem( rtPoint );
      }
    }

    #endregion

    #region API Render 

    /// <summary>
    /// Renders the static drawings on the base Canvas
    /// Needs to be done when the Map or decorations changes
    /// </summary>
    public void RenderStatic( )
    {
      LayouIfrMarkLabels( ); // recalc label positions
      _viewportRef.RenderStatic( );
    }

    /// <summary>
    /// Renders the dynamic drawings on the Canvas
    /// Needs to be done when a sprite is updated
    /// </summary>
    public void RenderSprite( ) => _viewportRef.RenderSprite( );

    /// <summary>
    /// Redraw the map
    /// </summary>
    public void Redraw( ) => _viewportRef.Redraw( );

    #endregion

  }
}

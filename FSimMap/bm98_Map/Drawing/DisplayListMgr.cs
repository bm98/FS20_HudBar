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

using dNetBm98;

using FSimFacilityIF;
using FSimFlightPlans;

using static bm98_Map.Drawing.FontsAndColors;
using static bm98_Map.WypExtensions;

using bm98_Map.Drawing.DispItems;

namespace bm98_Map.Drawing
{
  /// <summary>
  /// Manages the DisplayLists of the airport
  /// and maintains the items and supports aircraft prperty changes
  /// </summary>
  internal class DisplayListMgr
  {
    /// <summary>
    /// Returns the next display mode for AI aircrafts and cycles if at end 
    /// </summary>
    /// <param name="current">Current Mode</param>
    /// <returns></returns>
    public static AcftAiDisplayMode CycleAcftAiDisplayMode( AcftAiDisplayMode current )
    {
      AcftAiDisplayMode nextMode = current + 1;
      if (Enum.IsDefined( typeof( AcftAiDisplayMode ), nextMode )) { return nextMode; }
      else { return AcftAiDisplayMode.None; }
    }

    // *** CLASS 

    #region State Record Def 

    // Internal Display State record
    private struct StateRecord
    {
      // mode how the map behaves 
      public MapBehavior MapMode;

      // wether or not showing the aircraft 
      public bool ShowTrackedAcft;
      // wether or not showing the grid
      public bool ShowMapGrid;
      // wether or not showing the scale
      public bool ShowMapScale;
      // wether or not showing the airport Range
      public bool ShowAptRings;
      // wether or not showing the Navaids
      public bool ShowNavaids;
      // wether or not showing the Route
      public bool ShowRoute;
      // wether or not showing the VFR Markings
      public bool ShowVfrMarks;
      // wether or not showing the Airport (alternates) Marks
      public bool ShowAptMarks;

      // mode how to show the AI aircrafts
      public AcftAiDisplayMode ShowTrackedAcftAI;
      // positive filter list for AI acft
      public IList<string> AcftAiFilterList;

      // selected runwayIdent 
      public string SelectedRwy;
      // selected runwayApproachIdent 
      public string SelectedRwyApproach;
      // True if in Map mode
      public bool IsMap => MapMode == MapBehavior.Map;
      // True if in Radar mode
      public bool IsRadar => MapMode == MapBehavior.Radar;

      // returns a deep copy of the argument
      public static StateRecord Copy( StateRecord other )
      {
        StateRecord ret = (StateRecord)other.MemberwiseClone( );
        ret.AcftAiFilterList = other.AcftAiFilterList.ToList( );
        return ret;
      }
    }

    #endregion

    // maximum aircraft track length
    private const int c_maxTrackPoints = 600; // see what works here

    // ref of the managed airport
    private Data.Airport _airportRef;
    // ref of the ViewPort used to show this Drawings
    private VPort2 _viewportRef;

    // display state
    private StateRecord _state = new StateRecord( ) {
      MapMode = MapBehavior.Map,
      ShowTrackedAcft = false,
      ShowMapGrid = false,
      ShowMapScale = true, // default on
      ShowAptRings = false,
      ShowNavaids = false,
      ShowRoute = false,
      ShowVfrMarks = false,
      ShowAptMarks = false,
      SelectedRwy = "",
      SelectedRwyApproach = "",
      ShowTrackedAcftAI = AcftAiDisplayMode.None,
      AcftAiFilterList = new List<string>( ),
    };
    // stored display state in Map mode
    private StateRecord _stateMapModeStored = new StateRecord( );

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
      _state.SelectedRwy = "";
      _mapRangeHandler = new MapRangeHandler( MapRange.Mid, SetZoomLevel );
    }

    /// <summary>
    /// Clear all Airport DispItems from the List (make it empty)
    /// </summary>
    public void ClearDispItems( )
    {
      _state.SelectedRwy = "";
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
      MAP_CROSE, // compass rose
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
      // tracked AI aircrafts Hook
      AIRCRAFTS_AI,
      // tracking aircraft
      AIRCRAFT,
      AIRCRAFT_RINGS, // range rings around the acft
      AIRCRAFT_RANGE, // forward range display
      AIRCRAFT_TRACK, // acft track
      AIRCRAFT_WIND,  // acft wind arrow
      // route Hook
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
        Active = _state.ShowMapGrid,
        TileMatrixRef = _viewportRef.Map,
        Pen = PenScale1,
        TextBrush = BrushScale,
        Font = FtMid,
      };
      _viewportRef.GProc.Drawings.AddItem( grid );

      // Map Scale
      var scale = new ScaleItem( ) {
        Key = (int)DItems.MAP_SCALE,
        Active = _state.ShowMapScale,
        Pen = PenScale3,
        Font = FtLarger,
        TextBrush = BrushScale,
      };
      _viewportRef.GProcSprite.Drawings.AddItem( scale );

      // Map Compass rose
      var crose = new CRoseItem( ) {
        Key = (int)DItems.MAP_CROSE,
        Active = _state.IsRadar,
        Pen = PenRoseRdr,
        Font = FtLargest,
        TextBrush = BrushRoseRdr,
      };
      _viewportRef.GProc.Drawings.AddItem( crose );

      // Airport Range Rings
      var aptRange = new AirportRingsItem( ) {
        Key = (int)DItems.APT_Range,
        Active = _state.ShowAptRings,
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
        Active = _state.ShowRoute, // visible at any Range
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

      // Tracked AI aircrafts on the Sprite Processor
      var aircraftsAI = new ManagedHookItem( ) {
        Key = (int)DItems.AIRCRAFTS_AI,
        Active = _state.ShowTrackedAcftAI != AcftAiDisplayMode.None,
        String = "AI aircrafts", // only deco for debug
      };
      _viewportRef.GProcSprite.Drawings.AddItem( aircraftsAI );

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
          Active = _state.ShowTrackedAcft,
          CoordPoint = _airportRef.Coordinate,
          Font = FtLarger,
          TextBrush = BrushRwNumber, // TODO make a distinct one
          // drawn acft
          Pen = PenAcftOutline, // outline of the shape
        };
        _viewportRef.GProcSprite.Drawings.AddItem( aircraft );


        // Aircraft range arcs (sub item of Aircraft)
        var aircraftRange = new AcftRangeItem( ) {
          Key = (int)DItems.AIRCRAFT_RANGE,
          Active = false, // init false, will be shown on update if asked for
          CoordPoint = _airportRef.Coordinate, // will be updated later
          Pen = PenRange3,
          PenTrack = PenRange5,
          TgtPen = PenTRange3,
        };
        aircraft.SubItemList.AddItem( aircraftRange );

        // Aircraft range rings (sub item of Aircraft)
        var aircraftRings = new AcftRingsItem( ) {
          Key = (int)DItems.AIRCRAFT_RINGS,
          Active = false, // init false, will be shown on update if asked for
          CoordPoint = _airportRef.Coordinate, // will be updated later
          Pen = PenAptRange,
          TextBrush = BrushAptRange,
          Font = FtLarge,
        };
        aircraft.SubItemList.AddItem( aircraftRings );

        // Aircraft wind arrow (sub item of Aircraft)
        var aircraftWind = new AcftWindItem( ) {
          Key = (int)DItems.AIRCRAFT_WIND,
          Active = false, // init false, will be shown on update if asked for
          CoordPoint = _airportRef.Coordinate, // will be updated later
          Pen = _state.IsMap ? PenAcftWind : PenAcftWindRdr,
          FillBrush = _state.IsMap ? BrushAcftWind : BrushAcftWindRdr,
          TextBrush = _state.IsMap ? BrushAcftWind : BrushAcftWindRdr,
          Font = FtSmallest,
        };
        aircraft.SubItemList.AddItem( aircraftWind );
      }
      else {
        // existing aircraft, update display props for some 
        var sdi = di.SubItemList.DispItem( (int)DItems.AIRCRAFT_WIND );
        if (sdi != null) {
          var acftWind = sdi as AcftWindItem;
          sdi.Pen = _state.IsMap ? PenAcftWind : PenAcftWindRdr;
          sdi.FillBrush = _state.IsMap ? BrushAcftWind : BrushAcftWindRdr;
          sdi.TextBrush = _state.IsMap ? BrushAcftWind : BrushAcftWindRdr;
        }
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
          di = rwyItem as RunwayItem;
          di.Font = _mapRangeHandler.IsXFar || _mapRangeHandler.IsFarFar ? null // cannot draw Text at this resolution
            : _mapRangeHandler.IsFar || _mapRangeHandler.IsMid ? FtSmaller
            : FtMid;
        }
      }

      // disable clutter at XFar
      bool active = !_mapRangeHandler.IsXFar;
      var itemList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.NAVAIDS );
      if (itemList != null) itemList.Active = active && _state.ShowNavaids;

      itemList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.WAYPOINTS );
      if (itemList != null) itemList.Active = active && _state.ShowNavaids;

      itemList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.APTMARKS );
      if (itemList != null) itemList.Active = active && _state.ShowAptMarks;

      itemList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.VFRMARKS );
      if (itemList != null) itemList.Active = active; // the hook is only used to show no marks at XFar

      itemList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.IFRMARKS );
      if (itemList != null) itemList.Active = active; // the hook is only used to show no marks at XFar

      itemList = _viewportRef.GProcSprite.Drawings.DispItem( (int)DItems.AIRCRAFTS_AI );
      if (itemList != null) itemList.Active = active && (_state.ShowTrackedAcftAI != AcftAiDisplayMode.None); // the hook is only used to show no marks at XFar
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
      di = _viewportRef.GProcSprite.Drawings.DispItem( (int)DItems.MAP_SCALE );
      if (di != null) {
        (di as ScaleItem).PixelPerNm = pixelPerNm;
      }

    }

    #region API Show Decorations

    /// <summary>
    /// True to show the tracked AI aircrafts, false otherwise
    /// </summary>
    public AcftAiDisplayMode ShowTrackedAircraftAI {
      get => _state.ShowTrackedAcftAI;
      set {
        _state.ShowTrackedAcftAI = value;
        var acftAiHook = _viewportRef.GProcSprite.Drawings.DispItem( (int)DItems.AIRCRAFTS_AI );
        if (acftAiHook != null) acftAiHook.Active = _mapRangeHandler.IsXFar ? false : (value != AcftAiDisplayMode.None); // avoid clutter at XFar

        RenderSprite( );
        Redraw( );
      }
    }

    /// <summary>
    /// Load an filter list for other acfts
    /// </summary>
    public void SetAcftAiFilter( IList<string> filterList )
    {
      _state.AcftAiFilterList = filterList.ToList( ); // copy
    }


    /// <summary>
    /// True to show the tracked aircraft, false otherwise
    /// </summary>
    public bool ShowTrackedAircraft {
      get => _state.ShowTrackedAcft;
      set {
        _state.ShowTrackedAcft = value;
        var di = _viewportRef.GProcSprite.Drawings.DispItem( (int)DItems.AIRCRAFT );
        if (di != null) di.Active = _state.ShowTrackedAcft;

        di = _viewportRef.GProcSprite.Drawings.DispItem( (int)DItems.AIRCRAFT_RINGS );
        if (di != null) di.Active = false; // will be updated when the Aircraft is update, else it's shown for a blink even if not selected

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
      get => _state.ShowRoute;
      set {
        _state.ShowRoute = value;
        var di = _viewportRef.GProc.Drawings.DispItem( (int)DItems.ROUTE );
        if (di == null) return;  // not (yet) defined
        di.Active = _state.ShowRoute;

        RenderStatic( );
        Redraw( );
      }
    }

    /// <summary>
    /// True to show the map grid, false otherwise
    /// </summary>
    public bool ShowMapGrid {
      get => _state.ShowMapGrid;
      set {
        _state.ShowMapGrid = value;
        var di = _viewportRef.GProc.Drawings.DispItem( (int)DItems.MAP_GRID );
        if (di == null) return; // not (yet) defined
        di.Active = _state.ShowMapGrid;

        RenderStatic( );
        Redraw( );
      }
    }

    /// <summary>
    /// True to show the map scale, false otherwise
    /// </summary>
    public bool ShowMapScale {
      get => _state.ShowMapScale;
      set {
        _state.ShowMapScale = value;
        var di = _viewportRef.GProcSprite.Drawings.DispItem( (int)DItems.MAP_SCALE );
        if (di == null) return; // not (yet) defined
        di.Active = _state.ShowMapScale;

        RenderStatic( );
        Redraw( );
      }
    }
    /// <summary>
    /// True to show the airport range circles, false otherwise
    /// </summary>
    public bool ShowAiportRange {
      get => _state.ShowAptRings;
      set {
        _state.ShowAptRings = value;
        var di = _viewportRef.GProc.Drawings.DispItem( (int)DItems.APT_Range );
        if (di == null) return; // not (yet) defined
        di.Active = _state.ShowAptRings;

        RenderStatic( );
        Redraw( );
      }
    }

    /// <summary>
    /// True to show the airport range circles, false otherwise
    /// </summary>
    public bool ShowNavaids {
      get => _state.ShowNavaids;
      set {
        _state.ShowNavaids = value;
        // all Navaid items
        var navHook = _viewportRef.GProc.Drawings.DispItem( (int)DItems.NAVAIDS );
        if (navHook != null) navHook.Active = _mapRangeHandler.IsXFar ? false : _state.ShowNavaids; // avoid clutter at XFar

        // show wyp for the selected runway only
        var wypHook = _viewportRef.GProc.Drawings.DispItem( (int)DItems.WAYPOINTS );
        if (wypHook != null) wypHook.Active = _mapRangeHandler.IsXFar ? false : _state.ShowNavaids; // avoid clutter at XFar
        // per Wyp now
        RenderStatic( );
        Redraw( );
      }
    }

    /// <summary>
    /// True to show the VFR marks, false otherwise
    /// </summary>
    public bool ShowVFRMarks {
      get => _state.ShowVfrMarks;
      set {
        _state.ShowVfrMarks = value;
        // all VFR marks items
        var vfrList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.VFRMARKS )?.SubItemList;
        if (vfrList == null) return; // not (yet) defined
        foreach (var vfrItem in vfrList.Values) {
          var di = vfrItem as RwyVFRMarksItem;
          di.Active = true; // always true
          di.ShowFullDecoration = _state.ShowVfrMarks; // to draw the full painting
        }
        RenderStatic( );
        Redraw( );
      }
    }

    /// <summary>
    /// True to show the Airport marks
    /// </summary>
    public bool ShowAptMarks {
      get => _state.ShowAptMarks;
      set {
        _state.ShowAptMarks = value;
        // all VFR marks items
        var di = _viewportRef.GProc.Drawings.DispItem( (int)DItems.APTMARKS );
        if (di == null) return; // not (yet) defined
        di.Active = _mapRangeHandler.IsXFar ? false : _state.ShowAptMarks; // avoid clutter at XFar

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
            ShowFullDecoration = _state.ShowVfrMarks, // to draw the full painting when VFR is selected
            StartID = rwy.Ident,
            Start = rwy.StartCoordinate,
            StartHeading_degm = rwy.Bearing_deg,
            StartRunwayIdent = rwy.Ident,
            EndID = (otherEnd == null) ? "" : otherEnd.Ident,
            End = (otherEnd == null) ? LatLon.Empty : otherEnd.StartCoordinate,
            EndHeading_degm = (otherEnd == null) ? float.NaN : otherEnd.Bearing_deg,
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

    #region API Map 

    /// <summary>
    /// Set the map mode
    /// </summary>
    /// <param name="behavior">Map mode</param>
    public void SetMapBehavior( MapBehavior behavior )
    {
      if (_state.MapMode == behavior) return; // already
      if (behavior == MapBehavior.Map) {
        _state = StateRecord.Copy( _stateMapModeStored ); // restore the Map state
        _state.MapMode = behavior;
        ShowMapGrid = _state.ShowMapGrid;
        ShowMapScale = _state.ShowMapScale;
        ShowVFRMarks = _state.ShowVfrMarks;
        ShowRoute = _state.ShowRoute;
        ShowAiportRange = _state.ShowAptRings;
        SetSelectedNavIdRunway( _state.SelectedRwy );
        SetSelectedNavIdRunwayApproach( _state.SelectedRwyApproach );
        ShowTrackedAircraft = _state.ShowTrackedAcft;
      }
      else if (behavior == MapBehavior.Radar) {
        _stateMapModeStored = StateRecord.Copy( _state ); // store a copy of the Map state
        _state.MapMode = behavior;
        // some behavior is fixed in this mode
        ShowMapGrid = false;
        ShowMapScale = false;
        ShowVFRMarks = false;
        ShowRoute = false;
        ShowAiportRange = false;
        SetSelectedNavIdRunway( "" );
        SetSelectedNavIdRunwayApproach( "" );
        ShowTrackedAircraft = true; // force 
      }
      var acftAIList = _viewportRef.GProcSprite.Drawings.DispItem( (int)DItems.AIRCRAFTS_AI )?.SubItemList;
      if (acftAIList != null) {
        acftAIList.Clear( );
      }
    }

    /// <summary>
    /// Update the ZoomLevel related drawings
    /// </summary>
    /// <param name="zoomLevel">The ZoomLevel</param>
    public void SetMapZoom( ushort zoomLevel )
    {
      _mapRangeHandler.SetZoomLevel( zoomLevel );
    }

    #endregion

    #region API AI Aircrafts

    /// <summary>
    /// Returns the next valid Display mode 
    /// </summary>
    /// <param name="current">Current display mode</param>
    /// <returns></returns>
    public AcftAiDisplayMode NextDisplayMode( AcftAiDisplayMode current )
    {
      AcftAiDisplayMode nextMode = CycleAcftAiDisplayMode( current );

      if (nextMode == AcftAiDisplayMode.Filtered && (_state.AcftAiFilterList.Count == 0)) {
        // dont show filtered if there is no filter
        nextMode = CycleAcftAiDisplayMode( nextMode );
      }
      return nextMode;
    }

    /// <summary>
    /// Update all tracked AI aircrafts
    /// </summary>
    /// <param name="aircraftsAI">AI Aircraft list</param>
    public void UpdateAircraftsAI( IList<ITrackedAircraft> aircraftsAI )
    {
      // all AI aircraft Sprites
      var acftList = _viewportRef.GProcSprite.Drawings.DispItem( (int)DItems.AIRCRAFTS_AI )?.SubItemList;
      if (acftList == null) return; // CANNOT (yet)

      // update exisiting ones and add new ones
      // mark all visible ones as Alive
      var changeList = new List<AircraftAiItem>( );

      foreach (var acftSrc in aircraftsAI) {
        if (_state.ShowTrackedAcftAI == AcftAiDisplayMode.Filtered) {
          // show filtered
          if (_state.AcftAiFilterList.Count > 0) {
            // having a filter
            if (!_state.AcftAiFilterList.Contains( acftSrc.AircraftID )) continue;  // when a filter is applied include only filtered ones
          }
          else {
            // treat as non filtered
            if (acftSrc.OnGround) continue; // don't show the ones on ground...
          }
        }
        else if (_state.ShowTrackedAcftAI == AcftAiDisplayMode.All) {
          // show all
          if (acftSrc.OnGround) {
            if (_mapRangeHandler.ZoomLevel <= (int)MapRange.Near) {
              continue; // don't show the ones on ground...
            }
          }
        }
        else {
          // show none
          continue; // don't add one
        }

        // prep the display string here
        // like "↑ 234 / 18" is going up at 23400 ft MSL 180 kt GS 
        string vs = acftSrc.Vs_fpm > 200 ? "↑" : (acftSrc.Vs_fpm < -200) ? "↓" : " ";
        string values = $"\n{vs}{XMath.RoundInt( acftSrc.AltitudeMsl_ft, 100 ) / 100:000} {XMath.RoundInt( acftSrc.Gs_kt, 10 ) / 10:00}"; // use values conditionally ??
        string dispString = $"{acftSrc.AircraftID}{values}";
        if (acftList.Values.FirstOrDefault( ac => (ac as AircraftAiItem).AircraftID == acftSrc.AircraftID ) is AircraftAiItem dstItem) {
          // update 
          dstItem.CoordPoint = acftSrc.Position; // must include AltMsl ft
          dstItem.Heading = acftSrc.TrueHeading_deg; // THDG to draw on the map
          dstItem.OnGround = acftSrc.OnGround;
          dstItem.Alive = true; // mark for later update
          dstItem.TCAS = acftSrc.TCAS;
          //          dstItem.TCAS = TcasFlag.ProximityLevel;
          dstItem.String = dispString;
        }
        else {
          // add new 
          var aircraft = new AircraftAiItem( Properties.Resources.aircraft_mid ) {
            Key = GProc.DispID_Anon( ),
            Active = true, // managed by hook
            AircraftID = acftSrc.AircraftID,
            IsHeli = acftSrc.IsHeli,
            CoordPoint = acftSrc.Position, // must include AltMsl ft
            Heading = acftSrc.TrueHeading_deg, // THDG to draw on the map
            OnGround = acftSrc.OnGround,
            String = dispString,
            TCAS = acftSrc.TCAS,
            Alive = true, // mark for later update
                          // init item drawing properties
            Font = FtLarge,
            TextBrush = _state.IsMap ? BrushAcftAiText : BrushAcftAiTextRdr,
            TextRectFillBrush = _state.IsMap ? BrushAcftAiTextBG : BrushAcftAiTextBGRdr,
            Pen = _state.IsMap ? PenAcftAiOutline : PenAcftAiOutlineRdr, // outline of the shape
          };
          changeList.Add( aircraft ); // temp list else foreach is screwed
        }
      }
      // add when complete
      foreach (var a in changeList) acftList.AddItem( a );
      changeList.Clear( );

      // remove where not Alive and reset Alive flag for next round
      foreach (var acSprite in acftList.Values) {
        var dstItem = acSprite as AircraftAiItem;
        if (dstItem.Alive) {
          dstItem.Alive = false; // reset process flag
        }
        else {
          // not Alive anymore, remove later
          changeList.Add( dstItem );
        }
      }
      // remove when complete
      foreach (var a in changeList) acftList.RemoveItem( a.Key );
      changeList.Clear( );
    }

    #endregion

    #region API Aircraft

    /// <summary>
    /// Update the tracked aircraft
    /// </summary>
    /// <param name="aircraft">The internal Aircraft</param>
    public void UpdateAircraft( TrackedAircraft aircraft )
    {
      if (_state.MapMode == MapBehavior.Radar) {
        // SetRadarMap( aircraft.Position, aircraft.TrueHeading_deg );
        //acftItem.Heading = 0; // force upright
        _viewportRef.MapHeading = aircraft.TrueHeading_deg;
        _viewportRef.SetMapCenter( aircraft.Position );
      }
      else {
        _viewportRef.MapHeading = 0;
      }

      // set compass rose 
      var di = _viewportRef.GProc.Drawings.DispItem( (int)DItems.MAP_CROSE );
      if (di != null) {
        var rose = (di as CRoseItem);
        rose.CoordPoint = aircraft.Position;
      }

      // set aircraft icon on the sprite level
      var diAcft = _viewportRef.GProcSprite.Drawings.DispItem( (int)DItems.AIRCRAFT );
      if (diAcft == null) return; // CANNOT (yet)

      // set icon
      var acftItem = (diAcft as AircraftItem);
      acftItem.Active = _state.ShowTrackedAcft;
      acftItem.CoordPoint = aircraft.Position;
      acftItem.Heading = aircraft.TrueHeading_deg;
      acftItem.OnGround = aircraft.OnGround;
      acftItem.Pen = _state.IsMap ? PenAcftOutline : PenAcftOutlineRdr; // outline of the shape

      // set range
      di = diAcft.SubItemList.DispItem( (int)DItems.AIRCRAFT_RANGE );
      if (di != null) {
        var acftRange = (di as AcftRangeItem);
        // update display of range from aircraft record
        acftRange.Active = (_state.MapMode == MapBehavior.Map) && _state.ShowTrackedAcft && aircraft.ShowAircraftRange;
        acftRange.AircraftTrackRef = aircraft;
        acftRange.CoordPoint = aircraft.Position; // default property is also set (may not be used)
      }
      // set range rings
      di = diAcft.SubItemList.DispItem( (int)DItems.AIRCRAFT_RINGS );
      if (di != null) {
        var acftRings = (di as AcftRingsItem);
        // update display of range from aircraft record
        acftRings.Active = aircraft.ShowAircraftRings || (_state.MapMode == MapBehavior.Radar); // for now show in radar mode too
        acftRings.CoordPoint = aircraft.Position;
      }

      // set wind
      di = diAcft.SubItemList.DispItem( (int)DItems.AIRCRAFT_WIND );
      if (di != null) {
        var acftWind = (di as AcftWindItem);
        // update display of range from aircraft record
        acftWind.Active = _state.ShowTrackedAcft && aircraft.ShowAircraftWind;
        acftWind.WindDir_deg = aircraft.WindDirection_deg;
        acftWind.String = aircraft.WindSpeedS;
        acftWind.CoordPoint = aircraft.Position; // default property is also set (may not be used)
      }
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
        // update display of track from aircraft record - only possible in Map mode
        trackHook.Active = (_state.MapMode == MapBehavior.Map) && _state.ShowTrackedAcft && aircraft.ShowAircraftTrack;
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
          TextBrush = _state.IsMap ? BrushNavAid : BrushNavAidRdr,
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
          TextBrush = _state.IsMap ? BrushNavAidWyp : BrushNavAidWypRdr,
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
        var aprShow = !string.IsNullOrEmpty( _state.SelectedRwyApproach )
                      && (di.RunwayApproachIdent == _state.SelectedRwyApproach); // show none if not selected
        di.Active = !_mapRangeHandler.IsXFar && (di.RunwayIdent == _state.SelectedRwy) && aprShow; // show Wyps independent of the Navaids Button
        di.ShowOutboundTrack = (string.IsNullOrEmpty( _state.SelectedRwyApproach ) == false); // track only if an Apr is selected
      }
    }

    /// <summary>
    /// Set the WaypointCat for the selected Runway
    /// </summary>
    /// <param name="ident">Runway Ident 'nnd'</param>
    public void SetSelectedNavIdRunway( string ident )
    {
      _state.SelectedRwy = ident;
      LayouIfrMarkLabels( );
    }

    /// <summary>
    /// Returns the selected Runway name
    /// </summary>
    /// <returns>An Runway name or empty</returns>
    public string GetSelectedNavIdRunway( ) => _state.SelectedRwy;

    /// <summary>
    /// Set the WaypointCat for the selected Runway and Approach
    /// </summary>
    /// <param name="aproachName">Approach name (RNAV, ILS ..)</param>
    public void SetSelectedNavIdRunwayApproach( string aproachName )
    {
      _state.SelectedRwyApproach = aproachName;
      LayouIfrMarkLabels( );
    }

    /// <summary>
    /// Returns the selected approach name
    /// </summary>
    /// <returns>An approach name or empty</returns>
    public string GetSelectedNavIdRunwayApproach( ) => _state.SelectedRwyApproach;

    /// <summary>
    /// Set the List of alternate Airports for Rendering
    /// </summary>
    /// <param name="airports"></param>
    public void SetAltAirportList( IList<FSimFacilityIF.IAirportDesc> airports )
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
          TextBrush = _state.IsMap ? BrushNavAidApt : BrushNavAidAptRdr,
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
    /// To set the Flightplan plotted on the Map (replaces Route)
    /// </summary>
    /// <param name="flightplan">A Flightplan Obj</param>
    public void SetFlightplan( FlightPlan flightplan )
    {
      // sanity
      if (flightplan == null) return; // fatal programm error..
      if (!flightplan.IsValid) return; // invalid
      if (flightplan.Waypoints.Count( ) < 3) return; // not enough points in plan

      // all Route items
      var routeList = _viewportRef.GProc.Drawings.DispItem( (int)DItems.ROUTE )?.SubItemList;

      if (routeList == null) return; // CANNOT (yet)
      var segMgr = (_viewportRef.GProc.Drawings.DispItem( (int)DItems.ROUTE ) as RouteHookItem).SegmentMgr;
      routeList.Clear( );

      // make all
      foreach (var rp in flightplan.Waypoints) {
        var bm = rp.OutboundLatLonAlt.IsEmpty ? Properties.Resources.route_waypoint_end : Properties.Resources.route_waypoint;
        var rtPoint = new RoutePointItem( bm ) {
          Key = GProc.DispID_Anon( ),
          Active = !rp.HideInMap( ), // always true, we are using the ManagedHook
          SegmentMgr = segMgr,
          Pen = rp.OutboundIsApt ? PenRouteApt
                  : rp.OutboundIsSID ? PenRouteSid
                  : rp.OutboundIsSTAR ? PenRouteSid
                  : rp.OutboundIsAirway ? PenRouteAwy
                  : PenRoute, // select line pen, Apt has prio over Sid
          CoordPoint = rp.LatLonAlt_ft,
          String = rp.IsProc ? $"{rp.Ident}\n{rp.AltitudeLimitS}"
                  : (rp.LatLonAlt_ft.Altitude > 10) ? $"{rp.Ident}\n{rp.LatLonAlt_ft.Altitude:####0}" : $"{rp.Ident}",
          Font = FtMid,
          TextBrush = _state.IsMap ? BrushNavAidApt : BrushNavAidAptRdr,
          OutboundTrack_deg = rp.OutboundTrueTrk,
          OutboundLatLon = rp.OutboundLatLonAlt,
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

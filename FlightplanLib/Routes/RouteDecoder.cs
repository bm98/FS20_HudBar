using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using FSimFacilityIF;
using static FSimFacilityIF.Extensions;
using FSFData;
using bm98_hbFolders;
using DbgLib;

namespace FlightplanLib.Routes
{
  /// <summary>
  /// Decode a Route String
  /// i.e. has some context knowledge which due to the not stringent syntax of route lines
  /// </summary>
  internal class RouteDecoder
  {
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    private bool _isValid = false;

    private readonly StringBuilder _log = new StringBuilder( );
    private readonly StringBuilder _err = new StringBuilder( );

    private List<string> _wordList;
    private RouteCapture _route;

    /// <summary>
    /// Returns the captured route
    /// </summary>
    public RouteCapture Route => _route;

    /// <summary>
    /// True when able to serve
    /// </summary>
    public bool IsValid => _isValid;


    #region Word Decoding

    /*
     from LNM: FROM[ETD] [SPEEDALT] [SIDTRANS] [ENROUTE] [STARTRANS] TO[ETA] [ALTERNATES]

      SIDTRANS is a SID and an optional transition which can be given as "SID.TRANS" or "SID TRANS".
      The generic keyword ``SID creates a direct connection to the en-route part.

      ENROUTE is a space separated list of navaids, navaid/airway/navaid combinations or user defined waypoints as coordinates.

      STARTRANS is a STAR and an optional transition which can be given as "STAR.TRANS", "STAR TRANS", "TRANS.STAR" or "TRANS STAR"
      The generic keyword STAR creates a direct connection from the en-route part to the airport.

     from SimBrief FROM[/runway] - uses DCT to STAR Transition uses non DB SID and STAR names i.e. OTREX3A rather than OTRE3A

     extended for this use: FROM[ETD] [SPEEDALT] [SIDTRANS] [ENROUTE] [STARTRANS] [APPROACH] TO[ETA] [ALTERNATES]

      APPROACH is an approach APR.APR_ID  e.g. XAPR.ILS-Z
      USER_WYP is a waypoint with name given by coordinates and opt. minAlt (not in the database)
               XWYP.Coord

     our spec:
     icao = {ucLetter | digit}3-4  // are there with 2 letters ??
     rwy= (DD[L|R|C]) - there may be other designators..
     speedAlt = ({K|N|M}DDD[D]{A|F|M|S}DDD[D])
     coord = (DD[MM[SS]]{N|S}DDD[MM[SS]]{E|W})
     IDwyp = ({ucLetter | digit}2-6[/speedAlt])
     Uwyp =  (coord.IDwyp)
     IDsid = {ucLetter | digit}1-7
     IDstar = {ucLetter | digit}1-7
     IDapr =  {ucLetter | digit | -}1-7
     sid = (IDsid[{. }IDwyp])
     wyp = ([DCT] IDwyp | Uwyp | coord)
     awywyp = (IDairway IDwyp)
     enr = (wyp | awywyp)
     star = (([IDwyp{. }]IDstar) | (IDstar[{. }IDwyp]))
     apr = (XAPR.IDapr)
     icao[/rwy] [speedAlt] [sid] {enr} [star] [apr] icao[/rwy] .. alt not yet supported

     */

    /* Examples from LNM
        Direct connection:
        EDDF LIRF or EDDF DCT LIRF.

        VOR to VOR:
        EDDF FRD KPT BOA CMP LIRF.

        Same as above with departure time ( ETD ) and arrival time ( ETA ) which both will be ignored:
        EDDF1200 FRD KPT BOA CMP LIRF1300.

        Same as above on flight level 310 at 410 knots:
        EDDF N0410F310 DCT FRD DCT KPT DCT BOA DCT CMP DCT LIRF

        Using Jet airways:
        EDDF ASKIK T844 KOVAN UL608 TEDGO UL607 UTABA UM738 NATAG Y740 LORLO M738 AMTEL M727 TAQ LIRF

        Same as above on flight level 310 at mach 0.71 with an additional speed and altitude at NATAG which will be ignored:
        EDDF M071F310 SID ASKIK T844 KOVAN UL608 TEDGO UL607 UTABA UM738 NATAG/M069F350 Y740 LORLO M738 AMTEL M727 TAQ STAR LIRF

        User defined waypoints with degree/minute notation and an alternate airport LIRE :
        EDDF N0174F255 4732N00950E 4627N01019E 4450N01103E LIRF LIRE

        Flight plan using SID and STAR procedures with transitions:
        KPWA RDHK2.HOLLE ATOKA J25 FUZ J33 CRIED J50 LFK OHIO3.LFK KHOU

        Flight plan using the generic SID and STAR keywords:
        KPWA SID ATOKA J25 FUZ J33 CRIED J50 LFK STAR KHOU

        Flight plan using SID and STAR procedures with transitions and two alternate airports:
        KPWA N0169F190 MUDDE3 ATOKA J25 FUZ J33 CRIED J50 LFK BAYYY3.SJI KHOU KCLL KVCT      
     */


    // decoding a SpeedAlt sequence
    // {K|N|M}DDD[D]{A|F|M|S}DDD[D]
    private static Regex _X_spdAlt = new Regex( @"^(?<remark>(?<speed>[KMN][0-9]{3,4})(?<alt>[AFMS][0-9]{3,4}))$",
      RegexOptions.Compiled | RegexOptions.CultureInvariant );
    private static bool DecodeSpeedAlt( string word, out string speed, out string alt )
    {
      speed = ""; alt = "";
      Match match = _X_spdAlt.Match( word );
      if (match.Success) {
        speed = match.Groups["speed"].Value;
        alt = match.Groups["alt"].Value;
        return true;
      }
      return false;
    }

    // decoding a Coordinate
    // DD[MM[SS]]{N|S}DDD[MM[SS]]{E|W} 0..89°59'59" N/S 0..180°00'00" E/W
    private static Regex _X_coord = new Regex( @"^(?<coord>(?<lat>[0-8][0-9](([0-5][0-9])([0-5][0-9])?)?[NS])(?<lon>[0-1][0-9][0-9](([0-5][0-9])([0-5][0-9])?)?[EW]))$",
      RegexOptions.Compiled | RegexOptions.CultureInvariant );
    private static bool DecodeCOORD( string word, out string lat, out string lon )
    {
      lat = ""; lon = "";
      Match match = _X_coord.Match( word );
      if (match.Success) {
        lat = match.Groups["lat"].Value;
        lon = match.Groups["lon"].Value;
        return true;
      }
      return false;
    }

    // decoding a User Waypoint
    // DD[MM[SS]]{N|S}DDD[MM[SS]]{E|W}.[0-9A-Z]1..7 {+ SpeedAlt} 0..89°59'59" N/S 0..180°00'00" E/W
    private static Regex _X_userWyp
      = new Regex( @"^(?<coord>(?<lat>[0-8][0-9](([0-5][0-9])([0-5][0-9])?)?[NS])(?<lon>[0-1][0-9][0-9](([0-5][0-9])([0-5][0-9])?)?[EW]))\.(?<id>[0-9A-Z]{1,7})(\/(?<remark>[KMN][0-9]{3,4}[AFMS][0-9]{3,4}))?$",
      RegexOptions.Compiled | RegexOptions.CultureInvariant );
    private static bool DecodeUSERWYP( string word, out string lat, out string lon, out string ident, out string speedAltRemark )
    {
      lat = ""; lon = ""; ident = ""; speedAltRemark = "";
      Match match = _X_userWyp.Match( word );
      if (match.Success) {
        lat = match.Groups["lat"].Value;
        lon = match.Groups["lon"].Value;
        ident = match.Groups["id"].Value;
        speedAltRemark = (match.Groups["remark"].Success) ? match.Groups["remark"].Value : "";
        return true;
      }
      return false;
    }

    // decoding a simple ID
    // [0-9A-Z]1..7
    private static Regex _X_id = new Regex( @"^(?<id>[0-9A-Z]{1,7})$",
      RegexOptions.Compiled | RegexOptions.CultureInvariant );
    private static bool DecodeID( string word, out string id )
    {
      id = "";
      Match match = _X_id.Match( word );
      if (match.Success) {
        id = match.Groups["id"].Value;
        return true;
      }
      return false;
    }

    // decoding an ID with dotted trailer
    // [0-9A-Z]1..7 + . + [0-9A-Z]1..7
    private static Regex _X_idDotted = new Regex( @"^(?<idAll>(?<id>[0-9A-Z]{1,7})\.(?<id2>[0-9A-Z]{1,7}))$",
      RegexOptions.Compiled | RegexOptions.CultureInvariant );
    private static bool DecodeID_DOTTED( string word, out string id, out string id2 )
    {
      id = ""; id2 = "";
      Match match = _X_idDotted.Match( word );
      if (match.Success) {
        id = match.Groups["id"].Value;
        id2 = match.Groups["id2"].Value;
        return true;
      }
      return false;
    }

    // decoding an Approach with dotted trailer
    // APR. + [0-9A-Z]1..7[\.[0-9A-Z]]
    private static Regex _X_idApproach = new Regex( @"^(?<idAll>(?<apr>XAPR\.)(?<id>[0-9A-Z]{1,7})(\-(?<suffix>[0-9A-Z]))?)$",
      RegexOptions.Compiled | RegexOptions.CultureInvariant );
    private static bool DecodeID_APR( string word, out string aprId, out string suffix )
    {
      aprId = ""; suffix = "";
      Match match = _X_idApproach.Match( word );
      if (match.Success) {
        aprId = match.Groups["id"].Value;
        if (match.Groups["suffix"].Success) suffix = match.Groups["suffix"].Value;
        return true;
      }
      return false;
    }

    // decoding an ID with SpeedAlt option
    // [0-9A-Z]1..7 + SpeedAlt
    private static Regex _X_idSpeedAlt = new Regex( @"^(?<idAll>(?<id>[0-9A-Z]{1,7})\/(?<remark>[KMN][0-9]{3,4}[AFMS][0-9]{3,4}))$",
      RegexOptions.Compiled | RegexOptions.CultureInvariant );
    private static bool DecodeID_SA( string word, out string ident, out string speedAltRemark )
    {
      ident = ""; speedAltRemark = "";
      Match match = _X_idSpeedAlt.Match( word );
      if (match.Success) {
        ident = match.Groups["id"].Value;
        speedAltRemark = match.Groups["remark"].Value;
        return true;
      }
      return false;
    }

    // decoding an Airport with Est Time option
    // [0-9A-Z]2..4 + HHMM // must match an airport ICAO 2..4 char ID
    private static Regex _X_idEstTime = new Regex( @"^(?<idAll>(?<id>[0-9A-Z]{2,4})(?<et>[0-2][0-9][0-5][0-9]))$",
      RegexOptions.Compiled | RegexOptions.CultureInvariant );
    private static bool DecodeAPT_ET( string word, out string apt, out string estTimeRemark )
    {
      apt = ""; estTimeRemark = "";
      Match match = _X_idEstTime.Match( word );
      if (match.Success) {
        apt = match.Groups["id"].Value;
        estTimeRemark = match.Groups["et"].Value;
        return true;
      }
      return false;
    }

    // decoding an Airport with Est Time option
    // [0-9A-Z]2..4 + / + DD[A-Z] // must match an airport ICAO 2..4 char ID
    private static Regex _X_idRunway = new Regex( @"^(?<idAll>(?<id>[0-9A-Z]{2,4})\/(?<rwy>[0-9][0-9][A-Z]?))$",
      RegexOptions.Compiled | RegexOptions.CultureInvariant );
    private static bool DecodeAPT_RWY( string word, out string apt, out string runway )
    {
      apt = ""; runway = "";
      Match match = _X_idRunway.Match( word );
      if (match.Success) {
        apt = match.Groups["id"].Value;
        runway = match.Groups["rwy"].Value;
        return true;
      }
      return false;
    }

    #endregion

    #region Line Decoding Passes

    // Pass1 - Scan for Airport, Cruise Speed/Alt entries and remove them
    private void Pass1( )
    {
      bool firstWord = true;
      // cannot remove while processing
      var toRemove = new List<string>( );

      // check only 1st and last 2 entries for Airports
      var scanList = new List<string>( ); ;
      if (_wordList.Count > 3) scanList = _wordList.Take( 1 ).Concat( _wordList.Skip( _wordList.Count - 2 ).Take( 2 ) ).ToList( );
      foreach (var word in scanList) {
        // More complex words first, ID last, else the ID is just not found and it ends 
        string ident;
        if (DecodeAPT_RWY( word, out ident, out string rw )) {
          var apt = DbLookup.GetAirport( ident, Folders.GenAptDBFile );
          if (apt != null) {
            if (firstWord) {
              _route.DepAirport = apt; _route.DepRwIdent = AsRwIdent( rw );
            }
            else if (!_route.HasArrival) {
              _route.ArrAirport = apt; _route.ArrRwIdent = AsRwIdent( rw );
            }
            else { _route.AltAirports.Add( ident ); }
            _log.AppendLine( $"Pass1: Airport <{ident}> with Runway <{AsRwIdent( rw )}> added" );
          }
          else {
            _log.AppendLine( $"Pass1: Airport <{ident}> not found in DB" );
            _err.AppendLine( $"No Airport for <{ident}>" );
          }
          toRemove.Add( word );
        }
        else if (DecodeAPT_ET( word, out ident, out string estT )) {
          var apt = DbLookup.GetAirport( ident, Folders.GenAptDBFile );
          if (apt != null) {
            if (firstWord) { _route.DepAirport = apt; _route.DepEstTime = estT; }
            else if (!_route.HasArrival) { _route.ArrAirport = apt; _route.ArrEstTime = estT; }
            else { _route.AltAirports.Add( ident ); }
            _log.AppendLine( $"Pass1: Airport <{ident}> with est.Time <{estT}> added" );
          }
          else {
            _log.AppendLine( $"Pass1: Airport <{ident}> not found in DB" );
            _err.AppendLine( $"No Airport for <{ident}>" );
          }
          toRemove.Add( word );
        }
        // plain ID last
        else if (DecodeID( word, out ident )) {
          // any ID word 
          var apt = DbLookup.GetAirport( ident, Folders.GenAptDBFile );
          if (apt != null) {
            if (firstWord) { _route.DepAirport = apt; }
            else if (!_route.HasArrival) { _route.ArrAirport = apt; }
            else { _route.AltAirports.Add( ident ); }
            toRemove.Add( word ); // remove only if confirmed
            _log.AppendLine( $"Pass1: Airport <{ident}> added" );
          }
        }
        firstWord = false;
      }
      // Cruise Speed Alt
      foreach (var word in _wordList) {
        if (DecodeSpeedAlt( word, out string speed, out string alt )) {
          _route.CruiseSpeedAlt = new SpeedAltRemark( word );
          _log.AppendLine( $"Pass1: Cruise Speed/All remark <{_route.CruiseSpeedAlt.AsKnots}kt,{_route.CruiseSpeedAlt.AsFeet}ft> added" );
          toRemove.Add( word );
        }
      }
      // remove handled items
      foreach (var r in toRemove) _wordList.Remove( r );
    }

    // Pass2 - Find optional SID and STAR and APPROACH
    private void Pass2( )
    {
      bool firstWord = true;
      // cannot remove while processing
      var toRemove = new List<string>( );

      foreach (var word in _wordList) {
        // More complex words first, ID last, else the ID is just not found and it ends 
        string ident;
        if (DecodeID_APR( word, out ident, out string suffix )) {
          // must be APR.aprID
          _route.ApproachIdent = ident;
          _route.ApproachSuffix = suffix;
          _log.AppendLine( $"Pass2: Approach <{ident}({suffix})> added" );
          toRemove.Add( word );
        }
        else if (DecodeID_DOTTED( word, out string id1, out string id2 )) {
          // could be SID.WYP or STAR.WYP or WYP.STAR, WYP can never have SpeedAlt remarks
          if (firstWord) {
            // must be SID.WYP
            if (_route.HasDeparture) {
              if (_route.ArrAirport.SIDs( ).Any( proc => proc.Ident == id1 )) { // Ident in id1
                var sidTrans = id2;
                var dbSid = _route.DepRunway?.SIDs.FirstOrDefault( s => s.ProcRef == id1 ) // runway SID
                        ?? _route.DepAirport?.SIDs( ).FirstOrDefault( s => s.ProcRef == id1 ); // airport SID

                _route.SID = dbSid;
                _route.SID_Transition = new RouteWaypointCapture( ) { WaypointType = WaypointTyp.WYP, WaypointIdent = sidTrans };
                _log.AppendLine( $"Pass2: SID + Transition <{word}> added, coord <{Route.SID_Transition.Coord}>" );
              }
              else {
                _log.AppendLine( $"Pass2: SID <{word}> not found in DB" );
                _err.AppendLine( $"No SID for <{word}>, ignoring SID" );
              }
            }
            else {
              _log.AppendLine( $"Pass2: SID <{word}> but no Departure airport" );
              _err.AppendLine( $"Invalid SID <{word}> (have no airport), ignoring SID" );
            }
          }
          else {
            // would be STAR.WYP or WYP.STAR
            if (_route.HasArrival) {
              // STAR Ident can be in id1 or id2
              if (_route.ArrAirport.STARs( ).Any( proc => proc.Ident == id1 )) { // Ident in id1
                var starTrans = id2;
                var dbStar = _route.ArrRunway?.STARs.FirstOrDefault( s => s.ProcRef == id1 ) // runway STAR
                      ?? _route.ArrAirport?.STARs( ).FirstOrDefault( s => s.ProcRef == id1 ); // airport STAR

                _route.STAR = dbStar;
                _route.STAR_Transition = new RouteWaypointCapture( ) { WaypointType = WaypointTyp.WYP, WaypointIdent = starTrans };
                _log.AppendLine( $"Pass2: STAR + Transition  <{word}> added" );
              }
              else if (_route.ArrAirport.STARs( ).Any( proc => proc.Ident == id2 )) { // Ident in id2
                var starTrans = id1;
                var dbStar = _route.ArrRunway?.STARs.FirstOrDefault( s => s.ProcRef == id2 ) // runway STAR
                      ?? _route.ArrAirport?.STARs( ).FirstOrDefault( s => s.ProcRef == id2 ); // airport STAR

                _route.STAR = dbStar;
                _route.STAR_Transition = new RouteWaypointCapture( ) { WaypointType = WaypointTyp.WYP, WaypointIdent = starTrans };
                _log.AppendLine( $"Pass2: STAR + Transition  <{word}> added" );
              }
              else {
                _log.AppendLine( $"Pass2: STAR <{word}> not found in DB" );
                _err.AppendLine( $"No STAR for <{word}>, ignoring STAR" );
              }
            }
            else {
              _log.AppendLine( $"Pass2: STAR <{word}> but no Arrival airport" );
              _err.AppendLine( $"Invalid STAR <{word}> (have no airport), ignoring STAR" );
            }
          }
          toRemove.Add( word ); // remove it anyway
        }

        // plain ID last
        else if (DecodeID( word, out ident )) {
          // any plain ID word 
          // simple IDs may be keywords
          if (ident == "SID") {
            // connect from DepApt to next Wyp -- TODO see if we need it or replace it with DCT
            _log.AppendLine( $"Pass2: SID keyword found" );
            toRemove.Add( word ); // remove it anyway for now
          }
          else if (ident == "STAR") {
            // connect from last Wyp to ArrApt -- TODO see if we need it or replace it with DCT
            _log.AppendLine( $"Pass2: STAR keyword found" );
            toRemove.Add( word ); // remove it anyway for now
          }
          else {
            if (firstWord) {
              // must be SID [WYP] if at all
              if (_route.HasDeparture) {
                if (_route.DepAirport.SIDs( ).Any( proc => proc.Ident == ident )) {
                  var dbSid = _route.DepRunway?.SIDs.FirstOrDefault( s => s.ProcRef == ident ) // runway SID
                          ?? _route.DepAirport?.SIDs( ).FirstOrDefault( s => s.ProcRef == ident ); // airport SID

                  _route.SID = dbSid;
                  // don't know the WYP right now
                  toRemove.Add( word ); // remove it anyway
                  _log.AppendLine( $"Pass2: SID <{word}> added" );
                }
              }
            }
            else {
              // not first - see if we can make out a STAR
              if (_route.HasArrival) {
                if (_route.ArrAirport.STARs( ).Any( proc => proc.Ident == ident )) {
                  var dbStar = _route.ArrRunway?.STARs.FirstOrDefault( s => s.ProcRef == ident ) // runway STAR
                        ?? _route.ArrAirport?.STARs( ).FirstOrDefault( s => s.ProcRef == ident ); // airport STAR

                  _route.STAR = dbStar;
                  // don't know the WYP right now, could be before or after
                  _log.AppendLine( $"Pass2: STAR <{word}> added" );
                  toRemove.Add( word ); // remove it anyway
                }
              }
            }
          }
        }
        // prep for next round
        firstWord = false;
      }
      // remove handled items
      foreach (var r in toRemove) _wordList.Remove( r );
    }


    // Pass3 - Find left over SID and STAR Wyps
    private void Pass3( )
    {
      bool firstWord = true;
      // cannot remove while processing
      var toRemove = new List<string>( );

      foreach (var word in _wordList) {
        // More complex words first, ID last, else the ID is just not found and it ends
        string ident;
        if (DecodeID_SA( word, out ident, out string saRemark )) {
          // ID with SpeedAlt remark 
          if (firstWord) {
            // can be the trailing WYP of the SID if at all and not yet set
            if (_route.HasSID && (_route.SID_Transition == null)) {
              if (_route.SID.HasFixIdent( ident )) {
                _route.SID_Transition = new RouteWaypointCapture( ) { WaypointType = WaypointTyp.WYP, WaypointIdent = ident, SpeedAlt = new SpeedAltRemark( saRemark ) };
                _log.AppendLine( $"Pass3: SID Transition <{word}> added" );
                toRemove.Add( word ); // remove it 
              }
            }
          }
          else {
            // not first - see if we can make out a STAR WYP somewhere if at all and not yet set
            if (_route.HasSTAR && (_route.STAR_Transition == null)) {
              if (_route.STAR.HasFixIdent( ident )) {
                _route.STAR_Transition = new RouteWaypointCapture( ) { WaypointType = WaypointTyp.WYP, WaypointIdent = ident, SpeedAlt = new SpeedAltRemark( saRemark ) };
                _log.AppendLine( $"Pass3: STAR Transition <{word}> added" );
                toRemove.Add( word ); // remove it 
              }
            }
          }
        }

        // plain ID last
        else if (DecodeID( word, out ident )) {
          // any plain ID word 
          if (firstWord) {
            // can be the trailing WYP of the SID if at all and not yet set
            if (_route.HasSID && (_route.SID_Transition == null)) {
              if (_route.SID.HasFixIdent( ident )) {
                _route.SID_Transition = new RouteWaypointCapture( ) { WaypointType = WaypointTyp.WYP, WaypointIdent = ident };
                _log.AppendLine( $"Pass3: SID Transition <{word}> added" );
                toRemove.Add( word ); // remove it 
              }
            }
          }
          else {
            // not first - see if we can make out a STAR WYP somewhere if at all and not yet set
            if (_route.HasSTAR && (_route.STAR_Transition == null)) {
              if (_route.STAR.HasFixIdent( ident )) {
                _route.STAR_Transition = new RouteWaypointCapture( ) { WaypointType = WaypointTyp.WYP, WaypointIdent = ident };
                _log.AppendLine( $"Pass3: SID Transition <{word}> added" );
                toRemove.Add( word ); // remove it 
              }
            }
          }
        }
        // prep for next round
        firstWord = false;
      }
      // remove handled items
      foreach (var r in toRemove) _wordList.Remove( r );
    }

    // Pass4 - Find Enroutes and standalone Wyps and COORDs
    private void Pass4( )
    {
      var toRemove = new List<string>( ); // cannot remove while processing
      List<IAirway> prevAirway = null; // as there are more than one per Ident...
                                      // Airways must be followed by a WYP belonging to the Airway
      foreach (var word in _wordList) {
        // More complex words first, ID last, else the ID is just not found and it ends
        string ident, saRemark, latS, lonS;
        if (DecodeUSERWYP( word, out latS, out lonS, out ident, out saRemark )) {
          var ll = CoordLib.Dms.ParseRouteCoord( latS + lonS );
          if (!ll.IsEmpty) {
            if (!string.IsNullOrEmpty( saRemark )) {
              // with SA
              _route.Waypoints.Add( new RouteWaypointCapture( ) {
                WaypointType = WaypointTyp.USR,
                WaypointIdent = ident,
                Coord = ll,
                SpeedAlt = new SpeedAltRemark( saRemark )
              } );
            }
            else {
              // without SA
              _route.Waypoints.Add( new RouteWaypointCapture( ) { WaypointType = WaypointTyp.USR, WaypointIdent = ident, Coord = ll } );
            }
            _log.AppendLine( $"Pass4: USER Waypoint <{word}> added" );
          }
          else {
            // should really not happen ... Decode should return number strings only
            _log.AppendLine( $"Pass4: USER Waypoint <{word}> does not resolve in a Coordinate" );
            _err.AppendLine( $"Cannot convert USER Waypoint, ignoring segment" );
          }
          toRemove.Add( word );
          prevAirway = null; // reset
        }

        else if (DecodeCOORD( word, out latS, out lonS )) {
          // a plain COORD entry
          var ll = CoordLib.Dms.ParseRouteCoord( latS + lonS );
          if (!ll.IsEmpty) {
            _route.Waypoints.Add( new RouteWaypointCapture( ) { WaypointType = WaypointTyp.COR, WaypointIdent = word, Coord = ll } );
            _log.AppendLine( $"Pass4: COORD Waypoint <{word}> added" );
          }
          else {
            // should really not happen ... Decode should return number strings only
            _log.AppendLine( $"Pass4: COORD Waypoint <{word}> does not resolve in a Coordinate" );
            _err.AppendLine( $"Cannot convert COORD Waypoint, ignoring segment" );
          }
          toRemove.Add( word );
          prevAirway = null; // reset
        }
        // having a previous Airway
        else if ((prevAirway != null)
          && DecodeID_SA( word, out ident, out saRemark )) {
          // ID with SpeedAlt remark following an Airway
          if (prevAirway.Any( awy => awy.HasFixIdent( ident ) )) {
            _route.Waypoints.Add( new RouteWaypointCapture( ) { WaypointType = WaypointTyp.WYP, AirwayIdent = prevAirway.First( ).Ident, WaypointIdent = ident, SpeedAlt = new SpeedAltRemark( saRemark ) } );
            _log.AppendLine( $"Pass4: Waypoint <{word}> on Airway <{prevAirway.First( ).Ident}> added" );
          }
          else {
            // Waypoint does not belong to Airway
            _log.AppendLine( $"Pass4: Waypoint <{word}> does not belong to Airway <{prevAirway.First( ).Ident}>" );
            _err.AppendLine( $"Invalid Waypoint <{word}> for Airway <{prevAirway.First( ).Ident}>, ignoring segment" );
          }
          toRemove.Add( word );
          prevAirway = null; // reset Airway
        }
        else if ((prevAirway != null)
          && DecodeID( word, out ident )) {
          // ID following an Airway
          if (prevAirway.Any( awy => awy.HasFixIdent( ident ) )) {
            _route.Waypoints.Add( new RouteWaypointCapture( ) { WaypointType = WaypointTyp.WYP, AirwayIdent = prevAirway.First( ).Ident, WaypointIdent = ident } );
            _log.AppendLine( $"Pass4: Airway <{prevAirway.First( ).Ident}> to Waypoint <{word}> added" );
          }
          else {
            // Waypoint does not belong to Airway
            _log.AppendLine( $"Pass4: Waypoint <{word}> does not belong to Airway <{prevAirway.First( ).Ident}>" );
            _err.AppendLine( $"Invalid Waypoint <{word}> for Airway <{prevAirway.First( ).Ident}>, ignoring segment" );
          }
          toRemove.Add( word );
          prevAirway = null; // reset Airway
        }

        // no previous Airway
        else if (DecodeID_SA( word, out ident, out saRemark )) {
          // an ID_SA word not following an Airway
          _route.Waypoints.Add( new RouteWaypointCapture( ) { WaypointType = WaypointTyp.WYP, WaypointIdent = ident, SpeedAlt = new SpeedAltRemark( saRemark ) } );
          _log.AppendLine( $"Pass4: Waypoint <{word}> added" );
          toRemove.Add( word );
          prevAirway = null; // reset Airway
        }
        // plain ID last
        else if (DecodeID( word, out ident )) {
          if (ident == "DCT") {
            _log.AppendLine( $"Pass4: DCT keyword found" );
            toRemove.Add( word ); // remove it 
          }
          // an ID word not following an Airway, can be a new Airway
          else if (DbLookup.HasAirway( ident, Folders.GenAptDBFile )) {
            prevAirway = DbLookup.AirwayList( ident, Folders.GenAptDBFile ).ToList( );
            _log.AppendLine( $"Pass4: Airway <{word}> found" );
            toRemove.Add( word ); // remove it 
          }
          else {
            // take it as standalone WYP
            _route.Waypoints.Add( new RouteWaypointCapture( ) { WaypointType = WaypointTyp.WYP, WaypointIdent = ident } );
            _log.AppendLine( $"Pass4: Waypoint <{word}> added" );
            toRemove.Add( word ); // remove it 
            prevAirway = null; // reset Airway
          }
        }
      }
      // remove handled items
      foreach (var r in toRemove) _wordList.Remove( r );
    }

    // derive an Altitude Profile
    private void Pass5( )
    {
      // try to get an initial altitude - cruise Alt would be best..
      SpeedAltRemark current = _route.CruiseSpeedAlt.IsValid
        ? _route.CruiseSpeedAlt
        // N=Knots, A= Feet*100
        : new SpeedAltRemark( ) { Remark = "N180A050" }; // 180kt at 5000 ft (anything is wrong if not given...)
      foreach (var wyp in _route.Waypoints) {
        // change current if valid
        if (wyp.SpeedAlt.IsValid) {
          current = wyp.SpeedAlt;
        }
        wyp.SpeedAlt = current; // set all with current
      }
    }

    #endregion

    /// <summary>
    /// cTor: Instant Decoding when the Decoder is created
    /// </summary>
    /// <param name="routeString">The string to tokenize</param>
    /// <param name="err">Error Out</param>
    public RouteDecoder( string routeString, StringBuilder err )
    {
      _err = err ?? new StringBuilder( );
      _route = new RouteCapture( );

      if (string.IsNullOrWhiteSpace( routeString )) {
        _isValid = false;
        _log.AppendLine( "Route Decoder - routeString is empty" );
        _err.AppendLine( "Route Decoder - routeString is empty" );
      }

      else {
        _log.AppendLine( "Route Decoder - routeString is:" );
        _log.AppendLine( $"<{routeString}>" );

        _isValid = true; // invalidate if needed
                         // create a word list from the input (separator is space and/or tab)
        _wordList = routeString.Split( new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries ).ToList( );

        // scan and remove the Airports and CruiseSpeedAlt
        Pass1( );
        // try find and remove SID and STAR Idents
        Pass2( );
        // try find and remove SID and STAR Transition Wyps when there are
        Pass3( );
        // left with Enroutes and Wyps
        Pass4( );
        // by now the route should be complete and the _wordList empty..
        // Add SpeedAlt Profile
        Pass5( );

        if (_wordList.Count > 0) {
          _log.AppendLine( $"{_wordList.Count} remaining route items found" );
          _err.AppendLine( $"{_wordList.Count} remaining route items found" );
        }
      }

      if (_err.Length > 0) {
        _err.Insert( 0, "Route Decoder Errors:\n" );
      }

      _route.IsValid = _isValid;

      LOG.Log( "RouteDecoding", _log.ToString( ) );
      LOG.Log( "RouteDecoding", $"Outcome is valid <{_isValid}>" );
    }

  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

using FSimFacilityIF;

namespace FlightplanLib.LNM.LNMDEC
{
  /// <summary>
  /// An LNM Exported Flight Plan
  /// </summary>
  [XmlRoot( ElementName = "LittleNavmap", IsNullable = false )]
  public class LNM
  {
    // Attributes
    /// <summary>
    /// The LittleNavmap attributes
    /// </summary>
    [XmlAnyAttribute]
    public XmlAttribute[] Attributes { get; set; }

    // Elements
    /// <summary>
    /// Flightplan
    /// </summary>
    [XmlElement( ElementName = "Flightplan" )]
    public X_Flightplan Flightplan { get; set; } = null;

    // Non XML

    /// <summary>
    /// True if Departure is available
    /// </summary>
    [XmlIgnore]
    public bool HasDeparture => Flightplan.Departure != null;
    /// <summary>
    /// True if SID is available
    /// </summary>
    [XmlIgnore]
    public bool HasSID => Flightplan.Procedures.SID != null;
    /// <summary>
    /// True if STAR is available
    /// </summary>
    [XmlIgnore]
    public bool HasSTAR => Flightplan.Procedures.STAR != null;
    /// <summary>
    /// True if Approach is available
    /// </summary>
    [XmlIgnore]
    public bool HasApproach => Flightplan.Procedures.Approach != null;

    /// <summary>
    /// True if successfully retrieved
    /// </summary>
    [XmlIgnore]
    public bool IsValid => Flightplan != null;


    /// <summary>
    /// Returns the Plan as RouteString
    /// </summary>
    [XmlIgnore]
    public string AsRouteString {
      get {
        return LNMplnAsRouteString( this );
      }
    }

    /// <summary>
    /// Converts an LNM Plan into a RouteString
    /// </summary>
    /// <param name="pln">An LNM Plan</param>
    /// <returns>A route string or empty</returns>
    private static string LNMplnAsRouteString( LNM pln )
    {
      // sanity
      if (pln == null) return string.Empty;
      if (!pln.IsValid) return string.Empty;

      string rte = "", t = "";

      // shortcuts
      X_Flightplan fp = pln.Flightplan;
      X_Procedures proc = pln.Flightplan.Procedures;
      List<X_Waypoint> wyps = pln.Flightplan.WaypointCat.Waypoints;

      // DEP CSPEEDALT SID WYP ENROUTE STAR. APP. ARR ALT

      // Departure Apt / Runway
      // Apt is the 1st Wyp with Type AIRPORT
      string apt = wyps.FirstOrDefault( x => x.WaypointType == WaypointTyp.APT ).Ident;
      // Rwy is in the SID or in Departure
      string rwy = "";
      if (pln.HasSID) {
        rwy = proc.SID.Runway;
      }
      else if (pln.HasDeparture && pln.Flightplan.Departure.StartType == StartTypeE.Runway) {
        rwy = fp.Departure.Start;
      }
      t = string.IsNullOrEmpty( rwy ) ? $"{apt}" : $"{apt}/{rwy}"; rte += t;

      // cruise speed alt NdddAddd (any checks needed ??) speed is not known
      t = $" N0000A{fp.Header.CruiseAlt_ft / 100:000}"; rte += t;

      // SID
      // if SID is available SID is either an official name or CUSTOMDEPART
      if (pln.HasSID && !proc.SID.IsCustom) {
        t = $" {proc.SID.Name}";
        if (proc.SID.HasTransition) {
          t += $".{proc.SID.Transition}";
        }
        rte += t;
      }

      // ENROUTEs
      // LNM Plans have all route points of an airway included, must use the Exit only
      string lastAwy = "", lastWyp = "";
      foreach (X_Waypoint wp in wyps) {
        if (wp.WaypointType == WaypointTyp.APT) { continue; }
        if (wp.WaypointType == WaypointTyp.Unknown) { continue; }
        if (wp.WaypointType == WaypointTyp.USR) {
          // as a user WYP
          string llS = CoordLib.Dms.ToRouteCoord( new CoordLib.LatLon( wp.Pos.Lat, wp.Pos.Lon ), "dms" );
          t = $" {llS}.{wp.Ident}";
          // add SpeedAlt 
          // speed alt NdddAddd (any checks needed ??) speed is not known
          t += $"/N0000A{wp.Pos.Alt / 100f:000}"; rte += t;
          // kill Awy store when a USER Wyp is found
          lastAwy = ""; lastWyp = "";
        }
        else {
          // any wyp
          if (wp.HasAirway) {
            if (wp.Airway == lastAwy) {
              // store and wait for the next non Awy or Awy change
              lastWyp = wp.Ident;
            }
            else {
              // a new Awy - write the old one - if there is..
              if (!string.IsNullOrEmpty( lastAwy )) {
                rte += $" {lastAwy} {lastWyp}" + $"/N0000A{wp.Pos.Alt / 100f:000}";
              }
              // new Awy starts
              lastAwy = wp.Airway;
              lastWyp = wp.Ident;
            }
          }
          else {
            // no Awy - write the old one - if there is..
            if (!string.IsNullOrEmpty( lastAwy )) {
              rte += $" {lastAwy} {lastWyp}" + $"/N0000A{wp.Pos.Alt / 100f:000}";
            }
            // no more Awy
            lastAwy = "";
            lastWyp = "";
            // add SpeedAlt 
            // speed alt NdddAddd (any checks needed ??) speed is not known
            rte += $" {wp.Ident}" + $"/N0000A{wp.Pos.Alt / 100f:000}";
          }
        }
      }

      // STAR
      // if STAR is available STAR is either an official name or ?? others ??
      if (pln.HasSTAR) {
        t = $" {proc.STAR.Name}";
        if (proc.STAR.HasTransition) {
          t += $".{proc.STAR.Transition}";
        }
        rte += t;
      }

      // APR
      if (pln.HasApproach) {
        t = $" XAPR.{proc.Approach.ProcRef}";
        rte += t;
      }

      // Arrival Apt / Runway
      // Apt is the last Wyp with Type AIRPORT ?? check how Alternates are handled
      apt = wyps.LastOrDefault( x => x.WaypointType == WaypointTyp.APT ).Ident;
      // Rwy is in the SID or in Departure
      rwy = "";
      if (pln.HasApproach) {
        rwy = proc.Approach.Runway;
      }
      else if (pln.HasSTAR) {
        rwy = proc.STAR.Runway;
      }
      t = string.IsNullOrEmpty( rwy ) ? $" {apt}" : $" {apt}/{rwy}"; rte += t;

      // Alternates ??

      return rte;
    }

  }
}

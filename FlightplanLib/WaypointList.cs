using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib
{
  /// <summary>
  /// A list of Waypoints
  /// </summary>
  public class WaypointList : List<Waypoint>
  {
    /// <summary>
    /// Merges this Waypoint Lists into one, using this as Master
    /// and inserting from the Secondary Waypoints that are not in the this List
    /// identical Waypoints share the same Ident, Type and Usage
    /// (see Waypoint.Equals())
    /// </summary>
    /// <param name="sec">Secondary List</param>
    /// <returns>A merged list</returns>
    public WaypointList Merge( WaypointList sec ) => Merge( this, sec );




    /// <summary>
    /// Merges two Waypoint Lists into one starting with the Primary List as Master
    /// and inserting from the Secondary Waypoints that are not in the Primary List
    /// identical Waypoints share the same Ident, Type and Usage
    /// (see Waypoint.Equals())
    /// </summary>
    /// <param name="prim">Primary List</param>
    /// <param name="sec">Secondary List</param>
    /// <returns>A merged list</returns>
    public static WaypointList Merge( WaypointList prim, WaypointList sec )
    {
      // sanity
      if (prim == null && sec == null) return new WaypointList( );
      if (prim == null || (prim.Count == 0)) return sec;
      if (sec == null || (sec.Count == 0)) return prim;

      // both lists have at least one element
      WaypointList target = new WaypointList( );

      Queue<Waypoint> pQueue = new Queue<Waypoint>( prim );
      Queue<Waypoint> sQueue = new Queue<Waypoint>( sec );

      Waypoint p = pQueue.Dequeue( ), s = sQueue.Dequeue( );

      while (true) {
        if (p.Equals( s )) {
          // same items
          // prefer the one with Alt Info
          if (p.AltitudeLo_ft > 0) {
            target.Add( p ); // add from Prim and advance both
          }
          else if (s.AltitudeLo_ft > 0) {
            target.Add( s ); // add from Sec and advance both
          }
          else {
            target.Add( p ); // add from Prim and advance both
          }
          p = (pQueue.Count > 0) ? pQueue.Dequeue( ) : null;
          s = (sQueue.Count > 0) ? sQueue.Dequeue( ) : null;
        }
        else {
          // not the same...
          if (sQueue.Any( wyp => wyp.Equals( p ) )) {
            // sQueue has p later - add s and advance Sec
            target.Add( s );
            s = (sQueue.Count > 0) ? sQueue.Dequeue( ) : null;
          }
          else if (pQueue.Any( wyp => wyp.Equals( s ) )) {
            // pQueue has s later - add p and advance Prim
            target.Add( p );
            p = (pQueue.Count > 0) ? pQueue.Dequeue( ) : null;
          }
          else {
            // neither Prim nor Sec share p or s with each other
            // add p, then s and advance both
            target.Add( p );
            p = (pQueue.Count > 0) ? pQueue.Dequeue( ) : null;
            target.Add( s );
            s = (sQueue.Count > 0) ? sQueue.Dequeue( ) : null;
          }
        }

        // check end conditions
        if (p == null) {
          // no more Prim items left, add s and the rest of Sec and end
          if (s != null) target.Add( s );
          target.AddRange( sQueue );
          break;
        }
        if (s == null) {
          // no more Sec items left, add p and the rest of Prim and end
          if (p != null) target.Add( p );
          target.AddRange( pQueue );
          break;
        }
      }// while

      return target;
    }


  }
}
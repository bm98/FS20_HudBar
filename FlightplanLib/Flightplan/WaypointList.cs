using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.Flightplan
{
  /// <summary>
  /// A list of Waypoints
  /// </summary>
  public class WaypointList : List<Waypoint>
  {

    /// <summary>
    /// Append sec to prim List and Merge Equal Waypoints 
    /// Note: scans from the end of prim to find the merge point
    ///       then appends sec with merging where it applies
    /// </summary>
    /// <param name="prim">List to append to</param>
    /// <param name="sec">List to append</param>
    /// <param name="beforePt">Add before this point, if default just append</param>
    /// <returns>A merged, appended list</returns>
    internal static WaypointList AppendWithMerge( WaypointList prim, WaypointList sec, Waypoint beforePt )
    {
      // sanity
      if (prim == null || (prim.Count == 0)) return sec ?? new WaypointList( );
      if (sec == null || (sec.Count == 0)) return prim ?? new WaypointList( );

      // both lists have at least one element
      WaypointList target = (WaypointList)prim.MemberwiseClone( );
      Queue<Waypoint> sQueue = new Queue<Waypoint>( sec );

      int insertIndex = -1; // add
      if (beforePt != default) {
        insertIndex = prim.IndexOf( beforePt );
      }

      // find insertion point in prim
      while (true) {
        if (sQueue.Count <= 0) break; // sec is empty now

        var ptToAppend = sQueue.Dequeue( );
        // from end find the first Equal or append at end
        var mergePt = target.LastOrDefault( wyp => wyp.Equals( ptToAppend ) );
        if (mergePt != default) {
          mergePt.Merge( ptToAppend );
        }
        else {
          if (insertIndex >= 0) {
            target.Insert( insertIndex++, ptToAppend );
          }
          else {
            target.Add( ptToAppend );
          }
        }
      }

      return target;
    }

    /// <summary>
    /// Merges two Waypoint Lists into one starting with the Primary List as Master
    /// and inserting from the Secondary Waypoints that are not in the Primary List
    /// identical Waypoints share the same Ident, Type and Usage
    /// (see Waypoint.Equals())
    /// </summary>
    /// <param name="prim">Primary List</param>
    /// <param name="sec">Secondary List</param>
    /// <returns>A merged list</returns>
    internal static WaypointList Merge( WaypointList prim, WaypointList sec )
    {
      // sanity
      if (prim == null || (prim.Count == 0)) return sec ?? new WaypointList( );
      if (sec == null || (sec.Count == 0)) return prim ?? new WaypointList( );

      // both lists have at least one element
      WaypointList target = new WaypointList( );

      Queue<Waypoint> pQueue = new Queue<Waypoint>( prim );
      Queue<Waypoint> sQueue = new Queue<Waypoint>( sec );

      Waypoint p = pQueue.Dequeue( ), s = sQueue.Dequeue( );

      while (true) {
        if (p.Equals( s )) {
          // same items - Merge with Primary takes precedence
          //p.Merge( s );
          target.Add( p.Merge( s ) ); // add from Sec and advance both
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

    // ***** CLASS *****

    /// <summary>
    /// Add a Waypoint to the list
    /// Note: avoids duplicates by merging consecutive Waypoints which are Equal (the new one takes precedence)
    /// </summary>
    /// <param name="waypoint">A Waypoint</param>
    public new void Add( Waypoint waypoint )
    {
      // Merge if the same as before
      if (this.Count == 0) base.Add( waypoint );
      // having a previous
      var prevPt = this.Last( );
      if (waypoint.Equals( prevPt )) {
        base.Remove( prevPt );
        base.Add( waypoint.Merge( prevPt ) );
      }
      else {
        base.Add( waypoint );
      }
    }

    /// <summary>
    /// Add a Range of Waypoints to the list
    /// Note: avoids duplicates by merging consecutive Waypoints which are Equal (the new one takes precedence)
    /// </summary>
    /// <param name="waypoints">Range of waypoints</param>
    public new void AddRange( IEnumerable<Waypoint> waypoints )
    {
      foreach (Waypoint waypoint in waypoints) {
        this.Add( waypoint );
      }
    }


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
    /// Append sec to prim List and Merge Equal Waypoints 
    /// Note: scans from the end of prim to find the merge point
    ///       then appends sec with merging where it applies
    /// </summary>
    /// <param name="sec">List to append</param>
    /// <param name="beforePt">Add before this point, if default just append</param>
    /// <returns>A merged, appended list</returns>
    public WaypointList AppendWithMerge( WaypointList sec, Waypoint beforePt = default ) => AppendWithMerge( this, sec, beforePt );


  }
}
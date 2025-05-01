using System;


using CoordLib;

using FSimFacilityIF;

namespace bm98_Map
{
  /// <summary>
  /// A Map Creator Interface for the User Control
  ///  Define the Airport and then Commit will cause the UC to do it's thing
  ///  the Commited Airport will not change until the next Commit
  ///  
  /// </summary>
  sealed public class MapCreator
  {
    // the DataStore 
    private Data.Airport _airport = new Data.Airport( );
    private Data.Airport _airportCommited = new Data.Airport( );

    // data entry status
    private bool _commited = false;

    /// <summary>
    /// The Entry was Committed
    /// </summary>
    internal event EventHandler Committed;
    private void OnCommitted( )
    {
      Committed?.Invoke( this, new EventArgs( ) );
    }

    /// <summary>
    /// Returns the Commited airport
    /// </summary>
    internal Data.Airport CommitedAirport => _airportCommited;

    /// <summary>
    /// Returns a Dummy Airport at Coordinates
    /// </summary>
    /// <param name="latLon">Initial Coordinates</param>
    /// <returns>An IAirport object</returns>
    public IAirport DummyAirport( LatLon latLon ) { return Data.Airport.DummyAirport( latLon ); }

    /// <summary>
    /// Reset Data Entry - set the Dummy Airport @ 0,0,0, as default
    ///  Note: Comitting now will cause the Dummy Airport getting committed - which is OK if intended
    /// </summary>
    public void Reset( )
    {
      _airport = (Data.Airport)DummyAirport( new LatLon( 0, 0, 0 ) ).Clone( );
      _commited = false;
    }

    /// <summary>
    /// Define the Airport
    ///   Will overwrite each time written..
    /// Throws InvalidOperation if already Commited
    /// </summary>
    /// <param name="airport">Airport Source</param>
    public void SetAirport( IAirport airport )
    {
      if (_commited) throw new InvalidOperationException( "Already Commited - must Reset first" );
      _airport = airport.Clone<Data.Airport>( ); // must use the extension to clone from another IAirport Implementer
    }

    /// <summary>
    /// Commit DataEntry and inform the processor
    /// Throws InvalidOperation if the Airport is not defined
    /// </summary>
    public void Commit( )
    {
      if (string.IsNullOrWhiteSpace( _airport.ICAO )) throw new InvalidOperationException( "Airport is not defined, cannot commit" );

      // transfer to commited 
      _airportCommited = _airport;
      // and start a new one - cannot hold the ref of the commited apt
      _airport = (Data.Airport)DummyAirport( new LatLon( 0, 0, 0 ) ).Clone( );
      _commited = true;
      OnCommitted( ); // signal the user control
    }




  }
}

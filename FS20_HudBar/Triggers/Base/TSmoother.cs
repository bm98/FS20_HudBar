
namespace FS20_HudBar.Triggers.Base
{
  /// <summary>
  /// A delay line for values
  /// </summary>
  class TSmoother
  {
    private readonly int _weight = 5;
    private readonly int _weight1 = 6;
    private double _store = 0;

    /// <summary>
    /// cTor: create a Smoother with a weight 
    /// </summary>
    /// <param name="weight">A weight of newly added values (1/n)</param>
    public TSmoother( int weight = 5 )
    {
      if ( weight < 1 ) weight = 1;

      _weight = weight - 1;
      _weight1 = weight;
    }

    /// <summary>
    /// Set a value
    /// </summary>
    /// <param name="value">The value to set</param>
    public void Set( bool value )
    {
      Set( value ? 1 : 0 );
    }

    /// <summary>
    /// Set a value
    /// </summary>
    /// <param name="value">The value to set</param>
    public void Set( int value )
    {
      Set( (float)value );
    }

    /// <summary>
    /// Set a value
    /// </summary>
    /// <param name="value">The value to set</param>
    public void Set( float value )
    {
      _store = value;
    }

    /// <summary>
    /// Set a value
    /// </summary>
    /// <param name="value">The value to set</param>
    public void Add( bool value )
    {
      Add( value ? 1 : 0 );
    }

    /// <summary>
    /// Add a value
    /// </summary>
    /// <param name="value">The value to add</param>
    public void Add( int value )
    {
      Add( (float)value );
    }

    /// <summary>
    /// Add a value
    /// </summary>
    /// <param name="value">The value to add</param>
    public void Add( float value )
    {
      _store = ( ( _weight * _store ) + value ) / _weight1;
    }

    /// <summary>
    /// Return the smoothed result
    /// </summary>
    public int GetInt => (int)_store;
    /// <summary>
    /// Return the smoothed result
    /// </summary>
    public float GetFloat => (float)_store;
    /// <summary>
    /// Return the smoothed result
    /// </summary>
    public bool GetBool => _store > 0.5;


  }
}

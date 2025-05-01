namespace bm98_Map
{
  /// <summary>
  /// UC_Map asks to teleport the aircraft to a new location
  /// </summary>
  public class TeleportEventArgs
  {

    /// <summary>
    /// Coord Lat
    /// </summary>
    public double Lat { get; set; }
    /// <summary>
    /// Coord Lon
    /// </summary>
    public double Lon { get; set; }

    /// <summary>
    /// True when Altitude means MSL, else AOG
    /// </summary>
    public bool AltIsMSL { get; set; }
    /// <summary>
    /// Altitude value [ft]
    /// </summary>
    public int Altitude_ft { get; set; }

    /// <summary>
    /// cTor:
    /// </summary>
    public TeleportEventArgs( double lat, double lon, bool altIsMsl, int altitude_ft )
    {
      Lat = lat;
      Lon = lon;
      AltIsMSL = altIsMsl;
      Altitude_ft = altitude_ft;
    }
  }
}

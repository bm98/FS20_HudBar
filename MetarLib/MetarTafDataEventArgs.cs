namespace MetarLib
{
  /// <summary>
  /// Event fired when new METAR data arrives
  /// 
  /// </summary>
  public class MetarTafDataEventArgs
  {
    /// <summary>
    /// The Metar Data Record
    /// </summary>
    public MetarTafDataList MetarTafData { get; set; }
    /// <summary>
    /// cTor:
    /// </summary>
    public MetarTafDataEventArgs( MetarTafDataList data )
    {
      MetarTafData = data;
    }

  }
}

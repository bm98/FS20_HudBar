namespace MetarLib.MDEC
{
  /// <summary>
  /// An abstract base class for all Chunks
  /// </summary>
  abstract public class Chunk
  {
    /// <summary>
    /// Record Valid Flag
    /// </summary>
    public bool Valid { get; set; } = false;
    /// <summary>
    /// Used METAR chunks in this record 
    /// </summary>
    public string Chunks { get; set; } = ""; // debug, store raw chunks

    /// <summary>
    /// Readable Content
    /// </summary>
    virtual public string Pretty => Chunks;

  }

}

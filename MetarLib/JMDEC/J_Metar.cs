using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MetarLib.JMDEC
{
  /// <summary>
  /// Metar info as of NOV 2025
  /// </summary>
  [DataContract]
  internal class J_Metar
  {
    /// <summary>
    /// List of METAR observations delivered
    /// </summary>
    [DataMember (Name ="list")]
    public J_MetarEntry[] MetarList { get; set; } = null;


  }
}

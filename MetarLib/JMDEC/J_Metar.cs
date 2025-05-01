using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MetarLib.JMDEC
{
  /// <summary>
  /// Metar info as of OCT 2023
  /// </summary>
  [DataContract]
  internal class J_Metar
  {

    [DataMember]
    public List<J_MetarEntry> MetarList { get; set; }=new List<J_MetarEntry>();


  }
}

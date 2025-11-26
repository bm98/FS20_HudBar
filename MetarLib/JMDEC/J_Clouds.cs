using System.Runtime.Serialization;

namespace MetarLib.JMDEC
{
  /// <summary>
  /// Cloud Entry
  /// </summary>
  [DataContract]
  internal class J_Clouds
  {
    /*
     
              {
                "cover": "BKN",
                "base": 21000
              }
     
     */
    /// <summary>
    /// Cover coverage Allowed values "CLR" "CAVOK" "FEW" "SCT" "BKN" "OVC" "OVX"  - Default ?
    /// /// </summary>
    [DataMember( Name = "cover", IsRequired = true )] // "cover": "BKN",
    public string CoverCode { get; set; } = "";

    /// <summary>
    /// Cloud base in feet 
    /// </summary>
    [DataMember( Name = "base", IsRequired = false )] // "base": 21000
    public float? Base_ftS { get; set; } = null;
    /// <summary>
    /// Cloud top in feet
    /// </summary>
    [DataMember( Name = "top", IsRequired = false )] // "top": 21000
    public float? Top_ftS { get; set; } = null;

    // non JSON


  }
}

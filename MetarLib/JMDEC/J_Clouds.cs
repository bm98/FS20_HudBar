using System.Runtime.Serialization;

namespace MetarLib.JMDEC
{
  /// <summary>
  /// Cloud Entry
  /// </summary>
  [DataContract]
  internal class J_Clouds
  {
    [DataMember( Name = "cover", IsRequired = false )]
    public string CoverCode { get; set; } = "";

    [DataMember( Name = "base", IsRequired = false )]
    public float? Base_ft { get; set; } = 0f;
  }
}

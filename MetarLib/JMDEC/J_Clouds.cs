using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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

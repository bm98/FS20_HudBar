﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.SimBrief.SBDEC
{
  /// <summary>
  /// Images Data Record
  /// </summary>
  [DataContract]
  public class Json_Images
  {
    [DataMember( Name = "directory", IsRequired = true )]
    public string Directory { get; set; } = ""; // a base URL for getting file downloads

    [DataMember( Name = "map", IsRequired = false )]
    public List<Json_NameLinkPair> Files { get; set; } = new List<Json_NameLinkPair>( );

  }
}

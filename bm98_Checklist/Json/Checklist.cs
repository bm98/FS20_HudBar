using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace bm98_Checklist.Json
{
  /// <summary>
  /// One Checklist 
  ///  Name + 4 Phases
  /// </summary>
  [DataContract]
  internal class Checklist
  {
    [DataMember( IsRequired = true, Name = "name" )]
    public string Name { get; internal set; } = "";

    [DataMember( IsRequired = true, Name = "phases" )]
    public List<CheckPhase> Phases { get; internal set; } = new List<CheckPhase>( );

    // non Json

    internal void Populate( )
    {

    }

  }
}

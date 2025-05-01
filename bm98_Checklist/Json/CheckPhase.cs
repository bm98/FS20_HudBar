using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace bm98_Checklist.Json
{
  /// <summary>
  /// One Check Phase
  ///  Name + Enabled + 10 Checks
  /// </summary>
  [DataContract]
  internal class CheckPhase
  {
    /// <summary>
    /// The Name of this Phase
    /// </summary>
    [DataMember( IsRequired = true, Name = "name" )]
    public string Name { get; internal set; } = "";
    /// <summary>
    /// True when Enabled
    /// </summary>
    [DataMember( IsRequired = true, Name = "enabled" )]
    public bool Enabled { get; internal set; } = false;
    /// <summary>
    /// The List of Checks 1..N
    /// Assume not existing elements as empty
    /// </summary>
    [DataMember( IsRequired = true, Name = "checks" )]
    public List<string> Checks { get; internal set; } = new List<string>( );


    // non Json

    internal void Populate( )
    {

    }


  }
}

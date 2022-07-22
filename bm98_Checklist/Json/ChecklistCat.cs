using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace bm98_Checklist.Json
{
  /// <summary>
  /// Catalog of all checklists
  /// </summary>
  [DataContract]
  internal class ChecklistCat
  {
    /// <summary>
    /// True for a horizontal rather than vertical box
    /// </summary>
    [DataMember( IsRequired = true, Name = "horizontal" )]
    public bool Horizontal { get; internal set; } = false;

    /// <summary>
    /// The List of Checks 1..N
    /// Assume not existing elements as empty
    /// </summary>
    [DataMember( IsRequired = true, Name = "checklists" )]
    public List<Checklist> Checklists { get; internal set; } = new List<Checklist>( );


    /// <summary>
    /// A user Font config string
    /// </summary>
    [DataMember( IsRequired = false, Name = "userfont" )]
    public string UserFont { get; internal set; } = "";


    // non Json

    internal void Populate( )
    {

    }

  }
}

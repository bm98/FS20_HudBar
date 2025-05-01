using System;
using System.Collections.Generic;
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

    /// <summary>
    /// A user CheckColor config item
    /// </summary>
    [DataMember( IsRequired = false, Name = "checkcolor" )]
    public int CheckColor { get; internal set; } = (int)bm98_Checklist.SwitchColor.Blue;

    /// <summary>
    /// A user CheckBox Size config item
    /// </summary>
    [DataMember( IsRequired = false, Name = "checksize" )]
    public int CheckSize { get; internal set; } = (int)bm98_Checklist.CheckSize.SizeMedium;

    /// <summary>
    /// Json Version
    /// </summary>
    [DataMember( IsRequired = false, Name = "version" )]
    public int LayoutVersion { get; internal set; }


    // non Json

    /// <summary>
    /// The Json Layout version 
    /// </summary>
    public static int LAYOUT_VERSION = 0;

    internal static void VersionUp(ChecklistCat cat )
    {
      if (cat.LayoutVersion == LAYOUT_VERSION) return; // nothing to do

    }

    internal void Populate( )
    {

    }

  }
}

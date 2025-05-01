namespace FS20_HudBar.Config
{
  /// <summary>
  /// Store for the items that form one profile
  /// </summary>
  public class ProfileItemsStore
  {
    // NOTE Use the CProfile.Divider, FlowBreak, NoBreak chars to split the items

    /// <summary>
    /// The Profile Name
    /// </summary>
    public string Name { get; private set; } = "";
    /// <summary>
    /// The Profile string (0;1..)
    /// </summary>
    public string Profile { get; private set; } = "";
    /// <summary>
    /// The Display order of the items (numbers => 0 based index in the GUI sequence)
    /// </summary>
    public string DispOrder { get; private set; } = "";
    /// <summary>
    /// A Flow Break tag (1 for a break, 0 as default)
    /// </summary>
    public string FlowBreak { get; private set; } = "";

    /// <summary>
    /// cTor for an item
    /// </summary>
    public ProfileItemsStore( string name, string profile, string dispOrder, string flowBreak )
    {
      Name = name;
      Profile = profile;
      DispOrder = dispOrder;
      FlowBreak = flowBreak;
    }

    /// <summary>
    /// Show the Name as String (mainly for debug reasons)
    /// </summary>
    /// <returns>A string</returns>
    public override string ToString( ) => Name;

  }

}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FS20_HudBar.Bar.Items;
using static FS20_HudBar.Config.CProfile;

namespace FS20_HudBar.Config
{
  /// <summary>
  /// Manages the item tracking for one Config Profile
  /// (column of item checkboxes)
  /// </summary>
  internal class FlpItems
  {
    // List of Bool per Item, checked or not
    private Dictionary<LItem, bool> _profile = new Dictionary<LItem, bool>( );   // true to show the item
    // List of FlowBreaks per Item, FlowBreak enum
    private Dictionary<LItem, GUI.BreakType> _flowBreak = new Dictionary<LItem, GUI.BreakType>( ); // true to break the flow
    // List of the position per Item
    // Value is the position index of this Item in the list, position starts at 0 ..
    private Dictionary<LItem, int> _position = new Dictionary<LItem, int>( );    // display position 0...

    /// <summary>
    /// Returns the number of managed items
    /// </summary>
    public int Count => _profile.Count; // using the profile count as reference

    #region Position handling

    /// <summary>
    /// Set an Item to the position
    /// </summary>
    /// <param name="item">The Item</param>
    /// <param name="position">The valued to set</param>
    public void SetPosition( LItem item, int position )
    {
      try {
        _position[item] = position;
      }
      catch {
#if DEBUG
        throw new IndexOutOfRangeException( $"Item {item} cannot be handled" );
#endif
      }
    }

    /// <summary>
    /// Returns the Position for an item
    /// </summary>
    /// <param name="item">The Item</param>
    /// <returns>Position</returns>
    public int PositionFor( LItem item )
    {
      if (_position.TryGetValue( item, out var position )) { return position; }
#if DEBUG
      throw new IndexOutOfRangeException( $"Item {item} cannot be handled" );
#endif
#pragma warning disable CS0162 // Unreachable code detected
            return 999; // Error number, actually a Programm error
#pragma warning restore CS0162 // Unreachable code detected
        }

    /// <summary>
    /// True if the position is used by an item
    /// </summary>
    /// <param name="position">A position of an item</param>
    /// <returns>True if it is used </returns>
    public bool IsPositionUsed( int position ) => _position.ContainsValue( position );

    /// <summary>
    /// Returns the Items Key for a Display position
    /// </summary>
    /// <param name="item">A position 0..</param>
    /// <returns>The display item key</returns>
    public LItem ItemKeyFromPos( int pos )
    {
      // find the item to be shown at this position (find the pos value in m_sequence and get the item with it's Key)
      if (_position.ContainsValue( pos )) {
        return _position.FirstOrDefault( x => x.Value == pos ).Key;
      }
      else {
#if DEBUG
        throw new IndexOutOfRangeException( $"Item at position {pos} cannot be found" );
#endif
#pragma warning disable CS0162 // Unreachable code detected
                return (LItem)999;
#pragma warning restore CS0162 // Unreachable code detected
            }
    }

    /// <summary>
    /// Returns a list orderd along the item Pos, the list contains the LItem Key to be shown at this pos (0 based)
    /// Kind of Transposing the sequence list
    /// </summary>
    /// <returns></returns>
    public List<LItem> ItemPosList( )
    {
      var ret = new List<LItem>( );

      foreach (LItem i in Enum.GetValues( typeof( LItem ) )) {
        // we use the Enum only as position index 0... max here
        int idx = (int)i;
        // find the item to be shown at this position (find the pos value in m_sequence and get the item with it's Key)
        if (_position.ContainsValue( idx )) {
          // the item to put as next one / Add sequentially to the layout panel
          ret.Add( ItemKeyFromPos( idx ) );
        }
        else {
#if DEBUG
          throw new IndexOutOfRangeException( $"Item {i} cannot be handled" );
#endif
        }
      }
      return ret;
    }

    #endregion

    #region Item Checked handling

    /// <summary>
    /// Set an Item to the checkedVal
    /// </summary>
    /// <param name="item">The Item</param>
    /// <param name="checkedVal">The valued to set</param>
    public void SetChecked( LItem item, bool checkedVal )
    {
      try {
        _profile[item] = checkedVal;
      }
      catch {
#if DEBUG
        throw new IndexOutOfRangeException( $"Item {item} cannot be handled" );
#endif
      }
    }

    /// <summary>
    /// Returns the FlowBreak type for an item
    /// </summary>
    /// <param name="item">The Item</param>
    /// <returns>FlowBreak type</returns>
    public bool CheckedFor( LItem item )
    {
      if (_profile.TryGetValue( item, out var checkedVal )) { return checkedVal; }
#if DEBUG
      throw new IndexOutOfRangeException( $"Item {item} cannot be handled" );
#endif
#pragma warning disable CS0162 // Unreachable code detected
            return false;
#pragma warning restore CS0162 // Unreachable code detected
        }

    /// <summary>
    /// Create an empty profile with all items set to checkedVal
    /// </summary>
    /// <param name="checkedVal">Set value</param>
    public void CreateEmptyProfile( bool checkedVal )
    {
      foreach (LItem i in Enum.GetValues( typeof( LItem ) )) {
        _profile.Add( i, checkedVal );
      }
    }

    #endregion


    #region FlowBreak handling

    /// <summary>
    /// Set an Item to the flowBreak
    /// </summary>
    /// <param name="item">The Item</param>
    /// <param name="breakType">The valued to set</param>
    public void SetFlowBreak( LItem item, GUI.BreakType breakType )
    {
      try {
        _flowBreak[item] = breakType;
      }
      catch {
#if DEBUG
        throw new IndexOutOfRangeException( $"Item {item} cannot be handled" );
#endif
      }
    }

    /// <summary>
    /// Returns the FlowBreak type for an item
    /// </summary>
    /// <param name="item">The Item</param>
    /// <returns>FlowBreak type</returns>
    public GUI.BreakType FlowBreakFor( LItem item )
    {
      if (_flowBreak.TryGetValue( item, out var breakType )) { return breakType; }
#if DEBUG
      throw new IndexOutOfRangeException( $"Item {item} cannot be handled" );
#endif
#pragma warning disable CS0162 // Unreachable code detected
            return GUI.BreakType.None;
#pragma warning restore CS0162 // Unreachable code detected
        }

    #endregion

    #region String functions for AppSettings

    /// <summary>
    /// Returns the profile Settings string 
    /// </summary>
    /// <returns>A profile string </returns>
    public string ProfileString( )
    {
      string ret = "";

      foreach (var kv in _profile) {
        ret += (kv.Value ? "1" : "0") + Divider;
      }
      return ret;
    }

    /// <summary>
    /// Builds the item checked list from a config string
    /// </summary>
    /// <param name="fbString">Checked Profile string</param>
    public void ProfileFromString( string profileString )
    {
      // Get the visibility status (checked) for each item
      string[] e = profileString.Split( new char[] { Divider }, StringSplitOptions.RemoveEmptyEntries );
      _profile.Clear( );
      foreach (LItem i in Enum.GetValues( typeof( LItem ) )) {
        bool show = (i == LItem.MSFS) ? true : false; // default OFF except the first 20210708
        if (e.Length > (int)i) {
          show = e[(int)i] == "1"; // found an element in the string
        }
        _profile.Add( i, show );
      }
    }

    /// <summary>
    /// Returns the flowBreak Settings string 
    /// </summary>
    /// <returns>A flowBreak string </returns>
    public string FlowBreakString( )
    {
      string ret = "";

      foreach (var kv in _flowBreak) {
        ret += BreakTagFromEnum( kv.Value ).ToString( ) + Divider;
      }
      return ret;
    }

    /// <summary>
    /// Builds the item flowbreaks from a config string
    /// </summary>
    /// <param name="fbString">FlowBreak string</param>
    public void FlowBreaksFromString( string fbString )
    {
      // Get the flow break  status for each item
      string[] e = fbString.Split( new char[] { Divider }, StringSplitOptions.RemoveEmptyEntries );
      _flowBreak.Clear( );
      foreach (LItem i in Enum.GetValues( typeof( LItem ) )) {
        GUI.BreakType fbreak = GUI.BreakType.None; // default OFF
        if (e.Length > (int)i) {
          fbreak = EnumFromBreakTagString( e[(int)i] ); // found an element in the string
        }
        _flowBreak.Add( i, fbreak );
      }


    }

    /// <summary>
    /// Returns the item position Settings string 
    /// </summary>
    /// <returns>A item position string </returns>
    public string ItemPosString( )
    {
      string ret = "";

      foreach (var kv in _position) {
        ret += $"{kv.Value}" + Divider; ;
      }
      return ret;
    }

    /// <summary>
    /// Builds the item positions from a config string
    /// </summary>
    /// <param name="posString">Position string</param>
    public void PositionsFromString( string posString )
    {
      // Get the item position - don't validate the sequence here
      string[] e = posString.Split( new char[] { Divider }, StringSplitOptions.RemoveEmptyEntries );
      //m_sequence.Clear( );
      _position.Clear( );
      foreach (LItem i in Enum.GetValues( typeof( LItem ) )) {
        int idx = (int)i; // default Enum Sequence
        if (e.Length > idx) {
          if (int.TryParse( e[idx], NumberStyles.Integer, CultureInfo.InvariantCulture, out int iPos )) {
            if (iPos < this.Count) {
              _position.Add( i, iPos ); // found an integer
            }
            else {
              _position.Add( i, this.Count - 1 ); // found an integer - was out of the range ??
            }
          }
          else {
            _position.Add( i, idx ); // default
          }
        }
        else {
          _position.Add( i, idx ); // default
        }
      }
    }

    #endregion

  }
}

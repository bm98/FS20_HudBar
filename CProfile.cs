using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.Bar;

namespace FS20_HudBar
{


  /// <summary>
  /// A settings profile
  /// the checkbox values of the user settings
  /// </summary>
  class CProfile
  {
    private int m_pNumber = 0;

    private FlowLayoutPanel m_flp = null;
    private Color c_FBCol = Color.PaleGreen;

    private Dictionary<LItem, bool> m_profile = new Dictionary<LItem, bool>();   // true to show the item
    private Dictionary<LItem, bool> m_flowBreak = new Dictionary<LItem, bool>(); // true to break the flow
    private Dictionary<LItem, int> m_sequence = new Dictionary<LItem, int>();    // display position 0...

    private GUI.FontSize m_fontSize = GUI.FontSize.Regular;
    private GUI.Placement m_placement = GUI.Placement.Bottom;
    private GUI.Kind m_kind = GUI.Kind.Bar;
    private Point m_location = new Point(0,0);
    private bool m_condensed = false;

    public string PName { get; set; } = "Profile";
    public GUI.FontSize FontSize => m_fontSize;
    public GUI.Placement Placement => m_placement;
    public GUI.Kind Kind => m_kind;
    public Point Location => m_location;
    public bool Condensed => m_condensed;

    /// <summary>
    /// Update the Location of the profile
    /// </summary>
    /// <param name="location">A point</param>
    public void UpdateLocation( Point location )
    {
      m_location = location;
    }


    /// <summary>
    /// Create an empty profile with all items enabled
    /// </summary>
    public CProfile( )
    {
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        m_profile.Add( i, true );
      }
    }

    /// <summary>
    /// Create a profile from the profile string, must match, no existing items are set to true
    /// </summary>
    /// <param name="profileName">The Name of the Profile</param>
    /// <param name="profile">A semicolon separated string of 0 or 1 (shown) </param>
    /// <param name="flowBreak">A semicolon separated string of 0 or 1 (break) </param>
    /// <param name="sequence">A semicolon separated string of numbers (display position) </param>
    /// <param name="fontSize">The FontSize Number (matches the enum)</param>
    /// <param name="placement">The Placement Number (matches the enum)</param>
    /// <param name="condensed">The Condensed Font Flag</param>
    public CProfile( int pNum, string profileName,
                     string profile, string flowBreak, string sequence,
                     int fontSize, int placement, int kind, Point location, bool condensed )
    {
      m_pNumber = pNum;
      LoadProfile( profileName, profile, flowBreak, sequence,
                    (GUI.FontSize)fontSize, (GUI.Placement)placement, (GUI.Kind)kind, location, condensed );
    }

    /// <summary>
    /// The Key or Name of the CheckBox
    /// </summary>
    /// <param name="item">An Item</param>
    /// <returns>The Key/Name</returns>
    private string Key( LItem item )
    {
      return $"P{m_pNumber}_{item}";
    }

    private bool IsSamePanel( string srcKey, string destKey )
    {
      return srcKey[1] == destKey[1];
    }

    /// <summary>
    /// Load a profile Listbox with checked items from this profile
    /// </summary>
    /// <param name="box">The CListBox</param>
    /// <param name="hudBar">The HudBar</param>
    public void LoadCbx( CheckedListBox box, HudBar hudBar )
    {
      box.Items.Clear( );

      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        int idx = box.Items.Add( hudBar.CfgName( i ) );
        box.SetItemChecked( idx, m_profile[i] );
      }
    }


    /// <summary>
    /// Update this profile from the checked listbox
    /// </summary>
    /// <param name="box">The CListBox</param>
    public void GetItemsFromCbx( CheckedListBox box )
    {
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        m_profile[i] = box.GetItemChecked( (int)i );
      }
    }

    /// <summary>
    /// Load a profile FlowLayoutPanel with checked items from this profile
    /// </summary>
    /// <param name="flp">The FLPanel</param>
    /// <param name="hudBar">The HudBar</param>
    public void LoadFlp( FlowLayoutPanel flp, HudBar hudBar )
    {
      m_flp = flp;
      flp.Controls.Clear( );
      List<CheckBox> tmp = new  List<CheckBox>(); // temp list of checkboxes in native sequence (not yet user ordered)
      // Fill a temp panel
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        var cb = new CheckBox(){
          Name =Key(i),
          Text = hudBar.CfgName( i ),
          Tag = m_flowBreak[i] ? "1" : "", // use TAG as string 0 / 1
          Margin = new Padding(3,0,3,0),
          Anchor = AnchorStyles.Left,
          AutoSize = true,
          AllowDrop = true,
          Enabled = i!= LItem.MSFS, // Disable unckecking of MSFS Status
         // Cursor = Cursors.NoMoveVert,
        };
        cb.Checked = m_profile[i];
        cb.BackColor = m_flowBreak[i] ? c_FBCol : flp.BackColor; // FBs have a different BGCol
        cb.MouseDown += Cb_MouseDown;
        cb.MouseUp += Cb_MouseUp;
        cb.MouseMove += Cb_MouseMove;
        cb.DragOver += Cb_DragOver;
        cb.DragDrop += Cb_DragDrop;
        cb.GiveFeedback += Cb_GiveFeedback;
        tmp.Add( cb );
      }

      // now load the real panel accordingly from display position 0...
      flp.SuspendLayout( ); // avoid performance issued while loading all checkboxes

      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        // we use the Enum only as position index 0... max here
        int idx = (int)i;
        // find the item to be shown at this position (find the pos value in m_sequence and get the item with it's Key)
        if ( m_sequence.ContainsValue( idx ) ) {
          // the item to put as next one / Add sequentially to the layout panel
          flp.Controls.Add( tmp.ElementAt( (int)ItemKeyFromPos( idx ) ) );
        }
        else {
          // no such item ????
          ;
        }
      }
      // only now layout the FLPanel
      flp.ResumeLayout( );
    }


    #region Mouse + Drag and Drop Handling
    // DD vars
    private Rectangle dragBoxFromMouseDown;
    private string dragSourceKey = "";
    private string dragDropKey = "";

    // fired for a Drop destination
    //  final outcome depends on the validity of the move
    private void Cb_DragDrop( object sender, DragEventArgs e )
    {
      var dropDest = sender as CheckBox;
      // Ensure that the dropped item is at least a CheckBox item
      if ( e.Data.GetDataPresent( typeof( CheckBox ) ) ) {
        if ( e.Effect == DragDropEffects.Move ) {
          // only handle moves, ignore when source == dest
          if ( dragSourceKey != dropDest.Name ) {
            // Move the item by setting the index of the source to the destination
            m_flp.Controls.SetChildIndex( m_flp.Controls[dragSourceKey], m_flp.Controls.IndexOfKey( dropDest.Name ) );
          }
        }
      }
      dragSourceKey = "";
      dragDropKey = "";
      // Reset the drag rectangle when the mouse button is raised.
      dragBoxFromMouseDown = Rectangle.Empty;
    }

    // fired whenever a move over a drop destination is detected
    private void Cb_DragOver( object sender, DragEventArgs e )
    {
      var dropDest = sender as CheckBox;
      if ( !e.Data.GetDataPresent( typeof( CheckBox ) ) ) {
        e.Effect = DragDropEffects.None; // source is not a checkbox
        return;
      }
      if ( !IsSamePanel( dragSourceKey, dropDest.Name ) ) {
        e.Effect = DragDropEffects.None; // attempt to drop to other panel
        return;
      }
      // Finally a valid drop zone..
      dragDropKey = dropDest.Name;
      e.Effect = DragDropEffects.Move;

      // scroll outside items into view
      var srcIdx = m_flp.Controls.GetChildIndex(  m_flp.Controls[dragSourceKey] );
      var dstIdx = m_flp.Controls.GetChildIndex(  dropDest );
      if ( ( dstIdx < srcIdx ) && ( dstIdx > 0 ) ) {
        // going up and not topmost
        m_flp.ScrollControlIntoView( m_flp.Controls[dstIdx - 1] );
      }
      else if ( ( dstIdx > srcIdx ) && ( dstIdx < ( m_flp.Controls.Count - 1 ) ) ) {
        // going down and not last
        m_flp.ScrollControlIntoView( m_flp.Controls[dstIdx + 1] );
      }

    }

    private void Cb_GiveFeedback( object sender, GiveFeedbackEventArgs e )
    {
      var cb = sender as CheckBox;
      if ( cb.Name == dragSourceKey ) {
        e.UseDefaultCursors = false;
        if ( ( e.Effect & DragDropEffects.Move ) == DragDropEffects.Move ) {
          Cursor.Current = Cursors.NoMoveVert;
        }
        else {
          Cursor.Current = m_flp.Cursor;
        }
      }
    }


    // Handle the Mouse Down Event (we don't get right clicks from a CheckBox)
    private void Cb_MouseDown( object sender, MouseEventArgs e )
    {
      var cb = sender as CheckBox;
      // RIGHT Button - Toggle the FlowBreak when the RIGHT button is down
      if ( e.Button == MouseButtons.Right ) {
        if ( (string)cb.Tag == "1" ) {
          // remove FB
          cb.BackColor = m_flp.BackColor;
          cb.Tag = "";
        }
        else {
          // add FB
          cb.BackColor = Color.PaleGreen;
          cb.Tag = "1"; // FlowBreak 
        }
      }
      // LEFT Button - Start to MOVE a CheckBox
      else if ( e.Button == MouseButtons.Left ) {
        // cannot move MSFS Status which remains the first item in the FLPanel all the time
        if ( cb.Name == m_flp.Controls[0].Name ) {
          dragSourceKey = "";
          dragBoxFromMouseDown = Rectangle.Empty;
        }
        else {
          dragSourceKey = cb.Name;
          // Remember the point where the mouse down occurred. The DragSize indicates
          // the size that the mouse can move before a drag event should be started.
          Size dragSize = SystemInformation.DragSize;

          // Create a rectangle using the DragSize, with the mouse position being
          // at the center of the rectangle.
          dragBoxFromMouseDown = new Rectangle(
              new Point( e.X - ( dragSize.Width / 2 ),
                        e.Y - ( dragSize.Height / 2 ) ),
              dragSize );
        }
      }
    }

    // fired on release 
    private void Cb_MouseUp( object sender, MouseEventArgs e )
    {
      var cb = sender as CheckBox;

      if ( cb.Name == dragSourceKey ) {
        // released over the source item
        dragSourceKey = "";
        // Reset the drag rectangle when the mouse button is raised.
        dragBoxFromMouseDown = Rectangle.Empty;
      }
      // DragDrop is handled above
    }

    // fired on mouse movement
    //  check if we need to visualize the re-arrange of items
    private void Cb_MouseMove( object sender, MouseEventArgs e )
    {
      var cb = sender as CheckBox;
      if ( cb.Name == dragSourceKey ) {
        // moved within the originating Control
        // If the mouse moves outside the rectangle, start the drag.
        if ( dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains( e.X, e.Y ) ) {
          // start DD Action for the calling item as Move
          cb.DoDragDrop( cb, DragDropEffects.Move | DragDropEffects.Scroll );
        }
      }
      // moves across other items are handled as DD_Over above
    }

    #endregion


    /// <summary>
    /// Update this profile from the FLPanel
    /// </summary>
    /// <param name="flp">The FLPanel</param>
    public void GetItemsFromFlp( FlowLayoutPanel flp )
    {
      // process along the Enum sequence
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        // find the item with its Key in the layout panel - no checks - may break if not consistent
        try {
          var cb = flp.Controls[Key(i)] as CheckBox;
          int pos = flp.Controls.IndexOf(cb); // can be -1 if not found
          if ( pos >= 0 ) {
            // store along the Enum sequence
            m_sequence[i] = pos;
            m_profile[i] = cb.Checked;
            m_flowBreak[i] = (string)cb.Tag == "1";
          }
        }
        catch {
          m_sequence[i] = (int)i; m_profile[i] = true; m_flowBreak[i] = false; // dummys anyway - DEBUG STOP
        }
      }
    }



    /// <summary>
    /// Make the current FontSize the selected one
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void LoadFontSize( ComboBox box )
    {
      box.SelectedIndex = (int)m_fontSize;
    }

    /// <summary>
    /// Update this profile from the FontSize ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetFontSizeFromCombo( ComboBox box )
    {
      m_fontSize = (GUI.FontSize)box.SelectedIndex;
    }

    /// <summary>
    /// Make the current Placement the selected one
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void LoadPlacement( ComboBox box )
    {
      box.SelectedIndex = (int)m_placement;
    }

    /// <summary>
    /// Make the current Kind the selected one
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void LoadKind( ComboBox box )
    {
      box.SelectedIndex = (int)m_kind;
    }

    /// <summary>
    /// Make the current Condensed the selected one
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void LoadCond( ComboBox box )
    {
      box.SelectedIndex = m_condensed ? 1 : 0;
    }

    /// <summary>
    /// Update this profile from the Placement ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetPlacementFromCombo( ComboBox box )
    {
      m_placement = (GUI.Placement)box.SelectedIndex;
    }

    /// <summary>
    /// Update this profile from the Display Kind ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetKindFromCombo( ComboBox box )
    {
      m_kind = (GUI.Kind)box.SelectedIndex;
    }

    /// <summary>
    /// Update this profile from the Condensed ComboBox
    /// </summary>
    /// <param name="box">The ComboBox</param>
    public void GetCondFromCombo( ComboBox box )
    {
      m_condensed = box.SelectedIndex == 1;
    }


    // Load the profile from stored strings (Settings)
    private void LoadProfile( string profileName, string profile, string flowBreak, string sequence,
                                GUI.FontSize fontSize, GUI.Placement placement, GUI.Kind kind, Point location, bool condensed )
    {
      // save props in Profile
      PName = profileName;
      m_fontSize = fontSize;
      m_placement = placement;
      m_kind = kind;
      m_location = location;
      m_condensed = condensed;

      // The Dictionaries and stored strings of it maintain the Enum Sequence
      //   even if items are relocated for display
      // If a stored string does not contain a value (too short) then a default is taken
      // i.e. we always want a complete Dictionary when leaving the method!!!

      // Get the visibility status for each item
      string[] e = profile.Split(new char[]{ ';' }, StringSplitOptions.RemoveEmptyEntries );
      m_profile.Clear( );
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        bool show = (i== LItem.MSFS)?true : false; // default OFF except the first 20210708
        if ( e.Length > (int)i ) {
          show = e[(int)i] == "1"; // found an element in the string
        }
        m_profile.Add( i, show );
      }

      // Get the flow break  status for each item
      e = flowBreak.Split( new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries );
      m_flowBreak.Clear( );
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        bool fbreak = false; // default OFF
        if ( e.Length > (int)i ) {
          fbreak = e[(int)i] == "1"; // found an element in the string
        }
        m_flowBreak.Add( i, fbreak );
      }

      // Get the item position - don't validate the sequence here
      e = sequence.Split( new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries );
      m_sequence.Clear( );
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        int idx = (int)i; // default Enum Sequence
        if ( e.Length > idx ) {
          if ( int.TryParse( e[idx], out int iPos ) ) {
            if ( iPos < m_profile.Count )
              m_sequence.Add( i, iPos ); // found an integer
            else
              m_sequence.Add( i, m_profile.Count-1 ); // found an integer - was out of the range ??
          }
          else {
            m_sequence.Add( i, idx ); // default
          }
        }
        else {
          m_sequence.Add( i, idx ); // default
        }
      }
    }


    /// <summary>
    /// Returns the Show state of an item
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>True if it is shown</returns>
    public bool ShowItem( LItem item )
    {
      try {
        return m_profile[item];
      }
      catch {
        return false;
      }
    }

    /// <summary>
    /// Returns the FlowBreak state of an item
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>True if it breaks</returns>
    public bool BreakItem( LItem item )
    {
      try {
        return m_flowBreak[item];
      }
      catch {
        return false;
      }
    }

    /// <summary>
    /// Returns the Items Display position
    /// </summary>
    /// <param name="item">An item</param>
    /// <returns>The display position 0..</returns>
    public int ItemPos( LItem item )
    {
      try {
        return m_sequence[item];
      }
      catch {
        return 999;
      }
    }

    /// <summary>
    /// Returns the Items Key for a Display position
    /// </summary>
    /// <param name="item">A position 0..</param>
    /// <returns>The display item key</returns>
    public LItem ItemKeyFromPos( int pos )
    {
      // find the item to be shown at this position (find the pos value in m_sequence and get the item with it's Key)
      if ( m_sequence.ContainsValue( pos ) ) {
        return m_sequence.Where( x => x.Value == pos ).FirstOrDefault( ).Key;
      }
      else {
        // no such item ????
        return (LItem)999;
      }
    }

    /// <summary>
    /// Returns the profile Settings string 
    /// </summary>
    /// <returns>A profile string </returns>
    public string ProfileString( )
    {
      string ret="";

      foreach ( var kv in m_profile ) {
        ret += kv.Value ? "1;" : "0;";
      }
      return ret;
    }

    /// <summary>
    /// Returns the flowBreak Settings string 
    /// </summary>
    /// <returns>A flowBreak string </returns>
    public string FlowBreakString( )
    {
      string ret="";

      foreach ( var kv in m_flowBreak ) {
        ret += kv.Value ? "1;" : "0;";
      }
      return ret;
    }

    /// <summary>
    /// Returns the item position Settings string 
    /// </summary>
    /// <returns>A item position string </returns>
    public string ItemPosString( )
    {
      string ret="";

      foreach ( var kv in m_sequence ) {
        ret += $"{kv.Value};";
      }
      return ret;
    }

    /// <summary>
    /// Returns a list orderd along the item Pos, the list contains the LItem Key to be shown at this pos (0 based)
    /// Kind of Transposing the sequence list
    /// </summary>
    /// <returns></returns>
    public List<LItem> ItemPosList( )
    {
      var ret = new List<LItem>();

      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        // we use the Enum only as position index 0... max here
        int idx = (int)i;
        // find the item to be shown at this position (find the pos value in m_sequence and get the item with it's Key)
        if ( m_sequence.ContainsValue( idx ) ) {
          // the item to put as next one / Add sequentially to the layout panel
          ret.Add( ItemKeyFromPos( idx ) );
        }
        else {
          // no such item ????
          ;
        }
      }
      return ret;
    }


  }
}

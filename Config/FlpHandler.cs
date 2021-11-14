using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FS20_HudBar.Bar;
using FS20_HudBar.Bar.Items;
using static FS20_HudBar.CProfile;

namespace FS20_HudBar.Config
{

  /// <summary>
  /// Handles the FlowLayout Panel
  /// </summary>
  class FlpHandler
  {
    private int m_pNumber = 0; // Profile Number 1...

    private FlowLayoutPanel m_flp = null;

    private Dictionary<LItem, bool> m_profile = new Dictionary<LItem, bool>();   // true to show the item
    private Dictionary<LItem, bool> m_flowBreak = new Dictionary<LItem, bool>(); // true to break the flow
    private Dictionary<LItem, int> m_sequence = new Dictionary<LItem, int>();    // display position 0...

    /// <summary>
    /// Create 
    /// </summary>
    /// <param name="flp">The FlowLayoutPanel to handle</param>
    /// <param name="pNum">Profile Number 1...</param>
    /// <param name="profile">A semicolon separated string of 0 or 1 (shown) </param>
    /// <param name="flowBreak">A semicolon separated string of 0 or 1 (break) </param>
    /// <param name="sequence">A semicolon separated string of numbers (display position) </param>
    public FlpHandler(FlowLayoutPanel flp, int pNum, string profile, string flowBreak, string sequence )
    {
      m_pNumber = pNum;
      m_flp = flp;
      LoadProfile( profile, flowBreak, sequence);
    }

    /// <summary>
    /// The Key or Name of the CheckBox
    /// </summary>
    /// <param name="item">An Item</param>
    /// <returns>The Key/Name</returns>
    private static string Key( int pNum, LItem item )
    {
      return $"P{pNum}_{item}";
    }
    /// <summary>
    /// The Key or Name of the CheckBox
    /// </summary>
    /// <param name="item">An Item</param>
    /// <returns>The Key/Name</returns>
    private string Key( LItem item )
    {
      return Key( m_pNumber, item );
    }

    private bool IsSamePanel( string srcKey, string destKey )
    {
      return srcKey[1] == destKey[1];
    }

    /// <summary>
    /// Load a profile FlowLayoutPanel with checked items from this profile
    /// </summary>
    /// <param name="flp">The FLPanel</param>
    /// <param name="hudBar">The HudBar</param>
    public void LoadFlp( HudBar hudBar )
    {
      m_flp.Controls.Clear( );
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
        cb.BackColor = m_flowBreak[i] ? GUI.GUI_Colors.c_FBCol : m_flp.BackColor; // FBs have a different BGCol
        // Mouse Handlers
        cb.MouseDown += Cb_MouseDown;
        cb.MouseUp += Cb_MouseUp;
        cb.MouseMove += Cb_MouseMove;
        cb.DragOver += Cb_DragOver;
        cb.DragDrop += Cb_DragDrop;
        cb.GiveFeedback += Cb_GiveFeedback;
        tmp.Add( cb );
      }

      // now load the real panel accordingly from display position 0...
      m_flp.SuspendLayout( ); // avoid performance issued while loading all checkboxes

      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        // we use the Enum only as position index 0... max here
        int idx = (int)i;
        // find the item to be shown at this position (find the pos value in m_sequence and get the item with it's Key)
        if ( m_sequence.ContainsValue( idx ) ) {
          // the item to put as next one / Add sequentially to the layout panel
          m_flp.Controls.Add( tmp.ElementAt( (int)ItemKeyFromPos( idx ) ) );
        }
        else {
          // no such item ????
          ;
        }
      }
      // only now layout the FLPanel
      m_flp.ResumeLayout( );
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
    /// Get the items from current FLP
    /// </summary>
    /// <param name="pNum"></param>
    /// <param name="flp"></param>
    /// <returns></returns>
    public DefaultProfile GetItemsFromFlp( )
    {
      string profile = "";
      string flowBreak = "";
      string sequence = "";

      // process along the Enum sequence
      foreach ( LItem i in Enum.GetValues( typeof( LItem ) ) ) {
        // find the item with its Key in the layout panel - no checks - may break if not consistent
        try {
          var cb = m_flp.Controls[Key(i)] as CheckBox;
          int pos = m_flp.Controls.IndexOf(cb); // can be -1 if not found
          if ( pos >= 0 ) {
            // store along the Enum sequence
            sequence += $"{pos};";
            profile += cb.Checked ? "1;" : "0;";
            flowBreak += ( (string)cb.Tag == "1" ) ? "1;" : "0;";
          }
        }
        catch {
          sequence += $"{i};";
          profile += "0;";
          flowBreak += "0;";
        }
      }
      return new DefaultProfile( "COPY", profile, sequence, flowBreak );
    }

    /// <summary>
    /// Load and overwrite items from a default profile
    /// </summary>
    /// <param name="defaultProfile">The default profile ID</param>
    public void LoadDefaultProfile( DProfile defaultProfile )
    {
      var dp = GetDefaultProfile(defaultProfile);
      if ( dp == null ) return; // sanity, defaultProfile does not exist

      LoadDefaultProfile( dp ); // use default Bar props
    }

    /// <summary>
    /// Load and overwrite items from a default profile
    /// </summary>
    /// <param name="defaultProfile">The default profile object</param>
    public void LoadDefaultProfile( DefaultProfile defaultProfile )
    {
      if ( defaultProfile == null ) return; // sanity, defaultProfile does not exist

      LoadProfile( defaultProfile.Profile, defaultProfile.FlowBreak, defaultProfile.DispOrder ); // use default Bar props
    }

    // Load the profile from stored strings (Settings)
    private void LoadProfile( string profile, string flowBreak, string sequence )
    {
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
              m_sequence.Add( i, m_profile.Count - 1 ); // found an integer - was out of the range ??
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

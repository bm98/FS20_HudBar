using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.Bar;
using FS20_HudBar.Bar.Items;

using static FS20_HudBar.Config.CProfile;

namespace FS20_HudBar.Config
{

  /// <summary>
  /// Handles the FlowLayout Panel
  /// </summary>
  internal class FlpHandler : IDisposable
  {
    // Profile Index (GUI Column) 0...
    private int m_pIndex = 0;
    // The managed FlowLayoutPanel
    private FlowLayoutPanel m_flpRef = null;
    // the items handler
    private FlpItems m_items = new FlpItems( );

    /// <summary>
    /// Create 
    /// </summary>
    /// <param name="flpRef">The FlowLayoutPanel to handle</param>
    /// <param name="pIndex">Visible Profile Index 0...</param>
    /// <param name="profile">A semicolon separated string of 0 or 1 (shown) </param>
    /// <param name="flowBreak">A semicolon separated string of 0 or 1 (break) </param>
    /// <param name="sequence">A semicolon separated string of numbers (display position) </param>
    public FlpHandler( FlowLayoutPanel flpRef, int pIndex, string profile, string flowBreak, string sequence )
    {
      m_pIndex = pIndex;
      m_flpRef = flpRef;
      LoadProfile( profile, flowBreak, sequence );
    }

    /// <summary>
    /// The Key or Name of the CheckBox
    /// Controls need a unique name (key) within a Frame context
    /// </summary>
    /// <param name="pIndex">The profile column index</param>
    /// <param name="item">An Item</param>
    /// <returns>The Key/Name</returns>
    public static string Key( int pIndex, LItem item )
    {
      return $"P{pIndex}_{item}";
    }

    /// <summary>
    /// The Key or Name of this FlowPanel CheckBoxes
    /// </summary>
    /// <param name="item">An Item</param>
    /// <returns>The Key/Name</returns>
    private string Key( LItem item )
    {
      return Key( m_pIndex, item );
    }

    // Load the profile from stored strings (Settings)
    private void LoadProfile( string profile, string flowBreak, string sequence )
    {
      // The Dictionaries and stored strings of it maintain the Enum Sequence
      //   even if items are relocated for display
      // If a stored string does not contain a value (too short) then a default is taken
      // i.e. we always want a complete Dictionary when leaving the method!!!

      // Get the visibility status for each item
      m_items.ProfileFromString( profile );
      // Get the flow break  status for each item
      m_items.FlowBreaksFromString( flowBreak );
      // Get the item position - don't validate the sequence here
      m_items.PositionsFromString( sequence );
    }

    /// <summary>
    /// Load a profile FlowLayoutPanel with checked items from this profile
    /// </summary>
    /// <param name="hudBarRef">The HudBar</param>
    public void LoadFlp( HudBar hudBarRef )
    {
      List<CheckBox> tmpItemList = new List<CheckBox>( ); // temp list of checkboxes in native sequence (not yet user ordered)
      // Fill a temporary list where items are added along their Key appearance
      // later these items will be used to populate the FlowPanel
      foreach (LItem i in Enum.GetValues( typeof( LItem ) )) {
        var cb = new CheckBox {
          Name = Key( i ),
          Text = hudBarRef.CfgName( i ),
          Tag = BreakTagFromEnum( m_items.FlowBreakFor( i ) ),
          Margin = new Padding( 3, 0, 3, 0 ),
          Anchor = AnchorStyles.Left,
          AutoSize = true,
          AllowDrop = true,
          Enabled = i != LItem.MSFS, // Disable unckecking of MSFS Status
          Checked = m_items.CheckedFor( i ),
          Visible = !BarItems.IsINOP( i ), // hide INOPs
          BackColor = BreakColorFromEnum( m_items.FlowBreakFor( i ) ),
          ContextMenu = null, // avoid context menues for items
        };
        // Mouse Handlers
        cb.MouseEnter += Cb_MouseEnter;
        cb.MouseLeave += Cb_MouseLeave;
        cb.MouseDown += Cb_MouseDown;
        cb.MouseUp += Cb_MouseUp;
        cb.MouseMove += Cb_MouseMove;
        cb.DragOver += Cb_DragOver;
        cb.DragDrop += Cb_DragDrop;
        cb.GiveFeedback += Cb_GiveFeedback;
        tmpItemList.Add( cb );
      }

      ClearOldControls( );
      // now load the real panel accordingly from display position 0...
      // taking the items from the list above and put them in the used defined order
      m_flpRef.SuspendLayout( ); // avoid performance issued while loading all checkboxes

      foreach (LItem i in Enum.GetValues( typeof( LItem ) )) {
        // we use the Enum only as position 0... max here

        int position = (int)i;
        // find the item to be shown at this position
        if (m_items.IsPositionUsed( position )) {
          // the item to put as next one / Add sequentially to the layout panel
          // the item is at its (int)key position in the temp list 
          LItem item = m_items.ItemKeyFromPos( position );
          m_flpRef.Controls.Add( tmpItemList.ElementAt( (int)item ) );
        }
        else {
          // no such item ????
#if DEBUG
          throw new IndexOutOfRangeException( $"Item for position {position} cannot be found" );
#endif
        }
      }
      // only now layout the FLPanel
      m_flpRef.ResumeLayout( );
    }

    private void ClearOldControls( )
    {
      m_flpRef.SuspendLayout( );
      while (m_flpRef.Controls.Count > 0) {
        var c = m_flpRef.Controls[0];
        m_flpRef.Controls.Remove( c );
        c.Dispose( );
      }
      m_flpRef.ResumeLayout( );
    }


    /// <summary>
    /// Updates the Info object from the Flow Panel checkboxes
    /// </summary>
    /// <param name="flpRef">The FlowLayoutPanel to read from</param>
    /// <param name="flpIndex">The Index of this panel</param>
    /// <param name="itemsRef">The target Items store</param>
    public static void UpdateFromFlp( FlowLayoutPanel flpRef, int flpIndex, FlpItems itemsRef )
    {
      // process along the Enum sequence
      foreach (LItem i in Enum.GetValues( typeof( LItem ) )) {
        // find the item with its Key in the layout panel - no checks - may break if not consistent
        try {
          var cb = flpRef.Controls[Key( flpIndex, i )] as CheckBox; // as Controls need a unique name we use the Key() function
          int pos = flpRef.Controls.IndexOf( cb ); // can be -1 if not found
          if (pos >= 0) {
            // store along the Enum sequence
            itemsRef.SetPosition( i, pos );
            itemsRef.SetChecked( i, cb.Checked );
            itemsRef.SetFlowBreak( i, EnumFromBreakTag( (char)cb.Tag ) );
          }
        }
        catch {
          // dummys anyway - DEBUG STOP
          itemsRef.SetPosition( i, (int)i );
          itemsRef.SetChecked( i, true );
          itemsRef.SetFlowBreak( i, GUI.BreakType.None );
#if DEBUG
          throw new IndexOutOfRangeException( $"Item {i} cannot be handled" );
#endif
        }
      }
    }

    /// <summary>
    /// Updates the Info object from this FlowPanel checkboxes
    /// (moving things around with Drag/Drop will not track it)
    /// </summary>
    private void UpdateFromFlp( )
    {
      UpdateFromFlp( m_flpRef, m_pIndex, m_items );
    }


    /// <summary>
    /// Get the items from current FLP and return them as DefaultProfile obj
    /// </summary>
    /// <returns>A filled DefaultProfile</returns>
    public ProfileItemsStore GetProfileStoreFromFlp( )
    {
      UpdateFromFlp( );
      return new ProfileItemsStore( "COPY", m_items.ProfileString( ), m_items.ItemPosString( ), m_items.FlowBreakString( ) );
    }

    /// <summary>
    /// Load and overwrite items from a default profile
    /// </summary>
    /// <param name="defaultProfile">The default profile ID</param>
    public void LoadDefaultProfile( DProfile defaultProfile )
    {
      var dp = DefaultProfiles.GetDefaultProfile( defaultProfile );
      if (dp == null) return; // sanity, defaultProfile does not exist

      LoadDefaultProfile( dp ); // use default Bar props
    }

    /// <summary>
    /// Load and overwrite items from a default profile
    /// </summary>
    /// <param name="defaultProfile">The default profile object</param>
    public void LoadDefaultProfile( ProfileItemsStore defaultProfile )
    {
      if (defaultProfile == null) return; // sanity, defaultProfile does not exist

      LoadProfile( defaultProfile.Profile, defaultProfile.FlowBreak, defaultProfile.DispOrder ); // use default Bar props
    }

    /// <summary>
    /// Merges a new profile with the existing one
    ///  Rule: 1= forced, 0= forced, {any other character}= copy from current profile
    /// </summary>
    /// <param name="profile">A merge profile string</param>
    public void MergeProfile( string profile )
    {
      string thisProfile = this.ProfileString( );
      string mergedProfile = "";
      // scan the source and merge according to the rules, copy Semicolons as divider
      for (int i = 0; i < thisProfile.Length; i++) {
        char thisChar = thisProfile[i];
        // check the input only if there are chars..
        if (i < profile.Length) {
          if (thisProfile[i] == Divider) mergedProfile += Divider; // maintain the source dividers in all cases
          else if (profile[i] == '0') mergedProfile += '0'; // merge from input
          else if (profile[i] == '1') mergedProfile += '1'; // merge from input
          else mergedProfile += thisChar; // use current profile value
        }
        else {
          //no more chars in the input
          mergedProfile += thisChar; // use current profile value
        }
      }
      // Load from merged sources
      LoadProfile( mergedProfile, this.FlowBreakString( ), this.ItemPosString( ) );
    }

    /// <summary>
    /// Relocates the items so all used items will put to top and unused ones below
    /// additionally the unused ones are ordered alphabethically
    /// The sequence of the used one remains intact
    /// </summary>
    /// <param name="hudBarRef">The HudBar obj ref</param>
    public void ReOrderProfile( HudBar hudBarRef )
    {
      // make sure we have the current setting (should never be needed...)
      UpdateFromFlp( );
      // collect the unused ones
      var usedItems = new List<LItem>( );
      var unUsedItems = new Dictionary<LItem, string>( );
      // process along the position
      for (int i = 0; i < m_items.Count; i++) {
        LItem item = m_items.ItemKeyFromPos( i );
        if (m_items.CheckedFor( item ) || (m_items.FlowBreakFor( item ) != GUI.BreakType.None))
          usedItems.Add( item );
        else
          unUsedItems.Add( item, hudBarRef.CfgName( item ) ); // get the item description to sort them later
      }
      // used ones get on top
      int position = 0;
      foreach (var i in usedItems) {
        m_items.SetPosition( i, position++ );
      }
      // then the unused ones - sort them along the Descriptive text and then insert below
      var sortedItems = unUsedItems.OrderBy( x => x.Value ).ToList( );
      foreach (var kv in sortedItems) {
        m_items.SetPosition( kv.Key, position++ );
      }
      // changes are made in place - ready to be pushed back to the FLP by the caller
    }

    /// <summary>
    /// Returns the profile Settings string 
    /// </summary>
    /// <returns>A profile string </returns>
    public string ProfileString( ) => m_items.ProfileString( );

    /// <summary>
    /// Returns the flowBreak Settings string 
    /// </summary>
    /// <returns>A flowBreak string </returns>
    public string FlowBreakString( ) => m_items.FlowBreakString( );

    /// <summary>
    /// Returns the item position Settings string 
    /// </summary>
    /// <returns>A item position string </returns>
    public string ItemPosString( ) => m_items.ItemPosString( );

    /// <summary>
    /// Returns a list orderd along the item Pos, the list contains the LItem Key to be shown at this pos (0 based)
    /// Kind of Transposing the sequence list
    /// </summary>
    /// <returns></returns>
    public List<LItem> ItemPosList( ) => m_items.ItemPosList( );


    #region Mouse + Drag and Drop Handling

    /// <summary>
    /// True when the mouse is over an item
    /// </summary>
    public bool MouseOverItem => _overItem;

    private bool _overItem = false;
    // DD vars
    private Rectangle dragBoxFromMouseDown;
    private string dragSourceKey = "";
    private string dragDropKey = "";


    // fired for a Drop destination
    //  final outcome depends on the validity of the move
    private void Cb_DragDrop( object sender, DragEventArgs e )
    {
      // sanity
      if (!(sender is CheckBox)) return;

      var dropDest = sender as CheckBox;
      // Ensure that the dropped item is at least a CheckBox item
      if (e.Data.GetDataPresent( typeof( CheckBox ) )) {
        if (e.Effect == DragDropEffects.Move) {
          // only handle moves, ignore when source == dest
          if (dragSourceKey != dropDest.Name) {
            // Move the item by setting the index of the source to the destination
            m_flpRef.Controls.SetChildIndex( m_flpRef.Controls[dragSourceKey], m_flpRef.Controls.IndexOfKey( dropDest.Name ) );
            // update Info when something changed
            UpdateFromFlp( );
          }
        }
      }
      dragSourceKey = "";
      dragDropKey = "";
      // Reset the drag rectangle when the mouse button is raised.
      dragBoxFromMouseDown = Rectangle.Empty;
    }

    // support for drop op
    private bool IsSamePanel( string srcKey, string destKey ) => srcKey[1] == destKey[1];

    // fired whenever a move over a drop destination is detected
    private void Cb_DragOver( object sender, DragEventArgs e )
    {
      // sanity
      if (!(sender is CheckBox)) return;

      var dropDest = sender as CheckBox;
      if (!e.Data.GetDataPresent( typeof( CheckBox ) )) {
        e.Effect = DragDropEffects.None; // source is not a checkbox
        return;
      }
      if (!IsSamePanel( dragSourceKey, dropDest.Name )) {
        e.Effect = DragDropEffects.None; // attempt to drop to other panel
        return;
      }
      // Finally a valid drop zone..
      dragDropKey = dropDest.Name;
      e.Effect = DragDropEffects.Move;

      // scroll outside items into view
      var srcIdx = m_flpRef.Controls.GetChildIndex( m_flpRef.Controls[dragSourceKey] );
      var dstIdx = m_flpRef.Controls.GetChildIndex( dropDest );
      if ((dstIdx < srcIdx) && (dstIdx > 0)) {
        // going up and not topmost
        m_flpRef.ScrollControlIntoView( m_flpRef.Controls[dstIdx - 1] );
      }
      else if ((dstIdx > srcIdx) && (dstIdx < (m_flpRef.Controls.Count - 1))) {
        // going down and not last
        m_flpRef.ScrollControlIntoView( m_flpRef.Controls[dstIdx + 1] );
      }
    }

    private void Cb_GiveFeedback( object sender, GiveFeedbackEventArgs e )
    {
      // sanity
      if (!(sender is CheckBox)) return;

      var cb = sender as CheckBox;
      if (cb.Name == dragSourceKey) {
        e.UseDefaultCursors = false;
        if ((e.Effect & DragDropEffects.Move) == DragDropEffects.Move) {
          Cursor.Current = Cursors.NoMoveVert;
        }
        else {
          Cursor.Current = m_flpRef.Cursor;
        }
      }
    }

    // Handle the Mouse Down Event (we don't get right clicks from a CheckBox)
    private void Cb_MouseDown( object sender, MouseEventArgs e )
    {
      // sanity
      if (!(sender is CheckBox)) return;

      var cb = sender as CheckBox;
      // RIGHT Button - Cycle the FlowBreak/DivBreak/None when the RIGHT button is down
      if ((e.Button & MouseButtons.Right) > 0) {
        if ((char)cb.Tag == FlowBreakTag) {
          // change to DB1
          cb.BackColor = GUI.GUI_Colors.c_DB1Col;
          cb.Tag = DivBreakTag1;
        }
        else if ((char)cb.Tag == DivBreakTag1) {
          // change to DB2
          cb.BackColor = GUI.GUI_Colors.c_DB2Col;
          cb.Tag = DivBreakTag2;
        }
        else if ((char)cb.Tag == DivBreakTag2) {
          // change to none
          cb.BackColor = GUI.GUI_Colors.c_NBCol; // m_flp.BackColor;
          cb.Tag = NoBreakTag;
        }
        else {
          // change to FB
          cb.BackColor = GUI.GUI_Colors.c_FBCol;
          cb.Tag = FlowBreakTag;
        }
      }
      // LEFT Button - Start to MOVE a CheckBox
      else if ((e.Button & MouseButtons.Left) > 0) {
        // cannot move MSFS Status which remains the first item in the FLPanel all the time
        if (cb.Name == m_flpRef.Controls[0].Name) {
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
              new Point( e.X - (dragSize.Width / 2),
                        e.Y - (dragSize.Height / 2) ),
              dragSize );
        }
      }
    }

    // fired on release 
    private void Cb_MouseUp( object sender, MouseEventArgs e )
    {
      // sanity
      if (!(sender is CheckBox)) return;

      var cb = sender as CheckBox;
      if (cb.Name == dragSourceKey) {
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
      // sanity
      if (!(sender is CheckBox)) return;

      var cb = sender as CheckBox;
      if (cb.Name == dragSourceKey) {
        // moved within the originating Control
        // If the mouse moves outside the rectangle, start the drag.
        if (dragBoxFromMouseDown != Rectangle.Empty && !dragBoxFromMouseDown.Contains( e.X, e.Y )) {
          // start DD Action for the calling item as Move
          cb.DoDragDrop( cb, DragDropEffects.Move | DragDropEffects.Scroll );
        }
      }
      // moves across other items are handled as DD_Over above
    }

    private void Cb_MouseLeave( object sender, EventArgs e )
    {
      _overItem = false;
    }

    private void Cb_MouseEnter( object sender, EventArgs e )
    {
      _overItem = true;
    }

    #endregion

    #region DISPOSE

    private bool disposedValue;

    protected virtual void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
          ClearOldControls( );
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~FlpHandler()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose( )
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose( disposing: true );
      GC.SuppressFinalize( this );
    }

    #endregion
  }
}

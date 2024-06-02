using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using FSimClientIF;

namespace FCamControl
{
  /// <summary>
  /// Catalog of Slots maintained
  /// </summary>
  internal class SlotCat
  {
    private static SlotCat[] _slotFolders = new SlotCat[8]; // SlotFolders A..H

    /// <summary>
    /// The current SlotFolder
    /// </summary>
    public static SlotCat CurrentSlotFolder { get; private set; }

    /// <summary>
    /// All SlotFolders
    /// </summary>
    public static SlotCat[] SlotFolders => _slotFolders;

    /// <summary>
    /// Init the SlotFolders with a Button Handler and a Camera V2
    /// </summary>
    public static void InitSlotFolders( ButtonHandler buttonHandler, Camera camera )
    {
      for (int i = 0; i < _slotFolders.Length; i++) {
        _slotFolders[i] = new SlotCat( (uint)i );
        // create an empty slot for each button
        uint slotNo = 0;
        foreach (var slotButton in buttonHandler.HandledButtonList) {
          _slotFolders[i].Add( slotNo++, slotButton, camera );
        }
      }
    }

    /// <summary>
    /// Set the SlotFolder active
    /// </summary>
    /// <param name="slotFolder">A slot folder index0..max</param>
    public static void SetActiveSlotFolder( uint slotFolder )
    {
      // sanity
      if (slotFolder >= _slotFolders.Length) return;

      // disable all - may optimise to only disable Current...
      for (int i = 0; i < _slotFolders.Length; i++) {
        _slotFolders[i].EnableFolderSlots( false );
      }
      // set active Folder
      _slotFolders[slotFolder].EnableFolderSlots( true );
      CurrentSlotFolder = _slotFolders[slotFolder];
    }


    // ***** CLASS Implementation

    private int _slotFolderNo = -1;
    private List<Slot> _slots = new List<Slot>( );

    /// <summary>
    /// Fired when a Slot is saved
    /// </summary>
    public event EventHandler<EventArgs> SlotSaved;
    /// <summary>
    /// Fired when a Slot is cleared
    /// </summary>
    public event EventHandler<EventArgs> SlotCleared;

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="slotFolderNo">Number of slotfolders to support</param>
    public SlotCat( uint slotFolderNo )
    {
      _slotFolderNo = (int)slotFolderNo;
    }

    /// <summary>
    /// Get; the SlotFolderNo (0..max)
    /// </summary>
    public int SlotFolderNo => _slotFolderNo;

    /// <summary>
    /// Add an Item with Camera V2
    /// </summary>
    /// <param name="slotNo">This SlotNo</param>
    /// <param name="button">The Managed Button of this Slot</param>
    /// <param name="camera">Camera</param>
    private void Add( uint slotNo, HandledButton button, Camera camera )
    {
      var slot = new Slot( (uint)_slotFolderNo, slotNo, button, camera );
      slot.SlotSaved += slot_SlotSaved;
      slot.SlotCleared += slot_SlotCleared;
      _slots.Add( slot );
    }

    /// <summary>
    /// Set the Slots of this Folder Enabled or Disabled
    /// </summary>
    /// <param name="enabled">True to set Enabled</param>
    public void EnableFolderSlots( bool enabled )
    {
      foreach (var s in _slots) {
        s.Enabled = enabled;
        if (enabled) {
          s.MaintainButtonState( ); 
        } // visuals
      }
    }

    /// <summary>
    /// Trigger a SlotSave for the next Click
    /// </summary>
    public void ExpectSlotSave( )
    {
      foreach (var s in _slots) {
        s.ExpectSlotSave( );
      }
    }

    /// <summary>
    /// Cancel a SlotSave for the next Click
    /// </summary>
    public void CancelSlotSave( )
    {
      foreach (var s in _slots) {
        s.CancelSlotSave( );
      }
    }

    /// <summary>
    /// Trigger a SlotClear for the next Click
    /// </summary>
    public void ExpectSlotClear( )
    {
      foreach (var s in _slots) {
        s.ExpectSlotClear( );
      }
    }

    /// <summary>
    /// Cancel a SlotClear for the next Click
    /// </summary>
    public void CancelSlotClear( )
    {
      foreach (var s in _slots) {
        s.CancelSlotClear( );
      }
    }

    // Handle slot is saved by one
    private void slot_SlotSaved( object sender, EventArgs e )
    {
      CancelSlotSave( );
      SlotSaved?.Invoke( this, new EventArgs( ) ); // let the user know
    }

    private void slot_SlotCleared( object sender, EventArgs e )
    {
      CancelSlotClear( );
      SlotCleared?.Invoke( this, new EventArgs( ) ); // let the user know
    }

    // AppSettings Tools

    /// <summary>
    /// Get; Set: The AppSetting string for this Item
    ///   returns a folder setting string for all Slots of this folder
    /// 
    /// Get to save retrieve the setting string to store it in Settings
    /// Set to load from Settings - will load all Folder Slots from the Setting string
    /// </summary>
    public string FolderSettingString {
      get {
        string setting = "";
        // get all slot settings into one folder setting string
        foreach (var s in _slots) {
          setting += $"{s.SlotSettingString}";
        }
        return setting;
      }
      set {
        // retrieves all valid slot entries from the setting string
        var matches = Slot.SlotMatches( value );
        // load each slot with values
        foreach (Match match in matches) {
          string s = match.Value;
          var slotNo = Slot.SlotNo( s );
          if ((slotNo >= 0) && (slotNo < _slots.Count)) {
            _slots[slotNo].SlotSettingString = s;
          }
        }
      }
    }

  }
}

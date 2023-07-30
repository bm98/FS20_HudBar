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
    /// Init the SlotFolders 
    /// </summary>
    public static void InitSlotFolders( List<Button> buttons, Action<CameraSetting, int, Vector3, Vector3> switchCam )
    {
      for (int i = 0; i < _slotFolders.Length; i++) {
        _slotFolders[i] = new SlotCat( (uint)i );
        // create an empty slot for each button
        uint slotNo = 0;
        foreach (var slotButton in buttons) {
          _slotFolders[i].Add( slotNo++, slotButton, switchCam );
        }
      }
    }

    public static void SetActiveSlotFolder( uint slotFolder )
    {
      // sanity
      if (slotFolder >= _slotFolders.Length) return;

      // disable all - may optimise to only disable Current...
      for (int i = 0; i < _slotFolders.Length; i++) {
        _slotFolders[i].SetEState( false );
      }
      // set active Folder
      _slotFolders[slotFolder].SetEState( true );
      CurrentSlotFolder = _slotFolders[slotFolder];
    }


    // ***** CLASS Implementation

    private int _slotFolderNo = -1;
    private List<Slot> _slots = new List<Slot>( );

    // Declare the delegate (if using non-generic pattern).
    public delegate void SlotSavedEventHandler( object sender, EventArgs e );
    public event SlotSavedEventHandler SlotSaved;

    public SlotCat( uint slotFolderNo )
    {
      _slotFolderNo = (int)slotFolderNo;
    }

    /// <summary>
    /// Get; the SlotFolderNo (0..max)
    /// </summary>
    public int SlotFolderNo => _slotFolderNo;

    /// <summary>
    /// Add an Item
    /// </summary>
    /// <param name="slotNo">This SlotNo</param>
    /// <param name="button">The Managed Button of this Slot</param>
    /// <param name="switchCam">Camera Switch Method</param>
    private void Add( uint slotNo, Button button, Action<CameraSetting, int, Vector3, Vector3> switchCam )
    {
      var slot = new Slot( (uint)_slotFolderNo, slotNo, button, switchCam );
      slot.SlotSaved += slot_SlotSaved;
      _slots.Add( slot );
    }

    /// <summary>
    /// Set the Slot Enabled or Disabled
    /// </summary>
    /// <param name="enabled">True to set Enabled</param>
    public void SetEState( bool enabled )
    {
      foreach (var s in _slots) {
        s.Enabled = enabled;
        if (enabled) { s.MaintainButtonState( ); } // visuals
      }
    }

    /// <summary>
    /// Trigger a SlotSave for the next Click
    /// </summary>
    public void ExpectSlotSave( Vector3 position, Vector3 gimbal )
    {
      foreach (var s in _slots) {
        s.ExpectSlotSave( position, gimbal );
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

    // Handle slot is saved by one
    private void slot_SlotSaved( object sender, EventArgs e )
    {
      CancelSlotSave( );
      SlotSaved?.Invoke( this, new EventArgs( ) ); // let the user know
    }

    // AppSettings Tools

    /// <summary>
    /// Get; Set: The AppSetting string for this Item
    /// </summary>
    public string AppSettingString {
      get {
        string setting = "";
        foreach (var s in _slots) {
          setting += $"{s.AppSettingString}";
        }
        return setting;
      }
      set {
        var matches = Slot.SlotMatches( value );
        foreach (Match match in matches) {
          string s = match.Value;
          var slotNo = Slot.SlotNo( s );
          if ((slotNo >= 0) && (slotNo < _slots.Count)) {
            _slots[slotNo].AppSettingString = s;
          }
        }
      }
    }

  }
}

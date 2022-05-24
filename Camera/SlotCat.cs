using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

using FSimClientIF;

namespace FS20_HudBar.Camera
{
  /// <summary>
  /// Catalog of Slots maintaines
  /// </summary>
  internal class SlotCat
  {
    private static SlotCat[] _slotFolders = new SlotCat[6]; // SlotFolders A..F

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
    public static void InitSlotFolders( List<Button> buttons, Action<CameraSetting, uint> switchCam )
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

    private bool _slotSaving = false;

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
    private void Add( uint slotNo, Button button, Action<CameraSetting, uint> switchCam )
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
      }
    }

    /// <summary>
    /// Trigger a SlotSave for the next Click
    /// </summary>
    public void ExpectSlotSave( )
    {
      _slotSaving = true;
      foreach (var s in _slots) {
        s.ExpectSlotSave = true;
      }
    }

    /// <summary>
    /// Cancel a SlotSave for the next Click
    /// </summary>
    public void CancelSlotSave( )
    {
      _slotSaving = false;
      foreach (var s in _slots) {
        s.ExpectSlotSave = false;
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
          setting += $"{s.AppSettingString}¦";
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

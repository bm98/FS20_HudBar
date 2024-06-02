using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dNetBm98.Win;

using static dNetBm98.Win.WinKbdSender;

namespace FCamControl
{
  /// <summary>
  /// Custom Camera Controller
  ///  fakes Keyboard hits to switch the cam
  /// </summary>
  internal sealed class CustomCamController
  {

    private int _lastSlot = 0;

    // slots 0..9 (Slot1..Slot10)
    private KbdStroke[] _slots = new KbdStroke[10];

    private WinKbdSender _kbd = null;

    /// <summary>
    /// The last selected Slot
    /// </summary>
    public int LastSlotIndex => _lastSlot;
    /// <summary>
    /// Clear the last slot used
    /// </summary>
    public void ClearLastSlotIndex( ) => _lastSlot = -1;

    /// <summary>
    /// (Re)-Load the MSFS Keys
    /// </summary>
    /// <param name="msfsKeyCatalog">Key Catalog</param>
    public void ReloadKeyCatalog( MSFS_KeyCat msfsKeyCatalog )
    {
      // load slots 1..9,0
      int idx = 0;
      _slots[idx++] = msfsKeyCatalog[FS_Key.CustCam1].AsStroke( MSFS_Key.c_KeyDelay );
      _slots[idx++] = msfsKeyCatalog[FS_Key.CustCam2].AsStroke( MSFS_Key.c_KeyDelay );
      _slots[idx++] = msfsKeyCatalog[FS_Key.CustCam3].AsStroke( MSFS_Key.c_KeyDelay );
      _slots[idx++] = msfsKeyCatalog[FS_Key.CustCam4].AsStroke( MSFS_Key.c_KeyDelay );
      _slots[idx++] = msfsKeyCatalog[FS_Key.CustCam5].AsStroke( MSFS_Key.c_KeyDelay );
      _slots[idx++] = msfsKeyCatalog[FS_Key.CustCam6].AsStroke( MSFS_Key.c_KeyDelay );
      _slots[idx++] = msfsKeyCatalog[FS_Key.CustCam7].AsStroke( MSFS_Key.c_KeyDelay );
      _slots[idx++] = msfsKeyCatalog[FS_Key.CustCam8].AsStroke( MSFS_Key.c_KeyDelay );
      _slots[idx++] = msfsKeyCatalog[FS_Key.CustCam9].AsStroke( MSFS_Key.c_KeyDelay );
      _slots[idx++] = msfsKeyCatalog[FS_Key.CustCam0].AsStroke( MSFS_Key.c_KeyDelay );
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public CustomCamController( MSFS_KeyCat msfsKeyCatalog )
    {
      // load slots 1..9,0
      ReloadKeyCatalog( msfsKeyCatalog );

      _kbd = new WinKbdSender( );
    }

    /// <summary>
    /// Send a keyboard command to switch to slot N (0..9)
    /// </summary>
    /// <param name="slot">The Slot 0..9</param>
    public void SendSlot( int slot )
    {
      // sanity
      if (slot < 0) slot = 0;
      if (slot > 9) slot = 9;

      if (slot == _lastSlot) return; // don't set twice, else it is returning to pilot view (MSFS ????!!!!!)

      _lastSlot = slot;

      _kbd.AddStroke( _slots[slot] );
      _kbd.RunStrokes( MSFS_Key.c_SimWindowTitle, blocking: false );
    }


    /// <summary>
    /// Set the slot N 0..9 to the new Stroke
    /// </summary>
    /// <param name="slot">The Slot 0..9</param>
    /// <param name="stroke">KdbStroke</param>
    public void DefineSlot( int slot, KbdStroke stroke )
    {
      // sanity
      if (slot < 0) return;
      if (slot > 9) return;

      _slots[slot] = stroke.Clone( );
    }

    /// <summary>
    /// Simple serializer
    /// </summary>
    /// <returns>A string</returns>
    public string SlotsToString( )
    {
      var sb = new StringBuilder( );

      for (int i = 0; i < _slots.Length; i++) {
        sb.Append( _slots[i].ToString( ) + ";" );
      }
      return sb.ToString( );
    }

    /// <summary>
    /// Simple serializer
    /// </summary>
    /// <param name="str">A serialized string</param>
    public void SlotsFromString( string str )
    {
      string[] e = str.Split( new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries );
      for (int i = 0; i < e.Length; i++) {
        _slots[i] = new KbdStroke( e[i] );
      }
    }

  }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using FSimClientIF;

namespace FCamControl
{
  /// <summary>
  /// Manages a row of buttons WinForms CheckedButton
  ///  indexed from 0..N
  /// 
  /// Activation is
  /// Can set Active (with a new Fore/Back color)
  /// 
  /// </summary>
  internal class ButtonHandler
  {
    // all buttons
    private readonly List<HandledButton> _buttons = new List<HandledButton>( );

    // Radio Button Style handling
    private bool _radioStyle = false;
    // Active Slot when in Radio Style - -1 if none
    private int _radioActiveSlot = -1;


    /// <summary>
    /// Returns the enumeration of HandledButtons
    /// </summary>
    public IEnumerable<HandledButton> HandledButtonList => _buttons.OrderBy( bt => bt.Slot );


    /// <summary>
    /// Returns the enumeration of Active HandledButtons
    /// </summary>
    public IEnumerable<HandledButton> ActiveHandledButtonList => _buttons.Where( bt => bt.Active );

    /// <summary>
    /// Returns the enumeration of CheckedButton managed
    /// </summary>
    public IEnumerable<CheckedButton> ButtonList => HandledButtonList.Select( bt => bt.Button );


    // true when in range
    private bool SlotInRange( int slot ) => (slot >= 0) && (slot < _buttons.Count);

    /// <summary>
    /// returns the slot with a button named 'name' or -1 if not found
    /// </summary>
    /// <param name="name">Button Name</param>
    /// <returns>A slot or -1 if not found</returns>
    public int SlotWithName( string name )
    {
      // sanity
      if (string.IsNullOrWhiteSpace( name )) return -1;

      var button = _buttons.FirstOrDefault( bt => bt.Button.Name == name );
      if (button != default) return button.Slot;
      return -1;
    }


    /// <summary>
    /// Active ForeColor
    /// </summary>
    public Color ActFColor { get; set; } = SystemColors.ActiveCaptionText;
    /// <summary>
    /// Active BackColor
    /// </summary>
    public Color ActBColor { get; set; } = SystemColors.ActiveCaption;
    /// <summary>
    /// Inactive ForeColor
    /// </summary>
    public Color FColor { get; set; } = SystemColors.WindowText;
    /// <summary>
    /// Inactive BackColor
    /// </summary>
    public Color BColor { get; set; } = SystemColors.Control;

    /// <summary>
    /// Get: Radio Style Button Handling
    /// i.e. only one can be active, but all inactive
    /// </summary>
    public bool RadioButtonStyle => _radioStyle;

    /// <summary>
    /// The HandledButton from a Slot or null if not found
    /// </summary>
    /// <param name="slot">A Slot</param>
    /// <returns>A HandledButton or null </returns>
    public HandledButton ButtonFromSlot( int slot )
    {
      // sanity
      if (!SlotInRange( slot )) return null;

      return _buttons[slot];
    }

    /// <summary>
    /// The HandledButton from a Name or null if not found
    /// </summary>
    /// <param name="name">A Button Name</param>
    /// <returns>A HandledButton or null </returns>
    public HandledButton ButtonFromName( string name )
    {
      return ButtonFromSlot( SlotWithName( name ) );
    }

    /// <summary>
    /// The first HandledButton with CameraMode or null if not found
    /// </summary>
    /// <param name="cameraMode">Button CameraMode</param>
    /// <returns>A HandledButton or null </returns>
    public HandledButton ButtonFromCamSetting( CameraMode cameraMode )
    {
      var button = _buttons.FirstOrDefault( bt => bt.CamMode == cameraMode );
      if (button != default) return button;
      return null;
    }

    /// <summary>
    /// True if the Button in Slot is active
    /// </summary>
    /// <param name="slot">A Slot</param>
    /// <returns>True when active</returns>
    public bool IsActive( int slot )
    {
      // sanity
      if (!SlotInRange( slot )) return false;

      return _buttons[slot].Active;
    }

    /// <summary>
    /// Return the ButtonName for a Slot index
    /// </summary>
    /// <param name="slot">A Slot Index</param>
    /// <returns>A name or an empty string</returns>
    public string NameOfSlot( int slot )
    {
      // sanity
      if (!SlotInRange( slot )) return "";

      return _buttons[slot].Button.Name;
    }

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="radioStyle">True for RadioStyle buttons</param>
    public ButtonHandler( bool radioStyle )
    {
      _radioStyle = radioStyle;
    }

    /// <summary>
    /// Add a WinForms CheckedButton
    /// </summary>
    /// <param name="checkedButton">A CheckedButton</param>
    /// <returns>The SlotIndex 0..N</returns>
    public int AddButton( CheckedButton checkedButton )
    {
      var slotButton = new HandledButton( checkedButton, _buttons.Count, this, CameraMode.NONE, null );
      _buttons.Add( slotButton );
      slotButton.Activate( false );
      return _buttons.Count - 1;// 0 based slot index
    }

    /// <summary>
    /// Add a WinForms CheckedButton
    /// </summary>
    /// <param name="checkedButton">A CheckedButton</param>
    /// <param name="action">Action to exec or null</param>
    /// <returns>The SlotIndex 0..N</returns>
    public int AddButton( CheckedButton checkedButton, Action<HandledButton> action )
    {
      var bslot = new HandledButton( checkedButton, _buttons.Count, this, CameraMode.NONE, action );
      _buttons.Add( bslot );
      bslot.Activate( false );
      return _buttons.Count - 1;// 0 based slot index
    }

    /// <summary>
    /// Add a WinForms CheckedButton
    /// </summary>
    /// <param name="checkedButton">A CheckedButton</param>
    /// <param name="action">Action to exec or null</param>
    /// <param name="cameraMode">CameraMode used on NONE</param>
    /// <returns>The SlotIndex 0..N</returns>
    public int AddButton( CheckedButton checkedButton, Action<HandledButton> action, CameraMode cameraMode )
    {
      var bslot = new HandledButton( checkedButton, _buttons.Count, this, cameraMode, action );
      _buttons.Add( bslot );
      bslot.Activate( false );
      return _buttons.Count - 1;// 0 based slot index
    }

    /// <summary>
    /// Deactivate all buttons visual
    /// </summary>
    public void DeactivateAll( )
    {
      foreach (var bt in _buttons) {
        bt.Activate( false );
      }
      _radioActiveSlot = -1;
    }

    /// <summary>
    /// Activate a button by slot
    /// -1 will deactivate the current radio slot
    /// </summary>
    /// <param name="slot">A Slot Index</param>
    public void ActivateButton( int slot )
    {
      if (RadioButtonStyle && SlotInRange( _radioActiveSlot )) {
        // disable if Radio 
        _buttons[_radioActiveSlot].Activate( false );
        _radioActiveSlot = -1;
      }
      // activate if needed
      if (SlotInRange( slot )) {
        _buttons[slot].Activate( true );
        _radioActiveSlot = slot;
      }
    }

    /// <summary>
    /// Activate a button by Control Name
    /// </summary>
    /// <param name="buttonName">Name of the Button</param>
    public void ActivateButton( string buttonName )
    {
      int slot = SlotWithName( buttonName );
      ActivateButton( slot );
    }


  }
}

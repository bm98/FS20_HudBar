using System;

using FSimClientIF;

namespace FCamControl
{
  /// <summary>
  /// A CheckedButton Handled by the ButtonHandler
  /// Handles the Click of the button
  /// </summary>

  internal class HandledButton
  {
    // ref to the owning button handler
    private ButtonHandler _handlerRef = null;
    // action to do on Click
    private Action<HandledButton> _action = null;

    /// <summary>
    /// The CheckedButton
    /// </summary>
    public CheckedButton Button { get; private set; }
    /// <summary>
    /// The Slot where it is handled
    /// </summary>
    public int Slot { get; private set; }

    /// <summary>
    /// True when Active
    /// </summary>
    public bool Active {
      get {
        return Button.Checked;
      }
      private set {
        Button.Checked = value;
      }
    }

    /// <summary>
    /// Used CameraMode or CameraMode.NONE
    /// </summary>
    public CameraMode CamMode { get; private set; } = CameraMode.NONE;

    /// <summary>
    /// A stored ViewIndex
    /// </summary>
    public int LastViewIndex { get; set; } = -1;

    /// <summary>
    /// Set Active or Inactive
    /// </summary>
    /// <param name="activate">True when active</param>
    public void Activate( bool activate )
    {
      Active = activate;
    }

    /// <summary>
    /// Set the last used ViewIndex
    /// </summary>
    /// <param name="index">A ViewIndex</param>
    public void SetViewIndex( int index )
    {
      LastViewIndex = index;
    }

    /// <summary>
    /// cTor
    /// </summary>
    public HandledButton( CheckedButton button, int slotIndex, ButtonHandler buttonHandler, CameraMode cameraMode, Action<HandledButton> action )
    {
      _handlerRef = buttonHandler;
      Button = button;
      Slot = slotIndex;
      Active = true;// causes the Deactivate to trigger on Init..  (only change is accepted)
      CamMode = cameraMode;
      _action = action;
      button.Click += button_Click;
    }

    // action on click
    private void button_Click( object sender, EventArgs e ) => _action?.Invoke( this );


  }
}

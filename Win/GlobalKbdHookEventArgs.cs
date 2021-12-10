using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.Win
{
  internal class GlobalKbdHookEventArgs : HandledEventArgs
  {

    /// <summary>
    /// (Sys)Key Up/Down message
    /// </summary>
    public GlobalKbdHook.KeyboardState KeyboardState { get; private set; }
    /// <summary>
    /// True if any Key is Pressed, else it is released
    /// </summary>
    public bool KeyDown { get; private set; }
    /// <summary>
    /// Data from USER32 API
    /// </summary>
    public GlobalKbdHook.LowLevelKeyboardInputEvent KeyboardData { get; private set; }

    public GlobalKbdHookEventArgs(
        GlobalKbdHook.LowLevelKeyboardInputEvent keyboardData,
        GlobalKbdHook.KeyboardState keyboardState,
        bool keyState )
    {
      KeyboardData = keyboardData;
      KeyboardState = keyboardState;
      KeyDown = keyState;
    }

  }
}

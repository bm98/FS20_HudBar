using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bm98_Checklist
{
  /// <summary>
  /// The LED Color for the switch
  /// </summary>
  public enum SwitchColor
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    Red, Green, Blue, Amber, White, Dark,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }

  /// <summary>
  /// Detected Mouse Wheel direction
  /// </summary>
  public enum MouseWheelAction
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    None, Up, Down,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }


  /// <summary>
  /// Detected Mouse Wheel Direction
  /// </summary>
  public class MouseWheelActionArgs : EventArgs
  {
    /// <summary>
    /// Returns the detected mouse wheel direction
    /// </summary>
    public MouseWheelAction MouseWheelAction = MouseWheelAction.None;
    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="mouseWheelAction"></param>
    public MouseWheelActionArgs( MouseWheelAction mouseWheelAction )
    {
      MouseWheelAction = mouseWheelAction;
    }
  }

  /// <summary>
  /// Switch State 
  /// </summary>
  public class SwitchEventArgs : EventArgs
  {
    /// <summary>
    /// Returns the detected switch state
    /// </summary>
    public bool SwitchStateOn = false;
    /// <summary>
    /// cTor:
    /// </summary>
    public SwitchEventArgs( bool switchStateOn )
    {
      SwitchStateOn = switchStateOn;
    }
  }

  internal class Helper
  {
    public static readonly Color FogColor = Color.FromArgb(22,Color.WhiteSmoke);
    public static readonly Color ClearColor = Color.Transparent;

  }


}

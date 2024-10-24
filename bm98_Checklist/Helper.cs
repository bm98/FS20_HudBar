using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace bm98_Checklist
{
  /// <summary>
  /// The LED Color for the switch
  /// </summary>
  public enum SwitchColor
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    Blue, Red, Green, Yellow, White, Amber, Dark,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }

  /// <summary>
  /// Check Box Size 
  /// </summary>
  public enum CheckSize
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    SizeMedium = 0,
    SizeSmall,
    SizeLarge,
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
    public static readonly Color FogColor = Color.FromArgb( 22, Color.WhiteSmoke );
    public static readonly Color ClearColor = Color.Transparent;

    // some Sizing Helpers to make the Entry TextBoxes the same as the BT Labels will show later
    // Make them having the same ClientSize will hopefully do the job..
    // TextBoxes have a 2pix border i.e. ClientSize is 4pix less than Size in width and height

    // Padding of the CheckButton Text Label Control to align the text down below the LED
    public static readonly Padding BtPadding = new Padding( 10, 20, 10, 5 );

    // CheckButton Sizes Med, Small, Large
    // Button Size
    public static readonly Size[] CheckBoxSizes = new Size[3] { new Size( 164, 70 ), new Size( 130, 56 ), new Size( 180, 86 ) };
    // Config Page TextBox
    //    public static readonly Size[] WriteBoxSizes = new Size[3] { new Size( 150, 48 ), new Size( 117, 34 ), new Size( 163, 60 ) }; // Config Page Text
    public static readonly Size[] WriteBoxSizes = new Size[3] { TextBoxFromCheckBox(CheckBoxSizes[0]), TextBoxFromCheckBox( CheckBoxSizes[1] ), TextBoxFromCheckBox( CheckBoxSizes[2] ) };

    private static Size TextBoxFromCheckBox( Size checkboxSize )
    {
      return new Size( checkboxSize.Width - BtPadding.Left - BtPadding.Right + 4-1, checkboxSize.Height - BtPadding.Top - BtPadding.Bottom + 4 );
    }

  }


}

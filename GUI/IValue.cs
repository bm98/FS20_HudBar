using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// Up Down enum
  /// </summary>
  public enum Steps
  {
    Unk = 0, Up, Down, P1,P2,P3,P4,P5, On, Off
  }


  /// <summary>
  /// An interface our ValueLabels need to implement
  /// </summary>
  interface IValue
  {
    /// <summary>
    /// Set the numeric Value
    /// </summary>
    float? Value { set; }

    /// <summary>
    /// Set the integer Value
    /// </summary>
    int? IntValue { set; }

    /// <summary>
    /// Set the Step Value
    /// </summary>
    Steps Step { set; }

    /// <summary>
    /// Set the label text (Control provides this if not overridden)
    /// </summary>
    string Text { get;  set; }

    /// <summary>
    /// Set true to show units
    /// </summary>
    bool ShowUnit { get; set; }

  }
}

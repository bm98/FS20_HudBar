namespace FS20_HudBar.GUI
{
  /// <summary>
  /// Up Down enum
  /// </summary>
  public enum Steps
  {
    /// <summary>
    /// Unknown
    /// </summary>
    Unk = 0,
    /// <summary>
    /// Up and OK (Gear, Flaps)
    /// </summary>
    UpOK,
    /// <summary>
    /// Up and Info
    /// </summary>
    UpInfo,
    /// <summary>
    /// Down and OK (Gear)
    /// </summary>
    DownOK,
    /// <summary>
    /// Positional Levels (Flaps, Spoiler)
    /// </summary>
    P1, P2, P3, P4, P5, P6, P7, PEnd,

    /// <summary>
    /// On and OK - counter part is OffWarn
    /// </summary>
    OnOK,
    /// <summary>
    /// Off and needs attention- counter part is OnOK
    /// </summary>
    OffWarn,

    /// <summary>
    /// On and needs attention - counter part is OffOK
    /// </summary>
    OnWarn,
    /// <summary>
    /// Off and OK- counter part is OnWarn
    /// </summary>
    OffOK,
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
    string Text { get; set; }

    /// <summary>
    /// Set true to show units
    /// </summary>
    bool ShowUnit { get; set; }

    /// <summary>
    /// Set true to show Altitude in meter rather than ft
    /// </summary>
    bool Altitude_metric { get; set; }

    /// <summary>
    /// Set true to show Distance in km rather than nm
    /// </summary>
    bool Distance_metric { get; set; }


  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace FCamControl.State
{
  /// <summary>
  /// Values of Camera Settings 
  /// </summary>
  internal interface ICameraValues
  {
    /// <summary>
    /// True when Zoom is available
    /// </summary>
    bool CanZoom { get; }

    /// <summary>
    /// True when SmartTarget is available
    /// </summary>
    bool CanSmartTarget { get; }

    /// <summary>
    /// Returns the ViewIndex 0..N or -1 if not supported
    /// </summary>
    int ViewIndex { get; }

    /// <summary>
    /// Returns the Max View Index supported by this cam, -1 if no ViewIndex is supported
    /// </summary>
    int MaxViewIndex { get; }

    /// <summary>
    /// Returns the SmartTarget Index 0..N or -1 if not active
    /// </summary>
    int SmartTargetIndex { get; }

    /// <summary>
    /// Zoom Level 0..100 or -1 if no Zoom is supported for this Camera
    /// </summary>
    int ZoomLevel { get; }
    /// <summary>
    /// Drone Lock mode, false if not supported
    /// </summary>
    bool DroneLock { get; }
    /// <summary>
    /// Drone Follow mode, false if not supported
    /// </summary>
    bool DroneFollow { get; }
    /// <summary>
    /// Drone Move Speed 0..100 or -1 if not supported for this Camera
    /// </summary>
    int DroneMoveSpeed { get; }
    /// <summary>
    /// Drone Rot Speed 0..100 or -1 if not supported for this Camera
    /// </summary>
    int DroneRotSpeed { get; }

    /// <summary>
    /// Returns the current 6DOF Position
    /// </summary>
    Vector3 Cam6DofPosition { get; }
    /// <summary>
    /// Returns the current 6DOF Gimbal
    /// </summary>
    Vector3 Cam6DofGimbal { get; }
    /// <summary>
    /// Returns the current 6DOF Camera LockMode
    /// </summary>
    bool Cam6DofLocked { get; }

  }
}

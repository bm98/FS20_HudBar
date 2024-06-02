﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FSimClientIF;
using SC = SimConnectClient;
using static FSimClientIF.Sim;

namespace FCamControl.State
{
  /// <summary>
  /// Cockpit View generics
  /// handles mainly the Cockpit Zoom
  /// </summary>
  internal abstract class AState_CockpitView : AState
  {
    // maintain value
    protected float _zoomLevel = 0;
    protected float _storedZoomLevel = 50; // initial values as the sim has them

    /// <summary>
    /// True when Zoom is available
    /// </summary>
    public override bool CanZoom => true;

    /// <summary>
    /// True when SmartTarget is available
    /// </summary>
    public override bool CanSmartTarget => true; // all cockpit views can

    /// <summary>
    /// cTor:
    /// </summary>
    public AState_CockpitView( CameraMode mode, Context context )
      : base( mode, context )
    {
    }

    // Establish current
    public override void OnInit( CameraMode prevMode )
    {
      base.OnInit( prevMode );
    }

    /// <summary>
    /// Zoom Level 0..100 or -1 if no Zoom is supported for this Camera
    /// </summary>
    public override int ZoomLevel => (int)SV.Get<float>( SItem.fGS_Cam_Cockpit_zoomlevel );

    public override void OnZoomLevel( int zoomlevel )
    {
      base.OnZoomLevel( zoomlevel );

      _zoomLevel = zoomlevel;
    }

    // App Code Requests

    /// <summary>
    /// Requests a Cam Reset call
    /// </summary>
    public override void RequestResetCamera( )
    {
      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      _storedZoomLevel = SV.Get<float>( SItem.fGS_Cam_Cockpit_zoomlevel );

      base.RequestResetCamera( );

      RequestRestoreZoomValues( );
    }

    public override void RequestZoomLevel( int zoomLevel, bool untracked = false )
    {
      base.RequestZoomLevel( zoomLevel );

      // sanity
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      _zoomLevel = untracked ? _zoomLevel : zoomLevel; // dont track if requested
      SV.Set<float>( SItem.fGS_Cam_Cockpit_zoomlevel, (float)zoomLevel );
    }


    // set values to tracked values
    public void RequestRestoreZoomValues( )
    {
      SV.Set<float>( SItem.fGS_Cam_Cockpit_zoomlevel, _storedZoomLevel );
    }


  }
}

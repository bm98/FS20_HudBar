﻿using FSimClientIF;

using static FSimClientIF.Sim;

namespace FCamControl.State.ConcreteStates
{
  /// <summary>
  /// External - QuickView - Index 1..8
  /// </summary>
  internal class State_ExternalQuick : AState_ExternalView
  {
    /// <summary>
    /// True when SmartTarget is available
    /// </summary>
    public override bool CanSmartTarget => true;

    /// <summary>
    /// cTor:
    /// </summary>
    public State_ExternalQuick( CameraMode mode, Context context )
      : base( mode, context )
    {
    }

    public override void OnInit( CameraMode prevMode )
    {
      base.OnInit( prevMode );

      _firstInitDone = true;
    }


    // support ViewIndex

    // Switch 0..7
    public override void RequestViewIndex( int index )
    {
      base.RequestViewIndex( index );

      // sanity
      if ((index < 0) || (index > MaxViewIndex)) return;

      SV.Set<int>( SItem.iGS_Cam_Viewindex, index );
    }


  }
}

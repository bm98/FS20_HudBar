using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using FSimClientIF;

using static FSimClientIF.Sim;

namespace FCamControl.State.ConcreteStates
{
  /// <summary>
  ///  Cockpit - InstrumentView - Index 1..N
  /// No Zoom
  /// </summary>
  internal class State_InstrumentIndexed : AState
  {
    /// <summary>
    /// cTor:
    /// </summary>
    public State_InstrumentIndexed( CameraMode mode, Context context )
      : base( mode, context )
    {
    }

    public override void OnInit( CameraMode prevMode )
    {
      base.OnInit( prevMode );

      _firstInitDone = true;
    }


    // support ViewIndex

    // Switch 
    public override void RequestViewIndex( int index )
    {
      base.RequestViewIndex( index );

      // sanity
      if ((index < 0) || (index > MaxViewIndex)) return;

      SV.Set<int>( SItem.iGS_Cam_Viewindex, index );
    }


  }
}

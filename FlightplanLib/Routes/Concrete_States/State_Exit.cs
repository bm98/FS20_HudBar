using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.Routes.Concrete_States
{
  internal class State_Exit : AState
  {
    public State_Exit( DecoderState state, Context context ) : base( state, context )
    {
    }

    public override void OnInit( )
    {
      base.OnInit( );

      ; // NOP
    }

  }
}


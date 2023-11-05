using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FSimFacilityIF;
using FSimFacilityDataLib.AirportDB;


namespace FlightplanLib.Routes.Concrete_States
{
  internal class State_StarTrans : AState
  {
    public State_StarTrans( DecoderState state, Context context ) : base( state, context )
    {
    }

    public override void OnInit( )
    {
      base.OnInit( );

      // expect STAR keyword, or DottedID or a STAR ident 
    }


  }
}

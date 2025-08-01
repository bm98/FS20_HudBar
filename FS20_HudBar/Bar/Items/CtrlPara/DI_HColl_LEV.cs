﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;

using static FSimClientIF.Sim;


namespace FS20_HudBar.Bar.Items
{
  internal class DI_HColl_LEV : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.H_COLL_LEV;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "%Coll";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Collective Handle %";

    private readonly V_Base _label;
    private readonly V_Base _value1;

    public DI_HColl_LEV( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.H_COLL_LEV;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Prct( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      AddObserver( Desc, 5, OnDataArrival );
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        _value1.Value = SV.Get<float>( SItem.fG_Steer_RotorCollective_prct100  ) * 100.0f;
      }
    }

  }
}

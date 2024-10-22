using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;
using FS20_HudBar.GUI;

namespace FS20_HudBar.Bar.Items
{
  internal class DI_Nav1_Course : DispItem
  {
    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.NAV1_OBS;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "CRS 1";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "NAV-1 Course/Deviation";

    private readonly V_Base _label;
    private readonly V_Base _value1;
    private readonly A_WindArrow _obs;

    public DI_Nav1_Course( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      DiLayout = ItemLayout.Generic;
      var item = VItem.NAV1_OBS;
      _label = new L_Text( lblProto ) { Text = Short }; this.AddItem( _label );
      _value1 = new V_Deg( valueProto ) { ItemForeColor = cTxNav, ItemBackColor = cValBG };
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );
      _value1.MouseClick += _value1_MouseClick;
      _value1.MouseWheel += _value1_MouseWheel;
      _value1.Scrollable = true;
      _value1.Cursor = Cursors.SizeNS;

      item = VItem.NAV1_OBS_ANI;
      _obs = new A_WindArrow( ) { BorderStyle = BorderStyle.FixedSingle, AutoSizeWidth = true, ItemForeColor = cScale3 };
      _obs.Heading = 180; // Wind indicator is used - so inverse direction
      this.AddItem( _obs ); vCat.AddLbl( item, _obs );

      AddObserver( Short, 2, OnDataArrival );
    }

    // Synch with Radial
    private void _value1_MouseClick( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      SV.Set( SItem.S_Nav_1_OBS_Synch, true );
    }

    // Inc/Dec OBS 1
    private void _value1_MouseWheel( object sender, MouseEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      // 2/3 - 1/3  dectection for Digits
      var whole = e.Location.X < (_value1.Width / 3 * 2);
      if (e.Delta > 0) {
        float obiSet = (float)CoordLib.Geo.Wrap360( SV.Get<float>( SItem.fGS_Nav_1_OBS_deg ) + (whole ? 5f : 1f) );
        SV.Set( SItem.fGS_Nav_1_OBS_deg, obiSet );
        //        SV.Set( SItem.cmS_Nav_1_OBS_setting_step, whole ? FSimClientIF.CmdMode.Inc : FSimClientIF.CmdMode.Inc_Fract );
      }
      else if (e.Delta < 0) {
        float obiSet = (float)CoordLib.Geo.Wrap360( SV.Get<float>( SItem.fGS_Nav_1_OBS_deg ) - (whole ? 5f : 1f) );
        SV.Set( SItem.fGS_Nav_1_OBS_deg, obiSet );
        //        SV.Set( SItem.cmS_Nav_1_OBS_setting_step, whole ? FSimClientIF.CmdMode.Dec : FSimClientIF.CmdMode.Dec_Fract );
      }
    }


    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {
      if (this.Visible) {
        if (!SV.Get<bool>( SItem.bG_Nav_1_available )) {
          _value1.Text = "n.a.";
          _value1.ItemForeColor = cTxDim;
          _obs.ItemForeColor = cTxDim; // dimm
          _obs.DirectionFrom = 0;
          return;
        }

        // Has NAV1
        if (SV.Get<bool>( SItem.bG_Nav_1_hasSignal )) {
          // desired heading to the VOR
          _value1.Value = SV.Get<float>( SItem.fGS_Nav_1_OBS_deg );
          float err = SV.Get<float>( SItem.fG_Nav_1_RadialError_deg );
          _value1.ItemForeColor = Math.Abs( err ) <= 1.5 ? cOK : Math.Abs( err ) <= 3 ? cWarn : cInfo; // green / orange / white

          // Direction of Station
          var dir = (float)CoordLib.Geo.DirectionOf( SV.Get<float>( SItem.fG_Nav_2_Radial_degm ) + 180f,
                                                     SV.Get<float>( SItem.fG_Gps_GTRK_mag_degm ) );

          _obs.DirectionFrom = (int)dir;
          _obs.ItemForeColor = Math.Abs( dir ) <= 1.5 ? cOK : Math.Abs( dir ) <= 3 ? cWarn : cInfo; // green / orange / white
        }
        else {
          _value1.Value = null;
          _obs.ItemForeColor = cTxDim; // dimm
          _obs.DirectionFrom = 0;
        }
      }
    }

  }
}


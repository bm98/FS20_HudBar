using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DbgLib;

using SC = SimConnectClient;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;
using FS20_HudBar.GUI.Templates.Base;
using static FSimClientIF.Sim;

namespace FS20_HudBar.Bar.Items
{
  /// <summary>
  /// Comb Engine Load % 
  ///  Needs to be calibrated
  /// </summary>
  class DI_Load_prct : DispItem
  {
    #region STATIC

    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );

    #endregion

    /// <summary>
    /// The Label ID 
    /// </summary>
    public static readonly LItem LItem = LItem.LOAD_P;
    /// <summary>
    /// The GUI Name
    /// </summary>
    public static readonly string Short = "LOAD";
    /// <summary>
    /// The Configuration Description
    /// </summary>
    public static readonly string Desc = "Engine Pwr Load %";

    #region HP calibration

    /// <summary>
    /// Max HP Calibration storage
    /// </summary>
    static float[] s_maxHP = new float[] { 180, 180, 180, 180 }; // init 4 engines
    static bool s_calibrated = false;
    static private int m_acftTitleHash = 0;

    /// <summary>
    /// Calibrate engine 1..4 @ 50%
    /// </summary>
    /// <param name="engine">Engine No 1..4</param>
    /// <param name="torq">Torque at 50% Load</param>
    /// <param name="erpm">Engine RPM at 50% Load</param>
    private static void CalEngine( int engine, float torq, float erpm )
    {
      if (engine < 1 || engine > 4) return; // Sanity
      var maxHP = Calculator.MaxHPCalibration( torq, erpm ) * 2; // CALIBRATE @ 50% Load
      if (maxHP > 0) {
        s_maxHP[engine - 1] = maxHP;
        LOG.Log( $"CalEngine: engine {engine} max HP {maxHP}" );
      }
    }

    /// <summary>
    /// Calculated the Load % 0..1
    /// </summary>
    /// <param name="engine">Engine No 1..4</param>
    /// <param name="torq">Torque</param>
    /// <param name="erpm">Engine RPM</param>
    /// <returns>The Load % 0..100</returns>
    private static float Load_prct( int engine, float torq, float erpm )
    {
      if (engine < 1 || engine > 4) return 1; // Sanity
      return Calculator.LoadPrct( torq, erpm, s_maxHP[engine - 1] ) * 100f; // 0..100
    }

    #endregion

    private readonly B_Base _label;
    private readonly V_Base _value1;
    private readonly V_Base _value2;
    private readonly V_Base _value3;
    private readonly V_Base _value4;

    public DI_Load_prct( ValueItemCat vCat, Label lblProto, Label valueProto, Label value2Proto, Label signProto )
    {
      LabelID = LItem;
      var item = VItem.E1_LOAD_P;
      _label = new B_Text( item, lblProto ) { Text = Short }; this.AddItem( _label );
      _label.ButtonClicked += _label_ButtonClicked;

      _value1 = new V_Prct( value2Proto );
      this.AddItem( _value1 ); vCat.AddLbl( item, _value1 );

      item = VItem.E2_LOAD_P;
      _value2 = new V_Prct( value2Proto );
      this.AddItem( _value2 ); vCat.AddLbl( item, _value2 );

      // add 2 more values
      this.TwoRows = true;
      item = VItem.E3_LOAD_P;
      _value3 = new V_Prct( value2Proto ) { Visible = false };
      this.AddItem( _value3 ); vCat.AddLbl( item, _value3 );

      item = VItem.E4_LOAD_P;
      _value4 = new V_Prct( value2Proto ) { Visible = false };
      this.AddItem( _value4 ); vCat.AddLbl( item, _value4 );


      m_observerID = SV.AddObserver( Short, 2, OnDataArrival );
    }
    // Disconnect from updates
    protected override void UnregisterDataSource( )
    {
      UnregisterObserver_low( SV ); // use the generic one
    }

    // Calibrate the Load% per Engine 
    private void _label_ButtonClicked( object sender, ClickedEventArgs e )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return;

      int nEng = SV.Get<int>( SItem.iG_Cfg_NumberOfEngines_num );
      if (nEng > 0)
        CalEngine( 1, SV.Get<float>( SItem.fG_Eng_E1_Torque_ft_lbs ), SV.Get<float>( SItem.fG_Eng_E1_rpm ) );
      if (nEng > 1)
        CalEngine( 2, SV.Get<float>( SItem.fG_Eng_E2_Torque_ft_lbs ), SV.Get<float>( SItem.fG_Eng_E2_rpm ) );
      if (nEng > 2)
        CalEngine( 3, SV.Get<float>( SItem.fG_Eng_E3_Torque_ft_lbs ), SV.Get<float>( SItem.fG_Eng_E3_rpm ) );
      if (nEng > 3)
        CalEngine( 4, SV.Get<float>( SItem.fG_Eng_E4_Torque_ft_lbs ), SV.Get<float>( SItem.fG_Eng_E4_rpm ) );
      s_calibrated = true;
    }

    /// <summary>
    /// Update from Sim
    /// </summary>
    private void OnDataArrival( string dataRefName )
    {

      if (this.Visible) {
        // detect a new aircraft and derive the MaxHP if possible
        if (SV.Get<string>( SItem.sG_Cfg_AcftConfigFile ).GetHashCode( ) != m_acftTitleHash) {
          // acft title has changed
          m_acftTitleHash = SV.Get<string>( SItem.sG_Cfg_AcftConfigFile ).GetHashCode( );
          var acft = SC.MSFS.MsAcftTitles.AircraftFromTitle( SV.Get<string>( SItem.sG_Cfg_AcftConfigFile ) );
          if (acft != SC.MSFS.MsAcftTitles.Acft.Unknown) {
            // found in the SimConnectClient library
            var acdesc = SC.MSFS.MsAcftTitles.AircraftDesc( acft );
            if (acdesc.MaxHP > 0) { // sanity .. avoid Div0
              // cal 4 engines .. no matter how many there are
              s_maxHP[0] = acdesc.MaxHP; s_maxHP[1] = acdesc.MaxHP;
              s_maxHP[2] = acdesc.MaxHP; s_maxHP[3] = acdesc.MaxHP;
              s_calibrated = true;
            }
          }
          else {
            s_calibrated = false;
            //LOG.Log( $"OnDataArrival: Unknown Aircraft :{SV.AcftConfigFile}" );
          }
        }

        this.SetValuesVisible( SV.Get<int>( SItem.iG_Cfg_NumberOfEngines_num ) );
        _value1.Value = Load_prct( 1, SV.Get<float>( SItem.fG_Eng_E1_Torque_ft_lbs ),
                                    SV.Get<float>( SItem.fG_Eng_E1_rpm ) );

        _value2.Value = Load_prct( 2, SV.Get<float>( SItem.fG_Eng_E2_Torque_ft_lbs ),
                                      SV.Get<float>( SItem.fG_Eng_E2_rpm ) );

        _value3.Value = Load_prct( 3, SV.Get<float>( SItem.fG_Eng_E3_Torque_ft_lbs ),
                                      SV.Get<float>( SItem.fG_Eng_E3_rpm ) );

        _value4.Value = Load_prct( 4, SV.Get<float>( SItem.fG_Eng_E4_Torque_ft_lbs ),
                                      SV.Get<float>( SItem.fG_Eng_E4_rpm ) );
        this.ColorType.ItemBackColor = s_calibrated ? cActBG : cWarnBG; // change to live once established
      }
    }

  }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using DbgLib;
using static dNetBm98.Units;

using FSimClientIF.Flightplan;
using SC = SimConnectClient;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.Bar.Items;

using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;

using FS20_HudBar.Config;
using FSimClientIF.Modules;
using static FSimClientIF.Sim;
using FSimClientIF;
using PingLib;

namespace FS20_HudBar.Bar
{

  /// <summary>
  /// Instance of the current HudBar
  /// will initialize from profile settings
  /// </summary>
  internal sealed class HudBar : IDisposable
  {
    #region LOGGER
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );
    #endregion

    #region STATIC PART

    // this part is maintaned only once for all HudBar Instances (there is only One Instance at any given time - else it breaks...)

    // FLT ATC Flightplan
    /// <summary>
    /// The current ATC Flightplan (actually a copy of it)
    /// </summary>
    public static FSimClientIF.Flightplan.FlightPlan AtcFlightPlan => AtcFltPlanMgr.AtcFlightPlan;
    /// <summary>
    /// The current Shelf Flightplan (loaded by the user)
    /// </summary>
    public static FlightplanLib.Flightplan.FlightPlan UserFlightPlan => _userFlightPlanRef;
    /// <summary>
    /// Update the Shelf Flightplan (loaded by the user)
    /// </summary>
    /// <param name="userFlightPlan">A user loaded Flightplan</param>
    public static void UpdateUserFlightplan( FlightplanLib.Flightplan.FlightPlan userFlightPlan ) => _userFlightPlanRef = userFlightPlan;
    private static FlightplanLib.Flightplan.FlightPlan _userFlightPlanRef = new FlightplanLib.Flightplan.FlightPlan( ); // init empty

    // voice and sounds  - static, will never be disposed
    private static readonly PingLib.Loops m_ping = new PingLib.Loops( ); // Ping, Sound looping, Tone=0 will be Silence
    private static readonly PingLib.Sounds m_sound = new PingLib.Sounds( ); // Sound, single event, Tone=0 will be Silence

    // Use this Sound to set to Silence for Looped sounds
    private static readonly PingLib.SoundBite _silentLoop = new PingLib.SoundBite( PingLib.Melody.Silence, 0, 0, 0f );

    /// <summary>
    /// The Ping Loop Library
    /// </summary>
    public static PingLib.Loops PingLoop => m_ping;

    /// <summary>
    /// The Ping Sound Library
    /// </summary>
    public static PingLib.Sounds PingSound => m_sound;


    // the sound to play - set as loop samples with .6 sec playtime (at speed=1)
    // samples are high 0.45, lo 0.75 
    private static readonly PingLib.SoundBitePitched _soundLoop
      = new PingLib.SoundBitePitched( PingLib.Melody.TSynth3, 0, 0.75f, 0, 0.2f, 1f ); // Use this Sound to ping
    /// <summary>
    /// The Ping Sound
    /// </summary>
    public static PingLib.SoundBitePitched LoopSound => _soundLoop;

    // An alert chime: 2 bright bell rings
    private static readonly PingLib.SoundBite _soundChime
      = new SoundBite( Melody.Bell_1, 45, 1.2f, 2, 0.2f, 2f ); // rate=2 to play and end it faster, volume is lowered
    /// <summary>
    /// The Chime Sound
    /// </summary>
    public static PingLib.SoundBite ChimeSound => _soundChime;

    /// <summary>
    /// The Speech Library
    /// </summary>
    public static GUI_Speech SpeechLib => m_speech;
    private static readonly GUI_Speech m_speech = new GUI_Speech( ); // Speech

    // Voice Output Package
    private static readonly HudVoice m_voicePack = new HudVoice( m_speech );

    /// <summary>
    /// Provide acces to the VoicePack
    /// </summary>
    public static HudVoice VoicePack => m_voicePack;

    /// <summary>
    /// Enable or disable the Voice Out
    /// </summary>
    public static bool VoiceEnabled {
      get => m_speech.Enabled;
      set => m_speech.Enabled = value;
    }


    // Descriptive Configuration GUI label names to match the BarItems LItem Enum (sequence does not matter)
    private static Dictionary<LItem, string> m_cfgNames = new Dictionary<LItem, string>( ){
      {LItem.MSFS, DI_MsFS.Desc },
      {LItem.SimRate, DI_SimRate.Desc },      {LItem.FPS, DI_Fps.Desc },
      {LItem.LOG, DI_FlightLog.Desc },
      {LItem.TXT, DI_Text.Desc },
      {LItem.ACFT_ID, DI_Acft_ID.Desc },
      {LItem.TIME, DI_Time.Desc },            {LItem.ZULU, DI_ZuluTime.Desc },  {LItem.CTIME, DI_CompTime.Desc },

      {LItem.ETRIM, DI_ETrim.Desc },          {LItem.RTRIM, DI_RTrim.Desc },    {LItem.ATRIM, DI_ATrim.Desc },
     // {LItem.A_ETRIM, DI_A_ETrim.Desc},
      {LItem.H_TRIM, DI_HTrim.Desc },

      {LItem.WBALLAST_ANI,DI_WaterBallast.Desc},

      {LItem.OAT_C, DI_Oat_C.Desc },          {LItem.OAT_F, DI_Oat_F.Desc },
      {LItem.VIS, DI_Vis.Desc },
      {LItem.WIND_SD, DI_Wind_SD.Desc },      {LItem.WIND_XY, DI_Wind_XY.Desc },  {LItem.VWIND, DI_Wind_V.Desc },
      {LItem.BARO_HPA, DI_Baro_HPA.Desc },    {LItem.BARO_InHg, DI_Baro_InHg.Desc },
      {LItem.GEAR, DI_Gear.Desc },            {LItem.BRAKES, DI_Brakes.Desc },
      {LItem.FLAPS, DI_Flaps.Desc },          {LItem.SPOILER, DI_Spoilers.Desc },
      {LItem.FLAPS_ANI, DI_FlapsGraph.Desc }, {LItem.SPOILER_ANI, DI_SpoilersGraph.Desc },
      {LItem.Lights, DI_Lights.Desc },

      {LItem.NAV1_F, DI_Nav1.Desc },          {LItem.NAV2_F, DI_Nav2.Desc },
      {LItem.NAV1, DI_Nav1_Active.Desc },     {LItem.NAV2, DI_Nav2_Active.Desc },
      {LItem.NAV1_NAME, DI_Nav1_Name.Desc },  {LItem.NAV2_NAME, DI_Nav2_Name.Desc },
      {LItem.NAV1_OBS, DI_Nav1_Course.Desc }, {LItem.NAV2_OBS, DI_Nav2_Course.Desc },

      {LItem.ADF1_F, DI_Adf1.Desc },          {LItem.ADF2_F, DI_Adf2.Desc },
      {LItem.ADF1, DI_Adf1_Active.Desc },     {LItem.ADF2, DI_Adf2_Active.Desc },
      {LItem.ADF1_NAME, DI_Adf1_Name.Desc },  {LItem.ADF2_NAME, DI_Adf2_Name.Desc },

      {LItem.COM1, DI_Com1.Desc },            {LItem.COM2, DI_Com2.Desc },
      {LItem.COM1_NAME, DI_Com1_Name.Desc },  {LItem.COM2_NAME, DI_Com2_Name.Desc },
      {LItem.XPDR, DI_Xpdr.Desc },

      {LItem.MAN, DI_Man.Desc },
      {LItem.TORQ, DI_Torq.Desc },            {LItem.TORQP, DI_TorqP.Desc },        {LItem.TORQP_ANI, DI_TorqPGraph.Desc },
      {LItem.PRPM, DI_PRpm.Desc },            {LItem.ERPM, DI_ERpm.Desc },          {LItem.R_RPM, DI_RRpm.Desc },
      {LItem.PRPM_ANI, DI_PRpmGraph.Desc },   {LItem.ERPM_ANI, DI_ERpmGraph.Desc }, {LItem.R_RPM_ANI, DI_RRpmGraph.Desc },
      {LItem.N1, DI_N1.Desc },                {LItem.N2, DI_N2.Desc },
      {LItem.N1_ANI, DI_N1Graph.Desc },       {LItem.N2_ANI, DI_N2Graph.Desc },
      {LItem.EPR, DI_EPR.Desc },
      {LItem.AFTB, DI_Afterburner.Desc },     {LItem.AFTB_ANI, DI_AfterburnerGraph.Desc },
      {LItem.ITT, DI_Itt_C.Desc },
      {LItem.EGT_C, DI_Egt_C.Desc },          {LItem.EGT_F, DI_Egt_F.Desc },
      {LItem.CHT_C, DI_Cht_C.Desc },          {LItem.CHT_F, DI_Cht_F.Desc },
      {LItem.LOAD_P, DI_Load_prct.Desc },
      {LItem.COWL_ANI, DI_CowlFlapsGraph.Desc },

      {LItem.FFlow_gph, DI_FFlow_GPH.Desc },         {LItem.FFlow_pph, DI_FFlow_PPH.Desc },      {LItem.FFlow_kgh, DI_FFlow_KGH.Desc },
      {LItem.FUEL_LR_gal, DI_Fuel_LR_Gal.Desc },     {LItem.FUEL_LR_lb, DI_Fuel_LR_Lb.Desc },    {LItem.FUEL_LR_kg, DI_Fuel_LR_Kg.Desc },
      {LItem.FUEL_C_gal, DI_Fuel_C_Gal.Desc },       {LItem.FUEL_C_lb, DI_Fuel_C_Lb.Desc },      {LItem.FUEL_C_kg, DI_Fuel_C_Kg.Desc },
      {LItem.FUEL_TOT_gal, DI_Fuel_Total_Gal.Desc }, {LItem.FUEL_TOT_lb, DI_Fuel_Total_Lb.Desc },{LItem.FUEL_TOT_kg, DI_Fuel_Total_Kg.Desc },

      {LItem.FUEL_ANI, DI_FuelGraph.Desc },

      {LItem.GPS_WYP, DI_Gps_WYP.Desc },
      {LItem.GPS_WP_DIST, DI_Gps_WP_Dist.Desc }, {LItem.GPS_DST, DI_Gps_DST.Desc },
      {LItem.GPS_WP_ETE, DI_Gps_WP_Ete.Desc },   {LItem.GPS_ETE, DI_Gps_ETE.Desc },
      {LItem.GPS_TOD, DI_Gps_TOD.Desc },
      {LItem.GPS_BRGm, DI_Gps_BRGm.Desc },
      {LItem.GPS_TRK, DI_Gps_TRK.Desc },      {LItem.GPS_DTRK, DI_Gps_DTRK.Desc },
      {LItem.GPS_XTK, DI_Gps_XTK.Desc },
      {LItem.GPS_GS, DI_Gps_GS.Desc },
      {LItem.GPS_ALT, DI_Gps_ALT.Desc },
      {LItem.GPS_LAT_LON, DI_Gps_LatLon.Desc },
      {LItem.EST_VS,DI_Est_VS.Desc },         {LItem.EST_ALT, DI_Est_ALT.Desc },
      {LItem.ENROUTE, DI_Enroute.Desc },

      {LItem.COMPASS, DI_Compass.Desc },
      {LItem.HDG, DI_Hdg.Desc },              {LItem.HDGt, DI_HdgT.Desc },
      {LItem.ALT, DI_Alt_eff.Desc },          {LItem.ALT_INST, DI_Alt_Inst.Desc },
      {LItem.RA, DI_Ra.Desc },                {LItem.RA_VOICE, DI_Ra_Voice.Desc },
      {LItem.IAS, DI_Ias.Desc },
      {LItem.TAS, DI_Tas.Desc },
      {LItem.MACH, DI_Mach.Desc },
      {LItem.VS, DI_Vs.Desc },                {LItem.VS_PM, DI_Vs_PM.Desc },
      {LItem.VARIO_MPS, DI_VarioTE_mps_PM.Desc },    {LItem.VARIO_KTS, DI_VarioTE_kts_PM.Desc },   {LItem.VARIO_ANI, DI_VarioTEGraph.Desc },
      {LItem.NETTO_MPS, DI_VarioNetto_mps_PM.Desc }, {LItem.NETTO_KT,DI_VarioNetto_kts_PM.Desc }, {LItem.NETTO_ANI, DI_VarioNettoGraph.Desc },
      {LItem.MCRAD_MPS, DI_VarioMCS_mps.Desc },      {LItem.MCRAD_KT,DI_VarioMCS_kt.Desc },
      {LItem.AOA, DI_Aoa.Desc },              {LItem.FP_ANGLE, DI_FPAngle.Desc },
      {LItem.ESI_ANI, DI_ESIGraph.Desc },     {LItem.ACCEL_ANI,DI_AccelGraph.Desc },
      {LItem.GFORCE, DI_GForce.Desc },        {LItem.GFORCE_MM, DI_Gforce_MM.Desc },

      {LItem.AP, DI_Ap.Desc },
      {LItem.AP_HDGs, DI_Ap_HdgSet.Desc },
      {LItem.AP_ALTs, DI_Ap_AltSet.Desc },
      {LItem.AP_VSs, DI_Ap_VsSet.Desc },
      {LItem.AP_FLCs, DI_Ap_FlcSet.Desc },
      {LItem.AP_SPDs, DI_Ap_SpeedSet.Desc },
      {LItem.AP_BC, DI_Ap_BC.Desc },
      {LItem.AP_NAVg, DI_Ap_NavGps.Desc },
      {LItem.AP_VNAV, DI_Ap_VNav.Desc },
      {LItem.AP_APR_GS, DI_Ap_AprGs.Desc },
      {LItem.AP_ATT, DI_Ap_ATT.Desc },
      {LItem.AP_YD, DI_Ap_YD.Desc },
      {LItem.AP_LVL, DI_Ap_LVL.Desc },
      {LItem.AP_APR_INFO, DI_Ap_ApproachMode.Desc },
      {LItem.AP_ATHR, DI_Ap_AThrottle.Desc },
      {LItem.AP_ABRK, DI_Ap_ABrake.Desc },

      {LItem.ATC_APT, DI_Atc_APT.Desc },
      {LItem.ATC_RWY, DI_Atc_RWY.Desc },
      {LItem.ATC_ALT_HDG, DI_Atc_AltHdg.Desc },
      {LItem.METAR, DI_Metar.Desc },
      {LItem.DEPARR, DI_DepArr.Desc },

      {LItem.M_TIM_DIST1, DI_M_TimDist1.Desc },{LItem.M_TIM_DIST2, DI_M_TimDist2.Desc }, {LItem.M_TIM_DIST3, DI_M_TimDist3.Desc },
      {LItem.USR_ALERT_1,DI_UserAlert.Desc },  {LItem.USR_ALERT_2,DI_UserAlert2.Desc },  {LItem.USR_ALERT_3,DI_UserAlert3.Desc },

      {LItem.THR_LEV, DI_Thr_LEV.Desc },      {LItem.MIX_LEV, DI_Mix_LEV.Desc }, {LItem.PROP_LEV, DI_Prop_LEV.Desc },
      {LItem.TBRAKE, DI_ToeBrakes.Desc },     {LItem.THOOK, DI_THook.Desc },
      {LItem.A320THR, DI_A320Throttle.Desc },
      {LItem.SURF_ANI, DI_SurfacesGraph.Desc },
    };

    #endregion  // STATIC

    // SimVar access
    private readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;

    // as given by the caller
    private Configuration _configRef = null;

    /// <summary>
    /// The Configured Hotkeys
    /// </summary>
    public WinHotkeyCat Hotkeys { get; private set; }

    /// <summary>
    /// Returns the Show state of an item
    /// </summary>
    /// <param name="item">A Label item</param>
    /// <returns>True if it is shown</returns>
    public bool ShowItem( LItem item ) => _configRef.UsedProfile.IsShowItem( item );

    /// <summary>
    /// Placement of the Bar
    /// </summary>
    public Placement Placement => _configRef.UsedProfile.Placement;

    /// <summary>
    /// Display Kind of the Bar
    /// </summary>
    public Kind Kind => _configRef.UsedProfile.Kind;

    /// <summary>
    /// The Tooltip control for the FP
    /// </summary>
    public X_ToolTip ToolTipFP => m_toolTipFP;
    private X_ToolTip m_toolTipFP = new X_ToolTip( );     // Our Custom Mono ToolTip for the Flight path

    // The Font provider used internally and for Config
    private GUI_Fonts FONTS = null;
    /// <summary>
    /// Config Access for the used FONT provider
    /// </summary>
    public GUI_Fonts FontRef => FONTS;

    /// <summary>
    /// Ref for the Proto Label (used by config)
    /// </summary>
    public Label ProtoLabelRef { get; private set; }
    /// <summary>
    /// Ref for the Proto Value (used by config)
    /// </summary>
    public Label ProtoValueRef { get; private set; }
    /// <summary>
    /// Ref for the Proto Value2 (used by config)
    /// </summary>
    public Label ProtoValue2Ref { get; private set; }

    // maintain collections of the created Controls to do the processing
    // One collection contains the actionable interface
    // The second collection the WinForm Control itself (to act on the Control interface)

    // The Display Groups (DispItem is essentially a smaller FlowLayoutPanel containing the Label and 0, 1 or more Value Items)
    private DispItemCat m_dispItems = new DispItemCat( );

    // Value Items (updated from Sim, some may change colors based on conditions)
    private ValueItemCat m_valueItems = new ValueItemCat( );

    /// <summary>
    /// cTor: Of a completely new HudBar
    /// 
    /// Init the Hud Items - providing prototypes for the various label types and Config strings/values from AppSettings
    /// </summary>
    /// <param name="lblProto">A GUI prototype label, carrying fonts, colors etc (set in GUI design mode)</param>
    /// <param name="valueProto">A GUI prototype value, carrying fonts, colors etc (set in GUI design mode)</param>
    /// <param name="value2Proto">A GUI prototype value type 2, carrying fonts, colors etc (set in GUI design mode)</param>
    /// <param name="signProto">A GUI prototype icon(Wingdings), carrying fonts, colors etc (set in GUI design mode)</param>
    /// <param name="config">The actual configuration obj</param>
    public HudBar( Label lblProto, Label valueProto, Label value2Proto, Label signProto,
                   WinHotkeyCat hotkeys, Configuration config )
    {
      LOG.Log( "cTor HudBar: Start" );

      // use config items here
      _configRef = config;

      Hotkeys = hotkeys.Copy( );
      // Voice and Sounds
      VoiceEnabled = false; // disable, else we get early talks..
      _ = m_speech.SetVoice( _configRef.VoiceName );
      m_speech.SetOutputDevice( _configRef.OutputDeviceName );
      m_ping.SelectOutputDevice( _configRef.OutputDeviceName );
      m_sound.SelectOutputDevice( _configRef.OutputDeviceName );
      m_voicePack.SetFromConfigString( _configRef.VoiceCalloutProfile );

      ProtoLabelRef = lblProto;
      ProtoValueRef = valueProto;
      ProtoValue2Ref = value2Proto;

      PingLoop.PlayAsync( _silentLoop ); // use a default Silence Sound (to kill any ping) when restarting

      // Reset the observers as we rebuild the GUI now 
      // Depreciated - the Items are unregistered when Disposed

      // Reset the registered items for color handling - we create all new here
      GUI_Colors.ClearRegistry( );

      // Reset our own ToolTip control
      ToolTipFP.ResetDrawList( );

      // init the Fontprovider from the submitted labels
      LOG.Log( "cTor HudBar: Init Fontprovider" );
      FONTS = new GUI_Fonts( _configRef.UsedProfile.Condensed ); // get all fonts from built in
      FONTS.FromConfigString( _configRef.UsedFontsConfigString ); // and load from Config

      // set desired size
      FONTS.SetFontsize( _configRef.UsedProfile.FontSize );

      // and prepare the prototype Controls used below - not really clever but handier to have all label defaults ....
      lblProto.Font = (Font)FONTS.LabelFont.Clone( );   // need to use a Clone, else the Controls throw Parameter Error on Height ?? 
      valueProto.Font = (Font)FONTS.ValueFont.Clone( ); // seems due to Fonts are treated in WinForm Controls
      value2Proto.Font = (Font)FONTS.Value2Font.Clone( );
      signProto.Font = (Font)FONTS.SignFont.Clone( );

      // The pattern below repeats, create the display group and add it to the group list
      // All visual handling is done in the respective Class Implementation
      // Data Update is triggered by subscribing with the SimUpdate from the created object

      // Add all Display Items with prepared prototype labels (sequence does not matter)
      // While loading them into the GUI later, the ones not shown will be Disposed
      // We need all as there might be Breaks assigned to some that are not currently visible but 
      // we take those and realize the Breaks at the proper location

      LOG.Log( "cTor HudBar: Adding DI Items" );

      // Sim Status
      m_dispItems.AddDisp( new DI_MsFS( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_SimRate( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Fps( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_FlightLog( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // Free Text (we set the Initial Text from Settings here)
      m_dispItems.AddDisp( new DI_Text( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      this.Value( VItem.TXT ).Text = AppSettingsV2.Instance.FreeText;
      // Environment
      m_dispItems.AddDisp( new DI_Time( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_ZuluTime( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_CompTime( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Oat_C( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Oat_F( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Vis( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Wind_SD( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Wind_XY( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Wind_V( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // Aircraft
      m_dispItems.AddDisp( new DI_Acft_ID( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Baro_HPA( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Baro_InHg( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gear( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Brakes( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Flaps( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_FlapsGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_Spoilers( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_SpoilersGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_Lights( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // Engine
      m_dispItems.AddDisp( new DI_Man( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Torq( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_TorqP( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_TorqPGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_PRpm( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_PRpmGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_ERpm( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_ERpmGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_RRpm( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_RRpmGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_N1( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_N1Graph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_N2( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_N2Graph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_EPR( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Afterburner( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_AfterburnerGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_Itt_C( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Egt_C( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Egt_F( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Cht_C( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Cht_F( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Load_prct( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_CowlFlapsGraph( m_valueItems, lblProto ) );
      // Fuel
      m_dispItems.AddDisp( new DI_FFlow_PPH( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_FFlow_GPH( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_FFlow_KGH( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Fuel_LR_Gal( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Fuel_LR_Lb( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Fuel_LR_Kg( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Fuel_C_Gal( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Fuel_C_Lb( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Fuel_C_Kg( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Fuel_Total_Gal( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Fuel_Total_Lb( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Fuel_Total_Kg( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_FuelGraph( m_valueItems, lblProto ) );
      // Trim
      m_dispItems.AddDisp( new DI_ETrim( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // m_dispItems.AddDisp( new DI_A_ETrim( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_RTrim( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_ATrim( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_HTrim( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // Water Ballast
      m_dispItems.AddDisp( new DI_WaterBallast( m_valueItems, lblProto ) );
      // GPS
      m_dispItems.AddDisp( new DI_Gps_TRK( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gps_GS( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gps_DST( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gps_ETE( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gps_TOD( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gps_WYP( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gps_WP_Dist( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gps_WP_Ete( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gps_BRGm( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gps_DTRK( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gps_XTK( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gps_ALT( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gps_LatLon( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Est_VS( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Est_ALT( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Enroute( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // Aeronautics
      m_dispItems.AddDisp( new DI_Compass( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Hdg( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_HdgT( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Alt_eff( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Alt_Inst( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ra( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ra_Voice( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ias( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Tas( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Mach( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Vs( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Vs_PM( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_VarioTE_mps_PM( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_VarioTE_kts_PM( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_VarioTEGraph( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_VarioNetto_mps_PM( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_VarioNetto_kts_PM( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_VarioNettoGraph( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_VarioMCS_mps( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_VarioMCS_kt( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Aoa( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_FPAngle( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_ESIGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_AccelGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_GForce( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gforce_MM( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Nav1( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Nav2( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Adf1( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Adf2( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Nav1_Active( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Nav2_Active( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Adf1_Active( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Adf2_Active( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Nav1_Name( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Nav2_Name( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Adf1_Name( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Adf2_Name( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Nav1_Course( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Nav2_Course( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Xpdr( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Com1( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Com2( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Com1_Name( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Com2_Name( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // Autopilot
      m_dispItems.AddDisp( new DI_Ap( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_HdgSet( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_AltSet( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_VsSet( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_FlcSet( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_SpeedSet( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_BC( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_NavGps( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_VNav( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_AprGs( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_ATT( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_YD( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_LVL( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_ApproachMode( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_AThrottle( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_ABrake( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // ATC
      m_dispItems.AddDisp( new DI_Atc_APT( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Atc_RWY( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Atc_AltHdg( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // METAR
      m_dispItems.AddDisp( new DI_Metar( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // ROUTE
      m_dispItems.AddDisp( new DI_DepArr( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // METERS
      m_dispItems.AddDisp( new DI_M_TimDist1( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_M_TimDist2( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_M_TimDist3( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // ALERTS
      m_dispItems.AddDisp( new DI_UserAlert1( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_UserAlert2( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_UserAlert3( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // Controls
      m_dispItems.AddDisp( new DI_Thr_LEV( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Mix_LEV( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Prop_LEV( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_SurfacesGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_A320Throttle( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_ToeBrakes( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_THook( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );

      LOG.Log( $"cTor HudBar: {m_dispItems.Count} items loaded" );

      // **** post processing

      // Apply Metric Setting from AppSettings
      // Apply Unit modifier (shown, not shown) to all Values
      LOG.Log( $"cTor HudBar: Post Processing" );
      foreach (var lx in m_valueItems) {
        SetAltitudeMetric( AppSettingsV2.Instance.Altitude_Metric );
        SetDistanceMetric( AppSettingsV2.Instance.Distance_Metric );
        SetShowUnits( AppSettingsV2.Instance.ShowUnits );
      }

      // Align the Vertical alignment accross the bar
      if (_configRef.UsedProfile.Placement == Placement.Top
        || _configRef.UsedProfile.Placement == Placement.TopStack
        || _configRef.UsedProfile.Placement == Placement.Bottom) {

        // Set min size of the labels in order to have them better aligned
        // Using arrow chars gets a larger height than regular ASCII chars (odd autosize behavior)
        // Setting minSize to the Max found height of any at least allows for some hotizontal alignment
        int mh = 0;
        foreach (var lx in m_valueItems) {
          mh = (lx.Value.Ctrl.Height > mh) ? lx.Value.Ctrl.Height : mh;
        }
        foreach (var lx in m_dispItems) {
          mh = (lx.Value.Label.Height > mh) ? lx.Value.Label.Height : mh;
        }

        // set MinHeight for value Labels
        foreach (var lx in m_valueItems) {
          lx.Value.Ctrl.MinimumSize = new Size( 1, mh );
        }
        // define MinHeight for the Group Labels
        foreach (var lx in m_dispItems) {
          lx.Value.Label.MinimumSize = new Size( 1, mh );
        }
      }

      // Align the Value columns on left and right bound bar or tile
      LOG.Log( $"cTor HudBar: Aligning DI items" );
      if (_configRef.UsedProfile.Placement == Placement.Left
        || _configRef.UsedProfile.Placement == Placement.Right) {
        // Determine max width and make them aligned
        int maxLabelWidth = 0;
        int max1ValueWidth = 0; // the Single Value DispItems Value label 
        Queue<int> max1ValueWidthList = new Queue<int>( );
        foreach (var lItem in _configRef.UsedProfile.ItemPosList( )) {
          var dix = DispItem( lItem );
          if (dix != null) {
            maxLabelWidth = (dix.Controls[0].Width > maxLabelWidth) ? dix.Controls[0].Width : maxLabelWidth;
            // collect per column
            if (_configRef.UsedProfile.IsBreakItem( lItem )) {
              max1ValueWidthList.Enqueue( max1ValueWidth );
              max1ValueWidth = 0;
            }
            // eval the Single Value items
            if (dix.Controls.Count == 2) {
              max1ValueWidth = (dix.Controls[1].Width > max1ValueWidth) ? dix.Controls[1].Width : max1ValueWidth;
            }
          }
        }
        max1ValueWidthList.Enqueue( max1ValueWidth ); // last width

        // pad the label control to the right to have the value columns aligned
        max1ValueWidth = max1ValueWidthList.Dequeue( );
        foreach (var lItem in _configRef.UsedProfile.ItemPosList( )) {
          var dix = DispItem( lItem );
          if (dix != null) {
            // get the next column width
            if (_configRef.UsedProfile.IsBreakItem( lItem )) {
              max1ValueWidth = max1ValueWidthList.Dequeue( );
            }
            if (dix.Controls.Count == 2 && !(dix.Controls[1] is V_Steps) && !(dix.Controls[1] is V_Text)) {
              dix.Controls[0].Padding = new Padding( 0, 0, maxLabelWidth - dix.Controls[0].Width, 0 );
              // align single Value ones to the max right (pad left)
              dix.Controls[1].Padding = new Padding( max1ValueWidth - dix.Controls[1].Width, 0, 0, 0 );
            }
            else {
              // others just column align the Value Items by adding a right Padding to the Label
              dix.Controls[0].Padding = new Padding( 0, 0, maxLabelWidth - dix.Controls[0].Width, 0 );

              // For Left and Right Bars - Handle two row items, assuming they get 1 Label and 2 + 2  Value controls
              if (dix.TwoRows) {
                // Shift the 2nd row Value to the same location as the 1st Value right with some padding
                dix.Controls[3].Padding = new Padding( dix.Controls[1].Location.X, 0, 0, 0 );
                dix.WrapContents = true;
              }
            }
          }
        }
      }
      // VoicePack Data Registration
      LOG.Log( $"cTor HudBar: Re-Register Observers for Voice Pack" );
      m_voicePack.UnRegisterObservers( ); // clear from previous run
      m_voicePack.RegisterObservers( ); // add used ones
      // we want to know when an Aircraft is changed
      SC.SimConnectClient.Instance.AircraftChange += Instance_AircraftChange;
      // init RA Limit
      Calculator.SetRA_Limit( SV.Get<EngineType>( SItem.etG_Cfg_EngineType ) );

      LOG.Log( $"cTor HudBar: End" );
    }

    #region Update Content and Settings

    // track Aircraft changes
    private bool _aircraftChanged = true; // trigger this one on Load
    private long _nextSecTic = 0;
    private const long c_paceSec = 10; // 10sec pace to update the IAS tooltip 

    /// <summary>
    /// Update from values from Sim which are not part of the Item content update 
    /// </summary>
    public void UpdateGUI( string dataRefName, long secondsTic )
    {
      if (!SC.SimConnectClient.Instance.IsConnected) return; // sanity..

      // update calculations
      Calculator.PaceCalculator( );

      // try to get the WYPs
      //  SimConnect GPS Waypoint IDs are not always available - depends on acft implementation...
      string prevWypID = SV.Get<bool>( SItem.bG_Gps_FP_tracking ) ? SV.Get<string>( SItem.sG_Gps_WYP_prevID ) : "";
      string nextWypID = SV.Get<bool>( SItem.bG_Gps_FP_tracking ) ? SV.Get<string>( SItem.sG_Gps_WYP_nextID ) : "";
      // update from flightplans, user has prio over ATC
      if (UserFlightPlan.IsValid) {
        // User Loaded has prio
        AirportMgr.Update(
          UserFlightPlan.Origin.IsValid ? UserFlightPlan.Origin.Icao_Ident.ICAO : "",
          UserFlightPlan.Destination.IsValid ? UserFlightPlan.Destination.Icao_Ident.ICAO : ""
        );
        // if we don't know the next WYP from SimConnect, try to use the UserFPlan
        if (string.IsNullOrEmpty( nextWypID ) && UserFlightPlan.NextRoutePoint.IsValid) {
          nextWypID = UserFlightPlan.NextRoutePoint.Icao_Ident.ICAO;
          prevWypID = UserFlightPlan.PrevRoutePoint.Icao_Ident.ICAO;
        }
      }
      else if (AtcFlightPlan.HasFlightPlan) {
        // ATC Airport - maintain APTs (we should always have a Destination here)
        AirportMgr.Update( AtcFlightPlan.Departure, AtcFlightPlan.Destination );
        // does not track Waypoints, so no such option with the ATC Plan
      }

      // Maintain the WaypointID Tracker to support the GPS Flightplan 
      // WP Enroute Tracker
      WPTracker.Track(
           prevWypID, nextWypID,
           SV.Get<double>( SItem.dG_Env_Time_loc_sec ),
           SV.Get<bool>( SItem.bG_Sim_OnGround )
      );

      // load Tooltips of Waypoints - will show a remaining ATC plan if there is any
      // Load Remaining Plan if the WYP or Flightplan has changed
      if (WPTracker.HasChanged || AtcFltPlanMgr.HasChanged) {
        LOG.Log( $"UpdateGUI: WP or FlightPlan has changed" );
        string tt = AtcFlightPlan.RemainingPlan( WPTracker.Read( ) );
        var di = this.DispItem( LItem.GPS_WYP );
        if (di != null) {
          this.ToolTipFP.SetToolTip( di.Label, tt );
          di.Label.Cursor = string.IsNullOrEmpty( tt ) ? Cursors.Default : Cursors.PanEast;
        }
        tt = AtcFlightPlan.WaypointByName( WPTracker.PrevWP ).PrettyDetailed;
        var vc = this.ValueControl( VItem.GPS_PWYP );
        if (vc != null) {
          this.ToolTipFP.SetToolTip( vc, tt );
          vc.Cursor = string.IsNullOrEmpty( tt ) ? Cursors.Default : Cursors.PanEast;
        }
        tt = AtcFlightPlan.WaypointByName( WPTracker.NextWP ).PrettyDetailed;
        vc = this.ValueControl( VItem.GPS_NWYP );
        if (vc != null) {
          this.ToolTipFP.SetToolTip( vc, tt );
          vc.Cursor = string.IsNullOrEmpty( tt ) ? Cursors.Default : Cursors.PanEast;
        }
        tt = AtcFlightPlan.Pretty;
        di = this.DispItem( LItem.ATC_ALT_HDG );
        if (di != null) {
          this.ToolTipFP.SetToolTip( di.Label, tt );
          di.Label.Cursor = string.IsNullOrEmpty( tt ) ? Cursors.Default : Cursors.PanEast;
        }
        // commit that we read the changes of the Flight Plan
        AtcFltPlanMgr.Read( );
      }

      // update Aircraft Props - Fuel has probably changed, sometimes Speeds depending on configuration, or a different aircraft at all
      if (_aircraftChanged || (secondsTic > _nextSecTic)) {
        if (_aircraftChanged) {
          Calculator.SetRA_Limit( SV.Get<EngineType>( SItem.etG_Cfg_EngineType ) ); // set RA Limit
          LOG.Log( $"UpdateGUI: Aircraft has changed to {SV.Get<string>( SItem.sG_Cfg_AcftConfigFile )}" );
        }

        string tt = "";
        if (!string.IsNullOrWhiteSpace( SV.Get<string>( SItem.sG_Cfg_AcftConfigFile ) )) {
          tt = $"Aircraft Type:     {SV.Get<string>( SItem.sG_Cfg_AcftConfigFile )}\n\n"
             + $"Cruise Altitude:   {SV.Get<float>( SItem.fG_Dsg_CruiseAlt_ft ):##,##0} ft\n"
             + $"Vc  Cruise Speed:  {SV.Get<float>( SItem.fG_Dsg_SpeedVC_kt ):##0} kt\n"
             + $"Vy  Climb Speed    {SV.Get<float>( SItem.fG_Dsg_SpeedClimb_kt ):##0} kt\n"
             + $"Vmu Takeoff Speed: {SV.Get<float>( SItem.fG_Dsg_SpeedTakeoff_kt ):##0} kt\n"
             + $"Vr  Min Rotation:  {SV.Get<float>( SItem.fG_Dsg_SpeedMinRotation_kt ):##0} kt\n"
             + $"Vs1 Stall Speed:   {SV.Get<float>( SItem.fG_Dsg_SpeedVS1_kt ):##0} kt\n"
             + $"Vs0 Stall Speed:   {SV.Get<float>( SItem.fG_Dsg_SpeedVS0_kt ):##0} kt\n\n"
             + $"Fuel Weight:       {SV.Get<float>( SItem.fG_Fuel_Quantity_total_lb ):###,##0} lbs ({Kg_From_Lbs( SV.Get<float>( SItem.fG_Fuel_Quantity_total_lb ) ):###,##0} kg)\n"
             + $"Payload Weight:    {SV.Get<float>( SItem.fG_Acft_PayloadWeight_lbs ):###,##0} lbs ({Kg_From_Lbs( SV.Get<float>( SItem.fG_Acft_PayloadWeight_lbs ) ):###,##0} kg)\n"
             + $"TOTAL Weight:      {SV.Get<float>( SItem.fG_Acft_TotalAcftWeight_lbs ):###,##0} lbs ({Kg_From_Lbs( SV.Get<float>( SItem.fG_Acft_TotalAcftWeight_lbs ) ):###,##0} kg)\n"
             + $"Zero Fuel Weight:  {SV.Get<float>( SItem.fG_Acft_TotalAcftWeight_lbs ) - SV.Get<float>( SItem.fG_Fuel_Quantity_total_lb ):###,##0} lbs ({Kg_From_Lbs( SV.Get<float>( SItem.fG_Acft_TotalAcftWeight_lbs ) - SV.Get<float>( SItem.fG_Fuel_Quantity_total_lb ) ):###,##0} kg)\n"
             + $"CG lon / lat:      {SV.Get<float>( SItem.fG_Acft_AcftCGlong_perc ):#0.0} % / {SV.Get<float>( SItem.fG_Acft_AcftCGlat_perc ):#0.0} %\n"
             + $"Empty Weight:      {SV.Get<float>( SItem.fG_Dsg_EmptyAcftWeight_lbs ):###,##0} lbs ({Kg_From_Lbs( SV.Get<float>( SItem.fG_Dsg_EmptyAcftWeight_lbs ) ):###,##0} kg)\n"
             + $"Max. Weight:       {SV.Get<float>( SItem.fG_Dsg_MaxAcftWeight_lbs ):###,##0} lbs ({Kg_From_Lbs( SV.Get<float>( SItem.fG_Dsg_MaxAcftWeight_lbs ) ):###,##0} kg)\n"
             ;
        }
        var di = this.DispItem( LItem.IAS );
        if (di != null) {
          this.ToolTipFP.SetToolTip( di.Label, tt );
          di.Label.Cursor = string.IsNullOrEmpty( tt ) ? Cursors.Default : Cursors.PanEast;
        }
        _aircraftChanged = false; // no longer
        _nextSecTic = secondsTic + c_paceSec;
      }
    }

    // Handle Acft Change Event from SimConnectClient
    private void Instance_AircraftChange( object sender, EventArgs e )
    {
      _aircraftChanged = true; // will be handled in the GUIUpdate procedure
    }

    /// <summary>
    /// Set the Hotkey Catalog
    /// </summary>
    /// <param name="hotkeys"></param>
    public void SetHotkeys( WinHotkeyCat hotkeys )
    {
      Hotkeys = hotkeys.Copy( );
    }

    // session settings

    /// <summary>
    /// Set the Altitude Display to Metric
    /// </summary>
    /// <param name="setting">True for Metric Unit</param>
    public void SetAltitudeMetric( bool setting )
    {
      foreach (var lx in m_valueItems) {
        lx.Value.Value.Altitude_metric = setting;
      }
    }

    /// <summary>
    /// Set the Distance Display to Metric
    /// </summary>
    /// <param name="setting">True for Metric Unit</param>
    public void SetDistanceMetric( bool setting )
    {
      foreach (var lx in m_valueItems) {
        lx.Value.Value.Distance_metric = setting;
      }
    }

    /// <summary>
    /// Set the current show unit flag communicated by the HUD
    /// </summary>
    /// <param name="setting"></param>
    public void SetShowUnits( bool setting )
    {
      foreach (var lx in m_valueItems) {
        lx.Value.Value.ShowUnit = setting;
      }
    }

    #endregion

    #region FlowLayout Panel Handling

    /// <summary>
    /// Load the given FLP with all visible Display Items
    /// </summary>
    /// <param name="flp"></param>
    public void LoadFLPanel( FlowLayoutPanel flp )
    {
      if (flp == null) return; // Sanity check
      // release the docking for a freeflow alignment  
      flp.Dock = DockStyle.None;
      flp.AutoSize = true; // don't know if this changes by some Magic in WinForms - just make sure it is set properly

      // set the Panel Alignment 
      switch (this.Placement) {
        case GUI.Placement.TopStack:
        case GUI.Placement.Top: flp.FlowDirection = FlowDirection.LeftToRight; break;
        case GUI.Placement.Bottom: flp.FlowDirection = FlowDirection.LeftToRight; break;
        case GUI.Placement.Left: flp.FlowDirection = FlowDirection.TopDown; break;
        case GUI.Placement.Right: flp.FlowDirection = FlowDirection.TopDown; break;
        default: flp.FlowDirection = FlowDirection.LeftToRight; break;// Bottom
      }

      // Walk all DispItems and add the ones to be shown to the Flow Panel
      DispItem prevDi = null;
      GUI.BreakType registeredBreak = GUI.BreakType.None; ;
      // Walk through all DispItems from the Bar
      foreach (LItem i in Enum.GetValues( typeof( LItem ) )) {
        // using the enum index only to count from 0..max items
        var key = _configRef.UsedProfile.ItemKeyFromPos( (int)i );
        // The DispItem is a FlowPanel containing the Label and maybe some Values
        var di = this.DispItem( key );
        if (di != null) {
          // For Sanity only - check that we have Controls to show
          if (di.Controls.Count > 0) {
            // check and register breaks for 2nd up items if there is no break registered (FlowBreak takes priority over DivBreaks)
            // this takes breaks from not visible items too
            if (registeredBreak == GUI.BreakType.None) {
              // take any
              registeredBreak = _configRef.UsedProfile.IsBreakItem( key ) ? GUI.BreakType.FlowBreak :
                                _configRef.UsedProfile.IsDivItem1( key ) ? GUI.BreakType.DivBreak1 :
                                _configRef.UsedProfile.IsDivItem2( key ) ? GUI.BreakType.DivBreak2 : GUI.BreakType.None;
            }
            else if (registeredBreak == GUI.BreakType.FlowBreak) {
              // take no further
              ; // NOP
            }
            else {
              // override DivBreaks only with FlowBreaks
              registeredBreak = _configRef.UsedProfile.IsBreakItem( key ) ? GUI.BreakType.FlowBreak : registeredBreak;
            }
          }

          // Load and process shown items
          if (this.ShowItem( key )) {
            // apply breaks if there are any
            if (registeredBreak == GUI.BreakType.FlowBreak && prevDi != null) {
              // the flowbreak causes the tagged item to be on the same line and then to break for the next one
              // Not so intuitive for the user - so we mark the one that goes on the next line but need to attach the FB then to the prev one
              flp.SetFlowBreak( prevDi, true );
              registeredBreak = GUI.BreakType.None; // reset
            }
            else if (registeredBreak == GUI.BreakType.DivBreak1 || registeredBreak == GUI.BreakType.DivBreak2) {
              // separator must be set before the newly added item
              // select Color Type of the separator
              DI_Separator dSep = new DI_Separator( (registeredBreak == GUI.BreakType.DivBreak2) ? GUI_Colors.ColorType.cDivBG2 : GUI_Colors.ColorType.cDivBG1 );
              // need some fiddling to make it fit in either direction
              if ((this.Placement == GUI.Placement.Bottom) || (this.Placement == GUI.Placement.Top) || (this.Placement == GUI.Placement.TopStack)) {
                dSep.Dock = DockStyle.Left;// horizontal Bar
              }
              else {
                dSep.Dock = DockStyle.Top;// vertical Bar
              }
              GUI.GUI_Colors.Register( dSep ); // register for color management
              flp.Controls.Add( dSep ); // add it to the Main FlowPanel
              registeredBreak = GUI.BreakType.None; // reset
            }
            // add the item 
            di.SetLabelFlowBreak( this.Placement == Placement.TopStack ); // 20240222 added for Blinds mode

            flp.Controls.Add( di );
            /* Code to add tooltips to the Label Part of an item - NOT IN USE RIGHT NOW
            if ( !string.IsNullOrEmpty( di.TText ) ) {
              m_toolTip.SetToolTip( di.Label, di.TText );
            }
            */
            prevDi = di; // store for FlowBreak attachment for valid and visible ones if the next one is tagged
          }
          // don't show
          else {
            // remove the DispItem
            m_dispItems.Remove( di.LabelID );
            // Dispose these items to get some memory back and not having invisible ones to be processed
            di.Dispose( );
          }
        }
      }
      // controls are loaded now
      // reapply Docking - the form will Autosize itself
      flp.Dock = DockStyle.Fill;
    }

    #endregion

    #region Item Access

    /// <summary>
    /// Returns the Value Interface for a Value Item
    ///   BEWARE no checks for validity of the item ..
    /// </summary>
    /// <param name="item">A valid VItem</param>
    /// <returns>The Value Interface or null if not found</returns>
    public IValue Value( VItem item )
    {
      try {
        var vi = m_valueItems[item];
        if ((vi == null) || (vi.Ctrl == null) || vi.Ctrl.IsDisposed) return null;
        return vi.Value;
      }
      catch {
        return null;
      }
    }

    /// <summary>
    /// Returns the ColorType Interface for a Value Item
    ///   BEWARE no checks for validity of the item ..
    /// </summary>
    /// <param name="item">A valid VItem</param>
    /// <returns>The ColorType Interface or null if not found</returns>
    public IColorType ColorType( VItem item )
    {
      try {
        var vi = m_valueItems[item];
        if ((vi == null) || (vi.Ctrl == null) || vi.Ctrl.IsDisposed) return null;
        return vi.ColorType;
      }
      catch {
        return null;
      }
    }

    /// <summary>
    /// Returns the Value Label Control for an Item
    ///   BEWARE no checks for validity of the item ..
    /// </summary>
    /// <param name="item">A valid VItem</param>
    /// <returns>The 'Value' Label-Control or null if not found</returns>
    public Control ValueControl( VItem item )
    {
      try {
        var vi = m_valueItems[item];
        if ((vi == null) || (vi.Ctrl == null) || vi.Ctrl.IsDisposed) return null;
        return vi.Ctrl;
      }
      catch {
        return null;
      }
    }

    /// <summary>
    /// Returns the Display Item given an LItem enum
    /// </summary>
    /// <param name="item">A valid LItem</param>
    /// <returns>The DispItem or null</returns>
    public DispItem DispItem( LItem item )
    {
      // catch disabled items without raising an exception (to catch real issues further down)
      // retired items should not land here anyway
      if (BarItems.IsINOP( item )) return null;

      try {
        var di = m_dispItems[item];
        if ((di == null) || di.IsDisposed) return null;
        return di;
      }
      catch {
        return null;
      }
    }

    /// <summary>
    /// Returns the Config Name given an LItem enum
    /// </summary>
    /// <param name="item">A LItem</param>
    /// <returns>The Config Name</returns>
    public string CfgName( LItem item )
    {
      // catch disabled items without raising an exception (to catch real issues further down)
      // retired items should not land here anyway
      if (BarItems.IsINOP( item )) return $"{item} is INOP";

      try {
        return m_cfgNames[item];
      }
      catch {
        return $"Cfg Name undef {item}";
      }
    }

    #endregion

    #region DISPOSE

    private bool disposedValue;

    private void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)
          SC.SimConnectClient.Instance.AircraftChange -= Instance_AircraftChange;
          FONTS.Dispose( );
          foreach (var di in m_dispItems) {
            di.Value.Dispose( );
          }
          m_toolTipFP.Dispose( );
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~HudBar()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose( )
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose( disposing: true );
      GC.SuppressFinalize( this );
    }

    #endregion

  }
}

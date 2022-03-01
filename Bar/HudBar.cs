using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FSimClientIF.Flightplan;
using SC = SimConnectClient;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.Bar.Items;

using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates;

using FS20_HudBar.Config;

namespace FS20_HudBar.Bar
{

  /// <summary>
  /// Instance of the current HudBar
  /// will initialize from profile settings
  /// </summary>
  internal class HudBar
  {
    #region STATIC PART

    // this part is maintaned only once for all HudBar Instances (there is only One Instance at any given time - else it breaks...)

    // FLT ATC Flightplan
    //private static FlightPlan m_atcFlightPlan = new FlightPlan(); // empty one


    /// <summary>
    /// The current ATC Flightplan (actually a copy of it)
    /// </summary>
    public static FlightPlan AtcFlightPlan => FltPlanMgr.FlightPlan;


    /// <summary>
    /// The Ping Library
    /// </summary>
    public static PingLib.Loops PingLoop => m_ping;
    private static readonly PingLib.Loops m_ping = new PingLib.Loops(); // Ping, Tone=0 will be Silence
    private static readonly PingLib.SoundBite _silentLoop = new PingLib.SoundBite( PingLib.Melody.Silence,0,0,0f); // Use this Sound to set to Silence

    public static PingLib.SoundBite LoopSound => _soundLoop;
    private static readonly PingLib.SoundBite _soundLoop = new PingLib.SoundBite( PingLib.Melody.Synth_3,0,0.33333f,0,0.2f); // Use this Sound to ping

    /// <summary>
    /// The Speech Library
    /// </summary>
    public static GUI_Speech SpeechLib => m_speech;
    private static readonly GUI_Speech m_speech = new GUI_Speech(); // Speech

    // Voice Output Package
    private static readonly HudVoice m_voicePack = new HudVoice(m_speech);


    // Descriptive Configuration GUI label names to match the BarItems LItem Enum (sequence does not matter)
    private static Dictionary<LItem,string> m_cfgNames = new Dictionary<LItem, string>(){
      {LItem.MSFS, DI_MsFS.Desc },
      {LItem.SimRate, DI_SimRate.Desc },      {LItem.FPS, DI_Fps.Desc },
      {LItem.LOG, DI_FlightLog.Desc },
      {LItem.ACFT_ID, DI_Acft_ID.Desc },
      {LItem.TIME, DI_Time.Desc },            {LItem.ZULU, DI_ZuluTime.Desc },  {LItem.CTIME, DI_CompTime.Desc },

      {LItem.ETRIM, DI_ETrim.Desc },          {LItem.RTRIM, DI_RTrim.Desc },    {LItem.ATRIM, DI_ATrim.Desc },
      {LItem.A_ETRIM, DI_A_ETrim.Desc},

      {LItem.OAT_C, DI_Oat_C.Desc },          {LItem.OAT_F, DI_Oat_F.Desc },
      {LItem.VIS, DI_Vis.Desc },
      {LItem.WIND_SD, DI_Wind_SD.Desc },      {LItem.WIND_XY, DI_Wind_XY.Desc },
      {LItem.BARO_HPA, DI_Baro_HPA.Desc },    {LItem.BARO_InHg, DI_Baro_InHg.Desc },
      {LItem.GEAR, DI_Gear.Desc },            {LItem.BRAKES, DI_Brakes.Desc },
      {LItem.FLAPS, DI_Flaps.Desc },          {LItem.SPOILER, DI_Spoilers.Desc },
      {LItem.FLAPS_ANI, DI_FlapsGraph.Desc }, {LItem.SPOILER_ANI, DI_SpoilersGraph.Desc },
      {LItem.Lights, DI_Lights.Desc },
      {LItem.XPDR, DI_Xpdr.Desc },

      {LItem.MAN, DI_Man.Desc },
      {LItem.TORQ, DI_Torq.Desc },            {LItem.TORQP, DI_TorqP.Desc },        {LItem.TORQP_ANI, DI_TorqPGraph.Desc },
      {LItem.PRPM, DI_PRpm.Desc },            {LItem.ERPM, DI_ERpm.Desc },
      {LItem.PRPM_ANI, DI_PRpmGraph.Desc },   {LItem.ERPM_ANI, DI_ERpmGraph.Desc },
      {LItem.N1, DI_N1.Desc },                {LItem.N2, DI_N2.Desc },
      {LItem.N1_ANI, DI_N1Graph.Desc },       {LItem.N2_ANI, DI_N2Graph.Desc },
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
      {LItem.GPS_WP_DIST, DI_Gps_WP_Dist.Desc },
      {LItem.GPS_WP_ETE, DI_Gps_WP_Ete.Desc },{LItem.GPS_ETE, DI_Gps_ETE.Desc },
      {LItem.GPS_BRGm, DI_Gps_BRGm.Desc },
      {LItem.GPS_TRK, DI_Gps_TRK.Desc },      {LItem.GPS_DTRK, DI_Gps_DTRK.Desc },
      {LItem.GPS_XTK, DI_Gps_XTK.Desc },
      {LItem.GPS_GS, DI_Gps_GS.Desc },
      {LItem.GPS_ALT, DI_Gps_ALT.Desc },
      {LItem.GPS_LAT_LON, DI_Gps_LatLon.Desc },
      {LItem.EST_VS,DI_Est_VS.Desc },         {LItem.EST_ALT, DI_Est_ALT.Desc },
      {LItem.ENROUTE, DI_Enroute.Desc },

      {LItem.HDG, DI_Hdg.Desc },              {LItem.HDGt, DI_HdgT.Desc },
      {LItem.ALT, DI_Alt_eff.Desc },          {LItem.ALT_INST, DI_Alt_Inst.Desc },
      {LItem.RA, DI_Ra.Desc },                {LItem.RA_VOICE, DI_Ra_Voice.Desc },
      {LItem.IAS, DI_Ias.Desc },
      {LItem.TAS, DI_Tas.Desc },
      {LItem.MACH, DI_Mach.Desc },
      {LItem.VS, DI_Vs.Desc },                {LItem.VS_PM, DI_Vs_PM.Desc },
      {LItem.VARIO_MPS, DI_VarioTE_mps_PM.Desc }, {LItem.VARIO_KTS, DI_VarioTE_kts_PM.Desc }, {LItem.VARIO_ANI, DI_VarioGraph.Desc },
      {LItem.AOA, DI_Aoa.Desc },              {LItem.FP_ANGLE, DI_FPAngle.Desc },             {LItem.ESI_ANI, DI_ESIGraph.Desc },
      {LItem.GFORCE, DI_GForce.Desc },        {LItem.GFORCE_MM, DI_Gforce_MM.Desc },

      {LItem.AP, DI_Ap.Desc },
      {LItem.AP_HDGs, DI_Ap_HdgSet.Desc },
      {LItem.AP_ALTs, DI_Ap_AltSet.Desc },
      {LItem.AP_VSs, DI_Ap_VsSet.Desc },
      {LItem.AP_FLCs, DI_Ap_FlcSet.Desc },
      {LItem.AP_BC, DI_Ap_BC.Desc },
      {LItem.AP_NAVg, DI_Ap_NavGps.Desc },
      {LItem.AP_APR_GS, DI_Ap_AprGs.Desc },
      {LItem.AP_YD, DI_Ap_YD.Desc },
      {LItem.AP_LVL, DI_Ap_LVL.Desc },
      {LItem.AP_APR_INFO, DI_Ap_ApproachMode.Desc },

      {LItem.NAV1, DI_Nav1.Desc },            {LItem.NAV2, DI_Nav2.Desc },
      {LItem.NAV1_NAME, DI_Nav1_Name.Desc },  {LItem.NAV2_NAME, DI_Nav2_Name.Desc },

      {LItem.ATC_APT, DI_Atc_APT.Desc },
      {LItem.ATC_RWY, DI_Atc_RWY.Desc },
      {LItem.ATC_ALT_HDG, DI_Atc_AltHdg.Desc },
      {LItem.METAR, DI_Metar.Desc },

      {LItem.M_TIM_DIST1, DI_M_TimDist1.Desc },{LItem.M_TIM_DIST2, DI_M_TimDist2.Desc }, {LItem.M_TIM_DIST3, DI_M_TimDist3.Desc },

      {LItem.THR_LEV, DI_Thr_LEV.Desc },      {LItem.MIX_LEV, DI_Mix_LEV.Desc }, {LItem.PROP_LEV, DI_Prop_LEV.Desc },

    };

    #endregion  // STATIC




    /// <summary>
    /// The currently used profile settings for the Bar
    /// </summary>
    public CProfile Profile => m_profile;
    private CProfile m_profile = null;     // currently used profile

    /// <summary>
    /// Show Units if true
    /// </summary>
    public bool ShowUnits { get; private set; } = false;

    /// <summary>
    /// The Configured Hotkeys
    /// </summary>
    public WinHotkeyCat Hotkeys { get; private set; }

    /// <summary>
    /// Use Keyboard Hook if true
    /// </summary>
    public bool KeyboardHook { get; private set; } = false;

    /// <summary>
    /// Use InGame Hook if true
    /// </summary>
    public bool InGameHook { get; private set; } = false;

    /// <summary>
    /// Use Flight Recorder if true
    /// </summary>
    public bool FlightRecorder { get; private set; } = false;

    /// <summary>
    /// FLT File AutoSave and FlightPlan Handler Enabled
    /// </summary>
    public FSimClientIF.FlightPlanMode FltAutoSave { get; private set; } = FSimClientIF.FlightPlanMode.Disabled;

    /// <summary>
    /// The used VoiceName
    /// </summary>
    public string VoiceName { get; private set; } = "";

    /// <summary>
    /// Provide acces to the VoicePack
    /// </summary>
    public HudVoice VoicePack => m_voicePack;

    /// <summary>
    /// Enable or disable the Voice Out
    /// </summary>
    public bool VoiceEnabled {
      get => m_speech.Enabled;
      set => m_speech.Enabled = value;
    }

    /// <summary>
    /// The used FlightBag Folder
    /// </summary>
    public string ShelfFolder { get; private set; } = "";



    /// <summary>
    /// Returns the Show state of an item
    /// </summary>
    /// <param name="item">A Label item</param>
    /// <returns>True if it is shown</returns>
    public bool ShowItem( LItem item ) => m_profile.ShowItem( item );

    /// <summary>
    /// The Current FontSize to use
    /// </summary>
    public FontSize FontSize => m_profile.FontSize;

    /// <summary>
    /// Placement of the Bar
    /// </summary>
    public Placement Placement => m_profile.Placement;

    /// <summary>
    /// Display Kind of the Bar
    /// </summary>
    public Kind Kind => m_profile.Kind;

    /// <summary>
    /// The Tooltip control for the FP
    /// </summary>
    public X_ToolTip ToolTipFP => m_toolTipFP;
    private X_ToolTip m_toolTipFP = new X_ToolTip();     // Our Custom Mono ToolTip for the Flight path


    // maintain collections of the created Controls to do the processing
    // One collection contains the actionable interface
    // The second collection the WinForm Control itself (to act on the Control interface)

    // The Display Groups (DispItem is essentially a smaller FlowLayoutPanel containing the Label and 0, 1 or more Value Items)
    private DispItemCat  m_dispItems = new DispItemCat();

    // Value Items (updated from Sim, some may change colors based on conditions)
    private ValueItemCat m_valueItems = new ValueItemCat();

    // The Font provider
    private static GUI_Fonts FONTS = null; // handle fonts as static item to not waste resources

    /// <summary>
    /// cTor: Of a completely new HudBar
    /// 
    /// Init the Hud Items - providing prototypes for the various label types and Config strings/values from AppSettings
    /// </summary>
    /// <param name="lblProto">A GUI prototype label, carrying fonts, colors etc (set in GUI design mode)</param>
    /// <param name="valueProto">A GUI prototype value, carrying fonts, colors etc (set in GUI design mode)</param>
    /// <param name="value2Proto">A GUI prototype value type 2, carrying fonts, colors etc (set in GUI design mode)</param>
    /// <param name="signProto">A GUI prototype icon(Wingdings), carrying fonts, colors etc (set in GUI design mode)</param>
    /// <param name="showUnits">Showing units flag</param>
    /// <param name="cProfile">The current Profile</param>
    /// <param name="voiceName">The current VoiceName</param>
    /// <param name="fRecorder">FlightRecorder Enabled</param>
    public HudBar( Label lblProto, Label valueProto, Label value2Proto, Label signProto,
                      bool showUnits, bool keyboardHook, bool inGameHook, WinHotkeyCat hotkeys,
                      int autoSave, string shelfFolder, CProfile cProfile, string voiceName, bool fRecorder )
    {
      // just save them in the HUD mainly for config purpose
      m_profile = cProfile;
      ShowUnits = showUnits;
      Hotkeys = hotkeys.Copy( );
      KeyboardHook = keyboardHook;
      InGameHook = inGameHook;
      FltAutoSave = (FSimClientIF.FlightPlanMode)autoSave;
      VoiceEnabled = false; // disable, else we get early talks..
      VoiceName = voiceName;
      FlightRecorder = fRecorder;
      _ = m_speech.SetVoice( VoiceName );
      ShelfFolder = shelfFolder;

      PingLoop.PlayAsync( _silentLoop ); // use a default Silence Sound (to kill any ping) when restarting

      // Reset the observers as we rebuild the GUI now 
      SC.SimConnectClient.Instance.ClearAllObservers( );

      // Reset the registered items for color handling - we create all new here
      GUI_Colors.ClearRegistry( );

      // Reset our own ToolTip control
      ToolTipFP.ResetDrawList( );

      // init the Fontprovider from the submitted labels
      FONTS = new GUI_Fonts( m_profile.Condensed ); // get all fonts from built in
      // set desired size
      FONTS.SetFontsize( m_profile.FontSize );

      // and prepare the prototype Controls used below - not really clever but handier to have all label defaults ....
      lblProto.Font = FONTS.LabelFont;
      valueProto.Font = FONTS.ValueFont;
      value2Proto.Font = FONTS.Value2Font;
      signProto.Font = FONTS.SignFont;

      // reset all Catalogs before creating this Instance
      m_valueItems.Clear( );
      m_dispItems.Clear( );

      // The pattern below repeats, create the display group and add it to the group list
      // All visual handling is done in the respective Class Implementation
      // Data Update is triggered by subscribing with the SimUpdate from the created object

      // Add all Display Items with prepared prototype labels (sequence does not matter)

      // Sim Status
      m_dispItems.AddDisp( new DI_MsFS( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_SimRate( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Fps( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_FlightLog( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // Environment
      m_dispItems.AddDisp( new DI_Time( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_ZuluTime( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_CompTime( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Oat_C( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Oat_F( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Vis( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Wind_SD( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Wind_XY( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      // Aircraft
      m_dispItems.AddDisp( new DI_Acft_ID( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Baro_HPA( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Baro_InHg( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Xpdr( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Gear( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Brakes( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Flaps( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_FlapsGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_Spoilers( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_SpoilersGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_Lights( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // Engine
      m_dispItems.AddDisp( new DI_Man( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Torq( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_TorqP( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_TorqPGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_PRpm( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_PRpmGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_ERpm( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_ERpmGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_N1( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_N1Graph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_N2( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_N2Graph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_Afterburner( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_AfterburnerGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_Itt_C( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Egt_C( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Egt_F( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Cht_C( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Cht_F( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Load_prct( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_CowlFlapsGraph( m_valueItems, lblProto ) );
      // Fuel
      m_dispItems.AddDisp( new DI_FFlow_PPH( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_FFlow_GPH( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_FFlow_KGH( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Fuel_LR_Gal( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Fuel_LR_Lb( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Fuel_LR_Kg( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Fuel_C_Gal( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Fuel_C_Lb( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Fuel_C_Kg( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Fuel_Total_Gal( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Fuel_Total_Lb( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Fuel_Total_Kg( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_FuelGraph( m_valueItems, lblProto ) );
      // Trim
      m_dispItems.AddDisp( new DI_ETrim( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_A_ETrim( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_RTrim( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_ATrim( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // GPS
      m_dispItems.AddDisp( new DI_Gps_TRK( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Gps_GS( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Gps_ETE( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gps_WYP( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gps_WP_Dist( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Gps_WP_Ete( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Gps_BRGm( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Gps_DTRK( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Gps_XTK( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Gps_ALT( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Gps_LatLon( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Est_VS( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Est_ALT( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Enroute( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // Aeronautics
      m_dispItems.AddDisp( new DI_Hdg( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_HdgT( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Alt_eff( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Alt_Inst( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Ra( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Ra_Voice( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Ias( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Tas( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Mach( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Vs( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Vs_PM( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_VarioTE_mps_PM( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_VarioTE_kts_PM( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_VarioGraph( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Aoa( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_FPAngle( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_ESIGraph( m_valueItems, lblProto ) );
      m_dispItems.AddDisp( new DI_GForce( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Gforce_MM( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Nav1( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Nav2( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Nav1_Name( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Nav2_Name( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      // Autopilot
      m_dispItems.AddDisp( new DI_Ap( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_HdgSet( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Ap_AltSet( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Ap_VsSet( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Ap_FlcSet( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Ap_BC( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_NavGps( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_AprGs( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_YD( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_LVL( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Ap_ApproachMode( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      // ATC
      m_dispItems.AddDisp( new DI_Atc_APT( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Atc_RWY( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_Atc_AltHdg( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      // METAR
      m_dispItems.AddDisp( new DI_Metar( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      // METERS
      m_dispItems.AddDisp( new DI_M_TimDist1( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_M_TimDist2( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      m_dispItems.AddDisp( new DI_M_TimDist3( m_valueItems, lblProto, valueProto, value2Proto, signProto, showUnits ) );
      // Controls
      m_dispItems.AddDisp( new DI_Thr_LEV( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Mix_LEV( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );
      m_dispItems.AddDisp( new DI_Prop_LEV( m_valueItems, lblProto, valueProto, value2Proto, signProto ) );

      // **** post processing

      // Apply Unit modifier (shown, not shown) to all Values
      foreach ( var lx in m_valueItems ) {
        lx.Value.Value.ShowUnit = ShowUnits;
      }

      // Align the Vertical alignment accross the bar
      if ( m_profile.Placement == Placement.Top || m_profile.Placement == Placement.Bottom ) {
        // Set min size of the labels in order to have them better aligned
        // Using arrow chars gets a larger height than regular ASCII chars (odd autosize behavior)
        // Setting minSize to the Max found height of any at least allows for some hotizontal alignment
        int mh = 0;
        foreach ( var lx in m_valueItems ) {
          mh = ( lx.Value.Ctrl.Height > mh ) ? lx.Value.Ctrl.Height : mh;
        }
        foreach ( var lx in m_dispItems ) {
          mh = ( lx.Value.Label.Height > mh ) ? lx.Value.Label.Height : mh;
        }
        // define MinHeight for value Labels
        foreach ( var lx in m_valueItems ) {
          lx.Value.Ctrl.MinimumSize = new Size( 1, mh );
        }
        // define MinHeight for the Group Labels
        foreach ( var lx in m_dispItems ) {
          lx.Value.Label.MinimumSize = new Size( 1, mh );
        }
      }

      // Align the Value columns on left and right bound bar or tile
      if ( m_profile.Placement == Placement.Left || m_profile.Placement == Placement.Right ) {
        // Determine max width and make them aligned
        int maxLabelWidth = 0;
        int max1ValueWidth = 0; // the Single Value DispItems Value label 
        Queue<int> max1ValueWidthList = new Queue<int>();
        foreach ( var lItem in Profile.ItemPosList( ) ) {
          var dix = DispItem( lItem );
          if ( dix != null ) {
            maxLabelWidth = ( dix.Controls[0].Width > maxLabelWidth ) ? dix.Controls[0].Width : maxLabelWidth;
            // collect per column
            if ( Profile.BreakItem( lItem ) ) {
              max1ValueWidthList.Enqueue( max1ValueWidth );
              max1ValueWidth = 0;
            }
            // eval the Single Value items
            if ( dix.Controls.Count == 2 ) {
              max1ValueWidth = ( dix.Controls[1].Width > max1ValueWidth ) ? dix.Controls[1].Width : max1ValueWidth;
            }
          }
        }
        max1ValueWidthList.Enqueue( max1ValueWidth ); // last width

        // pad the label control to the right to have the value columns aligned
        max1ValueWidth = max1ValueWidthList.Dequeue( );
        foreach ( var lItem in Profile.ItemPosList( ) ) {
          var dix = DispItem( lItem );
          if ( dix != null ) {
            // get the next column width
            if ( Profile.BreakItem( lItem ) ) {
              max1ValueWidth = max1ValueWidthList.Dequeue( );
            }
            if ( dix.Controls.Count == 2 && !( dix.Controls[1] is V_Steps ) && !( dix.Controls[1] is V_Text ) ) {
              dix.Controls[0].Padding = new Padding( 0, 0, maxLabelWidth - dix.Controls[0].Width, 0 );
              // align single Value ones to the max right (pad left)
              dix.Controls[1].Padding = new Padding( max1ValueWidth - dix.Controls[1].Width, 0, 0, 0 );
            }
            else {
              // others just column align the label
              dix.Controls[0].Padding = new Padding( 0, 0, maxLabelWidth - dix.Controls[0].Width, 0 );
            }
          }
        }
      }
      // VoicePack Data Registration (as we cleared all above..)
      m_voicePack.RegisterObservers( );
    }

    #region Update Content and Settings

    // track RefSpeed changes
    int _RefSpeedsTTHash = 0;

    /// <summary>
    /// Update from values from Sim which are not part of the Item content update 
    /// </summary>
    public void UpdateGUI( string dataRefName )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return; // sanity..

      // update calculations
      Calculator.PaceCalculator( );

      // ATC Flightplan  - LOCK the flightplan while using it, else the Asynch Update may change it ..
      // ATC Airport
      AirportMgr.Update( AtcFlightPlan.Destination ); // maintain APT (we should always have a Destination here)
                                                      // Load Remaining Plan if the WYP or Flightplan has changed
      if ( WPTracker.HasChanged || FltPlanMgr.HasChanged ) {
        string tt = AtcFlightPlan.RemainingPlan( WPTracker.Read( ) );
        var di = this.DispItem( LItem.GPS_WYP );
        if ( di != null ) {
          this.ToolTipFP.SetToolTip( di.Label, tt );
          di.Label.Cursor = string.IsNullOrEmpty( tt ) ? Cursors.Default : Cursors.PanEast;
        }
        tt = AtcFlightPlan.WaypointByName( WPTracker.PrevWP ).PrettyDetailed;
        var vc = this.ValueControl( VItem.GPS_PWYP );
        if ( vc != null ) {
          this.ToolTipFP.SetToolTip( vc, tt );
          vc.Cursor = string.IsNullOrEmpty( tt ) ? Cursors.Default : Cursors.PanEast;
        }
        tt = AtcFlightPlan.WaypointByName( WPTracker.NextWP ).PrettyDetailed;
        vc = this.ValueControl( VItem.GPS_NWYP );
        if ( vc != null ) {
          this.ToolTipFP.SetToolTip( vc, tt );
          vc.Cursor = string.IsNullOrEmpty( tt ) ? Cursors.Default : Cursors.PanEast;
        }
        tt = AtcFlightPlan.Pretty;
        di = this.DispItem( LItem.ATC_ALT_HDG );
        if ( di != null ) {
          this.ToolTipFP.SetToolTip( di.Label, tt );
          di.Label.Cursor = string.IsNullOrEmpty( tt ) ? Cursors.Default : Cursors.PanEast;
        }
        // commit that we read the changes of the Flight Plan
        FltPlanMgr.Read( );
      }

      // check for the Ref Speeds every 30 sec - there is no event which let's us know that the acft or Ref Speeds have changed..
      if ( ( SC.SimConnectClient.Instance.HudBarModule.SimTime_zulu_sec % 30 ) == 0 ) {
        string tt="";
        if ( !string.IsNullOrWhiteSpace( SC.SimConnectClient.Instance.HudBarModule.AcftConfigFile ) ) {
          var ds = SC.SimConnectClient.Instance.HudBarModule;
          tt = $"Aircraft Type:     {ds.AcftConfigFile}\n\n"
             + $"Cruise Altitude:   {ds.DesingCruiseAlt_ft:##,##0} ft\n"
             + $"Vc  Cruise Speed:  {ds.DesingSpeedVC_kt:##0} kt\n"
             + $"Vy  Climb Speed    {ds.DesingSpeedClimb_kt:##0} kt\n"
             + $"Vmu Takeoff Speed: {ds.DesingSpeedTakeoff_kt:##0} kt\n"
             + $"Vr  Min Rotation:  {ds.DesingSpeedMinRotation_kt:##0} kt\n"
             + $"Vs1 Stall Speed:   {ds.DesingSpeedVS1_kt:##0} kt\n"
             + $"Vs0 Stall Speed:   {ds.DesingSpeedVS0_kt:##0} kt\n"
             + $"Empty Weight:      {ds.EmptyAcftWeight_lbs:##0} lbs\n"
             + $"Max. Weight:       {ds.MaxAcftWeight_lbs:##0} lbs\n"
             + $"TOTAL Weight:      {ds.TotalAcftWeight_lbs:##0} lbs\n"
             ;
        }
        // has it changed?
        var di = this.DispItem( LItem.IAS );
        if ( di != null ) {
          if ( _RefSpeedsTTHash != tt.GetHashCode( ) ) {
            this.ToolTipFP.SetToolTip( di.Label, tt );
            _RefSpeedsTTHash = tt.GetHashCode( );
          }
          di.Label.Cursor = string.IsNullOrEmpty( tt ) ? Cursors.Default : Cursors.PanEast;
        }
      }
    }


    /// <summary>
    /// Set the current show unit flag communicated by the HUD
    /// </summary>
    /// <param name="showUnits"></param>
    public void SetShowUnits( bool showUnits )
    {
      ShowUnits = showUnits;
    }

    /// <summary>
    /// Set the Hotkey Catalog
    /// </summary>
    /// <param name="hotkeys"></param>
    public void SetHotkeys( WinHotkeyCat hotkeys )
    {
      Hotkeys = hotkeys.Copy( );
    }

    /// <summary>
    /// Set the current keyboardHook flag communicated by the HUD
    /// </summary>
    /// <param name="keyboardHook"></param>
    public void SetKeyboardHook( bool keyboardHook )
    {
      KeyboardHook = keyboardHook;
    }

    /// <summary>
    /// Set the current InGame Hook flag communicated by the HUD
    /// </summary>
    /// <param name="inGameHook"></param>
    public void SetInGameHook( bool inGameHook )
    {
      InGameHook = inGameHook;
    }

    /// <summary>
    /// Set the Flight Recorder enabled flag
    /// </summary>
    /// <param name="fRec"></param>
    public void SetFlightRecorder( bool fRec )
    {
      FlightRecorder = fRec;
    }

    /// <summary>
    /// Set the current FltAutoSave communicated by the HUD
    /// </summary>
    /// <param name="opacity"></param>
    public void SetFltAutoSave( FSimClientIF.FlightPlanMode autoSave )
    {
      FltAutoSave = autoSave;
    }

    /// <summary>
    /// Set the current VoiceName communicated by the HUD
    /// </summary>
    /// <param name="voiceName"></param>
    public void SetVoiceName( string voiceName )
    {
      VoiceName = voiceName;
    }

    /// <summary>
    /// Set the current Flight Bag Folder
    /// </summary>
    /// <param name="folder"></param>
    public void SetShelfFolder( string folder )
    {
      ShelfFolder = folder;
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
        if ( ( vi == null ) || ( vi.Ctrl == null ) || vi.Ctrl.IsDisposed ) return null;
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
        if ( ( vi == null ) || ( vi.Ctrl == null ) || vi.Ctrl.IsDisposed ) return null;
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
        if ( ( vi == null ) || ( vi.Ctrl == null ) || vi.Ctrl.IsDisposed ) return null;
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
      try {
        var di = m_dispItems[item];
        if ( ( di == null ) || di.IsDisposed ) return null;
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
      try {
        return m_cfgNames[item];
      }
      catch {
        return $"Cfg Name undef {item}";
      }
    }

    #endregion

  }
}

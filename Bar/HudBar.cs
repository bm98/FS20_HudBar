using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FSimClientIF.Flightplan;
using SC = SimConnectClient;

using CoordLib;
using MetarLib;

using static FS20_HudBar.GUI.GUI_Colors;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.GUI;

//using FS20_HudBar.Bar.FltLib;

namespace FS20_HudBar.Bar
{
  /// <summary>
  /// All Labels (Topics etc.) shown in the hudBar 
  /// These items are used to Configure (on / off) 
  /// THE SEQUENCE MATCHES THE CONFIG STRING IN THE PROFILE APPSETTINGS.
  /// ADD NEW ITEMS ONLY AT THE END OF THE LIST (else the existing user config is screwed up)
  /// </summary>
  internal enum LItem
  {
    MSFS = 0, // MSFS connection status...
    SimRate,
    ETrim,    // Elevator Trim
    RTrim,    // Rudder Trim
    ATrim,    // Aileron Trim
    OAT,
    BARO_HPA,
    BARO_InHg,
    Gear,
    Brakes,
    Flaps,
    TORQ,     // Torque ft/lb
    TORQP,    // Torque %
    PRPM,     // Prop RPM
    ERPM,     // Engine RPM
    N1,
    ITT,
    EGT,
    FFlow_pph,    // pounds ph
    FFlow_gph,    // gallons ph
    GPS_WYP,    // prev-next WYP
    GPS_WP_DIST,  // GPS Distance to next Waypoint
    GPS_WP_ETE,   // GPS Time to next Waypoint
    GPS_TRK,
    GPS_GS,
    GPS_ALT,    // Next WYP Alt
    EST_VS,     // Estimate VS needed to WYP
    EST_ALT,     // Estimate ALT at WYP
    HDG,
    ALT,
    RA,
    IAS,
    VS,
    AP,
    AP_HDGs,
    AP_ALTs,
    AP_VSs,
    AP_FLCs,
    AP_NAVg,
    AP_APR_GS,

    M_TIM_DIST1,  // Checkpoint 1
    M_TIM_DIST2,  // Checkpoint 2
    M_TIM_DIST3,  // Checkpoint 3

    A_ETRIM,      // Auto ETrim

    MAN,          // MAN Pressure inHg

    GPS_BRGm,  // GPS BRG to Waypoint 000°
    GPS_DTRK,  // GPS Desired Track to Waypoint 000°
    GPS_XTK,   // GPS CrossTrack Error nm

    AOA,      // Angle of attack deg
    TAS,      // true airspeed kt
    ACFT_ID,  // aircraft ID

    WIND_SD,  // Wind Speed and Direction
    WIND_XY,  // Wind X and Y Component

    Lights,   // Lights indication
    Fuel_LR,    // Fuel quantity Left/Right
    Fuel_Total, // Fuel quantity Total

    MACH,       // Mach speed indication

    VIS,        // Visibility
    TIME,       // Time of Day
    HDGt,       // True heading
    AP_BC,      // AP BC signal
    AP_YD,      // Yaw Damper
    AP_LVL,     // Wing Leveler

    ENROUTE,    // Enroute Times
    ATC_APT,    // ATC assigned Apt arrives as TT.AIRPORTLR.ICAO.name
    ATC_RWY,    // ATC assigned RWY displacement
    GPS_ETE,    // GPS Time to Destination

    GPS_LAT_LON,// Lat and Lon from GPS
    METAR,      // METAR close to Lat and Lon
    ALT_INST,   // Instrument Altitude

    ATC_ALT_HDG, // ATC assigned ALT and HDG

    VS_PM,      // Vertical Speed with +-

    RA_VOICE,     // Radio Altitude with voice output

    NAV1,       // NAV1
    NAV2,       // NAV2
    //    GPS_APT_APR,// GPS Airport & Approach - SIM PROVIDES EMPTY STRINGS ...
  }


  /// <summary>
  /// All item values shown in the hudBar 
  /// These are to access and do the processing
  /// Those may include Engine 1/2 details
  /// ADD NEW ITEMS ONLY AT THE END OF THE LIST (else the existing user config is screwed up)
  /// </summary>
  internal enum VItem
  {
    Ad = 0,   // MSFS connection status...
    SimRate,  // simulation rate

    ETrim,      // Elevator Trim +-N%, active: set to zero
    RTrim,      // Rudder Trim +-N%, active: set to zero
    ATrim,      // Aileron Trim +-N%, active: set to zero

    OAT,        // Outside AirTemp °C
    BARO_HPA,   // Altimeter Setting HPA
    BARO_InHg,  // Altimeter Setting InHg

    Gear,       // Gear Up/Down
    Brakes,     // Brakes On Off
    Flaps,      // Flaps N level 

    E1_TORQP,   // Engine1 Torque %
    E2_TORQP,   // Engine2 Torque %
    E1_TORQ,    // Engine1 Torque ft/lb
    E2_TORQ,    // Engine2 Torque ft/lb

    P1_RPM,     // Prop1 RPM
    P2_RPM,     // Prop2 RPM
    E1_RPM,     // Engine1 RPM
    E2_RPM,     // Engine2 RPM
    E1_N1,      // Engine1 N1 %
    E2_N1,      // Engine2 N1 %

    E1_ITT,     // ITT 1 Celsius
    E2_ITT,     // ITT 2 Celsius
    E1_EGT,     // EGT 1 Celsius
    E2_EGT,     // EGT 2 Celsius

    E1_FFlow_pph,  // Fuel1 Flow pph
    E2_FFlow_pph,  // Fuel2 Flow pph

    E1_FFlow_gph,  // Fuel1 Flow gph
    E2_FFlow_gph,  // Fuel2 Flow gph

    GPS_PWYP,     // GPS Prev Waypoint
    GPS_NWYP,     // GPS Next Waypoint
    GPS_WP_DIST,  // GPS Distance to next Waypoint
    GPS_WP_ETE,   // GPS Time to next Waypoint
    GPS_TRK,      // GPS Track 000°
    GPS_GS,       // GPS Groundspeed 000kt

    GPS_ALT,      // GPS next Waypoint Altitude
    EST_VS,       // Estimate VS to reach WYP@Altitude
    EST_ALT,      // Estimate ALT@WYP

    HDG,          // Heading Mag 000°
    ALT,          // Altitude 00000 ft
    RA,           // Radio Altitude 000 ft 
    IAS,          // Ind. Airspeed 000 kt
    VS,           // Vertical Speed +-0000 fpm

    AP,           // Autopilot On/Off
    AP_HDG,       // AP HDG active
    AP_HDGset,    // AP HDG set
    AP_ALT,       // AP ALT hold active
    AP_ALTset,    // AP ALT set
    AP_VS,        // AP VS hold active
    AP_VSset,     // AP VS set
    AP_FLC,       // AP FLC hold active
    AP_FLCset,    // AP FLC IAS set
    AP_NAV,       // AP NAV active
    AP_NAVgps,    // AP NAV follow GPS
    AP_APR,       // AP APR hold active
    AP_GS,        // AP GS  hold active

    M_Elapsed1,   // Time elapsed since start of CP1
    M_Dist1,      // Distance from CP1
    M_Elapsed2,   // Time elapsed since start of CP2
    M_Dist2,      // Distance from CP2
    M_Elapsed3,   // Time elapsed since start of CP3
    M_Dist3,      // Distance from CP3

    A_ETRIM,      // Auto ETrim, activates ETrim Module, shows ETrim % (same as the standard one)

    E1_MAN,       // Man Pressure InHg
    E2_MAN,       // Man Pressure InHg

    GPS_BRGm,     // GPS Mag BRG to Waypoint 000°
    GPS_DTRK,     // GPS Desired Track to Waypoint 000°
    GPS_XTK,      // GPS CrossTrack Error nm

    AOA,          // Angle of attack deg
    TAS,          // true airspeed kt
    ACFT_ID,      // aircraft ID

    WIND_DIR,     // Wind direction °
    WIND_SPEED,   // Wind speed kt
    WIND_LAT,     // Wind lateral comp kt
    WIND_LON,     // Wind longitudinal comp kt

    Lights,       // Lights indication

    Fuel_Left,    // Fuel quantity Left Gallons
    Fuel_Right,   // Fuel quantity Right Gallons
    Fuel_Total,   // Fuel quantity Total Gallons
    Fuel_Reach,   // Fuel reach in seconds

    MACH,         // Mach speed indication

    VIS,          // Visibility nm
    TIME,         // Time of Day
    HDGt,         // True heading deg
    AP_BC,        // AP BC signal
    AP_YD,        // Yaw Damper signal
    AP_LVL,       // Wing Leveler signal

    ENR_WP,       // Enroute time for this WP sec
    ENR_TOTAL,    // Enroute time for this flight sec
    ATC_APT,      // ATC assigned Airport
    ATC_RWY_LAT,  // Lateral displacement ft
    ATC_RWY_ALT,  // Height displacement ft
    ATC_RWY_LON,  // Longitudinal displacement nm
    GPS_ETE,      // GPS Time to Destination

    GPS_LAT,      // Latitude
    GPS_LON,      // Longitude
    ATC_DIST,     // Distance from ATC Apt nm (lat lon based)
    METAR,        // METAR close to LAT,LON
    ALT_INST,     // Instrument Altitude ft

    ATC_ALT,      // ATC assigned ALT ft
    ATC_HDG,      // ATC assigned HDG °
    ATC_WYP,      // ATC next Wyp

    VS_PM,        // Vertical Speed with +- fpm

    RA_VOICE,     // Radio Altitude with voice output

    NAV1_ID,      // NAV1 ID
    NAV1_DST,     // NAV1 DME Distance
    NAV1_BRG,     // NAV1 BRG to

    NAV2_ID,      // NAV2 ID
    NAV2_DST,     // NAV2 DME Distance
    NAV2_BRG,     // NAV2 BRG to

    //GPS_APT,      // GPS Airport ID - SIM PROVIDES EMPTY STRINGS ...
    //GPS_APR,      // GPS Approach ID - SIM PROVIDES EMPTY STRINGS ...
  }

  /// <summary>
  /// Instance of the current HudBar
  /// will initialize from profile settings
  /// </summary>
  internal class HudBar
  {
    #region STATIC PART

    // this part is maintaned only once for all HudBar Instances (there is only One at a given time)

    /// <summary>
    /// cTor: Static init
    /// </summary>
    static HudBar( )
    {
      // Connect handlers to get updates
      m_metarApt.MetarDataEvent += METARapt_MetarDataEvent;
      m_metarLoc.MetarDataEvent += METARloc_MetarDataEvent;
      //FltMgr.NewFlightPlan += FLT_NewFlightPlan;
    }


    // METAR providers
    private static readonly HudMetar m_metarApt = new HudMetar( ); // For the Airport
    private static readonly HudMetar m_metarLoc = new HudMetar( ); // For the current location

    // save METARS when they arrive

    // For the Airport
    private static void METARapt_MetarDataEvent( object sender, MetarTafDataEventArgs e )
    {
      m_metarApt.Update( e.MetarTafData );
    }

    // For the Aircraft Location
    private static void METARloc_MetarDataEvent( object sender, MetarTafDataEventArgs e )
    {
      m_metarLoc.Update( e.MetarTafData );
    }


    /// <summary>
    /// The Airport Metar Handler
    /// </summary>
    public static HudMetar MetarApt => m_metarApt;
    /// <summary>
    /// The Location Metar Handler
    /// </summary>
    public static HudMetar MetarLoc => m_metarLoc;

    // FLT ATC Flightplan
    private static FlightPlan m_atcFlightPlan = new FlightPlan(); // empty one

    // 3 Checkpoint Meters
    private static readonly CPointMeter m_cpMeter1 = new CPointMeter();
    private static readonly CPointMeter m_cpMeter2 = new CPointMeter();
    private static readonly CPointMeter m_cpMeter3 = new CPointMeter();

    // Speech
    private static readonly GUI_Speech m_speech = new GUI_Speech();
    // Voice Output Package
    private static readonly HudVoice m_voicePack = new HudVoice(m_speech);

    /// <summary>
    /// The Checkpoint Meter 1
    /// </summary>
    public static CPointMeter CPMeter1 => m_cpMeter1;
    /// <summary>
    /// The Checkpoint Meter 1
    /// </summary>
    public static CPointMeter CPMeter2 => m_cpMeter2;
    /// <summary>
    /// The Checkpoint Meter 1
    /// </summary>
    public static CPointMeter CPMeter3 => m_cpMeter3;


    #endregion  // STATIC


    // Our Custom Mono ToolTip for the Flight path
    private X_ToolTip m_toolTipFP = new X_ToolTip(){
      // Set up the delays for the ToolTip.
      AutoPopDelay = 30_000, // looong
      InitialDelay = 800,
      ReshowDelay = 500,
      // Force the ToolTip text to be displayed whether or not the form is active.
      ShowAlways = true
    };

    // A standard ToolTip for the Metar Display
    private ToolTip m_toolTip = new ToolTip(){
      // Set up the delays for the ToolTip.
      AutoPopDelay = 10_000,
      InitialDelay = 800,
      ReshowDelay = 500,
      // Force the ToolTip text to be displayed whether or not the form is active.
      ShowAlways = true
    };

    // currently used profile
    private CProfile m_profile = null;

    /// <summary>
    /// The currently used profile settings for the Bar
    /// </summary>
    public CProfile Profile => m_profile;

    /// <summary>
    /// Show Units if true
    /// </summary>
    public bool ShowUnits { get; private set; } = false;

    /// <summary>
    /// FLT File AutoSave and FlightPlan Handler Enabled
    /// </summary>
    public bool FltAutoSave { get; private set; } = false;

    /// <summary>
    /// The used VoiceName
    /// </summary>
    public string VoiceName { get; private set; } = "";

    /// <summary>
    /// Provide acces to the VoicePack
    /// </summary>
    public HudVoice VoicePack => m_voicePack;

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
    /// The Tooltip control
    /// </summary>
    public ToolTip ToolTip => m_toolTip;

    /// <summary>
    /// The Tooltip control for the FP
    /// </summary>
    public X_ToolTip ToolTipFP => m_toolTipFP;

    // Hud Bar Label names to match the enum above (as short as possible)
    private Dictionary<LItem,string> m_guiNames = new Dictionary<LItem, string>(){
      {LItem.MSFS,"MSFS" },
      {LItem.SimRate,"SimRate" },
      {LItem.ACFT_ID,"ID" },
      {LItem.TIME,"Time" },

      {LItem.ETrim,"E-Trim" }, {LItem.RTrim,"R-Trim" }, {LItem.ATrim,"A-Trim" },
      {LItem.A_ETRIM,"A-ETrim" },
      {LItem.OAT,"OAT" },
      {LItem.VIS,"VIS" },
      {LItem.WIND_SD,"WIND" }, {LItem.WIND_XY,"WIND" },
      {LItem.BARO_HPA,"BARO" }, {LItem.BARO_InHg,"BARO" },
      {LItem.Gear,"Gear" }, {LItem.Brakes,"Brakes" }, {LItem.Flaps,"Flaps" },
      {LItem.Lights,"Lights" },
      {LItem.AOA,"AoA" },

      {LItem.MAN,"MAN" },
      {LItem.TORQ,"TORQ" },
      {LItem.TORQP,"TORQ" },
      {LItem.PRPM,"P-RPM" },
      {LItem.ERPM,"E-RPM" },
      {LItem.N1,"N1" },
      {LItem.ITT,"ITT" },
      {LItem.EGT,"EGT" },
      {LItem.FFlow_pph,"FFLOW" }, {LItem.FFlow_gph,"FFLOW" },
      {LItem.Fuel_LR,"F-LR" }, {LItem.Fuel_Total,"F-TOT" },

      {LItem.GPS_WYP,"≡GPS≡" },
      //{LItem.GPS_APT_APR,"APT" },  - SIM PROVIDES EMPTY STRINGS ...
      {LItem.GPS_WP_DIST,"DIST" },
      {LItem.GPS_WP_ETE, "ETE" },
      {LItem.GPS_ETE,"D-ETE" },
      {LItem.GPS_BRGm,"BRG" },
      {LItem.GPS_TRK, "TRK" },
      {LItem.GPS_DTRK,"DTK" },
      {LItem.GPS_XTK,"XTK" },
      {LItem.GPS_GS, "GS" },
      {LItem.GPS_ALT, "ALTP" },
      {LItem.EST_VS, "WP-VS" },
      {LItem.EST_ALT, "WP-ALT" },
      {LItem.ENROUTE, "Enroute" },

      {LItem.HDG,"HDG" },
      {LItem.HDGt,"HDGt" },
      {LItem.ALT,"ALTeff" },
      {LItem.ALT_INST,"ALT" },
      {LItem.RA,"RA" },
      {LItem.RA_VOICE,"RAv" },
      {LItem.IAS,"IAS" },
      {LItem.TAS,"TAS" },
      {LItem.MACH,"Mach" },
      {LItem.VS,"VS" },
      {LItem.VS_PM,"VS" },
      {LItem.GPS_LAT_LON,"POS" },

      {LItem.AP,"≡AP≡" },
      {LItem.AP_HDGs,"HDG" },
      {LItem.AP_ALTs,"ALT" },
      {LItem.AP_VSs,"VS" },
      {LItem.AP_FLCs,"FLC" },
      {LItem.AP_BC,"BC" },
      {LItem.AP_NAVg,"NAV" },
      {LItem.AP_APR_GS,"APR" },
      {LItem.AP_YD,"YD" },
      {LItem.AP_LVL,"LVL" },

      {LItem.NAV1,"NAV 1" },
      {LItem.NAV2,"NAV 2" },

      {LItem.ATC_APT,"APT" },
      {LItem.ATC_RWY,"RWY" },
      {LItem.ATC_ALT_HDG,"ATC" },
      {LItem.METAR,"METAR" },

      {LItem.M_TIM_DIST1,"CP 1" },
      {LItem.M_TIM_DIST2,"CP 2" },
      {LItem.M_TIM_DIST3,"CP 3" },
    };

    // Descriptive Configuration GUI label names to match the enum above
    private Dictionary<LItem,string> m_cfgNames = new Dictionary<LItem, string>(){
      {LItem.MSFS,"MSFS Status" },
      {LItem.SimRate,"Sim Rate" },
      {LItem.ACFT_ID,"Aircraft ID" },
      {LItem.TIME,"Time of day (Sim)" },

      {LItem.ETrim,"Elevator Trim" }, {LItem.RTrim,"Rudder Trim" }, {LItem.ATrim,"Aileron Trim" },
      {LItem.A_ETRIM,"Auto E-Trim" },
      {LItem.OAT,"Outsite Air Temp °C" },
      {LItem.VIS,"Visibility nm" },
      {LItem.WIND_SD,"Wind dir° @ speed kt" },
      {LItem.WIND_XY,"Wind cross / head kt" },
      {LItem.BARO_HPA,"Baro Setting hPa" },
      {LItem.BARO_InHg,"Baro Setting InHg" },
      {LItem.Gear,"Gear" }, {LItem.Brakes,"Brakes" }, {LItem.Flaps,"Flaps" }, {LItem.Lights,"Lights BNSTL" },
      {LItem.AOA,"Angle of attack deg" },

      {LItem.MAN,"MAN Pressure inHg" },
      {LItem.TORQ,"Torque ft/lb" },
      {LItem.TORQP,"Torque %" },
      {LItem.PRPM,"Propeller RPM" },
      {LItem.ERPM,"Engine RPM" },
      {LItem.N1,"Turbine N1" },
      {LItem.ITT,"Turbine ITT °C" },
      {LItem.EGT,"Engine EGT °C" },
      {LItem.FFlow_pph,"Fuel Flow pph" },
      {LItem.FFlow_gph,"Fuel Flow gph" },
      {LItem.Fuel_LR,"Fuel Left/Right Gal" },
      {LItem.Fuel_Total,"Fuel Total Gal" },

      {LItem.GPS_WYP,"≡GPS≡" },
      //{LItem.GPS_APT_APR,"Airport/Approach" },  - SIM PROVIDES EMPTY STRINGS ...
      {LItem.GPS_WP_DIST,"WYP Distance nm" },
      {LItem.GPS_WP_ETE,"WYP ETE h:mm:ss" },
      {LItem.GPS_ETE,"Destination ETE" },
      {LItem.GPS_BRGm,"Bearing to WYP (mag)" },
      {LItem.GPS_TRK,"Current Track" },
      {LItem.GPS_DTRK,"Desired track to WYP" },
      {LItem.GPS_XTK,"Cross track distance nm" },
      {LItem.GPS_GS,"Groundspeed" },
      {LItem.GPS_ALT,"Waypoint ALT ft" },
      {LItem.GPS_LAT_LON,"Aircraft Lat/Lon" },
      {LItem.EST_VS,"Estimate VS to WYP@ALT" },
      {LItem.EST_ALT,"Estimated ALT @WYP" },
      {LItem.ENROUTE, "Enroute Times (WP/Tot)" },

      {LItem.HDG,"Aircraft HDG" },
      {LItem.HDGt,"Aircraft True HDG" },
      {LItem.ALT,"Aircraft effective ALT ft" },
      {LItem.ALT_INST,"Aircraft ALT ft" },
      {LItem.RA,"Aircraft RA ft" },
      {LItem.RA_VOICE,"Aircraft RA ft audible" },
      {LItem.IAS,"Aircraft IAS kt" },
      {LItem.TAS,"Aircraft TAS kt" },
      {LItem.MACH,"Aircraft Mach number M" },
      {LItem.VS,"Aircraft VS fpm (arrows)" },
      {LItem.VS_PM,"Aircraft VS fpm (+- signs)" },

      {LItem.AP,"Autopilot Master" },
      {LItem.AP_HDGs,"AP HDG / Set" },
      {LItem.AP_ALTs,"AP ALT / Set" },
      {LItem.AP_VSs,"AP VS / Set" },
      {LItem.AP_FLCs,"AP FLC / Set" },
      {LItem.AP_BC, "AP BC" },
      {LItem.AP_NAVg,"AP NAV and GPS" },
      {LItem.AP_APR_GS,"AP APR and GS" },
      {LItem.AP_YD,"AP Yaw Damper" },
      {LItem.AP_LVL,"AP Wing Leveler" },

      {LItem.NAV1,"NAV-1 Id BRG DME" },
      {LItem.NAV2,"NAV-2 Id BRG DME" },

      {LItem.ATC_APT,"ATC Airport and distance nm" },
      {LItem.ATC_RWY,"ATC Rwy (Dist, Track, Alt)" },
      {LItem.ATC_ALT_HDG,"ATC assigned Alt/Hdg" },
      {LItem.METAR,"METAR Close to location" },

      {LItem.M_TIM_DIST1,"Checkpoint 1" },
      {LItem.M_TIM_DIST2,"Checkpoint 2" },
      {LItem.M_TIM_DIST3,"Checkpoint 3" },
    };

    // Bar value label names to match the enum above
    // Used when the individual value item is meant and the text here is the value itself (i.e. no numeric value)
    //  only used as GPS and GS labels for NAV / GPS and APR / GS  for now
    private Dictionary<VItem,string> m_barValueLabels = new Dictionary<VItem, string>(){
      {VItem.AP_NAVgps,"GPS" },
      {VItem.AP_GS,"►GS◄" },
    };

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
    public HudBar( Label lblProto, Label valueProto, Label value2Proto, Label signProto,
                      bool showUnits, bool autoSave, CProfile cProfile, string voiceName )
    {
      // just save them in the HUD mainly for config purpose
      m_profile = cProfile;
      ShowUnits = showUnits;
      FltAutoSave = autoSave;
      VoiceName = voiceName;
      var voiceOK = m_speech.SetVoice( VoiceName );

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

      // reset all dictionaries
      m_valueItems.Clear( );
      m_dispItems.Clear( );

      // we do this one by one..
      // NOTE: ONLY ONE BUTTON Control (B_ICAO) per item, One may leave out the Value item e.g. for AP and other non value items

      LItem disp; // the Label to add
      VItem item; // the Value item to add (can be defined and added multiple times for e.g. 2 engines values)
      // l is the Label control, v is a Value control, di is the Display Group containing Label and Values for one entity
      Control l, v; DispItem di = null;

      // The pattern below repeats, define the Label, create the display group and add it to the group list
      // Define the Value item and add it to the Value Label list to later change properties when data arrives
      // then use the desired formatter label Control (V_xy) set specifics and add it to the display group and access lists
      // for 2 engine items define the second value item and control (same procedure as with the first one)
      //  - we use value2Proto to get a smaller font usually for 2+ values are to be shown for an item
      // Colors are used from default or set here explicitely 

      // Sim Status
      disp = LItem.MSFS; di = m_dispItems.CreateDisp( disp );
      item = VItem.Ad;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ), ItemBackColor = cInverse }; di.AddItem( l );

      // Sim Rate
      disp = LItem.SimRate; di = m_dispItems.CreateDisp( disp );
      item = VItem.SimRate;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_SRate( value2Proto ) { }; di.AddItem( v ); m_valueItems.AddLbl( item, v );
      // Time of day
      disp = LItem.TIME; di = m_dispItems.CreateDisp( disp );
      item = VItem.TIME;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_TimeHHMMSS( value2Proto ) { }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // TRIMS
      // Elevator (plain)
      disp = LItem.ETrim; di = m_dispItems.CreateDisp( disp );
      item = VItem.ETrim;
      // All ERA-Trim label get a button to activate the 0 Trim action
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Prct( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      // Rudder
      disp = LItem.RTrim; di = m_dispItems.CreateDisp( disp );
      item = VItem.RTrim;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Prct( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      // Aileron
      disp = LItem.ATrim; di = m_dispItems.CreateDisp( disp );
      item = VItem.ATrim;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Prct( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // AutoElevator Trim Version
      disp = LItem.A_ETRIM; di = m_dispItems.CreateDisp( disp );
      item = VItem.A_ETRIM;
      // the Auto E-Trim label gets a button to activate the AutoTrim Module
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Prct( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // OAT
      disp = LItem.OAT; di = m_dispItems.CreateDisp( disp );
      item = VItem.OAT;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Temp( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      // VIS
      disp = LItem.VIS; di = m_dispItems.CreateDisp( disp );
      item = VItem.VIS;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Dist( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // BARO
      disp = LItem.BARO_HPA; di = m_dispItems.CreateDisp( disp );
      item = VItem.BARO_HPA;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_PressureHPA( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.BARO_InHg; di = m_dispItems.CreateDisp( disp );
      item = VItem.BARO_InHg;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_PressureInHg( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // Gear, Brakes, Flaps
      disp = LItem.Gear; di = m_dispItems.CreateDisp( disp );
      item = VItem.Gear;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Steps( signProto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.Brakes; di = m_dispItems.CreateDisp( disp );
      item = VItem.Brakes;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Steps( signProto ) { ItemForeColor = cRA }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.Flaps; di = m_dispItems.CreateDisp( disp );
      item = VItem.Flaps;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Steps( signProto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      // Lights
      disp = LItem.Lights; di = m_dispItems.CreateDisp( disp );
      item = VItem.Lights;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Lights( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // TORQ, PRPM, ERPM, FFLOW - we use value2Proto to get a smaller font for the numbers
      disp = LItem.TORQP; di = m_dispItems.CreateDisp( disp );
      item = VItem.E1_TORQP;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Prct( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.E2_TORQP;
      v = new V_Prct( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );


      disp = LItem.TORQ; di = m_dispItems.CreateDisp( disp );
      item = VItem.E1_TORQ;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Torque( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.E2_TORQ;
      v = new V_Torque( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );


      disp = LItem.PRPM; di = m_dispItems.CreateDisp( disp );
      item = VItem.P1_RPM;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_RPM( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.P2_RPM;
      v = new V_RPM( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );


      disp = LItem.ERPM; di = m_dispItems.CreateDisp( disp );
      item = VItem.E1_RPM;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_RPM( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.E2_RPM;
      v = new V_RPM( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );


      disp = LItem.N1; di = m_dispItems.CreateDisp( disp );
      item = VItem.E1_N1;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Prct( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.E2_N1;
      v = new V_Prct( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );


      disp = LItem.ITT; di = m_dispItems.CreateDisp( disp );
      item = VItem.E1_ITT;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Temp( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.E2_ITT;
      v = new V_Temp( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );


      disp = LItem.EGT; di = m_dispItems.CreateDisp( disp );
      item = VItem.E1_EGT;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Temp( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.E2_EGT;
      v = new V_Temp( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );


      disp = LItem.FFlow_pph; di = m_dispItems.CreateDisp( disp );
      item = VItem.E1_FFlow_pph;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Flow_pph( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.E2_FFlow_pph;
      v = new V_Flow_pph( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.FFlow_gph; di = m_dispItems.CreateDisp( disp );
      item = VItem.E1_FFlow_gph;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Flow_gph( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.E2_FFlow_gph;
      v = new V_Flow_gph( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // GPS
      disp = LItem.GPS_WYP; di = m_dispItems.CreateDisp( disp );
      item = VItem.GPS_PWYP;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_ICAO( valueProto ) { ItemForeColor = cGps }; di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.GPS_NWYP;
      v = new V_ICAO_L( valueProto ) { ItemForeColor = cGps }; di.AddItem( v ); m_valueItems.AddLbl( item, v );
      /* - SIM PROVIDES EMPTY STRINGS ...
      disp = LItem.GPS_APT_APR; di = m_dispItems.CreateDisp( disp );
      item = VItem.GPS_APT;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_ICAO( valueProto ) { ItemForeColor = cGps }; di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.GPS_APR;
      v = new V_ICAO( valueProto ) { ItemForeColor = cGps }; di.AddItem( v ); m_valueItems.AddLbl( item, v );
      */
      disp = LItem.GPS_WP_DIST; di = m_dispItems.CreateDisp( disp );
      item = VItem.GPS_WP_DIST;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Dist( valueProto, showUnits ) { ItemForeColor = cGps }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.GPS_WP_ETE; di = m_dispItems.CreateDisp( disp );
      item = VItem.GPS_WP_ETE;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_TimeHHMMSS( valueProto ) { ItemForeColor = cGps }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.GPS_ETE; di = m_dispItems.CreateDisp( disp );
      item = VItem.GPS_ETE;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_TimeHHMMSS( valueProto ) { ItemForeColor = cGps }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.GPS_TRK; di = m_dispItems.CreateDisp( disp );
      item = VItem.GPS_TRK;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Deg( valueProto, showUnits ) { ItemForeColor = cGps }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.GPS_GS; di = m_dispItems.CreateDisp( disp );
      item = VItem.GPS_GS;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Speed( valueProto, showUnits ) { ItemForeColor = cGps }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.GPS_ALT; di = m_dispItems.CreateDisp( disp );
      item = VItem.GPS_ALT;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Alt( valueProto, showUnits ) { ItemForeColor = cGps }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.GPS_BRGm; di = m_dispItems.CreateDisp( disp );
      item = VItem.GPS_BRGm;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Deg( valueProto, showUnits ) { ItemForeColor = cGps }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.GPS_DTRK; di = m_dispItems.CreateDisp( disp );
      item = VItem.GPS_DTRK;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Deg( valueProto, showUnits ) { ItemForeColor = cGps }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.GPS_XTK; di = m_dispItems.CreateDisp( disp );
      item = VItem.GPS_XTK;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Xtk( valueProto, showUnits ) { ItemForeColor = cGps }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.GPS_LAT_LON; di = m_dispItems.CreateDisp( disp );
      item = VItem.GPS_LAT;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Latitude( value2Proto ) { ItemForeColor = cGps }; di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.GPS_LON;
      v = new V_Longitude( value2Proto ) { ItemForeColor = cGps }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // Estimates
      disp = LItem.EST_VS; di = m_dispItems.CreateDisp( disp );
      item = VItem.EST_VS;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_VSpeed( valueProto, showUnits ) { ItemForeColor = cEst }; di.AddItem( v ); m_valueItems.AddLbl( item, v );
      disp = LItem.EST_ALT; di = m_dispItems.CreateDisp( disp );
      item = VItem.EST_ALT;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Alt( valueProto, showUnits ) { ItemForeColor = cEst }; di.AddItem( v ); m_valueItems.AddLbl( item, v );
      // Enroute times
      disp = LItem.ENROUTE; di = m_dispItems.CreateDisp( disp );
      item = VItem.ENR_WP;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_TimeHHMMSS( value2Proto ) { ItemForeColor = cInfo }; di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.ENR_TOTAL;
      v = new V_TimeHHMMSS( value2Proto ) { ItemForeColor = cInfo }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // Aircraft Data
      disp = LItem.HDG; di = m_dispItems.CreateDisp( disp );
      item = VItem.HDG;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Deg( valueProto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.HDGt; di = m_dispItems.CreateDisp( disp );
      item = VItem.HDGt;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Deg( valueProto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.ALT; di = m_dispItems.CreateDisp( disp );
      item = VItem.ALT;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Alt( valueProto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.ALT_INST; di = m_dispItems.CreateDisp( disp );
      item = VItem.ALT_INST;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Alt( valueProto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.RA; di = m_dispItems.CreateDisp( disp );
      item = VItem.RA;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Alt( valueProto, showUnits ) { ItemForeColor = cRA }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.RA_VOICE; di = m_dispItems.CreateDisp( disp );
      item = VItem.RA_VOICE;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_RAaudio( valueProto, showUnits, m_speech ) { ItemForeColor = cRA }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.IAS; di = m_dispItems.CreateDisp( disp );
      item = VItem.IAS;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Speed( valueProto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      // TAS
      disp = LItem.TAS; di = m_dispItems.CreateDisp( disp );
      item = VItem.TAS;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Speed( valueProto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      // MACH
      disp = LItem.MACH; di = m_dispItems.CreateDisp( disp );
      item = VItem.MACH;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Mach( valueProto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      // VS Arrows
      disp = LItem.VS; di = m_dispItems.CreateDisp( disp );
      item = VItem.VS;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_VSpeed( valueProto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      // VS +-
      disp = LItem.VS_PM; di = m_dispItems.CreateDisp( disp );
      item = VItem.VS_PM;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_VSpeedPM( valueProto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // AoA
      disp = LItem.AOA; di = m_dispItems.CreateDisp( disp );
      item = VItem.AOA;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Angle( valueProto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // Autopilot
      disp = LItem.AP; di = m_dispItems.CreateDisp( disp );
      item = VItem.AP;
      l = new B_Text( item, value2Proto ) { Text = GuiName( disp ) }; di.AddItem( l );

      // AP Heading
      disp = LItem.AP_HDGs; di = m_dispItems.CreateDisp( disp );
      item = VItem.AP_HDG;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      item = VItem.AP_HDGset;
      v = new V_Deg( value2Proto, showUnits ) { ItemForeColor = cSet }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // AP Altitude
      disp = LItem.AP_ALTs; di = m_dispItems.CreateDisp( disp );
      item = VItem.AP_ALT;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      item = VItem.AP_ALTset;
      v = new V_Alt( value2Proto, showUnits ) { ItemForeColor = cSet }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // AP VSpeed
      disp = LItem.AP_VSs; di = m_dispItems.CreateDisp( disp );
      item = VItem.AP_VS;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      item = VItem.AP_VSset;
      v = new V_VSpeed( value2Proto, showUnits ) { ItemForeColor = cSet }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // AP FLChange
      disp = LItem.AP_FLCs; di = m_dispItems.CreateDisp( disp );
      item = VItem.AP_FLC;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      item = VItem.AP_FLCset;
      v = new V_Speed( value2Proto, showUnits ) { ItemForeColor = cSet }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // AP BC
      disp = LItem.AP_BC; di = m_dispItems.CreateDisp( disp );
      item = VItem.AP_BC;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );

      // AP Nav, Apr, GS
      disp = LItem.AP_NAVg; di = m_dispItems.CreateDisp( disp );
      item = VItem.AP_NAV;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      item = VItem.AP_NAVgps;
      v = new V_Text( value2Proto ) { Text = BarValueLabel( item ) }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.AP_APR_GS; di = m_dispItems.CreateDisp( disp );
      item = VItem.AP_APR;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      item = VItem.AP_GS;
      v = new V_Text( value2Proto ) { Text = BarValueLabel( item ) }; di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // AP YD
      disp = LItem.AP_YD; di = m_dispItems.CreateDisp( disp );
      item = VItem.AP_YD;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );

      // AP LVL
      disp = LItem.AP_LVL; di = m_dispItems.CreateDisp( disp );
      item = VItem.AP_LVL;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );

      // ATC Airport
      disp = LItem.ATC_APT; di = m_dispItems.CreateDisp( disp );
      item = VItem.ATC_APT;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_ICAO_L( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.ATC_DIST;
      v = new V_Dist( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // ATC Runway aiming
      disp = LItem.ATC_RWY; di = m_dispItems.CreateDisp( disp );
      item = VItem.ATC_RWY_LON;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_AptDist( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.ATC_RWY_LAT;
      v = new V_LatDist( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.ATC_RWY_ALT;
      v = new V_Alt( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // NAV 1 ID, BRG DME
      disp = LItem.NAV1; di = m_dispItems.CreateDisp( disp );
      item = VItem.NAV1_ID;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_ICAO_L( value2Proto ) { ItemForeColor = cNav }; di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.NAV1_BRG;
      v = new V_Deg( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.NAV1_DST;
      v = new V_DmeDist( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // NAV 2 ID, BRG DME
      disp = LItem.NAV2; di = m_dispItems.CreateDisp( disp );
      item = VItem.NAV2_ID;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_ICAO_L( value2Proto ) { ItemForeColor = cNav }; di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.NAV2_BRG;
      v = new V_Deg( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.NAV2_DST;
      v = new V_DmeDist( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );


      // CP Meter
      disp = LItem.M_TIM_DIST1; di = m_dispItems.CreateDisp( disp );
      item = VItem.M_Elapsed1;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_TimeHHMMSS( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.M_Dist1;
      v = new V_Dist( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.M_TIM_DIST2; di = m_dispItems.CreateDisp( disp );
      item = VItem.M_Elapsed2;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_TimeHHMMSS( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.M_Dist2;
      v = new V_Dist( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.M_TIM_DIST3; di = m_dispItems.CreateDisp( disp );
      item = VItem.M_Elapsed3;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_TimeHHMMSS( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.M_Dist3;
      v = new V_Dist( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.MAN; di = m_dispItems.CreateDisp( disp );
      item = VItem.E1_MAN;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_PressureInHg( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.E2_MAN;
      v = new V_PressureInHg( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      disp = LItem.ACFT_ID; di = m_dispItems.CreateDisp( disp );
      item = VItem.ACFT_ID;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Text( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      // Wind Speed @ Direction
      disp = LItem.WIND_SD; di = m_dispItems.CreateDisp( disp );
      item = VItem.WIND_DIR;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Deg( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.WIND_SPEED;
      v = new V_Speed( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      // Wind XY
      disp = LItem.WIND_XY; di = m_dispItems.CreateDisp( disp );
      item = VItem.WIND_LAT;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Wind_X( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.WIND_LON;
      v = new V_Wind_HT( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      // Fuel quantities
      disp = LItem.Fuel_LR; di = m_dispItems.CreateDisp( disp );
      item = VItem.Fuel_Left;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Gallons( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.Fuel_Right;
      v = new V_Gallons( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      // Total Fuel and Reach
      disp = LItem.Fuel_Total; di = m_dispItems.CreateDisp( disp );
      item = VItem.Fuel_Total;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Gallons( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.Fuel_Reach;
      v = new V_TimeHHMM( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

      // ATC Alt / HDG
      disp = LItem.ATC_ALT_HDG; di = m_dispItems.CreateDisp( disp );
      item = VItem.ATC_ALT;
      l = new L_Text( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Alt( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.ATC_HDG;
      v = new V_Deg( value2Proto, showUnits ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      item = VItem.ATC_WYP;
      v = new V_ICAO( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );
      // METAR
      disp = LItem.METAR; di = m_dispItems.CreateDisp( disp );
      item = VItem.METAR;
      l = new B_Text( item, lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new L_Text( value2Proto ); di.AddItem( v ); m_valueItems.AddLbl( item, v );

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
        max1ValueWidthList.Enqueue( max1ValueWidth ); // last width

        // pad the label control to the right to have the value columns aligned
        max1ValueWidth = max1ValueWidthList.Dequeue( );
        foreach ( var lItem in Profile.ItemPosList( ) ) {
          var dix = DispItem( lItem );
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

    #region Update Content and Settings

    /// <summary>
    /// Update the GUI values from Sim (one Big Value Converter for all Value Items supported)
    ///  In general GUI elements are only updated when checked and visible
    ///  Trackers and Meters are maintained independent of the View state (another profile may use them..)
    /// </summary>
    public void UpdateGUI( string dataRefName )
    {
      if ( !SC.SimConnectClient.Instance.IsConnected ) return; // sanity..

      // check for an update of the Flightplan
      if ( dataRefName == SC.SimConnectClient.Instance.FlightPlanModule.ModuleName ) {
        lock ( m_atcFlightPlan ) {
          m_atcFlightPlan = SC.SimConnectClient.Instance.FlightPlanModule.FlightPlan;
        }
        return; // only update the flightplan, the values are updated during the SimData cycle
      }

      int numEngines = SC.SimConnectClient.Instance.EngineModule.NumEngines;
      var latLon = new LatLon( SC.SimConnectClient.Instance.AircraftModule.Lat, SC.SimConnectClient.Instance.AircraftModule.Lon );

      // update calculations
      Calculator.PaceCalculator( );

      // we do this one by one.. only setting values for visible ones - should save some resources

      // SimRate
      if ( this.ShowItem( LItem.SimRate ) ) {
        this.Value( VItem.SimRate ).Value = SC.SimConnectClient.Instance.AircraftModule.SimRate_rate;
        this.ColorType( VItem.SimRate ).ItemForeColor = ( SC.SimConnectClient.Instance.AircraftModule.SimRate_rate != 1.0f ) ? cInverse : cInfo;
        this.ColorType( VItem.SimRate ).ItemBackColor = ( SC.SimConnectClient.Instance.AircraftModule.SimRate_rate != 1.0f ) ? cSRATE : cBG;
      }
      // Acft ID
      if ( this.ShowItem( LItem.ACFT_ID ) ) this.Value( VItem.ACFT_ID ).Text = SC.SimConnectClient.Instance.AircraftModule.AcftID;
      // Time
      if ( this.ShowItem( LItem.TIME ) ) this.Value( VItem.TIME ).Value = SC.SimConnectClient.Instance.AircraftModule.SimTime_loc_sec;

      // TRIMS
      // Auto ETrim
      if ( this.ShowItem( LItem.A_ETRIM ) ) {
        this.DispItem( LItem.A_ETRIM ).ColorType.ItemBackColor = SC.SimConnectClient.Instance.AutoETrimModule.Enabled ? cLiveBG : cActBG;
        this.Value( VItem.A_ETRIM ).Value = SC.SimConnectClient.Instance.AircraftModule.PitchTrim_prct;
      }
      // Regular Trim
      if ( this.ShowItem( LItem.ETrim ) ) this.Value( VItem.ETrim ).Value = SC.SimConnectClient.Instance.AircraftModule.PitchTrim_prct;
      if ( this.ShowItem( LItem.RTrim ) ) this.Value( VItem.RTrim ).Value = SC.SimConnectClient.Instance.AircraftModule.RudderTrim_prct;
      if ( this.ShowItem( LItem.ATrim ) ) this.Value( VItem.ATrim ).Value = SC.SimConnectClient.Instance.AircraftModule.AileronTrim_prct;
      // OAT
      if ( this.ShowItem( LItem.OAT ) ) {
        this.Value( VItem.OAT ).Value = SC.SimConnectClient.Instance.AircraftModule.OutsideTemperature_degC;
        this.ColorType( VItem.OAT ).ItemForeColor = ( SC.SimConnectClient.Instance.AircraftModule.OutsideTemperature_degC < 4 ) ? cSubZero : cInfo; // icing conditions
      }
      // BARO
      if ( this.ShowItem( LItem.BARO_HPA ) ) this.Value( VItem.BARO_HPA ).Value = SC.SimConnectClient.Instance.AircraftModule.AltimeterSetting_mbar;
      if ( this.ShowItem( LItem.BARO_InHg ) ) this.Value( VItem.BARO_InHg ).Value = SC.SimConnectClient.Instance.AircraftModule.AltimeterSetting_inHg;
      // VIS
      if ( this.ShowItem( LItem.VIS ) ) this.Value( VItem.VIS ).Value = Tooling.NmFromM( SC.SimConnectClient.Instance.AircraftModule.Visibility_m );
      // Wind
      if ( this.ShowItem( LItem.WIND_SD ) ) {
        this.Value( VItem.WIND_SPEED ).Value = SC.SimConnectClient.Instance.AircraftModule.WindSpeed_kt;
        this.Value( VItem.WIND_DIR ).Value = SC.SimConnectClient.Instance.AircraftModule.WindDirection_deg;
      }
      if ( this.ShowItem( LItem.WIND_XY ) ) {
        this.Value( VItem.WIND_LAT ).Value = SC.SimConnectClient.Instance.AircraftModule.Wind_AcftX_kt;
        this.Value( VItem.WIND_LON ).Value = SC.SimConnectClient.Instance.AircraftModule.Wind_AcftZ_kt;
      }
      // Aoa
      if ( this.ShowItem( LItem.AOA ) ) this.Value( VItem.AOA ).Value = SC.SimConnectClient.Instance.AircraftModule.AngleOfAttack_deg;
      // Gear
      if ( this.ShowItem( LItem.Gear ) ) {
        if ( SC.SimConnectClient.Instance.AircraftModule.IsGearRetractable ) {
          this.Value( VItem.Gear ).Step =
            SC.SimConnectClient.Instance.AircraftModule.GearPosition == FSimClientIF.GearPosition.Down ? Steps.Down :
            SC.SimConnectClient.Instance.AircraftModule.GearPosition == FSimClientIF.GearPosition.Up ? Steps.Up : Steps.Unk;
        }
        else {
          this.Value( VItem.Gear ).Step = Steps.Down;
        }
      }
      // Brakes
      if ( this.ShowItem( LItem.Brakes ) ) this.Value( VItem.Brakes ).Step = SC.SimConnectClient.Instance.AircraftModule.Parkbrake_on ? Steps.OnWarn : Steps.OffOK;
      // Flaps
      if ( this.ShowItem( LItem.Flaps ) ) {
        if ( SC.SimConnectClient.Instance.AircraftModule.Flaps == FSimClientIF.CmdMode.Up ) {
          this.Value( VItem.Flaps ).Step = Steps.Up;
        }
        else if ( SC.SimConnectClient.Instance.AircraftModule.Flaps == FSimClientIF.CmdMode.Down ) {
          this.Value( VItem.Flaps ).Step = Steps.Down;
        }
        else if ( SC.SimConnectClient.Instance.AircraftModule.Flaps == FSimClientIF.CmdMode.Pos1 ) {
          this.Value( VItem.Flaps ).Step = Steps.P1;
        }
        else if ( SC.SimConnectClient.Instance.AircraftModule.Flaps == FSimClientIF.CmdMode.Pos2 ) {
          this.Value( VItem.Flaps ).Step = Steps.P2;
        }
        else if ( SC.SimConnectClient.Instance.AircraftModule.Flaps == FSimClientIF.CmdMode.Pos3 ) {
          this.Value( VItem.Flaps ).Step = Steps.P3;
        }
      }
      // Consolidated lights (RA colored for Taxi and/or Landing lights on)
      if ( this.ShowItem( LItem.Lights ) ) {
        int lightsInt = 0;
        this.ColorType( VItem.Lights ).ItemForeColor = cInfo;
        if ( SC.SimConnectClient.Instance.AircraftModule.Lights_Beacon ) lightsInt |= (int)V_Lights.Lights.Beacon;
        if ( SC.SimConnectClient.Instance.AircraftModule.Lights_Nav ) lightsInt |= (int)V_Lights.Lights.Nav;
        if ( SC.SimConnectClient.Instance.AircraftModule.Lights_Strobe ) lightsInt |= (int)V_Lights.Lights.Strobe;
        if ( SC.SimConnectClient.Instance.AircraftModule.Lights_Taxi ) {
          lightsInt |= (int)V_Lights.Lights.Taxi;
          this.ColorType( VItem.Lights ).ItemForeColor = cWarn;
        }
        if ( SC.SimConnectClient.Instance.AircraftModule.Lights_Landing ) {
          lightsInt |= (int)V_Lights.Lights.Landing;
          this.ColorType( VItem.Lights ).ItemForeColor = cWarn;
        }
        this.Value( VItem.Lights ).IntValue = lightsInt;
      }
      // TORQ %
      if ( this.ShowItem( LItem.TORQP ) ) {
        this.Value( VItem.E1_TORQP ).Value = SC.SimConnectClient.Instance.EngineModule.Turbine1_Torque_prct / 100;  // needs to be 0..1
        this.Value( VItem.E2_TORQP ).Value = SC.SimConnectClient.Instance.EngineModule.Turbine2_Torque_prct / 100;  // needs to be 0..1
        this.ValueControl( VItem.E2_TORQP ).Visible = ( numEngines > 1 );
      }
      // TORQ ftLb
      if ( this.ShowItem( LItem.TORQ ) ) {
        this.Value( VItem.E1_TORQ ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_Torque_ft_lbs;
        this.Value( VItem.E2_TORQ ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_Torque_ft_lbs;
        this.ValueControl( VItem.E2_TORQ ).Visible = ( numEngines > 1 );
      }
      // PRPM
      if ( this.ShowItem( LItem.PRPM ) ) {
        this.Value( VItem.P1_RPM ).Value = SC.SimConnectClient.Instance.EngineModule.Propeller1_rpm;
        this.Value( VItem.P2_RPM ).Value = SC.SimConnectClient.Instance.EngineModule.Propeller2_rpm;
        this.ValueControl( VItem.P2_RPM ).Visible = ( numEngines > 1 );
      }
      // ERPM
      if ( this.ShowItem( LItem.ERPM ) ) {
        this.Value( VItem.E1_RPM ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_rpm;
        this.Value( VItem.E2_RPM ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_rpm;
        this.ValueControl( VItem.E2_RPM ).Visible = ( numEngines > 1 );
      }
      // N1
      if ( this.ShowItem( LItem.N1 ) ) {
        this.Value( VItem.E1_N1 ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_N1_prct / 100;  // needs to be 0..1
        this.Value( VItem.E2_N1 ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_N1_prct / 100;  // needs to be 0..1
        this.ValueControl( VItem.E2_N1 ).Visible = ( numEngines > 1 );
      }
      // ITT
      if ( this.ShowItem( LItem.ITT ) ) {
        this.Value( VItem.E1_ITT ).Value = SC.SimConnectClient.Instance.EngineModule.Turbine1_itt;
        this.Value( VItem.E2_ITT ).Value = SC.SimConnectClient.Instance.EngineModule.Turbine2_itt;
        this.ValueControl( VItem.E2_ITT ).Visible = ( numEngines > 1 );
      }
      // EGT
      if ( this.ShowItem( LItem.EGT ) ) {
        this.Value( VItem.E1_EGT ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_egt;
        this.Value( VItem.E2_EGT ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_egt;
        this.ValueControl( VItem.E2_EGT ).Visible = ( numEngines > 1 );
      }
      // MAN
      if ( this.ShowItem( LItem.MAN ) ) {
        this.Value( VItem.E1_MAN ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1MAN_inhg;
        this.Value( VItem.E2_MAN ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2MAN_inhg;
        this.ValueControl( VItem.E2_MAN ).Visible = ( numEngines > 1 );
      }
      // FFLOW PPH
      if ( this.ShowItem( LItem.FFlow_pph ) ) {
        this.Value( VItem.E1_FFlow_pph ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_FuelFlow_lbPh;
        this.Value( VItem.E2_FFlow_pph ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_FuelFlow_lbPh;
        this.ValueControl( VItem.E2_FFlow_pph ).Visible = ( numEngines > 1 );
      }
      // FFLOW GPH
      if ( this.ShowItem( LItem.FFlow_gph ) ) {
        this.Value( VItem.E1_FFlow_gph ).Value = SC.SimConnectClient.Instance.EngineModule.Engine1_FuelFlow_galPh;
        this.Value( VItem.E2_FFlow_gph ).Value = SC.SimConnectClient.Instance.EngineModule.Engine2_FuelFlow_galPh;
        this.ValueControl( VItem.E2_FFlow_gph ).Visible = ( numEngines > 1 );
      }
      // Fuel LR
      if ( this.ShowItem( LItem.Fuel_LR ) ) {
        this.Value( VItem.Fuel_Left ).Value = SC.SimConnectClient.Instance.AircraftModule.FuelQuantityLeft_gal;
        this.Value( VItem.Fuel_Right ).Value = SC.SimConnectClient.Instance.AircraftModule.FuelQuantityRight_gal;
        // Color when there is a substantial unbalance
        var imbalanceGal
          =  Math.Abs( SC.SimConnectClient.Instance.AircraftModule.FuelQuantityLeft_gal -SC.SimConnectClient.Instance.AircraftModule.FuelQuantityRight_gal);
        if ( imbalanceGal < ( SC.SimConnectClient.Instance.AircraftModule.FuelCapacityTotal_gal * 0.15 ) ) {
          this.ColorType( VItem.Fuel_Left ).ItemForeColor = cInfo;
          this.ColorType( VItem.Fuel_Right ).ItemForeColor = cInfo;
        }
        else {
          //Imbalance >= 15% Total Fuel
          this.ColorType( VItem.Fuel_Left ).ItemForeColor = cWarn;
          this.ColorType( VItem.Fuel_Right ).ItemForeColor = cWarn;
        }
      }
      // Fuel Tot & Reach
      if ( this.ShowItem( LItem.Fuel_Total ) ) {
        this.Value( VItem.Fuel_Total ).Value = SC.SimConnectClient.Instance.AircraftModule.FuelQuantityTotal_gal;
        float ff = Calculator.FuelReach_sec( );
        this.Value( VItem.Fuel_Reach ).Value = ff;
        this.ColorType( VItem.Fuel_Reach ).ItemForeColor = ( ff <= 1800 ) ? cAlert : ( ff <= 3600 ) ? cWarn : cInfo; // warn <1h, alert <1/2h
      }

      // GPS Always
      if ( this.ShowItem( LItem.GPS_TRK ) ) this.Value( VItem.GPS_TRK ).Value = SC.SimConnectClient.Instance.GpsModule.GTRK;
      if ( this.ShowItem( LItem.GPS_GS ) ) this.Value( VItem.GPS_GS ).Value = SC.SimConnectClient.Instance.AircraftModule.Groundspeed_kt;
      // GPS (is nulled when no flightplan is active)
      if ( SC.SimConnectClient.Instance.GpsModule.IsGpsFlightplan_active ) {
        // WP Enroute Tracker
        WPTracker.Track(
          SC.SimConnectClient.Instance.GpsModule.WYP_prev,
          SC.SimConnectClient.Instance.GpsModule.WYP_next,
          SC.SimConnectClient.Instance.AircraftModule.SimTime_loc_sec,
          SC.SimConnectClient.Instance.AircraftModule.Sim_OnGround
        );

        if ( this.ShowItem( LItem.GPS_WYP ) ) {
          this.Value( VItem.GPS_PWYP ).Text = SC.SimConnectClient.Instance.GpsModule.WYP_prev;
          this.Value( VItem.GPS_NWYP ).Text = SC.SimConnectClient.Instance.GpsModule.WYP_next;
        }
        /*
        if ( this.ShowItem( LItem.GPS_APT_APR ) ) {
          this.Value( VItem.GPS_APT ).Text = SC.SimConnectClient.Instance.GpsModule.ApproachAptID;
          this.Value( VItem.GPS_APR ).Text = SC.SimConnectClient.Instance.GpsModule.ApproachApproachID;
        }
        */
        if ( this.ShowItem( LItem.GPS_WP_DIST ) ) this.Value( VItem.GPS_WP_DIST ).Value = SC.SimConnectClient.Instance.GpsModule.WYP_dist;
        if ( this.ShowItem( LItem.GPS_WP_ETE ) ) this.Value( VItem.GPS_WP_ETE ).Value = SC.SimConnectClient.Instance.GpsModule.WYP_ete;
        if ( this.ShowItem( LItem.GPS_ETE ) ) this.Value( VItem.GPS_ETE ).Value = SC.SimConnectClient.Instance.GpsModule.DEST_ete;
        if ( this.ShowItem( LItem.GPS_BRGm ) ) this.Value( VItem.GPS_BRGm ).Value = SC.SimConnectClient.Instance.GpsModule.BRG;
        if ( this.ShowItem( LItem.GPS_DTRK ) ) this.Value( VItem.GPS_DTRK ).Value = SC.SimConnectClient.Instance.GpsModule.DTK;
        if ( this.ShowItem( LItem.GPS_XTK ) ) this.Value( VItem.GPS_XTK ).Value = SC.SimConnectClient.Instance.GpsModule.GpsWaypointCrossTRK_nm;

        // Estimate handling
        float tgtAlt = SC.SimConnectClient.Instance.GpsModule.WYP_alt;
        if ( this.ShowItem( LItem.GPS_ALT ) ) this.Value( VItem.GPS_ALT ).Value = tgtAlt;
        // Estimates use WYP ALT if >0 (there is no distinction if a WYP ALT is given - it is 0 if not)
        ColorType estCol = cEst;
        if ( tgtAlt == 0 ) {
          // use Set Alt if WYP ALT is zero (see comment above)
          tgtAlt = SC.SimConnectClient.Instance.AP_G1000Module.ALT_setting_ft;
          estCol = cSet;
        }
        // Update Estimate Calculation with Acf data
        Estimates.UpdateValues(
          SC.SimConnectClient.Instance.AircraftModule.Groundspeed_kt,
          SC.SimConnectClient.Instance.AircraftModule.AltMsl_ft,
          SC.SimConnectClient.Instance.AircraftModule.VS_ftPmin
        );
        // Show Estimates
        if ( this.ShowItem( LItem.EST_VS ) ) {
          this.Value( VItem.EST_VS ).Value = Estimates.VSToTgt_AtAltitude( tgtAlt, SC.SimConnectClient.Instance.GpsModule.WYP_dist );
          this.ColorType( VItem.EST_VS ).ItemForeColor = estCol;
        }
        if ( this.ShowItem( LItem.EST_ALT ) ) {
          this.Value( VItem.EST_ALT ).Value = Estimates.AltitudeAtTgt( SC.SimConnectClient.Instance.GpsModule.WYP_dist );
          this.ColorType( VItem.EST_ALT ).ItemForeColor = estCol;
        }

        if ( this.ShowItem( LItem.ENROUTE ) ) {
          this.Value( VItem.ENR_WP ).Value = WPTracker.WPTimeEnroute_sec;
          this.Value( VItem.ENR_TOTAL ).Value = WPTracker.TimeEnroute_sec;
        }
      }
      else {
        // No SIM GPS - Flightplan active
        if ( this.ShowItem( LItem.GPS_WYP ) ) {
          this.Value( VItem.GPS_PWYP ).Text = null;
          this.Value( VItem.GPS_NWYP ).Text = null;
        }
        /*
        if ( PROFILE.ShowItem( LItem.GPS_APT_APR ) ) {
          this.Value( VItem.GPS_APT ).Text = "_____";
          this.Value( VItem.GPS_APR ).Text = "_____";
        }
        */
        if ( this.ShowItem( LItem.GPS_WP_DIST ) ) this.Value( VItem.GPS_WP_DIST ).Value = null;
        if ( this.ShowItem( LItem.GPS_WP_ETE ) ) this.Value( VItem.GPS_WP_ETE ).Value = null;
        if ( this.ShowItem( LItem.GPS_ETE ) ) this.Value( VItem.GPS_ETE ).Value = null;
        if ( this.ShowItem( LItem.GPS_BRGm ) ) this.Value( VItem.GPS_BRGm ).Value = null;
        if ( this.ShowItem( LItem.GPS_DTRK ) ) this.Value( VItem.GPS_DTRK ).Value = null;
        if ( this.ShowItem( LItem.GPS_XTK ) ) this.Value( VItem.GPS_XTK ).Value = null;
        if ( this.ShowItem( LItem.GPS_ALT ) ) this.Value( VItem.GPS_ALT ).Value = null;
        if ( this.ShowItem( LItem.EST_VS ) ) this.Value( VItem.EST_VS ).Value = null; // cannot if we don't have a WYP to aim at
        if ( this.ShowItem( LItem.EST_ALT ) ) this.Value( VItem.EST_ALT ).Value = null; // cannot if we don't have a WYP to aim at
        if ( this.ShowItem( LItem.ENROUTE ) ) {
          this.Value( VItem.ENR_WP ).Value = null;
          this.Value( VItem.ENR_TOTAL ).Value = null;
        }
      }

      // LOCK the flightplan while using it, else the Asynch Update may change it ..
      lock ( m_atcFlightPlan ) {
        // Load Remaining Plan if the WYP or Flightplan has changed
        if ( WPTracker.HasChanged || FltMgr.HasChanged ) {
          FltMgr.Read( ); // commit that we read the changes of the Flight Plan
          this.ToolTipFP.SetToolTip( this.DispItem( LItem.GPS_WYP ).Label, m_atcFlightPlan.RemainingPlan( WPTracker.Read( ) ) );
          this.ToolTipFP.SetToolTip( this.ValueControl( VItem.GPS_PWYP ), m_atcFlightPlan.WaypointByName( WPTracker.PrevWP ).PrettyDetailed );
          this.ToolTipFP.SetToolTip( this.ValueControl( VItem.GPS_NWYP ), m_atcFlightPlan.WaypointByName( WPTracker.NextWP ).PrettyDetailed );
          this.ToolTipFP.SetToolTip( this.DispItem( LItem.ATC_ALT_HDG ).Label, m_atcFlightPlan.Pretty );
        }

        // ATC Airport
        AirportMgr.Update( m_atcFlightPlan.Destination ); // maintain APT (we should always have a Destination here)
        if ( this.ShowItem( LItem.ATC_APT ) ) {
          if ( AirportMgr.HasChanged ) this.Value( VItem.ATC_APT ).Text = AirportMgr.Read( ); // update only when changed
          if ( MetarApt.HasNewData ) {
            // avoiding redraw and flicker for every cycle
            this.ToolTip.SetToolTip( this.DispItem( LItem.ATC_APT ).Label, MetarApt.Read( ) );
            this.DispItem( LItem.ATC_APT ).ColorType.ItemBackColor = MetarApt.ConditionColor;
          }
          // Distance to Destination
          if ( m_atcFlightPlan.HasFlightPlan ) {
            this.Value( VItem.ATC_DIST ).Value = m_atcFlightPlan.RemainingDist_nm(
            SC.SimConnectClient.Instance.GpsModule.WYP_next,
            SC.SimConnectClient.Instance.GpsModule.WYP_dist );
          }
          else {
            // calc straight distance if we don't have an ATC flightplan with waypoints
            this.Value( VItem.ATC_DIST ).Value = AirportMgr.Distance_nm( latLon );
          }
        }

        // if we have an ATC FlightPlan show ATC assignments
        if ( m_atcFlightPlan.HasFlightPlan ) {
          // ATC Alt Hdg NextWYP
          if ( this.ShowItem( LItem.ATC_ALT_HDG ) ) {
            this.Value( VItem.ATC_ALT ).Value = m_atcFlightPlan.AssignedAlt;
            this.Value( VItem.ATC_HDG ).Value = m_atcFlightPlan.AssignedHdg;
            this.Value( VItem.ATC_WYP ).Text = m_atcFlightPlan.NextWypIdent;
          }
        }
        else {
          // ATC Alt Hdg
          if ( this.ShowItem( LItem.ATC_ALT_HDG ) ) {
            this.Value( VItem.ATC_ALT ).Value = null;
            this.Value( VItem.ATC_HDG ).Value = null;
            this.Value( VItem.ATC_WYP ).Text = "";
          }
        }
      }// end m_flightPlan LOCK


      // LAT LON Position
      if ( this.ShowItem( LItem.GPS_LAT_LON ) ) {
        this.Value( VItem.GPS_LAT ).Value = (float)latLon.Lat;
        this.Value( VItem.GPS_LON ).Value = (float)latLon.Lon;
      }

      // Autopilot Items to show
      if ( this.ShowItem( LItem.AP ) )
        this.DispItem( LItem.AP ).ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.AP_mode == FSimClientIF.APMode.On ? cAP : cInfo;

      if ( this.ShowItem( LItem.AP_HDGs ) ) {
        this.DispItem( LItem.AP_HDGs ).ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.HDG_hold ? cAP : cInfo;
        this.Value( VItem.AP_HDGset ).Value = SC.SimConnectClient.Instance.AP_G1000Module.HDG_setting_degm;
      }

      if ( this.ShowItem( LItem.AP_ALTs ) ) {
        this.DispItem( LItem.AP_ALTs ).ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.ALT_hold ? cAP : cInfo;
        this.Value( VItem.AP_ALTset ).Value = SC.SimConnectClient.Instance.AP_G1000Module.ALT_setting_ft;
      }

      if ( this.ShowItem( LItem.AP_VSs ) ) {
        this.DispItem( LItem.AP_VSs ).ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.VS_hold ? cAP : cInfo;
        this.Value( VItem.AP_VSset ).Value = SC.SimConnectClient.Instance.AP_G1000Module.VS_setting_fpm;
      }

      if ( this.ShowItem( LItem.AP_FLCs ) ) {
        this.DispItem( LItem.AP_FLCs ).ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.FLC_active ? cAP : cInfo;
        this.Value( VItem.AP_FLCset ).Value = SC.SimConnectClient.Instance.AP_G1000Module.IAS_setting_kt;
      }

      if ( this.ShowItem( LItem.AP_BC ) )
        this.DispItem( LItem.AP_BC ).ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.BC_hold ? cAP : cInfo;

      if ( this.ShowItem( LItem.AP_NAVg ) ) {
        this.DispItem( LItem.AP_NAVg ).ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.NAV_hold ? cAP : cInfo;
        this.ColorType( VItem.AP_NAVgps ).ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.GPS_active ? cGps : cInfo;
      }

      if ( this.ShowItem( LItem.AP_APR_GS ) ) {
        this.DispItem( LItem.AP_APR_GS ).ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.APR_hold ? cAP : cInfo;
        this.ColorType( VItem.AP_GS ).ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.GS_active ? cAP : cInfo;
      }
      if ( this.ShowItem( LItem.AP_YD ) ) this.DispItem( LItem.AP_YD ).ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.YD ? cAP : cInfo;
      if ( this.ShowItem( LItem.AP_LVL ) ) this.DispItem( LItem.AP_LVL ).ColorType.ItemForeColor = SC.SimConnectClient.Instance.AP_G1000Module.LVL ? cAP : cInfo;

      // Aircraft Data
      if ( this.ShowItem( LItem.HDG ) ) this.Value( VItem.HDG ).Value = SC.SimConnectClient.Instance.AircraftModule.HDG_mag_degm;
      if ( this.ShowItem( LItem.HDGt ) ) this.Value( VItem.HDGt ).Value = SC.SimConnectClient.Instance.AircraftModule.HDG_true_deg;
      if ( this.ShowItem( LItem.ALT ) ) this.Value( VItem.ALT ).Value = SC.SimConnectClient.Instance.AircraftModule.AltMsl_ft; // REAL ALTITUDE (does not match the instrument)
      if ( this.ShowItem( LItem.ALT_INST ) ) this.Value( VItem.ALT_INST ).Value = SC.SimConnectClient.Instance.AircraftModule.Altimeter_ft; // Altimeter Instrument Readout
      if ( this.ShowItem( LItem.RA ) ) {
        if ( SC.SimConnectClient.Instance.AircraftModule.AltAoG_ft <= 1500 ) {
          this.Value( VItem.RA ).Value = SC.SimConnectClient.Instance.AircraftModule.AltAoG_ft;
        }
        else {
          this.Value( VItem.RA ).Text = " .....";
        }
      }
      if ( this.ShowItem( LItem.RA_VOICE ) ) {
        if ( SC.SimConnectClient.Instance.AircraftModule.AltAoG_ft <= 1500 ) {
          this.Value( VItem.RA_VOICE ).Value = SC.SimConnectClient.Instance.AircraftModule.AltAoG_ft;
        }
        else {
          this.Value( VItem.RA_VOICE ).Text = " .....";
        }
      }
      if ( this.ShowItem( LItem.IAS ) ) this.Value( VItem.IAS ).Value = SC.SimConnectClient.Instance.AircraftModule.IAS_kt;
      if ( this.ShowItem( LItem.TAS ) ) this.Value( VItem.TAS ).Value = SC.SimConnectClient.Instance.AircraftModule.TAS_kt;
      if ( this.ShowItem( LItem.MACH ) ) this.Value( VItem.MACH ).Value = SC.SimConnectClient.Instance.AircraftModule.Machspeed_mach;
      if ( this.ShowItem( LItem.VS ) ) this.Value( VItem.VS ).Value = Tooling.Round( SC.SimConnectClient.Instance.AircraftModule.VS_ftPmin, 20 ); // steps 20
      if ( this.ShowItem( LItem.VS_PM ) ) this.Value( VItem.VS_PM ).Value = Tooling.Round( SC.SimConnectClient.Instance.AircraftModule.VS_ftPmin, 20 ); // steps 20

      // ATC Runway
      if ( this.ShowItem( LItem.ATC_RWY ) ) {
        if ( SC.SimConnectClient.Instance.AircraftModule.AtcRunwaySelected ) {
          this.Value( VItem.ATC_RWY_LON ).Value = SC.SimConnectClient.Instance.AircraftModule.AtcRunway_Distance_nm;
          float f = SC.SimConnectClient.Instance.AircraftModule.AtcRunway_Displacement_ft;
          this.Value( VItem.ATC_RWY_LAT ).Value = f;
          this.ColorType( VItem.ATC_RWY_LAT ).ItemForeColor = ( Math.Abs( f ) <= 3 ) ? cOK : cInfo; // green if within +- 3ft
          f = SC.SimConnectClient.Instance.AircraftModule.AtcRunway_HeightAbove_ft;
          this.Value( VItem.ATC_RWY_ALT ).Value = f;
          this.ColorType( VItem.ATC_RWY_ALT ).ItemForeColor = ( f <= 500 ) ? cRA : cInfo;  // RA yellow if below 500ft

        }
        else {
          this.Value( VItem.ATC_RWY_LON ).Value = null;
          this.Value( VItem.ATC_RWY_LAT ).Value = null;
          this.Value( VItem.ATC_RWY_ALT ).Value = null;
        }
      }
      // Location METAR
      if ( this.ShowItem( LItem.METAR ) ) {
        if ( MetarLoc.HasNewData ) {
          // avoiding redraw for every cycle
          this.Value( VItem.METAR ).Text = MetarLoc.StationText;
          this.ToolTip.SetToolTip( this.DispItem( LItem.METAR ).Label, MetarLoc.Read( ) );
          this.DispItem( LItem.METAR ).ColorType.ItemBackColor = MetarLoc.ConditionColor;
        }
      }

      // NAV 1 Tuning
      if ( this.ShowItem( LItem.NAV1 ) ) {
        if ( SC.SimConnectClient.Instance.NavModule.Nav1_Ident != "" ) {
          this.Value( VItem.NAV1_ID ).Text =
             ( SC.SimConnectClient.Instance.NavModule.GS1_flag ? "‡◊"        // GS received
             : SC.SimConnectClient.Instance.NavModule.GS1_available ? "‡ "  // GS available
             : SC.SimConnectClient.Instance.NavModule.Nav1_hasLOC ? "† "    // LOC availbe
             : "  " ) + SC.SimConnectClient.Instance.NavModule.Nav1_Ident;

          if ( SC.SimConnectClient.Instance.NavModule.Nav1_Signal && SC.SimConnectClient.Instance.NavModule.FromToFlag1 != 0 ) {
            this.Value( VItem.NAV1_BRG ).Value = (float)Geo.Wrap360( SC.SimConnectClient.Instance.NavModule.Nav1_Radial_degm - 180 );
          }
          else {
            this.Value( VItem.NAV1_BRG ).Value = null;
          }
          if ( SC.SimConnectClient.Instance.NavModule.Nav1_hasDME ) {
            this.Value( VItem.NAV1_DST ).Value =
                V_DmeDist.DmeDistance( SC.SimConnectClient.Instance.NavModule.DMEdistNav1_nm, SC.SimConnectClient.Instance.NavModule.FromToFlag1 );
          }
          else {
            this.Value( VItem.NAV1_DST ).Value = null;
          }
        }
        else {
          this.Value( VItem.NAV1_ID ).Text = null;
          this.Value( VItem.NAV1_BRG ).Value = null;
          this.Value( VItem.NAV1_DST ).Value = null;
        }
      }
      // NAV 2 Tuning
      if ( this.ShowItem( LItem.NAV2 ) ) {
        if ( SC.SimConnectClient.Instance.NavModule.Nav2_Ident != "" ) {
          this.Value( VItem.NAV2_ID ).Text =
             ( SC.SimConnectClient.Instance.NavModule.GS2_flag ? "‡◊"       // GS received
             : SC.SimConnectClient.Instance.NavModule.GS2_available ? "‡ "  // GS available
             : SC.SimConnectClient.Instance.NavModule.Nav2_hasLOC ? "† "    // LOC availbe
             : "  " ) + SC.SimConnectClient.Instance.NavModule.Nav2_Ident;

          if ( SC.SimConnectClient.Instance.NavModule.Nav2_Signal && SC.SimConnectClient.Instance.NavModule.FromToFlag2 != 0 ) {
            this.Value( VItem.NAV2_BRG ).Value = (float)Geo.Wrap360( SC.SimConnectClient.Instance.NavModule.Nav2_Radial_degm - 180 );
          }
          else {
            this.Value( VItem.NAV2_BRG ).Value = null;
          }
          if ( SC.SimConnectClient.Instance.NavModule.Nav2_hasDME ) {
            this.Value( VItem.NAV2_DST ).Value =
              V_DmeDist.DmeDistance( SC.SimConnectClient.Instance.NavModule.DMEdistNav2_nm, SC.SimConnectClient.Instance.NavModule.FromToFlag2 );
          }
          else {
            this.Value( VItem.NAV2_DST ).Value = null;
          }
        }
        else {
          this.Value( VItem.NAV2_ID ).Text = null;
          this.Value( VItem.NAV2_BRG ).Value = null;
          this.Value( VItem.NAV2_DST ).Value = null;
        }
      }

      // Eval Meters
      if ( this.ShowItem( LItem.M_TIM_DIST1 ) ) {
        CPMeter1.Lapse( latLon, SC.SimConnectClient.Instance.AircraftModule.SimTime_zulu_sec );
        this.Value( VItem.M_Elapsed1 ).Value = CPMeter1.Duration;
        this.Value( VItem.M_Dist1 ).Value = (float)CPMeter1.Distance;
        this.DispItem( LItem.M_TIM_DIST1 ).ColorType.ItemBackColor = CPMeter1.Started ? cLiveBG : cActBG;
      }
      if ( this.ShowItem( LItem.M_TIM_DIST2 ) ) {
        CPMeter2.Lapse( latLon, SC.SimConnectClient.Instance.AircraftModule.SimTime_zulu_sec );
        this.Value( VItem.M_Elapsed2 ).Value = CPMeter2.Duration;
        this.Value( VItem.M_Dist2 ).Value = (float)CPMeter2.Distance;
        this.DispItem( LItem.M_TIM_DIST2 ).ColorType.ItemBackColor = CPMeter2.Started ? cLiveBG : cActBG;
      }
      if ( this.ShowItem( LItem.M_TIM_DIST3 ) ) {
        CPMeter3.Lapse( latLon, SC.SimConnectClient.Instance.AircraftModule.SimTime_zulu_sec );
        this.Value( VItem.M_Elapsed3 ).Value = CPMeter3.Duration;
        this.Value( VItem.M_Dist3 ).Value = (float)CPMeter3.Distance;
        this.DispItem( LItem.M_TIM_DIST3 ).ColorType.ItemBackColor = CPMeter3.Started ? cLiveBG : cActBG;
      }

      // VoicePack Update
      // TODO ... ONLY IF VOICEPACK ENABLED THEN
      m_voicePack.UpdateHudVoice( dataRefName );

    }


    /// <summary>
    /// Set the current show unit flag communicated by the HUD
    /// </summary>
    /// <param name="opacity"></param>
    public void SetShowUnits( bool showUnits )
    {
      ShowUnits = showUnits;
    }

    /// <summary>
    /// Set the current FltAutoSave communicated by the HUD
    /// </summary>
    /// <param name="opacity"></param>
    public void SetFltAutoSave( bool autoSave )
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
    /// Save the Voice Callout state from config
    /// </summary>
    /// <param name="clb">A checked listbox of Voice Items</param>
    public void SetVoiceCallouts( CheckedListBox clb )
    {

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
        return m_valueItems[item].Value;
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
        return m_valueItems[item].ColorType;
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
        return m_valueItems[item].Ctrl;
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
        return m_dispItems[item];
      }
      catch {
        return null;
      }
    }

    /// <summary>
    /// Returns the Bar Value Label of a VItem
    /// </summary>
    /// <param name="item">A VItem</param>
    /// <returns>The Bar Value Label</returns>
    public string BarValueLabel( VItem item )
    {
      try {
        return m_barValueLabels[item];
      }
      catch {
        return $"Bar VLabel undef {item}";
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

    /// <summary>
    /// Returns the Bar Name of a LItem
    /// </summary>
    /// <param name="item">A LItem</param>
    /// <returns>The Bar Name</returns>
    public string GuiName( LItem item )
    {
      try {
        return m_guiNames[item];
      }
      catch {
        return $"Bar Name undef {item}";
      }
    }

    #endregion

  }
}

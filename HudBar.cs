using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FS20_HudBar.GUI;

namespace FS20_HudBar
{
  /// <summary>
  /// All Labels (Topics etc.) shown in the hudBar 
  /// These items are used to Configure (on / off) 
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
    GPS_ETE,  // GPS Time to Destination
    //    GPS_APT_APR,// GPS Airport & Approach - SIM PROVIDES EMPTY STRINGS ...
  }


  /// <summary>
  /// All item values shown in the hudBar 
  /// These are to access and to the processing
  /// Mainly those include Engine 1/2 details
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

    //GPS_APT,      // GPS Airport ID - SIM PROVIDES EMPTY STRINGS ...
    //GPS_APR,      // GPS Approach ID - SIM PROVIDES EMPTY STRINGS ...
  }

  /// <summary>
  /// Instance of the current HudBar
  /// will initialize from profile settings
  /// </summary>
  internal class HudBar
  {
    // Item Colors Foreground
    public Color c_Info = Color.WhiteSmoke; // Info Text (basically white)
    public Color c_AP = Color.LimeGreen;    // Autopilot, NAV (green)
    public Color c_Gps = Color.Fuchsia;     // GPS (magenta)
    public Color c_Set = Color.Cyan;        // Set Values (cyan)
    public Color c_RA = Color.Orange;       // Radio Alt
    public Color c_Est = Color.Plum;        // Estimates 
    // those are set in the data receiver part in Main (here to have all in one place)
    public Color c_SubZero = Color.DeepSkyBlue;  // Temp sub zero
    public Color c_SRATE = Color.Goldenrod;  // SimRate != 1

    // Background
    public Color c_ActBG = Color.FromArgb(00,00,75); // Active Items Background (dark blue)
    public Color c_BG = Color.Black;

    /// <summary>
    /// Show Units if true
    /// </summary>
    public bool ShowUnits { get; set; } = false;
    /// <summary>
    /// Fully Opaque Form Background if true
    /// </summary>
    public bool OpaqueBackground { get; set; } = false;

    /// <summary>
    /// The Current FontSize to use
    /// </summary>
    public FontSize FontSize { get; set; } = FontSize.Regular;

    /// <summary>
    /// Placement of the Bar
    /// </summary>
    public Placement Placement { get; set; } = Placement.Bottom;

    /// <summary>
    /// Display Kind of the Bar
    /// </summary>
    public Kind Kind { get; set; } = Kind.Bar;


    // Hud Bar label names to match the enum above (as short as possible)
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
      {LItem.ALT,"ALT" },
      {LItem.RA,"RA" },
      {LItem.IAS,"IAS" },
      {LItem.TAS,"TAS" },
      {LItem.MACH,"Mach" },
      {LItem.VS,"VS" },

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

      {LItem.ATC_APT,"APT" },
      {LItem.ATC_RWY,"RWY" },

      {LItem.M_TIM_DIST1,"CP 1" },
      {LItem.M_TIM_DIST2,"CP 2" },
      {LItem.M_TIM_DIST3,"CP 3" },
    };

    // Descriptive GUI label names to match the enum above (shown in Config)
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
      {LItem.EST_VS,"Estimate VS to WYP@ALT" },
      {LItem.EST_ALT,"Estimated ALT @WYP" },
      {LItem.ENROUTE, "Enroute Times (WP/Tot)" },

      {LItem.HDG,"Aircraft HDG" },
      {LItem.HDGt,"Aircraft True HDG" },
      {LItem.ALT,"Aircraft ALT ft" },
      {LItem.RA,"Aircraft RA ft" },
      {LItem.IAS,"Aircraft IAS kt" },
      {LItem.TAS,"Aircraft TAS kt" },
      {LItem.MACH,"Aircraft Mach number M" },
      {LItem.VS,"Aircraft VS fpm" },

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

      {LItem.ATC_APT,"ATC Airport" },
      {LItem.ATC_RWY,"ATC Rwy (Dist, Track, Alt)" },

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

    // Labels (some are buttons, for some there are color changes
    private Dictionary<VItem, IValue> m_lblItems = new Dictionary<VItem, IValue>();
    private Dictionary<VItem,Control> m_lblLabelItems = new Dictionary<VItem, Control>();
    // add a label to the list
    private void AddLbl( VItem item, Control ctrl )
    {
      m_lblItems.Add( item, (IValue)ctrl );
      m_lblLabelItems.Add( item, ctrl );
    }

    // Value Items (updated from Sim, some may change colors)
    private  Dictionary<VItem, IValue>  m_valItems = new Dictionary<VItem, IValue> ();
    private Dictionary<VItem,Control> m_valLabelItems = new Dictionary<VItem,Control>();
    // add a value item to the list
    private void AddValue( VItem item, Control ctrl )
    {
      m_valItems.Add( item, (IValue)ctrl );
      m_valLabelItems.Add( item, ctrl );
    }

    // The Display Groups (DispItem is essentially a smaller FlowLayoutPanel containing the label and 1 or 2 values)
    private  Dictionary<LItem, DispItem>  m_dispItems = new Dictionary<LItem, DispItem> ();
    // add a DispItem to the collection
    private void AddDisp( LItem item, DispItem ctrl )
    {
      m_dispItems.Add( item, ctrl );
    }

    private static GUI_Fonts FONTS = null; // handle fonts as static item to not waste resources

    /// <summary>
    /// Init the Hud Items - providing prototypes for the various label types and Config strings/values from AppSettings
    /// </summary>
    /// <param name="lblProto">A GUI prototype label, carrying fonts, colors etc (set in GUI design mode)</param>
    /// <param name="valueProto">A GUI prototype value, carrying fonts, colors etc (set in GUI design mode)</param>
    /// <param name="value2Proto">A GUI prototype value type 2, carrying fonts, colors etc (set in GUI design mode)</param>
    /// <param name="signProto">A GUI prototype icon(Wingdings), carrying fonts, colors etc (set in GUI design mode)</param>
    /// <param name="showUnits">Showing units flag</param>
    /// <param name="opaque">Opaque flag</param>
    /// <param name="fontSize">Used fontsize</param>
    /// <param name="placement">Screen alignment of the bar</param>
    /// <param name="condensed">Condensed flag</param>
    public HudBar( Label lblProto, Label valueProto, Label value2Proto, Label signProto,                    
                    bool showUnits, bool opaque, FontSize fontSize, Placement placement, Kind kind, bool condensed )
    {
      // just save them in the HUD mainly for config purpose
      ShowUnits = showUnits;
      OpaqueBackground = opaque;

      FontSize = fontSize;
      Placement = placement;
      Kind = kind;

      // init from the submitted labels
      FONTS = new GUI_Fonts( condensed ); // get all fonts from built in

      // set desired size
      FONTS.SetFontsize( fontSize );
      // and prepare the prototypes used below - not really clever but handier to have all label defaults ....
      lblProto.Font = FONTS.LabelFont;
      valueProto.Font = FONTS.ValueFont;
      value2Proto.Font = FONTS.Value2Font;
      signProto.Font = FONTS.SignFont;

      // The Value Item Background - used when assessing and debugging only
      c_BG = lblProto.BackColor; // default taken from the prototype label
      //c_BG = Color.DarkBlue; // Debug color to see control outlines

      // reset all dictionaries
      m_lblItems.Clear( );
      m_lblLabelItems.Clear( );
      m_valItems.Clear( );
      m_valLabelItems.Clear( );
      m_dispItems.Clear( );

      // we do this one by one..
      // NOTE: ONLY ONE BUTTON Control (B_ICAO) per item, One may leave out the Value item e.g. for AP and other non value items

      LItem disp; // the label to add
      VItem item; // the value item to add (can be defined and added a second time for 2 engines values)
      Control l, v; DispItem di = null; // l is the label control, v is a value control, di is the Display Group containing label and values for one entity

      // The pattern below repeats, define the label, create the display group and add it to the group list
      // Define the value item
      // then use the desired formatter label Control (V_xy) set specifics and add it to the display group and access lists
      // for 2 engine items define the second value item and control (same procedure as with the first one)
      // Actionable Labels must be added to the Label List to have them accessable from the outside 

      // Sim Status
      disp = LItem.MSFS; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.Ad;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l ); AddLbl( item, l ); // access for Error Msg etc
      v = new V_ICAO( lblProto ) { Text = "" }; di.AddItem( v ); AddValue( item, v );

      // Sim Rate
      disp = LItem.SimRate; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.SimRate;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_SRate( value2Proto ) { }; di.AddItem( v ); AddValue( item, v );
      // Time of day
      disp = LItem.TIME; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.TIME;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Time( value2Proto ) { }; di.AddItem( v ); AddValue( item, v );

      // TRIMS - we use value2Proto to get a smaller font for the numbers
      disp = LItem.ETrim; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.ETrim;
      // the ERA-Trim label gets a button to activate the 0 Trim action
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.RTrim; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.RTrim;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.ATrim; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.ATrim;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // OAT
      disp = LItem.OAT; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.OAT;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Temp( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // VIS
      disp = LItem.VIS; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.VIS;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Dist( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // BARO
      disp = LItem.BARO_HPA; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.BARO_HPA;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_PressureHPA( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.BARO_InHg; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.BARO_InHg;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_PressureInHg( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // Gear, Brakes, Flaps
      disp = LItem.Gear; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.Gear;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Steps( signProto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.Brakes; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.Brakes;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Steps( signProto ) { ForeColor = c_RA, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.Flaps; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.Flaps;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Steps( signProto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      // Lights
      disp = LItem.Lights; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.Lights;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Lights( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // TORQ, PRPM, ERPM, FFLOW - we use value2Proto to get a smaller font for the numbers
      disp = LItem.TORQP; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.E1_TORQP;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.E2_TORQP;
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      disp = LItem.TORQ; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.E1_TORQ;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Torque( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.E2_TORQ;
      v = new V_Torque( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      disp = LItem.PRPM; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.P1_RPM;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_RPM( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.P2_RPM;
      v = new V_RPM( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      disp = LItem.ERPM; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.E1_RPM;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_RPM( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.E2_RPM;
      v = new V_RPM( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      disp = LItem.N1; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.E1_N1;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.E2_N1;
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      disp = LItem.ITT; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.E1_ITT;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Temp( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.E2_ITT;
      v = new V_Temp( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      disp = LItem.EGT; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.E1_EGT;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Temp( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.E2_EGT;
      v = new V_Temp( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      disp = LItem.FFlow_pph; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.E1_FFlow_pph;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Flow_pph( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.E2_FFlow_pph;
      v = new V_Flow_pph( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.FFlow_gph; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.E1_FFlow_gph;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Flow_gph( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.E2_FFlow_gph;
      v = new V_Flow_gph( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // GPS
      disp = LItem.GPS_WYP; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.GPS_PWYP;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_ICAO( valueProto ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.GPS_NWYP;
      v = new V_ICAO( valueProto ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      /* - SIM PROVIDES EMPTY STRINGS ...
      disp = LItem.GPS_APT_APR; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.GPS_APT;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_ICAO( valueProto ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.GPS_APR;
      v = new V_ICAO( valueProto ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      */
      disp = LItem.GPS_WP_DIST; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.GPS_WP_DIST;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Dist( valueProto, showUnits ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.GPS_WP_ETE; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.GPS_WP_ETE;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Time( valueProto ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.GPS_ETE; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.GPS_ETE;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Time( valueProto ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.GPS_TRK; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.GPS_TRK;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Deg( valueProto ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.GPS_GS; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.GPS_GS;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Speed( valueProto, showUnits ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.GPS_ALT; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.GPS_ALT;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Alt( valueProto, showUnits ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      // Estimates
      disp = LItem.EST_VS; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.EST_VS;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_VSpeed( valueProto, showUnits ) { ForeColor = c_Est, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      disp = LItem.EST_ALT; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.EST_ALT;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Alt( valueProto, showUnits ) { ForeColor = c_Est, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      // Enroute times
      disp = LItem.ENROUTE; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.ENR_WP;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_Time( valueProto ) { ForeColor = c_Info, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.ENR_TOTAL;
      v = new V_Time( valueProto ) { ForeColor = c_Info, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // Aircraft Data
      disp = LItem.HDG; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.HDG;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Deg( valueProto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.HDGt; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.HDGt;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Deg( valueProto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.ALT; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.ALT;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Alt( valueProto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.RA; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.RA;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Alt( valueProto, showUnits ) { ForeColor = c_RA, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.IAS; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.IAS;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Speed( valueProto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.VS; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.VS;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_VSpeed( valueProto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // Autopilot
      disp = LItem.AP; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.AP;
      l = new B_ICAO( item, value2Proto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item

      // AP Heading
      disp = LItem.AP_HDGs; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.AP_HDG;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      item = VItem.AP_HDGset;
      v = new V_Deg( value2Proto ) { ForeColor = c_Set, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // AP Altitude
      disp = LItem.AP_ALTs; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.AP_ALT;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      item = VItem.AP_ALTset;
      v = new V_Alt( value2Proto, showUnits ) { ForeColor = c_Set, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // AP VSpeed
      disp = LItem.AP_VSs; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.AP_VS;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      item = VItem.AP_VSset;
      v = new V_VSpeed( value2Proto, showUnits ) { ForeColor = c_Set, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // AP FLChange
      disp = LItem.AP_FLCs; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.AP_FLC;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      item = VItem.AP_FLCset;
      v = new V_Speed( value2Proto, showUnits ) { ForeColor = c_Set, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // AP BC
      disp = LItem.AP_BC; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.AP_BC;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item

      // AP Nav, Apr, GS
      disp = LItem.AP_NAVg; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.AP_NAV;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      item = VItem.AP_NAVgps;
      l = new V_ICAO( lblProto ) { Text = BarValueLabel( item ) }; di.AddItem( l ); AddLbl( item, l );

      disp = LItem.AP_APR_GS; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.AP_APR;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      item = VItem.AP_GS;
      l = new V_ICAO( lblProto ) { Text = BarValueLabel( item ) }; di.AddItem( l ); AddLbl( item, l );

      // AP YD
      disp = LItem.AP_YD; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.AP_YD;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item

      // AP LVL
      disp = LItem.AP_LVL; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.AP_LVL;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item

      // ATC Airport
      disp = LItem.ATC_APT; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.ATC_APT;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_ICAO( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // ATC Runway aiming
      disp = LItem.ATC_RWY; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.ATC_RWY_LON;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_AptDist( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.ATC_RWY_LAT;
      v = new V_LatDist( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.ATC_RWY_ALT;
      v = new V_Alt( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      // CP Meter
      disp = LItem.M_TIM_DIST1; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.M_Elapsed1;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_Time( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.M_Dist1;
      v = new V_Dist( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.M_TIM_DIST2; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.M_Elapsed2;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_Time( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.M_Dist2;
      v = new V_Dist( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.M_TIM_DIST3; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.M_Elapsed3;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_Time( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.M_Dist3;
      v = new V_Dist( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.A_ETRIM; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.A_ETRIM;
      // the Auto E-Trim label gets a button to activate the AutoTrim Module
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.MAN; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.E1_MAN;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_PressureInHg( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.E2_MAN;
      v = new V_PressureInHg( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.GPS_BRGm; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.GPS_BRGm;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Deg( valueProto ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.GPS_DTRK; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.GPS_DTRK;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Deg( valueProto ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.GPS_XTK; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.GPS_XTK;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Xtk( valueProto, showUnits ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.AOA; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.AOA;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Angle( valueProto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.TAS; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.TAS;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Speed( valueProto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.ACFT_ID; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.ACFT_ID;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_ICAO( valueProto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      // Wind
      disp = LItem.WIND_SD; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.WIND_DIR;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Deg( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.WIND_SPEED;
      v = new V_Speed( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.WIND_XY; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.WIND_LAT;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Wind_X( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.WIND_LON;
      v = new V_Wind_HT( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      // Fuel quantities
      disp = LItem.Fuel_LR; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.Fuel_Left;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Gallons( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = VItem.Fuel_Right;
      v = new V_Gallons( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.Fuel_Total; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.Fuel_Total;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Gallons( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.MACH; di = new DispItem( ); AddDisp( disp, di );
      item = VItem.MACH;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Mach( valueProto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      // Apply Unit modifier (shown, not shown) to all Values
      foreach ( var lx in m_valItems ) {
        lx.Value.ShowUnit = ShowUnits;
      }

      // Align the Vertical alignment accross the bar
      if ( placement == Placement.Top || placement == Placement.Bottom ) {
        // Set min size of the labels in order to have them better aligned
        // Using arrow chars gets a larger height than regular ASCII chars (odd autosize behavior)
        // Setting minSize to the Max found height of any at least allows for some hotizontal alignment
        int mh = 0;
        foreach ( var lx in m_valLabelItems ) {
          mh = ( lx.Value.Height > mh ) ? lx.Value.Height : mh;
        }
        foreach ( var lx in m_lblLabelItems ) {
          mh = ( lx.Value.Height > mh ) ? lx.Value.Height : mh;
        }
        // define MinHeight for value Labels
        foreach ( var lx in m_valLabelItems ) {
          lx.Value.MinimumSize = new Size( 1, mh );
        }
        // define MinHeight for text Labels
        foreach ( var lx in m_lblLabelItems ) {
          lx.Value.MinimumSize = new Size( 1, mh );
        }
      }

      // Align the Value columns on left and right bound bar or tile
      if ( placement == Placement.Left || placement == Placement.Right ) {
        // Determine max width and make them aligned
        int maxLabelWidth = 0;
        foreach ( var lx in m_dispItems ) {
          var dix = lx.Value as DispItem;
          maxLabelWidth = ( dix.Controls[0].Width > maxLabelWidth ) ? dix.Controls[0].Width : maxLabelWidth;
        }
        // pad the label control to the right to have the value columns aligned
        foreach ( var lx in m_dispItems ) {
          var dix = lx.Value as DispItem;
          dix.Controls[0].Padding = new Padding( 0, 0, maxLabelWidth - dix.Controls[0].Width, 0 );
        }
      }
    }

    /// <summary>
    /// Returns the Value Interface for a Value Item
    ///   BEWARE no checks for validity of the item ..
    /// </summary>
    /// <param name="item">A valid VItem</param>
    /// <returns>The Value Interface or null if not found</returns>
    public IValue Value( VItem item )
    {
      try {
        return m_valItems[item];
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
    /// <returns>The Value Label or null if not found</returns>
    public Control ValueControl( VItem item )
    {
      try {
        return m_valLabelItems[item];
      }
      catch {
        return null;
      }
    }

    /// <summary>
    /// Returns the Value interface for the Label Item
    ///   BEWARE no checks for validity of the item ..
    /// </summary>
    /// <param name="item">A valid VItem</param>
    /// <returns>The Value Interface or null if not found</returns>
    public IValue LabelValue( VItem item )
    {
      try {
        return m_lblItems[item];
      }
      catch {
        return null;
      }
    }

    /// <summary>
    /// Returns the Label-Label for an Item
    ///   BEWARE no checks for validity of the item ..
    /// </summary>
    /// <param name="item">A valid VItem</param>
    /// <returns>The Label Label or null if not found</returns>
    public Control LabelControl( VItem item )
    {
      try {
        return m_lblLabelItems[item];
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


  }
}

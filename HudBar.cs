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
  /// All items shown in the hudBar 
  /// </summary>
  internal enum GItem
  {
    Ad = 0, // MSFS connection status...
    ETrim, // Elevator Trim +-N%
    RTrim, // Rudder Trim +-N%
    ATrim, // Aileron Trim +-N%

    OAT,   // Outside AirTemp °C
    BARO_HPA,   // Altimeter Setting HPA
    BARO_InHg,  // Altimeter Setting InHg

    Gear, // Gear Up/Down
    Brakes, // Brakes On Off
    Flaps, // Flaps N level 

    E1_TORQP, // Engine1 Torque %
    E2_TORQP, // Engine2 Torque %
    E1_TORQ, // Engine1 Torque ft/lb
    E2_TORQ, // Engine2 Torque ft/lb

    P1_RPM,  // Prop1 RPM
    P2_RPM,  // Prop2 RPM
    E1_RPM,  // Engine1 RPM
    E2_RPM,  // Engine2 RPM
    E1_N1,   // Engine1 N1 %
    E2_N1,   // Engine2 N1 %

    E1_ITT,  // ITT 1 Celsius
    E2_ITT,  // ITT 2 Celsius
    E1_EGT,  // EGT 1 Celsius
    E2_EGT,  // EGT 2 Celsius

    E1_FFlow,  // Fuel1 Flow pph
    E2_FFlow,  // Fuel2 Flow pph

    GPS_PWYP, // GPS Prev Waypoint
    GPS_NWYP, // GPS Next Waypoint
    GPS_DIST,  // GPS Distance to next Waypoint
    GPS_ETE,  // GPS Time to next Waypoint
    GPS_TRK,  // GPS Track 000°
    GPS_GS,  // GPS Groundspeed 000kt

    HDG,  // Heading Mag 000°
    ALT,  // Altitude 00000 ft
    RA,   // Radio Altitude 000 ft 
    IAS,  // Ind. Airspeed 000 kt
    VS,   // Vertical Speed +-0000 fpm

    AP,   // Autopilot On/Off
    AP_HDG,  // AP HDG active
    AP_HDGset,  // AP HDG set
    AP_ALT,  // AP ALT hold active
    AP_ALTset,  // AP ALT set
    AP_VS,   // AP VS hold active
    AP_VSset,   // AP VS set
    AP_FLC,  // AP FLC hold active
    AP_FLCset,  // AP FLC IAS set
    AP_NAV,  // AP NAV active
    AP_NAVgps,  // AP NAV follow GPS
    AP_APR,  // AP APR hold active
    AP_GS,   // AP GS  hold active

    ITEM_COUNT, // Last one is the number of elements in the enum
  }

  internal class HudBar
  {
    // Item Colors Foreground
    public Color c_Info = Color.WhiteSmoke; // Info Text (basically white)
    public Color c_AP = Color.LimeGreen;    // Autopilot, NAV (green)
    public Color c_Gps = Color.Fuchsia;     // GPS (magenta)
    public Color c_Set = Color.Cyan;        // Set Values (cyan)
    public Color c_RA = Color.Orange;       // Radio Alt
    public Color c_SubZero = Color.DeepSkyBlue;       // Temp sub zero

    // Background
    public Color c_ActBG = Color.FromArgb(00,00,75); // Active Background

    /// <summary>
    /// Show Units if true
    /// </summary>
    public bool ShowUnits { get; set; } = false;
    /// <summary>
    /// Fully Opaque Form Background if true
    /// </summary>
    public bool OpaqueBackground { get; set; } = false;

    /// <summary>
    /// The FontSize to use
    /// </summary>
    public FontSize FontSize { get; set; } = FontSize.Regular;




    // Configuration label names to match the enum above
    private Dictionary<GItem,string> m_cfgNames = new Dictionary<GItem, string>(){
      {GItem.Ad,"MSFS Status" },
      {GItem.ETrim,"Elevator-Trim" },{GItem.RTrim, "Rudder-Trim" }, {GItem.ATrim,"Aileron-Trim" },
      {GItem.OAT,"Outside Air Temp °C" }, {GItem.BARO_HPA,"Alt.Setting HPA" }, {GItem.BARO_InHg,"Alt.Setting InHg" },
      {GItem.Gear,"Gear" }, {GItem.Brakes,"Brakes" }, {GItem.Flaps,"Flaps" },
      {GItem.E1_TORQP,"Eng.1 Torque %" }, {GItem.E2_TORQP,"Eng.2 Torque %" }, {GItem.E1_TORQ,"Eng.1 Torque ft/lb" }, {GItem.E2_TORQ,"Eng.2 Torque ft/lb" },
      {GItem.P1_RPM,"Prop 1 RPM" }, {GItem.P2_RPM,"Prop 2 RPM" }, {GItem.E1_RPM,"Eng.1 RPM" }, {GItem.E2_RPM,"Eng.2 RPM" },
      {GItem.E1_N1,"Eng.1 N1 %" }, {GItem.E2_N1,"Eng.2 N1 %" },
      {GItem.E1_ITT,"Eng.1 ITT °C" }, {GItem.E2_ITT,"Eng.2 ITT °C" }, {GItem.E1_EGT,"Eng.1 EGT °C" },{GItem.E2_EGT,"Eng.2 EGT °C" },
      {GItem.E1_FFlow,"Eng.1 Fuel Flow pph" },{GItem.E2_FFlow,"Eng.2 Fuel Flow pph" },
      {GItem.GPS_PWYP,"Prev WYP" }, {GItem.GPS_NWYP,"Next WYP" },
      {GItem.GPS_DIST,"WYP Distance" }, {GItem.GPS_ETE,"WYP ETE" }, {GItem.GPS_TRK,"GPS Track" }, {GItem.GPS_GS,"GPS Groundspeed" },
      {GItem.HDG,"HDG" }, {GItem.ALT,"ALT" }, {GItem.RA,"RA" },{GItem.IAS,"IAS" }, {GItem.VS,"VS" },
      {GItem.AP,"Autopilot Master" },
      {GItem.AP_HDG,"HDG Mode" },{GItem.AP_HDGset, "HDG Set" },
      {GItem.AP_ALT,"ALT Mode" },{GItem.AP_ALTset, "ALT Set" },
      {GItem.AP_VS,"VS Mode" }, {GItem.AP_VSset, "VS Set" },
      {GItem.AP_FLC,"FLC Mode" }, {GItem.AP_FLCset,"IAS Set" },
      {GItem.AP_NAV,"NAV Mode" }, {GItem.AP_NAVgps,"NAV is GPS" }, {GItem.AP_APR,"APR Mode" }, {GItem.AP_GS,"GS Captured" },
    };

    // Bar label names to match the enum above
    private Dictionary<GItem,string> m_barNames = new Dictionary<GItem, string>(){
      {GItem.Ad,"MSFS" },
      {GItem.ETrim,"ETrim" },{GItem.RTrim, "RTrim" }, {GItem.ATrim,"ATrim" },
      {GItem.OAT,"OAT" }, {GItem.BARO_HPA,"BARO" }, {GItem.BARO_InHg,"BARO" },
      {GItem.Gear,"Gear" }, {GItem.Brakes,"Brakes" }, {GItem.Flaps,"Flaps" },
      {GItem.E1_TORQP,"TORQ" }, {GItem.E2_TORQP,"" }, {GItem.E1_TORQ,"TORQ" }, {GItem.E2_TORQ,"" },
      {GItem.P1_RPM,"PRPM" }, {GItem.P2_RPM,"" }, {GItem.E1_RPM,"ERPM" }, {GItem.E2_RPM,"" },
      {GItem.E1_N1,"N1%" }, {GItem.E2_N1,"" },
      {GItem.E1_ITT,"ITT" }, {GItem.E2_ITT,"" }, {GItem.E1_EGT,"EGT" },{GItem.E2_EGT,"" },
      {GItem.E1_FFlow,"FFLOW" },{GItem.E2_FFlow,"" },// omit the label for the second one
      {GItem.GPS_PWYP,"≡GPS≡" }, {GItem.GPS_NWYP,"---" },
      {GItem.GPS_DIST,"DIST" }, {GItem.GPS_ETE,"ETE" }, {GItem.GPS_TRK,"TRK" }, {GItem.GPS_GS,"GS" },
      {GItem.HDG,"HDG" }, {GItem.ALT,"ALT" }, {GItem.RA,"RA" },{GItem.IAS,"IAS" }, {GItem.VS,"VS" },
      {GItem.AP,"≡AP≡" },
      {GItem.AP_HDG,"HDG" },{GItem.AP_HDGset, "" },
      {GItem.AP_ALT,"ALT" },{GItem.AP_ALTset, "" },
      {GItem.AP_VS,"VS" }, {GItem.AP_VSset, "" },
      {GItem.AP_FLC,"FLC" }, {GItem.AP_FLCset,"" },
      {GItem.AP_NAV,"NAV" }, {GItem.AP_NAVgps,"GPS" }, {GItem.AP_APR,"APR" }, {GItem.AP_GS,"►GS◄" },
    };

    private Dictionary<GItem, IValue> m_lblItems = new Dictionary<GItem, IValue>();
    private Dictionary<GItem,Control> m_lblLabelItems = new Dictionary<GItem, Control>();

    private  Dictionary<GItem, IValue>  m_valItems = new Dictionary<GItem, IValue> ();
    private Dictionary<GItem,Control> m_valLabelItems = new Dictionary<GItem,Control>();

    private static GUI_Fonts FONTS = null; // handle fonts as static item to not waste resources

    /// <summary>
    /// Init the Hud Items - providing prototypes for the various label types
    /// </summary>
    /// <param name="labelProto"></param>
    /// <param name="valueProto"></param>
    /// <param name="signProto"></param>
    public HudBar( Label lblProto, Label valueProto, Label value2Proto, Label signProto,
                    bool showUnits, bool opaque, FontSize fontSize )
    {
      // just save them in the HUD mainly for config purpose
      ShowUnits = showUnits;
      OpaqueBackground = opaque;

      FontSize = fontSize;
      // init once from the submitted labels
      if ( FONTS == null ) {
        FONTS = new GUI_Fonts( lblProto, valueProto, value2Proto, signProto );
      }
      // set desired size
      FONTS.SetFontsize( fontSize );
      // and prepare the prototypes used below - not really clever but ....
      lblProto.Font = FONTS.LabelFont;
      valueProto.Font = FONTS.ValueFont;
      value2Proto.Font = FONTS.Value2Font;
      signProto.Font = FONTS.SignFont;

      // The Value Item Background - used when assessing and debugging only
      Color backCol = lblProto.BackColor;
      // backCol = Color.MidnightBlue; // Debug color

      // reload all
      m_lblItems.Clear( );
      m_lblLabelItems.Clear( );
      m_valItems.Clear( );
      m_valLabelItems.Clear( );

      // we do this one by one..
      // NOTE: ONLY ONE BUTTON Control per item, One may leave out the Value item e.g. for AP and other non value items

      GItem item; Control l, v;
      item = GItem.Ad;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_ICAO( lblProto ) { Name = $"V_{item}", Text = "" }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );
      // TRIMS - we use value2Proto to get a smaller font for the numbers
      item = GItem.ETrim;
      // the ETrim label gets a button to activate the AutoTrim Module
      l = new B_ICAO( item, lblProto ) { Name = $"L_{item}", Text = BarName( item ), BackColor = c_ActBG }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Prct( value2Proto ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.RTrim;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Prct( value2Proto ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.ATrim;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Prct( value2Proto ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      // OAT, BARO
      item = GItem.OAT;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Temp( value2Proto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.BARO_HPA;
      l = new B_ICAO( item, lblProto ) { Name = $"L_{item}", Text = BarName( item ), BackColor = c_ActBG }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_PressureHPA( value2Proto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.BARO_InHg;
      l = new B_ICAO( item, lblProto ) { Name = $"L_{item}", Text = BarName( item ), BackColor = c_ActBG }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_PressureInHg( value2Proto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      // Gear, Brakes, Flaps
      item = GItem.Gear;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Steps( signProto ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.Brakes;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Steps( signProto ) { Name = $"V_{item}", ForeColor = c_RA, BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.Flaps;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Steps( signProto ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      // TORQ, PRPM, ERPM, FFLOW - we use value2Proto to get a smaller font for the numbers
      item = GItem.E1_TORQP;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Prct( value2Proto ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );
      item = GItem.E2_TORQP;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Prct( value2Proto ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.E1_TORQ;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Torque( value2Proto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );
      item = GItem.E2_TORQ;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Torque( value2Proto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.P1_RPM;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_RPM( value2Proto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );
      item = GItem.P2_RPM;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_RPM( value2Proto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.E1_RPM;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_RPM( value2Proto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );
      item = GItem.E2_RPM;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_RPM( value2Proto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.E1_N1;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Prct( value2Proto ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );
      item = GItem.E2_N1;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Prct( value2Proto ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.E1_ITT;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Temp( value2Proto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );
      item = GItem.E2_ITT;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Temp( value2Proto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.E1_EGT;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Temp( value2Proto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );
      item = GItem.E2_EGT;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Temp( value2Proto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.E1_FFlow;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Flow( value2Proto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );
      item = GItem.E2_FFlow;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Flow( value2Proto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      // GPS
      item = GItem.GPS_PWYP;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_ICAO( valueProto ) { Name = $"V_{item}", ForeColor = c_Gps, BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.GPS_NWYP;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_ICAO( valueProto ) { Name = $"V_{item}", ForeColor = c_Gps, BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.GPS_DIST;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Dist( valueProto, showUnits ) { Name = $"V_{item}", ForeColor = c_Gps, BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.GPS_ETE;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Time( valueProto ) { Name = $"V_{item}", ForeColor = c_Gps, BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.GPS_TRK;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Deg( valueProto ) { Name = $"V_{item}", ForeColor = c_Gps, BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.GPS_GS;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Speed( valueProto, showUnits ) { Name = $"V_{item}", ForeColor = c_Gps, BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      // Aircraft Data
      item = GItem.HDG;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Deg( valueProto ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.ALT;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Alt( valueProto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.RA;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Alt( valueProto, showUnits ) { Name = $"V_{item}", ForeColor = c_RA, BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.IAS;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Speed( valueProto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      item = GItem.VS;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_VSpeed( valueProto, showUnits ) { Name = $"V_{item}", BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      // Autopilot
      item = GItem.AP;
      l = new B_ICAO( item, valueProto ) { Name = $"L_{item}", Text = BarName( item ), BackColor = c_ActBG }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );

      // AP Heading
      item = GItem.AP_HDG;
      l = new B_ICAO( item, value2Proto ) { Name = $"L_{item}", Text = BarName( item ), BackColor = c_ActBG }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );

      item = GItem.AP_HDGset;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Deg( value2Proto ) { Name = $"V_{item}", ForeColor = c_Set, BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      // AP Altitude
      item = GItem.AP_ALT;
      l = new B_ICAO( item, value2Proto ) { Name = $"L_{item}", Text = BarName( item ), BackColor = c_ActBG }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );

      item = GItem.AP_ALTset;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Alt( value2Proto, showUnits ) { Name = $"V_{item}", ForeColor = c_Set, BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      // AP VSpeed
      item = GItem.AP_VS;
      l = new B_ICAO( item, value2Proto ) { Name = $"L_{item}", Text = BarName( item ), BackColor = c_ActBG }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );

      item = GItem.AP_VSset;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_VSpeed( value2Proto, showUnits ) { Name = $"V_{item}", ForeColor = c_Set, BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      // AP FLChange
      item = GItem.AP_FLC;
      l = new B_ICAO( item, value2Proto ) { Name = $"L_{item}", Text = BarName( item ), BackColor = c_ActBG }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );

      item = GItem.AP_FLCset;
      l = new V_ICAO( lblProto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );
      v = new V_Speed( value2Proto, showUnits ) { Name = $"V_{item}", ForeColor = c_Set, BackColor = backCol }; m_valItems.Add( item, (IValue)v ); m_valLabelItems.Add( item, v );

      // AP Nav, Apr, GS
      item = GItem.AP_NAV;
      l = new B_ICAO( item, value2Proto ) { Name = $"L_{item}", Text = BarName( item ), BackColor = c_ActBG }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );

      item = GItem.AP_NAVgps;
      l = new V_ICAO( value2Proto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );

      item = GItem.AP_APR;
      l = new B_ICAO( item, value2Proto ) { Name = $"L_{item}", Text = BarName( item ), BackColor = c_ActBG }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );

      item = GItem.AP_GS;
      l = new V_ICAO( value2Proto ) { Name = $"L_{item}", Text = BarName( item ) }; m_lblItems.Add( item, (IValue)l ); m_lblLabelItems.Add( item, l );

      // Apply Unit mod to Values
      foreach ( var lx in m_valItems ) {
        lx.Value.ShowUnit = ShowUnits;
      }

    }

    /// <summary>
    /// Returns the Value IF for an Item
    ///   BEWARE no checks for validity of the item ..
    /// </summary>
    /// <param name="item">A valid GItem</param>
    /// <returns>The Value Interface</returns>
    public IValue Value( GItem item )
    {
      try {
        return m_valItems[item];
      }
      catch {
        return null;
      }
    }

    /// <summary>
    /// Returns the ValueLabel for an Item
    ///   BEWARE no checks for validity of the item ..
    /// </summary>
    /// <param name="item">A valid GItem</param>
    /// <returns>The Value Label</returns>
    public Control ValueControl( GItem item )
    {
      try {
        return m_valLabelItems[item];
      }
      catch {
        return null;
      }
    }

    /// <summary>
    /// Returns the Value IF for an Label Item
    ///   BEWARE no checks for validity of the item ..
    /// </summary>
    /// <param name="item">A valid GItem</param>
    /// <returns>The Value Interface</returns>
    public IValue LabelValue( GItem item )
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
    /// <param name="item">A valid GItem</param>
    /// <returns>The Label Label</returns>
    public Control LabelControl( GItem item )
    {
      try {
        return m_lblLabelItems[item];
      }
      catch {
        return null;
      }
    }

    /// <summary>
    /// Returns the Config Name of a GItem
    /// </summary>
    /// <param name="item">A GItem</param>
    /// <returns>The Config Name</returns>
    public string CfgName( GItem item )
    {
      try {
        return m_cfgNames[item];
      }
      catch {
        return $"Cfg Name undef {item}";
      }
    }

    /// <summary>
    /// Returns the Bar Name of a GItem
    /// </summary>
    /// <param name="item">A GItem</param>
    /// <returns>The Bar Name</returns>
    public string BarName( GItem item )
    {
      try {
        return m_barNames[item];
      }
      catch {
        return $"Bar Name undef {item}";
      }
    }


  }
}

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
    GPS_DIST,  // GPS Distance to next Waypoint
    GPS_ETE,   // GPS Time to next Waypoint
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

  }


  /// <summary>
  /// All items shown in the hudBar 
  /// These are to access and to the processing
  /// Mainly those include Engine 1/2 details
  /// </summary>
  internal enum GItem
  {
    Ad = 0, // MSFS connection status...
    SimRate, // simulation rate

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

    E1_FFlow_pph,  // Fuel1 Flow pph
    E2_FFlow_pph,  // Fuel2 Flow pph

    E1_FFlow_gph,  // Fuel1 Flow gph
    E2_FFlow_gph,  // Fuel2 Flow gph

    GPS_PWYP, // GPS Prev Waypoint
    GPS_NWYP, // GPS Next Waypoint
    GPS_DIST,  // GPS Distance to next Waypoint
    GPS_ETE,  // GPS Time to next Waypoint
    GPS_TRK,  // GPS Track 000°
    GPS_GS,  // GPS Groundspeed 000kt

    GPS_ALT,  // GPS next Waypoint Altitude
    EST_VS,   // Estimate VS to reach WYP@Altitude
    EST_ALT,  // Estimate ALT@WYP

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

    M_Elapsed1, // Checkpoint 1
    M_Dist1, // Checkpoint 1
    M_Elapsed2, // Checkpoint 2
    M_Dist2, // Checkpoint 2
    M_Elapsed3, // Checkpoint 3
    M_Dist3, // Checkpoint 3

    A_ETRIM,  // Auto ETrim

    E1_MAN,   // Man Pressure InHg
    E2_MAN,   // Man Pressure InHg

    GPS_BRGm,  // GPS Mag BRG to Waypoint 000°
    GPS_DTRK,  // GPS Desired Track to Waypoint 000°
    GPS_XTK,   // GPS CrossTrack Error nm

    AOA,      // Angle of attack deg
    TAS,      // true airspeed kt
    ACFT_ID,  // aircraft ID
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
    public Color c_Est = Color.Plum;        // Estimates 
    public Color c_SRATE = Color.Goldenrod;  // SimRate >1

    // Background
    public Color c_ActBG = Color.FromArgb(00,00,75); // Active Background
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

      {LItem.ETrim,"E-Trim" }, {LItem.RTrim,"R-Trim" }, {LItem.ATrim,"A-Trim" },
      {LItem.OAT,"OAT" },
      {LItem.BARO_HPA,"BARO" }, {LItem.BARO_InHg,"BARO" },
      {LItem.Gear,"Gear" }, {LItem.Brakes,"Brakes" }, {LItem.Flaps,"Flaps" },

      {LItem.TORQ,"TORQ" },
      {LItem.TORQP,"TORQ" },
      {LItem.PRPM,"P-RPM" },
      {LItem.ERPM,"E-RPM" },
      {LItem.N1,"N1" },
      {LItem.ITT,"ITT" },
      {LItem.EGT,"EGT" },
      {LItem.FFlow_pph,"FFLOW" }, {LItem.FFlow_gph,"FFLOW" },

      {LItem.GPS_WYP,"≡GPS≡" },
      {LItem.GPS_DIST,"DIST" },
      {LItem.GPS_ETE, "ETE" },
      {LItem.GPS_TRK, "TRK" },
      {LItem.GPS_GS, "GS" },
      {LItem.GPS_ALT, "ALTP" },
      {LItem.EST_VS, "WP-VS" },
      {LItem.EST_ALT, "WP-ALT" },

      {LItem.HDG,"HDG" },
      {LItem.ALT,"ALT" },
      {LItem.RA,"RA" },
      {LItem.IAS,"IAS" },
      {LItem.VS,"VS" },

      {LItem.AP,"≡AP≡" },
      {LItem.AP_HDGs,"HDG" },
      {LItem.AP_ALTs,"ALT" },
      {LItem.AP_VSs,"VS" },
      {LItem.AP_FLCs,"FLC" },
      {LItem.AP_NAVg,"NAV" },
      {LItem.AP_APR_GS,"APR" },

      {LItem.M_TIM_DIST1,"CP 1" },
      {LItem.M_TIM_DIST2,"CP 2" },
      {LItem.M_TIM_DIST3,"CP 3" },

      {LItem.A_ETRIM,"A-ETrim" },

      {LItem.MAN,"MAN" },

      {LItem.GPS_BRGm,"BRG" },
      {LItem.GPS_DTRK,"DTK" },
      {LItem.GPS_XTK,"XTK" },

      {LItem.AOA,"AoA" },
      {LItem.TAS,"TAS" },
      {LItem.ACFT_ID,"ID" },

    };

    // Descriptive GUI label names to match the enum above (shown in Config)
    private Dictionary<LItem,string> m_cfgNames = new Dictionary<LItem, string>(){
      {LItem.MSFS,"MSFS Status" },
      {LItem.SimRate,"Sim Rate" },

      {LItem.ETrim,"Elevator Trim" }, {LItem.RTrim,"Rudder Trim" }, {LItem.ATrim,"Aileron Trim" },
      {LItem.OAT,"Outsite Air Temp °C" },
      {LItem.BARO_HPA,"Baro Setting hPa" },
      {LItem.BARO_InHg,"Baro Setting InHg" },
      {LItem.Gear,"Gear" }, {LItem.Brakes,"Brakes" }, {LItem.Flaps,"Flaps" },

      {LItem.TORQ,"Torque ft/lb" },
      {LItem.TORQP,"Torque %" },
      {LItem.PRPM,"Propeller RPM" },
      {LItem.ERPM,"Engine RPM" },
      {LItem.N1,"Turbine N1" },
      {LItem.ITT,"Turbine ITT °C" },
      {LItem.EGT,"Engine EGT °C" },
      {LItem.FFlow_pph,"Fuel Flow pph" },
      {LItem.FFlow_gph,"Fuel Flow gph" },

      {LItem.GPS_WYP,"≡GPS≡" },
      {LItem.GPS_DIST,"WYP Distance nm" },
      {LItem.GPS_ETE,"WYP ETE h:mm:ss" },
      {LItem.GPS_TRK,"Current Track" },
      {LItem.GPS_GS,"Groundspeed" },
      {LItem.GPS_ALT,"Waypoint ALT ft" },
      {LItem.EST_VS,"Estimate VS to WYP@ALT" },
      {LItem.EST_ALT,"Estimated ALT @WYP" },

      {LItem.HDG,"Aircraft HDG" },
      {LItem.ALT,"Aircraft ALT ft" },
      {LItem.RA,"Aircraft RA ft" },
      {LItem.IAS,"Aircraft IAS kt" },
      {LItem.VS,"Aircraft VS fpm" },

      {LItem.AP,"Autopilot Master" },
      {LItem.AP_HDGs,"AP HDG / Set" },
      {LItem.AP_ALTs,"AP ALT / Set" },
      {LItem.AP_VSs,"AP VS / Set" },
      {LItem.AP_FLCs,"AP FLC / Set" },
      {LItem.AP_NAVg,"AP NAV and GPS" },
      {LItem.AP_APR_GS,"AP APR and GS" },

      {LItem.M_TIM_DIST1,"Checkpoint 1" },
      {LItem.M_TIM_DIST2,"Checkpoint 2" },
      {LItem.M_TIM_DIST3,"Checkpoint 3" },

      {LItem.A_ETRIM,"Auto E-Trim" },

      {LItem.MAN,"MAN Pressure inHg" },

      {LItem.GPS_BRGm,"Bearing to WYP (mag)" },
      {LItem.GPS_DTRK,"Desired track to WYP" },
      {LItem.GPS_XTK,"Cross track distance nm" },

      {LItem.AOA,"Angle of attack deg" },
      {LItem.TAS,"Aircraft TAS kt" },
      {LItem.ACFT_ID,"Aircraft ID" },
    };

    // Bar value label names to match the enum above
    // Used when the individual value item is meant and the text here is the value 
    //  only used as GPS and GS  for NAV / GPS and APR / GS the rest is for completenes only
    private Dictionary<GItem,string> m_barValueLabels = new Dictionary<GItem, string>(){
      {GItem.AP_NAVgps,"GPS" },
      {GItem.AP_GS,"►GS◄" },
    };

    // maintain collections of the created Controls to do the processing

    // Label Items (some are buttons, for some there are color changes
    private Dictionary<GItem, IValue> m_lblItems = new Dictionary<GItem, IValue>();
    private Dictionary<GItem,Control> m_lblLabelItems = new Dictionary<GItem, Control>();
    private void AddLbl( GItem item, Control ctrl )
    {
      m_lblItems.Add( item, (IValue)ctrl );
      m_lblLabelItems.Add( item, ctrl );
    }

    // Value Items (updated from Sim, some may change colors)
    private  Dictionary<GItem, IValue>  m_valItems = new Dictionary<GItem, IValue> ();
    private Dictionary<GItem,Control> m_valLabelItems = new Dictionary<GItem,Control>();
    private void AddValue( GItem item, Control ctrl )
    {
      m_valItems.Add( item, (IValue)ctrl );
      m_valLabelItems.Add( item, ctrl );
    }

    // The Display Groups (DispItem is essentially a smaller FlowLayoutPanel containing the controls)
    private  Dictionary<LItem, DispItem>  m_dispItems = new Dictionary<LItem, DispItem> ();
    private void AddDisp( LItem item, DispItem ctrl )
    {
      m_dispItems.Add( item, ctrl );
    }

    private static GUI_Fonts FONTS = null; // handle fonts as static item to not waste resources

    /// <summary>
    /// Init the Hud Items - providing prototypes for the various label types
    /// </summary>
    /// <param name="lblProto">A GUI prototype label, carrying fonts, colors etc (set in GUI design mode)</param>
    /// <param name="valueProto">A GUI prototype value, carrying fonts, colors etc (set in GUI design mode)</param>
    /// <param name="value2Proto">A GUI prototype value type 2, carrying fonts, colors etc (set in GUI design mode)</param>
    /// <param name="signProto">A GUI prototype icon(Wingdings), carrying fonts, colors etc (set in GUI design mode)</param>
    /// <param name="showUnits">Showing units flag</param>
    /// <param name="opaque">Opaque flag</param>
    /// <param name="fontSize">Used fontsize</param>
    /// <param name="placement">Screen alignment of the bar</param>
    public HudBar( Label lblProto, Label valueProto, Label value2Proto, Label signProto,
                    bool showUnits, bool opaque, FontSize fontSize, Placement placement, Kind kind )
    {
      // just save them in the HUD mainly for config purpose
      ShowUnits = showUnits;
      OpaqueBackground = opaque;

      FontSize = fontSize;
      Placement = placement;
      Kind = kind;

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
      c_BG = lblProto.BackColor;
      //c_BG = Color.DarkBlue; // Debug color to see control outlines

      // reload all
      m_lblItems.Clear( );
      m_lblLabelItems.Clear( );
      m_valItems.Clear( );
      m_valLabelItems.Clear( );
      m_dispItems.Clear( );

      // we do this one by one..
      // NOTE: ONLY ONE BUTTON Control per item, One may leave out the Value item e.g. for AP and other non value items
      // Button Labels need to be added to the label collection else one cannot access them

      GItem item; LItem disp;
      Control l, v; DispItem di = null;

      disp = LItem.MSFS; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.Ad;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l ); AddLbl( item, l ); // access for Error Msg etc
      v = new V_ICAO( lblProto ) { Text = "" }; di.AddItem( v ); AddValue( item, v );

      // Sim Rate
      disp = LItem.SimRate; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.SimRate;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_SRate( lblProto ) { }; di.AddItem( v ); AddValue( item, v );

      // TRIMS - we use value2Proto to get a smaller font for the numbers
      disp = LItem.ETrim; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.ETrim;
      // the ERA-Trim label gets a button to activate the 0 Trim action
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.RTrim; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.RTrim;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.ATrim; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.ATrim;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // OAT, BARO
      disp = LItem.OAT; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.OAT;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Temp( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.BARO_HPA; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.BARO_HPA;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_PressureHPA( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.BARO_InHg; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.BARO_InHg;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_PressureInHg( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // Gear, Brakes, Flaps
      disp = LItem.Gear; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.Gear;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Steps( signProto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.Brakes; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.Brakes;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Steps( signProto ) { ForeColor = c_RA, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.Flaps; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.Flaps;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Steps( signProto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // TORQ, PRPM, ERPM, FFLOW - we use value2Proto to get a smaller font for the numbers
      disp = LItem.TORQP; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.E1_TORQP;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = GItem.E2_TORQP;
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      disp = LItem.TORQ; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.E1_TORQ;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Torque( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = GItem.E2_TORQ;
      v = new V_Torque( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      disp = LItem.PRPM; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.P1_RPM;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_RPM( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = GItem.P2_RPM;
      v = new V_RPM( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      disp = LItem.ERPM; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.E1_RPM;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_RPM( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = GItem.E2_RPM;
      v = new V_RPM( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      disp = LItem.N1; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.E1_N1;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = GItem.E2_N1;
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      disp = LItem.ITT; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.E1_ITT;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Temp( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = GItem.E2_ITT;
      v = new V_Temp( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      disp = LItem.EGT; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.E1_EGT;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Temp( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = GItem.E2_EGT;
      v = new V_Temp( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      disp = LItem.FFlow_pph; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.E1_FFlow_pph;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Flow_pph( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = GItem.E2_FFlow_pph;
      v = new V_Flow_pph( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.FFlow_gph; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.E1_FFlow_gph;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Flow_gph( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = GItem.E2_FFlow_gph;
      v = new V_Flow_gph( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // GPS
      disp = LItem.GPS_WYP; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.GPS_PWYP;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_ICAO( valueProto ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = GItem.GPS_NWYP;
      v = new V_ICAO( valueProto ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.GPS_DIST; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.GPS_DIST;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Dist( valueProto, showUnits ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.GPS_ETE; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.GPS_ETE;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Time( valueProto ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.GPS_TRK; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.GPS_TRK;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Deg( valueProto ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.GPS_GS; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.GPS_GS;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Speed( valueProto, showUnits ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.GPS_ALT; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.GPS_ALT;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Alt( valueProto, showUnits ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.EST_VS; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.EST_VS;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_VSpeed( valueProto, showUnits ) { ForeColor = c_Est, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      disp = LItem.EST_ALT; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.EST_ALT;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Alt( valueProto, showUnits ) { ForeColor = c_Est, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // Aircraft Data
      disp = LItem.HDG; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.HDG;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Deg( valueProto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.ALT; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.ALT;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Alt( valueProto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.RA; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.RA;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Alt( valueProto, showUnits ) { ForeColor = c_RA, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.IAS; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.IAS;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Speed( valueProto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.VS; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.VS;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_VSpeed( valueProto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // Autopilot
      disp = LItem.AP; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.AP;
      l = new B_ICAO( item, valueProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item

      // AP Heading
      disp = LItem.AP_HDGs; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.AP_HDG;
      l = new B_ICAO( item, value2Proto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      item = GItem.AP_HDGset;
      v = new V_Deg( value2Proto ) { ForeColor = c_Set, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // AP Altitude
      disp = LItem.AP_ALTs; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.AP_ALT;
      l = new B_ICAO( item, value2Proto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      item = GItem.AP_ALTset;
      v = new V_Alt( value2Proto, showUnits ) { ForeColor = c_Set, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // AP VSpeed
      disp = LItem.AP_VSs; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.AP_VS;
      l = new B_ICAO( item, value2Proto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      item = GItem.AP_VSset;
      v = new V_VSpeed( value2Proto, showUnits ) { ForeColor = c_Set, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // AP FLChange
      disp = LItem.AP_FLCs; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.AP_FLC;
      l = new B_ICAO( item, value2Proto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      item = GItem.AP_FLCset;
      v = new V_Speed( value2Proto, showUnits ) { ForeColor = c_Set, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      // AP Nav, Apr, GS
      disp = LItem.AP_NAVg; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.AP_NAV;
      l = new B_ICAO( item, value2Proto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      item = GItem.AP_NAVgps;
      l = new V_ICAO( value2Proto ) { Text = BarValueLabel( item ) }; di.AddItem( l ); AddLbl( item, l );

      disp = LItem.AP_APR_GS; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.AP_APR;
      l = new B_ICAO( item, value2Proto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      item = GItem.AP_GS;
      l = new V_ICAO( value2Proto ) { Text = BarValueLabel( item ) }; di.AddItem( l ); AddLbl( item, l );

      disp = LItem.M_TIM_DIST1; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.M_Elapsed1;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_Time( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = GItem.M_Dist1;
      v = new V_Dist( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.M_TIM_DIST2; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.M_Elapsed2;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_Time( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = GItem.M_Dist2;
      v = new V_Dist( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.M_TIM_DIST3; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.M_Elapsed3;
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_Time( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = GItem.M_Dist3;
      v = new V_Dist( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.A_ETRIM; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.A_ETRIM;
      // the Auto E-Trim label gets a button to activate the AutoTrim Module
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.MAN; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.E1_MAN;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_PressureInHg( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = GItem.E2_MAN;
      v = new V_PressureInHg( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.GPS_BRGm; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.GPS_BRGm;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Deg( valueProto ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.GPS_DTRK; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.GPS_DTRK;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Deg( valueProto ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.GPS_XTK; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.GPS_XTK;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Dist2( valueProto, showUnits ) { ForeColor = c_Gps, BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.AOA; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.AOA;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Angle( valueProto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.TAS; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.TAS;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Speed( valueProto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.ACFT_ID; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.ACFT_ID;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_ICAO( valueProto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );


      // Apply Unit mod to Values
      foreach ( var lx in m_valItems ) {
        lx.Value.ShowUnit = ShowUnits;
      }

      if ( placement == Placement.Left || placement == Placement.Right ) {
        // Determine max width and make them aligned
        int maxLabelWidth = 0;
        foreach ( var lx in m_dispItems ) {
          var dix = lx.Value as DispItem;
          maxLabelWidth = ( dix.Controls[0].Width > maxLabelWidth ) ? dix.Controls[0].Width : maxLabelWidth;
        }
        foreach ( var lx in m_dispItems ) {
          var dix = lx.Value as DispItem;
          dix.Controls[0].Padding = new Padding( 0, 0, maxLabelWidth - dix.Controls[0].Width, 0 );
        }
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
    /// Returns the Display Item
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
    /// Returns the Config Name of a GItem
    /// </summary>
    /// <param name="item">A GItem</param>
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
    /// Returns the Bar Value Label of a GItem
    /// </summary>
    /// <param name="item">A GItem</param>
    /// <returns>The Bar Value Label</returns>
    public string BarValueLabel( GItem item )
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

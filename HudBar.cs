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
    FFlow,
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

    E1_FFlow,  // Fuel1 Flow pph
    E2_FFlow,  // Fuel2 Flow pph

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
    /// The FontSize to use
    /// </summary>
    public FontSize FontSize { get; set; } = FontSize.Regular;

    /// <summary>
    /// Placement of the Bar
    /// </summary>
    public Placement Placement { get; set; } = Placement.Bottom;



    // Hud Bar label names to match the enum above (as short as possible)
    private Dictionary<LItem,string> m_guiNames = new Dictionary<LItem, string>(){
      {LItem.MSFS,"MSFS" },
      {LItem.SimRate,"SR" },

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
      {LItem.FFlow,"FFLOW" },

      {LItem.GPS_WYP,"≡GPS≡" },
      {LItem.GPS_DIST,"DIST" },
      {LItem.GPS_ETE,"ETE" },
      {LItem.GPS_TRK,"TRK" },
      {LItem.GPS_GS,"GS" },
      {LItem.GPS_ALT,"ALTP" },
      {LItem.EST_VS,"WP-VS" },
      {LItem.EST_ALT,"WP-ALT" },

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
      {LItem.AP_APR_GS,"APR/GS" },
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
      {LItem.FFlow,"Fuel Flow pph" },

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
    };

    // Bar label names to match the enum above
    private Dictionary<GItem,string> m_barNames = new Dictionary<GItem, string>(){
      {GItem.Ad,"MSFS" },
      {GItem.SimRate,"SR" },

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
      {GItem.GPS_ALT,"ALT" },
      {GItem.EST_VS,"E.VS" }, {GItem.EST_ALT,"E.ALT" },

      {GItem.HDG,"HDG" }, {GItem.ALT,"ALT" }, {GItem.RA,"RA" },{GItem.IAS,"IAS" }, {GItem.VS,"VS" },

      {GItem.AP,"≡AP≡" },
      {GItem.AP_HDG,"HDG" },{GItem.AP_HDGset, "" },
      {GItem.AP_ALT,"ALT" },{GItem.AP_ALTset, "" },
      {GItem.AP_VS,"VS" }, {GItem.AP_VSset, "" },
      {GItem.AP_FLC,"FLC" }, {GItem.AP_FLCset,"" },
      {GItem.AP_NAV,"NAV" }, {GItem.AP_NAVgps,"GPS" }, {GItem.AP_APR,"APR" }, {GItem.AP_GS,"►GS◄" },
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
    public HudBar( Label lblProto, Label valueProto, Label value2Proto, Label signProto,
                    bool showUnits, bool opaque, FontSize fontSize, Placement placement )
    {
      // just save them in the HUD mainly for config purpose
      ShowUnits = showUnits;
      OpaqueBackground = opaque;

      FontSize = fontSize;
      Placement = placement;

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
      //c_BG = Color.MidnightBlue; // Debug color to see control outlines

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
      // the ERA-Trim label gets a button to activate the AutoTrim Module
      l = new B_ICAO( item, lblProto ) { Text = GuiName( disp ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.RTrim; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.RTrim;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ), BackColor = c_BG }; di.AddItem( l );
      v = new V_Prct( value2Proto ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

      disp = LItem.ATrim; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.ATrim;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ), BackColor = c_BG }; di.AddItem( l );
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


      disp = LItem.FFlow; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.E1_FFlow;
      l = new V_ICAO( lblProto ) { Text = GuiName( disp ) }; di.AddItem( l );
      v = new V_Flow( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );
      item = GItem.E2_FFlow;
      v = new V_Flow( value2Proto, showUnits ) { BackColor = c_BG }; di.AddItem( v ); AddValue( item, v );

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
      l = new V_ICAO( value2Proto ) { Text = BarName( item ) }; di.AddItem( l ); AddLbl( item, l );

      disp = LItem.AP_APR_GS; di = new DispItem( ); AddDisp( disp, di );
      item = GItem.AP_APR;
      l = new B_ICAO( item, value2Proto ) { Text = BarName( item ), BackColor = c_ActBG }; di.AddItem( l ); AddLbl( item, l ); // Action item
      item = GItem.AP_GS;
      l = new V_ICAO( value2Proto ) { Text = BarName( item ) }; di.AddItem( l ); AddLbl( item, l );

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

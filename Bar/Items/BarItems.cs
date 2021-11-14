using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar.Bar.Items
{
  /// <summary>
  /// All Labels (Topics etc.) shown in the HudBar 
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
    OAT_C,
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
    EGT_C,    // EGT in °C
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

    THR_LEV,    // Throttle Lever
    MIX_LEV,    // Mixture Lever
    PROP_LEV,   // Propeller Lever

    OAT_F,      // OAT in °F
    EGT_F,      // EGT in °F
    CHT_C,      // CHT in °C
    CHT_F,      // CHT in °F
    LOAD_P,     // LOAD %
    N2,         // Turbine N2 %

  }


  /// <summary>
  /// All item values shown in the HudBar 
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

    OAT_C,        // Outside AirTemp °C
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
    E1_EGT_C,   // EGT 1 Celsius
    E2_EGT_C,   // EGT 2 Celsius

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

    E1_THR_LEV,    // Throttle Lever Engine 1
    E2_THR_LEV,    // Throttle Lever Engine 2
    E1_MIX_LEV,    // Mixture Lever Engine 1
    E2_MIX_LEV,    // Mixture Lever Engine 2
    E1_PROP_LEV,   // Propeller Lever Engine 1
    E2_PROP_LEV,   // Propeller Lever Engine 2

    OAT_F,         // OAT °F

    E1_EGT_F,      // EGT 1 °F
    E2_EGT_F,      // EGT 2 °F

    E1_CHT_C,      // CHT 1 °C
    E2_CHT_C,      // CHT 2 °C
    E1_CHT_F,      // CHT 1 °F
    E2_CHT_F,      // CHT 2 °F

    E1_LOAD_P,     // LOAD % 1 %
    E2_LOAD_P,     // LOAD % 2 %

    E1_N2,      // Engine1 N2 %
    E2_N2,      // Engine2 N2 %
  }

}

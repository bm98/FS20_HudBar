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
    ETRIM,    // Elevator Trim
    RTRIM,    // Rudder Trim
    ATRIM,    // Aileron Trim
    OAT_C,
    BARO_HPA,
    BARO_InHg,
    GEAR,
    BRAKES,
    FLAPS,
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
    FUEL_LR_gal,    // Fuel quantity Left/Right
    FUEL_TOT_gal, // Fuel quantity Total Gallons

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

    NAV1_NAME,  // NAV1
    NAV2_NAME,  // NAV2

    THR_LEV,    // Throttle Lever
    MIX_LEV,    // Mixture Lever
    PROP_LEV,   // Propeller Lever

    OAT_F,      // OAT in °F
    EGT_F,      // EGT in °F
    CHT_C,      // CHT in °C
    CHT_F,      // CHT in °F
    LOAD_P,     // LOAD %
    N2,         // Turbine N2 %
    FUEL_LR_lb,    // Fuel quantity Left/Right  Pounds
    FUEL_TOT_lb, // Fuel quantity Total Pounds

    FUEL_C_gal,   // Fuel quantity Center  Gallons
    FUEL_C_lb,    // Fuel quantity Center  Pounds
    GFORCE,       // GForce
    GFORCE_MM,    // GForce Min Max
    AFTB,         // Afterburner
    SPOILERS,     // Spoiler/Speedbrakes
    FPS,          // FPS value
    ZULU,         // Zulu Time (Sim)
    CTIME,        // Computer Time
    XPDR,         // Transponder

    LOG,          // Flight Log
  }


  /// <summary>
  /// All item values shown in the HudBar 
  /// These are to access and do the processing
  /// Those may include Engine 1/2 details
  /// </summary>
  internal enum VItem
  {
    // SIM
    Ad = 0,   // MSFS connection status...
    SimRate,  // simulation rate
    FPS,          // FPS value
    LOG,          // Flight Log

    ACFT_ID,      // aircraft ID

    // ENVIRONMENT
    TIME,         // Time of Day
    ZULU,         // Zulu Time (Sim)
    CTIME,        // Computer Time
    OAT_F,        // OAT °F
    OAT_C,        // Outside AirTemp °C
    BARO_HPA,     // Altimeter Setting HPA
    BARO_InHg,    // Altimeter Setting InHg
    VIS,          // Visibility nm
    WIND_DIR,     // Wind direction °
    WIND_SPEED,   // Wind speed kt
    WIND_LAT,     // Wind lateral comp kt
    WIND_LON,     // Wind longitudinal comp kt

    // TRIMs
    ETRIM,        // Elevator Trim +-N%, active: set to zero
    A_ETRIM,      // Auto ETrim, activates ETrim Module, shows ETrim % (same as the standard one)
    RTRIM,        // Rudder Trim +-N%, active: set to zero
    ATRIM,        // Aileron Trim +-N%, active: set to zero

    // ACFT
    GEAR,         // Gear Up/Down
    BRAKES,       // Brakes On Off
    FLAPS,        // Flaps N level 
    SPOLIERS,     // Spoilers / Speedbrakes
    LIGHTS,       // Lights indication

    // RPMs
    P1_RPM,       // Prop1 RPM
    P2_RPM,       // Prop2 RPM
    E1_RPM,       // Engine1 RPM
    E2_RPM,       // Engine2 RPM

    // ENGINE
    E1_MAN,       // Engine1 MAN Pressure InHg
    E2_MAN,       // Engine2 MAN Pressure InHg
    E1_LOAD_P,    // Engine1 LOAD % 1 %
    E2_LOAD_P,    // Engine2 LOAD % 2 %

    E1_EGT_C,     // Engine1 EGT °C
    E2_EGT_C,     // Engine2 EGT °C
    E1_EGT_F,     // Engine1 EGT °F
    E2_EGT_F,     // Engine2 EGT °F

    E1_CHT_C,     // Engine1 CHT °C
    E2_CHT_C,     // Engine2 CHT °C
    E1_CHT_F,     // Engine1 CHT °F
    E2_CHT_F,     // Engine2 CHT °F

    E1_TORQP,     // Engine1 Torque %
    E2_TORQP,     // Engine2 Torque %
    E1_TORQ,      // Engine1 Torque ft/lb
    E2_TORQ,      // Engine2 Torque ft/lb

    E1_ITT,       // Engine1 ITT °C
    E2_ITT,       // Engine2 ITT °C

    E1_N1,        // Engine1 N1 %
    E2_N1,        // Engine2 N1 %
    E1_N2,        // Engine1 N2 %
    E2_N2,        // Engine2 N2 %
    E1_AFTB,      // Engine1 Afterburner %
    E2_AFTB,      // Engine2 Afterburner %

    // FUEL
    E1_FFlow_pph,   // Fuel1 Flow pph
    E2_FFlow_pph,   // Fuel2 Flow pph

    E1_FFlow_gph,   // Fuel1 Flow gph
    E2_FFlow_gph,   // Fuel2 Flow gph

    FUEL_L_gal,     // Fuel quantity Left Gallons
    FUEL_R_gal,     // Fuel quantity Right Gallons
    FUEL_C_gal,     // Fuel quantity Center Gallons
    FUEL_TOT_gal,   // Fuel quantity Total Gallons
    FUEL_REACH_gal, // Fuel reach in seconds for the Gallons Readout

    FUEL_L_lb,      // Fuel quantity Left Pounds
    FUEL_R_lb,      // Fuel quantity Right Pounds
    FUEL_C_lb,      // Fuel quantity Center Pounds
    FUEL_TOT_lb,    // Fuel quantity Total Pounds
    FUEL_REACH_lb,  // Fuel reach in seconds for the Pounds Readout

    // GPS
    GPS_PWYP,     // GPS Prev Waypoint
    GPS_NWYP,     // GPS Next Waypoint
    GPS_ETE,      // GPS Time to Destination
    GPS_WP_DIST,  // GPS Distance to next Waypoint
    GPS_WP_ETE,   // GPS Time to next Waypoint
    GPS_TRK,      // GPS Track 000°
    GPS_GS,       // GPS Groundspeed 000kt
    GPS_BRGm,     // GPS Mag BRG to Waypoint 000°
    GPS_DTRK,     // GPS Desired Track to Waypoint 000°
    GPS_XTK,      // GPS CrossTrack Error nm
    GPS_ALT,      // GPS next Waypoint Altitude
    GPS_LAT,      // Latitude
    GPS_LON,      // Longitude

    EST_VS,       // Estimate VS to reach WYP@Altitude
    EST_ALT,      // Estimate ALT@WYP

    // AVIONICS ACFT
    HDG,          // Heading Mag 000°
    HDGt,         // True heading deg
    ALT_INST,     // Instrument Altitude ft
    ALT,          // Altitude 00000 ft
    RA,           // Radio Altitude 000 ft 
    RA_VOICE,     // Radio Altitude with voice output
    IAS,          // Ind. Airspeed 000 kt
    TAS,          // true airspeed kt
    MACH,         // Mach speed indication
    VS,           // Vertical Speed +-0000 fpm
    VS_PM,        // Vertical Speed with +- fpm
    AOA,          // Angle of attack deg
    GFORCE_Cur,    // GForce Current
    GFORCE_Min,    // GForce Minimum
    GFORCE_Max,    // GForce Maximum

    // AVIONICS NAV
    NAV1_ID,      // NAV1 ID
    NAV1_NAME,    // NAV1 Name {Apt}
    NAV1_DST,     // NAV1 DME Distance
    NAV1_BRG,     // NAV1 BRG to

    NAV2_ID,      // NAV2 ID
    NAV2_NAME,    // NAV2 Name {Apt}
    NAV2_DST,     // NAV2 DME Distance
    NAV2_BRG,     // NAV2 BRG to

    XPDR_CODE,    // Transponder Code
    XPDR_STAT,    // Transponder Status

    // AUTOPILOT
    AP,           // Autopilot On/Off
    AP_HDG,       // AP HDG active
    AP_HDGset,    // AP HDG set
    AP_HDGset_sel,// AP HDG set selected slot
    AP_ALT,       // AP ALT hold active
    AP_ALTset,    // AP ALT set
    AP_ALTset_sel,// AP ALT set selected slot
    AP_VS,        // AP VS hold active
    AP_VSset,     // AP VS set
    AP_VSset_sel, // AP VS set selected slot
    AP_FLC,       // AP FLC hold active
    AP_FLCset,    // AP FLC IAS set
    AP_FLCset_sel,// AP FLC IAS set selected slot
    AP_NAV,       // AP NAV active
    AP_NAVgps,    // AP NAV follow GPS
    AP_BC,        // AP BC signal
    AP_APR,       // AP APR hold active
    AP_GS,        // AP GS  hold active
    AP_YD,        // Yaw Damper signal
    AP_LVL,       // Wing Leveler signal

    // TIMER
    M_Elapsed1,   // Time elapsed since start of CP1
    M_Dist1,      // Distance from CP1
    M_Elapsed2,   // Time elapsed since start of CP2
    M_Dist2,      // Distance from CP2
    M_Elapsed3,   // Time elapsed since start of CP3
    M_Dist3,      // Distance from CP3

    ENR_WP,       // Enroute time for this WP sec
    ENR_TOTAL,    // Enroute time for this flight sec

    // ATC
    ATC_APT,      // ATC assigned Airport
    ATC_RWY_LAT,  // Lateral displacement ft
    ATC_RWY_ALT,  // Height displacement ft
    ATC_RWY_LON,  // Longitudinal displacement nm
    ATC_APT_DIST, // Distance from ATC Apt nm (lat lon based)
    ATC_APT_ALT,  // Altitude of the ATC Apt ft MSL
    ATC_ALT,      // ATC assigned ALT ft
    ATC_HDG,      // ATC assigned HDG °
    ATC_WYP,      // ATC next Wyp

    METAR,        // METAR close to LAT,LON

    // CONTROLs
    E1_THR_LEV,    // Throttle Lever Engine 1
    E2_THR_LEV,    // Throttle Lever Engine 2
    E1_MIX_LEV,    // Mixture Lever Engine 1
    E2_MIX_LEV,    // Mixture Lever Engine 2
    E1_PROP_LEV,   // Propeller Lever Engine 1
    E2_PROP_LEV,   // Propeller Lever Engine 2
  }

}

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

    GPS_BRGm,     // GPS BRG to Waypoint 000°
    GPS_DTRK,     // GPS Desired Track to Waypoint 000°
    GPS_XTK,      // GPS CrossTrack Error nm

    AOA,          // Angle of attack deg
    TAS,          // true airspeed kt
    ACFT_ID,      // aircraft ID

    WIND_SD,      // Wind Speed and Direction
    WIND_XY,      // Wind X and Y Component

    Lights,       // Lights indication
    FUEL_LR_gal,  // Fuel quantity Left/Right
    FUEL_TOT_gal, // Fuel quantity Total Gallons

    MACH,         // Mach speed indication

    VIS,          // Visibility
    TIME,         // Time of Day
    HDGt,         // True heading
    AP_BC,        // AP BC signal
    AP_YD,        // Yaw Damper
    AP_LVL,       // Wing Leveler

    ENROUTE,      // Enroute Times
    ATC_APT,      // ATC assigned Apt arrives as TT.AIRPORTLR.ICAO.name
    ATC_RWY,      // ATC assigned RWY displacement
    GPS_ETE,      // GPS Time to Destination

    GPS_LAT_LON,  // Lat and Lon from GPS
    METAR,        // METAR close to Lat and Lon
    ALT_INST,     // Instrument Altitude

    ATC_ALT_HDG,  // ATC assigned ALT and HDG

    VS_PM,        // Vertical Speed with +-

    RA_VOICE,     // Radio Altitude with voice output

    NAV1,         // NAV1
    NAV2,         // NAV2

    NAV1_NAME,    // NAV1
    NAV2_NAME,    // NAV2

    THR_LEV,      // Throttle Lever
    MIX_LEV,      // Mixture Lever
    PROP_LEV,     // Propeller Lever

    OAT_F,        // OAT in °F
    EGT_F,        // EGT in °F
    CHT_C,        // CHT in °C
    CHT_F,        // CHT in °F
    LOAD_P,       // LOAD %
    N2,           // Turbine N2 %
    FUEL_LR_lb,   // Fuel quantity Left/Right  Pounds
    FUEL_TOT_lb,  // Fuel quantity Total Pounds

    FUEL_C_gal,   // Fuel quantity Center  Gallons
    FUEL_C_lb,    // Fuel quantity Center  Pounds
    GFORCE,       // GForce
    GFORCE_MM,    // GForce Min Max
    AFTB,         // Afterburner
    SPOILER,      // Spoiler/Speedbrakes
    FPS,          // FPS value
    ZULU,         // Zulu Time (Sim)
    CTIME,        // Computer Time
    XPDR,         // Transponder

    LOG,          // Flight Log

    AP_APR_INFO,  // AP/GPS Approach Info
    FP_ANGLE,     // Flight Path Angle
    FFlow_kgh,    // Fuel Flow kg/h
    FUEL_LR_kg,   // Fuel quantity LR kilograms
    FUEL_C_kg,    // Fuel quantity C kilograms
    FUEL_TOT_kg,  // Fuel quantity Total kilograms
    VARIO_MPS,    // Total Energy Variometer m/sec with +-
    VARIO_KTS,    // Total Energy Variometer kts with +-

    FUEL_ANI,     // Fuel Graph
    VARIO_ANI,    // Vario Graph
    N1_ANI,       // N1 Graph
    N2_ANI,       // N2 Graph
    ERPM_ANI,     // Engine RPM Graph
    PRPM_ANI,     // Prop RPM Graph
    COWL_ANI,     // Cowl Flaps Graph
    TORQP_ANI,    // Turbine Torque Percent Graph
    AFTB_ANI,     // Afterburner % Graph
    FLAPS_ANI,    // Flaps Graph
    SPOILER_ANI,  // Spoilers Graph
    ESI_ANI,      // ESI & FPA Visual

    TXT,          // Free Text
    AP_ATT,       // Attitude Hold
    AP_ATHR,      // Auto Throttle (active, TOGA)
    AP_ABRK,      // Auto Brake (active)
    AP_SPDs,      // Auto Speed (IAS)
    COM1,         // COM1
    COM1_NAME,    // COM1 ID
    COM2,         // COM2
    COM2_NAME,    // COM2 ID
    NAV1_F,       // NAV1 Frequ
    NAV2_F,       // NAV2 Frequ
    A320THR,      // A320 Throttle

    SURF_ANI,     // Control Surfaces Graph

    DEPARR,       // Departure Arrival Apt

    VWIND,       // Vertical WindComponent
    COMPASS,     // Mag Compass

    GPS_DST,     // GPS or Calc Distance to Destination
    ADF1,        // ADF1 Direction
    ADF1_NAME,   // ADF1 ID,Name
    ADF1_F,      // ADF1 Frequ

    AP_VNAV,     // VNAV on G1000
    GPS_TOD,     // GPS distance to TOD 
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
    TXT,          // Free Text

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
    WIND_DIRA,    // Wind direction ° Arrow
    WIND_SPEED,   // Wind speed kt
    WIND_LAT,     // Wind lateral comp kt
    WIND_LON,     // Wind longitudinal comp kt

    VWIND,        // Vertical WindComponent kt
    VWIND_ANI,    // Vertical WindComponent Dot

    // TRIMs
    ETRIM,        // Elevator Trim +-N%, active: set to zero
    A_ETRIM,      // Auto ETrim, activates ETrim Module, shows ETrim % (same as the standard one)
    RTRIM,        // Rudder Trim +-N%, active: set to zero
    ATRIM,        // Aileron Trim +-N%, active: set to zero

    // ACFT
    GEAR,         // Gear Up/Down
    BRAKES,       // Brakes On Off
    FLAPS,        // Flaps N level 
    FLAPS_ANI,    // Flaps Graph
    SPOLIER,      // Spoilers / Speedbrakes
    SPOILER_ANI,  // Spoilers Graph
    LIGHTS,       // Lights indication

    // RPMs
    P1_RPM,       // Prop1 RPM
    P2_RPM,       // Prop2 RPM
    P3_RPM,       // Prop3 RPM
    P4_RPM,       // Prop4 RPM

    P1_RPM_ANI,   // Prop RPM Graph Single Engine
    P2_RPM_ANI,   // Prop RPM Graph Twin Engine
    P3_RPM_ANI,   // Prop RPM Graph 3rd Engine
    P4_RPM_ANI,   // Prop RPM Graph 4th Engine

    E1_RPM,       // Engine1 RPM
    E2_RPM,       // Engine2 RPM
    E3_RPM,       // Engine3 RPM
    E4_RPM,       // Engine4 RPM

    E1_RPM_ANI,   // Engine RPM Graph Single Engine
    E2_RPM_ANI,   // Engine RPM Graph Twin Engine
    E3_RPM_ANI,   // Engine RPM Graph 3rd Engine
    E4_RPM_ANI,   // Engine RPM Graph 4th Engine

    // ENGINE
    E1_MAN,       // Engine1 MAN Pressure InHg
    E2_MAN,       // Engine2 MAN Pressure InHg
    E3_MAN,       // Engine3 MAN Pressure InHg
    E4_MAN,       // Engine4 MAN Pressure InHg

    E1_LOAD_P,    // Engine1 LOAD % 1 %
    E2_LOAD_P,    // Engine2 LOAD % 2 %
    E3_LOAD_P,    // Engine3 LOAD % 1 %
    E4_LOAD_P,    // Engine4 LOAD % 2 %

    E1_EGT_C,     // Engine1 EGT °C
    E2_EGT_C,     // Engine2 EGT °C
    E3_EGT_C,     // Engine3 EGT °C
    E4_EGT_C,     // Engine4 EGT °C

    E1_EGT_F,     // Engine1 EGT °F
    E2_EGT_F,     // Engine2 EGT °F
    E3_EGT_F,     // Engine3 EGT °F
    E4_EGT_F,     // Engine4 EGT °F

    E1_CHT_C,     // Engine1 CHT °C
    E2_CHT_C,     // Engine2 CHT °C
    E3_CHT_C,     // Engine3 CHT °C
    E4_CHT_C,     // Engine4 CHT °C

    E1_CHT_F,     // Engine1 CHT °F
    E2_CHT_F,     // Engine2 CHT °F
    E3_CHT_F,     // Engine3 CHT °F
    E4_CHT_F,     // Engine4 CHT °F

    E1_TORQP,     // Engine1 Torque %
    E2_TORQP,     // Engine2 Torque %
    E3_TORQP,     // Engine3 Torque %
    E4_TORQP,     // Engine4 Torque %

    E1_TORQP_ANI,  // Turbine Torque Percent Graph Single Engine
    E2_TORQP_ANI,  // Turbine Torque Percent Graph Twin Engine
    E3_TORQP_ANI,  // Turbine Torque Percent Graph 3rd Engine
    E4_TORQP_ANI,  // Turbine Torque Percent Graph 4th Engine

    E1_TORQ,      // Engine1 Torque ft/lb
    E2_TORQ,      // Engine2 Torque ft/lb
    E3_TORQ,      // Engine3 Torque ft/lb
    E4_TORQ,      // Engine4 Torque ft/lb

    E1_ITT,       // Engine1 ITT °C
    E2_ITT,       // Engine2 ITT °C
    E3_ITT,       // Engine3 ITT °C
    E4_ITT,       // Engine4 ITT °C

    E1_N1,        // Engine1 N1 %
    E2_N1,        // Engine2 N1 %
    E3_N1,        // Engine3 N1 %
    E4_N1,        // Engine4 N1 %

    E1_N1_ANI,   // Engine N1 Graph Single Engine
    E2_N1_ANI,   // Engine N1 Graph Twin Engine
    E3_N1_ANI,   // Engine N1 Graph 3rd Engine
    E4_N1_ANI,   // Engine N1 Graph 4th Engine

    E1_N2,        // Engine1 N2 %
    E2_N2,        // Engine2 N2 %
    E3_N2,        // Engine3 N2 %
    E4_N2,        // Engine4 N2 %

    E1_N2_ANI,   // Engine N2 Graph Single Engine
    E2_N2_ANI,   // Engine N2 Graph Twin Engine
    E3_N2_ANI,   // Engine N2 Graph 3rd Engine
    E4_N2_ANI,   // Engine N2 Graph 4th Engine

    E1_AFTB,      // Engine1 Afterburner %
    E2_AFTB,      // Engine2 Afterburner %
    E3_AFTB,      // Engine3 Afterburner %
    E4_AFTB,      // Engine4 Afterburner %

    E1_AFTB_ANI,   // Afterburner % Graph Single Engine
    E2_AFTB_ANI,   // Afterburner % Graph Twin Engine
    E3_AFTB_ANI,   // Afterburner % Graph 3rd Engine
    E4_AFTB_ANI,   // Afterburner % Graph 4th Engine

    E1_COWL_ANI, // Cowl Flaps Graph Single Engine
    E2_COWL_ANI, // Cowl Flaps Graph Twin Engine
    E3_COWL_ANI, // Cowl Flaps Graph 3rd Engine
    E4_COWL_ANI, // Cowl Flaps Graph 4th Engine

    // FUEL
    E1_FFlow_pph,   // Fuel1 Flow pph
    E2_FFlow_pph,   // Fuel2 Flow pph
    E3_FFlow_pph,   // Fuel3 Flow pph
    E4_FFlow_pph,   // Fuel4 Flow pph

    E1_FFlow_gph,   // Fuel1 Flow gph
    E2_FFlow_gph,   // Fuel2 Flow gph
    E3_FFlow_gph,   // Fuel3 Flow gph
    E4_FFlow_gph,   // Fuel4 Flow gph

    E1_FFlow_kgh,   // Fuel1 Flow kg
    E2_FFlow_kgh,   // Fuel2 Flow kg
    E3_FFlow_kgh,   // Fuel3 Flow kg
    E4_FFlow_kgh,   // Fuel4 Flow kg

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

    FUEL_L_kg,      // Fuel quantity Left Kilograms
    FUEL_R_kg,      // Fuel quantity Right Kilograms
    FUEL_C_kg,      // Fuel quantity Center Kilograms
    FUEL_TOT_kg,    // Fuel quantity Total Kilograms
    FUEL_REACH_kg,  // Fuel reach in seconds for the Kilograms Readout

    FUEL_ANI_LR,    // Fuel Graph Left Right
    FUEL_ANI_C,     // Fuel Graph Center

    // GPS
    GPS_PWYP,     // GPS Prev Waypoint
    GPS_NWYP,     // GPS Next Waypoint
    GPS_DST,      // GPS or Calc Distance to Destination
    GPS_ETE,      // GPS Time to Destination
    GPS_WP_DIST,  // GPS Distance to next Waypoint
    GPS_WP_ETE,   // GPS Time to next Waypoint
    GPS_TRK,      // GPS Track 000°
    GPS_GS,       // GPS Groundspeed 000kt
    GPS_BRGm,     // GPS Mag BRG to Waypoint 000°
    GPS_DTRK,     // GPS Desired Track to Waypoint 000°
    GPS_XTK,      // GPS CrossTrack Error nm
    GPS_ALT,      // GPS next Waypoint Altitude
    GPS_TOD,      // GPS distance to TOD 
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
    VARIO_MPS,    // Total Energy Variometer m/sec with +-
    VARIO_MPS_AVG, // Total Energy Variometer Average m/sec with +-
    VARIO_KTS,    // Total Energy Variometer kts with +-
    VARIO_KTS_AVG, // Total Energy Variometer Average kts with +-
    VARIO_ANI,    // Vario Graph
    VARIO_ANI_AVG, // Vario Graph AVG text
    AOA,          // Angle of attack deg
    FP_ANGLE,     // Flight Path Angle
    ATT_ANI,      // Attitude Visual
    FPA_ANI,      // FPA Visual
    GFORCE_Cur,   // GForce Current
    GFORCE_Min,   // GForce Minimum
    GFORCE_Max,   // GForce Maximum
    COMPASS,      // Mag Compass
    COMPASS_ANI,  // Mag Compass Arrow

    // AVIONICS NAV
    NAV1_ID,      // NAV1 ID
    NAV1_NAME,    // NAV1 Name {Apt}
    NAV1_DST,     // NAV1 DME Distance
    NAV1_BRG,     // NAV1 BRG to

    NAV1_SWAP,    // NAV1 Swap
    NAV1_STDBY,   // NAV1 Standby Frequency
    NAV1_ACTIVE,  // NAV1 Active Frequency

    NAV2_ID,      // NAV2 ID
    NAV2_NAME,    // NAV2 Name {Apt}
    NAV2_DST,     // NAV2 DME Distance
    NAV2_BRG,     // NAV2 BRG to

    NAV2_SWAP,    // NAV2 Swap
    NAV2_STDBY,   // NAV2 Standby Frequency
    NAV2_ACTIVE,  // NAV2 Active Frequency

    ADF1_ID,      // ADF1 ID
    ADF1_NAME,    // ADF1 Name
    ADF1_BRG,     // ADF1 Radial
    ADF1_ANI,     // ADF1 Needle

    ADF1_SWAP,    // ADF1 Swap
    ADF1_STDBY,   // ADF1 Standby Frequency
    ADF1_ACTIVE,  // ADF1 Active Frequency

    COM1_SWAP,    // COM1 Swap
    COM1_STDBY,   // COM1 Standby Frequency
    COM1_ACTIVE,  // COM1 Active Frequency
    COM1_TYPE,    // COM1 Type
    COM1_ID,      // COM1 ID

    COM2_SWAP,    // COM2 Swap
    COM2_STDBY,   // COM2 Standby Frequency
    COM2_ACTIVE,  // COM2 Active Frequency
    COM2_TYPE,    // COM2 Type
    COM2_ID,      // COM2 ID

    XPDR_CODE,    // Transponder Code
    XPDR_STAT,    // Transponder Status

    // AUTOPILOT
    AP,           // Autopilot On/Off
    AP_HDG,       // AP HDG active
    AP_HDGset,    // AP HDG set
    AP_HDGset_man,// AP HDG set managed slot
    AP_ALT,       // AP ALT hold active
    AP_ALThold,   // AP ALT Hold SLOT 1
    AP_ALTset,    // AP ALT set 
    AP_ALTset_man,// AP ALT set managed slot
    AP_VS,        // AP VS hold active
    AP_VSset,     // AP VS set
    AP_VSset_man, // AP VS set managed slot
    AP_FLC,       // AP FLC hold active
    AP_FLCset,    // AP FLC IAS set
    AP_FLCset_man,// AP FLC IAS set managed slot
    AP_SPD,       // AP SPD hold active
    AP_SPDset,    // AP SPD IAS set
    AP_SPDset_man,// AP SPD IAS set managed slot
    AP_NAV,       // AP NAV active
    AP_VNAV,      // AP VNAV on G1000
    AP_NAVgps,    // AP NAV follow GPS
    AP_BC,        // AP BC signal
    AP_APR,       // AP APR hold active
    AP_GS,        // AP GS  hold active
    AP_ATT,       // Attitude Hold
    AP_YD,        // Yaw Damper signal
    AP_LVL,       // Wing Leveler signal
    AP_APR_INFO,  // AP/GPS Approach Information
    AP_ATHR_armed,// Auto Throttle Arm Click
    AP_ATHR_toga, // Auto Throttle TOGA Click
    AP_ABRK_armed,// Auto Brake Arm Click
    AP_ASKID,     // Auto Skid On

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
    DEPARR_DEP,   // Departure Apt
    DEPARR_ARR,   // Arrival Apt

    // CONTROLs
    E1_THR_LEV,    // Throttle Lever Engine 1
    E2_THR_LEV,    // Throttle Lever Engine 2
    E1_MIX_LEV,    // Mixture Lever Engine 1
    E2_MIX_LEV,    // Mixture Lever Engine 2
    E1_PROP_LEV,   // Propeller Lever Engine 1
    E2_PROP_LEV,   // Propeller Lever Engine 2

    E1_A320THR,    // A320 Throttle
    E2_A320THR,    // A320 Throttle

    SURF_ANI,         // Control Surfaces

  }

}

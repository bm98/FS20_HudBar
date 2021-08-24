using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using CoordLib;

namespace FS20_HudBar.Bar.FltLib
{

  #region Examples

  /*

  ------------- Different Types from OneStore FLTs

  Such FLT Waypoints are ending with ... ALT,  Extendend in Sim with the rest of the WYP properties 
  We should never see the ones from the original FLT file - or at least they will not be decoded here...

  [Main]
  MissionType=LandingChallengeMaster

  [Main]
  MissionType=LiveEvent

  [Main]
  MissionType=FreeFlight

  [Main] 
  MissionType=Discovery
  MissionSubType=WU
  MissionSubTypeTT=@asobo-discovery,TT:MENU.DISCOVERY_WU
  OriginalFlight=Tokyo.FLT
  FlightType=NORMAL

  ------------- BUSHTRIP SWEDEN 
  [Main]
  MissionType=BushTrip
  AppVersion=10.0.282174
  FlightVersion=1

  [Options]
  Save=True
  SaveOriginalFlightPlan=True

  [ATC_Aircraft.0]
  ActiveFlightPlan=True
  RequestedFlightPlan=False
  AcState=ACSTATE_REQUEST_TAXI_CLEARANCE_OUT_VFR_ATIS
  Waypoint.0=, ESSA, , Arlanda, A, N59° 39' 00.53", E17° 56' 37.14", +001500.00,
  Waypoint.1=AA, POI1, , Friends Arena, U, N59° 22' 20.98", E18° 00' 00.00", +001500.00,
  Waypoint.2=AA, POI2, , Royal Swedish Opera, U, N59° 19' 46.99", E18° 04' 14.01", +001500.00,
  Waypoint.3=AA, POI3, , Stockholm Palace, U, N59° 19' 37.01", E18° 04' 18.00", +001500.00,
  Waypoint.4=AA, POI4, , Ericsson Globe, U, N59° 17' 36.80", E18° 04' 59.65", +001500.00,
  Waypoint.5=AA, POI5, , Tele2 Arena, U, N59° 17' 26.92", E18° 05' 07.22", +001500.00,
  Waypoint.6=AA, POI6, , Kyrksjon, U, N59° 00' 29.11", E17° 34' 20.14", +001500.00,
  Waypoint.7=AA, POI7, , Nykoping, U, N58° 45' 53.34", E17° 01' 21.99", +001500.00,
  Waypoint.8=, ESSP, , Kungsangen, A, N58° 35' 15.17", E16° 14' 43.72", +001500.00,
  Waypoint.9=AB, POI8, , Norrkoping, U, N58° 35' 33.64", E16° 11' 49.80", +001500.00,
  Waypoint.10=AB, POI9, , Norsholm, U, N58° 30' 23.47", E15° 58' 17.17", +001500.00,
    ... more
  Waypoint.29=AE, POI22, , Oresund Bridge, U, N55° 33' 55.75", E12° 53' 28.01", +001500.00,
  Waypoint.30=AE, POI23, , Eleda Stadion, U, N55° 35' 01.00", E12° 59' 16.00", +001500.00,
  Waypoint.31=, ESMS, , Sturup, A, N55° 31' 44.03", E13° 22' 16.02", +001500.00,

  NumberofWaypoints=32

  ------------- BUSHTRIP FRANCE 

  [Main]
  MissionType=BushTrip

  [Options]
  Save=True
  SaveOriginalFlightPlan=True

  [ATC_Aircraft.0]
  ActiveFlightPlan=True
  RequestedFlightPlan=False
  AcState=ACSTATE_REQUEST_TAXI_CLEARANCE_OUT_VFR_ATIS
  Waypoint.0=, LFCS, , TT:France.Mission.WP_01_00, A, N44° 41' 56.99", W0° 35' 50.00", +000192.00,
  Waypoint.1=AA, POI1, , TT:France.Mission.WP_01_01, U, N44° 41' 49.96", W0° 45' 44.00", +000000.00,
  Waypoint.2=AA, POI2, , TT:France.Mission.WP_01_02, U, N44° 37' 27.12", W0° 51' 12.76", +000000.00,
  Waypoint.3=AA, POI3, , TT:France.Mission.WP_01_03, U, N44° 38' 48.03", W1° 03' 21.98", +000000.00,
  Waypoint.4=AA, POI4, , TT:France.Mission.WP_01_04, U, N44° 39' 02.09", W1° 14' 57.39", +000000.00,
  Waypoint.5=AA, POI5, , TT:France.Mission.WP_01_05, U, N44° 35' 06.31", W1° 13' 05.35", +000000.00,
  Waypoint.6=, LFCH, , TT:France.Mission.WP_02_00, A, N44° 35' 48.99", W1° 06' 50.00", +000000.00,
  Waypoint.7=AB, POI6, , TT:France.Mission.WP_02_01, U, N44° 30' 41.18", W1° 11' 37.60", +000000.00,
  Waypoint.8=AB, POI7, , TT:France.Mission.WP_02_02, U, N44° 25' 40.42", W1° 10' 36.11", +000000.00,
  ... many more
  Waypoint.141=AQ, PI125, , TT:France.Mission.WP_17_11, U, N45° 53' 41.23", E6° 47' 43.23", +000000.00,
  Waypoint.142=AQ, PI126, , TT:France.Mission.WP_17_12, U, N45° 51' 42.48", E6° 37' 42.56", +000000.00,
  Waypoint.143=, LFHM, , TT:France.Mission.WP_18_00, A, N45° 49' 28.00", E6° 38' 54.00", +004830.00,

  NumberofWaypoints=144

  ------------- BUSHTRIP NEVADA ORIG

  [Main]
  MissionType=BushTrip

  [Options]
  Save=True
  SaveOriginalFlightPlan=True

  [ATC_Aircraft.0]
  ActiveFlightPlan=True
  RequestedFlightPlan=False
  AcState=ACSTATE_REQUEST_TAXI_CLEARANCE_OUT_VFR_ATIS
  Waypoint.0=, O64, , TT:Nevada.Mission.173, A, N35°21'43.31",W118°51'16.33",+699, 
  Waypoint.1=!A, POI1, , TT:Nevada.Mission.175, U, N35° 26' 0.50",W118° 48' 45.99",+000000.00, 
  Waypoint.2=!A, POI2, , TT:Nevada.Mission.176, U, N35° 32' 58.75",W118° 36' 56.57",+000000.00, 
  Waypoint.3=!A, POI3, , TT:Nevada.Mission.177, U, N35° 35' 16.60",W118° 31' 34.84",+000000.00, 
  Waypoint.4=!A, POI4, , TT:Nevada.Mission.178, U, N35° 39' 8.70",W118° 28' 10.03",+000000.00, 
  Waypoint.5=!A, POI5, , TT:Nevada.Mission.179, U, N35° 39' 52.85",W118° 21' 55.97",+000000.00, 
  Waypoint.6=!A, POI6, , TT:Nevada.Mission.180, U, N35° 41' 21.23",W118° 13' 22.58",+000000.00, 
  Waypoint.7=!A, POI7, , TT:Nevada.Mission.182, U, N35° 44' 37.41",W118° 5' 32.50",+000000.00, 
  Waypoint.8=!A, POI8, , TT:Nevada.Mission.183, U, N35° 36' 6.86",W117° 54' 13.46",+000000.00, 
  Waypoint.9=, KIYK, , TT:Nevada.Mission.184, A, N35° 39' 18.23",W117° 49' 42.32",+0.00, 
  Waypoint.10=!B, POI1, , TT:Nevada.Mission.185, U, N35° 56' 29.08",W117° 54' 25.44",+000000.00, 
  .... many more
  Waypoint.106=!Y, POI8, , TT:Nevada.Mission.307, U, N37° 36' 16.72",W119° 57' 57.91",+010500.00, 
  Waypoint.107=!Y, POI9, , TT:Nevada.Mission.308, U, N37° 31' 21.09",W119° 55' 14.66",+010500.00, 
  Waypoint.108=!Y, POI10, , TT:Nevada.Mission.309, U, N37° 29' 58.12",W119° 58' 23.84",+000000.00, 
  Waypoint.109=, KMPI, , TT:Nevada.Mission.310, A, N37° 29' 33.98",W119° 58' 20.57",+010500.00
  NumberofWaypoints=110

  ------------- BUSHTRIP NEVADA AUTOSAVE
  [Main]
  Title=NEVADA_SAVE
  MissionType=BushTrip
  OriginalFlight=missions\Asobo\BushTrips\nevada\Nevada.FLT
  FlightType=SAVE
  StartingCameraCategory=Custom

  [Options]
  Save=True
  SaveOriginalFlightPlan=True

  [ATC_Aircraft.0]
  ActiveFlightPlan=True
  RequestedFlightPlan=False
  AcState=ACSTATE_CTAF_TAKEOFF
  ActiveVFRAirport=O64
  Waypoint.0=, O64, , TT:Nevada.Mission.173, A, N35° 21.72', W118° 51.27', +000699.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.1=!A, POI1, , TT:Nevada.Mission.175, U, N35° 26.01', W118° 48.77', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.2=!A, POI2, , TT:Nevada.Mission.176, U, N35° 32.98', W118° 36.94', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.3=!A, POI3, , TT:Nevada.Mission.177, U, N35° 35.28', W118° 31.58', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.4=!A, POI4, , TT:Nevada.Mission.178, U, N35° 39.14', W118° 28.17', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.5=!A, POI5, , TT:Nevada.Mission.179, U, N35° 39.88', W118° 21.93', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.6=!A, POI6, , TT:Nevada.Mission.180, U, N35° 41.35', W118° 13.38', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.7=!A, POI7, , TT:Nevada.Mission.182, U, N35° 44.62', W118° 5.54', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.8=!A, POI8, , TT:Nevada.Mission.183, U, N35° 36.11', W117° 54.22', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.9=, KIYK, , TT:Nevada.Mission.184, A, N35° 39.30', W117° 49.71', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  NumberofWaypoints=10

  --------------------------------------

  EXAMPLES of FLT contents of [ATC_Aircraft.0] Flightplans
    Once the ATC confirms that FP will be assumed to be flown (ActiveFlightPlan=True)
  A similar one exists in [ATC_ActiveFlightPlan.0] however this one does not carry Approaches and other ATC induced WPs

  [ATC_Aircraft.0]
  ActiveFlightPlan=True

  Waypoint.0=, LSMD, , LSMD, A, N47° 23.92', E8° 38.89', +001448.50, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.1=, , , TIMECRUIS, U, N47° 25.41', E8° 36.39', +001900.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.2=, , , EnRt, U, N47° 24.61', E8° 36.62', +002237.67, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.3=LS, AMIKI, , AMIKI, T, N47° 34.43', E9° 2.25', +002237.67, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.4=LS, ZUE, , ZUE, T, N47° 35.53', E8° 49.06', +007000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.5=LS, ZH701, LSZH, ZH701, T, N47° 37.85', E8° 40.07', +006000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.6=LS, TRA, , TRA, T, N47° 41.37', E8° 26.22', +005000.00, , , , , , NONE, 0, 0, 210, 0, 0,  
  Waypoint.7=LS, ZH714, LSZH, ZH714, T, N47° 37.62', E8° 20.25', +004000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.8=, , , ClrApprch, T, N47° 34.78', E8° 24.15', +004000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.9=, , , Runway, T, N47° 28.99', E8° 32.08', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.10=, LSZH, , LSZH, A, N47° 27.48', E8° 32.88', +001400.00, , , , RNAV, 14, NONE, 0, 0, -1, 0, 0,  
  NumberofWaypoints=11


  Waypoint.0=, RJAH, , RJAH, A, N36° 10.90', E140° 24.88', +000107.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.1=, , , D0, U, N36° 12.48', E140° 25.66', +000450.00, , HOKT5E, , , 03, R, 0, 0, -1, 0, 0,  
  Waypoint.2=, , , D1, U, N36° 11.82', E140° 27.48', +000750.00, , HOKT5E, , , 03, R, 0, 0, -1, 0, 0,  
  Waypoint.3=, , , EnRt, U, N36° 13.24', E140° 26.83', +000889.36, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.4=, , , EnRt, U, N35° 50.71', E140° 38.63', +009313.18, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.5=RJ, ELGAR, RJAA, ELGAR, T, N35° 31.49', E140° 45.46', +004000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.6=RJ, HARPS, RJAA, HARPS, T, N35° 27.96', E140° 40.25', +004000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.7=RJ, GIINA, RJAA, GIINA, T, N35° 31.34', E140° 32.99', +004000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.8=RJ, VIRGO, RJAA, VIRGO, T, N35° 36.31', E140° 29.42', +003244.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.9=, , , ClrApprch, T, N35° 42.19', E140° 25.18', +001076.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.10=, , , Runway, T, N35° 47.12', E140° 23.55', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.11=, RJAA, , RJAA, A, N35° 45.92', E140° 23.13', +000150.00, , , , ILS, 34, L, 0, 0, -1, 0, 0,  
  NumberofWaypoints=12


  // PRE-FLIGHT IFR low Alt (Col manually formatted - original see above)
  Waypoint.0=   ,  LSMD,     ,      LSMD, A, N47° 23.92', E8° 38.89', +001448.50, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.1=   ,      ,     , TIMECRUIS, U, N47° 25.41', E8° 36.39', +001900.00, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.2=   ,      ,     ,      EnRt, U, N47° 24.61', E8° 36.62', +002237.67, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.3= LS, AMIKI,     ,     AMIKI, T, N47° 34.43', E9°  2.25', +002237.67, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.4= LS,   ZUE,     ,       ZUE, T, N47° 35.53', E8° 49.06', +007000.00, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.5= LS, ZH701, LSZH,     ZH701, T, N47° 37.85', E8° 40.07', +006000.00, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.6= LS,   TRA,     ,       TRA, T, N47° 41.37', E8° 26.22', +005000.00, , , ,     ,   , NONE, 0, 0,  210, 0, 0,  
  Waypoint.7= LS, ZH714, LSZH,     ZH714, T, N47° 37.62', E8° 20.25', +004000.00, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.8=   ,      ,     , ClrApprch, T, N47° 34.78', E8° 24.15', +004000.00, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.9=   ,      ,     ,    Runway, T, N47° 28.99', E8° 32.08', +000000.00, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.10=  ,  LSZH,     ,      LSZH, A, N47° 27.48', E8° 32.88', +001400.00, , , , RNAV, 14, NONE, 0, 0,   -1, 0, 0,  
  NumberofWaypoints=11

  //EDITED AND RE-CLEARED WITH ATC (RW16 via AMIKI tx)
  Waypoint.0=   ,  LSMD,     ,      LSMD, A, N47° 23.92', E8° 38.89', +001448.50, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.1=   ,      ,     ,      EnRt, U, N47° 28.19', E8° 39.43', +004096.33, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.2=   ,      ,     ,      EnRt, U, N47° 29.24', E8° 40.10', +005053.10, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.3= LS, AMIKI,     ,     AMIKI, T, N47° 34.43', E9°  2.25', +005053.10, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.4= LS,   ZUE,     ,       ZUE, T, N47° 35.53', E8° 49.06', +007000.00, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.5= LS,   KLO,     ,       KLO, T, N47° 27.43', E8° 32.73', +007000.00, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.6= LS, ZH712, LSZH,     ZH712, T, N47° 36.02', E8° 21.41', +005000.00, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.7= ED,  CI16, LSZH,      CI16, T, N47° 37.59', E8° 25.90', +004000.00, , , ,     ,   , NONE, 0, 0,  210, 0, 0,  
  Waypoint.8=   ,      ,     , ClrApprch, T, N47° 35.78', E8° 27.15', +004000.00, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.9=   ,      ,     ,    Runway, T, N47° 28.99', E8° 32.08', +000000.00, , , ,     ,   , NONE, 0, 0,   -1, 0, 0,  
  Waypoint.10=  ,  LSZH,     ,      LSZH, A, N47° 27.48', E8° 32.88', +001400.00, , , , RNAV, 14, NONE, 0, 0,   -1, 0, 0,  
  NumberofWaypoints=11

  // PRE-FLIGHT VFR
  Waypoint.0=   ,  LSMD,     ,      LSMD, A, N47° 23.92', E8° 38.89', +001448.50, , , ,     ,   , NONE, 0, 0,   -1, 0, 0, +
  Waypoint.1=   ,      ,     , TIMECLIMB, U, N47° 24.19', E8° 37.59', +002048.50, , , ,     ,   , NONE, 0, 0,   -1, 0, 0, +
  Waypoint.2=   ,      ,     , TIMECRUIS, U, N47° 25.94', E8° 35.09', +002400.00, , , ,     ,   , NONE, 0, 0,   -1, 0, 0, +
  Waypoint.3=   ,      ,     , TIMEDSCNT, U, N47° 26.24', E8° 34.67', +002400.00, , , ,     ,   , NONE, 0, 0,   -1, 0, 0, +
  Waypoint.4=   ,  LSZH,     ,      LSZH, A, N47° 27.48', E8° 32.88', +001400.00, , , ,     ,   , NONE, 0, 0,   -1, 0, 0, +
  NumberofWaypoints=5

  EDDM KLAX Planned as:  Dep AKIN1N ;Arr ANJLL4; App ILS 25R
  Waypoint.0=, EDDM, , EDDM, A, N48° 21.23', E11° 47.17', +001468.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.1=ED, DM040, EDDM, DM040, I, N48° 21.49', E11° 42.48', +002100.00, , AKIN1N, , , 26, R, 0, 0, -1, 0, 0,  
  Waypoint.2=, , , D1, U, N48° 21.50', E11° 42.48', +002100.00, , AKIN1N, , , 26, R, 0, 0, 220, 0, 0,  
  Waypoint.3=, , , D2, U, N48° 21.86', E11° 40.39', +002400.00, , AKIN1N, , , 26, R, 0, 0, 220, 0, 0,  
  Waypoint.4=ED, DM041, EDDM, DM041, I, N48° 23.07', E11° 39.20', +002700.00, , AKIN1N, , , 26, R, 0, 0, 220, 0, 0,  
  Waypoint.5=ED, DM043, EDDM, DM043, I, N48° 26.01', E11° 38.31', +003300.00, , AKIN1N, , , 26, R, 0, 0, 220, 0, 0,  
  Waypoint.6=, , , D5, U, N48° 26.01', E11° 38.30', +003300.00, , AKIN1N, , , 26, R, 0, 0, 250, 0, 0,  
  Waypoint.7=, , , D6, U, N48° 28.30', E11° 38.59', +003750.00, , AKIN1N, , , 26, R, 0, 0, 250, 0, 0,  
  Waypoint.8=ED, DM044, EDDM, DM044, I, N48° 30.12', E11° 40.69', +004200.00, , AKIN1N, , , 26, R, 0, 0, 250, 0, 0,  
  Waypoint.9=ED, EMGEP, EDDM, EMGEP, I, N48° 42.01', E12° 5.42', +008250.00, , AKIN1N, , , 26, R, 0, 0, 250, 0, 0,  
  Waypoint.10=ED, AKINI, , AKINI, I, N48° 44.98', E12° 7.45', +008900.00, , AKIN1N, , , 26, R, 0, 0, 250, 0, 0,  
  Waypoint.11=ED, GASKA, , GASKA, I, N50° 21.12', E10° 16.72', +032900.00, L604, , , , , NONE, 0, 0, -1, 24500, 0, +
  Waypoint.12=ED, ALIBU, , ALIBU, I, N50° 22.63', E10° 13.82', +033400.00, L604, , , , , NONE, 0, 0, -1, 24500, 0, +
  Waypoint.13=ED, BIBEG, , BIBEG, I, N50° 26.10', E10° 8.22', +034400.00, L604, , , , , NONE, 0, 0, -1, 24500, 0, +
  Waypoint.14=ED, DEMAB, , DEMAB, I, N50° 32.47', E9° 57.35', +036300.00, L604, , , , , NONE, 0, 0, -1, 6500, 0, +
  Waypoint.15=ED, OBISI, , OBISI, I, N50° 39.83', E9° 43.38', +038600.00, L604, , , , , NONE, 0, 0, -1, 6500, 0, +
  Waypoint.16=ED, MASEK, , MASEK, I, N50° 43.75', E9° 35.90', +039850.00, L604, , , , , NONE, 0, 0, -1, 24500, 0, +
  Waypoint.17=ED, EDEGA, , EDEGA, I, N51° 2.08', E9° 0.47', +040000.00, L604, , , , , NONE, 0, 0, -1, 24500, 0, +
  Waypoint.18=ED, MAPOX, , MAPOX, I, N51° 8.02', E8° 48.83', +040000.00, L604, , , , , NONE, 0, 0, -1, 24500, 0, +
  Waypoint.19=ED, BIGGE, , BIGGE, I, N51° 19.53', E8° 25.97', +040000.00, T281, , , , , NONE, 0, 0, -1, 24500, 0, +
  Waypoint.20=ED, HMM, , HMM, V, N51° 51.41', E7° 42.50', +040000.00, T281, , , , , NONE, 0, 0, -1, 19500, 0, +
  Waypoint.21=ED, AMSAN, , AMSAN, I, N52° 9.02', E7° 6.68', +040000.00, T281, , , , , NONE, 0, 0, -1, 19500, 0, +
  Waypoint.22=ED, NORKU, , NORKU, I, N52° 12.93', E6° 58.58', +040000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.23=EG, SOTUN, , SOTUN, I, N54° 10.60', E3° 37.62', +040000.00, M90, , , , , NONE, 0, 0, -1, 25000, 0, +
  Waypoint.24=EG, ROLUM, , ROLUM, I, N54° 18.33', E3° 18.52', +040000.00, M90, , , , , NONE, 0, 0, -1, 25000, 0, +
  Waypoint.25=EG, GIVEM, , GIVEM, I, N55° 27.90', E0° 14.88', +040000.00, M90, , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.26=EG, NEXUS, , NEXUS, I, N56° 12.72', W1° 50.68', +040000.00, UP59, , , , , NONE, 0, 0, -1, 25000, 0, +
  Waypoint.27=EG, RUGID, , RUGID, I, N57° 22.15', W4° 53.92', +040000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.28=EG, ODPEX, , ODPEX, I, N59° 24.59', W9° 30.00', +040000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.29=BI, 6115N, , 6115N, I, N61° 0.00', W15° 0.00', +040000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.30=BI, H6222, , H6222, I, N62° 30.00', W22° 0.00', +040000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.31=BI, H6332, , H6332, I, N63° 30.00', W32° 0.00', +040000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.32=BG, KAYAK, , KAYAK, I, N64° 13.50', W39° 59.73', +040000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.33=, , , TIMECRUIS, U, N64° 11.60', W47° 13.62', +040000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.34=CY, EMBOK, , EMBOK, I, N63° 28.00', W58° 0.00', +040000.00, N886A, , , , , NONE, 0, 0, -1, 29000, 0, +
  Waypoint.35=CY, IKMAN, , IKMAN, I, N62° 30.00', W63° 0.00', +040000.00, N886A, , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.36=CY, FEDDY, , FEDDY, I, N61° 42.00', W67° 0.00', +040000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.37=CY, KELMU, , KELMU, I, N59° 10.15', W80° 0.00', +040000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.38=, , , TIMEDSCNT, U, N58° 21.60', W82° 22.83', +040000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.39=CY, TANRO, CYER, TANRO, I, N56° 6.29', W87° 58.94', +040000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.40=CY, ETBEK, CPV7, ETBEK, I, N52° 17.32', W94° 21.91', +040000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.41=K3, CFMSZ, , CFMSZ, I, N48° 59.99', W100° 49.96', +040000.00, J562, , , , , NONE, 0, 0, -1, 18000, 0, +
  Waypoint.42=K3, DIK, , DIK, V, N46° 51.60', W102° 46.41', +040000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.43=K1, LAGPE, KCPR, LAGPE, I, N42° 40.46', W106° 45.61', +040000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.44=K2, CLOEE, KRIF, CLOEE, I, N38° 55.51', W111° 12.11', +040000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.45=K2, CGNEY, , CGNEY, I, N34° 45.83', W114° 22.64', +040000.00, , , ANJLL4, , 25, R, 0, 0, -1, 0, 0,  
  Waypoint.46=K2, SLLRS, , SLLRS, I, N34° 47.59', W115° 27.64', +031350.00, , , ANJLL4, , 25, R, 0, 0, -1, 0, 0,  
  Waypoint.47=K2, FLOJO, KLAX, FLOJO, I, N34° 38.01', W115° 53.60', +026650.00, , , ANJLL4, , 25, R, 0, 0, -1, 0, 0,  
  Waypoint.48=K2, SALYY, , SALYY, I, N34° 25.01', W116° 28.62', +020300.00, , , ANJLL4, , 25, R, 0, 0, -1, 0, 0,  
  Waypoint.49=K2, GLESN, , GLESN, I, N34° 20.08', W116° 40.58', +018100.00, , , ANJLL4, , 25, R, 0, 0, 280, 30000, 24000, B
  Waypoint.50=K2, ANJLL, , ANJLL, I, N34° 12.67', W116° 58.97', +014700.00, , , ANJLL4, , 25, R, 0, 0, 280, 24000, 19000, B
  Waypoint.51=K2, CAANN, , CAANN, I, N34° 9.95', W117° 9.03', +012950.00, , , ANJLL4, , 25, R, 0, 0, 280, 17000, 0, +
  Waypoint.52=K2, BOYEL, , BOYEL, I, N34° 6.32', W117° 22.21', +010650.00, , , ANJLL4, , 25, R, 0, 0, 280, 14000, 0, +
  Waypoint.53=K2, CRCUS, , CRCUS, I, N34° 4.39', W117° 29.91', +009300.00, , , ANJLL4, , 25, R, 0, 0, 270, 14000, 12000, B
  Waypoint.54=, KLAX, , KLAX, A, N33° 56.55', W118° 24.48', +000100.00, , , , ILS, 25, R, 0, 0, -1, 0, 0,  
  NumberofWaypoints=55


  Waypoint.0=, LSZH, , Zurich, A, N47° 27.48', E8° 32.88', +001398.17, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.1=LS, D147A, LSZH, D147A, I, N47° 26.58', E8° 33.51', +001800.00, , DEGE2R, , , 16, NONE, 0, 0, -1, 0, 0,  
  Waypoint.2=, , , D1, U, N47° 25.23', E8° 34.47', +002350.00, , DEGE2R, , , 16, NONE, 0, 0, 210, 2000, 0, +
  Waypoint.3=, , , D2, U, N47° 27.53', E8° 37.46', +003500.00, , DEGE2R, , , 16, NONE, 0, 0, 210, 0, 0,  
  Waypoint.4=LS, ZH502, LSZH, ZH502, I, N47° 27.91', E8° 45.98', +005700.00, , DEGE2R, , , 16, NONE, 0, 0, 210, 4000, 0, +
  Waypoint.5=LS, KOLUL, , KOLUL, I, N47° 28.03', E8° 49.37', +006550.00, , DEGE2R, , , 16, NONE, 0, 0, 210, 0, 0,  
  Waypoint.6=LS, ZH504, , ZH504, I, N47° 27.38', E8° 53.82', +007700.00, , DEGE2R, , , 16, NONE, 0, 0, 210, 5000, 0, +
  Waypoint.7=LS, ZH525, , ZH525, I, N47° 26.41', E9° 0.66', +009500.00, , DEGE2R, , , 16, NONE, 0, 0, 210, 7000, 0, +
  Waypoint.8=LS, DEGES, , DEGES, I, N47° 24.75', E9° 12.12', +012500.00, , DEGE2R, , , 16, NONE, 0, 0, 210, 8000, 0, +
  Waypoint.9=LS, DEGES, , DEGES, I, N47° 24.75', E9° 12.12', +012500.00, , , , , , NONE, 0, 0, -1, 7000, 0, +
  Waypoint.10=ED, NUNRI, , NUNRI, I, N47° 35.20', E9° 39.15', +020500.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.11=, , , TIMECRUIS, U, N47° 45.06', E10° 4.16', +028000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.12=ED, OBUBI, , OBUBI, I, N47° 51.15', E10° 19.80', +027000.00, , , , , , NONE, 0, 0, -1, 5500, 0, +
  Waypoint.13=, , , TIMEDSCNT, U, N47° 52.10', E10° 24.93', +028000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.14=ED, ATMAX, , ATMAX, I, N47° 55.78', E10° 45.00', +022750.00, , , , , , NONE, 0, 0, -1, 5500, 0, +
  Waypoint.15=ED, MERSI, , MERSI, I, N47° 58.93', E11° 2.56', +018100.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.16=, , , EnRt, U, N47° 58.38', E10° 59.74', +013017.31, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.17=ED, BETOS, , BETOS, T, N48° 4.08', E11° 21.00', +013017.31, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.18=ED, DM452, EDDM, DM452, T, N48° 14.30', E11° 33.31', +013017.31, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.19=ED, DM459, EDDM, DM459, T, N48° 18.15', E12° 24.55', +013017.31, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.20=, , , Apprch, T, N48° 18.82', E12° 31.98', +013017.31, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.21=, , , Apprch, T, N48° 23.46', E12° 24.47', +013017.31, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.22=, , , ClrApprch, T, N48° 21.91', E12° 4.54', +005000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.23=, , , Runway, T, N48° 20.69', E11° 48.28', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.24=, EDDM, , Munich, A, N48° 21.23', E11° 47.17', +001450.00, , , , RNAV, 26, L, 0, 0, -1, 0, 0,  
  NumberofWaypoints=25

  Started as:
  waypoint.0=, LSZH, , Zurich, A, N47° 27.48', E8° 32.88', +001398.17, , , , , , NONE, 0, 0, -1, 0, 0,  
  waypoint.1=LS, D147A, LSZH, D147A, I, N47° 26.58', E8° 33.51', +001800.00, , DEGE2R, , , 16, NONE, 0, 0, -1, 0, 0,  
  waypoint.2=, , , D1, U, N47° 25.23', E8° 34.47', +002350.00, , DEGE2R, , , 16, NONE, 0, 0, 210, 2000, 0, +
  waypoint.3=, , , D2, U, N47° 27.53', E8° 37.46', +003500.00, , DEGE2R, , , 16, NONE, 0, 0, 210, 0, 0,  
  waypoint.4=LS, ZH502, LSZH, ZH502, I, N47° 27.91', E8° 45.98', +005700.00, , DEGE2R, , , 16, NONE, 0, 0, 210, 4000, 0, +
  waypoint.5=LS, KOLUL, , KOLUL, I, N47° 28.03', E8° 49.37', +006550.00, , DEGE2R, , , 16, NONE, 0, 0, 210, 0, 0,  
  waypoint.6=LS, ZH504, , ZH504, I, N47° 27.38', E8° 53.82', +007700.00, , DEGE2R, , , 16, NONE, 0, 0, 210, 5000, 0, +
  waypoint.7=LS, ZH525, , ZH525, I, N47° 26.41', E9° 0.66', +009500.00, , DEGE2R, , , 16, NONE, 0, 0, 210, 7000, 0, +
  waypoint.8=LS, DEGES, , DEGES, I, N47° 24.75', E9° 12.12', +012500.00, , DEGE2R, , , 16, NONE, 0, 0, 210, 8000, 0, +
  waypoint.9=LS, DEGES, , DEGES, I, N47° 24.75', E9° 12.12', +012500.00, , , , , , NONE, 0, 0, -1, 7000, 0, +
  waypoint.10=ED, NUNRI, , NUNRI, I, N47° 35.20', E9° 39.15', +020500.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  waypoint.11=, , , TIMECRUIS, U, N47° 45.06', E10° 4.16', +028000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  waypoint.12=ED, OBUBI, , OBUBI, I, N47° 51.15', E10° 19.80', +027000.00, , , , , , NONE, 0, 0, -1, 5500, 0, +
  waypoint.13=, , , TIMEDSCNT, U, N47° 52.10', E10° 24.93', +028000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  waypoint.14=ED, ATMAX, , ATMAX, I, N47° 55.78', E10° 45.00', +022750.00, , , , , , NONE, 0, 0, -1, 5500, 0, +
  waypoint.15=ED, MERSI, , MERSI, I, N47° 58.93', E11° 2.56', +018100.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  waypoint.16=ED, BETOS, , BETOS, I, N48° 4.08', E11° 21.00', +013000.00, , , BETO1B, , 26, R, 0, 0, 280, 13000, 0, -
  waypoint.17=ED, OTT, , OTT, V, N48° 10.82', E11° 48.99', +005450.00, , , BETO1B, , 26, R, 0, 0, 280, 0, 0,  
  waypoint.18=, EDDM, , Munich, A, N48° 21.23', E11° 47.17', +001450.00, , , , RNAV, 26, L, 0, 0, -1, 0, 0,  

  NEVADA MISSION
  Waypoint.0=, O64, , TT:Nevada.Mission.173, A, N35° 21.72', W118° 51.27', +000699.00, , , , , , NONE, 0, 0, -1, 0, 0, +
  Waypoint.1=!A, POI1, , TT:Nevada.Mission.175, U, N35° 26.01', W118° 48.77', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0, +
  Waypoint.2=!A, POI2, , TT:Nevada.Mission.176, U, N35° 32.98', W118° 36.94', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0, +
  Waypoint.3=!A, POI3, , TT:Nevada.Mission.177, U, N35° 35.28', W118° 31.58', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0, +
  Waypoint.4=!A, POI4, , TT:Nevada.Mission.178, U, N35° 39.14', W118° 28.17', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0, +
  Waypoint.5=!A, POI5, , TT:Nevada.Mission.179, U, N35° 39.88', W118° 21.93', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0, +
  Waypoint.6=!A, POI6, , TT:Nevada.Mission.180, U, N35° 41.35', W118° 13.38', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0, +
  Waypoint.7=!A, POI7, , TT:Nevada.Mission.182, U, N35° 44.62', W118° 5.54', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0, +
  Waypoint.8=!A, POI8, , TT:Nevada.Mission.183, U, N35° 36.11', W117° 54.22', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0, +
  Waypoint.9=, KIYK, , TT:Nevada.Mission.184, A, N35° 39.30', W117° 49.71', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0, +
  NumberofWaypoints=10


  ----- AI PILOT - cannot match the FP - so RequestedFlightPlan=True (i.e. not cleared by ATC)
  [ATC_Aircraft.0]
  ActiveFlightPlan=True
  RequestedFlightPlan=True
  AcState=ACSTATE_REQUEST_TAXI_CLEARANCE_OUT_VFR_ATIS
  Waypoint.0=, FASP, , FASP, A, S33° 45.55', E18° 32.89', +000225.81, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.1=, , , TIMECLIMB, U, S33° 42.73', E18° 32.67', +000832.94, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.2=, , , TIMECRUIS, U, S33° 48.48', E18° 33.99', +001400.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.3=, , , TIMEDSCNT, U, S33° 49.39', E18° 34.20', +001400.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.4=, , , TIMEAPPROACH, U, S34° 2.15', E18° 37.45', +001101.56, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.5=, FACT, , FACT, A, S33° 58.28', E18° 36.26', +000150.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  NumberofWaypoints=6

  [ATC_Aircraft.0]
  ActiveFlightPlan=True
  RequestedFlightPlan=False
  AcState=ACSTATE_ARRIVAL
  AgentTracking=2,6,18444288,CAPE TOWN
  ActiveVFRAirport=FASH
  Waypoint.0=, FASH, , Stellenbosch Airport, A, S33° 58.83', E18° 49.37', +000321.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.1=, , , TIMECRUIS, U, S33° 58.59', E18° 43.48', +005000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.2=, , , EnRt, U, S33° 59.81', E18° 45.77', +003832.81, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.3=FA, CB, , CB, T, S33° 52.57', E18° 34.40', +004000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.4=FA, CB, , CB, T, S33° 52.57', E18° 34.40', +004000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.5=, , , Apprch, T, S33° 48.99', E18° 33.20', +003000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.6=, , , Apprch, T, S33° 44.99', E18° 35.92', +003000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.7=, , , Apprch, T, S33° 43.43', E18° 35.40', +003000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.8=, , , Apprch, T, S33° 43.86', E18° 33.52', +003000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.9=, , , Apprch, T, S33° 45.79', E18° 32.21', +003000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.10=FA, CI19, FACT, CI19, T, S33° 48.66', E18° 33.17', +003000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.11=, , , ClrApprch, T, S33° 52.54', E18° 34.40', +003000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.12=, , , Runway, T, S33° 57.59', E18° 36.00', +000000.00, , , , , , NONE, 0, 0, -1, 0, 0,  
  Waypoint.13=, FACT, , Cape Town Intl, A, S33° 58.28', E18° 36.26', +000150.00, , , , ILS, 19, NONE, Y, 0, -1, 0, 0,  
  NumberofWaypoints=14


  WaypointNext=3  (index is 0 based)

  ********************
  ****** WP FIELDS
  ****** (deducted) Parts of old: http://www.prepar3d.com/SDK/Mission%20Creation%20Kit/FlightFiles.htm
  ********************
  00: RegionCode (ICAO)  OR other 2 letters  '!A' / AA, AB .. for Missions
  01: Ident (ICAO)
  02: Airport (ICAO) if the WP belongs to an Airport and it's Approaches
  03: NameID (ICAO or User or MSFS provided) can be as long as TT:Nevada.Mission.180 (ref to a translated text)
  04: Type (one of: A : airport, I : intersection, V : VOR, N : NDB, U : user, T : ATC)
  05: Lat S33° 45.79' (non unicode chars ° ' )
  06: Lon E18° 33.52' (non unicode chars ° ' )
  07: Altitude  +003000.00  likely -000100.00 ??
  08: Airway ID	 (default = empty) The airway ID, such as J5
  09: Departure ID  (default = empty) ID
  10: Arrival ID (default = empty) ID
  11: Approach Kind (default = empty) RNAV, ILS, VOR ..
  13: Rwy (default = empty) 14, 34 ..
  14: Rwy Designation (default = NONE) R, L, C, ..
  15: ApproachVariant (default = 0) (ILS RNAV Variant e.g. X, Y, Z etc.)
  16: Unk6 (default = 0) STILL NOT SEEN OR DECODED
  17: MaxSpeed (default = -1) Max Speed kt or -1 if none
  18: AltLimit1 (default = 0) an Altitude Limit or the lower Altitude Limit if B
  19: AltLimit2 (default = 0) an Altitude Limit or the higher Altitude Limit if B
  20: Alt Limit (blank, +, B, -) blank=NoLimit; +=Above AltLimit1; -=Below AltLimit1; B=Between AltLimit1 and AltLimit2

*/

  #endregion


  /// <summary>
  /// The Type of the Waypoint
  /// </summary>
  public enum WypType
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    Airport=0,
    Intersec,
    VOR,
    NDB,
    User,
    ATC,
    Other,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }

  /// <summary>
  /// Altitude Limits for a Waypoint
  /// </summary>
  public enum AltLimitType
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    NoLimit=0,
    Above,
    Below,
    Between,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }

  /// <summary>
  /// A Waypoint List
  /// </summary>
  public class Waypoints : List<Waypoint>
  {

  }


  /// <summary>
  /// A Waypoint of a Flightplan
  /// </summary>
  public class Waypoint
  {
    /// <summary>
    /// The Waypoint name, can be an official one or an MSFS internal one
    /// </summary>
    public string Ident { get; internal set; } = ""; // using Ident here
    /// <summary>
    /// The Waypoint name, can be an official one or an MSFS internal one
    /// </summary>
    public string Name { get; private set; } = ""; // using NameID here
    /// <summary>
    /// The Waypoint name (nax 8 chars), can be an official one or an MSFS internal one
    /// </summary>
    public string Name7 =>
      Name.Length > 7 ? Name.Substring( 0, 7 ) : Name;

    /// <summary>
    /// The ICAO Region (where given, else it is an empty string)
    /// </summary>
    public string Region { get; private set; } = "";

    /// <summary>
    /// The Latitude and Longitude of the Waypoint (null if not given or decoded)
    /// </summary>
    public LatLon LatLon { get; private set; } = null;
    /// <summary>
    /// The Altitude of the Waypoint (NaN if not given or decoded)
    /// </summary>
    public double Altitude { get; internal set; } = double.NaN;
    /// <summary>
    /// The Type of the Waypoint 
    /// </summary>
    public WypType WaypointType { get; private set; } = WypType.Other;

    /// <summary>
    /// The leg distance to the next WP
    /// (sum of leg distances)
    /// </summary>
    public float LegDist_nm { get; internal set; } = float.NaN;
    /// <summary>
    /// The remaining distance to the destination
    /// (sum of leg distances)
    /// </summary>
    public float RemainingDist_nm { get; internal set; } = float.NaN;
    /// <summary>
    /// The given Airway ID
    /// </summary>
    public string Airway { get; internal set; } = "";

    /// <summary>
    /// The given Departure ID
    /// </summary>
    public string Departure { get; internal set; } = "";

    /// <summary>
    /// The given ArrivalID
    /// </summary>
    public string Arrival { get; internal set; } = "";

    /// <summary>
    /// The given Approach
    /// </summary>
    public string Approach { get; internal set; } = "";

    /// <summary>
    /// The given Runway
    /// </summary>
    public string Runway { get; internal set; } = "";

    /// <summary>
    /// A Speed Limit if given (else it is -1)
    /// </summary>
    public int SpeedLimit_kt { get; internal set; } = -1;

    /// <summary>
    /// The Altitude Limit type
    /// </summary>
    public AltLimitType AltLimit { get; internal set; } = AltLimitType.NoLimit;
    /// <summary>
    /// A first Alt Limit for +;- or B Top
    /// </summary>
    public int AltLimit1_ft { get; internal set; } = 0;
    /// <summary>
    /// A second Alt Limit for B Bottom
    /// </summary>
    public int AltLimit2_ft { get; internal set; } = 0;


    /// <summary>
    /// Returns a line of Wyp Information
    /// </summary>
    public string Pretty {
      get {
        if ( string.IsNullOrEmpty( Ident ) )
          return $"Waypoint not found";

        string typ   = (
          string.IsNullOrEmpty( Departure ) ?
          string.IsNullOrEmpty( Arrival ) ?
          string.IsNullOrEmpty( Approach ) ?
          string.IsNullOrEmpty( Airway ) ?
          WaypointType.ToString( ):Airway : Approach : Arrival : Departure).PadRight(8);

        string limits =
            ( AltLimit == AltLimitType.Above   ? $"̲ {AltLimit1_ft:##,##0} " : $""  )
          + ( AltLimit == AltLimitType.Below   ? $"̅ {AltLimit1_ft:##,##0} " : $""  )
          + ( AltLimit == AltLimitType.Between ? $"∑{AltLimit2_ft:##,##0} · {AltLimit1_ft:##,##0} " : "" )
          + ( SpeedLimit_kt>0      ? $"Max. {SpeedLimit_kt:##0} kt" : "" );

        return $"{Ident.PadRight( 7 ),-8}\t({typ,-10}) {LegDist_nm,7:#,##0.0} - {RemainingDist_nm,7:#,##0.0} nm @ {Altitude,6:##,##0} ft   {limits}"; 
      }
    }

    /// <summary>
    /// Returns a block of detailed Wyp Information
    /// </summary>
    public string PrettyDetailed {
      get {
        if ( string.IsNullOrEmpty( Ident ) )
          return $"Waypoint not found";

        string typ   = (
          string.IsNullOrEmpty( Departure ) ?
          string.IsNullOrEmpty( Arrival ) ?
          string.IsNullOrEmpty( Approach ) ?
          string.IsNullOrEmpty( Airway ) ?
          WaypointType.ToString( ) : Airway : Approach : Arrival : Departure);

        string limits = ( SpeedLimit_kt>0      ? $"Speed Limit   : {SpeedLimit_kt} kt\n" : "" )
          + ( AltLimit == AltLimitType.Above   ? $"Altitude Limit: {AltLimit1_ft:##,##0} ft or above\n" : "" )
          + ( AltLimit == AltLimitType.Below   ? $"Altitude Limit: {AltLimit1_ft:##,##0} ft or below\n" : "" )
          + ( AltLimit == AltLimitType.Between ? $"Altitude Limit: between {AltLimit2_ft:##,##0} ft and {AltLimit1_ft:##,##0} ft\n" : "" )
          ;
        return $"Waypoint: {Ident} ({typ}) @ {Altitude:##,##0} ft\n"
          + $"{limits}Leg Dist: {LegDist_nm:##,##0.0} nm\n"
          + $"Remaining: {RemainingDist_nm:##,##0.0} nm";
      }
    }

    /// <summary>
    /// String of this Waypoint (the Ident)
    /// </summary>
    public override string ToString( )
    {
      return $"{Ident,-8}{Name}";
    }

    //, FACT, , Cape Town Intl, A, S33� 58.28', E18� 36.26', +000150.00, , , , ILS, 19, NONE, Y, 0, -1, 0, 0,
    // Decoding Part

    // See examples for the format
    private readonly static Regex c_wpRE =
      new Regex(@"^(?<reg>(\!\w|\w{2}))?,\s*(?<ident>\w{2,5})?,\s*(?<apt>\w{3,4})?,\s*(?<name>(\w|\s|:|\.)*),\s*(?<typ>[A-Z]),\s*(?<lat>(N|S)\d{1,2}.\s\d{1,2}\.\d{2}'),\s*(?<lon>(E|W)\d{1,3}.\s\d{1,2}\.\d{2}'),\s*(?<alt>(\+|-)\d{6}\.\d{2}),\s*(?<awy>\w*)?,\s*(?<depId>\w*)?,\s*(?<arrId>\w*)?,\s*(?<aprT>\w*)?,\s*(?<rwy>\w*)?,\s*(?<rwyD>\w*)?,\s*(?<aprV>-?\w*),\s*(?<u6>-?\w*),\s*(?<speedL>-?\d*),\s*(?<altL1>-?\d*),\s*(?<altL2>-?\d*),\s*(?<altLT>.*)?");

    // Waypoints to ignore while decoding
    private readonly static List<string> c_ignoreWP = new List<string>(){
      //"TIMECLIMB", "TIMECRUIS", "TIMEDSCNT", "TIMEAPPROACH",
      "Apprch", "ClrApprch", "EnRt",
    };

    /*
     There are MSFS:
        (T) Apprch 
        (T) ClrApprch
        (T) Runway => RWYNNx e.g. RW26L
        (U) EnRt
     */


    /// <summary>
    /// Decode one Waypoint Line to a Waypoint Object
    /// </summary>
    /// <param name="wpLine">A Waypoint Line</param>
    /// <returns>A new Waypoint or null on error/ignore</returns>
    internal static Waypoint Decode( string wpLine )
    {
      var ret = new Waypoint();
      Match match = c_wpRE.Match(wpLine);
      if ( match.Success ) {
        ret.Region = match.Groups["reg"].Success ? match.Groups["reg"].Value : "";
        ret.Ident = match.Groups["ident"].Value;
        ret.Name = match.Groups["name"].Value;
        // exclude MSFS ones that are not used in the GPS (afaik)
        if ( c_ignoreWP.Contains( ret.Name ) ) return null; // we don't collect this one

        if ( string.IsNullOrEmpty( ret.Ident ) ) ret.Ident = ret.Name7; // fix missing Idents with the Name (cut)
        ret.Ident = ret.Ident.Trim( ).ToUpperInvariant( ); // case issues ..

        // waypoint type
        string t = match.Groups["typ"].Value;
        switch ( t ) {
          case "A": ret.WaypointType = WypType.Airport; break;
          case "I": ret.WaypointType = WypType.Intersec; break;
          case "V": ret.WaypointType = WypType.VOR; break;
          case "N": ret.WaypointType = WypType.NDB; break;
          case "U": ret.WaypointType = WypType.User; break;
          case "T": ret.WaypointType = WypType.ATC; break;
          default: ret.WaypointType = WypType.Other; break;
        }
        ret.LatLon = new LatLon( Tooling.ToLat( match.Groups["lat"].Value ), Tooling.ToLon( match.Groups["lon"].Value ) );
        ret.Altitude = Tooling.ToAlt( match.Groups["alt"].Value );
        ret.Airway = match.Groups["awy"].Value;
        // Eval the Runway ID as NNx (complete later to RWNNx)
        ret.Runway = $"{match.Groups["rwy"].Value}";
        ret.Runway += ( match.Groups["rwyD"].Value != "NONE" ) ? $"{match.Groups["rwyD"].Value}" : ""; // seems to be NONE if not used...

        ret.Departure = match.Groups["depId"].Value; // and ID
        if ( !string.IsNullOrEmpty( ret.Departure ) )
          ret.Departure += $"-{ret.Runway}";
        ret.Arrival = match.Groups["arrId"].Value; // and ID
        if ( !string.IsNullOrEmpty( ret.Arrival ) )
          ret.Arrival += $"-{ret.Runway}";
        ret.Approach = match.Groups["aprT"].Value; // RNAV, ILS ..
        ret.Approach += ( match.Groups["aprV"].Value != "0" ) ? $" {match.Groups["aprV"].Value}" : "";  // ILS Z,  RNAV X ..
        if ( ret.WaypointType == WypType.Airport && !string.IsNullOrEmpty( ret.Approach ) )
          ret.Approach += $" {ret.Runway}"; // complete to e.g. ILS 25R,  ILS Z 19, RNAV X 23C

        if ( !string.IsNullOrEmpty( ret.Runway ) )
          ret.Runway = "RW" + ret.Runway; // complete to e.g. 25R -> RW25R to match GPS Waypoint name

        // Limits
        ret.SpeedLimit_kt = int.Parse( match.Groups["speedL"].Value ); // default -1
        ret.AltLimit1_ft = int.Parse( match.Groups["altL1"].Value );   // default 0
        ret.AltLimit2_ft = int.Parse( match.Groups["altL2"].Value );   // default 0
        // Altitude Limit type
        t = match.Groups["altLT"].Value;
        switch ( t ) {
          case "+": ret.AltLimit = AltLimitType.Above; break;
          case "-": ret.AltLimit = AltLimitType.Below; break;
          case "B": ret.AltLimit = AltLimitType.Between; break;
          default: ret.AltLimit = AltLimitType.NoLimit; break;
        }

        return ret;
      }
      // ERROR cannot decode
      Console.WriteLine( $"FLT-WYP Decoder - cannot decode:\n{wpLine}" );
      return null; 
    }

  }

}

FS20_HudBar V 0.37 - Build 31 BETA
(c) M. Burri - 14-Nov-2021

Contains files:

FS20_HudBar.exe               The program

- All libraries below MUST be in the same folder as the Exe file
FSimClientIF.dll            Generic FSim Client interface definition (updated)
FSimIF.dll                  Generic FSim interface definition (updated)
SimConnectClient.dll        FlightSim interface to MSFS2020 SimConnect (updated)
MetarLib.dll				A METAR access library (new V0.29)
CoordLib.dll				A coord. handling library (new V0.29)
FS20_AptLib.dll             MSFS2020 Airport DB (new V0.29)
SpeechLib.dll               A voice synth lib using Win10 TTS facilities (new V0.35)

From MSFS2020 Developer Kit for convenience included:
  SimConnect.cfg
  Microsoft.FlightSimulator.SimConnect.dll 
  SimConnect.dll

From Google Fonts Library embedded:
  Share_Tech_Mono			(Used as condensed font)

ReadMe.txt                   This file

MSFS Hud Bar (.Net 4.7.2)

Put all files into one folder and hit FS20_HudBar.exe to run it

For Updates and information visit:

https://github.com/bm98/FS20_HudBar


Scanned for viruses before packing... 
github@mail.burri-web.org

Changelog:
V 0.37-B31
- Add item Control Handle% readout for Throttle, Mix, Propeller
- Add item Power LOAD %
- Add item CHT in °C and °F
- Add item EGT in °F
- Add item OAT in °F
- Add Button Click for SimRate to set it to 1x (normal)
- Add VoiceOut item for LowFuel Alert
- Add Config Copy / Paste items between profiles
- Add Default profiles to select from
- Add Provide some default profiles when using the program the very first time
- Add Load default profiles from Config menu
- Update Button Click handling for CP Meters to Stop (within 2 sec of Start)
- Update GPS source to show NAV1 or NAV2 or GPS
- Update Cursor on ATC Tooltip changes to indicate content
- Update Internal re-design for most of the parts..
- Fix Interaction issue with SimConnect and loading flights (workaround MSFS bug)
- Update QuickGuide

V 0.36-B30
- Add NAV1/2 with ID, Bearing To, and DME distance item
- Add Fuel Time value to Fuel Total (based on the most current consumption)
- Add VoicePack for various Readouts other than RAv (selectable - default OFF)
- Add 'Voice disabled' as option for the Voice selection
- Add new Display Kind - 'Window no border'  looks and behaves like a freely movable Tile
- Add optional command line string to create multiple Configuration instances (any string will do)
- Fix RAv readout should capture steps better now
- Update QuickGuide

V 0.35-B29
- Add Appearance selection for the value items (3 levels Bright, Dimmed, Dark)
- Update to own SpeechLib allowing to use all Win10 installed voices
- Update QuickGuide

V 0.34-B28
- Add Audible RA item (RAv) - announces 400,300,200,100,50,40,30,20,10 ft RA
- Update Transparency setting, Opaque to 90% independent per profile
- Fix some more inconsistencies with color handling
- Update QuickGuide

V 0.33-B27
- Update ATC Airport handling in case no ATC flightplan is given
- Update step VS items in 20 fpm steps (G1000 would be 50)
- Update FuelQuantities get Amber readings if unbalanced more than 15% of total Capacity
- Fix some more inconsistencies
- Update QuickGuide

V 0.32-B26
- Add VS Item with +- signs
- Update Flight Plan handling (still the Sim ATC acts sometimes not as expected..)
- Update Alignment of value columns in vertical oriented bars
- Update some colors for easier reading
- Update SimConnect procedure - should connect in any situation now
- Fix some bugs encountered on the way
- Update QuickGuide

V 0.31-B25
- Add Config Switch to enable FLT handling/save (default off)
- Add ATC assigned/expected ALT, HDG and next WYP item
- Add Complete flightplan as tooltip on ATC item (the ATC assumed FP)
- Add FLT AutoSave if FLT handling is enabled (5Min interval, 12 saves max)
- Update Add FltLib as part of this project
- Update Complete rework of FLT File handling, should no longer interfere with SIM AutoSave in missions
- Fix Issues re. FLT file decoding causing exceptions due to unexpected states/contents
- Update QuickGuide

V 0.30-B24
- Update flight plan formatting
- Update QuickGuide

V 0.29-B23
- Add Instrument Altitude (ALT) item
- Update Effective Altitude (ALTeff) - was ALT before
- Add Lat Lon aircraft position item
- Add METAR action item for the current aircraft location (nearest station)
- Add Tooltip on METAR (displays the received message)
- Add Metar for the destination Apt (APT action item)
- Add Tooltip on APT (displays the received message)
- Add Collect actual Flightplan Data from FLT file every 30 sec
- Add Tooltip on Waypoints (Detailed WYP from FLT file)
- Add Tooltip on -GPS- (Remaining Flightplan)
- Add Remaining Distance on APT (nm)

V 0.28-B22
- Add Time Enroute (WYP, Total) items
- Add Destination ETE items
- Add Flightplan destination APT ICAO code item
- Add ATC assigned Runway deviations Dist, Track, Alt items
- Update RA shows at or below 1500 ft now
- Fix Item Sequence for 2nd profile was not loaded correctly

V 0.27-B21
- Add Window Kind - behaves like a Tile but has a Border and can be moved anywhere
- Add Time of Day item (Sim Time)
- Add Visibility item
- Add True HDG item
- Add Autopilot BC action item
- Add Autopilot YD action item
- Add Autopilot LVL action item
- Update Condensed font - using Share Tech Mono (smallest mono font found so far)
- Refacture Font Management
- Update QuickGuide

V 0.26-B20
- fix Display of Lights

V 0.25-B19
- Add Wind Dir/Speed + Cross/Head;Tail items
- Add Lights as B N S T L status item
- Add Fuel quantities Left/Right item (Gal only)
- Add Fuel quantity Total item (Gal only)
- Add MACH number item
- Add Condensed Font as per profile option
- Update Add XTK direction indicator
- Update Vertical alignment of horizontal bar items
- Update QuickGuide

V 0.24-B18
- update Scroll Config items while dragging up or down
- fix First time run does not show bar

V 0.23-B17
- Add mag BRG to waypoint item
- Add DTK desired track to waypoint item
- Add XTK Cross track distance item
- Add AoA Angle of attack item
- Add TAS True airspeed item
- Add Acft ID item
- Add even larger Fonts +12, +14
- Update Align Left and right value columns to the max size label
- Update New items are default OFF (was ON before)
- Update QuickGuide

V 0.22-B16
- Add 3 independent Chekpoints to track elapsed time and distance from Startpoint (Lat/Lon based)
- Add MAN Pressure inHg item
- Add Set Trim to 0% Action for all 3 Trims
- Update Create a separate Auto E-Trim item - showing the %Value (same as the regular E-Trim does)
- Update QuickGuide
- Fix Get the Pointer Cursor back when hovering the item fiels (where Tile movement is not available)

V 0.21-B14
- Add Movable Tile Mode (replaces Splitter)
- Add Re-arrange item sequence
- Add Manual Line Breaks
- Update Selected profile marking in Configuration
- Update Dampen and Round Estimates to 100 increments
- Update QuickGuide

V 0.20-B13
- Add Splitter to shift left/top start of bar
- Add FontSize and Placement are now profile specific
- Add Fuel Flow field for Gal per hour
- Add Bar auto scales from 1 to more rows/columns to show all selected items
- Update more Font Sizes to choose from
- Update context menu shows currently selected profile
- Fix Save Selected Profile on Exit

V 0.12-B8
- Added Left, right, top alignment
- Added Font selection Regular, Larger, Largest
- Update detects 1/2 engines on its own
- Update some refactoring due to alignment update

V 0.11-B7
- Added SimRate, Estimates
- Fix BARO mismatch

V 0.10-B6
- Added OAT, BARO fields

V 0.9-B5
- initial upload



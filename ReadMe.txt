FS20_HudBar V 0.55 - Build 55 BETA
(c) M. Burri - 10-Oct-2022

Contains files:

FS20_HudBar.exe             The program

.\DemoBag                   Contains some images to showcase the Flight Bag (new since V0.43)

- All libraries below MUST be in the same folder as the Exe file
bm98_Album.dll              A UserControl for displaying images (zoom & drag new since V0.43)
bm98_Checklist.dll          A UserControl for displaying the Checklist Box (new since V0.53)
bm98_hb_Controls.dll        UserControls for displaying graphs (new since V0.45)
SpeechLib.dll               A voice synth lib using Win10 TTS facilities (new since V0.35)
PingLib.dll                 An audio lib using Win10 Audio facilities (new since V0.43)
MetarLib.dll                A METAR access library (new since V0.29)
CoordLib.dll                A coord. handling library
SettingsLib.dll             Application settings persistence library
FSimIF.dll                  Generic FSim interface definition (updated)
FSimClientIF.dll            Generic FSim Client interface definition (updated)
SimConnectClient.dll        FlightSim interface to MSFS2020 SimConnect (updated)
FSimFacilityIF.dll          MS facility database interface definition
FSimFacilityDataLib.dll     MS facility database access library
BM98CH_WasmClient.dll       WASM Module client to get LVars (new since V0.51)

3rd Party:
BingMapsRESTToolkit.dll     Microsoft provided library for accessing Bing Map data
LiteDB.dll                  3rd party data management library

From MSFS2020 Developer Kit for convenience included:
  SimConnect.cfg.OFF        Config file used only when connecting via network to MSFS (edit server IP)
  Microsoft.FlightSimulator.SimConnect.dll 
  SimConnect.dll

From Google Fonts Library embedded:
  Share_Tech_Mono			(Used as condensed font)

dataLoader\ folder:
FacilityDataLoader.exe      MSFS Facility conversion tool
BGLlib.dll                  BGL and LLM decoder
CoordLib.dll                A coord. handling library
FSimFacilityIF.dll          MS facility database interface definition
FSimFacilityDataLib.dll     MS facility database access library
LiteDB.dll                  3rd party data management library
System.Data.SQLite.dll      3rd party data management library
x64\SQLite.Interop.dll      3rd party data management library 64bit C-library
x86\SQLite.Interop.dll      3rd party data management library 32bit C-library

ReadMe.txt                   This file

MSFS Hud Bar (.Net 4.7.2)

Extract the Zip File into a folder and hit FS20_HudBar.exe to run it

For Updates and information visit:

https://github.com/bm98/FS20_HudBar


Scanned for viruses before packing... 
github@mail.burri-web.org

Changelog:

V 0.55-B55
- Add VNAV Button (when using WASM module)
- Add Move Bar/Tile to next screen (Monitor) with RShift+RCtrl+Break
- Add Using the Facility DB for Aiport Management (replacing the FS20_AptLib)
- Add Check if the Facility DB is available and pop a msg box if not
- Add Performance Tab and Notes Tab to FlightBag
- Add Touchdown log (MyDocuments\MSFS_HudBarSave\TouchDownLog.csv) to FlightBag
- Upadate AP Settings allow for large change on the left side of the item field (mouse wheel)
- Fix Bar/Window Location was not stored for profiles 5..10
- Cleanup of all new features
- Disable SimConnect.cfg (as per MS it is only needed when connecting MSFS via Network)
- SU10 compatibility checks
- Moved to Visual Studio 2022 Community Edition
- Update QuickGuides

V 0.54-B51
- Add Map, Metar, and Config in FlightBag
- Refactoring part 1 of AppSettings transition to a new Settings Library
- Refactoring to allow for standalone Apps (Cam, ChecklistBox, FlightBag)
- Update QuickGuide

V 0.53-B50
- Add Checklist Box as independent feature
- Add ADF(1 only) Items Freq, Name, ID + Bearing + Needle
- Add Distance to Destination item (calculated or from FPLan)
- Add Scroll Wheel Inc/Dec for SimRate
- Fix Some memory disposal of unused resources
- Update QuickGuide

V 0.52-B49
- Add 5 more profiles (total of 10 now)
- Add Ambient Vertical Wind Item (LIFT)
- Add Compass [degm] and arrow (N-up) Item
- Add Altitudes, Distances can be shown in Common (default) or Metric units
- Update ShowUnits is now an interactive menu setting
- Update Ability to enter the Route via Kbd when no ATC plan is available (on RTE Item)
- Update Complete/improve actual weights data in IAS ToolTip (Design Speed and Weights)
- Update Improve capturing of mouse wheel input focus (cannot prevent a single scroll event to the Sim though)
- Update QuickGuide

V 0.51-B48
- Add Camera Management Console
- Add Departure, Arrival Apt item
- Add LVar retrieval facilities for non SimVar items (not active in release build yet)
- Update QuickGuide

V 0.50-B47
- Add Control Surfaces Item as Graphics
- SU9 Compatibility Check
- Update QuickGuide

V 0.49-B46
- Version increment only

V 0.48-B45
- Add Engine 3 and 4 support
- Add AP AutoThrottle item (ATHR, TOGA) - experimental, scroll to change, TOGA click to toggle
- Add AP AutoBrake item (ABRK) - experimental, scroll to change
- Add AP IAS/MACH Speed Hold item
- Add AP Attitude Hold Item
- Update AP Setting handling (mouse scroll over the setting number - not the Label anymore)
- Update AP HDG setting number, click to set to current heading
- Update AP ALT mode handling (as the Sim does not provide correct states)
- Update AP settings, visual background changed to indicate active settings
- Add COM Activ, Standby Frequ. (with mouse dial and swap)
- Add NAV Activ, Standby Frequ. (with mouse dial and swap)
- Add A320 Throttle Position Indicator
- Update Setup for Audio Speech Render Device (VoiceOut)
- Add Debug module
- Refactoring of the Layout procedure
- Fix some issues found throughout
- Update QuickGuide

V 0.47-B44
- Fix Startup Error when Share Tech Mono Font is not installed on PC

V 0.46-B43
- Add Ability to set Fonts in Config
- Add Free Text Field
- Update VARIO sound (more like a real one - but still not pleasant..)
- Updated Airport DB from SU8 + Iberia Update
- Update Clean Waypoint Names from B21 Soaring Task decorations
- Fix issue with WP-Alt calculation
- Fix Improve layout procedure for some items
- Update QuickGuide

V 0.45-B42
- Add Graph items for many of % items and others where ranges are known
- Add 'ESI' graphical item (Attitude/FPA)
- Add Wind arrow for the Wind Dir@Speed item (Wind direction the Acft will see)
- Add Cowl Flaps as % Graph
- Update Set HudBar visible in Windows Taskbar
- Update IAS text gets Orange (5kt before) and Red (Stall) when approaching Config Stall Speed 
- Fix Flight Path Angle calculation 
- Fix Layout issue with Trim items
- Refacturing disposal of not shown items
- SU8 Compatibility Check
- Update QuickGuide

V 0.44-B41
- Fix Sound Library issues for certain Output Devices (caused unhandled Exception)
- Fix Config Dialog can be revived with Alt-Tab (before the App seemed to be no longer running)
- Fix Test Voices are back in Config
- Fix Issue in KML Coordinate formatting (GEarth is confused by blanks..)
- Fix VARIO uses correct speed (TAS) to get the TE-Rate
- Fix Limit VARIO output (was way out when the Sim is providing unsreasonable numbers on startup)
- Update Add Arrows to VARIO numbers (should be easier to comprehend)
- Update VARIO Average (second value) is now yellow text
- Update Add TAS, VARIO, Spoiler (value and Placemark) to KML/JSON recording

V 0.43-B40
- Add 'Flight Bag light' as feature (display image documents with zoom and drag)
- Add Variometer (Total Energy Type) item for m/s and kts with Ping
- Add Item AP/GPS Approach information (Apr.type and phase)
- Add Item Flight Path Angle (FPA = Pitch - Aoa)
- Add for all Fuel properties add kilogram items (Airbus unit)
- Add Apt-ALT to ATC Airport Item
- Add FontSize +18,20,24,28
- Fix Wind Dir @ Speed values corrected
- Fix ALT holding voiceout if not going for ALTS but the ALT button was pressed (guesswork...)
- Fix Reduce 'Not recording' label to 'Not rec.' (avoiding column size changes)
- Update Context Menu shows current selection as Checked Item (Profile, Appearance)
- Update Support Libs / Added SpeechLib and MetarLib to the solution
- Update QuickGuide

V 0.42-B38
- Fix KML Number formatting when the locale decimal separator is not a point (#4)

V 0.41-B37
- Add Available Aircraft Reference Speeds as tooltip to the IAS label
- Add Divider/Separator Mark for item lists
- Update Use NewLine and Separator also from currently not visible items (NewLine is prioritized)
- Update Add ZuluTime, Aoa,Bank,Pitch data in flight recording
- Update Add Rotate and Touchdown placemarks in KML out file
- Update Dynamic flight recording intervals
- Update Create KMZ (zip archive) instead of KML; as per guidance for larger files
- Update Bundle JSON file into KMZ
- Update Show active Freq. when NAV1/2 has no ID (Info Color)
- Update QuickGuide

V 0.40-B36
- Add Flight Recorder to create a KML and JSON flight logfile
- Add Mouse wheel Inc/Dec for ERA-Trims (also extend to 0.1% readout)
- Add Spoilers Voice Out
- Update Spoilers and Flaps indication for the End position (out) is now a blue icon (was green)
- Fix Spoiler indication (was not active)
- Update FP Capture and Save disabled when on ground
- Fix numeric issue with Alt@WP
- Update QuickGuide

V 0.39-B35
- Update Allow for configuration of Keyboard Hotkeys
- Update Gear availability included in Aircraft merges
- Fix Used the smaller font for NAV1/2 Names
- Update QuickGuide

V 0.38-B34
- Add Rotate Callout while on ground - uses the Sim provided speed - may be correct.. or not depends on the plane (enable in Configuration)
- Update Keyboard Hotkey switching uses another method which should be more compatible with some Game Tools out there.
- Fix Delayed Voice callouts to avoid speech when the App starts

V 0.38-B33
- Add item Transponder (Code and Status)
- Add item FPS (frames per second)
- Add item ZULU (fictional UTC of the current Simulation)
- Add item C-CLK (the computer local time)
- Add item GForce current
- Add item GForce min, max (resetable)
- Add item Afterburner % for engine 1+2
- Add item Spoilers / Speedbrakes (are the same SimVar)
- Add item FuelTank Center in Gal and Lb
- Add mouse wheel Inc/Dec on AP settings HDG, ALT, VS, FLC
- Add switching to Hide/Show the Bar via InGame Events, must be enabled in Config
- Add switching of Profiles via InGame Events, must be enabled in Config
- Add switching to Hide/Show the Bar (Right Ctrl + Numpad 0), must be enabled in Config
- Add switching of Profiles (Right Ctrl + Numpad 1 .. 5), must be enabled in Config
- Update AutoBackup can be selected from DISABLED (default), AutoBackup (5 Min), AutoBackup+ATC (30 sec)
         Known issue: while saving an FLT file to capture the backup or the ATC flightplan the Sim may momentarily stutter
- Fix Enroute Timer does not longer reset when opening Config
- Fix Interaction issue with SimConnect and loading new flights (workaround another MSFS bug)
- Update QuickGuide

V 0.37-B32
- Add item NAV1/2 Name incl. ILS LOC information 
- Fix more inconsistencies found throughout

V 0.37-B31
- Add item Control Handle% readout for Throttle, Mix, Propeller
- Add item Power LOAD %
- Add item CHT in �C and �F
- Add item EGT in �F
- Add item OAT in �F
- Add Button Click for SimRate to set it to 1x (normal)
- Add VoiceOut item for LowFuel Alert
- Add Copy / Paste of Items between profiles in Config menu
- Add Load default profiles in Config menu
- Add Aircraft specific Merge Items in Config menu (modifies only engine readouts according to the plane)
- Add Provide some default profiles when using the program the very first time
- Update Button Click handling for CP Meters to Stop (within 2 sec of Start)
- Update GPS source to show NAV/LOC 1/2 or GPS
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



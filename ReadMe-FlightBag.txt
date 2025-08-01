FS20_FlightBag V 0.72 - Build 72 BETA
(c) M. Burri - 12-Jul-2025

Contains files:

FS20_FlightBag.exe               The program

.\DemoBag                   Contains some images to showcase the Flight Bag

- All libraries below MUST be in the same folder as the Exe file
bm98_Album.dll              A UserControl for displaying images
bm98_hb_Folders.dll         Unified Folder manager
bm98_Html.dll               Library and Wrapper for HTML to PDF/Image conversion
bm98_Map.dll                Mapping display library
bm98_VProfile.dll           UserControl for displaying VProfiles
BM98CH_WasmClient.dll       WASM Module client to get LVars
CoordLib.dll                A coord. handling library
dNetBm98                    Tools for .Net and WinForms
dNetWkhtmlWrap.dll          Wrapping library for PDF formatter
MapLib.dll                  Mapping library
FlightplanLib.dll           Flightplan library
FShelf.dll                  Shelf WinForms library
MetarLib.dll                A METAR access library
FSimClientIF.dll            Generic FSim Client interface definition
FSimIF.dll                  Generic FSim interface definition
SimConnectClient.dll        FlightSim interface to MSFS2020 SimConnect
SimConnectClientAdapter.dll Connection wrapper
FSimFacilityIF.dll          MS facility database interface definition
FSFDataLib.dll              MS facility database access library
SettingsLib.dll             Application settings persistence library
WkWrapper.WkhtmlToPdf.Mxe.dll deploys the PDF renderer application
NLog.config.OFF             Logging config file / remove .OFF to enable logging

3rd Party:
BingMapsRESTToolkit.dll     Microsoft provided library for accessing Bing Map data
LiteDB.dll                  3rd party data management library
NLog.dll                    3rd party logging library
PdfiumViewer.dll            3rd party PDF viewer library
wkhtmltopdf.exe             3rd party HTML formatting library wrapper /replaces HtmlRenderer

From MSFS2020 Developer Kit for convenience included:
  SimConnect.cfg.OFF        Config file used only when connecting via network to MSFS (edit server IP)
  Microsoft.FlightSimulator.SimConnect.dll  MSFS2020 latest C# assembly 
  SimConnect.dll	                        MSFS2020 latest C++ module

Simulator Extension Module (extr. to Community folder)
  BM98CH_DataConnector_Wasm-V0.8.zip  
  BM98CH_DataConnector_Wasm2024-V4.8.zip  

dataLoader\ folder:
FacilityDataLoader.exe      MSFS Facility conversion tool
BGLlib.dll                  BGL and LLM decoder
CoordLib.dll                A coord. handling library
FSimFacilityIF.dll          MS facility database interface definition
FSFData.dll     MS facility database access library
NLog.config                 Logging config file
NLog.dll                    3rd party logging library
LiteDB.dll                  3rd party data management library
System.Data.SQLite.dll      3rd party data management library
x64\SQLite.Interop.dll      3rd party data management library 64bit C-library
x86\SQLite.Interop.dll      3rd party data management library 32bit C-library
V2020\MSFSPlug.dll			MSFS2020 SimConnect Adapter
V2020\Microsoft.FlightSimulator.SimConnect.dll  MSFS2020 latest C# assembly 
V2020\SimConnect.dll        MSFS2020 latest C++ module
V2024\MSFSPlug.dll			MSFS2024 SimConnect Adapter
V2024\Microsoft.FlightSimulator.SimConnect.dll  MSFS2024 latest C# assembly 
V2024\SimConnect.dll        MSFS2024 latest C++ module

ReadMe-FlightBag.txt                   This file

MSFS Hud Bar (.Net 4.8)

Extract the Zip File into a folder and hit FS20_FlighBag.exe to run it

For Updates and information visit:

https://github.com/bm98/FS20_HudBar


Scanned for viruses before packing... 
github@mail.burri-web.org

Changelog:

V 0.72-B72
- Please UPDATE the WASM Module (sww quick guide) !!!

- Add FS2024 native WASM module (V4.x for FS2024)
- Update To select only Airports with Helipads in FlightBag-Config Runways
- Update Stamen 3D Terrain default URL extended with parameter {r}
- Fix MapProvider for Stamen sometimes not having the key read properly
- Note: Bing Map Service is no longer available for private customers 
        The funtionality remains in place but is no longer supportable

V 0.70-B70
- Please UPDATE the WASM Module (from BM98CH_DataConnector_Wasm-V0.5.zip) !!!

- Add VProfile display in FlightBag - Maps Tab
- Add Download the original Simbrief Flightplan PDF (@.FlightPlan), omit image download
- Update FlightPlan Table is now a PDF (@.FlightTable)
- Update Improverd Flightplan decoding from ext. formats
- Update PDF Converter for Shelf replaced with WkHtmlToPDF application
- Update WASM Module to V0.5 (according to SDK advice)
- Update QuickGuides

V 0.68-B68
- RERUN FacilityDataLoader mandatory for improved coordinate precision !!!

- Add Other aircraft display in Flight Bag (Enable in Config tab)
- Add 'Radar' View map display
- Add Coordinate window
- Add Teleport feature
- Update Improve aircraft properties via LVARs incl. B787, B747, Fenix A320 V2 and others
- Update QuickGuides

V 0.67-B67
- RERUN FacilityDataLoader !!!
- CHECK MapLibProvider.ini  to enable Stamen 3D 

- Add Missed Approach Legs to Map Approach
- Add Decoding and display of LNM Plan (LNM native format); load from file
- Add Decoding and display of GPX Plan; load from file
- Add PDF as supported Shelf Format
- Add Subfolders support in FlightBag - Shelf
- Add Airport Overview to FlightBag - Shelf (in folder Airport Reports)
- Update SimBrief Flightplan is now stored PDF document (@.FlightPlan)
- Update FlightTable (@.FlightTable) more details added when available in plan
- Update Display of Routes with procedures and limits
- Update MapLib for Stamen Maps (now served by Stadia Maps - needs a Key, default OFF)
- Update METAR collection (new URL from provider)
- Update RA limit for Jet engine acft is now 2500ft
- Fix DME only stations are now properly identified
- Fix METAR decoding of weather in some cases
- Update Facility data base extended for procedures
- Update MSFS Connection procedure reviewed and improved
- Update Using NLog for logging now
- Update QuickGuide

V 0.65-B66
- Update Improve focus capture and release for scrollable items
- Update Uses LVar Get via SimVar framework (Set of such LVars still needs the WASM module)
- Refactoring, complete rework of the DataProvider DLLs 
- Update MapLib to comply with OSM tile policy (2 concurrent DL only)
- Update QuickGuide

V 0.64-B64 (not published)
- Refactoring, complete rework of the DataProvider DLLs 

V 0.63-B63
- RERUN FacilityDataLoader !!!
- CHECK MapLibProvider.ini  to enable chartbundle tiles (only FAA coverage)

- Add Approach panel and selection to Map (Runway Table)
- Add AutoRange feature for zooming the map
- Add ChartBundle.com Tile servers (disabled by default, use MapLibProvider.ini)
- Add UserTileServer 4..6 to be used
- Add Selected runway 'ladder' line (replaces the blue dotted arrow)
- Update Rwy and Approach Panel selection readability
- Update OpenTopoMap, Stamen URL and timeout for map retrieval set to 60sec
- Fix Exception when SimBrief Image/Doc Source is disabled (from Website)
- FacilityDataLoader: major rework for Approaches and Navigraph merges
- Refactoring, consolidation for commonly used stuff (dNetBm98 library now)
- Fix A number of issues found while testing...
- Update QuickGuide

- Update to .Net Framework 4.8

V 0.62-B62
- Fix Allow 4..7 digit SimBrief Pilot ID (no clear doc what it is found...)
- Internal Complete code comments for public items

V 0.61-B61
- Add SimBrief support (loading docs into Shelf, show Flightplan on map)
- Add MSFS PLN support (loading table doc into Shelf, show Flightplan on map)
- Add MSFS FLT support (loading table doc into Shelf, show Flightplan on map)
- Add Wind Arrow for tracked aircraft
- Add Track in aircraft data in Map
- Add Allow True and Mag toggle when clicking Heading or Track in Map
- Add Toggle Av/SI units when clicking data labels in Map (alt, speed, vs )
- Add XF range to map (Zoom level 7)
- Update Support GIF images in Shelf
- Fix AppSettings of Aircraft Range and Track was mismatched
- Update QuickGuide

V 0.60-B60
- Update FlightBag Map handling improved
- Update DataLoader fix reading of airport names (file changed in SU11)

V 0.58-B58
- Fix Exception raised when closing the App (when the WASM module in not used)

V 0.57-B57
- Add Profile Calculator in Flightbag
- Add Write Check and Message for MyDocuments folder
- Update QuickGuides

V 0.56-B56
- Update SettingsLib - creates backups now

V 0.55-B55
- Add Performance Tab and Notes Tab to FlightBag
- Add Touchdown log (MyDocuments\MSFS_HudBarSave\TouchDownLog.csv)
- Disabled SimConnect.cfg (as per MS it is only needed when connecting MSFS via Network)
- Add Check if the Facility DB is available and pop a msg box if not
- Moved to Visual Studio 2022 Community Edition
- Update QuickGuide

V 0.54-B51
- initial upload



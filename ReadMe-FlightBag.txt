FS20_FlightBag V 0.63 - Build 63 BETA
(c) M. Burri - 17-Feb-2023

Contains files:

FS20_FlightBag.exe               The program

.\DemoBag                   Contains some images to showcase the Flight Bag

- All libraries below MUST be in the same folder as the Exe file
FShelf.dll                  Shelf WinForms library
bm98_Album.dll              A UserControl for displaying images
bm98_Map.dll                Mapping display library
MapLib.dll                  Mapping library
FlightplanLib.dll           Flightplan library
MetarLib.dll                A METAR access library
CoordLib.dll                A coord. handling library
SettingsLib.dll             Application settings persistence library
SimBriefLib.dll             SimBrief service library
dNetBm98                    Tools for .Net and WinForms
FSimClientIF.dll            Generic FSim Client interface definition
FSimIF.dll                  Generic FSim interface definition
SimConnectClient.dll        FlightSim interface to MSFS2020 SimConnect
FSimFacilityIF.dll          MS facility database interface definition
FSimFacilityDataLib.dll     MS facility database access library
BM98CH_WasmClient.dll       WASM Module client to get LVars

3rd Party:
BingMapsRESTToolkit.dll     Microsoft provided library for accessing Bing Map data
LiteDB.dll                  3rd party data management library
HtmlRenderer.dll            3rd party HTML formatting library
HtmlRenderer.WinForms.dll   3rd party HTML formatting library

From MSFS2020 Developer Kit for convenience included:
  SimConnect.cfg.OFF        Config file used only when connecting via network to MSFS (edit server IP)
  Microsoft.FlightSimulator.SimConnect.dll 
  SimConnect.dll


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


ReadMe-FlightBag.txt                   This file

MSFS Hud Bar (.Net 4.7.2)

Extract the Zip File into a folder and hit FS20_FlighBag.exe to run it

For Updates and information visit:

https://github.com/bm98/FS20_HudBar


Scanned for viruses before packing... 
github@mail.burri-web.org

Changelog:

V 0.63-B63
- RERUN FacilityDataLoader !!!

- Add Approach panel and selection to Map (Runway Table)
- Update OpenTopoMap URL and timeout for map retrieval set to 60sec
- Fix Exception when SimBrief Image/Doc Source is disabled (from Website)
- FacilityDataLoader: major rework for Approaches and Navigraph merges
- Refactoring, consolidation for commonly used stuff (dNetBm98 library now)
- Update QuickGuide

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



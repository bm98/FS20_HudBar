FS20_FlightBag V 0.56 - Build 56 BETA
(c) M. Burri - 25-Nov-2022

Contains files:

FS20_FlightBag.exe               The program

.\DemoBag                   Contains some images to showcase the Flight Bag

- All libraries below MUST be in the same folder as the Exe file
FShelf.dll                  Shelf WinForms library
bm98_Album.dll              A UserControl for displaying images
bm98_Map.dll                Mapping display library
MapLib.dll                  Mapping library
MetarLib.dll                A METAR access library
CoordLib.dll                A coord. handling library
SettingsLib.dll             Application settings persistence library
FSimClientIF.dll            Generic FSim Client interface definition
FSimIF.dll                  Generic FSim interface definition
SimConnectClient.dll        FlightSim interface to MSFS2020 SimConnect
FSimFacilityIF.dll          MS facility database interface definition
FSimFacilityDataLib.dll     MS facility database access library
BM98CH_WasmClient.dll       WASM Module client to get LVars

3rd Party:
BingMapsRESTToolkit.dll     Microsoft provided library for accessing Bing Map data
LiteDB.dll                  3rd party data management library

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



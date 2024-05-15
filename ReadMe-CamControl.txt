FS20_CamControl V 0.70 - Build 70 BETA
(c) M. Burri - 20-May-2024

Contains files:

FS20_CamControl.exe               The program

.\DemoBag                   Contains some images to showcase the Flight Bag

- All libraries below MUST be in the same folder as the Exe file
FCamControl                 CamControl WinForms Library

dNetBm98                    Tools for .Net and WinForms
CoordLib.dll				A coord. handling library
FSimClientIF.dll            Generic FSim Client interface definition
SettingsLib.dll             Application settings persistence library
FSimIF.dll                  Generic FSim interface definition
SimConnectClient.dll        FlightSim interface to MSFS2020 SimConnect
SimConnectClientAdapter.dll Connection wrapper
BM98CH_WasmClient.dll       WASM Module client to get LVars

From MSFS2020 Developer Kit for convenience included:
  SimConnect.cfg.OFF        Config file used only when connecting via network to MSFS (edit server IP)
  Microsoft.FlightSimulator.SimConnect.dll 
  SimConnect.dll

ReadMe-CamControl.txt                   This file

MSFS Hud Bar (.Net 4.8)

Extract the Zip File into a folder and hit FS20_CamControl.exe to run it

For Updates and information visit:

https://github.com/bm98/FS20_HudBar


Scanned for viruses before packing... 
github@mail.burri-web.org

Changelog:

V 0.70-B70
- Add FlyBy feature in Camera Window
- Add Use Custom Cameras in Camera Window
- Fix Camera 6DOF Pitch direction
- Update QuickGuide

V 0.65-B66
- Add 6DOF Camera
- Update Camera Control added 20 slots (folder G,H)
- Update Uses LVar Get via SimVar framework (Set of such LVars still needs the WASM module)
- Refactoring, complete rework of the DataProvider DLLs 

V 0.64-B64 (not published)
- Refactoring, complete rework of the DataProvider DLLs 

V 0.63-B63
- Update to .Net Framework 4.8

V 0.58-B58
- Fix Exception raised when closing the App (when the WASM module in not used)

V 0.56-B56
- Update SettingsLib - creates backups now

V 0.55-B55
- initial upload
- Update QuickGuide



FS20_HudBar V 0.24 - Build 18 BETA
(c) M. Burri - 08-July-2021

Contains files:

FS20_HudBar.exe               The program

- All libraries below MUST be in the same folder as the Exe file
FSimClientIF.dll            Generic FSim Client interface definition
FSimIF.dll                  Generic FSim interface definition
SimConnectClient.dll        FlightSim interface to MSFS2020 SimConnect

From MSFS2020 Developer Kit for convenience included:
  SimConnect.cfg
  Microsoft.FlightSimulator.SimConnect.dll 
  SimConnect.dll


ReadMe.txt                   This file

MSFS Hud Bar (.Net 4.7.2)

Put all files into one folder and hit FS20_HudBar.exe to run it

For Updates and information visit:

https://github.com/bm98/FS20_HudBar


Scanned for viruses before packing... 
github@mail.burri-web.org

Changelog:
V 0.24-B18
- update Scroll Confi items while dragging up or down
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



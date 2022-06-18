# MSFS HudBar V 0.52.0.49

### Display essential Information as Bar or Tile at any side of the primary screen or as Window anywhere you like  


* Displays more than 100 essential aircraft and flight information items as Bar, Tile or Window
* Supports 1, 2, 3 and 4 engine aircrafts (Prop/Engine RPM, FuelFlow for each)
* Provides 5 different content profiles which are fully configurable
* The pilot can directly activate Autopilot and other commands
* Auto Elevator Trim on a click
* Saves FLT backup file
* Records the flight as KML and Json data file
* Bottom/Top Bars work best with wide screen monitors
* Flight Bag 'light' - display image documents with zoom and drag

![FS20_HudBar wide screen view](https://raw.githubusercontent.com/bm98/FS20_HudBar/main/doc/HudBar-screen.jpg "Wide Screen view")


![FS20_HudBar scale 50%](https://raw.githubusercontent.com/bm98/FS20_HudBar/main/doc/HudBar-halfsize.jpg "HudBar 1/2 size")

![FS20_HudBar KML recording](https://raw.githubusercontent.com/bm98/FS20_HudBar/main/doc/HudBar-Log-1.jpg "HudBar KML")

-----

#### Full Credit goes to JayDeeGaming
where the idea of this HudBar is 'borrowed' from   
https://www.youtube.com/c/JayDeeGaming/about

-----

## Usage 

### See Quick Guide in Doc section
https://github.com/bm98/FS20_HudBar/tree/main/doc/MSFS_HudBar-QuickGuide.pdf


* Deploy the release zip content in a folder (no installer provided or needed)

Best is to start MSFS first, then the Bar  
<br>
*	Start MSFS2020 first and once the Main Menu is shown
*	Start FS20_HudBar.exe
*	It attempts to connect to the Flight simulator in 5 sec intervals, but shows an error message while it cannot connect
* Note: the shown values are a bit meaningless until the aircraft and flight is live
Also note that the bar is shown on the ++PRIMARY monitor++ at the bottom of the screen
<br>
* Right Click the Bar and choose from the pop up menu
  * To select a Profile (1..5 - your names)
  * To Configure.. to check or uncheck the items to be shown
  * To Exit and stop the program
<br>
*	The Hud can be shown as Bar or Tile 
(to be changed in Configuration, default is Bar at the Bottom of the screen)
    * Bar: a full width window attached to the defined side of the screen
    * Tile: a window sized to accommodate the selected items
A Tile can be moved freely along the side where it is attached to   

### Configuration

Find more information in the QuickGuide here:

https://github.com/bm98/FS20_HudBar/tree/main/doc/MSFS_HudBar-QuickGuide.pdf


-----

My FlightSim Libraries (included in the release package)
<br>
* SpeechLib.dll               -- A voice synth lib using Win10 TTS facilities (new since V0.35)
* PingLib.dll                 -- An audio lib using Win10 Audio facilities (new since V0.43)
* MetarLib.dll                -- A METAR access library 
* bm98_Album.dll              -- A UserControl for displaying images (zoom & drag new since V0.43)
* bm98_hb_Controls.dll        -- UserControls for displaying graphs (new since V0.45)
<br>
* SimConnectClient.dll        -- FlightSim interface to MSFS2020 SimConnect
* FSimClientIF.dll            -- Generic FSim Client interface definition
* FSimIF.dll                  -- Generic FSim interface definition
* FS20_AptLib.dll             -- MSFS2020 Airport DB (SU8 updated)
* CoordLib.dll                -- A coord. handling library (https://github.com/bm98/CoordLib)
<br>

From Google Fonts Library embedded:
  Share_Tech_Mono			(Used as condensed font)
<br>
From MSFS2020 Developer Kit for convenience included:
* SimConnect.cfg
* Microsoft.FlightSimulator.SimConnect.dll 
* SimConnect.dll
  
.\DemoBag                   Contains some images to showcase the Flight Bag (new since V0.43)

-----

### General note for builders
The Project files expect referenced Libraries which have no NuGet package reference in a Solution directory  "Redist"  
To integrate with MSFS2020 SimConnect the solution must be built as x64 binary!   

So far the sources for some of the used Libraries are not on GitHub (yet)
  

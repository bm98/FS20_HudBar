# MSFS HudBar V 0.20.0.13

### Display essential Information as Bar at any side of the primary screen   


* Displays essential aircraft and flight information as Bar
* Supports 1 and 2 engine aircrafts (Prop/Engine RPM, FuelFlow for each)
* Provides 5 different content profiles which are fully configurable
* The pilot can directly activate Autopilot commands
* Auto Elevator Trim on a click
* Bottom/Top Bars work best with wide screen monitors


![FS20_HudBar wide screen view](https://raw.githubusercontent.com/bm98/FS20_HudBar/main/doc/HudBar-screen.jpg "Wide Screen view")


![FS20_HudBar scale 50%](https://raw.githubusercontent.com/bm98/FS20_HudBar/main/doc/HudBar-halfsize.jpg "HudBar 1/2 size")

-----

#### Full Credit goes to JayDeeGaming
where the idea of this HudBar is 'borrowed' from   
https://www.youtube.com/c/JayDeeGaming/about

-----

## Usage 

* Deploy the release zip content in a folder (no installer provided or needed)
* Start FS20_HudBar.exe
* It attempts to connect to the Flightsimulator in 5 sec intervals, but shows an error message while it cannot connect  
<br>
* Start MSFS2020 - it will connect once the Sim is up and running  
Note: the shown values are a bit meaningless until the aircraft is live   
Also note that the bar is shown on the ++PRIMARY monitor++ at the bottom of the screen   
<br>
* Right Click the Bar and choose from the pop up menu
  * To select a Profile (1..5 - your names)
  * To Configure.. to check or uncheck the items to be shown
  * To Exit and stop the program
<br>
* Grab the Splitter left or above the first shown item and move the left/top start of the bar
<br>
![FS20_HudBar Overview](https://raw.githubusercontent.com/bm98/FS20_HudBar/main/doc/HudBar-overview.jpg "HudBar Overview")
<br>
Autopilot commands that accept mouse clicks to toggle have a dark blue background and show a hand while hovering the active area  
* AP,HDG,ALT,VS,FLC,NAV and APR can be directly toggled
* BARO set to current (keyboard B button)
<br>
* ETrim - Elevator Trim: clicking the ETrim label will activate the Auto Elevator Trim module for about 20 seconds. 
It will display ETrim in green color while active - clicking the active module again will switch it off   
Note: the module controls the Elevator Trim in a way to level the aircraft towards zero vertical speed.
It may or may not work to your expectation.. so use it only if you feel comfortable with.
<br>
* Calculated fields when a "Next Waypoint" is available:
  * Estimate VS to WYP@ALT (WP-VS):   
  Calculated VS to reach the next Waypoint at the proposed Altitude with the current GS and DIST (ALTP when purple or Setting Alt when blue)
  * Estimate ALT@WYP (WP-ALT):   
  Calculated Altitude at next Waypoint using the actual GS, VS and DIST

### Configuration

Name and use up to 5 different profiles   

![FS20_HudBar Configuration](https://raw.githubusercontent.com/bm98/FS20_HudBar/main/doc/HudBar-config.jpg "HudBar Configuration")

Check __Show Units__ to display the units along the values  
Check __Opaque Background__ to have the Bar completely black, else it is slightly transparent  
Select a __Fontsize__ from Regular, Plus 2,4,6,8,10 an Minus 2,4   
(the bar rescales to multiple rows/columns to fit all checked items on the screen)  
Select the __Alignment__ of the bar (left, right, top, bottom)  

-----

My FlightSim Libraries (included in the release package)
* SimConnectClient.dll        -- FlightSim interface to MSFS2020 SimConnect
* FSimClientIF.dll            -- Generic FSim Client interface definition
* FSimIF.dll                  -- Generic FSim interface definition

From MSFS2020 Developer Kit for convenience included:
* SimConnect.cfg
* Microsoft.FlightSimulator.SimConnect.dll 
* SimConnect.dll
  
-----

### General note for builders
The Project files expect referenced Libraries which have no NuGet package reference in a Solution directory  "Redist"  
To integrate with MSFS2020 SimConnect the solution must be built as x64 binary!   

So far the sources from the used Libraries are not on GitHub (yet)
  
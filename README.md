# MSFS HudBar V 0.9.0.5

### Display essential Information as Bar at the bottom of the primary screen   


* Displays essential information as Bar at the bottom of the primary screen
* Supports 1 and 2 engine aircrafts (Prop/Engine RPM, FuelFlow for each)
* Provides 5 different content profiles which are fully configurable
* The pilot can directly activate Autopilot commands
* Auto Elevator Trim on a click
* Works best with wide screen monitors


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

* Start MSFS2020 - it will connect once the Sim is up and running  
Note: the shown values are a bit meaningless until the aircraft is live   
Also note that the bar is shown on the ++PRIMARY monitor++ at the bottom of the screen   
   

* Right Click the Bar at the bottom of the screen and choose from the pop up menu
  * To select a Profile (1..5)
  * To Configure.. to check or uncheck the items to be shown
  * To Exit and stop the program

![FS20_HudBar Overview](https://raw.githubusercontent.com/bm98/FS20_HudBar/main/doc/HudBar-overview.jpg "HudBar Overview")

* Autopilot commands that accept mouse clicks to toggle have a dark blue background and show a hand while hovering the active area  
* AP,HDG,ALT,VS,FLC,NAV and APR can be directly toggled

* ETrim - Elevator Trim: clicking the ETrim label will activate the Auto Elevator Trim module
for about 20 seconds. 
It will display ETrim in green color while active - clicking the active module again will switch it off   
Note: the module controls the Elevator Trim in a way to level the aircraft towards zero vertical speed.
It may or may not work to your expectation.. so use it only if you feel comfortable with.

### Configuration

Name and use up to 5 different profiles   

![FS20_HudBar Configuration](https://raw.githubusercontent.com/bm98/FS20_HudBar/main/doc/HudBar-config.jpg "HudBar Configuration")

Check ++Show Units++ to display the units along the values  

Check ++Opaque Background++ to have the Bar completely black, else it is slightly transparent

Select a ++Fontsize++ from Regular, Larger and Largest (you may limit the items so all fit the screen)

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
  
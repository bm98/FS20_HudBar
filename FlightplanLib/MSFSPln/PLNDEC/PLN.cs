using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.MSFSPln.PLNDEC
{

  /// <summary>
  /// An MSFS GPX Flight Plan
  /// </summary>
  [XmlRoot( ElementName = "SimBase.Document", Namespace = "", IsNullable = false )]
  public class PLN
  {
    #region EXAMPLE
    /*
            <?xml version="1.0" encoding="UTF-8"?>
            <SimBase.Document Type="AceXML" version="1,0">
                <Descr>AceXML Document</Descr>
                <FlightPlan.FlightPlan>
                    <Title>SBGL to SBSP</Title>
                    <FPType>IFR</FPType>
                    <RouteType>LowAlt</RouteType>
                    <CruisingAlt>12000</CruisingAlt>
                    <DepartureID>SBGL</DepartureID>
                    <DepartureLLA>S22° 48' 35.44",W43° 15' 10.99",+000018.00</DepartureLLA>
                    <DestinationID>SBSP</DestinationID>
                    <DestinationLLA>S23° 37' 33.98",W46° 39' 22.99",+002634.00</DestinationLLA>
                    <Descr>SBGL, SBSP created by Little Navmap 2.8.12</Descr>
                    <DeparturePosition>GATE A 2</DeparturePosition>
                    <DepartureName>Galeao-Antonio C Jobim Intl</DepartureName>
                    <DestinationName>Congonhas Intl</DestinationName>
                    <AppVersion>
                        <AppVersionMajor>11</AppVersionMajor>
                        <AppVersionBuild>282174</AppVersionBuild>
                    </AppVersion>
                    <ATCWaypoint id="SBGL">
                        <ATCWaypointType>Airport</ATCWaypointType>
                        <WorldPosition>S22° 48' 35.85",W43° 15' 2.09",+000018.00</WorldPosition>
                        <ICAO>
                            <ICAOIdent>SBGL</ICAOIdent>
                        </ICAO>
                    </ATCWaypoint>
                    <ATCWaypoint id="RW15">
                        <ATCWaypointType>User</ATCWaypointType>
                        <WorldPosition>S22° 48' 44.74",W43° 15' 49.33",+000028.00</WorldPosition>
                        <DepartureFP>TIVR1B</DepartureFP>
                        <RunwayNumberFP>15</RunwayNumberFP>
                        <ICAO>
                            <ICAOIdent>RW15</ICAOIdent>
                        </ICAO>
                    </ATCWaypoint>
                    <ATCWaypoint id="AUTOWP1">
                        <ATCWaypointType>User</ATCWaypointType>
                        <WorldPosition>S22° 49' 53.58",W43° 14' 2.69",+001073.20</WorldPosition>
                        <DepartureFP>TIVR1B</DepartureFP>
                        <RunwayNumberFP>15</RunwayNumberFP>
                        <ICAO>
                            <ICAOIdent>AUTOWP1</ICAOIdent>
                        </ICAO>
                    </ATCWaypoint>
                    <ATCWaypoint id="UTMIP">
                        <ATCWaypointType>Intersection</ATCWaypointType>
                        <WorldPosition>S22° 45' 7.27",W43° 1' 42.80",+007516.61</WorldPosition>
                        <DepartureFP>TIVR1B</DepartureFP>
                        <RunwayNumberFP>15</RunwayNumberFP>
                        <ICAO>
                            <ICAORegion>SB</ICAORegion>
                            <ICAOIdent>UTMIP</ICAOIdent>
                            <ICAOAirport>SBGL</ICAOAirport>
                        </ICAO>
                    </ATCWaypoint>
                    <ATCWaypoint id="ISVUL">
                        <ATCWaypointType>Intersection</ATCWaypointType>
                        <WorldPosition>S22° 51' 10.31",W42° 57' 50.84",+011186.43</WorldPosition>
                        <DepartureFP>TIVR1B</DepartureFP>
                        <RunwayNumberFP>15</RunwayNumberFP>
                        <ICAO>
                            <ICAORegion>SB</ICAORegion>
                            <ICAOIdent>ISVUL</ICAOIdent>
                            <ICAOAirport>SBGL</ICAOAirport>
                        </ICAO>
                    </ATCWaypoint>
                    <ATCWaypoint id="EVSOB">
                        <ATCWaypointType>Intersection</ATCWaypointType>
                        <WorldPosition>S22° 59' 54.64",W43° 5' 22.59",+012000.00</WorldPosition>
                        <DepartureFP>TIVR1B</DepartureFP>
                        <RunwayNumberFP>15</RunwayNumberFP>
                        <ICAO>
                            <ICAORegion>SB</ICAORegion>
                            <ICAOIdent>EVSOB</ICAOIdent>
                        </ICAO>
                    </ATCWaypoint>
                    <ATCWaypoint id="TIVRO">
                        <ATCWaypointType>Intersection</ATCWaypointType>
                        <WorldPosition>S23° 20' 8.73",W43° 23' 6.89",+012000.00</WorldPosition>
                        <DepartureFP>TIVR1B</DepartureFP>
                        <RunwayNumberFP>15</RunwayNumberFP>
                        <ICAO>
                            <ICAORegion>SB</ICAORegion>
                            <ICAOIdent>TIVRO</ICAOIdent>
                        </ICAO>
                    </ATCWaypoint>
                    <ATCWaypoint id="NISBO">
                        <ATCWaypointType>Intersection</ATCWaypointType>
                        <WorldPosition>S23° 29' 13.20",W43° 28' 31.79",+012000.00</WorldPosition>
                        <ICAO>
                            <ICAORegion>SB</ICAORegion>
                            <ICAOIdent>NISBO</ICAOIdent>
                        </ICAO>
                    </ATCWaypoint>
                    <ATCWaypoint id="PORNA">
                        <ATCWaypointType>Intersection</ATCWaypointType>
                        <WorldPosition>S23° 32' 22.20",W44° 15' 33.00",+012000.00</WorldPosition>
                        <ICAO>
                            <ICAORegion>SB</ICAORegion>
                            <ICAOIdent>PORNA</ICAOIdent>
                        </ICAO>
                    </ATCWaypoint>
                    <ATCWaypoint id="IBDAL">
                        <ATCWaypointType>Intersection</ATCWaypointType>
                        <WorldPosition>S23° 45' 29.47",W45° 13' 53.44",+012000.00</WorldPosition>
                        <ArrivalFP>IBDA1A</ArrivalFP>
                        <RunwayNumberFP>35</RunwayNumberFP>
                        <RunwayDesignatorFP>LEFT</RunwayDesignatorFP>
                        <ICAO>
                            <ICAORegion>SB</ICAORegion>
                            <ICAOIdent>IBDAL</ICAOIdent>
                        </ICAO>
                    </ATCWaypoint>
                    <ATCWaypoint id="MANLO">
                        <ATCWaypointType>Intersection</ATCWaypointType>
                        <WorldPosition>S23° 47' 49.89",W45° 24' 29.70",+012000.00</WorldPosition>
                        <ArrivalFP>IBDA1A</ArrivalFP>
                        <RunwayNumberFP>35</RunwayNumberFP>
                        <RunwayDesignatorFP>LEFT</RunwayDesignatorFP>
                        <ICAO>
                            <ICAORegion>SB</ICAORegion>
                            <ICAOIdent>MANLO</ICAOIdent>
                        </ICAO>
                    </ATCWaypoint>
                    <ATCWaypoint id="UKDAN">
                        <ATCWaypointType>Intersection</ATCWaypointType>
                        <WorldPosition>S23° 58' 59.34",W46° 15' 48.02",+012000.00</WorldPosition>
                        <ArrivalFP>IBDA1A</ArrivalFP>
                        <RunwayNumberFP>35</RunwayNumberFP>
                        <RunwayDesignatorFP>LEFT</RunwayDesignatorFP>
                        <ICAO>
                            <ICAORegion>SB</ICAORegion>
                            <ICAOIdent>UKDAN</ICAOIdent>
                        </ICAO>
                    </ATCWaypoint>
                    <ATCWaypoint id="NAXUP">
                        <ATCWaypointType>Intersection</ATCWaypointType>
                        <WorldPosition>S23° 51' 15.00",W46° 27' 10.21",+007868.78</WorldPosition>
                        <ArrivalFP>IBDA1A</ArrivalFP>
                        <RunwayNumberFP>35</RunwayNumberFP>
                        <RunwayDesignatorFP>LEFT</RunwayDesignatorFP>
                        <ICAO>
                            <ICAORegion>SB</ICAORegion>
                            <ICAOIdent>NAXUP</ICAOIdent>
                            <ICAOAirport>SBSP</ICAOAirport>
                        </ICAO>
                    </ATCWaypoint>
                    <ATCWaypoint id="SBSP">
                        <ATCWaypointType>Airport</ATCWaypointType>
                        <WorldPosition>S23° 37' 34.00",W46° 39' 22.99",+002634.00</WorldPosition>
                        <SuffixFP>Z</SuffixFP>
                        <ApproachTypeFP>RNAV</ApproachTypeFP>
                        <RunwayNumberFP>35</RunwayNumberFP>
                        <RunwayDesignatorFP>LEFT</RunwayDesignatorFP>
                        <ICAO>
                            <ICAOIdent>SBSP</ICAOIdent>
                        </ICAO>
                    </ATCWaypoint>
                </FlightPlan.FlightPlan>
            </SimBase.Document>

     */
    #endregion

    // Attributes
    /// <summary>
    /// The Type attribute
    /// </summary>
    [XmlAttribute( AttributeName = "Type" )]
    public string DType { get; set; } = ""; // AceXML
    /// <summary>
    /// The Version attribute
    /// </summary>
    [XmlAttribute( AttributeName = "version" )]
    public string DVersion { get; set; } // "1,0" have not seen others

    // Elements
    /// <summary>
    /// The Descr Element (always 'AceXML Document')
    /// </summary>
    [XmlElement( ElementName = "Descr" )]
    public string Descr { get; set; } = ""; // "AceXML Document"


    /// <summary>
    /// The Flight Plan
    /// </summary>
    [XmlElement( ElementName = "FlightPlan.FlightPlan", Type = typeof( X_FlightPlan ) )]
    public X_FlightPlan FlightPlan { get; set; } = null;

    // Non XML

    /// <summary>
    /// True if successfully retrieved
    /// </summary>
    [XmlIgnore]
    public bool IsValid {
      get {
        if (FlightPlan == null) return false;
        FlightPlan.PostProc( );
        FlightPlan.InsertProcedures( );
        return true;
      }
    }

  }
}

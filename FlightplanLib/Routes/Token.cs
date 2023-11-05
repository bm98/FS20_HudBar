using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.Routes
{

  internal enum TokenType
  {
    /// <summary>
    /// The ending token
    /// </summary>
    END = 0,
    /// <summary>
    /// Keyword SID
    /// </summary>
    SID,
    /// <summary>
    /// Keyword STAR
    /// </summary>
    STAR,
    /// <summary>
    /// Keyword DCT (direct to)
    /// </summary>
    DCT,
    /// <summary>
    /// A single ID (airport, waypoint, sid-, star-name
    /// Arg1= ID
    /// </summary>
    ID,
    /// <summary>
    /// An ID which is followed by a dotted ID (e.g waypoint.star-name)
    /// Arg1= first ID
    /// Arg2= second ID
    /// may be followed by CRUISE_SPEED_ALT or EST_TIME
    /// </summary>
    ID_DOTTED,
    /// <summary>
    /// A coordinate
    /// Arg1= Latitude (DD[DD]{NS})
    /// Arg2= Longitude (DD[DD]{EW})
    /// </summary>
    COORD,
    /// <summary>
    /// A Cruise SpeedAlt remark
    /// Arg1= Speed
    /// Arg2= Altitude
    /// </summary>
    CRUISE_SPEED_ALT,
    /// <summary>
    /// A Time estimate following an airport ID
    /// Arg1= Time (HHMM) 24h
    /// </summary>
    EST_TIME,
    /// <summary>
    /// A Runway remark following an airport ID by slash
    /// Arg1= RunwayNum+Designation (DD[A-Z])
    /// </summary>
    RUNWAY,

    /// <summary>
    /// Departure Airport
    /// Arg1=ICAO
    /// Arg2=EstTime or empty
    /// Arg3=Runway or empty
    /// </summary>
    AIRPORT_DEP,
    /// <summary>
    /// Arrival Airport
    /// Arg1=ICAO
    /// Arg2=EstTime or empty
    /// Arg3=Runway or empty
    /// </summary>
    AIRPORT_ARR,
    /// <summary>
    /// Alternate Airport
    /// Arg1=ICAO
    /// Arg2=EstTime or empty
    /// Arg3=Runway or empty
    /// </summary>
    AIRPORT_ALT,

    /// <summary>
    /// Airway 
    /// Arg1=Airway Ident
    /// Arg2=Exit Waypoint Ident
    /// </summary>
    AIRWAY,

    /// <summary>
    /// A SID procedure
    /// Arg1=Ident
    /// </summary>
    SID_PROC,
    /// <summary>
    /// A STAR procedure
    /// Arg1=Ident
    /// </summary>
    STAR_PROC,

    /// <summary>
    /// Waypoint
    /// Arg1=Waypoint Ident
    /// Arg2=SpeedAlt Remark or empty
    /// </summary>
    WAYPOINT,

    /// <summary>
    /// An invalid / unidentifiable word
    /// </summary>
    Invalid = 99,
  }

  /// <summary>
  /// A Token 
  /// </summary>
  internal struct Token
  {
    public TokenType TokenT;
    public string TokenS;
    // decoded arguments of this Token
    // args are TokenType specific
    // args are numbered by appearance from left to right
    public string Arg1;
    public string Arg2;
    public string Arg3;

    public override string ToString( )
    {
      if (!string.IsNullOrEmpty( Arg3 ))
        return $"Token: {TokenT}-{TokenS}({Arg1},{Arg2},{Arg3})";
      else if (!string.IsNullOrEmpty( Arg2 ))
        return $"Token: {TokenT}-{TokenS}({Arg1},{Arg2})";
      else if (!string.IsNullOrEmpty( Arg1 ))
        return $"Token: {TokenT}-{TokenS}({Arg1})";
      else
        return $"Token: {TokenT}-{TokenS}";
    }

    /// <summary>
    /// True if this is an END token
    /// </summary>
    public bool IsEnd => IsEndToken( this );
    /// <summary>
    /// True if this is an Invalid token
    /// </summary>
    public bool IsInvalid => IsInvalidToken( this );
    /// <summary>
    /// True if this is a Valid token
    /// </summary>
    public bool IsValid => !IsInvalidToken( this );


    public bool IsDCT => TokenT == TokenType.DCT;
    public bool IsSID => TokenT == TokenType.SID;
    public bool IsID => TokenT == TokenType.ID;
    public bool IsID_DOTTED => TokenT == TokenType.ID_DOTTED;
    public bool IsSPEED_ALT => TokenT == TokenType.CRUISE_SPEED_ALT;
    public bool IsEST_TIME => TokenT == TokenType.EST_TIME;
    public bool IsRUNWAY => TokenT == TokenType.RUNWAY;
    public bool IsSTAR => TokenT == TokenType.STAR;
    public bool IsCOORD => TokenT == TokenType.COORD;


    /// <summary>
    /// True if the token is the END token
    /// </summary>
    public static bool IsEndToken( Token token ) => token.TokenT == TokenType.END;
    /// <summary>
    /// True if the token is the END token
    /// </summary>
    public static bool IsInvalidToken( Token token ) => token.TokenT == TokenType.Invalid;
    
    /// <summary>
    /// Returns an END Token
    /// </summary>
    public static Token EndToken => new Token { TokenT = TokenType.END, TokenS = "$EOS$" };

  }

}

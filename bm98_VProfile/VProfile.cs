using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using bm98_VProfile.Drawing;

using static bm98_VProfile.PanelConst;
using static bm98_VProfile.FontsAndColors;

namespace bm98_VProfile
{
  internal class VProfile
  {
    // number of waypoints supported
    private const int c_numWyp = 5; // MAX 10 (update DItems if more are needed)

    /// <summary>
    /// All Display items
    /// </summary>
    private enum DItems
    {
      FIRST_NOTUSED = 0,

      // Tapes
      ALTtape,

      // Vertical Path 
      VPath,

      // Wyps
      NextWyp,
      Wyp2,
      Wyp3,
      Wyp4,
      Wyp5,
      Wyp6,
      Wyp7,
      Wyp8,
      Wyp9,
      Wyp10,

      // Text
      DstMidGraph,
      DstEndGraph,
      MinEndGraph,
      FpaDegGraph,
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public VProfile( DisplayList drawings )
    {
      // create the required display items and hook them into the DisplayList
      var altTape = new TapeItem( 4000 ) {
        Key = (int)DItems.ALTtape,
        Value = 25460,
        AlignRight = true,
        Active = ActiveState.Engaged,
        Rectangle = AltTapeField,
        Font = FtNumSmall, //FtTape
        TextBrushActive = BrushInfo,
        TextBrushArmed = BrushInfoDim, // used for Tape numbers
        Pen = PenInfo,
        FillBrush = BrushInfoDim
      }; drawings.AddItem( altTape );

      // create the required display items and hook them into the DisplayList
      var vPath = new LineItem( ) {
        Key = (int)DItems.VPath,
        StartPoint = new PointF( 0, 0.5f ),
        EndPoint = new PointF( 1, 1 ),
        Active = ActiveState.Engaged,
        Rectangle = VPathGraphField,
        Pen = PenNav2,
      }; drawings.AddItem( vPath );

      // Waypoints
      var protoWyp = new WypItem( ) {
        Rectangle = VPathGraphField,
        Font = FtNumSmall,
        StringFormat = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far },
        Active = ActiveState.Off,
        FillBrush = BrushInfoDim, // whiteish
        TextBrushActive = BrushInfoDim,
        TextBrushWarn = BrushInfo,
        BackgBrushWarn = BrushInfoDarker,
      };

      // Next WYP
      var nextWyp = new WypItem( protoWyp ) {
        Key = (int)DItems.NextWyp,
        FillBrush = BrushGps,
        TextBrushActive = BrushGps, // next is Magenta
        String = $"WYP",
      }; drawings.AddItem( nextWyp );
      // following WYPs (default Off)
      for (int i = 1; i < c_numWyp; i++) {
        nextWyp = new WypItem( protoWyp ) {
          Key = (int)DItems.NextWyp + i, // enum further wyps
          String = $"WYP{i}",
        }; drawings.AddItem( nextWyp );
      }

      // Text Labels
      var protoText = new TextItem( ) {
        Active = ActiveState.Engaged,
        TextBrushArmed = BrushInfoDim,
        TextBrushActive = BrushInfo,
        BackgBrushAlarm = BrushDebug
      };

      // Mid Dist
      var midDistText = new TextItem( protoText ) {
        Key = (int)DItems.DstMidGraph,
        String = $"50 nm",
        Rectangle = TxMidDist,
        Font = FtNumSmall,
        StringFormat = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far },
        Active = ActiveState.Engaged, // use background
        TextBrushWarn = BrushInfo,
        BackgBrushWarn = BrushInfoDarker,
      }; drawings.AddItem( midDistText );
      // End Dist
      var endDistText = new TextItem( protoText ) {
        Key = (int)DItems.DstEndGraph,
        String = $"100",
        Rectangle = TxEndDist,
        Font = FtNumSmall,
        StringFormat = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far },
        Active = ActiveState.Engaged, // use background
        TextBrushWarn = BrushInfo,
        BackgBrushWarn = BrushInfoDarker,
      }; drawings.AddItem( endDistText );
      // End Minutes
      var endMinText = new TextItem( protoText ) {
        Key = (int)DItems.MinEndGraph,
        String = $"100",
        Rectangle = TxEndMin,
        Font = FtNumSmall,
        StringFormat = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far },
        Active = ActiveState.Engaged, // use background
        TextBrushWarn = BrushInfo,
        BackgBrushWarn = BrushInfoDarker,
      }; drawings.AddItem( endMinText );
      // Flightpath angle
      var fpaText = new TextItem( protoText ) {
        Key = (int)DItems.FpaDegGraph,
        String = $"3.0",
        Rectangle = TxFpaDeg,
        Font = FtNumSmall,
        //TextBrushActive = BrushInfo,  // text color
        StringFormat = new StringFormat( ) { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Far },
        Active = ActiveState.Engaged, // use background
        TextBrushWarn = BrushInfo,
        BackgBrushWarn = BrushInfoDarker,
      }; drawings.AddItem( fpaText );


    }

    /// <summary>
    /// Update from Data Record
    /// </summary>
    /// <param name="gProc">The Processor</param>
    /// <param name="props">The new data</param>
    public void Update( GProc gProc, UC_VProfile.UC_VProfileProps props )
    {
      DisplayItem di;

      /*
       // **** TEST DATA ONLY
       props.GS_kt = 320;
       props.ALT_ft = 16000;
       props.VS_fpm = -1600;
       props.NextWyp = new UC_VProfile.UC_VProfileWaypoint( ) { Ident = "WYPYX", Distance_nm = 1, TargetAlt_ft = 8000f };
       props.WaypointList = new List<UC_VProfile.UC_VProfileWaypoint> {
         new UC_VProfile.UC_VProfileWaypoint( ) { Ident = "WYPY1", Distance_nm = 30, TargetAlt_ft = float.NaN },
         new UC_VProfile.UC_VProfileWaypoint( ) { Ident = "WYPY2", Distance_nm = 40, TargetAlt_ft = 4000f },
       };
       // **** TEST DATA ONLY
      */

      if (!float.IsNaN( props.ALT_ft )) {
        // Tape
        di = gProc.Drawings.DispItem( (int)DItems.ALTtape );
        di.Active = ActiveState.Engaged;
        // calc vPath
        int hScale = DistScaleFor( props.GS_kt, props.ALT_ft );
        float nmPmin = props.GS_kt / 60f;
        float minToTgt = (2 * hScale) / nmPmin;
        float dAlt = props.VS_fpm * minToTgt;
        minToTgt = (props.GS_kt < 1) ? float.PositiveInfinity : minToTgt; // cut if GS is <1
        // clip to a reasonable number
        dAlt = dAlt > 60_000f ? 60_000f
               : dAlt < -60_000f ? -60_000f
               : dAlt;
        int vScale = AltScaleFor( Math.Abs( dAlt ) );

        (di as TapeItem).Value = props.ALT_ft;
        (di as TapeItem).ValScale = vScale;
        //        gProc.Drawings.DispItem( (int)DItems.ALTtape ).String = $"{Conversions.Quant20( props.ALT_ft ):## ##0}";

        // Distance legend text
        di = gProc.Drawings.DispItem( (int)DItems.DstMidGraph );
        di.Active = ActiveState.Engaged;
        di.String = $"{hScale} nm";

        di = gProc.Drawings.DispItem( (int)DItems.DstEndGraph );
        di.Active = ActiveState.Engaged;
        di.String = $"{hScale * 2}";

        // Start and Endpoint of the vprofile line
        // point values are normalized within the graph area
        di = gProc.Drawings.DispItem( (int)DItems.VPath );
        di.Active = ActiveState.Engaged;
        (di as LineItem).StartPoint = new PointF( 0, 0.5f ); // starts left middle
        // alt scale is +* 2.5 unit steps
        (di as LineItem).EndPoint = new PointF( 1f, 0.5f - dAlt / (5f * vScale) );

        di = gProc.Drawings.DispItem( (int)DItems.MinEndGraph );
        di.Active = ActiveState.Engaged;
        di.String = $"{minToTgt:0}\nMin";

        di = gProc.Drawings.DispItem( (int)DItems.FpaDegGraph );
        di.Active = ActiveState.Engaged;
        di.String = $"{props.FPA_deg:0.0}°";

        // Do Waypoints

        // Disable all before updating from the available ones
        for (int i = 0; i < c_numWyp; i++) {
          gProc.Drawings.DispItem( (int)DItems.NextWyp + i ).Active = ActiveState.Off;
        }
        // add from list if there are
        if ((props.WaypointList != null) && (props.WaypointList.Count > 0)) {
          // Next Wyp
          di = gProc.Drawings.DispItem( (int)DItems.NextWyp );
          var wyp = props.WaypointList[0];
          if (wyp.IsValid) {
            di.Active = ActiveState.Engaged;
            if (!float.IsNaN( wyp.TargetAlt_ft )) {
              // draw with Alt at Altitude
              di.String = $"{wyp.Ident}\n{wyp.TargetAlt_ft:## 000}";
              (di as WypItem).WypPoint = new PointF( (float)wyp.Distance_nm / (2 * hScale), 0.5f - (wyp.TargetAlt_ft - props.ALT_ft) / (5f * vScale) );
            }
            else {
              // draw without Alt - on the Acft Line
              di.String = $"{wyp.Ident}";
              (di as WypItem).WypPoint = new PointF( (float)wyp.Distance_nm / (2 * hScale), 0.5f );
            }
          }

          // further Wyps
          for (int i = 1; i < c_numWyp; i++) {
            if (props.WaypointList.Count <= i) break; // have no more in the list

            wyp = props.WaypointList[i];
            if (wyp.Distance_nm > (hScale * 2)) break; // point is out of bounds

            // show Wyp
            int key = (int)DItems.NextWyp + i;
            di = gProc.Drawings.DispItem( key );
            if (wyp.IsValid) {
              di.Active = ActiveState.Engaged;
              if (!float.IsNaN( wyp.TargetAlt_ft )) {
                // draw with Alt at Altitude
                di.String = $"{wyp.Ident}\n{wyp.TargetAlt_ft:## 000}";
                (di as WypItem).WypPoint = new PointF( (float)wyp.Distance_nm / (2 * hScale), 0.5f - (wyp.TargetAlt_ft - props.ALT_ft) / (5f * vScale) );
              }
              else {
                // draw without Alt - on the Acft Line
                di.String = $"{wyp.Ident}";
                (di as WypItem).WypPoint = new PointF( (float)wyp.Distance_nm / (2 * hScale), 0.5f );
              }
            }
          } // all Wyps in list
        }

      }
      else {
        // switch all off (could be done with a Hook item...)
        gProc.Drawings.DispItem( (int)DItems.ALTtape ).Active = ActiveState.Off;
        gProc.Drawings.DispItem( (int)DItems.DstMidGraph ).Active = ActiveState.Off;
        gProc.Drawings.DispItem( (int)DItems.DstEndGraph ).Active = ActiveState.Off;
        gProc.Drawings.DispItem( (int)DItems.MinEndGraph ).Active = ActiveState.Off;
        gProc.Drawings.DispItem( (int)DItems.NextWyp ).Active = ActiveState.Off;
        for (int i = 1; i < c_numWyp; i++) {
          gProc.Drawings.DispItem( (int)DItems.NextWyp + i ).Active = ActiveState.Off;
        }
      }

    }

    // calc the Altitude scaling for given full scale delta Alt
    private int AltScaleFor( float dAlt_ft )
    {
      // if (dAlt_ft > 28_000) return 15_000;
      // if (dAlt_ft > 24_000) return 12_000;
      //if (dAlt_ft > 20_000) return 10_000;
      if (dAlt_ft > 16_000) return 8_000;
      //if (dAlt_ft > 12_000) return 6_000;
      if (dAlt_ft > 8_000) return 4_000;
      return 2000;
    }

    // calc the Distance scaling for given speed and alt - the graph shows 2 unit steps
    private int DistScaleFor( float gs_kt, float alt_ft )
    {
      if ((alt_ft >= 10_000) || (gs_kt > 185)) return 30;
      return 10;
    }

  }
}

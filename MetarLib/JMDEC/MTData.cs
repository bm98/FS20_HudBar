using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetarLib.JMDEC
{
  internal class MTData : MDEC.MTData
  {

    // Static Decoder

    /// <summary>
    /// Decode a METAR/TAF message
    /// </summary>
    /// <param name="msg">The message</param>
    /// <returns>A filled MTData object</returns>
    public static MTData Decode( J_MetarEntry msg )
    {
      // sanity
      if (msg==null) return new MTData();

      var mdata = new MTData { RAW = msg.RawString }; // save reference
      if (string.IsNullOrWhiteSpace( msg.RawString )) return mdata;

      string raw = msg.RawString.Replace( "\n", "" ) + " "; // remove CRLF and we need a space at the end...

      //raw = M_MsgTypeDecoder.Decode( raw, mdata ); // defaults to METAR if not tagged with a Msg Type

      if (mdata.MsgType == MDEC.MsgType.METAR) {
        DecodeMetar( raw, mdata );
      }
      else if (mdata.MsgType == MDEC.MsgType.SPECI) {
        DecodeMetar( raw, mdata );
      }
      else if (mdata.MsgType == MDEC.MsgType.TAF) {
        DecodeTaf( raw, mdata );
      }

      return mdata;
    }

  }
}

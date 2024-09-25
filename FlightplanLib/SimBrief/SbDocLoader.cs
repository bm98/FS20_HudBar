using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using DbgLib;

using FlightplanLib.Flightplan;


namespace FlightplanLib.SimBrief
{
  /// <summary>
  /// SimBrief Document Loader 
  ///  gets SB Docs into the designated directory (usually the Shelf Folder)
  /// </summary>
  public class SbDocLoader
  {
    #region STATIC DEBUG
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );
    #endregion

    // Loader Service
    private SimBrief _simBrief;

    /// <summary>
    /// cTor:
    /// </summary>
    public SbDocLoader( )
    {
      _simBrief = new SimBrief( );
      _simBrief.SimBriefDownloadEvent += _simBrief_SimBriefDownloadEvent;
    }

    private void _simBrief_SimBriefDownloadEvent( object sender, EventArgs e )
    {
      ; // a ping from the downloader, ignored by now
    }

    // Render via Wkhtmltox HTML render
    // HTML string, targetFolder
    private bool RenderPDF( string html, string targetFolder, string header )
    {
      LOG.Log( "SbDocLoader", "Wkhtmltox_Render" );

      if (bm98_Html.HtmlRenderer.Instance.Add_ToPdf_A4_Portrait(
               html,
               Path.Combine( targetFolder, bm98_hbFolders.Folders.FPlanPDF_FileName ),
               "Simbrief Flightplan", header )
        ) {
        return true;
      }
      else {
        LOG.Log( "RenderPDF", "Rendering or saving to PDF failed" );
      }
      return false;
    }


    // get one document
    private void LoadDocument( FileLink link, string targetFolder, string localName )
    {
      // Sanity
      if (link == null) return;
      if (!link.IsValid) return;

      _simBrief.PostDownload_Request( link.DownloadUrl, localName, targetFolder );
    }

    /// <summary>
    /// Load all supported document into the target folder
    /// </summary>
    /// <param name="plan">A valid FlihtPlan</param>
    /// <param name="targetFolder">A valid destination directory</param>
    public bool LoadDocuments( FlightPlan plan, string targetFolder )
    {
      // Sanity
      if (plan == null) return false;
      if (!Directory.Exists( targetFolder )) return false;

      // in Simbrief, the PDF is the first document
      if ((plan.DocLinks.Count > 0) && plan.DocLinks[0].IsValid) {
        LoadDocument( plan.DocLinks[0], targetFolder, bm98_hbFolders.Folders.FPlanPDF_FileName );
        return true;
      }
      else {
        // determine layout if possible from received data
        if (!Enum.TryParse( plan.FPLayout, out HTMLdocFormat planLayout )) {
          // else try to figure out from known code snippets
          planLayout = DetectDocFormat( plan.HTMLdocument );
        }
        // build from HTML
        var limitedPlan = PrettyHtml( plan.HTMLdocument, planLayout );
        var header = PrettyHeader( plan, planLayout );
        return RenderPDF( limitedPlan, targetFolder, header );
      }
    }

    // known Flighplan Document types
    private enum HTMLdocFormat
    {
      Unknown = 0,
      AAL,
      ACA,
      BAW,
      DLH,
      EZY,
      LIDO,
      RYR,
      SWA,
      UAE,
      UAL_2018,
    }

    // adapt the header with the Layout examples
    // note this is the header center text, left is date, right is page no by policy...
    private string PrettyHeader( FlightPlan plan, HTMLdocFormat docFormat )
    {
      switch (docFormat) {
        case HTMLdocFormat.BAW:
          // like  UAE330
          return $"{plan.Airline_ICAO}{plan.FlightNumber}";

        case HTMLdocFormat.DLH:
          // like  UAE330/22/ZUE/GVA/
          return $"{plan.Airline_ICAO}{plan.FlightNumber}/{DateTime.Now:dd}/{plan.Origin.Iata_Ident}/{plan.Destination.Iata_Ident}/";

        case HTMLdocFormat.RYR:
          // like  UAE 330/22Aug24/ZUE-GVA Reg:GEDEE
          return $"{plan.Airline_ICAO} {plan.FlightNumber}/{DateTime.Now:dd}{DateTime.Now:MMM}{DateTime.Now:yy}/{plan.Origin.Iata_Ident}-{plan.Destination.Iata_Ident}  Reg:{plan.AircraftReg}";

        // same for all below
        case HTMLdocFormat.EZY:
        case HTMLdocFormat.LIDO:
          // like  UAE 330/22Aug24/ZUE-GVA
          return $"{plan.Airline_ICAO} {plan.FlightNumber}/{DateTime.Now:dd} {DateTime.Now:MMM}/{plan.Origin.Iata_Ident}-{plan.Destination.Iata_Ident}";


        // same for all below
        case HTMLdocFormat.AAL:
        case HTMLdocFormat.ACA:
        case HTMLdocFormat.SWA:
        case HTMLdocFormat.UAE:
        case HTMLdocFormat.UAL_2018:
          // like  UAE 330   LSZH/LSGG
          return $"{plan.Airline_ICAO} {plan.FlightNumber}   {plan.Origin.Icao_Ident}/{plan.Destination.Icao_Ident}";

        case HTMLdocFormat.Unknown:
        default:
          // like  UAE 330/LSZH/LSGG//LSZH
          return $"{plan.Airline_ICAO} {plan.FlightNumber}/{plan.Origin.Icao_Ident}/{plan.Destination.Icao_Ident}//{plan.Alternate.Icao_Ident}";
      }
    }

    // modify html from SimBrief for limitations or inconsistencies with the current Renderer
    private string PrettyHtml( string origHtml, HTMLdocFormat docFormat )
    {
      // sanity
      if (string.IsNullOrWhiteSpace( origHtml )) return "<div style=\"padding:10px\"><pre><h1>NO PLAN AVAILABLE !</pre></div>";


      // prepend with UTF8 charset explicitely for our library (may have issues otherwise)
      var html = "<meta http-equiv=\"Content-Type\" content=\"text/html;charset=UTF-8\">\n"
                + origHtml;

      if (docFormat == HTMLdocFormat.LIDO) {
        // get a larger text size for our rendering
        html = html.Replace(
          "<div style=\"line-height:14px;font-size:13px\">",
          "<div style=\"line-height:20px;font-size:19px; font-weight: 600;\">" );
        // inc image size 
        html = html.Replace( "width=\"600px\"></a>", "width=\"900px\"></a>" );
        return html;
      }

      else if (docFormat == HTMLdocFormat.AAL) {
        // get a larger text size for our rendering
        html = html.Replace(
          "<div style=\"line-height:14px;font-size:13px\">",
          "<div style=\"line-height:20px;font-size:19px; font-weight: 600;\">" );
        // inc image size 
        html = html.Replace( "width=\"600px\"></a>", "width=\"900px\"></a>" );
        return html;
      }

      else if (docFormat == HTMLdocFormat.ACA) {
        // get a larger text size for our rendering
        html = html.Replace(
          "<div style=\"line-height:15px\">",
          "<div style=\"line-height:20px;font-size:19px; font-weight: 600;\">" );
        // inc image size 
        html = html.Replace( "width=\"600px\"></a>", "width=\"900px\"></a>" );
        return html;
      }

      else if (docFormat == HTMLdocFormat.BAW) {
        // get a larger text size for our rendering
        html = html.Replace(
          "<div style=\"line-height:14px;font-size:13px\">",
          "<div style=\"line-height:20px;font-size:19px; font-weight: 600;\">" );
        // inc image size 
        html = html.Replace( "width=\"600px\"></a>", "width=\"900px\"></a>" );
        return html;
      }

      else if (docFormat == HTMLdocFormat.DLH) {
        // get a larger text size for our rendering
        html = html.Replace(
          "<div style=\"line-height:14px;font-size:13px\">",
          "<div style=\"line-height:20px;font-size:19px; font-weight: 600;\">" );
        // inc image size 
        html = html.Replace( "width=\"600px\"></a>", "width=\"900px\"></a>" );
        return html;
      }

      else if (docFormat == HTMLdocFormat.EZY) {
        // get a larger text size for our rendering
        html = html.Replace(
          "<div style=\"line-height:14px;font-size:13px\">",
          "<div style=\"line-height:20px;font-size:19px; font-weight: 600;\">" );
        // inc image size 
        html = html.Replace( "width=\"600px\"></a>", "width=\"900px\"></a>" );
        return html;
      }

      else if (docFormat == HTMLdocFormat.RYR) {
        // get a larger text size for our rendering
        html = html.Replace(
          "<div style=\"line-height:14px;font-size:13px\">",
          "<div style=\"line-height:20px;font-size:19px; font-weight: 600;\">" );
        // inc image size 
        html = html.Replace( "width=\"600px\"></a>", "width=\"900px\"></a>" );
        return html;
      }

      else if (docFormat == HTMLdocFormat.SWA) {
        // get a larger text size for our rendering
        html = html.Replace(
          "<div style=\"line-height:14px;font-size:12px\">",
          "<div style=\"line-height:19px;font-size:17px; font-weight: 600;\">" );
        // inc image size 
        html = html.Replace( "width=\"600px\"></a>", "width=\"900px\"></a>" );
        return html;
      }

      else if (docFormat == HTMLdocFormat.UAE) {
        // get a larger text size for our rendering
        html = html.Replace(
          "<div style=\"line-height:15px\">",
          "<div style=\"line-height:19px;font-size:18px; font-weight: 600;\">" );
        // inc image size 
        html = html.Replace( "width=\"600px\"></a>", "width=\"900px\"></a>" );
        return html;
      }

      else if (docFormat == HTMLdocFormat.UAL_2018) {
        /* 
         * Initial div: 
         <div style=\"line-height:17px;font-size:13px\"><pre>
         * 
         * style defined for tables: as of 20240803
         * 
            <style type=\"text/css\">\n
            <!--\n
            .fwztable {border: 1px solid black; margin:0px; width:575px; width:610px; margin-top:-1px; border-collapse:collapse; }\n
            .fwztable td {border: 1px solid black; padding: 0px 5px}\n
            .fwztable .fwzright {text-align:right}\n
            -->\n
            </style>         
         
         */
        html = html.Replace(
          "<div style=\"line-height:17px;font-size:13px\">",
          "<div style=\"line-height:19px; font-size:14px; font-weight: 600;\">" );
        // update width of tables
        html = html.Replace( "width:610px;", "width:85%; white-space: pre; " );
        // TD style: padding: top, right, bottom, left / add white-space handling as the initial pre is not applied for TD elements
        html = html.Replace(
          ".fwztable td {border: 1px solid black; padding: 0px 5px}",
          ".fwztable td {border: 1px solid black; padding: 3px 5px 3px; font-size:16px; font-weight: 500; white-space: pre; }\n"
        + ".fwztable b {white-space: pre; }"
         );
        // inc image size 
        html = html.Replace( "width=\"600px\"></a>", "width=\"900px\"></a>" );
        return html;
      }

      // not yet handled plan types
      else {
        // guessed common styles and apply some mods
        html = html.Replace(
          "<div style=\"line-height:14px;font-size:12px\">",
          "<div style=\"line-height:20px;font-size:19px; font-weight: 600;\">" );
        html = html.Replace(
          "<div style=\"line-height:14px;font-size:13px\">",
          "<div style=\"line-height:20px;font-size:19px; font-weight: 600;\">" );
        html = html.Replace(
          "<div style=\"line-height:15px\">",
          "<div style=\"line-height:20px;font-size:19px; font-weight: 600;\">" );

        // inc image size 
        html = html.Replace( "width=\"600px\"></a>", "width=\"900px\"></a>" );
        return html;
      }
    }


    // detect which Flightplan format is in use from the original HTML code
    private HTMLdocFormat DetectDocFormat( string origHtml )
    {
      var docFormat = HTMLdocFormat.Unknown;
      if (string.IsNullOrWhiteSpace( origHtml )) return docFormat; // shortcut

      // try to figure out which format we are dealing with (crude string compare of the initial part)
      // use the longest to compare first..

      // SB docs start with a <div..><pre> section except UALs (as far as checked)
      if (origHtml.StartsWith(
        "<div style=\"line-height:15px\"><pre><!--BKMK///OFP///0--><!--BKMK///Summary///1--><!--BKMK///Fuel and Weights///1--><b>OFP" )) {
        docFormat = HTMLdocFormat.UAE;
      }
      if (origHtml.StartsWith(
        "<div style=\"line-height:14px;font-size:13px\"><pre><!--BKMK///OFP///0--><!--BKMK///Summary and Fuel///1--><b>[ OFP ]" )) {
        docFormat = HTMLdocFormat.EZY;
      }
      else if (origHtml.StartsWith(
        "<div style=\"line-height:14px;font-size:13px\"><pre><!--BKMK///OFP///0--><!--BKMK///Summary and Fuel///1--><b>OFP" )) {
        docFormat = HTMLdocFormat.RYR;
      }
      else if (origHtml.StartsWith(
        "\n<div style=\"line-height:14px;font-size:13px\"><pre><!--BKMK///OFP///0--><!--BKMK///Summary///1--><b>OFP" )) {
        docFormat = HTMLdocFormat.BAW;
      }
      else if (origHtml.StartsWith(
        "<div style=\"line-height:14px;font-size:13px\"><pre><!--BKMK///OFP///0--><!--BKMK///Fuel and Weights///1-->" )) {
        docFormat = HTMLdocFormat.AAL;
      }
      else if (origHtml.StartsWith(
        "<div style=\"line-height:15px\"><pre><!--BKMK///OFP///0--><!--BKMK///Summary///1-->" )) {
        docFormat = HTMLdocFormat.ACA;
      }
      else if (origHtml.StartsWith(
        "<div style=\"line-height:14px;font-size:13px\"><pre><!--BKMK///OFP///0--><b>--- OFP" )) {
        docFormat = HTMLdocFormat.DLH;
      }
      else if (origHtml.StartsWith(
        "<div style=\"line-height:14px;font-size:13px\"><pre><!--BKMK///OFP///0-->" )) {
        docFormat = HTMLdocFormat.LIDO;
      }
      else if (origHtml.StartsWith(
        "<div style=\"line-height:14px;font-size:12px\"><pre>" )) {
        docFormat = HTMLdocFormat.SWA;
      }

      else if (origHtml.Contains(
        "<div style=\"line-height:17px;font-size:13px\"><pre><!--START-->BRIEFING PACKAGE FOR FLIGHT" )) {
        // starts with a style section - so match with Contains...
        docFormat = HTMLdocFormat.UAL_2018;
      }
      return docFormat;
    }
  }
}

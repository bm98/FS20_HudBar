using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

using DbgLib;
using FlightplanLib;
using FlightplanLib.SimBrief;
using Windows.UI.Xaml.Controls.Primitives;

namespace FShelf.FPlans
{
  /// <summary>
  /// SimBrief Document Loader 
  ///  gets SB Docs into the designated directory (usually the Shelf Folder)
  /// </summary>
  internal class SbDocLoader
  {
    #region STATIC DEBUG
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );
    #endregion


    // Service
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

    // get one document
    private void LoadDocument( FileLink link, string targetFolder )
    {
      // Sanity
      if (link == null) return;
      if (string.IsNullOrWhiteSpace( link.RemoteUrl )) return;

      _simBrief.PostDownload_Request( link.DownloadUrl, link.FilenameLocal, targetFolder );
    }

    /// <summary>
    /// Load all supported document into the target folder
    /// </summary>
    /// <param name="ofp">A valid FlihtPlan</param>
    /// <param name="targetFolder">A valid destination directory</param>
    public bool LoadDocuments( FlightPlan plan, string targetFolder )
    {
      // Sanity
      if (plan == null) return false;
      if (!Directory.Exists( targetFolder )) return false;

      /* OMIT THE PDF FOR NOW
      // supported docs
      remotePath = ofp.Plan_Files.Directory;
      LoadDocument( remotePath, ofp.Plan_Files.Pdf_File, targetFolder );
      */

      // suported images
      foreach (var lPair in plan.ImageLinks) {
        LoadDocument( lPair, targetFolder );
      }

      // create the OFP text from the included HTML
      var limitedPlan = LimitOfpHtml(plan.HTMLdocument );
      // protect from inadvertend crashes of the unknown Library...
      Image image;
      try {
        image = TheArtOfDev.HtmlRenderer.WinForms.HtmlRender.RenderToImage( limitedPlan );
      }
      catch (Exception ex) {
        LOG.LogError( "LoadDocuments", $"Converting to HTML failed:\n{ex}" );
        return false;
      }
      // dest may be locked when viewing
      try {
        image.Save( Path.Combine( targetFolder, "@.FlightPlan.png" ), ImageFormat.Png );
      }
      catch (Exception ex) {
        Console.WriteLine( $"LoadDocuments: Saving to file failed:\n{ex}" );
        return false;
      }
      return true;
    }

    // remove from NOTAM onwards
    private string LimitOfpHtml( string origHtml )
    {
      // sanity
      if (string.IsNullOrWhiteSpace( origHtml )) return "<div style=\"padding:10px\"><pre><h1>NO PLAN AVAILABLE !</pre></div>";

      // doc starts with
      // <div style=\"line-height:14px;font-size:13px\"><pre>
      // and must end with </pre></div> therefore when cutting to end
      // NOTAM section starts with the bookmark used below (as of 20230124...)

      // get a larger text size for our rendering
      var html = origHtml.Replace( "<pre>", "<pre style=\"line-height:20px;font-size:19px\"" );

      int notamStart = html.IndexOf( "<!--BKMK///NOTAM///0-->", 0, StringComparison.InvariantCultureIgnoreCase );
      if (notamStart > 0) {
        var limited = "<div style=\"padding:12px;\">"; // add padding around the created doc image
        limited += html.Substring( 0, notamStart );
        limited += "</pre></div>"; // doc closure from removed parts
        limited += "</div>"; // doc closure from our padding
        return limited;
      }
      else {
        return html;
      }
    }

  }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

using DbgLib;

using dNetWkhtmlWrap;

namespace bm98_Html
{
  /// <summary>
  /// Uses application wrapper of the WkHtmlToX Library
  /// 
  /// Singleton Wrapper around the HTML Renderer
  ///  the new Html Lib would not need to serialize jobs - but we leave it in for the moment
  ///  does not really harm as we don't convert often anyway
  /// 
  /// </summary>
  public sealed class HtmlRenderer
  {
    #region STATIC DEBUG
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );
    #endregion


    // Singleton Pattern
    public static HtmlRenderer Instance => lazy.Value;
    private static readonly Lazy<HtmlRenderer> lazy = new Lazy<HtmlRenderer>( ( ) => new HtmlRenderer( ) );

    // EXE deployment is static into a known Temp location
    private IDeployment s_Deployment = null;

    // ctor remains private
    private HtmlRenderer( )
    {
      // init static instance of the converter
      try {
        s_Deployment =
        new WinEDeploymentPdfExe(
          new StaticDeployment( bm98_hbFolders.Folders.UserTempPath ),
          true // Force SHA256 verification - may throw InvalidOperation if sha fails when accessing the wrapper
        ); // PDF only

        // start the converter serializer thread
        InitQueueWorker( );
      }
      catch (Exception ex) {
        s_Deployment = null;

        LOG.Error( "HtmlRenderer.InitConverter", ex, "Init Converters failed" );
      }
    }


    #region Serialized Converter Queue

    // flag
    private bool _converterRunning = false;

    // work Item to queue and process
    private struct WorkObject
    {
      public string htmlCode;
      public string targetFile;
      public string docTitle;
      public string header;
      public Func<string, string, string, string, bool> Job;
      // ctor
      public WorkObject( string h, string f, string t, string hl, Func<string, string, string, string, bool> j )
      {
        htmlCode = h;
        targetFile = f;
        docTitle = t;
        header = hl;
        Job = j;
      }
    }

    // the work item queue
    private BlockingCollection<WorkObject> _workItems = new BlockingCollection<WorkObject>( );

    // initialize the consumer task
    private void InitQueueWorker( )
    {
      try {
        // one task will support the non reentrant StandardConverter
        Task.Factory.StartNew( Consume );
        _converterRunning = true;
      }
      catch {
        _converterRunning = false;
      }
    }

    // Consumer Method
    private void Consume( )
    {
      // This sequence that we’re enumerating will block when no elements
      // are available and will end when CompleteAdding is called. 
      // which is not called as this is a Singleton for the App Lifetime
      foreach (WorkObject workItem in _workItems.GetConsumingEnumerable( )) {
        // never bail out from here
        try {
          bool result = workItem.Job(
            workItem.htmlCode,
            workItem.targetFile,
            workItem.docTitle, workItem.header );     // Perform task.

          if (!result) {
            LOG.Error( "ConverterTask", $"Error failed for: {workItem.Job.ToString( )}" );
          }
        }
        catch (Exception ex) {
          LOG.Error( "ConverterTask", ex, $"Error failed for: {workItem.Job.ToString( )}" );
        }
      }
    }

    // add a new item
    private bool EnqueWorkItem( WorkObject workItem ) => _workItems.TryAdd( workItem );

    #endregion

    #region API  

    /// <summary>
    /// Add a job to convert htmlCode to a A4 PDF File
    /// </summary>
    /// <param name="htmlCode">HTML plain code to convert</param>
    /// <param name="targetFile">PDF out file</param>
    /// <param name="docTitle">A Document Title</param>
    /// <param name="header">A header line</param>
    public bool Add_ToPdf_A4_Portrait( string htmlCode, string targetFile, string docTitle, string header )
    {
      // sanity
      if (!_converterRunning) return false;

      return EnqueWorkItem( new WorkObject( htmlCode, targetFile, docTitle, header, Job_Html2PDF_A4_Portrait ) );
    }

    /// <summary>
    /// Add a job to convert htmlCode to a A4 Landscape oriented PDF File
    /// </summary>
    /// <param name="htmlCode">HTML plain code to convert</param>
    /// <param name="targetFile">PDF out file</param>
    /// <param name="docTitle">A Document Title</param>
    /// <param name="header">A header line</param>
    public bool Add_ToPdf_A4_Landscape( string htmlCode, string targetFile, string docTitle, string header )
    {
      // sanity
      if (!_converterRunning) return false;

      return EnqueWorkItem( new WorkObject( htmlCode, targetFile, docTitle, header, Job_Html2PDF_A4_Landscape ) );
    }

    /// <summary>
    /// Add a job to convert htmlCode to a A3 PDF File
    /// </summary>
    /// <param name="htmlCode">HTML plain code to convert</param>
    /// <param name="targetFile">PDF out file</param>
    /// <param name="docTitle">A Document Title</param>
    /// <param name="header">A header line</param>
    public bool Add_ToPdf_A3_Portrait( string htmlCode, string targetFile, string docTitle, string header )
    {
      // sanity
      if (!_converterRunning) return false;

      return EnqueWorkItem( new WorkObject( htmlCode, targetFile, docTitle, header, Job_Html2PDF_A3_Portrait ) );
    }

    #endregion

    #region Jobs

    // will run the converter 
    private bool RunConverter( IDocument document )
    {
      // protect from inadvertend crashes of the Library...
      try {
        var wrapper = WkWrapperFactory.Create( s_Deployment );
        bool result = wrapper.Convert( document, out string outFile );
        // will convert and save to out location
        return result;
      }
      catch (Exception ex) {
        // may fail for missing exe files or SHA256 verification 
        LOG.Error( "Html2PDF.Convert", ex, "Rendering or saving to PDF failed" );
        return false;
      }
    }

    // basic document settings for all jobs
    private HtmlToPdfDocument BasicDocument( string html, string title, string header )
    {
      var doc = new HtmlToPdfDocument( html ) {
        GlobalSettings = {
          ProduceOutline = false,
          Margins = {
            Top=15,
            Left=8,
            Right=8,
            Bottom=12,
            Unit = Unit.Millimeters

          },
      }
      };
      if (!string.IsNullOrWhiteSpace( title )) doc.GlobalSettings.DocumentTitle = title;
      if (!string.IsNullOrWhiteSpace( header )) {
        doc.Objects[0].HeaderSettings.LeftText = "[date]";
        doc.Objects[0].HeaderSettings.CenterText = header;
        doc.Objects[0].HeaderSettings.RightText = "Page [page]";
        doc.Objects[0].HeaderSettings.FontName = "Arial";
        doc.Objects[0].HeaderSettings.FontSize = 12.0;
        doc.Objects[0].HeaderSettings.ContentSpacing = 6.0;
      }
      doc.Objects[0].FooterSettings.CenterText = "- Not for real world navigation -";
      doc.Objects[0].HeaderSettings.FontName = "Arial";
      doc.Objects[0].FooterSettings.FontSize = 8.0;
      doc.Objects[0].FooterSettings.ContentSpacing = 6.0;
      //
      return doc;
    }


    /// <summary>
    /// Converts a string of HTML code to PDF and saves it as File
    ///   Target directory must exist
    ///   
    ///   Pagesetup is: A4 Portrait 96dpi with 0.8cm margin all around
    ///   
    /// </summary>
    /// <param name="htmlCode">HTML code</param>
    /// <param name="targetFile">File to save to</param>
    /// <param name="docTitle">A document title</param>
    /// <param name="header">A header line</param>
    /// <returns>True when successfull</returns>
    private bool Job_Html2PDF_A4_Portrait( string htmlCode, string targetFile, string docTitle, string header )
    {
      // sanity
      if (s_Deployment == null) return false;
      if (string.IsNullOrEmpty( htmlCode )) return false;
      if (string.IsNullOrEmpty( targetFile )) return false;
      if (!Directory.Exists( Path.GetDirectoryName( targetFile ) )) return false;

      // some chars the library wants to have as proper HTML
      htmlCode = htmlCode.Replace( "'", "&apos;" );

      // Setup
      var document = BasicDocument( htmlCode, docTitle, header );
      document.GlobalSettings.OutputFile = targetFile;
      document.GlobalSettings.PaperSize = PaperKind.A4;
      document.GlobalSettings.Orientation = GlobalSettings.PaperOrientation.Portrait;

      return RunConverter( document );
    }

    /// <summary>
    /// Converts a string of HTML code to PDF and saves it as File
    ///   Target directory must exist
    ///   
    ///   Pagesetup is: A4 Portrait 96dpi
    ///   
    /// </summary>
    /// <param name="htmlCode">HTML code</param>
    /// <param name="targetFile">File to save to</param>
    /// <param name="docTitle">A document title</param>
    /// <param name="header">A header line</param>
    /// <returns>True when successfull</returns>
    private bool Job_Html2PDF_A4_Landscape( string htmlCode, string targetFile, string docTitle, string header )
    {
      // sanity
      if (s_Deployment == null) return false;
      if (string.IsNullOrEmpty( htmlCode )) return false;
      if (string.IsNullOrEmpty( targetFile )) return false;
      if (!Directory.Exists( Path.GetDirectoryName( targetFile ) )) return false;

      // some chars the library wants to have as proper HTML
      htmlCode = htmlCode.Replace( "'", "&apos;" );

      // Setup
      var document = BasicDocument( htmlCode, docTitle, header );
      document.GlobalSettings.DocumentTitle = docTitle;
      document.GlobalSettings.OutputFile = targetFile;
      document.GlobalSettings.PaperSize = PaperKind.A4;
      document.GlobalSettings.Orientation = GlobalSettings.PaperOrientation.Landscape;

      return RunConverter( document );
    }

    /// <summary>
    /// Converts a string of HTML code to PDF and saves it as File
    ///   Target directory must exist
    ///   
    ///   Pagesetup is: A3 Portrait 96dpi
    ///   
    /// </summary>
    /// <param name="htmlCode">HTML code</param>
    /// <param name="targetFile">File to save to</param>
    /// <param name="docTitle">A document title</param>
    /// <param name="header">A header line</param>
    /// <param name="footer">A footer line</param>
    /// <returns>True when successfull</returns>
    private bool Job_Html2PDF_A3_Portrait( string htmlCode, string targetFile, string docTitle, string header )
    {
      // sanity
      if (s_Deployment == null) return false;
      if (string.IsNullOrEmpty( htmlCode )) return false;
      if (string.IsNullOrEmpty( targetFile )) return false;
      if (!Directory.Exists( Path.GetDirectoryName( targetFile ) )) return false;

      // some chars the library wants to have as proper HTML
      htmlCode = htmlCode.Replace( "'", "&apos;" );

      // Setup
      var document = BasicDocument( htmlCode, docTitle, header );
      document.GlobalSettings.DocumentTitle = docTitle;
      document.GlobalSettings.OutputFile = targetFile;
      document.GlobalSettings.PaperSize = PaperKind.A3;
      document.GlobalSettings.Orientation = GlobalSettings.PaperOrientation.Landscape;

      return RunConverter( document );
    }

    #endregion

  }
}

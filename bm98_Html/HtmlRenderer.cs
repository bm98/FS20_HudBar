using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DbgLib;

using TuesPechkin;

namespace bm98_Html
{
  /// <summary>
  /// Uses x64 Library version of the WkHtmlToX Library - must be x64 configured
  /// Users must configure x64 therefore
  /// 
  /// Singleton Wrapper around the HTML Renderer
  /// 
  /// NOTE:  Only use StandardConverter and maintain it during the Prog Lifecycle
  ///        everything else will cause issues when trying to unload the underlying Wkhtmltox.dll. 
  ///        Mostly it just hangs in a tight FreeLibrary loop which never succeeds
  ///        Most likely due to statically linked OpenSSL library (Google search ... cannot unload DLL)
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

    // DLL deployment is static into a known Temp location
    private IDeployment s_Dll = null;
    // single instance of the Converter for PDF and IMG
    private IConverter s_TuesPechkinPDFConverter = null;
    private IConverter s_TuesPechkinIMGConverter = null;
    private object _converterLock = new object( );

    // ctor remains private
    private HtmlRenderer( )
    {
      // init static instance of the converter
      try {
        s_Dll =
          new Win64EmbeddedDeployment( new StaticDeployment( bm98_hbFolders.Folders.UserTempPath ) );
        // new Win64EmbeddedDeployment( new TempFolderDeployment( ) );

        // using 2 standard converters as ThreadSave seems to create/use competing resources
        // if needed serialize the calls in this class per converter
        s_TuesPechkinPDFConverter =
          new StandardConverter( new PdfToolset( s_Dll ) );
        s_TuesPechkinIMGConverter =
          new StandardConverter( new ImageToolset( s_Dll ) );

      }
      catch (Exception ex) {
        s_Dll = null;
        s_TuesPechkinPDFConverter = null;
        s_TuesPechkinIMGConverter = null;
        LOG.LogException( "HtmlRenderer.InitConverter", ex, "Init Converters failed" );
      }
    }

    /// <summary>
    /// The PDF Converter
    /// </summary>
    public IConverter Converter_PDF => s_TuesPechkinPDFConverter;
    /// <summary>
    /// The Image Converter
    /// </summary>
    public IConverter Converter_IMG => s_TuesPechkinIMGConverter;


    // Utilities

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
    /// <returns>True when successfull</returns>
    public bool Html2PDF( string htmlCode, string targetFile, string docTitle )
    {
      // sanity
      if (s_TuesPechkinPDFConverter == null) return false;
      if (string.IsNullOrEmpty( htmlCode )) return false;
      if (string.IsNullOrEmpty( targetFile )) return false;
      if (!Directory.Exists( Path.GetDirectoryName( targetFile ) )) return false;

      // some chars the library wants to have as proper HTML
      htmlCode = htmlCode.Replace( "'", "&apos;" );

      // Setup
      var document = new HtmlToPdfDocument {
        GlobalSettings = {
              ProduceOutline = false,
              DocumentTitle = docTitle,
              DPI = 96,
              PaperSize = PaperKind.A4, // Implicit conversion to PechkinPaperSize
              Orientation = GlobalSettings.PaperOrientation.Portrait,
              OutputFile = targetFile, // direct output
              Margins = {
                All = 0.8,
                Unit = Unit.Centimeters
              },
            },
        Objects = {
              new ObjectSettings { HtmlText =  htmlCode }, // use plain HTML code
            }
      };

      // protect from inadvertend crashes of the unknown Library...
      try {
        lock (_converterLock) {
          s_TuesPechkinPDFConverter?.Convert( document ); // will convert and save to out location
        }
        return true;
      }
      catch (Exception ex) {
        LOG.LogException( "Html2PDF.Convert", ex, "Rendering or saving to PDF failed" );
        return false;
      }

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
    /// <returns>True when successfull</returns>
    public bool Html2PDF_A3( string htmlCode, string targetFile, string docTitle )
    {
      // sanity
      if (s_TuesPechkinPDFConverter == null) return false;
      if (string.IsNullOrEmpty( htmlCode )) return false;
      if (string.IsNullOrEmpty( targetFile )) return false;
      if (!Directory.Exists( Path.GetDirectoryName( targetFile ) )) return false;

      // some chars the library wants to have as proper HTML
      htmlCode = htmlCode.Replace( "'", "&apos;" );

      // Setup
      var document = new HtmlToPdfDocument {
        GlobalSettings = {
              ProduceOutline = false,
              DocumentTitle = docTitle,
              DPI = 96,
              PaperSize = PaperKind.A3, // Implicit conversion to PechkinPaperSize
              Orientation = GlobalSettings.PaperOrientation.Portrait,
              OutputFile = targetFile, // direct output
              Margins = {
                All = 0.8,
                Unit = Unit.Centimeters
              },
            },
        Objects = {
              new ObjectSettings { HtmlText =  htmlCode }, // use plain HTML code
            }
      };

      // protect from inadvertend crashes of the unknown Library...
      try {
        lock (_converterLock) {
          s_TuesPechkinPDFConverter?.Convert( document ); // will convert and save to out location
        }
        return true;
      }
      catch (Exception ex) {
        LOG.LogException( "Html2PDF.Convert", ex, "Rendering or saving to PDF failed" );
        return false;
      }

    }

    /// <summary>
    /// Converts a string of HTML code to a PNG image and saves it as File
    ///   Target directory must exist
    ///   
    /// </summary>
    /// <param name="htmlCode">HTML code</param>
    /// <param name="targetFile">File to save to</param>
    /// <returns>True when successfull</returns>
    public bool Html2PNG( string htmlCode, string targetFile )
    {
      // sanity
      if (s_TuesPechkinIMGConverter == null) return false;
      if (string.IsNullOrEmpty( htmlCode )) return false;
      if (string.IsNullOrEmpty( targetFile )) return false;
      if (!Directory.Exists( Path.GetDirectoryName( targetFile ) )) return false;

      // need a temp file for image
      string htmlFile = TmpHtmlFile( htmlCode );
      if (string.IsNullOrEmpty( htmlFile )) return false;

      // Setup
      var document = new HtmlToImageDocument {
        Format = "png",
        In = htmlFile,
        Out = targetFile,
      };

      // protect from inadvertend crashes of the unknown Library...
      try {
        lock (_converterLock) {
          s_TuesPechkinIMGConverter?.Convert( document ); // will convert and save to out location
        }
        return true;
      }
      catch (Exception ex) {
        LOG.LogException( "Html2PNG.Convert", ex, "Rendering or saving to PNG failed" );
        return false;
      }
      finally {
        File.Delete( htmlFile );
      }
    }

    /// <summary>
    /// Converts a string of HTML code to a JPG image and saves it as File
    ///   Target directory must exist
    ///   
    /// </summary>
    /// <param name="htmlCode">HTML code</param>
    /// <param name="targetFile">File to save to</param>
    /// <returns>True when successfull</returns>
    public bool Html2JPG( string htmlCode, string targetFile )
    {
      // sanity
      if (s_TuesPechkinIMGConverter == null) return false;
      if (string.IsNullOrEmpty( htmlCode )) return false;
      if (string.IsNullOrEmpty( targetFile )) return false;
      if (!Directory.Exists( Path.GetDirectoryName( targetFile ) )) return false;

      // need a temp file for image
      string htmlFile = TmpHtmlFile( htmlCode );
      if (string.IsNullOrEmpty( htmlFile )) return false;

      // Setup
      var document = new HtmlToImageDocument {
        Format = "jpg",
        Quality = 75,
        In = htmlFile,
        Out = targetFile,
      };

      // protect from inadvertend crashes of the unknown Library...
      try {
        lock (_converterLock) {
          s_TuesPechkinIMGConverter?.Convert( document ); // will convert and save to out location
        }
        return true;
      }
      catch (Exception ex) {
        LOG.LogException( "Html2JPG.Convert", ex, "Rendering or saving to JPG failed" );
        return false;
      }
      finally {
        File.Delete( htmlFile );
      }
    }

    // must create a temp file for Image conversion
    private string TmpHtmlFile( string htmlCode )
    {
      string fname = $"{new Guid( ):B}.html"; // {nnnn-nnnn...}  format
      string fn = $"{Path.Combine( bm98_hbFolders.Folders.UserTempPath, fname )}";
      try {
        using (StreamWriter sw = File.CreateText( fn )) {
          sw.WriteLine( htmlCode );
          // must be written when returning
          sw.Flush( );
          sw.Close( );
        }
      }
      catch {
        fn = "";
      }
      return fn;
    }



  }
}

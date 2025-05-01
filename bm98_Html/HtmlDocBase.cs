using System;

namespace bm98_Html
{
  /// <summary>
  /// Generic HTML document
  /// </summary>
  public class HtmlDocument
  {
    /*
      HTML Format is like

      <!DOCTYPE html>
      <html>
        <head>
          <meta http-equiv="Content-Type" content="text/html;charset=UTF-8"> 

          <style>
          table {
            font-family: arial, sans-serif;
            border-collapse: collapse;
            width: 100%;
          }

          td, th {
            border: 1px solid #dddddd;
            text-align: left;
            padding: 8px;
          }

          tr:nth-child(even) {
            background-color: #dddddd;
          }
          </style>

        </head>

        <body>

        <h2>HTML Table</h2>

        <table>
          <tr>
            <th>Company</th>
            <th>Contact</th>
            <th>Country</th>
          </tr>
          ....
          <tr>
            <td>Island Trading</td>
            <td>Helen Bennett</td>
            <td>UK</td>
          </tr>
        </table>

        </body>
      </html>


     */

    /// <summary>
    /// Replacement Mark for the HEAD section
    /// </summary>
    protected const string MARK_HEAD = "+§+HEAD+§+";
    /// <summary>
    /// Replacement Mark for the BODY section
    /// </summary>
    protected const string MARK_BODY = "+§+BODY+§+";
    /// <summary>
    /// Replacement Mark for the META section
    /// </summary>
    protected const string MARK_META = "+§+META+§+";
    /// <summary>
    /// Replacement Mark for the STYLE section
    /// </summary>
    protected const string MARK_STYLE = "+§+STYLE+§+";
    /// <summary>
    /// Replacement Mark for optional HEAD content 
    /// </summary>
    protected const string MARK_HEAD_OPT = "+§+HEAD_OPT+§+";
    /// <summary>
    /// Replacement Mark for the BODY section
    /// </summary>
    protected const string MARK_BODY_CONTENT = "+§+BODY_CONTENT+§+";

    // internal templates
    private readonly string _htmlDoc = $"<!DOCTYPE html>\n<html>\n{MARK_HEAD}\n{MARK_BODY}\n</html>\n";
    private readonly string _htmlHead = $"<head>\n{MARK_META}\n{MARK_STYLE}\n{MARK_HEAD_OPT}\n</head>";
    private readonly string _htmlBody = $"<body>\n{MARK_BODY_CONTENT}\n</body>";

    /// <summary>
    /// Generic Standard UTF0 Text meta string, no <meta> tag, only the content
    /// </summary>
    protected string _metaUTF8 = @"http-equiv=""Content-Type"" content=""text/html;charset=UTF-8""";

    /// <summary>
    /// Provided Style Content,  no <style> tag, just content
    /// </summary>
    protected string _style = ""; // no <style> tag, just content
    /// <summary>
    /// Provided additional Head content, must include tags e.g. "<meta>something</meta>"
    ///  will be included into the head section as is
    /// </summary>
    protected string _headOpt = "";
    /// <summary>
    /// Provided doc content, no <body> tag, just content
    ///  will be included into the body section as is
    /// </summary>
    protected string _body = "";

    /// <summary>
    /// Returns the <HEAD> section by replacing marks with content
    /// </summary>
    protected string GetHeadSection( )
    {
      var html = _htmlHead;
      // UTF8 text content Meta line
      html = html.Replace( MARK_META, $"<meta {_metaUTF8} >" );
      // Style 
      html = html.Replace( MARK_STYLE, $"<style>\n{_style}\n</style>" );
      // More Head
      html = html.Replace( MARK_HEAD_OPT, _headOpt );

      return html;
    }
    /// <summary>
    /// Returns the <BODY> section by replacing marks with content
    /// </summary>
    protected string GetBodySection( )
    {
      var html = _htmlBody;
      html = html.Replace( MARK_BODY_CONTENT, _body );

      return html;
    }
    /// <summary>
    /// Returns the complete Html Doc by replacing marks with content
    /// </summary>
    protected string GetDocumentSection( )
    {
      var html = _htmlDoc;
      html = html.Replace( MARK_HEAD, GetHeadSection( ) );
      html = html.Replace( MARK_BODY, GetBodySection( ) );

      return html;
    }


    /// <summary>
    /// Add more Style content
    ///  NO CHECKS must be valid HTML Style content
    /// </summary>
    /// <param name="html">Additional STYLE Html</param>
    public void AddStyleHtml( string html )
    {
      _style += html;
    }
    /// <summary>
    /// Add more HEAD content
    ///  NO CHECKS must be valid HTML head sub sections
    /// </summary>
    /// <param name="html">Additional HEAD Html</param>
    public void AddHeadHtml( string html )
    {
      _headOpt += html;
    }

    /// <summary>
    /// Add more Body content
    ///  NO CHECKS must be valid HTML Body content
    /// </summary>
    /// <param name="html">Additional BODY Html</param>
    public void AddBodyHtml( string html )
    {
      _body += html;
    }

    /// <summary>
    /// Returns the HTML document as string
    /// </summary>
    /// <returns>A string from content</returns>
    public string GetDocument( ) => GetDocumentSection( );



  }
}

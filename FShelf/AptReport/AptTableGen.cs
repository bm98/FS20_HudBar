using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FShelf.AptReport
{
  /// <summary>
  /// Create a HTML Document for an Airport Overview in table format
  /// </summary>
  internal class AptTableGen
  {
    /*
     Formatted as:
      IDENT, ALT, TYPE, PROC, MTRK, DIST 

      HTML Format is like
      <!DOCTYPE html>
      <html>
      <head>
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

    // About the width of the Document set to 800px
    // it matches the A3 format when rendering the HTML to PDF with the given margins

    // The last col is set to a size and the ones before should be aligned by the renderer
    // based on content (the last col is set to width only once in the first table header in HeaderRow_html())
    // some properties do not work when set in this CSS, they are applied as style in HTML

    private const string c_docStyle = @"
<style>
  .bgWhite{
    background-color: #ffffff;
  }
  .bgAlt{
    background-color: #dddddd;
  }
  .bgGrey{
    background-color: #f0f0f0;
  }
  .bgIce{
    background-color: #f0ffff;
  }
  .fBold{
    font-weight: bold;
  }
  .wSmall{
    width:3em;
  }
  body {
    font-family: ""segoe ui"", arial, sans-serif;
  }
  table {
    font-family: arial, sans-serif;
    border-collapse: collapse;
    width: 100%;
  }
  th.frame {
    font-family: arial, sans-serif; font-weight: bold;
    background-color: #f0ffff;
    border: 1px solid #dddddd;
    text-align: left;
    padding: 5px;
  }
  td.frame {
    font-family: Calibri, ""lucida sans unicode"", ""arial unicode ms"", arial, sans-serif;
    border: 1px solid #dddddd;
    text-align: left;
    padding: 3px;
  }
  td.noframe {
    font-family: Calibri,""lucida sans unicode"", ""arial unicode ms"", arial, sans-serif;
    border: none;
    text-align: left;
    padding: 1px;
  }
}
</style>";

    // string replacement mark
    private const string c_MARK = "$$$+++$$$";
    // HTML templates 
    private readonly string t_doc;
    private readonly string t_title;
    private readonly string t_stitle;
    private readonly string t_timestamp;
    private readonly string t_rwyTable;
    private readonly string t_sidTable;
    private readonly string t_starTable;
    private readonly string t_commTable;
    private readonly string t_navaidTable;

    private string _document;
    private string _rwyRows;
    private string _sidRows;
    private string _starRows;
    private string _commRows;
    private string _navaidRows;
    private int _rowCount = 0;

    /// <summary>
    /// cTor:
    /// </summary>
    public AptTableGen( )
    {
      // rule is to leave the template with with \n where needed \n will be read by the Renderer as CR(LF)
      var docHead = $"<head>\n{c_docStyle}\n<meta charset=\"UTF-8\">\n</head>";
      var docBody = $"<body>\n{c_MARK}\n</body>";
      t_doc = $"<!DOCTYPE html>\n<html>\n{docHead}\n{docBody}\n</html>\n"; // using replacement mark in docBody

      t_timestamp = $"<h6>{c_MARK}</h6>\n";
      t_title = $"<h2>{c_MARK}</h2>\n";
      t_stitle = $"<h3>{c_MARK}</h3>\n";
      t_rwyTable = $"<h3>Runways</h3>\n<table>\n{c_MARK}\n</table>\n";
      t_sidTable = $"<h3>SID Procedures</h3>\n<table>\n{c_MARK}\n</table>\n";
      t_starTable = $"<h3>STAR Procedures</h3>\n<table>\n{c_MARK}\n</table>\n";
      t_commTable = $"<h3>Airport Communication</h3>\n<table>\n{c_MARK}\n</table>\n";
      t_navaidTable = $"<h3>Navaids in Range</h3>\n<table>\n{c_MARK}\n</table>\n";

      _document = "";
      _rwyRows = RwyRowData.HeaderRow_html( );
      _sidRows = RwyProcRowData.HeaderRow_html( "SID" );
      _starRows = RwyProcRowData.HeaderRow_html( "STAR" );
      _commRows = CommRowData.HeaderRow_html( );
      _navaidRows = NavaidRowData.HeaderRow_html( );
      _rowCount = 0;
    }


    /// <summary>
    /// Commit the document 
    ///  then reset the formatter
    /// </summary>
    /// <returns>The document as HTML</returns>
    public string CommitDocument( string titleContent, List<string> subTitles )
    {
      //  finalize by replacing the marks in the templates
      var body = t_timestamp.Replace( c_MARK, DateTime.Now.ToString( "g" ) );
      body += t_title.Replace( c_MARK, titleContent );
      foreach (var subTitle in subTitles) {
        if (string.IsNullOrWhiteSpace( subTitle )) continue; // ignore empty subtitle lines

        body += t_stitle.Replace( c_MARK, subTitle );
      }
      // create the body
      body += t_rwyTable.Replace( c_MARK, _rwyRows );
      body += t_sidTable.Replace( c_MARK, _sidRows );
      body += t_starTable.Replace( c_MARK, _starRows );
      body += t_commTable.Replace( c_MARK, _commRows );
      body += t_navaidTable.Replace( c_MARK, _navaidRows );
      // complete the document
      _document = t_doc.Replace( c_MARK, body );

      // copy
      var doc = _document;
      // reset
      _document = "";
      _rwyRows = RwyRowData.HeaderRow_html( );
      _sidRows = RwyProcRowData.HeaderRow_html( "SID" );
      _starRows = RwyProcRowData.HeaderRow_html( "STAR" );
      _commRows = CommRowData.HeaderRow_html( );
      _navaidRows = NavaidRowData.HeaderRow_html( );
      _rowCount = 0;

      // and return the created document
      return doc;
    }

    /// <summary>
    /// Reset the 'Zebra' outliner (starts with Odd)
    /// </summary>
    public void ResetZebra()=> _rowCount = 0;

    /// <summary>
    /// Add a Runway Row
    /// </summary>
    public void AddRwyRow( RwyRowData rowData ) => _rwyRows += rowData.Row_html( ++_rowCount );
    /// <summary>
    /// Add an Approach Row
    /// </summary>
    public void AddAprRow( AprRowData rowData ) => _rwyRows += rowData.Row_html( ++_rowCount );

    /// <summary>
    /// Add a Runway Proc Row
    /// </summary>
    public void AddRwySidRow( RwyProcRowData rowData ) => _sidRows += rowData.RwyRow_html( ++_rowCount );
    /// <summary>
    /// Add a Proc Row for a Runway
    /// </summary>
    public void AddSidRow( RwyProcRowData rowData ) => _sidRows += rowData.ProcRow_html( ++_rowCount );

    /// <summary>
    /// Add a Runway Proc Row
    /// </summary>
    public void AddRwyStarRow( RwyProcRowData rowData ) => _starRows += rowData.RwyRow_html( ++_rowCount );
    /// <summary>
    /// Add a Proc Row for a Runway
    /// </summary>
    public void AddStarRow( RwyProcRowData rowData ) => _starRows += rowData.ProcRow_html( ++_rowCount );


    /// <summary>
    /// Add an Communication Row
    /// </summary>
    public void AddCommRow( CommRowData rowData ) => _commRows += rowData.Row_html( ++_rowCount );

    /// <summary>
    /// Add a Navaid Row
    /// </summary>
    public void AddNavaidRow( NavaidRowData rowData ) => _navaidRows += rowData.Row_html( ++_rowCount );

    #region HTML codes for special chars

    /// <summary>
    /// Special Characters for HTML documents
    /// </summary>
    public enum SpclChar
    {
      Arrow_W = 0,
      Arrow_N,
      Arrow_E,
      Arrow_S,
      Arrow_NW,
      Arrow_NE,
      Arrow_SE,
      Arrow_SW,
      BULLSEYE, // circle with dot
    }
    // code list, index matches the SpclChar Enum value !!
    private static readonly string[] _sChars = new string[] {
      "&#x2190;", "&#x2191;", "&#x2192;", "&#x2193;",
      "&#x2196;", "&#x2197;", "&#x2198;", "&#x2199;",
      "&#x25CE;", "", "", "", "", "", "", "", "", "", };

    /// <summary>
    /// Returns the HTML code for a special Char
    /// (beware no validity checks for the argument)
    /// </summary>
    /// <param name="ch">A SpclChar</param>
    /// <returns>A HTML code</returns>
    public static string HChar( SpclChar ch ) => _sChars[(int)ch];

    #endregion

  }
}

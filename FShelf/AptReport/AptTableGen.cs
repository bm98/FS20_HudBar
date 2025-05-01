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

    private const string c_docStyle =
@"
  body { font-family: ""segoe ui"", arial, sans-serif; }
  table {
    font-family: arial, sans-serif;
    border-collapse: collapse;
    width: 100%;
  }
  th.frame {
    font-family: arial, sans-serif; font-weight: bold;
    background-color: #f0ffff;
    border: 1px solid #cccccc;
    text-align: left;
    padding: 5px;
  }
  td.frame {
    font-family: Calibri, ""lucida sans unicode"", ""arial unicode ms"", sans-serif;
    border: 1px solid #cccccc;
    text-align: left;
    padding: 3px;
  }
  td.noframe {
    font-family: Calibri,""lucida sans unicode"", ""arial unicode ms"", sans-serif;
    border: none;
    text-align: left;
    padding: 1px;
  }
  .bgWhite{ background-color: #ffffff; }
  .bgAlt{ background-color: #dddddd; }
  .bgGrey{ background-color: #f0f0f0; }
  .bgIce{ background-color: #f0ffff; }
  .fBold{ font-weight: bold; }
  .wSmall{ width:3em; }
  .wRwyCol_1{ width:6em; }
}
";

    // string replacement mark
    private const string c_MARK = "$$$+++$$$";
    // HTML templates 
    private readonly string t_title;
    private readonly string t_stitle;
    private readonly string t_timestamp;
    private readonly string t_rwyTable;
    private readonly string t_sidTable;
    private readonly string t_starTable;
    private readonly string t_commTable;
    private readonly string t_navaidTable;

    private bm98_Html.HtmlDocument _htmlDoc;

    private StringBuilder _rwyRows;
    private StringBuilder _sidRows;
    private StringBuilder _starRows;
    private StringBuilder _commRows;
    private StringBuilder _navaidRows;
    private int _rowCount = 0;

    // init to restart
    private void InitDocGen( )
    {
      _htmlDoc = new bm98_Html.HtmlDocument( );
      _rwyRows = new StringBuilder( );
      _sidRows = new StringBuilder( );
      _starRows = new StringBuilder( );
      _commRows = new StringBuilder( );
      _navaidRows = new StringBuilder( );

      _htmlDoc.AddStyleHtml( c_docStyle );
      _rwyRows.AppendLine( RwyRowData.HeaderRow_html( ) );
      _sidRows.AppendLine( RwyProcRowData.HeaderRow_html( "SID" ) );
      _starRows.AppendLine( RwyProcRowData.HeaderRow_html( "STAR" ) );
      _commRows.AppendLine( CommRowData.HeaderRow_html( ) );
      _navaidRows.AppendLine( NavaidRowData.HeaderRow_html( ) );
      _rowCount = 0;
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public AptTableGen( )
    {
      // create used templates
      // rule is to leave the template with with \n where needed \n will be read by the Renderer as CR(LF)
      t_timestamp = $"<h6>HudBar-FlightBag create date: {c_MARK}</h6>\n";
      t_title = $"<h2>{c_MARK}</h2>\n";
      t_stitle = $"<h3>{c_MARK}</h3>\n";
      t_rwyTable = $"<h3>Runways</h3>\n<table>\n{c_MARK}\n</table>\n";
      t_sidTable = $"<h3>SID Procedures</h3>\n<table>\n{c_MARK}\n</table>\n";
      t_starTable = $"<h3>STAR Procedures</h3>\n<table>\n{c_MARK}\n</table>\n";
      t_commTable = $"<h3>Airport Communication</h3>\n<table>\n{c_MARK}\n</table>\n";
      t_navaidTable = $"<h3>Navaids in Range</h3>\n<table>\n{c_MARK}\n</table>\n";

      InitDocGen( );
    }


    /// <summary>
    /// Commit the document 
    ///  then reset the formatter
    /// </summary>
    /// <returns>The document as HTML</returns>
    public string CommitDocument( string titleContent, List<string> subTitles, string dbSource )
    {
      //  finalize by replacing the marks in the templates
      _htmlDoc.AddBodyHtml( t_timestamp.Replace( c_MARK, DateTime.Now.ToString( "g" ) + $" source: {dbSource}" ) );
      _htmlDoc.AddBodyHtml( t_title.Replace( c_MARK, titleContent ) );
      foreach (var subTitle in subTitles) {
        if (string.IsNullOrWhiteSpace( subTitle )) continue; // ignore empty subtitle lines

        _htmlDoc.AddBodyHtml( t_stitle.Replace( c_MARK, subTitle ) );
      }
      // create the body
      _htmlDoc.AddBodyHtml( t_rwyTable.Replace( c_MARK, _rwyRows.ToString( ) ) );
      _htmlDoc.AddBodyHtml( t_sidTable.Replace( c_MARK, _sidRows.ToString( ) ) );
      _htmlDoc.AddBodyHtml( t_starTable.Replace( c_MARK, _starRows.ToString( ) ) );
      _htmlDoc.AddBodyHtml( t_commTable.Replace( c_MARK, _commRows.ToString( ) ) );
      _htmlDoc.AddBodyHtml( t_navaidTable.Replace( c_MARK, _navaidRows.ToString( ) ) );

      // return the outcome and prep for a next round of content
      var doc = _htmlDoc.GetDocument( );
      // init for next round
      InitDocGen( );

      return doc;
    }

    /// <summary>
    /// Reset the 'Zebra' outliner (starts with Odd)
    /// </summary>
    public void ResetZebra( ) => _rowCount = 0;

    /// <summary>
    /// Add a Runway Row
    /// </summary>
    public void AddRwyRow( RwyRowData rowData ) => _rwyRows.AppendLine( rowData.Row_html( ++_rowCount ) );
    /// <summary>
    /// Add an Approach Row
    /// </summary>
    public void AddAprRow( AprRowData rowData ) => _rwyRows.AppendLine( rowData.Row_html( ++_rowCount ) );

    /// <summary>
    /// Add a Runway Proc Row
    /// </summary>
    public void AddRwySidRow( RwyProcRowData rowData ) => _sidRows.AppendLine( rowData.RwyRow_html( ++_rowCount ) );
    /// <summary>
    /// Add a Proc Row for a Runway
    /// </summary>
    public void AddSidRow( RwyProcRowData rowData ) => _sidRows.AppendLine( rowData.ProcRow_html( ++_rowCount ) );

    /// <summary>
    /// Add a Runway Proc Row
    /// </summary>
    public void AddRwyStarRow( RwyProcRowData rowData ) => _starRows.AppendLine( rowData.RwyRow_html( ++_rowCount ) );
    /// <summary>
    /// Add a Proc Row for a Runway
    /// </summary>
    public void AddStarRow( RwyProcRowData rowData ) => _starRows.AppendLine( rowData.ProcRow_html( ++_rowCount ) );


    /// <summary>
    /// Add an Communication Row
    /// </summary>
    public void AddCommRow( CommRowData rowData ) => _commRows.AppendLine( rowData.Row_html( ++_rowCount ) );

    /// <summary>
    /// Add a Navaid Row
    /// </summary>
    public void AddNavaidRow( NavaidRowData rowData ) => _navaidRows.AppendLine( rowData.Row_html( ++_rowCount ) );


  }
}

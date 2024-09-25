using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static FlightplanLib.Flightplan.RowData;

namespace FlightplanLib.Flightplan
{
  /// <summary>
  /// Create a HTML Document for a FlightPlan in table format
  /// </summary>
  internal class FPTableGen
  {
    /*
     Formatted as:
      IDENT, ALT, TYPE, PROC, MTRK, DIST 

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


    // style section content
    private const string c_docStyle =
@"
  body { font-family: ""segoe ui"", arial, sans-serif; }
  table {
    font-family: arial, sans-serif;
    border-collapse: collapse;
    width: 1000px;
  }
  td, th {
    border: 1px solid #cccccc;
    text-align: left;
    padding: 8px;
  }
  th { font-weight: bold; background-color: #f0ffff; }
  td.even { background-color: #dddddd; }
  td.odd { background-color: #ffffff; }
  td.airport { background-color: #f0f8ff; }
  td.runway { background-color: #ffe4c4; }
  td.mapr { background-color: #fff0f5; }
  p.footer { font-size:80%; }
";

    // templates  t_ccc
    private string t_title = "";
    private string t_subtitle = "";
    private string t_table = "";
    private string t_footer = "";

    private bm98_Html.HtmlDocument _htmlDoc;
    private StringBuilder _sbRows;
    private int _rowCount = 0;


    // init to restart
    private void InitDocGen( )
    {
      _htmlDoc = new bm98_Html.HtmlDocument( );
      _sbRows = new StringBuilder( );
      _htmlDoc.AddStyleHtml( c_docStyle );
      _sbRows.AppendLine( FPlanRowData.HeaderRow_html( ) );
      _rowCount = 0;
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public FPTableGen( )
    {
      // create used templates
      // rule is to leave the template with with \n where needed \n will be read by the Renderer as CR(LF)
      t_title = $"<h2>{c_MARK}</h2>\n";
      t_subtitle = $"<h3>{c_MARK}</h3>\n";
      t_table = $"<table>\n{c_MARK}\n</table>\n";
      t_footer = $"<p class=\"footer\">{c_MARK}</p>";

      InitDocGen( );
    }

    /// <summary>
    /// Formatter for one FP Table Row
    ///   Set properties and use it to add a row
    /// </summary>
    public struct RowData
    {


    }// class RowData


    /// <summary>
    /// Add one row of data
    /// </summary>
    /// <param name="rowData">The rowData obj</param>
    public void AddRow( FPlanRowData rowData )
    {
      _sbRows.AppendLine( rowData.Row_html( ++_rowCount ) );
    }

    /// <summary>
    /// Commit the document 
    ///  then reset the formatter
    /// </summary>
    /// <returns>The document as HTML</returns>
    public string CommitDocument( string titleContent, List<string> subTitles, string footer )
    {
      //  finalize by replacing the marks in the templates
      _htmlDoc.AddBodyHtml( t_title.Replace( c_MARK, titleContent ) );
      foreach (var subTitle in subTitles) {
        if (string.IsNullOrWhiteSpace( subTitle )) continue; // ignore empty lines
        _htmlDoc.AddBodyHtml( t_subtitle.Replace( c_MARK, subTitle ) );
      }
      _htmlDoc.AddBodyHtml( t_table.Replace( c_MARK, _sbRows.ToString( ) ) );
      _htmlDoc.AddBodyHtml( t_footer.Replace( c_MARK, $"{DateTime.Now:g} - " + footer ) );

      // return the outcome and prep for a next round of content
      var doc = _htmlDoc.GetDocument( );
      // init for next round
      InitDocGen( );

      return doc;
    }

  }
}

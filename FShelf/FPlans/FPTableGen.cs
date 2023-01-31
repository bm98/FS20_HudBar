using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FShelf.FPlans
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

    private const string c_docStyle = @"
<style>
  table {
    font-family: arial, sans-serif;
    border-collapse: collapse;
    width: 1000px;
  }
  td, th {
    border: 1px solid #dddddd;
    text-align: left;
    padding: 8px;
  }
  th {
    font-weight: bold;
  }
}
</style>\n";

    // string replacement mark
    private const string c_MARK = "$$$+++$$$";
    // templates 
    private string t_doc;
    private string t_title;
    private string t_stitle;
    private string t_table;

    private string _document;
    private string _rows;
    private int _rowCount = 0;

    /// <summary>
    /// cTor:
    /// </summary>
    public FPTableGen( )
    {
      // rule is to leave the template with with \n where needed \n will be read by the Renderer as CR(LF)
      var docHead = $"<head>\n{c_docStyle}</head>";
      var docBody = $"<body>\n{c_MARK}\n</body>";
      t_doc = $"<!DOCTYPE html>\n<html>\n{docHead}{docBody}</html>\n"; // replacement mark

      t_title = $"<h2>{c_MARK}</h2>\n"; // replacement mark
      t_stitle = $"<h3>{c_MARK}</h3>\n"; // replacement mark
      t_table = $"<table>{c_MARK}</table>\n"; // replacement mark

      _document = "";
      _rows = RowData.HeaderRow_html( );
      _rowCount = 0;
    }

    public struct RowData
    {
      /// <summary>
      /// Format as HTML row
      /// </summary>
      /// <returns>A formatted row</returns>
      public static string HeaderRow_html( )
      {
        string cols = "";
        string t_td = $"<th style=\"background-color: #f0ffff;\">{c_MARK}</th>\n"; // background style for TR does not work with this renderer, use TD
        cols += t_td.Replace( c_MARK,"Ident");
        cols += t_td.Replace( c_MARK, "Alt ft");
        cols += t_td.Replace( c_MARK, "Type");
        cols += t_td.Replace( c_MARK, "Proc/Info");
        cols += t_td.Replace( c_MARK, "Trk °M");
        cols += t_td.Replace( c_MARK, "Dist nm" );
        cols += t_td.Replace( c_MARK, "Remaining nm" );

        string row = $"<tr>\n{cols}</tr>\n";
        return row;
      }

      public string Ident;
      public float Alt_ft;
      public string Type;
      public string Proc;
      public int InbTRK_degm;
      public float Dist_nm;

      /// <summary>
      /// Format as HTML row
      /// </summary>
      /// <returns>A formatted row</returns>
      public string Row_html( int lineNum, double remainingDist )
      {
        /*
        <tr>
          <td>Col 1</td>
          <td>Col 2</td>
          <td>Col 3</td>
        </tr>
         */
        string cols = "";
        string bg = (lineNum % 2 == 0) ? "background-color: #dddddd;" : "background-color: #ffffff;"; // even / odd
        string t_td = $"<td style=\"{bg}\">{c_MARK}</td>\n"; // background style for TR does not work with this renderer, use TD
        cols += t_td.Replace( c_MARK, Ident );
        cols += t_td.Replace( c_MARK, $"{Alt_ft:##,##0}" );
        cols += t_td.Replace( c_MARK, $"{Type}" );
        cols += t_td.Replace( c_MARK, Proc );
        cols += t_td.Replace( c_MARK, $"{InbTRK_degm:000}" );
        cols += t_td.Replace( c_MARK, $"{Dist_nm:##0.0}" );
        cols += t_td.Replace( c_MARK, $"{remainingDist:##0.0}" );

        string row = $"<tr>\n{cols}</tr>\n"; 
        return row;
      }
    }


    /// <summary>
    /// Commit the document 
    ///  then reset the formatter
    /// </summary>
    /// <returns>The document as HTML</returns>
    public string CommitDocument( string titleContent, List<string> subTitles )
    {
      //  finalize by replacing the marks in the templates
      var body = t_title.Replace( c_MARK, titleContent );
      foreach (var subTitle in subTitles) {
        if (string.IsNullOrWhiteSpace( subTitle)) continue; // ignore empty lines

        body += t_stitle.Replace( c_MARK, subTitle );
      }
      body += t_table.Replace( c_MARK, _rows );
      _document = t_doc.Replace( c_MARK, body );
      // copy to reset and return
      var doc = _document;
      _document = "";
      _rows = RowData.HeaderRow_html( );
      _rowCount = 0;
      return doc;
    }

    /// <summary>
    /// Add one row of data
    /// </summary>
    /// <param name="rowData"></param>
    public void AddRow( RowData rowData, double remainingDist_nm )
    {
      _rows += rowData.Row_html( ++_rowCount , remainingDist_nm);
    }

  }
}

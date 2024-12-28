using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FShelf.Profiles
{
  /// <summary>
  /// Profile Catalog
  /// </summary>
  internal class ProfileCat : Dictionary<decimal, Profile>
  {

    /// <summary>
    /// The Profile table name[°,%,ft/nm]
    /// </summary>
    public const string ProfileTable = "T_Profile";
    /// <summary>
    /// The Profile table column for DegValue
    /// </summary>
    public const string TProfile_DegValue = "DegVal";
    /// <summary>
    /// The Profile table column for Deg Display string
    /// </summary>
    public const string TProfile_Deg = "Deg";
    /// <summary>
    /// The Profile table column for Percent Display string
    /// </summary>
    public const string TProfile_Prct = "Prct";
    /// <summary>
    /// The Profile table column for Descent Rate Display string
    /// </summary>
    public const string TProfile_DescRate = "DRate";

    /// <summary>
    /// Returns the Cell value as object or null
    /// </summary>
    /// <param name="row">A DGV row</param>
    /// <param name="colName">A column name</param>
    /// <param name="type">The expected type of the value</param>
    /// <returns>The value as object or null</returns>
    public static object DgvCellValue( DataGridViewRow row, string colName, Type type )
    {
      try {
        var cell = row.Cells[colName].Value;
        if (cell.GetType( ) == type) {
          return cell;
        }
        else {
          return null;
        }
      }
      catch {
        return null;
      }
    }

    /// <summary>
    /// Check if a Profile Row matches the value
    /// </summary>
    /// <param name="dgvRow">A Profile Row</param>
    /// <param name="value">A value to match</param>
    /// <returns>True on match</returns>
    public static bool RowMatchOnDeg( DataGridViewRow dgvRow, float value )
    {
      object cValue = DgvCellValue( dgvRow, TProfile_DegValue, typeof( float ) );
      // sanity
      if (cValue == null) return false;

      return (float)cValue == value;

    }

    /// <summary>
    /// Check if a Profile Row matches the value
    /// </summary>
    /// <param name="dgvRow">A Profile Row</param>
    /// <param name="value">A value to match</param>
    /// <returns>True on match</returns>
    public static bool RowMatchOnAlt( DataGridViewRow dgvRow, int value )
    {
      object cValue = DgvCellValue( dgvRow, TProfile_DegValue, typeof( int ) );
      // sanity
      if (cValue == null) return false;

      return (int)cValue == value;

    }


    /// <summary>
    /// The Target VS table name [gsValue, gs, fpm]
    /// </summary>
    public const string TgtVsTable = "T_TgtVS";
    /// <summary>
    /// The Target VS table column for Ground Speed Display Value
    /// </summary>
    public const string TTgtVs_GSValue = "GSValue";
    /// <summary>
    /// The Target VS table column for Ground Speed Display string
    /// </summary>
    public const string TTgtVs_GS = "GS";
    /// <summary>
    /// The Target VS table column for Vertical Speed Display string
    /// </summary>
    public const string TTgtVs_VS = "VRate";


    /// <summary>
    /// The Distance for Altitude table name [Alt, Dist]
    /// </summary>
    public const string Dist4AltTable = "T_Dist4Alt";
    /// <summary>
    /// The Altitude table column for Altitude value
    /// </summary>
    public const string TAlt_AltValue = "AltValue";
    /// <summary>
    /// The Altitude column for Altitude string
    /// </summary>
    public const string TAlt_Alt = "Alt";
    /// <summary>
    /// The Altitude column for Distance string
    /// </summary>
    public const string TAlt_Dist = "Dist";



    private DataSet _pSet = new DataSet( );
    // the selected profile 
    private Profile _selProfile = null;

    // the starting altitude
    private float _altitude_ft = 0;

    /// <summary>
    /// cTor: init the supported profiles
    /// </summary>
    public ProfileCat( )
    {
      // create 1°..6° profiles with a 0.1 stepping
      for (decimal deg = 0m; deg <= 6m; deg += 0.1m) {
        var p = new Profile( deg );
        this.Add( p.Deg, p );
      }

      _selProfile = this[3m]; // set the default profile as 3°

      var table = _pSet.Tables.Add( ProfileTable );
      var colDegVal = table.Columns.Add( TProfile_DegValue ); colDegVal.Caption = "Deg °"; colDegVal.DataType = typeof( float ); // not visible
      var colDeg = table.Columns.Add( TProfile_Deg ); colDeg.Caption = "Profile °"; colDeg.DataType = typeof( string );
      var colPrct = table.Columns.Add( TProfile_Prct ); colPrct.Caption = "%"; colPrct.DataType = typeof( string );
      var colDRate = table.Columns.Add( TProfile_DescRate ); colDRate.Caption = "ft/nm"; colDRate.DataType = typeof( string );
      // build the data set (0.5.. at 0.2 step)
      for (int i = 0; i < 29; i++) {
        decimal deg = 0.4m + i * 0.2m;
        var dRow = table.Rows.Add( this[deg].AsArray );
      }

      // target rate table
      table = _pSet.Tables.Add( TgtVsTable );
      var colGSValue = table.Columns.Add( TTgtVs_GSValue ); colGSValue.Caption = "GS"; colGSValue.DataType = typeof( int ); // not visible
      var colGS = table.Columns.Add( TTgtVs_GS ); colGS.Caption = "GS [kt]"; colGS.DataType = typeof( string );
      var colVRate = table.Columns.Add( TTgtVs_VS ); colVRate.Caption = "VS [fpm]"; colVRate.DataType = typeof( string );
      UpdateVsTable( );

      // Alt Table 1
      table = _pSet.Tables.Add( Dist4AltTable );
      var colAltValue = table.Columns.Add( TAlt_AltValue ); colAltValue.Caption = "AltValue"; colAltValue.DataType = typeof( int ); // not visible
      var colAlt = table.Columns.Add( TAlt_Alt ); colAlt.Caption = "Alt [ft]"; colAlt.DataType = typeof( string );
      var colDst = table.Columns.Add( TAlt_Dist ); colDst.Caption = "Dist [nm]"; colDst.DataType = typeof( string );
      UpdateAltTable( );
    }

    // update the table values from the selected profile
    private void UpdateVsTable( )
    {
      // sanity
      if (_selProfile == null) return;

      var table = _pSet.Tables[TgtVsTable];
      for (int i = 0; i < 29; i++) {
        int gs = 600 - i * 20;
        int vs = (int)Math.Round( _selProfile.VRateTarget_fpm( gs ) / 50, 0 ) * 50; // 50 ft steps
        DataRow row;
        if (table.Rows.Count <= i) {
          row = table.Rows.Add( );
        }
        else {
          row = table.Rows[i];
        }
        row[0] = gs; row[1] = $"{gs:##0}"; row[2] = $"{vs:#,##0}";
      }
    }

    // handle some event concurrency issues with DGV

    private bool _altTableUpdating = false;
    // update the table values from the selected profile and start altitude
    private void UpdateAltTable( )
    {
      // sanity
      if (_selProfile == null) return;
      if (_altTableUpdating) return;

      _altTableUpdating = true;

      var table = _pSet.Tables[Dist4AltTable];
      int alt = 45000;
      for (int i = 0; i < 30; i++) {
        float dst = Math.Abs( _selProfile.Dist_nm( _altitude_ft - alt ) );
        DataRow row;
        if (table.Rows.Count <= i) {
          row = table.Rows.Add( );
        }
        else {
          row = table.Rows[i];
        }
        row[0] = alt; row[1] = $"{alt:##,##0}"; row[2] = $"{dst:##0}";
        alt -= (i > 12) ? 1000 : (i > 0) ? 2000 : 5000; // decrement alt
      }

      //table.AcceptChanges( );

      _altTableUpdating = false;
    }

    /// <summary>
    /// A DataSet for this catalogs Profile
    /// contains ProfileTable, TgtRangeTable
    /// </summary>
    public DataSet ProfileValueSet => _pSet;

    /// <summary>
    /// Get the selected profile (can be null)
    /// </summary>
    public Profile SelectedProfile => _selProfile;

    /// <summary>
    /// Get the nearest profile for a number
    /// </summary>
    /// <param name="deg"></param>
    /// <returns></returns>
    public Profile GetProfile( double deg )
    {
      decimal d = Math.Round( (decimal)deg, 1 );
      if (this.TryGetValue( d, out Profile p )) {
        return p;
      }
      else return null;
    }

    /// <summary>
    /// Set the selected profile from a decimal
    /// </summary>
    /// <param name="deg">A degree</param>
    public void SetSelectedProfile( double deg )
    {
      if (_altTableUpdating) return; // don't set while updating the tables

      var p = GetProfile( deg );
      if (p != null) {
        _selProfile = p;
        UpdateVsTable( );
        UpdateAltTable( );
      }
      // else no change
    }

    /// <summary>
    /// Set the selected profile from a decimal
    /// </summary>
    /// <param name="dgvRow">A row to set from</param>
    public void SetSelectedProfile( DataGridViewRow dgvRow )
    {
      object cValue = DgvCellValue( dgvRow, TProfile_DegValue, typeof( float ) );
      // sanity
      if (cValue == null) return;

      SetSelectedProfile( (float)cValue );
    }

    /// <summary>
    /// Set the starting Alt for the table display
    /// </summary>
    /// <param name="altitude">An Altitude</param>
    public void SetStartAltitude( float altitude )
    {
      if (_altTableUpdating) return; // don't set while updating the tables

      _altitude_ft = altitude;
      UpdateAltTable( );
    }

    /// <summary>
    /// Set the starting Alt for the table display
    /// </summary>
    /// <param name="dgvRow">The row to derive from</param>
    public void SetStartAltitude( DataGridViewRow dgvRow )
    {
      object cValue = DgvCellValue( dgvRow, TAlt_AltValue, typeof( int ) );
      // sanity
      if (cValue == null) return;

      SetStartAltitude( (int)cValue );
    }

    /// <summary>
    /// Returns the GS Row Index where the value is larger than the given GS
    /// </summary>
    /// <param name="gs">A GroundSpeed</param>
    /// <returns>A Row Index</returns>
    public int GsRowIndex( float gs )
    {
      for (int rIndex = 0; rIndex < _pSet.Tables[TgtVsTable].Rows.Count; rIndex++) {
        if ((int)_pSet.Tables[TgtVsTable].Rows[rIndex][0] < (gs - 5)) {
          return ((rIndex - 1) >= 0) ? (rIndex - 1) : 0;
        }
      }
      return _pSet.Tables[TgtVsTable].Rows.Count - 1;
    }

    /// <summary>
    /// Returns the GS Row Index where the value is larger than the given GS
    /// </summary>
    /// <param name="alt">An altitude</param>
    /// <returns>A Row Index</returns>
    public int AltRowIndex( float alt )
    {
      for (int rIndex = 0; rIndex < _pSet.Tables[Dist4AltTable].Rows.Count; rIndex++) {
        if ((int)_pSet.Tables[Dist4AltTable].Rows[rIndex][0] < (alt - 250)) {
          return ((rIndex - 1) >= 0) ? (rIndex - 1) : 0;
        }
      }
      return _pSet.Tables[Dist4AltTable].Rows.Count - 1;
    }

    /// <summary>
    /// Returns the Fpa Row Index where the value is larger than the given Fpa
    /// </summary>
    /// <param name="fpa">A FlightPath angle (sign will be ignored)</param>
    /// <returns>A Row Index or -1 if smaller than 0.4</returns>
    public int FpaRowIndex( float fpa )
    {
      fpa = (float)Math.Abs( fpa );
      if (fpa < 0.4) return -1;

      for (int rIndex = 0; rIndex < _pSet.Tables[ProfileTable].Rows.Count; rIndex++) {
        if ((float)_pSet.Tables[ProfileTable].Rows[rIndex][0] > (fpa - 0.1)) {
          return rIndex;
        }
      }
      return _pSet.Tables[ProfileTable].Rows.Count - 1;
    }

    /// <summary>
    /// Returns the Caption for this column name
    /// </summary>
    /// <param name="tableName">The table name</param>
    /// <param name="colName">The column name</param>
    /// <returns></returns>
    public string ProfileColumnCaption( string tableName, string colName )
    {
      try {
        return _pSet.Tables[tableName].Columns[colName].Caption;
      }
      catch { return colName; }
    }

    /// <summary>
    /// Returns the Caption for this column index
    /// </summary>
    /// <param name="tableName">The table name</param>
    /// <param name="colIndex">A column index</param>
    /// <returns></returns>
    public string ProfileColumnCaption( string tableName, int colIndex )
    {
      try {
        return _pSet.Tables[tableName].Columns[colIndex].Caption;
      }
      catch { return $"Col{colIndex}"; }
    }


  }
}

using System.Collections.Generic;

namespace MetarLib.MDEC
{
  /// <summary>
  /// Flight Category
  /// </summary>
  public enum FLCat
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    VFR,
    MVFR,
    IFR,
    LIFR,
    SUB_LIMIT,
    UNKOWN,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }

  /// <summary>
  /// The Flight Category
  /// </summary>
  public class M_Category : Chunk
  {
    /// <summary>
    /// The Category
    /// </summary>
    public FLCat FlightCategory { get; set; } = FLCat.LIFR;
    /// <summary>
    /// The Flight Category Color Code
    /// </summary>
    public string FlightCategoryColor => c_CatColor[FlightCategory];

    private Dictionary<FLCat, string>c_CatColor = new Dictionary<FLCat, string>(){
      {FLCat.VFR,"green" },
      {FLCat.MVFR,"blue" },
      {FLCat.IFR,"red" },
      {FLCat.LIFR,"magenta" },
      {FLCat.SUB_LIMIT,"black" },
      {FLCat.UNKOWN,"white" },
    };


    /// <summary>
    /// Readable Content
    /// </summary>
    public override string Pretty =>
      !Valid ? "" :
      $"Flight Category: {FlightCategory} ({FlightCategoryColor})";
  }

  // ******* DECODER CLASS

  internal static class M_CategoryDecoder
  {
    /// <summary>
    /// Evaluate the FlightCategory
    /// </summary>
    /// <param name="mdata">An MData record</param>
    /// <returns>A FlightCategory record</returns>
    public static M_Category Decode( MTData mdata )
    {
      var ret = new M_Category();

      // OFF_LIMIT Ceilings are less than 200 feet above ground level and/or visibility is less than 0.5 mile. 
      bool trigger = false;
      trigger |= mdata.Visibility.Valid && mdata.Visibility.Distance_SM < 0.5;
      trigger |= mdata.SkyConditions.Count > 0 && mdata.SkyConditions[0].Height_ft < 200;
      if ( trigger ) {
        ret.FlightCategory = FLCat.LIFR;
        ret.Valid = true;
        return ret;
      }
      // LIFR Ceilings are less than 500 feet above ground level and/or visibility is less than 1 mile. 
      trigger = false;
      trigger |= mdata.Visibility.Valid && mdata.Visibility.Distance_SM < 1.0;
      trigger |= mdata.SkyConditions.Count > 0 && mdata.SkyConditions[0].Height_ft < 500;
      if ( trigger ) {
        ret.FlightCategory = FLCat.LIFR;
        ret.Valid = true;
        return ret;
      }
      // IFR  Ceilings 500 to less than 1,000 feet and/or visibility 1 to less than 3 miles. 
      trigger = false;
      trigger |= mdata.Visibility.Valid && mdata.Visibility.Distance_SM >= 1.0 && mdata.Visibility.Distance_SM < 3.0;
      trigger |= mdata.SkyConditions.Count > 0 && mdata.SkyConditions[0].Height_ft >= 500 && mdata.SkyConditions[0].Height_ft < 1000;
      if ( trigger ) {
        ret.FlightCategory = FLCat.IFR;
        ret.Valid = true;
        return ret;
      }
      // MVFR Ceilings 1,000 to 3,000 feet and/or visibility is 3-5 miles inclusive. 
      trigger = false;
      trigger |= mdata.Visibility.Valid && mdata.Visibility.Distance_SM >= 3.0 && mdata.Visibility.Distance_SM <= 5.0;
      trigger |= mdata.SkyConditions.Count > 0 && mdata.SkyConditions[0].Height_ft >= 1000 && mdata.SkyConditions[0].Height_ft <= 3000;
      if ( trigger ) {
        ret.FlightCategory = FLCat.MVFR;
        ret.Valid = true;
        return ret;
      }
      // VFR  Ceiling greater than 3000 feet and visibility greater than 5 miles (includes sky clear).
      trigger = true;
      trigger &= mdata.Visibility.Valid && mdata.Visibility.Distance_SM > 5.0;
      trigger &= mdata.SkyConditions.Count > 0 && mdata.SkyConditions[0].Height_ft > 3000;
      trigger |= mdata.Visibility.CAVOK; // overrides
      if ( trigger ) {
        ret.FlightCategory = FLCat.VFR;
        ret.Valid = true;
        return ret;
      }
      // missing data
      ret.FlightCategory = FLCat.UNKOWN;
      ret.Valid = true;

      return ret;
    }

    /// <summary>
    /// Evaluate the FlightCategory
    /// </summary>
    /// <param name="forecast">A Forecast record</param>
    /// <returns>A FlightCategory record</returns>
    public static M_Category Decode( T_Forecast forecast )
    {
      var ret = new M_Category();

      // OFF_LIMIT Ceilings are less than 200 feet above ground level and/or visibility is less than 0.5 mile. 
      bool trigger = false;
      trigger |= forecast.Visibility.Valid && forecast.Visibility.Distance_SM < 0.5;
      trigger |= forecast.SkyConditions.Count > 0 && forecast.SkyConditions[0].Height_ft < 200;
      if ( trigger ) {
        ret.FlightCategory = FLCat.LIFR;
        ret.Valid = true;
        return ret;
      }
      // LIFR Ceilings are less than 500 feet above ground level and/or visibility is less than 1 mile. 
      trigger = false;
      trigger |= forecast.Visibility.Valid && forecast.Visibility.Distance_SM < 1.0;
      trigger |= forecast.SkyConditions.Count > 0 && forecast.SkyConditions[0].Height_ft < 500;
      if ( trigger ) {
        ret.FlightCategory = FLCat.LIFR;
        ret.Valid = true;
        return ret;
      }
      // IFR  Ceilings 500 to less than 1,000 feet and/or visibility 1 to less than 3 miles. 
      trigger = false;
      trigger |= forecast.Visibility.Valid && forecast.Visibility.Distance_SM >= 1.0 && forecast.Visibility.Distance_SM < 3.0;
      trigger |= forecast.SkyConditions.Count > 0 && forecast.SkyConditions[0].Height_ft >= 500 && forecast.SkyConditions[0].Height_ft < 1000;
      if ( trigger ) {
        ret.FlightCategory = FLCat.IFR;
        ret.Valid = true;
        return ret;
      }
      // MVFR Ceilings 1,000 to 3,000 feet and/or visibility is 3-5 miles inclusive. 
      trigger = false;
      trigger |= forecast.Visibility.Valid && forecast.Visibility.Distance_SM >= 3.0 && forecast.Visibility.Distance_SM <= 5.0;
      trigger |= forecast.SkyConditions.Count > 0 && forecast.SkyConditions[0].Height_ft >= 1000 && forecast.SkyConditions[0].Height_ft <= 3000;
      if ( trigger ) {
        ret.FlightCategory = FLCat.MVFR;
        ret.Valid = true;
        return ret;
      }
      // VFR  Ceiling greater than 3000 feet and visibility greater than 5 miles (includes sky clear).
      trigger = true;
      trigger &= forecast.Visibility.Valid && forecast.Visibility.Distance_SM > 5.0;
      trigger &= forecast.SkyConditions.Count > 0 && forecast.SkyConditions[0].Height_ft > 3000;
      trigger |= forecast.Visibility.CAVOK; // overrides
      if ( trigger ) {
        ret.FlightCategory = FLCat.VFR;
        ret.Valid = true;
        return ret;
      }
      // missing data
      ret.FlightCategory = FLCat.UNKOWN;
      ret.Valid = true;

      return ret;
    }

  }

}

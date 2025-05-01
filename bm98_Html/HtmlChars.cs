using System;

namespace bm98_Html
{
  /// <summary>
  /// Defines some special chars from UNICODE 
  /// </summary>
  public static class HtmlChars
  {
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

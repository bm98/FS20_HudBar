namespace FS20_HudBar
{
  /// <summary>
  /// Some general tools are located here..
  /// </summary>
  class Conversions
  {

    #region STATIC DME Dist Sign

    /// <summary>
    /// Returns a signed distance for the DME readout Control V_DistDme 
    /// flag==1 => To   + signed
    /// flag==2 => From - signed
    /// flag==0 => Off  NaN
    /// 
    /// </summary>
    /// <param name="absValue">DME Input from Sim</param>
    /// <param name="fromToFlag">FromTo Flag from Sim</param>
    /// <returns></returns>
    public static float DmeDistance( float absValue, int fromToFlag )
    {
      return (fromToFlag == 0) ? float.NaN : ((fromToFlag == 1) ? absValue : -absValue);
    }
    #endregion



  }
}

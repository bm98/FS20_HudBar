using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FShelf.Profiles
{

  /// <summary>
  /// A profile
  /// Note: only positive numbers
  /// i.e. descend profiles must handle the sign accordingly
  /// </summary>
  internal class Profile
  {
    /// <summary>
    /// Profile Degree
    /// Positive numbers only
    /// </summary>
    public decimal Deg { get; private set; } = 1;

    /// <summary>
    /// Profile Percentage ( tan(Deg) )
    /// </summary>
    public float Prct => (float)(Math.Tan( (double)(Deg / 180m) * Math.PI ) * 100.0);

    /// <summary>
    /// Profile descend rate [ft/nm]
    /// 100 fpm / Deg (approximation)
    /// </summary>
    public float DRate => (float)(Deg * 100.0m);

    public Profile( decimal deg )
    {
      Deg = deg;
    }

    public object[] AsArray => new object[] { Deg, $"{Deg:0.0}", $"{Prct:0.0}", $"{DRate:##0}" };

    /// <summary>
    /// The target vertical rate for a given groundspeed
    /// positive values are returned
    /// </summary>
    /// <param name="gs">The groundspeed [kt]</param>
    /// <returns>The vertical speed target to maintain this profile</returns>
    public float VRateTarget_fpm( float gs_kt )
    {
      return gs_kt * Prct;
    }

    /// <summary>
    /// The distance for a delta altitude when flying this profile
    /// </summary>
    /// <param name="dAlt_ft">Delta Altitude [ft]</param>
    /// <returns>A Distance [nm]</returns>
    public float Dist_nm( float dAlt_ft )
    {
      return dAlt_ft / DRate;
    }

  }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// A Bypass instance of a WebProxy
  /// --> NOT USED so far
  /// </summary>
  public sealed class BypassWebProxy : IWebProxy
  {
    /// <summary>
    /// The instance of this singleton
    /// </summary>
    public static readonly BypassWebProxy Instance = new BypassWebProxy( );

    /// <summary>
    /// Credential Store
    /// </summary>
    public ICredentials Credentials {
      get;
      set;
    }

    /// <summary>
    /// Returns a Proxi URI
    /// </summary>
    /// <param name="uri"></param>
    /// <returns></returns>
    public Uri GetProxy( Uri uri )
    {
      return uri;
    }

    /// <summary>
    /// Wether or not the Proxi is bypassed
    /// </summary>
    /// <param name="uri">The Proxi URI</param>
    /// <returns>State</returns>
    public bool IsBypassed( Uri uri )
    {
      return true;
    }
  }

}

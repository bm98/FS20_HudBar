using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// Implement ChartBundle IFR Enroute Low Charts
  /// </summary>
  internal sealed class CB_ENRL : CBBase
  {
    // Singleton Pattern
    public static CB_ENRL Instance => lazy.Value;
    private static readonly Lazy<CB_ENRL> lazy = new Lazy<CB_ENRL>( ( ) => new CB_ENRL( ) );

    private CB_ENRL( ) :
      base( MapProvider.CB_ENRL )
    {
      // set only distict items here - the rest is done in the base class
      RefererUrl = DefaultUrl;
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
    }

    // default URL
    private const string DefaultUrl = "https://wms.chartbundle.com/tms/1.0.0/enrl/{z}/{x}/{y}.png?origin=nw";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319D95" );

    public override string Name { get; } = "CB IFR Enroute Low Charts";

    #endregion

  }
}


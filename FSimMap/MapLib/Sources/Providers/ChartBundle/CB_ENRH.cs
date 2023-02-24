using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// Implement ChartBundle IFR Enroute High Charts
  /// </summary>
  internal sealed class CB_ENRH : CBBase
  {
    // Singleton Pattern
    public static CB_ENRH Instance => lazy.Value;
    private static readonly Lazy<CB_ENRH> lazy = new Lazy<CB_ENRH>( ( ) => new CB_ENRH( ) );

    private CB_ENRH( ) :
      base( MapProvider.CB_ENRH )
    {
      // set only distict items here - the rest is done in the base class
      RefererUrl = DefaultUrl;
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
    }

    // default URL
    private const string DefaultUrl = "https://wms.chartbundle.com/tms/1.0.0/enrh/{z}/{x}/{y}.png?origin=nw";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319D96" );

    public override string Name { get; } = "CB IFR Enroute High Charts";

    #endregion

  }
}

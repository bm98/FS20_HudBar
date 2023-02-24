using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// Implement ChartBundle Sectional Charts
  /// </summary>
  internal sealed class CB_SEC : CBBase
  {
    // Singleton Pattern
    public static CB_SEC Instance => lazy.Value;
    private static readonly Lazy<CB_SEC> lazy = new Lazy<CB_SEC>( ( ) => new CB_SEC( ) );

    private CB_SEC( ) :
      base( MapProvider.CB_SEC )
    {
      // set only distict items here - the rest is done in the base class
      RefererUrl = DefaultUrl;
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
    }

    // default URL
    private const string DefaultUrl = "https://wms.chartbundle.com/tms/1.0.0/sec/{z}/{x}/{y}.png?origin=nw";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319D92" );

    public override string Name { get; } = "CB Sectional Charts";

    #endregion

  }
}

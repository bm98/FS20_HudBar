using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// Implement ChartBundle Terminal Area Charts
  /// </summary>
  internal sealed class CB_TAC : CBBase
  {
    // Singleton Pattern
    public static CB_TAC Instance => lazy.Value;
    private static readonly Lazy<CB_TAC> lazy = new Lazy<CB_TAC>( ( ) => new CB_TAC( ) );

    private CB_TAC( ) :
      base( MapProvider.CB_TAC )
    {
      // set only distict items here - the rest is done in the base class
      RefererUrl = DefaultUrl;
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
    }

    // default URL
    private const string DefaultUrl = "https://wms.chartbundle.com/tms/1.0.0/tac/{z}/{x}/{y}.png?origin=nw";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319D93" );

    public override string Name { get; } = "CB Terminal Area Charts";

    #endregion

  }
}

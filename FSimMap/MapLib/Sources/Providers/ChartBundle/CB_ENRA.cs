using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapLib.Sources.Providers
{
  /// <summary>
  /// Implement ChartBundle IFR Area Charts
  /// </summary>
  internal sealed class CB_ENRA : CBBase
  {
    // Singleton Pattern
    public static CB_ENRA Instance => lazy.Value;
    private static readonly Lazy<CB_ENRA> lazy = new Lazy<CB_ENRA>( ( ) => new CB_ENRA( ) );

    private CB_ENRA( ) :
      base( MapProvider.CB_ENRA )
    {
      // set only distict items here - the rest is done in the base class
      RefererUrl = DefaultUrl;
      // check for Overrides
      if (!string.IsNullOrWhiteSpace( ProviderIni.ProviderHttp( MapProvider ) )) {
        RefererUrl = ProviderIni.ProviderHttp( MapProvider );
      }
    }

    // default URL
    private const string DefaultUrl = "https://wms.chartbundle.com/tms/1.0.0/enra/{z}/{x}/{y}.png?origin=nw";

    #region ProviderBase Members

    public override Guid Id => new Guid( "BEAB409B-6ED0-443F-B8E3-F6DD6F319D94" );

    public override string Name { get; } = "CB IFR Area Charts";

    #endregion

  }
}


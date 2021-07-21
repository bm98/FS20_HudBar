using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FS20_HudBar
{
  /// <summary>
  /// Some general tools are located here..
  /// </summary>
  class Tooling
  {
    private const float c_nmPerM = 5.399568e-4f;

    /// <summary>
    /// Nautical Miles from Meters
    /// </summary>
    /// <param name="meter">Meter</param>
    /// <returns>Nautical Miles</returns>
    public static float NmFromM(float meter )
    {
      return meter * c_nmPerM;
    }

  }
}

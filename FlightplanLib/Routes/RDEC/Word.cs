using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib.Routes.RDEC
{
  /// <summary>
  /// A Word from a route string
  /// </summary>
  internal abstract class Word
  {
    /// <summary>
    /// Record Valid Flag
    /// </summary>
    public virtual bool IsValid { get; } = false;

  }
}

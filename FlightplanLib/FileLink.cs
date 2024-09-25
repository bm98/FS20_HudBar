using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FlightplanLib
{
  /// <summary>
  /// A generic FileLink
  /// </summary>
  public class FileLink
  {
    /// <summary>
    /// True if contents are defined
    /// </summary>
    public bool IsValid => (!(
      string.IsNullOrWhiteSpace( Name )
      || string.IsNullOrWhiteSpace( RemoteUrl )
      || string.IsNullOrWhiteSpace( LinkUrl )
      ));

    /// <summary>
    /// The Name of the file document
    /// </summary>
    public string Name { get; internal set; } = "";

    /// <summary>
    /// The remote base url
    /// </summary>
    public string RemoteUrl { get; internal set; } = "";
    /// <summary>
    /// The link for the file document
    /// </summary>
    public string LinkUrl { get; internal set; } = ""; // a document file name for getting file downloads

    // tools 

    /// <summary>
    /// Returns the local filename to use
    /// </summary>
    public string FilenameLocal => $"@.{Name}{Path.GetExtension( LinkUrl )}";

    /// <summary>
    /// Returns the DL URL
    /// </summary>
    public string DownloadUrl => $"{RemoteUrl}{LinkUrl}";

  }
}

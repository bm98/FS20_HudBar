using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MapLib.Sources;

namespace MapLib.Service
{

  /// <summary>
  /// Wraps the Image to retrieve and a Stack of sources to process
  /// This is usually prepared by the LoadManager and processed by the Source Implementer
  /// </summary>
  internal class LoaderJobWrapper
  {
    // the sources as FIFO
    private Stack<IImgSource> _sources = new Stack<IImgSource>( );

    /// <summary>
    /// The Image ID sought
    /// </summary>
    public MapImageID MapImageID => TileLoaderJob.MapImageID;

    /// <summary>
    /// The initial Loader Job
    /// </summary>
    public Tiles.TileLoaderJob TileLoaderJob { get; private set; }

    /// <summary>
    /// cTor: the query
    /// </summary>
    /// <param name="tileLoaderJob">The original LoaderJob</param>
    public LoaderJobWrapper( Tiles.TileLoaderJob tileLoaderJob )
    {
      TileLoaderJob = tileLoaderJob;
      if (tileLoaderJob.ProviderInstance != null) {
        this.AddSource( tileLoaderJob.ProviderInstance );
      }
    }

    /// <summary>
    /// Add an Image Source to the Stack 
    /// </summary>
    /// <param name="imgSource"></param>
    public void AddSource( IImgSource imgSource )
    {
      _sources.Push( imgSource );
    }

    /// <summary>
    /// Get the next Source
    /// </summary>
    /// <returns>A source or null</returns>
    public IImgSource GetNextSource( )
    {
      if (_sources.Count > 0) {
        return _sources.Pop( );
      }
      return null;
    }

  }
}

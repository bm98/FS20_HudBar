using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using CoordLib;
using MapLib;

namespace TEST_MapLib
{
  public partial class Form1 : Form
  {
    public Form1( )
    {
      MapManager.Instance.InitMapLib( "" ); // Init before anything else
      MapManager.Instance.SetMemCacheStatus( false );  // disable Memory Cache here
      MapManager.Instance.SetDiskCacheStatus( false );  // disable Disk Cache here
      MapManager.Instance.SetProviderSource( true );  // disable Providers here
      //

      InitializeComponent( );

    }

    MapLib.Tiles.TileMatrix TMAT;

    private void btGetOne_Click( object sender, EventArgs e )
    {
      TMAT = new MapLib.Tiles.TileMatrix( 1, 1 );
      TMAT.LoadComplete += TMAT_LoadComplete;

      //TMAT.LoadMatrix( new LatLon( 47.458, 8.548 ), 13, MapProvider.Bing_OStreetMap );
      //TMAT.LoadMatrix( new LatLon( 47.458, 8.548 ), 13, MapProvider.ESRI_Imagery );
      //TMAT.LoadMatrix( new LatLon( 47.29, 8.26 ), 13, MapProvider.OSM_OpenStreetMap );

      TMAT.LoadMatrix( new LatLon( 47.458, 8.548 ), 13, MapManager.Instance.DefaultProvider ); // using the Ini File

    }

    private void UI_RTB( string text )
    {
      RTB.Invoke( (MethodInvoker)delegate { RTB.Text += text; } );
    }

    private void TMAT_LoadComplete( object sender, LoadCompleteEventArgs e )
    {
      UI_RTB( $"OnLoad Complete: {e.TileKey}\n" );
      if (!e.MatrixComplete) {
        UI_RTB( $"Failed Tile Load Reported: {e.TileKey}\n" );
      }
      else {
        UI_RTB( $"Matrix Load Complete Reported\n" );
        TMAT.GetMatrixImage( true ).Save( $"Test{DateTime.Now.Ticks / 10_000_000}.png" ); // get the image with tile borders drawn
        TMAT.LoadComplete -= TMAT_LoadComplete;
      }

    }

    private void Form1_FormClosing( object sender, FormClosingEventArgs e )
    {

      //MapLib.MapManager.Instance.ShutDown( );


    }
  }
}

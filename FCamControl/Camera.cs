using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SC = SimConnectClient;
using FSimClientIF;
using FCamControl.State;
using FSimClientIF.Modules;
using System.Windows.Forms;

namespace FCamControl
{

  /// <summary>
  /// Basic Camera Manager
  /// Owns State Management
  ///   Using CameraMode
  /// </summary>
  internal sealed class Camera : IDisposable
  {

    // attach the property module - this does not depend on the connection established or not
    private readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;
    private int _observerID = -1;

    private Control _invokeTargetControl;
    private Context _camContext = null;


    /// <summary>
    /// The Camera API
    /// </summary>
    public Context CameraAPI => _camContext;

    /// <summary>
    /// cTor:
    /// </summary>
    public Camera( Control ownerForm, MSFS_KeyCat msfs_KeyCat )
    {
      _invokeTargetControl = ownerForm;
      _camContext = new Context( CameraMode.NONE, msfs_KeyCat );

    }

    /// <summary>
    /// Connect to Sim
    /// </summary>
    public void ConnectSim( )
    {
      if (SC.SimConnectClient.Instance.IsConnected && _observerID < 0) {
        _observerID = SV.AddObserver(
           "FCamControl.Context",
           (int)(Sim.DataArrival_perSecond / 2), // twice per sec - else go faster..
           OnDataArrival, null ); // may be no need to run through the invoker ??
      }
    }

    /// <summary>
    /// Disconnect from Sim
    /// </summary>
    public void DisconnectSim( )
    {
      if (_observerID > 0) {
        SV.RemoveObserver( _observerID );
      }
      _observerID = -1;
    }

    /// <summary>
    /// Reload the KeyCatalog after changes
    /// </summary>
    /// <param name="msfs_KeyCat">The Catalog to load</param>
    public void ReloadKeyCatalog( MSFS_KeyCat msfs_KeyCat )
    {
      _camContext.OnMSFS_KeyCatalog( msfs_KeyCat );

    }

    // Camera data arrives
    private void OnDataArrival( string dataRef )
    {
      _camContext.DataArrival( );
    }


    #region DISPOSE

    private bool disposedValue;

    /// <inheritdoc/>
    private void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // TODO: dispose managed state (managed objects)

          DisconnectSim( );

          _camContext?.Dispose( );
        }

        // TODO: free unmanaged resources (unmanaged objects) and override finalizer
        // TODO: set large fields to null
        disposedValue = true;
      }
    }

    // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
    // ~Camera()
    // {
    //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
    //     Dispose(disposing: false);
    // }

    public void Dispose( )
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose( disposing: true );
      GC.SuppressFinalize( this );
    }

    #endregion

  }
}

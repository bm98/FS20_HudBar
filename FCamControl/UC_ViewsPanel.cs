using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using FSimClientIF;
using FSimClientIF.Modules;
using static FSimClientIF.Sim;

namespace FCamControl
{
  /// <summary>
  /// Implements the Views Panel
  /// </summary>
  internal partial class UC_ViewsPanel : UserControl
  {
    // attach the property module - this does not depend on the connection established or not
    private readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;

    // SmartCam Colors 
    private static readonly Color c_scActiveText = Color.Gold;
    private static readonly Color c_scPassiveText = Color.Gainsboro;


    private readonly ToolTip _tooltip = new ToolTip( );

    // ViewIndex selectors
    private readonly ButtonHandler _btHViewIndex = null;

    // used ref to the camera button handler
    private readonly ButtonHandler _bthCameraRef = null;

    // Camera Obj (Ref)
    private readonly Camera _camera;
    // Track Camera and ViewIndex
    private CameraMode _currentCameraMode = CameraMode.NONE;
    private int _currentViewIndex = -1;

    private int _maxIndexEnabled = 0;

    // Smart target items
    private UInt64 _smartTargetHash = 0; // triggers update of the Smart Target List if changed

    // track slider movement - to avoid concurrent slider updates from user and Sim
    private bool _mouseDown = false;
    // track GUI update and don't mix writes up..
    private bool _updatingGUI = false;

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_ViewsPanel( Camera camera, ButtonHandler camBtHandler )
    {
      // sanity
      _camera = camera ?? throw new ArgumentNullException( nameof( camera ) );
      _bthCameraRef = camBtHandler ?? throw new ArgumentNullException( nameof( camBtHandler ) );

      InitializeComponent( );

      // ViewIndex Buttons -- colors from Control Area
      _btHViewIndex = new ButtonHandler( true ) {
        BColor = frmCameraV2.c_CtrlUnselBColor,
        FColor = Color.Black,
        ActBColor = frmCameraV2.c_CtrlSelBColor,
        ActFColor = Color.Black,
      };

      _tooltip.SetToolTip( lbxSmartTargets, "Smart targets - click to view, click again to cancel" );

    }

    private void UC_ViewsPanel_Load( object sender, EventArgs e )
    {
      // Add ViewIndex buttons
      _btHViewIndex.AddButton( btIndex00, ViewIndex_Action );
      _btHViewIndex.AddButton( btIndex01, ViewIndex_Action ); _btHViewIndex.AddButton( btIndex02, ViewIndex_Action );
      _btHViewIndex.AddButton( btIndex03, ViewIndex_Action ); _btHViewIndex.AddButton( btIndex04, ViewIndex_Action );
      _btHViewIndex.AddButton( btIndex05, ViewIndex_Action ); _btHViewIndex.AddButton( btIndex06, ViewIndex_Action );
      _btHViewIndex.AddButton( btIndex07, ViewIndex_Action ); _btHViewIndex.AddButton( btIndex08, ViewIndex_Action );
      _btHViewIndex.AddButton( btIndex09, ViewIndex_Action ); _btHViewIndex.AddButton( btIndex10, ViewIndex_Action );
      _btHViewIndex.AddButton( btIndex11, ViewIndex_Action ); _btHViewIndex.AddButton( btIndex12, ViewIndex_Action );
      _btHViewIndex.AddButton( btIndex13, ViewIndex_Action ); _btHViewIndex.AddButton( btIndex14, ViewIndex_Action );
      _btHViewIndex.AddButton( btIndex15, ViewIndex_Action ); _btHViewIndex.AddButton( btIndex16, ViewIndex_Action );
      _btHViewIndex.AddButton( btIndex17, ViewIndex_Action ); _btHViewIndex.AddButton( btIndex18, ViewIndex_Action );
      _btHViewIndex.AddButton( btIndex19, ViewIndex_Action ); _btHViewIndex.AddButton( btIndex20, ViewIndex_Action );
      _btHViewIndex.AddButton( btIndex21, ViewIndex_Action ); _btHViewIndex.AddButton( btIndex22, ViewIndex_Action );
      _btHViewIndex.AddButton( btIndex23, ViewIndex_Action ); _btHViewIndex.AddButton( btIndex24, ViewIndex_Action );
      _btHViewIndex.AddButton( btIndex25, ViewIndex_Action ); _btHViewIndex.AddButton( btIndex26, ViewIndex_Action );
      _btHViewIndex.AddButton( btIndex27, ViewIndex_Action ); _btHViewIndex.AddButton( btIndex28, ViewIndex_Action );
      _btHViewIndex.AddButton( btIndex29, ViewIndex_Action );

    }


    /// <summary>
    /// Update the GUI from Data
    /// </summary>
    public void UpdateGUI( )
    {
      _updatingGUI = true;

      var camValues = _camera.CameraAPI.CamValueAPI;

      var newCam = _camera.CameraAPI.CurrentCamMode;
      var newIndex = camValues.ViewIndex;

      if ((newCam != _currentCameraMode) || (newIndex != _currentViewIndex)) {
        // cam and/or index has changed
        SetIndexEnabled( camValues.MaxViewIndex );

        var newButton = _btHViewIndex.ButtonFromSlot( newIndex );
        if (newButton != null) {
          _btHViewIndex.ActivateButton( newButton.Slot );
        }
        else {
          ;
          // ERROR 
        }
        // persist
        _currentCameraMode = newCam;
        _currentViewIndex = newIndex;

        // GUI elements
        tbZoom.Visible = camValues.CanZoom;
        lblZoom.Visible = tbZoom.Visible;
      }

      // Zoom for applicable CameraModes
      if (camValues.CanZoom && (!_mouseDown)) {
        tbZoom.Value = camValues.ZoomLevel;
      }

      // smart targets
      lbxSmartTargets.Visible = camValues.CanSmartTarget;
      txSmartTargetName.Visible = camValues.CanSmartTarget;
      txSmartTargetType.Visible = camValues.CanSmartTarget;

      if (camValues.CanSmartTarget) {
        // SmartTarget, illuminate the selected one
        if (SV.Get<bool>( SItem.bGS_Cam_Smart_active )) {
          txSmartTargetType.Text = $"{SV.Get<CameraTargetType>( SItem.cttG_Cam_Smart_targettype )}";
          txSmartTargetName.Text = SV.Get<string>( SItem.sG_Cam_Smart_targetname_selected );
          txSmartTargetType.ForeColor = c_scActiveText;
          txSmartTargetName.ForeColor = c_scActiveText;
          lbxSmartTargets.ForeColor = c_scActiveText;
        }
        else {
          txSmartTargetType.Text = "---";
          txSmartTargetName.Text = "---";
          txSmartTargetType.ForeColor = c_scPassiveText;
          txSmartTargetName.ForeColor = c_scPassiveText;
          lbxSmartTargets.ForeColor = c_scPassiveText;
        }
        PopulateSmartTargets( ); // new targets to show
      }

      _updatingGUI = false;
    }

    #region ViewIndex

    // set the Index buttons 1..N enabled and the rest disabled
    private void SetIndexEnabled( int maxIndex )
    {
      if (maxIndex == _maxIndexEnabled) return; // already there

      int index = 0;
      this.SuspendLayout( );
      foreach (CheckedButton hbt in _btHViewIndex.ButtonList.Cast<CheckedButton>( )) {
        hbt.Cursor = (index < maxIndex) ? Cursors.Hand : Cursors.Default;
        hbt.ForeColor = (index < maxIndex) ? frmCameraV2.c_CtrlUnselFColor : frmCameraV2.c_CtrlUnselFColorDim;
        hbt.Enabled = (index < maxIndex);
        hbt.Visible = (index < maxIndex);
        index++;
      }
      this.ResumeLayout( );
      _maxIndexEnabled = maxIndex;
    }



    // Handle any ViewIndex Button click
    private void ViewIndex_Action( HandledButton sender )
    {
      // will be called with the button instance as sender
      var index = sender.Slot;
      if (index >= 0) {
        // save the last selected per setting for later use
        _bthCameraRef.ButtonFromCamSetting( _camera.CameraAPI.CurrentCamMode )?.SetViewIndex( index );
        _camera?.CameraAPI.CamRequestAPI.RequestViewIndex( index );
      }
    }

    #endregion

    // Zoom is used also for indexed cams
    private void tbZoom_ValueChanged( object sender, EventArgs e )
    {
      if (_updatingGUI) return;

      _camera.CameraAPI.CamRequestAPI.RequestZoomLevel( tbZoom.Value );
    }

    // mouse up/down of sliders
    private void Slider_MouseDown( object sender, MouseEventArgs e ) => _mouseDown = true;
    private void Slider_MouseUp( object sender, MouseEventArgs e ) => _mouseDown = false;


    #region SmartTargets

    // Updates the SmartCam Target list if needed (something has changed)
    private void PopulateSmartTargets( )
    {
      // Build the list
      var list = new List<string>( );
      for (int i = 0; i < 10; i++) {
        // set the Query for the N.th item
        SV.Set( SItem.iS_Cam_Smart_targetQuery_index, i );
        var ttype = SV.Get<CameraTargetType>( SItem.cttG_Cam_Smart_targettype_of );
        var s = SV.Get<string>( SItem.sG_Cam_Smart_targetname_of ); // can be null
        // new query value for the class
        SV.Set( SItem.iS_Cam_Smart_targetQuery_index, (int)ttype );
        var ttClass = SV.Get<string>( SItem.sG_Cam_Smart_targettypeClass_of );
        // comlpete the entry
        list.Add( string.IsNullOrEmpty( s ) ? "---" : $"{ttClass}-{s}" );
      }
      // if changed - load the list into the ListBox
      var tgtHash = dNetBm98.Utilities.KnuthHash( string.Concat( list ) );
      if (tgtHash != _smartTargetHash) {
        lbxSmartTargets.Items.Clear( );
        foreach (var s in list) lbxSmartTargets.Items.Add( s );
        _smartTargetHash = tgtHash;
      }
    }


    private void lbxSmartTargets_SelectedIndexChanged( object sender, EventArgs e )
    {
      // sanity
      if (_updatingGUI) return;
      if (!_camera.CameraAPI.CamValueAPI.CanSmartTarget) return;

      int selected = lbxSmartTargets.SelectedIndex;
      if ((selected >= 0) && (selected < 10)) {
        // toggle selection when clicking the same item
        if (selected != _camera.CameraAPI.CamValueAPI.SmartTargetIndex) {
          // changed
          _camera.CameraAPI.CamRequestAPI.RequestSmartTarget( selected );
        }
        else {
          // toggled off
          _camera.CameraAPI.CamRequestAPI.RequestSmartTarget( -1 );
        }
      }
    }

    #endregion

  }
}

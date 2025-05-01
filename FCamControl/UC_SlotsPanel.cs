using System;
using System.Windows.Forms;

namespace FCamControl
{
  /// <summary>
  /// Implements the Slots Panel
  /// </summary>
  internal partial class UC_SlotsPanel : UserControl
  {
    private readonly ToolTip _tooltip = new ToolTip( );

    // Camera Obj
    private readonly Camera _camera;

    // track GUI update and don't mix writes up..
#pragma warning disable CS0414 // Remove unread private members
    private bool _updatingGUI = false;
#pragma warning restore CS0414 // Remove unread private members

    // flag when Slot Saving is active
    private bool _slotSaving = false;
    // flag when Slot Clearing is active
    private bool _slotClearing = false;

    private ButtonHandler _btHSlot = null; // Slot selectors
    private ButtonHandler _btHSlotFolder = null; // SlotFolder selectors

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_SlotsPanel( Camera camera )
    {
      _camera = camera ?? throw new ArgumentNullException( nameof( camera ) );

      InitializeComponent( );

      // Slot Buttons
      _btHSlot = new ButtonHandler( false ) {
        BColor = frmCameraV2.c_SlotButtonBColor,
        FColor = frmCameraV2.c_SlotButtonUnselFColor,
        ActBColor = frmCameraV2.c_SlotButtonBColor,
        ActFColor = frmCameraV2.c_SlotButtonSelFColor
      };

      // Slot Folders Buttons
      _btHSlotFolder = new ButtonHandler( true ) {
        BColor = frmCameraV2.c_SlotFolderBColor,
        FColor = frmCameraV2.c_SlotFolderUnselFColor,
        ActBColor = frmCameraV2.c_SlotFolderBColor,
        ActFColor = frmCameraV2.c_SlotFolderSelFColor
      };

      _tooltip.SetToolTip( btSaveToSlot, "Save the current view to a slot" );
      _tooltip.SetToolTip( btClearSlot, "Clear a slot" );

    }

    private void UC_SlotsPanel_Load( object sender, EventArgs e )
    {
      // Add Slot buttons
      _btHSlot.AddButton( btSlot00 );
      _btHSlot.AddButton( btSlot01 ); _btHSlot.AddButton( btSlot02 ); _btHSlot.AddButton( btSlot03 );
      _btHSlot.AddButton( btSlot04 ); _btHSlot.AddButton( btSlot05 ); _btHSlot.AddButton( btSlot06 );
      _btHSlot.AddButton( btSlot07 ); _btHSlot.AddButton( btSlot08 ); _btHSlot.AddButton( btSlot09 );

      // Add Slot Folder buttons
      _btHSlotFolder.AddButton( btSlotFolderA, SlotFolder_Action ); _btHSlotFolder.AddButton( btSlotFolderB, SlotFolder_Action );
      _btHSlotFolder.AddButton( btSlotFolderC, SlotFolder_Action ); _btHSlotFolder.AddButton( btSlotFolderD, SlotFolder_Action );
      _btHSlotFolder.AddButton( btSlotFolderE, SlotFolder_Action ); _btHSlotFolder.AddButton( btSlotFolderF, SlotFolder_Action );
      _btHSlotFolder.AddButton( btSlotFolderG, SlotFolder_Action ); _btHSlotFolder.AddButton( btSlotFolderH, SlotFolder_Action );

      // Starred Slots init
      SlotCat.InitSlotFolders( _btHSlot, _camera );
      // get save confirmation callback
      foreach (var slotFolder in SlotCat.SlotFolders) {
        slotFolder.SlotSaved += _slots_SlotSaved;
      }
      foreach (var slotFolder in SlotCat.SlotFolders) {
        slotFolder.SlotCleared += _slots_SlotCleared;
      }

      // Load AppSettings A..F - clumsy but the settings are by SlotFolder and not all in one
      SlotCat.SlotFolders[0].FolderSettingString = AppSettings.Instance.CameraSlotFolder0;
      SlotCat.SlotFolders[1].FolderSettingString = AppSettings.Instance.CameraSlotFolder1;
      SlotCat.SlotFolders[2].FolderSettingString = AppSettings.Instance.CameraSlotFolder2;
      SlotCat.SlotFolders[3].FolderSettingString = AppSettings.Instance.CameraSlotFolder3;
      SlotCat.SlotFolders[4].FolderSettingString = AppSettings.Instance.CameraSlotFolder4;
      SlotCat.SlotFolders[5].FolderSettingString = AppSettings.Instance.CameraSlotFolder5;
      SlotCat.SlotFolders[6].FolderSettingString = AppSettings.Instance.CameraSlotFolder6;
      SlotCat.SlotFolders[7].FolderSettingString = AppSettings.Instance.CameraSlotFolder7;
      // Enable SlotFolder A
      _btHSlotFolder.ActivateButton( 0 );
      SlotCat.SetActiveSlotFolder( 0 );

      lblSaveStar.Visible = false;
      lblClearStar.Visible = false;

    }

    /// <summary>
    /// Update the GUI from Data
    /// </summary>
    public void UpdateGUI( )
    {
      _updatingGUI = true;

      var camValues = _camera.CameraAPI.CamValueAPI;

      // may be nothing here...

      _updatingGUI = false;
    }



    #region Slot and SlotFolder Handling

    // Handle the Slot Saved Event from the SlotCat
    private void _slots_SlotSaved( object sender, EventArgs e )
    {
      // Save Settings A..F
      AppSettings.Instance.CameraSlotFolder0 = SlotCat.SlotFolders[0].FolderSettingString;
      AppSettings.Instance.CameraSlotFolder1 = SlotCat.SlotFolders[1].FolderSettingString;
      AppSettings.Instance.CameraSlotFolder2 = SlotCat.SlotFolders[2].FolderSettingString;
      AppSettings.Instance.CameraSlotFolder3 = SlotCat.SlotFolders[3].FolderSettingString;
      AppSettings.Instance.CameraSlotFolder4 = SlotCat.SlotFolders[4].FolderSettingString;
      AppSettings.Instance.CameraSlotFolder5 = SlotCat.SlotFolders[5].FolderSettingString;
      AppSettings.Instance.CameraSlotFolder6 = SlotCat.SlotFolders[6].FolderSettingString;
      AppSettings.Instance.CameraSlotFolder7 = SlotCat.SlotFolders[7].FolderSettingString;
      AppSettings.Instance.Save( );

      lblSaveStar.Visible = false;
      btClearSlot.Visible = true;
      _slotSaving = false;
    }

    // Handle the Slot Cleared Event from the SlotCat
    private void _slots_SlotCleared( object sender, EventArgs e )
    {
      // Save Settings A..F
      AppSettings.Instance.CameraSlotFolder0 = SlotCat.SlotFolders[0].FolderSettingString;
      AppSettings.Instance.CameraSlotFolder1 = SlotCat.SlotFolders[1].FolderSettingString;
      AppSettings.Instance.CameraSlotFolder2 = SlotCat.SlotFolders[2].FolderSettingString;
      AppSettings.Instance.CameraSlotFolder3 = SlotCat.SlotFolders[3].FolderSettingString;
      AppSettings.Instance.CameraSlotFolder4 = SlotCat.SlotFolders[4].FolderSettingString;
      AppSettings.Instance.CameraSlotFolder5 = SlotCat.SlotFolders[5].FolderSettingString;
      AppSettings.Instance.CameraSlotFolder6 = SlotCat.SlotFolders[6].FolderSettingString;
      AppSettings.Instance.CameraSlotFolder7 = SlotCat.SlotFolders[7].FolderSettingString;
      AppSettings.Instance.Save( );

      lblClearStar.Visible = false;
      btSaveToSlot.Visible = true;
      _slotClearing = false;
    }

    // Save Slot from current settings
    private void btSaveToSlot_Click( object sender, EventArgs e )
    {
      if (_slotSaving) {
        // click when active will cancel the save
        lblSaveStar.Visible = false;
        _slotSaving = false;
        btClearSlot.Visible = true;
        SlotCat.CurrentSlotFolder.CancelSlotSave( );
      }
      else if (_slotClearing) {
        // cannot
      }
      else {
        // click when inactive will trigger the save
        _slotSaving = true;
        lblSaveStar.Visible = true;
        btClearSlot.Visible = false;
        SlotCat.CurrentSlotFolder.ExpectSlotSave( );
      }
    }

    private void btClearSlot_Click( object sender, EventArgs e )
    {
      if (_slotClearing) {
        // click when active will cancel the save
        lblClearStar.Visible = false;
        _slotClearing = false;
        SlotCat.CurrentSlotFolder.CancelSlotClear( );
        btSaveToSlot.Visible = true;
      }
      else if (_slotSaving) {
        // cannot
      }
      else {
        // click when inactive will trigger the save
        _slotClearing = true;
        lblClearStar.Visible = true;
        btSaveToSlot.Visible = false;
        SlotCat.CurrentSlotFolder.ExpectSlotClear( );
      }
    }

    // Handle any SlotFolder click
    private void SlotFolder_Action( HandledButton sender )
    {
      // Make a SlotFolder current
      var index = sender.Slot;
      if (index >= 0) {
        _btHSlotFolder.ButtonFromSlot( SlotCat.CurrentSlotFolder.SlotFolderNo ).Activate( false );
        SlotCat.SetActiveSlotFolder( (uint)index );
        _btHSlotFolder.ButtonFromSlot( SlotCat.CurrentSlotFolder.SlotFolderNo ).Activate( true );
      }
    }


    #endregion

  }
}

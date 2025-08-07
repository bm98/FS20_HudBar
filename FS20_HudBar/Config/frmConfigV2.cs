using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using FS20_HudBar.Bar;
using FS20_HudBar.GUI;
using FS20_HudBar.GUI.Templates.Base;

using static FS20_HudBar.GUI.GUI_Colors;

namespace FS20_HudBar.Config
{
  /// <summary>
  /// The Configuration Form V2
  ///  Note: some values need to be set before Loading the Form !!!
  /// </summary>
  public partial class frmConfigV2 : Form
  {
    // number of profiles handled here
    private const int c_profiles = 10;

    private readonly Color c_profileCol = Color.WhiteSmoke;
    private readonly Color c_profileColSel = Color.PaleGreen;
    private readonly Color c_profileColCurrent = Color.PaleTurquoise;

    // Color and Fonts Buttons
    private readonly Color c_entryDefault = SystemColors.ControlText;
    private readonly Color c_entryAvailable = Color.Blue;

    /// <summary>
    /// HudBar References must be set by HudBar when creating the Config Window
    /// </summary>
    internal HudBar HudBarRef { get; set; } = null;

    /// <summary>
    /// Holds a Copy of the configuration and alters it's values
    /// This item will also return changes
    /// </summary>
    internal Configuration ConfigCopy { get; set; } = null;

    // buttons working as Tab selector
    private Button[] _pButtons = new Button[c_profiles];
    // handled Profile (Tab..)
    private int _selProfileIndex = 0;

    // per profile access
    private FlowLayoutPanel _pFlp;
    private FlpHandler _flpHandler;
    private TextBox _pName;
    private ComboBox _pFont;
    private ComboBox _pPlace;
    private ComboBox _pKind;
    private ComboBox _pCondensed;
    private ComboBox _pTransparency;
    private TextBox _pHotkey;
    private TextBox _pBgImageName;
    private TextBox _pBgImageBorder;
    private Padding _bgImageBorder = Padding.Empty;
    private CheckBox _pFrameItems;
    private CheckBox _pBoxDivider;

    // Dialogs
    private FrmHotkey HKdialog = new FrmHotkey( );

    private FrmFonts FONTSdialog;
    private GUI_Fonts _configFontsObj; // User Fonts obj
    private string _configFonts = "";
    private bool UsingDefaultFonts => string.IsNullOrEmpty( _configFonts );
    private bool m_applyUserFontChanges = false;

    private FrmColors COLORSdialog;
    private string _configColorReg = ""; // User Colors
    private string _configColorDim = "";
    private string _configColorInv = "";
    private bool UsingDefaultColors => string.IsNullOrEmpty( _configColorReg );

    private ToolTip_Base _tooltip = new ToolTip_Base( );

    // local instance for tests
    private GUI_Speech _speech = new GUI_Speech( );

    // concurency avoidance
    private bool _initDone = false;

    // fill the list with items and check them from the Instance
    private void PopulateASave( ComboBox cbx )
    {
      cbx.Items.Clear( );
      cbx.Items.Add( "AutoBackup DISABLED" );
      cbx.Items.Add( "AutoBackup (5 Min)" );
      //cbx.Items.Add( "AutoBackup + ATC" ); // DISABLED
    }

    // set from current TODO values from Config
    private void SetVoiceCalloutState( )
    {
      clbVoice.Items.Clear( );
      foreach (Callouts ce in Enum.GetValues( typeof( Callouts ) )) {
        var idx = clbVoice.Items.Add( HudBar.VoicePack.TriggerCat[ce].Name );
        clbVoice.SetItemChecked( idx, HudBar.VoicePack.TriggerCat[ce].Enabled );
      }
    }

    // a list of Flags
    private List<bool> GetVoiceCalloutState( )
    {
      var l = new List<bool>( );
      for (int idx = 0; idx < clbVoice.Items.Count; idx++) {
        l.Add( clbVoice.GetItemChecked( idx ) );
      }
      return l;
    }

    private void PopulateFonts( ComboBox cbx )
    {
      cbx.Items.Clear( );
      cbx.Items.Add( GUI.FontSize.Regular + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_2 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_4 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_6 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_8 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_10 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Minus_2 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Minus_4 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_12 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_14 + " Font Size" );
      // added 20220212
      cbx.Items.Add( GUI.FontSize.Plus_18 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_20 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_24 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_28 + " Font Size" );
      // added 20220304
      cbx.Items.Add( GUI.FontSize.Plus_32 + " Font Size" );
    }

    private void PopulatePlacement( ComboBox cbx )
    {
      cbx.Items.Clear( );
      cbx.Items.Add( GUI.Placement.Bottom + " bound" );
      cbx.Items.Add( GUI.Placement.Left + " bound" );
      cbx.Items.Add( GUI.Placement.Right + " bound" );
      cbx.Items.Add( GUI.Placement.Top + " bound" );
      cbx.Items.Add( GUI.Placement.TopStack + " mode" );
    }

    private void PopulateKind( ComboBox cbx )
    {
      cbx.Items.Clear( );
      cbx.Items.Add( "Bar" );
      cbx.Items.Add( "Tile" );
      cbx.Items.Add( "Window" ); // 20210718
      cbx.Items.Add( "Window no border" ); // 20211022
    }

    private void PopulateCond( ComboBox cbx )
    {
      cbx.Items.Clear( );
      cbx.Items.Add( "Regular Font" );
      cbx.Items.Add( "Condensed Font" );
    }

    private void PopulateTrans( ComboBox cbx )
    {
      cbx.Items.Clear( );
      cbx.Items.Add( "Opaque" ); // GUI.Transparent.T0
      cbx.Items.Add( $"{(int)GUI.Transparent.T10 * 10}%  Transparent" );
      cbx.Items.Add( $"{(int)GUI.Transparent.T20 * 10}%  Transparent" );
      cbx.Items.Add( $"{(int)GUI.Transparent.T30 * 10}%  Transparent" );
      cbx.Items.Add( $"{(int)GUI.Transparent.T40 * 10}%  Transparent" );
      cbx.Items.Add( $"{(int)GUI.Transparent.T50 * 10}%  Transparent" );
      cbx.Items.Add( $"{(int)GUI.Transparent.T60 * 10}%  Transparent" );
      cbx.Items.Add( $"{(int)GUI.Transparent.T70 * 10}%  Transparent" );
      cbx.Items.Add( $"{(int)GUI.Transparent.T80 * 10}%  Transparent" );
      cbx.Items.Add( $"{(int)GUI.Transparent.T90 * 10}%  Transparent" );
    }

    // Load the combo from installed voices
    private void PopulateVoice( ComboBox cbx )
    {
      cbx.Items.Clear( );
      foreach (var vn in GUI_Speech.AvailableVoices) {
        cbx.Items.Add( vn );
      }
    }

    // select the current voice from settings
    public void LoadVoice( ComboBox cbx, string voiceName )
    {
      if (cbx.Items.Contains( voiceName ))
        cbx.SelectedItem = voiceName;
      else if (cbx.Items.Count > 0) {
        cbx.SelectedIndex = 0;
      }
      else {
        // no voices installed...
      }
    }

    // Load the combo from installed devices
    private void PopulateOutputDevice( ComboBox cbx )
    {
      cbx.Items.Clear( );
      foreach (var vn in GUI_Speech.AvailableOutputDevices) {
        cbx.Items.Add( vn );
      }
    }

    // select the current output device from settings
    public void LoadOutputDevice( ComboBox cbx, string deviceName )
    {
      if (cbx.Items.Contains( deviceName ))
        cbx.SelectedItem = deviceName;
      else if (cbx.Items.Count > 0) {
        cbx.SelectedIndex = 0;
      }
      else {
        // no devices installed ???...
      }
    }

    // Initializes the selected Profile
    private void InitProfile( )
    {
      LoadProfile( _selProfileIndex );
    }

    // Loads a profile with Index
    private void LoadProfile( int profileIndex )
    {
      var profile = ConfigCopy.ProfileAt( profileIndex );

      _pName.Text = profile.PName;
      // the GUI column has its FlpHandler
      _flpHandler?.Dispose( );
      _flpHandler = new FlpHandler(
        _pFlp, 0,
        profile.ProfileString( ),
        profile.FlowBreakString( ),
        profile.ItemPosString( )
      );
      _flpHandler.LoadFlp( HudBarRef );

      profile.LoadFontSize( _pFont );
      profile.LoadPlacement( _pPlace );
      profile.LoadKind( _pKind );
      profile.LoadCond( _pCondensed );
      profile.LoadTrans( _pTransparency );

      _pFrameItems.Checked = profile.FrameItems;
      _pBoxDivider.Checked = profile.BoxDivider;

      _pHotkey.Text = profile.HKProfile;
      _pBgImageName.Text = profile.BgImageName;
      _bgImageBorder = profile.BgImageBorder; // maintain Master value as Struct
      _pBgImageBorder.Text = _bgImageBorder.ToString( );

      // mark the selected one 
      _pName.BackColor = (ConfigCopy.CurrentProfileIndex == profileIndex) ? c_profileColSel : c_profileCol;
      // Color/Font entry indication
      btProfileColors.ForeColor = profile.UsingDefaultColors ? c_entryDefault : c_entryAvailable;
      btProfileFonts.ForeColor = profile.UsingDefaultFonts ? c_entryDefault : c_entryAvailable;
    }

    // temp store the edits of the Profile with Index
    private void UpdateProfile( int profileIndex )
    {
      var profile = ConfigCopy.ProfileAt( profileIndex );

      // record profile Updates from the controls
      profile.PName = _pName.Text.Trim( );
      profile.GetItemsFromFlp( _pFlp, 0 );
      profile.GetFontSizeFromCombo( _pFont );
      profile.GetPlacementFromCombo( _pPlace );
      profile.GetKindFromCombo( _pKind );
      profile.GetCondensedFromCombo( _pCondensed );
      profile.GetTransparencyFromCombo( _pTransparency );
      profile.SetFrameItemsFromValue( _pFrameItems.Checked );
      profile.SetBoxDividerFromValue( _pBoxDivider.Checked );

      profile.SetHKfromValue( _pHotkey.Text );
      profile.SetBgImageFromValues( _pBgImageName.Text, _bgImageBorder );
      // color and font is cached on Color/FontDialog OK exit
    }

    /// <summary>
    /// cTor: for the Form
    /// </summary>
    public frmConfigV2( )
    {
      _initDone = false;
      InitializeComponent( );

      // indexed TabButton access
      _pButtons[0] = btP1; _pButtons[1] = btP2; _pButtons[2] = btP3; _pButtons[3] = btP4; _pButtons[4] = btP5;
      _pButtons[5] = btP6; _pButtons[6] = btP7; _pButtons[7] = btP8; _pButtons[8] = btP9; _pButtons[9] = btP10;

      // Show the instance name in the Window Border Text
      this.Text = "Hud Bar Configuration - Instance: " + (string.IsNullOrEmpty( Program.Instance ) ? "Default" : Program.Instance);
      // load the Profile Name Context Menu with items
      ctxMenu.Items.Clear( );
      ctxMenu.Items.Add( "Copy items", null, ctxCopy_Click );
      ctxMenu.Items.Add( "Paste items here", null, ctxPaste_Click );
      ctxMenu.Items.Add( "Re-Order items", null, ctxReOrder_Click );
      ctxMenu.Items.Add( new ToolStripSeparator( ) );
      ctxMenu.Items.Add( "Export Profile...", null, ctxExport_Click );
      ctxMenu.Items.Add( "Import Profile...", null, ctxImport_Click );
      ctxMenu.Items.Add( new ToolStripSeparator( ) );

      // Add Aircraft Merges
      var menu = new ToolStripMenuItem( "Aircraft Merges" );
      ctxMenu.Items.Add( menu );
      AcftMerges.AddMenuItems( menu, ctxAP_Click );

      // Add Default Profiles
      menu = new ToolStripMenuItem( "Default Profiles" );
      ctxMenu.Items.Add( menu );
      DefaultProfiles.AddMenuItems( menu, ctxDP_Click );
      _tooltip.ReshowDelay = 100; // pop a bit faster
      _tooltip.InitialDelay = 300; // pop a bit faster
      _tooltip.SetToolTip( txHkShowHide, "Hotkey to Show/Hide the Bar\nDouble click to edit the Hotkey" );
      _tooltip.SetToolTip( txHkP1, "Hotkey to select this Profile\nDouble click to edit the Hotkey" );
      _tooltip.SetToolTip( txHkShelf, "Hotkey to toggle the Flight Bag\nDouble click to edit the Hotkey" );
      _tooltip.SetToolTip( txHkCamera, "Hotkey to toggle the Camera Selector\nDouble click to edit the Hotkey" );
      _tooltip.SetToolTip( txHkChecklistBox, "Hotkey to toggle the Checklist Box Selector\nDouble click to edit the Hotkey" );

      // access for profile controls in the Form
      _pFlp = flpP1;
      _pName = txP1;
      _pFont = cbxFontP1;
      _pPlace = cbxPlaceP1;
      _pKind = cbxKindP1;
      _pCondensed = cbxCondP1;
      _pTransparency = cbxTransP1;
      _pHotkey = txHkP1;
      _pBgImageName = txBgFileP1;
      _pBgImageBorder = txBgFileBorderP1;
      _pFrameItems = cbxFrameItems;
      _pBoxDivider = cbxBoxDivider;

      // init combos values
      PopulateFonts( _pFont );
      PopulatePlacement( _pPlace );
      PopulateKind( _pKind );
      PopulateCond( _pCondensed );
      PopulateTrans( _pTransparency );

      PopulateASave( cbxASave ); //20211204
    }

    // LOAD is called on any invocation of the Dialog
    // Load all items from HUD to make them editable
    // not profile items, will be loaded at the end as a whole
    private void frmConfig_Load( object sender, EventArgs e )
    {
      this.TopMost = false; // inherited from parent - we don't want this here

      this.Location = new Point( 10, 10 );
      // init with the proposed location from profile (check within a virtual box)
      if (dNetBm98.Utilities.IsOnScreen( AppSettingsV2.Instance.ConfigLocation, new Size( 100, 100 ) )) {
        this.Location = AppSettingsV2.Instance.ConfigLocation;
      }
      this.Size = AppSettingsV2.Instance.ConfigSize;

#if DEBUG
      // throw to catch programming errors
      if (HudBarRef == null) throw new InvalidOperationException( "HudBarRef cannot be null" );
      if (ConfigCopy == null) throw new InvalidOperationException( "ConfigCopy cannot be null" );
#endif

      // sanity ..
      if (HudBarRef == null) return;
      if (ConfigCopy == null) return;

      // set internal current index from current user selection to start from
      _selProfileIndex = ConfigCopy.CurrentProfileIndex;

      // name TabButtons
      foreach (DProfile pe in Enum.GetValues( typeof( DProfile ) )) {
        int profileIndex = (int)pe;
        _pButtons[profileIndex].Text = ConfigCopy.Profiles[pe].PName;
        _pButtons[profileIndex].BackColor = c_profileCol;
        _pButtons[profileIndex].Tag = profileIndex; // get it's index
      }
      _pButtons[_selProfileIndex].BackColor = c_profileColSel; // mark the currently selected one

      cbxFlightRecorder.Checked = ConfigCopy.FRecorder;
      cbxMSFSHoverMode.Checked = ConfigCopy.MSFSHoverMode;
      cbxASave.SelectedIndex = (int)ConfigCopy.FltAutoSaveATC;

      PopulateOutputDevice( cbxOutputDevice );
      LoadOutputDevice( cbxOutputDevice, ConfigCopy.OutputDeviceName );
      _speech.SetOutputDevice( cbxOutputDevice.SelectedItem.ToString( ) );

      PopulateVoice( cbxVoice );// 20211006
      LoadVoice( cbxVoice, ConfigCopy.VoiceName );
      _speech.SetVoice( cbxVoice.SelectedItem.ToString( ) );
      _speech.Enabled = true;
      SetVoiceCalloutState( ); // 20211018

      // Hotkeys // 20211211
      chkInGame.Checked = ConfigCopy.InGameHook; // 20211208
      chkKeyboard.Checked = ConfigCopy.KeyboardHook; // 20211208
      txHkShowHide.Text = ConfigCopy.HKShowHide;
      txHkShelf.Text = ConfigCopy.HKShelf;
      txHkCamera.Text = ConfigCopy.HKCamera;
      txHkChecklistBox.Text = ConfigCopy.HKChecklistBox;

      // use a Config Copy to allow Cancel changes
      FONTSdialog = new FrmFonts( );
      // use our own Font Obj to work with
      _configFontsObj = new GUI_Fonts( HudBarRef.FontRef );
      _configFonts = ConfigCopy.UserFonts;
      _configFontsObj.FromConfigString( _configFonts ); // Init with User Fonts from Config

      // init Color Config and temp stores
      COLORSdialog = new FrmColors( );
      // use our own Color Config Strings to work with
      _configColorReg = ConfigCopy.UserColorsReg;
      _configColorDim = ConfigCopy.UserColorsDim;
      _configColorInv = ConfigCopy.UserColorsInv;

      // Color/Font entry indication
      btColors.ForeColor = ConfigCopy.UsingDefaultColors ? c_entryDefault : c_entryAvailable;
      btFonts.ForeColor = ConfigCopy.UsingDefaultFonts ? c_entryDefault : c_entryAvailable;

      // init with the currenty selected profile 
      SelectProfileIndex( ConfigCopy.CurrentProfileIndex, false ); // don't cache on init

      // Dump Button default is not visible
      btDumpConfigs.Visible = false;
#if DEBUG
      // Used in Debug only
      btDumpConfigs.Visible = true;
#endif


      _initDone = true;
    }

    private void frmConfig_VisibleChanged( object sender, EventArgs e )
    {
      ; // DEBUG stop
    }

    private void frmConfig_FormClosing( object sender, FormClosingEventArgs e )
    {
      // reset Sel Color
      _pName.BackColor = this.BackColor;
      _speech.Enabled = false;
      FONTSdialog?.Dispose( );
      COLORSdialog?.Dispose( );
    }


    // CANCEL - leave unchanged
    private void btCancel_Click( object sender, EventArgs e )
    {
      // should be on screen and valid when the button is clicked..
      AppSettingsV2.Instance.ConfigLocation = this.Location;
      AppSettingsV2.Instance.ConfigSize = this.Size;
      AppSettingsV2.Instance.Save( );

      this.DialogResult = DialogResult.Cancel;
      this.Hide( );
    }


    // ACCEPT - transfer all items back to Configuration
    private void btAccept_Click( object sender, EventArgs e )
    {
      // should be on screen and valid when the button is clicked..
      AppSettingsV2.Instance.ConfigLocation = this.Location;
      AppSettingsV2.Instance.ConfigSize = this.Size;
      AppSettingsV2.Instance.Save( );

      // update from edits
      ConfigCopy.SetFlightRecorder( cbxFlightRecorder.Checked );
      ConfigCopy.SetMSFSHoverMode( cbxMSFSHoverMode.Checked );
      ConfigCopy.SetKeyboardHook( chkKeyboard.Checked );
      ConfigCopy.SetInGameHook( chkInGame.Checked );
      ConfigCopy.SetFltAutoSave( (FSimClientIF.FltFileModuleMode)cbxASave.SelectedIndex ); // can no longer carry ATC (was removed)
      ConfigCopy.SetOutputDeviceName( cbxOutputDevice.SelectedItem.ToString( ) );
      ConfigCopy.SetVoiceName( cbxVoice.SelectedItem.ToString( ) );

      // hotkeys are already written back to config when the HKDialog closes

      var flags = GetVoiceCalloutState( );
      ConfigCopy.SetVoiceCalloutConfigString( HudVoice.AsConfigString( flags ) );

      // Update User fonts
      ConfigCopy.SetUserFontsConfigString( _configFonts );

      // Update User colors
      ConfigCopy.SetUserColorsConfigString( _configColorReg, _configColorDim, _configColorInv );

      // get last changes of the current profile
      UpdateProfile( _selProfileIndex );

      this.DialogResult = DialogResult.OK;
      this.Hide( );
    }

    #region Input Control Eventhandling

    private void txP1_Validating( object sender, CancelEventArgs e )
    {
      _pButtons[_selProfileIndex].Text = txP1.Text; // update from entry
    }

    // voice selected
    private void cbxVoice_SelectedIndexChanged( object sender, EventArgs e )
    {
      _speech.SetVoice( cbxVoice.SelectedItem.ToString( ) );
    }

    // test speech triggered
    private void cbxVoice_MouseClick( object sender, MouseEventArgs e )
    {
    }

    private void btTestVoice_Click( object sender, EventArgs e )
    {
      _speech.SetVoice( cbxVoice.SelectedItem.ToString( ) );
      _speech.SaySynched( 100 );
    }

    private void clbVoice_SelectedIndexChanged( object sender, EventArgs e )
    {
      if (clbVoice.SelectedIndex < 0) return;
      if (!clbVoice.GetItemChecked( clbVoice.SelectedIndex )) return;
      if (!_initDone) return; // don't talk at startup

      // Test when checked
      HudBar.VoicePack.TriggerCat[(Callouts)clbVoice.SelectedIndex].Test( _speech );
    }

    // output device selected
    private void cbxOutputDevice_SelectedIndexChanged( object sender, EventArgs e )
    {
      _speech.SetOutputDevice( cbxOutputDevice.SelectedItem.ToString( ) );
    }

    #endregion

    #region Context Menu

    // Buffer to maintain copied items
    private ProfileItemsStore m_copyBuffer;

    // avoid opening the context menu on Checkboxes
    private void ctxMenu_Opening( object sender, CancelEventArgs e )
    {
      if (sender is ContextMenuStrip) {
        if (ctxMenu.SourceControl is FlowLayoutPanel) {
          e.Cancel = _flpHandler.MouseOverItem;
        }
      }
    }


    // Copy Items is clicked
    private void ctxCopy_Click( object sender, EventArgs e )
    {
      m_copyBuffer = _flpHandler.GetProfileStoreFromFlp( );
    }

    // Paste items is clicked
    private void ctxPaste_Click( object sender, EventArgs e )
    {
      if (m_copyBuffer != null) {
        _flpHandler.LoadDefaultProfile( m_copyBuffer );
        _flpHandler.LoadFlp( HudBarRef );
      }
    }

    // ReOrder items is clicked
    private void ctxReOrder_Click( object sender, EventArgs e )
    {
      _flpHandler.ReOrderProfile( HudBarRef );
      _flpHandler.LoadFlp( HudBarRef );
    }

    // Export items is clicked
    private void ctxExport_Click( object sender, EventArgs e )
    {
      string serString = ConfigCopy.ProfileAt( _selProfileIndex ).GetAsSerString( );
      SFD.FileName = dNetBm98.Utilities.MakeValidFileName( ConfigCopy.ProfileAt( _selProfileIndex ).PName );
      if (SFD.ShowDialog( this ) == DialogResult.OK) {
        using (var sw = new StreamWriter( SFD.FileName, false )) {
          sw.Write( serString );
          sw.Flush( );
        }
      }
    }
    // Import items is clicked
    private void ctxImport_Click( object sender, EventArgs e )
    {
      string serString = "";
      if (OFD.ShowDialog( this ) == DialogResult.OK) {
        using (var sr = new StreamReader( OFD.FileName )) {
          serString = sr.ReadToEnd( );
        }
        var cp = CProfile.GetFromSerString( serString );
        if (cp != null) {
          ConfigCopy.ReplaceProfile( _selProfileIndex, cp );
          SelectProfileIndex( _selProfileIndex, false );
          txP1_Validating( this, new CancelEventArgs( ) ); // update as if typed
        }
      }
    }

    // A default profile is clicked
    private void ctxDP_Click( object sender, EventArgs e )
    {
      var tsi = (sender as ToolStripItem);
      object item = tsi.Owner;
      // backup the menu tree
      while (!(item is ContextMenuStrip)) {
        if (item is ToolStripDropDownMenu)
          item = (item as ToolStripDropDownMenu).OwnerItem;
        else if (item is ToolStripMenuItem)
          item = (item as ToolStripMenuItem).Owner;
        else
          return; // not an expected menu tree 
      }

      var dp = DefaultProfiles.GetDefaultProfile( tsi.Text );
      if (dp != null) {
        _flpHandler.LoadDefaultProfile( dp );
        _flpHandler.LoadFlp( HudBarRef );
        _pName.Text = dp.Name;
      }
    }

    // An aircraft merge profile is clicked
    private void ctxAP_Click( object sender, EventArgs e )
    {
      var tsi = (sender as ToolStripItem);
      object item = tsi.Owner;
      // backup the menu tree
      while (!(item is ContextMenuStrip)) {
        if (item is ToolStripDropDownMenu)
          item = (item as ToolStripDropDownMenu).OwnerItem;
        else if (item is ToolStripMenuItem)
          item = (item as ToolStripMenuItem).Owner;
        else
          return; // not an expected menu tree 
      }

      var dp = AcftMerges.GetAircraftProfile( tsi.Text );
      if (dp != null) {
        _flpHandler.MergeProfile( dp.Profile );
        _flpHandler.LoadFlp( HudBarRef );
        _pName.Text = dp.Name;
      }
    }

    #endregion

    #region Hotkey Configuration

    // show hide HK Boxes when MS HK is changed
    private void chkKeyboard_CheckedChanged( object sender, EventArgs e )
    {
      txHkShowHide.Visible = chkKeyboard.Checked;
      txHkP1.Visible = chkKeyboard.Checked;
      txHkShelf.Visible = chkKeyboard.Checked;
      txHkCamera.Visible = chkKeyboard.Checked;
      txHkChecklistBox.Visible = chkKeyboard.Checked;
    }


    // Handle the hotkey entry for the given Key item
    private string HandleHotkey( Hotkeys hotkey )
    {
      // Setup of the Input Form
      HKdialog.Hotkey = new Win.WinHotkey( ConfigCopy.HKStringOf( hotkey ) );
      var old = HKdialog.Hotkey.AsString;

      HKdialog.ProfileName = $"{hotkey} Hotkey";
      if (HKdialog.ShowDialog( this ) == DialogResult.OK) {
        ConfigCopy.SetHKStringOf( hotkey, HKdialog.Hotkey.AsString );
        return HKdialog.Hotkey.AsString;
      }
      else {
        // cancelled
        return old; // the one we started with
      }
    }

    private void txHkP1_DoubleClick( object sender, EventArgs e )
    {
      txHkP1.Text = HandleHotkey( (Hotkeys)((int)Hotkeys.Profile_1 + _selProfileIndex) );
      txHkP1.Select( 0, 0 );
    }

    private void txHkShowHide_DoubleClick( object sender, EventArgs e )
    {
      txHkShowHide.Text = HandleHotkey( Hotkeys.Show_Hide );
      txHkShowHide.Select( 0, 0 );
    }

    private void txHkShelf_DoubleClick( object sender, EventArgs e )
    {
      txHkShelf.Text = HandleHotkey( Hotkeys.FlightBag );
      txHkShelf.Select( 0, 0 );
    }

    private void txHkCamera_DoubleClick( object sender, EventArgs e )
    {
      txHkCamera.Text = HandleHotkey( Hotkeys.Camera );
      txHkCamera.Select( 0, 0 );
    }

    private void txHkChecklistBox_DoubleClick( object sender, EventArgs e )
    {
      txHkChecklistBox.Text = HandleHotkey( Hotkeys.ChecklistBox );
      txHkChecklistBox.Select( 0, 0 );
    }

    #endregion

    #region Dump Profile (R&D Mode..)

    // For Debug and Setup only
    private void btDumpConfigs_Click( object sender, EventArgs e )
    {
      DumpProfiles( );
    }

    /// <summary>
    /// Dump Profiles for embedding after adding items
    /// </summary>
    internal void DumpProfiles( )
    {
      using (var sw = new StreamWriter( DefaultProfiles.DefaultProfileName )) {
        sw.WriteLine( "# HEADER: 4 lines for each profile (Name, profile, order, flowbreak) All lines must be semicolon separated" );
        for (int p = 0; p < CProfile.c_numProfiles; p++) {
          // 4 lines
          sw.WriteLine( ConfigCopy.ProfileAt( p ).PName );
          sw.WriteLine( ConfigCopy.ProfileAt( p ).ProfileString( ) );
          sw.WriteLine( ConfigCopy.ProfileAt( p ).ItemPosString( ) );
          sw.WriteLine( ConfigCopy.ProfileAt( p ).FlowBreakString( ) );
        }
      }
    }

    #endregion

    #region FONT Dialog

    // General User Fonts Settings
    private void btFonts_Click( object sender, EventArgs e )
    {
      // prep dialog from cached values (user may open this dialog multiple times in a config session)
      // load labels from HUD prototypes (instead of copying all properties)
      FONTSdialog.ProtoLabelRef = HudBarRef.ProtoLabelRef;
      FONTSdialog.ProtoValueRef = HudBarRef.ProtoValueRef;
      FONTSdialog.ProtoValue2Ref = HudBarRef.ProtoValue2Ref;
      FONTSdialog.Fonts?.Dispose( );
      FONTSdialog.Fonts = new GUI_Fonts( _configFontsObj ); // let the Config use a clone to apply changes for preview
      FONTSdialog.DefaultFontsConfig = ""; // use system defaults

      if (FONTSdialog.ShowDialog( this ) == DialogResult.OK) {
        if (FONTSdialog.UsingDefault) {
          _configFontsObj = new GUI_Fonts( HudBarRef.FontRef ); // set default
          _configFontsObj.ResetUserFonts( );
          _configFonts = "";
        }
        else {
          // store User fonts in cache
          _configFontsObj.Dispose( );
          _configFontsObj = new GUI_Fonts( FONTSdialog.Fonts ); // maintain the changes
          _configFonts = _configFontsObj.AsConfigString( );
        }
      }
      btFonts.ForeColor = this.UsingDefaultFonts ? c_entryDefault : c_entryAvailable;

    }

    // change Profile Fonts
    private void btProfileFonts_Click( object sender, EventArgs e )
    {
      // prep dialog from cached values (user may open this dialog multiple times in a config session)
      var profile = ConfigCopy.ProfileAt( _selProfileIndex );

      // load labels from HUD prototypes (instead of copying all properties)
      FONTSdialog.ProtoLabelRef = HudBarRef.ProtoLabelRef;
      FONTSdialog.ProtoValueRef = HudBarRef.ProtoValueRef;
      FONTSdialog.ProtoValue2Ref = HudBarRef.ProtoValue2Ref;
      FONTSdialog.Fonts?.Dispose( );
      FONTSdialog.Fonts = new GUI_Fonts( _configFontsObj ); // let the Config use a clone to apply changes for preview
      FONTSdialog.Fonts.FromConfigString( profile.Fonts ); // set current from Profile Cache
      FONTSdialog.DefaultFontsConfig = ""; // use AppFonts as Fallback

      if (FONTSdialog.ShowDialog( this ) == DialogResult.OK) {
        if (FONTSdialog.UsingDefault) {
          profile.SetFontsFromValues( "" );
        }
        else {
          // store fonts in profile cache
          profile.SetFontsFromValues( FONTSdialog.Fonts.AsConfigString( ) );
        }
      }
      btProfileFonts.ForeColor = profile.UsingDefaultFonts ? c_entryDefault : c_entryAvailable;
    }

    #endregion

    #region COLOR Dialog

    /* Color works as follows:
     * 
     * There are:
     *   App Colors (coded defaults)
     *   User Colors (also legacy colors)
     *   Profile Colors (introduced Feb 2024)
     *   
     * Color Dialog 'Use Defaults' always refers to AppColors
     *  when using Defaults all 3 corresponding color string shall be empty (applies to Profile and User Colors)
     *  
     * The Color Dialog shall inspect the the current color on entry and apply the Use Defaults flag when empty
     *  then fill with defaults entries to have a visual
     *  
     * User Colors is the Fallback when Profile Colors are Default (empty)
     * i.e. When setting User Colors it shall be choosen when the Profile Color is Default (empty)
     *   else use AppColors
     * 
     * The Color Dialog sets it's Defaults flag when the user Clicks the 'Use Defaults' button
     * It clears the flag for any Accepted Color Pick
     * When the Color Dialog returns with Cancel no changes shall be recorded
     * When the Color Dialog returns with OK the handler below shall visit the Defaults Flag and set corresponding color strings to default (empty)
     * 
     */

    // General User Colors Settings
    private void btColors_Click( object sender, EventArgs e )
    {
      // use AppColors as fallback
      COLORSdialog.DefaultRegColors = GUI_Colors.GetDefaultColorSet( ColorSet.BrightSet );
      COLORSdialog.DefaultDimColors = GUI_Colors.GetDefaultColorSet( ColorSet.DimmedSet );
      COLORSdialog.DefaultInvColors = GUI_Colors.GetDefaultColorSet( ColorSet.InverseSet );
      // prep dialog from cached values 
      COLORSdialog.RegColors = GUI_Colors.FromConfigString( _configColorReg );
      COLORSdialog.DimColors = GUI_Colors.FromConfigString( _configColorDim );
      COLORSdialog.InvColors = GUI_Colors.FromConfigString( _configColorInv );

      if (COLORSdialog.ShowDialog( this ) == DialogResult.OK) {
        if (COLORSdialog.UsingDefault) {
          _configColorReg = "";
          _configColorDim = "";
          _configColorInv = "";
        }
        else {
          // store User colors in User cache
          _configColorReg = GUI_Colors.AsConfigString( COLORSdialog.RegColors );
          _configColorDim = GUI_Colors.AsConfigString( COLORSdialog.DimColors );
          _configColorInv = GUI_Colors.AsConfigString( COLORSdialog.InvColors );
        }
      }
      btColors.ForeColor = this.UsingDefaultColors ? c_entryDefault : c_entryAvailable;
    }

    // change Profile Colors
    private void btProfileColors_Click( object sender, EventArgs e )
    {
      // prep dialog from cached values (user may open this dialog multiple times in a config session)
      var profile = ConfigCopy.ProfileAt( _selProfileIndex );

      // use AppDefaults as fallback
      COLORSdialog.DefaultRegColors = GUI_Colors.GetDefaultColorSet( ColorSet.BrightSet );
      COLORSdialog.DefaultDimColors = GUI_Colors.GetDefaultColorSet( ColorSet.DimmedSet );
      COLORSdialog.DefaultInvColors = GUI_Colors.GetDefaultColorSet( ColorSet.InverseSet );
      // set from profile
      COLORSdialog.RegColors = profile.ColorSetReg.Clone( );
      COLORSdialog.DimColors = profile.ColorSetDim.Clone( );
      COLORSdialog.InvColors = profile.ColorSetInv.Clone( );

      if (COLORSdialog.ShowDialog( this ) == DialogResult.OK) {
        if (COLORSdialog.UsingDefault) {
          profile.SetDefaultColors( );

        }
        else {
          // store colors in profile cache
          profile.SetColorsFromValues(
           GUI_Colors.AsConfigString( COLORSdialog.RegColors ),
           GUI_Colors.AsConfigString( COLORSdialog.DimColors ),
           GUI_Colors.AsConfigString( COLORSdialog.InvColors )
         );
        }
        btProfileColors.ForeColor = profile.UsingDefaultColors ? c_entryDefault : c_entryAvailable;
      }
    }

    #endregion

    #region BG Image Dialog

    // background image dialog called
    private void btBgFile_Click( object sender, EventArgs e )
    {
      var BGF = new frmBgImage( ) {
        BgImageFile = _pBgImageName.Text,
        BgImageBorderArea = _bgImageBorder
      };
      if (BGF.ShowDialog( this ) == DialogResult.OK) {
        // store for later use
        _pBgImageName.Text = BGF.BgImageFile;
        _bgImageBorder = BGF.BgImageBorderArea;
        _pBgImageBorder.Text = _bgImageBorder.ToString( );
      }
      BGF.Close( );
      BGF.Dispose( );
    }

    #endregion

    #region TAB handling

    // Select a profile (opt out of caching when needed)
    private void SelectProfileIndex( int profileIndex, bool cache )
    {
      // use current sel to revert mark
      _pButtons[_selProfileIndex].BackColor = (_selProfileIndex == ConfigCopy.CurrentProfileIndex) ? c_profileColSel : c_profileCol;
      if (cache) {
        // store current
        UpdateProfile( _selProfileIndex );
      }

      // then switch
      _selProfileIndex = profileIndex;
      _pButtons[_selProfileIndex].BackColor = c_profileColCurrent; // set new current mark
      InitProfile( );
    }

    // Profile button clicked (acts like a Tab)
    private void btPN_Click( object sender, EventArgs e )
    {
      var bt = (sender as Button);
      int selIndex = (int)bt.Tag;
      SelectProfileIndex( selIndex, true );
    }

    #endregion


    private void timer1_Tick( object sender, EventArgs e )
    {
      this.BringToFront( ); // to reveal the Config Dialog, workaround issue with window stacks
    }

  }
}

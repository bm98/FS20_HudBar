using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.Bar;

namespace FS20_HudBar.Config
{
  public partial class frmConfig : Form
  {

    internal IList<CProfile> ProfilesRef { get; set; } = null;

    internal HudBar HudBarRef { get; set; } = null;

    internal int SelectedProfile { get; set; } = 0;

    private FlowLayoutPanel[] m_flps = new FlowLayoutPanel[5];

    private FlpHandler[] m_flpHandler = new FlpHandler[5];


    private bool initDone = false;

    public frmConfig( )
    {
      initDone = false;
      InitializeComponent( );

      this.Text = "Hud Bar Configuration - Instance: " + ( string.IsNullOrEmpty( Program.Instance ) ? "Default" : Program.Instance );

      ctxMenu.Items.Add( CProfile.GetDefaultProfile( CProfile.DProfile.Profile_All ).Name, null, ctxDP_Click );
      ctxMenu.Items.Add( CProfile.GetDefaultProfile( CProfile.DProfile.EssentialsProfile ).Name, null, ctxDP_Click );
      ctxMenu.Items.Add( CProfile.GetDefaultProfile( CProfile.DProfile.CombProfile_A ).Name, null, ctxDP_Click );
      ctxMenu.Items.Add( CProfile.GetDefaultProfile( CProfile.DProfile.CombProfile_B ).Name, null, ctxDP_Click );
      ctxMenu.Items.Add( CProfile.GetDefaultProfile( CProfile.DProfile.CombProfile_C ).Name, null, ctxDP_Click );
      ctxMenu.Items.Add( CProfile.GetDefaultProfile( CProfile.DProfile.TPropProfile_A ).Name, null, ctxDP_Click );
      ctxMenu.Items.Add( CProfile.GetDefaultProfile( CProfile.DProfile.TPropProfile_C208B ).Name, null, ctxDP_Click );
      ctxMenu.Items.Add( CProfile.GetDefaultProfile( CProfile.DProfile.JetProfile ).Name, null, ctxDP_Click );
      ctxMenu.Items.Add( CProfile.GetDefaultProfile( CProfile.DProfile.HeavyProfile ).Name, null, ctxDP_Click );
    }

    // fill the list with items and check them from the Instance
    private void PopulateVoiceCallouts( )
    {
      clbVoice.Items.Clear( );
      foreach ( var vt in HudBarRef.VoicePack.Triggers ) {
        var idx = clbVoice.Items.Add( vt.Name );
        clbVoice.SetItemChecked( idx, vt.Enabled );
      }
    }

    // Save the new Checked State in the Instance and save them in Settings
    private void SaveVoiceCallouts( )
    {
      int idx = 0;
      foreach ( var vt in HudBarRef.VoicePack.Triggers ) {
        vt.Enabled = clbVoice.GetItemChecked( idx++ );
      }
      HudBarRef.VoicePack.SaveSettings( );
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
    }

    private void PopulatePlacement( ComboBox cbx )
    {
      cbx.Items.Clear( );
      cbx.Items.Add( GUI.Placement.Bottom + " bound" );
      cbx.Items.Add( GUI.Placement.Left + " bound" );
      cbx.Items.Add( GUI.Placement.Right + " bound" );
      cbx.Items.Add( GUI.Placement.Top + " bound" );
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
      foreach ( var vn in GUI.GUI_Speech.AvailableVoices ) {
        cbx.Items.Add( vn );
      }
    }

    // select the current voice from settings
    public void LoadVoice( ComboBox cbx )
    {
      if ( cbx.Items.Contains( HudBarRef.VoiceName ) )
        cbx.SelectedItem = HudBarRef.VoiceName;
      else if ( cbx.Items.Count > 0 ) {
        cbx.SelectedIndex = 0;
      }
      else {
        // no voices installed...
      }
    }

    // LOAD is called on any invocation of the Dialog
    private void frmConfig_Load( object sender, EventArgs e )
    {
      this.TopMost = false;

      if ( HudBarRef == null ) return; // sanity ..
      if ( ProfilesRef?.Count < 5 ) return;// sanity ..

      // indexed access to the LayoutPanels
      m_flps[0] = flp1; m_flps[1] = flp2; m_flps[2] = flp3; m_flps[3] = flp4; m_flps[4] = flp5;

      cbxUnits.Checked = HudBarRef.ShowUnits;
      cbxFltSave.Checked = HudBarRef.FltAutoSave; // 20210821

      PopulateVoice( cbxVoice );// 20211006
      LoadVoice( cbxVoice );
      speech.SetVoice( cbxVoice.SelectedItem.ToString( ) );

      PopulateVoiceCallouts( ); // 20211018

      int p = 0;
      // Per profile
      txP1.Text = ProfilesRef[p].PName;
      //ProfilesRef[p].LoadFlp( flp1, HudBarRef );
      m_flpHandler[p] = new FlpHandler( m_flps[p], p + 1,
                            ProfilesRef[p].ProfileString( ), ProfilesRef[p].FlowBreakString( ), ProfilesRef[p].ItemPosString( ) );
      m_flpHandler[p].LoadFlp( HudBarRef );
      PopulateFonts( cbxFontP1 );
      ProfilesRef[p].LoadFontSize( cbxFontP1 );
      PopulatePlacement( cbxPlaceP1 );
      ProfilesRef[p].LoadPlacement( cbxPlaceP1 );
      PopulateKind( cbxKindP1 );
      ProfilesRef[p].LoadKind( cbxKindP1 );
      PopulateCond( cbxCondP1 );
      ProfilesRef[p].LoadCond( cbxCondP1 );
      PopulateTrans( cbxTrans1 );
      ProfilesRef[p].LoadTrans( cbxTrans1 );

      p++;
      txP2.Text = ProfilesRef[p].PName;
      //ProfilesRef[p].LoadFlp( flp2, HudBarRef );
      m_flpHandler[p] = new FlpHandler( m_flps[p], p + 1,
                            ProfilesRef[p].ProfileString( ), ProfilesRef[p].FlowBreakString( ), ProfilesRef[p].ItemPosString( ) );
      m_flpHandler[p].LoadFlp( HudBarRef );
      PopulateFonts( cbxFontP2 );
      ProfilesRef[p].LoadFontSize( cbxFontP2 );
      PopulatePlacement( cbxPlaceP2 );
      ProfilesRef[p].LoadPlacement( cbxPlaceP2 );
      PopulateKind( cbxKindP2 );
      ProfilesRef[p].LoadKind( cbxKindP2 );
      PopulateCond( cbxCondP2 );
      ProfilesRef[p].LoadCond( cbxCondP2 );
      PopulateTrans( cbxTrans2 );
      ProfilesRef[p].LoadTrans( cbxTrans2 );

      p++;
      txP3.Text = ProfilesRef[p].PName;
      //ProfilesRef[p].LoadFlp( flp3, HudBarRef );
      m_flpHandler[p] = new FlpHandler( m_flps[p], p + 1,
                            ProfilesRef[p].ProfileString( ), ProfilesRef[p].FlowBreakString( ), ProfilesRef[p].ItemPosString( ) );
      m_flpHandler[p].LoadFlp( HudBarRef );
      PopulateFonts( cbxFontP3 );
      ProfilesRef[p].LoadFontSize( cbxFontP3 );
      PopulatePlacement( cbxPlaceP3 );
      ProfilesRef[p].LoadPlacement( cbxPlaceP3 );
      PopulateKind( cbxKindP3 );
      ProfilesRef[p].LoadKind( cbxKindP3 );
      PopulateCond( cbxCondP3 );
      ProfilesRef[p].LoadCond( cbxCondP3 );
      PopulateTrans( cbxTrans3 );
      ProfilesRef[p].LoadTrans( cbxTrans3 );

      p++;
      txP4.Text = ProfilesRef[p].PName;
      //ProfilesRef[p].LoadFlp( flp4, HudBarRef );
      m_flpHandler[p] = new FlpHandler( m_flps[p], p + 1,
                              ProfilesRef[p].ProfileString( ), ProfilesRef[p].FlowBreakString( ), ProfilesRef[p].ItemPosString( ) );
      m_flpHandler[p].LoadFlp( HudBarRef );
      PopulateFonts( cbxFontP4 );
      ProfilesRef[p].LoadFontSize( cbxFontP4 );
      PopulatePlacement( cbxPlaceP4 );
      ProfilesRef[p].LoadPlacement( cbxPlaceP4 );
      PopulateKind( cbxKindP4 );
      ProfilesRef[p].LoadKind( cbxKindP4 );
      PopulateCond( cbxCondP4 );
      ProfilesRef[p].LoadCond( cbxCondP4 );
      PopulateTrans( cbxTrans4 );
      ProfilesRef[p].LoadTrans( cbxTrans4 );

      p++;
      txP5.Text = ProfilesRef[p].PName;
      //ProfilesRef[p].LoadFlp( flp5, HudBarRef );
      m_flpHandler[p] = new FlpHandler( m_flps[p], p + 1,
                                ProfilesRef[p].ProfileString( ), ProfilesRef[p].FlowBreakString( ), ProfilesRef[p].ItemPosString( ) );
      m_flpHandler[p].LoadFlp( HudBarRef );
      PopulateFonts( cbxFontP5 );
      ProfilesRef[p].LoadFontSize( cbxFontP5 );
      PopulatePlacement( cbxPlaceP5 );
      ProfilesRef[p].LoadPlacement( cbxPlaceP5 );
      PopulateKind( cbxKindP5 );
      ProfilesRef[p].LoadKind( cbxKindP5 );
      PopulateCond( cbxCondP5 );
      ProfilesRef[p].LoadCond( cbxCondP5 );
      PopulateTrans( cbxTrans5 );
      ProfilesRef[p].LoadTrans( cbxTrans5 );

      // mark the selected one 
      switch ( SelectedProfile ) {
        case 0: txP1.BackColor = Color.LimeGreen; break;
        case 1: txP2.BackColor = Color.LimeGreen; break;
        case 2: txP3.BackColor = Color.LimeGreen; break;
        case 3: txP4.BackColor = Color.LimeGreen; break;
        case 4: txP5.BackColor = Color.LimeGreen; break;
        default: break;
      }

      initDone = true;

    }

    private void frmConfig_FormClosing( object sender, FormClosingEventArgs e )
    {
      // reset Sel Color
      txP1.BackColor = this.BackColor;
      txP2.BackColor = this.BackColor;
      txP3.BackColor = this.BackColor;
      txP4.BackColor = this.BackColor;
      txP5.BackColor = this.BackColor;
    }

    private void btCancel_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel;
      this.Close( );
    }

    private void btAccept_Click( object sender, EventArgs e )
    {
      // update from edits
      // live update to HUD
      HudBarRef.SetShowUnits( cbxUnits.Checked );
      HudBarRef.SetFltAutoSave( cbxFltSave.Checked );
      HudBarRef.SetVoiceName( cbxVoice.SelectedItem.ToString( ) );
      SaveVoiceCallouts( ); // 20211018 - save voice callouts

      // profile Updates
      int p = 0;
      ProfilesRef[p].PName = txP1.Text.Trim();
      ProfilesRef[p].GetItemsFromFlp( m_flps[p] );
      ProfilesRef[p].GetFontSizeFromCombo( cbxFontP1 );
      ProfilesRef[p].GetPlacementFromCombo( cbxPlaceP1 );
      ProfilesRef[p].GetKindFromCombo( cbxKindP1 );
      ProfilesRef[p].GetCondFromCombo( cbxCondP1 );
      ProfilesRef[p].GetTramsFromCombo( cbxTrans1 );

      p++;
      ProfilesRef[p].PName = txP2.Text.Trim( );
      ProfilesRef[p].GetItemsFromFlp( m_flps[p] );
      ProfilesRef[p].GetFontSizeFromCombo( cbxFontP2 );
      ProfilesRef[p].GetPlacementFromCombo( cbxPlaceP2 );
      ProfilesRef[p].GetKindFromCombo( cbxKindP2 );
      ProfilesRef[p].GetCondFromCombo( cbxCondP2 );
      ProfilesRef[p].GetTramsFromCombo( cbxTrans2 );

      p++;
      ProfilesRef[p].PName = txP3.Text.Trim( );
      ProfilesRef[p].GetItemsFromFlp( m_flps[p] );
      ProfilesRef[p].GetFontSizeFromCombo( cbxFontP3 );
      ProfilesRef[p].GetPlacementFromCombo( cbxPlaceP3 );
      ProfilesRef[p].GetKindFromCombo( cbxKindP3 );
      ProfilesRef[p].GetCondFromCombo( cbxCondP3 );
      ProfilesRef[p].GetTramsFromCombo( cbxTrans3 );

      p++;
      ProfilesRef[p].PName = txP4.Text.Trim( );
      ProfilesRef[p].GetItemsFromFlp( m_flps[p] );
      ProfilesRef[p].GetFontSizeFromCombo( cbxFontP4 );
      ProfilesRef[p].GetPlacementFromCombo( cbxPlaceP4 );
      ProfilesRef[p].GetKindFromCombo( cbxKindP4 );
      ProfilesRef[p].GetCondFromCombo( cbxCondP4 );
      ProfilesRef[p].GetTramsFromCombo( cbxTrans4 );

      p++;
      ProfilesRef[p].PName = txP5.Text.Trim( );
      ProfilesRef[p].GetItemsFromFlp( m_flps[p] );
      ProfilesRef[p].GetFontSizeFromCombo( cbxFontP5 );
      ProfilesRef[p].GetPlacementFromCombo( cbxPlaceP5 );
      ProfilesRef[p].GetKindFromCombo( cbxKindP5 );
      ProfilesRef[p].GetCondFromCombo( cbxCondP5 );
      ProfilesRef[p].GetTramsFromCombo( cbxTrans5 );

      this.DialogResult = DialogResult.OK;
      this.Close( );
    }

    // local instance for tests
    private GUI.GUI_Speech speech = new GUI.GUI_Speech();

    private void cbxVoice_SelectedIndexChanged( object sender, EventArgs e )
    {
      speech.SetVoice( cbxVoice.SelectedItem.ToString( ) );
    }
    private void cbxVoice_MouseClick( object sender, MouseEventArgs e )
    {
      if ( cbxVoice.DroppedDown ) return;
      speech.SetVoice( cbxVoice.SelectedItem.ToString( ) );
      speech.SaySynched( 100 );
    }

    private void clbVoice_SelectedIndexChanged( object sender, EventArgs e )
    {
      if ( clbVoice.SelectedIndex < 0 ) return;
      if ( !clbVoice.GetItemChecked( clbVoice.SelectedIndex ) ) return;
      if ( !initDone ) return; // don't talk at startup

      // Test when checked
      HudBarRef.VoicePack.Triggers[clbVoice.SelectedIndex].Test( speech );
    }

    #region Context Menu

    private CProfile.DefaultProfile m_copyBuffer;
    private void ctxCopy_Click( object sender, EventArgs e )
    {
      var ctx = (sender as ToolStripItem).Owner as ContextMenuStrip;
      var col = tlp.GetColumn(ctx.SourceControl);
      if ( col > m_flpHandler.Length ) return; // sanity
      m_copyBuffer = m_flpHandler[col].GetItemsFromFlp( );
    }

    private void ctxPaste_Click( object sender, EventArgs e )
    {
      var ctx = (sender as ToolStripItem).Owner as ContextMenuStrip;
      var col = tlp.GetColumn(ctx.SourceControl);
      if ( col > m_flpHandler.Length ) return; // sanity

      if ( m_copyBuffer != null ) {
        m_flpHandler[col].LoadDefaultProfile( m_copyBuffer );
        m_flpHandler[col].LoadFlp( HudBarRef );
      }
    }

    private void ctxDP_Click( object sender, EventArgs e )
    {
      var tsi = (sender as ToolStripItem);
      var ctx = (sender as ToolStripItem).Owner as ContextMenuStrip;
      var col = tlp.GetColumn(ctx.SourceControl);
      if ( col > m_flpHandler.Length ) return; // sanity

      var dp = CProfile.GetDefaultProfile( tsi.Text );
      if ( dp != null ) {
        m_flpHandler[col].LoadDefaultProfile( dp );
        m_flpHandler[col].LoadFlp( HudBarRef );
      }
    }
    #endregion

  }
}

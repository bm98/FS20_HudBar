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

namespace FS20_HudBar
{
  public partial class frmConfig : Form
  {

    internal IList<CProfile> ProfilesRef { get; set; } = null;

    internal HudBar HudBarRef { get; set; } = null;

    internal int SelectedProfile { get; set; } = 0;

    private bool initDone = false;

    public frmConfig( )
    {
      initDone = false;
      InitializeComponent( );

      this.Text = "Hud Bar Configuration - Instance: " + ( string.IsNullOrEmpty( Program.Instance ) ? "Default" : Program.Instance );
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


    private void frmConfig_Load( object sender, EventArgs e )
    {
      this.TopMost = false;

      if ( HudBarRef == null ) return; // sanity ..
      if ( ProfilesRef?.Count < 5 ) return;// sanity ..


      cbxUnits.Checked = HudBarRef.ShowUnits;
      cbxFltSave.Checked = HudBarRef.FltAutoSave; // 20210821

      PopulateVoice( cbxVoice );// 20211006
      LoadVoice( cbxVoice );
      speech.SetVoice( cbxVoice.SelectedItem.ToString( ) );

      PopulateVoiceCallouts( ); // 20211018

      // Per profile
      txP1.Text = ProfilesRef[0].PName;
      ProfilesRef[0].LoadFlp( flp1, HudBarRef );
      PopulateFonts( cbxFontP1 );
      ProfilesRef[0].LoadFontSize( cbxFontP1 );
      PopulatePlacement( cbxPlaceP1 );
      ProfilesRef[0].LoadPlacement( cbxPlaceP1 );
      PopulateKind( cbxKindP1 );
      ProfilesRef[0].LoadKind( cbxKindP1 );
      PopulateCond( cbxCondP1 );
      ProfilesRef[0].LoadCond( cbxCondP1 );
      PopulateTrans( cbxTrans1 );
      ProfilesRef[0].LoadTrans( cbxTrans1 );

      txP2.Text = ProfilesRef[1].PName;
      ProfilesRef[1].LoadFlp( flp2, HudBarRef );
      PopulateFonts( cbxFontP2 );
      ProfilesRef[1].LoadFontSize( cbxFontP2 );
      PopulatePlacement( cbxPlaceP2 );
      ProfilesRef[1].LoadPlacement( cbxPlaceP2 );
      PopulateKind( cbxKindP2 );
      ProfilesRef[1].LoadKind( cbxKindP2 );
      PopulateCond( cbxCondP2 );
      ProfilesRef[1].LoadCond( cbxCondP2 );
      PopulateTrans( cbxTrans2 );
      ProfilesRef[1].LoadTrans( cbxTrans2 );

      txP3.Text = ProfilesRef[2].PName;
      ProfilesRef[2].LoadFlp( flp3, HudBarRef );
      PopulateFonts( cbxFontP3 );
      ProfilesRef[2].LoadFontSize( cbxFontP3 );
      PopulatePlacement( cbxPlaceP3 );
      ProfilesRef[2].LoadPlacement( cbxPlaceP3 );
      PopulateKind( cbxKindP3 );
      ProfilesRef[2].LoadKind( cbxKindP3 );
      PopulateCond( cbxCondP3 );
      ProfilesRef[2].LoadCond( cbxCondP3 );
      PopulateTrans( cbxTrans3 );
      ProfilesRef[2].LoadTrans( cbxTrans3 );

      txP4.Text = ProfilesRef[3].PName;
      ProfilesRef[3].LoadFlp( flp4, HudBarRef );
      PopulateFonts( cbxFontP4 );
      ProfilesRef[3].LoadFontSize( cbxFontP4 );
      PopulatePlacement( cbxPlaceP4 );
      ProfilesRef[3].LoadPlacement( cbxPlaceP4 );
      PopulateKind( cbxKindP4 );
      ProfilesRef[3].LoadKind( cbxKindP4 );
      PopulateCond( cbxCondP4 );
      ProfilesRef[3].LoadCond( cbxCondP4 );
      PopulateTrans( cbxTrans4 );
      ProfilesRef[3].LoadTrans( cbxTrans4 );

      txP5.Text = ProfilesRef[4].PName;
      ProfilesRef[4].LoadFlp( flp5, HudBarRef );
      PopulateFonts( cbxFontP5 );
      ProfilesRef[4].LoadFontSize( cbxFontP5 );
      PopulatePlacement( cbxPlaceP5 );
      ProfilesRef[4].LoadPlacement( cbxPlaceP5 );
      PopulateKind( cbxKindP5 );
      ProfilesRef[4].LoadKind( cbxKindP5 );
      PopulateCond( cbxCondP5 );
      ProfilesRef[4].LoadCond( cbxCondP5 );
      PopulateTrans( cbxTrans5 );
      ProfilesRef[4].LoadTrans( cbxTrans5 );

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
      ProfilesRef[0].PName = txP1.Text;
      ProfilesRef[0].GetItemsFromFlp( flp1 );
      ProfilesRef[0].GetFontSizeFromCombo( cbxFontP1 );
      ProfilesRef[0].GetPlacementFromCombo( cbxPlaceP1 );
      ProfilesRef[0].GetKindFromCombo( cbxKindP1 );
      ProfilesRef[0].GetCondFromCombo( cbxCondP1 );
      ProfilesRef[0].GetTramsFromCombo( cbxTrans1 );

      ProfilesRef[1].PName = txP2.Text;
      ProfilesRef[1].GetItemsFromFlp( flp2 );
      ProfilesRef[1].GetFontSizeFromCombo( cbxFontP2 );
      ProfilesRef[1].GetPlacementFromCombo( cbxPlaceP2 );
      ProfilesRef[1].GetKindFromCombo( cbxKindP2 );
      ProfilesRef[1].GetCondFromCombo( cbxCondP2 );
      ProfilesRef[1].GetTramsFromCombo( cbxTrans2 );

      ProfilesRef[2].PName = txP3.Text;
      ProfilesRef[2].GetItemsFromFlp( flp3 );
      ProfilesRef[2].GetFontSizeFromCombo( cbxFontP3 );
      ProfilesRef[2].GetPlacementFromCombo( cbxPlaceP3 );
      ProfilesRef[2].GetKindFromCombo( cbxKindP3 );
      ProfilesRef[2].GetCondFromCombo( cbxCondP3 );
      ProfilesRef[2].GetTramsFromCombo( cbxTrans3 );

      ProfilesRef[3].PName = txP4.Text;
      ProfilesRef[3].GetItemsFromFlp( flp4 );
      ProfilesRef[3].GetFontSizeFromCombo( cbxFontP4 );
      ProfilesRef[3].GetPlacementFromCombo( cbxPlaceP4 );
      ProfilesRef[3].GetKindFromCombo( cbxKindP4 );
      ProfilesRef[3].GetCondFromCombo( cbxCondP4 );
      ProfilesRef[3].GetTramsFromCombo( cbxTrans4 );

      ProfilesRef[4].PName = txP5.Text;
      ProfilesRef[4].GetItemsFromFlp( flp5 );
      ProfilesRef[4].GetFontSizeFromCombo( cbxFontP5 );
      ProfilesRef[4].GetPlacementFromCombo( cbxPlaceP5 );
      ProfilesRef[4].GetKindFromCombo( cbxKindP5 );
      ProfilesRef[4].GetCondFromCombo( cbxCondP5 );
      ProfilesRef[4].GetTramsFromCombo( cbxTrans5 );

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

  }

}

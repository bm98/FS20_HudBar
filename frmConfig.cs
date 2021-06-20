using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar
{
  public partial class frmConfig : Form
  {

    internal IList<CProfile> ProfilesRef { get; set; } = null;

    internal HudBar HudBarRef { get; set; } = null;

    public frmConfig( )
    {
      InitializeComponent( );
    }


    private void PopulateFonts( ComboBox cbxFont )
    {
      cbxFont.Items.Clear( );
      cbxFont.Items.Add( GUI.FontSize.Regular + " Font Size" );
      cbxFont.Items.Add( GUI.FontSize.Plus_2 + " Font Size" );
      cbxFont.Items.Add( GUI.FontSize.Plus_4 + " Font Size" );
      cbxFont.Items.Add( GUI.FontSize.Plus_6 + " Font Size" );
      cbxFont.Items.Add( GUI.FontSize.Plus_8 + " Font Size" );
      cbxFont.Items.Add( GUI.FontSize.Plus_10 + " Font Size" );
      cbxFont.Items.Add( GUI.FontSize.Minus_2 + " Font Size" );
      cbxFont.Items.Add( GUI.FontSize.Minus_4 + " Font Size" );
    }

    private void PopulatePlacement( ComboBox cbxPlace )
    {
      cbxPlace.Items.Clear( );
      cbxPlace.Items.Add( GUI.Placement.Bottom + " bound" );
      cbxPlace.Items.Add( GUI.Placement.Left + " bound" );
      cbxPlace.Items.Add( GUI.Placement.Right + " bound" );
      cbxPlace.Items.Add( GUI.Placement.Top + " bound" );
    }


    private void frmConfig_Load( object sender, EventArgs e )
    {
      this.TopMost = false;

      if ( HudBarRef == null ) return; // sanity ..
      if ( ProfilesRef?.Count < 5 ) return;// sanity ..

      cbxUnits.Checked = HudBarRef.ShowUnits;
      cbxOpaque.Checked = HudBarRef.OpaqueBackground;

      // Per profile
      txP1.Text = ProfilesRef[0].PName;
      ProfilesRef[0].LoadCbx( clbxP1, HudBarRef );
      PopulateFonts( cbxFontP1 );
      ProfilesRef[0].LoadFontSize( cbxFontP1 );
      PopulatePlacement( cbxPlaceP1 );
      ProfilesRef[0].LoadPlacement( cbxPlaceP1 );

      txP2.Text = ProfilesRef[1].PName;
      ProfilesRef[1].LoadCbx( clbxP2, HudBarRef );
      PopulateFonts( cbxFontP2 );
      ProfilesRef[1].LoadFontSize( cbxFontP2 );
      PopulatePlacement( cbxPlaceP2 );
      ProfilesRef[1].LoadPlacement( cbxPlaceP2 );

      txP3.Text = ProfilesRef[2].PName;
      ProfilesRef[2].LoadCbx( clbxP3, HudBarRef );
      PopulateFonts( cbxFontP3 );
      ProfilesRef[2].LoadFontSize( cbxFontP3 );
      PopulatePlacement( cbxPlaceP3 );
      ProfilesRef[2].LoadPlacement( cbxPlaceP3 );

      txP4.Text = ProfilesRef[3].PName;
      ProfilesRef[3].LoadCbx( clbxP4, HudBarRef );
      PopulateFonts( cbxFontP4 );
      ProfilesRef[3].LoadFontSize( cbxFontP4 );
      PopulatePlacement( cbxPlaceP4 );
      ProfilesRef[3].LoadPlacement( cbxPlaceP4 );

      txP5.Text = ProfilesRef[4].PName;
      ProfilesRef[4].LoadCbx( clbxP5, HudBarRef );
      PopulateFonts( cbxFontP5 );
      ProfilesRef[4].LoadFontSize( cbxFontP5 );
      PopulatePlacement( cbxPlaceP5 );
      ProfilesRef[4].LoadPlacement( cbxPlaceP5 );
    }

    private void frmConfig_FormClosing( object sender, FormClosingEventArgs e )
    {
    }

    private void btCancel_Click( object sender, EventArgs e )
    {

      this.DialogResult = DialogResult.Cancel;
      this.Close( );
    }

    private void btAccept_Click( object sender, EventArgs e )
    {
      // update from edits
      HudBarRef.ShowUnits = cbxUnits.Checked;
      HudBarRef.OpaqueBackground = cbxOpaque.Checked;

      ProfilesRef[0].PName = txP1.Text;
      ProfilesRef[0].GetItemsFromCbx( clbxP1 );
      ProfilesRef[0].GetFontSizeFromCombo( cbxFontP1 );
      ProfilesRef[0].GetPlacementFromCombo( cbxPlaceP1 );

      ProfilesRef[1].PName = txP2.Text;
      ProfilesRef[1].GetItemsFromCbx( clbxP2);
      ProfilesRef[1].GetFontSizeFromCombo( cbxFontP2 );
      ProfilesRef[1].GetPlacementFromCombo( cbxPlaceP2 );

      ProfilesRef[2].PName = txP3.Text;
      ProfilesRef[2].GetItemsFromCbx( clbxP3);
      ProfilesRef[2].GetFontSizeFromCombo( cbxFontP3 );
      ProfilesRef[2].GetPlacementFromCombo( cbxPlaceP3 );

      ProfilesRef[3].PName = txP4.Text;
      ProfilesRef[3].GetItemsFromCbx( clbxP4);
      ProfilesRef[3].GetFontSizeFromCombo( cbxFontP4 );
      ProfilesRef[3].GetPlacementFromCombo( cbxPlaceP4 );

      ProfilesRef[4].PName = txP5.Text;
      ProfilesRef[4].GetItemsFromCbx( clbxP5);
      ProfilesRef[4].GetFontSizeFromCombo( cbxFontP5 );
      ProfilesRef[4].GetPlacementFromCombo( cbxPlaceP5 );

      this.DialogResult = DialogResult.OK;
      this.Close( );
    }



  }
}

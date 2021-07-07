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

    internal int SelectedProfile { get; set; } = 0;


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
      cbxFont.Items.Add( GUI.FontSize.Plus_12 + " Font Size" );
      cbxFont.Items.Add( GUI.FontSize.Plus_14 + " Font Size" );
    }

    private void PopulatePlacement( ComboBox cbxPlace )
    {
      cbxPlace.Items.Clear( );
      cbxPlace.Items.Add( GUI.Placement.Bottom + " bound" );
      cbxPlace.Items.Add( GUI.Placement.Left + " bound" );
      cbxPlace.Items.Add( GUI.Placement.Right + " bound" );
      cbxPlace.Items.Add( GUI.Placement.Top + " bound" );
    }

    private void PopulateKind( ComboBox cbxPlace )
    {
      cbxPlace.Items.Clear( );
      cbxPlace.Items.Add( "Bar" );
      cbxPlace.Items.Add( "Tile" );
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
      ProfilesRef[0].LoadFlp( flp1, HudBarRef );
      PopulateFonts( cbxFontP1 );
      ProfilesRef[0].LoadFontSize( cbxFontP1 );
      PopulatePlacement( cbxPlaceP1 );
      ProfilesRef[0].LoadPlacement( cbxPlaceP1 );
      PopulateKind( cbxKindP1 );
      ProfilesRef[0].LoadKind( cbxKindP1 );

      txP2.Text = ProfilesRef[1].PName;
      ProfilesRef[1].LoadFlp( flp2, HudBarRef );
      PopulateFonts( cbxFontP2 );
      ProfilesRef[1].LoadFontSize( cbxFontP2 );
      PopulatePlacement( cbxPlaceP2 );
      ProfilesRef[1].LoadPlacement( cbxPlaceP2 );
      PopulateKind( cbxKindP2 );
      ProfilesRef[1].LoadKind( cbxKindP2 );

      txP3.Text = ProfilesRef[2].PName;
      ProfilesRef[2].LoadFlp( flp3, HudBarRef );
      PopulateFonts( cbxFontP3 );
      ProfilesRef[2].LoadFontSize( cbxFontP3 );
      PopulatePlacement( cbxPlaceP3 );
      ProfilesRef[2].LoadPlacement( cbxPlaceP3 );
      PopulateKind( cbxKindP3 );
      ProfilesRef[2].LoadKind( cbxKindP3 );

      txP4.Text = ProfilesRef[3].PName;
      ProfilesRef[3].LoadFlp( flp4, HudBarRef );
      PopulateFonts( cbxFontP4 );
      ProfilesRef[3].LoadFontSize( cbxFontP4 );
      PopulatePlacement( cbxPlaceP4 );
      ProfilesRef[3].LoadPlacement( cbxPlaceP4 );
      PopulateKind( cbxKindP4 );
      ProfilesRef[3].LoadKind( cbxKindP4 );

      txP5.Text = ProfilesRef[4].PName;
      ProfilesRef[4].LoadFlp( flp5, HudBarRef );
      PopulateFonts( cbxFontP5 );
      ProfilesRef[4].LoadFontSize( cbxFontP5 );
      PopulatePlacement( cbxPlaceP5 );
      ProfilesRef[4].LoadPlacement( cbxPlaceP5 );
      PopulateKind( cbxKindP5 );
      ProfilesRef[4].LoadKind( cbxKindP5 );

      // mark the selected one 
      switch ( SelectedProfile ) {
        case 0: txP1.BackColor = Color.LimeGreen; break;
        case 1: txP2.BackColor = Color.LimeGreen; break;
        case 2: txP3.BackColor = Color.LimeGreen; break;
        case 3: txP4.BackColor = Color.LimeGreen; break;
        case 4: txP5.BackColor = Color.LimeGreen; break;
        default:  break;
      }
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
      HudBarRef.ShowUnits = cbxUnits.Checked;
      HudBarRef.OpaqueBackground = cbxOpaque.Checked;

      ProfilesRef[0].PName = txP1.Text;
      ProfilesRef[0].GetItemsFromFlp( flp1 );
      ProfilesRef[0].GetFontSizeFromCombo( cbxFontP1 );
      ProfilesRef[0].GetPlacementFromCombo( cbxPlaceP1 );
      ProfilesRef[0].GetKindFromCombo( cbxKindP1 );

      ProfilesRef[1].PName = txP2.Text;
      ProfilesRef[1].GetItemsFromFlp( flp2 );
      ProfilesRef[1].GetFontSizeFromCombo( cbxFontP2 );
      ProfilesRef[1].GetPlacementFromCombo( cbxPlaceP2 );
      ProfilesRef[1].GetKindFromCombo( cbxKindP2 );

      ProfilesRef[2].PName = txP3.Text;
      ProfilesRef[2].GetItemsFromFlp( flp3 );
      ProfilesRef[2].GetFontSizeFromCombo( cbxFontP3 );
      ProfilesRef[2].GetPlacementFromCombo( cbxPlaceP3 );
      ProfilesRef[2].GetKindFromCombo( cbxKindP3 );

      ProfilesRef[3].PName = txP4.Text;
      ProfilesRef[3].GetItemsFromFlp( flp4 );
      ProfilesRef[3].GetFontSizeFromCombo( cbxFontP4 );
      ProfilesRef[3].GetPlacementFromCombo( cbxPlaceP4 );
      ProfilesRef[3].GetKindFromCombo( cbxKindP4 );

      ProfilesRef[4].PName = txP5.Text;
      ProfilesRef[4].GetItemsFromFlp( flp5);
      ProfilesRef[4].GetFontSizeFromCombo( cbxFontP5 );
      ProfilesRef[4].GetPlacementFromCombo( cbxPlaceP5 );
      ProfilesRef[4].GetKindFromCombo( cbxKindP5 );

      this.DialogResult = DialogResult.OK;
      this.Close( );
    }


  }
}

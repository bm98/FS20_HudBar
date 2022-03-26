using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.Config
{


  public partial class FrmFonts : Form
  {

    internal GUI.GUI_Fonts Fonts { get; set; }

    public Label ProtoLabelRef { get; set; }
    public Label ProtoValueRef { get; set; }
    public Label ProtoValue2Ref { get; set; }

    // Should look the same as in the Config Dialog itself
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
      cbx.Items.Add( GUI.FontSize.Plus_18 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_20 + " Font Size" );
      // don't add the largest ones, those may not fit the Dialog width
      /*
      cbx.Items.Add( GUI.FontSize.Plus_24 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_28 + " Font Size" );
      cbx.Items.Add( GUI.FontSize.Plus_32 + " Font Size" );
      */
    }

    // Apply the label props to have a similar appearance as the Bar itself
    private void LabelsSetup( Label target, Label proto )
    {
      target.Font = proto.Font;   
      target.AutoSize = true;                          // force AutoSize
      target.TextAlign = proto.TextAlign;
      target.Anchor = proto.Anchor;
      target.Margin = proto.Margin;
      target.Padding = proto.Padding;
      target.UseCompatibleTextRendering = true;        // make sure the WingDings an other font special chars display properly
      target.Cursor = Cursors.Default;                 // avoid the movement cross on the item controls

      target.BorderStyle = BorderStyle.FixedSingle; // DEBUG
    }


    public FrmFonts( )
    {
      InitializeComponent( );

    }

    private void FrmFonts_Load( object sender, EventArgs e )
    {
      // set Dialog Props
      FD.AllowSimulations = true;
      FD.FontMustExist = true;
      FD.ScriptsOnly = true;

      FD.AllowScriptChange = false;
      FD.AllowVectorFonts = false;
      FD.AllowVerticalFonts = false;
      FD.FixedPitchOnly = false;
      FD.ShowApply = false;
      FD.ShowColor = false;
      FD.ShowEffects = false;

      LabelsSetup( lbLabel1, ProtoLabelRef );
      LabelsSetup( lbNumber11, ProtoValueRef );
      LabelsSetup( lbLabel2, ProtoLabelRef );
      LabelsSetup( lbNumber21, ProtoValue2Ref );
      LabelsSetup( lbNumber22, ProtoValue2Ref );
      LabelsSetup( lbLabel3, ProtoLabelRef );
      LabelsSetup( lbNumber31, ProtoValue2Ref );
      LabelsSetup( lbNumber32, ProtoValue2Ref );

      PopulateFonts( cbxSize );
      cbxSize.SelectedIndex = cbxSize.Items.Count-1; // default to largest

      cxCondensed.Checked = Fonts.Condensed;

      SetFont( );
    }

    private void SetFont( )
    {
      Fonts.SetFontsize( (GUI.FontSize)cbxSize.SelectedIndex );

      lbLabel1.Font = new Font( Fonts.LabelFont, Fonts.LabelFont.Style );
      lbNumber11.Font = new Font( Fonts.ValueFont, Fonts.ValueFont.Style );

      lbLabel2.Font = new Font( Fonts.LabelFont, Fonts.LabelFont.Style );
      lbNumber21.Font = new Font( Fonts.Value2Font, Fonts.Value2Font.Style );
      lbNumber22.Font = new Font( Fonts.Value2Font, Fonts.Value2Font.Style );

      lbLabel3.Font = new Font( Fonts.LabelFont, Fonts.LabelFont.Style );
      lbNumber31.Font = new Font( Fonts.Value2Font, Fonts.Value2Font.Style );
      lbNumber32.Font = new Font( Fonts.Value2Font, Fonts.Value2Font.Style );
    }

    private void ResizeDemoPanel( )
    {
      // reset to use Width from the items
      lbLabel1.Padding = new Padding( 0, 0, 0, 0 );
      lbLabel2.Padding = new Padding( 0, 0, 0, 0 );
      lbLabel3.Padding = new Padding( 0, 0, 0, 0 );

      lbLabel3.Padding = new Padding( 0, 0, lbLabel1.Width - lbLabel3.Width + 8, 0 );
      lbLabel2.Padding = new Padding( 0, 0, lbLabel1.Width - lbLabel2.Width + 8, 0 );
      lbLabel1.Padding = new Padding( 0, 0, 8, 0 );

    }

    private void btLabelFont_Click( object sender, EventArgs e )
    {
      // set Dialog Props
      FD.Font = lbLabel1.Font;
      
      if ( FD.ShowDialog( this ) == DialogResult.OK ) {
        // save stuff
        Fonts.SetUserFont( GUI.GUI_Fonts.FKinds.Lbl, FD.Font, (GUI.FontSize)cbxSize.SelectedIndex, cxCondensed.Checked );
        Fonts.SetFontsize( (GUI.FontSize)cbxSize.SelectedIndex );
        SetFont( );
      }
    }

    private void btValueFont_Click( object sender, EventArgs e )
    {
      FD.Font = lbNumber11.Font;

      if ( FD.ShowDialog( this ) == DialogResult.OK ) {
        // save stuff
        Fonts.SetUserFont( GUI.GUI_Fonts.FKinds.Value, FD.Font, (GUI.FontSize)cbxSize.SelectedIndex, cxCondensed.Checked );
        Fonts.SetUserFont( GUI.GUI_Fonts.FKinds.Value2, FD.Font, (GUI.FontSize)cbxSize.SelectedIndex, cxCondensed.Checked );
        Fonts.SetFontsize( (GUI.FontSize)cbxSize.SelectedIndex );
        SetFont( );
      }
    }

    private void cbxSize_SelectionChangeCommitted( object sender, EventArgs e )
    {
      Fonts.SetFontsize( (GUI.FontSize)cbxSize.SelectedIndex );
      SetFont( );
    }

    private void cxCondensed_CheckedChanged( object sender, EventArgs e )
    {
      Fonts.SetFontCondensed( cxCondensed.Checked );
      SetFont( );
    }

    private void btDefaults_Click( object sender, EventArgs e )
    {
      Fonts.ResetUserFonts( );
      SetFont( );
    }

    private void flpMain_Layout( object sender, LayoutEventArgs e )
    {
      ResizeDemoPanel( );
    }

    private void btAccept_Click( object sender, EventArgs e )
    {
      // Save Stuff and hide
      this.DialogResult = DialogResult.OK;
      this.Hide( );
    }

  }
}

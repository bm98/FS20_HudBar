using FS20_HudBar.GUI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Windows.Devices.Lights.Effects;
using Windows.Networking.Proximity;
using static FS20_HudBar.GUI.GUI_Colors;

namespace FS20_HudBar.Config
{
  /// <summary>
  /// Color Config Dialog
  /// 
  /// Load from current settings
  /// </summary>
  public partial class FrmColors : Form
  {

    private readonly Dictionary<ColorType, Label> _regItems = new Dictionary<ColorType, Label>( );
    private readonly Dictionary<ColorType, Label> _dimItems = new Dictionary<ColorType, Label>( );
    private readonly Dictionary<ColorType, Label> _invItems = new Dictionary<ColorType, Label>( );

    private Label _editedLabel = null;

    /// <summary>
    /// The Regular ColorSet from Config
    /// </summary>
    public GUI_ColorSet RegColors { get; set; } = new GUI_ColorSet( );
    /// <summary>
    /// The Dimmed ColorSet from Config
    /// </summary>
    public GUI_ColorSet DimColors { get; set; } = new GUI_ColorSet( );
    /// <summary>
    /// The Inverse ColorSet from Config
    /// </summary>
    public GUI_ColorSet InvColors { get; set; } = new GUI_ColorSet( );

    // set the background for the ones that use the default background color
    private void SetBackColorReg( Color color )
    {
      _regItems[ColorType.cTxLabel].BackColor = color;
      _regItems[ColorType.cTxInfo].BackColor = color;
      _regItems[ColorType.cTxDim].BackColor = color;
      _regItems[ColorType.cTxAlert].BackColor = color;
      _regItems[ColorType.cTxWarn].BackColor = color;
      _regItems[ColorType.cTxNav].BackColor = color;
      _regItems[ColorType.cTxGps].BackColor = color;
      _regItems[ColorType.cTxEst].BackColor = color;
      _regItems[ColorType.cTxRA].BackColor = color;
      _regItems[ColorType.cTxAvg].BackColor = color;
      _regItems[ColorType.cTxSubZero].BackColor = color;
    }
    private void SetBackColorDim( Color color )
    {
      _dimItems[ColorType.cTxLabel].BackColor = color;
      _dimItems[ColorType.cTxInfo].BackColor = color;
      _dimItems[ColorType.cTxDim].BackColor = color;
      _dimItems[ColorType.cTxAlert].BackColor = color;
      _dimItems[ColorType.cTxWarn].BackColor = color;
      _dimItems[ColorType.cTxNav].BackColor = color;
      _dimItems[ColorType.cTxGps].BackColor = color;
      _dimItems[ColorType.cTxEst].BackColor = color;
      _dimItems[ColorType.cTxRA].BackColor = color;
      _dimItems[ColorType.cTxAvg].BackColor = color;
      _dimItems[ColorType.cTxSubZero].BackColor = color;
    }

    private void SetBackColorInv( Color color )
    {
      _invItems[ColorType.cTxLabel].BackColor = color;
      _invItems[ColorType.cTxInfo].BackColor = color;
      _invItems[ColorType.cTxDim].BackColor = color;
      _invItems[ColorType.cTxAlert].BackColor = color;
      _invItems[ColorType.cTxWarn].BackColor = color;
      _invItems[ColorType.cTxNav].BackColor = color;
      _invItems[ColorType.cTxGps].BackColor = color;
      _invItems[ColorType.cTxEst].BackColor = color;
      _invItems[ColorType.cTxRA].BackColor = color;
      _invItems[ColorType.cTxAvg].BackColor = color;
      _invItems[ColorType.cTxSubZero].BackColor = color;
    }

    // Set Label Colors from color catalogs
    private void SetColors( )
    {
      foreach (var item in _regItems) {
        item.Value.ForeColor = RegColors[item.Key];
      }
      lblRoBG.ForeColor = GUI_Colors.ItemColor( ColorType.cTxLabel, RegColors );
      lblRoBG.BackColor = RegColors[ColorType.cOpaqueBG];
      SetBackColorReg( lblRoBG.BackColor );

      foreach (var item in _dimItems) {
        item.Value.ForeColor = DimColors[item.Key];
      }
      lblDoBG.ForeColor = GUI_Colors.ItemColor( ColorType.cTxLabel, DimColors );
      lblDoBG.BackColor = DimColors[ColorType.cOpaqueBG];
      SetBackColorDim( lblDoBG.BackColor );

      foreach (var item in _invItems) {
        item.Value.ForeColor = InvColors[item.Key];
      }
      lblIoBG.ForeColor = GUI_Colors.ItemColor( ColorType.cTxLabel, InvColors );
      lblIoBG.BackColor = InvColors[ColorType.cOpaqueBG];
      SetBackColorInv( lblIoBG.BackColor );
    }

    // Set Catalog colors from Labels
    private void GetColors( )
    {
      foreach (var item in _regItems) {
        RegColors[item.Key] = item.Value.ForeColor;
      }
      RegColors[ColorType.cOpaqueBG] = lblRoBG.BackColor;

      foreach (var item in _dimItems) {
        DimColors[item.Key] = item.Value.ForeColor;
      }
      DimColors[ColorType.cOpaqueBG] = lblDoBG.BackColor;

      foreach (var item in _invItems) {
        InvColors[item.Key] = item.Value.ForeColor;
      }
      InvColors[ColorType.cOpaqueBG] = lblIoBG.BackColor;
    }

    /// <summary>
    /// cTor: Form
    /// </summary>
    public FrmColors( )
    {
      InitializeComponent( );
      // add labels for indexed access
      _regItems.Add( ColorType.cTxLabel, lblR01 ); _regItems.Add( ColorType.cTxLabelArmed, lblR02 ); _regItems.Add( ColorType.cTxInfo, lblR03 );
      _regItems.Add( ColorType.cTxInfoInverse, lblR04 ); _regItems.Add( ColorType.cTxDim, lblR05 ); _regItems.Add( ColorType.cTxActive, lblR06 );
      _regItems.Add( ColorType.cTxAlert, lblR07 ); _regItems.Add( ColorType.cTxWarn, lblR08 ); _regItems.Add( ColorType.cTxAPActive, lblR09 );
      _regItems.Add( ColorType.cTxSet, lblR10 ); _regItems.Add( ColorType.cTxNav, lblR11 ); _regItems.Add( ColorType.cTxGps, lblR12 );
      _regItems.Add( ColorType.cTxEst, lblR13 ); _regItems.Add( ColorType.cTxRA, lblR14 ); _regItems.Add( ColorType.cTxAvg, lblR15 );
      _regItems.Add( ColorType.cTxSubZero, lblR16 );
      foreach (var lbl in _regItems.Values) { lbl.Click += lbl_Click; }
      lblRoBG.Click += lbl_Click;

      _dimItems.Add( ColorType.cTxLabel, lblD01 ); _dimItems.Add( ColorType.cTxLabelArmed, lblD02 ); _dimItems.Add( ColorType.cTxInfo, lblD03 );
      _dimItems.Add( ColorType.cTxInfoInverse, lblD04 ); _dimItems.Add( ColorType.cTxDim, lblD05 ); _dimItems.Add( ColorType.cTxActive, lblD06 );
      _dimItems.Add( ColorType.cTxAlert, lblD07 ); _dimItems.Add( ColorType.cTxWarn, lblD08 ); _dimItems.Add( ColorType.cTxAPActive, lblD09 );
      _dimItems.Add( ColorType.cTxSet, lblD10 ); _dimItems.Add( ColorType.cTxNav, lblD11 ); _dimItems.Add( ColorType.cTxGps, lblD12 );
      _dimItems.Add( ColorType.cTxEst, lblD13 ); _dimItems.Add( ColorType.cTxRA, lblD14 ); _dimItems.Add( ColorType.cTxAvg, lblD15 );
      _dimItems.Add( ColorType.cTxSubZero, lblD16 );
      foreach (var lbl in _dimItems.Values) { lbl.Click += lbl_Click; }
      lblDoBG.Click += lbl_Click;

      _invItems.Add( ColorType.cTxLabel, lblI01 ); _invItems.Add( ColorType.cTxLabelArmed, lblI02 ); _invItems.Add( ColorType.cTxInfo, lblI03 );
      _invItems.Add( ColorType.cTxInfoInverse, lblI04 ); _invItems.Add( ColorType.cTxDim, lblI05 ); _invItems.Add( ColorType.cTxActive, lblI06 );
      _invItems.Add( ColorType.cTxAlert, lblI07 ); _invItems.Add( ColorType.cTxWarn, lblI08 ); _invItems.Add( ColorType.cTxAPActive, lblI09 );
      _invItems.Add( ColorType.cTxSet, lblI10 ); _invItems.Add( ColorType.cTxNav, lblI11 ); _invItems.Add( ColorType.cTxGps, lblI12 );
      _invItems.Add( ColorType.cTxEst, lblI13 ); _invItems.Add( ColorType.cTxRA, lblI14 ); _invItems.Add( ColorType.cTxAvg, lblI15 );
      _invItems.Add( ColorType.cTxSubZero, lblI16 );
      foreach (var lbl in _invItems.Values) { lbl.Click += lbl_Click; }
      lblIoBG.Click += lbl_Click;
    }

    // Form Load event
    private void FrmColors_Load( object sender, EventArgs e )
    {
      SetColors( );
    }

    // Accept button event
    private void btAccept_Click( object sender, EventArgs e )
    {
      GetColors( );
      this.DialogResult = DialogResult.OK;
      this.Hide( );
    }

    // Cancel button event
    private void btCancel_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel;
      this.Hide( );
    }

    // Use Defaults button event
    private void btDefaults_Click( object sender, EventArgs e )
    {
      RegColors = GUI_Colors.GetDefaultColorSet( ColorSet.BrightSet );
      DimColors = GUI_Colors.GetDefaultColorSet( ColorSet.DimmedSet );
      InvColors = GUI_Colors.GetDefaultColorSet( ColorSet.InverseSet );

      SetColors( );
    }

    // Item Label clicked (edit) event
    private void lbl_Click( object sender, EventArgs e )
    {
      // sanity
      if (!(sender is Label)) return;

      _editedLabel = sender as Label;

      // handle the Background edits
      if (_editedLabel.Name == "lblRoBG") CDLG.Color = _editedLabel.BackColor;
      else if (_editedLabel.Name == "lblDoBG") CDLG.Color = _editedLabel.BackColor;
      else if (_editedLabel.Name == "lblIoBG") CDLG.Color = _editedLabel.BackColor;
      else { CDLG.Color = _editedLabel.ForeColor; }

      if (CDLG.ShowDialog( this ) == DialogResult.OK) {
        if (_editedLabel.Name == "lblRoBG") {
          _editedLabel.BackColor = CDLG.Color;
          SetBackColorReg( _editedLabel.BackColor );
        }
        else if (_editedLabel.Name == "lblDoBG") {
          _editedLabel.BackColor = CDLG.Color;
          SetBackColorDim( _editedLabel.BackColor );
        }
        else if (_editedLabel.Name == "lblIoBG") {
          _editedLabel.BackColor = CDLG.Color;
          SetBackColorInv( _editedLabel.BackColor );
        }
        else {
          _editedLabel.ForeColor = CDLG.Color;
        }
      }
    }

  }
}

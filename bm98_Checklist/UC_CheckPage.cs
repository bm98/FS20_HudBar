using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

using bm98_Checklist.Json;

namespace bm98_Checklist
{
  internal partial class UC_CheckPage : UserControl
  {
    // number of Phases
    private const int c_numPhase = 8;
    // number of Checks / Phase
    private const int c_numChecks = 10;

    // Access for our Controls
    private List<TextBox[]> _pTxt = new List<TextBox[]>( );
    private TextBox[] _pName = new TextBox[c_numPhase];
    private CheckBox[] _pEnabled = new CheckBox[c_numPhase];

    // Empty all Checkbox Texts, disable all phases, clear Name
    private void ClearChecks( )
    {
      txCListName.Text = "";
      foreach (var t in _pTxt[0]) t.Text = ""; chkPhaseA.Checked = false; txPhaseA.Text = "";
      foreach (var t in _pTxt[1]) t.Text = ""; chkPhaseB.Checked = false; txPhaseB.Text = "";
      foreach (var t in _pTxt[2]) t.Text = ""; chkPhaseC.Checked = false; txPhaseC.Text = "";
      foreach (var t in _pTxt[3]) t.Text = ""; chkPhaseD.Checked = false; txPhaseD.Text = "";
      foreach (var t in _pTxt[4]) t.Text = ""; chkPhaseE.Checked = false; txPhaseE.Text = "";
      foreach (var t in _pTxt[5]) t.Text = ""; chkPhaseF.Checked = false; txPhaseF.Text = "";
      foreach (var t in _pTxt[6]) t.Text = ""; chkPhaseG.Checked = false; txPhaseG.Text = "";
      foreach (var t in _pTxt[7]) t.Text = ""; chkPhaseH.Checked = false; txPhaseH.Text = "";
    }

    /// <summary>
    /// Header Line (Checklist Name) Changed
    /// </summary>
    public event EventHandler HeaderChanged;
    private void OnHeaderChanged( )
    {
      HeaderChanged?.Invoke( this, new EventArgs( ) );
    }

    /// <summary>
    /// User Clicked the Copy Button
    /// </summary>
    public event EventHandler CopyClicked;
    private void OnCopyClicked( )
    {
      CopyClicked?.Invoke( this, new EventArgs( ) );
    }

    /// <summary>
    /// User Clicked the Delete Button
    /// </summary>
    public event EventHandler DeleteClicked;
    private void OnDeleteClicked( )
    {
      DeleteClicked?.Invoke( this, new EventArgs( ) );
    }

    /// <summary>
    /// Returns the Header Line of this checklist
    /// </summary>
    public string Header => txCListName.Text;

    /// <summary>
    /// Set the Box Size of the Check Input fields
    /// </summary>
    /// <param name="size">A Size</param>
    public void SetBoxSize( CheckSize size )
    {
      flpPhases.SuspendLayout( );
      Size newSize = Helper.WriteBoxSizes[(int)size];
      for (int phase = 0; phase < c_numPhase; phase++) {
        for (int i = 0; i < _pTxt[phase].Length; i++) {
          _pTxt[phase][i].Size = newSize;
        }
      }
      flpPhases.ResumeLayout( );
    }

    public UC_CheckPage( )
    {
      InitializeComponent( );
      // array access for textboxes
      for (int i = 0; i < c_numPhase; i++) _pTxt.Add( new TextBox[c_numChecks] );

      int phase = 0;
      _pName[phase] = txPhaseA; _pEnabled[phase] = chkPhaseA;
      _pTxt[phase][0] = txChkItem_A_0; _pTxt[phase][1] = txChkItem_A_1; _pTxt[phase][2] = txChkItem_A_2; _pTxt[phase][3] = txChkItem_A_3; _pTxt[phase][4] = txChkItem_A_4;
      _pTxt[phase][5] = txChkItem_A_5; _pTxt[phase][6] = txChkItem_A_6; _pTxt[phase][7] = txChkItem_A_7; _pTxt[phase][8] = txChkItem_A_8; _pTxt[phase][9] = txChkItem_A_9;

      phase++;
      _pName[phase] = txPhaseB; _pEnabled[phase] = chkPhaseB;
      _pTxt[phase][0] = txChkItem_B_0; _pTxt[phase][1] = txChkItem_B_1; _pTxt[phase][2] = txChkItem_B_2; _pTxt[phase][3] = txChkItem_B_3; _pTxt[phase][4] = txChkItem_B_4;
      _pTxt[phase][5] = txChkItem_B_5; _pTxt[phase][6] = txChkItem_B_6; _pTxt[phase][7] = txChkItem_B_7; _pTxt[phase][8] = txChkItem_B_8; _pTxt[phase][9] = txChkItem_B_9;

      phase++;
      _pName[phase] = txPhaseC; _pEnabled[phase] = chkPhaseC;
      _pTxt[phase][0] = txChkItem_C_0; _pTxt[phase][1] = txChkItem_C_1; _pTxt[phase][2] = txChkItem_C_2; _pTxt[phase][3] = txChkItem_C_3; _pTxt[phase][4] = txChkItem_C_4;
      _pTxt[phase][5] = txChkItem_C_5; _pTxt[phase][6] = txChkItem_C_6; _pTxt[phase][7] = txChkItem_C_7; _pTxt[phase][8] = txChkItem_C_8; _pTxt[phase][9] = txChkItem_C_9;

      phase++;
      _pName[phase] = txPhaseD; _pEnabled[phase] = chkPhaseD;
      _pTxt[phase][0] = txChkItem_D_0; _pTxt[phase][1] = txChkItem_D_1; _pTxt[phase][2] = txChkItem_D_2; _pTxt[phase][3] = txChkItem_D_3; _pTxt[phase][4] = txChkItem_D_4;
      _pTxt[phase][5] = txChkItem_D_5; _pTxt[phase][6] = txChkItem_D_6; _pTxt[phase][7] = txChkItem_D_7; _pTxt[phase][8] = txChkItem_D_8; _pTxt[phase][9] = txChkItem_D_9;

      phase++;
      _pName[phase] = txPhaseE; _pEnabled[phase] = chkPhaseE;
      _pTxt[phase][0] = txChkItem_E_0; _pTxt[phase][1] = txChkItem_E_1; _pTxt[phase][2] = txChkItem_E_2; _pTxt[phase][3] = txChkItem_E_3; _pTxt[phase][4] = txChkItem_E_4;
      _pTxt[phase][5] = txChkItem_E_5; _pTxt[phase][6] = txChkItem_E_6; _pTxt[phase][7] = txChkItem_E_7; _pTxt[phase][8] = txChkItem_E_8; _pTxt[phase][9] = txChkItem_E_9;

      phase++;
      _pName[phase] = txPhaseF; _pEnabled[phase] = chkPhaseF;
      _pTxt[phase][0] = txChkItem_F_0; _pTxt[phase][1] = txChkItem_F_1; _pTxt[phase][2] = txChkItem_F_2; _pTxt[phase][3] = txChkItem_F_3; _pTxt[phase][4] = txChkItem_F_4;
      _pTxt[phase][5] = txChkItem_F_5; _pTxt[phase][6] = txChkItem_F_6; _pTxt[phase][7] = txChkItem_F_7; _pTxt[phase][8] = txChkItem_F_8; _pTxt[phase][9] = txChkItem_F_9;

      phase++;
      _pName[phase] = txPhaseG; _pEnabled[phase] = chkPhaseG;
      _pTxt[phase][0] = txChkItem_G_0; _pTxt[phase][1] = txChkItem_G_1; _pTxt[phase][2] = txChkItem_G_2; _pTxt[phase][3] = txChkItem_G_3; _pTxt[phase][4] = txChkItem_G_4;
      _pTxt[phase][5] = txChkItem_G_5; _pTxt[phase][6] = txChkItem_G_6; _pTxt[phase][7] = txChkItem_G_7; _pTxt[phase][8] = txChkItem_G_8; _pTxt[phase][9] = txChkItem_G_9;

      phase++;
      _pName[phase] = txPhaseH; _pEnabled[phase] = chkPhaseH;
      _pTxt[phase][0] = txChkItem_H_0; _pTxt[phase][1] = txChkItem_H_1; _pTxt[phase][2] = txChkItem_H_2; _pTxt[phase][3] = txChkItem_H_3; _pTxt[phase][4] = txChkItem_H_4;
      _pTxt[phase][5] = txChkItem_H_5; _pTxt[phase][6] = txChkItem_H_6; _pTxt[phase][7] = txChkItem_H_7; _pTxt[phase][8] = txChkItem_H_8; _pTxt[phase][9] = txChkItem_H_9;
    }

    // Page Loading
    private void UC_CheckPage_Load( object sender, EventArgs e )
    {
      ReloadButtonFonts( );
    }


    /// <summary>
    /// Reload the button fonts from the manager
    /// </summary>
    public void ReloadButtonFonts( )
    {
      foreach (var phase in _pTxt) {
        foreach (var txt in phase) {
          txt.Font = frmConfig.FontManager.GetFont( "DEFAULT" );
        }
      }
    }

    /// <summary>
    /// Loads the Controls with the values from the provided Checklist
    /// </summary>
    /// <param name="checklist">A Checklist Obj</param>
    public void LoadChecklist( Checklist checklist )
    {
      ClearChecks( ); // All clear and disabled
      if (checklist == null) return; // Exit if nothing is provided

      txCListName.Text = checklist.Name;
      for (int phase = 0; phase < c_numPhase; phase++) {
        if (checklist.Phases.Count > phase) {
          // Phase exists
          _pName[phase].Text = checklist.Phases[phase].Name;
          _pEnabled[phase].Checked = checklist.Phases[phase].Enabled;
          for (int i = 0; i < _pTxt[phase].Length; i++) {
            if (checklist.Phases[phase].Checks.Count > i) {
              // Check is provided
              _pTxt[phase][i].Text = checklist.Phases[phase].Checks[i];
            }
          }
        }
      }
    }

    /// <summary>
    /// Returns the Controls values as Checklist
    /// </summary>
    /// <returns>A Checklist</returns>
    public Checklist GetChecklist( )
    {
      var chk = new Checklist( );
      chk.Name = txCListName.Text;
      for (int phase = 0; phase < c_numPhase; phase++) {
        chk.Phases.Add( new CheckPhase( ) );
        chk.Phases[phase].Name = _pName[phase].Text;
        chk.Phases[phase].Enabled = _pEnabled[phase].Checked;
        for (int i = 0; i < _pTxt[phase].Length; i++) {
          chk.Phases[phase].Checks.Add( _pTxt[phase][i].Text );
        }
      }
      return chk;
    }

    private void btCopy_Click( object sender, EventArgs e )
    {
      OnCopyClicked( );
    }

    private void btDelete_Click( object sender, EventArgs e )
    {
      OnDeleteClicked( );
    }

    private void txCListName_TextChanged( object sender, EventArgs e )
    {
      OnHeaderChanged( );
    }

    private void chkPhaseA_CheckedChanged( object sender, EventArgs e )
    {
      txPhaseA.BackColor = (chkPhaseA.Checked) ? Color.LightGreen : txChkItem_A_0.BackColor;
    }

    private void chkPhaseB_CheckedChanged( object sender, EventArgs e )
    {
      txPhaseB.BackColor = (chkPhaseB.Checked) ? Color.LightGreen : txChkItem_A_0.BackColor;
    }

    private void chkPhaseC_CheckedChanged( object sender, EventArgs e )
    {
      txPhaseC.BackColor = (chkPhaseC.Checked) ? Color.LightGreen : txChkItem_A_0.BackColor;
    }

    private void chkPhaseD_CheckedChanged( object sender, EventArgs e )
    {
      txPhaseD.BackColor = (chkPhaseD.Checked) ? Color.LightGreen : txChkItem_A_0.BackColor;
    }

    private void chkPhaseE_CheckedChanged( object sender, EventArgs e )
    {
      txPhaseE.BackColor = (chkPhaseE.Checked) ? Color.LightGreen : txChkItem_A_0.BackColor;
    }

    private void chkPhaseF_CheckedChanged( object sender, EventArgs e )
    {
      txPhaseF.BackColor = (chkPhaseF.Checked) ? Color.LightGreen : txChkItem_A_0.BackColor;
    }

    private void chkPhaseG_CheckedChanged( object sender, EventArgs e )
    {
      txPhaseG.BackColor = (chkPhaseG.Checked) ? Color.LightGreen : txChkItem_A_0.BackColor;
    }

    private void chkPhaseH_CheckedChanged( object sender, EventArgs e )
    {
      txPhaseH.BackColor = (chkPhaseH.Checked) ? Color.LightGreen : txChkItem_A_0.BackColor;
    }

    private void txChkItem_A_0_ClientSizeChanged( object sender, EventArgs e )
    {
      Debug.WriteLine( $"BT TextBox Size: {txChkItem_A_0.Size} - ClientSize: {txChkItem_A_0.ClientSize}" );
    }

  }
}

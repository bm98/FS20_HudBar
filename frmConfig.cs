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

    private void frmConfig_Load( object sender, EventArgs e )
    {
      this.TopMost = false;

      if ( HudBarRef == null ) return; // sanity ..
      if ( ProfilesRef?.Count < 5 ) return;// sanity ..

      cbxUnits.Checked = HudBarRef.ShowUnits;
      cbxOpaque.Checked = HudBarRef.OpaqueBackground;
      cbxFont.Items.Clear( );
      cbxFont.Items.Add( GUI.FontSize.Regular + " Font Size" );
      cbxFont.Items.Add( GUI.FontSize.Larger + " Font Size" );
      cbxFont.Items.Add( GUI.FontSize.Largest + " Font Size" );
      cbxFont.SelectedIndex = (int)HudBarRef.FontSize; // take care this matches the item list (and ENUM FontSize)

      txP1.Text = ProfilesRef[0].PName;
      ProfilesRef[0].LoadCbx( clbxP1, HudBarRef );
      txP2.Text = ProfilesRef[1].PName;
      ProfilesRef[1].LoadCbx( clbxP2, HudBarRef );
      txP3.Text = ProfilesRef[2].PName;
      ProfilesRef[2].LoadCbx( clbxP3, HudBarRef );
      txP4.Text = ProfilesRef[3].PName;
      ProfilesRef[3].LoadCbx( clbxP4, HudBarRef );
      txP5.Text = ProfilesRef[4].PName;
      ProfilesRef[4].LoadCbx( clbxP5, HudBarRef );
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
      HudBarRef.FontSize = (GUI.FontSize)cbxFont.SelectedIndex;

      ProfilesRef[0].PName = txP1.Text;
      ProfilesRef[0].GetFromCbx( clbxP1, HudBarRef );
      ProfilesRef[1].PName = txP2.Text;
      ProfilesRef[1].GetFromCbx( clbxP2, HudBarRef );
      ProfilesRef[2].PName = txP3.Text;
      ProfilesRef[2].GetFromCbx( clbxP3, HudBarRef );
      ProfilesRef[3].PName = txP4.Text;
      ProfilesRef[3].GetFromCbx( clbxP4, HudBarRef );
      ProfilesRef[4].PName = txP5.Text;
      ProfilesRef[4].GetFromCbx( clbxP5, HudBarRef );

      this.DialogResult = DialogResult.OK;
      this.Close( );
    }



  }
}

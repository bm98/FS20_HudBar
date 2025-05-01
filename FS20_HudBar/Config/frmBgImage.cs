using System;
using System.Drawing;
using System.Windows.Forms;

using DbgLib;

namespace FS20_HudBar.Config
{
  public partial class frmBgImage : Form
  {
    #region STATIC
    // A logger
    private static readonly IDbg LOG = Dbg.Instance.GetLogger(
      System.Reflection.Assembly.GetCallingAssembly( ),
      System.Reflection.MethodBase.GetCurrentMethod( ).DeclaringType );
    #endregion


    private Padding _curBorderArea = new Padding( 0, 0, 0, 0 );
    private bool _onInit = false; // ini flag

    public frmBgImage( )
    {
      _onInit = true;
      InitializeComponent( );

      // non std properties
      OFD.Title = "Select Image";
      OFD.Filter = "Image Files|*.png;*.jpg;*.jpeg";
      OFD.SupportMultiDottedExtensions = true;

      _onInit = false;
    }

    /// <summary>
    /// Get;Set: the image file name
    /// </summary>
    public string BgImageFile { get; set; } = "";
    /// <summary>
    /// Get;Set: image border area
    /// </summary>
    public Padding BgImageBorderArea { get; set; } = new Padding( );


    private void frmBgImage_Load( object sender, EventArgs e )
    {
      _onInit = true;
      // init with given file
      txBgImageFile.Text = BgImageFile;
      _curBorderArea = BgImageBorderArea;
      txImageBorder.Text = _curBorderArea.ToString( );
      numLeft.Value = _curBorderArea.Left;
      numTop.Value = _curBorderArea.Top;
      numRight.Value = _curBorderArea.Right;
      numBottom.Value = _curBorderArea.Bottom;
      _onInit = false;
    }

    // Accept Clicked
    private void btAccept_Click( object sender, EventArgs e )
    {
      // return values
      BgImageFile = txBgImageFile.Text;
      BgImageBorderArea = _curBorderArea;

      this.DialogResult = DialogResult.OK;
      this.Hide( ); // hide to let the caller retrieve the values
    }

    // Cancel Clicked
    private void btCancel_Click( object sender, EventArgs e )
    {
      this.DialogResult = DialogResult.Cancel;
      this.Hide( ); // hide to let the caller retrieve the values
    }

    // Clear Clicked
    private void btClear_Click( object sender, EventArgs e )
    {
      txBgImageFile.Text = "";
    }

    // Select Clicked
    private void btSelect_Click( object sender, EventArgs e )
    {
      OFD.FileName = txBgImageFile.Text;
      if (OFD.ShowDialog( this ) == DialogResult.OK) {
        // check if the file is a readable image file
        try {
          var img = Image.FromFile( OFD.FileName );
          // seems legit...
          txBgImageFile.Text = OFD.FileName;
        }
        catch (Exception ex) {
          // complain to user
          MessageBox.Show( this, "The selected image file failed to load\nPlease select another one.", "Image File Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
          LOG.Error( ex, "Image LoadFromFile failed with Exception" );
        }

      }
    }

    // any numerator changed
    private void num_ValueChanged( object sender, EventArgs e )
    {
      if (_onInit) return; // avoid roundtrips

      NumericUpDown num = (NumericUpDown)sender;
      if (num.Name == "numLeft") { _curBorderArea.Left = (int)num.Value; }
      else if (num.Name == "numTop") { _curBorderArea.Top = (int)num.Value; }
      else if (num.Name == "numRight") { _curBorderArea.Right = (int)num.Value; }
      else if (num.Name == "numBottom") { _curBorderArea.Bottom = (int)num.Value; }
      txImageBorder.Text = _curBorderArea.ToString( );
    }

    // border set zero
    private void btZero_Click( object sender, EventArgs e )
    {
      // set Numerators - the rest should be done by the updates
      numLeft.Value = 0; numRight.Value = 0;
      numTop.Value = 0; numBottom.Value = 0;
    }

  }
}

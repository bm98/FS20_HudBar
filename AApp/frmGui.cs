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
  /// <summary>
  /// As for the WinForms environment there is no way to have a Form to be translucent but maintain
  /// the GUI elements in full brightness
  /// 
  /// As a workaround we use 2 synched forms where one carries the GUI elements
  /// and the other acts as Main form and has opacity set to whatever is choosen.
  /// 
  /// This Form carries all GUI elements
  /// GUI elements need to have Transparent BackColor in order to shine through
  /// 
  /// The Form has a fully transparent background.
  /// No borders etc.
  /// The Main Form will Own this form and synch its size and movement
  /// </summary>
  public partial class frmGui : Form
  {
    public frmGui( )
    {
      InitializeComponent( );

      // renders the backround in an unlikely color and applies full transparency using the TransparencyKey
      // property to make it happen
      // seems we have to use a monochrome key to have it work as expected..
      this.BackColor = Color.FromArgb( 1, 1, 1 );
      this.TransparencyKey = this.BackColor;

      //this.BackColor = Color.FromArgb( 1, 99, 1 ); // DEBUG ONLY
    }
  }
}

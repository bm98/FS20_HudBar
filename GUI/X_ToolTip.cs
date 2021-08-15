using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  /// <summary>
  /// Make a monospace tooltip
  /// https://social.msdn.microsoft.com/Forums/en-US/19994c9f-bdaa-4c4c-bcb4-5c60cb241134/tooltip-font-change?forum=winformsdesigner
  /// </summary>
  class X_ToolTip : ToolTip
  {
    private Font ttFont = new Font("Consolas", 9f, FontStyle.Regular);
    public X_ToolTip( )
    {
      this.OwnerDraw = true;
      this.IsBalloon = false;
      this.Draw += new DrawToolTipEventHandler( OnDraw );
    }

    public X_ToolTip( System.ComponentModel.IContainer Cont )
    {
      this.OwnerDraw = true;
      this.Draw += new DrawToolTipEventHandler( OnDraw );
    }

    private void OnDraw( object sender, DrawToolTipEventArgs e )
    {
      DrawToolTipEventArgs newArgs = new DrawToolTipEventArgs ( e.Graphics,
                e.AssociatedWindow, e.AssociatedControl, e.Bounds, e.ToolTipText,
                this.BackColor, this.ForeColor, ttFont /*new Font ( e.Font, FontStyle.Regular ) */);
      newArgs.DrawBackground( );
      newArgs.DrawBorder( );
      newArgs.DrawText( TextFormatFlags.TextBoxControl | TextFormatFlags.VerticalCenter );
    }
  }


}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  /// <summary>
  /// Make a monospace tooltip
  /// https://social.msdn.microsoft.com/Forums/en-US/19994c9f-bdaa-4c4c-bcb4-5c60cb241134/tooltip-font-change?forum=winformsdesigner
  /// 
  /// Extended to work with all kind of Fonts
  /// We actually measure the string to be drawn and apply that size later 
  /// when the TTip needs to be drawn
  /// </summary>
  class X_ToolTip : ToolTip_Base
  {
    // Font used throughout for this type of tooltip
    // a mono font to have the columns neatly displayed
    private Font ttFont = new Font("Consolas", 9f, FontStyle.Regular);

    // Extend Size to have some padding around the measured box
    private SizeF m_extend = new SizeF( 20, 15 ); 

    //maintain the strings for all controls to apply the proper sizing when about to be drawn
    private Dictionary<Control, Size> m_ttSize = new Dictionary<Control, Size>();

    /// <summary>
    /// cTor:
    /// </summary>
    public X_ToolTip( )
    {
      // Set up the delays for the ToolTip.
      AutoPopDelay = 30_000; // looong

      this.OwnerDraw = true;
      this.IsBalloon = false;
      // interact and overwrite some behavior
      this.Draw += new DrawToolTipEventHandler( OnDraw );
      this.Popup += X_ToolTip_Popup;
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public X_ToolTip( System.ComponentModel.IContainer Cont )
    {
      this.OwnerDraw = true;
      this.Draw += new DrawToolTipEventHandler( OnDraw );
      this.Popup += X_ToolTip_Popup;
    }

    /// <summary>
    /// Resets the Draw Size list an removes old entries
    /// Beware, call it only when none of the controls will be needed ever again.
    /// </summary>
    public void ResetDrawList( )
    {
      m_ttSize.Clear( );
    }


    /// <summary>
    /// Called to set the tooltip caption 
    ///  we reimplement it to derive the bounding size of the TTip for later use
    /// </summary>
    /// <param name="control">The control to attach the TTip</param>
    /// <param name="caption">The string to show as TTip Caption</param>
    new public void SetToolTip( Control control, string caption )
    {
      // measure the string to be drawn and store it per control
      Size size = new Size(100,100);
      using ( var g = control.CreateGraphics( ) ) {
        var s = g.MeasureString( caption, ttFont );
        s += m_extend; // inflate the measured bound
        size = s.ToSize( );
      }
      // maintain the sizes for all TTips per control
      if ( !m_ttSize.ContainsKey( control ) ) {
        m_ttSize.Add( control, size );
      }
      else {
        m_ttSize[control] = size;
      }
      base.SetToolTip( control, caption );
    }


    // the boundary box should be set here via ToolTip PopUp event to the measured size of the text to draw
    private void OnDraw( object sender, DrawToolTipEventArgs e )
    {
      Console.WriteLine( $"OnDraw: {e.Bounds.Width} x {e.Bounds.Height}" );
      DrawToolTipEventArgs newArgs = new DrawToolTipEventArgs ( e.Graphics,
                e.AssociatedWindow, e.AssociatedControl, e.Bounds, e.ToolTipText,
                this.BackColor, this.ForeColor, ttFont /*new Font ( e.Font, FontStyle.Regular ) */);
      newArgs.DrawBackground( );
      newArgs.DrawBorder( );
      newArgs.DrawText( TextFormatFlags.TextBoxControl | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix );
    }


    // called on PopUp BEFORE the OnDraw Call
    private void X_ToolTip_Popup( object sender, PopupEventArgs e )
    {
      // shall never fail due to not found control - should really not..
      try {
        e.ToolTipSize = m_ttSize[e.AssociatedControl]; // assign measured size
      }
      catch { }
    }

  }


}

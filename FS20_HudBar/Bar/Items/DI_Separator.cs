using System;
using System.Windows.Forms;
using static FS20_HudBar.GUI.GUI_Colors;
using static FS20_HudBar.GUI.GUI_Colors.ColorType;

using FS20_HudBar.Bar.Items.Base;
using FS20_HudBar.GUI;
using System.Drawing;

namespace FS20_HudBar.Bar.Items
{
  /// <summary>
  /// A separator Display Item 
  /// </summary>
  class DI_Separator : DispItem, IColorType
  {

    private ColorType m_foreColorType = cInfo;
    private ColorType m_backColorType = cBG;

    /// <summary>
    /// Internal PictureBox Class to support the IColorType interface
    /// (though it is not used in this case as it is not registered with the ColorManager)
    /// </summary>
    private class S_Image : PictureBox, IColorType
    {
      public ColorType ItemForeColor { get => throw new NotImplementedException( ); set => throw new NotImplementedException( ); }
      public ColorType ItemBackColor { get => throw new NotImplementedException( ); set => throw new NotImplementedException( ); }
      public void UpdateColor( ) => throw new NotImplementedException( );
    }

    // the DI content control
    private readonly S_Image _pic;

    /// <summary>
    /// cTor:
    /// </summary>
    /// <param name="colorType">The Separator base color</param>
    public DI_Separator( ColorType colorType )
    {
      this.ItemBackColor = colorType;
      this.AutoSize = true;           // expand to full extent
      this.Margin = new Padding( 3 ); // have some spaceing around

      DiLayout = ItemLayout.Separator;

      // need a control with content to expand the DI in its container
      // uses a 3x3 fully transparent image - color of the separator is given by ItemBackColor
      _pic = new S_Image( ) { BackgroundImage = Properties.Resources._3x3void, BackgroundImageLayout = ImageLayout.Zoom, Margin = new Padding( 0 ), MaximumSize = new Size( 3, 3 ) };
      this.AddItem( _pic );
    }

    /// <summary>
    /// Set the Gap Pixel dimension
    /// </summary>
    /// <param name="pixels">Gap Dimension in pixels</param>
    virtual public void SetGapSize( ushort pixels )
    {
      if (this.Dock == DockStyle.Left) {
        int left = ((int)pixels - _pic.Height) / 2;
        int right = ((int)pixels - _pic.Height) - left;
        this.Margin = new Padding( left, 0, right, 0 );
      }
      else if (this.Dock == DockStyle.Top) {
        int top = ((int)pixels - _pic.Height) / 2;
        int bottom = ((int)pixels - _pic.Height) - top;
        this.Margin = new Padding( 0, top, 0, bottom );
      }
    }

    /// <summary>
    /// Get; Set the items Foreground Color by the type of the Item
    /// </summary>
    virtual public ColorType ItemForeColor {
      get => m_foreColorType;
      set {
        m_foreColorType = value;
        this.ForeColor = ItemColor( m_foreColorType );
      }
    }

    /// <summary>
    /// Get; Set the items Foreground Color by the type of the Item
    /// </summary>
    virtual public ColorType ItemBackColor {
      get => m_backColorType;
      set {
        m_backColorType = value;
        this.BackColor = ItemColor( m_backColorType );
      }
    }

    /// <summary>
    /// Asks the Object to update it's colors
    /// </summary>
    virtual public void UpdateColor( )
    {
      this.ForeColor = ItemColor( m_foreColorType );
      this.BackColor = ItemColor( m_backColorType );
    }


  }
}


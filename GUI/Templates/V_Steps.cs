using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using FS20_HudBar.GUI.Templates.Base;

namespace FS20_HudBar.GUI.Templates
{
  class V_Steps : V_Base
  {
    //private string c_On =Convert.ToChar(0x6E).ToString();  // Wingdings char  Filled Square

    //private string c_Off =Convert.ToChar(0xA8).ToString();  // Wingdings char Outline Square
    //private string c_Off =Convert.ToChar(0xA1).ToString();  // Wingdings char Outline Circle

    private string c_OnOK =Convert.ToChar(0xA4).ToString();  // Wingdings char  Circle with Dot
    private string c_OffWarn =Convert.ToChar(0x6F).ToString();  // Wingdings char Outline Square

    private string c_OnWarn =Convert.ToChar(0x75).ToString();  // Wingdings char  Filled Diamond
    private string c_OffOK =Convert.ToChar(0x9F).ToString();  // Wingdings char Outline Circle

    private string c_Up = Convert.ToChar(0xDD).ToString();  // Wingdings char Circled Arrow UP
    private string c_Unk =Convert.ToChar(0xDC).ToString();  // Wingdings char Circled Arrow Right
    private string c_Dn =Convert.ToChar(0xDE).ToString();  // Wingdings char  Circled Arrow Down
    private string c_1 = Convert.ToChar(0x8C).ToString();  // Wingdings char  Filled Circle 1
    private string c_2 = Convert.ToChar(0x8D).ToString();  // Wingdings char  Filled Circle 2
    private string c_3 = Convert.ToChar(0x8E).ToString();  // Wingdings char  Filled Circle 3
    private string c_4 = Convert.ToChar(0x8F).ToString();  // Wingdings char  Filled Circle 4
    private string c_5 = Convert.ToChar(0x90).ToString();  // Wingdings char  Filled Circle 5

    private Color m_fColor;
    private Color m_bColor;


    /// <summary>
    /// cTor: Inherits from Label Control
    /// </summary>
    /// <param name="proto"></param>
    public V_Steps( Label proto)
    : base( proto, false )
    {
      Font = new Font( "Wingdings", proto.Font.Size );
      UseCompatibleTextRendering = true; // MUST for WingDings else it is not showing all chars properly...

      m_unit = "";
      //m_default = Convert.ToChar( 0xA0 ).ToString( );
      m_default = Convert.ToChar( 0xB4 ).ToString( ); // Diamond with ? mark - bigger and may be avoid resize jumps to some extent (not monospace font issue)
      Text = UnitString( m_default );
    }

    public override Color ForeColor {
      get => base.ForeColor;
      set {
        m_fColor = value;
        base.ForeColor = value;
      }
    }

    public override Color BackColor {
      get => base.BackColor;
      set {
        m_bColor = value;
        base.BackColor = value;
      }
    }

    /// <summary>
    /// Set the value of the Control - formatted as NN0kt
    /// </summary>
    override public Steps Step {
      set {

        switch ( value ) {
          case Steps.Unk:
            this.Text = c_Unk; this.ItemForeColor = GUI_Colors.ColorType.cWarn; break;

          case Steps.Up:
            this.Text = c_Up; this.ItemForeColor = GUI_Colors.ColorType.cOK;  break;
          case Steps.Down:
            this.Text = c_Dn; this.ItemForeColor = GUI_Colors.ColorType.cOK;  break;

          case Steps.P1:
            this.Text = c_1; this.ItemForeColor = GUI_Colors.ColorType.cStep; break;
          case Steps.P2:
            this.Text = c_2; this.ItemForeColor = GUI_Colors.ColorType.cStep; break;
          case Steps.P3:
            this.Text = c_3; this.ItemForeColor = GUI_Colors.ColorType.cStep; break;
          case Steps.P4:
            this.Text = c_4; this.ItemForeColor = GUI_Colors.ColorType.cStep; break;
          case Steps.P5:
            this.Text = c_5; this.ItemForeColor = GUI_Colors.ColorType.cStep; break;

          case Steps.OnOK:
            this.Text = c_OnOK; this.ItemForeColor = GUI_Colors.ColorType.cOK; break;
          case Steps.OffWarn:
            this.Text = c_OffWarn; this.ItemForeColor = GUI_Colors.ColorType.cWarn; break;

          case Steps.OnWarn:
            this.Text = c_OnWarn; this.ItemForeColor = GUI_Colors.ColorType.cWarn; break;
          case Steps.OffOK:
            this.Text = c_OffOK; this.ItemForeColor = GUI_Colors.ColorType.cOK; break;

          default:
            this.Text = m_default; this.ItemForeColor = GUI_Colors.ColorType.cInfo; break;
        }
      }
    }

  }
}


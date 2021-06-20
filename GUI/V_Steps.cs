using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI
{
  class V_Steps : V_Base
  {
    private string c_On =Convert.ToChar(0x75).ToString();  // Wingdings char  Filled Diamond
    //private string c_Off =Convert.ToChar(0xA8).ToString();  // Wingdings char Outline Square
    //private string c_Off =Convert.ToChar(0xA1).ToString();  // Wingdings char Outline Circle
    private string c_Off =Convert.ToChar(0x9F).ToString();  // Wingdings char Outline Circle

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
            this.Text = c_Unk; base.ForeColor = Color.Gold; break;
          case Steps.Up:
            this.Text = c_Up; base.ForeColor = Color.LimeGreen; break;
          case Steps.Down:
            this.Text = c_Dn; base.ForeColor = Color.LimeGreen; break;
          case Steps.P1:
            this.Text = c_1; base.ForeColor = Color.LightBlue; break;
          case Steps.P2:
            this.Text = c_2; base.ForeColor = Color.LightBlue; break;
          case Steps.P3:
            this.Text = c_3; base.ForeColor = Color.LightBlue; break;
          case Steps.P4:
            this.Text = c_4; base.ForeColor = Color.LightBlue; break;
          case Steps.P5:
            this.Text = c_5; base.ForeColor = Color.LightBlue; break;
          case Steps.On:
            this.Text = c_On; base.ForeColor = m_fColor; break;
          case Steps.Off:
            this.Text = c_Off; base.ForeColor = Color.WhiteSmoke; break;
          default:
            this.Text = m_default; base.ForeColor = Color.WhiteSmoke; break;
        }
      }
    }

  }
}


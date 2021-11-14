using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FS20_HudBar.GUI.Templates.Base
{
  /// <summary>
  /// Improved text rendering as per:
  /// https://stackoverflow.com/questions/2609520/how-to-make-text-labels-smooth
  /// </summary>
  public partial class X_Label : Label
  {
    private TextRenderingHint _hint = TextRenderingHint.SystemDefault;
    public TextRenderingHint TextRenderingHint {
      get { return this._hint; }
      set { this._hint = value; }
    }

    protected override void OnPaint( PaintEventArgs pe )
    {
      pe.Graphics.TextRenderingHint = TextRenderingHint;
      base.OnPaint( pe );
    }
  }

  /// <summary>
  /// Base class to implement Value labels
  /// </summary>
  abstract class V_Base : X_Label, IValue, IColorType
  {
    protected string m_default = "";
    protected string m_unit = "";
    protected bool m_showUnit = false;
    protected GUI_Colors.ColorType m_foreColorType = GUI_Colors.ColorType.cInfo;
    protected GUI_Colors.ColorType m_backColorType = GUI_Colors.ColorType.cBG;

    private const string c_numbers="0123456789";
    private Random random = new Random();

    /// <summary>
    /// Set the numeric Value
    /// </summary>
    virtual public float? Value { set => throw new NotImplementedException( ); }

    /// <summary>
    /// Set the integer Value
    /// </summary>
    virtual public int? IntValue { set => throw new NotImplementedException( ); }

    /// <summary>
    /// Set the Step Value
    /// </summary>
    virtual public Steps Step { set => throw new NotImplementedException( ); }

    /// <summary>
    /// Get; Set the items Foreground Color by the type of the Item
    /// </summary>
    virtual public GUI_Colors.ColorType ItemForeColor {
      get => m_foreColorType;
      set {
        m_foreColorType = value;
        this.ForeColor = GUI_Colors.ItemColor( m_foreColorType );
      }
    }

    /// <summary>
    /// Get; Set the items Foreground Color by the type of the Item
    /// </summary>
    virtual public GUI_Colors.ColorType ItemBackColor {
      get => m_backColorType;
      set {
        m_backColorType = value;
        this.BackColor = GUI_Colors.ItemColor( m_backColorType );
      }
    }

    /// <summary>
    /// Asks the Object to update it's colors
    /// </summary>
    virtual public void UpdateColor( )
    {
      this.ForeColor = GUI_Colors.ItemColor( m_foreColorType );
      this.BackColor = GUI_Colors.ItemColor( m_backColorType );
    }


    /// <summary>
    /// If true shows the unit of value fields
    /// </summary>
    public bool ShowUnit { get => m_showUnit; set => m_showUnit = value; }

    /// <summary>
    /// Add a Unit if ShowUnit is true
    /// </summary>
    /// <param name="defaultString">The formatted Value string</param>
    /// <returns>A formatted string</returns>
    protected string UnitString( string defaultString )
    {
      // bail out where there is no unit anyway
      if ( string.IsNullOrEmpty( m_unit ) )
        return defaultString;

      return defaultString + ( m_showUnit ? $"{m_unit,-3}" : "" ); // right aling the unit with 3 chars
    }

    /// <summary>
    /// Debugging support, provide the default string with some numbers replacing _ (placeholder) chars
    /// </summary>
    /// <param name="defaultString">The formatted Value string</param>
    /// <returns>A formatted string</returns>
    protected string DefaultString( string defaultString )
    {
      string ret = defaultString;
#if DEBUG_NOT_ENABLED
      ret = "";

      for ( int i = 0; i < defaultString.Length; i++ ) {
        if ( defaultString[i] == '_' ) {
          ret += c_numbers[random.Next( 10 )];
        }
        else {
          ret += defaultString[i];
        }
      }
#endif
      return ret;
    }


    /// <summary>
    /// cTor: Create a UserControl based on a prototype control
    /// </summary>
    /// <param name="proto">A label Prototype to derive from</param>
    public V_Base( Label proto, bool showUnit )
    {
      Font = proto.Font;
      ItemForeColor = GUI_Colors.ColorType.cInfo;
      ItemBackColor = GUI_Colors.ColorType.cBG; // force our common BG color here
      AutoSize = true;                          // force AutoSize
      TextAlign = proto.TextAlign;
      Anchor = proto.Anchor;
      Margin = proto.Margin;
      Padding = proto.Padding;
      m_showUnit = showUnit;
      UseCompatibleTextRendering = true;        // make sure the WingDings an other font special chars display properly
      Cursor = Cursors.Default;                 // avoid the movement cross on the item controls
      Text = m_default;

      base.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;

      GUI_Colors.Register( this );
    }

  }
}
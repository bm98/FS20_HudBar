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
}
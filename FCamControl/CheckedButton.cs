using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FCamControl
{
  /// <summary>
  /// Inherit a button and make it a Checked Button which uses Color Indications to change the state
  ///  default Fore and Back Color is used for UNCHECKED state
  ///  new CheckedFore and Back Colors are used for CHECKED state
  ///  INDET state uses UNCHECKED colors
  /// </summary>
  public class CheckedButton : Button
  {
    /// <summary>
    /// internal CheckState
    /// </summary>
    protected CheckState _checkState = CheckState.Unchecked;
    /// <summary>
    /// internal ThreeState
    /// </summary>
    protected bool _threeState = false;

    /// <summary>
    /// internal BackColor
    /// </summary>
    protected Color _backColor = SystemColors.Control;
    /// <summary>
    /// internal ForeColor
    /// </summary>
    protected Color _foreColor = SystemColors.ControlText;

    /// <summary>
    /// internal CheckedBackColor
    /// </summary>
    protected Color _checkedBackColor = SystemColors.Control;
    /// <summary>
    /// internal CheckedForeColor
    /// </summary>
    protected Color _checkedForeColor = SystemColors.ControlText;

    /// <summary>
    /// Occurs when the value of the Checked property changes.
    /// </summary>
    [Description( "Fires when Checked changes" ), Category( "Misc" )]
    public event EventHandler<EventArgs> CheckedChanged;
    /// <summary>
    /// Occurs when the value of the CheckState property changes.
    /// </summary>
    [Description( "Fires when CheckedState changes" ), Category( "Misc" )]
    public event EventHandler<EventArgs> CheckedStateChanged;

    // maintain colors according to checked state
    private void MaintainColors( )
    {
      base.BackColor = (_checkState == CheckState.Checked) ? _checkedBackColor : _backColor;
      base.ForeColor = (_checkState == CheckState.Checked) ? _checkedForeColor : _foreColor;
    }

    /// <inheritdoc/>
    [Description( "The unchecked background color of the CheckButton" ), Category( "Appearance" )]
    public new Color BackColor {
      get => _backColor;
      set {
        if (value == _backColor) return;
        _backColor = value;
        MaintainColors( );
      }
    }

    /// <inheritdoc/>
    [Description( "The unchecked foreground color of the CheckButton" ), Category( "Appearance" )]
    public new Color ForeColor {
      get => _foreColor;
      set {
        if (value == _foreColor) return;
        _foreColor = value;
        MaintainColors( );
      }
    }


    /// <summary>
    /// BackColor in checked state
    /// </summary>
    [Description( "The checked background color of the CheckButton" ), Category( "Appearance" )]
    public Color BackColorChecked {
      get => _checkedBackColor;
      set {
        if (value == _checkedBackColor) return;
        _checkedBackColor = value;
        MaintainColors( );
      }
    }

    /// <summary>
    /// ForeColor in checked state
    /// </summary>
    [Description( "The checked foreground color of the CheckButton" ), Category( "Appearance" )]
    public Color ForeColorChecked {
      get => _checkedForeColor;
      set {
        if (value == _checkedForeColor) return;
        _checkedForeColor = value;
        MaintainColors( );
      }
    }

    /// <summary>
    /// Raises the CheckedChanged event.
    /// </summary>
    protected virtual void OnCheckedChanged( EventArgs e ) => CheckedChanged?.Invoke( this, e );
    /// <summary>
    /// Raises the CheckStateChanged event.
    /// </summary>
    protected virtual void OnCheckedStateChanged( EventArgs e ) => CheckedStateChanged?.Invoke( this, e );


    /// <summary>
    /// cTor:
    /// </summary>
    public CheckedButton( )
      : base( )
    {
      MaintainColors( );
    }

    /// <summary>
    /// Gets or set a value indicating whether the CheckButton is in the checked state.
    /// </summary>
    [Description( "Indicates whether the CheckButton is in the checked state." ), Category( "Appearance" )]
    public bool Checked {
      get => _checkState == CheckState.Checked;
      set {
        bool current = _checkState == CheckState.Checked;
        if (value == current) return; // no change

        _checkState = value ? CheckState.Checked : CheckState.Unchecked;

        MaintainColors( );

        OnCheckedStateChanged( EventArgs.Empty );
        OnCheckedChanged( EventArgs.Empty );
      }
    }

    /// <summary>
    /// Gets or sets the state of the CheckButton.
    /// </summary>
    [Description( "Indicates the state of the CheckButton" ), Category( "Appearance" )]
    public CheckState CheckedState {
      get => _checkState;
      set {
        // sanity
        if (value == CheckState.Indeterminate) {
          if (!_threeState) throw new InvalidEnumArgumentException( "The value assigned is not one of the CheckState enumeration values." );
        }

        if (value == _checkState) return; // no change

        bool @checked = _checkState == CheckState.Checked;
        _checkState = value;

        MaintainColors( );

        OnCheckedStateChanged( EventArgs.Empty );

        // has checked changed too??
        if ((_checkState == CheckState.Checked) != @checked) {
          OnCheckedChanged( EventArgs.Empty );
        }
      }
    }

    /// <summary>
    /// Gets or sets a value indicating whether the CheckButton will allow three check states rather than two.
    /// True if the CheckButton is able to display three check states; otherwise, false. The default value is false.
    /// </summary>
    [Description( "Indicates wether the CheckButton supports three states rather than two" ), Category( "Behavior" )]
    public bool ThreeState {
      get => _threeState;
      set {
        _threeState = value;
        _checkState = _threeState
          ? _checkState
          : (_checkState == CheckState.Checked) ? CheckState.Checked : CheckState.Unchecked;

        MaintainColors( );
      }
    }

  }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace bm98_Map.UI
{
  /// <summary>
  /// An item of the StripPanel
  /// Implements a Strip like label
  /// </summary>
  internal class StripLabel : Label
  {
    // color store
    private Color _fColor = Color.White;
    private Color _bColor = Color.Black;

    // properties
    private int _index = -1;
    private bool _selectable = false;
    private bool _itemSelected = false;

    /// <summary>
    /// Fired when the user selects an item
    /// </summary>
    [Category( "Action" )]
    [Description( "Fires when the item gets selected" )]
    public event EventHandler<EventArgs> Selected;
    private void OnSelected( )
    {
//      if (_inEvent) return; // don't fire when already processing an event

      Selected?.Invoke( this, new EventArgs( ) );
    }

    /// <summary>
    /// Fired when the user selects an item
    /// </summary>
    [Category( "Action" )]
    [Description( "Fires when the item gets clicked" )]
    public new event EventHandler<EventArgs> Click;
    private void OnClicked( EventArgs e )
    {
      this.Click?.Invoke( this, e );
    }

    /// <summary>
    /// Gets or sets the foreground color of the item
    /// </summary>
    [Category( "Appearance" )]
    [Description( "Gets or sets the foreground color of the item" )]
    public new Color ForeColor {
      get => _fColor;
      set {
        _fColor = value;
        base.ForeColor = _itemSelected ? _bColor : _fColor;
      }
    }
    /// <summary>
    /// Gets or sets the background color of the item
    /// </summary>
    [Category( "Appearance" )]
    [Description( "Gets or sets the background color of the item" )]
    public new Color BackColor {
      get => _bColor;
      set {
        _bColor = value;
        base.BackColor = _itemSelected ? _fColor : _bColor;
      }
    }


    /// <summary>
    /// cTor:
    /// </summary>
    public StripLabel( )
    {
      // default properties
      base.AutoSize = true;
      base.Dock = DockStyle.Top; // makes it a full width item
      base.Click += StripLabel_Click;
      base.Padding = new Padding( 5, 1, 1, 0 );
      _fColor = base.ForeColor;
      _bColor = base.BackColor;
    }


    // marks the item selected
    private void MarkSelected( )
    {
      // invert colors
      Color f = base.ForeColor;
      base.ForeColor = base.BackColor;
      base.BackColor = f;
      base.BorderStyle = BorderStyle.FixedSingle;
    }
    // marks the item unselected
    private void MarkUnSelected( )
    {
      // invert colors
      Color f = base.ForeColor;
      base.ForeColor = base.BackColor;
      base.BackColor = f;
      base.BorderStyle = BorderStyle.None;
    }

    /// <summary>
    /// The 0 based index of this item
    /// </summary>
    [Browsable( false )]
    public int Index {
      get => _index;
      set {
        _index = value;
      }
    }

    /// <summary>
    /// Determines wether the StripLabel is selectable or not
    /// </summary>
    [Category( "Behavior" )]
    [Description( "Determines wether the StripLabel is selectable or not" )]
    public bool Selectable {
      get => _selectable;
      set {
        if (value == _selectable) return;

        _selectable = value;
        base.Cursor = _selectable ? Cursors.Hand : Cursors.Default;
        ItemSelected = _selectable ? _itemSelected : false;
      }
    }

    /// <summary>
    /// Determines wether the StripLabel is selected or not
    /// </summary>
    [Category( "Behavior" )]
    [Description( "Determines wether the StripLabel is selected or not" )]
    public bool ItemSelected {
      get => _itemSelected;
      set {
        if (value == _itemSelected) return;

        _itemSelected = _selectable ? value : false;
        if (_itemSelected) {
          MarkSelected( );
          OnSelected( );
        }
        else {
          MarkUnSelected( );
        }
      }
    }


    // when clicked
    private void StripLabel_Click( object sender, EventArgs e )
    {
      this.OnClicked( e ); // base event is handled before our own event
    }


  }
}

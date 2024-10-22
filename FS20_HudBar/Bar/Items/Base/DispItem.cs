using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using SC = SimConnectClient;
using FS20_HudBar.GUI;
using FSimClientIF.Modules;

namespace FS20_HudBar.Bar.Items.Base
{
  /// <summary>
  /// A Display Item
  ///  a left to right orientated FlowPanel which carries a label and optionally some value controls
  ///  The DispItem is intended to be loaded into the main Hud Panel
  /// </summary>
  class DispItem : FlowLayoutPanel
  {
    // Implement Each DispItem as derived Control DI_LITEM 
    // In the implementation define the Label, 
    // Define the Value item and add it to the Value Label list to later change properties when data arrives
    // then use the desired formatter label Control (V_xy) set specifics and add it to the display group and access lists
    // for 2 engine items define the second value item and control (same procedure as with the first one)
    //  - we use value2Proto to get a smaller font usually for 2+ values are to be shown for an item
    // Colors are used from default or set here explicitely 

    /// <summary>
    /// Layout options for an Item
    /// </summary>
    public enum ItemLayout
    {
      /// <summary>
      /// Generic Layout (left aligned)
      /// </summary>
      Generic = 0,
      /// <summary>
      /// One Value Right aligned
      /// </summary>
      ValueRight,
      /// <summary>
      /// Two Row x two values (4 engine layout)
      /// </summary>
      Value2x2,
      /// <summary>
      /// One Symbol Item
      /// </summary>
      Symbol,
      /// <summary>
      /// One Graph item
      /// </summary>
      GraphX1,
      /// <summary>
      /// One or 2 Graph items 
      /// </summary>
      GraphX2,
      /// <summary>
      /// A Separator Item
      /// </summary>
      Separator,
    }

    /// <summary>
    /// The Label ID 
    /// </summary>
    public LItem LabelID { get; protected set; }

    // The Label (first item)
    private Control m_label = null;

    /// <summary>
    /// The expected Layout of this Item
    /// </summary>
    public ItemLayout DiLayout { get; protected set; } = ItemLayout.Generic;

    /// <summary>
    /// Returns the Label of this group (first added control)
    /// </summary>
    public Control Label => (Control)m_label;
    /// <summary>
    /// Returns the Label of this group (first added control)
    /// </summary>
    public IColorType ColorType => (IColorType)m_label;
    /// <summary>
    /// ToolTip Text 
    /// If set it will be attached to the Label Control of the DisplayItem
    /// </summary>
    public string TText = null;
    /// <summary>
    /// Set to true if the Left/Right layout should create a 2 Line Values item for it
    /// </summary>
    public bool TwoRows { get; protected set; } = false;
    /// <summary>
    /// Set to true if this Item is #engine managed
    /// i.e. Must contain 4 Value items which represent Engine 1..4 in this order
    /// </summary>
    public bool IsEngineItem { get; protected set; } = false;

    /// <summary>
    /// True for generic layout
    /// </summary>
    public bool IsGenericLayout => DiLayout == ItemLayout.Generic;
    /// <summary>
    /// True for 1 Value Right Aligned Layout
    /// </summary>
    public bool Is1xRightLayout => DiLayout == ItemLayout.ValueRight;
    /// <summary>
    /// True for 2 rows, 2 values layout (4 engine layout)
    /// </summary>
    public bool Is2x2Layout => DiLayout == ItemLayout.Value2x2;


    protected readonly ISimVar SV = SC.SimConnectClient.Instance.SimVarModule;

    /// <summary>
    /// Generic item used when registering as Observer
    /// </summary>
    protected int m_observerID = 0;

    /// <summary>
    /// Generic Add Observer to observe the SimVar Module 
    /// it will request with an invokeTarget of 'this'
    /// and populate the observerID
    /// </summary>
    /// <param name="itemId">An Item ID string</param>
    /// <param name="reports_perSecond">How many data reports per second are expected (0.1..10)</param>
    /// <param name="onDataArrival">The Action to perform if data arrives</param>
    protected void AddObserver( string itemId, float reports_perSecond, Action<string> onDataArrival )
    {
      // sanity
      var rps = dNetBm98.XMath.Clip( reports_perSecond, 0.1f, 10f );

      // 0.1: 10/0.1=100 (*100ms = 10sec), 1: 10/1=10 (*100ms=1sec),  10: 10/10=1 (*100ms=0.1sec)
      int divider = (int)Math.Round( FSimClientIF.Sim.DataArrival_perSecond / rps );
      m_observerID = SV.AddObserver( itemId, divider, onDataArrival, this );// divider = every nth reporting of 10/sec (nominal pace)
    }

    /// <summary>
    /// Generic Unregister from the DataSource(s) 
    /// will unregister from SimVar module
    /// Else overwrite it
    /// </summary>
    protected virtual void UnregisterDataSource( )
    {
      UnregisterObserver_low( SV );
    }

    // generic unregister method
    protected void UnregisterObserver_low( IModule module )
    {
      if (m_observerID > 0) {
        module.RemoveObserver( m_observerID );
        m_observerID = 0;
      }
    }

    /// <summary>
    /// Width for Aligned AP items (defined only here..)
    /// </summary>
    protected int m_alignWidth = 9;

    /// <summary>
    /// cTor: Create an item
    /// </summary>
    public DispItem( )
    {
      this.FlowDirection = FlowDirection.LeftToRight;
      this.WrapContents = false;
      this.AutoSize = true;
      this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
      this.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
      this.Dock = DockStyle.Bottom;
      this.Cursor = Cursors.Default; // avoid the movement cross on the item controls
      this.BackColor = GUI_Colors.ItemColor( GUI_Colors.ColorType.cBG ); // default (should be transparent - ex. for debugging)
      this.TabStop = false;
      this.Margin = new Padding( 0, 0, 0, 0 );  // no spacing around the DI item
      this.Padding = new Padding( 0 ); // no inner spacing

#if DEBUG
      // this.BackColor = Color.SteelBlue; // DEBUG color
      //this.BorderStyle = BorderStyle.FixedSingle; // DEBUG
#endif
    }

    /// <summary>
    /// Add an Item from Left to Right
    /// </summary>
    /// <param name="control"></param>
    public void AddItem( Control control )
    {
      //if ( !( control is Control ) ) throw new ArgumentException( "Argument must be of type Control" ); // sanity
      if (!(control is IColorType)) throw new ArgumentException( "Argument must implement IColorType" ); ; // sanity
      // First one is the label
      if (this.Controls.Count == 0) {
        // the Label
        m_label = control;
      }
      // Make Items which implement IAlign to have the Label height and AutoSize their Width when loading (e.g. WindArrow)
      if (control is IAlign) {
        (control as IAlign).AutoSizeHeight = false;
        (control as IAlign).AutoSizeWidth = true;
        control.Height = Label.Height;
      }
      this.Controls.Add( control );
    }

    /// <summary>
    /// Remove an Item from this DisplayItem
    /// </summary>
    /// <param name="control">A Control to be removed</param>
    public void RemoveItem( Control control )
    {
      // don't fail if an unknown obj is tried to be removed
      try {
        GUI_Colors.Unregister( control as IColorType ); // we checked this on arrival - should have that IF anyway
        this.Controls.Remove( control );
        control.Dispose( );
      }
      catch { }
    }

    /// <summary>
    /// Add a margin to 2Row Items 
    /// </summary>
    /// <param name="margin3">A Margin for the 3rd value</param>
    /// <param name="margin4">A Margin for the 4th value</param>
    public void AddSecondRowMargin( Padding margin3, Padding margin4 )
    {
      // sanity
      if (!this.TwoRows) return;

      if (this.Controls.Count > 3) this.Controls[3].Margin = margin3;
      if (this.Controls.Count > 4) this.Controls[4].Margin = margin4;
    }

    /// <summary>
    /// Set Values 1..n (4) visible and the rest invisible
    /// </summary>
    /// <param name="n">MaxVisible Item (1..N)</param>
    public void SetValuesVisible( int n )
    {
      if (n < 1) return;

      for (int i = 1; i < this.Controls.Count; i++) {
        this.Controls[i].Visible = (i <= n);
      }
    }

    /// <summary>
    /// Set a flowbreak for a Value Item
    /// </summary>
    /// <param name="break">True to set, false to remove</param>
    public void SetValueFlowBreak( bool @break, uint valueIndex )
    {
      // sanity
      if (valueIndex < 1) return; // values are index 1..max

      if (this.Controls.Count > valueIndex) {
        if (@break) {
          this.WrapContents = true;    // if break - enable wrapping with flowbreaks // else leave it alone...
        }
        this.SetFlowBreak( this.Controls[(int)valueIndex], @break ); // on prev element = Label
      }
    }

    /// <summary>
    /// Set a flowbreak after the Label
    /// </summary>
    /// <param name="break">True to set, false to remove</param>
    public void SetLabelFlowBreak( bool @break )
    {
      if (this.Controls.Count > 0) {
        if (@break) {
          this.WrapContents = true;    // if break - enable wrapping with flowbreaks // else leave it alone...
        }
        this.Dock = @break ? DockStyle.Top : this.Dock;   // to make single buttons top aligned
        //this.BorderStyle= BorderStyle.FixedSingle; // DEBUG Layout only
        this.SetFlowBreak( this.Controls[0], @break ); // on prev element = Label
      }
    }


    #region DISPOSE

    private bool disposedValue;

    /// <summary>
    /// Overridable Dispose
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    /// <param name="disposing">Disposing flag</param>
    protected override void Dispose( bool disposing )
    {
      if (!disposedValue) {
        if (disposing) {
          // dispose managed state (managed objects)
          UnregisterDataSource( );
          while (this.Controls.Count > 0) {
            var c = this.Controls[0];
            RemoveItem( c ); // Disposes
          }
        }

        disposedValue = true;
      }

      base.Dispose( disposing );
    }

    /// <summary>
    /// Final Dispose of the class
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public new void Dispose( )
    {
      // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
      Dispose( disposing: true );
      GC.SuppressFinalize( this );
    }

    #endregion

  }

}

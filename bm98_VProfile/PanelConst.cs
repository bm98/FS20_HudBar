using System.Drawing;

namespace bm98_VProfile
{
  /// <summary>
  /// Panel Geometries
  /// </summary>
  internal static class PanelConst
  {
    /// <summary>
    /// The panel painting Area relative to the UserControl
    /// We have relative sizes based on the original pictures
    /// </summary>

    // basic background image props
    public readonly static Rectangle PanelArea = new Rectangle( 0, 0, 900, 240 ); // 
    public readonly static Rectangle TapeArea = new Rectangle( 0, 0, 84, PanelArea.Height ); // 
    public readonly static Rectangle GraphArea = new Rectangle( TapeArea.Width+1, 0, 800, PanelArea.Height ); // 

    // useful points
    public readonly static Point PanelAreaTopLeft = PanelArea.Location; // top left of all painting
    public readonly static Point PanelAreaTopRight = new Point( PanelArea.Right, PanelArea.Top );
    public readonly static Point PanelAreaBottomLeft = new Point( PanelArea.Left, PanelArea.Bottom );
    public readonly static Point PanelAreaBottomRight = new Point( PanelArea.Right, PanelArea.Bottom );

    public readonly static Rectangle AltTapeField = TapeArea;

    public readonly static Rectangle VPathGraphField = GraphArea;
    public readonly static Rectangle TxMidDist = new Rectangle(
      VPathGraphField.Left + VPathGraphField.Width / 2 - 50, VPathGraphField.Bottom - 30, 100, 30 );
    public readonly static Rectangle TxEndDist = new Rectangle(
      VPathGraphField.Left + VPathGraphField.Width - 50 - 5, VPathGraphField.Bottom - 30, 100, 30 );
    public readonly static Rectangle TxEndMin = new Rectangle(
      VPathGraphField.Left + VPathGraphField.Width - 50 - 5, VPathGraphField.Height / 2 - 30, 100, 60 );
    public readonly static Rectangle TxFpaDeg = new Rectangle(
      VPathGraphField.Left + VPathGraphField.Width/2 - 50, 0, 100, 60 ); // center, top


  }
}

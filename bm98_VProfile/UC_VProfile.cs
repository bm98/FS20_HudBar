using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using bm98_VProfile.Drawing;

namespace bm98_VProfile
{
  /// <summary>
  /// UC To display a Vertical Profile
  ///  includes RoutePoints which are placed either at TargetAlt or on the current Alt if targetAlt is not defined
  ///  
  ///  Use UpdatePanelProps() to submit UC_VProfileProps at regular intervals which will update the Control
  /// </summary>
  public partial class UC_VProfile : UserControl
  {
    // local storage

    /// <summary>
    /// ALT items data
    /// </summary>
    public UC_VProfileProps VProfileProps { get; private set; } = new UC_VProfileProps( );

    private Bitmap _bitmap = null;

    private float _aspect = 0; // Ratio W/H
    private Rectangle _drawRect = new Rectangle( );

    /// <summary>
    ///Routepoint to be Drawn
    /// </summary>
    public struct UC_VProfilePropsRoutepoint
    {
      /// <summary>
      /// Ident of this Waypoint
      /// </summary>
      public string Ident { get; set; }
      /// <summary>
      /// Distance to this Waypoint
      /// </summary>
      public double Distance_nm { get; set; }
      /// <summary>
      /// Target Altitude for this Waypoint
      /// </summary>
      public int TargetAlt_ft { get; set; }
      /// <summary>
      /// VP Altitude/Limit String
      /// </summary>
      public string VPAltS { get; set; }
      /// <summary>
      /// True if valid
      /// </summary>
      public bool IsValid => !string.IsNullOrEmpty( Ident );
    }

    /// <summary>
    /// Property Set to submit for updating the control
    /// </summary>
    public struct UC_VProfileProps
    {
      /// <summary>
      /// Current Altitude [ft]
      /// </summary>
      public float ALT_ft { get; set; }
      /// <summary>
      /// Current Vertical Speed [ft/min]
      /// </summary>
      public float VS_fpm { get; set; }

      /// <summary>
      /// Current Ground Speed [kt]
      /// </summary>
      public float GS_kt { get; set; }

      /// <summary>
      /// Flightpath angle [°]
      /// </summary>
      public float FPA_deg { get; set; }

      /// <summary>
      /// A list of Waypoints, where the first is expected to be the Next Routepoint
      /// </summary>
      public List<UC_VProfilePropsRoutepoint> WaypointList { get; set; }
      /// <summary>
      /// TopOfDescent 'Routepoint'
      /// </summary>
      public UC_VProfilePropsRoutepoint TopOfDescentRtp { get; set; }

      /// <summary>
      /// cTor: Copy constructor
      /// </summary>
      /// <param name="other">The Props to copy from</param>
      public UC_VProfileProps( UC_VProfileProps other )
      {
        this.ALT_ft = other.ALT_ft;
        this.GS_kt = other.GS_kt;
        this.VS_fpm = other.VS_fpm;
        this.FPA_deg = other.FPA_deg;
        this.WaypointList = default;
        if (other.WaypointList != default) {
          this.WaypointList = other.WaypointList.ToList( );
        }
        this.TopOfDescentRtp = other.TopOfDescentRtp;
      }

    }


    /// <summary>
    /// Returns the desired Aspect of the Control
    ///  Aspect:  Ratio Width / Height
    /// </summary>
    public float DesiredAspect => _aspect;

    private readonly GProc GProc = new GProc( );
    // all handlers
    private readonly VProfile vProfileItems;

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_VProfile( )
    {
      InitializeComponent( );

      vProfileItems = new VProfile( GProc.Drawings );

      // Get the original aspect ratio
      _aspect = (float)pbDrawing.BackgroundImage.Width / (float)pbDrawing.BackgroundImage.Height;
      // set the control to the size of the original background image
      // and put it out of sight
      pbDrawing.Size = pbDrawing.BackgroundImage.Size;
      pbDrawing.Location = new Point( -2000, -2000 ); // outside of view
      pbDrawing.Visible = false;
      //pbDrawing.Visible = true;
    }

    private void UC_VProfile_Load( object sender, EventArgs e )
    {
      RecalcDrawRectangle( );
      UpdatePanel( );
    }

    private void UC_VProfile_Paint( object sender, PaintEventArgs e )
    {
      _bitmap = (Bitmap)pbDrawing.BackgroundImage.Clone( );
      var g = Graphics.FromImage( _bitmap );
      GProc.Paint( g );
      g.Dispose( );

      e.Graphics.DrawImage( _bitmap, _drawRect );
      _bitmap.Dispose( );
    }

    private void UC_VProfile_ClientSizeChanged( object sender, EventArgs e )
    {
      RecalcDrawRectangle( );
      UpdatePanel( );
    }

    private void pbDrawing_SizeChanged( object sender, EventArgs e )
    {

    }

    private void UpdatePanel( )
    {
      if (this.Visible) {
        vProfileItems.Update( GProc, VProfileProps );
        this.Refresh( );
      }
    }

    private void RecalcDrawRectangle( )
    {
      _drawRect = this.ClientRectangle;
      // leave it stretched 
      //return;

      // would scale it proportionally to the background image aspect
      // determine the drawing size from the new control size
      int drawWidth = (int)Math.Floor( this.ClientRectangle.Height * _aspect );
      int drawHeight = (int)Math.Floor( this.ClientRectangle.Width / _aspect );
      if (drawWidth > this.ClientRectangle.Width) {
        drawWidth = this.ClientRectangle.Width;
      }
      else if (drawHeight > this.ClientRectangle.Height) {
        drawHeight = this.ClientRectangle.Height;
      }
      _drawRect = this.ClientRectangle;
      _drawRect.Width = drawWidth;
      _drawRect.Height = drawHeight;
    }

    /// <summary>
    /// Update the VProfile Data from Sim
    /// </summary>
    /// <param name="props">A PropsStruct</param>
    public void UpdatePanelProps( UC_VProfileProps props )
    {
      VProfileProps = new UC_VProfileProps( props );
      UpdatePanel( );
    }

  }
}

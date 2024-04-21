namespace FShelf.LandPerf
{
  ///<inheritdoc/>
  partial class UC_AcftLand
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose( bool disposing )
    {
      if (disposing && (components != null)) {
        components.Dispose( );
      }
      base.Dispose( disposing );
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent( )
    {
      this.SuspendLayout( );
      // 
      // UC_AcftLand
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BackColor = System.Drawing.Color.Transparent;
      this.BackgroundImage = global::FShelf.Properties.Resources.Runway_top;
      this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.DoubleBuffered = true;
      this.Font = new System.Drawing.Font( "Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)) );
      this.Name = "UC_AcftLand";
      this.Size = new System.Drawing.Size( 400, 400 );
      this.Load += new System.EventHandler( this.UC_AcftLand_Load );
      this.Paint += new System.Windows.Forms.PaintEventHandler( this.UC_AcftLand_Paint );
      this.ResumeLayout( false );

    }

    #endregion
  }
}

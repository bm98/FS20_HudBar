namespace bm98_hbControls
{
  partial class UC_LedBar
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
      this.SuspendLayout();
      // 
      // UC_LedBar5
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BackColor = System.Drawing.Color.Transparent;
      this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.DoubleBuffered = true;
      this.Name = "UC_LedBar5";
      this.Size = new System.Drawing.Size(8, 8);
      this.Load += new System.EventHandler(this.UC_LedBar_Load);
      this.ClientSizeChanged += new System.EventHandler(this.UC_LedBar_ClientSizeChanged);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.UC_LedBar_Paint);
      this.Resize += new System.EventHandler(this.UC_LedBar_Resize);
      this.ResumeLayout(false);

    }

    #endregion
  }
}

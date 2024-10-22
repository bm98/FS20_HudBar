namespace bm98_hbControls
{
  partial class UC_Accel
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
      // UC_Accel
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.CausesValidation = false;
      this.DoubleBuffered = true;
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ForeColor = System.Drawing.Color.GreenYellow;
      this.MinimumSize = new System.Drawing.Size(30, 10);
      this.Name = "UC_Accel";
      this.Size = new System.Drawing.Size(30, 10);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.UC_Acccel_Paint);
      this.Resize += new System.EventHandler(this.UC_Acccel_Resize);
      this.ResumeLayout(false);

    }

    #endregion
  }
}

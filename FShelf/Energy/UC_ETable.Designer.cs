namespace FShelf.Energy
{
  partial class UC_ETable
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
      // UC_ETable
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.CausesValidation = false;
      this.DoubleBuffered = true;
      this.Name = "UC_ETable";
      this.Size = new System.Drawing.Size(580, 615);
      this.Load += new System.EventHandler(this.UC_ETable_Load);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.UC_ETable_Paint);
      this.ResumeLayout(false);

    }

    #endregion
  }
}

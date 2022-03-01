
namespace bm98_hbControls
{
  partial class UC_FPA
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
      if ( disposing && ( components != null ) ) {
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
      // US_FPA
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.CausesValidation = false;
      this.DoubleBuffered = true;
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.MinimumSize = new System.Drawing.Size(10, 10);
      this.Name = "US_FPA";
      this.Size = new System.Drawing.Size(8, 8);
      this.ClientSizeChanged += new System.EventHandler(this.UC_FPA_ClientSizeChanged);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.UC_FPA_Paint);
      this.Resize += new System.EventHandler(this.UC_FPA_Resize);
      this.ResumeLayout(false);

    }

    #endregion
  }
}

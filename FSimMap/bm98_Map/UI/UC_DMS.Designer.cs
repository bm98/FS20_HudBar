namespace bm98_Map.UI
{
  partial class UC_DMS
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
      this.txDeg = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.txMin = new System.Windows.Forms.TextBox();
      this.txSec = new System.Windows.Forms.TextBox();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.flowLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // txDeg
      // 
      this.txDeg.Location = new System.Drawing.Point(0, 0);
      this.txDeg.Margin = new System.Windows.Forms.Padding(0);
      this.txDeg.MaxLength = 5;
      this.txDeg.Name = "txDeg";
      this.txDeg.Size = new System.Drawing.Size(42, 25);
      this.txDeg.TabIndex = 0;
      this.txDeg.Text = "-123";
      this.txDeg.WordWrap = false;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(45, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(13, 17);
      this.label1.TabIndex = 1;
      this.label1.Text = "°";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(90, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(11, 17);
      this.label2.TabIndex = 1;
      this.label2.Text = "\'";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(133, 0);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(13, 17);
      this.label3.TabIndex = 1;
      this.label3.Text = "\"";
      // 
      // txMin
      // 
      this.txMin.Location = new System.Drawing.Point(61, 0);
      this.txMin.Margin = new System.Windows.Forms.Padding(0);
      this.txMin.MaxLength = 3;
      this.txMin.Name = "txMin";
      this.txMin.Size = new System.Drawing.Size(26, 25);
      this.txMin.TabIndex = 0;
      this.txMin.Text = "00";
      this.txMin.WordWrap = false;
      // 
      // txSec
      // 
      this.txSec.Location = new System.Drawing.Point(104, 0);
      this.txSec.Margin = new System.Windows.Forms.Padding(0);
      this.txSec.MaxLength = 3;
      this.txSec.Name = "txSec";
      this.txSec.Size = new System.Drawing.Size(26, 25);
      this.txSec.TabIndex = 0;
      this.txSec.Text = "00";
      this.txSec.WordWrap = false;
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.AutoSize = true;
      this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.flowLayoutPanel1.Controls.Add(this.txDeg);
      this.flowLayoutPanel1.Controls.Add(this.label1);
      this.flowLayoutPanel1.Controls.Add(this.txMin);
      this.flowLayoutPanel1.Controls.Add(this.label2);
      this.flowLayoutPanel1.Controls.Add(this.txSec);
      this.flowLayoutPanel1.Controls.Add(this.label3);
      this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
      this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(149, 25);
      this.flowLayoutPanel1.TabIndex = 2;
      // 
      // UC_DMS
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.Controls.Add(this.flowLayoutPanel1);
      this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(0);
      this.Name = "UC_DMS";
      this.Size = new System.Drawing.Size(152, 28);
      this.Load += new System.EventHandler(this.UC_DMS_Load);
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txDeg;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txMin;
    private System.Windows.Forms.TextBox txSec;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
  }
}

namespace Z_TEST_Flightplan
{
  partial class Form1
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent( )
    {
      this.OFD = new System.Windows.Forms.OpenFileDialog();
      this.btLoadFP = new System.Windows.Forms.Button();
      this.RTB = new System.Windows.Forms.RichTextBox();
      this.SuspendLayout();
      // 
      // OFD
      // 
      this.OFD.SupportMultiDottedExtensions = true;
      // 
      // btLoadFP
      // 
      this.btLoadFP.Location = new System.Drawing.Point(12, 12);
      this.btLoadFP.Name = "btLoadFP";
      this.btLoadFP.Size = new System.Drawing.Size(100, 56);
      this.btLoadFP.TabIndex = 0;
      this.btLoadFP.Text = "Load FP";
      this.btLoadFP.UseVisualStyleBackColor = true;
      this.btLoadFP.Click += new System.EventHandler(this.btLoadFP_Click);
      // 
      // RTB
      // 
      this.RTB.Location = new System.Drawing.Point(139, 12);
      this.RTB.Name = "RTB";
      this.RTB.Size = new System.Drawing.Size(605, 363);
      this.RTB.TabIndex = 1;
      this.RTB.Text = "";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.RTB);
      this.Controls.Add(this.btLoadFP);
      this.Name = "Form1";
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.OpenFileDialog OFD;
    private System.Windows.Forms.Button btLoadFP;
    private System.Windows.Forms.RichTextBox RTB;
  }
}


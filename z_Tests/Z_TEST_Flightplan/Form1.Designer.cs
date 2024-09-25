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
      this.btNextWYP = new System.Windows.Forms.Button();
      this.txLat = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.txLon = new System.Windows.Forms.TextBox();
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
      // btNextWYP
      // 
      this.btNextWYP.Location = new System.Drawing.Point(12, 90);
      this.btNextWYP.Name = "btNextWYP";
      this.btNextWYP.Size = new System.Drawing.Size(100, 56);
      this.btNextWYP.TabIndex = 2;
      this.btNextWYP.Text = "Eval Next WYP";
      this.btNextWYP.UseVisualStyleBackColor = true;
      this.btNextWYP.Click += new System.EventHandler(this.btNextWYP_Click);
      // 
      // txLat
      // 
      this.txLat.Location = new System.Drawing.Point(12, 183);
      this.txLat.Name = "txLat";
      this.txLat.Size = new System.Drawing.Size(99, 20);
      this.txLat.TabIndex = 3;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 167);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(25, 13);
      this.label1.TabIndex = 4;
      this.label1.Text = "Lat:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 206);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(28, 13);
      this.label2.TabIndex = 6;
      this.label2.Text = "Lon:";
      // 
      // txLon
      // 
      this.txLon.Location = new System.Drawing.Point(12, 222);
      this.txLon.Name = "txLon";
      this.txLon.Size = new System.Drawing.Size(99, 20);
      this.txLon.TabIndex = 5;
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.txLon);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txLat);
      this.Controls.Add(this.btNextWYP);
      this.Controls.Add(this.RTB);
      this.Controls.Add(this.btLoadFP);
      this.Name = "Form1";
      this.Text = "Form1";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.OpenFileDialog OFD;
    private System.Windows.Forms.Button btLoadFP;
    private System.Windows.Forms.RichTextBox RTB;
    private System.Windows.Forms.Button btNextWYP;
    private System.Windows.Forms.TextBox txLat;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox txLon;
  }
}


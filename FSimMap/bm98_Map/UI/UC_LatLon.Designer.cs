namespace bm98_Map.UI
{
  partial class UC_LatLon
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
      this.txLat = new System.Windows.Forms.TextBox();
      this.lblLat = new System.Windows.Forms.Label();
      this.lblLon = new System.Windows.Forms.Label();
      this.rbDMS = new System.Windows.Forms.RadioButton();
      this.rbDEG = new System.Windows.Forms.RadioButton();
      this.txLon = new System.Windows.Forms.TextBox();
      this.flpLat = new System.Windows.Forms.FlowLayoutPanel();
      this.flpLon = new System.Windows.Forms.FlowLayoutPanel();
      this.pnlMSA = new System.Windows.Forms.Panel();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.lblMsaFt1000 = new System.Windows.Forms.Label();
      this.lblMsaFt100 = new System.Windows.Forms.Label();
      this.dmsLon = new bm98_Map.UI.UC_DMS();
      this.dmsLat = new bm98_Map.UI.UC_DMS();
      this.flpLat.SuspendLayout();
      this.flpLon.SuspendLayout();
      this.pnlMSA.SuspendLayout();
      this.flowLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // txLat
      // 
      this.txLat.Location = new System.Drawing.Point(4, 3);
      this.txLat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.txLat.Name = "txLat";
      this.txLat.ReadOnly = true;
      this.txLat.Size = new System.Drawing.Size(151, 25);
      this.txLat.TabIndex = 0;
      this.txLat.TabStop = false;
      // 
      // lblLat
      // 
      this.lblLat.AutoSize = true;
      this.lblLat.Location = new System.Drawing.Point(4, 6);
      this.lblLat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblLat.Name = "lblLat";
      this.lblLat.Size = new System.Drawing.Size(28, 17);
      this.lblLat.TabIndex = 1;
      this.lblLat.Text = "Lat:";
      // 
      // lblLon
      // 
      this.lblLon.AutoSize = true;
      this.lblLon.Location = new System.Drawing.Point(4, 42);
      this.lblLon.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblLon.Name = "lblLon";
      this.lblLon.Size = new System.Drawing.Size(32, 17);
      this.lblLon.TabIndex = 1;
      this.lblLon.Text = "Lon:";
      // 
      // rbDMS
      // 
      this.rbDMS.AutoSize = true;
      this.rbDMS.Cursor = System.Windows.Forms.Cursors.Hand;
      this.rbDMS.Location = new System.Drawing.Point(204, 38);
      this.rbDMS.Name = "rbDMS";
      this.rbDMS.Size = new System.Drawing.Size(54, 21);
      this.rbDMS.TabIndex = 2;
      this.rbDMS.Text = "DMS";
      this.rbDMS.UseVisualStyleBackColor = true;
      this.rbDMS.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
      // 
      // rbDEG
      // 
      this.rbDEG.AutoSize = true;
      this.rbDEG.Checked = true;
      this.rbDEG.Cursor = System.Windows.Forms.Cursors.Hand;
      this.rbDEG.Location = new System.Drawing.Point(204, 7);
      this.rbDEG.Name = "rbDEG";
      this.rbDEG.Size = new System.Drawing.Size(48, 21);
      this.rbDEG.TabIndex = 2;
      this.rbDEG.TabStop = true;
      this.rbDEG.Text = "Dec";
      this.rbDEG.UseVisualStyleBackColor = true;
      this.rbDEG.CheckedChanged += new System.EventHandler(this.rb_CheckedChanged);
      // 
      // txLon
      // 
      this.txLon.Location = new System.Drawing.Point(4, 3);
      this.txLon.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.txLon.Name = "txLon";
      this.txLon.ReadOnly = true;
      this.txLon.Size = new System.Drawing.Size(151, 25);
      this.txLon.TabIndex = 0;
      this.txLon.TabStop = false;
      // 
      // flpLat
      // 
      this.flpLat.AutoSize = true;
      this.flpLat.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.flpLat.Controls.Add(this.txLat);
      this.flpLat.Controls.Add(this.dmsLat);
      this.flpLat.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flpLat.Location = new System.Drawing.Point(39, 3);
      this.flpLat.Name = "flpLat";
      this.flpLat.Size = new System.Drawing.Size(159, 59);
      this.flpLat.TabIndex = 5;
      // 
      // flpLon
      // 
      this.flpLon.AutoSize = true;
      this.flpLon.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.flpLon.Controls.Add(this.txLon);
      this.flpLon.Controls.Add(this.dmsLon);
      this.flpLon.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flpLon.Location = new System.Drawing.Point(39, 74);
      this.flpLon.Name = "flpLon";
      this.flpLon.Size = new System.Drawing.Size(159, 59);
      this.flpLon.TabIndex = 7;
      // 
      // pnlMSA
      // 
      this.pnlMSA.AutoSize = true;
      this.pnlMSA.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.pnlMSA.BackColor = System.Drawing.Color.DarkKhaki;
      this.pnlMSA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pnlMSA.Controls.Add(this.flowLayoutPanel1);
      this.pnlMSA.Font = new System.Drawing.Font("Gadugi", 15.75F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.pnlMSA.ForeColor = System.Drawing.Color.DimGray;
      this.pnlMSA.Location = new System.Drawing.Point(287, 16);
      this.pnlMSA.Name = "pnlMSA";
      this.pnlMSA.Size = new System.Drawing.Size(62, 35);
      this.pnlMSA.TabIndex = 9;
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.AutoSize = true;
      this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.flowLayoutPanel1.BackColor = System.Drawing.Color.Transparent;
      this.flowLayoutPanel1.Controls.Add(this.lblMsaFt1000);
      this.flowLayoutPanel1.Controls.Add(this.lblMsaFt100);
      this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(60, 33);
      this.flowLayoutPanel1.TabIndex = 8;
      this.flowLayoutPanel1.WrapContents = false;
      // 
      // lblMsaFt1000
      // 
      this.lblMsaFt1000.AutoSize = true;
      this.lblMsaFt1000.Location = new System.Drawing.Point(0, 0);
      this.lblMsaFt1000.Margin = new System.Windows.Forms.Padding(0);
      this.lblMsaFt1000.Name = "lblMsaFt1000";
      this.lblMsaFt1000.Size = new System.Drawing.Size(36, 25);
      this.lblMsaFt1000.TabIndex = 7;
      this.lblMsaFt1000.Text = "10";
      // 
      // lblMsaFt100
      // 
      this.lblMsaFt100.AutoSize = true;
      this.lblMsaFt100.Location = new System.Drawing.Point(36, 8);
      this.lblMsaFt100.Margin = new System.Windows.Forms.Padding(0, 8, 0, 0);
      this.lblMsaFt100.Name = "lblMsaFt100";
      this.lblMsaFt100.Size = new System.Drawing.Size(24, 25);
      this.lblMsaFt100.TabIndex = 7;
      this.lblMsaFt100.Text = "3";
      // 
      // dmsLon
      // 
      this.dmsLon.AutoSize = true;
      this.dmsLon.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.dmsLon.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.dmsLon.IsLat = false;
      this.dmsLon.Location = new System.Drawing.Point(0, 31);
      this.dmsLon.Margin = new System.Windows.Forms.Padding(0);
      this.dmsLon.Name = "dmsLon";
      this.dmsLon.ReadOnly = true;
      this.dmsLon.Size = new System.Drawing.Size(152, 28);
      this.dmsLon.TabIndex = 6;
      this.dmsLon.TabStop = false;
      this.dmsLon.Value = 0D;
      // 
      // dmsLat
      // 
      this.dmsLat.AutoSize = true;
      this.dmsLat.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.dmsLat.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.dmsLat.IsLat = true;
      this.dmsLat.Location = new System.Drawing.Point(0, 31);
      this.dmsLat.Margin = new System.Windows.Forms.Padding(0);
      this.dmsLat.Name = "dmsLat";
      this.dmsLat.ReadOnly = true;
      this.dmsLat.Size = new System.Drawing.Size(152, 28);
      this.dmsLat.TabIndex = 4;
      this.dmsLat.TabStop = false;
      this.dmsLat.Value = 0D;
      // 
      // UC_LatLon
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BackColor = System.Drawing.Color.DarkKhaki;
      this.Controls.Add(this.pnlMSA);
      this.Controls.Add(this.flpLon);
      this.Controls.Add(this.flpLat);
      this.Controls.Add(this.rbDEG);
      this.Controls.Add(this.rbDMS);
      this.Controls.Add(this.lblLon);
      this.Controls.Add(this.lblLat);
      this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
      this.Name = "UC_LatLon";
      this.Size = new System.Drawing.Size(374, 70);
      this.Load += new System.EventHandler(this.UC_LatLonAlt_Load);
      this.flpLat.ResumeLayout(false);
      this.flpLat.PerformLayout();
      this.flpLon.ResumeLayout(false);
      this.flpLon.PerformLayout();
      this.pnlMSA.ResumeLayout(false);
      this.pnlMSA.PerformLayout();
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txLat;
    private System.Windows.Forms.Label lblLat;
    private System.Windows.Forms.Label lblLon;
    private System.Windows.Forms.RadioButton rbDMS;
    private System.Windows.Forms.RadioButton rbDEG;
    private System.Windows.Forms.TextBox txLon;
    private UC_DMS dmsLat;
    private System.Windows.Forms.FlowLayoutPanel flpLat;
    private UC_DMS dmsLon;
    private System.Windows.Forms.FlowLayoutPanel flpLon;
    private System.Windows.Forms.Panel pnlMSA;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.Label lblMsaFt1000;
    private System.Windows.Forms.Label lblMsaFt100;
  }
}

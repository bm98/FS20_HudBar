namespace bm98_Map.UI
{
  partial class UC_Teleport
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
      this.rbMSL = new System.Windows.Forms.RadioButton();
      this.rbAOG = new System.Windows.Forms.RadioButton();
      this.txAlt = new System.Windows.Forms.TextBox();
      this.btTeleport = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.lblLat = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // rbMSL
      // 
      this.rbMSL.AutoSize = true;
      this.rbMSL.Checked = true;
      this.rbMSL.Location = new System.Drawing.Point(160, 4);
      this.rbMSL.Name = "rbMSL";
      this.rbMSL.Size = new System.Drawing.Size(51, 21);
      this.rbMSL.TabIndex = 1;
      this.rbMSL.TabStop = true;
      this.rbMSL.Text = "MSL";
      this.rbMSL.UseVisualStyleBackColor = true;
      // 
      // rbAOG
      // 
      this.rbAOG.AutoSize = true;
      this.rbAOG.Location = new System.Drawing.Point(217, 4);
      this.rbAOG.Name = "rbAOG";
      this.rbAOG.Size = new System.Drawing.Size(53, 21);
      this.rbAOG.TabIndex = 1;
      this.rbAOG.Text = "AOG";
      this.rbAOG.UseVisualStyleBackColor = true;
      // 
      // txAlt
      // 
      this.txAlt.Location = new System.Drawing.Point(75, 3);
      this.txAlt.Name = "txAlt";
      this.txAlt.Size = new System.Drawing.Size(79, 25);
      this.txAlt.TabIndex = 2;
      // 
      // btTeleport
      // 
      this.btTeleport.Location = new System.Drawing.Point(288, 4);
      this.btTeleport.Name = "btTeleport";
      this.btTeleport.Size = new System.Drawing.Size(83, 34);
      this.btTeleport.TabIndex = 3;
      this.btTeleport.Text = "Teleport";
      this.btTeleport.UseVisualStyleBackColor = true;
      this.btTeleport.Click += new System.EventHandler(this.btTeleport_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(158, 41);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(213, 17);
      this.label1.TabIndex = 4;
      this.label1.Text = "Hint: Use Active Pause to teleport";
      // 
      // lblLat
      // 
      this.lblLat.AutoSize = true;
      this.lblLat.Location = new System.Drawing.Point(4, 6);
      this.lblLat.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.lblLat.Name = "lblLat";
      this.lblLat.Size = new System.Drawing.Size(64, 17);
      this.lblLat.TabIndex = 5;
      this.lblLat.Text = "Altitude ft";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(4, 31);
      this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(141, 17);
      this.label2.TabIndex = 6;
      this.label2.Text = "and Center of the Map";
      // 
      // UC_Teleport
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BackColor = System.Drawing.Color.PaleGoldenrod;
      this.Controls.Add(this.label2);
      this.Controls.Add(this.lblLat);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btTeleport);
      this.Controls.Add(this.txAlt);
      this.Controls.Add(this.rbAOG);
      this.Controls.Add(this.rbMSL);
      this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "UC_Teleport";
      this.Size = new System.Drawing.Size(374, 70);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.RadioButton rbMSL;
    private System.Windows.Forms.RadioButton rbAOG;
    private System.Windows.Forms.TextBox txAlt;
    private System.Windows.Forms.Button btTeleport;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label lblLat;
    private System.Windows.Forms.Label label2;
  }
}

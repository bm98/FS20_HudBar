namespace FCamControl
{
  partial class frmKeyConfig
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
      this.flp = new System.Windows.Forms.FlowLayoutPanel();
      this.uC_Hotkey1 = new FCamControl.UC_Hotkey();
      this.btAccept = new System.Windows.Forms.Button();
      this.btCancel = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.flp.SuspendLayout();
      this.SuspendLayout();
      // 
      // flp
      // 
      this.flp.AutoSize = true;
      this.flp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.flp.Controls.Add(this.uC_Hotkey1);
      this.flp.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flp.Location = new System.Drawing.Point(15, 36);
      this.flp.MinimumSize = new System.Drawing.Size(20, 20);
      this.flp.Name = "flp";
      this.flp.Size = new System.Drawing.Size(634, 36);
      this.flp.TabIndex = 3;
      this.flp.WrapContents = false;
      // 
      // uC_Hotkey1
      // 
      this.uC_Hotkey1.BackColor = System.Drawing.Color.DarkGray;
      this.uC_Hotkey1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.uC_Hotkey1.Location = new System.Drawing.Point(1, 1);
      this.uC_Hotkey1.Margin = new System.Windows.Forms.Padding(1);
      this.uC_Hotkey1.MSFS_Key = null;
      this.uC_Hotkey1.Name = "uC_Hotkey1";
      this.uC_Hotkey1.Size = new System.Drawing.Size(632, 34);
      this.uC_Hotkey1.TabIndex = 0;
      // 
      // btAccept
      // 
      this.btAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btAccept.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btAccept.Location = new System.Drawing.Point(440, 636);
      this.btAccept.Name = "btAccept";
      this.btAccept.Size = new System.Drawing.Size(83, 40);
      this.btAccept.TabIndex = 4;
      this.btAccept.Text = "Accept";
      this.btAccept.UseVisualStyleBackColor = true;
      this.btAccept.Click += new System.EventHandler(this.btAccept_Click);
      // 
      // btCancel
      // 
      this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btCancel.Location = new System.Drawing.Point(569, 636);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(83, 40);
      this.btCancel.TabIndex = 5;
      this.btCancel.Text = "Cancel";
      this.btCancel.UseVisualStyleBackColor = true;
      this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(141, 15);
      this.label1.TabIndex = 6;
      this.label1.Text = "MSFS Key Configuration";
      // 
      // frmKeyConfig
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.ClientSize = new System.Drawing.Size(664, 688);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btCancel);
      this.Controls.Add(this.btAccept);
      this.Controls.Add(this.flp);
      this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "frmKeyConfig";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "MSFS Keyboard Keys";
      this.Load += new System.EventHandler(this.frmKeyConfig_Load);
      this.flp.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel flp;
    private System.Windows.Forms.Button btAccept;
    private System.Windows.Forms.Button btCancel;
    private System.Windows.Forms.Label label1;
    private UC_Hotkey uC_Hotkey1;
  }
}
namespace FCamControl
{
  partial class UC_Hotkey
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
      this.btConfig = new System.Windows.Forms.Button();
      this.lblName = new System.Windows.Forms.Label();
      this.txEntry = new System.Windows.Forms.RichTextBox();
      this.btDefault = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btConfig
      // 
      this.btConfig.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btConfig.Location = new System.Drawing.Point(447, 2);
      this.btConfig.Name = "btConfig";
      this.btConfig.Size = new System.Drawing.Size(85, 23);
      this.btConfig.TabIndex = 6;
      this.btConfig.Text = "Config...";
      this.btConfig.UseVisualStyleBackColor = true;
      this.btConfig.Click += new System.EventHandler(this.btConfig_Click);
      // 
      // lblName
      // 
      this.lblName.AutoSize = true;
      this.lblName.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblName.Location = new System.Drawing.Point(3, 6);
      this.lblName.Name = "lblName";
      this.lblName.Size = new System.Drawing.Size(134, 15);
      this.lblName.TabIndex = 4;
      this.lblName.Text = "Drone Move FORWARD";
      // 
      // txEntry
      // 
      this.txEntry.AcceptsTab = true;
      this.txEntry.BackColor = System.Drawing.Color.Honeydew;
      this.txEntry.CausesValidation = false;
      this.txEntry.Cursor = System.Windows.Forms.Cursors.Arrow;
      this.txEntry.DetectUrls = false;
      this.txEntry.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txEntry.HideSelection = false;
      this.txEntry.Location = new System.Drawing.Point(199, 3);
      this.txEntry.Multiline = false;
      this.txEntry.Name = "txEntry";
      this.txEntry.ReadOnly = true;
      this.txEntry.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
      this.txEntry.ShortcutsEnabled = false;
      this.txEntry.Size = new System.Drawing.Size(242, 26);
      this.txEntry.TabIndex = 7;
      this.txEntry.Text = "..";
      this.txEntry.WordWrap = false;
      // 
      // btDefault
      // 
      this.btDefault.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btDefault.Location = new System.Drawing.Point(538, 2);
      this.btDefault.Name = "btDefault";
      this.btDefault.Size = new System.Drawing.Size(85, 23);
      this.btDefault.TabIndex = 8;
      this.btDefault.Text = "Default";
      this.btDefault.UseVisualStyleBackColor = true;
      this.btDefault.Click += new System.EventHandler(this.btDefault_Click);
      // 
      // UC_Hotkey
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BackColor = System.Drawing.Color.Gainsboro;
      this.Controls.Add(this.btDefault);
      this.Controls.Add(this.txEntry);
      this.Controls.Add(this.btConfig);
      this.Controls.Add(this.lblName);
      this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(1);
      this.Name = "UC_Hotkey";
      this.Size = new System.Drawing.Size(632, 34);
      this.Load += new System.EventHandler(this.UC_Hotkey_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btConfig;
    private System.Windows.Forms.Label lblName;
    private System.Windows.Forms.RichTextBox txEntry;
    private System.Windows.Forms.Button btDefault;
  }
}

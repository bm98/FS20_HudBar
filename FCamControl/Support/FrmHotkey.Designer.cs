
namespace FCamControl
{
  partial class FrmHotkey
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent( )
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmHotkey));
      this.lblProfile = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.btCancel = new System.Windows.Forms.Button();
      this.btAccept = new System.Windows.Forms.Button();
      this.label2 = new System.Windows.Forms.Label();
      this.txEntry = new System.Windows.Forms.RichTextBox();
      this.SuspendLayout();
      // 
      // lblProfile
      // 
      this.lblProfile.AutoSize = true;
      this.lblProfile.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblProfile.Location = new System.Drawing.Point(12, 36);
      this.lblProfile.Name = "lblProfile";
      this.lblProfile.Size = new System.Drawing.Size(60, 17);
      this.lblProfile.TabIndex = 1;
      this.lblProfile.Text = "Profile 1";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(216, 17);
      this.label1.TabIndex = 2;
      this.label1.Text = "Type a key combination for this key:";
      // 
      // btCancel
      // 
      this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btCancel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btCancel.Location = new System.Drawing.Point(191, 138);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(100, 29);
      this.btCancel.TabIndex = 3;
      this.btCancel.Text = "Cancel";
      this.btCancel.UseVisualStyleBackColor = true;
      this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
      // 
      // btAccept
      // 
      this.btAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btAccept.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btAccept.Location = new System.Drawing.Point(12, 138);
      this.btAccept.Name = "btAccept";
      this.btAccept.Size = new System.Drawing.Size(100, 29);
      this.btAccept.TabIndex = 2;
      this.btAccept.Text = "Accept";
      this.btAccept.UseVisualStyleBackColor = true;
      this.btAccept.Click += new System.EventHandler(this.btAccept_Click);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 101);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(260, 17);
      this.label2.TabIndex = 5;
      this.label2.Text = "Note: Return and NP Enter cannot be used.";
      // 
      // txEntry
      // 
      this.txEntry.AcceptsTab = true;
      this.txEntry.BackColor = System.Drawing.Color.Honeydew;
      this.txEntry.CausesValidation = false;
      this.txEntry.DetectUrls = false;
      this.txEntry.HideSelection = false;
      this.txEntry.Location = new System.Drawing.Point(12, 60);
      this.txEntry.Multiline = false;
      this.txEntry.Name = "txEntry";
      this.txEntry.ReadOnly = true;
      this.txEntry.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
      this.txEntry.ShortcutsEnabled = false;
      this.txEntry.Size = new System.Drawing.Size(279, 26);
      this.txEntry.TabIndex = 0;
      this.txEntry.Text = "..";
      this.txEntry.WordWrap = false;
      this.txEntry.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txEntry_KeyPress);
      this.txEntry.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txEntry_KeyUp);
      this.txEntry.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txEntry_PreviewKeyDown);
      // 
      // FrmHotkey
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.ClientSize = new System.Drawing.Size(306, 175);
      this.ControlBox = false;
      this.Controls.Add(this.txEntry);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.btAccept);
      this.Controls.Add(this.btCancel);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.lblProfile);
      this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FrmHotkey";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Configure a Key";
      this.Load += new System.EventHandler(this.FrmHotkey_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Label lblProfile;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btCancel;
    private System.Windows.Forms.Button btAccept;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.RichTextBox txEntry;
  }
}
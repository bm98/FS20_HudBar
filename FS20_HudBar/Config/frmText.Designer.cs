
namespace FS20_HudBar.Config
{
  partial class frmText
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmText));
      this.txFree = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.btCancel = new System.Windows.Forms.Button();
      this.btAccept = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // txFree
      // 
      this.txFree.Location = new System.Drawing.Point(13, 34);
      this.txFree.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txFree.Name = "txFree";
      this.txFree.Size = new System.Drawing.Size(260, 27);
      this.txFree.TabIndex = 0;
      this.txFree.Text = "...";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(71, 20);
      this.label1.TabIndex = 1;
      this.label1.Text = "Free Text:";
      // 
      // btCancel
      // 
      this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btCancel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btCancel.Location = new System.Drawing.Point(167, 84);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(106, 25);
      this.btCancel.TabIndex = 2;
      this.btCancel.Text = "Cancel";
      this.btCancel.UseVisualStyleBackColor = true;
      // 
      // btAccept
      // 
      this.btAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btAccept.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btAccept.Location = new System.Drawing.Point(13, 84);
      this.btAccept.Name = "btAccept";
      this.btAccept.Size = new System.Drawing.Size(106, 25);
      this.btAccept.TabIndex = 1;
      this.btAccept.Text = "Accept";
      this.btAccept.UseVisualStyleBackColor = true;
      this.btAccept.Click += new System.EventHandler(this.btAccept_Click);
      // 
      // frmText
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.CancelButton = this.btCancel;
      this.ClientSize = new System.Drawing.Size(293, 125);
      this.Controls.Add(this.btCancel);
      this.Controls.Add(this.btAccept);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txFree);
      this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "frmText";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Free Text Entry";
      this.TopMost = true;
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmText_FormClosing);
      this.Load += new System.EventHandler(this.frmText_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txFree;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btCancel;
    private System.Windows.Forms.Button btAccept;
  }
}
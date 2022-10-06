
namespace FS20_HudBar.Config
{
  partial class frmApt
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmApt));
      this.btCancel = new System.Windows.Forms.Button();
      this.btAccept = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.txArr = new System.Windows.Forms.TextBox();
      this.btClear = new System.Windows.Forms.Button();
      this.txDep = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // btCancel
      // 
      this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btCancel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btCancel.Location = new System.Drawing.Point(166, 137);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(106, 25);
      this.btCancel.TabIndex = 3;
      this.btCancel.Text = "Cancel";
      this.btCancel.UseVisualStyleBackColor = true;
      // 
      // btAccept
      // 
      this.btAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btAccept.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btAccept.Location = new System.Drawing.Point(12, 137);
      this.btAccept.Name = "btAccept";
      this.btAccept.Size = new System.Drawing.Size(106, 25);
      this.btAccept.TabIndex = 2;
      this.btAccept.Text = "Accept";
      this.btAccept.UseVisualStyleBackColor = true;
      this.btAccept.Click += new System.EventHandler(this.btAccept_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(16, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(97, 20);
      this.label1.TabIndex = 5;
      this.label1.Text = "Airport ICAO:";
      // 
      // txArr
      // 
      this.txArr.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
      this.txArr.Location = new System.Drawing.Point(60, 88);
      this.txArr.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txArr.Name = "txArr";
      this.txArr.Size = new System.Drawing.Size(105, 27);
      this.txArr.TabIndex = 1;
      this.txArr.Text = "...";
      // 
      // btClear
      // 
      this.btClear.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btClear.Location = new System.Drawing.Point(209, 54);
      this.btClear.Name = "btClear";
      this.btClear.Size = new System.Drawing.Size(63, 48);
      this.btClear.TabIndex = 4;
      this.btClear.Text = "Clear";
      this.btClear.UseVisualStyleBackColor = true;
      this.btClear.Click += new System.EventHandler(this.btClear_Click);
      // 
      // txDep
      // 
      this.txDep.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
      this.txDep.Location = new System.Drawing.Point(59, 39);
      this.txDep.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txDep.Name = "txDep";
      this.txDep.Size = new System.Drawing.Size(106, 27);
      this.txDep.TabIndex = 0;
      this.txDep.Text = "...";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(16, 42);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(36, 20);
      this.label3.TabIndex = 8;
      this.label3.Text = "DEP";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(16, 91);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(37, 20);
      this.label4.TabIndex = 9;
      this.label4.Text = "ARR";
      // 
      // frmApt
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.CancelButton = this.btCancel;
      this.ClientSize = new System.Drawing.Size(284, 175);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.txDep);
      this.Controls.Add(this.btClear);
      this.Controls.Add(this.btCancel);
      this.Controls.Add(this.btAccept);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txArr);
      this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "frmApt";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Route Departure - Destination";
      this.TopMost = true;
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmApt_FormClosing);
      this.Load += new System.EventHandler(this.frmApt_Load);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btCancel;
    private System.Windows.Forms.Button btAccept;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txArr;
    private System.Windows.Forms.Button btClear;
    private System.Windows.Forms.TextBox txDep;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
  }
}
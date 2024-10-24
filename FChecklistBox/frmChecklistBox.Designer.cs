
using System;

namespace FChecklistBox
{
  partial class frmChecklistBox
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
      this.components = new System.ComponentModel.Container();
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmChecklistBox));
      this.chklistBox = new bm98_Checklist.UC_Checklist();
      this.SuspendLayout();
      // 
      // chklistBox
      // 
      this.chklistBox.AutoSize = true;
      this.chklistBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.chklistBox.BackColor = System.Drawing.Color.Transparent;
      this.chklistBox.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chklistBox.BackgroundImage")));
      this.chklistBox.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.chklistBox.CausesValidation = false;
      this.chklistBox.Cursor = System.Windows.Forms.Cursors.SizeAll;
      this.chklistBox.Dock = System.Windows.Forms.DockStyle.Fill;
      this.chklistBox.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chklistBox.Location = new System.Drawing.Point(10, 10);
      this.chklistBox.Name = "chklistBox";
      this.chklistBox.Padding = new System.Windows.Forms.Padding(2);
      this.chklistBox.Size = new System.Drawing.Size(245, 367);
      this.chklistBox.TabIndex = 0;
      this.chklistBox.UserDir = "";
      this.chklistBox.HideClicked += new System.EventHandler<EventArgs>(this.chklistBox_HideClicked);
      this.chklistBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmChecklistBox_MouseDown);
      this.chklistBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmChecklistBox_MouseMove);
      this.chklistBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmChecklistBox_MouseUp);
      // 
      // frmChecklistBox
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.AutoSize = true;
      this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(10)))), ((int)(((byte)(10)))), ((int)(((byte)(0)))));
      this.ClientSize = new System.Drawing.Size(265, 387);
      this.ControlBox = false;
      this.Controls.Add(this.chklistBox);
      this.Cursor = System.Windows.Forms.Cursors.Default;
      this.DoubleBuffered = true;
      this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "frmChecklistBox";
      this.Padding = new System.Windows.Forms.Padding(10);
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "Checklist Box";
      this.Activated += new System.EventHandler(this.frmChecklistBox_Activated);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmChecklistBox_FormClosing);
      this.Load += new System.EventHandler(this.frmChecklistBox_Load);
      this.LocationChanged += new System.EventHandler(this.frmChecklistBox_LocationChanged);
      this.VisibleChanged += new System.EventHandler(this.frmChecklistBox_VisibleChanged);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmChecklistBox_MouseDown);
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmChecklistBox_MouseMove);
      this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmChecklistBox_MouseUp);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private bm98_Checklist.UC_Checklist chklistBox;
  }
}
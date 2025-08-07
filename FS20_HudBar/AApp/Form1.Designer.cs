
namespace FS20_HudBar
{
  partial class frmMain
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
      this.components = new System.ComponentModel.Container();
      this.lblProto = new System.Windows.Forms.Label();
      this.flpMAIN = new System.Windows.Forms.FlowLayoutPanel();
      this.valueProto = new System.Windows.Forms.Label();
      this.cMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.mSelProfile = new System.Windows.Forms.ToolStripTextBox();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.mProfile = new System.Windows.Forms.ToolStripMenuItem();
      this.mP1 = new System.Windows.Forms.ToolStripMenuItem();
      this.mP2 = new System.Windows.Forms.ToolStripMenuItem();
      this.mP3 = new System.Windows.Forms.ToolStripMenuItem();
      this.mP4 = new System.Windows.Forms.ToolStripMenuItem();
      this.mP5 = new System.Windows.Forms.ToolStripMenuItem();
      this.mP6 = new System.Windows.Forms.ToolStripMenuItem();
      this.mP7 = new System.Windows.Forms.ToolStripMenuItem();
      this.mP8 = new System.Windows.Forms.ToolStripMenuItem();
      this.mP9 = new System.Windows.Forms.ToolStripMenuItem();
      this.mP10 = new System.Windows.Forms.ToolStripMenuItem();
      this.mAppearance = new System.Windows.Forms.ToolStripMenuItem();
      this.maBright = new System.Windows.Forms.ToolStripMenuItem();
      this.maDimm = new System.Windows.Forms.ToolStripMenuItem();
      this.maDark = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.mShelf = new System.Windows.Forms.ToolStripMenuItem();
      this.mCamera = new System.Windows.Forms.ToolStripMenuItem();
      this.mChecklistBox = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
      this.mUnits = new System.Windows.Forms.ToolStripMenuItem();
      this.mAltMetric = new System.Windows.Forms.ToolStripMenuItem();
      this.mDistMetric = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
      this.mShowUnits = new System.Windows.Forms.ToolStripMenuItem();
      this.mConfig = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.mManBackup = new System.Windows.Forms.ToolStripMenuItem();
      this.mManSaveLanding = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
      this.mExit = new System.Windows.Forms.ToolStripMenuItem();
      this.signProto = new System.Windows.Forms.Label();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.value2Proto = new System.Windows.Forms.Label();
      this.B = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.cMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // lblProto
      // 
      this.lblProto.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.lblProto.AutoSize = true;
      this.lblProto.BackColor = System.Drawing.Color.Transparent;
      this.lblProto.Font = new System.Drawing.Font("Bahnschrift", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblProto.ForeColor = System.Drawing.Color.Silver;
      this.lblProto.Location = new System.Drawing.Point(510, 44);
      this.lblProto.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
      this.lblProto.Name = "lblProto";
      this.lblProto.Size = new System.Drawing.Size(43, 16);
      this.lblProto.TabIndex = 0;
      this.lblProto.Text = "ETrim:";
      this.lblProto.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.lblProto.Visible = false;
      // 
      // flpMAIN
      // 
      this.flpMAIN.AutoSize = true;
      this.flpMAIN.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(1)))));
      this.flpMAIN.CausesValidation = false;
      this.flpMAIN.Location = new System.Drawing.Point(12, 12);
      this.flpMAIN.Name = "flpMAIN";
      this.flpMAIN.Size = new System.Drawing.Size(110, 31);
      this.flpMAIN.TabIndex = 1;
      this.flpMAIN.Visible = false;
      this.flpMAIN.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseDown);
      this.flpMAIN.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseMove);
      this.flpMAIN.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseUp);
      // 
      // valueProto
      // 
      this.valueProto.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.valueProto.AutoSize = true;
      this.valueProto.BackColor = System.Drawing.Color.Transparent;
      this.valueProto.Font = new System.Drawing.Font("Lucida Console", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.valueProto.ForeColor = System.Drawing.Color.WhiteSmoke;
      this.valueProto.Location = new System.Drawing.Point(586, 44);
      this.valueProto.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
      this.valueProto.Name = "valueProto";
      this.valueProto.Size = new System.Drawing.Size(56, 25);
      this.valueProto.TabIndex = 1;
      this.valueProto.Text = "-20%";
      this.valueProto.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.valueProto.UseCompatibleTextRendering = true;
      this.valueProto.Visible = false;
      // 
      // cMenu
      // 
      this.cMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mSelProfile,
            this.toolStripSeparator1,
            this.mProfile,
            this.mAppearance,
            this.toolStripSeparator2,
            this.mShelf,
            this.mCamera,
            this.mChecklistBox,
            this.toolStripSeparator4,
            this.mUnits,
            this.mConfig,
            this.toolStripSeparator3,
            this.mManBackup,
            this.mManSaveLanding,
            this.toolStripSeparator6,
            this.mExit});
      this.cMenu.Name = "cMenu";
      this.cMenu.Size = new System.Drawing.Size(183, 279);
      // 
      // mSelProfile
      // 
      this.mSelProfile.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.mSelProfile.Name = "mSelProfile";
      this.mSelProfile.ReadOnly = true;
      this.mSelProfile.Size = new System.Drawing.Size(100, 23);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
      // 
      // mProfile
      // 
      this.mProfile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mP1,
            this.mP2,
            this.mP3,
            this.mP4,
            this.mP5,
            this.mP6,
            this.mP7,
            this.mP8,
            this.mP9,
            this.mP10});
      this.mProfile.Name = "mProfile";
      this.mProfile.Size = new System.Drawing.Size(186, 22);
      this.mProfile.Text = "Select Profile";
      // 
      // mP1
      // 
      this.mP1.Name = "mP1";
      this.mP1.Size = new System.Drawing.Size(123, 22);
      this.mP1.Text = "Profile 1";
      this.mP1.Click += new System.EventHandler(this.mP1_Click);
      // 
      // mP2
      // 
      this.mP2.Name = "mP2";
      this.mP2.Size = new System.Drawing.Size(123, 22);
      this.mP2.Text = "Profile 2";
      this.mP2.Click += new System.EventHandler(this.mP2_Click);
      // 
      // mP3
      // 
      this.mP3.Name = "mP3";
      this.mP3.Size = new System.Drawing.Size(123, 22);
      this.mP3.Text = "Profile 3";
      this.mP3.Click += new System.EventHandler(this.mP3_Click);
      // 
      // mP4
      // 
      this.mP4.Name = "mP4";
      this.mP4.Size = new System.Drawing.Size(123, 22);
      this.mP4.Text = "Profile 4";
      this.mP4.Click += new System.EventHandler(this.mP4_Click);
      // 
      // mP5
      // 
      this.mP5.Name = "mP5";
      this.mP5.Size = new System.Drawing.Size(123, 22);
      this.mP5.Text = "Profile 5";
      this.mP5.Click += new System.EventHandler(this.mP5_Click);
      // 
      // mP6
      // 
      this.mP6.Name = "mP6";
      this.mP6.Size = new System.Drawing.Size(123, 22);
      this.mP6.Text = "Profile 6";
      this.mP6.Click += new System.EventHandler(this.mP6_Click);
      // 
      // mP7
      // 
      this.mP7.Name = "mP7";
      this.mP7.Size = new System.Drawing.Size(123, 22);
      this.mP7.Text = "Profile 7";
      this.mP7.Click += new System.EventHandler(this.mP7_Click);
      // 
      // mP8
      // 
      this.mP8.Name = "mP8";
      this.mP8.Size = new System.Drawing.Size(123, 22);
      this.mP8.Text = "Profile 8";
      this.mP8.Click += new System.EventHandler(this.mP8_Click);
      // 
      // mP9
      // 
      this.mP9.Name = "mP9";
      this.mP9.Size = new System.Drawing.Size(123, 22);
      this.mP9.Text = "Profile 9";
      this.mP9.Click += new System.EventHandler(this.mP9_Click);
      // 
      // mP10
      // 
      this.mP10.Name = "mP10";
      this.mP10.Size = new System.Drawing.Size(123, 22);
      this.mP10.Text = "Profile 10";
      this.mP10.Click += new System.EventHandler(this.mP10_Click);
      // 
      // mAppearance
      // 
      this.mAppearance.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.maBright,
            this.maDimm,
            this.maDark});
      this.mAppearance.Name = "mAppearance";
      this.mAppearance.Size = new System.Drawing.Size(186, 22);
      this.mAppearance.Text = "Appearance";
      this.mAppearance.DropDownOpening += new System.EventHandler(this.mAppearance_DropDownOpening);
      // 
      // maBright
      // 
      this.maBright.Name = "maBright";
      this.maBright.Size = new System.Drawing.Size(120, 22);
      this.maBright.Text = "Bright";
      this.maBright.Click += new System.EventHandler(this.maBright_Click);
      // 
      // maDimm
      // 
      this.maDimm.Name = "maDimm";
      this.maDimm.Size = new System.Drawing.Size(120, 22);
      this.maDimm.Text = "Dimmed";
      this.maDimm.Click += new System.EventHandler(this.maDimm_Click);
      // 
      // maDark
      // 
      this.maDark.Name = "maDark";
      this.maDark.Size = new System.Drawing.Size(120, 22);
      this.maDark.Text = "Inverse";
      this.maDark.Click += new System.EventHandler(this.maDark_Click);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(183, 6);
      // 
      // mShelf
      // 
      this.mShelf.Name = "mShelf";
      this.mShelf.Size = new System.Drawing.Size(186, 22);
      this.mShelf.Text = "Flight Bag...";
      this.mShelf.Click += new System.EventHandler(this.mShelf_Click);
      // 
      // mCamera
      // 
      this.mCamera.Name = "mCamera";
      this.mCamera.Size = new System.Drawing.Size(186, 22);
      this.mCamera.Text = "Camera...";
      this.mCamera.Click += new System.EventHandler(this.mCamera_Click);
      // 
      // mChecklistBox
      // 
      this.mChecklistBox.Name = "mChecklistBox";
      this.mChecklistBox.Size = new System.Drawing.Size(186, 22);
      this.mChecklistBox.Text = "Checklist Box...";
      this.mChecklistBox.Click += new System.EventHandler(this.mChecklistBox_Click);
      // 
      // toolStripSeparator4
      // 
      this.toolStripSeparator4.Name = "toolStripSeparator4";
      this.toolStripSeparator4.Size = new System.Drawing.Size(183, 6);
      // 
      // mUnits
      // 
      this.mUnits.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mAltMetric,
            this.mDistMetric,
            this.toolStripSeparator5,
            this.mShowUnits});
      this.mUnits.Name = "mUnits";
      this.mUnits.Size = new System.Drawing.Size(186, 22);
      this.mUnits.Text = "Units";
      // 
      // mAltMetric
      // 
      this.mAltMetric.CheckOnClick = true;
      this.mAltMetric.Name = "mAltMetric";
      this.mAltMetric.Size = new System.Drawing.Size(184, 22);
      this.mAltMetric.Text = "Metric Altitudes (m)";
      this.mAltMetric.CheckedChanged += new System.EventHandler(this.mAltMetric_CheckedChanged);
      // 
      // mDistMetric
      // 
      this.mDistMetric.CheckOnClick = true;
      this.mDistMetric.Name = "mDistMetric";
      this.mDistMetric.Size = new System.Drawing.Size(184, 22);
      this.mDistMetric.Text = "Metric Distance (km)";
      this.mDistMetric.CheckedChanged += new System.EventHandler(this.mDistMetric_CheckedChanged);
      // 
      // toolStripSeparator5
      // 
      this.toolStripSeparator5.Name = "toolStripSeparator5";
      this.toolStripSeparator5.Size = new System.Drawing.Size(181, 6);
      // 
      // mShowUnits
      // 
      this.mShowUnits.CheckOnClick = true;
      this.mShowUnits.Name = "mShowUnits";
      this.mShowUnits.Size = new System.Drawing.Size(184, 22);
      this.mShowUnits.Text = "Show Units";
      this.mShowUnits.CheckedChanged += new System.EventHandler(this.mShowUnits_CheckedChanged);
      // 
      // mConfig
      // 
      this.mConfig.Name = "mConfig";
      this.mConfig.Size = new System.Drawing.Size(186, 22);
      this.mConfig.Text = "Configure...";
      this.mConfig.Click += new System.EventHandler(this.mConfig_Click);
      // 
      // toolStripSeparator3
      // 
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(183, 6);
      // 
      // mManBackup
      // 
      this.mManBackup.Name = "mManBackup";
      this.mManBackup.Size = new System.Drawing.Size(186, 22);
      this.mManBackup.Text = "Save FLT";
      this.mManBackup.Click += new System.EventHandler(this.mManBackup_Click);
      // 
      // mManSaveLanding
      // 
      this.mManSaveLanding.Name = "mManSaveLanding";
      this.mManSaveLanding.Size = new System.Drawing.Size(186, 22);
      this.mManSaveLanding.Text = "Save Landing Sketch";
      this.mManSaveLanding.Click += new System.EventHandler(this.mManSaveLanding_Click);
      // 
      // toolStripSeparator6
      // 
      this.toolStripSeparator6.Name = "toolStripSeparator6";
      this.toolStripSeparator6.Size = new System.Drawing.Size(183, 6);
      // 
      // mExit
      // 
      this.mExit.Name = "mExit";
      this.mExit.Size = new System.Drawing.Size(186, 22);
      this.mExit.Text = "Exit";
      this.mExit.Click += new System.EventHandler(this.mExit_Click);
      // 
      // signProto
      // 
      this.signProto.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.signProto.AutoSize = true;
      this.signProto.Font = new System.Drawing.Font("Wingdings", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
      this.signProto.ForeColor = System.Drawing.Color.WhiteSmoke;
      this.signProto.Location = new System.Drawing.Point(761, 44);
      this.signProto.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
      this.signProto.Name = "signProto";
      this.signProto.Size = new System.Drawing.Size(106, 29);
      this.signProto.TabIndex = 2;
      this.signProto.Text = "";
      this.signProto.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
      this.signProto.UseCompatibleTextRendering = true;
      this.signProto.Visible = false;
      // 
      // timer1
      // 
      this.timer1.Interval = 1000;
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // value2Proto
      // 
      this.value2Proto.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.value2Proto.AutoSize = true;
      this.value2Proto.BackColor = System.Drawing.Color.Transparent;
      this.value2Proto.Font = new System.Drawing.Font("Lucida Console", 12F, System.Drawing.FontStyle.Bold);
      this.value2Proto.ForeColor = System.Drawing.Color.WhiteSmoke;
      this.value2Proto.Location = new System.Drawing.Point(677, 47);
      this.value2Proto.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
      this.value2Proto.Name = "value2Proto";
      this.value2Proto.Size = new System.Drawing.Size(47, 21);
      this.value2Proto.TabIndex = 3;
      this.value2Proto.Text = "-20%";
      this.value2Proto.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.value2Proto.UseCompatibleTextRendering = true;
      this.value2Proto.Visible = false;
      // 
      // B
      // 
      this.B.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.B.AutoSize = true;
      this.B.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.B.FlatAppearance.BorderSize = 0;
      this.B.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.B.ForeColor = System.Drawing.Color.Coral;
      this.B.Location = new System.Drawing.Point(734, 21);
      this.B.Name = "B";
      this.B.Size = new System.Drawing.Size(25, 26);
      this.B.TabIndex = 4;
      this.B.Text = "B";
      this.B.UseVisualStyleBackColor = true;
      this.B.Visible = false;
      // 
      // label1
      // 
      this.label1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.label1.AutoSize = true;
      this.label1.BackColor = System.Drawing.Color.Transparent;
      this.label1.Font = new System.Drawing.Font("Share Tech Mono", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ForeColor = System.Drawing.Color.WhiteSmoke;
      this.label1.Location = new System.Drawing.Point(681, 12);
      this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(43, 24);
      this.label1.TabIndex = 5;
      this.label1.Text = "-20%";
      this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
      this.label1.UseCompatibleTextRendering = true;
      this.label1.Visible = false;
      // 
      // frmMain
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BackColor = System.Drawing.Color.Black;
      this.CausesValidation = false;
      this.ClientSize = new System.Drawing.Size(872, 81);
      this.ContextMenuStrip = this.cMenu;
      this.Controls.Add(this.label1);
      this.Controls.Add(this.flpMAIN);
      this.Controls.Add(this.B);
      this.Controls.Add(this.value2Proto);
      this.Controls.Add(this.signProto);
      this.Controls.Add(this.lblProto);
      this.Controls.Add(this.valueProto);
      this.Cursor = System.Windows.Forms.Cursors.Arrow;
      this.Font = new System.Drawing.Font("Bahnschrift", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "frmMain";
      this.Padding = new System.Windows.Forms.Padding(0, 3, 0, 0);
      this.ShowIcon = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "HudBar by bm98ch";
      this.TopMost = true;
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
      this.Load += new System.EventHandler(this.frmMain_Load);
      this.LocationChanged += new System.EventHandler(this.frmMain_LocationChanged);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseDown);
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseMove);
      this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseUp);
      this.Move += new System.EventHandler(this.frmMain_Move);
      this.cMenu.ResumeLayout(false);
      this.cMenu.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblProto;
    private System.Windows.Forms.FlowLayoutPanel flpMAIN;
    private System.Windows.Forms.Label valueProto;
    private System.Windows.Forms.ContextMenuStrip cMenu;
    private System.Windows.Forms.ToolStripMenuItem mExit;
    private System.Windows.Forms.Label signProto;
    private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.ToolStripMenuItem mConfig;
    private System.Windows.Forms.ToolStripMenuItem mProfile;
    private System.Windows.Forms.ToolStripMenuItem mP1;
    private System.Windows.Forms.ToolStripMenuItem mP2;
    private System.Windows.Forms.ToolStripMenuItem mP3;
    private System.Windows.Forms.ToolStripMenuItem mP4;
    private System.Windows.Forms.ToolStripMenuItem mP5;
    private System.Windows.Forms.Label value2Proto;
    private System.Windows.Forms.Button B;
    private System.Windows.Forms.ToolStripTextBox mSelProfile;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ToolStripMenuItem mAppearance;
    private System.Windows.Forms.ToolStripMenuItem maBright;
    private System.Windows.Forms.ToolStripMenuItem maDimm;
    private System.Windows.Forms.ToolStripMenuItem maDark;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripMenuItem mShelf;
    private System.Windows.Forms.ToolStripMenuItem mCamera;
    private System.Windows.Forms.ToolStripMenuItem mUnits;
    private System.Windows.Forms.ToolStripMenuItem mAltMetric;
    private System.Windows.Forms.ToolStripMenuItem mDistMetric;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
    private System.Windows.Forms.ToolStripMenuItem mShowUnits;
    private System.Windows.Forms.ToolStripMenuItem mP6;
    private System.Windows.Forms.ToolStripMenuItem mP7;
    private System.Windows.Forms.ToolStripMenuItem mP8;
    private System.Windows.Forms.ToolStripMenuItem mP9;
    private System.Windows.Forms.ToolStripMenuItem mP10;
    private System.Windows.Forms.ToolStripMenuItem mChecklistBox;
    private System.Windows.Forms.ToolStripMenuItem mManBackup;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
    private System.Windows.Forms.ToolStripMenuItem mManSaveLanding;
  }
}


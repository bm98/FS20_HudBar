
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
      this.flp = new System.Windows.Forms.FlowLayoutPanel();
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
      this.mConfig = new System.Windows.Forms.ToolStripMenuItem();
      this.mExit = new System.Windows.Forms.ToolStripMenuItem();
      this.signProto = new System.Windows.Forms.Label();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.value2Proto = new System.Windows.Forms.Label();
      this.B = new System.Windows.Forms.Button();
      this.cMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // lblProto
      // 
      this.lblProto.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.lblProto.AutoSize = true;
      this.lblProto.Font = new System.Drawing.Font("Bahnschrift", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblProto.ForeColor = System.Drawing.Color.WhiteSmoke;
      this.lblProto.Location = new System.Drawing.Point(510, 38);
      this.lblProto.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
      this.lblProto.Name = "lblProto";
      this.lblProto.Size = new System.Drawing.Size(44, 16);
      this.lblProto.TabIndex = 0;
      this.lblProto.Text = "ETrim:";
      this.lblProto.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
      this.lblProto.Visible = false;
      // 
      // flp
      // 
      this.flp.AutoSize = true;
      this.flp.CausesValidation = false;
      this.flp.Location = new System.Drawing.Point(12, 12);
      this.flp.Name = "flp";
      this.flp.Size = new System.Drawing.Size(110, 31);
      this.flp.TabIndex = 1;
      this.flp.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseDown);
      this.flp.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseMove);
      this.flp.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseUp);
      // 
      // valueProto
      // 
      this.valueProto.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.valueProto.AutoSize = true;
      this.valueProto.Font = new System.Drawing.Font("Lucida Console", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.valueProto.ForeColor = System.Drawing.Color.WhiteSmoke;
      this.valueProto.Location = new System.Drawing.Point(586, 38);
      this.valueProto.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
      this.valueProto.Name = "valueProto";
      this.valueProto.Size = new System.Drawing.Size(57, 19);
      this.valueProto.TabIndex = 1;
      this.valueProto.Text = "-20%";
      this.valueProto.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
      this.valueProto.Visible = false;
      // 
      // cMenu
      // 
      this.cMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mSelProfile,
            this.toolStripSeparator1,
            this.mProfile,
            this.mConfig,
            this.mExit});
      this.cMenu.Name = "cMenu";
      this.cMenu.Size = new System.Drawing.Size(161, 101);
      // 
      // mSelProfile
      // 
      this.mSelProfile.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.mSelProfile.Name = "mSelProfile";
      this.mSelProfile.Size = new System.Drawing.Size(100, 23);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
      // 
      // mProfile
      // 
      this.mProfile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mP1,
            this.mP2,
            this.mP3,
            this.mP4,
            this.mP5});
      this.mProfile.Name = "mProfile";
      this.mProfile.Size = new System.Drawing.Size(160, 22);
      this.mProfile.Text = "Select Profile";
      // 
      // mP1
      // 
      this.mP1.Name = "mP1";
      this.mP1.Size = new System.Drawing.Size(117, 22);
      this.mP1.Text = "Profile 1";
      this.mP1.Click += new System.EventHandler(this.mP1_Click);
      // 
      // mP2
      // 
      this.mP2.Name = "mP2";
      this.mP2.Size = new System.Drawing.Size(117, 22);
      this.mP2.Text = "Profile 2";
      this.mP2.Click += new System.EventHandler(this.mP2_Click);
      // 
      // mP3
      // 
      this.mP3.Name = "mP3";
      this.mP3.Size = new System.Drawing.Size(117, 22);
      this.mP3.Text = "Profile 3";
      this.mP3.Click += new System.EventHandler(this.mP3_Click);
      // 
      // mP4
      // 
      this.mP4.Name = "mP4";
      this.mP4.Size = new System.Drawing.Size(117, 22);
      this.mP4.Text = "Profile 4";
      this.mP4.Click += new System.EventHandler(this.mP4_Click);
      // 
      // mP5
      // 
      this.mP5.Name = "mP5";
      this.mP5.Size = new System.Drawing.Size(117, 22);
      this.mP5.Text = "Profile 5";
      this.mP5.Click += new System.EventHandler(this.mP5_Click);
      // 
      // mConfig
      // 
      this.mConfig.Name = "mConfig";
      this.mConfig.Size = new System.Drawing.Size(160, 22);
      this.mConfig.Text = "Configure...";
      this.mConfig.Click += new System.EventHandler(this.mConfig_Click);
      // 
      // mExit
      // 
      this.mExit.Name = "mExit";
      this.mExit.Size = new System.Drawing.Size(160, 22);
      this.mExit.Text = "Exit";
      this.mExit.Click += new System.EventHandler(this.mExit_Click);
      // 
      // signProto
      // 
      this.signProto.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
      this.signProto.AutoSize = true;
      this.signProto.Font = new System.Drawing.Font("Wingdings", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
      this.signProto.ForeColor = System.Drawing.Color.WhiteSmoke;
      this.signProto.Location = new System.Drawing.Point(761, 35);
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
      this.value2Proto.Font = new System.Drawing.Font("Lucida Console", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.value2Proto.ForeColor = System.Drawing.Color.WhiteSmoke;
      this.value2Proto.Location = new System.Drawing.Point(677, 38);
      this.value2Proto.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
      this.value2Proto.Name = "value2Proto";
      this.value2Proto.Size = new System.Drawing.Size(52, 16);
      this.value2Proto.TabIndex = 3;
      this.value2Proto.Text = "-20%";
      this.value2Proto.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
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
      this.B.Location = new System.Drawing.Point(734, 12);
      this.B.Name = "B";
      this.B.Size = new System.Drawing.Size(26, 26);
      this.B.TabIndex = 4;
      this.B.Text = "B";
      this.B.UseVisualStyleBackColor = true;
      this.B.Visible = false;
      // 
      // frmMain
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BackColor = System.Drawing.Color.Black;
      this.CausesValidation = false;
      this.ClientSize = new System.Drawing.Size(872, 72);
      this.ContextMenuStrip = this.cMenu;
      this.Controls.Add(this.flp);
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
      this.ShowIcon = false;
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "HudBar by bm98";
      this.TopMost = true;
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
      this.Load += new System.EventHandler(this.frmMain_Load);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseDown);
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseMove);
      this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.frmMain_MouseUp);
      this.cMenu.ResumeLayout(false);
      this.cMenu.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label lblProto;
    private System.Windows.Forms.FlowLayoutPanel flp;
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
  }
}


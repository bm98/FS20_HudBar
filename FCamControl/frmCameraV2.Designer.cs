namespace FCamControl
{
  /// <inheritdoc/>
  partial class frmCameraV2
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmCameraV2));
      this.lblSimConnected = new System.Windows.Forms.Label();
      this.pnlCamButtons = new System.Windows.Forms.Panel();
      this.btDOF6 = new FCamControl.CheckedButton();
      this.lblInop = new System.Windows.Forms.Label();
      this.btResetView = new System.Windows.Forms.Button();
      this.btDrone = new FCamControl.CheckedButton();
      this.btLandView = new FCamControl.CheckedButton();
      this.btExternalFree = new FCamControl.CheckedButton();
      this.btExternalIndexed = new FCamControl.CheckedButton();
      this.btExternalQuick = new FCamControl.CheckedButton();
      this.btInstrumentIndexed = new FCamControl.CheckedButton();
      this.btInstrumentQuick = new FCamControl.CheckedButton();
      this.btCustomCamera = new FCamControl.CheckedButton();
      this.btCloseView = new FCamControl.CheckedButton();
      this.btCoPilotView = new FCamControl.CheckedButton();
      this.btPilotView = new FCamControl.CheckedButton();
      this.ctxMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
      this.mnuKeyConfig = new System.Windows.Forms.ToolStripMenuItem();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.pnlCamButtons.SuspendLayout();
      this.ctxMenu.SuspendLayout();
      this.SuspendLayout();
      // 
      // lblSimConnected
      // 
      this.lblSimConnected.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
      this.lblSimConnected.Dock = System.Windows.Forms.DockStyle.Top;
      this.lblSimConnected.ForeColor = System.Drawing.Color.Chocolate;
      this.lblSimConnected.Location = new System.Drawing.Point(0, 0);
      this.lblSimConnected.Name = "lblSimConnected";
      this.lblSimConnected.Size = new System.Drawing.Size(687, 5);
      this.lblSimConnected.TabIndex = 26;
      // 
      // pnlCamButtons
      // 
      this.pnlCamButtons.BackColor = System.Drawing.Color.Transparent;
      this.pnlCamButtons.Controls.Add(this.btDOF6);
      this.pnlCamButtons.Controls.Add(this.lblInop);
      this.pnlCamButtons.Controls.Add(this.btResetView);
      this.pnlCamButtons.Controls.Add(this.btDrone);
      this.pnlCamButtons.Controls.Add(this.btLandView);
      this.pnlCamButtons.Controls.Add(this.btExternalFree);
      this.pnlCamButtons.Controls.Add(this.btExternalIndexed);
      this.pnlCamButtons.Controls.Add(this.btExternalQuick);
      this.pnlCamButtons.Controls.Add(this.btInstrumentIndexed);
      this.pnlCamButtons.Controls.Add(this.btInstrumentQuick);
      this.pnlCamButtons.Controls.Add(this.btCustomCamera);
      this.pnlCamButtons.Controls.Add(this.btCloseView);
      this.pnlCamButtons.Controls.Add(this.btCoPilotView);
      this.pnlCamButtons.Controls.Add(this.btPilotView);
      this.pnlCamButtons.Location = new System.Drawing.Point(3, 8);
      this.pnlCamButtons.Name = "pnlCamButtons";
      this.pnlCamButtons.Size = new System.Drawing.Size(644, 53);
      this.pnlCamButtons.TabIndex = 34;
      // 
      // btDOF6
      // 
      this.btDOF6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(99)))), ((int)(((byte)(87)))));
      this.btDOF6.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(214)))), ((int)(((byte)(129)))));
      this.btDOF6.BackgroundImage = global::FCamControl.Properties.Resources.bt6DOF;
      this.btDOF6.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btDOF6.Checked = false;
      this.btDOF6.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btDOF6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btDOF6.ForeColor = System.Drawing.Color.Black;
      this.btDOF6.ForeColorChecked = System.Drawing.Color.Yellow;
      this.btDOF6.Location = new System.Drawing.Point(537, 6);
      this.btDOF6.Name = "btDOF6";
      this.btDOF6.Size = new System.Drawing.Size(40, 40);
      this.btDOF6.TabIndex = 36;
      this.btDOF6.TabStop = false;
      this.btDOF6.ThreeState = false;
      this.btDOF6.UseVisualStyleBackColor = false;
      // 
      // lblInop
      // 
      this.lblInop.AutoSize = true;
      this.lblInop.BackColor = System.Drawing.Color.Khaki;
      this.lblInop.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblInop.Location = new System.Drawing.Point(176, 7);
      this.lblInop.Name = "lblInop";
      this.lblInop.Size = new System.Drawing.Size(257, 30);
      this.lblInop.TabIndex = 36;
      this.lblInop.Text = "Inop - while not in flight";
      this.lblInop.Visible = false;
      // 
      // btResetView
      // 
      this.btResetView.BackColor = System.Drawing.Color.Gray;
      this.btResetView.BackgroundImage = global::FCamControl.Properties.Resources.ResetBt;
      this.btResetView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btResetView.CausesValidation = false;
      this.btResetView.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btResetView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btResetView.ForeColor = System.Drawing.Color.Black;
      this.btResetView.Location = new System.Drawing.Point(601, 6);
      this.btResetView.Name = "btResetView";
      this.btResetView.Size = new System.Drawing.Size(40, 40);
      this.btResetView.TabIndex = 26;
      this.btResetView.TabStop = false;
      this.btResetView.UseVisualStyleBackColor = false;
      this.btResetView.Click += new System.EventHandler(this.btResetView_Click);
      // 
      // btDrone
      // 
      this.btDrone.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(99)))), ((int)(((byte)(87)))));
      this.btDrone.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(214)))), ((int)(((byte)(129)))));
      this.btDrone.BackgroundImage = global::FCamControl.Properties.Resources.btDrone;
      this.btDrone.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btDrone.Checked = true;
      this.btDrone.CheckedState = System.Windows.Forms.CheckState.Checked;
      this.btDrone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btDrone.ForeColor = System.Drawing.Color.Black;
      this.btDrone.ForeColorChecked = System.Drawing.Color.Yellow;
      this.btDrone.Location = new System.Drawing.Point(491, 6);
      this.btDrone.Name = "btDrone";
      this.btDrone.Size = new System.Drawing.Size(40, 40);
      this.btDrone.TabIndex = 36;
      this.btDrone.TabStop = false;
      this.btDrone.ThreeState = false;
      this.btDrone.UseVisualStyleBackColor = false;
      // 
      // btLandView
      // 
      this.btLandView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(99)))), ((int)(((byte)(87)))));
      this.btLandView.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(214)))), ((int)(((byte)(129)))));
      this.btLandView.BackgroundImage = global::FCamControl.Properties.Resources.btLanding;
      this.btLandView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btLandView.Checked = false;
      this.btLandView.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btLandView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btLandView.ForeColor = System.Drawing.Color.Black;
      this.btLandView.ForeColorChecked = System.Drawing.Color.Yellow;
      this.btLandView.Location = new System.Drawing.Point(98, 6);
      this.btLandView.Name = "btLandView";
      this.btLandView.Size = new System.Drawing.Size(40, 40);
      this.btLandView.TabIndex = 35;
      this.btLandView.TabStop = false;
      this.btLandView.ThreeState = false;
      this.btLandView.UseVisualStyleBackColor = false;
      // 
      // btExternalFree
      // 
      this.btExternalFree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(99)))), ((int)(((byte)(87)))));
      this.btExternalFree.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(214)))), ((int)(((byte)(129)))));
      this.btExternalFree.BackgroundImage = global::FCamControl.Properties.Resources.btExternal;
      this.btExternalFree.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btExternalFree.Checked = false;
      this.btExternalFree.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btExternalFree.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btExternalFree.ForeColor = System.Drawing.Color.Black;
      this.btExternalFree.ForeColorChecked = System.Drawing.Color.Yellow;
      this.btExternalFree.Location = new System.Drawing.Point(439, 6);
      this.btExternalFree.Name = "btExternalFree";
      this.btExternalFree.Size = new System.Drawing.Size(40, 40);
      this.btExternalFree.TabIndex = 35;
      this.btExternalFree.TabStop = false;
      this.btExternalFree.ThreeState = false;
      this.btExternalFree.UseVisualStyleBackColor = false;
      // 
      // btExternalIndexed
      // 
      this.btExternalIndexed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(99)))), ((int)(((byte)(87)))));
      this.btExternalIndexed.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(214)))), ((int)(((byte)(129)))));
      this.btExternalIndexed.BackgroundImage = global::FCamControl.Properties.Resources.btShowcase;
      this.btExternalIndexed.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btExternalIndexed.Checked = true;
      this.btExternalIndexed.CheckedState = System.Windows.Forms.CheckState.Checked;
      this.btExternalIndexed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btExternalIndexed.ForeColor = System.Drawing.Color.Black;
      this.btExternalIndexed.ForeColorChecked = System.Drawing.Color.Yellow;
      this.btExternalIndexed.Location = new System.Drawing.Point(393, 6);
      this.btExternalIndexed.Name = "btExternalIndexed";
      this.btExternalIndexed.Size = new System.Drawing.Size(40, 40);
      this.btExternalIndexed.TabIndex = 35;
      this.btExternalIndexed.TabStop = false;
      this.btExternalIndexed.ThreeState = false;
      this.btExternalIndexed.UseVisualStyleBackColor = false;
      // 
      // btExternalQuick
      // 
      this.btExternalQuick.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(99)))), ((int)(((byte)(87)))));
      this.btExternalQuick.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(214)))), ((int)(((byte)(129)))));
      this.btExternalQuick.BackgroundImage = global::FCamControl.Properties.Resources.btExternalQuick;
      this.btExternalQuick.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btExternalQuick.Checked = false;
      this.btExternalQuick.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btExternalQuick.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btExternalQuick.ForeColor = System.Drawing.Color.Black;
      this.btExternalQuick.ForeColorChecked = System.Drawing.Color.Yellow;
      this.btExternalQuick.Location = new System.Drawing.Point(347, 6);
      this.btExternalQuick.Name = "btExternalQuick";
      this.btExternalQuick.Size = new System.Drawing.Size(40, 40);
      this.btExternalQuick.TabIndex = 35;
      this.btExternalQuick.TabStop = false;
      this.btExternalQuick.ThreeState = false;
      this.btExternalQuick.UseVisualStyleBackColor = false;
      // 
      // btInstrumentIndexed
      // 
      this.btInstrumentIndexed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(99)))), ((int)(((byte)(87)))));
      this.btInstrumentIndexed.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(214)))), ((int)(((byte)(129)))));
      this.btInstrumentIndexed.BackgroundImage = global::FCamControl.Properties.Resources.btInstrumentView;
      this.btInstrumentIndexed.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btInstrumentIndexed.Checked = false;
      this.btInstrumentIndexed.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btInstrumentIndexed.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btInstrumentIndexed.ForeColor = System.Drawing.Color.Black;
      this.btInstrumentIndexed.ForeColorChecked = System.Drawing.Color.Yellow;
      this.btInstrumentIndexed.Location = new System.Drawing.Point(295, 6);
      this.btInstrumentIndexed.Name = "btInstrumentIndexed";
      this.btInstrumentIndexed.Size = new System.Drawing.Size(40, 40);
      this.btInstrumentIndexed.TabIndex = 35;
      this.btInstrumentIndexed.TabStop = false;
      this.btInstrumentIndexed.ThreeState = false;
      this.btInstrumentIndexed.UseVisualStyleBackColor = false;
      // 
      // btInstrumentQuick
      // 
      this.btInstrumentQuick.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(99)))), ((int)(((byte)(87)))));
      this.btInstrumentQuick.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(214)))), ((int)(((byte)(129)))));
      this.btInstrumentQuick.BackgroundImage = global::FCamControl.Properties.Resources.btCockpitQuick;
      this.btInstrumentQuick.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btInstrumentQuick.Checked = false;
      this.btInstrumentQuick.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btInstrumentQuick.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btInstrumentQuick.ForeColor = System.Drawing.Color.Black;
      this.btInstrumentQuick.ForeColorChecked = System.Drawing.Color.Yellow;
      this.btInstrumentQuick.Location = new System.Drawing.Point(249, 6);
      this.btInstrumentQuick.Name = "btInstrumentQuick";
      this.btInstrumentQuick.Size = new System.Drawing.Size(40, 40);
      this.btInstrumentQuick.TabIndex = 35;
      this.btInstrumentQuick.TabStop = false;
      this.btInstrumentQuick.ThreeState = false;
      this.btInstrumentQuick.UseVisualStyleBackColor = false;
      // 
      // btCustomCamera
      // 
      this.btCustomCamera.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(99)))), ((int)(((byte)(87)))));
      this.btCustomCamera.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(214)))), ((int)(((byte)(129)))));
      this.btCustomCamera.BackgroundImage = global::FCamControl.Properties.Resources.btCustomView;
      this.btCustomCamera.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btCustomCamera.Checked = false;
      this.btCustomCamera.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btCustomCamera.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btCustomCamera.ForeColor = System.Drawing.Color.Black;
      this.btCustomCamera.ForeColorChecked = System.Drawing.Color.Yellow;
      this.btCustomCamera.Location = new System.Drawing.Point(197, 6);
      this.btCustomCamera.Name = "btCustomCamera";
      this.btCustomCamera.Size = new System.Drawing.Size(40, 40);
      this.btCustomCamera.TabIndex = 35;
      this.btCustomCamera.TabStop = false;
      this.btCustomCamera.ThreeState = false;
      this.btCustomCamera.UseVisualStyleBackColor = false;
      // 
      // btCloseView
      // 
      this.btCloseView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(99)))), ((int)(((byte)(87)))));
      this.btCloseView.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(214)))), ((int)(((byte)(129)))));
      this.btCloseView.BackgroundImage = global::FCamControl.Properties.Resources.btClose;
      this.btCloseView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btCloseView.Checked = false;
      this.btCloseView.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btCloseView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btCloseView.ForeColor = System.Drawing.Color.Black;
      this.btCloseView.ForeColorChecked = System.Drawing.Color.Yellow;
      this.btCloseView.Location = new System.Drawing.Point(51, 6);
      this.btCloseView.Name = "btCloseView";
      this.btCloseView.Size = new System.Drawing.Size(40, 40);
      this.btCloseView.TabIndex = 35;
      this.btCloseView.TabStop = false;
      this.btCloseView.ThreeState = false;
      this.btCloseView.UseVisualStyleBackColor = false;
      // 
      // btCoPilotView
      // 
      this.btCoPilotView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(99)))), ((int)(((byte)(87)))));
      this.btCoPilotView.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(214)))), ((int)(((byte)(129)))));
      this.btCoPilotView.BackgroundImage = global::FCamControl.Properties.Resources.btFO;
      this.btCoPilotView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btCoPilotView.Checked = false;
      this.btCoPilotView.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btCoPilotView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btCoPilotView.ForeColor = System.Drawing.Color.Black;
      this.btCoPilotView.ForeColorChecked = System.Drawing.Color.Yellow;
      this.btCoPilotView.Location = new System.Drawing.Point(145, 6);
      this.btCoPilotView.Name = "btCoPilotView";
      this.btCoPilotView.Size = new System.Drawing.Size(40, 40);
      this.btCoPilotView.TabIndex = 35;
      this.btCoPilotView.TabStop = false;
      this.btCoPilotView.ThreeState = false;
      this.btCoPilotView.UseVisualStyleBackColor = false;
      // 
      // btPilotView
      // 
      this.btPilotView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(99)))), ((int)(((byte)(87)))));
      this.btPilotView.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(214)))), ((int)(((byte)(129)))));
      this.btPilotView.BackgroundImage = global::FCamControl.Properties.Resources.btPilot;
      this.btPilotView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btPilotView.Checked = false;
      this.btPilotView.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btPilotView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btPilotView.ForeColor = System.Drawing.Color.Black;
      this.btPilotView.ForeColorChecked = System.Drawing.Color.Yellow;
      this.btPilotView.Location = new System.Drawing.Point(4, 6);
      this.btPilotView.Name = "btPilotView";
      this.btPilotView.Size = new System.Drawing.Size(40, 40);
      this.btPilotView.TabIndex = 35;
      this.btPilotView.TabStop = false;
      this.btPilotView.ThreeState = false;
      this.btPilotView.UseVisualStyleBackColor = false;
      // 
      // ctxMenu
      // 
      this.ctxMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuKeyConfig});
      this.ctxMenu.Name = "ctxMenu";
      this.ctxMenu.Size = new System.Drawing.Size(155, 26);
      // 
      // mnuKeyConfig
      // 
      this.mnuKeyConfig.Name = "mnuKeyConfig";
      this.mnuKeyConfig.Size = new System.Drawing.Size(154, 22);
      this.mnuKeyConfig.Text = "Configure Keys";
      this.mnuKeyConfig.Click += new System.EventHandler(this.mnuKeyConfig_Click);
      // 
      // timer1
      // 
      this.timer1.Interval = 1000;
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // frmCameraV2
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(37)))), ((int)(((byte)(36)))));
      this.BackgroundImage = global::FCamControl.Properties.Resources.camBackgroundV2_darker;
      this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
      this.CausesValidation = false;
      this.ClientSize = new System.Drawing.Size(687, 477);
      this.ContextMenuStrip = this.ctxMenu;
      this.Controls.Add(this.pnlCamButtons);
      this.Controls.Add(this.lblSimConnected);
      this.DoubleBuffered = true;
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "frmCameraV2";
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "HudBar - Camera";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmCameraV2_FormClosing);
      this.Load += new System.EventHandler(this.frmCameraV2_Load);
      this.pnlCamButtons.ResumeLayout(false);
      this.pnlCamButtons.PerformLayout();
      this.ctxMenu.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label lblSimConnected;
    private System.Windows.Forms.Panel pnlCamButtons;
    private System.Windows.Forms.Button btResetView;
    private CheckedButton btExternalFree;
    private CheckedButton btDrone;
    private CheckedButton btDOF6;
    private CheckedButton btExternalIndexed;
    private CheckedButton btExternalQuick;
    private CheckedButton btInstrumentIndexed;
    private CheckedButton btInstrumentQuick;
    private CheckedButton btCustomCamera;
    private CheckedButton btCoPilotView;
    private CheckedButton btLandView;
    private CheckedButton btCloseView;
    private CheckedButton btPilotView;
    private System.Windows.Forms.ContextMenuStrip ctxMenu;
    private System.Windows.Forms.ToolStripMenuItem mnuKeyConfig;
    private System.Windows.Forms.Label lblInop;
    private System.Windows.Forms.Timer timer1;
  }
}
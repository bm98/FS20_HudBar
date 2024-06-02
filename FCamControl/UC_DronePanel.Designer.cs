namespace FCamControl
{
  partial class UC_DronePanel
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
      this.btDrone_TogControls = new FCamControl.CheckedButton();
      this.btDrone_FromRight = new FCamControl.CheckedButton();
      this.btDrone_FromLeft = new FCamControl.CheckedButton();
      this.btDrone_FromAbove = new FCamControl.CheckedButton();
      this.btDrone_FromBelow = new FCamControl.CheckedButton();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.lblFlyByCountdown = new System.Windows.Forms.Label();
      this.lblFlyByFailed = new System.Windows.Forms.Label();
      this.btDrone_FlyByReset = new System.Windows.Forms.Button();
      this.tbFlyByDist = new System.Windows.Forms.TrackBar();
      this.pnlDroneSlider = new System.Windows.Forms.Panel();
      this.btDrone_CamFollow = new FCamControl.CheckedButton();
      this.btDrone_CamLock = new FCamControl.CheckedButton();
      this.label6 = new System.Windows.Forms.Label();
      this.lblDroneRot = new System.Windows.Forms.Label();
      this.lblDroneMove = new System.Windows.Forms.Label();
      this.tbZoom = new System.Windows.Forms.TrackBar();
      this.tbMove = new System.Windows.Forms.TrackBar();
      this.tbRotate = new System.Windows.Forms.TrackBar();
      this.rbStage0 = new System.Windows.Forms.RadioButton();
      this.rbStage1 = new System.Windows.Forms.RadioButton();
      this.rbStage2 = new System.Windows.Forms.RadioButton();
      this.rbStage3 = new System.Windows.Forms.RadioButton();
      this.btDrone_FlyByPrep = new FCamControl.CheckedButton();
      this.btDrone_FlyByFire = new FCamControl.CheckedButton();
      this.btDrone_FlyByInit = new FCamControl.CheckedButton();
      ((System.ComponentModel.ISupportInitialize)(this.tbFlyByDist)).BeginInit();
      this.pnlDroneSlider.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.tbZoom)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.tbMove)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.tbRotate)).BeginInit();
      this.SuspendLayout();
      // 
      // btDrone_TogControls
      // 
      this.btDrone_TogControls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(105)))), ((int)(((byte)(104)))));
      this.btDrone_TogControls.BackColorChecked = System.Drawing.Color.Orange;
      this.btDrone_TogControls.BackgroundImage = global::FCamControl.Properties.Resources.YokeBt;
      this.btDrone_TogControls.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btDrone_TogControls.Checked = true;
      this.btDrone_TogControls.CheckedState = System.Windows.Forms.CheckState.Checked;
      this.btDrone_TogControls.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btDrone_TogControls.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btDrone_TogControls.ForeColor = System.Drawing.Color.DarkGray;
      this.btDrone_TogControls.ForeColorChecked = System.Drawing.Color.Linen;
      this.btDrone_TogControls.Location = new System.Drawing.Point(580, 158);
      this.btDrone_TogControls.Name = "btDrone_TogControls";
      this.btDrone_TogControls.Size = new System.Drawing.Size(56, 56);
      this.btDrone_TogControls.TabIndex = 57;
      this.btDrone_TogControls.TabStop = false;
      this.btDrone_TogControls.ThreeState = false;
      this.btDrone_TogControls.UseVisualStyleBackColor = false;
      // 
      // btDrone_FromRight
      // 
      this.btDrone_FromRight.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(105)))), ((int)(((byte)(104)))));
      this.btDrone_FromRight.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(148)))), ((int)(((byte)(108)))));
      this.btDrone_FromRight.BackgroundImage = global::FCamControl.Properties.Resources.btFromRight;
      this.btDrone_FromRight.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btDrone_FromRight.Checked = false;
      this.btDrone_FromRight.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btDrone_FromRight.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btDrone_FromRight.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btDrone_FromRight.ForeColor = System.Drawing.Color.DarkGray;
      this.btDrone_FromRight.ForeColorChecked = System.Drawing.Color.Linen;
      this.btDrone_FromRight.Location = new System.Drawing.Point(284, 89);
      this.btDrone_FromRight.Name = "btDrone_FromRight";
      this.btDrone_FromRight.Size = new System.Drawing.Size(36, 36);
      this.btDrone_FromRight.TabIndex = 60;
      this.btDrone_FromRight.TabStop = false;
      this.btDrone_FromRight.ThreeState = false;
      this.btDrone_FromRight.UseVisualStyleBackColor = false;
      this.btDrone_FromRight.CheckedChanged += new System.EventHandler<System.EventArgs>(this.btDrone_FromRight_CheckedChanged);
      // 
      // btDrone_FromLeft
      // 
      this.btDrone_FromLeft.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(105)))), ((int)(((byte)(104)))));
      this.btDrone_FromLeft.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(148)))), ((int)(((byte)(108)))));
      this.btDrone_FromLeft.BackgroundImage = global::FCamControl.Properties.Resources.btFromLeft;
      this.btDrone_FromLeft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btDrone_FromLeft.Checked = false;
      this.btDrone_FromLeft.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btDrone_FromLeft.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btDrone_FromLeft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btDrone_FromLeft.ForeColor = System.Drawing.Color.DarkGray;
      this.btDrone_FromLeft.ForeColorChecked = System.Drawing.Color.Linen;
      this.btDrone_FromLeft.Location = new System.Drawing.Point(239, 89);
      this.btDrone_FromLeft.Name = "btDrone_FromLeft";
      this.btDrone_FromLeft.Size = new System.Drawing.Size(36, 36);
      this.btDrone_FromLeft.TabIndex = 59;
      this.btDrone_FromLeft.TabStop = false;
      this.btDrone_FromLeft.ThreeState = false;
      this.btDrone_FromLeft.UseVisualStyleBackColor = false;
      this.btDrone_FromLeft.CheckedChanged += new System.EventHandler<System.EventArgs>(this.btDrone_FromLeft_CheckedChanged);
      // 
      // btDrone_FromAbove
      // 
      this.btDrone_FromAbove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(105)))), ((int)(((byte)(104)))));
      this.btDrone_FromAbove.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(148)))), ((int)(((byte)(108)))));
      this.btDrone_FromAbove.BackgroundImage = global::FCamControl.Properties.Resources.btFromAbove;
      this.btDrone_FromAbove.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btDrone_FromAbove.Checked = false;
      this.btDrone_FromAbove.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btDrone_FromAbove.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btDrone_FromAbove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btDrone_FromAbove.ForeColor = System.Drawing.Color.DarkGray;
      this.btDrone_FromAbove.ForeColorChecked = System.Drawing.Color.Linen;
      this.btDrone_FromAbove.Location = new System.Drawing.Point(160, 89);
      this.btDrone_FromAbove.Name = "btDrone_FromAbove";
      this.btDrone_FromAbove.Size = new System.Drawing.Size(36, 36);
      this.btDrone_FromAbove.TabIndex = 58;
      this.btDrone_FromAbove.TabStop = false;
      this.btDrone_FromAbove.ThreeState = false;
      this.btDrone_FromAbove.UseVisualStyleBackColor = false;
      this.btDrone_FromAbove.CheckedChanged += new System.EventHandler<System.EventArgs>(this.btDrone_FromAbove_CheckedChanged);
      // 
      // btDrone_FromBelow
      // 
      this.btDrone_FromBelow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(105)))), ((int)(((byte)(104)))));
      this.btDrone_FromBelow.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(148)))), ((int)(((byte)(108)))));
      this.btDrone_FromBelow.BackgroundImage = global::FCamControl.Properties.Resources.btFromBelow;
      this.btDrone_FromBelow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btDrone_FromBelow.Checked = true;
      this.btDrone_FromBelow.CheckedState = System.Windows.Forms.CheckState.Checked;
      this.btDrone_FromBelow.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btDrone_FromBelow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btDrone_FromBelow.ForeColor = System.Drawing.Color.DarkGray;
      this.btDrone_FromBelow.ForeColorChecked = System.Drawing.Color.Linen;
      this.btDrone_FromBelow.Location = new System.Drawing.Point(115, 89);
      this.btDrone_FromBelow.Name = "btDrone_FromBelow";
      this.btDrone_FromBelow.Size = new System.Drawing.Size(36, 36);
      this.btDrone_FromBelow.TabIndex = 56;
      this.btDrone_FromBelow.TabStop = false;
      this.btDrone_FromBelow.ThreeState = false;
      this.btDrone_FromBelow.UseVisualStyleBackColor = false;
      this.btDrone_FromBelow.CheckedChanged += new System.EventHandler<System.EventArgs>(this.btDrone_FromBelow_CheckedChanged);
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.CausesValidation = false;
      this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.ForeColor = System.Drawing.Color.DarkGray;
      this.label4.Location = new System.Drawing.Point(95, 177);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(57, 17);
      this.label4.TabIndex = 55;
      this.label4.Text = "Distance";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.BackColor = System.Drawing.Color.Transparent;
      this.label5.ForeColor = System.Drawing.Color.WhiteSmoke;
      this.label5.Location = new System.Drawing.Point(198, 177);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(41, 13);
      this.label5.TabIndex = 52;
      this.label5.Text = "0.5 nm";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.BackColor = System.Drawing.Color.Transparent;
      this.label1.ForeColor = System.Drawing.Color.WhiteSmoke;
      this.label1.Location = new System.Drawing.Point(331, 177);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(32, 13);
      this.label1.TabIndex = 53;
      this.label1.Text = "1 nm";
      // 
      // lblFlyByCountdown
      // 
      this.lblFlyByCountdown.AutoSize = true;
      this.lblFlyByCountdown.BackColor = System.Drawing.Color.Transparent;
      this.lblFlyByCountdown.Cursor = System.Windows.Forms.Cursors.Hand;
      this.lblFlyByCountdown.ForeColor = System.Drawing.Color.LightCyan;
      this.lblFlyByCountdown.Location = new System.Drawing.Point(455, 165);
      this.lblFlyByCountdown.Name = "lblFlyByCountdown";
      this.lblFlyByCountdown.Size = new System.Drawing.Size(38, 13);
      this.lblFlyByCountdown.TabIndex = 51;
      this.lblFlyByCountdown.Text = "Failed";
      this.lblFlyByCountdown.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.lblFlyByCountdown.Visible = false;
      // 
      // lblFlyByFailed
      // 
      this.lblFlyByFailed.AutoSize = true;
      this.lblFlyByFailed.BackColor = System.Drawing.Color.Transparent;
      this.lblFlyByFailed.ForeColor = System.Drawing.Color.Tomato;
      this.lblFlyByFailed.Location = new System.Drawing.Point(368, 140);
      this.lblFlyByFailed.Name = "lblFlyByFailed";
      this.lblFlyByFailed.Size = new System.Drawing.Size(38, 13);
      this.lblFlyByFailed.TabIndex = 50;
      this.lblFlyByFailed.Text = "Failed";
      this.lblFlyByFailed.Visible = false;
      // 
      // btDrone_FlyByReset
      // 
      this.btDrone_FlyByReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(105)))), ((int)(((byte)(104)))));
      this.btDrone_FlyByReset.BackgroundImage = global::FCamControl.Properties.Resources.ResetBt;
      this.btDrone_FlyByReset.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btDrone_FlyByReset.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btDrone_FlyByReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btDrone_FlyByReset.ForeColor = System.Drawing.Color.DarkGray;
      this.btDrone_FlyByReset.Location = new System.Drawing.Point(579, 79);
      this.btDrone_FlyByReset.Name = "btDrone_FlyByReset";
      this.btDrone_FlyByReset.Size = new System.Drawing.Size(57, 54);
      this.btDrone_FlyByReset.TabIndex = 47;
      this.btDrone_FlyByReset.TabStop = false;
      this.btDrone_FlyByReset.UseVisualStyleBackColor = false;
      this.btDrone_FlyByReset.Click += new System.EventHandler(this.btDrone_FlyByReset_Click);
      // 
      // tbFlyByDist
      // 
      this.tbFlyByDist.AutoSize = false;
      this.tbFlyByDist.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(13)))), ((int)(((byte)(13)))));
      this.tbFlyByDist.CausesValidation = false;
      this.tbFlyByDist.Cursor = System.Windows.Forms.Cursors.Hand;
      this.tbFlyByDist.LargeChange = 10;
      this.tbFlyByDist.Location = new System.Drawing.Point(84, 137);
      this.tbFlyByDist.Maximum = 100;
      this.tbFlyByDist.Name = "tbFlyByDist";
      this.tbFlyByDist.Size = new System.Drawing.Size(267, 37);
      this.tbFlyByDist.SmallChange = 2;
      this.tbFlyByDist.TabIndex = 49;
      this.tbFlyByDist.TabStop = false;
      this.tbFlyByDist.TickFrequency = 10;
      this.tbFlyByDist.Value = 20;
      // 
      // pnlDroneSlider
      // 
      this.pnlDroneSlider.BackColor = System.Drawing.Color.Transparent;
      this.pnlDroneSlider.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.pnlDroneSlider.Controls.Add(this.btDrone_CamFollow);
      this.pnlDroneSlider.Controls.Add(this.btDrone_CamLock);
      this.pnlDroneSlider.Controls.Add(this.label6);
      this.pnlDroneSlider.Controls.Add(this.lblDroneRot);
      this.pnlDroneSlider.Controls.Add(this.lblDroneMove);
      this.pnlDroneSlider.Controls.Add(this.tbZoom);
      this.pnlDroneSlider.Controls.Add(this.tbMove);
      this.pnlDroneSlider.Controls.Add(this.tbRotate);
      this.pnlDroneSlider.ForeColor = System.Drawing.Color.DarkGray;
      this.pnlDroneSlider.Location = new System.Drawing.Point(3, 5);
      this.pnlDroneSlider.Name = "pnlDroneSlider";
      this.pnlDroneSlider.Size = new System.Drawing.Size(638, 68);
      this.pnlDroneSlider.TabIndex = 48;
      // 
      // btDrone_CamFollow
      // 
      this.btDrone_CamFollow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(105)))), ((int)(((byte)(104)))));
      this.btDrone_CamFollow.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(148)))), ((int)(((byte)(108)))));
      this.btDrone_CamFollow.BackgroundImage = global::FCamControl.Properties.Resources.btFollow;
      this.btDrone_CamFollow.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btDrone_CamFollow.Checked = true;
      this.btDrone_CamFollow.CheckedState = System.Windows.Forms.CheckState.Checked;
      this.btDrone_CamFollow.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btDrone_CamFollow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btDrone_CamFollow.ForeColor = System.Drawing.Color.DarkGray;
      this.btDrone_CamFollow.ForeColorChecked = System.Drawing.Color.Linen;
      this.btDrone_CamFollow.Location = new System.Drawing.Point(575, 15);
      this.btDrone_CamFollow.Name = "btDrone_CamFollow";
      this.btDrone_CamFollow.Size = new System.Drawing.Size(36, 36);
      this.btDrone_CamFollow.TabIndex = 37;
      this.btDrone_CamFollow.TabStop = false;
      this.btDrone_CamFollow.ThreeState = false;
      this.btDrone_CamFollow.UseVisualStyleBackColor = false;
      // 
      // btDrone_CamLock
      // 
      this.btDrone_CamLock.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(105)))), ((int)(((byte)(104)))));
      this.btDrone_CamLock.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(148)))), ((int)(((byte)(108)))));
      this.btDrone_CamLock.BackgroundImage = global::FCamControl.Properties.Resources.btLock;
      this.btDrone_CamLock.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btDrone_CamLock.Checked = false;
      this.btDrone_CamLock.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btDrone_CamLock.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btDrone_CamLock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btDrone_CamLock.ForeColor = System.Drawing.Color.DarkGray;
      this.btDrone_CamLock.ForeColorChecked = System.Drawing.Color.Linen;
      this.btDrone_CamLock.Location = new System.Drawing.Point(516, 15);
      this.btDrone_CamLock.Name = "btDrone_CamLock";
      this.btDrone_CamLock.Size = new System.Drawing.Size(36, 36);
      this.btDrone_CamLock.TabIndex = 36;
      this.btDrone_CamLock.TabStop = false;
      this.btDrone_CamLock.ThreeState = false;
      this.btDrone_CamLock.UseVisualStyleBackColor = false;
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.CausesValidation = false;
      this.label6.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.ForeColor = System.Drawing.Color.DarkGray;
      this.label6.Location = new System.Drawing.Point(340, 47);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(42, 17);
      this.label6.TabIndex = 26;
      this.label6.Text = "Zoom";
      // 
      // lblDroneRot
      // 
      this.lblDroneRot.AutoSize = true;
      this.lblDroneRot.CausesValidation = false;
      this.lblDroneRot.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblDroneRot.ForeColor = System.Drawing.Color.DarkGray;
      this.lblDroneRot.Location = new System.Drawing.Point(176, 47);
      this.lblDroneRot.Name = "lblDroneRot";
      this.lblDroneRot.Size = new System.Drawing.Size(98, 17);
      this.lblDroneRot.TabIndex = 22;
      this.lblDroneRot.Text = "Rotation Speed";
      // 
      // lblDroneMove
      // 
      this.lblDroneMove.AutoSize = true;
      this.lblDroneMove.CausesValidation = false;
      this.lblDroneMove.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblDroneMove.ForeColor = System.Drawing.Color.DarkGray;
      this.lblDroneMove.Location = new System.Drawing.Point(16, 47);
      this.lblDroneMove.Name = "lblDroneMove";
      this.lblDroneMove.Size = new System.Drawing.Size(82, 17);
      this.lblDroneMove.TabIndex = 22;
      this.lblDroneMove.Text = "Move Speed";
      // 
      // tbZoom
      // 
      this.tbZoom.AutoSize = false;
      this.tbZoom.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(13)))), ((int)(((byte)(13)))));
      this.tbZoom.CausesValidation = false;
      this.tbZoom.Cursor = System.Windows.Forms.Cursors.Hand;
      this.tbZoom.LargeChange = 10;
      this.tbZoom.Location = new System.Drawing.Point(328, 7);
      this.tbZoom.Maximum = 100;
      this.tbZoom.Name = "tbZoom";
      this.tbZoom.Size = new System.Drawing.Size(155, 37);
      this.tbZoom.SmallChange = 2;
      this.tbZoom.TabIndex = 25;
      this.tbZoom.TabStop = false;
      this.tbZoom.TickFrequency = 10;
      this.tbZoom.ValueChanged += new System.EventHandler(this.tbZoom_ValueChanged);
      this.tbZoom.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Slider_MouseDown);
      this.tbZoom.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Slider_MouseUp);
      // 
      // tbMove
      // 
      this.tbMove.AutoSize = false;
      this.tbMove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(13)))), ((int)(((byte)(13)))));
      this.tbMove.CausesValidation = false;
      this.tbMove.Cursor = System.Windows.Forms.Cursors.Hand;
      this.tbMove.LargeChange = 10;
      this.tbMove.Location = new System.Drawing.Point(6, 7);
      this.tbMove.Maximum = 100;
      this.tbMove.Name = "tbMove";
      this.tbMove.Size = new System.Drawing.Size(155, 37);
      this.tbMove.SmallChange = 2;
      this.tbMove.TabIndex = 17;
      this.tbMove.TabStop = false;
      this.tbMove.TickFrequency = 10;
      this.tbMove.ValueChanged += new System.EventHandler(this.tbMove_ValueChanged);
      this.tbMove.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Slider_MouseDown);
      this.tbMove.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Slider_MouseUp);
      // 
      // tbRotate
      // 
      this.tbRotate.AutoSize = false;
      this.tbRotate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(13)))), ((int)(((byte)(13)))));
      this.tbRotate.CausesValidation = false;
      this.tbRotate.Cursor = System.Windows.Forms.Cursors.Hand;
      this.tbRotate.LargeChange = 10;
      this.tbRotate.Location = new System.Drawing.Point(167, 7);
      this.tbRotate.Maximum = 100;
      this.tbRotate.Name = "tbRotate";
      this.tbRotate.Size = new System.Drawing.Size(155, 37);
      this.tbRotate.SmallChange = 2;
      this.tbRotate.TabIndex = 17;
      this.tbRotate.TabStop = false;
      this.tbRotate.TickFrequency = 10;
      this.tbRotate.ValueChanged += new System.EventHandler(this.tbRotate_ValueChanged);
      this.tbRotate.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Slider_MouseDown);
      this.tbRotate.MouseUp += new System.Windows.Forms.MouseEventHandler(this.Slider_MouseUp);
      // 
      // rbStage0
      // 
      this.rbStage0.AutoSize = true;
      this.rbStage0.BackColor = System.Drawing.Color.Transparent;
      this.rbStage0.Cursor = System.Windows.Forms.Cursors.Hand;
      this.rbStage0.ForeColor = System.Drawing.Color.Gainsboro;
      this.rbStage0.Location = new System.Drawing.Point(234, 206);
      this.rbStage0.Name = "rbStage0";
      this.rbStage0.Size = new System.Drawing.Size(59, 17);
      this.rbStage0.TabIndex = 43;
      this.rbStage0.Text = "0.5 nm";
      this.rbStage0.UseVisualStyleBackColor = false;
      this.rbStage0.CheckedChanged += new System.EventHandler(this.rbStage0_CheckedChanged);
      // 
      // rbStage1
      // 
      this.rbStage1.AutoSize = true;
      this.rbStage1.BackColor = System.Drawing.Color.Transparent;
      this.rbStage1.Cursor = System.Windows.Forms.Cursors.Hand;
      this.rbStage1.ForeColor = System.Drawing.Color.Gainsboro;
      this.rbStage1.Location = new System.Drawing.Point(70, 206);
      this.rbStage1.Name = "rbStage1";
      this.rbStage1.Size = new System.Drawing.Size(73, 17);
      this.rbStage1.TabIndex = 46;
      this.rbStage1.Text = "GS 100 kt";
      this.rbStage1.UseVisualStyleBackColor = false;
      this.rbStage1.CheckedChanged += new System.EventHandler(this.rbStage1_CheckedChanged);
      // 
      // rbStage2
      // 
      this.rbStage2.AutoSize = true;
      this.rbStage2.BackColor = System.Drawing.Color.Transparent;
      this.rbStage2.Cursor = System.Windows.Forms.Cursors.Hand;
      this.rbStage2.ForeColor = System.Drawing.Color.Gainsboro;
      this.rbStage2.Location = new System.Drawing.Point(152, 206);
      this.rbStage2.Name = "rbStage2";
      this.rbStage2.Size = new System.Drawing.Size(73, 17);
      this.rbStage2.TabIndex = 44;
      this.rbStage2.Text = "GS 250 kt";
      this.rbStage2.UseVisualStyleBackColor = false;
      this.rbStage2.CheckedChanged += new System.EventHandler(this.rbStage2_CheckedChanged);
      // 
      // rbStage3
      // 
      this.rbStage3.AutoSize = true;
      this.rbStage3.BackColor = System.Drawing.Color.Transparent;
      this.rbStage3.Cursor = System.Windows.Forms.Cursors.Hand;
      this.rbStage3.ForeColor = System.Drawing.Color.Gainsboro;
      this.rbStage3.Location = new System.Drawing.Point(303, 206);
      this.rbStage3.Name = "rbStage3";
      this.rbStage3.Size = new System.Drawing.Size(73, 17);
      this.rbStage3.TabIndex = 45;
      this.rbStage3.Text = "GS 400 kt";
      this.rbStage3.UseVisualStyleBackColor = false;
      this.rbStage3.CheckedChanged += new System.EventHandler(this.rbStage3_CheckedChanged);
      // 
      // btDrone_FlyByPrep
      // 
      this.btDrone_FlyByPrep.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(105)))), ((int)(((byte)(104)))));
      this.btDrone_FlyByPrep.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(99)))), ((int)(((byte)(87)))));
      this.btDrone_FlyByPrep.BackgroundImage = global::FCamControl.Properties.Resources.FlagBt;
      this.btDrone_FlyByPrep.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btDrone_FlyByPrep.Checked = false;
      this.btDrone_FlyByPrep.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btDrone_FlyByPrep.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btDrone_FlyByPrep.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btDrone_FlyByPrep.ForeColor = System.Drawing.Color.DarkGray;
      this.btDrone_FlyByPrep.ForeColorChecked = System.Drawing.Color.Linen;
      this.btDrone_FlyByPrep.Location = new System.Drawing.Point(347, 78);
      this.btDrone_FlyByPrep.Name = "btDrone_FlyByPrep";
      this.btDrone_FlyByPrep.Size = new System.Drawing.Size(68, 57);
      this.btDrone_FlyByPrep.TabIndex = 61;
      this.btDrone_FlyByPrep.TabStop = false;
      this.btDrone_FlyByPrep.ThreeState = false;
      this.btDrone_FlyByPrep.UseVisualStyleBackColor = false;
      // 
      // btDrone_FlyByFire
      // 
      this.btDrone_FlyByFire.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(105)))), ((int)(((byte)(104)))));
      this.btDrone_FlyByFire.BackColorChecked = System.Drawing.Color.MediumSpringGreen;
      this.btDrone_FlyByFire.BackgroundImage = global::FCamControl.Properties.Resources.CameraBt;
      this.btDrone_FlyByFire.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btDrone_FlyByFire.Checked = true;
      this.btDrone_FlyByFire.CheckedState = System.Windows.Forms.CheckState.Checked;
      this.btDrone_FlyByFire.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btDrone_FlyByFire.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btDrone_FlyByFire.ForeColor = System.Drawing.Color.DarkGray;
      this.btDrone_FlyByFire.ForeColorChecked = System.Drawing.Color.Linen;
      this.btDrone_FlyByFire.Location = new System.Drawing.Point(458, 158);
      this.btDrone_FlyByFire.Name = "btDrone_FlyByFire";
      this.btDrone_FlyByFire.Size = new System.Drawing.Size(80, 57);
      this.btDrone_FlyByFire.TabIndex = 62;
      this.btDrone_FlyByFire.TabStop = false;
      this.btDrone_FlyByFire.ThreeState = false;
      this.btDrone_FlyByFire.UseVisualStyleBackColor = false;
      // 
      // btDrone_FlyByInit
      // 
      this.btDrone_FlyByInit.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(105)))), ((int)(((byte)(104)))));
      this.btDrone_FlyByInit.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(99)))), ((int)(((byte)(87)))));
      this.btDrone_FlyByInit.BackgroundImage = global::FCamControl.Properties.Resources.PinBt;
      this.btDrone_FlyByInit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btDrone_FlyByInit.Checked = false;
      this.btDrone_FlyByInit.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btDrone_FlyByInit.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btDrone_FlyByInit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btDrone_FlyByInit.ForeColor = System.Drawing.Color.DarkGray;
      this.btDrone_FlyByInit.ForeColorChecked = System.Drawing.Color.Linen;
      this.btDrone_FlyByInit.Location = new System.Drawing.Point(3, 78);
      this.btDrone_FlyByInit.Name = "btDrone_FlyByInit";
      this.btDrone_FlyByInit.Size = new System.Drawing.Size(68, 57);
      this.btDrone_FlyByInit.TabIndex = 61;
      this.btDrone_FlyByInit.TabStop = false;
      this.btDrone_FlyByInit.ThreeState = false;
      this.btDrone_FlyByInit.UseVisualStyleBackColor = false;
      // 
      // UC_DronePanel
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.BackColor = System.Drawing.Color.Transparent;
      this.BackgroundImage = global::FCamControl.Properties.Resources.camDroneBackgroundV2;
      this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
      this.Controls.Add(this.btDrone_FlyByFire);
      this.Controls.Add(this.btDrone_FlyByInit);
      this.Controls.Add(this.btDrone_FlyByPrep);
      this.Controls.Add(this.btDrone_TogControls);
      this.Controls.Add(this.btDrone_FromRight);
      this.Controls.Add(this.btDrone_FromLeft);
      this.Controls.Add(this.btDrone_FromAbove);
      this.Controls.Add(this.btDrone_FromBelow);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.lblFlyByCountdown);
      this.Controls.Add(this.lblFlyByFailed);
      this.Controls.Add(this.btDrone_FlyByReset);
      this.Controls.Add(this.tbFlyByDist);
      this.Controls.Add(this.pnlDroneSlider);
      this.Controls.Add(this.rbStage0);
      this.Controls.Add(this.rbStage1);
      this.Controls.Add(this.rbStage2);
      this.Controls.Add(this.rbStage3);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "UC_DronePanel";
      this.Size = new System.Drawing.Size(644, 228);
      this.Load += new System.EventHandler(this.UC_DronePanel_Load);
      ((System.ComponentModel.ISupportInitialize)(this.tbFlyByDist)).EndInit();
      this.pnlDroneSlider.ResumeLayout(false);
      this.pnlDroneSlider.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.tbZoom)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.tbMove)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.tbRotate)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private CheckedButton btDrone_TogControls;
    private CheckedButton btDrone_FromRight;
    private CheckedButton btDrone_FromLeft;
    private CheckedButton btDrone_FromAbove;
    private CheckedButton btDrone_FromBelow;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label lblFlyByCountdown;
    private System.Windows.Forms.Label lblFlyByFailed;
    private System.Windows.Forms.Button btDrone_FlyByReset;
    private System.Windows.Forms.TrackBar tbFlyByDist;
    private System.Windows.Forms.Panel pnlDroneSlider;
    private CheckedButton btDrone_CamFollow;
    private CheckedButton btDrone_CamLock;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label lblDroneRot;
    private System.Windows.Forms.Label lblDroneMove;
    private System.Windows.Forms.TrackBar tbZoom;
    private System.Windows.Forms.TrackBar tbMove;
    private System.Windows.Forms.TrackBar tbRotate;
    private System.Windows.Forms.RadioButton rbStage0;
    private System.Windows.Forms.RadioButton rbStage1;
    private System.Windows.Forms.RadioButton rbStage2;
    private System.Windows.Forms.RadioButton rbStage3;
    private CheckedButton btDrone_FlyByPrep;
    private CheckedButton btDrone_FlyByFire;
    private CheckedButton btDrone_FlyByInit;
  }
}

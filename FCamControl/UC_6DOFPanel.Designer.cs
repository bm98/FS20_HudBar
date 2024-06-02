namespace FCamControl
{
  partial class UC_6DOFPanel
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
      this.numX = new System.Windows.Forms.NumericUpDown();
      this.numZ = new System.Windows.Forms.NumericUpDown();
      this.numY = new System.Windows.Forms.NumericUpDown();
      this.numP = new System.Windows.Forms.NumericUpDown();
      this.numH = new System.Windows.Forms.NumericUpDown();
      this.numB = new System.Windows.Forms.NumericUpDown();
      this.btViewOut = new System.Windows.Forms.Button();
      this.btResetView = new System.Windows.Forms.Button();
      this.label6 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.btCamLock = new FCamControl.CheckedButton();
      ((System.ComponentModel.ISupportInitialize)(this.numX)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numZ)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numY)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numP)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numH)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numB)).BeginInit();
      this.SuspendLayout();
      // 
      // numX
      // 
      this.numX.DecimalPlaces = 1;
      this.numX.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.numX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numX.Location = new System.Drawing.Point(107, 16);
      this.numX.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
      this.numX.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            -2147483648});
      this.numX.Name = "numX";
      this.numX.Size = new System.Drawing.Size(65, 27);
      this.numX.TabIndex = 0;
      this.numX.ValueChanged += new System.EventHandler(this.numX_ValueChanged);
      this.numX.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_KeyDown);
      this.numX.Leave += new System.EventHandler(this.num_Leave);
      // 
      // numZ
      // 
      this.numZ.DecimalPlaces = 1;
      this.numZ.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.numZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numZ.Location = new System.Drawing.Point(213, 101);
      this.numZ.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
      this.numZ.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            -2147483648});
      this.numZ.Name = "numZ";
      this.numZ.Size = new System.Drawing.Size(65, 27);
      this.numZ.TabIndex = 1;
      this.numZ.ValueChanged += new System.EventHandler(this.numZ_ValueChanged);
      this.numZ.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_KeyDown);
      this.numZ.Leave += new System.EventHandler(this.num_Leave);
      // 
      // numY
      // 
      this.numY.DecimalPlaces = 1;
      this.numY.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.numY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numY.Location = new System.Drawing.Point(13, 160);
      this.numY.Maximum = new decimal(new int[] {
            500,
            0,
            0,
            0});
      this.numY.Minimum = new decimal(new int[] {
            500,
            0,
            0,
            -2147483648});
      this.numY.Name = "numY";
      this.numY.Size = new System.Drawing.Size(65, 27);
      this.numY.TabIndex = 2;
      this.numY.ValueChanged += new System.EventHandler(this.numY_ValueChanged);
      this.numY.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_KeyDown);
      this.numY.Leave += new System.EventHandler(this.num_Leave);
      // 
      // numP
      // 
      this.numP.DecimalPlaces = 1;
      this.numP.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.numP.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numP.Location = new System.Drawing.Point(350, 16);
      this.numP.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
      this.numP.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
      this.numP.Name = "numP";
      this.numP.Size = new System.Drawing.Size(65, 27);
      this.numP.TabIndex = 3;
      this.numP.ValueChanged += new System.EventHandler(this.numP_ValueChanged);
      this.numP.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_KeyDown);
      this.numP.Leave += new System.EventHandler(this.num_Leave);
      // 
      // numH
      // 
      this.numH.DecimalPlaces = 1;
      this.numH.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.numH.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numH.Location = new System.Drawing.Point(430, 101);
      this.numH.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
      this.numH.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
      this.numH.Name = "numH";
      this.numH.Size = new System.Drawing.Size(65, 27);
      this.numH.TabIndex = 4;
      this.numH.ValueChanged += new System.EventHandler(this.numH_ValueChanged);
      this.numH.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_KeyDown);
      this.numH.Leave += new System.EventHandler(this.num_Leave);
      // 
      // numB
      // 
      this.numB.DecimalPlaces = 1;
      this.numB.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.numB.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numB.Location = new System.Drawing.Point(288, 160);
      this.numB.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
      this.numB.Minimum = new decimal(new int[] {
            180,
            0,
            0,
            -2147483648});
      this.numB.Name = "numB";
      this.numB.Size = new System.Drawing.Size(65, 27);
      this.numB.TabIndex = 5;
      this.numB.ValueChanged += new System.EventHandler(this.numB_ValueChanged);
      this.numB.KeyDown += new System.Windows.Forms.KeyEventHandler(this.num_KeyDown);
      this.numB.Leave += new System.EventHandler(this.num_Leave);
      // 
      // btViewOut
      // 
      this.btViewOut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(105)))), ((int)(((byte)(104)))));
      this.btViewOut.BackgroundImage = global::FCamControl.Properties.Resources.bt6DOF_LookDown;
      this.btViewOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btViewOut.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btViewOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btViewOut.ForeColor = System.Drawing.Color.DarkGray;
      this.btViewOut.Location = new System.Drawing.Point(569, 94);
      this.btViewOut.Name = "btViewOut";
      this.btViewOut.Size = new System.Drawing.Size(56, 56);
      this.btViewOut.TabIndex = 7;
      this.btViewOut.UseVisualStyleBackColor = false;
      this.btViewOut.Click += new System.EventHandler(this.btViewOut_Click);
      // 
      // btResetView
      // 
      this.btResetView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(105)))), ((int)(((byte)(104)))));
      this.btResetView.BackgroundImage = global::FCamControl.Properties.Resources.ResetBt;
      this.btResetView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btResetView.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btResetView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btResetView.ForeColor = System.Drawing.Color.DarkGray;
      this.btResetView.Location = new System.Drawing.Point(569, 16);
      this.btResetView.Name = "btResetView";
      this.btResetView.Size = new System.Drawing.Size(56, 56);
      this.btResetView.TabIndex = 48;
      this.btResetView.TabStop = false;
      this.btResetView.UseVisualStyleBackColor = false;
      this.btResetView.Click += new System.EventHandler(this.btResetView_Click);
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.BackColor = System.Drawing.Color.Transparent;
      this.label6.CausesValidation = false;
      this.label6.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.ForeColor = System.Drawing.Color.DarkGray;
      this.label6.Location = new System.Drawing.Point(313, 16);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(35, 17);
      this.label6.TabIndex = 49;
      this.label6.Text = "Pitch";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.BackColor = System.Drawing.Color.Transparent;
      this.label1.CausesValidation = false;
      this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.ForeColor = System.Drawing.Color.DarkGray;
      this.label1.Location = new System.Drawing.Point(74, 16);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(27, 17);
      this.label1.TabIndex = 49;
      this.label1.Text = "L/R";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.BackColor = System.Drawing.Color.Transparent;
      this.label2.CausesValidation = false;
      this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.ForeColor = System.Drawing.Color.DarkGray;
      this.label2.Location = new System.Drawing.Point(10, 192);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(31, 17);
      this.label2.TabIndex = 49;
      this.label2.Text = "U/D";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.BackColor = System.Drawing.Color.Transparent;
      this.label3.CausesValidation = false;
      this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.ForeColor = System.Drawing.Color.DarkGray;
      this.label3.Location = new System.Drawing.Point(210, 81);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(26, 17);
      this.label3.TabIndex = 49;
      this.label3.Text = "F/B";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.BackColor = System.Drawing.Color.Transparent;
      this.label4.CausesValidation = false;
      this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.ForeColor = System.Drawing.Color.DarkGray;
      this.label4.Location = new System.Drawing.Point(285, 192);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(35, 17);
      this.label4.TabIndex = 49;
      this.label4.Text = "Bank";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.BackColor = System.Drawing.Color.Transparent;
      this.label5.CausesValidation = false;
      this.label5.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.ForeColor = System.Drawing.Color.DarkGray;
      this.label5.Location = new System.Drawing.Point(427, 81);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(30, 17);
      this.label5.TabIndex = 49;
      this.label5.Text = "Yaw";
      // 
      // btCamLock
      // 
      this.btCamLock.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(108)))), ((int)(((byte)(105)))), ((int)(((byte)(104)))));
      this.btCamLock.BackColorChecked = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(148)))), ((int)(((byte)(108)))));
      this.btCamLock.BackgroundImage = global::FCamControl.Properties.Resources.btLock;
      this.btCamLock.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btCamLock.Checked = false;
      this.btCamLock.CheckedState = System.Windows.Forms.CheckState.Unchecked;
      this.btCamLock.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btCamLock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btCamLock.ForeColor = System.Drawing.Color.DarkGray;
      this.btCamLock.ForeColorChecked = System.Drawing.Color.Linen;
      this.btCamLock.Location = new System.Drawing.Point(442, 161);
      this.btCamLock.Name = "btCamLock";
      this.btCamLock.Size = new System.Drawing.Size(48, 48);
      this.btCamLock.TabIndex = 35;
      this.btCamLock.ThreeState = false;
      this.btCamLock.UseVisualStyleBackColor = false;
      this.btCamLock.Click += new System.EventHandler(this.btCamLock_Click);
      // 
      // UC_6DOFPanel
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(37)))), ((int)(((byte)(36)))));
      this.BackgroundImage = global::FCamControl.Properties.Resources.Cam6DOF_V2;
      this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.btResetView);
      this.Controls.Add(this.btCamLock);
      this.Controls.Add(this.btViewOut);
      this.Controls.Add(this.numB);
      this.Controls.Add(this.numH);
      this.Controls.Add(this.numP);
      this.Controls.Add(this.numY);
      this.Controls.Add(this.numZ);
      this.Controls.Add(this.numX);
      this.DoubleBuffered = true;
      this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Name = "UC_6DOFPanel";
      this.Size = new System.Drawing.Size(644, 228);
      this.Load += new System.EventHandler(this.UC_6DOFPanel_Load);
      ((System.ComponentModel.ISupportInitialize)(this.numX)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numZ)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numY)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numP)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numH)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numB)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.NumericUpDown numX;
    private System.Windows.Forms.NumericUpDown numZ;
    private System.Windows.Forms.NumericUpDown numY;
    private System.Windows.Forms.NumericUpDown numP;
    private System.Windows.Forms.NumericUpDown numH;
    private System.Windows.Forms.NumericUpDown numB;
    private System.Windows.Forms.Button btViewOut;
    private CheckedButton btCamLock;
    private System.Windows.Forms.Button btResetView;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
  }
}

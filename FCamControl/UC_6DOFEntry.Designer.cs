namespace FCamControl
{
  partial class UC_6DOFEntry
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
      this.cbxLockAcftView = new System.Windows.Forms.CheckBox();
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
      this.numX.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numX.Location = new System.Drawing.Point(67, 7);
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
      this.numX.Size = new System.Drawing.Size(51, 20);
      this.numX.TabIndex = 0;
      this.numX.ValueChanged += new System.EventHandler(this.numX_ValueChanged);
      // 
      // numZ
      // 
      this.numZ.DecimalPlaces = 1;
      this.numZ.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numZ.Location = new System.Drawing.Point(99, 106);
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
      this.numZ.Size = new System.Drawing.Size(51, 20);
      this.numZ.TabIndex = 1;
      this.numZ.ValueChanged += new System.EventHandler(this.numZ_ValueChanged);
      // 
      // numY
      // 
      this.numY.DecimalPlaces = 1;
      this.numY.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numY.Location = new System.Drawing.Point(8, 106);
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
      this.numY.Size = new System.Drawing.Size(51, 20);
      this.numY.TabIndex = 2;
      this.numY.ValueChanged += new System.EventHandler(this.numY_ValueChanged);
      // 
      // numP
      // 
      this.numP.DecimalPlaces = 1;
      this.numP.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numP.Location = new System.Drawing.Point(175, 60);
      this.numP.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
      this.numP.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
      this.numP.Name = "numP";
      this.numP.Size = new System.Drawing.Size(51, 20);
      this.numP.TabIndex = 3;
      this.numP.ValueChanged += new System.EventHandler(this.numP_ValueChanged);
      // 
      // numH
      // 
      this.numH.DecimalPlaces = 1;
      this.numH.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numH.Location = new System.Drawing.Point(239, 7);
      this.numH.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
      this.numH.Minimum = new decimal(new int[] {
            360,
            0,
            0,
            -2147483648});
      this.numH.Name = "numH";
      this.numH.Size = new System.Drawing.Size(51, 20);
      this.numH.TabIndex = 4;
      this.numH.ValueChanged += new System.EventHandler(this.numH_ValueChanged);
      // 
      // numB
      // 
      this.numB.DecimalPlaces = 1;
      this.numB.Increment = new decimal(new int[] {
            1,
            0,
            0,
            65536});
      this.numB.Location = new System.Drawing.Point(175, 106);
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
      this.numB.Size = new System.Drawing.Size(51, 20);
      this.numB.TabIndex = 5;
      this.numB.ValueChanged += new System.EventHandler(this.numB_ValueChanged);
      // 
      // btViewOut
      // 
      this.btViewOut.BackColor = System.Drawing.Color.Transparent;
      this.btViewOut.BackgroundImage = global::FCamControl.Properties.Resources.Target_Next_Tower;
      this.btViewOut.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btViewOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btViewOut.Location = new System.Drawing.Point(373, 7);
      this.btViewOut.Name = "btViewOut";
      this.btViewOut.Size = new System.Drawing.Size(55, 55);
      this.btViewOut.TabIndex = 7;
      this.btViewOut.UseVisualStyleBackColor = false;
      this.btViewOut.Click += new System.EventHandler(this.btViewOut_Click);
      // 
      // cbxLockAcftView
      // 
      this.cbxLockAcftView.AutoSize = true;
      this.cbxLockAcftView.BackColor = System.Drawing.Color.Transparent;
      this.cbxLockAcftView.ForeColor = System.Drawing.Color.WhiteSmoke;
      this.cbxLockAcftView.Location = new System.Drawing.Point(284, 91);
      this.cbxLockAcftView.Name = "cbxLockAcftView";
      this.cbxLockAcftView.Size = new System.Drawing.Size(50, 17);
      this.cbxLockAcftView.TabIndex = 8;
      this.cbxLockAcftView.Text = "Lock";
      this.cbxLockAcftView.UseVisualStyleBackColor = false;
      // 
      // UC_6DOFEntry
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackgroundImage = global::FCamControl.Properties.Resources.Cam6DOF;
      this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.Controls.Add(this.cbxLockAcftView);
      this.Controls.Add(this.btViewOut);
      this.Controls.Add(this.numB);
      this.Controls.Add(this.numH);
      this.Controls.Add(this.numP);
      this.Controls.Add(this.numY);
      this.Controls.Add(this.numZ);
      this.Controls.Add(this.numX);
      this.DoubleBuffered = true;
      this.Name = "UC_6DOFEntry";
      this.Size = new System.Drawing.Size(441, 132);
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
    private System.Windows.Forms.CheckBox cbxLockAcftView;
  }
}

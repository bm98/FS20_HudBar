namespace FS20_HudBar.Config
{
  partial class frmBgImage
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
      this.OFD = new System.Windows.Forms.OpenFileDialog();
      this.txBgImageFile = new System.Windows.Forms.TextBox();
      this.label1 = new System.Windows.Forms.Label();
      this.btSelect = new System.Windows.Forms.Button();
      this.btClear = new System.Windows.Forms.Button();
      this.btAccept = new System.Windows.Forms.Button();
      this.btCancel = new System.Windows.Forms.Button();
      this.label3 = new System.Windows.Forms.Label();
      this.txImageBorder = new System.Windows.Forms.TextBox();
      this.numTop = new System.Windows.Forms.NumericUpDown();
      this.numBottom = new System.Windows.Forms.NumericUpDown();
      this.numLeft = new System.Windows.Forms.NumericUpDown();
      this.numRight = new System.Windows.Forms.NumericUpDown();
      this.btZero = new System.Windows.Forms.Button();
      ((System.ComponentModel.ISupportInitialize)(this.numTop)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numBottom)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numLeft)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numRight)).BeginInit();
      this.SuspendLayout();
      // 
      // OFD
      // 
      this.OFD.FileName = "BackgroundImage.png";
      this.OFD.Filter = "Image Files|*.png;*.jpg;*.jpeg";
      this.OFD.SupportMultiDottedExtensions = true;
      this.OFD.Title = "Select Image";
      // 
      // txBgImageFile
      // 
      this.txBgImageFile.Location = new System.Drawing.Point(15, 27);
      this.txBgImageFile.Name = "txBgImageFile";
      this.txBgImageFile.ReadOnly = true;
      this.txBgImageFile.Size = new System.Drawing.Size(665, 23);
      this.txBgImageFile.TabIndex = 11;
      this.txBgImageFile.TabStop = false;
      this.txBgImageFile.WordWrap = false;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(12, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(113, 15);
      this.label1.TabIndex = 1;
      this.label1.Text = "Image File  selected:";
      // 
      // btSelect
      // 
      this.btSelect.Location = new System.Drawing.Point(584, 56);
      this.btSelect.Name = "btSelect";
      this.btSelect.Size = new System.Drawing.Size(96, 51);
      this.btSelect.TabIndex = 0;
      this.btSelect.Text = "Select Image File...";
      this.btSelect.UseVisualStyleBackColor = true;
      this.btSelect.Click += new System.EventHandler(this.btSelect_Click);
      // 
      // btClear
      // 
      this.btClear.Location = new System.Drawing.Point(446, 56);
      this.btClear.Name = "btClear";
      this.btClear.Size = new System.Drawing.Size(96, 51);
      this.btClear.TabIndex = 6;
      this.btClear.Text = "Clear Image File";
      this.btClear.UseVisualStyleBackColor = true;
      this.btClear.Click += new System.EventHandler(this.btClear_Click);
      // 
      // btAccept
      // 
      this.btAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btAccept.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btAccept.Location = new System.Drawing.Point(467, 146);
      this.btAccept.Name = "btAccept";
      this.btAccept.Size = new System.Drawing.Size(106, 32);
      this.btAccept.TabIndex = 7;
      this.btAccept.Text = "Accept";
      this.btAccept.UseVisualStyleBackColor = true;
      this.btAccept.Click += new System.EventHandler(this.btAccept_Click);
      // 
      // btCancel
      // 
      this.btCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btCancel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btCancel.Location = new System.Drawing.Point(579, 146);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(106, 32);
      this.btCancel.TabIndex = 8;
      this.btCancel.Text = "Cancel";
      this.btCancel.UseVisualStyleBackColor = true;
      this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 64);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(118, 15);
      this.label3.TabIndex = 11;
      this.label3.Text = "Border Area (L;T;R;B):";
      // 
      // txImageBorder
      // 
      this.txImageBorder.Location = new System.Drawing.Point(136, 61);
      this.txImageBorder.Name = "txImageBorder";
      this.txImageBorder.ReadOnly = true;
      this.txImageBorder.Size = new System.Drawing.Size(244, 23);
      this.txImageBorder.TabIndex = 12;
      this.txImageBorder.TabStop = false;
      this.txImageBorder.WordWrap = false;
      // 
      // numTop
      // 
      this.numTop.Location = new System.Drawing.Point(232, 100);
      this.numTop.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
      this.numTop.Name = "numTop";
      this.numTop.Size = new System.Drawing.Size(53, 23);
      this.numTop.TabIndex = 2;
      this.numTop.ValueChanged += new System.EventHandler(this.num_ValueChanged);
      // 
      // numBottom
      // 
      this.numBottom.Location = new System.Drawing.Point(232, 129);
      this.numBottom.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
      this.numBottom.Name = "numBottom";
      this.numBottom.Size = new System.Drawing.Size(53, 23);
      this.numBottom.TabIndex = 4;
      this.numBottom.ValueChanged += new System.EventHandler(this.num_ValueChanged);
      // 
      // numLeft
      // 
      this.numLeft.Location = new System.Drawing.Point(165, 112);
      this.numLeft.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
      this.numLeft.Name = "numLeft";
      this.numLeft.Size = new System.Drawing.Size(53, 23);
      this.numLeft.TabIndex = 1;
      this.numLeft.ValueChanged += new System.EventHandler(this.num_ValueChanged);
      // 
      // numRight
      // 
      this.numRight.Location = new System.Drawing.Point(298, 112);
      this.numRight.Maximum = new decimal(new int[] {
            300,
            0,
            0,
            0});
      this.numRight.Name = "numRight";
      this.numRight.Size = new System.Drawing.Size(53, 23);
      this.numRight.TabIndex = 3;
      this.numRight.ValueChanged += new System.EventHandler(this.num_ValueChanged);
      // 
      // btZero
      // 
      this.btZero.Location = new System.Drawing.Point(67, 150);
      this.btZero.Name = "btZero";
      this.btZero.Size = new System.Drawing.Size(79, 27);
      this.btZero.TabIndex = 5;
      this.btZero.Text = "Set Zero";
      this.btZero.UseVisualStyleBackColor = true;
      this.btZero.Click += new System.EventHandler(this.btZero_Click);
      // 
      // frmBgImage
      // 
      this.AcceptButton = this.btAccept;
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.CancelButton = this.btCancel;
      this.ClientSize = new System.Drawing.Size(697, 190);
      this.ControlBox = false;
      this.Controls.Add(this.btZero);
      this.Controls.Add(this.numRight);
      this.Controls.Add(this.numLeft);
      this.Controls.Add(this.numBottom);
      this.Controls.Add(this.numTop);
      this.Controls.Add(this.txImageBorder);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.btAccept);
      this.Controls.Add(this.btCancel);
      this.Controls.Add(this.btClear);
      this.Controls.Add(this.btSelect);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txBgImageFile);
      this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "frmBgImage";
      this.ShowIcon = false;
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
      this.Text = "Select Background Image";
      this.Load += new System.EventHandler(this.frmBgImage_Load);
      ((System.ComponentModel.ISupportInitialize)(this.numTop)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numBottom)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numLeft)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numRight)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.OpenFileDialog OFD;
    private System.Windows.Forms.TextBox txBgImageFile;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btSelect;
    private System.Windows.Forms.Button btClear;
    private System.Windows.Forms.Button btAccept;
    private System.Windows.Forms.Button btCancel;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox txImageBorder;
    private System.Windows.Forms.NumericUpDown numTop;
    private System.Windows.Forms.NumericUpDown numBottom;
    private System.Windows.Forms.NumericUpDown numLeft;
    private System.Windows.Forms.NumericUpDown numRight;
    private System.Windows.Forms.Button btZero;
  }
}
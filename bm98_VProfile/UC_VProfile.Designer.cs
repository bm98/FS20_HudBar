namespace bm98_VProfile
{
    partial class UC_VProfile
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      this.pbDrawing = new System.Windows.Forms.PictureBox();
      ((System.ComponentModel.ISupportInitialize)(this.pbDrawing)).BeginInit();
      this.SuspendLayout();
      // 
      // pbDrawing
      // 
      this.pbDrawing.BackgroundImage = global::bm98_VProfile.Properties.Resources.VProfile_base_1;
      this.pbDrawing.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.pbDrawing.Location = new System.Drawing.Point(14, 16);
      this.pbDrawing.Name = "pbDrawing";
      this.pbDrawing.Size = new System.Drawing.Size(496, 216);
      this.pbDrawing.TabIndex = 0;
      this.pbDrawing.TabStop = false;
      this.pbDrawing.SizeChanged += new System.EventHandler(this.pbDrawing_SizeChanged);
      // 
      // UC_VProfile
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(24)))));
      this.Controls.Add(this.pbDrawing);
      this.DoubleBuffered = true;
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.MaximumSize = new System.Drawing.Size(900, 240);
      this.MinimumSize = new System.Drawing.Size(900, 240);
      this.Name = "UC_VProfile";
      this.Size = new System.Drawing.Size(900, 240);
      this.Load += new System.EventHandler(this.UC_VProfile_Load);
      this.ClientSizeChanged += new System.EventHandler(this.UC_VProfile_ClientSizeChanged);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.UC_VProfile_Paint);
      ((System.ComponentModel.ISupportInitialize)(this.pbDrawing)).EndInit();
      this.ResumeLayout(false);

        }

    #endregion

    private System.Windows.Forms.PictureBox pbDrawing;
  }
}

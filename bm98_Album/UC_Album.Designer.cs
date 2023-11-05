namespace bm98_Album
{
    partial class UC_Album
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
      this.picPlus = new System.Windows.Forms.PictureBox();
      this.picMinus = new System.Windows.Forms.PictureBox();
      this.picShelf = new System.Windows.Forms.PictureBox();
      this.flp = new System.Windows.Forms.FlowLayoutPanel();
      this.pdfV = new PdfiumViewer.PdfViewer();
      ((System.ComponentModel.ISupportInitialize)(this.picPlus)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.picMinus)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.picShelf)).BeginInit();
      this.SuspendLayout();
      // 
      // picPlus
      // 
      this.picPlus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.picPlus.Cursor = System.Windows.Forms.Cursors.Hand;
      this.picPlus.Image = global::bm98_Album.Properties.Resources.Plus;
      this.picPlus.Location = new System.Drawing.Point(203, 113);
      this.picPlus.Margin = new System.Windows.Forms.Padding(4);
      this.picPlus.Name = "picPlus";
      this.picPlus.Size = new System.Drawing.Size(40, 40);
      this.picPlus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.picPlus.TabIndex = 0;
      this.picPlus.TabStop = false;
      this.picPlus.Click += new System.EventHandler(this.picPlus_Click);
      // 
      // picMinus
      // 
      this.picMinus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.picMinus.Cursor = System.Windows.Forms.Cursors.Hand;
      this.picMinus.Image = global::bm98_Album.Properties.Resources.Minus;
      this.picMinus.Location = new System.Drawing.Point(155, 113);
      this.picMinus.Margin = new System.Windows.Forms.Padding(4);
      this.picMinus.Name = "picMinus";
      this.picMinus.Size = new System.Drawing.Size(40, 40);
      this.picMinus.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.picMinus.TabIndex = 1;
      this.picMinus.TabStop = false;
      this.picMinus.Click += new System.EventHandler(this.picMinus_Click);
      // 
      // picShelf
      // 
      this.picShelf.Cursor = System.Windows.Forms.Cursors.Hand;
      this.picShelf.Image = global::bm98_Album.Properties.Resources.Shelf;
      this.picShelf.Location = new System.Drawing.Point(5, 5);
      this.picShelf.Margin = new System.Windows.Forms.Padding(4);
      this.picShelf.Name = "picShelf";
      this.picShelf.Size = new System.Drawing.Size(50, 50);
      this.picShelf.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
      this.picShelf.TabIndex = 2;
      this.picShelf.TabStop = false;
      this.picShelf.Click += new System.EventHandler(this.picShelf_Click);
      // 
      // flp
      // 
      this.flp.AutoScroll = true;
      this.flp.BackColor = System.Drawing.Color.Transparent;
      this.flp.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flp.Location = new System.Drawing.Point(62, 5);
      this.flp.Margin = new System.Windows.Forms.Padding(4);
      this.flp.Name = "flp";
      this.flp.Size = new System.Drawing.Size(110, 64);
      this.flp.TabIndex = 3;
      this.flp.WrapContents = false;
      // 
      // pdfV
      // 
      this.pdfV.Location = new System.Drawing.Point(51, 90);
      this.pdfV.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
      this.pdfV.Name = "pdfV";
      this.pdfV.Size = new System.Drawing.Size(71, 51);
      this.pdfV.TabIndex = 4;
      // 
      // UC_Album
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.Controls.Add(this.pdfV);
      this.Controls.Add(this.flp);
      this.Controls.Add(this.picShelf);
      this.Controls.Add(this.picMinus);
      this.Controls.Add(this.picPlus);
      this.DoubleBuffered = true;
      this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.Margin = new System.Windows.Forms.Padding(4);
      this.MinimumSize = new System.Drawing.Size(267, 157);
      this.Name = "UC_Album";
      this.Size = new System.Drawing.Size(267, 157);
      this.BackColorChanged += new System.EventHandler(this.UC_Album_BackColorChanged);
      this.ClientSizeChanged += new System.EventHandler(this.UC_Album_ClientSizeChanged);
      this.Paint += new System.Windows.Forms.PaintEventHandler(this.UC_Album_Paint);
      this.Enter += new System.EventHandler(this.UC_Album_Enter);
      this.Leave += new System.EventHandler(this.UC_Album_Leave);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.UC_Album_MouseDown);
      this.MouseEnter += new System.EventHandler(this.UC_Album_MouseEnter);
      this.MouseLeave += new System.EventHandler(this.UC_Album_MouseLeave);
      this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.UC_Album_MouseMove);
      this.MouseUp += new System.Windows.Forms.MouseEventHandler(this.UC_Album_MouseUp);
      ((System.ComponentModel.ISupportInitialize)(this.picPlus)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.picMinus)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.picShelf)).EndInit();
      this.ResumeLayout(false);

        }

    #endregion

    private System.Windows.Forms.PictureBox picPlus;
    private System.Windows.Forms.PictureBox picMinus;
    private System.Windows.Forms.PictureBox picShelf;
    private System.Windows.Forms.FlowLayoutPanel flp;
    private PdfiumViewer.PdfViewer pdfV;
  }
}

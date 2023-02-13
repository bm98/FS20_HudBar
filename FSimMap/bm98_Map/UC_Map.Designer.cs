namespace bm98_Map
{
    partial class UC_Map
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
      this.lblCopyright = new System.Windows.Forms.Label();
      this.lblAirport = new System.Windows.Forms.Label();
      this.btRangeMid = new System.Windows.Forms.Button();
      this.btRangeNear = new System.Windows.Forms.Button();
      this.btRangeClose = new System.Windows.Forms.Button();
      this.btZoomOut = new System.Windows.Forms.Button();
      this.btZoomIn = new System.Windows.Forms.Button();
      this.btZoomNorm = new System.Windows.Forms.Button();
      this.btRangeFar = new System.Windows.Forms.Button();
      this.flpAcftData = new System.Windows.Forms.FlowLayoutPanel();
      this.lblTHdg = new System.Windows.Forms.Label();
      this.lblMTrk = new System.Windows.Forms.Label();
      this.lblAlt = new System.Windows.Forms.Label();
      this.lblIAS = new System.Windows.Forms.Label();
      this.lblGS = new System.Windows.Forms.Label();
      this.lblVS = new System.Windows.Forms.Label();
      this.lblRA = new System.Windows.Forms.Label();
      this.flpProvider = new System.Windows.Forms.FlowLayoutPanel();
      this.btRangeFarFar = new System.Windows.Forms.Button();
      this.btTogApt = new System.Windows.Forms.Button();
      this.btTogVFR = new System.Windows.Forms.Button();
      this.btTogNavaids = new System.Windows.Forms.Button();
      this.btTogRings = new System.Windows.Forms.Button();
      this.btTogGrid = new System.Windows.Forms.Button();
      this.btTogAcftData = new System.Windows.Forms.Button();
      this.btNavaids = new System.Windows.Forms.Button();
      this.btCenterAircraft = new System.Windows.Forms.Button();
      this.btMapProvider = new System.Windows.Forms.Button();
      this.btCenterApt = new System.Windows.Forms.Button();
      this.btTower = new System.Windows.Forms.Button();
      this.btRunway = new System.Windows.Forms.Button();
      this.pbDrawing = new System.Windows.Forms.PictureBox();
      this.pbAltLadder = new System.Windows.Forms.PictureBox();
      this.lblLoading = new System.Windows.Forms.Label();
      this.btTogShowRoute = new System.Windows.Forms.Button();
      this.btRangeXFar = new System.Windows.Forms.Button();
      this.flpAcftData.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbDrawing)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.pbAltLadder)).BeginInit();
      this.SuspendLayout();
      // 
      // lblCopyright
      // 
      this.lblCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.lblCopyright.AutoEllipsis = true;
      this.lblCopyright.AutoSize = true;
      this.lblCopyright.BackColor = System.Drawing.Color.White;
      this.lblCopyright.CausesValidation = false;
      this.lblCopyright.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblCopyright.ForeColor = System.Drawing.Color.Black;
      this.lblCopyright.Location = new System.Drawing.Point(0, 619);
      this.lblCopyright.Name = "lblCopyright";
      this.lblCopyright.Size = new System.Drawing.Size(65, 17);
      this.lblCopyright.TabIndex = 5;
      this.lblCopyright.Text = "Copyright";
      // 
      // lblAirport
      // 
      this.lblAirport.AutoSize = true;
      this.lblAirport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(48)))), ((int)(((byte)(25)))));
      this.lblAirport.CausesValidation = false;
      this.lblAirport.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblAirport.Location = new System.Drawing.Point(3, 42);
      this.lblAirport.Name = "lblAirport";
      this.lblAirport.Size = new System.Drawing.Size(21, 38);
      this.lblAirport.TabIndex = 6;
      this.lblAirport.Text = "...\r\n...";
      // 
      // btRangeMid
      // 
      this.btRangeMid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btRangeMid.BackColor = System.Drawing.Color.Turquoise;
      this.btRangeMid.CausesValidation = false;
      this.btRangeMid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btRangeMid.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btRangeMid.ForeColor = System.Drawing.Color.SaddleBrown;
      this.btRangeMid.Location = new System.Drawing.Point(561, 177);
      this.btRangeMid.Name = "btRangeMid";
      this.btRangeMid.Size = new System.Drawing.Size(34, 34);
      this.btRangeMid.TabIndex = 7;
      this.btRangeMid.Text = "M";
      this.btRangeMid.UseVisualStyleBackColor = false;
      this.btRangeMid.Click += new System.EventHandler(this.btRangeMid_Click);
      // 
      // btRangeNear
      // 
      this.btRangeNear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btRangeNear.BackColor = System.Drawing.Color.Aquamarine;
      this.btRangeNear.CausesValidation = false;
      this.btRangeNear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btRangeNear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btRangeNear.ForeColor = System.Drawing.Color.SaddleBrown;
      this.btRangeNear.Location = new System.Drawing.Point(561, 217);
      this.btRangeNear.Name = "btRangeNear";
      this.btRangeNear.Size = new System.Drawing.Size(34, 34);
      this.btRangeNear.TabIndex = 7;
      this.btRangeNear.Text = "N";
      this.btRangeNear.UseVisualStyleBackColor = false;
      this.btRangeNear.Click += new System.EventHandler(this.btRangeNear_Click);
      // 
      // btRangeClose
      // 
      this.btRangeClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btRangeClose.BackColor = System.Drawing.Color.Aqua;
      this.btRangeClose.CausesValidation = false;
      this.btRangeClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btRangeClose.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btRangeClose.ForeColor = System.Drawing.Color.SaddleBrown;
      this.btRangeClose.Location = new System.Drawing.Point(561, 257);
      this.btRangeClose.Name = "btRangeClose";
      this.btRangeClose.Size = new System.Drawing.Size(34, 34);
      this.btRangeClose.TabIndex = 7;
      this.btRangeClose.Text = "C";
      this.btRangeClose.UseVisualStyleBackColor = false;
      this.btRangeClose.Click += new System.EventHandler(this.btRangeClose_Click);
      // 
      // btZoomOut
      // 
      this.btZoomOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btZoomOut.BackColor = System.Drawing.Color.LavenderBlush;
      this.btZoomOut.CausesValidation = false;
      this.btZoomOut.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btZoomOut.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btZoomOut.ForeColor = System.Drawing.Color.Indigo;
      this.btZoomOut.Location = new System.Drawing.Point(563, 601);
      this.btZoomOut.Name = "btZoomOut";
      this.btZoomOut.Size = new System.Drawing.Size(34, 34);
      this.btZoomOut.TabIndex = 8;
      this.btZoomOut.Text = "-";
      this.btZoomOut.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btZoomOut.UseVisualStyleBackColor = false;
      this.btZoomOut.Click += new System.EventHandler(this.btZoomOut_Click);
      // 
      // btZoomIn
      // 
      this.btZoomIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btZoomIn.BackColor = System.Drawing.Color.LavenderBlush;
      this.btZoomIn.CausesValidation = false;
      this.btZoomIn.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btZoomIn.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btZoomIn.ForeColor = System.Drawing.Color.Indigo;
      this.btZoomIn.Location = new System.Drawing.Point(563, 561);
      this.btZoomIn.Name = "btZoomIn";
      this.btZoomIn.Size = new System.Drawing.Size(34, 34);
      this.btZoomIn.TabIndex = 8;
      this.btZoomIn.Text = "+";
      this.btZoomIn.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btZoomIn.UseVisualStyleBackColor = false;
      this.btZoomIn.Click += new System.EventHandler(this.btZoomIn_Click);
      // 
      // btZoomNorm
      // 
      this.btZoomNorm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btZoomNorm.BackColor = System.Drawing.Color.LavenderBlush;
      this.btZoomNorm.CausesValidation = false;
      this.btZoomNorm.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btZoomNorm.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btZoomNorm.ForeColor = System.Drawing.Color.Indigo;
      this.btZoomNorm.Location = new System.Drawing.Point(563, 521);
      this.btZoomNorm.Name = "btZoomNorm";
      this.btZoomNorm.Size = new System.Drawing.Size(34, 34);
      this.btZoomNorm.TabIndex = 9;
      this.btZoomNorm.Text = "=";
      this.btZoomNorm.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btZoomNorm.UseVisualStyleBackColor = false;
      this.btZoomNorm.Click += new System.EventHandler(this.btZoomNorm_Click);
      // 
      // btRangeFar
      // 
      this.btRangeFar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btRangeFar.BackColor = System.Drawing.Color.LightSeaGreen;
      this.btRangeFar.CausesValidation = false;
      this.btRangeFar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btRangeFar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btRangeFar.ForeColor = System.Drawing.Color.SaddleBrown;
      this.btRangeFar.Location = new System.Drawing.Point(561, 137);
      this.btRangeFar.Name = "btRangeFar";
      this.btRangeFar.Size = new System.Drawing.Size(34, 34);
      this.btRangeFar.TabIndex = 10;
      this.btRangeFar.Text = "F";
      this.btRangeFar.UseVisualStyleBackColor = false;
      this.btRangeFar.Click += new System.EventHandler(this.btRangeFar_Click);
      // 
      // flpAcftData
      // 
      this.flpAcftData.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.flpAcftData.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(12)))), ((int)(((byte)(32)))));
      this.flpAcftData.CausesValidation = false;
      this.flpAcftData.Controls.Add(this.lblTHdg);
      this.flpAcftData.Controls.Add(this.lblMTrk);
      this.flpAcftData.Controls.Add(this.lblAlt);
      this.flpAcftData.Controls.Add(this.lblIAS);
      this.flpAcftData.Controls.Add(this.lblGS);
      this.flpAcftData.Controls.Add(this.lblVS);
      this.flpAcftData.Controls.Add(this.lblRA);
      this.flpAcftData.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flpAcftData.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.flpAcftData.Location = new System.Drawing.Point(266, 388);
      this.flpAcftData.Name = "flpAcftData";
      this.flpAcftData.Size = new System.Drawing.Size(128, 201);
      this.flpAcftData.TabIndex = 13;
      this.flpAcftData.WrapContents = false;
      // 
      // lblTHdg
      // 
      this.lblTHdg.AutoSize = true;
      this.lblTHdg.CausesValidation = false;
      this.lblTHdg.Cursor = System.Windows.Forms.Cursors.Hand;
      this.lblTHdg.ForeColor = System.Drawing.Color.SkyBlue;
      this.lblTHdg.Location = new System.Drawing.Point(5, 5);
      this.lblTHdg.Margin = new System.Windows.Forms.Padding(5);
      this.lblTHdg.Name = "lblTHdg";
      this.lblTHdg.Size = new System.Drawing.Size(64, 18);
      this.lblTHdg.TabIndex = 3;
      this.lblTHdg.Text = "lblTHdg";
      this.lblTHdg.Click += new System.EventHandler(this.lblTHdg_Click);
      // 
      // lblMTrk
      // 
      this.lblMTrk.AutoSize = true;
      this.lblMTrk.CausesValidation = false;
      this.lblMTrk.Cursor = System.Windows.Forms.Cursors.Hand;
      this.lblMTrk.ForeColor = System.Drawing.Color.SkyBlue;
      this.lblMTrk.Location = new System.Drawing.Point(5, 33);
      this.lblMTrk.Margin = new System.Windows.Forms.Padding(5);
      this.lblMTrk.Name = "lblMTrk";
      this.lblMTrk.Size = new System.Drawing.Size(64, 18);
      this.lblMTrk.TabIndex = 3;
      this.lblMTrk.Text = "lblMTrk";
      this.lblMTrk.Click += new System.EventHandler(this.lblMTrk_Click);
      // 
      // lblAlt
      // 
      this.lblAlt.AutoSize = true;
      this.lblAlt.CausesValidation = false;
      this.lblAlt.Cursor = System.Windows.Forms.Cursors.Hand;
      this.lblAlt.ForeColor = System.Drawing.Color.SkyBlue;
      this.lblAlt.Location = new System.Drawing.Point(5, 61);
      this.lblAlt.Margin = new System.Windows.Forms.Padding(5);
      this.lblAlt.Name = "lblAlt";
      this.lblAlt.Size = new System.Drawing.Size(56, 18);
      this.lblAlt.TabIndex = 0;
      this.lblAlt.Text = "lblAlt";
      this.lblAlt.Click += new System.EventHandler(this.lblAlt_Click);
      // 
      // lblIAS
      // 
      this.lblIAS.AutoSize = true;
      this.lblIAS.CausesValidation = false;
      this.lblIAS.Cursor = System.Windows.Forms.Cursors.Hand;
      this.lblIAS.ForeColor = System.Drawing.Color.SkyBlue;
      this.lblIAS.Location = new System.Drawing.Point(5, 89);
      this.lblIAS.Margin = new System.Windows.Forms.Padding(5);
      this.lblIAS.Name = "lblIAS";
      this.lblIAS.Size = new System.Drawing.Size(56, 18);
      this.lblIAS.TabIndex = 1;
      this.lblIAS.Text = "lblIAS";
      this.lblIAS.Click += new System.EventHandler(this.lblIAS_Click);
      // 
      // lblGS
      // 
      this.lblGS.AutoSize = true;
      this.lblGS.CausesValidation = false;
      this.lblGS.Cursor = System.Windows.Forms.Cursors.Hand;
      this.lblGS.ForeColor = System.Drawing.Color.SkyBlue;
      this.lblGS.Location = new System.Drawing.Point(5, 117);
      this.lblGS.Margin = new System.Windows.Forms.Padding(5);
      this.lblGS.Name = "lblGS";
      this.lblGS.Size = new System.Drawing.Size(48, 18);
      this.lblGS.TabIndex = 2;
      this.lblGS.Text = "lblGS";
      this.lblGS.Click += new System.EventHandler(this.lblGS_Click);
      // 
      // lblVS
      // 
      this.lblVS.AutoSize = true;
      this.lblVS.CausesValidation = false;
      this.lblVS.Cursor = System.Windows.Forms.Cursors.Hand;
      this.lblVS.ForeColor = System.Drawing.Color.SkyBlue;
      this.lblVS.Location = new System.Drawing.Point(5, 145);
      this.lblVS.Margin = new System.Windows.Forms.Padding(5);
      this.lblVS.Name = "lblVS";
      this.lblVS.Size = new System.Drawing.Size(48, 18);
      this.lblVS.TabIndex = 2;
      this.lblVS.Text = "lblVS";
      this.lblVS.Click += new System.EventHandler(this.lblVS_Click);
      // 
      // lblRA
      // 
      this.lblRA.AutoSize = true;
      this.lblRA.CausesValidation = false;
      this.lblRA.ForeColor = System.Drawing.Color.Peru;
      this.lblRA.Location = new System.Drawing.Point(5, 173);
      this.lblRA.Margin = new System.Windows.Forms.Padding(5);
      this.lblRA.Name = "lblRA";
      this.lblRA.Size = new System.Drawing.Size(48, 18);
      this.lblRA.TabIndex = 2;
      this.lblRA.Text = "lblRA";
      // 
      // flpProvider
      // 
      this.flpProvider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.flpProvider.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
      this.flpProvider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(209)))), ((int)(((byte)(149)))));
      this.flpProvider.CausesValidation = false;
      this.flpProvider.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
      this.flpProvider.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.flpProvider.ForeColor = System.Drawing.Color.SaddleBrown;
      this.flpProvider.Location = new System.Drawing.Point(393, 57);
      this.flpProvider.Name = "flpProvider";
      this.flpProvider.Size = new System.Drawing.Size(162, 92);
      this.flpProvider.TabIndex = 15;
      this.flpProvider.WrapContents = false;
      // 
      // btRangeFarFar
      // 
      this.btRangeFarFar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btRangeFarFar.BackColor = System.Drawing.Color.Teal;
      this.btRangeFarFar.CausesValidation = false;
      this.btRangeFarFar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btRangeFarFar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btRangeFarFar.ForeColor = System.Drawing.Color.SaddleBrown;
      this.btRangeFarFar.Location = new System.Drawing.Point(561, 97);
      this.btRangeFarFar.Name = "btRangeFarFar";
      this.btRangeFarFar.Size = new System.Drawing.Size(34, 34);
      this.btRangeFarFar.TabIndex = 25;
      this.btRangeFarFar.Text = "FF";
      this.btRangeFarFar.UseVisualStyleBackColor = false;
      this.btRangeFarFar.Click += new System.EventHandler(this.btRangeFarFar_Click);
      // 
      // btTogApt
      // 
      this.btTogApt.BackColor = System.Drawing.Color.DarkGray;
      this.btTogApt.BackgroundImage = global::bm98_Map.Properties.Resources.airport_facility;
      this.btTogApt.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.btTogApt.CausesValidation = false;
      this.btTogApt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btTogApt.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btTogApt.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
      this.btTogApt.Location = new System.Drawing.Point(255, 3);
      this.btTogApt.Name = "btTogApt";
      this.btTogApt.Size = new System.Drawing.Size(36, 36);
      this.btTogApt.TabIndex = 24;
      this.btTogApt.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btTogApt.UseVisualStyleBackColor = false;
      this.btTogApt.Click += new System.EventHandler(this.btTogApt_Click);
      // 
      // btTogVFR
      // 
      this.btTogVFR.BackColor = System.Drawing.Color.DarkGray;
      this.btTogVFR.BackgroundImage = global::bm98_Map.Properties.Resources.vfr;
      this.btTogVFR.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btTogVFR.CausesValidation = false;
      this.btTogVFR.Cursor = System.Windows.Forms.Cursors.Default;
      this.btTogVFR.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btTogVFR.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btTogVFR.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
      this.btTogVFR.Location = new System.Drawing.Point(213, 3);
      this.btTogVFR.Name = "btTogVFR";
      this.btTogVFR.Size = new System.Drawing.Size(36, 36);
      this.btTogVFR.TabIndex = 23;
      this.btTogVFR.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btTogVFR.UseVisualStyleBackColor = false;
      this.btTogVFR.Click += new System.EventHandler(this.btTogVFR_Click);
      // 
      // btTogNavaids
      // 
      this.btTogNavaids.BackColor = System.Drawing.Color.LimeGreen;
      this.btTogNavaids.BackgroundImage = global::bm98_Map.Properties.Resources.navaids;
      this.btTogNavaids.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btTogNavaids.CausesValidation = false;
      this.btTogNavaids.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btTogNavaids.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btTogNavaids.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
      this.btTogNavaids.Location = new System.Drawing.Point(171, 3);
      this.btTogNavaids.Name = "btTogNavaids";
      this.btTogNavaids.Size = new System.Drawing.Size(36, 36);
      this.btTogNavaids.TabIndex = 22;
      this.btTogNavaids.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btTogNavaids.UseVisualStyleBackColor = false;
      this.btTogNavaids.Click += new System.EventHandler(this.btTogNavaids_Click);
      // 
      // btTogRings
      // 
      this.btTogRings.BackColor = System.Drawing.Color.DarkGray;
      this.btTogRings.BackgroundImage = global::bm98_Map.Properties.Resources.rings;
      this.btTogRings.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btTogRings.CausesValidation = false;
      this.btTogRings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btTogRings.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btTogRings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
      this.btTogRings.Location = new System.Drawing.Point(45, 3);
      this.btTogRings.Name = "btTogRings";
      this.btTogRings.Size = new System.Drawing.Size(36, 36);
      this.btTogRings.TabIndex = 21;
      this.btTogRings.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btTogRings.UseVisualStyleBackColor = false;
      this.btTogRings.Click += new System.EventHandler(this.btTogRings_Click);
      // 
      // btTogGrid
      // 
      this.btTogGrid.BackColor = System.Drawing.Color.DarkGray;
      this.btTogGrid.BackgroundImage = global::bm98_Map.Properties.Resources.grid;
      this.btTogGrid.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btTogGrid.CausesValidation = false;
      this.btTogGrid.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btTogGrid.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btTogGrid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
      this.btTogGrid.Location = new System.Drawing.Point(3, 3);
      this.btTogGrid.Name = "btTogGrid";
      this.btTogGrid.Size = new System.Drawing.Size(36, 36);
      this.btTogGrid.TabIndex = 20;
      this.btTogGrid.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btTogGrid.UseVisualStyleBackColor = false;
      this.btTogGrid.Click += new System.EventHandler(this.btTogGrid_Click);
      // 
      // btTogAcftData
      // 
      this.btTogAcftData.BackColor = System.Drawing.Color.DarkGray;
      this.btTogAcftData.BackgroundImage = global::bm98_Map.Properties.Resources.aircraft_dia;
      this.btTogAcftData.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btTogAcftData.CausesValidation = false;
      this.btTogAcftData.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btTogAcftData.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btTogAcftData.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
      this.btTogAcftData.Location = new System.Drawing.Point(87, 3);
      this.btTogAcftData.Name = "btTogAcftData";
      this.btTogAcftData.Size = new System.Drawing.Size(36, 36);
      this.btTogAcftData.TabIndex = 19;
      this.btTogAcftData.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btTogAcftData.UseVisualStyleBackColor = false;
      this.btTogAcftData.Click += new System.EventHandler(this.btTogAcftData_Click);
      // 
      // btNavaids
      // 
      this.btNavaids.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btNavaids.BackColor = System.Drawing.Color.AntiqueWhite;
      this.btNavaids.BackgroundImage = global::bm98_Map.Properties.Resources.navaid;
      this.btNavaids.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btNavaids.CausesValidation = false;
      this.btNavaids.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btNavaids.Location = new System.Drawing.Point(511, 3);
      this.btNavaids.Name = "btNavaids";
      this.btNavaids.Size = new System.Drawing.Size(40, 40);
      this.btNavaids.TabIndex = 18;
      this.btNavaids.UseVisualStyleBackColor = false;
      this.btNavaids.Click += new System.EventHandler(this.btNavaids_Click);
      // 
      // btCenterAircraft
      // 
      this.btCenterAircraft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btCenterAircraft.BackColor = System.Drawing.Color.LightCyan;
      this.btCenterAircraft.BackgroundImage = global::bm98_Map.Properties.Resources.aircraft_smaller;
      this.btCenterAircraft.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.btCenterAircraft.CausesValidation = false;
      this.btCenterAircraft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btCenterAircraft.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btCenterAircraft.ForeColor = System.Drawing.Color.Indigo;
      this.btCenterAircraft.Location = new System.Drawing.Point(561, 344);
      this.btCenterAircraft.Name = "btCenterAircraft";
      this.btCenterAircraft.Size = new System.Drawing.Size(34, 34);
      this.btCenterAircraft.TabIndex = 16;
      this.btCenterAircraft.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btCenterAircraft.UseVisualStyleBackColor = false;
      this.btCenterAircraft.Click += new System.EventHandler(this.btCenterAircraft_Click);
      // 
      // btMapProvider
      // 
      this.btMapProvider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btMapProvider.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(209)))), ((int)(((byte)(149)))));
      this.btMapProvider.BackgroundImage = global::bm98_Map.Properties.Resources.map_icon;
      this.btMapProvider.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btMapProvider.CausesValidation = false;
      this.btMapProvider.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btMapProvider.Location = new System.Drawing.Point(557, 3);
      this.btMapProvider.Name = "btMapProvider";
      this.btMapProvider.Size = new System.Drawing.Size(40, 40);
      this.btMapProvider.TabIndex = 14;
      this.btMapProvider.UseVisualStyleBackColor = false;
      this.btMapProvider.Click += new System.EventHandler(this.btMapProvider_Click);
      // 
      // btCenterApt
      // 
      this.btCenterApt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btCenterApt.BackColor = System.Drawing.Color.LightCyan;
      this.btCenterApt.BackgroundImage = global::bm98_Map.Properties.Resources.airport_facility;
      this.btCenterApt.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.btCenterApt.CausesValidation = false;
      this.btCenterApt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btCenterApt.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btCenterApt.ForeColor = System.Drawing.Color.Indigo;
      this.btCenterApt.Location = new System.Drawing.Point(561, 306);
      this.btCenterApt.Name = "btCenterApt";
      this.btCenterApt.Size = new System.Drawing.Size(34, 34);
      this.btCenterApt.TabIndex = 11;
      this.btCenterApt.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btCenterApt.UseVisualStyleBackColor = false;
      this.btCenterApt.Click += new System.EventHandler(this.btCenterApt_Click);
      // 
      // btTower
      // 
      this.btTower.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btTower.BackColor = System.Drawing.Color.LightSkyBlue;
      this.btTower.BackgroundImage = global::bm98_Map.Properties.Resources.tower_255;
      this.btTower.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btTower.CausesValidation = false;
      this.btTower.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btTower.Location = new System.Drawing.Point(465, 3);
      this.btTower.Name = "btTower";
      this.btTower.Size = new System.Drawing.Size(40, 40);
      this.btTower.TabIndex = 2;
      this.btTower.UseVisualStyleBackColor = false;
      this.btTower.Click += new System.EventHandler(this.btTower_Click);
      // 
      // btRunway
      // 
      this.btRunway.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btRunway.BackColor = System.Drawing.Color.YellowGreen;
      this.btRunway.BackgroundImage = global::bm98_Map.Properties.Resources.runway_alt;
      this.btRunway.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btRunway.CausesValidation = false;
      this.btRunway.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btRunway.Location = new System.Drawing.Point(419, 3);
      this.btRunway.Name = "btRunway";
      this.btRunway.Size = new System.Drawing.Size(40, 40);
      this.btRunway.TabIndex = 1;
      this.btRunway.UseVisualStyleBackColor = false;
      this.btRunway.Click += new System.EventHandler(this.btRunway_Click);
      // 
      // pbDrawing
      // 
      this.pbDrawing.BackColor = System.Drawing.Color.LemonChiffon;
      this.pbDrawing.BackgroundImage = global::bm98_Map.Properties.Resources.background;
      this.pbDrawing.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.pbDrawing.Location = new System.Drawing.Point(9, 141);
      this.pbDrawing.Name = "pbDrawing";
      this.pbDrawing.Size = new System.Drawing.Size(94, 65);
      this.pbDrawing.TabIndex = 0;
      this.pbDrawing.TabStop = false;
      // 
      // pbAltLadder
      // 
      this.pbAltLadder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
      this.pbAltLadder.BackgroundImage = global::bm98_Map.Properties.Resources.alt_ladder;
      this.pbAltLadder.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.pbAltLadder.Location = new System.Drawing.Point(0, 592);
      this.pbAltLadder.Margin = new System.Windows.Forms.Padding(0);
      this.pbAltLadder.Name = "pbAltLadder";
      this.pbAltLadder.Size = new System.Drawing.Size(557, 25);
      this.pbAltLadder.TabIndex = 26;
      this.pbAltLadder.TabStop = false;
      this.pbAltLadder.Visible = false;
      // 
      // lblLoading
      // 
      this.lblLoading.AutoSize = true;
      this.lblLoading.BackColor = System.Drawing.Color.YellowGreen;
      this.lblLoading.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblLoading.ForeColor = System.Drawing.Color.Black;
      this.lblLoading.Location = new System.Drawing.Point(308, 3);
      this.lblLoading.Name = "lblLoading";
      this.lblLoading.Size = new System.Drawing.Size(105, 17);
      this.lblLoading.TabIndex = 27;
      this.lblLoading.Text = "Loading Map ...";
      // 
      // btTogShowRoute
      // 
      this.btTogShowRoute.BackColor = System.Drawing.Color.DarkGray;
      this.btTogShowRoute.BackgroundImage = global::bm98_Map.Properties.Resources.route_waypoint;
      this.btTogShowRoute.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
      this.btTogShowRoute.CausesValidation = false;
      this.btTogShowRoute.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btTogShowRoute.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btTogShowRoute.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(16)))), ((int)(((byte)(16)))));
      this.btTogShowRoute.Location = new System.Drawing.Point(129, 3);
      this.btTogShowRoute.Name = "btTogShowRoute";
      this.btTogShowRoute.Size = new System.Drawing.Size(36, 36);
      this.btTogShowRoute.TabIndex = 28;
      this.btTogShowRoute.TextAlign = System.Drawing.ContentAlignment.TopCenter;
      this.btTogShowRoute.UseVisualStyleBackColor = false;
      this.btTogShowRoute.Click += new System.EventHandler(this.btTogShowRoute_Click);
      // 
      // btRangeXFar
      // 
      this.btRangeXFar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.btRangeXFar.BackColor = System.Drawing.Color.DarkSlateGray;
      this.btRangeXFar.CausesValidation = false;
      this.btRangeXFar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btRangeXFar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btRangeXFar.ForeColor = System.Drawing.Color.SaddleBrown;
      this.btRangeXFar.Location = new System.Drawing.Point(561, 57);
      this.btRangeXFar.Name = "btRangeXFar";
      this.btRangeXFar.Size = new System.Drawing.Size(34, 34);
      this.btRangeXFar.TabIndex = 29;
      this.btRangeXFar.Text = "XF";
      this.btRangeXFar.UseVisualStyleBackColor = false;
      this.btRangeXFar.Click += new System.EventHandler(this.btRangeXFar_Click);
      // 
      // UC_Map
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(16)))));
      this.CausesValidation = false;
      this.Controls.Add(this.btRangeXFar);
      this.Controls.Add(this.btTogShowRoute);
      this.Controls.Add(this.lblLoading);
      this.Controls.Add(this.btRangeFarFar);
      this.Controls.Add(this.btTogApt);
      this.Controls.Add(this.btTogVFR);
      this.Controls.Add(this.flpProvider);
      this.Controls.Add(this.btTogNavaids);
      this.Controls.Add(this.btTogRings);
      this.Controls.Add(this.btTogGrid);
      this.Controls.Add(this.btTogAcftData);
      this.Controls.Add(this.btNavaids);
      this.Controls.Add(this.btCenterAircraft);
      this.Controls.Add(this.btMapProvider);
      this.Controls.Add(this.flpAcftData);
      this.Controls.Add(this.btCenterApt);
      this.Controls.Add(this.btRangeFar);
      this.Controls.Add(this.btZoomNorm);
      this.Controls.Add(this.btZoomIn);
      this.Controls.Add(this.btZoomOut);
      this.Controls.Add(this.btRangeClose);
      this.Controls.Add(this.btRangeNear);
      this.Controls.Add(this.btRangeMid);
      this.Controls.Add(this.lblAirport);
      this.Controls.Add(this.lblCopyright);
      this.Controls.Add(this.btTower);
      this.Controls.Add(this.btRunway);
      this.Controls.Add(this.pbDrawing);
      this.Controls.Add(this.pbAltLadder);
      this.DoubleBuffered = true;
      this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.ForeColor = System.Drawing.Color.LightYellow;
      this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.MinimumSize = new System.Drawing.Size(600, 640);
      this.Name = "UC_Map";
      this.Size = new System.Drawing.Size(600, 640);
      this.Load += new System.EventHandler(this.UC_Map_Load);
      this.flpAcftData.ResumeLayout(false);
      this.flpAcftData.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pbDrawing)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.pbAltLadder)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

        }

    #endregion

    private System.Windows.Forms.PictureBox pbDrawing;
    private System.Windows.Forms.Button btRunway;
    private System.Windows.Forms.Button btTower;
    private System.Windows.Forms.Label lblCopyright;
    private System.Windows.Forms.Label lblAirport;
    private System.Windows.Forms.Button btRangeMid;
    private System.Windows.Forms.Button btRangeNear;
    private System.Windows.Forms.Button btRangeClose;
    private System.Windows.Forms.Button btZoomOut;
    private System.Windows.Forms.Button btZoomIn;
    private System.Windows.Forms.Button btZoomNorm;
    private System.Windows.Forms.Button btRangeFar;
    private System.Windows.Forms.Button btCenterApt;
    private System.Windows.Forms.FlowLayoutPanel flpAcftData;
    private System.Windows.Forms.Label lblAlt;
    private System.Windows.Forms.Label lblIAS;
    private System.Windows.Forms.Label lblVS;
    private System.Windows.Forms.Label lblRA;
    private System.Windows.Forms.Button btMapProvider;
    private System.Windows.Forms.FlowLayoutPanel flpProvider;
    private System.Windows.Forms.Button btCenterAircraft;
    private System.Windows.Forms.Label lblTHdg;
    private System.Windows.Forms.Button btNavaids;
    private System.Windows.Forms.Button btTogAcftData;
    private System.Windows.Forms.Button btTogGrid;
    private System.Windows.Forms.Button btTogRings;
    private System.Windows.Forms.Button btTogNavaids;
    private System.Windows.Forms.Button btTogVFR;
    private System.Windows.Forms.Button btTogApt;
    private System.Windows.Forms.Button btRangeFarFar;
    private System.Windows.Forms.Label lblGS;
    private System.Windows.Forms.PictureBox pbAltLadder;
    private System.Windows.Forms.Label lblLoading;
    private System.Windows.Forms.Button btTogShowRoute;
    private System.Windows.Forms.Button btRangeXFar;
    private System.Windows.Forms.Label lblMTrk;
  }
}

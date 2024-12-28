
namespace FS20_HudBar.Config
{
  partial class frmApt
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmApt));
      this.btClose = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.txArr = new System.Windows.Forms.TextBox();
      this.btClear = new System.Windows.Forms.Button();
      this.txDep = new System.Windows.Forms.TextBox();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.lblSBPilotID = new System.Windows.Forms.Label();
      this.btLoadSBPlan = new System.Windows.Forms.Button();
      this.btLoadDefaultPLN = new System.Windows.Forms.Button();
      this.btLoadPlanFile = new System.Windows.Forms.Button();
      this.lblLoading = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.btRouteString = new System.Windows.Forms.Button();
      this.groupBox1 = new System.Windows.Forms.GroupBox();
      this.txRoute = new System.Windows.Forms.TextBox();
      this.lblRouteStatus = new System.Windows.Forms.Label();
      this.groupBox1.SuspendLayout();
      this.SuspendLayout();
      // 
      // btClose
      // 
      this.btClose.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btClose.Location = new System.Drawing.Point(584, 274);
      this.btClose.Name = "btClose";
      this.btClose.Size = new System.Drawing.Size(106, 25);
      this.btClose.TabIndex = 2;
      this.btClose.Text = "Close";
      this.btClose.UseVisualStyleBackColor = true;
      this.btClose.Click += new System.EventHandler(this.btClose_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(8, 226);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(187, 20);
      this.label1.TabIndex = 5;
      this.label1.Text = "or enter valid airport ICAO:";
      // 
      // txArr
      // 
      this.txArr.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
      this.txArr.Location = new System.Drawing.Point(212, 253);
      this.txArr.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txArr.Name = "txArr";
      this.txArr.Size = new System.Drawing.Size(105, 27);
      this.txArr.TabIndex = 1;
      this.txArr.Text = "...";
      // 
      // btClear
      // 
      this.btClear.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btClear.Location = new System.Drawing.Point(584, 225);
      this.btClear.Name = "btClear";
      this.btClear.Size = new System.Drawing.Size(105, 25);
      this.btClear.TabIndex = 4;
      this.btClear.Text = "Clear all";
      this.btClear.UseVisualStyleBackColor = true;
      this.btClear.Click += new System.EventHandler(this.btClear_Click);
      // 
      // txDep
      // 
      this.txDep.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
      this.txDep.Location = new System.Drawing.Point(51, 253);
      this.txDep.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.txDep.Name = "txDep";
      this.txDep.Size = new System.Drawing.Size(106, 27);
      this.txDep.TabIndex = 0;
      this.txDep.Text = "...";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(8, 256);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(36, 20);
      this.label3.TabIndex = 8;
      this.label3.Text = "DEP";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(172, 256);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(37, 20);
      this.label4.TabIndex = 9;
      this.label4.Text = "ARR";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(155, 37);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(52, 17);
      this.label5.TabIndex = 11;
      this.label5.Text = "Pilot ID:";
      // 
      // lblSBPilotID
      // 
      this.lblSBPilotID.AutoSize = true;
      this.lblSBPilotID.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblSBPilotID.Location = new System.Drawing.Point(213, 37);
      this.lblSBPilotID.Name = "lblSBPilotID";
      this.lblSBPilotID.Size = new System.Drawing.Size(57, 17);
      this.lblSBPilotID.TabIndex = 12;
      this.lblSBPilotID.Text = "0000000";
      // 
      // btLoadSBPlan
      // 
      this.btLoadSBPlan.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btLoadSBPlan.Location = new System.Drawing.Point(12, 32);
      this.btLoadSBPlan.Name = "btLoadSBPlan";
      this.btLoadSBPlan.Size = new System.Drawing.Size(137, 29);
      this.btLoadSBPlan.TabIndex = 13;
      this.btLoadSBPlan.Text = "Load SimBrief Plan";
      this.btLoadSBPlan.UseVisualStyleBackColor = true;
      this.btLoadSBPlan.Click += new System.EventHandler(this.btLoadSBPlan_Click);
      // 
      // btLoadDefaultPLN
      // 
      this.btLoadDefaultPLN.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btLoadDefaultPLN.Location = new System.Drawing.Point(287, 32);
      this.btLoadDefaultPLN.Name = "btLoadDefaultPLN";
      this.btLoadDefaultPLN.Size = new System.Drawing.Size(120, 29);
      this.btLoadDefaultPLN.TabIndex = 13;
      this.btLoadDefaultPLN.Text = "Load default PLN";
      this.btLoadDefaultPLN.UseVisualStyleBackColor = true;
      this.btLoadDefaultPLN.Click += new System.EventHandler(this.btLoadDefaultPLN_Click);
      // 
      // btLoadPlanFile
      // 
      this.btLoadPlanFile.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btLoadPlanFile.Location = new System.Drawing.Point(413, 32);
      this.btLoadPlanFile.Name = "btLoadPlanFile";
      this.btLoadPlanFile.Size = new System.Drawing.Size(120, 29);
      this.btLoadPlanFile.TabIndex = 13;
      this.btLoadPlanFile.Text = "Load Plan f. Disk ...";
      this.btLoadPlanFile.UseVisualStyleBackColor = true;
      this.btLoadPlanFile.Click += new System.EventHandler(this.btLoadPlanFile_Click);
      // 
      // lblLoading
      // 
      this.lblLoading.AutoSize = true;
      this.lblLoading.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblLoading.Location = new System.Drawing.Point(364, 243);
      this.lblLoading.Name = "lblLoading";
      this.lblLoading.Size = new System.Drawing.Size(43, 17);
      this.lblLoading.TabIndex = 14;
      this.lblLoading.Text = "Status";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(8, 9);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(127, 20);
      this.label2.TabIndex = 15;
      this.label2.Text = "Load a Flight Plan";
      // 
      // btRouteString
      // 
      this.btRouteString.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btRouteString.Location = new System.Drawing.Point(569, 32);
      this.btRouteString.Name = "btRouteString";
      this.btRouteString.Size = new System.Drawing.Size(120, 29);
      this.btRouteString.TabIndex = 17;
      this.btRouteString.Text = "Load Route";
      this.btRouteString.UseVisualStyleBackColor = true;
      this.btRouteString.Click += new System.EventHandler(this.btRouteString_Click);
      // 
      // groupBox1
      // 
      this.groupBox1.Controls.Add(this.lblRouteStatus);
      this.groupBox1.Controls.Add(this.txRoute);
      this.groupBox1.Location = new System.Drawing.Point(12, 67);
      this.groupBox1.Name = "groupBox1";
      this.groupBox1.Size = new System.Drawing.Size(678, 152);
      this.groupBox1.TabIndex = 18;
      this.groupBox1.TabStop = false;
      this.groupBox1.Text = "Route";
      // 
      // txRoute
      // 
      this.txRoute.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txRoute.Location = new System.Drawing.Point(6, 26);
      this.txRoute.Multiline = true;
      this.txRoute.Name = "txRoute";
      this.txRoute.Size = new System.Drawing.Size(666, 98);
      this.txRoute.TabIndex = 0;
      this.txRoute.Text = resources.GetString("txRoute.Text");
      // 
      // lblRouteStatus
      // 
      this.lblRouteStatus.AutoSize = true;
      this.lblRouteStatus.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblRouteStatus.ForeColor = System.Drawing.Color.Blue;
      this.lblRouteStatus.Location = new System.Drawing.Point(6, 127);
      this.lblRouteStatus.Name = "lblRouteStatus";
      this.lblRouteStatus.Size = new System.Drawing.Size(58, 17);
      this.lblRouteStatus.TabIndex = 15;
      this.lblRouteStatus.Text = "RTE help";
      // 
      // frmApt
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.ClientSize = new System.Drawing.Size(702, 308);
      this.Controls.Add(this.groupBox1);
      this.Controls.Add(this.btRouteString);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.lblLoading);
      this.Controls.Add(this.btLoadPlanFile);
      this.Controls.Add(this.btLoadDefaultPLN);
      this.Controls.Add(this.btLoadSBPlan);
      this.Controls.Add(this.lblSBPilotID);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.txDep);
      this.Controls.Add(this.btClear);
      this.Controls.Add(this.btClose);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.txArr);
      this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "frmApt";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Route Departure - Destination";
      this.TopMost = true;
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmApt_FormClosing);
      this.Load += new System.EventHandler(this.frmApt_Load);
      this.groupBox1.ResumeLayout(false);
      this.groupBox1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion
    private System.Windows.Forms.Button btClose;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txArr;
    private System.Windows.Forms.Button btClear;
    private System.Windows.Forms.TextBox txDep;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label lblSBPilotID;
    private System.Windows.Forms.Button btLoadSBPlan;
    private System.Windows.Forms.Button btLoadDefaultPLN;
    private System.Windows.Forms.Button btLoadPlanFile;
    private System.Windows.Forms.Label lblLoading;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button btRouteString;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.TextBox txRoute;
    private System.Windows.Forms.Label lblRouteStatus;
  }
}

namespace TEST_Map
{
  partial class Form1
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
      this.button1 = new System.Windows.Forms.Button();
      this.btZoomOut = new System.Windows.Forms.Button();
      this.btZoomIn = new System.Windows.Forms.Button();
      this.btTrackAcft = new System.Windows.Forms.Button();
      this.tlp = new System.Windows.Forms.TableLayoutPanel();
      this.panel1 = new System.Windows.Forms.Panel();
      this.txAirport = new System.Windows.Forms.TextBox();
      this.btNavaid = new System.Windows.Forms.Button();
      this.lblEvent = new System.Windows.Forms.Label();
      this.btTogGrid = new System.Windows.Forms.Button();
      this.uC_Map1 = new bm98_Map.UC_Map();
      this.btSetRoute = new System.Windows.Forms.Button();
      this.tlp.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(14, 13);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(96, 44);
      this.button1.TabIndex = 1;
      this.button1.Text = "button1";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // btZoomOut
      // 
      this.btZoomOut.Location = new System.Drawing.Point(14, 74);
      this.btZoomOut.Name = "btZoomOut";
      this.btZoomOut.Size = new System.Drawing.Size(45, 44);
      this.btZoomOut.TabIndex = 2;
      this.btZoomOut.Text = "-";
      this.btZoomOut.UseVisualStyleBackColor = true;
      this.btZoomOut.Click += new System.EventHandler(this.btZoomOut_Click);
      // 
      // btZoomIn
      // 
      this.btZoomIn.Location = new System.Drawing.Point(65, 74);
      this.btZoomIn.Name = "btZoomIn";
      this.btZoomIn.Size = new System.Drawing.Size(45, 44);
      this.btZoomIn.TabIndex = 2;
      this.btZoomIn.Text = "+";
      this.btZoomIn.UseVisualStyleBackColor = true;
      this.btZoomIn.Click += new System.EventHandler(this.btZoomIn_Click);
      // 
      // btTrackAcft
      // 
      this.btTrackAcft.Location = new System.Drawing.Point(3, 344);
      this.btTrackAcft.Name = "btTrackAcft";
      this.btTrackAcft.Size = new System.Drawing.Size(78, 28);
      this.btTrackAcft.TabIndex = 6;
      this.btTrackAcft.Text = "Set Aircraft";
      this.btTrackAcft.UseVisualStyleBackColor = true;
      this.btTrackAcft.Click += new System.EventHandler(this.btTrackAcft_Click);
      // 
      // tlp
      // 
      this.tlp.ColumnCount = 2;
      this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 125F));
      this.tlp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
      this.tlp.Controls.Add(this.panel1, 0, 0);
      this.tlp.Controls.Add(this.uC_Map1, 1, 0);
      this.tlp.Location = new System.Drawing.Point(12, 12);
      this.tlp.Name = "tlp";
      this.tlp.RowCount = 1;
      this.tlp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tlp.Size = new System.Drawing.Size(738, 652);
      this.tlp.TabIndex = 7;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btSetRoute);
      this.panel1.Controls.Add(this.txAirport);
      this.panel1.Controls.Add(this.btNavaid);
      this.panel1.Controls.Add(this.lblEvent);
      this.panel1.Controls.Add(this.btTogGrid);
      this.panel1.Controls.Add(this.btTrackAcft);
      this.panel1.Controls.Add(this.button1);
      this.panel1.Controls.Add(this.btZoomOut);
      this.panel1.Controls.Add(this.btZoomIn);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel1.Location = new System.Drawing.Point(3, 3);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(119, 646);
      this.panel1.TabIndex = 0;
      // 
      // txAirport
      // 
      this.txAirport.Location = new System.Drawing.Point(14, 147);
      this.txAirport.Name = "txAirport";
      this.txAirport.Size = new System.Drawing.Size(84, 20);
      this.txAirport.TabIndex = 10;
      // 
      // btNavaid
      // 
      this.btNavaid.Location = new System.Drawing.Point(6, 483);
      this.btNavaid.Name = "btNavaid";
      this.btNavaid.Size = new System.Drawing.Size(78, 28);
      this.btNavaid.TabIndex = 9;
      this.btNavaid.Text = "Set Navaid";
      this.btNavaid.UseVisualStyleBackColor = true;
      this.btNavaid.Click += new System.EventHandler(this.btNavaid_Click);
      // 
      // lblEvent
      // 
      this.lblEvent.AutoSize = true;
      this.lblEvent.Location = new System.Drawing.Point(3, 444);
      this.lblEvent.Name = "lblEvent";
      this.lblEvent.Size = new System.Drawing.Size(35, 13);
      this.lblEvent.TabIndex = 8;
      this.lblEvent.Text = "label2";
      // 
      // btTogGrid
      // 
      this.btTogGrid.Location = new System.Drawing.Point(3, 378);
      this.btTogGrid.Name = "btTogGrid";
      this.btTogGrid.Size = new System.Drawing.Size(74, 35);
      this.btTogGrid.TabIndex = 7;
      this.btTogGrid.Text = "Tog Grid";
      this.btTogGrid.UseVisualStyleBackColor = true;
      this.btTogGrid.Click += new System.EventHandler(this.btTogGrid_Click);
      // 
      // uC_Map1
      // 
      this.uC_Map1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(16)))));
      this.uC_Map1.CausesValidation = false;
      this.uC_Map1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.uC_Map1.ForeColor = System.Drawing.Color.LightYellow;
      this.uC_Map1.Location = new System.Drawing.Point(128, 4);
      this.uC_Map1.MapRange = bm98_Map.MapRange.Near;
      this.uC_Map1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.uC_Map1.MinimumSize = new System.Drawing.Size(600, 640);
      this.uC_Map1.Name = "uC_Map1";
      this.uC_Map1.ShowAirportRange = false;
      this.uC_Map1.ShowAptMarks = false;
      this.uC_Map1.ShowMapGrid = false;
      this.uC_Map1.ShowNavaids = false;
      this.uC_Map1.ShowTrackedAircraft = false;
      this.uC_Map1.ShowVFRMarks = false;
      this.uC_Map1.Size = new System.Drawing.Size(600, 640);
      this.uC_Map1.TabIndex = 1;
      // 
      // btSetRoute
      // 
      this.btSetRoute.Location = new System.Drawing.Point(6, 517);
      this.btSetRoute.Name = "btSetRoute";
      this.btSetRoute.Size = new System.Drawing.Size(78, 28);
      this.btSetRoute.TabIndex = 11;
      this.btSetRoute.Text = "Set Route";
      this.btSetRoute.UseVisualStyleBackColor = true;
      this.btSetRoute.Click += new System.EventHandler(this.btSetRoute_Click);
      // 
      // Form1
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.ClientSize = new System.Drawing.Size(765, 673);
      this.Controls.Add(this.tlp);
      this.MinimumSize = new System.Drawing.Size(781, 712);
      this.Name = "Form1";
      this.Text = "Form1";
      this.tlp.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button btZoomOut;
    private System.Windows.Forms.Button btZoomIn;
    private System.Windows.Forms.Button btTrackAcft;
    private System.Windows.Forms.TableLayoutPanel tlp;
    private System.Windows.Forms.Panel panel1;
    private bm98_Map.UC_Map uC_Map1;
    private System.Windows.Forms.Button btTogGrid;
    private System.Windows.Forms.Label lblEvent;
    private System.Windows.Forms.Button btNavaid;
    private System.Windows.Forms.TextBox txAirport;
    private System.Windows.Forms.Button btSetRoute;
  }
}


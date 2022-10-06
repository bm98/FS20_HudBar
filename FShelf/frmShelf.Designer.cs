
namespace FShelf
{
  partial class frmShelf
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmShelf));
      this.tab = new System.Windows.Forms.TabControl();
      this.tabShelf = new System.Windows.Forms.TabPage();
      this.aShelf = new bm98_Album.UC_Album();
      this.tabMap = new System.Windows.Forms.TabPage();
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.panel1 = new System.Windows.Forms.Panel();
      this.lblSimConnectedMap = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label1 = new System.Windows.Forms.Label();
      this.btGetAirport = new System.Windows.Forms.Button();
      this.txEntry = new System.Windows.Forms.TextBox();
      this.lblARR = new System.Windows.Forms.Label();
      this.lblDEP = new System.Windows.Forms.Label();
      this.aMap = new bm98_Map.UC_Map();
      this.tabMetar = new System.Windows.Forms.TabPage();
      this.lblMetArr = new System.Windows.Forms.Label();
      this.lblMetDep = new System.Windows.Forms.Label();
      this.btMetAcft = new System.Windows.Forms.Button();
      this.label17 = new System.Windows.Forms.Label();
      this.btMetClear = new System.Windows.Forms.Button();
      this.btMetApt = new System.Windows.Forms.Button();
      this.btMetArr = new System.Windows.Forms.Button();
      this.btMetDep = new System.Windows.Forms.Button();
      this.label14 = new System.Windows.Forms.Label();
      this.label12 = new System.Windows.Forms.Label();
      this.txMetApt = new System.Windows.Forms.TextBox();
      this.label13 = new System.Windows.Forms.Label();
      this.rtbMetar = new System.Windows.Forms.RichTextBox();
      this.tabPerf = new System.Windows.Forms.TabPage();
      this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
      this.lblSimConnectedNotes = new System.Windows.Forms.Label();
      this.rtbPerf = new System.Windows.Forms.RichTextBox();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.rbKLbs = new System.Windows.Forms.RadioButton();
      this.rbKg = new System.Windows.Forms.RadioButton();
      this.pictureBox1 = new System.Windows.Forms.PictureBox();
      this.btPerfRefresh = new System.Windows.Forms.Button();
      this.tabNotes = new System.Windows.Forms.TabPage();
      this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
      this.rtbNotes = new System.Windows.Forms.RichTextBox();
      this.btNotesClear = new System.Windows.Forms.Button();
      this.tabConfig = new System.Windows.Forms.TabPage();
      this.cbxAcftTrack = new System.Windows.Forms.CheckBox();
      this.cbxAcftRange = new System.Windows.Forms.CheckBox();
      this.label16 = new System.Windows.Forms.Label();
      this.comboCfgRunwayLength = new System.Windows.Forms.ComboBox();
      this.label15 = new System.Windows.Forms.Label();
      this.label11 = new System.Windows.Forms.Label();
      this.label10 = new System.Windows.Forms.Label();
      this.cbxIFRwaypoints = new System.Windows.Forms.CheckBox();
      this.cbxCfgPrettyMetar = new System.Windows.Forms.CheckBox();
      this.btCfgSelFolder = new System.Windows.Forms.Button();
      this.txCfgShelfFolder = new System.Windows.Forms.TextBox();
      this.label8 = new System.Windows.Forms.Label();
      this.label7 = new System.Windows.Forms.Label();
      this.label9 = new System.Windows.Forms.Label();
      this.lblCfgArr = new System.Windows.Forms.Label();
      this.lblCfgDep = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.txCfgArr = new System.Windows.Forms.TextBox();
      this.txCfgDep = new System.Windows.Forms.TextBox();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      this.FBD = new System.Windows.Forms.FolderBrowserDialog();
      this.tab.SuspendLayout();
      this.tabShelf.SuspendLayout();
      this.tabMap.SuspendLayout();
      this.tableLayoutPanel1.SuspendLayout();
      this.panel1.SuspendLayout();
      this.tabMetar.SuspendLayout();
      this.tabPerf.SuspendLayout();
      this.tableLayoutPanel3.SuspendLayout();
      this.flowLayoutPanel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
      this.tabNotes.SuspendLayout();
      this.tableLayoutPanel2.SuspendLayout();
      this.tabConfig.SuspendLayout();
      this.SuspendLayout();
      // 
      // tab
      // 
      this.tab.Controls.Add(this.tabShelf);
      this.tab.Controls.Add(this.tabMap);
      this.tab.Controls.Add(this.tabMetar);
      this.tab.Controls.Add(this.tabPerf);
      this.tab.Controls.Add(this.tabNotes);
      this.tab.Controls.Add(this.tabConfig);
      this.tab.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tab.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tab.HotTrack = true;
      this.tab.ItemSize = new System.Drawing.Size(100, 30);
      this.tab.Location = new System.Drawing.Point(0, 0);
      this.tab.Margin = new System.Windows.Forms.Padding(0);
      this.tab.Name = "tab";
      this.tab.SelectedIndex = 0;
      this.tab.Size = new System.Drawing.Size(619, 736);
      this.tab.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
      this.tab.TabIndex = 1;
      this.tab.SelectedIndexChanged += new System.EventHandler(this.tab_SelectedIndexChanged);
      // 
      // tabShelf
      // 
      this.tabShelf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(44)))), ((int)(((byte)(67)))));
      this.tabShelf.Controls.Add(this.aShelf);
      this.tabShelf.Location = new System.Drawing.Point(4, 34);
      this.tabShelf.Name = "tabShelf";
      this.tabShelf.Padding = new System.Windows.Forms.Padding(3);
      this.tabShelf.Size = new System.Drawing.Size(611, 698);
      this.tabShelf.TabIndex = 0;
      this.tabShelf.Text = "Shelf";
      this.tabShelf.SizeChanged += new System.EventHandler(this.tabShelf_SizeChanged);
      // 
      // aShelf
      // 
      this.aShelf.BackColor = System.Drawing.Color.Transparent;
      this.aShelf.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.aShelf.Location = new System.Drawing.Point(7, 7);
      this.aShelf.Margin = new System.Windows.Forms.Padding(4);
      this.aShelf.MinimumSize = new System.Drawing.Size(267, 157);
      this.aShelf.Name = "aShelf";
      this.aShelf.Size = new System.Drawing.Size(267, 157);
      this.aShelf.TabIndex = 0;
      // 
      // tabMap
      // 
      this.tabMap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(44)))), ((int)(((byte)(67)))));
      this.tabMap.Controls.Add(this.tableLayoutPanel1);
      this.tabMap.Location = new System.Drawing.Point(4, 34);
      this.tabMap.Name = "tabMap";
      this.tabMap.Padding = new System.Windows.Forms.Padding(3);
      this.tabMap.Size = new System.Drawing.Size(611, 698);
      this.tabMap.TabIndex = 1;
      this.tabMap.Text = "Map";
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.aMap, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 3);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel1.Size = new System.Drawing.Size(605, 692);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.lblSimConnectedMap);
      this.panel1.Controls.Add(this.label3);
      this.panel1.Controls.Add(this.label2);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Controls.Add(this.btGetAirport);
      this.panel1.Controls.Add(this.txEntry);
      this.panel1.Controls.Add(this.lblARR);
      this.panel1.Controls.Add(this.lblDEP);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel1.ForeColor = System.Drawing.Color.FloralWhite;
      this.panel1.Location = new System.Drawing.Point(3, 3);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(599, 44);
      this.panel1.TabIndex = 0;
      // 
      // lblSimConnectedMap
      // 
      this.lblSimConnectedMap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
      this.lblSimConnectedMap.Dock = System.Windows.Forms.DockStyle.Top;
      this.lblSimConnectedMap.Location = new System.Drawing.Point(0, 0);
      this.lblSimConnectedMap.Name = "lblSimConnectedMap";
      this.lblSimConnectedMap.Size = new System.Drawing.Size(599, 5);
      this.lblSimConnectedMap.TabIndex = 5;
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label3.Location = new System.Drawing.Point(340, 16);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(80, 17);
      this.label3.TabIndex = 4;
      this.label3.Text = "Search ICAO";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label2.Location = new System.Drawing.Point(127, 15);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(32, 17);
      this.label2.TabIndex = 4;
      this.label2.Text = "ARR";
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(0, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(31, 17);
      this.label1.TabIndex = 3;
      this.label1.Text = "DEP";
      // 
      // btGetAirport
      // 
      this.btGetAirport.BackColor = System.Drawing.Color.SteelBlue;
      this.btGetAirport.BackgroundImage = global::FShelf.Properties.Resources.airport_facility;
      this.btGetAirport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
      this.btGetAirport.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btGetAirport.Location = new System.Drawing.Point(545, 6);
      this.btGetAirport.Name = "btGetAirport";
      this.btGetAirport.Size = new System.Drawing.Size(50, 33);
      this.btGetAirport.TabIndex = 2;
      this.btGetAirport.UseVisualStyleBackColor = false;
      this.btGetAirport.Click += new System.EventHandler(this.btGetAirport_Click);
      // 
      // txEntry
      // 
      this.txEntry.BackColor = System.Drawing.Color.SteelBlue;
      this.txEntry.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
      this.txEntry.ForeColor = System.Drawing.Color.FloralWhite;
      this.txEntry.Location = new System.Drawing.Point(426, 6);
      this.txEntry.MaxLength = 5;
      this.txEntry.Name = "txEntry";
      this.txEntry.Size = new System.Drawing.Size(113, 33);
      this.txEntry.TabIndex = 1;
      this.txEntry.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.txEntry.WordWrap = false;
      this.txEntry.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txEntry_KeyPress);
      // 
      // lblARR
      // 
      this.lblARR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lblARR.Cursor = System.Windows.Forms.Cursors.Hand;
      this.lblARR.Location = new System.Drawing.Point(165, 6);
      this.lblARR.Name = "lblARR";
      this.lblARR.Size = new System.Drawing.Size(84, 33);
      this.lblARR.TabIndex = 0;
      this.lblARR.Text = "KORD";
      this.lblARR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.lblARR.Click += new System.EventHandler(this.lblARR_Click);
      // 
      // lblDEP
      // 
      this.lblDEP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lblDEP.Cursor = System.Windows.Forms.Cursors.Hand;
      this.lblDEP.Location = new System.Drawing.Point(37, 6);
      this.lblDEP.Name = "lblDEP";
      this.lblDEP.Size = new System.Drawing.Size(84, 33);
      this.lblDEP.TabIndex = 0;
      this.lblDEP.Text = "LSZH";
      this.lblDEP.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.lblDEP.Click += new System.EventHandler(this.lblDEP_Click);
      // 
      // aMap
      // 
      this.aMap.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(16)))));
      this.aMap.CausesValidation = false;
      this.aMap.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.aMap.ForeColor = System.Drawing.Color.LightYellow;
      this.aMap.Location = new System.Drawing.Point(3, 54);
      this.aMap.MapRange = bm98_Map.MapRange.Near;
      this.aMap.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.aMap.MinimumSize = new System.Drawing.Size(600, 640);
      this.aMap.Name = "aMap";
      this.aMap.ShowAirportRange = false;
      this.aMap.ShowAptMarks = false;
      this.aMap.ShowMapGrid = false;
      this.aMap.ShowNavaids = false;
      this.aMap.ShowTrackedAircraft = false;
      this.aMap.ShowVFRMarks = false;
      this.aMap.Size = new System.Drawing.Size(600, 640);
      this.aMap.TabIndex = 1;
      this.aMap.MapCenterChanged += new System.EventHandler<bm98_Map.MapEventArgs>(this.AMap_MapCenterChanged);
      this.aMap.MapRangeChanged += new System.EventHandler<bm98_Map.MapEventArgs>(this.AMap_MapRangeChanged);
      // 
      // tabMetar
      // 
      this.tabMetar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(44)))), ((int)(((byte)(67)))));
      this.tabMetar.Controls.Add(this.lblMetArr);
      this.tabMetar.Controls.Add(this.lblMetDep);
      this.tabMetar.Controls.Add(this.btMetAcft);
      this.tabMetar.Controls.Add(this.label17);
      this.tabMetar.Controls.Add(this.btMetClear);
      this.tabMetar.Controls.Add(this.btMetApt);
      this.tabMetar.Controls.Add(this.btMetArr);
      this.tabMetar.Controls.Add(this.btMetDep);
      this.tabMetar.Controls.Add(this.label14);
      this.tabMetar.Controls.Add(this.label12);
      this.tabMetar.Controls.Add(this.txMetApt);
      this.tabMetar.Controls.Add(this.label13);
      this.tabMetar.Controls.Add(this.rtbMetar);
      this.tabMetar.ForeColor = System.Drawing.Color.FloralWhite;
      this.tabMetar.Location = new System.Drawing.Point(4, 34);
      this.tabMetar.Name = "tabMetar";
      this.tabMetar.Size = new System.Drawing.Size(611, 698);
      this.tabMetar.TabIndex = 3;
      this.tabMetar.Text = "METAR";
      // 
      // lblMetArr
      // 
      this.lblMetArr.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lblMetArr.Cursor = System.Windows.Forms.Cursors.Hand;
      this.lblMetArr.Location = new System.Drawing.Point(89, 66);
      this.lblMetArr.Name = "lblMetArr";
      this.lblMetArr.Size = new System.Drawing.Size(113, 33);
      this.lblMetArr.TabIndex = 20;
      this.lblMetArr.Text = "LSZH";
      this.lblMetArr.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // lblMetDep
      // 
      this.lblMetDep.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
      this.lblMetDep.Cursor = System.Windows.Forms.Cursors.Hand;
      this.lblMetDep.Location = new System.Drawing.Point(89, 19);
      this.lblMetDep.Name = "lblMetDep";
      this.lblMetDep.Size = new System.Drawing.Size(113, 33);
      this.lblMetDep.TabIndex = 20;
      this.lblMetDep.Text = "LSZH";
      this.lblMetDep.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      // 
      // btMetAcft
      // 
      this.btMetAcft.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btMetAcft.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btMetAcft.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btMetAcft.Location = new System.Drawing.Point(208, 159);
      this.btMetAcft.Name = "btMetAcft";
      this.btMetAcft.Size = new System.Drawing.Size(84, 33);
      this.btMetAcft.TabIndex = 6;
      this.btMetAcft.Text = "Request";
      this.btMetAcft.UseVisualStyleBackColor = true;
      this.btMetAcft.Click += new System.EventHandler(this.btMetAcft_Click);
      // 
      // label17
      // 
      this.label17.AutoSize = true;
      this.label17.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label17.Location = new System.Drawing.Point(19, 165);
      this.label17.Name = "label17";
      this.label17.Size = new System.Drawing.Size(46, 20);
      this.label17.TabIndex = 19;
      this.label17.Text = "ACFT";
      // 
      // btMetClear
      // 
      this.btMetClear.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btMetClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btMetClear.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btMetClear.Location = new System.Drawing.Point(23, 667);
      this.btMetClear.Name = "btMetClear";
      this.btMetClear.Size = new System.Drawing.Size(84, 29);
      this.btMetClear.TabIndex = 7;
      this.btMetClear.Text = "Clear";
      this.btMetClear.UseVisualStyleBackColor = true;
      this.btMetClear.Click += new System.EventHandler(this.btMetClear_Click);
      // 
      // btMetApt
      // 
      this.btMetApt.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btMetApt.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btMetApt.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btMetApt.Location = new System.Drawing.Point(208, 113);
      this.btMetApt.Name = "btMetApt";
      this.btMetApt.Size = new System.Drawing.Size(84, 32);
      this.btMetApt.TabIndex = 5;
      this.btMetApt.Text = "Request";
      this.btMetApt.UseVisualStyleBackColor = true;
      this.btMetApt.Click += new System.EventHandler(this.btMetApt_Click);
      // 
      // btMetArr
      // 
      this.btMetArr.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btMetArr.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btMetArr.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btMetArr.Location = new System.Drawing.Point(208, 66);
      this.btMetArr.Name = "btMetArr";
      this.btMetArr.Size = new System.Drawing.Size(84, 33);
      this.btMetArr.TabIndex = 3;
      this.btMetArr.Text = "Request";
      this.btMetArr.UseVisualStyleBackColor = true;
      this.btMetArr.Click += new System.EventHandler(this.btMetArr_Click);
      // 
      // btMetDep
      // 
      this.btMetDep.Cursor = System.Windows.Forms.Cursors.Hand;
      this.btMetDep.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
      this.btMetDep.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btMetDep.Location = new System.Drawing.Point(208, 19);
      this.btMetDep.Name = "btMetDep";
      this.btMetDep.Size = new System.Drawing.Size(84, 33);
      this.btMetDep.TabIndex = 1;
      this.btMetDep.Text = "Request";
      this.btMetDep.UseVisualStyleBackColor = true;
      this.btMetDep.Click += new System.EventHandler(this.btMetDep_Click);
      // 
      // label14
      // 
      this.label14.AutoSize = true;
      this.label14.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label14.Location = new System.Drawing.Point(19, 120);
      this.label14.Name = "label14";
      this.label14.Size = new System.Drawing.Size(38, 20);
      this.label14.TabIndex = 14;
      this.label14.Text = "APT";
      // 
      // label12
      // 
      this.label12.AutoSize = true;
      this.label12.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label12.Location = new System.Drawing.Point(19, 74);
      this.label12.Name = "label12";
      this.label12.Size = new System.Drawing.Size(40, 20);
      this.label12.TabIndex = 14;
      this.label12.Text = "ARR";
      // 
      // txMetApt
      // 
      this.txMetApt.BackColor = System.Drawing.Color.SteelBlue;
      this.txMetApt.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
      this.txMetApt.ForeColor = System.Drawing.Color.FloralWhite;
      this.txMetApt.Location = new System.Drawing.Point(89, 113);
      this.txMetApt.MaxLength = 5;
      this.txMetApt.Name = "txMetApt";
      this.txMetApt.Size = new System.Drawing.Size(113, 33);
      this.txMetApt.TabIndex = 4;
      this.txMetApt.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.txMetApt.WordWrap = false;
      this.txMetApt.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txMetApt_KeyPress);
      // 
      // label13
      // 
      this.label13.AutoSize = true;
      this.label13.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label13.Location = new System.Drawing.Point(19, 27);
      this.label13.Name = "label13";
      this.label13.Size = new System.Drawing.Size(37, 20);
      this.label13.TabIndex = 11;
      this.label13.Text = "DEP";
      // 
      // rtbMetar
      // 
      this.rtbMetar.AcceptsTab = true;
      this.rtbMetar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.rtbMetar.BackColor = System.Drawing.Color.LightGray;
      this.rtbMetar.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.rtbMetar.ForeColor = System.Drawing.Color.Black;
      this.rtbMetar.Location = new System.Drawing.Point(23, 203);
      this.rtbMetar.Name = "rtbMetar";
      this.rtbMetar.ReadOnly = true;
      this.rtbMetar.Size = new System.Drawing.Size(558, 458);
      this.rtbMetar.TabIndex = 0;
      this.rtbMetar.Text = "";
      // 
      // tabPerf
      // 
      this.tabPerf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(44)))), ((int)(((byte)(67)))));
      this.tabPerf.Controls.Add(this.tableLayoutPanel3);
      this.tabPerf.ForeColor = System.Drawing.Color.FloralWhite;
      this.tabPerf.Location = new System.Drawing.Point(4, 34);
      this.tabPerf.Name = "tabPerf";
      this.tabPerf.Size = new System.Drawing.Size(611, 698);
      this.tabPerf.TabIndex = 4;
      this.tabPerf.Text = "Perf.";
      // 
      // tableLayoutPanel3
      // 
      this.tableLayoutPanel3.ColumnCount = 1;
      this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel3.Controls.Add(this.lblSimConnectedNotes, 0, 0);
      this.tableLayoutPanel3.Controls.Add(this.rtbPerf, 0, 2);
      this.tableLayoutPanel3.Controls.Add(this.flowLayoutPanel1, 0, 1);
      this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel3.Name = "tableLayoutPanel3";
      this.tableLayoutPanel3.RowCount = 3;
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 7F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
      this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel3.Size = new System.Drawing.Size(611, 698);
      this.tableLayoutPanel3.TabIndex = 1;
      // 
      // lblSimConnectedNotes
      // 
      this.lblSimConnectedNotes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
      this.lblSimConnectedNotes.Dock = System.Windows.Forms.DockStyle.Top;
      this.lblSimConnectedNotes.Location = new System.Drawing.Point(3, 0);
      this.lblSimConnectedNotes.Name = "lblSimConnectedNotes";
      this.lblSimConnectedNotes.Size = new System.Drawing.Size(605, 5);
      this.lblSimConnectedNotes.TabIndex = 6;
      // 
      // rtbPerf
      // 
      this.rtbPerf.BackColor = System.Drawing.Color.LightGray;
      this.rtbPerf.DetectUrls = false;
      this.rtbPerf.Dock = System.Windows.Forms.DockStyle.Fill;
      this.rtbPerf.Location = new System.Drawing.Point(3, 50);
      this.rtbPerf.Name = "rtbPerf";
      this.rtbPerf.ReadOnly = true;
      this.rtbPerf.Size = new System.Drawing.Size(605, 645);
      this.rtbPerf.TabIndex = 0;
      this.rtbPerf.TabStop = false;
      this.rtbPerf.Text = "";
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.Controls.Add(this.rbKLbs);
      this.flowLayoutPanel1.Controls.Add(this.rbKg);
      this.flowLayoutPanel1.Controls.Add(this.pictureBox1);
      this.flowLayoutPanel1.Controls.Add(this.btPerfRefresh);
      this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.flowLayoutPanel1.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 10);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(605, 34);
      this.flowLayoutPanel1.TabIndex = 1;
      // 
      // rbKLbs
      // 
      this.rbKLbs.AutoSize = true;
      this.rbKLbs.Location = new System.Drawing.Point(3, 3);
      this.rbKLbs.Name = "rbKLbs";
      this.rbKLbs.Size = new System.Drawing.Size(59, 24);
      this.rbKLbs.TabIndex = 0;
      this.rbKLbs.Text = "kLbs";
      this.rbKLbs.UseVisualStyleBackColor = true;
      this.rbKLbs.CheckedChanged += new System.EventHandler(this.rbKLbs_CheckedChanged);
      // 
      // rbKg
      // 
      this.rbKg.AutoSize = true;
      this.rbKg.Checked = true;
      this.rbKg.Location = new System.Drawing.Point(68, 3);
      this.rbKg.Name = "rbKg";
      this.rbKg.Size = new System.Drawing.Size(44, 24);
      this.rbKg.TabIndex = 1;
      this.rbKg.TabStop = true;
      this.rbKg.Text = "kg";
      this.rbKg.UseVisualStyleBackColor = true;
      this.rbKg.CheckedChanged += new System.EventHandler(this.rbKg_CheckedChanged);
      // 
      // pictureBox1
      // 
      this.pictureBox1.Location = new System.Drawing.Point(118, 3);
      this.pictureBox1.Name = "pictureBox1";
      this.pictureBox1.Size = new System.Drawing.Size(127, 22);
      this.pictureBox1.TabIndex = 2;
      this.pictureBox1.TabStop = false;
      // 
      // btPerfRefresh
      // 
      this.btPerfRefresh.ForeColor = System.Drawing.Color.Black;
      this.btPerfRefresh.Location = new System.Drawing.Point(251, 3);
      this.btPerfRefresh.Name = "btPerfRefresh";
      this.btPerfRefresh.Size = new System.Drawing.Size(87, 31);
      this.btPerfRefresh.TabIndex = 3;
      this.btPerfRefresh.Text = "Refresh";
      this.btPerfRefresh.UseVisualStyleBackColor = true;
      this.btPerfRefresh.Click += new System.EventHandler(this.btPerfRefresh_Click);
      // 
      // tabNotes
      // 
      this.tabNotes.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(44)))), ((int)(((byte)(67)))));
      this.tabNotes.Controls.Add(this.tableLayoutPanel2);
      this.tabNotes.ForeColor = System.Drawing.Color.FloralWhite;
      this.tabNotes.Location = new System.Drawing.Point(4, 34);
      this.tabNotes.Name = "tabNotes";
      this.tabNotes.Size = new System.Drawing.Size(611, 698);
      this.tabNotes.TabIndex = 5;
      this.tabNotes.Text = "Notes";
      // 
      // tableLayoutPanel2
      // 
      this.tableLayoutPanel2.ColumnCount = 1;
      this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel2.Controls.Add(this.rtbNotes, 0, 1);
      this.tableLayoutPanel2.Controls.Add(this.btNotesClear, 0, 0);
      this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel2.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel2.Name = "tableLayoutPanel2";
      this.tableLayoutPanel2.RowCount = 2;
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
      this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
      this.tableLayoutPanel2.Size = new System.Drawing.Size(611, 698);
      this.tableLayoutPanel2.TabIndex = 1;
      // 
      // rtbNotes
      // 
      this.rtbNotes.AcceptsTab = true;
      this.rtbNotes.BackColor = System.Drawing.Color.LightGray;
      this.rtbNotes.DetectUrls = false;
      this.rtbNotes.Dock = System.Windows.Forms.DockStyle.Fill;
      this.rtbNotes.Font = new System.Drawing.Font("Calibri", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.rtbNotes.HideSelection = false;
      this.rtbNotes.Location = new System.Drawing.Point(3, 43);
      this.rtbNotes.Name = "rtbNotes";
      this.rtbNotes.Size = new System.Drawing.Size(605, 652);
      this.rtbNotes.TabIndex = 0;
      this.rtbNotes.Text = "This is notepad text";
      // 
      // btNotesClear
      // 
      this.btNotesClear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.btNotesClear.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btNotesClear.ForeColor = System.Drawing.Color.Black;
      this.btNotesClear.Location = new System.Drawing.Point(521, 3);
      this.btNotesClear.Name = "btNotesClear";
      this.btNotesClear.Size = new System.Drawing.Size(87, 31);
      this.btNotesClear.TabIndex = 1;
      this.btNotesClear.Text = "Clear";
      this.btNotesClear.UseVisualStyleBackColor = true;
      this.btNotesClear.Click += new System.EventHandler(this.btNotesClear_Click);
      // 
      // tabConfig
      // 
      this.tabConfig.BackColor = System.Drawing.Color.Gainsboro;
      this.tabConfig.Controls.Add(this.cbxAcftTrack);
      this.tabConfig.Controls.Add(this.cbxAcftRange);
      this.tabConfig.Controls.Add(this.label16);
      this.tabConfig.Controls.Add(this.comboCfgRunwayLength);
      this.tabConfig.Controls.Add(this.label15);
      this.tabConfig.Controls.Add(this.label11);
      this.tabConfig.Controls.Add(this.label10);
      this.tabConfig.Controls.Add(this.cbxIFRwaypoints);
      this.tabConfig.Controls.Add(this.cbxCfgPrettyMetar);
      this.tabConfig.Controls.Add(this.btCfgSelFolder);
      this.tabConfig.Controls.Add(this.txCfgShelfFolder);
      this.tabConfig.Controls.Add(this.label8);
      this.tabConfig.Controls.Add(this.label7);
      this.tabConfig.Controls.Add(this.label9);
      this.tabConfig.Controls.Add(this.lblCfgArr);
      this.tabConfig.Controls.Add(this.lblCfgDep);
      this.tabConfig.Controls.Add(this.label4);
      this.tabConfig.Controls.Add(this.label5);
      this.tabConfig.Controls.Add(this.label6);
      this.tabConfig.Controls.Add(this.txCfgArr);
      this.tabConfig.Controls.Add(this.txCfgDep);
      this.tabConfig.Location = new System.Drawing.Point(4, 34);
      this.tabConfig.Name = "tabConfig";
      this.tabConfig.Padding = new System.Windows.Forms.Padding(3);
      this.tabConfig.Size = new System.Drawing.Size(611, 698);
      this.tabConfig.TabIndex = 2;
      this.tabConfig.Text = "Config.";
      // 
      // cbxAcftTrack
      // 
      this.cbxAcftTrack.AutoSize = true;
      this.cbxAcftTrack.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbxAcftTrack.Location = new System.Drawing.Point(23, 370);
      this.cbxAcftTrack.Name = "cbxAcftTrack";
      this.cbxAcftTrack.Size = new System.Drawing.Size(217, 25);
      this.cbxAcftTrack.TabIndex = 19;
      this.cbxAcftTrack.Text = "Show Aircraft tracking dots";
      this.cbxAcftTrack.UseVisualStyleBackColor = true;
      // 
      // cbxAcftRange
      // 
      this.cbxAcftRange.AutoSize = true;
      this.cbxAcftRange.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbxAcftRange.Location = new System.Drawing.Point(23, 339);
      this.cbxAcftRange.Name = "cbxAcftRange";
      this.cbxAcftRange.Size = new System.Drawing.Size(199, 25);
      this.cbxAcftRange.TabIndex = 18;
      this.cbxAcftRange.Text = "Show Aircraft range arcs";
      this.cbxAcftRange.UseVisualStyleBackColor = true;
      // 
      // label16
      // 
      this.label16.AutoSize = true;
      this.label16.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label16.Location = new System.Drawing.Point(19, 276);
      this.label16.Name = "label16";
      this.label16.Size = new System.Drawing.Size(258, 21);
      this.label16.TabIndex = 17;
      this.label16.Text = "Min. Runway length for Alt. Airports";
      // 
      // comboCfgRunwayLength
      // 
      this.comboCfgRunwayLength.FormattingEnabled = true;
      this.comboCfgRunwayLength.Location = new System.Drawing.Point(23, 300);
      this.comboCfgRunwayLength.Name = "comboCfgRunwayLength";
      this.comboCfgRunwayLength.Size = new System.Drawing.Size(390, 33);
      this.comboCfgRunwayLength.TabIndex = 16;
      // 
      // label15
      // 
      this.label15.AutoSize = true;
      this.label15.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label15.Location = new System.Drawing.Point(8, 542);
      this.label15.Name = "label15";
      this.label15.Size = new System.Drawing.Size(48, 21);
      this.label15.TabIndex = 15;
      this.label15.Text = "Shelf:";
      // 
      // label11
      // 
      this.label11.AutoSize = true;
      this.label11.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label11.Location = new System.Drawing.Point(8, 456);
      this.label11.Name = "label11";
      this.label11.Size = new System.Drawing.Size(62, 21);
      this.label11.TabIndex = 15;
      this.label11.Text = "METAR:";
      // 
      // label10
      // 
      this.label10.AutoSize = true;
      this.label10.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label10.Location = new System.Drawing.Point(8, 224);
      this.label10.Name = "label10";
      this.label10.Size = new System.Drawing.Size(131, 21);
      this.label10.TabIndex = 12;
      this.label10.Text = "Map Decorations:";
      // 
      // cbxIFRwaypoints
      // 
      this.cbxIFRwaypoints.AutoSize = true;
      this.cbxIFRwaypoints.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbxIFRwaypoints.Location = new System.Drawing.Point(23, 248);
      this.cbxIFRwaypoints.Name = "cbxIFRwaypoints";
      this.cbxIFRwaypoints.Size = new System.Drawing.Size(207, 25);
      this.cbxIFRwaypoints.TabIndex = 11;
      this.cbxIFRwaypoints.Text = "Show Airport IFR Navaids";
      this.cbxIFRwaypoints.UseVisualStyleBackColor = true;
      // 
      // cbxCfgPrettyMetar
      // 
      this.cbxCfgPrettyMetar.AutoSize = true;
      this.cbxCfgPrettyMetar.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbxCfgPrettyMetar.Location = new System.Drawing.Point(23, 486);
      this.cbxCfgPrettyMetar.Name = "cbxCfgPrettyMetar";
      this.cbxCfgPrettyMetar.Size = new System.Drawing.Size(186, 25);
      this.cbxCfgPrettyMetar.TabIndex = 9;
      this.cbxCfgPrettyMetar.Text = "Add  \'readable\' METAR";
      this.cbxCfgPrettyMetar.UseVisualStyleBackColor = true;
      // 
      // btCfgSelFolder
      // 
      this.btCfgSelFolder.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btCfgSelFolder.Location = new System.Drawing.Point(534, 590);
      this.btCfgSelFolder.Name = "btCfgSelFolder";
      this.btCfgSelFolder.Size = new System.Drawing.Size(40, 26);
      this.btCfgSelFolder.TabIndex = 2;
      this.btCfgSelFolder.Text = "...";
      this.btCfgSelFolder.UseVisualStyleBackColor = true;
      this.btCfgSelFolder.Click += new System.EventHandler(this.btCfgSelFolder_Click);
      // 
      // txCfgShelfFolder
      // 
      this.txCfgShelfFolder.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.txCfgShelfFolder.Location = new System.Drawing.Point(7, 589);
      this.txCfgShelfFolder.Name = "txCfgShelfFolder";
      this.txCfgShelfFolder.ReadOnly = true;
      this.txCfgShelfFolder.Size = new System.Drawing.Size(521, 27);
      this.txCfgShelfFolder.TabIndex = 3;
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label8.Location = new System.Drawing.Point(8, 18);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(258, 21);
      this.label8.TabIndex = 7;
      this.label8.Text = "Map Departure and Arrival Airports:";
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label7.Location = new System.Drawing.Point(13, 569);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(159, 21);
      this.label7.TabIndex = 7;
      this.label7.Text = "Shelf Folder Location:";
      // 
      // label9
      // 
      this.label9.AutoSize = true;
      this.label9.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label9.Location = new System.Drawing.Point(14, 176);
      this.label9.Name = "label9";
      this.label9.Size = new System.Drawing.Size(323, 17);
      this.label9.TabIndex = 7;
      this.label9.Text = "Enter the ICAO Code and hit Return to find the Airport";
      // 
      // lblCfgArr
      // 
      this.lblCfgArr.AutoSize = true;
      this.lblCfgArr.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblCfgArr.Location = new System.Drawing.Point(208, 136);
      this.lblCfgArr.Name = "lblCfgArr";
      this.lblCfgArr.Size = new System.Drawing.Size(28, 17);
      this.lblCfgArr.TabIndex = 7;
      this.lblCfgArr.Text = "n.a.";
      // 
      // lblCfgDep
      // 
      this.lblCfgDep.AutoSize = true;
      this.lblCfgDep.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblCfgDep.Location = new System.Drawing.Point(208, 89);
      this.lblCfgDep.Name = "lblCfgDep";
      this.lblCfgDep.Size = new System.Drawing.Size(28, 17);
      this.lblCfgDep.TabIndex = 7;
      this.lblCfgDep.Text = "n.a.";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label4.Location = new System.Drawing.Point(86, 50);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(98, 17);
      this.label4.TabIndex = 7;
      this.label4.Text = "Search by ICAO";
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label5.Location = new System.Drawing.Point(19, 134);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(40, 20);
      this.label5.TabIndex = 8;
      this.label5.Text = "ARR";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label6.Location = new System.Drawing.Point(19, 87);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(37, 20);
      this.label6.TabIndex = 6;
      this.label6.Text = "DEP";
      // 
      // txCfgArr
      // 
      this.txCfgArr.BackColor = System.Drawing.Color.SteelBlue;
      this.txCfgArr.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
      this.txCfgArr.ForeColor = System.Drawing.Color.FloralWhite;
      this.txCfgArr.Location = new System.Drawing.Point(89, 127);
      this.txCfgArr.MaxLength = 5;
      this.txCfgArr.Name = "txCfgArr";
      this.txCfgArr.Size = new System.Drawing.Size(113, 33);
      this.txCfgArr.TabIndex = 1;
      this.txCfgArr.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.txCfgArr.WordWrap = false;
      this.txCfgArr.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txCfgArr_KeyPress);
      // 
      // txCfgDep
      // 
      this.txCfgDep.BackColor = System.Drawing.Color.SteelBlue;
      this.txCfgDep.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
      this.txCfgDep.ForeColor = System.Drawing.Color.FloralWhite;
      this.txCfgDep.Location = new System.Drawing.Point(89, 80);
      this.txCfgDep.MaxLength = 5;
      this.txCfgDep.Name = "txCfgDep";
      this.txCfgDep.Size = new System.Drawing.Size(113, 33);
      this.txCfgDep.TabIndex = 0;
      this.txCfgDep.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
      this.txCfgDep.WordWrap = false;
      this.txCfgDep.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txCfgDep_KeyPress);
      // 
      // timer1
      // 
      this.timer1.Interval = 500;
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // FBD
      // 
      this.FBD.Description = "Select Flight Bag Folder";
      // 
      // frmShelf
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(44)))), ((int)(((byte)(67)))));
      this.ClientSize = new System.Drawing.Size(619, 736);
      this.Controls.Add(this.tab);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(635, 775);
      this.Name = "frmShelf";
      this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "HudBar - Flight Bag";
      this.Activated += new System.EventHandler(this.frmShelf_Activated);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmShelf_FormClosing);
      this.Load += new System.EventHandler(this.frmShelf_Load);
      this.LocationChanged += new System.EventHandler(this.frmShelf_LocationChanged);
      this.tab.ResumeLayout(false);
      this.tabShelf.ResumeLayout(false);
      this.tabMap.ResumeLayout(false);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.tabMetar.ResumeLayout(false);
      this.tabMetar.PerformLayout();
      this.tabPerf.ResumeLayout(false);
      this.tableLayoutPanel3.ResumeLayout(false);
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
      this.tabNotes.ResumeLayout(false);
      this.tableLayoutPanel2.ResumeLayout(false);
      this.tabConfig.ResumeLayout(false);
      this.tabConfig.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private bm98_Album.UC_Album aShelf;
    private System.Windows.Forms.TabControl tab;
    private System.Windows.Forms.TabPage tabShelf;
    private System.Windows.Forms.TabPage tabMap;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btGetAirport;
    private System.Windows.Forms.TextBox txEntry;
    private System.Windows.Forms.Label lblARR;
    private System.Windows.Forms.Label lblDEP;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label lblSimConnectedMap;
    private System.Windows.Forms.Timer timer1;
    private System.Windows.Forms.TabPage tabConfig;
    private System.Windows.Forms.Button btCfgSelFolder;
    private System.Windows.Forms.TextBox txCfgShelfFolder;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox txCfgArr;
    private System.Windows.Forms.TextBox txCfgDep;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.FolderBrowserDialog FBD;
    private System.Windows.Forms.Label lblCfgArr;
    private System.Windows.Forms.Label lblCfgDep;
    private System.Windows.Forms.TabPage tabMetar;
    private System.Windows.Forms.Button btMetApt;
    private System.Windows.Forms.Button btMetArr;
    private System.Windows.Forms.Button btMetDep;
    private System.Windows.Forms.Label label14;
    private System.Windows.Forms.Label label12;
    private System.Windows.Forms.TextBox txMetApt;
    private System.Windows.Forms.Label label13;
    private System.Windows.Forms.RichTextBox rtbMetar;
    private System.Windows.Forms.Button btMetClear;
    private System.Windows.Forms.Button btMetAcft;
    private System.Windows.Forms.Label label17;
    private System.Windows.Forms.CheckBox cbxCfgPrettyMetar;
    private System.Windows.Forms.Label lblMetArr;
    private System.Windows.Forms.Label lblMetDep;
    private System.Windows.Forms.Label label10;
    private System.Windows.Forms.CheckBox cbxIFRwaypoints;
    private bm98_Map.UC_Map aMap;
    private System.Windows.Forms.Label label15;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.Label label16;
    private System.Windows.Forms.ComboBox comboCfgRunwayLength;
    private System.Windows.Forms.CheckBox cbxAcftTrack;
    private System.Windows.Forms.CheckBox cbxAcftRange;
        private System.Windows.Forms.TabPage tabPerf;
        private System.Windows.Forms.TabPage tabNotes;
        private System.Windows.Forms.RichTextBox rtbNotes;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button btNotesClear;
        private System.Windows.Forms.RichTextBox rtbPerf;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.RadioButton rbKLbs;
        private System.Windows.Forms.RadioButton rbKg;
        private System.Windows.Forms.Label lblSimConnectedNotes;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btPerfRefresh;
    }
}
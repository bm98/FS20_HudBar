
namespace bm98_Checklist
{
  partial class frmConfig
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
      this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
      this.tabCfg = new System.Windows.Forms.TabControl();
      this.tPage1 = new System.Windows.Forms.TabPage();
      this.panel1 = new System.Windows.Forms.Panel();
      this.label2 = new System.Windows.Forms.Label();
      this.cbxCheckSize = new System.Windows.Forms.ComboBox();
      this.label1 = new System.Windows.Forms.Label();
      this.cbxCheckColor = new System.Windows.Forms.ComboBox();
      this.btFont = new System.Windows.Forms.Button();
      this.chkOrientation = new System.Windows.Forms.CheckBox();
      this.btAdd = new System.Windows.Forms.Button();
      this.btCancel = new System.Windows.Forms.Button();
      this.btAccept = new System.Windows.Forms.Button();
      this.fntDlg = new System.Windows.Forms.FontDialog();
      this.tableLayoutPanel1.SuspendLayout();
      this.tabCfg.SuspendLayout();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tableLayoutPanel1
      // 
      this.tableLayoutPanel1.ColumnCount = 1;
      this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.Controls.Add(this.tabCfg, 0, 0);
      this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 1);
      this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
      this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.tableLayoutPanel1.Name = "tableLayoutPanel1";
      this.tableLayoutPanel1.RowCount = 2;
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 80F));
      this.tableLayoutPanel1.Size = new System.Drawing.Size(953, 779);
      this.tableLayoutPanel1.TabIndex = 0;
      // 
      // tabCfg
      // 
      this.tabCfg.Controls.Add(this.tPage1);
      this.tabCfg.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabCfg.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.tabCfg.HotTrack = true;
      this.tabCfg.Location = new System.Drawing.Point(3, 4);
      this.tabCfg.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.tabCfg.Multiline = true;
      this.tabCfg.Name = "tabCfg";
      this.tabCfg.SelectedIndex = 0;
      this.tabCfg.Size = new System.Drawing.Size(947, 691);
      this.tabCfg.TabIndex = 0;
      // 
      // tPage1
      // 
      this.tPage1.AutoScroll = true;
      this.tPage1.Location = new System.Drawing.Point(4, 26);
      this.tPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.tPage1.Name = "tPage1";
      this.tPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.tPage1.Size = new System.Drawing.Size(939, 661);
      this.tPage1.TabIndex = 0;
      this.tPage1.Text = "Check 1";
      this.tPage1.UseVisualStyleBackColor = true;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.label2);
      this.panel1.Controls.Add(this.cbxCheckSize);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Controls.Add(this.cbxCheckColor);
      this.panel1.Controls.Add(this.btFont);
      this.panel1.Controls.Add(this.chkOrientation);
      this.panel1.Controls.Add(this.btAdd);
      this.panel1.Controls.Add(this.btCancel);
      this.panel1.Controls.Add(this.btAccept);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.panel1.Location = new System.Drawing.Point(3, 702);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(947, 74);
      this.panel1.TabIndex = 1;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(343, 13);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(72, 17);
      this.label2.TabIndex = 9;
      this.label2.Text = "Check Size:";
      // 
      // cbxCheckSize
      // 
      this.cbxCheckSize.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbxCheckSize.FormattingEnabled = true;
      this.cbxCheckSize.Location = new System.Drawing.Point(430, 10);
      this.cbxCheckSize.Name = "cbxCheckSize";
      this.cbxCheckSize.Size = new System.Drawing.Size(93, 25);
      this.cbxCheckSize.TabIndex = 8;
      this.cbxCheckSize.SelectedIndexChanged += new System.EventHandler(this.cbxCheckSize_SelectedIndexChanged);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(157, 13);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(81, 17);
      this.label1.TabIndex = 7;
      this.label1.Text = "Check Color:";
      // 
      // cbxCheckColor
      // 
      this.cbxCheckColor.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.cbxCheckColor.FormattingEnabled = true;
      this.cbxCheckColor.Location = new System.Drawing.Point(244, 10);
      this.cbxCheckColor.Name = "cbxCheckColor";
      this.cbxCheckColor.Size = new System.Drawing.Size(93, 25);
      this.cbxCheckColor.TabIndex = 6;
      // 
      // btFont
      // 
      this.btFont.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btFont.Location = new System.Drawing.Point(552, 39);
      this.btFont.Name = "btFont";
      this.btFont.Size = new System.Drawing.Size(119, 24);
      this.btFont.TabIndex = 5;
      this.btFont.Text = "Font...";
      this.btFont.UseVisualStyleBackColor = true;
      this.btFont.Click += new System.EventHandler(this.btFont_Click);
      // 
      // chkOrientation
      // 
      this.chkOrientation.AutoSize = true;
      this.chkOrientation.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
      this.chkOrientation.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.chkOrientation.Location = new System.Drawing.Point(552, 12);
      this.chkOrientation.Name = "chkOrientation";
      this.chkOrientation.Size = new System.Drawing.Size(119, 21);
      this.chkOrientation.TabIndex = 4;
      this.chkOrientation.Text = "Horizontal Box";
      this.chkOrientation.UseVisualStyleBackColor = true;
      // 
      // btAdd
      // 
      this.btAdd.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btAdd.Location = new System.Drawing.Point(9, 16);
      this.btAdd.Name = "btAdd";
      this.btAdd.Size = new System.Drawing.Size(101, 44);
      this.btAdd.TabIndex = 3;
      this.btAdd.Text = "Add a new Checklist";
      this.btAdd.UseVisualStyleBackColor = true;
      this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
      // 
      // btCancel
      // 
      this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btCancel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btCancel.Location = new System.Drawing.Point(852, 39);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(86, 25);
      this.btCancel.TabIndex = 2;
      this.btCancel.Text = "Cancel";
      this.btCancel.UseVisualStyleBackColor = true;
      this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
      // 
      // btAccept
      // 
      this.btAccept.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.btAccept.Location = new System.Drawing.Point(742, 39);
      this.btAccept.Name = "btAccept";
      this.btAccept.Size = new System.Drawing.Size(86, 25);
      this.btAccept.TabIndex = 1;
      this.btAccept.Text = "Accept";
      this.btAccept.UseVisualStyleBackColor = true;
      this.btAccept.Click += new System.EventHandler(this.btAccept_Click);
      // 
      // fntDlg
      // 
      this.fntDlg.AllowScriptChange = false;
      this.fntDlg.AllowVectorFonts = false;
      this.fntDlg.AllowVerticalFonts = false;
      this.fntDlg.FontMustExist = true;
      this.fntDlg.ScriptsOnly = true;
      this.fntDlg.ShowEffects = false;
      // 
      // frmConfig
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.CancelButton = this.btCancel;
      this.ClientSize = new System.Drawing.Size(953, 779);
      this.Controls.Add(this.tableLayoutPanel1);
      this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
      this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
      this.Name = "frmConfig";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Configure Checklists";
      this.Load += new System.EventHandler(this.frmConfig_Load);
      this.tableLayoutPanel1.ResumeLayout(false);
      this.tabCfg.ResumeLayout(false);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.TabControl tabCfg;
    private System.Windows.Forms.TabPage tPage1;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btCancel;
    private System.Windows.Forms.Button btAccept;
    private System.Windows.Forms.Button btAdd;
    private System.Windows.Forms.CheckBox chkOrientation;
    private System.Windows.Forms.Button btFont;
    private System.Windows.Forms.FontDialog fntDlg;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ComboBox cbxCheckColor;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.ComboBox cbxCheckSize;
  }
}
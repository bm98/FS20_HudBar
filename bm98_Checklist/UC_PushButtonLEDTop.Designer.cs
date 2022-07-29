
namespace bm98_Checklist
{
  partial class UC_PushButtonLEDTop
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
      if ( disposing && ( components != null ) ) {
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
      this.lblText = new System.Windows.Forms.Label();
      this.SuspendLayout();
      // 
      // lblText
      // 
      this.lblText.Cursor = System.Windows.Forms.Cursors.Hand;
      this.lblText.Location = new System.Drawing.Point(15, 31);
      this.lblText.Name = "lblText";
      this.lblText.Size = new System.Drawing.Size(54, 22);
      this.lblText.TabIndex = 3;
      this.lblText.Text = "TEST";
      this.lblText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
      this.lblText.ClientSizeChanged += new System.EventHandler(this.lblText_ClientSizeChanged);
      this.lblText.Click += new System.EventHandler(this.lblText_Click);
      this.lblText.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblText_MouseDown);
      this.lblText.MouseEnter += new System.EventHandler(this.lblText_MouseEnter);
      this.lblText.MouseLeave += new System.EventHandler(this.lblText_MouseLeave);
      this.lblText.MouseUp += new System.Windows.Forms.MouseEventHandler(this.lblText_MouseUp);
      // 
      // UC_PushButtonLEDTop
      // 
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
      this.BackColor = System.Drawing.Color.Transparent;
      this.BackgroundImage = global::bm98_Checklist.Properties.Resources.button_Rect_LEDstripe_off;
      this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
      this.Controls.Add(this.lblText);
      this.DoubleBuffered = true;
      this.Name = "UC_PushButtonLEDTop";
      this.Size = new System.Drawing.Size(128, 64);
      this.Click += new System.EventHandler(this.lblText_Click);
      this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lblText_MouseDown);
      this.MouseEnter += new System.EventHandler(this.lblText_MouseEnter);
      this.MouseLeave += new System.EventHandler(this.lblText_MouseLeave);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Label lblText;
  }
}

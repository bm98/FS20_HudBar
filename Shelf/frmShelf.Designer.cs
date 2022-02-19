
namespace FS20_HudBar.Shelf
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
      if ( disposing && ( components != null ) ) {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmShelf));
      this.aShelf = new bm98_Album.UC_Album();
      this.SuspendLayout();
      // 
      // aShelf
      // 
      this.aShelf.BackColor = System.Drawing.Color.Transparent;
      this.aShelf.Dock = System.Windows.Forms.DockStyle.Fill;
      this.aShelf.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.aShelf.Location = new System.Drawing.Point(0, 0);
      this.aShelf.Margin = new System.Windows.Forms.Padding(4);
      this.aShelf.MinimumSize = new System.Drawing.Size(267, 157);
      this.aShelf.Name = "aShelf";
      this.aShelf.Size = new System.Drawing.Size(284, 171);
      this.aShelf.TabIndex = 0;
      // 
      // frmShelf
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(48)))));
      this.ClientSize = new System.Drawing.Size(284, 171);
      this.Controls.Add(this.aShelf);
      this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.MinimumSize = new System.Drawing.Size(300, 210);
      this.Name = "frmShelf";
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "HudBar - Flight Bag";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmShelf_FormClosing);
      this.Load += new System.EventHandler(this.frmShelf_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private bm98_Album.UC_Album aShelf;
  }
}
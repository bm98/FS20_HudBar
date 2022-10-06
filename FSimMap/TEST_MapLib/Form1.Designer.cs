
namespace TEST_MapLib
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
      this.btGetOne = new System.Windows.Forms.Button();
      this.RTB = new System.Windows.Forms.RichTextBox();
      this.SuspendLayout();
      // 
      // btGetOne
      // 
      this.btGetOne.Location = new System.Drawing.Point(23, 26);
      this.btGetOne.Name = "btGetOne";
      this.btGetOne.Size = new System.Drawing.Size(104, 46);
      this.btGetOne.TabIndex = 0;
      this.btGetOne.Text = "GetOneTile";
      this.btGetOne.UseVisualStyleBackColor = true;
      this.btGetOne.Click += new System.EventHandler(this.btGetOne_Click);
      // 
      // RTB
      // 
      this.RTB.Location = new System.Drawing.Point(169, 30);
      this.RTB.Name = "RTB";
      this.RTB.Size = new System.Drawing.Size(596, 266);
      this.RTB.TabIndex = 1;
      this.RTB.Text = "";
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(800, 450);
      this.Controls.Add(this.RTB);
      this.Controls.Add(this.btGetOne);
      this.Name = "Form1";
      this.Text = "Form1";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btGetOne;
    private System.Windows.Forms.RichTextBox RTB;
  }
}


﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

using PdfiumViewer;

namespace bm98_Album
{
  /// <summary>
  /// An Image Display Control with Zoom and Drag,
  /// includes a Bookshelf to choose the image from
  /// </summary>
  public partial class UC_Album : UserControl
  {
    // mark the temp file with this (start of filename)
    private const string c_tmpMark = "$§$";

    // Viewport Controller
    private ViewportZ _VPC = new ViewportZ( );
    // The loaded image to show
    private Image _pic = null;

    // Left Mouse Button State vars
    private bool _mouseDown = false;
    private bool _mouseMoved = false;

    private string _shelfFolder = "";       // currently used folder for Image files
    private string _shelfFilename = "";     // currently shown image filename
    private List<string> _shelfList = new List<string>( ); // current list of avialable Images in the Shelf

    private readonly Color c_BackBwDark = Color.FromArgb( 46, 69, 97 );
    //private readonly Color c_BackBwMid = Color.FromArgb( 48, 48, 48 ); // d-grey
    private readonly Color c_BackBwMid = Color.DarkSlateBlue;
    //    private readonly Color c_BackBwBright = Color.FromArgb(64,64,64);
    private readonly Color c_BackBwBright = Color.FromArgb( 64, 81, 102 );
    // Image Frame
    private Rectangle _frame = new Rectangle( );
    private readonly Pen _framePen = new Pen( Brushes.LightSteelBlue, 5 );
    // fallback brush
    private Brush _backBrush;

    // PDF Viewer size and placement
    private int _pdvTopBorder = 4; // how much to shift down
    private int _pdvLeftBorder = 5; // how much to shift left
    private int _pdvHeightMargin = 0; // how much to subtract from Height
    private int _pdvWidthMargin = 0; // how much to subtract from Width

    /// <summary>
    /// Event triggered when the Shelf button was clicked
    /// </summary>
    [Description( "Shelf Button was clicked" ), Category( "Action" )]
    public event EventHandler<EventArgs> ShelfClicked;
    private void OnShelfClicked( )
    {
      ShelfClicked?.Invoke( this, new EventArgs( ) );
    }

    /// <summary>
    /// Get; True if the shelf is visible
    /// </summary>
    public bool ShelfVisible => flp.Visible;

    /// <summary>
    /// The shown filename 
    /// </summary>
    public string ImageFilename => _shelfFilename;

    /// <summary>
    /// The folder in use for shelf documents
    /// </summary>
    public string ShelfFolder => _shelfFolder;

    /// <summary>
    /// Load and display the document
    /// NOTE: this will never fail, but ignore the offending file
    /// </summary>
    /// <param name="filename">Filename of the Image to display</param>
    public void SetDocument( string filename )
    {
      // this shall never fail - could be an invalid image..
      try {
        if (_pic != null) {
          _pic.Dispose( );
          _pic = null;
        }
        if (pdfV.Document != null) {
          pdfV.Document.Dispose( );
        }
        // remove temps here, else a file might be locked while viewing it
        CleanShelf( );

        // make a copy to be able to replace it while shown
        var tmpName = Path.GetFileName( filename );
        tmpName = Path.Combine( Path.GetDirectoryName( filename ), c_tmpMark + tmpName );
        File.Copy( filename, tmpName, true );

        if (Path.GetExtension( tmpName ).ToLowerInvariant( ) == ".pdf") {
          pdfV.Visible = true;
          pdfV.Document = PdfDocument.Load( tmpName );
        }
        else {
          pdfV.Visible = false;
          _pic = Image.FromFile( tmpName );
          var gu = GraphicsUnit.Pixel;
          _VPC.SetImageSize( _pic.GetBounds( ref gu ).Size );
        }
        _shelfFilename = filename;
      }
      catch {
        ; // DEBUG STOP;  we just ignore invalid image loading
      }
      this.Invalidate( this.ClientRectangle );
    }

    /// <summary>
    /// Set the foldername for the Bookshelf directory
    /// Throws: ArgumentException if the directory does not exist
    /// </summary>
    /// <param name="foldername">A Directorypath</param>
    public void SetShelfFolder( string foldername )
    {
      if (!Directory.Exists( foldername )) throw new ArgumentException( "Foldername does not exist." );

      _shelfFolder = foldername;

      // changed the folder while the shelf is visible..
      if (ShelfVisible) {
        LoadShelf( _shelfFolder, true ); // reload
      }
    }

    /// <summary>
    /// clean from tempfiles 
    /// </summary>
    public void CleanShelf( )
    {
      // get the tmp file list
      var files = Directory.EnumerateFiles( _shelfFolder, $"{c_tmpMark}*.*", SearchOption.AllDirectories );
      foreach (var file in files) {
        // shall never fail
        try { File.Delete( file ); } catch { }
      }
    }

    /// <summary>
    /// cTor:
    /// </summary>
    public UC_Album( )
    {
      InitializeComponent( );

      // Attach Mousewheel for Zoom
      this.MouseWheel += UC_Album_MouseWheel;

      // Prep the Flow Panel
      flp.BackColor = Color.Transparent;
      flp.ForeColor = Color.Lavender;
      flp.Visible = false;
      flp.AutoSize = true;

      _pdvTopBorder = picShelf.Height + 8;
      _pdvHeightMargin = picShelf.Height + 8 + 5; // top + bottom margin
      _pdvLeftBorder = 5;
      _pdvWidthMargin = 2 * _pdvLeftBorder; // left + right margin
      pdfV.Visible = false;
      pdfV.ShowBookmarks = false;
      pdfV.ShowToolbar = false;
      pdfV.Top = _pdvTopBorder;
      pdfV.ZoomMode = PdfViewerZoomMode.FitWidth;
      pdfV.Renderer.BackColor = this.BackColor;
      pdfV.Left = 5;

      _frame = this.ClientRectangle;
      _frame.Inflate( -2, -2 );

      // get the Zoom buttons in front
      picPlus.BringToFront( );
      picMinus.BringToFront( );

      _backBrush = new SolidBrush( this.BackColor );
    }

    // Capture BackColor changes to update the fallback Brush
    private void UC_Album_BackColorChanged( object sender, EventArgs e )
    {
      _backBrush.Dispose( );
      _backBrush = new SolidBrush( this.BackColor );
    }

    private void UC_Album_Enter( object sender, EventArgs e )
    {

    }

    private void UC_Album_Leave( object sender, EventArgs e )
    {

    }

    // Capture Left Mouse Button down for draging,
    // Right Button will Reset the Zoom
    private void UC_Album_MouseDown( object sender, MouseEventArgs e )
    {
      if (_pic == null) return;

      if (e.Button == MouseButtons.Left) {
        _mouseDown = true;
        _mouseMoved = false;
        this.Cursor = Cursors.NoMove2D;
        _VPC.DragStart( e.Location );
      }
      else if (e.Button == MouseButtons.Right) {
        _VPC.ZoomReset( );
        this.Invalidate( this.ClientRectangle );
      }
    }

    // Capture Mouse Move and Drag the image
    private void UC_Album_MouseMove( object sender, MouseEventArgs e )
    {
      if (_pic == null) return;

      if (_mouseDown && (e.Button == MouseButtons.Left)) {
        if (!_mouseMoved) {
          _mouseMoved = true;
        }
        _VPC.Drag( e.Location );
        this.Invalidate( this.ClientRectangle );
      }
    }

    // Capture Mouse Up and stop draging
    private void UC_Album_MouseUp( object sender, MouseEventArgs e )
    {
      if (_pic == null) return;

      if (_mouseDown) {
        this.Cursor = Cursors.Default;
        if (!_mouseMoved) {
          // not yet moved, handle Non Move Left Click
          //  if ( LeftSide( this.ClientRectangle, e.Location ) ) _VPC.ZoomOut( );
          //  else _VPC.ZoomIn( );
        }
        else {
          _VPC.DragStop( e.Location );
        }
        this.Invalidate( this.ClientRectangle );
      }
      // in any case reset state
      _mouseDown = false;
      _mouseMoved = false;
    }

    // Capture Mouse Wheel for Zoom
    private void UC_Album_MouseWheel( object sender, MouseEventArgs e )
    {
      if (pdfV.Visible) {
        if (e.Delta < 0) {
          pdfV.Renderer.ZoomOut( );
        }
        else {
          pdfV.Renderer.ZoomIn( );
        }
      }
      else if (_pic != null) {
        if (e.Delta < 0) {
          _VPC.ZoomOut( );
        }
        else {
          _VPC.ZoomIn( );
        }
        this.Invalidate( this.ClientRectangle );
      }
    }

    // Capture Zoom + Icon
    private void picPlus_Click( object sender, EventArgs e )
    {
      if (pdfV.Visible) {
        pdfV.Renderer.ZoomIn( );
      }
      else if (_pic != null) {
        _VPC.ZoomIn( );
        this.Invalidate( this.ClientRectangle );
      }
    }

    // Capture Zoom - Icon
    private void picMinus_Click( object sender, EventArgs e )
    {
      if (pdfV.Visible) {
        pdfV.Renderer.ZoomOut( );
      }
      else if (_pic != null) {
        _VPC.ZoomOut( );
        this.Invalidate( this.ClientRectangle );
      }
    }


    private void UC_Album_MouseEnter( object sender, EventArgs e )
    {

    }

    private void UC_Album_MouseLeave( object sender, EventArgs e )
    {
      _mouseDown = false;
      _mouseMoved = false;
    }

    // Capture Size Change for the Painted Area
    private void UC_Album_ClientSizeChanged( object sender, EventArgs e )
    {
      // recalc the Shelf Flow Panel outlines
      flp.MaximumSize = new Size( this.ClientSize.Width - picShelf.Width - 10, this.ClientSize.Height - picPlus.Height - 10 );
      // recalc the frame dimensions
      _frame = this.ClientRectangle;
      _frame.Inflate( -2, -2 );
      // VPC needs to know the new viewport
      _VPC.SetViewport( this.ClientSize );
      // update the view

      pdfV.Width = this.ClientSize.Width - _pdvWidthMargin;
      pdfV.Height = this.ClientSize.Height - _pdvHeightMargin;
      this.Invalidate( this.ClientRectangle );
    }

    // Draw the Image into the Client Area
    // If no image can be shown it is filled with the Control BackColor
    private void UC_Album_Paint( object sender, PaintEventArgs e )
    {
      var save = e.Graphics.Save( ); // preserve the default G-Context

      e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
      if (_pic != null) {
        e.Graphics.DrawImage( _pic, this.ClientRectangle, _VPC.SrcRect, GraphicsUnit.Pixel );
      }
      else {
        // no pic show background
        e.Graphics.FillRectangle( _backBrush, this.ClientRectangle );
      }
      // draw a 5px frame 
      e.Graphics.DrawRectangle( _framePen, _frame );

      // Restore G-Context
      e.Graphics.Restore( save );
    }

    // Capture Shelf Icon
    private void picShelf_Click( object sender, EventArgs e )
    {
      if (flp.Visible) {
        flp.Visible = false; // Hide if prev. shown
      }
      else {
        // reload the shelf from scratch 
        LoadShelf( _shelfFolder, true );
        flp.Visible = true;
      }
      // Callback if someone needs to know
      OnShelfClicked( );
    }


    // Load the file list for selection
    private void LoadShelf( string foldername, bool isMainFolder )
    {
      _shelfList.Clear( );
      flp.Controls.Clear( );

      // catch vanished directory errors
      if (!Directory.Exists( foldername )) {
        flp.Controls.Add(
          new Label( ) {
            Text = $"Folder \n{foldername} not found or not set!!",
            ForeColor = Color.Goldenrod,
            BackColor = Color.Black,
            Padding = new Padding( 5 ),
            AutoSize = true,
            Dock = DockStyle.Top
          }
        );
        return; // sanity bail out
      }
      // get level 1 folders in the shelf dir
      var folders = Directory.EnumerateDirectories( foldername, "*", SearchOption.TopDirectoryOnly );
      foreach (var folder in folders) {
        _shelfList.Add( $"./{folder}" );
      }
      // get the display file list (JPG and PNG anf GIF are supported)
      var files = Directory.EnumerateFiles( foldername, "*.*", SearchOption.TopDirectoryOnly );
      foreach (var file in files) {
        // cannot use a pattern to completely resolve the supported files
        if (file.ToLowerInvariant( ).EndsWith( ".jpg" )
          || file.ToLowerInvariant( ).EndsWith( ".png" )
          || file.ToLowerInvariant( ).EndsWith( ".pdf" )
          || file.ToLowerInvariant( ).EndsWith( ".gif" )) {
          // exclude our temp files
          if (!Path.GetFileName( file ).StartsWith( c_tmpMark )) {
            _shelfList.Add( file );
          }
        }
      }

      // process file list
      bool pingpong = true; // zebra toggle
      flp.SuspendLayout( );
      flp.AutoSize = false;

      if (!isMainFolder) {
        // add upper dir
        var l = new Label( ) {
          Text = @"..\  Leave Subfolder",
          Tag = Path.GetFullPath( Path.GetDirectoryName( foldername ) ),
          BackColor = c_BackBwMid,
          AutoSize = true,
          UseCompatibleTextRendering = true, // some chars don't show up for some fonts
          AutoEllipsis = true,
          Padding = new Padding( 5 ),
          Cursor = Cursors.Hand,
          Dock = DockStyle.Top,
          TextAlign = ContentAlignment.MiddleLeft,
        };
        l.Click += L_Click; // add Click handler as File Selector
        flp.Controls.Add( l );
      }

      foreach (var file in _shelfList) {
        if (file.StartsWith( "./" )) {
          // a folder
          string dirName = file.Substring( 2 );
          var l = new Label( ) {
            Text = @".\" + Path.GetFileNameWithoutExtension( dirName ),
            Tag = Path.GetFullPath( dirName ),
            BackColor = c_BackBwMid,
            AutoSize = true,
            UseCompatibleTextRendering = true, // some chars don't show up for some fonts
            AutoEllipsis = true,
            Padding = new Padding( 5 ),
            Cursor = Cursors.Hand,
            Dock = DockStyle.Top,
            TextAlign = ContentAlignment.MiddleLeft,
          };
          l.Click += L_Click; // add Click handler as File Selector
          flp.Controls.Add( l );
        }
        else {
          var l = new Label( ) {
            Text = Path.GetFileNameWithoutExtension( file ),
            Tag = Path.GetFullPath( file ),
            BackColor = pingpong ? c_BackBwBright : c_BackBwDark, // Zebra coloring
            AutoSize = true,
            UseCompatibleTextRendering = true, // some chars don't show up for some fonts
            AutoEllipsis = true,
            Padding = new Padding( 5 ),
            Cursor = Cursors.Hand,
            Dock = DockStyle.Top,
            TextAlign = ContentAlignment.MiddleLeft,
          };
          l.Click += L_Click; // add Click handler as File Selector
          flp.Controls.Add( l );
          pingpong = !pingpong;
        }
      }
      flp.AutoSize = true;
      flp.BringToFront( );
      flp.ResumeLayout( );

    }

    // Handle the Selection of a Shelf entry
    // .Tag contains the fully qualified path
    private void L_Click( object sender, EventArgs e )
    {
      if (sender is Label) {
        var fname = (string)(sender as Label).Tag;
        if (Directory.Exists( fname )) {
          flp.Visible = false; // hide shelf
          bool isShelfFolder = fname.ToLowerInvariant( ) == _shelfFolder.ToLowerInvariant( );
          LoadShelf( fname, isShelfFolder );
          flp.Visible = true;
        }
        else {
          SetDocument( fname );
          flp.Visible = false; // hide shelf
        }
      }
    }


    // STATIC Helpers

    private static bool LeftSide( Rectangle target, PointF point )
    {
      return (point.X < (target.X + target.Width / 2));
    }

  }
}

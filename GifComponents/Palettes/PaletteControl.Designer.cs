namespace GifComponents.Palettes
{
    partial class PaletteControl
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
        	this.components = new System.ComponentModel.Container();
        	this.listView1 = new System.Windows.Forms.ListView();
        	this.ColumnRbgDecimal = new System.Windows.Forms.ColumnHeader();
        	this.ColumnRgbHex = new System.Windows.Forms.ColumnHeader();
        	this.contextMenuColor = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.detailsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.listToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.tilesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.addColourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.editColourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.deleteColourToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
        	this.openStandardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
        	this.contextMenuColor.SuspendLayout();
        	this.SuspendLayout();
        	// 
        	// listView1
        	// 
        	this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
        	this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
        	        	        	this.ColumnRbgDecimal,
        	        	        	this.ColumnRgbHex});
        	this.listView1.ContextMenuStrip = this.contextMenuColor;
        	this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
        	this.listView1.FullRowSelect = true;
        	this.listView1.Location = new System.Drawing.Point(0, 0);
        	this.listView1.MultiSelect = false;
        	this.listView1.Name = "listView1";
        	this.listView1.Size = new System.Drawing.Size(150, 150);
        	this.listView1.TabIndex = 0;
        	this.listView1.TileSize = new System.Drawing.Size(100, 34);
        	this.listView1.UseCompatibleStateImageBehavior = false;
        	this.listView1.View = System.Windows.Forms.View.Tile;
        	this.listView1.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
        	this.listView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
        	// 
        	// ColumnRbgDecimal
        	// 
        	this.ColumnRbgDecimal.Text = "Decimal value";
        	this.ColumnRbgDecimal.Width = 120;
        	// 
        	// ColumnRgbHex
        	// 
        	this.ColumnRgbHex.Text = "Hex value";
        	this.ColumnRgbHex.Width = 120;
        	// 
        	// contextMenuColor
        	// 
        	this.contextMenuColor.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.fileToolStripMenuItem,
        	        	        	this.viewToolStripMenuItem,
        	        	        	this.addColourToolStripMenuItem,
        	        	        	this.editColourToolStripMenuItem,
        	        	        	this.deleteColourToolStripMenuItem});
        	this.contextMenuColor.Name = "contextMenuColor";
        	this.contextMenuColor.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
        	this.contextMenuColor.ShowImageMargin = false;
        	this.contextMenuColor.Size = new System.Drawing.Size(129, 136);
        	this.contextMenuColor.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuColor_Opening);
        	// 
        	// fileToolStripMenuItem
        	// 
        	this.fileToolStripMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
        	this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.newToolStripMenuItem,
        	        	        	this.openToolStripMenuItem,
        	        	        	this.openStandardToolStripMenuItem,
        	        	        	this.saveToolStripMenuItem});
        	this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
        	this.fileToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
        	this.fileToolStripMenuItem.Text = "File";
        	// 
        	// newToolStripMenuItem
        	// 
        	this.newToolStripMenuItem.Name = "newToolStripMenuItem";
        	this.newToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
        	this.newToolStripMenuItem.Text = "New";
        	this.newToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItemClick);
        	// 
        	// openToolStripMenuItem
        	// 
        	this.openToolStripMenuItem.Name = "openToolStripMenuItem";
        	this.openToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
        	this.openToolStripMenuItem.Text = "Open...";
        	this.openToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItemClick);
        	// 
        	// saveToolStripMenuItem
        	// 
        	this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
        	this.saveToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
        	this.saveToolStripMenuItem.Text = "Save...";
        	this.saveToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItemClick);
        	// 
        	// viewToolStripMenuItem
        	// 
        	this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
        	        	        	this.detailsToolStripMenuItem,
        	        	        	this.listToolStripMenuItem,
        	        	        	this.tilesToolStripMenuItem});
        	this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
        	this.viewToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
        	this.viewToolStripMenuItem.Text = "View";
        	// 
        	// detailsToolStripMenuItem
        	// 
        	this.detailsToolStripMenuItem.Name = "detailsToolStripMenuItem";
        	this.detailsToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
        	this.detailsToolStripMenuItem.Text = "Details";
        	this.detailsToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItemClick);
        	// 
        	// listToolStripMenuItem
        	// 
        	this.listToolStripMenuItem.Name = "listToolStripMenuItem";
        	this.listToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
        	this.listToolStripMenuItem.Text = "List";
        	this.listToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItemClick);
        	// 
        	// tilesToolStripMenuItem
        	// 
        	this.tilesToolStripMenuItem.Name = "tilesToolStripMenuItem";
        	this.tilesToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
        	this.tilesToolStripMenuItem.Text = "Tiles";
        	this.tilesToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItemClick);
        	// 
        	// addColourToolStripMenuItem
        	// 
        	this.addColourToolStripMenuItem.Name = "addColourToolStripMenuItem";
        	this.addColourToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
        	this.addColourToolStripMenuItem.Text = "Add colour (a)";
        	this.addColourToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItemClick);
        	// 
        	// editColourToolStripMenuItem
        	// 
        	this.editColourToolStripMenuItem.Name = "editColourToolStripMenuItem";
        	this.editColourToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
        	this.editColourToolStripMenuItem.Text = "Edit colour";
        	this.editColourToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItemClick);
        	// 
        	// deleteColourToolStripMenuItem
        	// 
        	this.deleteColourToolStripMenuItem.Name = "deleteColourToolStripMenuItem";
        	this.deleteColourToolStripMenuItem.Size = new System.Drawing.Size(128, 22);
        	this.deleteColourToolStripMenuItem.Text = "Delete colour";
        	this.deleteColourToolStripMenuItem.Click += new System.EventHandler(this.ToolStripMenuItemClick);
        	// 
        	// contextMenuStrip1
        	// 
        	this.contextMenuStrip1.Name = "contextMenuStrip1";
        	this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
        	// 
        	// openStandardToolStripMenuItem
        	// 
        	this.openStandardToolStripMenuItem.Name = "openStandardToolStripMenuItem";
        	this.openStandardToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
        	this.openStandardToolStripMenuItem.Text = "Open standard palette...";
        	// 
        	// PaletteControl
        	// 
        	this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        	this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        	this.Controls.Add(this.listView1);
        	this.Name = "PaletteControl";
        	this.contextMenuColor.ResumeLayout(false);
        	this.ResumeLayout(false);
        }
        private System.Windows.Forms.ToolStripMenuItem openStandardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem editColourToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteColourToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem addColourToolStripMenuItem;
        private System.Windows.Forms.ColumnHeader ColumnRgbHex;
        private System.Windows.Forms.ColumnHeader ColumnRbgDecimal;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;

        #endregion

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ContextMenuStrip contextMenuColor;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem detailsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem listToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tilesToolStripMenuItem;
    }
}

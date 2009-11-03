#region Copyright (C) Simon Bridewell
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 3
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

// You can read the full text of the GNU General Public License at:
// http://www.gnu.org/licenses/gpl.html

// See also the Wikipedia entry on the GNU GPL at:
// http://en.wikipedia.org/wiki/GNU_General_Public_License
#endregion
namespace GifInspector
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.buttonLoadGif = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageFile = new System.Windows.Forms.TabPage();
			this.propertyGridFile = new System.Windows.Forms.PropertyGrid();
			this.tabPageImages = new System.Windows.Forms.TabPage();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.propertyGridFrame = new System.Windows.Forms.PropertyGrid();
			this.buttonNext = new System.Windows.Forms.Button();
			this.buttonPrevious = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxFrameCount = new System.Windows.Forms.TextBox();
			this.textBoxFrameNumber = new System.Windows.Forms.TextBox();
			this.textBoxStatus = new System.Windows.Forms.TextBox();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.buttonExtractFrames = new System.Windows.Forms.Button();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabPageFile.SuspendLayout();
			this.tabPageImages.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonLoadGif
			// 
			this.buttonLoadGif.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonLoadGif.Location = new System.Drawing.Point(5, 33);
			this.buttonLoadGif.Name = "buttonLoadGif";
			this.buttonLoadGif.Size = new System.Drawing.Size(353, 23);
			this.buttonLoadGif.TabIndex = 0;
			this.buttonLoadGif.TabStop = false;
			this.buttonLoadGif.Text = "Load GIF file...";
			this.buttonLoadGif.UseVisualStyleBackColor = true;
			this.buttonLoadGif.Click += new System.EventHandler(this.ButtonLoadGifClick);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(347, 113);
			this.pictureBox1.TabIndex = 1;
			this.pictureBox1.TabStop = false;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.Filter = "GIF files | *.gif";
			this.openFileDialog1.RestoreDirectory = true;
			this.openFileDialog1.SupportMultiDottedExtensions = true;
			this.openFileDialog1.Title = "Pick GIF file to inspect";
			// 
			// tabControl1
			// 
			this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl1.Controls.Add(this.tabPageFile);
			this.tabControl1.Controls.Add(this.tabPageImages);
			this.tabControl1.Location = new System.Drawing.Point(5, 122);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(357, 351);
			this.tabControl1.TabIndex = 0;
			this.tabControl1.TabStop = false;
			// 
			// tabPageFile
			// 
			this.tabPageFile.Controls.Add(this.propertyGridFile);
			this.tabPageFile.Location = new System.Drawing.Point(4, 22);
			this.tabPageFile.Name = "tabPageFile";
			this.tabPageFile.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageFile.Size = new System.Drawing.Size(349, 325);
			this.tabPageFile.TabIndex = 0;
			this.tabPageFile.Text = "File";
			this.tabPageFile.UseVisualStyleBackColor = true;
			// 
			// propertyGridFile
			// 
			this.propertyGridFile.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGridFile.Location = new System.Drawing.Point(3, 3);
			this.propertyGridFile.Name = "propertyGridFile";
			this.propertyGridFile.Size = new System.Drawing.Size(343, 319);
			this.propertyGridFile.TabIndex = 6;
			// 
			// tabPageImages
			// 
			this.tabPageImages.Controls.Add(this.splitContainer1);
			this.tabPageImages.Controls.Add(this.buttonNext);
			this.tabPageImages.Controls.Add(this.buttonPrevious);
			this.tabPageImages.Controls.Add(this.label1);
			this.tabPageImages.Controls.Add(this.textBoxFrameCount);
			this.tabPageImages.Controls.Add(this.textBoxFrameNumber);
			this.tabPageImages.Location = new System.Drawing.Point(4, 22);
			this.tabPageImages.Name = "tabPageImages";
			this.tabPageImages.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageImages.Size = new System.Drawing.Size(349, 325);
			this.tabPageImages.TabIndex = 1;
			this.tabPageImages.Text = "Images";
			this.tabPageImages.UseVisualStyleBackColor = true;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitContainer1.Location = new System.Drawing.Point(0, 29);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.propertyGridFrame);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
			this.splitContainer1.Size = new System.Drawing.Size(349, 296);
			this.splitContainer1.SplitterDistance = 177;
			this.splitContainer1.TabIndex = 6;
			// 
			// propertyGridFrame
			// 
			this.propertyGridFrame.Dock = System.Windows.Forms.DockStyle.Fill;
			this.propertyGridFrame.Location = new System.Drawing.Point(0, 0);
			this.propertyGridFrame.Name = "propertyGridFrame";
			this.propertyGridFrame.Size = new System.Drawing.Size(347, 175);
			this.propertyGridFrame.TabIndex = 6;
			// 
			// buttonNext
			// 
			this.buttonNext.AutoSize = true;
			this.buttonNext.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.buttonNext.Location = new System.Drawing.Point(139, 1);
			this.buttonNext.Name = "buttonNext";
			this.buttonNext.Size = new System.Drawing.Size(23, 23);
			this.buttonNext.TabIndex = 5;
			this.buttonNext.TabStop = false;
			this.buttonNext.Text = ">";
			this.buttonNext.UseVisualStyleBackColor = true;
			this.buttonNext.Click += new System.EventHandler(this.ButtonNextClick);
			// 
			// buttonPrevious
			// 
			this.buttonPrevious.AutoSize = true;
			this.buttonPrevious.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.buttonPrevious.Location = new System.Drawing.Point(0, 1);
			this.buttonPrevious.Name = "buttonPrevious";
			this.buttonPrevious.Size = new System.Drawing.Size(23, 23);
			this.buttonPrevious.TabIndex = 5;
			this.buttonPrevious.TabStop = false;
			this.buttonPrevious.Text = "<";
			this.buttonPrevious.UseVisualStyleBackColor = true;
			this.buttonPrevious.Click += new System.EventHandler(this.ButtonPreviousClick);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(73, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(16, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "of";
			// 
			// textBoxFrameCount
			// 
			this.textBoxFrameCount.Location = new System.Drawing.Point(95, 3);
			this.textBoxFrameCount.Name = "textBoxFrameCount";
			this.textBoxFrameCount.ReadOnly = true;
			this.textBoxFrameCount.Size = new System.Drawing.Size(38, 20);
			this.textBoxFrameCount.TabIndex = 3;
			this.textBoxFrameCount.TabStop = false;
			// 
			// textBoxFrameNumber
			// 
			this.textBoxFrameNumber.Location = new System.Drawing.Point(29, 3);
			this.textBoxFrameNumber.Name = "textBoxFrameNumber";
			this.textBoxFrameNumber.ReadOnly = true;
			this.textBoxFrameNumber.Size = new System.Drawing.Size(38, 20);
			this.textBoxFrameNumber.TabIndex = 3;
			this.textBoxFrameNumber.TabStop = false;
			// 
			// textBoxStatus
			// 
			this.textBoxStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxStatus.Location = new System.Drawing.Point(5, 96);
			this.textBoxStatus.Name = "textBoxStatus";
			this.textBoxStatus.ReadOnly = true;
			this.textBoxStatus.Size = new System.Drawing.Size(353, 20);
			this.textBoxStatus.TabIndex = 3;
			this.textBoxStatus.TabStop = false;
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.Timer1Tick);
			// 
			// buttonExtractFrames
			// 
			this.buttonExtractFrames.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonExtractFrames.Enabled = false;
			this.buttonExtractFrames.Location = new System.Drawing.Point(5, 62);
			this.buttonExtractFrames.Name = "buttonExtractFrames";
			this.buttonExtractFrames.Size = new System.Drawing.Size(353, 23);
			this.buttonExtractFrames.TabIndex = 4;
			this.buttonExtractFrames.TabStop = false;
			this.buttonExtractFrames.Text = "Extract frames to bitmaps";
			this.buttonExtractFrames.UseVisualStyleBackColor = true;
			this.buttonExtractFrames.Click += new System.EventHandler(this.ButtonExtractFramesClick);
			// 
			// menuStrip1
			// 
			this.menuStrip1.BackColor = System.Drawing.SystemColors.MenuBar;
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(365, 24);
			this.menuStrip1.TabIndex = 12;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(40, 20);
			this.helpToolStripMenuItem.Text = "Help";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(126, 22);
			this.aboutToolStripMenuItem.Text = "About...";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItemClick);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(365, 472);
			this.Controls.Add(this.menuStrip1);
			this.Controls.Add(this.buttonExtractFrames);
			this.Controls.Add(this.textBoxStatus);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.buttonLoadGif);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "MainForm";
			this.Text = "GifInspector";
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tabPageFile.ResumeLayout(false);
			this.tabPageImages.ResumeLayout(false);
			this.tabPageImages.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.Button buttonExtractFrames;
		private System.Windows.Forms.TabPage tabPageFile;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.PropertyGrid propertyGridFile;
		private System.Windows.Forms.PropertyGrid propertyGridFrame;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.TextBox textBoxFrameNumber;
		private System.Windows.Forms.TextBox textBoxFrameCount;
		private System.Windows.Forms.Button buttonPrevious;
		private System.Windows.Forms.Button buttonNext;
		private System.Windows.Forms.TextBox textBoxStatus;
		private System.Windows.Forms.TabPage tabPageImages;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.Button buttonLoadGif;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.PictureBox pictureBox1;
	}
}

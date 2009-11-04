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
namespace GifBuilder
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
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.propertyGridFrame = new System.Windows.Forms.PropertyGrid();
			this.buttonAddFrameAfter = new System.Windows.Forms.Button();
			this.buttonRemoveFrame = new System.Windows.Forms.Button();
			this.buttonAddFrameBefore = new System.Windows.Forms.Button();
			this.buttonNextFrame = new System.Windows.Forms.Button();
			this.buttonPreviousFrame = new System.Windows.Forms.Button();
			this.labelFrameNumber = new System.Windows.Forms.Label();
			this.labelNoImages = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPageEncoder = new System.Windows.Forms.TabPage();
			this.propertyGridEncoder = new System.Windows.Forms.PropertyGrid();
			this.buttonEncode = new System.Windows.Forms.Button();
			this.tabPageFrames = new System.Windows.Forms.TabPage();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.menuStrip1.SuspendLayout();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.tabControl1.SuspendLayout();
			this.tabPageEncoder.SuspendLayout();
			this.tabPageFrames.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
									this.helpToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(647, 24);
			this.menuStrip1.TabIndex = 0;
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
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(114, 22);
			this.aboutToolStripMenuItem.Text = "About";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.AboutToolStripMenuItemClick);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(3, 3);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.propertyGridFrame);
			this.splitContainer1.Panel1.Controls.Add(this.buttonAddFrameAfter);
			this.splitContainer1.Panel1.Controls.Add(this.buttonRemoveFrame);
			this.splitContainer1.Panel1.Controls.Add(this.buttonAddFrameBefore);
			this.splitContainer1.Panel1.Controls.Add(this.buttonNextFrame);
			this.splitContainer1.Panel1.Controls.Add(this.buttonPreviousFrame);
			this.splitContainer1.Panel1.Controls.Add(this.labelFrameNumber);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.labelNoImages);
			this.splitContainer1.Panel2.Controls.Add(this.pictureBox1);
			this.splitContainer1.Size = new System.Drawing.Size(633, 352);
			this.splitContainer1.SplitterDistance = 231;
			this.splitContainer1.TabIndex = 1;
			// 
			// propertyGridFrame
			// 
			this.propertyGridFrame.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGridFrame.Location = new System.Drawing.Point(0, 60);
			this.propertyGridFrame.Name = "propertyGridFrame";
			this.propertyGridFrame.Size = new System.Drawing.Size(622, 169);
			this.propertyGridFrame.TabIndex = 6;
			this.propertyGridFrame.TabStop = false;
			this.propertyGridFrame.ToolbarVisible = false;
			this.propertyGridFrame.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.PropertyGridFramePropertyValueChanged);
			// 
			// buttonAddFrameAfter
			// 
			this.buttonAddFrameAfter.Location = new System.Drawing.Point(113, 30);
			this.buttonAddFrameAfter.Name = "buttonAddFrameAfter";
			this.buttonAddFrameAfter.Size = new System.Drawing.Size(97, 23);
			this.buttonAddFrameAfter.TabIndex = 5;
			this.buttonAddFrameAfter.TabStop = false;
			this.buttonAddFrameAfter.Text = "Add frame &after...";
			this.buttonAddFrameAfter.UseVisualStyleBackColor = true;
			this.buttonAddFrameAfter.Click += new System.EventHandler(this.ButtonClick);
			// 
			// buttonRemoveFrame
			// 
			this.buttonRemoveFrame.Location = new System.Drawing.Point(219, 30);
			this.buttonRemoveFrame.Name = "buttonRemoveFrame";
			this.buttonRemoveFrame.Size = new System.Drawing.Size(93, 23);
			this.buttonRemoveFrame.TabIndex = 4;
			this.buttonRemoveFrame.TabStop = false;
			this.buttonRemoveFrame.Text = "&Remove frame";
			this.buttonRemoveFrame.UseVisualStyleBackColor = true;
			this.buttonRemoveFrame.Click += new System.EventHandler(this.ButtonClick);
			// 
			// buttonAddFrameBefore
			// 
			this.buttonAddFrameBefore.Location = new System.Drawing.Point(0, 30);
			this.buttonAddFrameBefore.Name = "buttonAddFrameBefore";
			this.buttonAddFrameBefore.Size = new System.Drawing.Size(107, 23);
			this.buttonAddFrameBefore.TabIndex = 3;
			this.buttonAddFrameBefore.TabStop = false;
			this.buttonAddFrameBefore.Text = "Add frame &before...";
			this.buttonAddFrameBefore.UseVisualStyleBackColor = true;
			this.buttonAddFrameBefore.Click += new System.EventHandler(this.ButtonClick);
			// 
			// buttonNextFrame
			// 
			this.buttonNextFrame.AutoSize = true;
			this.buttonNextFrame.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.buttonNextFrame.Location = new System.Drawing.Point(29, 0);
			this.buttonNextFrame.Name = "buttonNextFrame";
			this.buttonNextFrame.Size = new System.Drawing.Size(23, 23);
			this.buttonNextFrame.TabIndex = 2;
			this.buttonNextFrame.TabStop = false;
			this.buttonNextFrame.Text = ">";
			this.buttonNextFrame.UseVisualStyleBackColor = true;
			this.buttonNextFrame.Click += new System.EventHandler(this.ButtonClick);
			// 
			// buttonPreviousFrame
			// 
			this.buttonPreviousFrame.AutoSize = true;
			this.buttonPreviousFrame.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.buttonPreviousFrame.Location = new System.Drawing.Point(0, 0);
			this.buttonPreviousFrame.Name = "buttonPreviousFrame";
			this.buttonPreviousFrame.Size = new System.Drawing.Size(23, 23);
			this.buttonPreviousFrame.TabIndex = 1;
			this.buttonPreviousFrame.TabStop = false;
			this.buttonPreviousFrame.Text = "<";
			this.buttonPreviousFrame.UseVisualStyleBackColor = true;
			this.buttonPreviousFrame.Click += new System.EventHandler(this.ButtonClick);
			// 
			// labelFrameNumber
			// 
			this.labelFrameNumber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.labelFrameNumber.Location = new System.Drawing.Point(55, 5);
			this.labelFrameNumber.Name = "labelFrameNumber";
			this.labelFrameNumber.Size = new System.Drawing.Size(575, 18);
			this.labelFrameNumber.TabIndex = 0;
			this.labelFrameNumber.Text = "Frame number";
			// 
			// labelNoImages
			// 
			this.labelNoImages.Location = new System.Drawing.Point(29, 29);
			this.labelNoImages.Name = "labelNoImages";
			this.labelNoImages.Size = new System.Drawing.Size(173, 18);
			this.labelNoImages.TabIndex = 1;
			this.labelNoImages.Text = "No images have been added yet";
			// 
			// pictureBox1
			// 
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(10, 10);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			this.pictureBox1.LoadCompleted += new System.ComponentModel.AsyncCompletedEventHandler(this.PictureBox1LoadCompleted);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.DefaultExt = "bmp";
			this.openFileDialog1.Filter = "Image Files|*.gif;*.png;*.jpg;*.jpeg;*.bmp|PNG Files(*.png)|*.png|JPEG Files(*.jp" +
			"g;*.jpeg)|*.jpg;*.jpeg|GIF Files(*.gif)|*.gif|BMP Files(*.bmp)|*.bmp";
			this.openFileDialog1.RestoreDirectory = true;
			this.openFileDialog1.Title = "Add image...";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPageEncoder);
			this.tabControl1.Controls.Add(this.tabPageFrames);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 24);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(647, 384);
			this.tabControl1.TabIndex = 2;
			// 
			// tabPageEncoder
			// 
			this.tabPageEncoder.Controls.Add(this.propertyGridEncoder);
			this.tabPageEncoder.Controls.Add(this.buttonEncode);
			this.tabPageEncoder.Location = new System.Drawing.Point(4, 22);
			this.tabPageEncoder.Name = "tabPageEncoder";
			this.tabPageEncoder.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageEncoder.Size = new System.Drawing.Size(639, 358);
			this.tabPageEncoder.TabIndex = 0;
			this.tabPageEncoder.Text = "Encoder properties";
			this.tabPageEncoder.UseVisualStyleBackColor = true;
			// 
			// propertyGridEncoder
			// 
			this.propertyGridEncoder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
									| System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.propertyGridEncoder.Location = new System.Drawing.Point(0, 29);
			this.propertyGridEncoder.Name = "propertyGridEncoder";
			this.propertyGridEncoder.Size = new System.Drawing.Size(639, 329);
			this.propertyGridEncoder.TabIndex = 1;
			this.propertyGridEncoder.TabStop = false;
			// 
			// buttonEncode
			// 
			this.buttonEncode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
									| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonEncode.Location = new System.Drawing.Point(0, 0);
			this.buttonEncode.Name = "buttonEncode";
			this.buttonEncode.Size = new System.Drawing.Size(639, 23);
			this.buttonEncode.TabIndex = 0;
			this.buttonEncode.Text = "Encode GIF file...";
			this.buttonEncode.UseVisualStyleBackColor = true;
			this.buttonEncode.Click += new System.EventHandler(this.ButtonClick);
			// 
			// tabPageFrames
			// 
			this.tabPageFrames.Controls.Add(this.splitContainer1);
			this.tabPageFrames.Location = new System.Drawing.Point(4, 22);
			this.tabPageFrames.Name = "tabPageFrames";
			this.tabPageFrames.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageFrames.Size = new System.Drawing.Size(639, 358);
			this.tabPageFrames.TabIndex = 1;
			this.tabPageFrames.Text = "Frames";
			this.tabPageFrames.UseVisualStyleBackColor = true;
			// 
			// saveFileDialog1
			// 
			this.saveFileDialog1.DefaultExt = "gif";
			this.saveFileDialog1.Filter = "GIF files|*.gif";
			this.saveFileDialog1.RestoreDirectory = true;
			this.saveFileDialog1.Title = "Save GIF file...";
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(647, 408);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MainForm";
			this.Text = "GifBuilder";
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel1.PerformLayout();
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.Panel2.PerformLayout();
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.tabControl1.ResumeLayout(false);
			this.tabPageEncoder.ResumeLayout(false);
			this.tabPageFrames.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		private System.Windows.Forms.PropertyGrid propertyGridFrame;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.TabPage tabPageFrames;
		private System.Windows.Forms.Button buttonEncode;
		private System.Windows.Forms.PropertyGrid propertyGridEncoder;
		private System.Windows.Forms.TabPage tabPageEncoder;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.Button buttonAddFrameBefore;
		private System.Windows.Forms.Button buttonAddFrameAfter;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Button buttonRemoveFrame;
		private System.Windows.Forms.Label labelNoImages;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label labelFrameNumber;
		private System.Windows.Forms.Button buttonPreviousFrame;
		private System.Windows.Forms.Button buttonNextFrame;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.MenuStrip menuStrip1;
	}
}

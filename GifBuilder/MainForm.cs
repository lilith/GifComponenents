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

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using GifComponents;
using CommonForms;
using CommonForms.Responsiveness;

namespace GifBuilder
{
	/// <summary>
	/// The main form in the GifBuilder application.
	/// </summary>
	public partial class MainForm : ResponsiveForm
	{
		#region declarations
		private AnimatedGifEncoder _encoder;
		private int _currentIndex;
		private string _saveFileName;
		#endregion
		
		#region constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms 
			// designer support.
			//
			InitializeComponent();
			
			//
			// Add constructor code after the InitializeComponent() call.
			//
			_encoder = new AnimatedGifEncoder();
			propertyGridEncoder.SelectedObject = _encoder;
			RefreshUI();
		}
		#endregion
		
		#region event handlers
		
		#region menu item click event handler
		[SuppressMessage("Microsoft.Design", 
		                 "CA1031:DoNotCatchGeneralExceptionTypes")]
		void ToolStripMenuItemClick(object sender, EventArgs e)
		{
			try 
			{
				ToolStripMenuItem tsmi = (ToolStripMenuItem) sender;
				switch( tsmi.Name )
				{
					case "encodeGIFFileToolStripMenuItem":
						DoEncode();
						break;
						
					case "addFrameBeforeCurrentToolStripMenuItem":
						AddFrames( true );
						break;
						
					case "addFrameAfterCurrentToolStripMenuItem":
						AddFrames( false );
						break;
						
					case "removeCurrentFrameToolStripMenuItem":
						RemoveFrame();
						break;
						
					case "reorderFramesToolStripMenuItem":
						ReorderFrames();
						break;
						
					case "aboutToolStripMenuItem":
						AboutForm.Show();
						break;
						
					default:
						CleverMessageBox.Show( tsmi.Name, this );
						break;
				}
			} 
			catch( Exception ex ) 
			{
				HandleException( ex );
			}
		}
		#endregion
		
		#region button click event handler
		[SuppressMessage("Microsoft.Design", 
		                 "CA1031:DoNotCatchGeneralExceptionTypes")]
		void ButtonClick(object sender, EventArgs e)
		{
			try 
			{
				Button b = (Button) sender;
				string name = b.Name;
				
				switch( name )
				{
					case "buttonPreviousFrame":
						PreviousFrame();
						break;
	
					case "buttonNextFrame":
						NextFrame();
						break;
	
					default:
						string message = "Unexpected button: " + name;
						throw new InvalidOperationException( message );
				}
			} 
			catch( Exception ex ) 
			{
				HandleException( ex );
			}
		}
		#endregion
		
		#region PictureBox LoadCompleted handler
		[SuppressMessage("Microsoft.Design", 
		                 "CA1031:DoNotCatchGeneralExceptionTypes")]
		void PictureBox1LoadCompleted(object sender, 
		                              System.ComponentModel.AsyncCompletedEventArgs e)
		{
			try 
			{
				PictureBox pb = (PictureBox) sender;
				pb.Size = pb.Image.Size;
				labelNoImages.Visible = false;
			} 
			catch( Exception ex ) 
			{
				HandleException( ex );
			}
		}
		#endregion

		#region PropertyGridFramePropertyValueChanged event handler
		void PropertyGridFramePropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			RefreshUI();
		}
		#endregion

		#endregion
		
		#region private methods
		
		#region private AddFrames method
		private void AddFrames( bool addBefore )
		{
			DialogResult result = openFileDialog1.ShowDialog();
			if( result == DialogResult.OK )
			{
				foreach( string fileName in openFileDialog1.FileNames )
				{
					Image thisImage = Image.FromFile( fileName );
					GifFrame thisFrame = new GifFrame( thisImage );
					if( _encoder.Frames.Count == 0 )
					{
						_encoder.AddFrame( thisFrame );
					}
					else
					{
						if( addBefore )
						{
							_encoder.Frames.Insert( _currentIndex, thisFrame );
						}
						else
						{
							_encoder.Frames.Insert( ++_currentIndex, thisFrame );
						}
					}
					RefreshUI();
				}
			}
		}
		#endregion
		
		#region private RemoveFrame method
		private void RemoveFrame()
		{
			_encoder.Frames.Remove( _encoder.Frames[_currentIndex] );
			RefreshUI();
		}
		#endregion
		
		#region private ReorderFrames method
		private void ReorderFrames()
		{
			ReorderFramesForm f = new ReorderFramesForm( _encoder );
			f.ShowDialog();
			RefreshUI();
		}
		#endregion
		
		#region private PreviousFrame method
		private void PreviousFrame()
		{
			_currentIndex--;
			RefreshUI();
		}
		#endregion
		
		#region private NextFrame method
		private void NextFrame()
		{
			_currentIndex++;
			RefreshUI();
		}
		#endregion
		
		#region private RefreshUI method
		private void RefreshUI()
		{
			#region ensure current index is within frame count
			if( _currentIndex < 0 )
			{
				_currentIndex = 0;
			}
			
			if( _currentIndex >= _encoder.Frames.Count )
			{
				_currentIndex = _encoder.Frames.Count - 1;
			}
			#endregion
			
			labelFrameNumber.Text 
				= "Frame " + (_currentIndex + 1)
				+ " of " + _encoder.Frames.Count;
			
			if( _encoder.Frames.Count > 0 )
			{
				pictureBox1.Image = _encoder.Frames[_currentIndex].TheImage;
				pictureBox1.Location = _encoder.Frames[_currentIndex].Position;
				propertyGridFrame.SelectedObject = _encoder.Frames[_currentIndex];
				addFrameAfterCurrentToolStripMenuItem.Enabled = true;
				removeCurrentFrameToolStripMenuItem.Enabled = true;
				labelNoImages.Visible = false;
			}
			else
			{
				pictureBox1.Image = null;
				propertyGridFrame.SelectedObject = null;
				addFrameAfterCurrentToolStripMenuItem.Enabled = false;
				removeCurrentFrameToolStripMenuItem.Enabled = false;
				labelNoImages.Visible = true;
			}
			
			if( _encoder.Frames.Count > 1 )
			{
				reorderFramesToolStripMenuItem.Enabled = true;
			}
			else
			{
				reorderFramesToolStripMenuItem.Enabled = false;
			}
		}
		#endregion
		
		#region private DoEncode method - called from UI thread
		private void DoEncode()
		{
			if( _encoder.Frames.Count < 1 )
			{
				CleverMessageBox.Show( "This animation has no frames!", 
				                       "Error",
				                       MessageBoxButtons.OK,
				                       MessageBoxIcon.Warning, 
				                       this );
				return;
			}
			
			DialogResult result = saveFileDialog1.ShowDialog();
			if( result == DialogResult.OK )
			{
				_saveFileName = saveFileDialog1.FileName;
				StartBackgroundProcess( _encoder, Encode, "Encoding..." );
			}
		}
		#endregion
		
		#region private Encode method - run on background thread
		/// <summary>
		/// Starts off encoding the output animation on a background thread.
		/// </summary>
		[SuppressMessage("Microsoft.Design", 
		                 "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void Encode()
		{
			// Note the encoder was instantiated in this form's constructor
			// and the user has set its properties in the UI, including adding
			// all the frames.
			try
			{
				_encoder.WriteToFile( _saveFileName );
				Invoke( new MethodInvoker( StopTheClock ) );
			}
			catch( Exception ex )
			{
				HandleException( ex );
			}
		}
		#endregion
		
		#endregion

	}
}

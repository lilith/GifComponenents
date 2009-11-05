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
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;

using GifComponents;
using CommonForms;

namespace GifBuilder
{
	/// <summary>
	/// The main form in this application.
	/// </summary>
	public partial class MainForm : Form
	{
		#region declarations
		private AnimatedGifEncoder _encoder;
		private int _currentIndex;
		private string _saveFileName;
		private bool _allowToClose;
		private Thread _t;
		private Exception _exception;
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
			toolStripStatusLabelEncoder.Text = string.Empty;
			toolStripStatusLabelPixelAnalysis.Text = string.Empty;
			_encoder = new AnimatedGifEncoder();
			propertyGridEncoder.SelectedObject = _encoder;
			_allowToClose = true;
			RefreshUI();
		}
		#endregion
		
		#region event handlers
		
		#region menu item click handlers
		[SuppressMessage("Microsoft.Design", 
		                 "CA1031:DoNotCatchGeneralExceptionTypes")]
		void AboutToolStripMenuItemClick(object sender, EventArgs e)
		{
			try 
			{
				string aboutFile = "GifBuilder.AboutFormParameters.xml";
				AboutFormParameters parameters 
					= AboutFormParameters.LoadXml( aboutFile );
				AboutForm af = new AboutForm( parameters );
				af.ShowDialog();
			} 
			catch( Exception ex ) 
			{
				ExceptionForm ef = new ExceptionForm( ex );
				ef.ShowDialog();
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
	
					case "buttonAddFrameBefore":
						AddFrames( true );
						break;
	
					case "buttonAddFrameAfter":
						AddFrames( false );
						break;
	
					case "buttonRemoveFrame":
						RemoveFrame();
						break;
						
					case "buttonReorderFrames":
						ReorderFrames();
						break;
						
					case "buttonEncode":
						Encode();
						break;
	
					default:
						string message = "Unexpected button: " + name;
						throw new InvalidOperationException( message );
				}
			} 
			catch( Exception ex ) 
			{
				ExceptionForm ef = new ExceptionForm( ex );
				ef.ShowDialog();
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
				ExceptionForm ef = new ExceptionForm( ex );
				ef.ShowDialog();
			}
		}
		#endregion

		#region PropertyGridFramePropertyValueChanged event handler
		void PropertyGridFramePropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
			RefreshUI();
		}
		#endregion

		#region Form closing event handler
		void MainFormFormClosing(object sender, FormClosingEventArgs e)
		{
			if( _allowToClose == false )
			{
				string message = "The process is still running - "
					+ "are you sure you want to close this window?";
				string caption = "Close window?";
				DialogResult result =
					CleverMessageBox.Show( message,
					                       caption,
					                       MessageBoxButtons.YesNo,
					                       MessageBoxIcon.Warning, 
					                       this );
				if( result == DialogResult.No )
				{
					// don't close the window
					e.Cancel = true;
				}
				else
				{
					_t.Abort();
				}
			}
		}
		#endregion

		#region Timer1 tick event handler
		void Timer1Tick(object sender, EventArgs e)
		{
			UpdateStatusBar();
		}
		#endregion

		#endregion
		
		#region other private methods
		
		#region AddFrames
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
		
		#region RemoveFrame
		private void RemoveFrame()
		{
			_encoder.Frames.Remove( _encoder.Frames[_currentIndex] );
			RefreshUI();
		}
		#endregion
		
		#region PreviousFrame
		private void PreviousFrame()
		{
			_currentIndex--;
			RefreshUI();
		}
		#endregion
		
		#region NextFrame
		private void NextFrame()
		{
			_currentIndex++;
			RefreshUI();
		}
		#endregion
		
		#region ReorderFrames
		private void ReorderFrames()
		{
			ReorderFramesForm f = new ReorderFramesForm( _encoder );
			f.ShowDialog();
			RefreshUI();
		}
		#endregion
		
		#region RefreshUI
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
				buttonAddFrameAfter.Enabled = true;
				buttonRemoveFrame.Enabled = true;
				labelNoImages.Visible = false;
			}
			else
			{
				pictureBox1.Image = null;
				propertyGridFrame.SelectedObject = null;
				buttonAddFrameAfter.Enabled = false;
				buttonRemoveFrame.Enabled = false;
				labelNoImages.Visible = true;
			}
			
			if( _encoder.Frames.Count > 1 )
			{
				buttonReorderFrames.Enabled = true;
			}
			else
			{
				buttonReorderFrames.Enabled = false;
			}
		}
		#endregion
		
		#region UpdateStatusBar
		private void UpdateStatusBar()
		{
			toolStripStatusLabelEncoder.Text = _encoder.Status;
			toolStripStatusLabelPixelAnalysis.Text = _encoder.PixelAnalysisStatus;
			double frameCount = (double) _encoder.Frames.Count;
			double currentFrame, progress;
			
			currentFrame = (double) _encoder.ProcessingFrame;
			progress = currentFrame / frameCount * 100;
			toolStripProgressBarEncoder.Value = (int) progress;
			
			currentFrame = (double) _encoder.PixelAnalysisProcessingFrame;
			progress = currentFrame / frameCount * 100;
			toolStripProgressBarPixelAnalysis.Value = (int) progress;
		}
		#endregion
		
		#region Encode
		private void Encode()
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
				foreach( Control c in this.Controls )
				{
					c.Enabled = false;
				}
				_allowToClose = false;
				_exception = null;
				_t = new Thread( StartEncoding );
				_t.IsBackground = true;
				_t.Start();
				timer1.Start();
			}
		}
		#endregion
		
		#region StartEncoding method
		/// <summary>
		/// Starts off encoding the output animation on a background thread.
		/// </summary>
		[SuppressMessage("Microsoft.Design", 
		                 "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void StartEncoding()
		{
			// Note the encoder was instantiated in this form's constructor
			// and the user has set its properties in the UI, including adding
			// all the frames.
			try
			{
				_encoder.WriteToFile( _saveFileName );
			}
			catch( Exception ex )
			{
				_exception = ex;
			}
			finally
			{
				Invoke( new MethodInvoker( StopTheClock ) );
			}
		}
		#endregion
		
		#region StopTheClock method
		private void StopTheClock()
		{
			timer1.Stop();
			foreach( Control c in this.Controls )
			{
				c.Enabled = true;
			}
			_allowToClose = true;
			if( _exception != null )
			{
				ExceptionForm ef = new ExceptionForm( _exception );
				ef.ShowDialog();
			}
			UpdateStatusBar();
			RefreshUI();
		}
		#endregion
		
		#endregion
		
	}
}

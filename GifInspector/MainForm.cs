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

#region using directives
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using GifComponents;
using CommonForms;
using CommonForms.Responsiveness;
#endregion

namespace GifInspector
{
	/// <summary>
	/// The main form in the GIF Inspector application
	/// </summary>
	public partial class MainForm : ResponsiveForm
	{
		#region declarations
		private GifDecoder _decoder;
		private string _extractPath;
		private int _imageIndex;
		#endregion
		
		#region constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public MainForm()
		{
			//
			// The InitializeComponent() call is required for Windows Forms designer support.
			//
			InitializeComponent();
			
			//
			// Add constructor code after the InitializeComponent() call.
			//
		}
		#endregion
		
		#region private methods

		#region private DoLoadGif method - called by event handler
		/// <summary>
		/// Call this method from the event handler. Starts the LoadGif method
		/// off on a background thread.
		/// </summary>
		private void DoLoadGif()
		{
			DialogResult result = openFileDialog1.ShowDialog();
			if( result == DialogResult.OK )
			{
				// Point at the first frame to avoid IndexOutOfRangeException
				// whilst loading the GIF.
				_imageIndex = 0;
				_decoder = new GifDecoder( openFileDialog1.FileName, 
				                           createDebugXMLToolStripMenuItem.Checked );
				StartBackgroundProcess( _decoder, LoadGif, "Decoding..." );
			}
		}
		#endregion
		
		#region private LoadGif method - runs on background thread
		[SuppressMessage("Microsoft.Design", 
		                 "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void LoadGif()
		{
			try
			{
				_decoder.Decode();
				Invoke( new MethodInvoker( StopTheClock ) );
			}
			catch( Exception ex )
			{
				HandleException( ex );
			}
		}
		#endregion

		#region private DoExtractFrames method - called by event handler
		/// <summary>
		/// Call this method from the event handler. Starts the ExtractFrames
		/// method off on a background thread.
		/// </summary>
		void DoExtractFrames()
		{
			if( _decoder == null )
			{
				CleverMessageBox.Show( "You haven't loaded a GIF file yet!", 
				                       this );
				return;
			}
			
			folderBrowserDialog1.SelectedPath 
				= Path.GetDirectoryName( openFileDialog1.FileName );
			DialogResult result = folderBrowserDialog1.ShowDialog();
			if( result == DialogResult.OK )
			{
				_extractPath = folderBrowserDialog1.SelectedPath;
				ExtractFrames();
			}
		}
		#endregion

		#region private ExtractFrames method
		[SuppressMessage("Microsoft.Design", 
		                 "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void ExtractFrames()
		{
			try
			{
				string baseFileName 
					= _extractPath
					+ Path.DirectorySeparatorChar
					+ Path.GetFileNameWithoutExtension( openFileDialog1.FileName );
				for( int i = 0; i < _decoder.Frames.Count; i++ )
				{
					string fileName = baseFileName + ".frame " + i + ".bmp";
					_decoder.Frames[i].TheImage.Save( fileName, ImageFormat.Bmp );
				}
			}
			catch( Exception ex )
			{
				HandleException( ex );
			}
			finally
			{
				Invoke( new MethodInvoker( StopTheClock ) );
			}
		}
		#endregion
		
		#region private PreviousFrame method
		private void PreviousFrame()
		{
			_imageIndex--;
			if( _imageIndex < 1 )
			{
				_imageIndex = 0;
			}
			RefreshUI();
		}
		#endregion

		#region private NextFrame method
		private void NextFrame()
		{
			_imageIndex++;
			if( _imageIndex >= _decoder.Frames.Count )
			{
				_imageIndex = _decoder.Frames.Count - 1;
			}
			RefreshUI();
		}
		#endregion
		
		#region private ProvideFeedback method
		/// <summary>
		/// Updates the user interface to provide the user with feedback on the 
		/// progress of a long-running process.
		/// </summary>
		private void RefreshUI()
		{
			// Display the number of the frame we're looking at
			textBoxFrameNumber.Text 
				= (_imageIndex + 1).ToString( CultureInfo.InvariantCulture );
			
			// If the decoder hasn't been instantiated then stop here
			if( _decoder != null )
			{
				if( _decoder.Frames == null || _decoder.Frames.Count == 0 )
				{
					// Decoder isn't initialised so display nothing
					pictureBox1.Image = null;
					propertyGridFrame.SelectedObject = null;
				}
				else
				{
					// Display frame count, current frame and frame properties
					textBoxFrameCount.Text = _decoder.Frames.Count.ToString( CultureInfo.InvariantCulture );
					pictureBox1.Image = _decoder.Frames[_imageIndex].TheImage;

					// If the decoder isn't in a Done state then it could be in
					// the process of decoding a GIF stream, so don't try to
					// update any PropertyGrids else we could get an XmlException
					// due to the decoder's DebugXml being incomplete.
					if( _decoder.State == GifDecoderState.Done )
					{
						propertyGridFrame.SelectedObject = _decoder.Frames[_imageIndex];
						// TODO: update propertyGridFile.SelectedObject here too?
//						propertyGridFile.SelectedObject = _decoder;
					}
				}
			}
		}
		#endregion

		#endregion

		#region protected override StopTheClock method - invoked by background thread
		/// <summary>
		/// Stops the timer, updates the UI and enables any disabled controls.
		/// </summary>
		protected override void StopTheClock()
		{
			propertyGridFile.SelectedObject = _decoder;
			_imageIndex = 0;
			base.StopTheClock();
		}
		#endregion
		
		#region event handlers
		
		#region button click event handler
		[SuppressMessage("Microsoft.Design", 
			             "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void ButtonClick( object sender, EventArgs e )
		{
			try
			{
				Button b = (Button) sender;
				switch( b.Name )
				{
					case "buttonPrevious":
						PreviousFrame();
						break;
						
					case "buttonNext":
						NextFrame();
						break;
						
					default:
						CleverMessageBox.Show( b.Name, this );
						break;
				}
			}
			catch( Exception ex )
			{
				HandleException( ex );
			}
		}
		#endregion

		#region menu item click handler
		[SuppressMessage("Microsoft.Design", 
		                 "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void ToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				ToolStripMenuItem item = (ToolStripMenuItem) sender;
				switch( item.Name )
				{
					case "loadGIFFileToolStripMenuItem":
						DoLoadGif();
						break;
						
					case "extractFramesToBitmapsToolStripMenuItem":
						DoExtractFrames();
						break;
						
					case "createDebugXMLToolStripMenuItem":
						// Do nothing - we check the state of this menu item
						// as part of the decode process.
						break;
						
					case "aboutToolStripMenuItem":
						AboutForm.Show();
						break;
						
					default:
						CleverMessageBox.Show( item.Name, this );
						break;
				}
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

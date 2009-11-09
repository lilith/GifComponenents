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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using GifComponents;
using CommonForms;
#endregion

namespace GifInspector
{
	/// <summary>
	/// The main form in the GIF Inspector application
	/// </summary>
	public partial class MainForm : Form
	{
		#region declarations
		private GifDecoder _decoder;
		private Thread _t;
		private int _imageIndex;
		private Exception _exception;
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

		#region private LoadGif method - runs on background thread
		[SuppressMessage("Microsoft.Design", 
		                 "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void LoadGif()
		{
			try
			{
				_decoder = new GifDecoder( openFileDialog1.FileName );
				_decoder.Decode();
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

		#region private ExtractFrames method - runs on background thread
		private void ExtractFrames()
		{
			if( _decoder != null )
			{
				for( int i = 0; i < _decoder.Frames.Count; i++ )
				{
					string fileName = openFileDialog1.FileName + ".frame " + i + ".bmp";
					// TODO: remove
//					_status = Path.GetFileName( fileName );
					_decoder.Frames[i].TheImage.Save( fileName, ImageFormat.Bmp );
				}
			}
			Invoke( new MethodInvoker( StopTheClock ) );
		}
		#endregion
		
		#region private StopTheClock method
		private void StopTheClock()
		{
			// TODO: remove
//			_status = openFileDialog1.FileName;
			propertyGridFile.SelectedObject = _decoder;
			_imageIndex = 0;
			UpdateUI();
			EnableControls();
			if( _exception != null )
			{
				ExceptionForm ef = new ExceptionForm( _exception );
				ef.ShowDialog();
				_exception = null;
			}
		}
		#endregion
		
		#region private UpdateUI method
		private void UpdateUI()
		{
			if( _decoder != null )
			{
				textBoxStatus.Text = _decoder.Status;
			}
			textBoxFrameNumber.Text = _imageIndex.ToString( CultureInfo.InvariantCulture );
			if( _decoder != null )
			{
				if( _decoder.Frames == null || _decoder.Frames.Count == 0 )
				{
					pictureBox1.Image = null;
					propertyGridFrame.SelectedObject = null;
				}
				else
				{
					textBoxFrameCount.Text = _decoder.Frames.Count.ToString( CultureInfo.InvariantCulture );
					pictureBox1.Image = _decoder.Frames[_imageIndex].TheImage;
					propertyGridFrame.SelectedObject = _decoder.Frames[_imageIndex];
				}
			}
		}
		#endregion

		#region private DisableControls method
		private void DisableControls()
		{
			foreach( Control c in this.Controls )
			{
				Button b = c as Button;
				if( b != null )
				{
					b.Enabled = false;
				}
			}
		}
		#endregion
		
		#region private EnableControls method
		private void EnableControls()
		{
			foreach( Control c in this.Controls )
			{
				Button b = c as Button;
				if( b != null )
				{
					b.Enabled = true;
				}
			}
		}
		#endregion
		
		#endregion

		#region event handlers

		#region ButtonLoadGif click handler
		void ButtonLoadGifClick(object sender, EventArgs e)
		{
			DialogResult result = openFileDialog1.ShowDialog();
			if( result == DialogResult.OK )
			{
				DisableControls();
				// TODO: remove
//				_status = "Loading...";
				_t = new Thread( LoadGif );
				_t.IsBackground = true;
				_t.Start();
				timer1.Start();
			}
		}
		#endregion
		
		#region ButtonPrevious click handler
		void ButtonPreviousClick(object sender, EventArgs e)
		{
			_imageIndex--;
			if( _imageIndex < 1 )
			{
				_imageIndex = 0;
			}
			UpdateUI();
		}
		#endregion

		#region ButtonNext click handler
		void ButtonNextClick(object sender, EventArgs e)
		{
			_imageIndex++;
			if( _imageIndex >= _decoder.Frames.Count )
			{
				_imageIndex = _decoder.Frames.Count - 1;
			}
			UpdateUI();
		}
		#endregion
		
		#region ButtonExtractFrames click handler
		void ButtonExtractFramesClick(object sender, EventArgs e)
		{
			DisableControls();
			// TODO: remove
//			_status = "Extracting...";
			_t = new Thread( ExtractFrames );
			_t.IsBackground = true;
			_t.Start();
			timer1.Start();
		}
		#endregion

		#region timer tick handler
		void Timer1Tick(object sender, EventArgs e)
		{
			UpdateUI();
		}
		#endregion
		
		#region help/about menu item click handler
		[SuppressMessage("Microsoft.Design", 
		                 "CA1031:DoNotCatchGeneralExceptionTypes")]
		void AboutToolStripMenuItemClick(object sender, EventArgs e)
		{
			try
			{
				string fileName = "GifInspector.AboutFormParameters.xml";
				AboutFormParameters parameters
					= AboutFormParameters.LoadXml( fileName );
				AboutForm f = new AboutForm( parameters );
				f.ShowDialog();
			}
			catch( Exception ex )
			{
				ExceptionForm ef = new ExceptionForm( ex );
				ef.ShowDialog();
			}
		}
		#endregion

		#endregion
	}
}

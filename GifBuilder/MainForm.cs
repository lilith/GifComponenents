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
		private AnimatedGifEncoder _encoder;
		private Collection<GifFrame> _frames;
		private int _currentIndex;
		
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
			_frames = new Collection<GifFrame>();
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
						AddFrame( true );
						break;
	
					case "buttonAddFrameAfter":
						AddFrame( false );
						break;
	
					case "buttonRemoveFrame":
						RemoveFrame();
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

		#endregion
		
		#region other private methods
		
		#region AddFrame
		private void AddFrame( bool addBefore )
		{
			DialogResult result = openFileDialog1.ShowDialog();
			if( result == DialogResult.OK )
			{
				Image thisImage = Bitmap.FromFile( openFileDialog1.FileName );
				GifFrame thisFrame = new GifFrame( thisImage );
				if( _frames.Count == 0 )
				{
					_frames.Add( thisFrame );
				}
				else
				{
					if( addBefore )
					{
						_frames.Insert( _currentIndex, thisFrame );
					}
					else
					{
						_frames.Insert( ++_currentIndex, thisFrame );
					}
				}
				RefreshUI();
			}
		}
		#endregion
		
		#region RemoveFrame
		private void RemoveFrame()
		{
			_frames.Remove( _frames[_currentIndex] );
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
		
		#region RefreshUI
		private void RefreshUI()
		{
			#region ensure current index is within frame count
			if( _currentIndex < 0 )
			{
				_currentIndex = 0;
			}
			
			if( _currentIndex >= _frames.Count )
			{
				_currentIndex = _frames.Count - 1;
			}
			#endregion
			
			labelFrameNumber.Text 
				= "Frame " + (_currentIndex + 1)
				+ " of " + _frames.Count;
			
			if( _frames.Count > 0 )
			{
				pictureBox1.Image = _frames[_currentIndex].TheImage;
				pictureBox1.Location = _frames[_currentIndex].Position;
				propertyGridFrame.SelectedObject = _frames[_currentIndex];
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
			
		}
		#endregion
		
		#region Encode
		private void Encode()
		{
			DialogResult result = saveFileDialog1.ShowDialog();
			if( result == DialogResult.OK )
			{
				// Note the encoder was instantiated in this form's constructor
				// and the user has set its properties in the UI.
				foreach( GifFrame thisFrame in _frames )
				{
					_encoder.AddFrame( thisFrame );
				}
				_encoder.WriteToFile( saveFileDialog1.FileName );
			}
		}
		#endregion
		
		#endregion
		
	}
}

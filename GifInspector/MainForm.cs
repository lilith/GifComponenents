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
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
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
		private string _extractPath;
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
		
		// TODO: move TypesToDisable into base class in CommonForms and make it virtual
		#region TypesToDisable property
		/// <summary>
		/// Gets a collection of types of control to disable whilst a background
		/// thread is running.
		/// </summary>
		protected static Collection<Type> TypesToDisable
		{
			get
			{
				Collection<Type> types = new Collection<Type>();
				types.Add( typeof( Button ) );
				types.Add( typeof( PropertyGrid ) );
				types.Add( typeof( MenuStrip ) );
				types.Add( typeof( ToolStripMenuItem ) );
				return types;
			}
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
				
				DisableControls( this );
				_t = new Thread( LoadGif );
				_t.IsBackground = true;
				_t.Start();
				timer1.Start();
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
				_decoder = new GifDecoder( openFileDialog1.FileName, 
				                           createDebugXMLToolStripMenuItem.Checked );
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
				DisableControls( this );
				_extractPath = folderBrowserDialog1.SelectedPath;
				_t = new Thread( ExtractFrames );
				_t.IsBackground = true;
				_t.Start();
				timer1.Start();
			}
		}
		#endregion

		#region private ExtractFrames method - runs on background thread
		private void ExtractFrames()
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
			Invoke( new MethodInvoker( StopTheClock ) );
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
			UpdateUI();
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
			UpdateUI();
		}
		#endregion
		
		// TODO: move ShowAboutForm into base class in CommonForms
		#region private static ShowAboutForm method - called by event handler
		private static void ShowAboutForm()
		{
			string assemblyName = Assembly.GetEntryAssembly().GetName().Name;
			string fileName = assemblyName + ".AboutFormParameters.xml";
			AboutFormParameters parameters
				= AboutFormParameters.LoadXml( fileName );
			AboutForm f = new AboutForm( parameters );
			f.ShowDialog();
		}
		#endregion
		
		#region private StopTheClock method - invoked by background thread
		private void StopTheClock()
		{
			propertyGridFile.SelectedObject = _decoder;
			_imageIndex = 0;
			UpdateUI();
			EnableControls( this );
			if( _exception != null )
			{
				ExceptionForm ef = new ExceptionForm( _exception );
				ef.ShowDialog();
				_exception = null;
			}
		}
		#endregion
		
		#region private UpdateUI method - called by the timer tick event handler
		private void UpdateUI()
		{
			// Display the decoder status
			if( _decoder != null )
			{
				// TODO: replace textBoxStatus with a progress bar (progress form in CommonForms?)
				textBoxStatus.Text = _decoder.Status;
			}
			
			// Display the number of the frame we're looking at
			textBoxFrameNumber.Text 
				= _imageIndex.ToString( CultureInfo.InvariantCulture );
			
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

		// TODO: move DisableControls and EnableControls into a base class in CommonForms
		#region protected EnableControls method
		/// <summary>
		/// Enables all the controls on the supplied control, and their child
		/// controls, provided they are of a type defined in the TypesToDisable
		/// property
		/// </summary>
		/// <param name="parentControl">
		/// The parent control or form which contains the controls to enable.
		/// </param>
		protected void EnableControls( Control parentControl )
		{
			SetControlsEnabled( parentControl, true );
		}
		#endregion
		
		#region protected DisableControls method
		/// <summary>
		/// Disables all the controls on the supplied control, and their child
		/// controls, provided they are of a type defined in the TypesToDisable
		/// property
		/// </summary>
		/// <param name="parentControl">
		/// The parent control or form which contains the controls to disable.
		/// </param>
		protected void DisableControls( Control parentControl )
		{
			SetControlsEnabled( parentControl, false );
		}
		#endregion
		
		#region private SetControlsEnabled method
		private void SetControlsEnabled( Control parentControl, bool enabling )
		{
			foreach( Control c in parentControl.Controls )
			{
				// Is this a type of control that we want to disable whilst 
				// running a process on a background thread?
				if( TypesToDisable.Contains( c.GetType() ) )
				{
					// Is it a MenuStrip?
					MenuStrip ms = c as MenuStrip;
					if( ms == null )
					{
						// Not a MenuStrip so just enable/disable it
						c.Enabled = enabling;
					}
					else
					{
						// It's a MenuStrip so don't disable/enable it but 
						// disable/enable its child items
						foreach( ToolStripMenuItem i in ms.Items )
						{
							// i is an item in the row of menus
							foreach( ToolStripMenuItem tsmi in i.DropDownItems )
							{
								// tsmi is an option within one of the menus
								tsmi.Enabled = enabling;
							}
						}
					}
				}
				
				// disable/enable any child controls too
				SetControlsEnabled( c, enabling );
			}
		}
		#endregion
		
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
				ExceptionForm ef = new ExceptionForm( ex );
				ef.ShowDialog();
			}
		}
		#endregion

		// TODO: rename the timer to BackgroundProcessTimer and move it to base class in CommonForms
		#region timer tick handler
		/// <summary>
		/// Handles the Tick event from the Timer which is started when a 
		/// process is started on a background thread.
		/// </summary>
		/// <param name="sender">The object which raised the Tick event</param>
		/// <param name="e">More information about the Tick event</param>
		[SuppressMessage("Microsoft.Design", 
			             "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void Timer1Tick( object sender, EventArgs e )
		{
			try
			{
				UpdateUI();
			}
			catch( Exception ex )
			{
				ExceptionForm ef = new ExceptionForm( ex );
				ef.ShowDialog();
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
						ShowAboutForm();
						break;
						
					default:
						CleverMessageBox.Show( item.Name, this );
						break;
				}
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

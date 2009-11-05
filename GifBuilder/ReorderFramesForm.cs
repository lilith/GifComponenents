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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using GifComponents;

namespace GifBuilder
{
	/// <summary>
	/// A form which allows the order of the frames in an animation to be
	/// changed.
	/// </summary>
	public partial class ReorderFramesForm : Form
	{
		private AnimatedGifEncoder _encoder;
		
		#region constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="encoder">
		/// The AnimatedGifEncoder whose frames need reordering.
		/// </param>
		public ReorderFramesForm( AnimatedGifEncoder encoder )
		{
			//
			// The InitializeComponent() call is required for Windows Forms 
			// designer support.
			//
			InitializeComponent();
			
			//
			// Add constructor code after the InitializeComponent() call.
			//
			_encoder = encoder;
			
			for( int i = 0; i < _encoder.Frames.Count; i++ )
			{
				dataGridView1.Rows.Add( new Bitmap( _encoder.Frames[i].TheImage, 100, 100 ) );
				dataGridView1.Rows[i].Height = 100;
				dataGridView1.Height = this.Height;
			}
		}
		#endregion
		
		#region button click event handler
		void ButtonClick(object sender, EventArgs e)
		{
			Button button = (Button) sender;
			int frameIndex = dataGridView1.SelectedRows[0].Index;
			GifFrame frameToMove = _encoder.Frames[frameIndex];
			switch( button.Name )
			{
				case "buttonMoveUp":
					if( frameIndex > 0 )
					{
						_encoder.Frames.Remove( frameToMove );
						_encoder.Frames.Insert( frameIndex - 1, frameToMove );
						dataGridView1.Rows[frameIndex-1].Selected = true;
						UpdateDataGridView();
					}
					break;
					
				case "buttonMoveDown":
					if( frameIndex < _encoder.Frames.Count - 1 )
					{
						_encoder.Frames.Remove( frameToMove );
						_encoder.Frames.Insert( frameIndex + 1, frameToMove );
						dataGridView1.Rows[frameIndex+1].Selected = true;
						UpdateDataGridView();
					}
					break;
					
				default:
					string message = "Unexpected button name: " + button.Name;
					throw new InvalidOperationException( message );
			}
		}
		#endregion
		
		#region UpdateDataGridView method
		private void UpdateDataGridView()
		{
			for( int i = 0; i < _encoder.Frames.Count; i++ )
			{
				dataGridView1.Rows[i].Cells[0].Value 
					= new Bitmap( _encoder.Frames[i].TheImage, 100, 100 );
			}
		}
		#endregion
	}
}

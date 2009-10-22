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
using System.IO;
using NUnit.Framework;

namespace GifComponents.NUnit
{
	/// <summary>
	/// Test fixture for the GifFrame class.
	/// TODO: aim for 100% coverage of GifFrame class
	/// </summary>
	[TestFixture]
	public class GifFrameTest
	{
		private GifFrame _frame;
		
		#region Constructor2Test
		/// <summary>
		/// Tests the constructor which accepts an image, colour table, image
		/// descriptor and graphic control extension as parameters.
		/// </summary>
		[Test]
		public void Constructor2Test()
		{
			ColourTable ct = WikipediaExample.GlobalColourTable;
			WikipediaExample.CheckGlobalColourTable( ct );
			ImageDescriptor id = WikipediaExample.ImageDescriptor;
			GraphicControlExtension gce = WikipediaExample.GraphicControlExtension;
			LogicalScreenDescriptor lsd = WikipediaExample.LogicalScreenDescriptor;
			WikipediaExample.CheckLogicalScreenDescriptor( lsd );

			TableBasedImageData indexedPixels = WikipediaExample.ImageData;
			
			_frame = new GifFrame( indexedPixels, ct, id, gce, WikipediaExample.BackgroundColour, lsd, null, null );

			Assert.AreEqual( ErrorState.Ok, _frame.ConsolidatedState );
			
			WikipediaExample.CheckBitmap( _frame.TheImage );
			WikipediaExample.CheckImageDescriptor( _frame.ImageDescriptor );
			WikipediaExample.CheckGraphicControlExtension( _frame.GraphicControlExtension );
			WikipediaExample.CheckImageData( _frame.IndexedPixels );
			Assert.AreEqual( 0, _frame.BackgroundColour.R );
			Assert.AreEqual( 0, _frame.BackgroundColour.G );
			Assert.AreEqual( 0, _frame.BackgroundColour.B );
		}
		#endregion

		#region FromStreamTest
		/// <summary>
		/// Checks that the FromStream method works correctly under normal
		/// circumstances.
		/// </summary>
		[Test]
		public void FromStreamTest()
		{
			MemoryStream s = new MemoryStream();
			
			// Image descriptor
			byte[] idBytes = WikipediaExample.ImageDescriptorBytes;
			s.Write( idBytes, 0, idBytes.Length );
			
			// Table-based image data
			byte[] imageData = WikipediaExample.ImageDataBytes;
			s.Write( imageData, 0, imageData.Length );

			s.Seek( 0, SeekOrigin.Begin );

			// Extra stuff not included in the frame stream, to pass to the
			// FromStream method
			ColourTable colourTable = WikipediaExample.GlobalColourTable;
			GraphicControlExtension ext = WikipediaExample.GraphicControlExtension;
			LogicalScreenDescriptor lsd = WikipediaExample.LogicalScreenDescriptor;

			_frame = GifFrame.FromStream( s, lsd, colourTable, ext, null, null );
			
			Assert.AreEqual( ErrorState.Ok, _frame.ConsolidatedState );

			WikipediaExample.CheckBitmap( _frame.TheImage );
			WikipediaExample.CheckImageDescriptor( _frame.ImageDescriptor );
			WikipediaExample.CheckGraphicControlExtension( _frame.GraphicControlExtension );
			WikipediaExample.CheckImageData( _frame.IndexedPixels );
			Assert.AreEqual( 0, _frame.BackgroundColour.R );
			Assert.AreEqual( 0, _frame.BackgroundColour.G );
			Assert.AreEqual( 0, _frame.BackgroundColour.B );
		}
		#endregion
		
	}
}

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
using System.Drawing.Imaging;
using System.IO;
using NUnit.Extensions;
using NUnit.Framework;

namespace GifComponents.NUnit
{
	/// <summary>
	/// Test fixture for the GifFrame class.
	/// TODO: aim for 100% coverage of GifFrame class
	/// </summary>
	[TestFixture]
	public class GifFrameTest : IDisposable
	{
		private GifFrame _frame;
		private GifDecoder _decoder;
		private AnimatedGifEncoder _encoder;
		
		#region Setup method
		/// <summary>
		/// Instantiates the GifFrame, and also tests the default properties 
		/// that it sets.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			_frame = new GifFrame( WikipediaExample.ExpectedBitmap );
			Assert.AreEqual( 10, _frame.Delay );
			BitmapAssert.AreEqual( WikipediaExample.ExpectedBitmap, 
			                       (Bitmap) _frame.TheImage );
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
			MemoryStream s = CreateStream();

			// Extra stuff not included in the frame stream, to pass to the
			// FromStream method
			ColourTable colourTable = WikipediaExample.GlobalColourTable;
			GraphicControlExtension ext = WikipediaExample.GraphicControlExtension;
			LogicalScreenDescriptor lsd = WikipediaExample.LogicalScreenDescriptor;

			_frame = GifFrame.FromStream( s, lsd, colourTable, ext, null, null );
			
			Assert.AreEqual( ErrorState.Ok, _frame.ConsolidatedState );

			WikipediaExample.CheckImage( _frame.TheImage );
			WikipediaExample.CheckImageDescriptor( _frame.ImageDescriptor );
			WikipediaExample.CheckGraphicControlExtension( _frame.GraphicControlExtension );
			WikipediaExample.CheckImageData( _frame.IndexedPixels );
			Assert.AreEqual( 0, _frame.BackgroundColour.R );
			Assert.AreEqual( 0, _frame.BackgroundColour.G );
			Assert.AreEqual( 0, _frame.BackgroundColour.B );
		}
		#endregion
		
		#region FromStreamNoImageDataTest
		/// <summary>
		/// Checks that the FromStream method sets the correct status when
		/// decoding a stream which contains no image data.
		/// FIXME: unable to cause a NoImageData status at the moment
		/// </summary>
		[Test]
		[Ignore( "Causes an IndexOutOfRangeException in TableBasedImageData constructor" )]
		public void FromStreamNoImageDataTest()
		{
			MemoryStream s = new MemoryStream();
			
			// Image descriptor
			byte[] idBytes = WikipediaExample.ImageDescriptorBytes;
			s.Write( idBytes, 0, idBytes.Length );

			// TODO: remove (this is how we miss out the image data)
//			// Table-based image data
//			byte[] imageData = WikipediaExample.ImageDataBytes;
//			s.Write( imageData, 0, imageData.Length );
			byte[] imageData = new byte[]
			{
				0x08, // LZW minimum code size
//				0x00, // block size = 11
//				// 11 bytes of LZW encoded data follows
//				0x00, 0x01, 0x04, 0x18, 0x28, 0x70, 0xA0, 0xC1, 0x83, 0x01, 0x01,
				0x00 // block terminator
			};
			s.Write( imageData, 0, imageData.Length );

			s.Seek( 0, SeekOrigin.Begin );
			
			// Extra stuff not included in the frame stream, to pass to the
			// FromStream method
			ColourTable colourTable = WikipediaExample.GlobalColourTable;
			GraphicControlExtension ext = WikipediaExample.GraphicControlExtension;
			LogicalScreenDescriptor lsd = WikipediaExample.LogicalScreenDescriptor;

			_frame = GifFrame.FromStream( s, lsd, colourTable, ext, null, null );
			
			Assert.AreEqual( ErrorState.FrameHasNoImageData, 
			                 _frame.ConsolidatedState );
		}
		#endregion
		
		#region FromStreamNullLogicalScreenDescriptorTest
		/// <summary>
		/// Checks that the correct exception is thrown when the FromStream 
		/// method is passed a null logical screen descriptor.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		public void FromStreamNullLogicalScreenDescriptorTest()
		{
			MemoryStream s = CreateStream();

			// Extra stuff not included in the frame stream, to pass to the
			// FromStream method
			ColourTable colourTable = WikipediaExample.GlobalColourTable;
			GraphicControlExtension ext = WikipediaExample.GraphicControlExtension;
			LogicalScreenDescriptor lsd = null;

			try
			{
				_frame = GifFrame.FromStream( s, lsd, colourTable, ext, null, null );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "lsd", ex.ParamName );
				throw;
			}
		}
		#endregion
		
		#region FromStreamNullGraphicControlExtensionTest
		/// <summary>
		/// Checks that the correct error state is set when the FromStream 
		/// method is passed a null graphic control extension.
		/// </summary>
		[Test]
		public void FromStreamNullGraphicControlExtensionTest()
		{
			_decoder = new GifDecoder( @"images\NoGraphicControlExtension.gif" );
			_decoder.Decode();
			Assert.AreEqual( ErrorState.NoGraphicControlExtension, 
			                 _decoder.Frames[0].ErrorState );
			Bitmap expected = new Bitmap( @"images\NoGraphicControlExtension.bmp" );
			Bitmap actual = (Bitmap) _decoder.Frames[0].TheImage;
			BitmapAssert.AreEqual( expected, actual );
		}
		#endregion
		
		#region TransparencyTest
		/// <summary>
		/// Checks that transparent pixels are encoded and decoded correctly.
		/// </summary>
		[Test]
		public void TransparencyTest()
		{
			Color noColour = Color.FromArgb( 0, 0, 0, 0 ); // note alpha of 0
			Color blue = Color.FromArgb( 0, 0, 255 );
			Color transparent = Color.FromArgb( 100, 100, 100 );
			_encoder = new AnimatedGifEncoder();
			_encoder.Transparent = transparent;
			
			// transparent | transparent
			// -------------------------
			// blue        | blue
			Bitmap bitmap = new Bitmap( 2, 2 );
			bitmap.SetPixel( 0, 0, transparent );
			bitmap.SetPixel( 1, 0, transparent );
			bitmap.SetPixel( 0, 1, blue );
			bitmap.SetPixel( 1, 1, blue );
			_frame = new GifFrame( bitmap );
			_encoder.AddFrame( _frame );
			
			// encode and decode
			string filename = "GifFrameTest.TransparencyTest.gif";
			_encoder.WriteToFile( filename );
			_decoder = new GifDecoder( filename );
			_decoder.Decode();
			
			// Result should be:
			// black | black
			// -------------
			// blue  | blue
			
			Bitmap actual = (Bitmap) _decoder.Frames[0].TheImage;
			// TODO: decoder returns transparent pixels as black - is this correct?
			Assert.AreEqual( noColour, actual.GetPixel( 0, 0 ) );
			Assert.AreEqual( noColour, actual.GetPixel( 1, 0 ) );
			Assert.AreEqual( blue, actual.GetPixel( 0, 1 ) );
			Assert.AreEqual( blue, actual.GetPixel( 1, 1 ) );
			
		}
		#endregion
		
		#region InterlaceTest
		/// <summary>
		/// Checks that an interlaced image can be decoded correctly.
		/// </summary>
		[Test]
		public void InterlaceTest()
		{
			_decoder = new GifDecoder( @"images\Interlaced.gif" );
			_decoder.Decode();
			Assert.AreEqual( true, _decoder.Frames[0].ImageDescriptor.IsInterlaced );
			Bitmap expected = new Bitmap( @"images\Interlaced.bmp" );
			Bitmap actual = (Bitmap) _decoder.Frames[0].TheImage;
			BitmapAssert.AreEqual( expected, actual );
		}
		#endregion
		
		#region property tests
		
		#region BackgroundColourTest
		/// <summary>
		/// Checks that the BackgroundColour property works correctly.
		/// </summary>
		[Test]
		public void BackgroundColourTest()
		{
			for( int r = 0; r < 255; r += 3 )
			{
				for( int g = 0; g < 255; g += 7 )
				{
					for( int b = 0; b < 255; b += 11 )
					{
						Color c = Color.FromArgb( 0, r, g, b );
						_frame.BackgroundColour = c;
						Assert.AreEqual( c, _frame.BackgroundColour );
					}
				}
			}
		}
		#endregion
		
		#region DelayTest
		/// <summary>
		/// Checks that the Delay property works correctly.
		/// </summary>
		[Test]
		public void DelayTest()
		{
			for( int delay = 0; delay < 200; delay++ )
			{
				_frame.Delay = delay;
				Assert.AreEqual( delay, _frame.Delay );
			}
		}
		#endregion
		
		#region ExpectsUserInputNewFrameTest
		/// <summary>
		/// Checks that the ExpectsUserInput property works correctly.
		/// </summary>
		[Test]
		public void ExpectsUserInputNewFrameTest()
		{
			_frame.ExpectsUserInput = true;
			Assert.AreEqual( true, _frame.ExpectsUserInput );
			_frame.ExpectsUserInput = false;
			Assert.AreEqual( false, _frame.ExpectsUserInput );
		}
		#endregion
		
		#region ExpectsUserInputDecodedFrameGetTest
		/// <summary>
		/// Checks that the ExpectsUserInput property of a GifFrame which was
		/// created by a GifDecoder returns the same value as the 
		/// ExpectsUserInput property of its graphic control extension.
		/// </summary>
		[Test]
		public void ExpectsUserInputDecodedFrameGetTest()
		{
			_decoder = new GifDecoder( @"images\smiley\smiley.gif" );
			_decoder.Decode();
			for( int i = 0; i < _decoder.Frames.Count; i++ )
			{
				_frame = _decoder.Frames[i];
				Assert.AreEqual( _frame.GraphicControlExtension.ExpectsUserInput, 
				                 _frame.ExpectsUserInput, 
				                 "Frame " + i );
			}
		}
		#endregion
		
		#region ExpectsUserInputDecodedFrameSetTest
		/// <summary>
		/// Checks that the correct exception is thrown when an attempt is made
		/// to set the ExpectsUserInput property of a GifFrame which was created
		/// by a GifDecoder.
		/// </summary>
		[Test]
		[ExpectedException( typeof( InvalidOperationException ) )]
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1702:CompoundWordsShouldBeCasedCorrectly", 
		                 MessageId = "FrameSet")]
		public void ExpectsUserInputDecodedFrameSetTest()
		{
			_decoder = new GifDecoder( @"images\smiley\smiley.gif" );
			_decoder.Decode();
			_frame = _decoder.Frames[0];
			try
			{
				_frame.ExpectsUserInput = true;
			}
			catch( InvalidOperationException ex )
			{
				string message
					= "This GifFrame was returned by a GifDecoder so this "
					+ "property is read-only";
				StringAssert.Contains( message, ex.Message );
				throw;
			}
		}
		#endregion
		
		#region PositionNewFrameTest
		/// <summary>
		/// Checks that the Position property works correctly on a GifFrame
		/// created by the constructor.
		/// </summary>
		[Test]
		public void PositionTest()
		{
			for( int x = 0; x < 100; x += 7 )
			{
				for( int y = 0; y < 200; y += 11 )
				{
					Point position = new Point( x, y );
					_frame.Position = position;
					Assert.AreEqual( position, _frame.Position );
				}
			}
		}
		#endregion

		#region PositionDecodedFrameGetTest
		/// <summary>
		/// Checks that the Position property of a GifFrame which was created
		/// by a GifDecoder returns the same value as the Position property of
		/// its image descriptor.
		/// </summary>
		[Test]
		public void PositionDecodedFrameGetTest()
		{
			_decoder = new GifDecoder( @"images\smiley\smiley.gif" );
			_decoder.Decode();
			for( int i = 0; i < _decoder.Frames.Count; i++ )
			{
				_frame = _decoder.Frames[i];
				Assert.AreEqual( _frame.ImageDescriptor.Position, 
				                 _frame.Position, 
				                 "Frame " + i );
			}
		}
		#endregion
		
		#region PositionDecodedFrameSetTest
		/// <summary>
		/// Checks that the correct exception is thrown when an attempt is made
		/// to set the Position property of a GifFrame which was created by
		/// a GifDecoder.
		/// </summary>
		[Test]
		[ExpectedException( typeof( InvalidOperationException ) )]
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1702:CompoundWordsShouldBeCasedCorrectly", 
		                 MessageId = "FrameSet")]
		public void PositionDecodedFrameSetTest()
		{
			_decoder = new GifDecoder( @"images\smiley\smiley.gif" );
			_decoder.Decode();
			_frame = _decoder.Frames[0];
			try
			{
				_frame.Position = new Point( 1, 1 );
			}
			catch( InvalidOperationException ex )
			{
				string message
					= "This GifFrame was returned by a GifDecoder so this "
					+ "property is read-only";
				StringAssert.Contains( message, ex.Message );
				throw;
			}
		}
		#endregion
		
		#endregion

		#region private CreateStream method
		private static MemoryStream CreateStream()
		{
			MemoryStream s = new MemoryStream();
			
			// Image descriptor
			byte[] idBytes = WikipediaExample.ImageDescriptorBytes;
			s.Write( idBytes, 0, idBytes.Length );
			
			// Table-based image data
			byte[] imageData = WikipediaExample.ImageDataBytes;
			s.Write( imageData, 0, imageData.Length );

			s.Seek( 0, SeekOrigin.Begin );
			
			return s;
		}
		#endregion

		#region IDisposable implementation
		private bool _isDisposed; // defaults to false
		
		/// <summary>
		/// Disposes resources used by this class.
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( true );
		}
		
		/// <summary>
		/// Disposes resources used by this class.
		/// </summary>
		/// <param name="isDisposing">
		/// Indicates whether this method is being called by the class's Dispose
		/// method (true) or by the garbage collector (false).
		/// </param>
		protected virtual void Dispose( bool isDisposing )
		{
			if( _isDisposed )
			{
				return;
			}
			
			if( isDisposing )
			{
				_decoder.Dispose();
			}
			
			_isDisposed = true;
		}
		#endregion
	}
}

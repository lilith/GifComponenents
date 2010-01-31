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
using GifComponents.Components;

namespace GifComponents.NUnit.Components
{
	/// <summary>
	/// Test fixture for the GifFrame class.
	/// </summary>
	[TestFixture]
	public class GifFrameTest : GifComponentTestFixtureBase, IDisposable
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
		
		#region ConstructorStreamTest
		/// <summary>
		/// Checks that the constructor( Stream )works correctly under normal
		/// circumstances.
		/// </summary>
		[Test]
		public void ConstructorStreamTest()
		{
			ReportStart();
			ConstructorStreamTest( true );
			ConstructorStreamTest( false );
			ReportEnd();
		}
		
		private void ConstructorStreamTest( bool xmlDebugging )
		{
			MemoryStream s = CreateStream();

			// Extra stuff not included in the frame stream, to pass to the
			// constructor.
			ColourTable colourTable = WikipediaExample.GlobalColourTable;
			GraphicControlExtension ext = WikipediaExample.GraphicControlExtension;
			LogicalScreenDescriptor lsd = WikipediaExample.LogicalScreenDescriptor;

			_frame = new GifFrame( s, lsd, colourTable, ext, null, null, xmlDebugging );
			
			Assert.AreEqual( ErrorState.Ok, _frame.ConsolidatedState );

			WikipediaExample.CheckImage( _frame.TheImage );
			WikipediaExample.CheckImageDescriptor( _frame.ImageDescriptor );
			WikipediaExample.CheckGraphicControlExtension( _frame.GraphicControlExtension );
			WikipediaExample.CheckImageData( _frame.IndexedPixels );
			Assert.AreEqual( 0, _frame.BackgroundColour.R );
			Assert.AreEqual( 0, _frame.BackgroundColour.G );
			Assert.AreEqual( 0, _frame.BackgroundColour.B );
			
			if( xmlDebugging )
			{
				Assert.AreEqual( ExpectedDebugXml, _frame.DebugXml );
			}
		}
		#endregion
		
		#region ConstructorStreamNoImageDataTest
		/// <summary>
		/// Checks that the constructor( Stream ) sets the correct status when
		/// decoding a stream which contains no image data.
		/// </summary>
		[Test]
		public void ConstructorStreamNoImageDataTest()
		{
			ReportStart();
			ConstructorStreamNoImageDataTest( true );
			ConstructorStreamNoImageDataTest( false );
			ReportEnd();
		}
		
		private void ConstructorStreamNoImageDataTest( bool xmlDebugging )
		{
			MemoryStream s = new MemoryStream();
			
			// Image descriptor
			byte[] idBytes = WikipediaExample.ImageDescriptorBytes;
			s.Write( idBytes, 0, idBytes.Length );

			// miss out the image data
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
			// constructor
			ColourTable colourTable = WikipediaExample.GlobalColourTable;
			GraphicControlExtension ext = WikipediaExample.GraphicControlExtension;
			LogicalScreenDescriptor lsd = WikipediaExample.LogicalScreenDescriptor;

			_frame = new GifFrame( s, lsd, colourTable, ext, null, null, xmlDebugging );
			
			Assert.AreEqual( ErrorState.TooFewPixelsInImageData, 
			                 _frame.ConsolidatedState );
			
			if( xmlDebugging )
			{
				Assert.AreEqual( ExpectedDebugXml, _frame.DebugXml );
			}
		}
		#endregion
		
		#region ConstructorStreamNullLogicalScreenDescriptorTest
		/// <summary>
		/// Checks that the correct exception is thrown when the 
		/// constructor( Stream ) is passed a null logical screen descriptor.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		public void ConstructorStreamNullLogicalScreenDescriptorTest()
		{
			MemoryStream s = CreateStream();

			// Extra stuff not included in the frame stream, to pass to the
			// constructor
			ColourTable colourTable = WikipediaExample.GlobalColourTable;
			GraphicControlExtension ext = WikipediaExample.GraphicControlExtension;
			LogicalScreenDescriptor lsd = null;

			try
			{
				_frame = new GifFrame( s, lsd, colourTable, ext, null, null );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "lsd", ex.ParamName );
				throw;
			}
		}
		#endregion
		
		#region ConstructorStreamNullGraphicControlExtensionTest
		/// <summary>
		/// Checks that the correct error state is set when the  
		/// constructor( Stream ) is passed a null graphic control extension.
		/// </summary>
		[Test]
		public void ConstructorStreamNullGraphicControlExtensionTest()
		{
			ReportStart();
			ConstructorStreamNullGraphicControlExtensionTest( true );
			ConstructorStreamNullGraphicControlExtensionTest( false );
			ReportEnd();
		}
		
		private void ConstructorStreamNullGraphicControlExtensionTest( bool xmlDebugging )
		{
			_decoder = new GifDecoder( @"images\NoGraphicControlExtension.gif", 
			                           xmlDebugging );
			_decoder.Decode();
			Assert.AreEqual( ErrorState.NoGraphicControlExtension, 
			                 _decoder.Frames[0].ErrorState );
			Bitmap expected = new Bitmap( @"images\NoGraphicControlExtension.bmp" );
			Bitmap actual = (Bitmap) _decoder.Frames[0].TheImage;
			BitmapAssert.AreEqual( expected, actual );
			
			if( xmlDebugging )
			{
				Assert.AreEqual( ExpectedDebugXml, _decoder.Frames[0].DebugXml );
			}
		}
		#endregion
		
		#region DecoderStreamNoColourTableTest
		/// <summary>
		/// Checks that the correct error status is set when decoding a frame
		/// which has neither local nor global colour table.
		/// </summary>
		[Test]
		public void ConstructorStreamNoColourTableTest()
		{
			ReportStart();
			_decoder = new GifDecoder( @"images\NoColourTable.gif" );
			_decoder.Decode();
			Assert.AreEqual( ErrorState.FrameHasNoColourTable, _decoder.ConsolidatedState );
			ReportEnd();
		}
		#endregion
		
		#region TransparencyTest
		/// <summary>
		/// Checks that transparent pixels are encoded and decoded correctly.
		/// </summary>
		[Test]
		public void TransparencyTest()
		{
			ReportStart();
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
			Assert.AreEqual( ErrorState.Ok, _decoder.ConsolidatedState );
			
			// Result should be:
			// black | black
			// -------------
			// blue  | blue
			
			Bitmap actual = (Bitmap) _decoder.Frames[0].TheImage;
			Assert.AreEqual( noColour, actual.GetPixel( 0, 0 ) );
			Assert.AreEqual( noColour, actual.GetPixel( 1, 0 ) );
			Assert.AreEqual( blue, actual.GetPixel( 0, 1 ) );
			Assert.AreEqual( blue, actual.GetPixel( 1, 1 ) );
			
			ReportEnd();
		}
		#endregion
		
		#region InterlaceTest
		/// <summary>
		/// Checks that an interlaced image can be decoded correctly.
		/// </summary>
		[Test]
		public void InterlaceTest()
		{
			ReportStart();
			_decoder = new GifDecoder( @"images\Interlaced.gif" );
			_decoder.Decode();
			Assert.AreEqual( true, _decoder.Frames[0].ImageDescriptor.IsInterlaced );
			Bitmap expected = new Bitmap( @"images\Interlaced.bmp" );
			Bitmap actual = (Bitmap) _decoder.Frames[0].TheImage;
			BitmapAssert.AreEqual( expected, actual );
			ReportEnd();
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
			ReportStart();
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
			ReportEnd();
		}
		#endregion
		
		#region DelayTest
		/// <summary>
		/// Checks that the Delay property works correctly.
		/// </summary>
		[Test]
		public void DelayTest()
		{
			ReportStart();
			for( int delay = 0; delay < 200; delay++ )
			{
				_frame.Delay = delay;
				Assert.AreEqual( delay, _frame.Delay );
			}
			ReportEnd();
		}
		#endregion
		
		#region ExpectsUserInputNewFrameTest
		/// <summary>
		/// Checks that the ExpectsUserInput property works correctly.
		/// </summary>
		[Test]
		public void ExpectsUserInputNewFrameTest()
		{
			ReportStart();
			_frame.ExpectsUserInput = true;
			Assert.AreEqual( true, _frame.ExpectsUserInput );
			_frame.ExpectsUserInput = false;
			Assert.AreEqual( false, _frame.ExpectsUserInput );
			ReportEnd();
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
			ReportStart();
			_decoder = new GifDecoder( @"images\smiley\smiley.gif" );
			_decoder.Decode();
			for( int i = 0; i < _decoder.Frames.Count; i++ )
			{
				_frame = _decoder.Frames[i];
				Assert.AreEqual( _frame.GraphicControlExtension.ExpectsUserInput, 
				                 _frame.ExpectsUserInput, 
				                 "Frame " + i );
			}
			ReportEnd();
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
			ReportStart();
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
				ReportEnd();
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
			ReportStart();
			for( int x = 0; x < 100; x += 7 )
			{
				for( int y = 0; y < 200; y += 11 )
				{
					Point position = new Point( x, y );
					_frame.Position = position;
					Assert.AreEqual( position, _frame.Position );
				}
			}
			ReportEnd();
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
			ReportStart();
			_decoder = new GifDecoder( @"images\smiley\smiley.gif" );
			_decoder.Decode();
			for( int i = 0; i < _decoder.Frames.Count; i++ )
			{
				_frame = _decoder.Frames[i];
				Assert.AreEqual( _frame.ImageDescriptor.Position, 
				                 _frame.Position, 
				                 "Frame " + i );
			}
			ReportEnd();
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
			ReportStart();
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
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region PaletteNullTest
		/// <summary>
		/// Checks that the correct exception is thrown when the Palette 
		/// property is set to null.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		public void PaletteNullTest()
		{
			try
			{
				_frame.Palette = null;
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "value", ex.ParamName );
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
		/// <summary>
		/// Indicates whether or not the Dispose( bool ) method has already been 
		/// called.
		/// </summary>
		bool _disposed;

		/// <summary>
		/// Finalzer.
		/// </summary>
		~GifFrameTest()
		{
			Dispose( false );
		}

		/// <summary>
		/// Disposes resources used by this class.
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/// <summary>
		/// Disposes resources used by this class.
		/// </summary>
		/// <param name="disposing">
		/// Indicates whether this method is being called by the class's Dispose
		/// method (true) or by the garbage collector (false).
		/// </param>
		protected virtual void Dispose( bool disposing )
		{
			if( !_disposed )
			{
				if( disposing )
				{
					// dispose-only, i.e. non-finalizable logic
					_frame.Dispose();
					_decoder.Dispose();
					_encoder.Dispose();
				}

				// new shared cleanup logic
				_disposed = true;
			}

			// Uncomment if the base type also implements IDisposable
//			base.Dispose( disposing );
		}
		#endregion
	}
}

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
using System.IO;
using NUnit.Framework;

namespace GifComponents.NUnit
{
	/// <summary>
	/// Test fixture for the TableBasedImageData class.
	/// TODO: aim for 100% coverage of TableBasedImageData.cs
	/// </summary>
	[TestFixture]
	public class TableBasedImageDataTest
	{
		private TableBasedImageData _tbid;
		
		#region ConstructorTest
		/// <summary>
		/// Tests the FromStream method using the example from
		/// http://en.wikipedia.org/wiki/Gif#Example_.gif_file
		/// </summary>
		[Test]
		public void ConstructorTest()
		{
			byte[] bytes = WikipediaExample.ImageDataBytes;
			MemoryStream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Seek( 0, SeekOrigin.Begin );
			
			int pixelCount = WikipediaExample.FrameSize.Width
							* WikipediaExample.FrameSize.Height;
			_tbid = new TableBasedImageData( s, pixelCount );

			Assert.AreEqual( ErrorState.Ok, _tbid.ConsolidatedState );
			WikipediaExample.CheckImageData( _tbid );
		}
		#endregion
		
		#region PixelCountTooSmall
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor
		/// is passed a pixel count that is less than 1.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void PixelCountTooSmall()
		{
			int pixelCount = 0;
			try
			{
				_tbid = new TableBasedImageData( new MemoryStream(), pixelCount );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "The pixel count must be greater than zero. " 
					+ "Supplied value was " + pixelCount;
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "pixelCount", ex.ParamName );
				throw;
			}
		}
		#endregion
		
		#region BlockTerminatorTest
		/// <summary>
		/// Tests the FromStream method where the end of the data is marked by
		/// a block terminator because no end-of-information code is supplied.
		/// </summary>
		[Test]
		public void BlockTerminatorTest()
		{
			byte[] bytes = new byte[]
			{
				0x08, // LZW minimum code size
				0x05, // block size = 5
				// 5 bytes of LZW encoded data follows
				0x00, 0x51, 0xFC, 0x1B, 0x28, 
				0x06, // block size = 6
				// 6 bytes of LZW encoded data follows
				0x70, 0xA0, 0xC1, 0x83, 0x01, 0x01,
				0x00, // block terminator - end of table based image data
			};
			MemoryStream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Seek( 0, SeekOrigin.Begin );
			
			int pixelCount = WikipediaExample.FrameSize.Width
							* WikipediaExample.FrameSize.Height;
			_tbid = new TableBasedImageData( s, pixelCount );
			
			Assert.AreEqual( ErrorState.Ok, _tbid.ConsolidatedState );

			Assert.AreEqual( 15, _tbid.Pixels.Count );
			Assert.AreEqual( ErrorState.Ok, _tbid.ConsolidatedState );
			Assert.AreEqual( 8, _tbid.LzwMinimumCodeSize );
			Assert.AreEqual( 9, _tbid.InitialCodeSize );
			Assert.AreEqual( Math.Pow( 2, 8 ), _tbid.ClearCode );
			Assert.AreEqual( Math.Pow( 2, 8 ) + 1, _tbid.EndOfInformation );
			
			IndexedPixels expectedIndices = new IndexedPixels();
			
			expectedIndices.Add( 40 ); // first pixel is black - index 0 in colour table
			expectedIndices.Add( 255 ); // 2nd pixel is white - index 255 in colour table
			expectedIndices.Add( 255 ); // 3rd pixel
			expectedIndices.Add( 255 ); // 4th pixel
			expectedIndices.Add( 40 ); // 5th pixel
			expectedIndices.Add( 255 ); // 6th pixel
			expectedIndices.Add( 255 ); // 7th pixel
			expectedIndices.Add( 255 ); // 8th pixel
			expectedIndices.Add( 255 ); // 9th pixel
			expectedIndices.Add( 255 ); // 10th pixel
			expectedIndices.Add( 255 ); // 11th pixel
			expectedIndices.Add( 255 ); // 12th pixel
			expectedIndices.Add( 255 ); // 13th pixel
			expectedIndices.Add( 255 ); // 14th pixel
			expectedIndices.Add( 255 ); // 15th pixel
			
			for( int i = 0; i < 15; i++ )
			{
				Assert.AreEqual( expectedIndices[i], _tbid.Pixels[i], "pixel " + i );
			}
		}
		#endregion
		
		#region MinimumCodeSizeTooLarge
		/// <summary>
		/// Checks that the correct error status is set when the LZW minimum 
		/// code size is too large for a stack of 4096 codes.
		/// </summary>
		[Test]
		public void MinimumCodeSizeTooLarge()
		{
			byte[] bytes = new byte[]
			{
				12, // LZW minimum code size
			};
			MemoryStream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Seek( 0, SeekOrigin.Begin );

			_tbid = new TableBasedImageData( s, 25 );
			
			// Processing will be abandoned before any pixels are set
			Assert.AreEqual( ErrorState.LzwMinimumCodeSizeTooLarge, 
			                 _tbid.ConsolidatedState );
			
			// TBID will still return the number of pixels passed to the constructor
			Assert.AreEqual( 25, _tbid.Pixels.Count );
			
			Assert.AreEqual( 12, _tbid.LzwMinimumCodeSize );
			Assert.AreEqual( 13, _tbid.InitialCodeSize );
			Assert.AreEqual( Math.Pow( 2, 12 ), _tbid.ClearCode );
			Assert.AreEqual( Math.Pow( 2, 12 ) + 1, _tbid.EndOfInformation );
		}
		#endregion

		#region MissingPixels
		/// <summary>
		/// Checks that the correct error status is set when the input stream
		/// does not contain enough data for the image size supplied to the
		/// constructor, and that missing pixels are populated correctly.
		/// </summary>
		[Test]
		public void MissingPixels()
		{
			byte[] bytes = WikipediaExample.ImageDataBytes;
			MemoryStream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Seek( 0, SeekOrigin.Begin );
			
			// The stream contains 15 pixels, this image size implies 18 pixels
			_tbid = new TableBasedImageData( s, 18 );
			
			Assert.AreEqual( 18, _tbid.Pixels.Count );
			Assert.AreEqual( ErrorState.TooFewPixelsInImageData, _tbid.ConsolidatedState );
			Assert.AreEqual( 8, _tbid.LzwMinimumCodeSize );
			Assert.AreEqual( 9, _tbid.InitialCodeSize );
			Assert.AreEqual( Math.Pow( 2, 8 ), _tbid.ClearCode );
			Assert.AreEqual( Math.Pow( 2, 8 ) + 1, _tbid.EndOfInformation );
			
			IndexedPixels expectedIndices = new IndexedPixels();

			expectedIndices.Add( 0 ); // first pixel is black - index 0 in colour table
			expectedIndices.Add( 1 ); // 2nd pixel is white - index 1 in colour table
			expectedIndices.Add( 1 ); // 3rd pixel
			expectedIndices.Add( 1 ); // 4th pixel
			expectedIndices.Add( 0 ); // 5th pixel
			expectedIndices.Add( 1 ); // 6th pixel
			expectedIndices.Add( 1 ); // 7th pixel
			expectedIndices.Add( 1 ); // 8th pixel
			expectedIndices.Add( 1 ); // 9th pixel
			expectedIndices.Add( 1 ); // 10th pixel
			expectedIndices.Add( 1 ); // 11th pixel
			expectedIndices.Add( 1 ); // 12th pixel
			expectedIndices.Add( 1 ); // 13th pixel
			expectedIndices.Add( 1 ); // 14th pixel
			expectedIndices.Add( 1 ); // 15th pixel
			expectedIndices.Add( 0 ); // 16th pixel
			expectedIndices.Add( 0 ); // 17th pixel
			expectedIndices.Add( 0 ); // 18th pixel
			
			for( int i = 0; i < 18; i++ )
			{
				Assert.AreEqual( expectedIndices[i], _tbid.Pixels[i], "pixel " + i );
			}
		}
		#endregion
		
		#region CodeNotInDictionary
		/// <summary>
		/// Checks that the correct error status is set when the input stream
		/// contains a code which isn't in the dictionary.
		/// </summary>
		[Test]
		public void CodeNotInDictionary()
		{
			byte[] bytes = WikipediaExample.ImageDataBytes;
			bytes[4] = 0xFF; // put an unexpected code into the stream
			MemoryStream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Seek( 0, SeekOrigin.Begin );
			
			int pixelCount = WikipediaExample.FrameSize.Width
							* WikipediaExample.FrameSize.Height;
			_tbid = new TableBasedImageData( s, pixelCount );
			
			Assert.IsTrue( _tbid.TestState( ErrorState.CodeNotInDictionary ) );
			Assert.AreEqual( 15, _tbid.Pixels.Count );
			Assert.AreEqual( 8, _tbid.LzwMinimumCodeSize );
			Assert.AreEqual( 9, _tbid.InitialCodeSize );
			Assert.AreEqual( Math.Pow( 2, 8 ), _tbid.ClearCode );
			Assert.AreEqual( Math.Pow( 2, 8 ) + 1, _tbid.EndOfInformation );
			
			IndexedPixels expectedIndices = new IndexedPixels();

			// Check all the pixels have been zero-filled
			expectedIndices.Add( 0 ); // first pixel 
			expectedIndices.Add( 0 ); // 2nd pixel 
			expectedIndices.Add( 0 ); // 3rd pixel
			expectedIndices.Add( 0 ); // 4th pixel
			expectedIndices.Add( 0 ); // 5th pixel
			expectedIndices.Add( 0 ); // 6th pixel
			expectedIndices.Add( 0 ); // 7th pixel
			expectedIndices.Add( 0 ); // 8th pixel
			expectedIndices.Add( 0 ); // 9th pixel
			expectedIndices.Add( 0 ); // 10th pixel
			expectedIndices.Add( 0 ); // 11th pixel
			expectedIndices.Add( 0 ); // 12th pixel
			expectedIndices.Add( 0 ); // 13th pixel
			expectedIndices.Add( 0 ); // 14th pixel
			expectedIndices.Add( 0 ); // 15th pixel
			
			for( int i = 0; i < 15; i++ )
			{
				Assert.AreEqual( expectedIndices[i], _tbid.Pixels[i], "pixel " + i );
			}
		}
		#endregion
		
		#region DataBlockTooShort
		/// <summary>
		/// Checks that the correct error status is set when the end of the
		/// input stream is encountered prematurely.
		/// </summary>
		[Test]
		public void DataBlockTooShort()
		{
			byte[] bytes = new byte[]
			{
				0x08, // LZW minimum code size
				0x0b, // block size = 11
				// 11 bytes of LZW encoded data follows (actually there's only 10 for this test)
				0x00, 0x51, 0xFC, 0x1B, 0x28, 0x70, 0xA0, 0xC1, 0x83, 0x01, //0x01,
//				0x00 // block terminator
			};
			MemoryStream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Seek( 0, SeekOrigin.Begin );
			
			int pixelCount = WikipediaExample.FrameSize.Width
							* WikipediaExample.FrameSize.Height;
			_tbid = new TableBasedImageData( s, pixelCount );
			
			Assert.AreEqual( 15, _tbid.Pixels.Count );
			Assert.AreEqual( ErrorState.DataBlockTooShort | ErrorState.TooFewPixelsInImageData, _tbid.ConsolidatedState );
			Assert.AreEqual( 8, _tbid.LzwMinimumCodeSize );
			Assert.AreEqual( 9, _tbid.InitialCodeSize );
			Assert.AreEqual( Math.Pow( 2, 8 ), _tbid.ClearCode );
			Assert.AreEqual( Math.Pow( 2, 8 ) + 1, _tbid.EndOfInformation );
			
			IndexedPixels expectedIndices = new IndexedPixels();

			expectedIndices.Add( 0 ); // first pixel 
			expectedIndices.Add( 0 ); // 2nd pixel 
			expectedIndices.Add( 0 ); // 3rd pixel
			expectedIndices.Add( 0 ); // 4th pixel
			expectedIndices.Add( 0 ); // 5th pixel
			expectedIndices.Add( 0 ); // 6th pixel
			expectedIndices.Add( 0 ); // 7th pixel
			expectedIndices.Add( 0 ); // 8th pixel
			expectedIndices.Add( 0 ); // 9th pixel
			expectedIndices.Add( 0 ); // 10th pixel
			expectedIndices.Add( 0 ); // 11th pixel
			expectedIndices.Add( 0 ); // 12th pixel
			expectedIndices.Add( 0 ); // 13th pixel
			expectedIndices.Add( 0 ); // 14th pixel
			expectedIndices.Add( 0 ); // 15th pixel
			
			for( int i = 0; i < 15; i++ )
			{
				Assert.AreEqual( expectedIndices[i], _tbid.Pixels[i], "pixel " + i );
			}
		}
		#endregion
		
		#region HandleRubbish
		/// <summary>
		/// Checks that no exception is thrown when the constructor is passed
		/// a completely invalid input stream. Any errors should be handled by
		/// setting the error status.
		/// This is done by creating streams using whichever files are in the
		/// executing directory and attempting to instantiate a TBID from each
		/// of them.
		/// </summary>
		[Test]
		public void HandleRubbish()
		{
			string[] files = Directory.GetFiles( Directory.GetCurrentDirectory() );
			foreach( string file in files )
			{
				byte[] bytes = File.ReadAllBytes( file );
				MemoryStream s = new MemoryStream();
				s.WriteByte( 8 ); // write a valid LZW min code size
				s.Write( bytes, 0, bytes.Length );
				s.Seek( 0, SeekOrigin.Begin );
				_tbid = new TableBasedImageData( s, 1000000 );
				Console.WriteLine( file + " " + _tbid.ConsolidatedState );
			}
		}
		#endregion

		#region RotatingGlobeTest
		/// <summary>
		/// Calls the RotatingGlobeTest from the GifDecoder test framework
		/// in order to check that the TBID can cope with decoding large images.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
//		[Ignore( "Don't run when checking code coverage by this test fixture" )]
		public void RotatingGlobeTest()
		{
			GifDecoderTest t = new GifDecoderTest();
			t.DecodeRotatingGlobe();
		}
		#endregion
	}
}

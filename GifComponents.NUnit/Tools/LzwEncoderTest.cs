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
using System.Globalization;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace GifComponents.NUnit
{
	/// <summary>
	/// Test fixture for the LZWEncoder class.
	/// Most of the test cases are based on the principle that LZW compression
	/// should be lossless, so no matter what you encode, it should decode to
	/// exactly what you started with.
	/// Encoding performed using LzwEncoder, decoding performed using 
	/// TableBasedImageData.
	/// </summary>
	[TestFixture]
	[SuppressMessage("Microsoft.Naming", 
	                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
	                 MessageId = "Lzw")]
	public class LzwEncoderTest
	{
		private LzwEncoder _e;
		private IndexedPixels _ip;
		
		#region Setup method
		/// <summary>
		/// Setup method
		/// </summary>
		[SetUp]
		public void Setup()
		{
			_ip = new IndexedPixels();
		}
		#endregion

		#region Test1Pixel1
		/// <summary>
		/// 0.
		/// </summary>
		[Test]
		public void Test1Pixel1()
		{
			_ip.Add( 0 );
			TestIt();
		}
		#endregion
		
		#region Test1Pixel2
		/// <summary>
		/// 255.
		/// </summary>
		[Test]
		public void Test1Pixel2()
		{
			_ip.Add( 255 );
			TestIt();
		}
		#endregion
		
		#region Test4Pixels1
		/// <summary>
		/// Reproduces the problem seen in AnimatedGifEncoderTest.UseGlobal, with
		/// the 4-pixel checkerboard image.
		/// 9,   255.
		/// 255, 9.
		/// </summary>
		[Test]
		public void Test4Pixels1()
		{
			_ip.Add( 9 );
			_ip.Add( 255 );
			_ip.Add( 255 );
			_ip.Add( 9 );
			TestIt();
		}
		#endregion
		
		#region Test4Pixels2
		/// <summary>
		/// 0,   0.
		/// 0,   0.
		/// </summary>
		[Test]
		public void Test4Pixels2()
		{
			_ip.Add( 0 );
			_ip.Add( 0 );
			_ip.Add( 0 );
			_ip.Add( 0 );
			TestIt();
		}
		#endregion
		
		#region Test4Pixels3
		/// <summary>
		/// 255, 255.
		/// 255, 255.
		/// </summary>
		[Test]
		public void Test4Pixels3()
		{
			_ip.Add( 255 );
			_ip.Add( 255 );
			_ip.Add( 255 );
			_ip.Add( 255 );
			TestIt();
		}
		#endregion
		
		#region Test15Pixels1
		/// <summary>
		/// 40,  255, 255.
		/// 255, 40,  255.
		/// 255, 255, 255.
		/// 255, 255, 255.
		/// 255, 255, 255.
		/// </summary>
		[Test]
		public void Test15Pixels1()
		{
			_ip.Add( 40 );
			_ip.Add( 255 );
			_ip.Add( 255 );

			_ip.Add( 255 );
			_ip.Add( 40 );
			_ip.Add( 255 );

			_ip.Add( 255 );
			_ip.Add( 255 );
			_ip.Add( 255 );

			_ip.Add( 255 );
			_ip.Add( 255 );
			_ip.Add( 255 );

			_ip.Add( 255 );
			_ip.Add( 255 );
			_ip.Add( 255 );
			
			TestIt();
		}
		#endregion
		
		#region Test15Pixels2
		/// <summary>
		/// 0, 0, 0.
		/// 0, 0, 0.
		/// 0, 0, 0.
		/// 0, 0, 0.
		/// 0, 0, 0.
		/// </summary>
		[Test]
		public void Test15Pixels2()
		{
			_ip.Add( 0 );
			_ip.Add( 0 );
			_ip.Add( 0 );

			_ip.Add( 0 );
			_ip.Add( 0 );
			_ip.Add( 0 );

			_ip.Add( 0 );
			_ip.Add( 0 );
			_ip.Add( 0 );

			_ip.Add( 0 );
			_ip.Add( 0 );
			_ip.Add( 0 );

			_ip.Add( 0 );
			_ip.Add( 0 );
			_ip.Add( 0 );

			TestIt();
		}
		#endregion
		
		#region Test100Pixels
		/// <summary>
		/// Tests the encoder with 100 random pixel values.
		/// </summary>
		[Test]
		public void Test100Pixels()
		{
			RandomFill( 100 );
		}
		#endregion
		
		#region Test1000Pixels
		/// <summary>
		/// Tests the encoder with 1,000 random pixel values.
		/// </summary>
		[Test]
		public void Test1000Pixels()
		{
			RandomFill( 1000 );
		}
		#endregion
		
		#region Test10000Pixels
		/// <summary>
		/// Tests the encoder with 10,000 random pixel values.
		/// </summary>
		[Test]
		public void Test10000Pixels()
		{
			RandomFill( 10000 );
		}
		#endregion
		
		#region Test100000Pixels
		/// <summary>
		/// Tests the encoder with 100,000 random pixel values.
		/// </summary>
		[Test]
		public void Test100000Pixels()
		{
			RandomFill( 100000 );
		}
		#endregion
		
		#region Test1000000Pixels (commented out - takes a while to run)
//		/// <summary>
//		/// Tests the encoder with 1,000,000 random pixel values.
//		/// </summary>
//		[Test]
//		public void Test1000000Pixels()
//		{
//			RandomFill( 1000000 );
//		}
		#endregion
		
		#region private RandomFill method
		/// <summary>
		/// Fills _ip with random values
		/// </summary>
		/// <param name="count">
		/// Number of values to add
		/// </param>
		private void RandomFill( int count )
		{
			Random r = new Random();
			for( int blockiness = 1; blockiness < 20; blockiness++ )
			{
				_ip = new IndexedPixels();
				byte[] bytes = new byte[count];
				r.NextBytes( bytes );
				byte lastByte = 0;
				foreach( byte b in bytes )
				{
					// Add a new value to the collection only if we throw a 1
					int diceThrow = r.Next( 1, blockiness );
					if( diceThrow == 1 )
					{
						_ip.Add( b );
						lastByte = b;
					}
					else
					{
						// otherwise add the previous value again
						// (this more accurately simulates colour distribution in
						// an actual image)
						_ip.Add( lastByte );
					}
				}
				TestIt();
			}
		}
		#endregion
		
		#region private TestIt method
		/// <summary>
		/// Encodes _ip and decodes the encoded data, then compares the decoded
		/// data against the original.
		/// Also calculates the compression rate.
		/// </summary>
		private void TestIt()
		{
			MemoryStream s = new MemoryStream();
			_e = new LzwEncoder( _ip );
			_e.Encode( s );
			int encodedByteCount = (int) s.Position;
			
			s.Seek( 0, SeekOrigin.Begin );
			TableBasedImageData tbid = new TableBasedImageData( s, _ip.Count );
			
			s.Seek( 0, SeekOrigin.Begin );
			byte[] encodedBytes = new byte[encodedByteCount];
			s.Read( encodedBytes, 0, encodedByteCount );
			
			Assert.AreEqual( _ip.Count, tbid.Pixels.Count, "Pixel counts differ" );
			for( int i = 0; i < _ip.Count; i++ )
			{
				Assert.AreEqual( _ip[i], tbid.Pixels[i], "pixel index " + i );
			}
			
			float compression = 100 - (100 * encodedByteCount / _ip.Count);
			Console.WriteLine( "Original byte count: " + _ip.Count 
			                   + ". Encoded byte count: " + encodedByteCount
			                   + ". Compression rate: " + compression + "%" );
		}
		#endregion

		#region NullStream
		/// <summary>
		/// Checks that the correct exception is thrown when the encode method
		/// is passed a null stream.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		public void NullStream()
		{
			_e = new LzwEncoder( _ip );
			try
			{
				_e.Encode( null );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "outputStream", ex.ParamName );
				throw;
			}
		}
		#endregion
		
		#region NullPixels
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor is
		/// passed a null IndexedPixels instance.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		public void NullPixels()
		{
			try
			{
				_e = new LzwEncoder( null );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "pixels", ex.ParamName );
				throw;
			}
		}
		#endregion
	}
}

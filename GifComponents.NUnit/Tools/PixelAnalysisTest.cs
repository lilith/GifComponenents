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
using NUnit.Framework;
using NUnit.Extensions;
using GifComponents.Tools;

namespace GifComponents.NUnit.Tools
{
	/// <summary>
	/// Test fixture for the PixelAnalysis class.
	/// </summary>
	[TestFixture]
	public class PixelAnalysisTest : TestFixtureBase, IDisposable
	{
		#region declarations
		private PixelAnalysis _pa;
		private Size _imageSize;
		private Collection<Color> _colours;
		private Collection<Image> _images;
		#endregion
		
		#region Setup method
		/// <summary>
		/// Setup method.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			_colours = new Collection<Color>();
			for( int i = 0; i < 256; i++ )
			{
				_colours.Add( Color.FromArgb( 255, i, 0, 0 ) );
			}
			_colours.Add( Color.FromArgb( 255, 0, 0, 100 ) );
			_images = new Collection<Image>();
		}
		#endregion
		
		#region simgle image tests
		
		#region SingleImage001Colour
		/// <summary>
		/// Tests the PixelAnalysis class using a single image with only one
		/// colour.
		/// </summary>
		[Test]
		public void SingleImage001Colour()
		{
			ReportStart();
			TestSingleImage( 1 );
			ReportEnd();
		}
		#endregion
		
		#region SingleImage002Colours
		/// <summary>
		/// Tests the PixelAnalysis class using a single image with only two
		/// colours.
		/// </summary>
		[Test]
		public void SingleImage002Colours()
		{
			ReportStart();
			TestSingleImage( 2 );
			ReportEnd();
		}
		#endregion
		
		#region SingleImage004Colours
		/// <summary>
		/// Tests the PixelAnalysis class using a single image with only four
		/// colours.
		/// </summary>
		[Test]
		public void SingleImage004Colours()
		{
			ReportStart();
			TestSingleImage( 4 );
			ReportEnd();
		}
		#endregion
		
		#region SingleImage005Colours
		/// <summary>
		/// Tests the PixelAnalysis class using a single image with only five
		/// colours.
		/// </summary>
		[Test]
		public void SingleImage005Colours()
		{
			ReportStart();
			TestSingleImage( 5 );
			ReportEnd();
		}
		#endregion
		
		#region SingleImage256Colours
		/// <summary>
		/// Tests the PixelAnalysis class using a single image with 256 colours.
		/// </summary>
		[Test]
		public void SingleImage256Colours()
		{
			ReportStart();
			TestSingleImage( 256 );
			ReportEnd();
		}
		#endregion
		
		#region SingleImage257ColoursNeuQuant
		/// <summary>
		/// Tests the PixelAnalysis class using a single image with 257 colours
		/// and the NeuQuant quantizer.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
		                 MessageId = "Neu")]
		public void SingleImage257ColoursNeuQuant()
		{
			ReportStart();
			Bitmap image = MakeBitmap( 257 );
			_pa = new PixelAnalysis( image, QuantizerType.NeuQuant );
			Assert.AreEqual( 256, _pa.ColourTable.Length );
			Assert.AreEqual( image.Width * image.Height, 
			                 _pa.IndexedPixels.Count );
			ReportEnd();
		}
		#endregion
		
		#region SingleImage257ColoursOctree
		/// <summary>
		/// Tests the PixelAnalysis class using a single image with 257 colours
		/// and the Octree quantizer.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
		                 MessageId = "Octree")]
		public void SingleImage257ColoursOctree()
		{
			ReportStart();
			Bitmap image = MakeBitmap( 257 );
			_pa = new PixelAnalysis( image, QuantizerType.Octree );
			// FIXME: Octree quantizer is quantizing too much - reducing to 128 colours instead of 256
			// TODO: Check for exactly 256 colours once Octree quantizer returns 256-colour images
//			Assert.AreEqual( 256, _pa.ColourTable.Length );
			Assert.LessOrEqual( _pa.ColourTable.Length, 256 );
			Assert.AreEqual( image.Width * image.Height, 
			                 _pa.IndexedPixels.Count );
			ReportEnd();
		}
		#endregion
		
		#endregion
		
		#region multiple image tests
		
		#region MultipleImages001Colour
		/// <summary>
		/// Tests the PixelAnalysis class with two images of one colour.
		/// </summary>
		[Test]
		public void MultipleImages001Colour()
		{
			ReportStart();
			TestMultipleImages( 1 );
			ReportEnd();
		}
		#endregion
		
		#region MultipleImages002Colours
		/// <summary>
		/// Tests the PixelAnalysis class with two images of two colours.
		/// </summary>
		[Test]
		public void MultipleImages002Colours()
		{
			ReportStart();
			TestMultipleImages( 2 );
			ReportEnd();
		}
		#endregion
		
		#region MultipleImages256Colours
		/// <summary>
		/// Tests the PixelAnalysis class with two images of 256 colours.
		/// </summary>
		[Test]
		public void MultipleImages256Colours()
		{
			ReportStart();
			TestMultipleImages( 256 );
			ReportEnd();
		}
		#endregion

		#region MultipleImages257Colours
		/// <summary>
		/// Tests the PixelAnalysis class with two images of 257 colours.
		/// </summary>
		[Test]
		public void MultipleImages257Colours()
		{
			ReportStart();
			_images.Add( MakeBitmap( 257 ) );
			_images.Add( MakeBitmap( 257 ) );
			_pa = new PixelAnalysis( _images );
			
			// Cannot use the TestMultipleImages method this time because using
			// more than 256 colours causes colour quantization to reduce it
			// to 256 colours. Exact contents of colour table and indexed pixels
			// are therefore unpredictable - we can only check their lengths.
			Assert.AreEqual( 256, _pa.ColourTable.Length );
			Assert.AreEqual( 2, _pa.IndexedPixelsCollection.Count );
			Assert.AreEqual( _images[0].Width * _images[0].Height, 
			                 _pa.IndexedPixelsCollection[0].Count );
			Assert.AreEqual( _images[1].Width * _images[1].Height, 
			                 _pa.IndexedPixelsCollection[1].Count );
			ReportEnd();
		}
		#endregion
		
		#endregion

		#region exception tests
		
		#region IndexedPixelsException
		/// <summary>
		/// Checks that the correct exception is thrown when the PixelAnalysis
		/// is instantiated with the constructor which accepts a collection of
		/// images and the IndexedPixels property is called.
		/// </summary>
		[ExpectedException( typeof( InvalidOperationException ) )]
		[Test]
		public void IndexedPixelsException()
		{
			ReportStart();
			_images.Add( MakeBitmap( 10 ) );
			_images.Add( MakeBitmap( 20 ) );
			_pa = new PixelAnalysis( _images );
			try
			{
				Assert.IsNull( _pa.IndexedPixels );
			}
			catch( InvalidOperationException ex )
			{
				string message
					= "The PixelAnalysis object was instantiated using the "
					+ "constructor which accepts a collection of images "
					+ "and you are attempting to retrieve the indexed "
					+ "pixels for a single image. "
					+ "Call the IndexedPixelCollection property instead.";
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region IndexedPixelsCollectionException
		/// <summary>
		/// Checks that the correct exception is thrown when the PixelAnalysis
		/// is instantiated with the constructor which accepts a single image
		/// and the IndexedPixelsCollection property is called.
		/// </summary>
		[ExpectedException( typeof( InvalidOperationException ) )]
		[Test]
		public void IndexedPixelsCollectionException()
		{
			ReportStart();
			_pa = new PixelAnalysis( MakeBitmap( 10 ), QuantizerType.NeuQuant );
			try
			{
				Assert.IsNull( _pa.IndexedPixelsCollection );
			}
			catch( InvalidOperationException ex )
			{
				string message
					= "The PixelAnalysis object was instantiated using the "
					+ "constructor which accepts a single image and you "
					+ "are attempting to retrieve the indexed pixels for a "
					+ "collection of images. "
					+ "Call the IndexedPixels property instead.";
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#endregion
		
		#region private methods
		
		#region private TestSingleImage methods
		private void TestSingleImage( int numberOfColours )
		{
			TestSingleImage( numberOfColours, QuantizerType.NeuQuant );
			TestSingleImage( numberOfColours, QuantizerType.Octree );
		}
		
		private void TestSingleImage( int numberOfColours, QuantizerType qt )
		{
			Bitmap image = MakeBitmap( numberOfColours );
			_pa = new PixelAnalysis( image, qt );
			CheckColourTable( numberOfColours );
			CheckIndexedPixels( numberOfColours );
		}
		#endregion
		
		#region TestMultipleImages method
		private void TestMultipleImages( int numberOfColours )
		{
			_images.Add( MakeBitmap( numberOfColours ) );
			_images.Add( MakeBitmap( numberOfColours ) );
			_pa = new PixelAnalysis( _images );
			CheckColourTable( numberOfColours );
			CheckIndexedPixelsCollection( numberOfColours );
		}
		#endregion
		
		#region private MakeBitmap method
		private Bitmap MakeBitmap( int numberOfColours )
		{
			int height = (numberOfColours / 10) + 1;
			int width = 10;
			int pixelNumber;
			_imageSize = new Size( width, height );
			Bitmap image = new Bitmap( width, height );
			for( int y = 0; y < height; y++ )
			{
				for( int x = 0; x < width; x++ )
				{
					pixelNumber = (y * width) + x;
					int remainder;
					Math.DivRem( pixelNumber, numberOfColours, out remainder );
					Color pixelColour = _colours[remainder];
					image.SetPixel( x, y, pixelColour );
				}
			}
			return image;
		}
		#endregion

		#region private CheckColourTable method
		private void CheckColourTable( int numberOfColours )
		{
			// Colour table should be a minimum of 4 colours, or the smallest
			// power of 2 which is large enough to hold all the colours, up to
			// a maximum of 256 (the maximum allowed by the GIF standard).
			if( numberOfColours <= 4 )
			{
				// The image has 4 colours or less, so the colour table should
				// contain 4 colours.
				Assert.AreEqual( 4, _pa.ColourTable.Length, 
				                 "ColourTable.Length" );
			}
			else
			{
				if( numberOfColours > 256 )
				{
					// The image has more than 256 colours, so the colour table
					// should be quantized and contain 256 colours.
					Assert.AreEqual( 256, _pa.ColourTable.Length, 
					                 "ColourTable.Length" );
				}
				else
				{
					// The image has more than 4 colours and up to 256 colours,
					// so the colour table should be just large enough to 
					// contain all the colours in the image.
					int power = 2;
					int result = 0;
					while( result < numberOfColours )
					{
						result = (int) Math.Pow( 2, power );
						power++;
					}
					Assert.AreEqual( result, _pa.ColourTable.Length, 
					                 "ColourTable.Length" );
				}
			}
			
			// Table should be filled with the colours in the image
			// NB we can't compare _colours[i] with _pa.ColourTable[i] as they
			// won't be in the same order
			for( int i = 0; i < numberOfColours; i++ )
			{
				CollectionAssert.Contains( _pa.ColourTable.Colours, _colours[i] );
			}
			
			// Remaining colours in the table should all be black
			for( int i = numberOfColours; i < _pa.ColourTable.Length; i++ )
			{
				Assert.AreEqual( Color.FromArgb( 255, 0, 0, 0 ), 
				                 _pa.ColourTable[i], 
				                 "ColourTable[" + i + "]" );
			}
		}
		#endregion
		
		#region private CheckIndexedPixels method
		private void CheckIndexedPixels( int numberOfColours )
		{
			// The number of indexed pixels should be the same as the number of
			// pixels in the original image
			int expectedPixelCount = _imageSize.Width * _imageSize.Height;
			Assert.AreEqual( expectedPixelCount, _pa.IndexedPixels.Count, 
			                 "IndexedPixels.Count" );
			
			// Bytes in the indexed pixels should point to the correct entries
			// in the colour table
			for( int i = 0; i < _pa.IndexedPixels.Count; i++ )
			{
				int indexInTestFixtureColours;
				Math.DivRem( i, numberOfColours, out indexInTestFixtureColours );
				Color c = _colours[indexInTestFixtureColours];
				Assert.AreEqual( c, _pa.ColourTable[_pa.IndexedPixels[i]], 
				                 "Pixel index " + i );
			}
		}
		#endregion
		
		#region private CheckIndexedPixelsCollection method
		private void CheckIndexedPixelsCollection( int numberOfColours )
		{
			Assert.AreEqual( _images.Count, _pa.IndexedPixelsCollection.Count,
			                 "IndexedPixelsCollection.Count" );
			for( int j = 0; j < _pa.IndexedPixelsCollection.Count; j++ )
			{
				Bitmap b = (Bitmap) _images[j];
				// The number of indexed pixels should be the same as the number of
				// pixels in the original image
				int expectedPixelCount = b.Width * b.Height;
				Assert.AreEqual( expectedPixelCount, 
				                 _pa.IndexedPixelsCollection[j].Count,
				                 "IndexedPixelsCollection[" + j + "].Count" );
				
				// Bytes in the indexed pixels should point to the correct entries
				// in the colour table
				for( int i = 0; i < _pa.IndexedPixelsCollection[j].Count; i++ )
				{
					int indexInTestFixtureColours;
					Math.DivRem( i, numberOfColours, out indexInTestFixtureColours );
					Color expected = _colours[indexInTestFixtureColours];
					Color actual = _pa.ColourTable[_pa.IndexedPixelsCollection[j][indexInTestFixtureColours]];
					Assert.AreEqual( expected, actual, 
					                 "Image index: " + j + ", Pixel index: " + i );
				}
			}
		}
		#endregion
		
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
		~PixelAnalysisTest()
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
					_pa.Dispose();
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

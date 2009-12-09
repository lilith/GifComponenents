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

namespace GifComponents.NUnit
{
	/// <summary>
	/// Test fixture for the OctreeQuantizer class.
	/// </summary>
	[TestFixture]
	[SuppressMessage("Microsoft.Naming", 
	                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
	                 MessageId = "Octree")]
	[SuppressMessage("Microsoft.Naming", 
	                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
	                 MessageId = "Quantizer")]
	public class OctreeQuantizerTest : TestFixtureBase
	{
		private OctreeQuantizer _oq;
		
		#region CompareQuantizedImages
		/// <summary>
		/// Compares quantized images with the corresponding original image,
		/// for images with increasing numbers of colours
		/// </summary>
		[Test]
		public void CompareQuantizedImages()
		{
			ReportStart();
			_oq = new OctreeQuantizer( 255, 8 );
			for( int colourCount = 1; colourCount < 500; colourCount++ )
			{
				Bitmap original = MakeBitmap( new Size( 50, 50 ), colourCount );
				Bitmap quantized = _oq.Quantize( original );
				BitmapAssert.AreEqual( original, quantized, 3, 
				                       colourCount + " colours" );
			}
			ReportEnd();
		}
		#endregion
		
		#region out of range tests
		
		#region MaxColoursTooLarge
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor is
		/// passed a maxColours parameter which is too small.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void MaxColoursTooSmall()
		{
			ReportStart();
			try
			{
				_oq = new OctreeQuantizer( 0, 8 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message = "The number of colours should be between 1 and 255";
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "maxColours", ex.ParamName );
				Assert.AreEqual( 0, ex.ActualValue );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region MaxColoursTooLarge
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor is
		/// passed a maxColours parameter which is too large.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void MaxColoursTooLarge()
		{
			ReportStart();
			try
			{
				_oq = new OctreeQuantizer( 256, 1 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message = "The number of colours should be between 1 and 255";
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "maxColours", ex.ParamName );
				Assert.AreEqual( 256, ex.ActualValue );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region MaxColourBitsTooSmall
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor is
		/// passed a maxColourBits parameter which is too small.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void MaxColourBitsTooSmall()
		{
			ReportStart();
			try
			{
				_oq = new OctreeQuantizer( 10, 0 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message = "This should be between 1 and 8";
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "maxColourBits", ex.ParamName );
				Assert.AreEqual( 0, ex.ActualValue );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region maxColourBitsTooLarge
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor is
		/// passed a maxColourBits parameter which is too large.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void MaxColourBitsTooLarge()
		{
			ReportStart();
			try
			{
				_oq = new OctreeQuantizer( 10, 9 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message = "This should be between 1 and 8";
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "maxColourBits", ex.ParamName );
				Assert.AreEqual( 9, ex.ActualValue );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#endregion
		
		#region private methods
		private static Bitmap MakeBitmap( Size size, int numberOfColours )
		{
			Bitmap bitmap = new Bitmap( size.Width, size.Height );
			Collection<Color> colours = new Collection<Color>();
			Color c;
			Random rand = new Random();
			for( int i = 0; i < numberOfColours; i++ )
			{
				int channel = rand.Next( 0, 2 );
				int intensity = (int) ( (double) i * (double) 255 / (double) numberOfColours );
				switch( channel )
				{
					case 0:
						c = Color.FromArgb( intensity, 0, 0 );
						break;
						
					case 1:
						c = Color.FromArgb( 0, intensity, 0 );
						break;
						
					default:
						c = Color.FromArgb( 0, 0, intensity );
						break;
				}
				colours.Add( c );
			}
			
			int colourIndex = 0;
			for( int y = 0; y < size.Height; y++ )
			{
				for( int x = 0; x < size.Width; x++ )
				{
					bitmap.SetPixel( x, y, colours[colourIndex] );
					colourIndex++;
					if( colourIndex >= colours.Count )
					{
						colourIndex = 0;
					}
				}
			}
			return bitmap;
		}
		#endregion
	}
}

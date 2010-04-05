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
using System.Drawing.Imaging;
using NUnit.Framework;
using NUnit.Extensions;
using GifComponents.Tools;

namespace GifComponents.NUnit.Tools
{
	/// <summary>
	/// Test fixture for the ImageTools class.
	/// </summary>
	/// <remarks>
	/// These test cases deliberately use the (possibly slow) .net method of
	/// manipulating an image.
	/// The methods being tested could be updated to use faster (unsafe?) 
	/// methods, but the test cases should still use the .net methods to verify
	/// the results.
	/// </remarks>
	[TestFixture]
	public class ImageToolsTest : TestFixtureBase
	{
		private Bitmap _bitmap;
		
		#region GetColoursTest
		/// <summary>
		/// Checks that the GetColours method works as expected.
		/// </summary>
		[Test]
		public void GetColoursTest()
		{
			ReportStart();
			_bitmap = RandomBitmap.Create( new Size( 100, 100 ), 20, 
			                               PixelFormat.Format32bppArgb );
			
			Collection<Color> expectedColours = new Collection<Color>();
			for( int y = 0; y < _bitmap.Height; y++ )
			{
				for( int x = 0; x < _bitmap.Width; x++ )
				{
					expectedColours.Add( _bitmap.GetPixel( x, y ) );
				}
			}
			
			Color[] actualColours = ImageTools.GetColours( _bitmap );
			
			Assert.AreEqual( expectedColours.Count, actualColours.Length );
			
			for( int i = 0; i < expectedColours.Count; i++ )
			{
				ColourAssert.AreEqual( expectedColours[i], 
				                       actualColours[i], 
				                       "Index " + i );
			}
			ReportEnd();
		}
		#endregion
		
		#region GetDistinctColoursTest
		/// <summary>
		/// Checks that the GetDistinctColours method returns the expected 
		/// value.
		/// </summary>
		[Test]
		public void GetDistinctColoursTest()
		{
			ReportStart();
			_bitmap = RandomBitmap.Create( new Size( 50, 50 ), 10, 
			                               PixelFormat.Format32bppArgb );
			
			Collection<Color> expectedDistinctColours = new Collection<Color>();
			Color c;
			for( int y = 0; y <_bitmap.Height; y++ )
			{
				for( int x = 0; x < _bitmap.Width; x++ )
				{
					c = _bitmap.GetPixel( x, y );
					if( expectedDistinctColours.Contains( c ) == false )
					{
						expectedDistinctColours.Add( c );
					}
				}
			}
			
			Color[] actualColours = ImageTools.GetColours( _bitmap );
			Collection<Color> actualDistinctColours 
				= ImageTools.GetDistinctColours( actualColours );
			
			Assert.AreEqual( expectedDistinctColours.Count, 
			                 actualDistinctColours.Count );
			
			// NB we can't compare expected[i] to actual[i] as the two collections
			// are in different orders due to the use of Hashtable in the 
			// GetDistinctColours method
			foreach( Color thisColour in expectedDistinctColours )
			{
				CollectionAssert.Contains( actualDistinctColours, thisColour, 
				                           thisColour.ToString() );
			}
			ReportEnd();
		}
		#endregion
		
		#region GetRgbArrayTest
		/// <summary>
		/// Checks that the GetRgbArray method works as expected.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
		                 MessageId = "Rgb")]
		public void GetRgbArrayTest()
		{
			ReportStart();
			_bitmap = RandomBitmap.Create( new Size( 50, 50 ), 10, 
			                               PixelFormat.Format32bppArgb );
			
			Color[] colours = ImageTools.GetColours( _bitmap );
			
			int len = colours.Length;
			byte[] exected = new byte[len * 3];
			int rgbIndex;
			for( int colourIndex = 0; colourIndex < len; colourIndex++ )
			{
				rgbIndex = colourIndex * 3;
				exected[rgbIndex] = (byte) colours[colourIndex].R;
				exected[rgbIndex + 1] = (byte) colours[colourIndex].G;
				exected[rgbIndex + 2] = (byte) colours[colourIndex].B;
			}
			
			byte[] actual = ImageTools.GetRgbArray( colours );
			
			Assert.AreEqual( exected.Length, actual.Length );
			
			for( int i = 0; i < exected.Length; i++ )
			{
				Assert.AreEqual( actual[i], exected[i], "Index " + i );
			}
			ReportEnd();
		}
		#endregion
		
		#region GetRgbCollectionTest
		/// <summary>
		/// Checks that the GetRgbCollection method works as expected.
		/// </summary>
		[Test]
		[Obsolete( "Just to stop the compiler complaining about " +
		          "GetRgbCollection being obsolete - it still needs testing " +
		          "until it's removed completely" )]
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
		                 MessageId = "Rgb")]
		public void GetRgbCollectionTest()
		{
			ReportStart();
			_bitmap = RandomBitmap.Create( new Size( 50, 50 ), 10, 
			                               PixelFormat.Format32bppArgb );
			
			Color[] colours = ImageTools.GetColours( _bitmap );
			
			int len = colours.Length;
			byte[] exected = new byte[len * 3];
			int rgbIndex;
			for( int colourIndex = 0; colourIndex < len; colourIndex++ )
			{
				rgbIndex = colourIndex * 3;
				exected[rgbIndex] = (byte) colours[colourIndex].R;
				exected[rgbIndex + 1] = (byte) colours[colourIndex].G;
				exected[rgbIndex + 2] = (byte) colours[colourIndex].B;
			}
			
			Collection<byte> actual = ImageTools.GetRgbCollection( colours );
			
			Assert.AreEqual( exected.Length, actual.Count );
			
			for( int i = 0; i < exected.Length; i++ )
			{
				Assert.AreEqual( actual[i], exected[i], "Index " + i );
			}
			ReportEnd();
		}
		#endregion
		
		#region GetRgbSpeedComparisonTest
		/// <summary>
		/// Compares the speed of the GetRgbArray and GetRgbCollection methods
		/// and checks that they return the same results.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
		                 MessageId = "Rgb")]
		[Obsolete( "Just to stop the compiler complaining about " +
		          "GetRgbCollection being obsolete - it still needs testing " +
		          "until it's removed completely" )]
		public void GetRgbSpeedComparisonTest()
		{
			ReportStart();
			_bitmap = RandomBitmap.Create( new Size( 1000, 1000 ), 100, 
			                               PixelFormat.Format32bppArgb );
			_bitmap.Save( "GetRgbSpeedComparisonTest.bmp" );
			Color[] colours = ImageTools.GetColours( _bitmap );
			
			DateTime startTime;
			DateTime endTime;
			
			startTime = DateTime.Now;
			Collection<byte> rgbCollection 
				= ImageTools.GetRgbCollection( colours );
			endTime = DateTime.Now;
			TimeSpan collectionRunTime = endTime - startTime;
			WriteMessage( "GetRgbCollection took " + collectionRunTime );
			
			startTime = DateTime.Now;
			byte[] rgbArray = ImageTools.GetRgbArray( colours );
			endTime = DateTime.Now;
			TimeSpan arrayRunTime = endTime - startTime;
			WriteMessage( "GetRgbArray took " + arrayRunTime );
			
			// GetRgbArray should be quicker than GetRgbCollection. If not then
			// there's probably something wrong with GetRgbArray
			Assert.IsTrue( arrayRunTime < collectionRunTime );
			
			Assert.AreEqual( rgbCollection.Count, rgbArray.Length );
			for( int i = 0; i < rgbCollection.Count; i++ )
			{
				string colourName;
				int remainder;
				int quotient = Math.DivRem( i, 3, out remainder );
				switch( remainder )
				{
					case 0:
						colourName = "red";
						break;
						
					case 1:
						colourName = "green";
						break;
						
					case 2:
						colourName = "blue";
						break;
						
					default:
						throw new InvalidOperationException( "unexpected remainder: " + remainder );
				}
				string message
					= "Index: " + i + ", pixel " + quotient
					+ " (" + colourName + ")";
				Assert.AreEqual( rgbCollection[i], rgbArray[i], message );
			}
			ReportEnd();
		}
		#endregion

	}
}

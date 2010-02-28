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

namespace GifComponents.NUnit.Tools
{
	/// <summary>
	/// Helper class for creating random bitmap files.
	/// </summary>
	public static class RandomBitmap
	{
		#region Create method
		/// <summary>
		/// Creates and returns a random bitmap.
		/// </summary>
		/// <param name="size">
		/// The System.Drawing.Size of the required bitmap.
		/// </param>
		/// <param name="blockiness">
		/// Controls how often the colours of pixels in the image changes.
		/// The lower this value, the smaller the contiguous blocks of colour.
		/// </param>
		/// <param name="pixelFormat">
		/// One of the System.Drawing.Imaging.PixelFormat values.
		/// </param>
		/// <returns>
		/// A bitmap containing random colours.
		/// </returns>
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
		                 MessageId = "1#blockiness")]
		public static Bitmap Create( Size size, 
		                             int blockiness, 
		                             PixelFormat pixelFormat )
		{
			#region guard against invalid pixel formats
			if(
				pixelFormat == PixelFormat.DontCare // ArgumentException in Bitmap constructor
				|| pixelFormat == PixelFormat.Max // ArgumentException in Bitmap constructor
				|| pixelFormat == PixelFormat.Indexed // ArgumentException in Bitmap constructor
				|| pixelFormat == PixelFormat.Gdi // ArgumentException in Bitmap constructor
				|| pixelFormat == PixelFormat.Alpha // ArgumentException in Bitmap constructor
				|| pixelFormat == PixelFormat.PAlpha // ArgumentException in Bitmap constructor
				|| pixelFormat == PixelFormat.Extended // ArgumentException in Bitmap constructor
				|| pixelFormat == PixelFormat.Canonical // ArgumentException in Bitmap constructor
			  )
			{
				string message
					= pixelFormat + " is not a valid PixelFormat for this method as it "
					+ "causes an ArgumentException in the Bitmap constructor.";
				throw new ArgumentException( message, "pixelFormat" );
			}
			if(
				pixelFormat == PixelFormat.Format1bppIndexed // SetPixel not supported for images with indexed pixel formats
				|| pixelFormat == PixelFormat.Format4bppIndexed // SetPixel not supported for images with indexed pixel formats
				|| pixelFormat == PixelFormat.Format8bppIndexed // SetPixel not supported for images with indexed pixel formats
				|| pixelFormat == PixelFormat.Format16bppGrayScale // ArgumentException in SetPixel (no parameter name)
			  )
			{
				string message
					= pixelFormat + " is not a valid PixelFormat for this method as it "
					+ "is not supported by the Bitmap.SetPixel method.";
				throw new ArgumentException( message, "pixelFormat" );
			}
			#endregion
			
			Random rand = new Random();
			int r, g, b;
			Color c = Color.FromArgb( 0, 0, 0 );
			Bitmap bitmap = new Bitmap( size.Width, size.Height, pixelFormat );
			
			for( int y = 0; y < bitmap.Height; y++ )
			{
				for( int x = 0; x < bitmap.Width; x++ )
				{
					int dice = rand.Next( 0, blockiness );
					if( dice == 0 )
					{
						r = rand.Next( 0, 255 );
						g = rand.Next( 0, 255 );
						b = rand.Next( 0, 255 );
						c = Color.FromArgb( r, g, b );
					}
					bitmap.SetPixel( x, y, c );
				}
			}
			return bitmap;
		}
		#endregion
	}
}

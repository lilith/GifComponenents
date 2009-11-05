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
using System.Drawing;
using System.Drawing.Imaging;
using NUnit.Framework;

namespace GifComponents.NUnit
{
	/// <summary>
	/// Test cases which compare the length of time taken to encode GIF files
	/// under different circumstances.
	/// </summary>
	[TestFixture]
	public class AnimatedGifEncoderSpeedTests
	{
		private AnimatedGifEncoder _encoder;
		
		#region RunColourDepthTest
		/// <summary>
		/// Compares the encoding time for GIF files of the same size and
		/// number of frames, differing only in their PixelFormat (i.e. colour
		/// depth).
		/// </summary>
		[Test]
		public void RunColourDepthTest()
		{
			for( int i = 0; i < 10; i++ )
			{
				TryDifferentPixelFormats();
			}
		}
		#endregion
		
		#region ColourDepthTest
		private void TryDifferentPixelFormats()
		{
			Size size = new Size( 50, 50 );
			int blockiness = 10;
			PixelFormat[] pixelFormats = new PixelFormat[]
			{
				PixelFormat.Format16bppArgb1555,
				PixelFormat.Format16bppRgb555,
				PixelFormat.Format16bppRgb565,
				PixelFormat.Format24bppRgb,
				PixelFormat.Format32bppArgb,
				PixelFormat.Format32bppPArgb,
				PixelFormat.Format32bppRgb,
				PixelFormat.Format48bppRgb,
				PixelFormat.Format64bppArgb,
				PixelFormat.Format64bppPArgb,
			};
			foreach( PixelFormat pf in pixelFormats )
			{
				string formatName = pf.ToString();
				
				_encoder = new AnimatedGifEncoder();
				for( int i = 0; i < 10; i++ )
				{
					Bitmap bitmap = RandomBitmap.Create( size, 
					                                     blockiness,
					                                     pf );
					_encoder.AddFrame( new GifFrame( bitmap ) );
				}
				
				DateTime startTime = DateTime.Now;
				_encoder.WriteToFile( formatName + ".gif" );
				DateTime endTime = DateTime.Now;
				TimeSpan timeToEncode8bit = endTime - startTime;
				Console.WriteLine( "Encoding " + formatName + " took " + timeToEncode8bit );
			}
			
		}
		#endregion
	}
}

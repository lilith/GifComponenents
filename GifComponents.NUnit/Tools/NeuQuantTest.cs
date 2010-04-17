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
using GifComponents.Components;
using GifComponents.Tools;

namespace GifComponents.NUnit.Tools
{
	/// <summary>
	/// Test fixture for the NewQuant class.
	/// </summary>
	[TestFixture]
	[SuppressMessage("Microsoft.Naming", 
	                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
	                 MessageId = "Neu")]
	public class NeuQuantTest : TestFixtureBase
	{
		private NeuQuant _nq;
		private byte[] _rgb;
		private ColourTable _table;
		
		#region ConstructorNullArgument
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor
		/// is passed null image data.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		public void ConstructorNullArgument()
		{
			try
			{
				_nq = new NeuQuant( null, 10 );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "thePicture", ex.ParamName );
				throw;
			}
		}
		#endregion
		
		#region OneBlackPixel
		/// <summary>
		/// Tests with an image consisting of a single black pixel.
		/// </summary>
		[Test]
		public void OneBlackPixel()
		{
			// simulate a 1 pixel black image
			_rgb = new byte[] { 0, 0, 0 };
			_nq = new NeuQuant( _rgb, 1 );
			_table = _nq.Process();
			
			// This results in a greyscale palette of 256 colours for some reason.
			Assert.AreEqual( 256, _table.Length );
			
			for( int i = 0; i < 256; i++ )
			{
				Assert.AreEqual( Color.FromArgb( i, i, i ), _table[i], 
				                 "index " + i );
			}
			
			// black should be the first colour in the palette
			int index = _nq.Map( 0, 0, 0 );
			Assert.AreEqual( 0, index );
		}
		#endregion
		
		#region OneRedPixel
		/// <summary>
		/// Tests with an image consisting of a single black pixel.
		/// </summary>
		[Test]
		public void OneRedPixel()
		{
			// simulate a 1 pixel black image
			_rgb = new byte[] { 1, 0, 0 };
			_nq = new NeuQuant( _rgb, 1 );
			_table = _nq.Process();
			
			// This results in a greyscale palette of 256 colours for some reason.
			Assert.AreEqual( 256, _table.Length );
			
			for( int i = 0; i < 256; i++ )
			{
				Assert.AreEqual( Color.FromArgb( i, i, i ), _table[i], 
				                 "index " + i );
			}
			
			// black should be the first colour in the palette
			int index = _nq.Map( 0, 0, 0 );
			Assert.AreEqual( 0, index );
			
			Color nearestToRed = _table[_nq.Map( 255, 0, 0 )];
			WriteMessage( "Nearest colour to red is " + nearestToRed );
		}
		#endregion
		
		#region RedImage
		/// <summary>
		/// Investigation of how NeuQuant behaves with a single-colour image.
		/// </summary>
		[Test]
		public void RedImage()
		{
			Bitmap b = new Bitmap( 10, 10 );
			Graphics g = Graphics.FromImage( b );
			Brush brush = new SolidBrush( Color.FromArgb( 255, 0, 0 ) );
			Rectangle r = new Rectangle( new Point( 0, 0 ), b.Size );
			g.FillRectangle( brush, r );
			g.Dispose();
			brush.Dispose();
			
			Color[] colours = ImageTools.GetColours( b );
			_rgb = ImageTools.GetRgbArray( colours );
			_nq = new NeuQuant( _rgb, 1 );
			_table = _nq.Process();
			
			// Length of returned colour table is always 256 regardless of
			// actual number of colours.
			Assert.AreEqual( 256, _table.Length );
		}
		#endregion
		
	}
}

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
using System.Drawing;
using System.IO;
using NUnit.Framework;
using NUnit.Extensions;
using GifComponents.Palettes;

namespace GifComponents.NUnit
{
	/// <summary>
	/// Test fixture for the Palette class.
	/// </summary>
	[TestFixture]
	public class PaletteTest
	{
		#region declarations
		private Palette _actual;
		private Palette _expected;
		private string[] _paletteFiles;
		#endregion
		
		#region Setup method
		/// <summary>
		/// Setup method.
		/// Gets the filenames of the sample palette files.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			_paletteFiles = Directory.GetFiles( "ColourTables", "*.act" );
			if( _paletteFiles.Length == 0 )
			{
				throw new InvalidOperationException( "No sample palette files!" );
			}
		}
		#endregion
		
		#region FromFileTest
		/// <summary>
		/// Test case for the FromFile method.
		/// Also implicitly tests the FromStream method.
		/// </summary>
		[Test]
		public void FromFileTest()
		{
			foreach( string file in _paletteFiles )
			{
				_actual = Palette.FromFile( file );
				
				_expected = new Palette();
				byte[] bytes = File.ReadAllBytes( file );
				for( int i = 0; i < bytes.Length; i+=3 )
				{
					Color c = Color.FromArgb( bytes[i], bytes[i+1], bytes[i+2] );
					_expected.Add( c );
				}
				
				Assert.AreEqual( _expected.Count, _actual.Count, file );
				
				for( int i = 0; i < _expected.Count; i++ )
				{
					ColourAssert.AreEqual( _expected[i], _actual[i], 
					                       file + " Colour number " + i );
				}
			}
		}
		#endregion

		#region FromStreamNullStreamTest
		/// <summary>
		/// Checks that the correct exception is thrown when the FromStream is
		/// passed a null stream.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		public void FromStreamNullStreamTest()
		{
			try
			{
				_actual = Palette.FromStream( null );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "inputStream", ex.ParamName );
				throw;
			}
		}
		#endregion
		
		#region FromStreamTooLongTest
		/// <summary>
		/// Checks that when the FromStream method is passed a stream which is 
		/// longer than 768 bytes, everything after the 768th byte is ignored.
		/// </summary>
		[Test]
		public void FromStreamTooLongTest()
		{
			MemoryStream s = new MemoryStream();
			for( int i = 0; i < 769; i++ )
			{
				int b;
				Math.DivRem( i, 256, out b );
				s.WriteByte( (byte) b );
			}
			s.Seek( 0, SeekOrigin.Begin );
			
			_actual = Palette.FromStream( s );
			Assert.AreEqual( 256, _actual.Count );

			s.Seek( 0, SeekOrigin.Begin );
			for( int i = 0; i < 256; i++ )
			{
				Color c = Color.FromArgb( s.ReadByte(), s.ReadByte(), s.ReadByte() );
				ColourAssert.AreEqual( c, _actual[i], "Colour index " + i );
			}
		}
		#endregion
		
		#region FromStreamTooShortTest
		/// <summary>
		/// Checks that the correct exception is thrown when the FromStream 
		/// method is passed a stream which is shorter than 768 bytes.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentException ) )]
		public void FromStreamTooShortTest()
		{
			MemoryStream s = new MemoryStream();
			for( int i = 0; i < 767; i++ )
			{
				int b;
				Math.DivRem( i, 256, out b );
				s.WriteByte( (byte) b );
			}
			s.Seek( 0, SeekOrigin.Begin );
			
			try
			{
				_actual = Palette.FromStream( s );
			}
			catch( ArgumentException ex )
			{
				string message
					= "Adobe Colour Table files should be exactly "
					+ "768 bytes long, and the supplied stream is "
					+ "767 bytes long";
				Assert.AreEqual( "inputStream", ex.ParamName );
				StringAssert.Contains( message, ex.Message );
				throw;
			}
		}
		#endregion
		
		#region WriteToFileTest
		/// <summary>
		/// Test case for the WriteToFile method.
		/// Reads in each of the sample palettes, saves them to a new file, and
		/// then compares the saved file with the original file.
		/// Also implicitly tests the WriteToStream method.
		/// </summary>
		[Test]
		public void WriteToFileTest()
		{
			foreach( string file in _paletteFiles )
			{
				_expected = Palette.FromFile( file );
				string saveFile = Path.GetFileName( file );
				_expected.WriteToFile( saveFile );
				
				byte[] expected = File.ReadAllBytes( file );
				byte[] actual = File.ReadAllBytes( saveFile );
				CollectionAssert.AreEqual( expected, actual, saveFile );
			}
		}
		#endregion
		
		#region AddTest
		/// <summary>
		/// Test case for the Add method.
		/// </summary>
		[Test]
		public void AddTest()
		{
			_actual = new Palette();
			
			// Add a first colour and check it's the only one in the palette
			_actual.Add( Color.AliceBlue );
			Assert.AreEqual( 1, _actual.Count );
			ColourAssert.AreEqual( Color.AliceBlue, _actual[0] );
			
			// Add a second colour and check both are in the palette
			_actual.Add( Color.AntiqueWhite );
			Assert.AreEqual( 2, _actual.Count );
			ColourAssert.AreEqual( Color.AliceBlue, _actual[0] );
			ColourAssert.AreEqual( Color.AntiqueWhite, _actual[1] );
			
			// Add a colour that's already in the palette, and check nothing has changed
			_actual.Add( Color.AliceBlue );
			Assert.AreEqual( 2, _actual.Count );
			ColourAssert.AreEqual( Color.AliceBlue, _actual[0] );
			ColourAssert.AreEqual( Color.AntiqueWhite, _actual[1] );
			
			// Add a third colour and check that all 3 are in the palete
			_actual.Add( Color.Aqua );
			Assert.AreEqual( 3, _actual.Count );
			ColourAssert.AreEqual( Color.AliceBlue, _actual[0] );
			ColourAssert.AreEqual( Color.AntiqueWhite, _actual[1] );
			ColourAssert.AreEqual( Color.Aqua, _actual[2] );
		}
		#endregion

		#region AddTestMaxColours
		/// <summary>
		/// Checks that the correct exception is thrown when the Add method is
		/// used to add a colour to a palette which already contains 256 colours.
		/// </summary>
		[Test]
		[ExpectedException( typeof( InvalidOperationException ) )]
		public void AddTestMaxColours()
		{
			_actual = new Palette();
			for( int i = 0; i < 256; i++ )
			{
				Color c = Color.FromArgb( i, i, i );
				_actual.Add( c );
				Assert.AreEqual( i + 1, _actual.Count );
			}
			try
			{
				_actual.Add( Color.FromArgb( 1, 2, 3 ) );
			}
			catch( InvalidOperationException ex )
			{
				string message
					= "This palette already contains the maximum number of "
					+ "colours allowed.";
				StringAssert.Contains( message, ex.Message );
				throw;
			}
		}
		#endregion

		#region ToStringTest
		/// <summary>
		/// Test case for the ToString method.
		/// </summary>
		[Test]
		public void ToStringTest()
		{
			_actual = new Palette();
			
			for( int i = 0; i < 256; i++ )
			{
				Color c = Color.FromArgb( i, i, i );
				_actual.Add( c );
				Assert.AreEqual( (i + 1) + " colours", _actual.ToString() );
			}
		}
		#endregion

		#region ToBitmapTest
		/// <summary>
		/// Test case for the ToBitmap method.
		/// </summary>
		[Test]
		public void ToBitmapTest()
		{
			foreach( string file in _paletteFiles )
			{
				_actual = Palette.FromFile( file );
				Bitmap b = _actual.ToBitmap();
				
				Bitmap expected = new Bitmap( file.Replace( "ColourTables", "images/PaletteBitmaps" ).Replace( ".act", ".bmp" ) );
				BitmapAssert.AreEqual( expected, b, file );
			}
		}
		#endregion
	}
}

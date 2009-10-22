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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using NUnit.Framework;

namespace GifComponents.NUnit
{
	/// <summary>
	/// Test fixture for the ColourTable class.
	/// </summary>
	[TestFixture]
	public class ColourTableTest
	{
		private ColourTable _table;

		#region indexer tests
		
		#region IndexerTest
		/// <summary>
		/// Checks that the indexer works correctly under normal circumstances.
		/// </summary>
		[Test]
		public void IndexerTest()
		{
			Stream s = PrepareStream();

			_table = ColourTable.FromStream( s, 4 );
			
			Random r = new Random();
			for( int i = 0; i < _table.Length; i++ )
			{
				Color c = Color.FromArgb( r.Next() );
				_table[i] = c;
				Assert.AreEqual( c, _table[i], "Index " + i );
				Assert.AreEqual( c, _table.Colours[i], "Index " + i );
			}
		}
		#endregion
		
		#region IndexerGetTestIndexOutOfRange
		/// <summary>
		/// Checks that the correct exception is thrown when the get accessor
		/// of the indexer is passed an index which is out of range.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		[SuppressMessage("Microsoft.Performance", 
		                 "CA1804:RemoveUnusedLocals", 
		                 MessageId = "c")]
		public void IndexerGetTestIndexOutOfRange()
		{
			Stream s = PrepareStream();
			_table = ColourTable.FromStream( s, 4 );
			try
			{
				Color c = _table[4];
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message = "Colour table size: 4. Index: " + 4;
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "index", ex.ParamName );
				throw;
			}
		}
		#endregion
		
		#region IndexerGetTestIndexOutOfRange
		/// <summary>
		/// Checks that the correct exception is thrown when the set accessor
		/// of the indexer is passed an index which is out of range.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void IndexerSetTestIndexOutOfRange()
		{
			Stream s = PrepareStream();
			_table = ColourTable.FromStream( s, 4 );
			try
			{
				_table[4] = Color.FromArgb( 0 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message = "Colour table size: 4. Index: " + 4;
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "index", ex.ParamName );
				throw;
			}
		}
		#endregion
		
		#endregion
		
		#region FromStream tests
		
		#region FromStreamTest
		/// <summary>
		/// Tests that the FromStream method works correctly under normal
		/// circumstances.
		/// </summary>
		[Test]
		public void FromStreamTest()
		{
			Stream s = PrepareStream();

			_table = ColourTable.FromStream( s, 4 );
			Assert.AreEqual( 4, _table.Length );
			Assert.AreEqual( 4, _table.Colours.Length );
			Assert.AreEqual( Color.FromArgb( 255, 0, 0, 0 ), _table.Colours[0] );
			Assert.AreEqual( Color.FromArgb( 255, 255, 0, 0 ), _table.Colours[1] );
			Assert.AreEqual( Color.FromArgb( 255, 0, 255, 0 ), _table.Colours[2] );
			Assert.AreEqual( Color.FromArgb( 255, 0, 0, 255 ), _table.Colours[3] );
			Assert.AreEqual( Color.FromArgb( 255, 0, 0, 0 ), _table[0] );
			Assert.AreEqual( Color.FromArgb( 255, 255, 0, 0 ), _table[1] );
			Assert.AreEqual( Color.FromArgb( 255, 0, 255, 0 ), _table[2] );
			Assert.AreEqual( Color.FromArgb( 255, 0, 0, 255 ), _table[3] );
			Assert.AreEqual( ErrorState.Ok, _table.ConsolidatedState );
		}
		#endregion
		
		#region FromStreamTestTooManyColours
		/// <summary>
		/// Checks that the FromStream method throws the correct exception 
		/// when the number of colours parameter is too large.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void FromStreamTestTooManyColours()
		{
			Stream s = new MemoryStream();
			try
			{
				_table = ColourTable.FromStream( s, 257 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "The number of colours must be between 0 and 256. "
					+ "Number supplied: 257";
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "numberOfColours", ex.ParamName );
				throw;
			}
		}
		#endregion

		#region FromStreamTestTooFewColours
		/// <summary>
		/// Checks that the FromStream method throws the correct exception 
		/// when the number of colours parameter is too small.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void FromStreamTestTooFewColours()
		{
			Stream s = new MemoryStream();
			try
			{
				_table = ColourTable.FromStream( s, -1 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "The number of colours must be between 0 and 256. "
					+ "Number supplied: -1";
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "numberOfColours", ex.ParamName );
				throw;
			}
		}
		#endregion

		#region FromStreamTestColourTableTooShort
		/// <summary>
		/// Tests that the correct error status is set when the FromStream
		/// method is passed a number of colours which is greater than the
		/// number of colours actually in the stream.
		/// </summary>
		[Test]
		public void FromStreamTestColourTableTooShort()
		{
			Stream s = PrepareStream();

			// Pass larger number of colours than are actually in the stream
			_table = ColourTable.FromStream( s, 5 );
			Assert.AreEqual( ErrorState.ColourTableTooShort, _table.ErrorState );
			int i = 0;
			foreach( Color c in _table.Colours )
			{
				Assert.AreEqual( Color.FromArgb( 0 ), c, "Colour " + i );
				i++;
			}
		}
		#endregion

		#endregion
		
		#region WriteToStreamTest
		/// <summary>
		/// Checks that the WriteToStream method works correctly.
		/// </summary>
		[Test]
		public void WriteToStreamTest()
		{
			_table = new ColourTable();
			_table.Add( Color.FromArgb( 255, 255, 0, 0 ) ); // red
			_table.Add( Color.FromArgb( 255, 0, 255, 0 ) ); // green
			_table.Add( Color.FromArgb( 255, 0, 0, 255 ) ); // blue
			_table.Add( Color.FromArgb( 255, 255, 255, 255 ) ); // white
			
			MemoryStream s = new MemoryStream();
			_table.WriteToStream( s );
			s.Seek( 0, SeekOrigin.Begin );
			
			ColourTable t = ColourTable.FromStream( s, 4 );
			Assert.AreEqual( ErrorState.Ok, t.ConsolidatedState );
			Assert.AreEqual( _table.Colours.Length, t.Colours.Length );
			for( int i = 0; i < t.Colours.Length; i++ )
			{
				Assert.AreEqual( _table[i], t[i], "Colour index " + i );
			}
		}
		#endregion
		
		#region internal static PrepareStream method
		/// <summary>
		/// Returns a MemoryStream containing four RGB colour values, suitable 
		/// for passing to the FromStream method.
		/// </summary>
		/// <returns></returns>
		internal static Stream PrepareStream()
		{
			byte[] colour1 = new byte[3] { 0, 0, 0 }; // black
			byte[] colour2 = new byte[3] { 255, 0, 0 }; // red
			byte[] colour3 = new byte[3] { 0, 255, 0 }; // green
			byte[] colour4 = new byte[3] { 0, 0, 255 }; // blue
			Stream s = new MemoryStream();
			s.Write( colour1, 0, 3 );
			s.Write( colour2, 0, 3 );
			s.Write( colour3, 0, 3 );
			s.Write( colour4, 0, 3 );
			s.Seek( 0, SeekOrigin.Begin ); // point to start of stream
			return s;
		}
		#endregion
	}
}

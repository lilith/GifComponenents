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
using GifComponents.Components;

namespace GifComponents.NUnit.Components
{
	/// <summary>
	/// Test fixture for the ColourTable class.
	/// </summary>
	[TestFixture]
	public class ColourTableTest : GifComponentTestFixtureBase, IDisposable
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
			ReportStart();
			Stream s = PrepareStream();

			_table = new ColourTable ( s, 4 );
			
			Random r = new Random();
			for( int i = 0; i < _table.Length; i++ )
			{
				Color c = Color.FromArgb( r.Next() );
				_table[i] = c;
				Assert.AreEqual( c, _table[i], "Index " + i );
				Assert.AreEqual( c, _table.Colours[i], "Index " + i );
			}
			ReportEnd();
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
			ReportStart();
			Stream s = PrepareStream();
			_table = new ColourTable( s, 4 );
			try
			{
				Color c = _table[4];
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message = "Colour table size: 4. Index: " + 4;
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "index", ex.ParamName );
				ReportEnd();
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
			ReportStart();
			Stream s = PrepareStream();
			_table = new ColourTable( s, 4 );
			try
			{
				_table[4] = Color.FromArgb( 0 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message = "Colour table size: 4. Index: " + 4;
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "index", ex.ParamName );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#endregion
		
		#region constructor( stream ) tests
		
		#region ConstructorStreamTest
		/// <summary>
		/// Tests that the constructor( stream) works correctly under normal 
		/// circumstances.
		/// </summary>
		[Test]
		public void ConstructorStreamTest()
		{
			ReportStart();
			ConstructorStreamTest( true );
			ConstructorStreamTest( false );
			ReportEnd();
		}
		
		private void ConstructorStreamTest( bool xmlDebugging )
		{
			Stream s = PrepareStream();

			_table = new ColourTable( s, 4, xmlDebugging );
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
			
			if( xmlDebugging )
			{
				Assert.AreEqual( ExpectedDebugXml, _table.DebugXml );
			}
		}
		#endregion
		
		#region ConstructorStreamTestTooManyColours
		/// <summary>
		/// Checks that the constructor( stream ) throws the correct exception 
		/// when the number of colours parameter is too large.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void ConstructorStreamTestTooManyColours()
		{
			ReportStart();
			Stream s = new MemoryStream();
			try
			{
				_table = new ColourTable( s, 257 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "The number of colours must be between 0 and 256. "
					+ "Number supplied: 257";
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "numberOfColours", ex.ParamName );
				ReportEnd();
				throw;
			}
		}
		#endregion

		#region ConstructorStreamTestTooFewColours
		/// <summary>
		/// Checks that the constructor( Stream ) throws the correct exception 
		/// when the number of colours parameter is too small.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void ConstructorStreamTestTooFewColours()
		{
			ReportStart();
			Stream s = new MemoryStream();
			try
			{
				_table = new ColourTable( s, -1 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "The number of colours must be between 0 and 256. "
					+ "Number supplied: -1";
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "numberOfColours", ex.ParamName );
				ReportEnd();
				throw;
			}
		}
		#endregion

		#region ConstructorStreamTestColourTableTooShort
		/// <summary>
		/// Tests that the correct error status is set when the 
		/// constructor( stream ) is passed a number of colours which is greater 
		/// than the number of colours actually in the stream.
		/// </summary>
		[Test]
		public void ConstructorStreamTestColourTableTooShort()
		{
			ReportStart();
			ConstructorStreamTestColourTableTooShort( true );
			ConstructorStreamTestColourTableTooShort( false );
			ReportEnd();
		}
		private void ConstructorStreamTestColourTableTooShort( bool xmlDebugging )
		{
			Stream s = PrepareStream();

			// Pass larger number of colours than are actually in the stream
			_table = new ColourTable( s, 5, xmlDebugging );
			Assert.AreEqual( ErrorState.ColourTableTooShort, _table.ErrorState );
			Assert.AreEqual( 5, _table.Length );
			Assert.AreEqual( Color.FromArgb( 255, 0, 0, 0 ), _table[0] );
			Assert.AreEqual( Color.FromArgb( 255, 255, 0, 0 ), _table[1] );
			Assert.AreEqual( Color.FromArgb( 255, 0, 255, 0 ), _table[2] );
			Assert.AreEqual( Color.FromArgb( 255, 0, 0, 255 ), _table[3] );
			// missing colour should be set to black by the constructor
			Assert.AreEqual( Color.FromArgb( 0, 0, 0, 0 ), _table[4] );
			
			if( xmlDebugging )
			{
				Assert.AreEqual( ExpectedDebugXml, _table.DebugXml );
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
			ReportStart();
			_table = new ColourTable();
			_table.Add( Color.FromArgb( 255, 255, 0, 0 ) ); // red
			_table.Add( Color.FromArgb( 255, 0, 255, 0 ) ); // green
			_table.Add( Color.FromArgb( 255, 0, 0, 255 ) ); // blue
			_table.Add( Color.FromArgb( 255, 255, 255, 255 ) ); // white
			
			MemoryStream s = new MemoryStream();
			_table.WriteToStream( s );
			s.Seek( 0, SeekOrigin.Begin );
			
			ColourTable t = new ColourTable( s, 4 );
			Assert.AreEqual( ErrorState.Ok, t.ConsolidatedState );
			Assert.AreEqual( _table.Colours.Length, t.Colours.Length );
			for( int i = 0; i < t.Colours.Length; i++ )
			{
				Assert.AreEqual( _table[i], t[i], "Colour index " + i );
			}
			ReportEnd();
		}
		#endregion
		
		#region internal static PrepareStream method
		/// <summary>
		/// Returns a MemoryStream containing four RGB colour values, suitable 
		/// for passing to the ConstructorStream method.
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

		#region IDisposable implementation
		/// <summary>
		/// Indicates whether or not the Dispose( bool ) method has already been 
		/// called.
		/// </summary>
		bool _disposed;

		/// <summary>
		/// Finalzer.
		/// </summary>
		~ColourTableTest()
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
					_table.Dispose();
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

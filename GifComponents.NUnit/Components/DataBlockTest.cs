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
using System.IO;
using NUnit.Framework;
using GifComponents.Components;

namespace GifComponents.NUnit.Components
{
	/// <summary>
	/// Test fixture for the DataBlock class.
	/// </summary>
	[TestFixture]
	public class DataBlockTest : GifComponentTestFixtureBase, IDisposable
	{
		private DataBlock _block;
		private byte[] _data;
		
		#region Setup method
		/// <summary>
		/// Setup method.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			_data = new byte[] { 12, 16, 5, 7 };
		}
		#endregion
		
		#region constructor tests
		
		#region ConstructorTest
		/// <summary>
		/// Checks that the constructor works correctly under normal 
		/// circumstances.
		/// </summary>
		[Test]
		public void ConstructorTest()
		{
			ReportStart();
			_block = new DataBlock( 4, _data );
			Assert.AreEqual( 4, _block.ActualBlockSize );
			Assert.AreEqual( 4, _block.DeclaredBlockSize );
			Assert.AreEqual( ErrorState.Ok, _block.ConsolidatedState );
			Assert.AreEqual( _data.Length, _block.Data.Length );
			for( int i = 0; i < _data.Length; i++ )
			{
				Assert.AreEqual( _data[i], _block.Data[i], i );
				Assert.AreEqual( _data[i], _block[i], i );
			}
			ReportEnd();
		}
		#endregion
		
		#region ConstructorTestBlockSizeTooSmall
		/// <summary>
		/// Checks that the correct error status is set when the supplied block
		/// size is smaller than the size of the supplied byte array.
		/// </summary>
		[Test]
		public void ConstructorTestBlockSizeTooSmall()
		{
			ReportStart();
			_block = new DataBlock( 5, _data );
			Assert.AreEqual( 4, _block.ActualBlockSize );
			Assert.AreEqual( 5, _block.DeclaredBlockSize );
			Assert.AreEqual( ErrorState.DataBlockTooShort, _block.ErrorState );
			Assert.AreEqual( _data.Length, _block.Data.Length );
			for( int i = 0; i < _data.Length; i++ )
			{
				Assert.AreEqual( _data[i], _block.Data[i], i );
			}
			ReportEnd();
		}
		#endregion
		
		#region ConstructorTestBlockSizeTooLarge
		/// <summary>
		/// Checks that the correct error status is set when the supplied block
		/// size is larger than the size of the supplied byte array.
		/// </summary>
		[Test]
		public void ConstructorTestBlockSizeTooLarge()
		{
			ReportStart();
			_block = new DataBlock( 3, _data );
			Assert.AreEqual( 4, _block.ActualBlockSize );
			Assert.AreEqual( 3, _block.DeclaredBlockSize );
			Assert.AreEqual( ErrorState.DataBlockTooLong, _block.ErrorState );
			Assert.AreEqual( _data.Length, _block.Data.Length );
			for( int i = 0; i < _data.Length; i++ )
			{
				Assert.AreEqual( _data[i], _block.Data[i], i );
			}
			ReportEnd();
		}
		#endregion
		
		#region ConstructorTestNullArgument
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor
		/// is passed a null data argument.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		public void ConstructorTestNullArgument()
		{
			ReportStart();
			try
			{
				_block = new DataBlock( 1, null );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "data", ex.ParamName );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#endregion

		#region ConstructorStream tests
		
		#region ConstructorStreamTest
		/// <summary>
		/// Checks that the constructor( Stream ) works correctly under normal
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
			Stream s = new MemoryStream();
			s.WriteByte( 4 ); // write block size
			s.Write( _data, 0, _data.Length ); // write data block
			s.Seek( 0, SeekOrigin.Begin ); // go to start of stream
			
			_block = new DataBlock( s, xmlDebugging );
			Assert.AreEqual( 4, _block.ActualBlockSize );
			Assert.AreEqual( 4, _block.DeclaredBlockSize );
			Assert.AreEqual( ErrorState.Ok, _block.ConsolidatedState );
			Assert.AreEqual( _data.Length, _block.Data.Length );
			for( int i = 0; i < _data.Length; i++ )
			{
				Assert.AreEqual( _data[i], _block.Data[i], i );
				Assert.AreEqual( _data[i], _block[i], i );
			}
			
			if( xmlDebugging )
			{
				Assert.AreEqual( ExpectedDebugXml, _block.DebugXml );
			}
		}
		#endregion
		
		#region ConstructorStreamTestBlockSizeTooSmall
		/// <summary>
		/// Checks that the correct error status is set when the data block
		/// is shorter than its declared size.
		/// </summary>
		[Test]
		public void ConstructorStreamTestBlockSizeTooLarge()
		{
			ReportStart();
			ConstructorStreamTestBlockSizeTooLarge( true );
			ConstructorStreamTestBlockSizeTooLarge( false );
			ReportEnd();
		}
		
		private void ConstructorStreamTestBlockSizeTooLarge( bool xmlDebugging )
		{
			Stream s = new MemoryStream();
			s.WriteByte( 5 ); // write block size
			s.Write( _data, 0, _data.Length ); // write data block
			s.Seek( 0, SeekOrigin.Begin ); // go to start of stream
			
			_block = new DataBlock( s, xmlDebugging );
			Assert.AreEqual( 5, _block.ActualBlockSize );
			Assert.AreEqual( 5, _block.DeclaredBlockSize );
			Assert.AreEqual( ErrorState.DataBlockTooShort, _block.ErrorState );
			Assert.AreEqual( 5, _block.Data.Length );
			for( int i = 0; i < _data.Length; i++ )
			{
				Assert.AreEqual( _data[i], _block.Data[i], i );
			}
			
			if( xmlDebugging )
			{
				Assert.AreEqual( ExpectedDebugXml, _block.DebugXml );
			}
		}
		#endregion
		
		#region ConstructorStreamTestNullArgument
		/// <summary>
		/// Checks that the correct exception is thrown when the 
		/// constructor( Stream )is passed a null stream.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		public void ConstructorStreamTestNullArgument()
		{
			ReportStart();
			try
			{
				_block = new DataBlock( null );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "inputStream", ex.ParamName );
				ReportEnd();
				throw;
			}
		}
		#endregion

		#region ConstructorStreamTestEmptyBlock
		/// <summary>
		/// Tests that the constructor( Stream ) reads an empty block (i.e. just
		/// one byte containing a block length of zero) correctly.
		/// </summary>
		[Test]
		public void ConstructorStreamTestEmptyBlock()
		{
			ReportStart();
			ConstructorStreamTestEmptyBlock( true );
			ConstructorStreamTestEmptyBlock( false );
			ReportEnd();
		}
		private void ConstructorStreamTestEmptyBlock( bool xmlDebugging )
		{
			Stream s = new MemoryStream();
			s.WriteByte( 0 ); // write length (0 bytes)
			s.Seek( 0, SeekOrigin.Begin ); // go to start of stream

			_block = new DataBlock( s, xmlDebugging );
			Assert.AreEqual( 0, _block.DeclaredBlockSize );
			Assert.AreEqual( 0, _block.ActualBlockSize );
			Assert.AreEqual( 0, _block.Data.Length );
			Assert.AreEqual( ErrorState.Ok, _block.ConsolidatedState );
			
			if( xmlDebugging )
			{
				Assert.AreEqual( ExpectedDebugXml, _block.DebugXml );
			}
		}
		#endregion

		#region ConstructorStreamTestEndOfStream
		/// <summary>
		/// Checks that the correct error status is set when the 
		/// constructor( Stream ) is passed a stream which is already at its end.
		/// </summary>
		[Test]
		public void ConstructorStreamTestEndOfStream()
		{
			ReportStart();
			ConstructorStreamTestEndOfStream( true );
			ConstructorStreamTestEndOfStream( false );
			ReportEnd();
		}
		
		private void ConstructorStreamTestEndOfStream( bool xmlDebugging )
		{
			Stream s = new MemoryStream();
			s.Seek( 0, SeekOrigin.Begin );
			
			_block = new DataBlock( s, xmlDebugging );
			Assert.AreEqual( ErrorState.EndOfInputStream, _block.ErrorState );
			if( xmlDebugging )
			{
				Assert.AreEqual( ExpectedDebugXml, _block.DebugXml );
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
			MemoryStream s = new MemoryStream();
			_block.WriteToStream( s );
			s.Seek( 0, SeekOrigin.Begin );
			
			DataBlock d = new DataBlock( s );
			Assert.AreEqual( ErrorState.Ok, d.ConsolidatedState );
			Assert.AreEqual( _block.DeclaredBlockSize, d.DeclaredBlockSize );
			Assert.AreEqual( _block.ActualBlockSize, d.ActualBlockSize );
			for( int i = 0; i < _block.ActualBlockSize; i++ )
			{
				Assert.AreEqual( _block.Data[i], d.Data[i], "byte " + i );
			}
			ReportEnd();
		}
		#endregion

		#region IndexerTestArgumentOutOfRange
		/// <summary>
		/// Checks that the correct exception is thrown when the indexer is
		/// passed an index which is outside the bounds of the array.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		[SuppressMessage("Microsoft.Performance", 
		                 "CA1804:RemoveUnusedLocals", 
		                 MessageId = "b")]
		public void IndexerTestArgumentOutOfRange()
		{
			ReportStart();
			_block = new DataBlock( 4, _data );
			try
			{
				byte b = _block[4];
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "Supplied index: 4. Array length: 4";
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "index", ex.ParamName );
				ReportEnd();
				throw;
			}
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
		~DataBlockTest()
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
					_block.Dispose();
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

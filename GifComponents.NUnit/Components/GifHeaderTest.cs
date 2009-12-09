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
using System.IO;
using NUnit.Framework;
using GifComponents.Components;

namespace GifComponents.NUnit.Components
{
	/// <summary>
	/// Test fixture for the GifHeader class.
	/// </summary>
	[TestFixture]
	public class GifHeaderTest : GifComponentTestFixtureBase, IDisposable
	{
		private GifHeader _header;

		#region ConstructorPropertiesTest
		/// <summary>
		/// Checks that the constructor works correctly under normal 
		/// circumstances.
		/// </summary>
		[Test]
		public void ConstructorPropertiesTest()
		{
			ReportStart();
			
			_header = new GifHeader( "GIF", "89A" );
			Assert.AreEqual( "GIF", _header.Signature );
			Assert.AreEqual( "89A", _header.Version );
			Assert.AreEqual( ErrorState.Ok, _header.ConsolidatedState );
			
			ReportEnd();
		}
		#endregion
		
		#region ConstructorTestBadSignature
		/// <summary>
		/// Checks that the correct error status is set when the constructor
		/// is passed a GIF signature other than "GIF".
		/// </summary>
		[Test]
		public void ConstructorTestBadSignature()
		{
			ReportStart();
			
			_header = new GifHeader( "FIG", "89A" );
			Assert.AreEqual( "FIG", _header.Signature );
			Assert.AreEqual( "89A", _header.Version );
			Assert.AreEqual( ErrorState.BadSignature, _header.ErrorState );
			
			ReportEnd();
		}
		#endregion

		#region ConstructorStreamTest
		/// <summary>
		/// Checks that the constructor( Stream ) method works correctly.
		/// </summary>
		[Test]
		public void ConstructorStreamTest()
		{
			ReportStart();
			
			byte[] bytes = new byte[]
			{
				(byte) 'G',
				(byte) 'I',
				(byte) 'F',
				(byte) '8',
				(byte) '9',
				(byte) 'A',
			};
			MemoryStream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Seek( 0, SeekOrigin.Begin );
			
			string expectedSignature = "GIF";
			string expectedVersion = "89A";
			ErrorState expectedErrorState = ErrorState.Ok;
			string expectedErrorMessage = "";
			CheckConstructorStream( s, 
			                        expectedSignature,
			                        expectedVersion,
			                        expectedErrorState,
			                        expectedErrorMessage,
			                        ExpectedDebugXml );
			
			ReportEnd();
		}
		#endregion

		#region ConstructorStreamEndOfInputStreamTest
		/// <summary>
		/// Checks that the correct error state is set when the input stream
		/// does not contain enough data to form a GIF header.
		/// </summary>
		[Test]
		public void ConstructorStreamEndOfInputStreamTest()
		{
			ReportStart();
			
			MemoryStream s = new MemoryStream();
			s.WriteByte( (byte) 'G' );
			s.Seek( 0, SeekOrigin.Begin );
			string expectedSignature = "G\0\0"; // ends with 2 nulls
			string expectedVersion = "\0\0\0"; // 3 nulls
			ErrorState expectedErrorState
				= ErrorState.EndOfInputStream | ErrorState.BadSignature;
			string expectedErrorMessage 
				= "Bytes read: 1"
				+ Environment.NewLine
				+ "Bad signature: G\0\0";
			CheckConstructorStream( s, 
			                        expectedSignature,
			                        expectedVersion,
			                        expectedErrorState,
			                        expectedErrorMessage,
			                        ExpectedDebugXml );
			
			ReportEnd();
		}
		#endregion
		
		#region private CheckConstructorStream method
		private void CheckConstructorStream( Stream s, 
		                                     string expectedSignature,
		                                     string expectedVersion,
		                                     ErrorState expectedErrorState,
		                                     string expectedErrorMessage,
		                                     string expectedXml )
		{
			// Save our current position in the stream, in case we want to go
			// back and read it again later.
			long streamPosition = s.Position;
			
			// Without XML debugging
			_header = new GifHeader( s );
			CheckProperties( expectedSignature, 
			                 expectedVersion, 
			                 expectedErrorState, 
			                 expectedErrorMessage );
			string message
				= "<Message>There is no DebugXml because XML debugging has not "
				+ "been enabled for this GifHeader instance.</Message>";
			Assert.AreEqual( message, _header.DebugXml );
			
			// Go back to where we were in the stream before we try reading it again
			s.Seek( streamPosition, SeekOrigin.Begin );
			
			// With XML debugging
			_header = new GifHeader( s, true );
			CheckProperties( expectedSignature, 
			                 expectedVersion, 
			                 expectedErrorState, 
			                 expectedErrorMessage );
			Assert.AreEqual( expectedXml, _header.DebugXml, "DebugXml" );
		}
		#endregion
		
		#region private CheckProperties method
		private void CheckProperties( string expectedSignature, 
		                              string expectedVersion, 
		                              ErrorState expectedErrorState, 
		                              string expectedErrorMessage )
		{
			Assert.AreEqual( expectedSignature, _header.Signature, "Signature" );
			Assert.AreEqual( expectedVersion, _header.Version, "Version" );
			Assert.AreEqual( expectedErrorState, _header.ErrorState, "ErrorState" );
			Assert.AreEqual( expectedErrorMessage, _header.ErrorMessage, "ErrorMessage" );
		}
		#endregion
		
		#region WriteToStreamTest
		/// <summary>
		/// Checks that the WriteToStream method works correctly.
		/// </summary>
		[Test]
		public void WriteToStreamTest()
		{
			ReportStart();
			
			_header = new GifHeader( "GIF", "87a" );
			MemoryStream s = new MemoryStream();
			_header.WriteToStream( s );
			s.Seek( 0, SeekOrigin.Begin );
			_header = new GifHeader( s );
			Assert.AreEqual( ErrorState.Ok, _header.ConsolidatedState );
			Assert.AreEqual( "GIF", _header.Signature );
			Assert.AreEqual( "87a", _header.Version );
			
			ReportEnd();
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
		~GifHeaderTest()
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
					_header.Dispose();
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

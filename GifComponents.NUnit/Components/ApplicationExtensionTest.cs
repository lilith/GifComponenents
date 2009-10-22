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
using System.IO;
using NUnit.Framework;

namespace GifComponents.NUnit
{
	/// <summary>
	/// Test fixture for the ApplicationExtension class.
	/// </summary>
	[TestFixture]
	public class ApplicationExtensionTest
	{
		private ApplicationExtension _ext;

		#region ConstructorTest
		/// <summary>
		/// Tests that the constructor works correctly under normal 
		/// circumstances.
		/// </summary>
		[Test]
		public void ConstructorTest()
		{
			byte[] identification = new byte[]
			{
				(byte) 'N',
				(byte) 'E',
				(byte) 'T',
				(byte) 'S',
				(byte) 'C',
				(byte) 'A',
				(byte) 'P',
				(byte) 'E',
				(byte) '2',
				(byte) '.',
				(byte) '0',
			};
			DataBlock identificationBlock = new DataBlock( 11, identification );
			DataBlock data = new DataBlock( 1, new byte[] { 1 } );
			Collection<DataBlock> applicationData = new Collection<DataBlock>();
			applicationData.Add( data );
			_ext = new ApplicationExtension( identificationBlock, applicationData );
			
			Assert.AreEqual( "NETSCAPE", _ext.ApplicationIdentifier );
			Assert.AreEqual( "2.0", _ext.ApplicationAuthenticationCode );
			Assert.AreEqual( identificationBlock, _ext.IdentificationBlock );
			Assert.AreEqual( applicationData, _ext.ApplicationData );
			Assert.AreEqual( ErrorState.Ok, _ext.ConsolidatedState );
		}
		#endregion
		
		#region ConstructorIdentificationBlockTooLongTest
		/// <summary>
		/// Checks that the correct error status is set if the identification
		/// block contains too many bytes.
		/// </summary>
		[Test]
		public void ConstructorIdentificationBlockTooLongTest()
		{
			byte[] identification = new byte[]
			{
				(byte) 'N',
				(byte) 'E',
				(byte) 'T',
				(byte) 'S',
				(byte) 'C',
				(byte) 'A',
				(byte) 'P',
				(byte) 'E',
				(byte) '2',
				(byte) '.',
				(byte) '0',
				(byte) '1',
			};
			DataBlock identificationBlock = new DataBlock( 11, identification );
			DataBlock data = new DataBlock( 1, new byte[] { 1 } );
			Collection<DataBlock> applicationData = new Collection<DataBlock>();
			applicationData.Add( data );
			_ext = new ApplicationExtension( identificationBlock, applicationData );

			Assert.AreEqual( "NETSCAPE", _ext.ApplicationIdentifier );
			Assert.AreEqual( "2.0", _ext.ApplicationAuthenticationCode );
			Assert.AreEqual( identificationBlock, _ext.IdentificationBlock );
			Assert.AreEqual( applicationData, _ext.ApplicationData );
			Assert.AreEqual( ErrorState.IdentificationBlockTooLong, _ext.ErrorState );
		}
		#endregion
		
		#region ConstructorIdentificationBlockTooShortTest
		/// <summary>
		/// Checks that the correct exception is thrown if the identification
		/// block contains too many bytes.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentException ) )]
		public void ConstructorIdentificationBlockTooShortTest()
		{
			byte[] identification = new byte[]
			{
				(byte) 'N',
				(byte) 'E',
				(byte) 'T',
				(byte) 'S',
				(byte) 'C',
				(byte) 'A',
				(byte) 'P',
				(byte) 'E',
				(byte) '2',
				(byte) '.',
			};
			DataBlock identificationBlock = new DataBlock( 11, identification );
			DataBlock data = new DataBlock( 1, new byte[] { 1 } );
			Collection<DataBlock> applicationData = new Collection<DataBlock>();
			applicationData.Add( data );
			
			try
			{
				_ext = new ApplicationExtension( identificationBlock, 
				                                 applicationData );
			}
			catch( ArgumentException ex )
			{
				string message
					= "The identification block should be 11 bytes long but "
					+ "is only 10 bytes.";
				StringAssert.Contains( message, ex.Message );
				throw;
			}
		}
		#endregion
		
		#region FromStreamTest
		/// <summary>
		/// Tests that the FromStream method works correctly under normal
		/// circumstances.
		/// </summary>
		[Test]
		public void FromStreamTest()
		{
			Stream s = new MemoryStream();
			
			WriteApplicationIdentifier( s );
			WriteHello( s );
			WriteWorld( s );
			WriteBlockTerminator( s );
			
			s.Seek( 0, SeekOrigin.Begin ); // point to beginning of stream
			
			_ext = ApplicationExtension.FromStream( s );

			CheckExtension( _ext );
			
			// Check block terminator
			Assert.AreEqual( 0, _ext.ApplicationData[2].DeclaredBlockSize );
			Assert.AreEqual( 0, _ext.ApplicationData[2].ActualBlockSize );
			Assert.AreEqual( ErrorState.Ok, 
			                 _ext.ApplicationData[2].ConsolidatedState );
		}
		#endregion
		
		#region FromStreamTestNoBlockTerminator
		/// <summary>
		/// Checks that the correct error status is set when the FromStream
		/// method is passed a stream where the application extension has no 
		/// block terminator.
		/// </summary>
		[Test]
		public void FromStreamTestNoBlockTerminator()
		{
			Stream s = new MemoryStream();
			
			WriteApplicationIdentifier( s );
			WriteHello( s );
			WriteWorld( s );
			// Don't write a block terminator - that's the point of this test
			
			s.Seek( 0, SeekOrigin.Begin ); // point to start of stream
			
			// Instantiate the ApplicationExtension
			_ext = ApplicationExtension.FromStream( s );

			Assert.AreEqual( ErrorState.EndOfInputStream, _ext.ConsolidatedState );
		}
		#endregion
		
		#region WriteToStreamTest
		/// <summary>
		/// Checks that the WriteToStream method works correctly
		/// </summary>
		[Test]
		public void WriteToStreamTest()
		{
			byte[] identificationData = new byte[]
			{
				(byte) 'B', (byte) 'I', (byte) 'G', (byte) 'P', 
				(byte) 'A', (byte) 'N', (byte) 'T', (byte) 'S',
				(byte) '1', (byte) '.', (byte) '3'
			};
			DataBlock identificationBlock = new DataBlock( 11, identificationData );
			
			byte[] helloData = new byte[]
			{
				(byte) 'H', (byte) 'E', (byte) 'L', (byte) 'L', (byte) 'O'
			};
			DataBlock helloBlock = new DataBlock( 5, helloData );
			
			byte[] worldData = new byte[]
			{
				(byte) 'W', (byte) 'O', (byte) 'R', (byte) 'L', (byte) 'D'
			};
			DataBlock worldBlock = new DataBlock( 5, worldData );
			
			DataBlock blockTerminator = new DataBlock( 0, new byte[] { 0 } );
			
			Collection<DataBlock> applicationData = new Collection<DataBlock>();
			applicationData.Add( helloBlock );
			applicationData.Add( worldBlock );
			applicationData.Add( blockTerminator );
			
			_ext = new ApplicationExtension( identificationBlock, applicationData );
			
			MemoryStream s = new MemoryStream();
			_ext.WriteToStream( s );
			s.Seek( 0, SeekOrigin.Begin );
			
			ApplicationExtension e = ApplicationExtension.FromStream( s );
			
			CheckExtension( e );
		}
		#endregion
		
		#region private static methods
		
		#region private static WriteApplicationIdentifier method
		/// <summary>
		/// Writes the application identifier "BIGPANTS1.3" to the supplied 
		/// stream as an application identifier.
		/// </summary>
		/// <param name="outputStream"></param>
		private static void WriteApplicationIdentifier( Stream outputStream )
		{
			outputStream.WriteByte( 11 ); // write block size 11
			byte[] applicationIdentifier = new byte[]
			{
				(byte) 'B',
				(byte) 'I',
				(byte) 'G',
				(byte) 'P',
				(byte) 'A',
				(byte) 'N',
				(byte) 'T',
				(byte) 'S'
			};
			outputStream.Write( applicationIdentifier, 0, 8 );
			byte[] appAuthenticationCode = new byte[]
			{
				(byte) '1',
				(byte) '.',
				(byte) '3',
			};
			outputStream.Write( appAuthenticationCode, 0, 3 );
		}
		#endregion
		
		#region private static WriteHello method
		/// <summary>
		/// Writes a data block containing the word "HELLO" to the supplied
		/// output stream
		/// </summary>
		/// <param name="outputStream"></param>
		private static void WriteHello( Stream outputStream )
		{
			outputStream.WriteByte( 5 ); // write block size 5
			byte[] data = new byte[]
			{
				(byte) 'H',
				(byte) 'E',
				(byte) 'L',
				(byte) 'L',
				(byte) 'O',
			};
			outputStream.Write( data, 0, 5 );
		}
		#endregion
		
		#region private static WriteWorld method
		/// <summary>
		/// Writes a data block containing the word "WORLD" to the supplied
		/// output stream
		/// </summary>
		/// <param name="outputStream"></param>
		private static void WriteWorld( Stream outputStream )
		{
			outputStream.WriteByte( 5 ); // write block size 5
			byte[] data = new byte[]
			{
				(byte) 'W',
				(byte) 'O',
				(byte) 'R',
				(byte) 'L',
				(byte) 'D',
			};
			outputStream.Write( data, 0, 5 );
		}
		#endregion
		
		#region private static WriteBlockTerminator method
		/// <summary>
		/// Writes a zero-length data block to the supplied output stream,
		/// indicating the end of the data sub-blocks and therefore the end
		/// of the application extension.
		/// </summary>
		/// <param name="outputStream"></param>
		private static void WriteBlockTerminator( Stream outputStream )
		{
			outputStream.WriteByte( 0 );
		}
		#endregion

		#region private static CheckExtension method
		private static void CheckExtension( ApplicationExtension ext )
		{
			// Check application identifier / authentication code
			Assert.AreEqual( "BIGPANTS", ext.ApplicationIdentifier );
			Assert.AreEqual( "1.3", ext.ApplicationAuthenticationCode );
			Assert.AreEqual( ErrorState.Ok, ext.ConsolidatedState );
			
			// Check number of blocks in application data
			Assert.AreEqual( 3, ext.ApplicationData.Count );
			
			// Check application data block 1
			Assert.AreEqual( 5, ext.ApplicationData[0].DeclaredBlockSize );
			Assert.AreEqual( 5, ext.ApplicationData[0].ActualBlockSize );
			Assert.AreEqual( (byte) 'H', ext.ApplicationData[0][0] );
			Assert.AreEqual( (byte) 'E', ext.ApplicationData[0][1] );
			Assert.AreEqual( (byte) 'L', ext.ApplicationData[0][2] );
			Assert.AreEqual( (byte) 'L', ext.ApplicationData[0][3] );
			Assert.AreEqual( (byte) 'O', ext.ApplicationData[0][4] );
			Assert.AreEqual( ErrorState.Ok, ext.ApplicationData[0].ConsolidatedState );

			// Check application data block 2
			Assert.AreEqual( 5, ext.ApplicationData[1].DeclaredBlockSize );
			Assert.AreEqual( 5, ext.ApplicationData[1].ActualBlockSize );
			Assert.AreEqual( (byte) 'W', ext.ApplicationData[1][0] );
			Assert.AreEqual( (byte) 'O', ext.ApplicationData[1][1] );
			Assert.AreEqual( (byte) 'R', ext.ApplicationData[1][2] );
			Assert.AreEqual( (byte) 'L', ext.ApplicationData[1][3] );
			Assert.AreEqual( (byte) 'D', ext.ApplicationData[1][4] );
			Assert.AreEqual( ErrorState.Ok, ext.ApplicationData[1].ConsolidatedState );
		}
		#endregion

		#endregion
	}
}

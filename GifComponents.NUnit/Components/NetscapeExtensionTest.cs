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
	/// Test fixture for the NetscapeExtension class.
	/// </summary>
	[TestFixture]
	public class NetscapeExtensionTest
	{
		private NetscapeExtension _ne;
		
		#region ConstructorIntTest
		/// <summary>
		/// Checks that the constructor which accepts an integer value works
		/// correctly.
		/// </summary>
		[Test]
		public void ConstructorIntTest()
		{
			_ne = new NetscapeExtension( 257 );
			
			Assert.AreEqual( "NETSCAPE", _ne.ApplicationIdentifier );
			Assert.AreEqual( "2.0", _ne.ApplicationAuthenticationCode );
			Assert.AreEqual( 2, _ne.ApplicationData.Count );
			
			Assert.AreEqual( 3, _ne.ApplicationData[0].Data.Length );
			Assert.AreEqual( 1, _ne.ApplicationData[0].Data[0] );
			Assert.AreEqual( 1, _ne.ApplicationData[0].Data[1] );
			Assert.AreEqual( 1, _ne.ApplicationData[0].Data[2] );
			
			Assert.AreEqual( 0, _ne.ApplicationData[1].Data.Length );
			
			Assert.AreEqual( 257, _ne.LoopCount );
		}
		#endregion
		
		#region ConstructorTest
		/// <summary>
		/// Checks that the constructor works correctly under normal 
		/// circumstances.
		/// </summary>
		[Test]
		public void ConstructorTest()
		{
			byte[] idData = new byte[]
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
			DataBlock idBlock = new DataBlock( 11, idData );
			
			Collection<DataBlock> appData = new Collection<DataBlock>();

			// First byte in the data block is always 1 for a netscape extension
			// Second and third bytes are the loop count, lsb first
			byte[] loopCount = new byte[] { 1, 4, 0 };
			appData.Add( new DataBlock( 3, loopCount ) );
			
			// Add the block terminator
			appData.Add( new DataBlock( 0, new byte[0] ) );

			ApplicationExtension appExt
				= new ApplicationExtension( idBlock, appData );
			_ne = new NetscapeExtension( appExt );
			
			Assert.AreEqual( 4, _ne.LoopCount );
			Assert.AreEqual( "NETSCAPE", _ne.ApplicationIdentifier );
			Assert.AreEqual( "2.0", _ne.ApplicationAuthenticationCode );
			Assert.AreEqual( ErrorState.Ok, _ne.ConsolidatedState );
		}
		#endregion

		#region ConstructorTestNotNetscape
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor
		/// is passed an identification block where the application identifier
		/// is not "NETSCAPE".
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentException ) )]
		public void ConstructorTestNotNetscape()
		{
			byte[] idData = new byte[]
			{
				(byte) 'B',
				(byte) 'I',
				(byte) 'G',
				(byte) 'P',
				(byte) 'A',
				(byte) 'N',
				(byte) 'T',
				(byte) 'S',
				(byte) '2',
				(byte) '.',
				(byte) '0',
			};
			DataBlock idBlock = new DataBlock( 11, idData );
			
			ApplicationExtension appExt 
				= new ApplicationExtension( idBlock, new Collection<DataBlock>() );
			try
			{
				_ne = new NetscapeExtension( appExt );
			}
			catch( ArgumentException ex )
			{
				string message
					= "The application identifier is not 'NETSCAPE' "
					+ "therefore this application extension is not a "
					+ "Netscape extension. Application identifier: BIGPANTS";
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "applicationExtension", ex.ParamName );
				throw;
			}
		}
		#endregion

		#region ConstructorTestNot2Point0
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor
		/// is passed an identification block where the application 
		/// authentication code is not "2.0".
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentException ) )]
		public void ConstructorTestNot2Point0()
		{
			byte[] idData = new byte[]
			{
				(byte) 'N',
				(byte) 'E',
				(byte) 'T',
				(byte) 'S',
				(byte) 'C',
				(byte) 'A',
				(byte) 'P',
				(byte) 'E',
				(byte) '3',
				(byte) '.',
				(byte) '0',
			};
			DataBlock idBlock = new DataBlock( 11, idData );
			
			ApplicationExtension appExt 
				= new ApplicationExtension( idBlock, new Collection<DataBlock>() );
			try
			{
				_ne = new NetscapeExtension( appExt );
			}
			catch( ArgumentException ex )
			{
				string message
					= "The application authentication code is not '2.0' "
					+ "therefore this application extension is not a "
					+ "Netscape extension. Application authentication code: 3.0";
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "applicationExtension", ex.ParamName );
				throw;
			}
		}
		#endregion
	}
}

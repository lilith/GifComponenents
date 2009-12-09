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
using NUnit.Framework;
using NUnit.Extensions;

namespace GifComponents.NUnit
{
	/// <summary>
	/// Test fixture for the PackedFields class.
	/// </summary>
	[TestFixture]
	public class PackedFieldsTest : TestFixtureBase
	{
		private PackedFields _pf;
		
		#region ConstructorTest
		/// <summary>
		/// Checks that the constructor which accepts an integer value works
		/// correctly.
		/// </summary>
		[Test]
		public void ConstructorTest()
		{
			ReportStart();
			_pf = new PackedFields( 0 ); // 00000000
			Assert.AreEqual( 0, _pf.Byte );
			for( int i = 0; i < 8; i++ )
			{
				Assert.AreEqual( false, _pf.GetBit( i ), "bit " + i );
			}
			
			_pf = new PackedFields( 255 ); // 11111111
			Assert.AreEqual( 255, _pf.Byte );
			for( int i = 0; i < 8; i++ )
			{
				Assert.AreEqual( true, _pf.GetBit( i ), "bit " + i );
			}
			
			_pf = new PackedFields( 36 ); // 00100010
			Assert.AreEqual( 36, _pf.Byte );
			Assert.AreEqual( false, _pf.GetBit( 0 ) );
			Assert.AreEqual( false, _pf.GetBit( 1 ) );
			Assert.AreEqual( true, _pf.GetBit( 2 ) );
			Assert.AreEqual( false, _pf.GetBit( 3 ) );
			Assert.AreEqual( false, _pf.GetBit( 4 ) );
			Assert.AreEqual( true, _pf.GetBit( 5 ) );
			Assert.AreEqual( false, _pf.GetBit( 6 ) );
			Assert.AreEqual( false, _pf.GetBit( 7 ) );
			ReportEnd();
		}
		#endregion

		#region test cases for the GetBit and SetBit methods
		
		#region GetSetBitTest
		/// <summary>
		/// Checks that the GetBit and SetBit methods work correctly.
		/// </summary>
		[Test]
		public void GetSetBitTest()
		{
			ReportStart();
			for( int i = 0; i < 8; i++ )
			{
				_pf = new PackedFields();
				_pf.SetBit( i, true );
				Assert.AreEqual( true, _pf.GetBit( i ) );
			}
			ReportEnd();
		}
		#endregion
		
		#region GetBitTestIndexTooSmall
		/// <summary>
		/// Checks that the correct exception is thrown when the GetBit method
		/// is passed an index which is too small.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void GetBitTestIndexTooSmall()
		{
			ReportStart();
			try
			{
				_pf.GetBit( -1 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "Index must be between 0 and 7. Supplied index: -1";
				Assert.AreEqual( "index", ex.ParamName );
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region GetBitTestIndexTooLarge
		/// <summary>
		/// Checks that the correct exception is thrown when the GetBit method
		/// is passed an index which is too large.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void GetBitTestIndexTooLarge()
		{
			ReportStart();
			try
			{
				_pf.GetBit( 8 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "Index must be between 0 and 7. Supplied index: 8";
				Assert.AreEqual( "index", ex.ParamName );
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region SetBitTestIndexTooSmall
		/// <summary>
		/// Checks that the correct exception is thrown when the SetBit method
		/// is passed an index which is too small.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void SetBitTestIndexTooSmall()
		{
			ReportStart();
			try
			{
				_pf.SetBit( -1, true );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "Index must be between 0 and 7. Supplied index: -1";
				Assert.AreEqual( "index", ex.ParamName );
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region SetBitTestIndexTooLarge
		/// <summary>
		/// Checks that the correct exception is thrown when the SetBit method
		/// is passed an index which is too large.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void SetBitTestIndexTooLarge()
		{
			ReportStart();
			try
			{
				_pf.SetBit( 8, true );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "Index must be between 0 and 7. Supplied index: 8";
				Assert.AreEqual( "index", ex.ParamName );
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion

		#endregion
		
		#region test cases for the GetBits and SetBits methods
		
		#region GetSetBitsTest
		/// <summary>
		/// Checks that the GetBits and SetBits methods work correctly.
		/// </summary>
		[Test]
		public void GetSetBitsTest()
		{
			ReportStart();
			_pf = new PackedFields();
			_pf.SetBits( 2, 3, 4 );
			Assert.AreEqual( false, _pf.GetBit( 0 ) );
			Assert.AreEqual( false, _pf.GetBit( 1 ) );
			Assert.AreEqual( true, _pf.GetBit( 2 ) );
			Assert.AreEqual( false, _pf.GetBit( 3 ) );
			Assert.AreEqual( false, _pf.GetBit( 4 ) );
			Assert.AreEqual( false, _pf.GetBit( 5 ) );
			Assert.AreEqual( false, _pf.GetBit( 6 ) );
			Assert.AreEqual( false, _pf.GetBit( 7 ) );
			Assert.AreEqual( 4, _pf.GetBits( 2, 3 ) );
			Assert.AreEqual( 4, _pf.GetBits( 1, 4 ) );
			Assert.AreEqual( 4, _pf.GetBits( 0, 5 ) );
			Assert.AreEqual( 8, _pf.GetBits( 0, 6 ) );
			Assert.AreEqual( 16, _pf.GetBits( 0, 7 ) );
			Assert.AreEqual( 32, _pf.GetBits( 0, 8 ) );
			Assert.AreEqual( 32, _pf.Byte );
			ReportEnd();
		}
		#endregion
		
		#region GetSetBitsTest2
		/// <summary>
		/// Checks that the GetBits and SetBits methods work correctly.
		/// </summary>
		[Test]
		public void GetSetBitsTest2()
		{
			ReportStart();
			_pf = new PackedFields();
			_pf.SetBits( 1, 5, 13 );
			Assert.AreEqual( false, _pf.GetBit( 0 ) );
			Assert.AreEqual( false, _pf.GetBit( 1 ) );
			Assert.AreEqual( true, _pf.GetBit( 2 ) );
			Assert.AreEqual( true, _pf.GetBit( 3 ) );
			Assert.AreEqual( false, _pf.GetBit( 4 ) );
			Assert.AreEqual( true, _pf.GetBit( 5 ) );
			Assert.AreEqual( false, _pf.GetBit( 6 ) );
			Assert.AreEqual( false, _pf.GetBit( 7 ) );
			Assert.AreEqual( 13, _pf.GetBits( 1, 5 ) );
			Assert.AreEqual( 13, _pf.GetBits( 0, 6 ) );
			Assert.AreEqual( 13, _pf.GetBits( 2, 4 ) );
			Assert.AreEqual( 26, _pf.GetBits( 1, 6 ) );
			Assert.AreEqual( 13, _pf.GetBits( 0, 6 ) );
			Assert.AreEqual( 26, _pf.GetBits( 0, 7 ) );
			Assert.AreEqual( 52, _pf.GetBits( 0, 8 ) );
			Assert.AreEqual( 52, _pf.Byte );
			ReportEnd();
		}
		#endregion
		
		#region GetBitsTestIndexTooSmall
		/// <summary>
		/// Checks that the correct exception is thrown when the GetBits method
		/// is passed a start index which is too small.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void GetBitsTestIndexTooSmall()
		{
			ReportStart();
			try
			{
				_pf.GetBits( -1, 4 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "Start index must be between 0 and 7. Supplied index: -1";
				Assert.AreEqual( "startIndex", ex.ParamName );
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region GetBitsTestIndexTooLarge
		/// <summary>
		/// Checks that the correct exception is thrown when the GetBits method
		/// is passed a start index which is too large.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void GetBitsTestIndexTooLarge()
		{
			ReportStart();
			try
			{
				_pf.GetBits( 8, 4 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "Start index must be between 0 and 7. Supplied index: 8";
				Assert.AreEqual( "startIndex", ex.ParamName );
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region SetBitsTestIndexTooSmall
		/// <summary>
		/// Checks that the correct exception is thrown when the SetBits method
		/// is passed a start index which is too small.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void SetBitsTestIndexTooSmall()
		{
			ReportStart();
			try
			{
				_pf.SetBits( -1, 4, 6 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "Start index must be between 0 and 7. Supplied index: -1";
				Assert.AreEqual( "startIndex", ex.ParamName );
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region SetBitsTestIndexTooLarge
		/// <summary>
		/// Checks that the correct exception is thrown when the SetBits method
		/// is passed a start index which is too large.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void SetBitsTestIndexTooLarge()
		{
			ReportStart();
			try
			{
				_pf.SetBits( 8, 4, 6 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "Start index must be between 0 and 7. Supplied index: 8";
				Assert.AreEqual( "startIndex", ex.ParamName );
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region GetBitsTestLengthTooSmall
		/// <summary>
		/// Checks that the correct exception is thrown when the GetBits method
		/// is passed a length which is too small.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void GetBitsTestLengthTooSmall()
		{
			ReportStart();
			try
			{
				_pf.GetBits( 1, 0 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "Length must be greater than zero and the sum of length "
					+ "and start index must be less than 8. Supplied length: "
					+ "0. Supplied start index: 1";
				Assert.AreEqual( "length", ex.ParamName );
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion

		#region GetBitsTestLengthTooLarge
		/// <summary>
		/// Checks that the correct exception is thrown when the GetBits method
		/// is passed a length which is too large.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void GetBitsTestLengthTooLarge()
		{
			ReportStart();
			try
			{
				_pf.GetBits( 4, 5 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "Length must be greater than zero and the sum of length "
					+ "and start index must be less than 8. Supplied length: "
					+ "5. Supplied start index: 4";
				Assert.AreEqual( "length", ex.ParamName );
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion

		#region SetBitsTestLengthTooSmall
		/// <summary>
		/// Checks that the correct exception is thrown when the SetBits method
		/// is passed a length which is too small.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void SetBitsTestLengthTooSmall()
		{
			ReportStart();
			try
			{
				_pf.SetBits( 1, 0, 5 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "Length must be greater than zero and the sum of length "
					+ "and start index must be less than 8. Supplied length: "
					+ "0. Supplied start index: 1";
				Assert.AreEqual( "length", ex.ParamName );
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion

		#region SetBitsTestLengthTooLarge
		/// <summary>
		/// Checks that the correct exception is thrown when the SetBits method
		/// is passed a length which is too large.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void SetBitsTestLengthTooLarge()
		{
			ReportStart();
			try
			{
				_pf.SetBits( 4, 5, 6 );
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message
					= "Length must be greater than zero and the sum of length "
					+ "and start index must be less than 8. Supplied length: "
					+ "5. Supplied start index: 4";
				Assert.AreEqual( "length", ex.ParamName );
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion

		#endregion
		
	}
}

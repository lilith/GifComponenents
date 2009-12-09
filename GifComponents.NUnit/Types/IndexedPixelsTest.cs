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
using NUnit.Framework;
using NUnit.Extensions;

namespace GifComponents.NUnit
{
	/// <summary>
	/// Test fixture for the IndexedPixels class.
	/// </summary>
	[TestFixture]
	public class IndexedPixelsTest : TestFixtureBase
	{
		private IndexedPixels _ip;
		
		#region DefaultConstructor
		/// <summary>
		/// Test case for the default constructor, the one which accepts no
		/// parameters.
		/// Also tests the Add method and the indexer.
		/// </summary>
		[Test]
		public void DefaultConstructor()
		{
			ReportStart();
			byte[] bytes = new byte[] { 24, 39, 2 };
			
			// Test the constructor
			_ip = new IndexedPixels();
			
			// Test the Add method
			foreach( byte b in bytes )
			{
				_ip.Add( b );
			}
			
			Assert.AreEqual( bytes.Length, _ip.Count );

			// Test the get accessor of the indexer
			for( int i = 0; i < bytes.Length; i++ )
			{
				Assert.AreEqual( bytes[i], _ip[i] );
			}
			
			// Test the set accessor of the indexer
			_ip[1] = 246;
			Assert.AreEqual( 246, _ip[1] );
			ReportEnd();
		}
		#endregion
		
		#region CapacityConstructor
		/// <summary>
		/// Checks that the constructor which accepts an initial capacity works
		/// correctly under normal circumstances.
		/// </summary>
		[Test]
		public void CapacityConstructor()
		{
			ReportStart();
			byte[] bytes = new byte[] { 24, 39, 2 };
			
			// Test the constructor
			_ip = new IndexedPixels( bytes.Length );
			
			// Test the set accessor of the indexer
			for( int i = 0; i < bytes.Length; i++ )
			{
				_ip[i] = bytes[i];
			}
			
			Assert.AreEqual( bytes.Length, _ip.Count );

			// Test the get accessor of the indexer
			for( int i = 0; i < bytes.Length; i++ )
			{
				Assert.AreEqual( bytes[i], _ip[i] );
			}
			ReportEnd();
		}
		#endregion
		
		#region CapacityConstructorAdd
		/// <summary>
		/// Demonstrates that the return value of the constructor which accepts 
		/// an initial capacity cannot be expanded by the Add method.
		/// </summary>
		[Test]
		[ExpectedException( typeof( NotSupportedException ) )]
		public void CapacityConstructorAdd()
		{
			ReportStart();
			_ip = new IndexedPixels( 3 );
			try
			{
				_ip.Add( 1 );
			}
			catch( NotSupportedException ex )
			{
				string message
					= "You cannot add pixels to this instance because it was "
					+ "instantiated with a fixed size.";
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion

		#region IndexOutOfRangeGet
		/// <summary>
		/// Checks that the get accessor of the indexer throws the correct
		/// exception when passed an index which is out of range
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		[SuppressMessage("Microsoft.Performance", 
		                 "CA1804:RemoveUnusedLocals", 
		                 MessageId = "temp")]
		public void IndexOutOfRangeGet()
		{
			ReportStart();
			_ip = new IndexedPixels();
			try
			{
				byte temp = _ip[0];
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message = "Collection size: 0. Supplied index: 0.";
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion

		#region IndexOutOfRangeSet
		/// <summary>
		/// Checks that the set accessor of the indexer throws the correct
		/// exception when passed an index which is out of range
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentOutOfRangeException ) )]
		public void IndexOutOfRangeSet()
		{
			ReportStart();
			_ip = new IndexedPixels();
			try
			{
				_ip[0] = 6;
			}
			catch( ArgumentOutOfRangeException ex )
			{
				string message = "Collection size: 0. Supplied index: 0.";
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion
	}
}

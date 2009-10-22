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

namespace GifComponents.NUnit
{
	/// <summary>
	/// Test fixture for the GifHeader class.
	/// </summary>
	[TestFixture]
	public class GifHeaderTest
	{
		private GifHeader _header;
		
		#region ConstructorTest
		/// <summary>
		/// Checks that the constructor works correctly under normal 
		/// circumstances.
		/// </summary>
		[Test]
		public void ConstructorTest()
		{
			_header = new GifHeader( "GIF", "89A" );
			Assert.AreEqual( "GIF", _header.Signature );
			Assert.AreEqual( "89A", _header.Version );
			Assert.AreEqual( ErrorState.Ok, _header.ConsolidatedState );
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
			_header = new GifHeader( "FIG", "89A" );
			Assert.AreEqual( "FIG", _header.Signature );
			Assert.AreEqual( "89A", _header.Version );
			Assert.AreEqual( ErrorState.BadSignature, _header.ErrorState );
		}
		#endregion

		#region FromStreamTest
		/// <summary>
		/// Checks that the FromStream method works correctly.
		/// </summary>
		[Test]
		public void FromStreamTest()
		{
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
			
			_header = GifHeader.FromStream( s );
			Assert.AreEqual( ErrorState.Ok, _header.ConsolidatedState );
			Assert.AreEqual( "GIF", _header.Signature );
			Assert.AreEqual( "89A", _header.Version );
			Assert.AreEqual( ErrorState.Ok, _header.ConsolidatedState );
		}
		#endregion

		#region WriteToStreamTest
		/// <summary>
		/// Checks that the WriteToStream method works correctly.
		/// </summary>
		[Test]
		public void WriteToStreamTest()
		{
			_header = new GifHeader( "GIF", "87a" );
			MemoryStream s = new MemoryStream();
			_header.WriteToStream( s );
			s.Seek( 0, SeekOrigin.Begin );
			_header = GifHeader.FromStream( s );
			Assert.AreEqual( ErrorState.Ok, _header.ConsolidatedState );
			Assert.AreEqual( "GIF", _header.Signature );
			Assert.AreEqual( "87a", _header.Version );
		}
		#endregion
	}
}

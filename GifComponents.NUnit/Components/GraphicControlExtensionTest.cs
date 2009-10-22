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
	/// Test fixture for the GraphicControlExtension class.
	/// </summary>
	[TestFixture]
	public class GraphicControlExtensionTest
	{
		private GraphicControlExtension _gce;

		#region ConstructorTest
		/// <summary>
		/// Checks that the constructor works correctly under normal 
		/// circumstances.
		/// </summary>
		[Test]
		public void ConstructorTest()
		{
			int blockSize = 4;
			DisposalMethod method = DisposalMethod.DoNotDispose;
			bool expectsUserInput = false;
			bool hasTransparentColour = true;
			int delayTime = 40;
			int transparentColourIndex = 22;
			_gce = new GraphicControlExtension( blockSize, 
			                                    method, 
			                                    expectsUserInput, 
			                                    hasTransparentColour, 
			                                    delayTime, 
			                                    transparentColourIndex );
			
			Assert.AreEqual( blockSize, _gce.BlockSize );
			Assert.AreEqual( method, _gce.DisposalMethod );
			Assert.AreEqual( expectsUserInput, _gce.ExpectsUserInput );
			Assert.AreEqual( hasTransparentColour, _gce.HasTransparentColour );
			Assert.AreEqual( delayTime, _gce.DelayTime );
			Assert.AreEqual( transparentColourIndex, _gce.TransparentColourIndex );
			Assert.AreEqual( ErrorState.Ok, _gce.ErrorState );
		}
		#endregion
		
		#region FromStreamTest
		/// <summary>
		/// Checks that the FromStream method works correctly.
		/// </summary>
		[Test]
		public void FromStreamTest()
		{
			int blockSize = 4;
			DisposalMethod method = DisposalMethod.DoNotDispose;
			bool expectsUserInput = false;
			bool hasTransparentColour = true;
			int delayTime = 40;
			int transparentColourIndex = 22;
			
			MemoryStream s = new MemoryStream();
			s.WriteByte( (byte) blockSize );
			
			// Packed fields:
			//	bits 1-3 = reserved
			//	bits 4-6 = disposal method
			//	bit 7 = user input flag
			//	bit 8 = transparent flag
			byte packedFields = (byte)
				(
					  ( (int) method & 7 ) << 2
					| ( expectsUserInput ? 1 : 0 ) << 1
					| ( hasTransparentColour ? 1 : 0 )
				);
			s.WriteByte( packedFields );
			
			// Write delay time, least significant byte first
			s.WriteByte( (byte) ( delayTime & 0xff ) );
			s.WriteByte( (byte) ( ( delayTime & 0xff00 ) >> 8 ) );
			
			s.WriteByte( (byte) transparentColourIndex );
			
			s.WriteByte( 0 ); // block terminator
			
			s.Seek( 0, SeekOrigin.Begin );
			
			_gce = GraphicControlExtension.FromStream( s );
			
			Assert.AreEqual( blockSize, _gce.BlockSize );
			Assert.AreEqual( method, _gce.DisposalMethod );
			Assert.AreEqual( expectsUserInput, _gce.ExpectsUserInput );
			Assert.AreEqual( hasTransparentColour, _gce.HasTransparentColour );
			Assert.AreEqual( delayTime, _gce.DelayTime );
			Assert.AreEqual( transparentColourIndex, _gce.TransparentColourIndex );
			Assert.AreEqual( ErrorState.Ok, _gce.ConsolidatedState );
		}
		#endregion

		#region FromStreamTestDisposalMethodNotSpecified
		/// <summary>
		/// Checks that the FromStream method works correctly when the disposal
		/// method is not specified (defaults to Do Not Dispose).
		/// </summary>
		[Test]
		public void FromStreamTestDisposalMethodNotSpecified()
		{
			int blockSize = 4;
			DisposalMethod method = DisposalMethod.NotSpecified;
			bool expectsUserInput = false;
			bool hasTransparentColour = true;
			int delayTime = 40;
			int transparentColourIndex = 22;
			
			MemoryStream s = new MemoryStream();
			s.WriteByte( (byte) blockSize );
			
			// Packed fields:
			//	bits 1-3 = reserved
			//	bits 4-6 = disposal method
			//	bit 7 = user input flag
			//	bit 8 = transparent flag
			byte packedFields = (byte)
				(
					  ( (int) method & 7 ) << 2
					| ( expectsUserInput ? 1 : 0 ) << 1
					| ( hasTransparentColour ? 1 : 0 )
				);
			s.WriteByte( packedFields );
			
			// Write delay time, least significant byte first
			s.WriteByte( (byte) ( delayTime & 0xff ) );
			s.WriteByte( (byte) ( ( delayTime & 0xff00 ) >> 8 ) );
			
			s.WriteByte( (byte) transparentColourIndex );
			
			s.WriteByte( 0 ); // block terminator
			
			s.Seek( 0, SeekOrigin.Begin );
			
			_gce = GraphicControlExtension.FromStream( s );
			
			Assert.AreEqual( blockSize, _gce.BlockSize );
			Assert.AreEqual( DisposalMethod.DoNotDispose, _gce.DisposalMethod );
			Assert.AreEqual( expectsUserInput, _gce.ExpectsUserInput );
			Assert.AreEqual( hasTransparentColour, _gce.HasTransparentColour );
			Assert.AreEqual( delayTime, _gce.DelayTime );
			Assert.AreEqual( transparentColourIndex, _gce.TransparentColourIndex );
			Assert.AreEqual( ErrorState.Ok, _gce.ConsolidatedState );
		}
		#endregion

		#region WriteToStreamTest
		/// <summary>
		/// Checks that the WriteToStream method works correctly.
		/// </summary>
		[Test]
		public void WriteToStreamTest()
		{
			int blockSize = 4;
			DisposalMethod disposalMethod = DisposalMethod.DoNotDispose;
			bool expectsUserInput = false;
			bool hasTransparentColour = true;
			int delayTime = 10;
			int transparentColourIndex = 6;
			_gce = new GraphicControlExtension( blockSize, 
			                                    disposalMethod, 
			                                    expectsUserInput, 
			                                    hasTransparentColour, 
			                                    delayTime, 
			                                    transparentColourIndex );
			
			MemoryStream s = new MemoryStream();
			_gce.WriteToStream( s );
			s.Seek( 0, SeekOrigin.Begin );
			
			_gce = GraphicControlExtension.FromStream( s );
			
			Assert.AreEqual( ErrorState.Ok, _gce.ConsolidatedState );
			Assert.AreEqual( blockSize, _gce.BlockSize );
			Assert.AreEqual( disposalMethod, _gce.DisposalMethod );
			Assert.AreEqual( expectsUserInput, _gce.ExpectsUserInput );
			Assert.AreEqual( hasTransparentColour, _gce.HasTransparentColour );
			Assert.AreEqual( delayTime, _gce.DelayTime );
			Assert.AreEqual( transparentColourIndex, _gce.TransparentColourIndex );
		}
		#endregion
	}
}

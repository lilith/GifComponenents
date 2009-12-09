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
using System.IO;
using NUnit.Framework;
using GifComponents.Components;

namespace GifComponents.NUnit.Components
{
	/// <summary>
	/// Test fixture for the ImageDescriptor class.
	/// </summary>
	[TestFixture]
	public class ImageDescriptorTest : GifComponentTestFixtureBase, IDisposable
	{
		private ImageDescriptor _id;
		
		#region ConstructorTest
		/// <summary>
		/// Checks that the constructor works correctly under normal 
		/// circumstances.
		/// </summary>
		[Test]
		public void ConstructorTest()
		{
			ReportStart();
			Point position = new Point( 1, 2 );
			Size size = new Size( 20, 30 );
			bool hasLocalColourTable = false;
			bool isInterlaced = false;
			bool isSorted = false;
			int localColourTableSizeBits = 7;
			
			_id = new ImageDescriptor( position, 
			                           size, 
			                           hasLocalColourTable, 
			                           isInterlaced, 
			                           isSorted, 
			                           localColourTableSizeBits );
			
			Assert.AreEqual( position, _id.Position );
			Assert.AreEqual( size, _id.Size );
			Assert.AreEqual( hasLocalColourTable, _id.HasLocalColourTable );
			Assert.AreEqual( isInterlaced, _id.IsInterlaced );
			Assert.AreEqual( isSorted, _id.IsSorted );
			Assert.AreEqual( localColourTableSizeBits, _id.LocalColourTableSizeBits );
			Assert.AreEqual( (int) Math.Pow( 2, localColourTableSizeBits + 1 ), 
			                 _id.LocalColourTableSize );
			ReportEnd();
		}
		#endregion
		
		#region ConstructorStreamTest
		/// <summary>
		/// Checks that the constructor( stream ) works correctly.
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
			Point position = new Point( 1, 2 );
			Size size = new Size( 20, 30 );
			bool hasLocalColourTable = false;
			bool isInterlaced = false;
			bool isSorted = false;
			int localColourTableSizeBits = 7;
			
			MemoryStream s = new MemoryStream();
			
			WriteImageDescriptor( s, 
			                      position, 
			                      size, 
			                      hasLocalColourTable, 
			                      isInterlaced, 
			                      isSorted, 
			                      localColourTableSizeBits );
			
			s.Seek( 0, SeekOrigin.Begin );
			
			_id = new ImageDescriptor( s, xmlDebugging );

			Assert.AreEqual( position, _id.Position );
			Assert.AreEqual( size, _id.Size );
			Assert.AreEqual( hasLocalColourTable, _id.HasLocalColourTable );
			Assert.AreEqual( isInterlaced, _id.IsInterlaced );
			Assert.AreEqual( isSorted, _id.IsSorted );
			Assert.AreEqual( localColourTableSizeBits, _id.LocalColourTableSizeBits );
			
			// Actual colour table size is 2 to the power of the value in the
			// stream plus 1. Value in the stream is the number of bits needed
			// to hold the actual size.
			Assert.AreEqual( Math.Pow( 2, localColourTableSizeBits + 1), 
			                 _id.LocalColourTableSize );
			
			if( xmlDebugging )
			{
				Assert.AreEqual( ExpectedDebugXml, _id.DebugXml );
			}
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
			Point position = new Point( 1, 4 );
			Size size = new Size( 12, 15 );
			bool hasLocalColourTable = true;
			bool isInterlaced = false;
			bool localColourTableIsSorted = false;
			int localColourTableSizeBits = 3;
			
			_id = new ImageDescriptor( position, 
			                           size,
			                           hasLocalColourTable,
			                           isInterlaced,
			                           localColourTableIsSorted,
			                           localColourTableSizeBits );
			MemoryStream s = new MemoryStream();
			_id.WriteToStream( s );
			s.Seek( 0, SeekOrigin.Begin );
			
			_id = new ImageDescriptor( s );
			
			Assert.AreEqual( ErrorState.Ok, _id.ConsolidatedState );
			Assert.AreEqual( position, _id.Position );
			Assert.AreEqual( size, _id.Size );
			Assert.AreEqual( hasLocalColourTable, _id.HasLocalColourTable );
			Assert.AreEqual( isInterlaced, _id.IsInterlaced );
			Assert.AreEqual( localColourTableIsSorted, _id.IsSorted );
			Assert.AreEqual( localColourTableSizeBits, _id.LocalColourTableSizeBits );
			Assert.AreEqual( (int) Math.Pow( 2, localColourTableSizeBits + 1 ),
			                 _id.LocalColourTableSize );
			ReportEnd();
		}
		#endregion
		
		#region internal WriteImageDescriptor method
		/// <summary>
		/// Writes an image descriptor to the supplied stream
		/// </summary>
		/// <param name="outputStream"></param>
		/// <param name="position"></param>
		/// <param name="size"></param>
		/// <param name="hasLocalColourTable"></param>
		/// <param name="isInterlaced"></param>
		/// <param name="isSorted"></param>
		/// <param name="localColourTableSize"></param>
		internal static void WriteImageDescriptor( Stream outputStream, 
		                                           Point position, 
		                                           Size size, 
		                                           bool hasLocalColourTable, 
		                                           bool isInterlaced, 
		                                           bool isSorted, 
		                                           int localColourTableSize )
		{
			// Write image x and y positions, least significant byte first
			outputStream.WriteByte( (byte) ( position.X & 0xff ) );
			outputStream.WriteByte( (byte) ( ( position.X & 0xff00 ) >> 8 ) );
			outputStream.WriteByte( (byte) ( position.Y & 0xff ) );
			outputStream.WriteByte( (byte) ( ( position.Y & 0xff00 ) >> 8 ) );

			// Write width and height, least significant byte first
			outputStream.WriteByte( (byte) ( size.Width & 0xff ) );
			outputStream.WriteByte( (byte) ( ( size.Width & 0xff00 ) >> 8 ) );
			outputStream.WriteByte( (byte) ( size.Height & 0xff ) );
			outputStream.WriteByte( (byte) ( ( size.Height & 0xff00 ) >> 8 ) );
			
			// Packed fields:
			//	bit 1 = local colour table flag
			//	bit 2 = interlace flag
			//	bit 3 = sort flag
			//	bits 4-5 = reserved
			//	bits 6-8 = local colour table size
			byte packed = (byte)
				(
					( hasLocalColourTable ? 1 : 0 ) << 7
					| ( isInterlaced ? 1 : 0 ) << 6
					| ( isSorted ? 1 : 0 ) << 5
					| ( localColourTableSize & 7 )
				);
			outputStream.WriteByte( packed );
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
		~ImageDescriptorTest()
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
					_id.Dispose();
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

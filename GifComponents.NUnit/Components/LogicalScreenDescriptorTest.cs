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
	/// Test fixture for the LogicalScreenDescriptor class.
	/// </summary>
	[TestFixture]
	public class LogicalScreenDescriptorTest : GifComponentTestFixtureBase, IDisposable
	{
		private LogicalScreenDescriptor _lsd;
		
		#region ConstructorTest
		/// <summary>
		/// Checks that the constructor works correctly under normal 
		/// circumstances.
		/// </summary>
		[Test]
		public void ConstructorTest()
		{
			ReportStart();
			
			int width = 15;
			int height = 20;
			Size screenSize = new Size( width, height );
			bool hasGlobalColourTable = true;
			int colourResolution = 6;
			bool colourTableIsSorted = false;
			int globalColourTableSizeBits = 3;
			int backgroundColourIndex = 12;
			int pixelAspectRatio = 2;
			
			_lsd = new LogicalScreenDescriptor( screenSize, 
			                                    hasGlobalColourTable, 
			                                    colourResolution, 
			                                    colourTableIsSorted, 
			                                    globalColourTableSizeBits, 
			                                    backgroundColourIndex, 
			                                    pixelAspectRatio );
			
			Assert.AreEqual( screenSize, _lsd.LogicalScreenSize );
			Assert.AreEqual( hasGlobalColourTable, _lsd.HasGlobalColourTable );
			Assert.AreEqual( colourResolution, _lsd.ColourResolution );
			Assert.AreEqual( colourTableIsSorted, _lsd.GlobalColourTableIsSorted );
			Assert.AreEqual( globalColourTableSizeBits, 
			                 _lsd.GlobalColourTableSizeBits );
			Assert.AreEqual( Math.Pow( 2, globalColourTableSizeBits + 1 ), 
			                 _lsd.GlobalColourTableSize );
			Assert.AreEqual( backgroundColourIndex, _lsd.BackgroundColourIndex );
			Assert.AreEqual( pixelAspectRatio, _lsd.PixelAspectRatio );
			Assert.AreEqual( ErrorState.Ok, _lsd.ConsolidatedState );
			
			ReportEnd();
		}
		#endregion
		
		#region ConstructorTestTooWide
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor
		/// is passed a logical image width which is too large.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentException ) )]
		public void ConstructorTestTooWide()
		{
			ReportStart();
			
			int width = ushort.MaxValue + 1;
			int height = 20;
			Size screenSize = new Size( width, height );
			bool hasGlobalColourTable = true;
			int colourResolution = 6;
			bool colourTableIsSorted = false;
			int globalColourTableSizeBits = 3;
			int backgroundColourIndex = 12;
			int pixelAspectRatio = 2;
			
			try
			{
				_lsd = new LogicalScreenDescriptor( screenSize, 
				                                    hasGlobalColourTable, 
				                                    colourResolution, 
				                                    colourTableIsSorted, 
				                                    globalColourTableSizeBits, 
				                                    backgroundColourIndex, 
				                                    pixelAspectRatio );
			}
			catch( ArgumentException ex )
			{
				string message
					= "Logical screen width cannot be more than "
					+ ushort.MaxValue + ". "
					+ "Supplied value: " + width;
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "logicalScreenSize", ex.ParamName );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region ConstructorTestTooTall
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor
		/// is passed a logical image height which is too large.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentException ) )]
		public void ConstructorTestTooTall()
		{
			ReportStart();
			
			int width = 15;
			int height = ushort.MaxValue + 1;
			Size screenSize = new Size( width, height );
			bool hasGlobalColourTable = true;
			int colourResolution = 6;
			bool colourTableIsSorted = false;
			int globalColourTableSizeBits = 3;
			int backgroundColourIndex = 12;
			int pixelAspectRatio = 2;
			
			try
			{
				_lsd = new LogicalScreenDescriptor( screenSize, 
				                                    hasGlobalColourTable, 
				                                    colourResolution, 
				                                    colourTableIsSorted, 
				                                    globalColourTableSizeBits, 
				                                    backgroundColourIndex, 
				                                    pixelAspectRatio );
			}
			catch( ArgumentException ex )
			{
				string message
					= "Logical screen height cannot be more than "
					+ ushort.MaxValue + ". "
					+ "Supplied value: " + height;
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "logicalScreenSize", ex.ParamName );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region ConstructorTestColourResolutionTooLarge
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor
		/// is passed a colour resolution which is too large.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentException ) )]
		public void ConstructorTestColourResolutionTooLarge()
		{
			ReportStart();
			
			int width = 15;
			int height = 20;
			Size screenSize = new Size( width, height );
			bool hasGlobalColourTable = true;
			int colourResolution = 8;
			bool colourTableIsSorted = false;
			int globalColourTableSizeBits = 3;
			int backgroundColourIndex = 12;
			int pixelAspectRatio = 2;
			
			try
			{
				_lsd = new LogicalScreenDescriptor( screenSize, 
				                                    hasGlobalColourTable, 
				                                    colourResolution, 
				                                    colourTableIsSorted, 
				                                    globalColourTableSizeBits, 
				                                    backgroundColourIndex, 
				                                    pixelAspectRatio );
			}
			catch( ArgumentException ex )
			{
				string message
					= "Colour resolution cannot be more than 7. "
					+ "Supplied value: " + colourResolution;
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "colourResolution", ex.ParamName );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region ConstructorTestGlobalColourTableTooLarge
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor
		/// is passed a global colour table size which is too large.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentException ) )]
		public void ConstructorTestGlobalColourTableTooLarge()
		{
			ReportStart();
			
			int width = 15;
			int height = 20;
			Size screenSize = new Size( width, height );
			bool hasGlobalColourTable = true;
			int colourResolution = 6;
			bool colourTableIsSorted = false;
			int globalColourTableSizeBits = 8;
			int backgroundColourIndex = 12;
			int pixelAspectRatio = 2;
			
			try
			{
				_lsd = new LogicalScreenDescriptor( screenSize, 
				                                    hasGlobalColourTable, 
				                                    colourResolution, 
				                                    colourTableIsSorted, 
				                                    globalColourTableSizeBits, 
				                                    backgroundColourIndex, 
				                                    pixelAspectRatio );
			}
			catch( ArgumentException ex )
			{
				string message
					= "Global colour table size cannot be more than 7. "
					+ "Supplied value: " + globalColourTableSizeBits;
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "globalColourTableSizeBits", ex.ParamName );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region ConstructorTestBackgroundColourIndexTooLarge
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor
		/// is passed a background colour index which is too large.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentException ) )]
		public void ConstructorTestBackgroundColourIndexTooLarge()
		{
			ReportStart();
			int width = 15;
			int height = 20;
			Size screenSize = new Size( width, height );
			bool hasGlobalColourTable = true;
			int colourResolution = 6;
			bool colourTableIsSorted = false;
			int globalColourTableSizeBits = 3;
			int backgroundColourIndex = byte.MaxValue + 1;
			int pixelAspectRatio = 2;
			
			try
			{
				_lsd = new LogicalScreenDescriptor( screenSize, 
				                                    hasGlobalColourTable, 
				                                    colourResolution, 
				                                    colourTableIsSorted, 
				                                    globalColourTableSizeBits, 
				                                    backgroundColourIndex, 
				                                    pixelAspectRatio );
			}
			catch( ArgumentException ex )
			{
				string message
					= "Background colour index cannot be more than "
					+ byte.MaxValue + ". "
					+ "Supplied value: " + backgroundColourIndex;
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "backgroundColourIndex", ex.ParamName );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region ConstructorTestPixelAspectRatioTooLarge
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor
		/// is passed a pixel aspect ratio which is too large.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentException ) )]
		public void ConstructorTestPixelAspectRatioTooLarge()
		{
			ReportStart();
			
			int width = 15;
			int height = 20;
			Size screenSize = new Size( width, height );
			bool hasGlobalColourTable = true;
			int colourResolution = 6;
			bool colourTableIsSorted = false;
			int globalColourTableSizeBits = 3;
			int backgroundColourIndex = 12;
			int pixelAspectRatio = byte.MaxValue + 1;
			
			try
			{
				_lsd = new LogicalScreenDescriptor( screenSize, 
				                                    hasGlobalColourTable, 
				                                    colourResolution, 
				                                    colourTableIsSorted, 
				                                    globalColourTableSizeBits, 
				                                    backgroundColourIndex, 
				                                    pixelAspectRatio );
			}
			catch( ArgumentException ex )
			{
				string message
					= "Pixel aspect ratio cannot be more than "
					+ byte.MaxValue + ". "
					+ "Supplied value: " + pixelAspectRatio;
				StringAssert.Contains( message, ex.Message );
				Assert.AreEqual( "pixelAspectRatio", ex.ParamName );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
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
			int width = 15;
			int height = 20;
			Size screenSize = new Size( width, height );
			bool hasGlobalColourTable = true;
			int colourResolution = 6;
			bool colourTableIsSorted = false;
			int globalColourTableSizeBits = 3;
			int backgroundColourIndex = 12;
			int pixelAspectRatio = 2;
			
			MemoryStream s = new MemoryStream();
			
			// Write screen width and height, least significant bit first
			s.WriteByte( (byte) ( width & 0xff ) );
			s.WriteByte( (byte) ( ( width & 0xff00 ) >> 8 ) );
			s.WriteByte( (byte) ( height & 0xff ) );
			s.WriteByte( (byte) ( ( height & 0xff00 ) >> 8 ) );
			
			// Packed fields:
			//	bit 1 = global colour table flag
			//	bits 2-4 = colour resolution
			//	bit 5 = sort flag
			//	bits 6-8 = global colour table size
			byte packed = (byte)
				(
					  ( hasGlobalColourTable ? 1 : 0 ) << 7
					| ( colourResolution & 7 ) << 4
					| ( colourTableIsSorted ? 1 : 0 ) << 3
					| ( globalColourTableSizeBits & 7 )
				);
			s.WriteByte( packed );
			
			s.WriteByte( (byte) backgroundColourIndex );
			s.WriteByte( (byte) pixelAspectRatio );
			
			s.Seek( 0, SeekOrigin.Begin );
			_lsd = new LogicalScreenDescriptor( s, xmlDebugging );
			
			Assert.AreEqual( ErrorState.Ok, _lsd.ConsolidatedState );
			Assert.AreEqual( screenSize, _lsd.LogicalScreenSize );
			Assert.AreEqual( hasGlobalColourTable, _lsd.HasGlobalColourTable );
			Assert.AreEqual( colourResolution, _lsd.ColourResolution );
			Assert.AreEqual( colourTableIsSorted, _lsd.GlobalColourTableIsSorted );
			Assert.AreEqual( globalColourTableSizeBits, 
			                 _lsd.GlobalColourTableSizeBits );
			Assert.AreEqual( Math.Pow( 2, globalColourTableSizeBits + 1 ), 
			                 _lsd.GlobalColourTableSize );
			Assert.AreEqual( backgroundColourIndex, _lsd.BackgroundColourIndex );
			Assert.AreEqual( pixelAspectRatio, _lsd.PixelAspectRatio );
			Assert.AreEqual( ErrorState.Ok, _lsd.ConsolidatedState );
			
			if( xmlDebugging )
			{
				Assert.AreEqual( ExpectedDebugXml, _lsd.DebugXml );
			}
		}
		#endregion

		#region ConstructorEndOfStreamTest
		/// <summary>
		/// Checks that the correct error status is set if the end of the input
		/// stream is encountered before the LSD is complete
		/// </summary>
		[Test]
		public void ConstructorEndOfStreamTest()
		{
			ReportStart();
			ConstructorStreamEndOfStreamTest( true );
			ConstructorStreamEndOfStreamTest( false );
			ReportEnd();
		}
		
		private void ConstructorStreamEndOfStreamTest( bool xmlDebugging )
		{
			Size screenSize = new Size( 12, 4 );
			bool hasGlobalColourTable = false;
			int colourResolution = 3;
			bool globalColourTableIsSorted = true;
			int globalColourTableSizeBits = 4;
			int backgroundColourIndex = 2;
			int pixelAspectRatio = 1;
			_lsd = new LogicalScreenDescriptor( screenSize, 
			                                    hasGlobalColourTable, 
			                                    colourResolution, 
			                                    globalColourTableIsSorted, 
			                                    globalColourTableSizeBits, 
			                                    backgroundColourIndex, 
			                                    pixelAspectRatio );
			
			MemoryStream s = new MemoryStream();
			_lsd.WriteToStream( s );
			s.SetLength( s.Length - 1 ); // remove final byte from stream
			s.Seek( 0, SeekOrigin.Begin );
			_lsd = new LogicalScreenDescriptor( s, xmlDebugging );
			
			Assert.AreEqual( ErrorState.EndOfInputStream, _lsd.ConsolidatedState );
			Assert.AreEqual( screenSize, _lsd.LogicalScreenSize );
			Assert.AreEqual( hasGlobalColourTable, _lsd.HasGlobalColourTable );
			Assert.AreEqual( colourResolution, _lsd.ColourResolution );
			Assert.AreEqual( globalColourTableIsSorted, _lsd.GlobalColourTableIsSorted );
			Assert.AreEqual( globalColourTableSizeBits, _lsd.GlobalColourTableSizeBits );
			Assert.AreEqual( backgroundColourIndex, _lsd.BackgroundColourIndex );
			Assert.AreEqual( -1, _lsd.PixelAspectRatio );
			
			if( xmlDebugging )
			{
				Assert.AreEqual( ExpectedDebugXml, _lsd.DebugXml );
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
			
			Size screenSize = new Size( 12, 4 );
			bool hasGlobalColourTable = false;
			int colourResolution = 3;
			bool globalColourTableIsSorted = true;
			int globalColourTableSizeBits = 4;
			int backgroundColourIndex = 2;
			int pixelAspectRatio = 1;
			_lsd = new LogicalScreenDescriptor( screenSize, 
			                                    hasGlobalColourTable, 
			                                    colourResolution, 
			                                    globalColourTableIsSorted, 
			                                    globalColourTableSizeBits, 
			                                    backgroundColourIndex, 
			                                    pixelAspectRatio );
			
			MemoryStream s = new MemoryStream();
			_lsd.WriteToStream( s );
			s.Seek( 0, SeekOrigin.Begin );
			_lsd = new LogicalScreenDescriptor( s );
			
			Assert.AreEqual( ErrorState.Ok, _lsd.ConsolidatedState );
			Assert.AreEqual( screenSize, _lsd.LogicalScreenSize );
			Assert.AreEqual( hasGlobalColourTable, _lsd.HasGlobalColourTable );
			Assert.AreEqual( colourResolution, _lsd.ColourResolution );
			Assert.AreEqual( globalColourTableIsSorted, _lsd.GlobalColourTableIsSorted );
			Assert.AreEqual( globalColourTableSizeBits, _lsd.GlobalColourTableSizeBits );
			Assert.AreEqual( backgroundColourIndex, _lsd.BackgroundColourIndex );
			Assert.AreEqual( pixelAspectRatio, _lsd.PixelAspectRatio );
			
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
		~LogicalScreenDescriptorTest()
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
					_lsd.Dispose();
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

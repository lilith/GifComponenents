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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Globalization;
using System.Text;
using System.Threading;
using NUnit.Framework;
using NUnit.Extensions;
using GifComponents.Components;
using GifComponents.NUnit.Components;

namespace GifComponents.NUnit
{
	/// <summary>
	/// Test fixture for the GifDecoder class.
	/// </summary>
	[TestFixture]
	public class GifDecoderTest : GifComponentTestFixtureBase, IDisposable
	{
		private GifDecoder _decoder;
		private LogicalScreenDescriptor _lsd;
		
		#region DecodeSmiley
		/// <summary>
		/// Tests the GifDecoder class with a smiley animated image.
		/// </summary>
		[Test]
		public void DecodeSmiley()
		{
			ReportStart();
			DecodeSmiley( true );
			DecodeSmiley( false );
			ReportEnd();
		}
		
		private void DecodeSmiley( bool xmlDebugging )
		{
			string fileName = @"images\smiley\smiley.gif";
			_decoder = new GifDecoder( fileName, xmlDebugging );
			_decoder.Decode();
			_lsd = _decoder.LogicalScreenDescriptor;
			int expectedColourTableSize = 128;
			
			Assert.AreEqual( "GIF", _decoder.Header.Signature );
			Assert.AreEqual( "89a", _decoder.Header.Version );
			Assert.IsNotNull( _decoder.GlobalColourTable );
			Assert.AreEqual( true, _lsd.HasGlobalColourTable );
			Assert.AreEqual( expectedColourTableSize, 
			                 _lsd.GlobalColourTableSize );
			Assert.AreEqual( expectedColourTableSize, 
			                	_decoder.GlobalColourTable.Length );
			Assert.AreEqual( 7, _lsd.ColourResolution );
			Assert.AreEqual( Color.FromArgb( 255, 0, 0, 255 ), _decoder.BackgroundColour );
			Assert.AreEqual( 0, _lsd.BackgroundColourIndex );
			Assert.AreEqual( false, _lsd.GlobalColourTableIsSorted );
			Assert.AreEqual( 18, _lsd.LogicalScreenSize.Width );
			Assert.AreEqual( 18, _lsd.LogicalScreenSize.Height );
			Assert.AreEqual( 0, _lsd.PixelAspectRatio );
			Assert.AreEqual( 0, _decoder.NetscapeExtension.LoopCount );
			Assert.AreEqual( ErrorState.Ok, _decoder.ConsolidatedState );
			Assert.AreEqual( 4, _decoder.Frames.Count );
			int frameNumber = 0;
			string frameId;
			foreach( GifFrame thisFrame in _decoder.Frames )
			{
				frameId = "Frame " + frameNumber;
				Assert.IsNull( thisFrame.LocalColourTable, frameId );

				#region image descriptor properties
				ImageDescriptor descriptor = thisFrame.ImageDescriptor;
				Assert.AreEqual( false, descriptor.HasLocalColourTable, frameId );
				Assert.AreEqual( expectedColourTableSize, 
				                 descriptor.LocalColourTableSize, 
				                 frameId );
				Assert.AreEqual( false, descriptor.IsInterlaced, frameId );
				Assert.AreEqual( false, descriptor.IsSorted, frameId );
				#endregion
				
				#region graphic control extension properties
				GraphicControlExtension gce = thisFrame.GraphicControlExtension;
				Assert.AreEqual( 4, gce.BlockSize, frameId );
				Assert.AreEqual( 0, gce.TransparentColourIndex, frameId );
				Assert.IsTrue( gce.HasTransparentColour, frameId );
				Assert.IsFalse( gce.ExpectsUserInput, frameId );
				#endregion

				switch( frameNumber )
				{
					case 0:
						Assert.AreEqual( 250, thisFrame.Delay, frameId );
						Assert.AreEqual( 250, gce.DelayTime, frameId );
						Assert.AreEqual( DisposalMethod.DoNotDispose, 
						                 gce.DisposalMethod, frameId );
						break;
					case 1:
						Assert.AreEqual( 5, thisFrame.Delay, frameId );
						Assert.AreEqual( 5, gce.DelayTime, frameId );
						Assert.AreEqual( DisposalMethod.RestoreToBackgroundColour, 
						                 gce.DisposalMethod, frameId );
						break;
					case 2:
						Assert.AreEqual( 10, thisFrame.Delay, frameId );
						Assert.AreEqual( 10, gce.DelayTime, frameId );
						Assert.AreEqual( DisposalMethod.RestoreToBackgroundColour, 
						                 gce.DisposalMethod, frameId );
						break;
					case 3:
						Assert.AreEqual( 5, thisFrame.Delay, frameId );
						Assert.AreEqual( 5, gce.DelayTime, frameId );
						Assert.AreEqual( DisposalMethod.DoNotDispose, 
						                 gce.DisposalMethod, frameId );
						break;
				}
				frameNumber++;
			}
			CompareFrames( fileName );
			
			if( xmlDebugging )
			{
				Assert.AreEqual( ExpectedDebugXml, _decoder.DebugXml );
			}
		}
		#endregion
		
		#region DecodeSmileyStream
		/// <summary>
		/// Tests the GifDecoder class with a smiley animated image, using the
		/// constructor which accepts a stream.
		/// </summary>
		[Test]
		public void DecodeSmileyStream()
		{
			ReportStart();
			
			string fileName = @"images\smiley\smiley.gif";
			Stream inputStream = new FileInfo( fileName ).OpenRead();
			_decoder = new GifDecoder( inputStream );
			_decoder.Decode();
			_lsd = _decoder.LogicalScreenDescriptor;
			int expectedColourTableSize = 128;
			
			Assert.AreEqual( "GIF", _decoder.Header.Signature );
			Assert.AreEqual( "89a", _decoder.Header.Version );
			Assert.IsNotNull( _decoder.GlobalColourTable );
			Assert.AreEqual( true, _lsd.HasGlobalColourTable );
			Assert.AreEqual( expectedColourTableSize, 
			                 _lsd.GlobalColourTableSize );
			Assert.AreEqual( expectedColourTableSize, 
			                	_decoder.GlobalColourTable.Length );
			Assert.AreEqual( 7, _lsd.ColourResolution );
			Assert.AreEqual( Color.FromArgb( 255, 0, 0, 255 ), _decoder.BackgroundColour );
			Assert.AreEqual( 0, _lsd.BackgroundColourIndex );
			Assert.AreEqual( false, _lsd.GlobalColourTableIsSorted );
			Assert.AreEqual( 18, _lsd.LogicalScreenSize.Width );
			Assert.AreEqual( 18, _lsd.LogicalScreenSize.Height );
			Assert.AreEqual( 0, _lsd.PixelAspectRatio );
			Assert.AreEqual( 0, _decoder.NetscapeExtension.LoopCount );
			Assert.AreEqual( ErrorState.Ok, _decoder.ConsolidatedState );
			Assert.AreEqual( 4, _decoder.Frames.Count );
			int frameNumber = 0;
			string frameId;
			foreach( GifFrame thisFrame in _decoder.Frames )
			{
				frameId = "Frame " + frameNumber;
				Assert.IsNull( thisFrame.LocalColourTable, frameId );

				#region image descriptor properties
				ImageDescriptor descriptor = thisFrame.ImageDescriptor;
				Assert.AreEqual( false, descriptor.HasLocalColourTable, frameId );
				Assert.AreEqual( expectedColourTableSize, 
				                 descriptor.LocalColourTableSize, 
				                 frameId );
				Assert.AreEqual( false, descriptor.IsInterlaced, frameId );
				Assert.AreEqual( false, descriptor.IsSorted, frameId );
				#endregion
				
				#region graphic control extension properties
				GraphicControlExtension gce = thisFrame.GraphicControlExtension;
				Assert.AreEqual( 4, gce.BlockSize, frameId );
				Assert.AreEqual( 0, gce.TransparentColourIndex, frameId );
				Assert.IsTrue( gce.HasTransparentColour, frameId );
				Assert.IsFalse( gce.ExpectsUserInput, frameId );
				#endregion

				switch( frameNumber )
				{
					case 0:
						Assert.AreEqual( 250, thisFrame.Delay, frameId );
						Assert.AreEqual( 250, gce.DelayTime, frameId );
						Assert.AreEqual( DisposalMethod.DoNotDispose, 
						                 gce.DisposalMethod, frameId );
						break;
					case 1:
						Assert.AreEqual( 5, thisFrame.Delay, frameId );
						Assert.AreEqual( 5, gce.DelayTime, frameId );
						Assert.AreEqual( DisposalMethod.RestoreToBackgroundColour, 
						                 gce.DisposalMethod, frameId );
						break;
					case 2:
						Assert.AreEqual( 10, thisFrame.Delay, frameId );
						Assert.AreEqual( 10, gce.DelayTime, frameId );
						Assert.AreEqual( DisposalMethod.RestoreToBackgroundColour, 
						                 gce.DisposalMethod, frameId );
						break;
					case 3:
						Assert.AreEqual( 5, thisFrame.Delay, frameId );
						Assert.AreEqual( 5, gce.DelayTime, frameId );
						Assert.AreEqual( DisposalMethod.DoNotDispose, 
						                 gce.DisposalMethod, frameId );
						break;
				}
				frameNumber++;
			}
			CompareFrames( fileName );
			
			ReportEnd();
		}
		#endregion
		
		#region DecodeRotatingGlobe
		/// <summary>
		/// Tests the GifDecoder class with a rotating globe image.
		/// </summary>
		[Test]
		public void DecodeRotatingGlobe()
		{
			ReportStart();
			DecodeRotatingGlobe( true );
			DecodeRotatingGlobe( false );
			ReportEnd();
		}
		
		private void DecodeRotatingGlobe( bool xmlDebugging )
		{
			string fileName 
				= @"images\globe\spinning globe better 200px transparent background.gif";
			_decoder = new GifDecoder( fileName, xmlDebugging );
			WriteMessage( "Started decoding. XmlDebugging=" + xmlDebugging );
			_decoder.Decode();
			WriteMessage( "Finished decoding. XmlDebugging=" + xmlDebugging );
			
			if( xmlDebugging )
			{
				Assert.AreEqual( ExpectedDebugXml, _decoder.DebugXml );
			}
			
			_lsd = _decoder.LogicalScreenDescriptor;
			int expectedColourTableSize = 64;
			
			Assert.AreEqual( ErrorState.Ok, _decoder.ConsolidatedState );
			Assert.AreEqual( "GIF", _decoder.Header.Signature );
			Assert.AreEqual( "89a", _decoder.Header.Version );
			Assert.IsNotNull( _decoder.GlobalColourTable );
			Assert.AreEqual( true, _lsd.HasGlobalColourTable );
			Assert.AreEqual( expectedColourTableSize, 
			                 _lsd.GlobalColourTableSize );
			Assert.AreEqual( expectedColourTableSize, 
			                	_decoder.GlobalColourTable.Length );
			Assert.AreEqual( 2, _lsd.ColourResolution );
			Assert.AreEqual( Color.FromArgb( 255, 255, 255, 255 ), 
			                 _decoder.BackgroundColour );
			Assert.AreEqual( 63, _lsd.BackgroundColourIndex );
			Assert.AreEqual( false, _lsd.GlobalColourTableIsSorted );
			Assert.AreEqual( 200, _lsd.LogicalScreenSize.Width );
			Assert.AreEqual( 191, _lsd.LogicalScreenSize.Height );
			Assert.AreEqual( 0, _lsd.PixelAspectRatio );
			Assert.AreEqual( 0, _decoder.NetscapeExtension.LoopCount );
			Assert.AreEqual( ErrorState.Ok, _decoder.ErrorState );
			Assert.AreEqual( 20, _decoder.Frames.Count );
			int frameNumber = 0;
			string frameId;
			foreach( GifFrame thisFrame in _decoder.Frames )
			{
				frameId = "Frame " + frameNumber;
				Assert.IsNull( thisFrame.LocalColourTable, frameId );
				Assert.AreEqual( 10, thisFrame.Delay, frameId );
				
				#region image descriptor tests
				ImageDescriptor descriptor = thisFrame.ImageDescriptor;
				Assert.AreEqual( false, descriptor.HasLocalColourTable, frameId );
				Assert.AreEqual( 2, 
				                 descriptor.LocalColourTableSize, 
				                 frameId );
				Assert.AreEqual( 200, descriptor.Size.Width, frameId );
				Assert.AreEqual( 191, descriptor.Size.Height, frameId );
				Assert.AreEqual( 0, descriptor.Position.X, frameId );
				Assert.AreEqual( 0, descriptor.Position.Y, frameId );
				Assert.AreEqual( false, descriptor.IsInterlaced, frameId );
				Assert.AreEqual( false, descriptor.IsSorted, frameId );
				#endregion
				
				#region graphic control extension tests
				GraphicControlExtension gce = thisFrame.GraphicControlExtension;
				Assert.AreEqual( 4, gce.BlockSize, frameId );
				Assert.AreEqual( 10, gce.DelayTime, frameId );
				if( frameNumber == 19 )
				{
					Assert.AreEqual( DisposalMethod.DoNotDispose, 
					                 gce.DisposalMethod, frameId );
				}
				else
				{
					Assert.AreEqual( DisposalMethod.RestoreToBackgroundColour, 
					                 gce.DisposalMethod, frameId );
				}
				Assert.AreEqual( 63, gce.TransparentColourIndex, frameId );
				Assert.IsTrue( gce.HasTransparentColour, frameId );
				Assert.IsFalse( gce.ExpectsUserInput, frameId );
				#endregion
				
				frameNumber++;
			}

			CompareFrames( fileName );
		}
		#endregion

		#region BadSignature
		/// <summary>
		/// Checks that an attempt to decode a non-GIF file results in a status
		/// of BadSignature.
		/// </summary>
		[Test]
		public void BadSignature()
		{
			ReportStart();
			
			string[] files = Directory.GetFiles( Directory.GetCurrentDirectory() );
			foreach( string file in files )
			{
				Stream s = File.Open( file, FileMode.Open );
				byte[] bytes = new byte[3];
				s.Read( bytes, 0, 3 );
				s.Close();
				
				StringBuilder sb = new StringBuilder();
				foreach( byte b in bytes )
				{
					sb.Append( (char) b );
				}
				if( sb.ToString() != "GIF" )
				{
					_decoder = new GifDecoder( file );
					_decoder.Decode();
					Assert.AreEqual( true, 
					                 _decoder.TestState( ErrorState.BadSignature ),
					                 file + ": " + _decoder.ConsolidatedState );
				}
			}
			ReportEnd();
		}
		#endregion
		
		#region NullFileName
		/// <summary>
		/// Checks that the correct exception is thrown if the constructor
		/// is passed a null filename.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		public void NullFileName()
		{
			ReportStart();
			
			string fileName = null;
			try
			{
				_decoder = new GifDecoder( fileName );
			}
			catch( ArgumentNullException ex )
			{
				StringAssert.Contains( "name", ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region NullStream
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor
		/// is passed a null stream.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		public void NullStream()
		{
			ReportStart();
			
			Stream stream = null;
			try
			{
				_decoder = new GifDecoder( stream );
			}
			catch( ArgumentNullException ex )
			{
				StringAssert.Contains( "inputStream", ex.Message );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region FileNotFound
		/// <summary>
		/// Tests that the decoder throws the correct exception when attempting
		/// to read a file which does not exist.
		/// </summary>
		[Test]
		[ExpectedException( typeof( FileNotFoundException ) )]
		public void FileNotFound()
		{
			ReportStart();
			
			_decoder = new GifDecoder( "nonexist.gif" );
			// Don't try to catch the exception because it's thrown by the CLR
			// rather than by GifDecoder.
		}
		#endregion
		
		#region UnreadableStream
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor
		/// is passed a stream that cannot be read.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentException ) )]
		public void UnreadableStream()
		{
			ReportStart();
			
			// open a write-only stream
			Stream s = File.OpenWrite( "temp.temp" );
			try
			{
				_decoder = new GifDecoder( s );
			}
			catch( ArgumentException ex )
			{
				s.Close();
				string message
					= "The supplied stream cannot be read";
				Assert.AreEqual( "inputStream", ex.ParamName );
				StringAssert.Contains( message, ex.Message );
				ReportEnd();
				throw;
			}
			finally
			{
				s.Close();
				File.Delete( "temp.temp" );
			}
		}
		#endregion

		#region LsdEndOfInputStream
		/// <summary>
		/// Checks that the correct error status is set when the end of the
		/// input stream is reached whilst decoding the logical screen 
		/// descriptor.
		/// </summary>
		[Test]
		public void LsdEndOfInputStream()
		{
			_decoder = new GifDecoder( GifFileName );
			_decoder.Decode();
			Assert.AreEqual( ErrorState.EndOfInputStream, 
			                 _decoder.ConsolidatedState );
		}
		#endregion
		
		#region DecodeAllGifs
		private string _status;
		
		/// <summary>
		/// Attempts to decode every file with a .gif extension on the current
		/// drive.
		/// This helps to highlight any scenarios where the decoder fails to
		/// deal gracefully with a badly encoded file.
		/// </summary>
		[Test]
		[Ignore( "Takes ages to run" )]
		public void DecodeAllGifs()
		{
			ReportStart();
			
			Thread t = new Thread( StartLooking );
			t.IsBackground = true;
			t.Start();
			while( t.IsAlive )
			{
				Thread.Sleep( 5000 );
				WriteMessage( _status );
			}
			
			ReportEnd();
		}
		
		private void StartLooking()
		{
			LookIn( Path.GetPathRoot( Directory.GetCurrentDirectory() ) );
		}

		[SuppressMessage("Microsoft.Performance", 
		                 "CA1804:RemoveUnusedLocals", 
		                 MessageId = "decoder")]
		private void LookIn( string path )
		{
			_status = "Looking in " + path;
			string[] gifs;
			try
			{
				gifs = Directory.GetFiles( path, "*.gif" );
				foreach( string gif in gifs )
				{
					_status = "Decoding " + gif;
					try
					{
						GifDecoder decoder = new GifDecoder( gif );
						decoder.Dispose();
					}
					catch( Exception )
					{
						WriteMessage( "Exception while decoding " + gif );
						throw;
					}
				}
				
				string[] folders = Directory.GetDirectories( path );
				foreach( string folder in folders )
				{
					LookIn( folder );
				}
			}
			catch( UnauthorizedAccessException ) 
			{
				// suppress
			}
		}
		
		#endregion
		
		#region private void CompareFrames method
		private void CompareFrames( string baseFileName )
		{
			for( int f = 0; f < _decoder.Frames.Count; f++ )
			{
				string frameFileName = baseFileName + ".frame " + f + ".bmp";
				Bitmap expectedBitmap = new Bitmap( frameFileName );
				Bitmap actualBitmap = (Bitmap) _decoder.Frames[f].TheImage;
				BitmapAssert.AreEqual( expectedBitmap, 
				                       actualBitmap, 
				                       "frame " + f );
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
		~GifDecoderTest()
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
					_decoder.Dispose();
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

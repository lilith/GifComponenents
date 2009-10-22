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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Globalization;

namespace GifComponents.NUnit
{
	/// <summary>
	/// Test fixture for the GifDecoder class.
	/// </summary>
	[TestFixture]
	[SuppressMessage("Microsoft.Design", 
	                 "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public class GifDecoderTest
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
			string fileName = @"images\smiley\smiley.gif";
			_decoder = new GifDecoder( fileName );
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
			string fileName = @"images\smiley\smiley.gif";
			Stream inputStream = new FileInfo( fileName ).OpenRead();
			_decoder = new GifDecoder( inputStream );
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
		}
		#endregion
		
		#region DecodeRotatingGlobe
		/// <summary>
		/// Tests the GifDecoder class with a rotating globe image.
		/// </summary>
		[Test]
		public void DecodeRotatingGlobe()
		{
			string fileName 
				= @"images\globe\spinning globe better 200px transparent background.gif";
			_decoder = new GifDecoder( fileName );
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
			Assert.AreEqual( Color.FromArgb( 255, 255, 255, 255 ), _decoder.BackgroundColour );
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
			string[] files = Directory.GetFiles( Directory.GetCurrentDirectory() );
			foreach( string file in files )
			{
				if( string.Compare( Path.GetExtension( file ), 
				                    ".gif",
				                    true,
				                    CultureInfo.InvariantCulture ) 
				    != 0 )
				{
					_decoder = new GifDecoder( file );
					Assert.IsTrue( _decoder.TestState( ErrorState.BadSignature ),
					               _decoder.ErrorState.ToString() );
				}
			}
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
			string fileName = null;
			try
			{
				_decoder = new GifDecoder( fileName );
			}
			catch( ArgumentNullException ex )
			{
				StringAssert.Contains( "name", ex.Message );
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
			Stream stream = null;
			try
			{
				_decoder = new GifDecoder( stream );
			}
			catch( ArgumentNullException ex )
			{
				StringAssert.Contains( "inputStream", ex.Message );
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
				throw;
			}
			finally
			{
				s.Close();
				File.Delete( "temp.temp" );
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
				Bitmap actualBitmap = _decoder.Frames[f].TheImage;
				BitmapAssert.AreEqual( expectedBitmap, 
				                       actualBitmap, 
				                       "frame " + f );
			}
		}
		#endregion
		
	}
}

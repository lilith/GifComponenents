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
using System.Drawing;
using System.IO;
using NUnit.Framework;
using NUnit.Extensions;

namespace GifComponents.NUnit
{
	/// <summary>
	/// Test fixture for the AnimatedGifEncoder class.
	/// TODO: more private methods needed to simplify the actual test cases
	/// </summary>
	[TestFixture]
	public class AnimatedGifEncoderTest
	{
		private AnimatedGifEncoder _e;
		private GifDecoder _d;
		
		#region UseGlobal
		/// <summary>
		/// Tests the encoder using a two-frame 2x2 pixel checkerboard animation
		/// with a global colour table.
		/// </summary>
		[Test]
		public void UseGlobal()
		{
			_e = new AnimatedGifEncoder( new Size( 2, 2 ), 
			                             0,
			                             ColourTableStrategy.UseGlobal,
			                             10 );
			_e.AddFrame( Bitmap1(), 10 );
			_e.AddFrame( Bitmap2(), 10 );
			string fileName = "Checks.UseGlobal.gif";
			_e.WriteToFile( fileName );
			
			Stream s = File.OpenRead( fileName );

			// global info
			CheckGifHeader( s );
			LogicalScreenDescriptor lsd = CheckLogicalScreenDescriptor( s, true );
			ColourTable gct = CheckColourTable( s, lsd.GlobalColourTableSize );
			CheckExtensionIntroducer( s );
			CheckAppExtensionLabel( s );
			CheckNetscapeExtension( s, 0 );
			
			// start of first frame info
			CheckExtensionIntroducer( s );
			CheckGraphicControlLabel( s );
			CheckGraphicControlExtension( s );
			CheckImageSeparator( s );
			ImageDescriptor id = CheckImageDescriptor( s, false, 0 );
			CheckImageData( s, gct, id, Bitmap1() );
			CheckBlockTerminator( s );
			
			// start of second frame info
			CheckExtensionIntroducer( s );
			CheckGraphicControlLabel( s );
			CheckGraphicControlExtension( s );
			CheckImageSeparator( s );
			id = CheckImageDescriptor( s, false, 0 );
			CheckImageData( s, gct, id, Bitmap2() );
			CheckBlockTerminator( s );
			
			// end of image data
			CheckGifTrailer( s );
			CheckEndOfStream( s );
			s.Close();
			
			// Check the file using the decoder
			_d = new GifDecoder( fileName );
			Assert.AreEqual( ErrorState.Ok, _d.ConsolidatedState );
			Assert.AreEqual( 2, _d.Frames.Count );
			Assert.AreEqual( true, _d.LogicalScreenDescriptor.HasGlobalColourTable );
			BitmapAssert.AreEqual( Bitmap1(), _d.Frames[0].TheImage, "frame 0" );
			BitmapAssert.AreEqual( Bitmap2(), _d.Frames[1].TheImage, "frame 1" );
			Assert.AreEqual( false, _d.Frames[0].ImageDescriptor.HasLocalColourTable );
			Assert.AreEqual( false, _d.Frames[1].ImageDescriptor.HasLocalColourTable );
		}
		#endregion

		#region UseLocal
		/// <summary>
		/// Tests the encoder using a two-frame 2x2 pixel checkerboard animation
		/// with local colour tables.
		/// </summary>
		[Test]
		public void UseLocal()
		{
			_e = new AnimatedGifEncoder( new Size( 2, 2 ), 
			                             0,
			                             ColourTableStrategy.UseLocal,
			                             10 );
			_e.AddFrame( Bitmap1(), 10 );
			_e.AddFrame( Bitmap2(), 10 );
			string fileName = "Checks.UseLocal.gif";
			_e.WriteToFile( fileName );
			
			Stream s = File.OpenRead( fileName );

			// global info
			CheckGifHeader( s );
			CheckLogicalScreenDescriptor( s, false );
			// no global colour table here
			CheckExtensionIntroducer( s );
			CheckAppExtensionLabel( s );
			CheckNetscapeExtension( s, 0 );
			
			ColourTable lct;
			
			// start of first frame info
			CheckExtensionIntroducer( s );
			CheckGraphicControlLabel( s );
			CheckGraphicControlExtension( s );
			CheckImageSeparator( s );
			ImageDescriptor id = CheckImageDescriptor( s, true, 1 );
			lct = CheckColourTable( s, id.LocalColourTableSize );
			CheckImageData( s, lct, id, Bitmap1() );
			CheckBlockTerminator( s );
			
			// start of second frame info
			CheckExtensionIntroducer( s );
			CheckGraphicControlLabel( s );
			CheckGraphicControlExtension( s );
			CheckImageSeparator( s );
			id = CheckImageDescriptor( s, true, 1 );
			lct = CheckColourTable( s, id.LocalColourTableSize );
			CheckImageData( s, lct, id, Bitmap2() );
			CheckBlockTerminator( s );
			
			// end of image data
			CheckGifTrailer( s );
			CheckEndOfStream( s );
			s.Close();
			
			// Check the file using the decoder
			_d = new GifDecoder( fileName );
			Assert.AreEqual( ErrorState.Ok, _d.ConsolidatedState );
			Assert.AreEqual( 2, _d.Frames.Count );
			Assert.AreEqual( false, _d.LogicalScreenDescriptor.HasGlobalColourTable );
			BitmapAssert.AreEqual( Bitmap1(), _d.Frames[0].TheImage, "frame 0" );
			BitmapAssert.AreEqual( Bitmap2(), _d.Frames[1].TheImage, "frame 1" );
			Assert.AreEqual( true, _d.Frames[0].ImageDescriptor.HasLocalColourTable );
			Assert.AreEqual( true, _d.Frames[1].ImageDescriptor.HasLocalColourTable );
		}
		#endregion

		#region DefaultRepeatCount
		/// <summary>
		/// Checks that when the encoded is passed a repeat count of less than
		/// -1, the repeat count defaults to -1.
		/// </summary>
		[Test]
		public void DefaultRepeatCount()
		{
			_e = new AnimatedGifEncoder( new Size( 2, 2 ), 
			                             -2,
			                             ColourTableStrategy.UseGlobal,
			                             10 );
			_e.AddFrame( Bitmap1(), 10 );
			_e.AddFrame( Bitmap2(), 10 );
			string fileName = "Checks.DefaultRepeatCount.gif";
			_e.WriteToFile( fileName );
			
			Stream s = File.OpenRead( fileName );

			// global info
			CheckGifHeader( s );
			LogicalScreenDescriptor lsd = CheckLogicalScreenDescriptor( s, true );
			ColourTable gct = CheckColourTable( s, lsd.GlobalColourTableSize );
			
			// Because the repeat count is set to -1 (no repeat) there should
			// be no Netscape extension
//			CheckExtensionIntroducer( s );
//			CheckAppExtensionLabel( s );
//			CheckNetscapeExtension( s, -1 );
			
			// start of first frame info
			CheckExtensionIntroducer( s );
			CheckGraphicControlLabel( s );
			CheckGraphicControlExtension( s );
			CheckImageSeparator( s );
			ImageDescriptor id = CheckImageDescriptor( s, false, 0 );
			CheckImageData( s, gct, id, Bitmap1() );
			CheckBlockTerminator( s );
			
			// start of second frame info
			CheckExtensionIntroducer( s );
			CheckGraphicControlLabel( s );
			CheckGraphicControlExtension( s );
			CheckImageSeparator( s );
			id = CheckImageDescriptor( s, false, 0 );
			CheckImageData( s, gct, id, Bitmap2() );
			CheckBlockTerminator( s );
			
			// end of image data
			CheckGifTrailer( s );
			CheckEndOfStream( s );
			s.Close();
			
			// Check the file using the decoder
			_d = new GifDecoder( fileName );
			Assert.AreEqual( ErrorState.Ok, _d.ConsolidatedState );
			Assert.AreEqual( 2, _d.Frames.Count );
			Assert.AreEqual( true, _d.LogicalScreenDescriptor.HasGlobalColourTable );
			BitmapAssert.AreEqual( Bitmap1(), _d.Frames[0].TheImage, "frame 0" );
			BitmapAssert.AreEqual( Bitmap2(), _d.Frames[1].TheImage, "frame 1" );
			Assert.AreEqual( false, _d.Frames[0].ImageDescriptor.HasLocalColourTable );
			Assert.AreEqual( false, _d.Frames[1].ImageDescriptor.HasLocalColourTable );
		}
		#endregion

		#region DefaultQuality
		/// <summary>
		/// Checks that when the encoder is passed a quality of less than 1, the
		/// quality defaults to 1.
		/// </summary>
		[Test]
		public void DefaultQuality()
		{
			_e = new AnimatedGifEncoder( new Size( 2, 2 ), 
			                             0,
			                             ColourTableStrategy.UseGlobal,
			                             0 );
			_e.AddFrame( Bitmap1(), 10 );
			_e.AddFrame( Bitmap2(), 10 );
			string fileName = "Checks.DefaultQuality.gif";
			_e.WriteToFile( fileName );
			
			// TODO: no way to check the actual colour quantization quality!
			
			Stream s = File.OpenRead( fileName );

			// global info
			CheckGifHeader( s );
			LogicalScreenDescriptor lsd = CheckLogicalScreenDescriptor( s, true );
			ColourTable gct = CheckColourTable( s, lsd.GlobalColourTableSize );
			CheckExtensionIntroducer( s );
			CheckAppExtensionLabel( s );
			CheckNetscapeExtension( s, 0 );
			
			// start of first frame info
			CheckExtensionIntroducer( s );
			CheckGraphicControlLabel( s );
			CheckGraphicControlExtension( s );
			CheckImageSeparator( s );
			ImageDescriptor id = CheckImageDescriptor( s, false, 0 );
			CheckImageData( s, gct, id, Bitmap1() );
			CheckBlockTerminator( s );
			
			// start of second frame info
			CheckExtensionIntroducer( s );
			CheckGraphicControlLabel( s );
			CheckGraphicControlExtension( s );
			CheckImageSeparator( s );
			id = CheckImageDescriptor( s, false, 0 );
			CheckImageData( s, gct, id, Bitmap2() );
			CheckBlockTerminator( s );
			
			// end of image data
			CheckGifTrailer( s );
			CheckEndOfStream( s );
			s.Close();
			
			// Check the file using the decoder
			_d = new GifDecoder( fileName );
			Assert.AreEqual( ErrorState.Ok, _d.ConsolidatedState );
			Assert.AreEqual( 2, _d.Frames.Count );
			Assert.AreEqual( true, _d.LogicalScreenDescriptor.HasGlobalColourTable );
			BitmapAssert.AreEqual( Bitmap1(), _d.Frames[0].TheImage, "frame 0" );
			BitmapAssert.AreEqual( Bitmap2(), _d.Frames[1].TheImage, "frame 1" );
			Assert.AreEqual( false, _d.Frames[0].ImageDescriptor.HasLocalColourTable );
			Assert.AreEqual( false, _d.Frames[1].ImageDescriptor.HasLocalColourTable );
		}
		#endregion

		#region WithTransparency
		/// <summary>
		/// Checks that when the encoder sets the transparent colour correctly.
		/// </summary>
		[Test]
		[Ignore( "The way transparency should work isn't clearly defined" )]
		public void WithTransparency()
		{
			_e = new AnimatedGifEncoder( new Size( 2, 2 ), 
			                             0,
			                             ColourTableStrategy.UseGlobal,
			                             10 );
			_e.Transparent = Color.White;
			_e.AddFrame( Bitmap1(), 10 );
			_e.AddFrame( Bitmap2(), 10 );
			string fileName = "Checks.WithTransparency.gif";
			_e.WriteToFile( fileName );
			
			Stream s = File.OpenRead( fileName );

			// global info
			CheckGifHeader( s );
			LogicalScreenDescriptor lsd = CheckLogicalScreenDescriptor( s, true );
			ColourTable gct = CheckColourTable( s, lsd.GlobalColourTableSize );
			CheckExtensionIntroducer( s );
			CheckAppExtensionLabel( s );
			CheckNetscapeExtension( s, 0 );
			
			// start of first frame info
			CheckExtensionIntroducer( s );
			CheckGraphicControlLabel( s );
			CheckGraphicControlExtension( s, Color.White );
			CheckImageSeparator( s );
			ImageDescriptor id = CheckImageDescriptor( s, false, 0 );
			CheckImageData( s, gct, id, Bitmap1() );
			CheckBlockTerminator( s );
			
			// start of second frame info
			CheckExtensionIntroducer( s );
			CheckGraphicControlLabel( s );
			CheckGraphicControlExtension( s, Color.White );
			CheckImageSeparator( s );
			id = CheckImageDescriptor( s, false, 0 );
			CheckImageData( s, gct, id, Bitmap2() );
			CheckBlockTerminator( s );
			
			// end of image data
			CheckGifTrailer( s );
			CheckEndOfStream( s );
			s.Close();
			
			// Check the file using the decoder
			_d = new GifDecoder( fileName );
			Assert.AreEqual( ErrorState.Ok, _d.ConsolidatedState );
			Assert.AreEqual( 2, _d.Frames.Count );
			Assert.AreEqual( true, _d.LogicalScreenDescriptor.HasGlobalColourTable );
			BitmapAssert.AreEqual( Bitmap1(), _d.Frames[0].TheImage, "frame 0" );
			BitmapAssert.AreEqual( Bitmap2(), _d.Frames[1].TheImage, "frame 1" );
			Assert.AreEqual( false, _d.Frames[0].ImageDescriptor.HasLocalColourTable );
			Assert.AreEqual( false, _d.Frames[1].ImageDescriptor.HasLocalColourTable );
		}
		#endregion
		
		#region private static Bitmap1 method
		private static Bitmap Bitmap1()
		{
			Bitmap bitmap1 = new Bitmap( 2, 2 );
			bitmap1.SetPixel( 0, 0, Color.Black );
			bitmap1.SetPixel( 0, 1, Color.White );
			bitmap1.SetPixel( 1, 0, Color.White );
			bitmap1.SetPixel( 1, 1, Color.Black );
			return bitmap1;
		}
		#endregion
		
		#region private static Bitmap2 method
		private static Bitmap Bitmap2()
		{
			Bitmap bitmap2 = new Bitmap( 2, 2 );
			bitmap2.SetPixel( 0, 0, Color.White );
			bitmap2.SetPixel( 0, 1, Color.Black );
			bitmap2.SetPixel( 1, 0, Color.Black );
			bitmap2.SetPixel( 1, 1, Color.White );
			return bitmap2;
		}
		#endregion

		#region private static CheckGifHeader method
		private static void CheckGifHeader( Stream s )
		{
			// check GIF header
			GifHeader gh = GifHeader.FromStream( s );
			Assert.AreEqual( ErrorState.Ok, gh.ConsolidatedState );
			Assert.AreEqual( "GIF", gh.Signature );
			Assert.AreEqual( "89a", gh.Version );
		}
		#endregion
		
		#region private static CheckLogicalScreenDescriptor method
		private static LogicalScreenDescriptor
			CheckLogicalScreenDescriptor( Stream s,
			                              bool shouldHaveGlobalColourTable )
		{
			// check logical screen descriptor
			LogicalScreenDescriptor lsd = LogicalScreenDescriptor.FromStream( s );
			Assert.AreEqual( ErrorState.Ok, lsd.ConsolidatedState );
			Assert.AreEqual( new Size( 2, 2 ), lsd.LogicalScreenSize );
			Assert.AreEqual( shouldHaveGlobalColourTable, lsd.HasGlobalColourTable );
			Assert.AreEqual( false, lsd.GlobalColourTableIsSorted );
			return lsd;
		}
		#endregion
		
		#region private static CheckColourTable method
		private static ColourTable CheckColourTable( Stream s, 
		                                             int colourTableSize )
		{
			// read colour table
			ColourTable ct = ColourTable.FromStream( s, colourTableSize );
			Assert.AreEqual( ErrorState.Ok, ct.ConsolidatedState );
			return ct;
		}
		#endregion
		
		#region private static CheckExtensionIntroducer method
		private static void CheckExtensionIntroducer( Stream s )
		{
			// check for extension introducer
			int code = ExampleComponent.CallRead( s );
			Assert.AreEqual( GifComponent.CodeExtensionIntroducer, code );
		}
		#endregion
		
		#region private static CheckAppExtensionLabel method
		private static void CheckAppExtensionLabel( Stream s )
		{
			// check for app extension label
			int code = ExampleComponent.CallRead( s );
			Assert.AreEqual( GifComponent.CodeApplicationExtensionLabel, code );
		}
		#endregion
		
		#region private static CheckNetscapeExtension method
		private static void CheckNetscapeExtension( Stream s, int loopCount )
		{
			// check netscape extension
			ApplicationExtension ae = ApplicationExtension.FromStream( s );
			Assert.AreEqual( ErrorState.Ok, ae.ConsolidatedState );
			NetscapeExtension ne = new NetscapeExtension( ae );
			Assert.AreEqual( ErrorState.Ok, ne.ConsolidatedState );
			Assert.AreEqual( loopCount, ne.LoopCount );
		}
		#endregion
		
		#region private static CheckGraphicControlLabel method
		private static void CheckGraphicControlLabel( Stream s )
		{
			// check for gce label
			int code = ExampleComponent.CallRead( s );
			Assert.AreEqual( GifComponent.CodeGraphicControlLabel, code );
		}
		#endregion
		
		#region private static CheckGraphicControlExtension methods
		private static void CheckGraphicControlExtension( Stream s )
		{
			CheckGraphicControlExtension( s, Color.Empty );
		}
		
		private static void CheckGraphicControlExtension( Stream s, 
		                                                  Color transparentColour )
		{
			// check graphic control extension
			GraphicControlExtension gce = GraphicControlExtension.FromStream( s );
			Assert.AreEqual( ErrorState.Ok, gce.ConsolidatedState );
			Assert.AreEqual( 10, gce.DelayTime );
			if( transparentColour == Color.Empty )
			{
				Assert.AreEqual( 0, gce.TransparentColourIndex );
				Assert.AreEqual( false, gce.HasTransparentColour );
				Assert.AreEqual( DisposalMethod.DoNotDispose, gce.DisposalMethod );
			}
			else
			{
				Assert.AreEqual( true, gce.HasTransparentColour );
				// TODO: a way to check the transparent colour index?
				Assert.AreEqual( DisposalMethod.RestoreToBackgroundColour, 
				                 gce.DisposalMethod );
			}
			Assert.AreEqual( false, gce.ExpectsUserInput );
		}
		#endregion
		
		#region private static CheckImageSeparator method
		private static void CheckImageSeparator( Stream s )
		{
			// check for image separator
			int code = ExampleComponent.CallRead( s );
			Assert.AreEqual( GifComponent.CodeImageSeparator, code );
		}
		#endregion
		
		#region private static CheckImageDescriptor method
		private static ImageDescriptor 
			CheckImageDescriptor( Stream s, 
			                      bool shouldHaveLocalColourTable,
			                      int localColourTableSizeBits )
		{
			// check for image descriptor
			ImageDescriptor id = ImageDescriptor.FromStream( s );
			Assert.AreEqual( ErrorState.Ok, id.ConsolidatedState );
			Assert.AreEqual( shouldHaveLocalColourTable, id.HasLocalColourTable );
			Assert.AreEqual( false, id.IsInterlaced );
			Assert.AreEqual( false, id.IsSorted );
			Assert.AreEqual( localColourTableSizeBits, id.LocalColourTableSizeBits );
			Assert.AreEqual( 1 << (localColourTableSizeBits + 1), 
			                 id.LocalColourTableSize );
			Assert.AreEqual( new Size( 2, 2 ), id.Size );
			Assert.AreEqual( new Point( 0, 0 ), id.Position );
			return id;
		}
		#endregion
		
		#region private static CheckImageData method
		private static void CheckImageData( Stream s, 
		                                    ColourTable act, 
		                                    ImageDescriptor id, 
		                                    Bitmap expectedBitmap )
		{
			// read, decode and check image data
			// Cannot compare encoded LZW data directly as different encoders
			// will create different colour tables, so even if the bitmaps are
			// identical, the colour indices will be different
			int pixelCount = id.Size.Width * id.Size.Height;
			TableBasedImageData tbid = new TableBasedImageData( s, pixelCount );
			Assert.AreEqual( ErrorState.Ok, tbid.ConsolidatedState );
			for( int y = 0; y < id.Size.Height; y++ )
			{
				for( int x = 0; x < id.Size.Width; x++ )
				{
					int i = (y * id.Size.Width) + x;
					Assert.AreEqual( expectedBitmap.GetPixel( x, y ),
					                 act[tbid.Pixels[i]],
					                 "X: " + x + ", Y: " + y );
				}
			}
		}
		#endregion
		
		#region private static void CheckBlockTerminator method
		private static void CheckBlockTerminator( Stream s )
		{
			// Check for block terminator after image data
			int code = ExampleComponent.CallRead( s );
			Assert.AreEqual( 0x00, code );
		}
		#endregion
		
		#region private static void CheckGifTrailer method
		private static void CheckGifTrailer( Stream s )
		{
			// check for GIF trailer
			int code = ExampleComponent.CallRead( s );
			Assert.AreEqual( GifComponent.CodeTrailer, code );
		}
		#endregion
		
		#region private static void CheckEndOfStream method
		private static void CheckEndOfStream( Stream s )
		{
			// check we're at the end of the stream
			int code = ExampleComponent.CallRead( s );
			Assert.AreEqual( -1, code );
		}
		#endregion
		
		#region WikipediaExampleTest
		/// <summary>
		/// Uses the encoder to create a GIF file using the example at
		/// http://en.wikipedia.org/wiki/Gif#Example_.gif_file and then reads
		/// back in and verifies all its components.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
		                 MessageId = "Wikipedia")]
		public void WikipediaExampleTest()
		{
			_e = new AnimatedGifEncoder();
			_e.AddFrame( WikipediaExample.ExpectedBitmap,
			             WikipediaExample.DelayTime );
			string fileName = "WikipediaExampleUseGlobal.gif";
			_e.WriteToFile( fileName );
			Stream s = File.OpenRead( fileName );
			
			int code;

			// check GIF header
			GifHeader gh = GifHeader.FromStream( s );
			Assert.AreEqual( ErrorState.Ok, gh.ConsolidatedState );

			// check logical screen descriptor
			LogicalScreenDescriptor lsd = LogicalScreenDescriptor.FromStream( s );
			Assert.AreEqual( ErrorState.Ok, lsd.ConsolidatedState );
			WikipediaExample.CheckLogicalScreenDescriptor( lsd );
			
			// read global colour table
			ColourTable gct = ColourTable.FromStream( s, WikipediaExample.GlobalColourTableSize );
			Assert.AreEqual( ErrorState.Ok, gct.ConsolidatedState );
			// cannot compare global colour table as different encoders will
			// produce difference colour tables.
//			WikipediaExample.CheckGlobalColourTable( gct );
			
			// check for extension introducer
			code = ExampleComponent.CallRead( s );
			Assert.AreEqual( GifComponent.CodeExtensionIntroducer, code );
			
			// check for app extension label
			code = ExampleComponent.CallRead( s );
			Assert.AreEqual( GifComponent.CodeApplicationExtensionLabel, code );
			
			// check netscape extension
			ApplicationExtension ae = ApplicationExtension.FromStream( s );
			Assert.AreEqual( ErrorState.Ok, ae.ConsolidatedState );
			NetscapeExtension ne = new NetscapeExtension( ae );
			Assert.AreEqual( ErrorState.Ok, ne.ConsolidatedState );
			Assert.AreEqual( 0, ne.LoopCount );
			
			// check for extension introducer
			code = ExampleComponent.CallRead( s );
			Assert.AreEqual( GifComponent.CodeExtensionIntroducer, code );

			// check for gce label
			code = ExampleComponent.CallRead( s );
			Assert.AreEqual( GifComponent.CodeGraphicControlLabel, code );
			
			// check graphic control extension
			GraphicControlExtension gce = GraphicControlExtension.FromStream( s );
			Assert.AreEqual( ErrorState.Ok, gce.ConsolidatedState );
			WikipediaExample.CheckGraphicControlExtension( gce );

			// check for image separator
			code = ExampleComponent.CallRead( s );
			Assert.AreEqual( GifComponent.CodeImageSeparator, code );
			
			// check for image descriptor
			ImageDescriptor id = ImageDescriptor.FromStream( s );
			Assert.AreEqual( ErrorState.Ok, id.ConsolidatedState );
			WikipediaExample.CheckImageDescriptor( id );

			// read, decode and check image data
			// Cannot compare encoded LZW data directly as different encoders
			// will create different colour tables, so even if the bitmaps are
			// identical, the colour indices will be different
			int pixelCount = WikipediaExample.FrameSize.Width
							* WikipediaExample.FrameSize.Height;
			TableBasedImageData tbid = new TableBasedImageData( s, pixelCount );
			for( int y = 0; y < WikipediaExample.LogicalScreenSize.Height; y++ )
			{
				for( int x = 0; x < WikipediaExample.LogicalScreenSize.Width; x++ )
				{
					int i = (y * WikipediaExample.LogicalScreenSize.Width) + x;
					Assert.AreEqual( WikipediaExample.ExpectedBitmap.GetPixel( x, y ),
					                 gct[tbid.Pixels[i]],
					                 "X: " + x + ", Y: " + y );
				}
			}

			// Check for block terminator after image data
			code = ExampleComponent.CallRead( s );
			Assert.AreEqual( 0x00, code );
			
			// check for GIF trailer
			code = ExampleComponent.CallRead( s );
			Assert.AreEqual( GifComponent.CodeTrailer, code );
			
			// check we're at the end of the stream
			code = ExampleComponent.CallRead( s );
			Assert.AreEqual( -1, code );
			s.Close();
			
			_d = new GifDecoder( fileName );
			Assert.AreEqual( ErrorState.Ok, _d.ConsolidatedState );
			BitmapAssert.AreEqual( WikipediaExample.ExpectedBitmap, 
			                       _d.Frames[0].TheImage, 
			                       "" );
		}
		#endregion
	}
}

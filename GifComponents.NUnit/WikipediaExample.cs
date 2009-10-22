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
using NUnit.Extensions;

namespace GifComponents.NUnit
{
	/// <summary>
	/// Returns data items and data streams related to the GIF example published
	/// at http://en.wikipedia.org/wiki/Gif#Example_.gif_file
	/// TODO: base (abstract?) class for multiple example files
	/// </summary>
	internal static class WikipediaExample
	{
		#region file-level constants
		
		/// <summary>
		/// The size of the logical screen for this file
		/// </summary>
		internal static readonly Size LogicalScreenSize = new Size( 3, 5);
		
		/// <summary>
		/// Indicates that this file has a global colour table
		/// </summary>
		internal const bool HasGlobalColourTable = true;
		
		/// <summary>
		/// The number of bits required to hold the colour resolution of the
		/// file, minus 1.
		/// </summary>
		internal const int ColourResolution = 7;
		
		/// <summary>
		/// Indicates that the global colour table for this file is not sorted.
		/// </summary>
		internal const bool GlobalColourTableIsSorted = false;
		
		/// <summary>
		/// The number of bits required to hold the number of bytes in the
		/// global colour table, minus 1.
		/// </summary>
		internal const int GlobalColourTableSizeBits = 1;
		
		/// <summary>
		/// The number of colours in the global colour table
		/// </summary>
		internal const int GlobalColourTableSize = 4;
		
		/// <summary>
		/// The index within the global colour table of the background colour.
		/// </summary>
		internal const int BackgroundColourIndex = 0;
		
		/// <summary>
		/// The pixel aspect ratio of images within this file.
		/// </summary>
		internal const int PixelAspectRatio = 0;
		
		#endregion
		
		#region frame-level constants
		
		/// <summary>
		/// The size of the one frame in this file
		/// </summary>
		internal static readonly Size FrameSize = new Size( 3, 5 );
		
		/// <summary>
		/// The position of the one frame in this file within the logical screen
		/// </summary>
		internal static readonly Point FramePosition = new Point( 0, 0 );
		
		/// <summary>
		/// The background colour of the one frame in the file
		/// </summary>
		internal static readonly Color BackgroundColour = Color.Black;
		
		/// <summary>
		/// The disposal method of the one frame in the file
		/// </summary>
		internal static readonly DisposalMethod DisposalMethod = DisposalMethod.DoNotDispose;
		
		/// <summary>
		/// Indicates that the one frame in the file does not expect user input
		/// before any subsequent frames are displayed
		/// </summary>
		internal const bool ExpectsUserInput = false;
		
		/// <summary>
		/// Indicates that the one frame in the file does not have a transparent
		/// colour
		/// </summary>
		internal const bool HasTransparentColour = false;
		
		/// <summary>
		/// The delay in hundredths of a second between the one frame in the 
		/// file being displayed and any subsequent frames being displayed
		/// </summary>
		internal const int DelayTime = 5;
		
		/// <summary>
		/// The index in the active colour table of the colour which is to be
		/// treated as transparent, for the one frame in the file.
		/// </summary>
		internal const int TransparentColourIndex = 0;
		
		/// <summary>
		/// Indicates that the one frame in the file does not have a local 
		/// colour table.
		/// </summary>
		internal const bool HasLocalColourTable = false;
		
		/// <summary>
		/// Indicates that the one frame in the file is not an interlaced image.
		/// </summary>
		internal const bool IsInterlaced = false;
		
		/// <summary>
		/// Indicates that the local colour table for the one frame in the file
		/// is not sorted.
		/// </summary>
		internal const bool LocalColourTableIsSorted = false;
		
		/// <summary>
		/// The number of bits required to hold the size of the local colour 
		/// table for the one frame in the file, minus 1.
		/// </summary>
		internal const int LocalColourTableSizeBits = 0;

		#endregion
		
		#region internal static file-level properties
		
		#region internal static LogicalScreenDescriptor property
		/// <summary>
		/// Gets the logical screen descriptor for the file.
		/// </summary>
		internal static LogicalScreenDescriptor LogicalScreenDescriptor
		{
			get
			{
				LogicalScreenDescriptor lsd =
					new LogicalScreenDescriptor( LogicalScreenSize,
					                             HasGlobalColourTable,
					                             ColourResolution,
					                             GlobalColourTableIsSorted,
					                             GlobalColourTableSizeBits,
					                             BackgroundColourIndex,
					                             PixelAspectRatio
					                            );
				CheckLogicalScreenDescriptor( lsd );
				return lsd;
			}
		}
		#endregion
		
		#region internal static GlobalColourTable property
		/// <summary>
		/// Gets the global colour table for the GIF file
		/// </summary>
		internal static ColourTable GlobalColourTable
		{
			get
			{
				ColourTable ct = 
					ColourTable.FromStream( GlobalColourTableStream,
					                        GlobalColourTableSize );
//				CheckGlobalColourTable( ct );
				return ct;
			}
		}
		#endregion
		
		#region internal static GlobalColourTableStream property
		/// <summary>
		/// Gets a stream from which the global colour table for the GIF file
		/// can be instantiated.
		/// </summary>
		internal static Stream GlobalColourTableStream
		{
			get
			{
				byte[] colours = new byte[GlobalColourTableSize*3];
				for( int i = 0; i < GlobalColourTableSize*3; i++ )
				{
					// zeroise everything
					colours[i] = 0;
				}
				// colour 0 is black
				colours[0*3] = 0;
				colours[0*3+1] = 0;
				colours[0*3+2] = 0;
				// colour 1 is white
				colours[1*3] = 255;
				colours[1*3+1] = 255;
				colours[1*3+2] = 255;
				// remaining colours are black
				colours[2*3] = 0;
				colours[2*3+1] = 0;
				colours[2*3+2] = 0;
				colours[3*3] = 0;
				colours[3*3+1] = 0;
				colours[3*3+2] = 0;
				MemoryStream ctStream = new MemoryStream();
				ctStream.Write( colours, 0, colours.Length );
				ctStream.Seek( 0, SeekOrigin.Begin );
				return ctStream;
			}
		}
		#endregion

		#endregion
		
		// TODO maybe frame level properties should be indexed? or in another class?
		#region internal static frame-level properties
		
		#region internal static GraphicControlExtension property
		/// <summary>
		/// Gets the graphic control extension for the one frame in the file.
		/// </summary>
		internal static GraphicControlExtension GraphicControlExtension
		{
			get
			{
				GraphicControlExtension gce 
					= new GraphicControlExtension( GraphicControlExtension.ExpectedBlockSize,
					                               DisposalMethod, 
					                               ExpectsUserInput, 
					                               HasTransparentColour, 
					                               DelayTime, 
					                               TransparentColourIndex
					                              );
				CheckGraphicControlExtension( gce );
				return gce;
			}
		}
		#endregion
		
		#region internal static ImageDescriptor property
		/// <summary>
		/// Gets the image descriptor for the one frame in the file.
		/// </summary>
		internal static ImageDescriptor ImageDescriptor
		{
			get
			{
				ImageDescriptor id 
					= ImageDescriptor.FromStream( ImageDescriptorStream );
				CheckImageDescriptor( id );
				return id;
			}
		}
		#endregion
		
		#region internal static ImageDescriptorStream property
		/// <summary>
		/// Gets a stream from which the image descriptor for the one frame in 
		/// the file can be instantiated.
		/// </summary>
		internal static Stream ImageDescriptorStream
		{
			get
			{
				MemoryStream idStream = new MemoryStream();
				WriteUShort( idStream, FramePosition.X );
				WriteUShort( idStream, FramePosition.Y );
				WriteUShort( idStream, FrameSize.Width );
				WriteUShort( idStream, FrameSize.Height );
				
				byte packed
					= ( HasLocalColourTable ? 1 : 0 ) << 7 // bit 1
					| ( IsInterlaced ? 1 : 0 ) << 6 // bit 2
					| ( LocalColourTableIsSorted ? 1 : 0 ) << 5 // bit 3
					// bits 4 and 5 are reserved
					| LocalColourTableSizeBits // bits 6-8
					;
				idStream.WriteByte( packed );
				idStream.Seek( 0, SeekOrigin.Begin );
				return idStream;
			}
		}
		#endregion
		
		#region internal static ImageDescriptorBytes property
		/// <summary>
		/// Gets an array of bytes representing the image descriptor for the 
		/// one frame in the file.
		/// </summary>
		internal static byte[] ImageDescriptorBytes
		{
			get
			{
				int lengthOfStream = 9; // image descriptor is always 9 bytes
				// (excluding the image separator)
				byte[] idBytes = new byte[lengthOfStream];
				ImageDescriptorStream.Read( idBytes, 0, lengthOfStream );
				return idBytes;
			}
		}
		#endregion
		
		#region internal static ImageData property
		/// <summary>
		/// Gets the table-based image data for the one frame in the file.
		/// </summary>
		internal static TableBasedImageData ImageData
		{
			get
			{
				int pixelCount = FrameSize.Width * FrameSize.Height;
				TableBasedImageData tbid = 
					new TableBasedImageData( ImageDataStream, pixelCount );
				return tbid;
			}
		}
		#endregion
		
		#region internal static ImageDataStream property
		/// <summary>
		/// Gets a stream from which the table-based image data for the one
		/// frame in the file can be instantiated.
		/// </summary>
		internal static Stream ImageDataStream
		{
			get
			{
				MemoryStream tbids = new MemoryStream();
				byte[] bytes = ImageDataBytes;
				tbids.Write( bytes, 0, bytes.Length );
				tbids.Seek( 0, SeekOrigin.Begin );
				return tbids;
			}
		}
		#endregion
		
		#region internal static ImageDataBytes property
		/// <summary>
		/// Gets an array of bytes representing the table-based image data for
		/// the one frame in the file.
		/// </summary>
		internal static byte[] ImageDataBytes
		{
			get
			{
				byte[] bytes = new byte[]
				{
					0x08, // LZW minimum code size
					0x0b, // block size = 11
					// 11 bytes of LZW encoded data follows
					0x00, 0x01, 0x04, 0x18, 0x28, 0x70, 0xA0, 0xC1, 0x83, 0x01, 0x01,
					0x00 // block terminator
				};
				return bytes;
			}
		}
		#endregion

		#region internal static ExpectedBitmap property
		/// <summary>
		/// Gets the bitmap that the one frame in this file should resolve to.
		/// </summary>
		internal static Bitmap ExpectedBitmap
		{
			get
			{
				Bitmap returnValue = new Bitmap( LogicalScreenSize.Width, 
				                                 LogicalScreenSize.Height );
				Color[] colours = new Color[]
				{
					Color.Black, Color.White, Color.White,
					Color.White, Color.Black, Color.White,
					Color.White, Color.White, Color.White,
					Color.White, Color.White, Color.White,
					Color.White, Color.White, Color.White
				};
				
				int i = 0;
				for( int y = 0; y < LogicalScreenSize.Height; y++ )
				{
					for( int x = 0; x < LogicalScreenSize.Width; x++ )
					{
						returnValue.SetPixel( x, y, colours[i] );
						i++;
					}
				}
				
				return returnValue;
			}
		}
		#endregion
		
		#endregion
		
		#region internal static checkout methods
		
		#region CheckLogicalScreenDescriptor method
		/// <summary>
		/// Checks whether the supplied logical screen descriptor matches that
		/// returned by this class.
		/// </summary>
		/// <param name="lsd"></param>
		internal static void CheckLogicalScreenDescriptor( LogicalScreenDescriptor lsd )
		{
			Assert.AreEqual( BackgroundColourIndex, 
			                 lsd.BackgroundColourIndex, 
			                 "BackgroundColourIndex" );
			Assert.AreEqual( ColourResolution, 
			                 lsd.ColourResolution, 
			                 "ColourResolution" );
			Assert.AreEqual( GlobalColourTableSize, 
			                 lsd.GlobalColourTableSize, 
			                 "GlobalColourTableSize" );
			Assert.AreEqual( GlobalColourTableSizeBits, 
			                 lsd.GlobalColourTableSizeBits, 
			                 "GlobalColourTableSizeBits" );
			Assert.AreEqual( HasGlobalColourTable,
			                 lsd.HasGlobalColourTable, 
			                 "HasGlobalColourTable" );
			Assert.AreEqual( LogicalScreenSize, 
			                 lsd.LogicalScreenSize, 
			                 "LogicalScreenSize" );
			Assert.AreEqual( PixelAspectRatio, 
			                 lsd.PixelAspectRatio, 
			                 "PixelAspectRatio" );
			Assert.AreEqual( GlobalColourTableIsSorted, 
			                 lsd.GlobalColourTableIsSorted, 
			                 "GlobalColourTableIsSorted" );
		}
		#endregion
		
		#region CheckGlobalColourTable method
		internal static void CheckGlobalColourTable( ColourTable gct )
		{
			Assert.AreEqual( GlobalColourTable.Colours.Length, 
			                 gct.Colours.Length );
			Assert.AreEqual( GlobalColourTable.Length, gct.Length );
			Assert.AreEqual( GlobalColourTable.Colours.Length, gct.Length );
			Assert.AreEqual( GlobalColourTable.Length, gct.Colours.Length );
			
			for( int i = 0; i < gct.Colours.Length; i++ )
			{
				Assert.AreEqual( GlobalColourTable.Colours[i], 
				                 gct.Colours[i], 
				                 "colour index: " + i );
			}
		}
		#endregion
		
		#region CheckGraphicControlExtension method
		internal static void CheckGraphicControlExtension( GraphicControlExtension ext )
		{
			Assert.AreEqual( GraphicControlExtension.ExpectedBlockSize, 
			                 ext.BlockSize, 
			                 "ExpectedBlockSize" );
			Assert.AreEqual( DelayTime, ext.DelayTime, "DelayTime" );
			Assert.AreEqual( DisposalMethod, ext.DisposalMethod, "DisposalMethod" );
			Assert.AreEqual( TransparentColourIndex, 
			                 ext.TransparentColourIndex,
			                 "TransparentColourIndex" );
			Assert.AreEqual( HasTransparentColour, ext.HasTransparentColour, "HasTransparentColour" );
			Assert.AreEqual( ExpectsUserInput, ext.ExpectsUserInput, "ExpectsUserInput" );
		}
		#endregion
		
		#region CheckImageDescriptor method
		internal static void CheckImageDescriptor( ImageDescriptor id )
		{
			Assert.AreEqual( HasLocalColourTable, id.HasLocalColourTable, 
			                 "HasLocalColourTable" );
			Assert.AreEqual( IsInterlaced, id.IsInterlaced, "IsInterlaced" );
			Assert.AreEqual( LocalColourTableIsSorted, id.IsSorted, 
			                 "LocalColourTableIsSorted" );
			Assert.AreEqual( Math.Pow( 2, LocalColourTableSizeBits + 1 ), 
			                 id.LocalColourTableSize, "LocalColourTableSize" );
			Assert.AreEqual( FramePosition, id.Position, "Position" );
			Assert.AreEqual( FrameSize, id.Size, "Size" );
		}
		#endregion
		
		#region CheckImageData method
		internal static void CheckImageData( TableBasedImageData imageData )
		{
			TableBasedImageData expectedData = ImageData;
			Assert.AreEqual( expectedData.ClearCode, 
			                 imageData.ClearCode );
			Assert.AreEqual( expectedData.DataBlocks.Length, 
			                 imageData.DataBlocks.Length );
			Assert.AreEqual( expectedData.EndOfInformation, 
			                 imageData.EndOfInformation );
			Assert.AreEqual( expectedData.InitialCodeSize, 
			                 imageData.InitialCodeSize );
			Assert.AreEqual( expectedData.LzwMinimumCodeSize, 
			                 imageData.LzwMinimumCodeSize );
			Assert.AreEqual( expectedData.Pixels.Count, 
			                 imageData.Pixels.Count );
			
			string info;
			
			for( int i = 0; i < expectedData.Pixels.Count; i++ )
			{
				info = "Pixel index: " + i;
				Assert.AreEqual( expectedData.Pixels[i], 
				                 imageData.Pixels[i], 
				                 info );
			}
			
			for( int i = 0; i < expectedData.DataBlocks.Length; i++ )
			{
				info = "Data block number: " + i;
				DataBlock expectedBlock = expectedData.DataBlocks[i];
				DataBlock actualBlock = imageData.DataBlocks[i];
				Assert.AreEqual( expectedBlock.ActualBlockSize, 
				                 actualBlock.ActualBlockSize, 
				                 info );
				Assert.AreEqual( expectedBlock.DeclaredBlockSize, 
				                 actualBlock.DeclaredBlockSize, 
				                 info );
				
				for( int b = 0; b < expectedBlock.ActualBlockSize; b++ )
				{
					Assert.AreEqual( expectedBlock.Data[i], 
					                 actualBlock.Data[i], 
					                 info + ". Block index " + b );
				}
			}
			
		}
		#endregion
		
		#region CheckBitmap method
		internal static void CheckBitmap( Bitmap bitmap )
		{
			BitmapAssert.AreEqual( ExpectedBitmap, bitmap, "" );
		}
		#endregion
		
		#endregion
		
		#region other private methods
		
		#region WriteShort method
		/// <summary>
		/// Writes the supplied ushort value to the supplied stream, least 
		/// significant bit first.
		/// </summary>
		/// <param name="outputStream"></param>
		/// <param name="twoBytes"></param>
		/// <remarks>
		/// Values larger than ushort.MaxValue will simply be truncated, no
		/// exception will be thrown.
		/// </remarks>
		private static void WriteUShort( Stream outputStream, int twoBytes )
		{
			byte leastSignificantByte = (byte) (twoBytes & 0xff);
			byte mostSignificantByte = (byte) ( (twoBytes & 0xff00) << 8 );
			outputStream.WriteByte( leastSignificantByte );
			outputStream.WriteByte( mostSignificantByte );
		}
		#endregion
		
		#endregion
	}
}

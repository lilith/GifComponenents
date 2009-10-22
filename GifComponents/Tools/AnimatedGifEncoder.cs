#region Copyright (C) Simon Bridewell, Kevin Weiner, Phil Garcia
// 
// This file is part of the GifComponents library.
// GifComponents is free software; you can redistribute it and/or
// modify it under the terms of the Code Project Open License.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// Code Project Open License for more details.
// 
// You can read the full text of the Code Project Open License at:
// http://www.codeproject.com/info/cpol10.aspx
//
// GifComponents is a derived work based on NGif written by gOODiDEA.NET
// and published at http://www.codeproject.com/KB/GDI-plus/NGif.aspx,
// with an enhancement by Phil Garcia published at
// http://www.thinkedge.com/blogengine/post/2008/02/20/Animated-GIF-Encoder-for-NET-Update.aspx
//
// Simon Bridewell makes no claim to be the original author of this library,
// only to have created a derived work.
#endregion

using System;
using System.Drawing;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace GifComponents
{
	/// <summary>
	/// TODO: remove support for transparency until it's understood better?
	/// Class AnimatedGifEncoder - Encodes a GIF file consisting of one or
	/// more frames.
	/// Instantiate the encoder using the constructor, call the AddFrame to add
	/// as many GifFrames as desired, then call the WriteToStream or WriteToFile
	/// method to create the animation.
	/// 
	/// No copyright asserted on the source code of this class.  May be used
	/// for any purpose, however, refer to the Unisys LZW patent for restrictions
	/// on use of the associated LZWEncoder class.  Please forward any corrections
	/// to kweiner@fmsware.com.
	/// 
	/// @author Kevin Weiner, FM Software
	/// @version 1.03 November 2003
	/// 
	/// Modified by Phil Garcia (phil@thinkedge.com) 
	///		1. Add support to output the Gif to a MemoryStream (9/2/2005)
	/// 
	/// Modified by Simon Bridewell, June-August 2009:
	/// Downloaded from 
	/// http://www.thinkedge.com/BlogEngine/file.axd?file=NGif_src2.zip
	/// 	* Corrected FxCop code analysis errors.
	/// 	* Documentation comments converted to .net XML comments.
	/// 	* Refactored so that all properties are set in the constructor.
	/// 	* Writing of GIF components to the output stream delegated to the 
	/// 	  classes for those components.
	/// 	* Added option to use a global colour table instead of local colour tables.
	/// 	* Added support for colour tables with fewer than 256 colours
	/// 	* Colour quantization only performed for animations with more than 
	/// 	  256 colours.
	/// </summary>
	public class AnimatedGifEncoder : GifComponent
	{
		#region declarations
		/// <summary>
		/// The ColourTableStrategy indicating whether a global colour table
		/// or local colour tables should be used.
		/// </summary>
		private ColourTableStrategy _strategy;
		
		/// <summary>
		/// The images which make up the frames of the animation.
		/// </summary>
		private Collection<Image> _frameImages;
		
		/// <summary>
		/// The delay in hundredths of a second between showing one frame and 
		/// the next.
		/// </summary>
		private Collection<int> _frameDelays;
		
		/// <summary>
		/// Collection of flags indicating whether each frame expects the user 
		/// to press a key before the animation continues.
		/// </summary>
		private Collection<bool> _framesExpectUserInput;
		
		/// <summary>
		/// Collection of the positions of the frames within the logical screen.
		/// </summary>
		private Collection<Point> _framePositions;
		
		/// <summary>
		/// Size, in pixels, of the animated GIF file.
		/// </summary>
		private Size _logicalScreenSize;
		
		private Color _transparent = Color.Empty; // transparent color if given

		/// <summary>
		/// The number of times to repeat the animation.
		/// 0 to repeat indefinitely.
		/// -1 to not repeat.
		/// </summary>
		private int _repeatCount;
		
		/// <summary>
		/// All the pixels from all the frames which make up the image.
		/// Used only when the ColourTableStrategy is UseGlobal.
		/// </summary>
		private Collection<byte> _pixels;
		
		private bool[] _usedEntry = new bool[256]; // active palette entries
		
		/// <summary>
		/// Quality of color quantization (conversion of images to the maximum 
		/// 256 colors allowed by the GIF specification).
		/// Lower values (minimum = 1) produce better colors, but slow 
		/// processing significantly.
		/// 10 is the default, and produces good color mapping at reasonable 
		/// speeds.
		/// Values greater than 20 do not yield significant improvements in 
		/// speed.
		/// </summary>
		private int _quality;
		
		#endregion

		#region default constructor
		/// <summary>
		/// Default constructor.
		/// Sets repeat count to 0 (repeat indefinitely)
		/// Sets colour table strategy to UseGlobal
		/// Sets image quantization quality to 10.
		/// Screen size defaults to size of first frame.
		/// </summary>
		public AnimatedGifEncoder()
		{
			_strategy = ColourTableStrategy.UseGlobal;
			_quality = 10;
			_logicalScreenSize = Size.Empty;
			_framesExpectUserInput = new Collection<bool>();
			_framePositions = new Collection<Point>();
			_frameImages = new Collection<Image>();
			_frameDelays = new Collection<int>();
		}
		#endregion
		
		#region constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="screenSize">
		/// The size, in pixels, of the animated GIF file.
		/// If this parameter is Size.Empty, the size of the first frame to be 
		/// added will be used.
		/// </param>
		/// <param name="repeatCount">
		/// The number of times to repeat the animation.
		/// 0 to repeat indefinitely.
		/// -1 to not repeat.
		/// Defaults to -1 if less than -1.
		/// </param>
		/// <param name="strategy">
		/// The colour table strategy to use when encoding this file.
		/// Either global colour tables are to be used, or local colour tables
		/// are to be used.
		/// </param>
		/// <param name="quality">
		/// Sets quality of color quantization (conversion of images
		/// to the maximum 256 colors allowed by the GIF specification).
		/// Lower values (minimum = 1) produce better colors, but slow
		/// processing significantly.  10 is the default, and produces
		/// good color mapping at reasonable speeds.  Values greater
		/// than 20 do not yield significant improvements in speed.
		/// Defaults to 1 if less than 1.
		/// </param>
		public AnimatedGifEncoder( Size screenSize, 
		                           int repeatCount, 
		                           ColourTableStrategy strategy, 
		                           int quality )
		{
			_logicalScreenSize = screenSize;
			if( repeatCount < -1 )
			{
				_repeatCount = -1;
			}
			else
			{
				_repeatCount = repeatCount;
			}
			_strategy = strategy;
			if( quality < 1 )
			{
				_quality = 1;
			}
			else
			{
				_quality = quality;
			}
			_framesExpectUserInput = new Collection<bool>();
			_framePositions = new Collection<Point>();
			_frameImages = new Collection<Image>();
			_frameDelays = new Collection<int>();
			_pixels = new Collection<byte>();
		}
		#endregion
		
		#region WriteToFile method
		/// <summary>
		/// Writes an animated GIF file to the supplied file name.
		/// </summary>
		/// <param name="fileName">
		/// The file to write the animation to.
		/// </param>
		public void WriteToFile( string fileName )
		{
			Stream outputStream = File.Create( fileName );
			WriteToStream( outputStream );
			outputStream.Close();
		}
		#endregion
		
		#region WriteToStream method
		/// <summary>
		/// Writes the GIF animation to the supplied stream.
		/// </summary>
		/// <param name="outputStream">
		/// The stream to write the animation to.
		/// </param>
		public override void WriteToStream( Stream outputStream )
		{
			GifHeader header = new GifHeader( "GIF", "89a" );
			header.WriteToStream( outputStream );
			
			PixelAnalysis analysis = null;
			ColourTable gct = null; // global colour table
			if( _strategy == ColourTableStrategy.UseGlobal )
			{
				// Analyze the pixels in all the images to build the
				// global colour table.
				Collection<Image> images = new Collection<Image>();
				foreach( Image thisImage in _frameImages )
				{
					images.Add( thisImage );
				}
				analysis = new PixelAnalysis( images, _quality );
				gct = analysis.ColourTable;
				WriteLogicalScreenDescriptor( _logicalScreenSize, 
				                              _strategy,
				                              gct.SizeBits,
				                              outputStream );
				gct.WriteToStream( outputStream );
			}
			else
			{
				WriteLogicalScreenDescriptor( _logicalScreenSize, 
				                              _strategy,
				                              7,
				                              outputStream );
			}
			
			// Repeat count -1 means don't repeat the animation, so don't add
			// a Netscape extension.
			if( _repeatCount >= 0 ) 
			{
				// use NS app extension to indicate repeating animation
				WriteNetscapeExt( _repeatCount, outputStream );
			}
			
			for( int i = 0; i < _frameImages.Count; i++ )
			{
				Image thisImage = _frameImages[i];
				ColourTable act; // active colour table
				ColourTable lct = null; // local colour table
				if( _strategy == ColourTableStrategy.UseLocal )
				{
					analysis = new PixelAnalysis( thisImage, _quality );
					lct = analysis.ColourTable;
					// make local colour table active
					act = lct;
				}
				else
				{
					// make global colour table active
					act = gct;
				}
				int transparentColourIndex;
				if( _transparent == Color.Empty )
				{
					transparentColourIndex = 0;
				}
				else
				{
					transparentColourIndex = FindClosest( _transparent, act );
				}
				WriteGraphicCtrlExt( _frameDelays[i],
				                     transparentColourIndex,
				                     _framesExpectUserInput[i],
				                     outputStream );
				WriteImageDescriptor( thisImage.Size, 
				                      _framePositions[i],
				                      lct,
				                      outputStream );
				
				// Write a local colour table if the strategy is to do so
				if( _strategy == ColourTableStrategy.UseLocal )
				{
					lct.WriteToStream( outputStream );
					WritePixels( analysis.IndexedPixels, 
					             outputStream ); // encode and write pixel data
				}
				else
				{
					WritePixels( analysis.IndexedPixelsCollection[i],
					             outputStream ); // encode and write pixel data
				}
			}
			
			// GIF trailer
			WriteByte( CodeTrailer, outputStream );
		}
		#endregion
		
		#region Transparent property
		/// <summary>
		/// Gets and sets the transparent color for the next added frame and 
		/// any subsequent frames.
		/// Since all colors are subject to modification in the quantization 
		/// process, the color in the final palette for each frame closest to 
		/// the given color becomes the transparent color for that frame.
		/// May be set to Color.Empty to indicate no transparent color.
		/// </summary>
		public Color Transparent
		{
			get{ return _transparent; }
			set{ _transparent = value; }
		}
		#endregion

		#region AddFrame methods
		
		#region AddFrame( Image, int ) method
		/// <summary>
		/// Adds a frame to the GIF animation.
		/// </summary>
		/// <param name="imageToAdd">
		/// The image to be added to the GIF animation.
		/// </param>
		/// <param name="delay">
		/// The delay in hundredths of a second between displaying this frame 
		/// and displaying the next frame.
		/// </param>
		public void AddFrame( Image imageToAdd, int delay )
		{
			AddFrame( imageToAdd, delay, false );
		}
		#endregion

		#region AddFrame( Image, int, bool ) method
		/// <summary>
		/// Adds a frame to the GIF animation.
		/// </summary>
		/// <param name="imageToAdd">
		/// The image to be added to the GIF animation.
		/// </param>
		/// <param name="delay">
		/// The delay in hundredths of a second between displaying this frame 
		/// and displaying the next frame.
		/// </param>
		/// <param name="expectsUserInput">
		/// A flag indicating whether user interaction is required before the
		/// next frame in the animation is displayed.
		/// </param>
		public void AddFrame( Image imageToAdd, int delay, bool expectsUserInput )
		{
			AddFrame( imageToAdd, delay, expectsUserInput, new Point( 0, 0 ) );
		}
		#endregion
		
		#region AddFrame( Image, int, Point ) method
		/// <summary>
		/// Adds a frame to the GIF animation.
		/// </summary>
		/// <param name="imageToAdd">
		/// The image to be added to the GIF animation.
		/// </param>
		/// <param name="delay">
		/// The delay in hundredths of a second between displaying this frame 
		/// and displaying the next frame.
		/// </param>
		/// <param name="position">
		/// A System.Drawing.Point representing the top-left position of the 
		/// supplied image within the logical screen.
		/// </param>
		public void AddFrame( Image imageToAdd, int delay, Point position )
		{
			AddFrame( imageToAdd, delay, false, position );
		}
		#endregion
		
		#region AddFrame( Image, int, bool, Point ) method
		/// <summary>
		/// Adds a frame to the GIF animation.
		/// </summary>
		/// <param name="imageToAdd">
		/// The image to be added to the GIF animation.
		/// </param>
		/// <param name="delay">
		/// The delay in hundredths of a second between displaying this frame 
		/// and displaying the next frame.
		/// </param>
		/// <param name="expectsUserInput">
		/// A flag indicating whether user interaction is required before the
		/// next frame in the animation is displayed.
		/// </param>
		/// <param name="position">
		/// A System.Drawing.Point representing the top-left position of the 
		/// supplied image within the logical screen.
		/// </param>
		public void AddFrame( Image imageToAdd, int delay, bool expectsUserInput, Point position )
		{
			if( _logicalScreenSize == Size.Empty )
			{
				// use first frame's size if logical screen size hasn't been set
				_logicalScreenSize = imageToAdd.Size;
			}
			
			_frameImages.Add( imageToAdd );
			_frameDelays.Add( delay );
			_framesExpectUserInput.Add( expectsUserInput );
			_framePositions.Add( position );
		}
		#endregion

		#endregion
	
		#region protected / private methods
	
		#region private static FindClosest method
		/// <summary>
		/// Returns the index within the supplied colour table of the colour 
		/// closest to the supplied colour.
		/// </summary>
		/// <param name="colourToFind">
		/// The colour to find the closest match for.
		/// </param>
		/// <param name="colourTable">
		/// The active colour table.
		/// </param>
		/// <returns>
		/// Returns -1 if the supplied colour is null.
		/// </returns>
		private static int FindClosest( Color colourToFind, 
		                                ColourTable colourTable )
		{
			if( colourTable == null )
			{
				return -1;
			}
			int r = colourToFind.R;
			int g = colourToFind.G;
			int b = colourToFind.B;
			int minpos = 0;
			int dmin = 256 * 256 * 256;
			int len = colourTable.Length;
			for( int i = 0; i < len; i++ ) 
			{
				int dr = r - colourTable[i].R;
				int dg = g - colourTable[i].G;
				int db = b - colourTable[i].B;
				int d = dr * dr + dg * dg + db * db;
				if( d < dmin )
				{
					dmin = d;
					minpos = i;
				}
			}
			return minpos;
		}
		#endregion
	
		#region private WriteGraphicCtrlExt method
		/// <summary>
		/// Writes a Graphic Control Extension to the supplied output stream.
		/// </summary>
		/// <param name="delay">
		/// The delay between showing this frame and the next.
		/// </param>
		/// <param name="transparentColourIndex">
		/// The index within the active colour table of the transparent colour.
		/// </param>
		/// <param name="expectsUserInput">
		/// A flag indicating whether user interaction is required before the
		/// next frame in the animation is displayed.
		/// </param>
		/// <param name="outputStream">
		/// The stream to write to.
		/// </param>
		private void WriteGraphicCtrlExt( int delay,
		                                  int transparentColourIndex, 
		                                  bool expectsUserInput,
		                                  Stream outputStream )
		{
			outputStream.WriteByte( GifComponent.CodeExtensionIntroducer );
			outputStream.WriteByte( GifComponent.CodeGraphicControlLabel );
			
			// The frame doesn't have have a graphic control extension yet, so we
			// need to work out what it would contain.
			DisposalMethod disposalMethod;
			bool hasTransparentColour;
			if( _transparent == Color.Empty ) // TODO: remove reference to _transparent - parameterise?
			{
				hasTransparentColour = false;
				disposalMethod = DisposalMethod.NotSpecified; // dispose = no action
			} 
			else 
			{
				hasTransparentColour = true;
				disposalMethod = DisposalMethod.RestoreToBackgroundColour; // force clear if using transparent color
			}
			int blockSize = 4;
			GraphicControlExtension gce
				= new GraphicControlExtension( blockSize, 
				                               disposalMethod, 
				                               expectsUserInput, 
				                               hasTransparentColour, 
				                               delay, 
				                               transparentColourIndex );
			gce.WriteToStream( outputStream );
		}
		#endregion
	
		#region private static WriteImageDescriptor method
		/// <summary>
		/// Writes an image descriptor to the supplied stream.
		/// </summary>
		/// <param name="imageSize">
		/// The size, in pixels, of the image in this frame.
		/// </param>
		/// <param name="position">
		/// The position of this image within the logical screen.
		/// </param>
		/// <param name="localColourTable">
		/// The local colour table for this frame.
		/// Supply null if the global colour table is to be used for this frame.
		/// </param>
		/// <param name="outputStream">
		/// The stream to write to.
		/// </param>
		private static void WriteImageDescriptor( Size imageSize, 
		                                          Point position,
		                                          ColourTable localColourTable,
		                                          Stream outputStream )
		{
			bool hasLocalColourTable;
			int localColourTableSize;
			if( localColourTable == null )
			{
				hasLocalColourTable = false;
				localColourTableSize = 0;
			}
			else
			{
				hasLocalColourTable = true;
				localColourTableSize = localColourTable.SizeBits;
			}
			bool isInterlaced = false; // encoding of interlaced images not currently supported
			bool localColourTableIsSorted = false; // sorting of colour tables not currently supported
			ImageDescriptor id = new ImageDescriptor( position, 
			                                          imageSize, 
			                                          hasLocalColourTable, 
			                                          isInterlaced, 
			                                          localColourTableIsSorted, 
			                                          localColourTableSize );
			outputStream.WriteByte( GifComponent.CodeImageSeparator );
			id.WriteToStream( outputStream );
		}
		#endregion
	
		#region private static WriteLogicalScreenDescriptor method
		/// <summary>
		/// Writes a Logical Screen Descriptor to the supplied stream.
		/// </summary>
		/// <param name="screenSize">
		/// The size, in pixels, of the logical screen on which the animation
		/// will be displayed.
		/// </param>
		/// <param name="strategy">
		/// Indicates whether the stream has a global colour table.
		/// </param>
		/// <param name="globalColourTableSizeBits">
		/// The number of bits required to hold the size of the global colour
		/// table, minus 1.
		/// </param>
		/// <param name="outputStream">
		/// The stream to write to.
		/// </param>
		private static void WriteLogicalScreenDescriptor( Size screenSize, 
		                                                  ColourTableStrategy strategy,
		                                                  int globalColourTableSizeBits,
		                                                  Stream outputStream )
		{
			bool hasGlobalColourTable = ( strategy == ColourTableStrategy.UseGlobal );
			int colourResolution = 7; // TODO: parameterise colourResolution?
			bool globalColourTableIsSorted = false; // Sorting of colour tables is not currently supported
			int backgroundColorIndex = 0; // TODO: parameterise backgroundColourIndex?
			int pixelAspectRatio = 0; // TODO: parameterise pixelAspectRatio?
			LogicalScreenDescriptor lsd = 
				new LogicalScreenDescriptor( screenSize, 
				                             hasGlobalColourTable, 
				                             colourResolution, 
				                             globalColourTableIsSorted, 
				                             globalColourTableSizeBits, 
				                             backgroundColorIndex, 
				                             pixelAspectRatio );
			
			lsd.WriteToStream( outputStream );
		}
		#endregion
	
		#region private static WriteNetscapeExt method
		/// <summary>
		/// Writes a Netscape application extension defining the repeat count
		/// to the supplied output stream.
		/// </summary>
		/// <param name="repeatCount">
		/// The number of times to repeat the animation. 0 to repeat 
		/// indefinitely.
		/// </param>
		/// <param name="outputStream">
		/// The stream to write to.
		/// </param>
		private static void WriteNetscapeExt( int repeatCount, Stream outputStream )
		{
			outputStream.WriteByte( GifComponent.CodeExtensionIntroducer );
			outputStream.WriteByte( GifComponent.CodeApplicationExtensionLabel );
			NetscapeExtension ne = new NetscapeExtension( repeatCount );
			ne.WriteToStream( outputStream );
		}
		#endregion
	
		#region private static WritePixels method
		/// <summary>
		/// Encodes and writes pixel data to the supplied stream
		/// </summary>
		/// <param name="indexedPixels">
		/// Collection of indices of the pixel colours within the active colour 
		/// table.
		/// </param>
		/// <param name="outputStream">
		/// The stream to write to.
		/// </param>
		private static void WritePixels( IndexedPixels indexedPixels,
		                                 Stream outputStream )
		{
			LzwEncoder encoder = new LzwEncoder( indexedPixels );
			encoder.Encode( outputStream );
		}
		#endregion

		#endregion
	}

}
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
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;

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
		/// The frames which make up the animation/
		/// </summary>
		private Collection<GifFrame> _frames;
		
		/// <summary>
		/// The ColourTableStrategy indicating whether a global colour table
		/// or local colour tables should be used.
		/// </summary>
		private ColourTableStrategy _strategy;
		
		/// <summary>
		/// The global colour table, if used.
		/// </summary>
		private ColourTable _globalColourTable;
		
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
		
		/// <summary>
		/// Indicates the type of quantizer used to reduce the colour palette
		/// to 256 colours.
		/// </summary>
		private QuantizerType _quantizerType;
		
		private string _status;
		private int _processingFrame;
		private PixelAnalysis _pixelAnalysis;
		#endregion

		#region default constructor
		/// <summary>
		/// Default constructor.
		/// Sets repeat count to 0 (repeat indefinitely)
		/// Sets colour table strategy to UseGlobal
		/// Sets image quantization quality to 10.
		/// Sets quantizer type to NeuQuant.
		/// Screen size defaults to size of first frame.
		/// </summary>
		public AnimatedGifEncoder()
		{
			_frames = new Collection<GifFrame>();
			_strategy = ColourTableStrategy.UseGlobal;
			_quality = 10;
			_quantizerType = QuantizerType.NeuQuant;
			_logicalScreenSize = Size.Empty;
		}
		#endregion
		
		#region properties
		
		#region Frames property
		/// <summary>
		/// Gets a collection of the GifFrames which make up the animation.
		/// </summary>
		public Collection<GifFrame> Frames
		{
			get { return _frames; }
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

		#region ColourTableStrategy property
		/// <summary>
		/// Indicates whether the animation will contain a single global colour
		/// table for all frames (UseGlobal) or a local colour table for each
		/// frame (UseLocal)
		/// </summary>
		[Description( "Indicates whether the animation will contain a single " +
		              "global colour table for all frames (UseGlobal) or a " +
		              "local colour table for each frame (UseLocal)" )]
		public ColourTableStrategy ColourTableStrategy
		{
			get { return _strategy; }
			set { _strategy = value; }
		}
		#endregion
		
		#region RepeatCount property
		/// <summary>
		/// The number of times to repeat the animation.
		/// 0 to repeat indefinitely.
		/// -1 to not repeat.
		/// Defaults to -1 if less than -1.
		/// </summary>
		[Description( "The number of times to repeat the animation. 0 to " +
		              "repeat indefinitely. -1 to not repeat. Defaults to -1 " +
		              "if less than -1." )]
		public int RepeatCount
		{
			get { return _repeatCount; }
			set
			{
				if( value < -1 )
				{
					_repeatCount = -1;
				}
				else
				{
					_repeatCount = value;
				}
			}
		}
		#endregion
		
		#region ColourQuality property
		/// <summary>
		/// Sets quality of color quantization (conversion of images
		/// to the maximum 256 colors allowed by the GIF specification).
		/// Lower values (minimum = 1) produce better colors, but slow
		/// processing significantly.  10 is the default, and produces
		/// good color mapping at reasonable speeds.  Values greater
		/// than 20 do not yield significant improvements in speed.
		/// Defaults to 1 if less than 1.
		/// </summary>
		[Description( "Sets quality of color quantization (conversion of " +
		              "images to the maximum 256 colors allowed by the GIF " +
		              "specification). Lower values (minimum = 1) produce " +
		              "better colors, but slow processing significantly. " +
		              "10 is the default, and produces good color mapping " +
		              "at reasonable speeds. Values greater than 20 do not " +
		              "yield significant improvements in speed. Defaults to " +
		              "1 if less than 1." )]
		public int ColourQuality
		{
			get { return _quality; }
			set
			{
				if( value < 1 )
				{
					_quality = 1;
				}
				else
				{
					_quality = value;
				}
			}
		}
		#endregion
		
		#region LogicalScreenSize property
		/// <summary>
		/// The size, in pixels, of the animated GIF file.
		/// If this property is not set before the animation is encoder, the 
		/// size of the first frame to be added will be used.
		/// </summary>
		[Description( "The size, in pixels, of the animated GIF file. If " +
		              "this property is not set before the animation is " +
		              "encoder, the size of the first frame to be added " +
		              "will be used." )]
		public Size LogicalScreenSize
		{
			get { return _logicalScreenSize; }
			set { _logicalScreenSize = value; }
		}
		#endregion
		
		#region QuantizerType property
		/// <summary>
		/// Gets and sets the type of quantizer used to reduce the colour
		/// palette to 256 colours, if required.
		/// </summary>
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
		                 MessageId = "Quantizer")]
		public QuantizerType QuantizerType
		{
			get { return _quantizerType; }
			set { _quantizerType = value; }
		}
		#endregion
		
		#region Status property
		/// <summary>
		/// Gets a string indicating the current status of the encoder.
		/// For use while the WriteToFile or WriteToStream method is running,
		/// to indicate how far through the process it is.
		/// </summary>
		[Browsable( false )]
		public string Status
		{
			get { return _status; }
		}
		#endregion
		
		#region PixelAnalysisStatus property
		/// <summary>
		/// Gets the status of the PixelAnalysis used to calculate colour tables.
		/// </summary>
		[Browsable( false )]
		public string PixelAnalysisStatus
		{
			get
			{
				if( _pixelAnalysis == null )
				{
					return string.Empty;
				}
				else
				{
					return _pixelAnalysis.Status;
				}
			}
		}
		#endregion
		
		#region ProcessingFrame property
		/// <summary>
		/// Gets the frame number currently being processed by the encoder.
		/// For use while the WriteToFile or WriteToStream method is running,
		/// to indicate how far through the process it is.
		/// </summary>
		[Browsable( false )]
		public int ProcessingFrame
		{
			get { return _processingFrame; }
		}
		#endregion
		
		#region PixelAnalysisProcessingFrame property
		/// <summary>
		/// Gets the frame number being analysed by the PixelAnalysis.
		/// </summary>
		[Browsable( false )]
		public int PixelAnalysisProcessingFrame
		{
			get
			{
				if( _pixelAnalysis == null )
				{
					return 0;
				}
				else
				{
					return _pixelAnalysis.ProcessingFrame;
				}
			}
		}
		#endregion
		
		#endregion

		#region methods
		
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
		/// TODO: this method is too long - break it down into smaller bits
		/// </summary>
		/// <param name="outputStream">
		/// The stream to write the animation to.
		/// </param>
		public override void WriteToStream( Stream outputStream )
		{
			if( _frames.Count == 0 )
			{
				string message
					= "The AnimatedGifEncoder has no frames to write!";
				throw new InvalidOperationException( message );
			}
			
			if( _logicalScreenSize == Size.Empty )
			{
				// use first frame's size if logical screen size hasn't been set
				_logicalScreenSize = _frames[0].TheImage.Size;
			}
			
			_processingFrame = 0;

			WriteGifHeader( outputStream );
			
			WriteLogicalScreenDescriptor( outputStream );
			
			WriteNetscapeExtension( outputStream );
			
			#region looping through the frames
			for( int i = 0; i < _frames.Count; i++ )
			{
				_processingFrame = i + 1;
				_status = "Frame " + _processingFrame + " of " + _frames.Count;
				GifFrame thisFrame = _frames[i];
				Image thisImage = thisFrame.TheImage;
				ColourTable act; // active colour table
				ColourTable lct = null; // local colour table
				if( _strategy == ColourTableStrategy.UseLocal )
				{
					_status 
						= "Frame " + _processingFrame 
						+ " of " + _frames.Count 
						+ ": building local colour table - analysing pixels";
					_pixelAnalysis = new PixelAnalysis( thisImage, 
					                                    _quality, 
					                                    _quantizerType );
					lct = _pixelAnalysis.ColourTable;
					// make local colour table active
					act = lct;
				}
				else
				{
					// make global colour table active
					act = _globalColourTable;
				}
				int transparentColourIndex;
				if( _transparent == Color.Empty )
				{
					transparentColourIndex = 0;
				}
				else
				{
					_status 
						= "Frame " + _processingFrame 
						+ " of " + _frames.Count 
						+ ": finding closest to transparent colour";
					// TODO: test case for this once transparency is understood
					transparentColourIndex = FindClosest( _transparent, act );
				}
				_status 
					= "Frame " + _processingFrame 
					+ " of " + _frames.Count 
					+ ": writing graphic control extension";
				WriteGraphicCtrlExt( thisFrame, 
				                     transparentColourIndex, 
				                     outputStream );
				_status 
					= "Frame " + _processingFrame 
					+ " of " + _frames.Count 
					+ ": writing image descriptor";
				WriteImageDescriptor( thisImage.Size, 
				                      _frames[i].Position, 
				                      lct, 
				                      outputStream );
				
				// Write a local colour table if the strategy is to do so
				if( _strategy == ColourTableStrategy.UseLocal )
				{
					_status 
						= "Frame " + _processingFrame 
						+ " of " + _frames.Count 
						+ ": writing local colour table";
					lct.WriteToStream( outputStream );
					_status 
						= "Frame " + _processingFrame 
						+ " of " + _frames.Count 
						+ ": encoding pixel data";
					// FIXME: null reference exception when using QuantizerType.UseSuppliedPalette
					WritePixels( _pixelAnalysis.IndexedPixels, 
					             outputStream ); // encode and write pixel data
				}
				else
				{
					_status 
						= "Frame " + _processingFrame 
						+ " of " + _frames.Count 
						+ ": encoding pixel data";
					WritePixels( _pixelAnalysis.IndexedPixelsCollection[i],
					             outputStream ); // encode and write pixel data
				}
			}
			#endregion
			
			// GIF trailer
			_status = "Writing GIF trailer";
			WriteByte( CodeTrailer, outputStream );
			_status = "Done";
		}
		#endregion
		
		#region AddFrame( GifFrame ) method
		/// <summary>
		/// Adds a frame to the animation.
		/// </summary>
		/// <param name="frame">
		/// The frame to add to the animation.
		/// </param>
		public void AddFrame( GifFrame frame )
		{
			_frames.Add( frame );
		}
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

		#region private WriteGifHeader method
		private void WriteGifHeader( Stream outputStream )
		{
			_status = "Writing GIF header";
			GifHeader header = new GifHeader( "GIF", "89a" );
			header.WriteToStream( outputStream );
			_status = "Done writing GIF header";
		}
		#endregion
		
		#region private WriteGraphicCtrlExt method
		/// <summary>
		/// Writes a Graphic Control Extension to the supplied output stream.
		/// </summary>
		/// <param name="frame">
		/// The GifFrame to which this graphic control extension relates.
		/// </param>
		/// <param name="transparentColourIndex">
		/// The index within the active colour table of the transparent colour.
		/// </param>
		/// <param name="outputStream">
		/// The stream to write to.
		/// </param>
		private void WriteGraphicCtrlExt( GifFrame frame,
		                                  int transparentColourIndex, 
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
				// TODO: test case for this once transparency is better understood
				hasTransparentColour = true;
				disposalMethod = DisposalMethod.RestoreToBackgroundColour; // force clear if using transparent color
			}
			int blockSize = 4;
			GraphicControlExtension gce
				= new GraphicControlExtension( blockSize, 
				                               disposalMethod, 
				                               frame.ExpectsUserInput, 
				                               hasTransparentColour, 
				                               frame.Delay, 
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
	
		#region private WriteLogicalScreenDescriptor method
		/// <summary>
		/// Writes a Logical Screen Descriptor to the supplied stream.
		/// Also writes a global colour table if required.
		/// </summary>
		/// <param name="outputStream">
		/// The stream to write to.
		/// </param>
		private void WriteLogicalScreenDescriptor( Stream outputStream )
		{
			bool hasGlobalColourTable = ( _strategy == ColourTableStrategy.UseGlobal );
			int colourResolution = 7; // TODO: parameterise colourResolution?
			bool globalColourTableIsSorted = false; // Sorting of colour tables is not currently supported
			int backgroundColorIndex = 0; // TODO: parameterise backgroundColourIndex?
			int pixelAspectRatio = 0; // TODO: parameterise pixelAspectRatio?
			if( _strategy == ColourTableStrategy.UseGlobal )
			{
				// Analyse the pixels in all the images to build the
				// global colour table.
				Collection<Image> images = new Collection<Image>();
				foreach( GifFrame thisFrame in _frames )
				{
					Image thisImage = thisFrame.TheImage;
					images.Add( thisImage );
				}
				_status = "Building global colour table - analysing pixels";
				_pixelAnalysis = new PixelAnalysis( images, _quality );
				_globalColourTable = _pixelAnalysis.ColourTable;
				_status = "Writing logical screen descriptor (global)";
				LogicalScreenDescriptor lsd = 
					new LogicalScreenDescriptor( _logicalScreenSize, 
					                             hasGlobalColourTable, 
					                             colourResolution, 
					                             globalColourTableIsSorted, 
					                             _globalColourTable.SizeBits,
					                             backgroundColorIndex, 
					                             pixelAspectRatio );
				lsd.WriteToStream( outputStream );
				_status = "Writing global colour table";
				_globalColourTable.WriteToStream( outputStream );
			}
			else
			{
				_status = "Writing logical screen descriptor (local)";
				LogicalScreenDescriptor lsd = 
					new LogicalScreenDescriptor( _logicalScreenSize, 
					                             hasGlobalColourTable, 
					                             colourResolution, 
					                             globalColourTableIsSorted, 
				// TODO: global colour table size for a UseLocal GIF is irrelevant - pass 7 instead?
					                             7, 
					                             backgroundColorIndex, 
					                             pixelAspectRatio );
				lsd.WriteToStream( outputStream );
			}
		}
		#endregion
	
		#region private WriteNetscapeExtension method
		/// <summary>
		/// Writes a Netscape application extension defining the repeat count
		/// to the supplied output stream, if the repeat count is greater than
		/// or equal to zero.
		/// </summary>
		/// <param name="outputStream">
		/// The stream to write to.
		/// </param>
		private void WriteNetscapeExtension( Stream outputStream )
		{
			// Repeat count -1 means don't repeat the animation, so don't add
			// a Netscape extension.
			if( _repeatCount >= 0 ) 
			{
				outputStream.WriteByte( GifComponent.CodeExtensionIntroducer );
				outputStream.WriteByte( GifComponent.CodeApplicationExtensionLabel );
				NetscapeExtension ne = new NetscapeExtension( _repeatCount );
				ne.WriteToStream( outputStream );
			}
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

		#endregion
	}
}
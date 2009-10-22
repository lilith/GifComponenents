#region Copyright (C) Simon Bridewell, Kevin Weiner
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
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace GifComponents
{
	/// <summary>
	/// A single image frame from a GIF file.
	/// Originally a nested class within the GifDecoder class by Kevin Weiner.
	/// Downloaded from 
	/// http://www.thinkedge.com/BlogEngine/file.axd?file=NGif_src2.zip
	///
	/// Amended by Simon Bridewell June-August 2009:
	/// 1. Made member variables private.
	/// 2. Added various properties to expose all the elements of the GifFrame.
	/// 3. Added constructors for use in both encoding and decoding.
	/// 4. Derive from GifComponent.
	/// 5. Added FromStream method
	/// </summary>
	[TypeConverter( typeof( ExpandableObjectConverter ) )]
	public class GifFrame : GifComponent
	{
		#region declarations
		private Bitmap _image;
		private int _delay;
		private ColourTable _localColourTable;
		private GraphicControlExtension _extension;
		private ImageDescriptor _imageDescriptor;
		private Color _backgroundColour;
		private TableBasedImageData _indexedPixels;
		#endregion

		#region constructors
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="theImage">
		/// The image held in this frame of the GIF file
		/// </param>
		/// <param name="delay">
		/// Delay in hundredths of a second before showing the next frame.
		/// </param>
		internal GifFrame( Bitmap theImage, 
		                 int delay )
		{
			_image = theImage;
			_delay = delay;
		}
		
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="indexedPixels">
		/// <see cref="TableBasedImageData"/> for this frame.
		/// </param>
		/// <param name="activeColourTable">
		/// The colour table for this frame - this could be a local colour table
		/// or the stream's global colour table.
		/// </param>
		/// <param name="imageDescriptor">
		/// The image descriptor for this frame.
		/// </param>
		/// <param name="extension">
		/// The graphic control extension, if any, which precedes this image in
		/// the input stream.
		/// </param>
		/// <param name="backgroundColour">
		/// The background colour of this frame.
		/// </param>
		/// <param name="logicalScreenDescriptor">
		/// The logical screen descriptor for the GIF stream.
		/// </param>
		/// <param name="previousFrame">
		/// The frame which precedes this one in the GIF stream, if present.
		/// </param>
		/// <param name="previousFrameBut1">
		/// The frame which precedes the frame before this one in the GIF stream,
		/// if present.
		/// </param>
		public GifFrame( TableBasedImageData indexedPixels,
		                 ColourTable activeColourTable,
		                 ImageDescriptor imageDescriptor,
		                 GraphicControlExtension extension,
		                 Color backgroundColour, 
		                 LogicalScreenDescriptor logicalScreenDescriptor,
		                 GifFrame previousFrame, 
		                 GifFrame previousFrameBut1 )
		{
			#region guard against null arguments
			if( imageDescriptor == null )
			{
				throw new ArgumentNullException( "imageDescriptor" );
			}
			#endregion

			_indexedPixels = indexedPixels;

			if( imageDescriptor.HasLocalColourTable )
			{
				_localColourTable = activeColourTable;
			}
			else
			{
				_localColourTable = null;
			}

			_extension = extension;
			if( extension != null )
			{
				_delay = extension.DelayTime;
			}
			_imageDescriptor = imageDescriptor;
			_backgroundColour = backgroundColour;
			_image = CreateBitmap( indexedPixels, 
			                       logicalScreenDescriptor,
			                       imageDescriptor,
			                       activeColourTable,
			                       extension,
			                       previousFrame, 
			                       previousFrameBut1 );
		}
		#endregion
		
		#region properties
		
		#region TheImage property
		/// <summary>
		/// Gets the image held in this frame.
		/// </summary>
		public Bitmap TheImage
		{
			get { return _image; }
		}
		#endregion
		
		#region Delay property
		/// <summary>
		/// Gets the delay in hundredths of a second before showing the next
		/// frame.
		/// </summary>
		public int Delay
		{
			get { return _delay; }
		}
		#endregion
		
		#region LocalColourTable property
		/// <summary>
		/// Gets the local colour table for this frame.
		/// </summary>
		public ColourTable LocalColourTable
		{
			get { return _localColourTable; }
		}
		#endregion

		#region GraphicControlExtension property
		/// <summary>
		/// Gets the graphic control extension which precedes this image.
		/// </summary>
		public GraphicControlExtension GraphicControlExtension
		{
			get { return _extension; }
		}
		#endregion
		
		#region ImageDescriptor property
		/// <summary>
		/// Gets the image descriptor for this frame.
		/// </summary>
		public ImageDescriptor ImageDescriptor
		{
			get { return _imageDescriptor; }
		}
		#endregion

		#region BackgroundColour property
		/// <summary>
		/// Gets the background colour of the current frame
		/// </summary>
		public Color BackgroundColour
		{
			get { return _backgroundColour; }
		}
		#endregion
		
		#region IndexedPixels property
		/// <summary>
		/// Gets the table-based image data containing the indices within the
		/// active colour table of the colours of each of the pixels in the
		/// frame.
		/// </summary>
		[Description( "Gets the table-based image data containing the " + 
		              "indices within the active colour table of the colours " + 
		              "of each of the pixels in the frame." )]
		public TableBasedImageData IndexedPixels
		{
			get { return _indexedPixels; }
		}
		#endregion
		
		#endregion
		
		#region static FromStream method
		/// <summary>
		/// Creates and returns a GifFrame by reading its data from the supplied
		/// input stream.
		/// </summary>
		/// <param name="inputStream">
		/// A stream containing the data which makes the GifStream, starting 
		/// with the image descriptor for this frame.
		/// </param>
		/// <param name="lsd">
		/// The logical screen descriptor for the GIF stream.
		/// </param>
		/// <param name="gct">
		/// The global colour table for the GIF stream.
		/// </param>
		/// <param name="gce">
		/// The graphic control extension, if any, which precedes this image in
		/// the input stream.
		/// </param>
		/// <param name="previousFrame">
		/// The frame which precedes this one in the GIF stream, if present.
		/// </param>
		/// <param name="previousFrameBut1">
		/// The frame which precedes the frame before this one in the GIF stream,
		/// if present.
		/// </param>
		/// <returns>
		/// The GIF frame read from the input stream.
		/// </returns>
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
		                 MessageId = "2#gct")]
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
		                 MessageId = "3#gce")]
		public static GifFrame FromStream( Stream inputStream,
		                                   LogicalScreenDescriptor lsd,
		                                   ColourTable gct,
		                                   GraphicControlExtension gce,
		                                   GifFrame previousFrame,
		                                   GifFrame previousFrameBut1 )
		{
			#region guard against null arguments
			if( lsd == null )
			{
				throw new ArgumentNullException( "lsd" );
			}
			
			if( gce == null )
			{
				throw new ArgumentNullException( "gce" );
			}
			#endregion
			
			#region get transparent colour index from graphic control extension
			int transparentColourIndex;
			if( gce == null )
			{
				transparentColourIndex = 0;
			}
			else
			{
				transparentColourIndex = gce.TransparentColourIndex;
			}
			#endregion

			ImageDescriptor imageDescriptor = ImageDescriptor.FromStream( inputStream );
			
			#region determine the colour table to use for this frame
			Color backgroundColour = Color.FromArgb( 0 ); // TODO: is this the right background colour?
			// TODO: use backgroundColourIndex from the logical screen descriptor?
			ColourTable activeColourTable;
			ColourTable localColourTable;
			if( imageDescriptor.HasLocalColourTable ) 
			{
				localColourTable = ColourTable.FromStream( inputStream, imageDescriptor.LocalColourTableSize );
				activeColourTable = localColourTable; // make local table active
			} 
			else 
			{
				localColourTable = null;
				activeColourTable = gct; // make global table active
				if( lsd.BackgroundColourIndex == transparentColourIndex )
				{
					backgroundColour = Color.FromArgb( 0 );
				}
			}
			#endregion

			// If this frame has a transparent colour then replace its entry in
			// the active colour table with black // TODO: (why?)
			Color savedTransparentColour = Color.FromArgb( 0 );
			if( gce.HasTransparentColour )
			{
				savedTransparentColour = activeColourTable[transparentColourIndex];
				activeColourTable[transparentColourIndex] = Color.FromArgb( 0 );
			}

			#region handle the situation when there's no colour table
			bool noColourTable = false;
			string noColourTableMessage = "";
			if( activeColourTable == null ) 
			{
				noColourTableMessage
					= "No active colour table for this frame. "
					+ "Local colour table flag: " + imageDescriptor.HasLocalColourTable
					+ ". Local colour table size: " + imageDescriptor.LocalColourTableSize
					+ ". Global colour table size: " + lsd.GlobalColourTableSize;
				noColourTable = true; // use this flag later to set the GifFrame status
			}
			#endregion

			// decode pixel data
			int pixelCount = imageDescriptor.Size.Width * imageDescriptor.Size.Height;
			TableBasedImageData indexedPixels 
				= new TableBasedImageData( inputStream, pixelCount );
			
			if( indexedPixels.Pixels.Count == 0 )
			{
				Bitmap emptyBitmap = new Bitmap( lsd.LogicalScreenSize.Width, 
				                                 lsd.LogicalScreenSize.Height );
				GifFrame emptyFrame = new GifFrame( emptyBitmap, 
				                                    gce.DelayTime );
				emptyFrame.SetStatus( ErrorState.FrameHasNoImageData, "" );
				return emptyFrame;
			}
			
			// Skip any remaining blocks up to the next block terminator (in
			// case there is any surplus data before the next frame)
			SkipBlocks( inputStream );

			// If we replaced the transparent colour's entry in the active 
			// colour table with black, then restore its original colour.
			if( gce.HasTransparentColour )
			{
				activeColourTable[transparentColourIndex] = savedTransparentColour;
			}

			GifFrame frame = new GifFrame( indexedPixels,
			                               activeColourTable,
			                               imageDescriptor,
			                               gce,
			                               backgroundColour,
			                               lsd,
			                               previousFrame,
			                               previousFrameBut1 );

			if( noColourTable )
			{
				frame.SetStatus( ErrorState.FrameHasNoColourTable, 
				                 noColourTableMessage );
			}
			
			return frame;
		}
		#endregion

		#region private static CreateBitmap( GifDecoder, ImageDescriptor, ColourTable, bool ) method
		/// <summary>
		/// Sets the pixels of the decoded image.
		/// </summary>
		/// <param name="imageData">
		/// Table based image data containing the indices within the active
		/// colour table of the colours of the pixels in this frame.
		/// </param>
		/// <param name="lsd">
		/// The logical screen descriptor for the GIF stream.
		/// </param>
		/// <param name="id">
		/// The image descriptor for this frame.
		/// </param>
		/// <param name="activeColourTable">
		/// The colour table to use with this frame - either the global colour
		/// table or a local colour table.
		/// </param>
		/// <param name="gce">
		/// The graphic control extension, if any, which precedes this image in
		/// the input stream.
		/// </param>
		/// <param name="previousFrame">
		/// The frame which precedes this one in the GIF stream, if present.
		/// </param>
		/// <param name="previousFrameBut1">
		/// The frame which precedes the frame before this one in the GIF stream,
		/// if present.
		/// </param>
		private static Bitmap CreateBitmap( TableBasedImageData imageData,
		                                    LogicalScreenDescriptor lsd,
		                                    ImageDescriptor id,
		                                    ColourTable activeColourTable,
		                                    GraphicControlExtension gce,
		                                    GifFrame previousFrame,
		                                    GifFrame previousFrameBut1 )
		{
			Color[] pixelsForThisFrame = new Color[lsd.LogicalScreenSize.Width 
			                                       * lsd.LogicalScreenSize.Height];
			
			#region Get the disposal method of the previous frame read from the GIF stream
			DisposalMethod previousDisposalMethod;
			if( previousFrame == null )
			{
				previousDisposalMethod = DisposalMethod.NotSpecified;
			}
			else
			{
				previousDisposalMethod = previousFrame.GraphicControlExtension.DisposalMethod;
			}
			#endregion

			// fill in starting image contents based on last image's dispose code
			Bitmap previousImageBut1;
			if( previousDisposalMethod > 0 )
			{
				if( previousFrameBut1 == null )
				{
					previousImageBut1 = null;
				}
				else
				{
					previousImageBut1 = previousFrameBut1.TheImage;
				}

				if( previousImageBut1 != null ) 
				{
					Color[] previousFramePixels = GetPixels( new Bitmap( previousImageBut1 ) );
					int size = lsd.LogicalScreenSize.Width 
							 * lsd.LogicalScreenSize.Height;
					Array.Copy( previousFramePixels, 0, pixelsForThisFrame, 0, size );
					// copy pixels

					if( previousDisposalMethod == DisposalMethod.RestoreToBackgroundColour ) 
					{
						// fill last image rect area with background color
						Graphics g = Graphics.FromImage( previousFrame.TheImage );
						Color c = Color.Empty;
						if( gce.HasTransparentColour )
						{
							// assume background is transparent // TODO: why?
							c = Color.FromArgb( 0, 0, 0, 0 ); 	
						} 
						else 
						{
							// use given background color
							c = previousFrame.BackgroundColour;
						}
						Brush brush = new SolidBrush( c );
						Rectangle previousImageRectangle 
							= new Rectangle( previousFrame.ImageDescriptor.Position, 
							                 previousFrame.ImageDescriptor.Size );
						g.FillRectangle( brush, previousImageRectangle );
						brush.Dispose();
						g.Dispose();
					}
				}
			}

			// copy each source line to the appropriate place in the destination
			int pass = 1;
			int interlaceRowIncrement = 8;
			int interlaceRowNumber = 0; // the row of pixels we're currently 
										// setting in an interlaced image.
			for( int i = 0; i < id.Size.Height; i++)  
			{
				int pixelRowNumber = i;
				if( id.IsInterlaced ) 
				{
					#region work out the pixel row we're setting for an interlaced image
					if( interlaceRowNumber >= id.Size.Height ) 
					{
						pass++;
						switch( pass )
						{
							case 2 :
								interlaceRowNumber = 4;
								break;
							case 3 :
								interlaceRowNumber = 2;
								interlaceRowIncrement = 4;
								break;
							case 4 :
								interlaceRowNumber = 1;
								interlaceRowIncrement = 2;
								break;
						}
					}
					#endregion
					pixelRowNumber = interlaceRowNumber;
					interlaceRowNumber += interlaceRowIncrement;
				}
				
				// Colour in the pixels for this row
				pixelRowNumber += id.Position.Y;
				if( pixelRowNumber < lsd.LogicalScreenSize.Height ) 
				{
					int k = pixelRowNumber * lsd.LogicalScreenSize.Width;
					int dx = k + id.Position.X; // start of line in dest
					int dlim = dx + id.Size.Width; // end of dest line
					if( (k + lsd.LogicalScreenSize.Width) < dlim ) 
					{
						// TODO: does this ever happen?
						dlim = k + lsd.LogicalScreenSize.Width; // past dest edge
					}
					int sx = i * id.Size.Width; // start of line in source
					while (dx < dlim) 
					{
						// map color and insert in destination
						int indexInColourTable = (int) imageData.Pixels[sx++];
						// Set this pixel's colour if its index isn't the 
						// transparent colour index, or if this frame doesn't
						// have a transparent colour.
						if( indexInColourTable != gce.TransparentColourIndex 
						    || gce.HasTransparentColour == false )
						{
							Color c = activeColourTable[indexInColourTable];
							pixelsForThisFrame[dx] = c;
						}
						dx++;
					}
				}
			}
			Size screenSize = new Size( lsd.LogicalScreenSize.Width, 
			                            lsd.LogicalScreenSize.Height );
			return CreateBitmap( screenSize, pixelsForThisFrame );
		}
		#endregion

		#region private static CreateBitmap( Size, Color[] ) method
		/// <summary>
		/// Creates and returns a Bitmap of the supplied size composed of pixels
		/// of the supplied colours, working left to right and then top to 
		/// bottom.
		/// </summary>
		/// <param name="size">
		/// The size of the bitmap to be created.
		/// </param>
		/// <param name="pixels">
		/// An array of the colours of the pixels for the bitmap to be created.
		/// </param>
		private static Bitmap CreateBitmap( Size size, Color[] pixels )
		{
			Bitmap returnBitmap = new Bitmap( size.Width, size.Height );
			int count = 0;
			for( int th = 0; th < returnBitmap.Height; th++ )
			{
				for( int tw = 0; tw < returnBitmap.Width; tw++ )
				{
					returnBitmap.SetPixel( tw, th, pixels[count++] );
				}
			}
			return returnBitmap;
		}
		#endregion

		#region private static GetPixels method
		/// <summary>
		/// Returns an array of the colours of the pixels in the supplied image,
		/// working from left to right and top to bottom.
		/// </summary>
		/// <param name="bitmap"></param>
		/// <returns></returns>
		private static Color[] GetPixels( Bitmap bitmap )
		{
			Color[] pixels = new Color[ bitmap.Width * bitmap.Height ];
			int count = 0;
			for (int th = 0; th < bitmap.Height; th++)
			{
				for (int tw = 0; tw < bitmap.Width; tw++)
				{
					Color color = bitmap.GetPixel(tw, th);
					pixels[count] = color;
					count++;
				}
			}
			return pixels;
		}
		#endregion

		#region public WriteToStream method
		/// <summary>
		/// Writes this component to the supplied output stream.
		/// </summary>
		/// <param name="outputStream">
		/// The output stream to write to.
		/// </param>
		public override void WriteToStream( Stream outputStream )
		{
			throw new NotImplementedException();
		}
		#endregion

	}
}

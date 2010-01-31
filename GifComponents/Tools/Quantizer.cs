#region Copyright (C) Morgan Skinner, Simon Bridewell
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
//
// This file is based on the Quantizer base class by Morgan Skinner - 
// http://msdn.microsoft.com/en-us/library/aa479306.aspx
//
// Amended by Simon Bridewell, November 2009:
// * Small edits to XML comments
// * Changed namespace to GifComponents
// * Fixed / suppressed some FxCop warnings
// * Style changes (e.g. add missing curly brackets around conditional blocks)
// * Changed constructor access modifier from public to protected
// * Added null argument test to constructor
// * Changed various access modifiers to internal
#endregion

using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;

namespace GifComponents
{
	/// <summary>
	/// Base class for a colour quantizer.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", 
	                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
	                 MessageId = "Quantizer")]
	public unsafe abstract class Quantizer
	{
		#region constructor
		/// <summary>
		/// Construct the quantizer
		/// </summary>
		/// <param name="singlePass">
		/// If true, the quantization only needs to loop through the source 
		/// pixels once
		/// </param>
		/// <remarks>
		/// If you construct this class with a true value for singlePass, then 
		/// the code will, when quantizing your image, only call the 
		/// 'QuantizeImage' function. 
		/// If two passes are required, the code will call 
		/// 'InitialQuantizeImage' and then 'QuantizeImage'.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1720:AvoidTypeNamesInParameters", 
		                 MessageId = "0#")]
		protected Quantizer( bool singlePass )
		{
			_singlePass = singlePass;
		}
		#endregion

		#region Quantize method
		/// <summary>
		/// Quantize an image and return the resulting output bitmap
		/// </summary>
		/// <param name="source">The image to quantize</param>
		/// <returns>A quantized version of the image</returns>
		public Bitmap Quantize( Image source )
		{
			if( source == null )
			{
				// TESTME: constructor - null argument
				throw new ArgumentNullException( "source" );
			}
			
			// Get the size of the source image
			int	height = source.Height;
			int width = source.Width;

			// And construct a rectangle from these dimensions
			Rectangle bounds = new Rectangle( 0, 0, width, height );

			// First off take a 32bpp copy of the image
			Bitmap copy = new Bitmap( width, height, PixelFormat.Format32bppArgb );

			// And construct an 8bpp version
			Bitmap output = new Bitmap( width, height, PixelFormat.Format8bppIndexed );

			// Now lock the bitmap into memory
			using( Graphics g = Graphics.FromImage ( copy ) )
			{
				g.PageUnit = GraphicsUnit.Pixel;

				// Draw the source image onto the copy bitmap,
				// which will effect a widening as appropriate.
				g.DrawImageUnscaled ( source, bounds );
			}

			// Define a pointer to the bitmap data
			BitmapData	sourceData = null;

			try
			{
				// Get the source image bits and lock into memory
				sourceData = copy.LockBits( bounds, 
				                            ImageLockMode.ReadOnly, 
				                            PixelFormat.Format32bppArgb );

				// Call the FirstPass function if not a single pass algorithm.
				// For something like an octree quantizer, this will run through
				// all image pixels, build a data structure, and create a palette.
				if( !_singlePass )
				{
					FirstPass( sourceData, width, height );
					// TODO: temp debug code - restore when testing with TextualRepresentation
//					Console.WriteLine( "After first pass" );
//					ReportDataStructure();
				}

				// Then set the color palette on the output bitmap. I'm passing 
				// in the current palette as there's no way to construct a new, 
				// empty palette.
				output.Palette = this.GetPalette( output.Palette );

				// Then call the second pass which actually does the conversion
				SecondPass( sourceData, output, width, height, bounds );
				
				// TODO: temp debug code - restore when testing with TextualRepresentation
//				Console.WriteLine( "After second pass" );
//				ReportDataStructure();
			}
			finally
			{
				// Ensure that the bits are unlocked
				copy.UnlockBits( sourceData );
			}

			// Last but not least, return the output bitmap
			return output;
		}
		#endregion

		#region FirstPass method
		/// <summary>
		/// Execute the first pass through the pixels in the image
		/// </summary>
		/// <param name="sourceData">The source data</param>
		/// <param name="width">The width in pixels of the image</param>
		/// <param name="height">The height in pixels of the image</param>
		protected virtual void FirstPass( BitmapData sourceData, 
		                                  int width, 
		                                  int height )
		{
			// Define the source data pointers. The source row is a byte to
			// keep addition of the stride value easier (as this is in bytes)
			byte* pSourceRow = (byte*) sourceData.Scan0.ToPointer( ) ;
			Int32* pSourcePixel;

			// Loop through each row
			for( int row = 0; row < height; row++ )
			{
				// Set the source pixel to the first pixel in this row
				pSourcePixel = (Int32*) pSourceRow;

				// And loop through each column
				for( int col = 0; col < width; col++, pSourcePixel++ )
				{
					// Now I have the pixel, call the FirstPassQuantize function...
					InitialQuantizePixel( (Colour32*) pSourcePixel );
				}

				// Add the stride to the source row
				pSourceRow += sourceData.Stride;
			}
		}
		#endregion

		#region protected virtual SecondPass method
		/// <summary>
		/// Execute a second pass through the bitmap
		/// </summary>
		/// <param name="sourceData">The source bitmap, locked into memory</param>
		/// <param name="output">The output bitmap</param>
		/// <param name="width">The width in pixels of the image</param>
		/// <param name="height">The height in pixels of the image</param>
		/// <param name="bounds">The bounding rectangle</param>
		protected virtual void SecondPass( BitmapData sourceData, 
		                                   Bitmap output, 
		                                   int width, 
		                                   int height, 
		                                   Rectangle bounds )
		{
			BitmapData outputData = null;

			try
			{
				// Lock the output bitmap into memory
				outputData = output.LockBits( bounds, 
				                              ImageLockMode.WriteOnly, 
				                              PixelFormat.Format8bppIndexed );

				// Define the source data pointers. The source row is a byte to
				// keep addition of the stride value easier (as this is in bytes)
				byte* pSourceRow = (byte*) sourceData.Scan0.ToPointer();
				Int32* pSourcePixel = (Int32*) pSourceRow;
				Int32* pPreviousPixel = pSourcePixel;

				// Now define the destination data pointers
				byte* pDestinationRow = (byte*) outputData.Scan0.ToPointer();
				byte* pDestinationPixel = pDestinationRow;

				// And convert the first pixel, so that I have values going into the loop
				byte pixelValue = QuantizePixel( (Colour32*)pSourcePixel );

				// Assign the value of the first pixel
				*pDestinationPixel = pixelValue;

				// Loop through each row
				for( int row = 0; row < height; row++ )
				{
					// Set the source pixel to the first pixel in this row
					pSourcePixel = (Int32*) pSourceRow;

					// And set the destination pixel pointer to the first pixel in the row
					pDestinationPixel = pDestinationRow;

					// Loop through each pixel on this scan line
					for( int col = 0; col < width; col++, pSourcePixel++, pDestinationPixel++ )
					{
						// Check if this is the same as the last pixel. If so 
						// use that value rather than calculating it again. 
						// This is an inexpensive optimisation.
						if( *pPreviousPixel != *pSourcePixel )
						{
							// Quantize the pixel
							pixelValue = QuantizePixel( (Colour32*) pSourcePixel );

							// And setup the previous pointer
							pPreviousPixel = pSourcePixel;
						}

						// And set the pixel in the output
						*pDestinationPixel = pixelValue;
					}

					// Add the stride to the source row
					pSourceRow += sourceData.Stride;

					// And to the destination row
					pDestinationRow += outputData.Stride;
				}
			}
			finally
			{
				// Ensure that I unlock the output bits
				output.UnlockBits( outputData );
			}
		}
		#endregion

		#region protected virtual InitialQuantizePixel method
		/// <summary>
		/// Override this to process the pixel in the first pass of the algorithm
		/// </summary>
		/// <param name="pixel">The pixel to quantize</param>
		/// <remarks>
		/// This function need only be overridden if your quantize algorithm 
		/// needs two passes, such as an Octree quantizer.
		/// </remarks>
		internal virtual void InitialQuantizePixel( Colour32* pixel )
		{
		}
		#endregion

		#region protected abstract QuantizePixel method
		/// <summary>
		/// Override this to process the pixel in the second pass of the algorithm
		/// </summary>
		/// <param name="pixel">The pixel to quantize</param>
		/// <returns>The quantized value</returns>
		internal abstract byte QuantizePixel( Colour32* pixel );
		#endregion

		#region public abstract GetPalette method
		/// <summary>
		/// Retrieve the palette for the quantized image
		/// </summary>
		/// <param name="original">Any old palette, this is overwritten</param>
		/// <returns>The new color palette</returns>
		protected abstract ColorPalette GetPalette( ColorPalette original ) ;
		#endregion
		
		// TODO: temp debug code - restore when testing with TextualRepresentation
//		protected abstract void ReportDataStructure();

		#region declarations
		/// <summary>
		/// Flag used to indicate whether a single pass or two passes are needed for quantization.
		/// </summary>
		private bool	_singlePass ;
		#endregion

	}
}

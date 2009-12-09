#region Copyright (C) Simon Bridewell
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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;
using GifComponents.Components;

namespace GifComponents
{
	/// <summary>
	/// PixelAnalysis wraps various classes for quantizing images (i.e. changing
	/// their colour palettes so that a maximum of 256 colours is used).
	/// The input is an image or collection of images, a quantization colour
	/// quality (applicable only when NeuQuant is the quantization method),
	/// and a quantizer type indicating the class which performs the actual
	/// quantization.
	/// The output is a <see cref="ColourTable"/> containing all the colours 
	/// from the quantized image(s), and a collection of indices into that
	/// colour table indicating the colours of each of the pixels in the 
	/// supplied image(s), both suitable for use by the 
	/// <see cref="AnimatedGifEncoder"/>.
	/// </summary>
	public class PixelAnalysis : IDisposable
	{
		#region declarations
		private NeuQuant _nq;
		private OctreeQuantizer _oq;
		private QuantizerType _quantizerType;
		private Collection<Color> _distinctColours;
		private ColourTable _colourTable;
		private IndexedPixels _indexedPixels;
		private Collection<IndexedPixels> _indexedPixelsCollection;
		private Image _imageToStudy;
		private Collection<Image> _imagesToStudy;
		private int _colourQuality;
		private string _status;
		private int _processingFrame;
		#endregion
		
		#region constructor( Image, int )
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="imageToStudy">
		/// The image containing the pixels to be analyzed.
		/// </param>
		/// <param name="colourQuantizationQuality">
		/// Sets quality of color quantization (conversion of images
		/// to the maximum 256 colors allowed by the GIF specification).
		/// Lower values (minimum = 1) produce better colors, but slow
		/// processing significantly.  10 is the default, and produces
		/// good color mapping at reasonable speeds.  Values greater
		/// than 20 do not yield significant improvements in speed.
		/// Defaults to 1 if not greater than zero.
		/// </param>
		/// <param name="quantizerType">
		/// The type of object to use to quantize the image to 255 colours.
		/// </param>
		/// <exception cref="ArgumentException">
		/// The supplied quantizer type is not valid.
		/// </exception>
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
		                 MessageId = "2#quantizer")]
		[SuppressMessage("Microsoft.Usage", 
		                 "CA2204:LiteralsShouldBeSpelledCorrectly", 
		                 MessageId = "quantizer")]
		public PixelAnalysis( Image imageToStudy, 
		                      int colourQuantizationQuality, 
		                      QuantizerType quantizerType )
		{
			_imageToStudy = imageToStudy;
			_colourQuality = colourQuantizationQuality;
			_status = string.Empty;
			_quantizerType = quantizerType;
			switch( quantizerType )
			{
				case QuantizerType.NeuQuant:
					// TODO: maybe _nq should be instantiated here?
					break;
					
				case QuantizerType.Octree:
					_oq = new OctreeQuantizer( 255, 8 );
					break;
					
				default:
					string message
						= "Unexpected quantizer type: " + quantizerType.ToString();
					throw new ArgumentException( message, "quantizerType" );
			}
		}
		#endregion
		
		#region constructor( Collection<Image>, int )
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="imagesToStudy">
		/// The images for which to analyse the pixels.
		/// </param>
		/// <param name="colourQuantizationQuality">
		/// Sets quality of color quantization (conversion of images
		/// to the maximum 256 colors allowed by the GIF specification).
		/// Lower values (minimum = 1) produce better colors, but slow
		/// processing significantly.  10 is the default, and produces
		/// good color mapping at reasonable speeds.  Values greater
		/// than 20 do not yield significant improvements in speed.
		/// Defaults to 1 if not greater than zero.
		/// </param>
		public PixelAnalysis( Collection<Image> imagesToStudy, 
		                      int colourQuantizationQuality )
		{
			_imagesToStudy = imagesToStudy;
			_colourQuality = colourQuantizationQuality;
			_status = string.Empty;
			_quantizerType = QuantizerType.NeuQuant;
		}
		#endregion
		
		#region properties
		
		#region ColourTable property
		/// <summary>
		/// Gets a collection colours which includes the colours of all the 
		/// supplied pixels.
		/// </summary>
		public ColourTable ColourTable
		{
			get 
			{ 
				if( _colourTable == null )
				{
					Analyse();
				}
				return _colourTable; 
			}
		}
		#endregion
		
		#region IndexedPixels property
		/// <summary>
		/// Gets the indices within the colour table of the colours of each of 
		/// the pixels in the supplied image.
		/// </summary>
		public IndexedPixels IndexedPixels
		{
			get
			{
				if( _imageToStudy == null )
				{
					string message
						= "The PixelAnalysis object was instantiated using the "
						+ "constructor which accepts a collection of images "
						+ "and you are attempting to retrieve the indexed "
						+ "pixels for a single image. "
						+ "Call the IndexedPixelCollection property instead.";
					throw new InvalidOperationException( message );
				}
				if( _indexedPixels == null )
				{
					Analyse();
				}
				return _indexedPixels; 
			}
		}
		#endregion
		
		#region IndexedPixelsCollection property
		/// <summary>
		/// Gets a collection of the indices within the colour table of the 
		/// colours of each of the pixels in the supplied images.
		/// </summary>
		public Collection<IndexedPixels> IndexedPixelsCollection
		{
			get
			{
				if( _imagesToStudy == null )
				{
					string message
						= "The PixelAnalysis object was instantiated using the "
						+ "constructor which accepts a single image and you "
						+ "are attempting to retrieve the indexed pixels for a "
						+ "collection of images. "
						+ "Call the IndexedPixels property instead.";
					throw new InvalidOperationException( message );
				}
				if( _indexedPixelsCollection == null )
				{
					Analyse();
				}
				return _indexedPixelsCollection; 
			}
		}
		#endregion
		
		#region Status property
		/// <summary>
		/// Gets a string representing the current status of the PixelAnalysis.
		/// </summary>
		public string Status
		{
			get { return _status; }
		}
		#endregion
		
		#region ProcessingFrame property
		/// <summary>
		/// Gets the frame number currently being analysed.
		/// </summary>
		public int ProcessingFrame
		{
			get { return _processingFrame; }
		}
		#endregion
		
		#endregion

		#region private methods
		
		#region private analysis methods
		
		#region private Analyse method
		private void Analyse()
		{
			_status = "Pixel analysis started";
			_processingFrame = 0;
			if( _imagesToStudy == null )
			{
				AnalyseSingleImage();
			}
			else
			{
				AnalyseManyImages();
			}
			_status = "Pixel analysis complete";
		}
		#endregion
		
		#region private AnalyseSingleImage method
		private void AnalyseSingleImage()
		{
			_indexedPixels = new IndexedPixels();

			switch( _quantizerType )
			{
				case QuantizerType.NeuQuant:
					AnalyseSingleImageWithNeuQuant();
					break;
					
				case QuantizerType.Octree:
					AnalyseSingleImageWithOctree();
					break;
			}
		}
		#endregion
		
		#region private AnalyseSingleImageWithNeuQuant method
		private void AnalyseSingleImageWithNeuQuant()
		{
			// Work out the colour table for the pixels in the supplied image
			_status = "Getting image pixels";
			Collection<Color> pixelColours = ImageTools.GetColours( _imageToStudy );
			_status = "Setting colour table";
			SetColourTable( pixelColours, _colourQuality );

			// Work out the indices in the colour table of each of the pixels
			// in the supplied image.
			_status = "Getting indexed pixels";
			_indexedPixels = GetIndexedPixels( pixelColours );
		}
		#endregion
		
		#region private AnalyseSingleImageWithOctree method
		[SuppressMessage("Microsoft.Usage", 
		                 "CA2204:LiteralsShouldBeSpelledCorrectly", 
		                 MessageId = "Octree")]
		private void AnalyseSingleImageWithOctree()
		{
			// TODO: use Octree quantizer to quantize image, build colour table and get pixel indices
			_status = "Quantizing image";
			Image quantized = _oq.Quantize( _imageToStudy );
			// FIXME: _imageToStudy.Palette contains 0 entries
			_status = "Getting palette for quantized image";
			ColorPalette palette = _oq.GetPalette( _imageToStudy.Palette );
			// TODO: work out how to convert the quantized image and palette to the required formats
			throw new NotImplementedException( "Octree quantization not implemented yet" );
		}
		#endregion
		
		#region private AnalyseManyImages method
		private void AnalyseManyImages()
		{
			// TODO: once OctreeQuantizer works for single images, use it here too
			Collection<Color> pixelData = new Collection<Color>(); // pixel data for the entire animation
			Collection<Color> imagePixelData; // pixel data for a single image

			// Work out the colour table for the pixels in each of the 
			// supplied images
			for( int i = 0; i < _imagesToStudy.Count; i++ )
			{
				_processingFrame = i + 1;
				_status 
					= "Getting image pixels for frame " + _processingFrame 
					+ " of " + _imagesToStudy.Count;
				Image thisImage = _imagesToStudy[i];
				imagePixelData = ImageTools.GetColours( thisImage );
				foreach( Color c in imagePixelData )
				{
					pixelData.Add( c );
				}
			}
			_status = "Setting colour table";
			SetColourTable( pixelData, _colourQuality );

			// Work out the indices in the colour table of each of the pixels
			// in each of the supplied images.
			_indexedPixelsCollection = new Collection<IndexedPixels>();
			for( int i = 0; i < _imagesToStudy.Count; i++ )
			{
				_processingFrame = i + 1;
				Image thisImage = _imagesToStudy[i];
				_status 
					= "Getting image pixels for frame " + _processingFrame 
					+ " of " + _imagesToStudy.Count;
				imagePixelData = ImageTools.GetColours( thisImage );
				_status 
					= "Getting indexed pixels for frame " + _processingFrame 
					+ " of " + _imagesToStudy.Count;
				IndexedPixels indexedPixels = GetIndexedPixels( imagePixelData );
				_status 
					= "Adding indexed pixels for frame " + _processingFrame 
					+ " of " + _imagesToStudy.Count;
				_indexedPixelsCollection.Add( indexedPixels );
			}
			_status = "Pixel analysis complete";
		}
		#endregion

		#endregion
		
		#region private SetColourTable method
		/// <summary>
		/// Calculates the colour table needed to index the supplied pixels.
		/// </summary>
		/// <param name="pixelColours">
		/// A collection of the colours of all the pixels in the image
		/// </param>
		/// <param name="colourQuantizationQuality">
		/// Sets quality of color quantization (conversion of images
		/// to the maximum 256 colors allowed by the GIF specification).
		/// Lower values (minimum = 1) produce better colors, but slow
		/// processing significantly.  10 is the default, and produces
		/// good color mapping at reasonable speeds.  Values greater
		/// than 20 do not yield significant improvements in speed.
		/// Defaults to 1 if not greater than zero.
		/// </param>
		private void SetColourTable( Collection<Color> pixelColours, 
		                             int colourQuantizationQuality )
		{
			System.Diagnostics.Debug.WriteLine( DateTime.Now + " PixelAnalysis.SetColourTable - start" );
			int len = pixelColours.Count;
			_status = "Getting distinct colours";
			_distinctColours = ImageTools.GetDistinctColours( pixelColours );
			_status = "Done getting distinct colours";
			
			if( _distinctColours.Count > 256 )
			{
				// more than 256 colours so need to adjust for a colour table
				// of 256 colours.
				if( _quantizerType == QuantizerType.NeuQuant )
				{
					_status = "Populating RGB collection for NeuQuant";
					byte[] rgb = ImageTools.GetRgbArray( pixelColours );
					_status = "Instantiating NeuQuant";
					_nq = new NeuQuant( rgb, len, colourQuantizationQuality );
					_status = "Processing NeuQuant";
					_colourTable = _nq.Process(); // create reduced palette
					_status = "Done processing NeuQuant";
				}
				else
				{
					// TODO: use OctreeQuantizer
					throw new InvalidOperationException( "Octree quantization not implemented yet" );
				}
			}
			else
			{
				// few enough colours to create a colour table directly without
				// quantization.
				_colourTable = new ColourTable();
				foreach( Color c in _distinctColours )
				{
					_colourTable.Add( c );
				}
				_colourTable.Pad();
			}
			System.Diagnostics.Debug.WriteLine( DateTime.Now + " PixelAnalysis.SetColourTable - finish" );
		}
		#endregion
		
		#region private GetIndexedPixels method
		/// <summary>
		/// Gets the indices of the colours of each of the supplied pixels 
		/// within the colour table.
		/// </summary>
		/// <param name="pixelColours">
		/// A collection of the colours for which to get the indices in the 
		/// colour table.
		/// </param>
		/// <returns>
		/// A collection of the indices of the colours of each of the supplied 
		/// pixels within the colour table.
		/// </returns>
		private IndexedPixels GetIndexedPixels( Collection<Color> pixelColours )
		{
			IndexedPixels indexedPixels = new IndexedPixels();
			int numberOfPixels = pixelColours.Count;
			int indexInColourTable;
			int red;
			int green;
			int blue;
			for (int i = 0; i < numberOfPixels; i++) 
			{
				red = pixelColours[i].R;
				green = pixelColours[i].G;
				blue = pixelColours[i].B;
				
				// Get the index in the colour table of the colour of this pixel
				if( _distinctColours.Count > 256 )
				{
					indexInColourTable = _nq.Map( red, green, blue );
				}
				else
				{
					indexInColourTable = _distinctColours.IndexOf( pixelColours[i] );
				}
				indexedPixels.Add( (byte) indexInColourTable );
			}
			
			return indexedPixels;
		}
		#endregion

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
		~PixelAnalysis()
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
					_colourTable.Dispose();
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

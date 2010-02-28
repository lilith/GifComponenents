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

namespace GifComponents.Tools
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
		private Collection<Color> _imageColours;
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

		#region constructor( Image )
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="imageToStudy">
		/// The image containing the pixels to be analyzed.
		/// </param>
		/// <param name="quantizerType">
		/// The type of object to use to quantize the image to 255 colours.
		/// </param>
		/// <exception cref="ArgumentException">
		/// The supplied quantizer type is not valid.
		/// </exception>
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
		                 MessageId = "1#quantizer")]
		public PixelAnalysis( Image imageToStudy, QuantizerType quantizerType )
		{
			_imageToStudy = imageToStudy;
			_colourQuality = 10;
			_status = string.Empty;
			_quantizerType = quantizerType;
			GetColours( imageToStudy );
			
			if( _distinctColours.Count > 256 )
			{
				switch( quantizerType )
				{
					case QuantizerType.NeuQuant:
						break;
						
					case QuantizerType.Octree:
						_oq = new OctreeQuantizer( 255, 8 );
						break;
				}
			}
		}
		#endregion
		
		#region constructor( Collection<Image> )
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="imagesToStudy">
		/// The images for which to analyse the pixels.
		/// </param>
		public PixelAnalysis( Collection<Image> imagesToStudy )
		{
			_imagesToStudy = imagesToStudy;
			_colourQuality = 10;
			_status = string.Empty;
			_quantizerType = QuantizerType.NeuQuant;
			GetColours( _imagesToStudy );
		}
		#endregion

		#region properties
		
		#region ColourQuality property
		/// <summary>
		/// Gets and sets quality of color quantization (conversion of images
		/// to the maximum 256 colors allowed by the GIF specification).
		/// Lower values (minimum = 1) produce better colors, but slow
		/// processing significantly.  10 is the default, and produces
		/// good color mapping at reasonable speeds.  Values greater
		/// than 20 do not yield significant improvements in speed.
		/// Defaults to 1 if not greater than zero.
		/// Only used if the quantizer type is set to NeuQuant.
		/// </summary>
		public int ColourQuality
		{
			get { return _colourQuality; }
			set
			{
				_colourQuality = value;
				if( _colourQuality < 1 )
				{
					// TESTME: ColourQuality set to less than 1
					_colourQuality = 1;
				}
			}
		}
		#endregion
		
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
					// TESTME: IndexedPixels - _indexedPixels == null
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
					// TESTME: IndexedPixelsCollection = _indexedPixelsCollection == null
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
			if( _distinctColours.Count > 256 )
			{
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
			else
			{
				// few enough colours to create a colour table directly without
				// quantization.
				CreateDirectColourTable();
			}

			// Work out the indices in the colour table of each of the pixels
			// in the supplied image.
			_status = "Getting indexed pixels";
			_indexedPixels = GetIndexedPixels( _imageColours );
		}
		#endregion
		
		#region private AnalyseSingleImageWithNeuQuant method
		private void AnalyseSingleImageWithNeuQuant()
		{
			CreateColourTableUsingNeuQuant( _colourQuality );
		}
		#endregion
		
		#region private AnalyseSingleImageWithOctree method
		[SuppressMessage("Microsoft.Usage", 
		                 "CA2204:LiteralsShouldBeSpelledCorrectly", 
		                 MessageId = "Octree")]
		private void AnalyseSingleImageWithOctree()
		{
			_status = "Quantizing image";
			_imageToStudy = _oq.Quantize( _imageToStudy );
			_status = "Getting distinct colours from quantized image";
			// TODO: see Quantizer.Quantize method for how to get a palette for the quantized image
			GetColours( _imageToStudy );
			CreateDirectColourTable();
		}
		#endregion
		
		#region private AnalyseManyImages method
		private void AnalyseManyImages()
		{
			// Work out the colour table for the pixels in each of the 
			// supplied images
			if( _distinctColours.Count > 256 )
			{
				_status = "Setting colour table";
				CreateColourTableUsingNeuQuant( _colourQuality );
				
				// TODO: work out how to create a global colour table using Octree quantizer
			}
			else
			{
				CreateDirectColourTable();
			}

			// Work out the indices in the colour table of each of the pixels
			// in each of the supplied images.
			Collection<Color> imagePixelData; // pixel data for a single image
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
		
		#region private CreateColourTableUsingNeuQuant method
		/// <summary>
		/// Calculates the colour table needed to index the supplied pixels.
		/// </summary>
		/// <param name="colourQuantizationQuality">
		/// Sets quality of color quantization (conversion of images
		/// to the maximum 256 colors allowed by the GIF specification).
		/// Lower values (minimum = 1) produce better colors, but slow
		/// processing significantly.  10 is the default, and produces
		/// good color mapping at reasonable speeds.  Values greater
		/// than 20 do not yield significant improvements in speed.
		/// Defaults to 1 if not greater than zero.
		/// </param>
		private void CreateColourTableUsingNeuQuant( int colourQuantizationQuality )
		{
			_status = "Populating RGB collection for NeuQuant";
			byte[] rgb = ImageTools.GetRgbArray( _imageColours );
			_status = "Instantiating NeuQuant";
			_nq = new NeuQuant( rgb, colourQuantizationQuality );
			_status = "Processing NeuQuant";
			_colourTable = _nq.Process(); // create reduced palette
			_status = "Done processing NeuQuant";
		}
		#endregion

		#region void CreateDirectColourTable method
		/// <summary>
		/// Creates a colour table directly from the distinct colours in the
		/// supplied image(s).
		/// </summary>
		private void CreateDirectColourTable()
		{
			_colourTable = new ColourTable();
			foreach( Color c in _distinctColours )
			{
				_colourTable.Add( c );
			}
			_colourTable.Pad();
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
		
		#region private GetColours( Image ) method
		/// <summary>
		/// Gets the colours in the supplied image and the distinct colours in 
		/// the supplied image, amd stores them in local variables.
		/// </summary>
		/// <param name="image">The image to examine</param>
		private void GetColours( Image image )
		{
			_imageColours = ImageTools.GetColours( image );
			_distinctColours = ImageTools.GetDistinctColours( _imageColours );
		}
		#endregion

		#region private GetColours( Collection<Image> ) method
		/// <summary>
		/// Gets the colours in the supplied images and the distinct colours in 
		/// the supplied images, amd stores them in local variables.
		/// </summary>
		/// <param name="images">The images to examine</param>
		private void GetColours( Collection<Image> images )
		{
			_imageColours = new Collection<Color>();
			foreach( Image image in images )
			{
				Collection<Color> colours = ImageTools.GetColours( image );
				foreach( Color c in colours )
				{
					_imageColours.Add( c );
				}
			}
			_distinctColours = ImageTools.GetDistinctColours( _imageColours );
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

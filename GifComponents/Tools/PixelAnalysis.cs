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
using System.Drawing;

namespace GifComponents
{
	/// <summary>
	/// The result of analysing the pixels in an image or a collection of
	/// images.
	/// Creates a colour table containing the colours in the image or images,
	/// and a collection of indexed pixels for each image containing the indices
	/// of each pixel's colour within the colour table.
	/// </summary>
	public class PixelAnalysis
	{
		#region declarations
		private NeuQuant _nq;
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
		public PixelAnalysis( Image imageToStudy, int colourQuantizationQuality )
		{
			_imageToStudy = imageToStudy;
			_colourQuality = colourQuantizationQuality;
			_status = string.Empty;
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
		
		#region private Analyse method
		private void Analyse()
		{
			_processingFrame = 0;
			if( _imagesToStudy == null )
			{
				// We're analysing a single image
				_indexedPixels = new IndexedPixels();
	
				// Work out the colour table for the pixels in the supplied image
				_status = "Getting image pixels";
				Collection<Color> pixelColours = GetImagePixels( _imageToStudy );
				_status = "Setting colour table";
				SetColourTable( pixelColours, _colourQuality );
	
				// Work out the indices in the colour table of each of the pixels
				// in the supplied image.
				_status = "Getting indexed pixels";
				_indexedPixels = GetIndexedPixels( pixelColours );
				_status = "Pixel analysis complete";
			}
			else
			{
				// We're analysing a collection of images
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
					imagePixelData = GetImagePixels( thisImage );
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
					imagePixelData = GetImagePixels( thisImage );
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
		}
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
			_distinctColours = new Collection<Color>();
			Collection<byte> rgb = new Collection<byte>(); // for NeuQuant constructor, if used
			for( int i = 0; i < len; i ++ )
			{
				_status 
					= "Getting distinct colours: pixel " + i + " of " + len
					+ ". Distinct colours: " + _distinctColours.Count;
				byte red = (byte) pixelColours[i].R;
				byte green = (byte) pixelColours[i].G;
				byte blue = (byte) pixelColours[i].B;
				rgb.Add( red );
				rgb.Add( green );
				rgb.Add( blue );
				Color c = Color.FromArgb( red, green, blue );
				if( _distinctColours.Contains( c ) == false )
				{
					_distinctColours.Add( c );
				}
			}
			if( _distinctColours.Count > 256 )
			{
				// more than 256 colours so need to adjust for a colour table
				// of 256 colours.
				_status = "Calling colour quantizer";
				_nq = new NeuQuant( rgb, len, colourQuantizationQuality );
				_status = "Processing colour quantizer";
				_colourTable = _nq.Process(); // create reduced palette
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
				// Pad colour table out to a length of an exact power of 2
				while( IsPowerOf2( _colourTable.Length ) == false )
				{
					_colourTable.Add( Color.FromArgb( 0, 0, 0 ) );
				}
			}
			System.Diagnostics.Debug.WriteLine( DateTime.Now + " PixelAnalysis.SetColourTable - finish" );
		}
		#endregion
		
		#region private static IsPowerOf2 method
		/// <summary>
		/// Determines whether the supplied number is an exact power of 2 and
		/// therefore a suitable size for a colour table.
		/// </summary>
		/// <param name="number"></param>
		/// <returns></returns>
		private static bool IsPowerOf2( int number )
		{
			switch( number )
			{
				case 4:
				case 8:
				case 16:
				case 32:
				case 64:
				case 128:
				case 256:
					return true;
					
				default:
					return false;
			}
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
					Color c = Color.FromArgb( red, green, blue );
					indexInColourTable = _distinctColours.IndexOf( c );
				}
				indexedPixels.Add( (byte) indexInColourTable );
			}
			
			return indexedPixels;
		}
		#endregion
		
		#region private static GetImagePixels method
		/// <summary>
		/// Extracts the pixels of the supplied image into a byte array.
		/// </summary>
		/// <param name="imageToStudy">
		/// The image from which to extract the pixels.
		/// </param>
		/// <returns>
		/// A collection of the colours of all the pixels in the supplied image.
		/// </returns>
		private static Collection<Color> GetImagePixels( Image imageToStudy )
		{
			// SB comment - this comment was present when I downloaded the
			// code from thinkedge.com
			// TODO: improve performance: use unsafe code
			Collection<Color> pixelColours = new Collection<Color>();
			Bitmap tempBitmap = new Bitmap( imageToStudy );
			for( int th = 0; th < imageToStudy.Height; th++ )
			{
				for( int tw = 0; tw < imageToStudy.Width; tw++ )
				{
					Color color = tempBitmap.GetPixel(tw, th);
					pixelColours.Add( color );
				}
			}
			return pixelColours;
		}
		#endregion

		#endregion
		
	}
}

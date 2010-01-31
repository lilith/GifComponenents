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
// This file is based on the OctreeQuantizer by Morgan Skinner - 
// http://msdn.microsoft.com/en-us/library/aa479306.aspx
#endregion

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Imaging;

namespace GifComponents
{
	/// <summary>
	/// Quantize using an Octree.
	/// Based on code downloaded from 
	/// http://msdn.microsoft.com/en-us/library/aa479306.aspx
	/// 
	/// Amended by Simon Bridewell, November-December 2009:
	/// * Small edits to XML comments
	/// 	* Changed namespace to GifComponents
	/// 	* Fixed / suppressed some FxCop warnings
	/// 	* Style changes (e.g. add missing curly brackets around conditional blocks)
	/// 	* Added test for maxColours less than 1 into constructor
	/// 	* Moved Octree and OctreeNode classes into their own files
	/// 	* Changed various access modifiers to internal
	/// 	* Added null argument test to GetPalette method
	///		* Added array size test to GetPalette method
	/// </summary>
	[SuppressMessage("Microsoft.Naming", 
	                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
	                 MessageId = "Quantizer")]
	[SuppressMessage("Microsoft.Naming", 
	                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
	                 MessageId = "Octree")]
	public unsafe class OctreeQuantizer : Quantizer
	{
		#region declarations
		/// <summary>
		/// Stores the tree
		/// </summary>
		private	Octree _octree ;

		/// <summary>
		/// Maximum allowed color depth
		/// </summary>
		private int _maxColors ;
		#endregion

		#region constructor
		/// <summary>
		/// Construct the octree quantizer
		/// </summary>
		/// <remarks>
		/// The Octree quantizer is a two pass algorithm. The initial pass sets 
		/// up the octree, the second pass quantizes a colour based on the nodes 
		/// in the tree.
		/// </remarks>
		/// <param name="maxColours">The maximum number of colours to return</param>
		/// <param name="maxColourBits">The number of significant bits</param>
		public OctreeQuantizer( int maxColours, int maxColourBits ) : base( false )
		{
			#region guard against arguments out of range
			if( maxColours > 255 || maxColours < 1 )
			{
				string message = "The number of colours should be between 1 and 255";
				throw new ArgumentOutOfRangeException( "maxColours", 
				                                       maxColours, 
				                                       message );
			}

			if( ( maxColourBits < 1 ) | ( maxColourBits > 8 ) )
			{
				string message = "This should be between 1 and 8";
				throw new ArgumentOutOfRangeException( "maxColourBits", 
				                                       maxColourBits, 
				                                       message );
			}
			#endregion

			// Construct the octree
			_octree = new Octree( maxColourBits );

			_maxColors = maxColours;
		}
		#endregion

		#region protected override GetPalette method
		/// <summary>
		/// Retrieve the palette for the quantized image
		/// </summary>
		/// <param name="original">Any old palette, this is overwritten</param>
		/// <returns>The new color palette</returns>
		protected override ColorPalette GetPalette( ColorPalette original )
		{
			if( original == null )
			{
				// TESTME: ColorPalette null argument
				throw new ArgumentNullException( "original" );
			}
			
			// First off convert the octree to _maxColors colors
			ArrayList palette = _octree.Palletize( _maxColors - 1 );

			if( original.Entries.Length < palette.Count )
			{
				// TESTME: GetPalette - supplied palette having not enough colours
				string message
					= "The supplied palette contains only "
					+ original.Entries.Length
					+ " entries, and a palette with at least "
					+ palette.Count
					+ " is required.";
				throw new ArgumentException( message, "original" );
			}
			
			// Then convert the palette based on those colors
			for( int index = 0; index < palette.Count; index++ )
			{
				original.Entries[index] = (Color) palette[index];
			}

			// Add the transparent color
			original.Entries[_maxColors] = Color.FromArgb( 0, 0, 0, 0 ) ;

			return original ;
		}
		#endregion

		#region protected override methods
		
		#region protected override InitialQuantizePixel method
		/// <summary>
		/// Process the pixel in the first pass of the algorithm
		/// </summary>
		/// <param name="pixel">The pixel to quantize</param>
		/// <remarks>
		/// This function need only be overridden if your quantize algorithm 
		/// needs two passes, such as an Octree quantizer.
		/// </remarks>
		internal override void InitialQuantizePixel( Colour32* pixel )
		{
			// Add the color to the octree
			_octree.AddColour( pixel );
		}
		#endregion

		#region protected override QuantizePixel method
		/// <summary>
		/// Override this to process the pixel in the second pass of the 
		/// algorithm
		/// </summary>
		/// <param name="pixel">The pixel to quantize</param>
		/// <returns>The quantized value</returns>
		internal override byte QuantizePixel( Colour32* pixel )
		{
			// The color at [_maxColors] is set to transparent
			byte paletteIndex = (byte) _maxColors;

			// Get the palette index if this non-transparent
			if( pixel->Alpha > 0 )
			{
				paletteIndex = (byte) _octree.GetPaletteIndex( pixel );
			}

			return paletteIndex;
		}
		#endregion

		// TODO: remove / restore temp debug code
//		protected override void ReportDataStructure()
//		{
//			Console.WriteLine( _octree.ToString() );
//		}
		#endregion

	}
}

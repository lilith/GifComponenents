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
// * Moved out of OctreeQuantizer.cs into its own file
// * Changed class access modifier from private to public and marked as unsafe
// * Small edits to XML comments
// * Changed namespace to GifComponents
// * Fixed / suppressed some FxCop warnings
// * Style changes (e.g. add missing curly brackets around conditional blocks)
// * Changed ReducibleNodes access modifier from protected to public
// * Changed TrackPrevious access modifier from protected to public
// * Moved mask ino OctreeNode.cs
#endregion

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace GifComponents
{
	/// <summary>
	/// Class which does the actual quantization
	/// </summary>
	[SuppressMessage("Microsoft.Naming", 
	                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
	                 MessageId = "Octree")]
	public unsafe class Octree
	{
		#region declarations
		/// <summary>
		/// The root of the octree
		/// </summary>
		private	OctreeNode _root;

		/// <summary>
		/// Number of leaves in the tree
		/// </summary>
		private int _leafCount;

		/// <summary>
		/// Array of reducible nodes
		/// </summary>
		private OctreeNode[] _reducibleNodes;

		/// <summary>
		/// Maximum number of significant bits in the image
		/// </summary>
		private int _maxColorBits;

		/// <summary>
		/// Store the last node quantized
		/// </summary>
		private OctreeNode _previousNode;

		/// <summary>
		/// Cache the previous color quantized
		/// </summary>
		private int _previousColor;
		#endregion

		#region constructor
		/// <summary>
		/// Construct the octree
		/// </summary>
		/// <param name="maxColourBits">
		/// The maximum number of significant bits in the image
		/// </param>
		public Octree( int maxColourBits )
		{
			_maxColorBits = maxColourBits;
			_reducibleNodes = new OctreeNode[9];
			_root = new OctreeNode( 0, _maxColorBits, this ); 
		}
		#endregion

		#region methods
		
		#region AddColour method
		/// <summary>
		/// Add a given color value to the octree
		/// </summary>
		/// <param name="pixel">The colour to add</param>
		public void AddColour( Colour32* pixel )
		{
			// Check if this request is for the same color as the last
			if( _previousColor == pixel->ARGB )
			{
				// If so, check if I have a previous node setup. This will 
				// only ocurr if the first color in the image happens to be 
				// black, with an alpha component of zero.
				if( null == _previousNode )
				{
					// TODO: test case for this
					_previousColor = pixel->ARGB;
					_root.AddColour( pixel, _maxColorBits, 0, this );
				}
				else
					// Just update the previous node
					_previousNode.Increment( pixel );
			}
			else
			{
				_previousColor = pixel->ARGB ;
				_root.AddColour( pixel, _maxColorBits, 0, this ) ;
			}
		}
		#endregion

		#region Reduce method
		/// <summary>
		/// Reduce the depth of the tree
		/// </summary>
		public void Reduce()
		{
			int	index;

			// Find the deepest level containing at least one reducible node
			for( 
			    index = _maxColorBits - 1;
			    ( index > 0 ) && ( null == _reducibleNodes[index] ); 
			    index-- 
			   );

			// Reduce the node most recently added to the list at level 'index'
			OctreeNode	node = _reducibleNodes[index];
			_reducibleNodes[index] = node.NextReducible;

			// Decrement the leaf count after reducing the node
			_leafCount -= node.Reduce();

			// And just in case I've reduced the last color to be added, 
			// and the next color to be added is the same, invalidate the 
			// previousNode...
			_previousNode = null;
		}
		#endregion

		#region Palletize method
		/// <summary>
		/// Convert the nodes in the octree to a palette with a maximum of 
		/// colorCount colors
		/// </summary>
		/// <param name="colourCount">The maximum number of colours</param>
		/// <returns>An arraylist with the palettized colours</returns>
		public ArrayList Palletize( int colourCount )
		{
			while( Leaves > colourCount )
			{
				// TODO: test case for this
				Reduce();
			}

			// Now palettize the nodes
			ArrayList palette = new ArrayList( Leaves );
			int	 paletteIndex = 0;
			_root.ConstructPalette( palette, ref paletteIndex );

			// And return the palette
			return palette;
		}
		#endregion

		#region GetPaletteIndex method
		/// <summary>
		/// Get the palette index for the passed color
		/// </summary>
		/// <param name="pixel"></param>
		/// <returns></returns>
		public int GetPaletteIndex( Colour32* pixel )
		{
			return _root.GetPaletteIndex( pixel, 0 );
		}
		#endregion

		#region TrackPrevious method
		/// <summary>
		/// Keep track of the previous node that was quantized
		/// </summary>
		/// <param name="node">The node last quantized</param>
		public void TrackPrevious( OctreeNode node )
		{
			_previousNode = node;
		}
		#endregion

		#endregion

		#region properties
		
		#region Leaves property
		/// <summary>
		/// Get/Set the number of leaves in the tree
		/// </summary>
		public int Leaves
		{
			get { return _leafCount; }
			set { _leafCount = value; }
		}
		#endregion

		#region ReducibleNodes property
		/// <summary>
		/// Return the array of reducible nodes
		/// </summary>
		public OctreeNode[] ReducibleNodes
		{
			get { return _reducibleNodes; }
		}
		#endregion

		#endregion

	}
}

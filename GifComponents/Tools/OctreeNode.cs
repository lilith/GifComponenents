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
// * Changed various access modifiers to internal
// * Small edits to XML comments
// * Changed namespace to GifComponents
// * Fixed / suppressed some FxCop warnings
// * Style changes (e.g. add missing curly brackets around conditional blocks)
// * Added null argument test to constructor and ConstructPalette method
#endregion

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace GifComponents
{
	/// <summary>
	/// Class which encapsulates each node in an Octree
	/// </summary>
	[SuppressMessage("Microsoft.Naming", 
	                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
	                 MessageId = "Octree")]
	internal unsafe class OctreeNode
	{
		#region declarations
		/// <summary>
		/// Mask used when getting the appropriate pixels for a given node
		/// </summary>
		private static int[] mask = new int[8] 
			{ 0x80 , 0x40 , 0x20 , 0x10 , 0x08 , 0x04 , 0x02 , 0x01 };

		/// <summary>
		/// Flag indicating that this is a leaf node
		/// </summary>
		private	bool _leaf;

		/// <summary>
		/// Number of pixels in this node
		/// </summary>
		private	int _pixelCount;

		/// <summary>
		/// Red component
		/// </summary>
		private	int _red;

		/// <summary>
		/// Green Component
		/// </summary>
		private	int _green;

		/// <summary>
		/// Blue component
		/// </summary>
		private int _blue;

		/// <summary>
		/// Pointers to any child nodes
		/// </summary>
		private OctreeNode[] _children;

		/// <summary>
		/// Pointer to next reducible node
		/// </summary>
		private OctreeNode _nextReducible;

		/// <summary>
		/// The index of this node in the palette
		/// </summary>
		private	int _paletteIndex;
		#endregion

		#region constructor
		/// <summary>
		/// Construct the node
		/// </summary>
		/// <param name="level">The level in the tree = 0 - 7</param>
		/// <param name="colourBits">
		/// The number of significant color bits in the image
		/// </param>
		/// <param name="octree">
		/// The tree to which this node belongs
		/// </param>
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
		                 MessageId = "2#octree")]
		public OctreeNode( int level, int colourBits, Octree octree )
		{
			if( octree == null )
			{
				throw new ArgumentNullException( "octree" );
			}
			
			// Construct the new node
			_leaf = ( level == colourBits );

			_red = _green = _blue = 0;

			// If a leaf, increment the leaf count
			if( _leaf )
			{
				octree.Leaves++;
			}
			else
			{
				// Otherwise add this to the reducible nodes
				_nextReducible = octree.ReducibleNodes[level];
				octree.ReducibleNodes[level] = this;
				_children = new OctreeNode[8];
			}
		}
		#endregion

		#region methods
		
		#region AddColour method
		/// <summary>
		/// Add a color into the tree
		/// </summary>
		/// <param name="pixel">The color</param>
		/// <param name="colourBits">The number of significant color bits</param>
		/// <param name="level">The level in the tree</param>
		/// <param name="octree">The tree to which this node belongs</param>
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
		                 MessageId = "3#octree")]
		public void AddColour( Colour32* pixel, 
		                       int colourBits, 
		                       int level, 
		                       Octree octree )
		{
			// Update the color information if this is a leaf
			if( _leaf )
			{
				Increment( pixel );
				// Setup the previous node
				octree.TrackPrevious( this );
			}
			else
			{
				// Go to the next level down in the tree
				// FIXME: Correct the potential overflow in the operation '7-level' in 'OctreeNode.AddColour(Colour32*, Int32, Int32, Octree):Void'. (CA2233)
				int	shift = 7 - level;
				int index = ( ( pixel->Red & mask[level] ) >> ( shift - 2 ) ) |
							( ( pixel->Green & mask[level] ) >> ( shift - 1 ) ) |
							( ( pixel->Blue & mask[level] ) >> ( shift ) );

				OctreeNode	child = _children[index];

				if( null == child )
				{
					// Create a new child node & store in the array
					// FIXME: Correct the potential overflow in the operation 'level+1' in 'OctreeNode.AddColour(Colour32*, Int32, Int32, Octree):Void'. (CA2233) 
					child = new OctreeNode( level + 1, colourBits, octree ); 
					_children[index] = child;
				}

				// Add the color to the child node
				// FIXME: Correct the potential overflow in the operation 'level+1' in 'OctreeNode.AddColour(Colour32*, Int32, Int32, Octree):Void'. (CA2233) 
				child.AddColour( pixel, colourBits, level + 1, octree );
			}

		}
		#endregion

		#region Reduce method
		/// <summary>
		/// Reduce this node by removing all of its children
		/// </summary>
		/// <returns>The number of leaves removed</returns>
		public int Reduce()
		{
			_red = _green = _blue = 0;
			int	children = 0;

			// Loop through all children and add their information to this node
			for( int index = 0; index < 8; index++ )
			{
				if( null != _children[index] )
				{
					_red += _children[index]._red;
					_green += _children[index]._green;
					_blue += _children[index]._blue;
					_pixelCount += _children[index]._pixelCount;
					++children;
					_children[index] = null;
				}
			}

			// Now change this to a leaf node
			_leaf = true;

			// Return the number of nodes to decrement the leaf count by
			return ( children - 1 );
		}
		#endregion

		#region ConstructPalette method
		/// <summary>
		/// Traverse the tree, building up the color palette
		/// </summary>
		/// <param name="palette">The palette</param>
		/// <param name="paletteIndex">The current palette index</param>
		public void ConstructPalette( ArrayList palette, 
		                              ref int paletteIndex )
		{
			if( palette == null )
			{
				throw new ArgumentNullException( "palette" );
			}
			
			if( _leaf )
			{
				// Consume the next palette index
				_paletteIndex = paletteIndex++;

				// And set the color of the palette entry
				palette.Add( Color.FromArgb( _red / _pixelCount, 
				                             _green / _pixelCount, 
				                             _blue / _pixelCount ) );
			}
			else
			{
				// Loop through children looking for leaves
				for( int index = 0; index < 8; index++ )
				{
					if( null != _children[index] )
					{
						_children[index].ConstructPalette( palette, 
						                                   ref paletteIndex );
					}
				}
			}
		}
		#endregion

		#region GetPaletteIndex method
		/// <summary>
		/// Return the palette index for the passed color
		/// </summary>
		public int GetPaletteIndex( Colour32* pixel, int level )
		{
			int	paletteIndex = _paletteIndex;

			if( !_leaf )
			{
				// FIXME: Correct the potential overflow in the operation '7-level' in 'OctreeNode.GetPaletteIndex(Colour32*, Int32):Int32'. (CA2233)
				int	shift = 7 - level;
				int index = ( ( pixel->Red & mask[level] ) >> ( shift - 2 ) ) |
							( ( pixel->Green & mask[level] ) >> ( shift - 1 ) ) |
							( ( pixel->Blue & mask[level] ) >> ( shift ) );

				if( null != _children[index] )
				{
					// FIXME: Correct the potential overflow in the operation 'level+1' in 'OctreeNode.GetPaletteIndex(Colour32*, Int32):Int32'. (CA2233)
					paletteIndex 
						= _children[index].GetPaletteIndex( pixel, 
						                                    level + 1 );
				}
				else
				{
					// TODO: test case for this?
					throw new InvalidOperationException( "Didn't expect this!" );
				}
			}

			return paletteIndex;
		}
		#endregion

		#region Increment method
		/// <summary>
		/// Increment the pixel count and add to the color information
		/// </summary>
		public void Increment( Colour32* pixel )
		{
			_pixelCount++;
			_red += pixel->Red;
			_green += pixel->Green;
			_blue += pixel->Blue;
		}
		#endregion

		#endregion

		#region properties
		
		#region NextReducible property
		/// <summary>
		/// Get/Set the next reducible node
		/// </summary>
		public OctreeNode NextReducible
		{
			get { return _nextReducible; }
			set { _nextReducible = value; }
		}
		#endregion

		#region Children property
		/// <summary>
		/// Return the child nodes
		/// </summary>
		public OctreeNode[] Children
		{
			get { return _children; }
		}
		#endregion

		#endregion

	}
}

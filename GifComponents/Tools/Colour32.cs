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
// * Moved out of Quantizer.cs into its own file
// * Changed namespace to GifComponents
#endregion

using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace GifComponents
{
	/// <summary>
	/// Struct that defines a 32 bpp colour
	/// </summary>
	/// <remarks>
	/// This struct is used to read data from a 32 bits per pixel image
	/// in memory, and is ordered in this manner as this is the way that
	/// the data is layed out in memory
	/// </remarks>
	[StructLayout(LayoutKind.Explicit)]
	internal struct Colour32
	{
		/// <summary>
		/// Holds the blue component of the colour
		/// </summary>
		[FieldOffset(0)]
		public byte Blue ;
		/// <summary>
		/// Holds the green component of the colour
		/// </summary>
		[FieldOffset(1)]
		public byte Green ;
		/// <summary>
		/// Holds the red component of the colour
		/// </summary>
		[FieldOffset(2)]
		public byte Red ;
		/// <summary>
		/// Holds the alpha component of the colour
		/// </summary>
		[FieldOffset(3)]
		public byte Alpha ;

		/// <summary>
		/// Permits the color32 to be treated as an int32
		/// </summary>
		[FieldOffset(0)]
		public int ARGB ;

		/// <summary>
		/// Return the color for this Color32 object
		/// </summary>
		public Color Colour
		{
			get	{ return Color.FromArgb( Alpha, Red, Green, Blue ); }
		}
	}
}

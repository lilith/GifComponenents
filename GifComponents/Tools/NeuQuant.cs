#region Copyright (C) Simon Bridewell, Kevin Weiner, Anthony Dekker
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
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using GifComponents.Components;

namespace GifComponents.Tools
{
	/// <summary>
	/// NeuQuant Neural-Net Quantization Algorithm
	/// ------------------------------------------
	/// 
	/// Copyright (c) 1994 Anthony Dekker
	/// 
	/// NEUQUANT Neural-Net quantization algorithm by Anthony Dekker, 1994.
	/// See "Kohonen neural networks for optimal colour quantization"
	/// in "Network: Computation in Neural Systems" Vol. 5 (1994) pp 351-367.
	/// for a discussion of the algorithm.
	/// 
	/// Any party obtaining a copy of these files from the author, directly or
	/// indirectly, is granted, free of charge, a full and unrestricted irrevocable,
	/// world-wide, paid up, royalty-free, nonexclusive right and license to deal
	/// in this software and documentation files (the "Software"), including without
	/// limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
	/// and/or sell copies of the Software, and to permit persons who receive
	/// copies from any such party to do so, with the only requirement being
	/// that this copyright notice remain intact.
	///
	/// Ported to Java 12/00 K Weiner
	/// 
	/// Modified by Simon Bridewell, June-December 2009:
	/// Downloaded from 
	/// http://www.thinkedge.com/BlogEngine/file.axd?file=NGif_src2.zip
	/// Adapted for FxCop code analysis compliance and documentation comments 
	/// converted to .net XML comments.
	/// Removed "len" parameter from constructor.
	/// </summary>
	[SuppressMessage("Microsoft.Naming", 
	                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
	                 MessageId = "Neu")]
	public class NeuQuant 
	{
		#region declarations
		private const int _netsize = 256; /* number of colours used */
		/* four primes near 500 - assume no image has a length so large */
		/* that it is divisible by all four primes */
		private const int _prime1 = 499;
		private const int _prime2 = 491;
		private const int _prime3 = 487;
		private const int _prime4 = 503;
		private const int _minpicturebytes = ( 3 * _prime4 );
		/* minimum size for input image */
		/* Program Skeleton
		   ----------------
		   [select samplefac in range 1..30]
		   [read image from input file]
		   pic = (unsigned char*) malloc(3*width*height);
		   initnet(pic,3*width*height,samplefac);
		   learn();
		   unbiasnet();
		   [write output image header, using writecolourmap(f)]
		   inxbuild();
		   write output image using inxsearch(b,g,r)      */

		/* Network Definitions
		   ------------------- */
		private const int _maxnetpos = (_netsize - 1);
		private const int _netbiasshift = 4; /* bias for colour values */
		private const int _ncycles = 100; /* no. of learning cycles */

		/* defs for freq and bias */
		private const int _intbiasshift = 16; /* bias for fractions */
		private const int _intbias = (((int) 1) << _intbiasshift);
		private const int _gammashift = 10; /* gamma = 1024 */
		private const int _gamma = (((int) 1) << _gammashift);
		private const int _betashift = 10;
		private const int _beta = (_intbias >> _betashift); /* beta = 1/1024 */
		private const int _betagamma =
			(_intbias << (_gammashift - _betashift));

		/* defs for decreasing radius factor */
		private const int _initrad = (_netsize >> 3); /* for 256 cols, radius starts */
		private const int _radiusbiasshift = 6; /* at 32.0 biased by 6 bits */
		private const int _radiusbias = (((int) 1) << _radiusbiasshift);
		private const int _initradius = (_initrad * _radiusbias); /* and decreases by a */
		private const int _radiusdec = 30; /* factor of 1/30 each cycle */

		/* defs for decreasing alpha factor */
		private const int _alphabiasshift = 10; /* alpha starts at 1.0 */
		private const int _initalpha = (((int) 1) << _alphabiasshift);

		private int _alphadec; /* biased by 10 bits */

		/* radbias and alpharadbias used for radpower calculation */
		private const int _radbiasshift = 8;
		private const int _radbias = (((int) 1) << _radbiasshift);
		private const int _alpharadbshift = (_alphabiasshift + _radbiasshift);
		private const int _alpharadbias = (((int) 1) << _alpharadbshift);

		/* Types and Global Variables
		-------------------------- */

		private byte[] _thepicture; /* the input image itself */
		private int _lengthcount; /* lengthcount = H*W*3 */

		private int _samplefac; /* sampling factor 1..30 */

		//   typedef int pixel[4];                /* BGRc */
		private int[][] _network; /* the network itself - [netsize][4] */

		private int[] _netindex = new int[256];
		/* for network lookup - really 256 */

		private int[] _bias = new int[_netsize];
		/* bias and freq arrays for learning */
		private int[] _freq = new int[_netsize];
		private int[] _radpower = new int[_initrad];
		/* radpower for precomputation */
		#endregion

		#region constructor
		/// <summary>
		/// Constructor.
		/// Initialise network in range (0,0,0) to (255,255,255) and set parameters
		/// </summary>
		/// <param name="thePicture">
		/// A collection of byte colour intensities, in the order red, green, 
		/// blue, representing the colours of each of the pixels in an image.
		/// </param>
		/// <param name="sample">
		/// Image quantization quality.
		/// </param>
		public NeuQuant( byte[] thePicture, int sample )
		{
			if( thePicture == null )
			{
				// TESTME: constructor - null picture
				throw new ArgumentNullException( "thePicture" );
			}

			int i;
			int[] p;

			_thepicture = thePicture;
			_lengthcount = thePicture.Length / 3;
			_samplefac = sample;

			_network = new int[_netsize][];
			for (i = 0; i < _netsize; i++) 
			{
				_network[i] = new int[4];
				p = _network[i];
				p[0] = p[1] = p[2] = (i << (_netbiasshift + 8)) / _netsize;
				_freq[i] = _intbias / _netsize; /* 1/netsize */
				_bias[i] = 0;
			}
		}
		#endregion

		#region BuildIndex method
		/// <summary>
		/// Insertion sort of network and building of netindex[0..255] (to do 
		/// after unbias)
		/// </summary>
		public void BuildIndex() 
		{

			int i, j, smallpos, smallval;
			int[] p;
			int[] q;
			int previouscol, startpos;

			previouscol = 0;
			startpos = 0;
			for (i = 0; i < _netsize; i++) 
			{
				p = _network[i];
				smallpos = i;
				smallval = p[1]; /* index on g */
				/* find smallest in i..netsize-1 */
				for (j = i + 1; j < _netsize; j++) 
				{
					q = _network[j];
					if (q[1] < smallval) 
					{ /* index on g */
						smallpos = j;
						smallval = q[1]; /* index on g */
					}
				}
				q = _network[smallpos];
				/* swap p (i) and q (smallpos) entries */
				if (i != smallpos) 
				{
					j = q[0];
					q[0] = p[0];
					p[0] = j;
					j = q[1];
					q[1] = p[1];
					p[1] = j;
					j = q[2];
					q[2] = p[2];
					p[2] = j;
					j = q[3];
					q[3] = p[3];
					p[3] = j;
				}
				/* smallval entry is now in position i */
				if (smallval != previouscol) 
				{
					_netindex[previouscol] = (startpos + i) >> 1;
					for (j = previouscol + 1; j < smallval; j++)
						_netindex[j] = i;
					previouscol = smallval;
					startpos = i;
				}
			}
			_netindex[previouscol] = (startpos + _maxnetpos) >> 1;
			for (j = previouscol + 1; j < 256; j++)
				_netindex[j] = _maxnetpos; /* really 256 */
		}
		#endregion
	
		#region Learn method
		/// <summary>
		/// Main Learning Loop
		/// </summary>
		public void Learn() 
		{

			int i, j, b, g, r;
			int radius, rad, alpha, step, delta, samplepixels;
			byte[] p;
			int pix, lim;

			if (_lengthcount < _minpicturebytes)
				_samplefac = 1;
			_alphadec = 30 + ((_samplefac - 1) / 3);
			p = _thepicture;
			pix = 0;
			lim = _lengthcount;
			samplepixels = _lengthcount / (3 * _samplefac);
			delta = samplepixels / _ncycles;
			alpha = _initalpha;
			radius = _initradius;

			rad = radius >> _radiusbiasshift;
			if (rad <= 1)
				rad = 0; // TESTME: Learn - rad <= 1
			for (i = 0; i < rad; i++)
				_radpower[i] =
					alpha * (((rad * rad - i * i) * _radbias) / (rad * rad));

			//fprintf(stderr,"beginning 1D learning: initial radius=%d\n", rad);

			if (_lengthcount < _minpicturebytes)
				step = 3;
			else if ((_lengthcount % _prime1) != 0)
				step = 3 * _prime1;
			else 
			{
				// TESTME: Learn - _lengthcount >= _minpicturebytes and divisible by _prime1
				if ((_lengthcount % _prime2) != 0)
					step = 3 * _prime2;
				else 
				{
					if ((_lengthcount % _prime3) != 0)
						step = 3 * _prime3;
					else
						step = 3 * _prime4;
				}
			}

			i = 0;
			while (i < samplepixels) 
			{
				b = (p[pix + 0] & 0xff) << _netbiasshift;
				g = (p[pix + 1] & 0xff) << _netbiasshift;
				r = (p[pix + 2] & 0xff) << _netbiasshift;
				j = Contest(b, g, r);

				Altersingle(alpha, j, b, g, r);
				if (rad != 0)
					Alterneigh(rad, j, b, g, r); /* alter neighbours */

				pix += step;
				if (pix >= lim)
					pix -= _lengthcount;

				i++;
				if (delta == 0)
					delta = 1;
				if (i % delta == 0) 
				{
					alpha -= alpha / _alphadec;
					radius -= radius / _radiusdec;
					rad = radius >> _radiusbiasshift;
					if (rad <= 1)
						rad = 0;
					for (j = 0; j < rad; j++)
						_radpower[j] =
							alpha * (((rad * rad - j * j) * _radbias) / (rad * rad));
				}
			}
			//fprintf(stderr,"finished 1D learning: readonly alpha=%f !\n",((float)alpha)/initalpha);
		}
		#endregion
	
		#region Map method
		/// <summary>
		/// Search for BGR values 0..255 (after net is unbiased) and return 
		/// the index in the colour table of the colour closest to the supplied
		/// colour.
		/// </summary>
		/// <param name="blue">
		/// The blue component of the input colour.
		/// </param>
		/// <param name="green">
		/// The green component of the input colour.
		/// </param>
		/// <param name="red">
		/// The red component of the input colour.
		/// </param>
		/// <returns>
		/// The index in the colour table of the colour closest to the supplied
		/// colour.
		/// </returns>
		public int Map(int blue, int green, int red) 
		{

			int i, j, dist, a, bestd;
			int[] p;
			int best;

			bestd = 1000; /* biggest possible dist is 256*3 */
			best = -1;
			i = _netindex[green]; /* index on g */
			j = i - 1; /* start at netindex[g] and work outwards */

			while ((i < _netsize) || (j >= 0)) 
			{
				if (i < _netsize) 
				{
					p = _network[i];
					dist = p[1] - green; /* inx key */
					if (dist >= bestd)
						i = _netsize; /* stop iter */
					else 
					{
						i++;
						if (dist < 0)
							dist = -dist;
						a = p[0] - blue;
						if (a < 0)
							a = -a;
						dist += a;
						if (dist < bestd) 
						{
							a = p[2] - red;
							if (a < 0)
								a = -a;
							dist += a;
							if (dist < bestd) 
							{
								bestd = dist;
								best = p[3];
							}
						}
					}
				}
				if (j >= 0) 
				{
					p = _network[j];
					dist = green - p[1]; /* inx key - reverse dif */
					if (dist >= bestd)
						j = -1; /* stop iter */
					else 
					{
						j--;
						if (dist < 0)
							dist = -dist; // TESTME: Map - dist < 0
						a = p[0] - blue;
						if (a < 0)
							a = -a;
						dist += a;
						if (dist < bestd) 
						{
							a = p[2] - red;
							if (a < 0)
								a = -a;
							dist += a;
							if (dist < bestd) 
							{
								bestd = dist;
								best = p[3];
							}
						}
					}
				}
			}
			return (best);
		}
		#endregion

		#region Process method
		/// <summary>
		/// Calls the Learn, UnbiasNetwork and BuildIndex method and returns the
		/// ColorMap.
		/// </summary>
		/// <returns>
		/// The colour table containing the colours of the image after 
		/// quantization.
		/// </returns>
		public ColourTable Process()
		{
			Learn();
			UnbiasNetwork();
			BuildIndex();
			return ColourMap();
		}
		#endregion
	
		#region UnbiasNetwork method
		/// <summary>
		/// Unbias network to give byte values 0..255 and record position i to 
		/// prepare for sort
		/// </summary>
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
		                 MessageId = "Unbias")]
		public void UnbiasNetwork() 
		{

			int i;

			for (i = 0; i < _netsize; i++) 
			{
				_network[i][0] >>= _netbiasshift;
				_network[i][1] >>= _netbiasshift;
				_network[i][2] >>= _netbiasshift;
				_network[i][3] = i; /* record colour no */
			}
		}
		#endregion

		#region private methods
		
		#region private Alterneigh method
		/// <summary>
		/// Move adjacent neurons by precomputed alpha*(1-((i-j)^2/[r]^2)) in 
		/// radpower[|i-j|]
		/// </summary>
		/// <param name="rad"></param>
		/// <param name="i"></param>
		/// <param name="b"></param>
		/// <param name="g"></param>
		/// <param name="r"></param>
		private void Alterneigh(int rad, int i, int b, int g, int r) 
		{

			int j, k, lo, hi, a, m;
			int[] p;

			lo = i - rad;
			if (lo < -1)
				lo = -1;
			hi = i + rad;
			if (hi > _netsize)
				hi = _netsize;

			j = i + 1;
			k = i - 1;
			m = 1;
			while ((j < hi) || (k > lo)) 
			{
				a = _radpower[m++];
				if (j < hi) 
				{
					p = _network[j++];
					try 
					{
						p[0] -= (a * (p[0] - b)) / _alpharadbias;
						p[1] -= (a * (p[1] - g)) / _alpharadbias;
						p[2] -= (a * (p[2] - r)) / _alpharadbias;
					} 
					catch( Exception ex ) 
					{
						// FXCOP: Modify 'NeuQuant.Alterneigh(Int32, Int32, Int32, Int32, Int32):Void' to catch a more specific exception than 'System.Exception' or rethrow the exception. (CA1031)
						// REDUNDANT: is this exception handling needed?
						Utils.Handle( ex );
						throw;
					} // prevents 1.3 miscompilation
				}
				if (k > lo) 
				{
					p = _network[k--];
					try 
					{
						p[0] -= (a * (p[0] - b)) / _alpharadbias;
						p[1] -= (a * (p[1] - g)) / _alpharadbias;
						p[2] -= (a * (p[2] - r)) / _alpharadbias;
					} 
					catch( Exception ex ) 
					{
						// FXCOP: Modify 'NeuQuant.Alterneigh(Int32, Int32, Int32, Int32, Int32):Void' to catch a more specific exception than 'System.Exception' or rethrow the exception. (CA1031)
						// REDUNDANT: is this exception handling needed?
						Utils.Handle( ex );
						throw;
					}
				}
			}
		}
		#endregion
	
		#region private Altersingle method
		/// <summary>
		/// Move neuron i towards biased (b,g,r) by factor alpha
		/// </summary>
		/// <param name="alpha"></param>
		/// <param name="i"></param>
		/// <param name="b"></param>
		/// <param name="g"></param>
		/// <param name="r"></param>
		private void Altersingle(int alpha, int i, int b, int g, int r) 
		{

			/* alter hit neuron */
			int[] n = _network[i];
			n[0] -= (alpha * (n[0] - b)) / _initalpha;
			n[1] -= (alpha * (n[1] - g)) / _initalpha;
			n[2] -= (alpha * (n[2] - r)) / _initalpha;
		}
		#endregion
	
		#region private Contest method
		/// <summary>
		/// Search for biased BGR values
		/// </summary>
		/// <param name="b"></param>
		/// <param name="g"></param>
		/// <param name="r"></param>
		/// <returns></returns>
		private int Contest(int b, int g, int r) 
		{

			/* finds closest neuron (min dist) and updates freq */
			/* finds best neuron (min dist-bias) and returns position */
			/* for frequently chosen neurons, freq[i] is high and bias[i] is negative */
			/* bias[i] = gamma*((1/netsize)-freq[i]) */

			int i, dist, a, biasdist, betafreq;
			int bestpos, bestbiaspos, bestd, bestbiasd;
			int[] n;

			bestd = ~(((int) 1) << 31);
			bestbiasd = bestd;
			bestpos = -1;
			bestbiaspos = bestpos;

			for (i = 0; i < _netsize; i++) 
			{
				n = _network[i];
				dist = n[0] - b;
				if (dist < 0)
					dist = -dist;
				a = n[1] - g;
				if (a < 0)
					a = -a;
				dist += a;
				a = n[2] - r;
				if (a < 0)
					a = -a;
				dist += a;
				if (dist < bestd) 
				{
					bestd = dist;
					bestpos = i;
				}
				biasdist = dist - ((_bias[i]) >> (_intbiasshift - _netbiasshift));
				if (biasdist < bestbiasd) 
				{
					bestbiasd = biasdist;
					bestbiaspos = i;
				}
				betafreq = (_freq[i] >> _betashift);
				_freq[i] -= betafreq;
				_bias[i] += (betafreq << _gammashift);
			}
			_freq[bestpos] += _beta;
			_bias[bestpos] -= _betagamma;
			return (bestbiaspos);
		}
		#endregion

		#region private ColorMap method
		/// <summary>
		/// ColorMap method.
		/// </summary>
		/// <returns>
		/// A colour table containing up to 256 colours, being the colours of
		/// the image after quantization.
		/// </returns>
		private ColourTable ColourMap()
		{
			ColourTable map = new ColourTable();
			int[] index = new int[_netsize];
			for( int i = 0; i < _netsize; i++ )
			{
				index[_network[i][3]] = i;
			}
			for( int i = 0; i < _netsize; i++ ) 
			{
				int j = index[i];
				map.Add( Color.FromArgb( 255, 
				                         _network[j][0], 
				                         _network[j][1], 
				                         _network[j][2] ) );
			}
			return map;
		}
		#endregion
	
		#endregion
	}
}
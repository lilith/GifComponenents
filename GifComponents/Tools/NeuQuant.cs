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
using CommonForms.Responsiveness;
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
	/// http://members.ozemail.com.au/~dekker/NeuQuant.pdf
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
	/// Modified by Simon Bridewell, June 2009 - April 2010:
	/// Downloaded from 
	/// http://www.thinkedge.com/BlogEngine/file.axd?file=NGif_src2.zip
	/// * Adapted for FxCop code analysis compliance and documentation comments 
	///   converted to .net XML comments.
	/// * Removed "len" parameter from constructor.
	/// * Added AnalysingPixel property.
	/// * Made Learn, UnbiasNetwork and BuildIndex methods private.
	/// * Derive from LongRunningProcess to allow use of ProgressCounters.
	/// * Renamed lots of private members, local variables and methods.
	/// * Refactored some code into separate methods:
	/// 	GetPixelIncrement
	/// 	ManhattanDistance
	/// 	MoveNeuron
	/// 	MoveNeighbouringNeurons
	/// 	MoveNeighbour
	/// 	SetNeighbourhoodAlphas
	/// * Added lots of comments.
	/// TODO: consider a separate Neuron class - R, G, B, frequency and bias properties?
	/// TODO: consider a separate NeuralNetwork class, not specific to quantizing images
	/// </summary>
	[SuppressMessage("Microsoft.Naming", 
	                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
	                 MessageId = "Neu")]
	public class NeuQuant : LongRunningProcess
	{
		#region constants
		
		/// <summary>
		/// Number of neurons in the neural network
		/// -- or (in this implementation) --
		/// Maximum number of colours in the output image.
		/// </summary>
		private const int _neuronCount = 256;
		
		// Four primes near 500 - assume no image has a length so large
		// that it is divisible by all four primes
		private const int _prime1 = 499;
		private const int _prime2 = 491;
		private const int _prime3 = 487;
		private const int _prime4 = 503;
		
		/// <summary>
		/// Minimum size for input image.
		/// If the image has fewer pixels than this then the learning loop will
		/// step through its pixels 3 at a time rather than using one of the
		/// four prime constants.
		/// </summary>
		private const int _minpicturebytes = ( 3 * _prime4 );

		/* Network Definitions
		   ------------------- */
		
		/// <summary>
		/// Highest possibly index for a neuron in the neural network
		/// (the network is a zero-based array).
		/// </summary>
		private const int _highestNeuronIndex = _neuronCount - 1;
		
		/// <summary>
		/// Bias for colour values
		/// TODO: find a better description / name for _netbiasshift
		/// </summary>
		private const int _netbiasshift = 4; /* bias for colour values */
		
		/// <summary>
		/// Number of learning cycles.
		/// </summary>
		private const int _numberOfLearningCycles = 100; /* no. of learning cycles */

		#region constants for frequency and bias
		
		/// <summary>
		/// This affects the value of _intbias
		/// TODO: find a better description / name for _intbiasshift
		/// </summary>
		private const int _intbiasshift = 16; /* bias for fractions */
		
		/// <summary>
		/// 1 * (2 to the power of (_intbiasshift - 1))
		/// TODO: find a better description / name for _intbias
		/// </summary>
		private const int _intbias = (((int) 1) << _intbiasshift);
		private const int _gammashift = 10; /* gamma = 1024 */
		private const int _gamma = (((int) 1) << _gammashift);
		private const int _betashift = 10;
		private const int _beta = (_intbias >> _betashift); /* beta = 1/1024 */
		private const int _betagamma =
			(_intbias << (_gammashift - _betashift));
		#endregion

		#region constants for decreasing radius factor
		/* defs for decreasing radius factor */
		private const int _initrad = (_neuronCount >> 3); /* for 256 cols, radius starts */
		private const int _radiusbiasshift = 6; /* at 32.0 biased by 6 bits */
		private const int _radiusbias = (((int) 1) << _radiusbiasshift);
		private const int _initradius = (_initrad * _radiusbias); /* and decreases by a */
		private const int _radiusdec = 30; /* factor of 1/30 each cycle */
		#endregion

		#region constants for decreasing alpha factor
		/// <summary>
		/// The initial value of alpha will be set to 1, left shifted by this
		/// many bits.
		/// </summary>
		private const int _alphaBiasShift = 10;
		
		/// <summary>
		/// The starting value of alpha.
		/// Alpha is a factor which controls how far neurons are moved during
		/// the learning loop, and it decreases as learning proceeds.
		/// </summary>
		private const int _initialAlpha = (((int) 1) << _alphaBiasShift);

		/// <summary>
		/// Controls how much alpha decreases by at each step of the learning
		/// loop - alpha will be decremented by alpha divided by this value, so
		/// the larger this value, the more slowly alpha will be reduced.
		/// </summary>
		private int _alphaDecrement;
		#endregion

		/* radbias and alpharadbias used for radpower calculation */
		private const int _radbiasshift = 8;
		private const int _radbias = (((int) 1) << _radbiasshift);
		private const int _alpharadbshift = (_alphaBiasShift + _radbiasshift);
		private const int _alpharadbias = (((int) 1) << _alpharadbshift);

		#endregion

		#region declarations
		
		/* Types and Global Variables
		-------------------------- */

		/// <summary>
		/// A collection of byte colour intensities, in the order red, green, 
		/// blue, representing the colours of each of the pixels in an image
		/// to be quantized.
		/// </summary>
		private byte[] _thePicture; /* the input image itself */
		
		/// <summary>
		/// Number of pixels in the input image.
		/// </summary>
		private int _pixelCount; /* lengthcount = H*W*3 */

		/// <summary>
		/// Sampling factor 1-30. Supplied to constructor unless input image is
		/// very small.
		/// The larger _samplingFactor is, the slower the value of alpha will 
		/// be reduced during the learning loop.
		/// TODO: comment on _samplingFactor impact on samplepixels.
		/// </summary>
		private int _samplingFactor;

		//   typedef int pixel[4];                /* BGRc */
		/// <summary>
		/// The neural network used to quantize the image.
		/// This is a two-dimensional array, with each element of the first 
		/// dimension representing one of the colours in the colour table of 
		/// the quantized output image, amd the elements of the second dimension 
		/// representing the blue, green and red components of those colours, 
		/// and ??? TODO: 4th element???
		/// </summary>
		private int[][] _network; /* the network itself - [netsize][4] */

		private int[] _netindex = new int[256];
		/* for network lookup - really 256 */

		/* bias and freq arrays for learning */
		
		/// <summary>
		/// Array of biases for each neuron.
		/// TODO: better description for _bias array
		/// </summary>
		private int[] _bias = new int[_neuronCount];

		/// <summary>
		/// Array of frequencies for each neuron.
		/// TODO: better description for frequency array, rename from _freq
		/// </summary>
		private int[] _freq = new int[_neuronCount];
		
		/// <summary>
		/// Alpha values controlling how far towards a target colour any 
		/// neighbouring are moved.
		/// </summary>
		private int[] _neighbourhoodAlphas = new int[_initrad];
		
		#endregion

		#region constructor
		/// <summary>
		/// Constructor.
		/// Initialise network in range (0,0,0) to (255,255,255) and set parameters
		/// TODO: consider accepting a collection / array of Colors instead of
		/// a byte array.
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
			
			int[] thisNeuron;

			_thePicture = thePicture;
			_pixelCount = thePicture.Length / 3;
			_samplingFactor = sample;

			// Initialise the number of neurons in the neural network to the 
			// maximum number of colours allowed in the output image.
			_network = new int[_neuronCount][];
			
			for( int neuronIndex = 0; neuronIndex < _neuronCount; neuronIndex++ ) 
			{
				// Initialise the size of the second dimension of the neural 
				// network to 4. // TODO: why 4? 3 colour intensities + ???
				_network[neuronIndex] = new int[4];
				
				// Make a reference to this neuron
				thisNeuron = _network[neuronIndex];
				
				// Initialise the values of the neurons so that they lie on a 
				// diagonal line from 0,0,0 to 4080,4080,4080. They will be 
				// adjusted during the learning loop as the pixels of the image
				// are analysed.
				thisNeuron[0] = thisNeuron[1] = thisNeuron[2] 
					= (neuronIndex << (_netbiasshift + 8)) / _neuronCount;
				
				// Set the frequency of this neuron to _intbias divided by the
				// number of neurons.
				// TODO: better explanation of this step.
				_freq[neuronIndex] = _intbias / _neuronCount; /* 1/netsize */
				
				// Set the bias of this neuron to zero.
				// TODO: what is the bias?
				_bias[neuronIndex] = 0;
			}
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

			while ((i < _neuronCount) || (j >= 0)) 
			{
				if (i < _neuronCount) 
				{
					p = _network[i];
					dist = p[1] - green; /* inx key */
					if (dist >= bestd)
						i = _neuronCount; /* stop iter */
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

		#region private methods
		
		#region Learn method and methods called by it
		
		#region private Learn method
		/// <summary>
		/// Main Learning Loop
		/// </summary>
		private void Learn() 
		{
			int closestNeuronIndex, blue, green, red;
			int radius, delta, samplepixels;
			byte[] p;
			int pixelIndex, lim;

			// TODO: what is _minpicturebytes and why do we set the sampling factor to 1 if we have fewer pixels than that?
			if( _pixelCount < _minpicturebytes )
			{
				_samplingFactor = 1;
			}
			
			// The larger _alphaDecrement is, the slower the value of alpha will 
			// be reduced during the learning process.
			// Therefore the larger _samplingFactor is, the slower the value of
			// aplha will be reduced.
			_alphaDecrement = 30 + ((_samplingFactor - 1) / 3);
			
			// TODO: why take a local copy of _thePicture?
			p = _thePicture;
			
			// TODO: what is pix?
			pixelIndex = 0;
			
			// TODO: why make a local copy of _pixelCount?
			lim = _pixelCount;
			
			// TODO: why set samplepixels to _pixelCount / (3*_samplingFactor)?
			samplepixels = _pixelCount / (3 * _samplingFactor);
			
			// Allows a ResponsiveForm to track the progress of this method
			string learnCounterText = "Neural net quantizer - learning";
			AddCounter( learnCounterText, samplepixels );
			
			// TODO: what is delta and why is it set to this?
			delta = samplepixels / _numberOfLearningCycles;
			
			// Alpha is a factor which controls how far neurons are moved during
			// the learning loop, and it decreases as learning proceeds.
			int alpha = _initialAlpha;
			
			// TODO: what is radius?
			radius = _initradius;

			// Set the size of the neighbourhood which makes up the neighbouring
			// neurons which also need to be moved when a neuron is moved.
			int neighbourhoodSize = radius >> _radiusbiasshift;
			if( neighbourhoodSize <= 1 )
			{
				neighbourhoodSize = 0; // TESTME: Learn - neighbourhoodSize <= 1
			}
			
			// Set the initial alpha values for neighbouring neurons.
			SetNeighbourhoodAlphas( neighbourhoodSize, alpha );

			int step = GetPixelIndexIncrement();

			int pixelsExamined = 0;
			while( pixelsExamined < samplepixels ) 
			{
				// Update the progress counter for the benefit of the UI
				MyProgressCounters[learnCounterText].Value = pixelsExamined;
				
				// TODO: what is pix?
				blue = (p[pixelIndex + 0] & 0xff) << _netbiasshift;
				green = (p[pixelIndex + 1] & 0xff) << _netbiasshift;
				red = (p[pixelIndex + 2] & 0xff) << _netbiasshift;
				// TODO: investigate the Contest method, particularly what it's return value actually represents
				closestNeuronIndex = Contest( blue, green, red );

				// Move this neuron closer to the colour of the current pixel
				// by a factor of alpha.
				MoveNeuron( alpha, closestNeuronIndex, blue, green, red );
				
				// If appropriate, move neighbouring neurons closer to the 
				// colour of the current pixel.
				if( neighbourhoodSize != 0 )
				{
					MoveNeighbouringNeurons( neighbourhoodSize, 
					                         closestNeuronIndex, 
					                         blue, 
					                         green, 
					                         red );
				}

				// Move on to the next pixel to be examined
				pixelIndex += step;
				if( pixelIndex >= lim )
				{
					// We've gone past the end of the image, so wrap round to
					// the start again
					pixelIndex -= _pixelCount;
				}

				// Keep track of how many pixels have been examined so far
				pixelsExamined++;
				
				if( delta == 0 )
				{
					// TODO: why does delta need to be 1 instead of 0?
					delta = 1;
				}
				
				if( pixelsExamined % delta == 0 ) 
				{
					// pixelsExamined is divisible by delta
					// TODO: what is delta? better names/comments needed
					alpha -= alpha / _alphaDecrement;
					radius -= radius / _radiusdec;
					neighbourhoodSize = radius >> _radiusbiasshift;
					
					if( neighbourhoodSize <= 1 )
					{
						// TODO: why does rad need to be zero or greater?
						neighbourhoodSize = 0;
					}
					
					// Update the alpha values to be used for moving 
					// neighbouring neurons.
					SetNeighbourhoodAlphas( neighbourhoodSize, alpha );
				}
			}
			
			// This method is finished now so the UI doesn't need to monitor it
			// any more.
			RemoveCounter( learnCounterText );
		}
		#endregion
		
		#region private SetNeighbourhoodAlphas method
		/// <summary>
		/// Sets the alpha values for moving neighbouring neurons.
		/// </summary>
		private void SetNeighbourhoodAlphas( int neighbourhoodSize, int alpha )
		{
			int neighbourhoodSizeSquared = neighbourhoodSize * neighbourhoodSize;
			for( int alphaIndex = 0; alphaIndex < neighbourhoodSize; alphaIndex++ )
			{
				_neighbourhoodAlphas[alphaIndex] =
					alpha 
					* 
					(
						(
							(
								neighbourhoodSizeSquared
								- 
								( alphaIndex * alphaIndex )
							) 
							* 
							_radbias
						) 
						/ 
						neighbourhoodSizeSquared
					);
			}
		}
		#endregion
		
		#region private GetPixelIndexIncrement method
		/// <summary>
		/// Calculates an increment to step through the pixels of the image, 
		/// such that all pixels will eventually be examined, but not 
		/// sequentially.
		/// This is required because the learning loop needs to examine the
		/// pixels in a psuedo-random order.
		/// </summary>
		/// <returns>The increment.</returns>
		private int GetPixelIndexIncrement()
		{
			int step;
			if( _pixelCount < _minpicturebytes )
			{
				step = 3;
			}
			else if( (_pixelCount % _prime1) != 0 )
			{
				// The number of pixels is not divisible by the first prime number
				step = 3 * _prime1;
			}
			else if( (_pixelCount % _prime2) != 0 )
			{
				// TESTME: GetPixelIndexIncrement - _lengthcount >= _minpicturebytes and divisible by _prime1
				// The number of pixels is not divisible by the second prime number
				step = 3 * _prime2;
			}
			else if( (_pixelCount % _prime3) != 0 )
			{
				// The number of pixels is not divisible by the third prime number
				step = 3 * _prime3;
			}
			else
			{
				// The number of pixels is divisible by the first, second and
				// third prime numbers.
				step = 3 * _prime4;
			}
			return step;
		}
		#endregion
		
		#region private Contest method and methods called by it
		
		#region private Contest method
		/// <summary>
		/// Search for biased BGR values
		/// TODO: comment and understand (and rename?) Contest method
		/// </summary>
		/// <param name="blue">The blue component of the colour</param>
		/// <param name="green">The green component of the colour</param>
		/// <param name="red">The red component of the colour</param>
		/// <returns>
		/// TODO: what does the return value of Contest represent?
		/// </returns>
		private int Contest( int blue, int green, int red ) 
		{

			/* finds closest neuron (min dist) and updates freq */
			/* finds best neuron (min dist-bias) and returns position */
			/* for frequently chosen neurons, freq[i] is high and bias[i] is negative */
			/* bias[i] = gamma*((1/netsize)-freq[i]) */

			int distance, biasdist, betafreq;
			int[] thisNeuron;

			// Initialise closest neuron distance and its index in the network
			int closestDistance = ~(((int) 1) << 31); // bitwise complement of 2^31 - returns 2147483647
			int bestBiasDistance = closestDistance;
			int closestNeuronIndex = -1;
			int bestBiasNeuronIndex = closestNeuronIndex;

			for( int neuronIndex = 0; neuronIndex < _neuronCount; neuronIndex++ ) 
			{
				// How close is this neuron to the supplied colour?
				thisNeuron = _network[neuronIndex];
				distance = ManhattanDistance( thisNeuron, red, green, blue );
				
				// If it's closer than the closest one found so far, then it's
				// now the closest one.
				if( distance < closestDistance ) 
				{
					closestDistance = distance;
					closestNeuronIndex = neuronIndex;
				}
				
				// TODO: what's happening here with biases?
				biasdist = distance - ((_bias[neuronIndex]) >> (_intbiasshift - _netbiasshift));
				if( biasdist < bestBiasDistance ) 
				{
					bestBiasDistance = biasdist;
					bestBiasNeuronIndex = neuronIndex;
				}
				
				// TODO: what do we mean by beta and gamma here?
				betafreq = _freq[neuronIndex] >> _betashift;
				_freq[neuronIndex] -= betafreq;
				_bias[neuronIndex] += (betafreq << _gammashift);
			}
			
			// TODO: what do _freq and _bias represent?
			_freq[closestNeuronIndex] += _beta;
			_bias[closestNeuronIndex] -= _betagamma;
			return bestBiasNeuronIndex;
		}
		#endregion
		
		#region private static ManhattanDistance method
		/// <summary>
		/// Calculates how close the colour represented by the supplied red,
		/// green and blue values is to the colour held in the supplied neuron.
		/// </summary>
		/// <param name="neuron">
		/// A neuron with blue, green and red values held in its first three
		/// elements.
		/// </param>
		/// <param name="red">The red intensity of the supplied colour</param>
		/// <param name="green">The green intensity of the supplied colour</param>
		/// <param name="blue">The blue intensity of the supplied colour</param>
		/// <returns>The distance between the two colours</returns>
		private static int ManhattanDistance( int[] neuron, int red, int green, int blue )
		{
			int distance 
				= Math.Abs( neuron[0] - blue )
				+ Math.Abs( neuron[1] - green )
				+ Math.Abs( neuron[2] - red );
			return distance;
		}
		#endregion
		
		#endregion
		
		#region private MoveNeuron method
		/// <summary>
		/// Moves the neuron at the supplied index in the neural network closer
		/// to the target colour by a factor of alpha.
		/// </summary>
		/// <param name="alpha">
		/// The greater this parameter, the more the neuron will be moved in
		/// the direction of the target colour.
		/// </param>
		/// <param name="neuronIndex">
		/// Index in the neural network of the neuron to be moved.
		/// </param>
		/// <param name="blue">The blue component of the target colour</param>
		/// <param name="green">The green component of the target colour</param>
		/// <param name="red">The red component of the target colour</param>
		/// <remarks>This method was originally called Altersingle</remarks>
		private void MoveNeuron( int alpha, 
		                         int neuronIndex,
		                         int blue,
		                         int green,
		                         int red )
		{
			int[] thisNeuron = _network[neuronIndex];
			thisNeuron[0] -= (alpha * (thisNeuron[0] - blue)) / _initialAlpha;
			thisNeuron[1] -= (alpha * (thisNeuron[1] - green)) / _initialAlpha;
			thisNeuron[2] -= (alpha * (thisNeuron[2] - red)) / _initialAlpha;
		}
		#endregion
	
		#region private MoveNeighbouringNeurons method
		/// <summary>
		/// Moves neighbours of the neuron at the supplied index in the network
		/// closer to the supplied target colour.
		/// </summary>
		/// <param name="neighbourhoodSize">
		/// Size of the neighbourhood which makes up the neurons to be moved.
		/// For example, if this parameter is set to 3 then the 2 neurons before
		/// and the 2 after the supplied neuron index will be moved.
		/// </param>
		/// <param name="neuronIndex">
		/// The index in the network of the neuron whose neighbours need moving.
		/// </param>
		/// <param name="blue">
		/// The blue component of the target colour.
		/// </param>
		/// <param name="green">
		/// The green component of the target colour.
		/// </param>
		/// <param name="red">
		/// The red component of the target colour.
		/// </param>
		/// <remarks>This method was originally called Alterneigh.</remarks>
		private void MoveNeighbouringNeurons( int neighbourhoodSize, 
		                                      int neuronIndex,
		                                      int blue,
		                                      int green,
		                                      int red )
		{
			int lowNeuronIndexLimit = neuronIndex - neighbourhoodSize;
			if( lowNeuronIndexLimit < -1 )
			{
				// lowNeuronIndexLimit cannot be less than -1 as it is the lower 
				// limit for an array index.
				lowNeuronIndexLimit = -1;
			}
			int highNeuronIndexLimit = neuronIndex + neighbourhoodSize;
			if( highNeuronIndexLimit > _neuronCount )
			{
				// highNeuronIndexLimit cannot be more than the size of the array 
				// being accessed.
				highNeuronIndexLimit = _neuronCount;
			}

			int highNeuronIndex = neuronIndex + 1;
			int lowNeuronIndex = neuronIndex - 1;
			int neighbourAlphaIndex = 1;
			
			// Loop through the neighbouring neurons, starting with those 
			// on either side of the supplied neuron, moving them closer to the
			// target colour.
			while( (highNeuronIndex < highNeuronIndexLimit) || (lowNeuronIndex > lowNeuronIndexLimit) ) 
			{
				int neighbourAlpha = _neighbourhoodAlphas[neighbourAlphaIndex++];
				if( highNeuronIndex < highNeuronIndexLimit ) 
				{
					MoveNeighbour( highNeuronIndex++, neighbourAlpha, blue, green, red );
				}
				if( lowNeuronIndex > lowNeuronIndexLimit ) 
				{
					MoveNeighbour( lowNeuronIndex--, neighbourAlpha, blue, green, red );
				}
			}
		}
		#endregion
		
		#region private MoveNeighbour method
		/// <summary>
		/// Moves an individual neighbouring neuron closer to the supplied 
		/// target colour by a factor of alpha.
		/// </summary>
		/// <param name="neuronIndex">
		/// The index in the network of the neighbour to be moved.
		/// </param>
		/// <param name="alpha">
		/// Controls how far towards the target colour the neuron is to be moved.
		/// </param>
		/// <param name="blue">The blue component of the target colour.</param>
		/// <param name="green">The green component of the target colour.</param>
		/// <param name="red">The red component of the target colour.</param>
		private void MoveNeighbour( int neuronIndex, int alpha, int blue, int green, int red )
		{
			int[] thisNeuron = _network[neuronIndex];
			thisNeuron[0] -= (alpha * (thisNeuron[0] - blue)) / _alpharadbias;
			thisNeuron[1] -= (alpha * (thisNeuron[1] - green)) / _alpharadbias;
			thisNeuron[2] -= (alpha * (thisNeuron[2] - red)) / _alpharadbias;
		}
		#endregion
		
		#endregion
		
		#region UnbiasNetwork method
		/// <summary>
		/// Unbias network to give byte values 0..255 and record position i to 
		/// prepare for sort
		/// </summary>
		[SuppressMessage("Microsoft.Naming", 
		                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
		                 MessageId = "Unbias")]
		private void UnbiasNetwork() 
		{
			// Allows a ResponsiveForm to track the progress of this method
			string unbiasCounterText = "Neural net quantizer - unbiasing network";
			AddCounter( unbiasCounterText, _neuronCount );
			for( int thisNeuronIndex = 0; thisNeuronIndex < _neuronCount; thisNeuronIndex++ ) 
			{
				// Update the progress counter for the benefit of the UI
				MyProgressCounters[unbiasCounterText].Value = thisNeuronIndex;
				
				// Shift the values of this neuron's r, g, b values right by the value of _netbiasshift
				// TODO: what is _netbiasshift?
				_network[thisNeuronIndex][0] >>= _netbiasshift;
				_network[thisNeuronIndex][1] >>= _netbiasshift;
				_network[thisNeuronIndex][2] >>= _netbiasshift;
				
				// Record this neuron's index in the third element of its array
				// TODO: why does this neuron need to know its own index now?
				_network[thisNeuronIndex][3] = thisNeuronIndex; /* record colour no */
			}
			
			// This method has finished so remove its progress counter
			RemoveCounter( unbiasCounterText );
		}
		#endregion

		#region BuildIndex method
		/// <summary>
		/// Insertion sort of network and building of netindex[0..255] (to do 
		/// after unbias)
		/// TODO: better description for BuildIndex method
		/// TODO: would this be better as a .net collection sort?
		/// </summary>
		private void BuildIndex() 
		{
			// Allows a ResponsiveForm to track the progress of this method
			string buildIndexCounterText = "Neural net quantizer - building index";
			AddCounter( buildIndexCounterText, _neuronCount );

			int otherNeuronIndex, smallpos, smallval;
			int[] thisNeuron;
			int[] otherNeuron;

			int previouscol = 0;
			int startpos = 0;
			for( int thisNeuronIndex = 0; 
			     thisNeuronIndex < _neuronCount; 
			     thisNeuronIndex++ )
			{
				// Update the progress counter for the benefit of the UI
				MyProgressCounters[buildIndexCounterText].Value = thisNeuronIndex;
				
				thisNeuron = _network[thisNeuronIndex];
				smallpos = thisNeuronIndex;
				smallval = thisNeuron[1]; /* index on g */
				/* find smallest in i..netsize-1 */
				for( otherNeuronIndex = thisNeuronIndex + 1; 
				     otherNeuronIndex < _neuronCount; 
				     otherNeuronIndex++ )
				{
					otherNeuron = _network[otherNeuronIndex];
					if (otherNeuron[1] < smallval) 
					{ /* index on g */
						smallpos = otherNeuronIndex;
						smallval = otherNeuron[1]; /* index on g */
					}
				}
				otherNeuron = _network[smallpos];
				// TODO: this is apparently where the two neurons swap places
				/* swap p (i) and q (smallpos) entries */
				if (thisNeuronIndex != smallpos) 
				{
					otherNeuronIndex	= otherNeuron[0];
					otherNeuron[0]		= thisNeuron[0];
					thisNeuron[0]		= otherNeuronIndex;
					otherNeuronIndex	= otherNeuron[1];
					otherNeuron[1]		= thisNeuron[1];
					thisNeuron[1]		= otherNeuronIndex;
					otherNeuronIndex	= otherNeuron[2];
					otherNeuron[2]		= thisNeuron[2];
					thisNeuron[2]		= otherNeuronIndex;
					otherNeuronIndex	= otherNeuron[3];
					otherNeuron[3]		= thisNeuron[3];
					thisNeuron[3]		= otherNeuronIndex;
				}
				/* smallval entry is now in position i */
				// TODO: examine this loop - what are smallval and previouscol?
				if (smallval != previouscol) 
				{
					_netindex[previouscol] = (startpos + thisNeuronIndex) >> 1;
					for (otherNeuronIndex = previouscol + 1; otherNeuronIndex < smallval; otherNeuronIndex++)
						_netindex[otherNeuronIndex] = thisNeuronIndex;
					previouscol = smallval;
					startpos = thisNeuronIndex;
				}
			}
			
			// TODO: why are we updating _netindex[previouscol] here?
			_netindex[previouscol] = (startpos + _highestNeuronIndex) >> 1;
			
			// TODO: use a new variable instead of otherNeuronIndex here?
			for( otherNeuronIndex = previouscol + 1; 
			     otherNeuronIndex < 256; 
			     otherNeuronIndex++)
			{
				_netindex[otherNeuronIndex] = _highestNeuronIndex; /* really 256 */
			}

			// This method has finished so remove its progress counter
			RemoveCounter( buildIndexCounterText );
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
			int[] index = new int[_neuronCount];
			for( int i = 0; i < _neuronCount; i++ )
			{
				index[_network[i][3]] = i;
			}
			for( int i = 0; i < _neuronCount; i++ ) 
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
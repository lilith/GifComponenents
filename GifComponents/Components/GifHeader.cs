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
using System.ComponentModel;
using System.IO;
using System.Text;

namespace GifComponents
{
	/// <summary>
	/// The header section of a Graphics Interchange Format stream.
	/// See http://www.w3.org/Graphics/GIF/spec-gif89a.txt section 17.
	/// </summary>
	/// <remarks>
	/// The Header identifies the GIF Data Stream in context. The Signature 
	/// field marks the beginning of the Data Stream, and the Version field 
	/// identifies the set of capabilities required of a decoder to fully 
	/// process the Data Stream.
	/// This block is REQUIRED; exactly one Header must be present per Data 
	/// Stream.
	/// </remarks>
	public class GifHeader : GifComponent
	{
		private string _signature;
		private string _gifVersion;
		
		#region constructor( logical properties )
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="signature">
		/// The GIF signature which identifies a GIF stream.
		/// Should contain the fixed value "GIF".
		/// </param>
		/// <param name="gifVersion">
		/// The version of the GIF standard used by this stream.
		/// </param>
		public GifHeader( string signature, string gifVersion )
		{
			_signature = signature;
			_gifVersion = gifVersion;

			if( _signature != "GIF" )
			{
				string errorInfo = "Bad signature: " + _signature;
				ErrorState status = ErrorState.BadSignature;
				SetStatus( status, errorInfo );
			}
		}
		#endregion

		#region Signature property
		/// <summary>
		/// Gets the signature which introduces the GIF stream.
		/// This should contain the fixed value "GIF".
		/// </summary>
		[Description( "The signature which introduces the GIF stream. " + 
		             "This should contain the fixed value \"GIF\"." )]
		public string Signature
		{
			get { return _signature; }
		}
		#endregion
		
		#region Version property
		/// <summary>
		/// Gets the version of the Graphics Interchange Format used by the GIF 
		/// stream which contains this header.
		/// </summary>
		[Description( "The version of the Graphics Interchange Format used " + 
		             "by the GIF stream which contains this header." )]
		public string Version
		{
			get { return _gifVersion; }
		}
		#endregion

		#region public static FromStream method
		/// <summary>
		/// Reads and returns a GIF header from the supplied stream.
		/// </summary>
		/// <param name="inputStream">
		/// The input stream to read.
		/// </param>
		/// <returns>
		/// The GIF header read from the supplied input stream.
		/// </returns>
		public static GifHeader FromStream( Stream inputStream )
		{
			StringBuilder sb = new StringBuilder();
			// Read 6 bytes from the GIF stream
			// These should contain the signature and GIF version.
			bool endOfFile = false;
			for( int i = 0; i < 6; i++ ) 
			{
				int nextByte = Read( inputStream );
				if( nextByte == -1 )
				{
					endOfFile = true;
					nextByte = 0;
				}
				sb.Append( (char) nextByte );
			}
			string headerString = sb.ToString();
			string signature = headerString.Substring( 0, 3 );
			string gifVersion = headerString.Substring( 3, 3 );
			GifHeader header = new GifHeader( signature, gifVersion );
			if( endOfFile )
			{
				header.SetStatus( ErrorState.EndOfInputStream, "" );
			}
			
			return header;
		}
		#endregion

		#region public WriteToStream method
		/// <summary>
		/// Writes this component to the supplied output stream.
		/// </summary>
		/// <param name="outputStream">
		/// The output stream to write to.
		/// </param>
		public override void WriteToStream( Stream outputStream )
		{
			WriteString( _signature, outputStream );
			WriteString( _gifVersion, outputStream );
		}
		#endregion
	}
}

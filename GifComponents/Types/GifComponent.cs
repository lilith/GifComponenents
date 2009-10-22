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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;

namespace GifComponents
{
	/// <summary>
	/// The base class for a component of a Graphics Interchange File data 
	/// stream.
	/// </summary>
	[TypeConverter( typeof( ExpandableObjectConverter ) )]
	public abstract class GifComponent
	{
		#region declarations
		private GifComponentStatus _status;
		#endregion
		
		#region public constants
		
		/// <summary>
		/// Plain text label - identifies the current block as a plain text
		/// extension.
		/// Value 0x01.
		/// TODO: add see cref once PlainTextExtension class implemented
		/// </summary>
		public const byte CodePlaintextLabel = 0x01;
		
		/// <summary>
		/// Extension introducer - identifies the start of an extension block.
		/// Value 0x21.
		/// </summary>
		public const byte CodeExtensionIntroducer = 0x21;
		
		/// <summary>
		/// Image separator - identifies the start of an 
		/// <see cref="ImageDescriptor"/>.
		/// Value 0x2C.
		/// </summary>
		public const byte CodeImageSeparator = 0x2C;
		
		/// <summary>
		/// Trailer - This is a single-field block indicating the end of the GIF
		/// data stream.
		/// Value 0x3B.
		/// </summary>
		public const byte CodeTrailer = 0x3B;
		
		/// <summary>
		/// Graphic control label - identifies the current block as a
		/// <see cref="GraphicControlExtension"/>.
		/// Value 0xF9.
		/// </summary>
		public const byte CodeGraphicControlLabel = 0xF9;
		
		/// <summary>
		/// Comment label - identifies the current block as a comment extension.
		/// Value 0xFE.
		/// TODO: add see cref once CommentExtension class is implemented.
		/// </summary>
		public const byte CodeCommentLabel = 0xFE;
		
		/// <summary>
		/// Application extension label - identifies the current block as a
		/// <see cref="ApplicationExtension"/>.
		/// Value 0xFF.
		/// </summary>
		public const byte CodeApplicationExtensionLabel = 0xFF;
		
		#endregion
		
		#region protected constructor
		/// <summary>
		/// Constructor.
		/// This is implicitly called by constructors of derived types.
		/// </summary>
		protected GifComponent()
		{
			_status = new GifComponentStatus( ErrorState.Ok, "" );
		}
		#endregion

		#region properties
		
		#region ComponentStatus property
		/// <summary>
		/// Gets the status of this component, consisting of its error state
		/// and any associated error message.
		/// </summary>
		[System.ComponentModel.Category( "Status" )]
		public GifComponentStatus ComponentStatus
		{
			get { return _status; }
		}
		#endregion
		
		#region ErrorState property
		/// <summary>
		/// Gets the member of the Gif.Components.ErrorState held within the 
		/// ComponentStatus property.
		/// </summary>
		[System.ComponentModel.Browsable( false )]
		[System.ComponentModel.Category( "Status" )]
		public ErrorState ErrorState
		{
			get { return _status.ErrorState; }
		}
		#endregion
		
		#region ConsolidatedState property
		/// <summary>
		/// Gets the combined error states of this component and all its child
		/// components.
		/// </summary>
		/// <remarks>
		/// This property uses reflection to inspect the runtime type of the
		/// current instance and performs a bitwise or of the ErrorStates of
		/// the current instance and of any GifComponents within it.
		/// </remarks>
		[System.ComponentModel.Category( "Status" )]
		public ErrorState ConsolidatedState
		{
			get
			{
				ErrorState state = this.ErrorState;
				GifComponent component;
				GifComponent[] componentArray;
				PropertyInfo[] properties = this.GetType().GetProperties();
				foreach( PropertyInfo property in properties )
				{
					// We don't want to inspect the ConsolidatedState property
					// else we get a StackOverflowException.
					if( property.Name == "ConsolidatedState" )
					{
						continue;
					}
					
					// Is this property an array?
					if( property.PropertyType.IsArray )
					{
						// Is this property an array of GifComponents?
						componentArray = property.GetValue( this, null ) 
							as GifComponent[];
						if( componentArray != null )
						{
							// It's an array of GifComponents, so inspect
							// their ConsolidatedState properties.
							foreach( GifComponent c in componentArray )
							{
								state |= c.ConsolidatedState;
							}
						}
						continue;
					}

					// Is this property an indexer?
					if( property.GetIndexParameters().Length > 0 )
					{
						// it's probably an indexer, so ignore it
						continue;
					}
					
					// Is this property of a type derived from GifComponent?
					if( property.PropertyType.IsSubclassOf( typeof( GifComponent ) ) )
					{
						// Yes, so it also has a ConsolidatedState property
						component = property.GetValue( this, null ) as GifComponent;
						if( component != null )
						{
							state |= component.ConsolidatedState;
						}
						continue;
					}
					
					// Is this property a generic type?
					if( property.PropertyType.IsGenericType )
					{
						IEnumerable objectCollection
							= property.GetValue( this, null )
							as IEnumerable;
						if( objectCollection != null )
						{
							// Yes, it's IEnumerable, so iterate through its members
							foreach( object o in objectCollection )
							{
								GifComponent c = o as GifComponent;
								if( c != null )
								{
									state |= c.ConsolidatedState;
								}
							}
						}
						continue;
					}

				}
				return state;
			}
		}
		#endregion
		
		#region ErrorMessage property
		/// <summary>
		/// Gets any error message associated with the component's error state.
		/// </summary>
		[System.ComponentModel.Browsable( false )]
		[System.ComponentModel.Category( "Status" )]
		public string ErrorMessage
		{
			get { return _status.ErrorMessage; }
		}
		#endregion

		#endregion
		
		#region public methods
		
		#region override ToString method
		/// <summary>
		/// Gets a string representation of the error status of this component
		/// and its subcomponents.
		/// </summary>
		/// <returns>
		/// A string representation of the error status of this component and
		/// its subcomponents.
		/// </returns>
		public override string ToString()
		{
			return this.ConsolidatedState.ToString();
		}
		#endregion
		
		#region public TestState method
		/// <summary>
		/// Tests whether the error state of this component or any of its member
		/// components contains the supplied member of the ErrorState 
		/// enumeration.
		/// </summary>
		/// <param name="state">
		/// The error state to look for in the current instance's state.
		/// </param>
		/// <returns>
		/// True if the current instance's error state includes the supplied
		/// error state, otherwise false.
		/// </returns>
		public bool TestState( ErrorState state )
		{
			return( ConsolidatedState & state ) == state;
		}
		#endregion

		#endregion
		
		#region protected methods
		
		#region protected SetStatus method
		/// <summary>
		/// Sets the ComponentStatus property of thie GifComponent.
		/// </summary>
		/// <param name="errorState">
		/// A member of the Gif.Components.ErrorState enumeration.
		/// </param>
		/// <param name="errorMessage">
		/// An error message associated with the error state.
		/// </param>
		protected void SetStatus( ErrorState errorState, string errorMessage )
		{
			ErrorState newState = _status.ErrorState | errorState;
			string newMessage = _status.ErrorMessage;
			if( !String.IsNullOrEmpty( newMessage ) )
			{
				newMessage += Environment.NewLine;
			}
			newMessage += errorMessage;
			_status = new GifComponentStatus( newState, newMessage );
		}
		#endregion

		#region protected static Read method
		/// <summary>
		/// Reads a single byte from the input stream and advances the position
		/// within the stream by one byte, or returns -1 if at the end of the
		/// stream.
		/// </summary>
		/// <param name="inputStream">
		/// The input stream to read.
		/// </param>
		/// <returns>
		/// The unsigned byte, cast to an Int32, or -1 if at the end of the 
		/// stream.
		/// </returns>
		protected static int Read( Stream inputStream ) 
		{
			if( inputStream == null )
			{
				throw new ArgumentNullException( "inputStream" );
			}
			int curByte = 0;
			curByte = inputStream.ReadByte();
			return curByte;
		}
		#endregion

		#region protected static ReadShort method
		/// <summary>
		/// Reads next 16-bit value, least significant byte first, and advances 
		/// the position within the stream by two bytes.
		/// </summary>
		/// <param name="inputStream">
		/// The input stream to read.
		/// </param>
		/// <returns>
		/// The next two bytes in the stream, cast to an Int32, or -1 if at the 
		/// end of the stream.
		/// </returns>
		protected static int ReadShort( Stream inputStream )
		{
			// read 16-bit value, LSB first
//			return Read( inputStream ) | (Read( inputStream ) << 8);
			
			// Least significant byte is first in the stream
			int leastSignificant = Read( inputStream );
			
			// Most significant byte is next - shift its value left by 8 bits
			int mostSignificant = Read( inputStream ) << 8;
			
			// Use bitwise or to combine them to a short return value
			int returnValue = leastSignificant | mostSignificant;
			
			// Ensure the return value is -1 if the end of stream has been 
			// reached (if the first byte wasn't the end of stream then we'd
			// get a different negative number instead).
			if( returnValue < 0 )
			{
				returnValue = -1;
			}
			
			return returnValue;
		}
		#endregion

		#region protected static SkipBlocks method
		/// <summary>
		/// Skips variable length blocks up to and including next zero length 
		/// block (block terminator).
		/// </summary>
		/// <param name="inputStream">
		/// The input stream to read.
		/// </param>
		protected static void SkipBlocks( Stream inputStream )
		{
			DataBlock block;
			do 
			{
				block = DataBlock.FromStream( inputStream );
			} 
			while( block.DeclaredBlockSize > 0 && block.ErrorState == ErrorState.Ok );
		}
		#endregion

		#region protected static WriteString method
		/// <summary>
		/// Writes the supplied string to the supplied output stream
		/// </summary>
		/// <param name="textToWrite">
		/// The string to be written to the output stream
		/// </param>
		/// <param name="outputStream">
		/// The stream to write the string to.
		/// </param>
		protected static void WriteString( String textToWrite, 
		                                   Stream outputStream )
		{
			if( outputStream == null )
			{
				throw new ArgumentNullException( "outputStream" );
			}

			// if textToWrite is null then write nothing
			if( textToWrite == null )
			{
				return;
			}
			
			char[] chars = textToWrite.ToCharArray();
			for( int i = 0; i < chars.Length; i++ )
			{
				outputStream.WriteByte( (byte) chars[i] );
			}
		}
		#endregion

		#region protected static WriteShort method
		/// <summary>
		/// Writes a 16-bit value to the supplied output stream, 
		/// least-significant byte first.
		/// The first two bytes in the supplied value are discarded.
		/// </summary>
		/// <param name="valueToWrite">
		/// The value to write to the output stream.
		/// </param>
		/// <param name="outputStream">
		/// The stream to write to.
		/// </param>
		protected static void WriteShort( int valueToWrite, Stream outputStream )
		{
			if( outputStream == null )
			{
				throw new ArgumentNullException( "outputStream" );
			}
			
			// Write least significant byte
			outputStream.WriteByte( Convert.ToByte( valueToWrite & 0xff) );
			// Write second-least significant byte
			outputStream.WriteByte( Convert.ToByte( (valueToWrite >> 8) & 0xff ) );
		}
		#endregion
		
		#region protected static WriteByte method
		/// <summary>
		/// Writes the least significant byte of the supplied value to the 
		/// supplied stream.
		/// The first 3 bytes of the supplied value are discarded.
		/// </summary>
		/// <param name="valueToWrite">
		/// The value to write to the output stream.
		/// </param>
		/// <param name="outputStream">
		/// The stream to write to.
		/// </param>
		protected static void WriteByte( int valueToWrite, Stream outputStream )
		{
			if( outputStream == null )
			{
				throw new ArgumentNullException( "outputStream" );
			}
			
			outputStream.WriteByte( Convert.ToByte( valueToWrite & 0xFF ) );
		}
		#endregion
	
		#endregion

		#region abstract WriteToStream method
		/// <summary>
		/// Appends the current GifComponent to the supplied output stream.
		/// </summary>
		/// <param name="outputStream">
		/// The stream to which the component is to be written.
		/// </param>
		public abstract void WriteToStream( Stream outputStream );
		#endregion
	}
}

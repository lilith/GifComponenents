#region Copyright (C) Simon Bridewell
// 
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 3
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

// You can read the full text of the GNU General Public License at:
// http://www.gnu.org/licenses/gpl.html

// See also the Wikipedia entry on the GNU GPL at:
// http://en.wikipedia.org/wiki/GNU_General_Public_License
#endregion

using System;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using NUnit.Framework;

namespace GifComponents.NUnit
{
	#region GifComponentTest
	/// <summary>
	/// Test fixure for the GifComponent base type.
	/// </summary>
	[TestFixture]
	public class GifComponentTest
	{
		#region declarations
		private ExampleComponent _component;
		private SubComponent _sub1;
		private SubComponent _sub2;
		private SubComponent _sub3;
		#endregion

		#region constants
		private const ErrorState _sub1State = ErrorState.ColourTableTooShort;
		private const ErrorState _sub2State = ErrorState.EndOfInputStream;
		private const ErrorState _sub3State = ErrorState.FrameHasNoColourTable;
		private const ErrorState _componentState = ErrorState.BadDataBlockIntroducer;
		private const string _sub1Message = "sub status 1";
		private const string _sub2Message = "sub status 2";
		private const string _sub3Message = "sub status 3";
		private const string _sub3Message2 = "sub status 3a";
		private const string _componentMessage = "component status";
		#endregion
		
		#region Setup Method
		/// <summary>
		/// Setup method.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			_component = new ExampleComponent();
			_sub1 = new SubComponent();
			_sub2 = new SubComponent();
			_sub3 = new SubComponent();
			
			_sub1.SetMyStatus( _sub1State, _sub1Message );
			_sub2.SetMyStatus( _sub2State, _sub2Message );
			_sub3.SetMyStatus( _sub3State, _sub3Message );
			_sub3.SetMyStatus( _sub3State, _sub3Message2 );
			_component.SetMyStatus( _componentState, _componentMessage );
			
			_component.SubComponent1 = _sub1;
			_component.AddSubComponent( _sub2 );
			_component.AddSubComponent( _sub3 );
		}
		#endregion
		
		#region property tests
		
		#region ComponentStatusTest
		/// <summary>
		/// Checks that the ComponentStatus property returns the expected 
		/// values.
		/// </summary>
		[Test]
		public void ComponentStatusTest()
		{
			Assert.AreEqual( _sub1State, 
			                 _component.SubComponent1.ComponentStatus.ErrorState );
			Assert.AreEqual( _sub2State, 
			                 _component.SubComponents[0].ComponentStatus.ErrorState );
			Assert.AreEqual( _sub3State, 
			                 _component.SubComponents[1].ComponentStatus.ErrorState );
			Assert.AreEqual( _componentState, 
			                 _component.ComponentStatus.ErrorState );
			Assert.AreEqual( _sub1Message, 
			                 _component.SubComponent1.ComponentStatus.ErrorMessage );
			Assert.AreEqual( _sub2Message, 
			                 _component.SubComponents[0].ComponentStatus.ErrorMessage );
			Assert.AreEqual( _sub3Message + Environment.NewLine + _sub3Message2, 
			                 _component.SubComponents[1].ComponentStatus.ErrorMessage );
			Assert.AreEqual( _componentMessage, 
			                 _component.ComponentStatus.ErrorMessage );
		}
		#endregion

		#region ConsolidatedStateTest
		/// <summary>
		/// Checks that the ConsolidatedState property returns the expected
		/// values.
		/// </summary>
		[Test]
		public void ConsolidatedStateTest()
		{
			ErrorState expected = _sub1State | _sub2State | _sub3State | _componentState;
			Assert.AreEqual( expected, _component.ConsolidatedState );
		}
		#endregion
		
		#region ErrorMessageTest
		/// <summary>
		/// Checks that the ErrorMessage property returns the expected value.
		/// </summary>
		[Test]
		public void ErrorMessageTest()
		{
			Assert.AreEqual( _sub1Message, 
			                 _component.SubComponent1.ErrorMessage );
			Assert.AreEqual( _sub2Message, 
			                 _component.SubComponents[0].ErrorMessage );
			Assert.AreEqual( _sub3Message + Environment.NewLine + _sub3Message2, 
			                 _component.SubComponents[1].ErrorMessage );
			Assert.AreEqual( _componentMessage, _component.ErrorMessage );
		}
		#endregion
		
		#region ErrorStateTest
		/// <summary>
		/// Checks that the ErrorState property returns the expected values.
		/// </summary>
		[Test]
		public void ErrorStateTest()
		{
			Assert.AreEqual( _sub1State, _component.SubComponent1.ErrorState );
			Assert.AreEqual( _sub2State, _component.SubComponents[0].ErrorState );
			Assert.AreEqual( _sub3State, _component.SubComponents[1].ErrorState );
			Assert.AreEqual( _componentState, _component.ErrorState );
		}
		#endregion
		
		#endregion

		#region method tests
		
		#region ReadTest
		/// <summary>
		/// Checks that the static Read method works correctly under normal
		/// circumstances.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void ReadTest()
		{
			// Write some bytes to a stream and then position it at the start
			byte[] bytes = new byte[]
			{
				21, 18, 69
			};
			Stream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Seek( 0, SeekOrigin.Begin );
			
			// Use the Read method of GifComponent to read the stream back in
			Assert.AreEqual( 21, ExampleComponent.CallRead( s ) );
			Assert.AreEqual( 18, ExampleComponent.CallRead( s ) );
			Assert.AreEqual( 69, ExampleComponent.CallRead( s ) );
			// There are no more bytes to read, so the next read should return -1
			Assert.AreEqual( -1, ExampleComponent.CallRead( s ) );
		}
		#endregion
		
		#region ReadTestNullArgument
		/// <summary>
		/// Checks that the static Read method throws the correct exception when
		/// passed a null input stream.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void ReadTestNullArgument()
		{
			try
			{
				ExampleComponent.CallRead( null );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "inputStream", ex.ParamName );
				throw;
			}
		}
		#endregion
		
		#region ReadShort tests
		
		#region ReadShortTest1Byte
		/// <summary>
		/// Tests the ReadShort method with a 1-byte stream.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void ReadShortTest1Byte()
		{
			byte[] bytes = new byte[] { 1 };
			Stream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Seek( 0, SeekOrigin.Begin );
			
			Assert.AreEqual( -1, ExampleComponent.CallReadShort( s ) );
		}
		#endregion
		
		#region ReadShortTest2Byte
		/// <summary>
		/// Tests the ReadShort method with a 2-byte stream.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void ReadShortTest2Byte()
		{
			byte[] bytes = new byte[] { 1, 2 };
			Stream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Seek( 0, SeekOrigin.Begin );
			
			Assert.AreEqual( 513, ExampleComponent.CallReadShort( s ) );
			Assert.AreEqual( -1, ExampleComponent.CallReadShort( s ) );
		}
		#endregion
		
		#region ReadShortTest3Byte
		/// <summary>
		/// Tests the ReadShort method with a 3-byte stream.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void ReadShortTest3Byte()
		{
			byte[] bytes = new byte[] { 1, 2, 3 };
			Stream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Seek( 0, SeekOrigin.Begin );
			
			Assert.AreEqual( 513, ExampleComponent.CallReadShort( s ) );
			Assert.AreEqual( -1, ExampleComponent.CallReadShort( s ) );
		}
		#endregion
		
		#region ReadShortTest4Byte
		/// <summary>
		/// Tests the ReadShort method with a 4-byte stream.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void ReadShortTest4Byte()
		{
			byte[] bytes = new byte[] { 1, 2, 3, 4 };
			Stream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Seek( 0, SeekOrigin.Begin );
			
			Assert.AreEqual( 513, ExampleComponent.CallReadShort( s ) );
			Assert.AreEqual( 1027, ExampleComponent.CallReadShort( s ) );
			Assert.AreEqual( -1, ExampleComponent.CallReadShort( s ) );
		}
		#endregion
		
		#endregion
		
		#region TestStateTest
		/// <summary>
		/// Checks that the TestState method returns true when the current
		/// GifComponent or any of its member components have an error state
		/// which includes the supplied error state.
		/// Also checks that the TestState method returns false when neither 
		/// the current GifComponent nor any of its member components have an
		/// error state which includes the supplied error state.
		/// </summary>
		[Test]
		public void TestStateTest()
		{
			Assert.IsTrue( _component.TestState( _componentState ) );
			Assert.IsTrue( _component.TestState( _sub1State ) );
			Assert.IsTrue( _component.TestState( _sub2State ) );
			Assert.IsTrue( _component.TestState( _sub3State ) );
			Assert.IsTrue( _component.SubComponent1.TestState( _sub1State ) );
			Assert.IsTrue( _component.SubComponents[0].TestState( _sub2State ) );
			Assert.IsTrue( _component.SubComponents[1].TestState( _sub3State ) );
			
			Array possibleStates = ErrorState.GetValues( typeof( ErrorState ) );
			
			foreach( ErrorState thisState in possibleStates )
			{
				if( thisState != ErrorState.Ok
				   	&& thisState != _sub1State 
				  )
				{
					Assert.IsFalse( _component.SubComponent1.TestState( thisState ), 
					                thisState.ToString() );
				}
			}
			
			foreach( ErrorState thisState in possibleStates )
			{
				if( thisState != ErrorState.Ok
				   	&& thisState != _sub2State 
				  )
				{
					Assert.IsFalse( _component.SubComponents[0].TestState( thisState ),
					                thisState.ToString() );
				}
			}
			
			foreach( ErrorState thisState in possibleStates )
			{
				if( thisState != ErrorState.Ok
				   	&& thisState != _sub3State 
				  )
				{
					Assert.IsFalse( _component.SubComponents[1].TestState( thisState ),
					                thisState.ToString() );
				}
			}
			
			foreach( ErrorState thisState in possibleStates )
			{
				if( thisState != ErrorState.Ok
				    && thisState != _sub1State
				   	&& thisState != _sub2State 
				   	&& thisState != _sub3State 
				   	&& thisState != _componentState
				  )
				{
					Assert.IsFalse( _component.TestState( thisState ), 
					                thisState.ToString() );
				}
			}
		}
		#endregion
		
		#region ToStringTest
		/// <summary>
		/// Checks that the ToString method returns a string representation of
		/// the component's error status.
		/// </summary>
		[Test]
		public void ToStringTest()
		{
			Assert.AreEqual( _sub1State.ToString(), 
			                 _component.SubComponent1.ToString() );
			Assert.AreEqual( _sub2State.ToString(), 
			                 _component.SubComponents[0].ToString() );
			Assert.AreEqual( _sub3State.ToString(), 
			                 _component.SubComponents[1].ToString() );
			
			ErrorState expected = _componentState 
								| _sub1State 
								| _sub2State 
								| _sub3State;
			Assert.AreEqual( expected.ToString(), _component.ToString() );
		}
		#endregion
		
		#region SkipBlocksTest
		/// <summary>
		/// Checks that the SkipBlocks method works as expected.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void SkipBlocksTest()
		{
			byte[] data1 = new byte[] { 24, 38, 53 };
			byte[] data2 = new byte[] { 66, 23, 58, 56, 23 };
			byte[] data3 = new byte[] { }; // block terminator
			byte[] data4 = new byte[] { 43 };
			byte[] data5 = new byte[] { }; // block terminator
			
			MemoryStream s = new MemoryStream();
			s.WriteByte( (byte) data1.Length );
			s.Write( data1, 0, data1.Length );
			s.WriteByte( (byte) data2.Length );
			s.Write( data2, 0, data2.Length );
			s.WriteByte( (byte) data3.Length );
			s.Write( data3, 0, data3.Length );
			s.WriteByte( (byte) data4.Length );
			s.Write( data4, 0, data4.Length );
			s.WriteByte( (byte) data5.Length );
			s.Write( data5, 0, data5.Length );
			s.Seek( 0, SeekOrigin.Begin );
			
			ExampleComponent.CallSkipBlocks( s );
			DataBlock db;
			
			db = DataBlock.FromStream( s );
			Assert.AreEqual( data4.Length, db.DeclaredBlockSize );
			Assert.AreEqual( data4.Length, db.ActualBlockSize );
			Assert.AreEqual( data4, db.Data );
			
			db = DataBlock.FromStream( s );
			Assert.AreEqual( data5.Length, db.DeclaredBlockSize );
			Assert.AreEqual( data5.Length, db.ActualBlockSize );
			Assert.AreEqual( data5, db.Data );
		}
		#endregion
		
		#region WriteStringTest
		/// <summary>
		/// Checks that the WriteString method works correctly.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void WriteStringTest()
		{
			string text = "hello world";
			MemoryStream s = new MemoryStream();
			ExampleComponent.CallWriteString( text, s );
			s.Seek( 0, SeekOrigin.Begin );
			byte[] bytes = new byte[s.Length];
			s.Read( bytes, 0, (int) s.Length );
			StringBuilder sb = new StringBuilder();
			foreach( byte b in bytes )
			{
				sb.Append( (char) b );
			}
			string textRead = sb.ToString();
			Assert.AreEqual( text, textRead );
		}
		#endregion
		
		#region WriteStringNullTest
		/// <summary>
		/// Checks that the WriteString method works correctly when passed a 
		/// null string.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void WriteStringNullTest()
		{
			MemoryStream s = new MemoryStream();
			ExampleComponent.CallWriteString( null, s );
			Assert.AreEqual( 0, s.Length );
		}
		#endregion
		
		#region WriteStringNullStreamTest
		/// <summary>
		/// Checks that the correct exception is thrown when the WriteString
		/// is passed a null stream.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void WriteStringNullStreamTest()
		{
			Stream s = null;
			try
			{
				ExampleComponent.CallWriteString( "hello", s );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "outputStream", ex.ParamName );
				throw;
			}
		}
		#endregion
		
		#region WriteShortTest
		/// <summary>
		/// Checks that the WriteShort method works correctly.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void WriteShortTest()
		{
			MemoryStream s = new MemoryStream();
			int valueToWrite = 0x3b2a13;
			ExampleComponent.CallWriteShort( valueToWrite, s );
			s.Seek( 0, SeekOrigin.Begin );
			Assert.AreEqual( 2, s.Length ); // first byte will have been discarded
			int byte1 = s.ReadByte();
			int byte2 = s.ReadByte();
			Assert.AreEqual( 0x13, byte1 );
			Assert.AreEqual( 0x2a, byte2 );
		}
		#endregion
		
		#region WriteShortNullStreamTest
		/// <summary>
		/// Checks that the correct exception is thrown when the WriteShort
		/// is passed a null stream.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void WriteShortNullStreamTest()
		{
			Stream s = null;
			try
			{
				ExampleComponent.CallWriteShort( 12, s );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "outputStream", ex.ParamName );
				throw;
			}
		}
		#endregion
		
		#region WriteByteTest
		/// <summary>
		/// Checks that the WriteByte method works correctly.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void WriteByteTest()
		{
			MemoryStream s = new MemoryStream();
			int valueToWrite = 0x3b2a13;
			ExampleComponent.CallWriteByte( valueToWrite, s );
			s.Seek( 0, SeekOrigin.Begin );
			Assert.AreEqual( 1, s.Length ); // first 2 bytes will have been discarded
			int byte1 = s.ReadByte();
			Assert.AreEqual( 0x13, byte1 );
		}
		#endregion
		
		#region WriteByteNullStreamTest
		/// <summary>
		/// Checks that the correct exception is thrown when the WriteByte
		/// is passed a null stream.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void WriteByteNullStreamTest()
		{
			Stream s = null;
			try
			{
				ExampleComponent.CallWriteByte( 12, s );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "outputStream", ex.ParamName );
				throw;
			}
		}
		#endregion
		
		#endregion
	}
	#endregion

	#region internal ExampleComponent class
	/// <summary>
	/// A class derived from GifComponent in order to test the GifComponent
	/// class, as it is abstract and cannot be instantiated directly.
	/// </summary>
	internal class ExampleComponent : SubComponent
	{
		private SubComponent _subComponent1;
		private Collection<SubComponent> _subComponents;
		private byte[] _byteArray;
		
		#region constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public ExampleComponent()
		{
			_subComponent1 = new SubComponent();
			_subComponents = new Collection<SubComponent>();
			_byteArray = new byte[] { 1, 2, 3, 4, 5, 6 };
		}
		#endregion
		
		#region SubComponent1 property
		/// <summary>
		/// Gets and sets SubComponent1.
		/// </summary>
		public SubComponent SubComponent1
		{
			get { return _subComponent1; }
			set { _subComponent1 = value; }
		}
		#endregion
		
		#region SubComponents property
		/// <summary>
		/// Gets SubComponents as a generic collection.
		/// </summary>
		public Collection<SubComponent> SubComponents
		{
			get
			{ 
				return _subComponents; 
			}
		}
		#endregion
		
		#region SubComponentArray property
		/// <summary>
		/// Gets SubComponents as an array.
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		public SubComponent[] SubComponentArray
		{
			get
			{ 
				SubComponent[] components = new SubComponent[_subComponents.Count];
				_subComponents.CopyTo( components, 0 );
				return components;
			}
		}
		#endregion
		
		#region indexer
		/// <summary>
		/// This property is implemented to test the ConsolidatedStatus 
		/// property.
		/// </summary>
		public override byte this[int index]
		{
			get { return _byteArray[index]; }
		}
		#endregion
		
		#region AddSubComponent method
		/// <summary>
		/// Adds the supplied component to the SubComponents collection.
		/// </summary>
		/// <param name="component"></param>
		public void AddSubComponent( SubComponent component )
		{
			_subComponents.Add( component );
		}
		#endregion

		#region public static CallRead method
		/// <summary>
		/// Returns the next byte read from the supplied input stream, as 
		/// returned by GifComponent.Read
		/// </summary>
		/// <param name="inputStream"></param>
		/// <returns></returns>
		public static int CallRead( Stream inputStream )
		{
			return Read( inputStream );
		}
		#endregion

		#region public static CallReadShort method
		/// <summary>
		/// Returns the next two bytes read from the supplied input stream, as 
		/// returned by GifComponent.ReadShort.
		/// </summary>
		/// <param name="inputStream"></param>
		/// <returns></returns>
		public static int CallReadShort( Stream inputStream )
		{
			return ReadShort( inputStream );
		}
		#endregion

		#region public static CallSkipBlocks method
		/// <summary>
		/// Skips variable length blocks in the input stream, up to and 
		/// including the next block terminator, as performed by the 
		/// GifComponent.SkipBlocks method.
		/// </summary>
		/// <param name="inputStream"></param>
		public static void CallSkipBlocks( Stream inputStream )
		{
			SkipBlocks( inputStream );
		}
		#endregion
		
		#region public static CallWriteString method
		/// <summary>
		/// Writes the supplied string to the supplied stream.
		/// </summary>
		/// <param name="textToWrite"></param>
		/// <param name="outputStream"></param>
		public static void CallWriteString( string textToWrite, 
		                                    Stream outputStream )
		{
			WriteString( textToWrite, outputStream );
		}
		#endregion

		#region public static CallWriteShort method
		/// <summary>
		/// Writes the last 2 bytes of the supplied value to the supplied 
		/// stream, least-significant byte first.
		/// </summary>
		/// <param name="valueToWrite"></param>
		/// <param name="outputStream"></param>
		public static void CallWriteShort( int valueToWrite, Stream outputStream )
		{
			WriteShort( valueToWrite, outputStream );
		}
		#endregion

		#region public static CallWriteByte method
		/// <summary>
		/// Writes the least significant byte of the supplied value to the
		/// supplied stream.
		/// </summary>
		/// <param name="valueToWrite"></param>
		/// <param name="outputStream"></param>
		public static void CallWriteByte( int valueToWrite, Stream outputStream )
		{
			WriteByte( valueToWrite, outputStream );
		}
		#endregion

	}
	#endregion
	
	#region internal SubComponent class
	/// <summary>
	/// Another class derived from GifComponent, for use as properties of
	/// ExampleComponent.
	/// </summary>
	internal class SubComponent : GifComponent
	{
		private byte[] _byteArray;
		
		#region constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public SubComponent()
		{
			_byteArray = new byte[] { 1, 2, 3, 4, 5, 6 };
		}
		#endregion
		
		#region SetMyStatus method
		/// <summary>
		/// Allows the component's status to be set via its SetStatus method.
		/// This is only accesible to derived types.
		/// </summary>
		/// <param name="state"></param>
		/// <param name="message"></param>
		public void SetMyStatus( ErrorState state, string message )
		{
			SetStatus( state, message );
		}
		#endregion

		#region indexer
		/// <summary>
		/// This property is implemented to test the ConsolidatedStatus 
		/// property.
		/// </summary>
		public virtual byte this[int index]
		{
			get { return _byteArray[index]; }
		}
		#endregion

		#region WriteToStream method
		/// <summary>
		/// Writes this component to the supplied stream.
		/// </summary>
		/// <param name="outputStream"></param>
		public override void WriteToStream( Stream outputStream )
		{
			outputStream.Write( _byteArray, 0, _byteArray.Length );
		}
		#endregion
	}
	#endregion
}

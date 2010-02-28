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
using GifComponents.Components;

namespace GifComponents.NUnit.Components
{
	#region GifComponentTest
	/// <summary>
	/// Test fixure for the GifComponent base type.
	/// </summary>
	[TestFixture]
	public class GifComponentTest : GifComponentTestFixtureBase, IDisposable
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
		
		#region ConstructorStreamTest
		/// <summary>
		/// Checks that the constructors which accept an input stream work as
		/// expected.
		/// </summary>
		[Test]
		public void ConstructorStreamTest()
		{
			ReportStart();
			
			byte[] bytes = new byte[] { 6, 5, 4, 3, 2, 1 };
			MemoryStream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			
			// without XmlDebug
			s.Position = 0;
			_sub1 = new SubComponent( s );
			Assert.AreEqual( ErrorState.Ok, _sub1.ConsolidatedState );
			CollectionAssert.AreEqual( bytes, _sub1.Bytes );
			string xml
				= "<Message>There is no DebugXml because XML debugging "
				+ "has not been enabled for this SubComponent instance.</Message>";
			Assert.AreEqual( xml, _sub1.DebugXml );
			
			// with XmlDebug
			s.Position = 0;
			_sub1 = new SubComponent( s, true );
			Assert.AreEqual( ErrorState.Ok, _sub1.ConsolidatedState );
			CollectionAssert.AreEqual( bytes, _sub1.Bytes );
			Assert.AreEqual( ExpectedDebugXml, _sub1.DebugXml );
			
			ReportEnd();
		}
		#endregion
		
		#region WriteNullBytesTest
		/// <summary>
		/// Checks that the correct exception is thrown when the 
		/// WriteDebugXmlByteValues is passed a null byte array.
		/// </summary>
		[ExpectedException( typeof( ArgumentNullException ) )]
		[Test]
		public void WriteNullBytesTest()
		{
			ReportStart();
			
			MemoryStream s = new MemoryStream();
			s.WriteByte( (byte) 1 );
			s.Position = 0;
			try
			{
				_sub1 = new SubComponent( s, true, true, false );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "bytes", ex.ParamName );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#region WriteNullIntegersTest
		/// <summary>
		/// Checks that the correct exception is thrown when the 
		/// WriteDebugXmlByteValues is passed a null integer array.
		/// </summary>
		[ExpectedException( typeof( ArgumentNullException ) )]
		[Test]
		public void WriteNullIntegersTest()
		{
			ReportStart();
			
			MemoryStream s = new MemoryStream();
			s.WriteByte( (byte) 1 );
			s.Position = 0;
			try
			{
				_sub1 = new SubComponent( s, true, false, true );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "bytes", ex.ParamName );
				ReportEnd();
				throw;
			}
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
			ReportStart();
			
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
			
			ReportEnd();
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
			ReportStart();
			
			ErrorState expected = _sub1State | _sub2State | _sub3State | _componentState;
			Assert.AreEqual( expected, _component.ConsolidatedState );
			
			ReportEnd();
		}
		#endregion
		
		#region ErrorMessageTest
		/// <summary>
		/// Checks that the ErrorMessage property returns the expected value.
		/// </summary>
		[Test]
		public void ErrorMessageTest()
		{
			ReportStart();
			
			Assert.AreEqual( _sub1Message, 
			                 _component.SubComponent1.ErrorMessage );
			Assert.AreEqual( _sub2Message, 
			                 _component.SubComponents[0].ErrorMessage );
			Assert.AreEqual( _sub3Message + Environment.NewLine + _sub3Message2, 
			                 _component.SubComponents[1].ErrorMessage );
			Assert.AreEqual( _componentMessage, _component.ErrorMessage );
			
			ReportEnd();
		}
		#endregion
		
		#region ErrorStateTest
		/// <summary>
		/// Checks that the ErrorState property returns the expected values.
		/// </summary>
		[Test]
		public void ErrorStateTest()
		{
			ReportStart();
			
			Assert.AreEqual( _sub1State, _component.SubComponent1.ErrorState );
			Assert.AreEqual( _sub2State, _component.SubComponents[0].ErrorState );
			Assert.AreEqual( _sub3State, _component.SubComponents[1].ErrorState );
			Assert.AreEqual( _componentState, _component.ErrorState );
			
			ReportEnd();
		}
		#endregion
		
		#endregion

		#region method tests
		
		#region SetStatusTest
		/// <summary>
		/// Checks that the SetStatus method works correctly.
		/// </summary>
		[Test]
		public void SetStatusTest()
		{
			ReportStart();
			
			// without XmlDebug
			_sub1 = new SubComponent();
			_sub1.SetMyStatus( ErrorState.EndOfInputStream, "Oops" );
			Assert.AreEqual( ErrorState.EndOfInputStream, _sub1.ErrorState );
			Assert.AreEqual( "Oops", _sub1.ErrorMessage );
			
			// with XmlDebug
			byte[] bytes = new byte[] { 1, 4 }; // too short, causes error state
			MemoryStream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Position = 0;
			_sub1 = new SubComponent( s, true );
			Assert.AreEqual( ErrorState.ColourTableTooShort, _sub1.ErrorState );
			Assert.AreEqual( "Bother!", _sub1.ErrorMessage );
			Assert.AreEqual( ExpectedDebugXml, _sub1.DebugXml );
			
			ReportEnd();
		}
		#endregion
		
		#region ReadTest
		/// <summary>
		/// Checks that the static Read method works correctly under normal
		/// circumstances.
		/// </summary>
		[Test]
		[SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
		public void ReadTest()
		{
			ReportStart();
			
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
			
			ReportEnd();
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
			ReportStart();
			
			try
			{
				ExampleComponent.CallRead( null );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "inputStream", ex.ParamName );
				ReportEnd();
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
			ReportStart();
			
			byte[] bytes = new byte[] { 1 };
			Stream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Seek( 0, SeekOrigin.Begin );
			
			Assert.AreEqual( -1, ExampleComponent.CallReadShort( s ) );
			
			ReportEnd();
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
			ReportStart();
			
			byte[] bytes = new byte[] { 1, 2 };
			Stream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Seek( 0, SeekOrigin.Begin );
			
			Assert.AreEqual( 513, ExampleComponent.CallReadShort( s ) );
			Assert.AreEqual( -1, ExampleComponent.CallReadShort( s ) );
			
			ReportEnd();
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
			ReportStart();
			
			byte[] bytes = new byte[] { 1, 2, 3 };
			Stream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Seek( 0, SeekOrigin.Begin );
			
			Assert.AreEqual( 513, ExampleComponent.CallReadShort( s ) );
			Assert.AreEqual( -1, ExampleComponent.CallReadShort( s ) );
			
			ReportEnd();
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
			ReportStart();
			
			byte[] bytes = new byte[] { 1, 2, 3, 4 };
			Stream s = new MemoryStream();
			s.Write( bytes, 0, bytes.Length );
			s.Seek( 0, SeekOrigin.Begin );
			
			Assert.AreEqual( 513, ExampleComponent.CallReadShort( s ) );
			Assert.AreEqual( 1027, ExampleComponent.CallReadShort( s ) );
			Assert.AreEqual( -1, ExampleComponent.CallReadShort( s ) );
			
			ReportEnd();
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
			ReportStart();
			
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
			
			ReportEnd();
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
			ReportStart();
			
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
			
			ReportEnd();
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
			ReportStart();
			
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
			
			_component.CallSkipBlocks( s );
			DataBlock db;
			
			db = new DataBlock( s );
			Assert.AreEqual( data4.Length, db.DeclaredBlockSize );
			Assert.AreEqual( data4.Length, db.ActualBlockSize );
			Assert.AreEqual( data4, db.Data );
			
			db = new DataBlock( s );
			Assert.AreEqual( data5.Length, db.DeclaredBlockSize );
			Assert.AreEqual( data5.Length, db.ActualBlockSize );
			Assert.AreEqual( data5, db.Data );
			
			ReportEnd();
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
			ReportStart();
			
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
			
			ReportEnd();
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
			ReportStart();
			
			MemoryStream s = new MemoryStream();
			ExampleComponent.CallWriteString( null, s );
			Assert.AreEqual( 0, s.Length );
			
			ReportEnd();
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
			ReportStart();
			
			Stream s = null;
			try
			{
				ExampleComponent.CallWriteString( "hello", s );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "outputStream", ex.ParamName );
				ReportEnd();
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
			ReportStart();
			
			MemoryStream s = new MemoryStream();
			int valueToWrite = 0x3b2a13;
			ExampleComponent.CallWriteShort( valueToWrite, s );
			s.Seek( 0, SeekOrigin.Begin );
			Assert.AreEqual( 2, s.Length ); // first byte will have been discarded
			int byte1 = s.ReadByte();
			int byte2 = s.ReadByte();
			Assert.AreEqual( 0x13, byte1 );
			Assert.AreEqual( 0x2a, byte2 );
			
			ReportEnd();
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
			ReportStart();
			
			Stream s = null;
			try
			{
				ExampleComponent.CallWriteShort( 12, s );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "outputStream", ex.ParamName );
				ReportEnd();
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
			ReportStart();
			
			MemoryStream s = new MemoryStream();
			int valueToWrite = 0x3b2a13;
			ExampleComponent.CallWriteByte( valueToWrite, s );
			s.Seek( 0, SeekOrigin.Begin );
			Assert.AreEqual( 1, s.Length ); // first 2 bytes will have been discarded
			int byte1 = s.ReadByte();
			Assert.AreEqual( 0x13, byte1 );
			
			ReportEnd();
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
			ReportStart();
			
			Stream s = null;
			try
			{
				ExampleComponent.CallWriteByte( 12, s );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "outputStream", ex.ParamName );
				ReportEnd();
				throw;
			}
		}
		#endregion
		
		#endregion

		#region IDisposable implementation
		/// <summary>
		/// Indicates whether or not the Dispose( bool ) method has already been 
		/// called.
		/// </summary>
		bool _disposed;

		/// <summary>
		/// Finalzer.
		/// </summary>
		~GifComponentTest()
		{
			Dispose( false );
		}

		/// <summary>
		/// Disposes resources used by this class.
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/// <summary>
		/// Disposes resources used by this class.
		/// </summary>
		/// <param name="disposing">
		/// Indicates whether this method is being called by the class's Dispose
		/// method (true) or by the garbage collector (false).
		/// </param>
		protected virtual void Dispose( bool disposing )
		{
			if( !_disposed )
			{
				if( disposing )
				{
					// dispose-only, i.e. non-finalizable logic
					_sub1.Dispose();
					_sub2.Dispose();
					_sub3.Dispose();
					_component.Dispose();
				}

				// new shared cleanup logic
				_disposed = true;
			}

			// Uncomment if the base type also implements IDisposable
//			base.Dispose( disposing );
		}
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

		#region public CallSkipBlocks method
		/// <summary>
		/// Skips variable length blocks in the input stream, up to and 
		/// including the next block terminator, as performed by the 
		/// GifComponent.SkipBlocks method.
		/// </summary>
		/// <param name="inputStream"></param>
		public void CallSkipBlocks( Stream inputStream )
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
		
		#region constructor()
		/// <summary>
		/// Constructor.
		/// </summary>
		public SubComponent()
		{
			_byteArray = new byte[] { 1, 2, 3, 4, 5, 6 };
		}
		#endregion
		
		#region constructor( stream )
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="inputStream">Input stream</param>
		public SubComponent( Stream inputStream )
			: this( inputStream, false, false, false ) {}
		#endregion
		
		#region constructor( stream, bool )
		public SubComponent( Stream inputStream, bool xmlDebugging )
			: this( inputStream, xmlDebugging, false, false ) {}
		#endregion
		
		#region constructor( stream, bool, bool, bool )
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="inputStream">Input stream</param>
		/// <param name="xmlDebugging">Whether or not to create debug XML</param>
		/// <param name="nullBytes">
		/// Whether to attempt to write a null byte array to the debug XML.
		/// </param>
		/// <param name="nullInts">
		/// Whether to attempt to write a null integer array to the debug XML.
		/// </param>
		public SubComponent( Stream inputStream, bool xmlDebugging, bool nullBytes, bool nullInts )
			: base( xmlDebugging )
		{
			_byteArray = new byte[inputStream.Length];
			int count = inputStream.Read( _byteArray, 0, (int) inputStream.Length );
			WriteDebugXmlByteValues( "Bytes", _byteArray );
			if( count < 3 )
			{
				SetStatus( ErrorState.ColourTableTooShort, "Bother!" );
			}
			
			// For coverage of WriteDebugXmlByteValues( string, int[] )
			int[] intArray = new int[3] { 22, 34, 56 };
			WriteDebugXmlByteValues( "Integers", intArray );
			
			// For coverage of WriteDebugXmlElement( null )
			WriteDebugXmlElement( "NullString", null );
			
			// For coverage of WriteDebugXmlAttribute methods
			WriteDebugXmlStartElement( "WithAttributes" );
			WriteDebugXmlAttribute( "IsSet", true );
			WriteDebugXmlAttribute( "Number", 56 );
			WriteDebugXmlAttribute( "Text", "Hello world" );
			WriteDebugXmlEndElement();
			if( nullBytes )
			{
				byte[] bytes = null;
				WriteDebugXmlByteValues( "NullBytes", bytes );
			}
			
			if( nullInts )
			{
				int[] ints = null;
				WriteDebugXmlByteValues( "NullBytes", ints );
			}
			
			WriteDebugXmlFinish();
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

		#region Bytes property
		/// <summary>
		/// Gets the byte array held in this sub component.
		/// </summary>
		public byte[] Bytes
		{
			get { return _byteArray; }
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

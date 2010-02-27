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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using NUnit.Framework;
using NUnit.Extensions;
using GifComponents.Palettes;

namespace GifComponents.NUnit.Palettes
{
	/// <summary>
	/// Test fixture for the PaletteConverter class.
	/// </summary>
	[TestFixture]
	[SuppressMessage("Microsoft.Design", 
	                 "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public class PaletteConverterTest : TestFixtureBase
	{
		#region declarations
		private Form _form;
		private PropertyGrid _pg;
		private HasAPaletteProperty _hap;
		private Palette _palette;
		private TypeConverter _converter; // PaletteConverter
		private Type[] _types = 
		{
			null,
			typeof( InstanceDescriptor ),
			typeof( string ),
			typeof( int ),
			typeof( double ),
			typeof( float ),
			typeof( Palette ),
			typeof( Form ),
			typeof( System.IO.File ),
		};
		#endregion
		
		#region setup method
		/// <summary>
		/// Setup method
		/// </summary>
		[SetUp]
		public void Setup()
		{
			#region set up the form
			string paletteFile = @"ColourTables\gameboy.act";
			_palette = Palette.FromFile( paletteFile );
			
			string paletteName = Path.GetFileNameWithoutExtension( paletteFile );
			_hap = new HasAPaletteProperty( _palette, paletteName );
			
			_pg = new PropertyGrid();
			_pg.Dock = DockStyle.Fill;
			_pg.SelectedObject = _hap;

			_form = new Form();
			_form.Controls.Add( _pg );
			_form.Show();
			#endregion

			#region get the PaletteConverter from the form
			PropertyDescriptorCollection pdc 
				= TypeDescriptor.GetProperties( _pg.SelectedObject, true );
			foreach( PropertyDescriptor pd in pdc )
			{
				string name = pd.Name;
				if( name == "ThePalette" )
				{
					// it's the palette property - get its PaletteConverter
					_converter = pd.Converter;
				}
			}
			#endregion
		}
		#endregion
		
		#region teardown method
		/// <summary>
		/// Teardown method
		/// </summary>
		[TearDown]
		public void Teardown()
		{
			_pg.Dispose();
			_form.Dispose();
		}
		#endregion
		
		#region CanConvertTo
		/// <summary>
		/// Tests the CanConvertTo method
		/// </summary>
		[Test]
		public void CanConvertTo()
		{
			bool result;
			foreach( Type t in _types )
			{
				result = _converter.CanConvertTo( t );
				string typeName;
				if( t == null )
				{
					typeName = "null";
				}
				else
				{
					typeName = t.FullName;
				}
				
				switch( typeName )
				{
					case "GifComponents.Palettes.Palette":
					case "System.String":
					case "System.ComponentModel.Design.Serialization.InstanceDescriptor":
						Assert.IsTrue( result, typeName );
						break;
						
					default:
						Assert.IsFalse( result, typeName );
						break;
				}
			}
		}
		#endregion

		#region ConvertToString
		/// <summary>
		/// Tests the ConvertTo method with a target type of String.
		/// </summary>
		[Test]
		public void ConvertToString()
		{
			string expected = _palette.ToString();
			string actual = (string) _converter.ConvertTo( _palette, typeof( string ) );
			Assert.AreEqual( expected, actual );
		}
		#endregion
		
		#region ConvertToInstanceDescriptor
		/// <summary>
		/// Tests the ConvertTo method with a target type of InstanceDescriptor.
		/// </summary>
		[Test]
		public void ConvertToInstanceDescriptor()
		{
			// Call the ConvertTo method to convert the Palette instance to an
			// ImageDescriptor
			InstanceDescriptor actual 
				= (InstanceDescriptor) _converter.ConvertTo( _palette, 
				                                             typeof( InstanceDescriptor ) );
			
			// This returns an InstanceDescriptor instance with an Arguments
			// property, which is an ICollection, the first element of which 
			// should be the original Palette instance
			object[] actualArguments = (object[]) actual.Arguments;
			Palette actualPalette = (Palette) actualArguments[0];
			
			// Compare the returned Palette instance with the original one
			Assert.AreEqual( _palette.Count, actualPalette.Count );
			for( int i = 0; i < _palette.Count; i++ )
			{
				ColourAssert.AreEqual( _palette[i], actualPalette[i], "Colour " + i );
			}
		}
		#endregion

		#region ConvertToNullValue
		/// <summary>
		/// Checks that the ConvertTo method returns null if passed a null value.
		/// </summary>
		[Test]
		public void ConvertToNullValue()
		{
			object result = _converter.ConvertTo( null, typeof( string ) );
			Assert.IsNull( result );
		}
		#endregion

		#region ConvertToUnsupportedType
		/// <summary>
		/// Tests the ConvertTo method with a type not specified in the method.
		/// </summary>
		[Test]
		[ExpectedException( typeof( NotSupportedException ) )]
		public void ConvertToUnsupportedType()
		{
			try
			{
				_converter.ConvertTo( _palette, typeof( int ) );
			}
			catch( NotSupportedException ex )
			{
				string message
					= "'PaletteConverter' is unable to convert "
					+ "'GifComponents.Palettes.Palette' to 'System.Int32'";
				StringAssert.Contains( message, ex.Message );
				throw;
			}
		}
		#endregion
	}
	
	#region HasAPaletteProperty class
	/// <summary>
	/// A class to be displayed in a PropertyGrid, having a Palette property
	/// so that the PaletteConverter class can be tested.
	/// </summary>
	public class HasAPaletteProperty
	{
		private Palette _thePalette;
		private string _name;
		
		#region constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="palette">The palette</param>
		/// <param name="name">The name of this instance</param>
		public HasAPaletteProperty( Palette palette, string name )
		{
			_thePalette = palette;
			_name = name;
		}
		#endregion
		
		#region ThePalette property
		/// <summary>
		/// Gets the Palette held within this instance
		/// </summary>
		public Palette ThePalette
		{
			get { return _thePalette; }
		}
		#endregion
		
		#region Name property
		/// <summary>
		/// Gets the name of this instance
		/// </summary>
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
		#endregion
	}
	#endregion
}

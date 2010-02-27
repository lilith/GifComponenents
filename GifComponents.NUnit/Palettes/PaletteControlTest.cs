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
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Windows.Forms;
using NUnit.Framework;
using NUnit.Extensions;
using GifComponents.Palettes;

namespace GifComponents.NUnit.Palettes
{
	/// <summary>
	/// Test fixture for the PaletteControl class.
	/// </summary>
	[TestFixture]
	[SuppressMessage("Microsoft.Design", 
	                 "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable")]
	public class PaletteControlTest : TestFixtureBase
	{
		private Form _form;
		private PaletteControl _pc;
		private Palette _palette;
		
		#region setup method
		/// <summary>
		/// Instantiates a Form, a Palette and a PaletteControl, sets the 
		/// PaletteControl's value to the Palette, and adds the PaletteControl
		/// to the Form. Finally, calls the Form's show method.
		/// </summary>
		[SetUp]
		public void Setup()
		{
			string paletteFile = @"ColourTables\gameboy.act";
			_palette = Palette.FromFile( paletteFile );
			
			_pc = new PaletteControl();
			_pc.Value = _palette;

			_form = new Form();
			_form.Controls.Add( _pc );
			
			_form.Show();
		}
		#endregion
		
		#region teardown method
		/// <summary>
		/// Disposes resources used in the unit test
		/// </summary>
		[TearDown]
		public void Teardown()
		{
			_pc.Dispose();
			_form.Dispose();
		}
		#endregion
		
		#region ValueTest
		/// <summary>
		/// Checks that the Value method returns the expected Palette.
		/// </summary>
		[Test]
		public void ValueTest()
		{
			ReportStart();
			Palette expected = _palette;
			Palette actual = _pc.Value;
			Assert.AreEqual( expected.Count, actual.Count );
			for( int i = 0; i < expected.Count; i++ )
			{
				ColourAssert.AreEqual( expected[i], actual[i], "Colour " + i );
			}
			ReportEnd();
		}
		#endregion

		#region ValueNullTest
		/// <summary>
		/// Checks that the Value property of a newly-instantiated PaletteControl
		/// returns an empty palette rather than null;
		/// </summary>
		[Test]
		public void ValueNullTest()
		{
			ReportStart();
			_pc = new PaletteControl();
			// Following line will throw a NullReferenceException if the Value
			// property is not initialised correctly.
			Assert.AreEqual( 0, _pc.Value.Count );
			ReportEnd();
		}
		#endregion

		// TODO: a way to test the interactive features of the control
		// e.g. menu strip -> add, remove colours
	}
}

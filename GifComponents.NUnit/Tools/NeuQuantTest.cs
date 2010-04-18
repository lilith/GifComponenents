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
using System.Drawing;
using NUnit.Framework;
using NUnit.Extensions;
using GifComponents.Components;
using GifComponents.Tools;

namespace GifComponents.NUnit.Tools
{
	/// <summary>
	/// Test fixture for the NewQuant class.
	/// </summary>
	[TestFixture]
	[SuppressMessage("Microsoft.Naming", 
	                 "CA1704:IdentifiersShouldBeSpelledCorrectly", 
	                 MessageId = "Neu")]
	public class NeuQuantTest : TestFixtureBase
	{
		private NeuQuant _nq;
		private byte[] _rgb;
		private ColourTable _table;
		
		#region ConstructorNullArgument
		/// <summary>
		/// Checks that the correct exception is thrown when the constructor
		/// is passed null image data.
		/// </summary>
		[Test]
		[ExpectedException( typeof( ArgumentNullException ) )]
		public void ConstructorNullArgument()
		{
			try
			{
				_nq = new NeuQuant( null, 10 );
			}
			catch( ArgumentNullException ex )
			{
				Assert.AreEqual( "thePicture", ex.ParamName );
				throw;
			}
		}
		#endregion
		
	}
}

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
using NUnit.Framework;
using NUnit.Extensions;

namespace GifComponents.NUnit
{
	/// <summary>
	/// Test fixture for the GifComponentStatus class.
	/// </summary>
	[TestFixture]
	public class GifComponentStatusTest : TestFixtureBase
	{
		private GifComponentStatus _status;
		
		#region ErrorStateTest
		/// <summary>
		/// Checks that the ErrorState property works correctly.
		/// </summary>
		[Test]
		public void ErrorStateTest()
		{
			ReportStart();
			foreach( ErrorState state in ErrorState.GetValues( typeof( ErrorState ) ) )
			{
				_status = new GifComponentStatus( state, "error" );
				Assert.AreEqual( state, _status.ErrorState );
			}
			ReportEnd();
		}
		#endregion
		
		#region ErrorMessageTest
		/// <summary>
		/// Checks that the ErrorMessage property works correctly.
		/// </summary>
		[Test]
		public void ErrorMessageTest()
		{
			ReportStart();
			_status = new GifComponentStatus( ErrorState.Ok, "message" );
			Assert.AreEqual( "message", _status.ErrorMessage );
			ReportEnd();
		}
		#endregion

		#region ToStringTest
		/// <summary>
		/// Checks that the ToString method works properly.
		/// </summary>
		[Test]
		public void ToStringTest()
		{
			ReportStart();
			_status = new GifComponentStatus( ErrorState.NotANetscapeExtension 
			                                 | ErrorState.BadDataBlockIntroducer, 
			                                 "error" );
			Assert.AreEqual( "BadDataBlockIntroducer, NotANetscapeExtension", 
			                 _status.ToString() );
			ReportEnd();
		}
		#endregion
	}
}

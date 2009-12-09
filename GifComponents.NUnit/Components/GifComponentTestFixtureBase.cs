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
using System.IO;
using NUnit.Framework;
using NUnit.Extensions;
using GifComponents.Components;

namespace GifComponents.NUnit.Components
{
	/// <summary>
	/// Base class for test fixtures for classes derived from GifComponent.
	/// </summary>
	public abstract class GifComponentTestFixtureBase : TestFixtureBase
	{
		
		#region properties
		
		#region protected static ExpectedDebugXml property
		/// <summary>
		/// Gets the debug XML which is expected by the currently executing test
		/// case, based on the test case name and test fixture name.
		/// </summary>
		protected static string ExpectedDebugXml
		{
			get { return File.ReadAllText( DebugXmlFileName ); }
		}
		#endregion
		
		#region protected static DebugXmlFileName property
		/// <summary>
		/// Gets the file name of an XML file specific to the currently executing
		/// test case, from which the expected debug XML can be read.
		/// </summary>
		protected static string DebugXmlFileName
		{
			get
			{
				string fileName
					= @"DebugXml/"
					+ TestFixtureName + "."
					+ TestCaseName + ".xml";
				return fileName;
			}
		}
		#endregion

		#region protected static GifFileName property
		/// <summary>
		/// Gets the file name of a GIF data stream to be used in the currently
		/// executing test case.
		/// </summary>
		protected static string GifFileName
		{
			get
			{ 
				string fileName
					= @"images/"
					+ TestFixtureName + "."
					+ TestCaseName + ".gif";
				return fileName;
			}
		}
		#endregion
		
		#endregion
	}
}

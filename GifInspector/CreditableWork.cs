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
using System.Xml.Serialization;

namespace GifInspector
{
	/// <summary>
	/// Contains information about the work of another author which has been
	/// incorporated into your project, or of which your project contains a
	/// derivative work.
	/// Used by the LicenseFormParameters class.
	/// </summary>
	public class CreditableWork
	{
		#region declarations
		private string _author;
		private string _workName;
		#endregion
		
		#region Author property
		/// <summary>
		/// The name(s) of the authors of the creditable work.
		/// </summary>
		[XmlAttribute]
		public string Author 
		{
			get { return _author; }
			set { _author = value; }
		}
		#endregion
		
		#region WorkName property
		/// <summary>
		/// The name of the creditable work.
		/// </summary>
		[XmlAttribute]
		public string WorkName 
		{
			get { return _workName; }
			set { _workName = value; }
		}
		#endregion
	}
}

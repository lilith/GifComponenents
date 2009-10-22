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
using System.Drawing;

namespace GifInspector
{
	/// <summary>
	/// Parameters for instantiation of a <see cref="AboutForm"/>.
	/// </summary>
	public class AboutFormParameters : XmlSerializableItem<AboutFormParameters>
	{
		#region declarations
		private int _formWidth;
		private string _applicationName;
		private string _author;
		private string _copyrightYear;
		private string _extraCopyrightStatement;
		private Collection<CreditableWork> _creditableWorks;
		private string _licenseFileName;
		#endregion

		#region constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		public AboutFormParameters()
		{
			_creditableWorks = new Collection<CreditableWork>();
			_applicationName = "this application";
			_formWidth = 400;
		}
		#endregion
		
		#region properties
		
		#region FormWidth
		/// <summary>
		/// The width of the form in pixels.
		/// </summary>
		public int FormWidth
		{
			get { return _formWidth; }
			set { _formWidth = value; }
		}
		#endregion
		
		#region ApplicationName
		/// <summary>
		/// The name of the application.
		/// This will be displayed in the title bar of the About box.
		/// </summary>
		public string ApplicationName 
		{
			get { return _applicationName; }
			set { _applicationName = value; }
		}
		#endregion

		#region Author
		/// <summary>
		/// The name of the author of the application.
		/// </summary>
		public string Author 
		{
			get { return _author; }
			set { _author = value; }
		}
		#endregion

		#region CopyrightYear
		/// <summary>
		/// The year of the copyright statement (may be a range of years)
		/// </summary>
		public string CopyrightYear 
		{
			get { return _copyrightYear; }
			set { _copyrightYear = value; }
		}
		#endregion

		#region ExtraCopyrightStatement
		/// <summary>
		/// Any additional information about the license, e.g. a brief 
		/// description of the type of license.
		/// </summary>
		public string ExtraCopyrightStatement 
		{
			get { return _extraCopyrightStatement; }
			set { _extraCopyrightStatement = value; }
		}
		#endregion

		#region CreditableWorks
		/// <summary>
		/// A collection of <see cref="CreditableWorks"/> included in this
		/// application.
		/// </summary>
		public Collection<CreditableWork> CreditableWorks {
			get { return _creditableWorks; }
		}
		#endregion

		#region LicenseFileName
		/// <summary>
		/// The name of the file containing the license under which this
		/// application is released.
		/// </summary>
		public string LicenseFileName 
		{
			get { return _licenseFileName; }
			set { _licenseFileName = value; }
		}
		#endregion

		#endregion
	}
}

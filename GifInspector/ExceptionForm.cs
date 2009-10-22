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
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;

namespace GifInspector
{
	/// <summary>
	/// A form for displaying details of an unhandled exception to the user.
	/// </summary>
	public partial class ExceptionForm : Form
	{
		#region constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="ex">
		/// The exception to display information about.
		/// </param>
		public ExceptionForm( Exception ex )
		{
			//
			// The InitializeComponent() call is required for Windows Forms
			// designer support.
			//
			InitializeComponent();
			
			if( ex == null )
			{
				StackTrace trace = new StackTrace();
				textBoxExceptionText.Text 
					= "No exception information was supplied by the application"
					+ trace.ToString();
			}
			else
			{
				textBoxExceptionText.Text = ex.ToString();
			}
		}
		#endregion
	}
}

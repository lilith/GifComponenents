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
using System.IO;
using System.Windows.Forms;

namespace GifInspector
{
	/// <summary>
	/// Form to display copyright notice and license agreement
	/// </summary>
	public partial class AboutForm : Form
	{
		#region private constants
		/// <summary>
		/// Space between form edge and top/bottom of outermost controls.
		/// </summary>
		private const int _verticalPadding = 3;
		
		/// <summary>
		/// Space between form edge and left/right hand edge of outermost 
		/// controls.
		/// </summary>
		private const int _horizontalPadding = 3;
		
		/// <summary>
		/// Vertical spacing between adjacent controls.
		/// </summary>
		private const int _verticalSpacing = 3;
		#endregion
		
		#region declarations
		/// <summary>
		/// Vertical position of the next form to be added to the control.
		/// </summary>
		private int _verticalPosition;
		#endregion
		
		#region constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="parameters">
		/// An object encapsulating information such as the author's name, 
		/// copyright details and location of the license agreement.
		/// </param>
		public AboutForm( AboutFormParameters parameters )
		{
			//
			// The InitializeComponent() call is required for Windows Forms 
			// designer support.
			//
			InitializeComponent();
			
			//
			// Add constructor code after the InitializeComponent() call.
			//
			if( parameters == null )
			{
				parameters = new AboutFormParameters();
			}
			
			_verticalPosition = _verticalPadding;
			this.Width = parameters.FormWidth;
			this.Height = _verticalPadding;
			
			this.ShowInTaskbar = false;
			this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			
			this.Text = "About " + parameters.ApplicationName;
			if( string.IsNullOrEmpty( parameters.ExtraCopyrightStatement ) == false )
			{
				AddLabel( parameters.ExtraCopyrightStatement );
			}
			AddLabel( "Copyright (C) " + parameters.CopyrightYear 
			          + " " + parameters.Author );
			
			TextBox agreementBox = new TextBox();
			agreementBox.ReadOnly = true;
			agreementBox.Multiline = true;
			agreementBox.ScrollBars = ScrollBars.Vertical;
			agreementBox.Height = 200;
			agreementBox.Text = File.ReadAllText( parameters.LicenseFileName );
			AddControl( agreementBox );
			
			if( parameters.CreditableWorks.Count > 0 )
			{
				DataGridView grid = new DataGridView();
				grid.Columns.Add( "Author", "" );
				grid.Columns.Add( "WorkName", "" );
				grid.ReadOnly = true;
				grid.RowHeadersVisible = false;
				grid.ColumnHeadersVisible = false;
				grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
				grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
				grid.Height = 200;
				
				AddLabel( "Contains work by the following:" );
				foreach( CreditableWork work in parameters.CreditableWorks )
				{
					int rowCount = grid.Rows.Add( work.Author, work.WorkName );
					for( int i = 0; i < 2; i++ )
					{
						grid.Rows[rowCount].Cells[i].Style.WrapMode 
							= DataGridViewTriState.True;
					}
				}
				grid.AllowUserToAddRows = false;
				AddControl( grid );
			}
		}
		#endregion
		
		#region private AddLabel metbod
		/// <summary>
		/// Adds a label containing the supplied text to the form.
		/// </summary>
		/// <param name="text">
		/// The text of the label to add.
		/// </param>
		private void AddLabel( string text )
		{
			Label label = new Label();
			label.TextAlign = ContentAlignment.MiddleCenter;
			label.Text = text;
			AddControl( label );
		}
		#endregion
		
		#region private AddControl method
		/// <summary>
		/// Adds the supplied control to the form.
		/// </summary>
		/// <param name="control">
		/// The control to add to the form.
		/// </param>
		private void AddControl( Control control )
		{
			control.Width = this.ClientSize.Width - (_horizontalPadding * 2);
			control.Left = _horizontalPadding;
			control.Top = _verticalPosition;
			_verticalPosition += control.Height + _verticalSpacing;
			this.Controls.Add( control );
			
			this.Height += control.Height + _verticalPadding;
		}
		#endregion
	}
}

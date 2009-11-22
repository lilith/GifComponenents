#region Copyright (C) Simon Bridewell
// 
// This file is part of the GifComponents library.
// GifComponents is free software; you can redistribute it and/or
// modify it under the terms of the Code Project Open License.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// Code Project Open License for more details.
// 
// You can read the full text of the Code Project Open License at:
// http://www.codeproject.com/info/cpol10.aspx
//
// GifComponents is a derived work based on NGif written by gOODiDEA.NET
// and published at http://www.codeproject.com/KB/GDI-plus/NGif.aspx,
// with an enhancement by Phil Garcia published at
// http://www.thinkedge.com/blogengine/post/2008/02/20/Animated-GIF-Encoder-for-NET-Update.aspx
//
// Simon Bridewell makes no claim to be the original author of this library,
// only to have created a derived work.
#endregion

#region using directives
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Windows.Forms.Design;
#endregion

namespace GifComponents.Palettes
{
	/// <summary>
	/// A form for editing a <see cref="Palette"/>.
	/// </summary>
	public partial class PaletteForm : Form
	{
        private IWindowsFormsEditorService _editorService;

        #region constructor
        /// <summary>
        /// Constructor.
        /// </summary>
		public PaletteForm()
		{
			InitializeComponent();
		}
        #endregion
		
        #region properties
        
		#region Value property
		/// <summary>
		/// Gets and sets the Palette held by the PaletteControl in this form.
		/// </summary>
		[SuppressMessage("Microsoft.Usage", 
		                 "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public Palette Value
		{
			get { return paletteControl1.Value; }
			set { paletteControl1.Value = value; }
		}
		#endregion

		#region EditorService property
		/// <summary>
		/// Gets and sets the editor service which allows this control to be
		/// used to edit a Palette in a 
		/// <see cref="System.Windows.Forms.PropertyGrid"/>.
		/// </summary>
		public IWindowsFormsEditorService EditorService 
		{
			get { return _editorService; }
			set { _editorService = value; }
		}
		#endregion

		#endregion

		#region PaletteControl Validated event handler
		void PaletteControl1Validated( object sender, EventArgs e )
		{
			string title = "Palette editor: ";
			if( paletteControl1.FileName == null )
			{
				title += "untitled";
			}
			else
			{
				title += Path.GetFileName( paletteControl1.FileName );
			}
			
			if( paletteControl1.IsDirty )
			{
				title += " *";
			}
			
			this.Text = title;
		}
		#endregion
	}
}

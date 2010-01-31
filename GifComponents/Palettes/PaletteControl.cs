#region Copyright (C) Jambon Bill and Simon Bridewell
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using CommonForms;
using GifComponents;
#endregion

namespace GifComponents.Palettes
{
	/// <summary>
	/// A UserControl for editing an instance of the <see cref="Palette"/> 
	/// class, i.e. an Adobe Colour Table.
	/// </summary>
    public partial class PaletteControl : UserControl
    {
        #region declarations
		private Palette _palette;
        private bool _isDirty;
        private string _fileName;
        #endregion
        
		#region constructor
		/// <summary>
		/// Constructor.
		/// </summary>
		[SuppressMessage("Microsoft.Design", 
		                 "CA1031:DoNotCatchGeneralExceptionTypes")]
        public PaletteControl()
        {
            InitializeComponent();
            
            try
            {
	            NewPalette();
	            
	            string[] standardPalettes 
	            	= Directory.GetFiles( Application.StartupPath + "/ColourTables", 
	            	                      "*.act" );
	            foreach( string file in standardPalettes )
	            {
	            	Palette p = Palette.FromFile( file );
	            	Bitmap b = p.ToBitmap();
	            	string name = Path.GetFileNameWithoutExtension( file );
	            	ToolStripItem tsi = new ToolStripMenuItem( name, b );
	            	tsi.Name = "palette" + name;
	            	tsi.Click += ToolStripMenuItemClick;
	            	openStandardToolStripMenuItem.DropDownItems.Add( tsi );
	            }
            }
            catch( Exception ex )
            {
            	ExceptionForm ef = new ExceptionForm( ex );
            	ef.ShowDialog();
            }
        }
		#endregion

        #region properties
        
		#region Value property
		/// <summary>
		/// Gets and sets the <see cref="Palette"/> held in this control.
		/// </summary>
		[SuppressMessage("Microsoft.Usage", 
		                 "CA2227:CollectionPropertiesShouldBeReadOnly")]
		public Palette Value
		{
		    get
		    {
		        if( _palette == null )
		        {
		        	_palette = new Palette();
		        }
		        return _palette;
		    }
		    set
		    { 
		    	_palette = value;
		    	UpdateUI();
		    }
		}
		#endregion

		#region View property
		/// <summary>
		/// Gets and sets a member of the <see cref="System.Windows.Forms.View"/>
		/// enumeration which indicates how colours are arranged in the control,
		/// i.e. list, details or tile.
		/// </summary>
		public View View
        {
            get { return listView1.View; }
            set { listView1.View = value; }
        }
		#endregion
        
		#region FileName property
		/// <summary>
		/// Gets and sets the file name of the palette being edited.
		/// </summary>
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
		#endregion

		#region IsDirty property
		/// <summary>
		/// Gets a boolean value indicating whether or not the Palette has been
		/// changed since it was last saved.
		/// </summary>
        public bool IsDirty
        {
            get { return _isDirty; }
        }
		#endregion

		#endregion

        #region event handlers

        #region ContextMenu Opening event handler
        /// <summary>
        /// Occurs when the context menu is opening.
        /// Enables and disables menu items depending on whether they are valid
        /// given the current state of the control.
        /// </summary>
        /// <param name="sender">
        /// The object which raised this event
        /// </param>
        /// <param name="e">
        /// Information about the event.
        /// </param>
		[SuppressMessage("Microsoft.Design", 
                         "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void contextMenuColor_Opening( object sender, CancelEventArgs e )
        {
        	try
        	{
	            if (listView1.SelectedItems.Count == 0)
	            {
	            	editColourToolStripMenuItem.Enabled = false;
	            	deleteColourToolStripMenuItem.Enabled = false;
	            }
	            else
	            {
	            	editColourToolStripMenuItem.Enabled = true;
	            	deleteColourToolStripMenuItem.Enabled = true;
	            }
        	}
        	catch( Exception ex )
        	{
        		ExceptionForm ef = new ExceptionForm( ex );
        		ef.ShowDialog();
        	}
        }
        #endregion

		#region ToolStripMenuItem click event handler
		[SuppressMessage("Microsoft.Design", 
		                 "CA1031:DoNotCatchGeneralExceptionTypes")]
		private void ToolStripMenuItemClick( object sender, EventArgs e )
		{
			ToolStripMenuItem item = (ToolStripMenuItem) sender;
			
			try
			{
				switch( item.Name )
				{
					#region load / save / new
					case "openToolStripMenuItem":
						LoadPalette();
						break;
						
					case "saveToolStripMenuItem":
						Save();
						break;
						
					case "newToolStripMenuItem":
						NewPalette();
						break;
					#endregion
						
					#region add / delete / edit colours
					case "addColourToolStripMenuItem":
						AddColour();
						break;
						
					case "deleteColourToolStripMenuItem":
						DeleteColour();
						break;
						
					case "editColourToolStripMenuItem":
						EditColour();
						break;
					#endregion
						
					#region change ListView View property
					case "detailsToolStripMenuItem":
						listView1.View = View.Details;
						break;
						
					case "listToolStripMenuItem":
						listView1.View = View.List;
						break;
						
					case "tilesToolStripMenuItem":
						listView1.View = View.Tile;
						break;
					#endregion
						
					default:
						if( item.Name.StartsWith( "palette" ) )
						{
							string fileName = item.Name.Remove( 0, 7 ) + ".act";
							LoadPalette( @"ColourTables/" + fileName );
							break;
						}
						string message = "Unexpected control: " + item.Name;
						throw new ArgumentException( message, "sender" );
				}
			}
			catch( Exception ex )
			{
				ExceptionForm ef = new ExceptionForm( ex );
				ef.ShowDialog();
			}
		}
		#endregion

		#region ListView double click event handler
		[SuppressMessage("Microsoft.Design", 
		                 "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
        	try
        	{
        		EditColour();
        	}
        	catch( Exception ex )
        	{
        		ExceptionForm ef = new ExceptionForm( ex );
        		ef.ShowDialog();
        	}
        }
		#endregion

        // FEATURE: a public property to enable / disable keydown events?
        #region ListView KeyDown event handler
		[SuppressMessage("Microsoft.Design", 
                         "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void listView1_KeyDown( object sender, KeyEventArgs e )
        {
        	try
        	{
	            switch( e.KeyValue )
	            {
	
	                case 65://A
	                    AddColour();
	                    break;
	
	                case 46://Suppr (delete)
	                    DeleteColour();
	                    break;
	
	                default:
	                    break;
	                      
	            }
        	}
        	catch( Exception ex )
        	{
        		ExceptionForm ef = new ExceptionForm( ex );
        		ef.ShowDialog();
        	}
        }
        #endregion

		#endregion

        #region private methods
        
        #region private static SelectFile method
        /// <summary>
        /// Prompts the user to browse for an Adobe Colour Table file with an
        /// OpenFileDialog.
        /// </summary>
        /// <param name="initialDirectory">
        /// The folder in which the OpenFileDialog is initially positioned
        /// </param>
        /// <returns>
        /// The path to the file selected by the user, or null if the 
        /// OpenFileDialog was closed without selecting a file.
        /// </returns>
        private static string SelectFile( string initialDirectory )
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Adobe color table files (*.act)|*.act";
            dialog.InitialDirectory = initialDirectory;
            dialog.Title = "Select a file";
            return (dialog.ShowDialog() == DialogResult.OK)
               ? dialog.FileName : null;
        }
        #endregion

        #region private LoadPalette method
        /// <summary>
        /// Loads a Palette from an Adobe Colour Table file
        /// </summary>
        /// <param name="fileName">
        /// Path to the file to load.
        /// </param>
        private void LoadPalette( string fileName )
        {
			_palette = Palette.FromFile( fileName );
			_fileName = fileName;
            UpdateUI();
            _isDirty = false;
        }

        #endregion

        #region private LoadPalette method
        private void LoadPalette()
        {
        	if( CheckSaveChangedPalette() != DialogResult.Cancel )
        	{
	            string fn = SelectFile( Application.StartupPath );
	            if( fn == null ) 
	            {
	            	return;
	            }
	            LoadPalette( fn );
        	}
        }
        #endregion
        
        #region private SavePaletteAs method
        /// <summary>
        /// Saves the current palette as an Adobe Colour Table file, prompting
        /// the user to choose a path and filename with a SaveFileDialog.
        /// </summary>
        private void SavePaletteAs()
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Adobe color table files (*.act)|*.act";
            dialog.Title = "Name your file";

            if( dialog.ShowDialog() == DialogResult.OK )
            {
                _fileName = dialog.FileName;
                Save();
            }
            else
            {
                return;
            }

        }
        #endregion

        #region private Save method
        /// <summary>
        /// Saves the current palette to an Adobe Colour Table file using the
        /// filename it was loaded from.
        /// If it doesn't have a filename yet, the user is prompted to set one
        /// using a SaveFileDialog.
        /// </summary>
        private void Save()
        {
			if( string.IsNullOrEmpty( _fileName ) )
            {
         SavePaletteAs();
                return;
            }
			else
			{
				_palette.WriteToFile( _fileName );
			}

            string basename = System.IO.Path.GetFileName( _fileName );
            string message = "File " + basename + " saved";
            CleverMessageBox.Show( message, 
                                   "Saved", 
                                   MessageBoxButtons.OK, 
                                   MessageBoxIcon.Exclamation, 
                                   this );
            _isDirty = false;
        }
        #endregion

        #region private NewPalette method
        /// <summary>
        /// Populates the control with a new Palette.
        /// If the Palette currently held in the control has been changed since
        /// it was last saved, the user is asked whether they want to save any
        /// changes to it.
        /// </summary>
        private void NewPalette()
        {
        	if( CheckSaveChangedPalette() != DialogResult.Cancel )
        	{
				_palette = new Palette();
	            UpdateUI();
        	}
        }
        #endregion

        #region private UpdateUI method
        /// <summary>
        /// Updates the control, showing the latest changes to the Palette.
        /// </summary>
        private void UpdateUI()
        {
            listView1.Items.Clear();
            for( int i = 0; i < _palette.Count; i++ )
            {
                Color c = _palette[i];
                Color fc = Color.Black;

                string rgb = c.R + "," + c.G + "," + c.B;
                int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                if (luma < 128)
                {
                    fc = Color.White;// FORECOLOR
                }
                string hex = String.Format( CultureInfo.InvariantCulture, 
                                            "#{0:X2}{1:X2}{2:X2}", 
                                            c.R, c.G, c.B);
                ListViewItem lvi = new ListViewItem( rgb );
                lvi.SubItems.Add( hex );
                lvi.BackColor = c;
                lvi.ForeColor = fc;
                lvi.SubItems[1].ForeColor = fc;
                listView1.Items.Add( lvi );
            }
            OnValidated( new EventArgs() );
        }
        #endregion

        #region private AddColour method
        /// <summary>
        /// Allows the user to add a colour to the palette, using a
        /// <see cref="System.Windows.Forms.ColorDialog"/>.
        /// </summary>
        private void AddColour()
        {
            if ( _palette.Count >= 256 )
            {
                CleverMessageBox.Show( "Colour table is full", 
            	                       "Error", 
            	                       MessageBoxButtons.OK, 
            	                       MessageBoxIcon.Exclamation, 
            	                       this );
                return;
            }
            
            ColorDialog CD = new ColorDialog();
            CD.FullOpen = true;
            DialogResult result = CD.ShowDialog();
            if( result.Equals( DialogResult.OK ) )
            {
                Color n = CD.Color;
                _palette.Add( n );
                _isDirty = true;
                UpdateUI();
            }
        }
        #endregion

        #region private EditColour method
        /// <summary>
        /// Allows the user to edit the currently selected colour in the Palette,
        /// using a <see cref="System.Windows.Forms.ColorDialog"/>.
        /// </summary>
        private void EditColour()
        {
        	// If no colour is selected, don't do anything
            if( listView1.SelectedItems.Count == 0 )
            {
            	return;
            }
            
            int id = listView1.SelectedIndices[0];
            
            ColorDialog CD = new ColorDialog();
            CD.Color = _palette[id];
            CD.FullOpen = true;
            DialogResult result = CD.ShowDialog();
            if( result.Equals( DialogResult.OK ) )
            {
                UpdateColour( id, CD.Color );
            }
        }
        #endregion

        #region private UpdateColour method
        /// <summary>
        /// Updates the colour at the specified index in the Palette, replacing
        /// it with the supplied colour.
        /// </summary>
        /// <param name="id">
        /// The index within the Palette of the colour to be replaced.
        /// </param>
        /// <param name="c">
        /// The colour to replace it with.
        /// </param>
        private void UpdateColour( int id, Color c )
        {
            
            _palette[id] = c;
            _isDirty = true;            
            
            listView1.Items[id].BackColor = c;

            int luma = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
            if( luma < 128 )
            {
                listView1.Items[id].ForeColor = Color.White;
            }
            else
            {
                listView1.Items[id].ForeColor = Color.Black;
            }

            listView1.Items[id].Text = c.R + "," + c.G + "," + c.B;
            listView1.Items[id].SubItems[1].Text 
            	= "#" 
            	+ String.Format( CultureInfo.InvariantCulture, 
            	                 "#{0:X2}{1:X2}{2:X2}", 
            	                 c.R, c.G, c.B);
        }
        #endregion

        #region private DeleteColour method
        /// <summary>
        /// Removes the currently selected colour from the Palette.
        /// </summary>
        private void DeleteColour()
        {
        	// If no colour is selected, don't do anything
            if( listView1.SelectedItems.Count == 0 )
            {
            	return;
            }
            
            int id = listView1.SelectedIndices[0];
            _palette.RemoveAt(id);
            _isDirty = true;
            UpdateUI();
        }
        #endregion

        #region private CheckSaveChangedPalette method
        private DialogResult CheckSaveChangedPalette()
        {
            if( _isDirty )
            {
				string message = "Palette was changed, do you want to save it first ?";
				string caption = "Save first ?";
				
				DialogResult result 
					= CleverMessageBox.Show( message,
					                         caption,
					                         MessageBoxButtons.YesNoCancel,
					                         MessageBoxIcon.Question, 
					                         this );
				return result;
            }
            return DialogResult.OK;
        }
        #endregion
        
        #endregion

    }
}

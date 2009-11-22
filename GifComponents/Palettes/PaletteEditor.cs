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
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using CommonForms;
#endregion

namespace GifComponents.Palettes
{
	/// <summary>
	/// Allows a PaletteControl to be used to edit <see cref="Palette"/> 
	/// properties in a
	/// <see cref="System.Windows.Forms.PropertyGrid"/>.
	/// </summary>
	public class PaletteEditor : UITypeEditor, IDisposable
	{
		private PaletteForm _paletteForm;

		#region public override methods
		
		#region public override GetEditStyle
		/// <summary>
		/// Gets a <see cref="System.Drawing.Design.UITypeEditorEditStyle"/>
		/// indicating that the Palette should be edited using a modal form.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		public override UITypeEditorEditStyle GetEditStyle( ITypeDescriptorContext context )
		{
			return UITypeEditorEditStyle.Modal;
		}
		#endregion
		
		#region public override EditValue method
		/// <summary>
		/// Tells Windows Forms how to edit an instance of a Palette.
		/// </summary>
		/// <param name="context">Contextual information</param>
		/// <param name="provider">Retrieves a service provider</param>
		/// <param name="value">The initial value of the Palette</param>
		/// <returns>
		/// The new value of the Palette after it has been edited by the user.
		/// </returns>
		[SuppressMessage("Microsoft.Design", 
		                 "CA1031:DoNotCatchGeneralExceptionTypes")]
		public override object EditValue( ITypeDescriptorContext context, 
		                                  IServiceProvider provider, 
		                                  object value )
		{
			Palette original = (Palette) value;
			try
			{
				IWindowsFormsEditorService editorService 
					= (IWindowsFormsEditorService) 
					provider.GetService( typeof( IWindowsFormsEditorService ) );
				
				_paletteForm = new PaletteForm();
				
				if( value == null )
				{
					value = new Palette();
				}
				
				// Take a copy of the original Palette in case the user cancels
				// the PaletteForm
				Palette copy = new Palette();
				foreach( Color c in original )
				{
					copy.Add( c );
				}
				
				_paletteForm.Value = original;
				_paletteForm.EditorService = editorService;
				DialogResult result = editorService.ShowDialog( _paletteForm );
				if( result == DialogResult.OK )
				{
					return _paletteForm.Value;
				}
				else
				{
					return copy;
				}
			}
			catch( Exception ex )
			{
				ExceptionForm ef = new ExceptionForm( ex );
				ef.ShowDialog();
				return original;
			}
		}
		#endregion
		
		#region public override GetPaintValueSupported method
		/// <summary>
		/// Gets a boolean value indicating whether a Palette can be represented
		/// by a small bitmap.
		/// </summary>
		/// <param name="context">Contextual information.</param>
		/// <returns>True.</returns>
		public override bool GetPaintValueSupported( ITypeDescriptorContext context ) 
		{
			return true;
		}
		#endregion

		#region public override PaintValue method
		/// <summary>
		/// Paints a small icon representing the current value of a Palette onto
		/// a PropertyGrid.
		/// </summary>
		/// <param name="e">
		/// Event arguments.
		/// </param>
		public override void PaintValue( PaintValueEventArgs e ) 
		{
			if( e == null )
			{
				throw new ArgumentNullException( "e" );
			}
			
			Palette p = (Palette) e.Value;
			
			Bitmap b;
			if( p.Count > 0 )
			{
				// Build an icon out of the colours in the palette
				b = p.ToBitmap();
			}
			else
			{
				// Build an icon representing an empty palette
				b = GetEmptyPaletteBitmap();
			}

			e.Graphics.DrawImage( b, e.Bounds );

			b.Dispose();			
		}
		#endregion

		#endregion
		
		#region private static GetEmptyPaletteBitmap method
		/// <summary>
		/// Gets a bitmap representing a palette with no colours.
		/// </summary>
		/// <returns>
		/// A bitmap representing a palette with no colours.
		/// </returns>
		private static Bitmap GetEmptyPaletteBitmap()
		{
			Bitmap b = new Bitmap( 16, 16 );
			for( int i = 0; i < 8; i++ )
			{
				b.SetPixel( i, i, Color.Red );
				b.SetPixel( i, 15 - i, Color.Red );
				b.SetPixel( 15 - i, i, Color.Red );
				b.SetPixel( 15 - i, 15 - i, Color.Red );
			}
			return b;
		}
		#endregion

		#region IDisposable implementation
		private bool _isDisposed; // defaults to false
		
		/// <summary>
		/// Disposes resources used by this class.
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( true );
		}
		
		/// <summary>
		/// Disposes resources used by this class.
		/// </summary>
		/// <param name="isDisposing">
		/// Indicates whether this method is being called by the class's Dispose
		/// method (true) or by the garbage collector (false).
		/// </param>
		protected virtual void Dispose( bool isDisposing )
		{
			if( _isDisposed )
			{
				return;
			}
			
			if( isDisposing )
			{
				_paletteForm.Dispose();
			}
			
			_isDisposed = true;
		}
		#endregion
	}
}

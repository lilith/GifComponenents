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
using System.Diagnostics.CodeAnalysis;
using System.Globalization; 
using System.IO; 
using System.Xml.Serialization; 

namespace GifInspector
{
	/// <summary>
	/// A Generic class which provides serialization methods and properties,
	/// regardless of the item type.
	/// Your class should inherit from this class, specifying the item type in
	/// angle brackets, for example...
	/// <code>
	/// public class Elephant : SerializableItem&lt;Elephant&gt;
	/// {
	/// 	// ... your code here ...
	/// }
	/// </code>
	/// ... to define a Elephant class.
	/// The item type is referred to as T in the comments throughout this class.
	/// </summary>
	/// <typeparam name="T">
	/// The type which derives from XmlSerializable item in order to inherit its
	/// serialization and deserialization methods.
	/// </typeparam>
	public abstract class XmlSerializableItem<T>
	{
		[SuppressMessage("Microsoft.Performance", 
		                 "CA1823:AvoidUnusedPrivateFields")]
		private object _notWanted;
		
		#region ToXml method (object to xml string)
		/// <summary>
		/// Serializes the T to an XML string.
		/// </summary>
		/// <returns>
		/// An XML string representing the object being serialized.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// An InvalidOperationException occurred whilst serializing the item.
		/// The type you are trying to serialize does not support serialization.
		/// Maybe it, or the type of one of its public members, is missing a
		/// parameterless constructor.
		/// </exception>
		public string ToXml()
		{
			try
			{
				XmlSerializer xs = new XmlSerializer( this.GetType() );
				TextWriter xw 
					= new StringWriter( CultureInfo.InvariantCulture );
				xs.Serialize( xw, this );
				return Convert.ToString( xw, CultureInfo.InvariantCulture );
			}
			catch( InvalidOperationException ex )
			{
				string message 
					= "Error serializing " + this.GetType().Name + " to XML";
				throw new InvalidOperationException( message, ex );
			}
		}
		#endregion
		
		#region SaveXml method (object to xml file)
		/// <summary>
		/// Serializes the item to XML and saves it to a text file.
		/// </summary>
		/// <param name="fileName">
		/// The name of the file to save the serialized XML to.
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// An InvalidOperationException occurred whilst serializing the item.
		/// The type you are trying to serialize does not support serialization.
		/// Maybe it, or the type of one of its public members, is missing a
		/// parameterless constructor.
		/// </exception>
		public void SaveXml( string fileName )
		{
			File.WriteAllText( fileName, ToXml() );
		}
		#endregion

		#region static FromXml method
		/// <summary>
		/// Deserializes an XML string to an instance of T.
		/// TODO: check the XML against a schema for the type and report any errors
		/// </summary>
		/// <param name="xml">
		/// An XML string representing an instance of the supplied System.Type.
		/// </param>
		/// <returns>
		/// The System.Object represented by the supplied XML string.
		/// </returns>
		/// <exception cref="InvalidDataException">
		/// The XML string could not be deserialized to the given type. See the
		/// InnerException for more information.
		/// </exception>
		/// <exception cref="ArgumentNullException">
		/// The supplied System.Type is null.
		/// </exception>
		[SuppressMessage("Microsoft.Design", 
		                 "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static T FromXml( string xml )
		{
			try
			{
				XmlSerializer xs = new XmlSerializer( typeof( T ) );
				TextReader tr = new StringReader( xml );
				return (T) xs.Deserialize( tr );
			}
			catch( InvalidOperationException ex )
			{
				#region handle the exception
				string message = "XmlDeserializer.FromXmlString was unable to "
					+ "deserialize an XML string to an instance of the type "
					+ typeof( T ).ToString()
					+ ". "
					+ Environment.NewLine
					+ Environment.NewLine
					+ "This could mean the XML string is corrupt, or it could mean "
					+ "you are trying to deserialize an XML string which is a "
					+ "representation of a completely different System.Type."
					+ Environment.NewLine
					+ Environment.NewLine
					+ "The XML string is: "
					+ xml;
				throw new InvalidDataException( message, ex );
				#endregion
			}
		}
		#endregion
		
		#region static LoadXml method
		/// <summary>
		/// Deserializes an XML file to an instance of the supplied System.Type.
		/// </summary>
		/// <param name="fileName">
		/// An XML file representing an instance of the supplied System.Type.
		/// </param>
		/// <returns>
		/// The System.Object represented by the supplied XML file.
		/// </returns>
		[SuppressMessage("Microsoft.Design", 
		                 "CA1000:DoNotDeclareStaticMembersOnGenericTypes")]
		public static T LoadXml( string fileName )
		{
			string xml = File.ReadAllText( fileName );
			return FromXml( xml );
		}
		#endregion
		
		#region Ignore method
		/// <summary>
		/// A neat way of doing something with the value parameter of the set
		/// accessor of a public property for which the get accessor returns
		/// a derived or calculated value, and the set accessor has only been
		/// included in order for the property to be serialized by the ToXml
		/// method.
		/// This is a workaround for the FxCop rule ReviewUnusedParameters
		/// which will otherwise be violated by an empty set accessor.
		/// </summary>
		/// <param name="rubbish">
		/// The object which is passed to the set accessor, to be ignored.
		/// </param>
		public void Ignore( object rubbish )
		{
			_notWanted = rubbish;
		}
		#endregion

	}
}

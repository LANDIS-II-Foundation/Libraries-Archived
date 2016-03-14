using System.Collections.Generic;

namespace Landis.RasterIO
{
	/// <summary>
	/// The metadata for a raster.
	/// </summary>
	/// <remarks>
	/// A simple implementation of IMetadata interface that library
	/// implementors may use.
	/// </remarks>
	public class Metadata
		: IMetadata
	{
		private Dictionary<string, object> dictionary;

		//---------------------------------------------------------------------

		public object this[string name]
		{
			get {
				return dictionary[name];
			}
			set {
				dictionary[name] = value;
			}
		}

		//---------------------------------------------------------------------

		public Metadata()
		{
			dictionary = new Dictionary<string, object>();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets a metadata value by name as a specific data type.
		/// </summary>
		/// <exception cref="System.InvalidCastException">
		/// Thrown if the metadata value cannot be converted to the specific
		/// type.
		/// </exception>
		/// <returns>
		/// true if there is a metadata value associated with the name, and it
		/// was converted to the specified data type and assigned to the out
		/// parameter.  false if there is no metadata associated with the name.
		/// </returns>
		public bool TryGetValue<T>(string name,
		                    	   ref T  dataValue)
		{
			object val;
			if (! dictionary.TryGetValue(name, out val))
				return false;
			dataValue = (T) val;
			return true;
		}
	}
}

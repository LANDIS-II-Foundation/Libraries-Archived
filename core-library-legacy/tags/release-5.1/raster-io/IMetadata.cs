namespace Landis.RasterIO
{
	/// <summary>
	/// The metadata for a raster.
	/// </summary>
	public interface IMetadata
	{
		/// <summary>
		/// Gets a specific metadata value by name.
		/// </summary>
		/// <returns>
		/// null if there is no metadata associated with the given name.
		/// </returns>
		object this[string name]
		{
			get;
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
		/// was converted to the specified data type and assigned to the
		/// dataValue parameter.  false if there is no metadata associated
		/// with the name.
		/// </returns>
		bool TryGetValue<T>(string name,
		                    ref T  dataValue);
	}
}

namespace Landis.RasterIO
{
	/// <summary>
	/// An individual band value in a raster pixel.
	/// </summary>
	public interface IPixelBand
	{
		/// <summary>
		/// The type code for the band's data type.
		/// </summary>
		System.TypeCode TypeCode
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets the byte sequence representing the band's value.
		/// </summary>
		byte[] GetBytes();

		//---------------------------------------------------------------------


		/// <summary>
		/// Sets the byte sequence represeting the band's value.
		/// </summary>
		/// <param name="startIndex">
		/// The index in the byte array where the bytes for the band's value
		/// are located.
		/// </param>
		void SetBytes(byte[] bytes,
		              int    startIndex);
	}
}

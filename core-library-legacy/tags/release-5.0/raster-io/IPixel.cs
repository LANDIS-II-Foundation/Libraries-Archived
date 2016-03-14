namespace Landis.RasterIO
{
	/// <summary>
	/// A raster pixel with one or more bands.
	/// </summary>
	public interface IPixel
	{
		/// <summary>
		/// The number of bands.
		/// </summary>
		int BandCount
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Accesses a particular band by its index.
		/// </summary>
		/// <param name="index">
		/// The index of the desired band.
		/// </param>
		/// <exception cref="System.IndexOutOfRangeException">
		/// index is not in the range of 0 to BandCount-1.
		/// </exception>
		IPixelBand this[int index]
		{
			get;
		}
	}
}

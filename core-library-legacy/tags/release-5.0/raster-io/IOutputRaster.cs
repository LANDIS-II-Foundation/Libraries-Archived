namespace Landis.RasterIO
{
	/// <summary>
	/// An output raster file to which pixel data are written.  Pixels are
	/// written in row-major order, from the upper-left corner to the
	/// lower-right corner.
	/// </summary>
	public interface IOutputRaster<T>
		: IRaster
		where T : IPixel
	{
		/// <summary>
		/// The number of pixels written to the raster.
		/// </summary>
		int PixelsWritten
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Writes a pixel to the raster.
		/// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// This method was called too many times (more than the number of
        /// pixels in the raster).
        /// </exception>
		/// <exception cref="System.IO.IOException">
		/// An error occurred writing the pixel data to the raster.
		/// </exception>
		void WritePixel(T pixel);
	}
}

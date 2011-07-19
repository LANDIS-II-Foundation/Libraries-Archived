namespace Landis.Raster
{
	/// <summary>
	/// An input raster file to which pixel data are written.  Pixels are
	/// written in row-major order, from the upper-left corner to the
	/// lower-right corner.
	/// </summary>
	public interface IOutputRaster<T>
		: IRaster
		where T : IPixel
	{
		/// <summary>
		/// Writes a pixel to the raster.
		/// </summary>
		/// <exception cref="System.IO.EndOfStreamException">
		/// Attempted to write after the last pixel in the raster.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An error occurred writing the pixel data to the raster.
		/// </exception>
		void WritePixel(T pixel);
	}
}

using Edu.Wisc.Forest.Flel.Grids;

namespace Landis.RasterIO
{
	/// <summary>
	/// A factory that produces rasters by opening raster files for reading and
	/// by creating new files for writing.
	/// </summary>
	public interface IRasterFactory
	{
		/// <summary>
		/// Opens an existing raster file for reading.
		/// </summary>
		/// <param name="path">
		/// The path of the raster file.
		/// </param>
		/// <remarks>
		/// The raster file must have the same number of bands as the specified
		/// pixel class <i>TPixel</i>.  Furthermore, the datatype of each pixel
		/// band must support the corresponding band in the raster file without
		/// loss of data.
		/// <pre>
		///     Pixel Band     Types of Raster Band Supported
		///     ----------     ------------------------------
		///     byte           byte
		///     sbyte          sbyte
		///     short          short, byte, sbyte
		///     ushort         ushort, byte
		///     int            int, short, ushort, byte, sbyte
		///     uint           uint, ushort, byte
		///     float          float, short, ushort, byte, sbyte
		///     double         double, float, int, uint, short, ushort, byte, sbyte
		/// </pre>
		/// For example, if a pixel band is of type "ushort", then the
		/// corresponding band in the raster file must be either "ushort" or
		/// "byte".
		/// </remarks>
		/// <exception cref="System.ArgumentException">
		///     path is an empty string (""). 
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		///     path is null. 
		/// </exception>
		/// <exception cref="System.IO.FileNotFoundException">
		///     The file cannot be found. 
		/// </exception>
		/// <exception cref="System.IO.DirectoryNotFoundException">
		///     The drive or directory portion of the path is invalid.
		/// </exception>
		/// <exception cref="System.FormatException">
		///     The file extension in the path is an unknown raster format.
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		///     The driver cannot read rasters with the format specified in the
		///     path; it can only write rasters in that format.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		///     An error occurred reading the raster file.
		/// </exception>
		/// <exception cref="System.ApplicationException">
		///     <list type="bullet">
		///     <item>
		///         The number of bands in the pixel class <i>TPixel</i> is
		///         different than the number of bands in the raster; or
		///     </item>
		///     <item>
		///         The data type of a band in the pixel class <i>TPixel</i> is
		///         not large enough to hold the data values of the
		///         corresponding band in the raster; or
		///     </item>
		///     <item>
		///         The raster's header is missing, incomplete, or invalid.
		///     </item>
		///     </list>
		/// </exception>
		IInputRaster<TPixel> OpenRaster<TPixel>(string path)
			where TPixel : IPixel, new();

		//---------------------------------------------------------------------

		/// <summary>
		/// Creates a raster file for writing.
		/// </summary>
		/// <param name="path">
		/// The path of the raster file.
		/// </param>
		/// <param name="dimensions">
		/// The dimensions of the raster file.
		/// </param>
		/// <param name="metadata">
		/// The metadata for the raster file.
		/// </param>
		/// <exception cref="System.ArgumentException">
		///     path is an empty string (""). 
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		///     path is null. 
		/// </exception>
		/// <exception cref="System.IO.PathTooLongException">
		///     The specified path, file name, or both exceed the system-defined
		///     maximum length. For example, on Windows-based platforms, paths
		///     must be less than 248 characters, and file names must be less
		///     than 260 characters. 
		/// </exception>
		/// <exception cref="System.IO.DirectoryNotFoundException">
		///     The drive or directory portion of the path is invalid.
		/// </exception>
		/// <exception cref="System.UnauthorizedAccessException">
		///     Access is denied. 
		/// </exception>
 		/// <exception cref="System.Security.SecurityException">
 		///     The caller does not have the required permission. 
 		/// </exception>
		/// <exception cref="System.FormatException">
		///     The file extension in the path is an unknown raster format.
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		///     The driver cannot write rasters with the format specified in
		///     the path; it can only read rasters in that format.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		///     An error occurred when writing header to the raster file.
		/// </exception>
		/// <exception cref="System.ApplicationException">
		///     <list type="bullet">
		///     <item>
		///         The raster format does not support the number of bands in
		///         the pixel class <i>TPixel</i>; or
		///     </item>
		///     <item>
		///         The raster format does not support the data type of a band
		///         in the pixel class <i>TPixel</i>.
		///     </item>
		///     </list>
		/// </exception>
 		IOutputRaster<TPixel> CreateRaster<TPixel>(string     path,
		                                           Dimensions dimensions,
		                                           IMetadata  metadata)
 			where TPixel : IPixel, new();
	}
}

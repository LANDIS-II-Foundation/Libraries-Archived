namespace Landis.Raster
{
	/// <summary>
	/// A driver for opening raster files for reading and creating new files
	/// for writing.
	/// </summary>
	public interface IDriver
	{
		/// <summary>
		/// The list of file extensions that this driver recognizes; a file
		/// extension denotes a particular file format.
		/// </summary>
		string[] FileExtensions
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Opens an existing raster file for reading.
		/// </summary>
		/// <param name="path">
		/// The path of the raster file.
		/// </param>
		/// <remarks>
		/// The raster file must have the same number of bands as the specified
		/// pixel class <i>T</i>.  Furthermore, the datatype of each pixel
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
		/// path is an empty string (""). 
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// path is null. 
		/// </exception>
		/// <exception cref="System.IO.FileNotFoundException">
		/// The file cannot be found. 
		/// </exception>
		/// <exception cref="System.IO.DirectoryNotFoundException">
		/// The drive or directory portion of the path is invalid.
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// A band in the raster file has a datatype that is not supported by
		/// the corresponding band in the pixel class <i>T</i>.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An error occurred reading the raster file.
		/// </exception>
		IInputRaster<T> Open<T>(string path)
			where T : IPixel, new();

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
		/// <exception cref="System.ArgumentException">
		/// path is an empty string (""). 
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// path is null. 
		/// </exception>
		/// <exception cref="System.IO.PathTooLongException">
		/// The specified path, file name, or both exceed the system-defined
		/// maximum length. For example, on Windows-based platforms, paths
		/// must be less than 248 characters, and file names must be less
		/// than 260 characters. 
		/// </exception>
		/// <exception cref="System.IO.DirectoryNotFoundException">
		/// The drive or directory portion of the path is invalid.
		/// </exception>
		/// <exception cref="System.NotSupportedException">
		/// A band in the raster file has a datatype that is not supported by
		/// the corresponding band in the pixel class <i>T</i>.
		/// </exception>
		/// <exception cref="System.IO.IOException">
		/// An error occurred when writing data to the raster file.
		/// </exception>
		/// <exception cref="System.UnauthorizedAccessException">
		/// Access is denied. 
		/// </exception>
 		/// <exception cref="System.Security.SecurityException">
 		/// The caller does not have the required permission. 
 		/// </exception>
 		IOutputRaster<T> Create<T>(string     path,
		                           Dimensions dimensions,
		                           IMetadata  metadata)
 			where T : IPixel, new();
	}
}

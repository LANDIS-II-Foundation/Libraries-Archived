namespace Landis.RasterIO
{
	/// <summary>
	/// A driver for opening raster files for reading and creating new files
	/// for writing.
	/// </summary>
	public interface IDriver
	    : IRasterFactory
	{
		/// <summary>
		/// The list of raster formats that this driver recognizes.  A format
		/// is denoted by a filename extension, e.g., ".xyx".
		/// </summary>
		string[] Formats
		{
			get;
		}
	}
}

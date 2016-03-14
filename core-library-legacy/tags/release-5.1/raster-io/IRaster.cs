using Edu.Wisc.Forest.Flel.Grids;

namespace Landis.RasterIO
{
	/// <summary>
	/// A file with raster data.
	/// </summary>
	public interface IRaster
		: System.IDisposable
	{
		/// <summary>
		/// The path used to open/create the raster.
		/// </summary>
		string Path
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The dimensions of the raster.
		/// </summary>
		Dimensions Dimensions
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Metadata for the raster.
		/// </summary>
		IMetadata Metadata
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Closes the raster, releasing any unmanaged resources.
		/// </summary>
		void Close();
	}
}

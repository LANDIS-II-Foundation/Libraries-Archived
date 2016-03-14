using Edu.Wisc.Forest.Flel.Grids;

namespace Landis.RasterIO
{
	/// <summary>
	/// An output raster file to which pixel data are written.
	/// </summary>
	/// <remarks>
	/// A helper class for implementing the IOutputRaster interface.
	/// </remarks>
	public abstract class OutputRaster
		: Raster
	{
		private int pixelsWritten;
		private bool disposed;

		//---------------------------------------------------------------------

		/// <summary>
		/// The number of pixels written to the raster.
		/// </summary>
		public int PixelsWritten
		{
			get {
				if (disposed)
					throw new System.ObjectDisposedException(null);
				return pixelsWritten;
			}
		}

		//---------------------------------------------------------------------

		public OutputRaster(string     path,
		                    Dimensions dimensions,
		                    IMetadata  metadata)
			: base(path, dimensions, metadata)
		{
			this.pixelsWritten = 0;
			this.disposed = false;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Increments the number of pixels written by 1.
		/// </summary>
        /// <exception cref="System.InvalidOperationException">
		/// The number of pixels written is equal to the number of pixels
		/// allowed by the raster's dimensions.
		/// </exception>
		/// <remarks>
		/// The WritePixel method of derived classes should call this method
		/// before actually writing a pixel's data.
		/// </remarks>
		protected void IncrementPixelsWritten()
		{
			if (disposed)
				throw new System.ObjectDisposedException(null);
			if (pixelsWritten >= PixelCount)
				throw new System.InvalidOperationException("Trying to write extra pixel to output raster");
			pixelsWritten++;
		}

		//---------------------------------------------------------------------

		/// <summary>Dispose unmanaged resources and possibly managed
		/// resources as well.
		/// </summary>
		/// <param name="disposeManaged">
		/// If true, then managed resources should be disposed along with
		/// unmanaged resources.  If false, then only unmanaged resources
		/// should be disposed.
		/// </param>
		protected override void Dispose(bool disposeManaged)
		{
			if (! disposed) {
				if (pixelsWritten < PixelCount) {
					PartialOutputRaster.Closed(this);
				}
			}
			disposed = true;
			base.Dispose(disposeManaged);
		}
	}
}

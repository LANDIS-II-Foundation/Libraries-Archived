namespace Landis.RasterIO
{
	/// <summary>
	/// An input file with raster data.
	/// </summary>
	/// <remarks>
	/// This purpose of this abstract class is to aid driver implementors.
	/// </remarks>
	public abstract class InputRaster
		: Raster
	{
		private uint pixelsRead;
		private bool disposed;

		//---------------------------------------------------------------------

		/// <summary>
		/// The number of pixels read.
		/// </summary>
		/// <remarks>
		/// This property specifically represents the number of times that
		/// the IInputRaster.Read method has been called.
		/// </remarks>
		public uint PixelsRead
		{
			get {
				if (disposed)
					throw new System.ObjectDisposedException(null);
				return pixelsRead;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public InputRaster(string path)
			: base(path)
		{
			this.pixelsRead = 0;
			this.disposed = false;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Increments the number of pixels read by 1.
		/// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// The number of pixels read is equal to the pixel count in the
        /// raster (according to its dimensions).
        /// </exception>
		protected void IncrementPixelsRead()
		{
			if (disposed)
				throw new System.ObjectDisposedException(null);
			if (pixelsRead >= PixelCount)
				throw new System.InvalidOperationException("Trying to read more pixels than input raster has.");
			pixelsRead++;
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
				disposed = true;
				base.Dispose(disposeManaged);
			}
		}
	}
}

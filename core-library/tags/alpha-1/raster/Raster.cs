namespace Landis.Raster
{
	/// <summary>
	/// A file with raster data.
	/// </summary>
	/// <remarks>
	/// This purpose of this abstract class is to aid driver implementors.
	/// </remarks>
	public abstract class Raster
		: IRaster
	{
		private string path;
		private Dimensions dimensions;
		private IMetadata metadata;

		// This class is somewhat based on the design pattern outlined in the
		// topic "Implementing a Dispose Method" in the .NET Framework
		// Developer's Guide.
		private bool disposed;

		//---------------------------------------------------------------------

		/// <summary>
		/// The path used to open/create the raster.
		/// </summary>
		public string Path
		{
			get {
				if (disposed)
					throw new System.ObjectDisposedException(null);
				return path;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The dimensions of the raster.
		/// </summary>
		public Dimensions Dimensions
		{
			get {
				if (disposed)
					throw new System.ObjectDisposedException(null);
				return dimensions;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Metadata for the raster.
		/// </summary>
		public IMetadata Metadata
		{
			get {
				if (disposed)
					throw new System.ObjectDisposedException(null);
				return metadata;
			}
		}

		//---------------------------------------------------------------------

		public Raster(string     path,
		              Dimensions dimensions,
		              IMetadata  metadata)
		{
			this.path = path;
			this.dimensions = dimensions;
			this.metadata = metadata;
			this.disposed = false;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Closes the raster, releasing any unmanaged resources.
		/// </summary>
		public void Close()
		{
			Dispose();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Closes the raster, releasing any unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			System.GC.SuppressFinalize(this);
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
		protected virtual void Dispose(bool disposeManaged)
		{
			disposed = true;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Finalizes an instance before its destruction.
		/// </summary>
		/// <remarks>
		/// This destructor will run only if the Close or Dispose method wasn't
		/// called.  Derived classes have no need to provide their own
		/// destructors.
		/// </remarks>
		~Raster()
		{
			Dispose(false);
		}
	}
}

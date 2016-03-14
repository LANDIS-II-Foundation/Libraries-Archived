using Edu.Wisc.Forest.Flel.Grids;

namespace Landis.RasterIO
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
		private uint pixelCount;

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
				    throw new System.ObjectDisposedException(GetType().FullName);
				return dimensions;
			}
			protected set {
				if (disposed)
					throw new System.ObjectDisposedException(GetType().FullName);
				dimensions = value;
				UpdatePixelCount();
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
					throw new System.ObjectDisposedException(GetType().FullName);
				return metadata;
			}
			protected set {
				if (disposed)
					throw new System.ObjectDisposedException(GetType().FullName);
				metadata = value;
			}
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// The number of pixels in the raster.
		/// </summary>
		/// <remarks>
		/// This is 0 if at least one of the dimensions (rows or columns) is
		/// negative.
		/// </remarks>
		public uint PixelCount
		{
			get {
				if (disposed)
					throw new System.ObjectDisposedException(GetType().FullName);
				return pixelCount;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public Raster(string     path,
		              Dimensions dimensions,
		              IMetadata  metadata)
		{
			this.path = path;
			this.dimensions = dimensions;
			this.metadata = metadata;
			this.disposed = false;
			UpdatePixelCount();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance.
		/// </summary>
		public Raster(string path)
		{
			this.path = path;
			this.disposed = false;
		}

		//---------------------------------------------------------------------

		private void UpdatePixelCount()
		{
			if (dimensions.Rows < 0 || dimensions.Columns < 0)
				pixelCount = 0;
			else
				pixelCount = (uint) (dimensions.Rows * dimensions.Columns);
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

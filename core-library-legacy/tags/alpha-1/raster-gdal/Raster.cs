using Gdal = GDAL;

namespace Landis.Raster.GDAL
{
	public class Raster
		: System.IDisposable
	{
		private bool disposed;
		protected Gdal.Dataset dataset;

		//---------------------------------------------------------------------

		internal Raster(Gdal.Dataset dataset)
		{
			this.dataset = dataset;
			this.disposed = false;
		}

		//---------------------------------------------------------------------

		private System.ObjectDisposedException CreateObjectDisposedException()
		{
			return new System.ObjectDisposedException(GetType().FullName);
		}

		//---------------------------------------------------------------------

		public Dimensions Dimensions
		{
			get {
				if (disposed)
					throw CreateObjectDisposedException();
				return new Dimensions(dataset.GetRasterYSize(),	  // rows
				                      dataset.GetRasterXSize());  // columns
			}
		}

		//---------------------------------------------------------------------

		public int BandCount
		{
			get {
				if (disposed)
					throw CreateObjectDisposedException();
				return dataset.GetRasterCount();
			}
		}

		//---------------------------------------------------------------------

		public void Close()
		{
			if (disposed)
				throw CreateObjectDisposedException();
			Dispose();
		}

		//---------------------------------------------------------------------

		public void Dispose()
		{
			Dispose(true);
			System.GC.SuppressFinalize(this);
		}

		//---------------------------------------------------------------------

		protected virtual void Dispose(bool disposing)
		{
			if (! disposed) {
				if (disposing) {
					//	Release managed resources.
					//  Have none.
				}
				//  Release unmanaged resources.
				dataset.Close();
				dataset = null;
				disposed = true;
			}
		}

		//---------------------------------------------------------------------

		~Raster()
		{
			Dispose(false);
		}
	}
}

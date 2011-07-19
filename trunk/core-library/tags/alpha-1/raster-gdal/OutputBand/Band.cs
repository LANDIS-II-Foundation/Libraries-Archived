using Gdal = GDAL;
using Landis.Util;

namespace Landis.Raster.GDAL.OutputBand
{
	public class Band<T, U>
		: RasterBand<T>, Landis.Raster.IOutputBand<T>
		where T: struct
		where U: IByteMethods<T>, new()
	{
		private IOutputRaster raster;
		private ToBytesMethod<T> toBytes;

		//---------------------------------------------------------------------

		public Band(Gdal.RasterBand band,
		            IOutputRaster   raster)
			: base(band)
		{
			this.raster = raster;
			toBytes = new ToBytesMethod<T>(new U().ToBytes);
		}

		//---------------------------------------------------------------------

		public IOutputRaster Raster
		{
			get {
				return raster;
			}
		}

		//---------------------------------------------------------------------

		public T this[int row,
		              int column]
		{
			set {
				byte[] bytes = toBytes(value);
				//  TODO: The logic below is a crude write-one-pixel at a
				//  time approach.  But hey, it works.  Eventually, we need
				//  to use the block-sized buffer of the base class for
				//	performance.  The trick is that we need will likely be
				//  filling more than one block at a time (if raster XSize
				//  > BlockXSize).  How?  Multiple buffers in the base class?
				//  Or multiple buffers at this class level?
				Gdal.CPLErr result = gdalBand.RasterIO(Gdal.RWFlag.Write,
				                                       column - 1,
				                                       row - 1,
				                                       bytes.Length,
				                                       1,
				                                       bytes,
				                                       bytes.Length,
				                                       1,
				                                       PixelType.GDALType,
				                                       0,  // pixelSpace
				                                       0); // lineSpace
					// Will the call above work for a partial block?  e.g.,
					// raster's XSize does not divide evenly by BlockXSize
				if (result != Gdal.CPLErr.None)
					throw new System.ApplicationException();
					//  TODO: define a Landis.Raster.Exception class
					//  TODO: maybe create a function to create a
					//		  Landis.Raster.Exception, and fills it in
					//		  with info from CPLGetLastError[No|Type|Msg]
			}
		}

		//---------------------------------------------------------------------

		public T this[Landis.Landscape.Location location]
		{
			set {
				this[(int) location.Row, (int) location.Column] = value;
			}
		}

		//---------------------------------------------------------------------

		public T this[Landis.Landscape.Site site]
		{
			set {
				this[site.Location] = value;
			}
		}
	}
}

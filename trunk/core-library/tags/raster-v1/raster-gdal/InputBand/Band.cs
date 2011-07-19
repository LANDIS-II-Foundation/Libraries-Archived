using Gdal = GDAL;
using Landis.Util;

namespace Landis.Raster.GDAL.InputBand
{
	public class Band<T, U>
		: RasterBand<T>, Landis.Raster.IInputBand<T>
		where T: struct
		where U: IByteMethods<T>, new()
	{
		private IInputRaster raster;
		private FromBytesMethod<T> fromBytes;

		//---------------------------------------------------------------------

		public Band(Gdal.RasterBand band,
		            IInputRaster    raster)
			: base(band)
		{
			this.raster = raster;
			fromBytes = new FromBytesMethod<T>(new U().FromBytes);
		}

		//---------------------------------------------------------------------

		public IInputRaster Raster
		{
			get {
				return raster;
			}
		}

		//---------------------------------------------------------------------

		public T this[int row,
		              int column]
		{
			get {
				int index;
				BlockLocation desiredBlockLoc = GetBlockLocation(row,
				                                                 column,
				                                                 out index);
				if (desiredBlockLoc != currentBlockLoc) {
					Gdal.CPLErr result = gdalBand.RasterIO(Gdal.RWFlag.Read,
					                                       column - 1,
					                                       row - 1,
					                                       BlockXSize,
					                                       BlockYSize,
					                                       buffer,
					                                       BlockXSize,
					                                       BlockYSize,
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
					currentBlockLoc = desiredBlockLoc;
				}
				return fromBytes(buffer, index);;
			}
		}

		//---------------------------------------------------------------------

		public T this[Landis.Landscape.Location location]
		{
			get {
				return this[(int) location.Row, (int) location.Column];
			}
		}

		//---------------------------------------------------------------------

		public T this[Landis.Landscape.Site site]
		{
			get {
				return this[site.Location];
			}
		}
	}
}

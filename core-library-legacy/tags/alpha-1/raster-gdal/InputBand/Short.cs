using Gdal = GDAL;

namespace Landis.Raster.GDAL.InputBand
{
	public class Short
		: Band<short, Landis.Util.ByteMethods.Short>
	{
		public Short(Gdal.RasterBand gdalBand,
		             IInputRaster    raster)
			: base(gdalBand, raster)
		{
		}           
	}
}

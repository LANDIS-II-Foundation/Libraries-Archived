using Gdal = GDAL;

namespace Landis.Raster.GDAL.OutputBand
{
	public class Short
		: Band<short, Landis.Util.ByteMethods.Short>
	{
		public Short(Gdal.RasterBand gdalBand,
		             IOutputRaster   raster)
			: base(gdalBand, raster)
		{
		}           
	}
}

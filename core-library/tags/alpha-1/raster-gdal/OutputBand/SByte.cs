using Gdal = GDAL;

namespace Landis.Raster.GDAL.OutputBand
{
	public class SByte
		: Band<sbyte, Landis.Util.ByteMethods.SByte>
	{
		public SByte(Gdal.RasterBand gdalBand,
		             IOutputRaster   raster)
			: base(gdalBand, raster)
		{
		}           
	}
}

using Gdal = GDAL;

namespace Landis.Raster.GDAL.InputBand
{
	public class SByte
		: Band<sbyte, Landis.Util.ByteMethods.SByte>
	{
		public SByte(Gdal.RasterBand gdalBand,
		             IInputRaster    raster)
			: base(gdalBand, raster)
		{
		}           
	}
}

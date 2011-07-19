using Gdal = GDAL;

namespace Landis.Raster.GDAL.InputBand
{
	public class Byte
		: Band<byte, Landis.Util.ByteMethods.Byte>
	{
		public Byte(Gdal.RasterBand gdalBand,
		            IInputRaster    raster)
			: base(gdalBand, raster)
		{
		}           
	}
}

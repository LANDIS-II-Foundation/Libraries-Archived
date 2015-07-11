using Gdal = GDAL;

namespace Landis.Raster.GDAL.OutputBand
{
	public class Byte
		: Band<byte, Landis.Util.ByteMethods.Byte>
	{
		public Byte(Gdal.RasterBand gdalBand,
		            IOutputRaster   raster)
			: base(gdalBand, raster)
		{
		}           
	}
}

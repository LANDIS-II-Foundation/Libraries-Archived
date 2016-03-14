using Gdal = GDAL;

namespace Landis.Raster.GDAL.InputBand
{
	public class UShort
		: Band<ushort, Landis.Util.ByteMethods.UShort>
	{
		public UShort(Gdal.RasterBand gdalBand,
		              IInputRaster    raster)
			: base(gdalBand, raster)
		{
		}           
	}
}

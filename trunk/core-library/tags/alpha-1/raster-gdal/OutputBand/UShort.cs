using Gdal = GDAL;

namespace Landis.Raster.GDAL.OutputBand
{
	public class UShort
		: Band<ushort, Landis.Util.ByteMethods.UShort>
	{
		public UShort(Gdal.RasterBand gdalBand,
		              IOutputRaster   raster)
			: base(gdalBand, raster)
		{
		}           
	}
}

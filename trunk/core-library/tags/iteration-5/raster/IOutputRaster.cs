namespace Landis.Raster
{
	public interface IOutputRaster
		: IRaster
	{
		IOutputBand<T> GetBand<T>(int bandIndex);
	}
}

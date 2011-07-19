namespace Landis.Raster
{
	public interface IInputRaster
		: IRaster
	{
		IInputBand<T> GetBand<T>(int bandIndex);
	}
}

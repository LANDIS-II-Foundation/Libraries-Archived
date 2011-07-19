namespace Landis.Raster
{
	public interface IRasterFactory
	{
		IInputRaster Open(string path);

		//---------------------------------------------------------------------

		IOutputRaster Create(string path,
		                     Dimensions dimensions,
		                     System.Type[] bandTypes);
	}
}

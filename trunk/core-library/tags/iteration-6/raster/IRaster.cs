namespace Landis.Raster
{
	public interface IRaster
	{
		Dimensions Dimensions
		{
			get;
		}

		//---------------------------------------------------------------------

		int BandCount
		{
			get;
		}

		//---------------------------------------------------------------------

		void Close();
	}
}

namespace Landis.SiteInitialization
{
	public interface IClasses
	{
		/// <summary>
		/// Gets the class based on its id number.
		/// </summary>
		IClass this[int classId]
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// A map of the classes assigned to the sites on the landscape.
		/// </summary>
		Raster.IInputBand<byte> Map
		{
			get;
		}
	}
}

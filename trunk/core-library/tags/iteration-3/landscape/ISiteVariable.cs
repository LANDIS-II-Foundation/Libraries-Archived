namespace Landis.Landscape
{
	public interface ISiteVariable
	{
		string Name
		{
			get;
		}

		//---------------------------------------------------------------------

		System.Type DataType
		{
			get;
		}

		//---------------------------------------------------------------------

		ILandscape Landscape
		{
			get;
		}
	}
}

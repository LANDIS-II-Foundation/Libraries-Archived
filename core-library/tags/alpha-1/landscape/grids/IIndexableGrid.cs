namespace Landis.Landscape
{
	public interface IIndexableGrid<T>
		: IGrid
	{
		T this [uint row,
		        uint column]
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		T this [Location location]
		{
			get;
			set;
		}
	}
}

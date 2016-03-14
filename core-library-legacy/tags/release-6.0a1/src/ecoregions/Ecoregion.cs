namespace Landis.Ecoregions
{
	/// <summary>
	/// The information for an ecoregion (its index and parameters).
	/// </summary>
	public class Ecoregion
		: Parameters, IEcoregion
	{
		private int index;

		//---------------------------------------------------------------------

		public int Index
		{
			get {
				return index;
			}
		}

		//---------------------------------------------------------------------

		public Ecoregion(int         index,
		                 IParameters parameters)
			: base(parameters)
		{
			this.index = index;
		}
	}
}

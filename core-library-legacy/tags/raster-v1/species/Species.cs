namespace Landis.Species
{
	/// <summary>
	/// The information for a tree species (its index and parameters).
	/// </summary>
	public class Species
		: Parameters, ISpecies
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

		public Species(int         index,
		               IParameters parameters)
			: base(parameters)
		{
			this.index = index;
		}
	}
}

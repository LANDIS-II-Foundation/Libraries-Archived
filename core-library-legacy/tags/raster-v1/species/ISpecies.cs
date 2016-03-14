namespace Landis.Species
{
	/// <summary>
	/// The information for a tree species (its index and parameters).
	/// </summary>
	public interface ISpecies
		: IParameters
	{
		/// <summary>
		/// Index of the species in the dataset of species parameters.
		/// </summary>
		int Index
		{
			get;
		}
	}
}

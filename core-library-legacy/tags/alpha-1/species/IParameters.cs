namespace Landis.Species
{
	/// <summary>
	/// The parameters for a tree species that can be edited.
	/// </summary>
	public interface IParameters
	{
		/// <summary>
		/// Name
		/// </summary>
		string Name
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Longevity (years)
		/// </summary>
		int Longevity
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Age of sexual maturity (years)
		/// </summary>
		int Maturity
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Shade tolerance class (1-5)
		/// </summary>
		byte ShadeTolerance
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Fire tolerance class (1-5)
		/// </summary>
		byte FireTolerance
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Effective seed dispersal distance (m?)
		/// </summary>
		int EffectiveSeedDist
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Maximum seed dispersal distance (m?)
		/// </summary>
		int MaxSeedDist
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Vegetative reproduction probability
		/// </summary>
		float VegReprodProb
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Minimum age for sprouting (years)
		/// </summary>
		int MinSproutAge
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Maximum age for sprouting (years)
		/// </summary>
		int MaxSproutAge
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Indicates whether the species needs fire to disperse (release) its
		/// seeds.
		/// </summary>
		bool Serotiny
		{
			get;
		}
	}
}

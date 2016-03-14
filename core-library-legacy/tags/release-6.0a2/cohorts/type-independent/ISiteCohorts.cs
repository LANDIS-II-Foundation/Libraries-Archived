using Landis.Species;
using System.Collections.Generic;

namespace Landis.Cohorts.TypeIndependent
{
	/// <summary>
	/// All the cohorts at a site.
	/// </summary>
	public interface ISiteCohorts
		: IEnumerable<ISpeciesCohorts>
	{
	    /// <summary>
	    /// Gets a list of the species present at the site.
	    /// </summary>
	    IList<ISpecies> SpeciesPresent
	    {
	        get;
	    }

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets the cohorts for a particular species.
		/// </summary>
		ISpeciesCohorts this[ISpecies species]
		{
			get;
		}
	}
}

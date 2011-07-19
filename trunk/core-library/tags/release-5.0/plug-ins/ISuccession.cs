using Landis.Landscape;
using Landis.Species;
using System.Collections.Generic;

namespace Landis.PlugIns
{
	/// <summary>
	/// Interface between the model's main module and succession components.
	/// </summary>
	public interface ISuccession
		: IPlugInWithTimestep
	{
		/// <summary>
		/// Initializes the site cohorts for the active sites.
		/// </summary>
		/// <param name="initialCommunities">
		/// Path to the file with initial communities' definitions.
		/// </param>
		/// <param name="initialCommunitiesMap">
		/// Path to the raster file showing where the initial communities are.
		/// </param>
		void InitializeSites(string initialCommunities,
		                     string initialCommunitiesMap);

		//---------------------------------------------------------------------

		/// <summary>
		/// Advances the age of all the cohorts at certain specified sites.
		/// </summary>
		/// <param name="sites">
		/// The sites whose cohorts are to be aged.
		/// </param>
		/// <param name="currentTime">
		/// The current model timestep.
		/// </param>
		void AgeCohorts(IEnumerable<MutableActiveSite> sites,
		                int                            currentTime);

		//---------------------------------------------------------------------

		/// <summary>
		/// Computes the shade at certain specified sites.
		/// </summary>
		/// <param name="sites">
		/// The sites where shade is to be computed.
		/// </param>
		void ComputeShade(IEnumerable<MutableActiveSite> sites);

		//---------------------------------------------------------------------

		/// <summary>
		/// Does cohort reproduction at certain specified sites.
		/// </summary>
		/// <param name="sites">
		/// The sites where cohort reproduction should be done.
		/// </param>
		void ReproduceCohorts(IEnumerable<MutableActiveSite> sites);
	}
}

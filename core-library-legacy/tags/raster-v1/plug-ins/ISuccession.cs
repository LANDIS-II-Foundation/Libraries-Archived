using Edu.Wisc.Forest.Flel.Util.PlugIns;
using Landis.Landscape;
using Landis.Species;
using System.Collections.Generic;

namespace Landis.PlugIns
{
	/// <summary>
	/// Interface between the model's main module and succession components.
	/// </summary>
	public interface ISuccession
		: IPlugIn
	{
		/// <summary>
		/// The next timestep where the component should run.
		/// </summary>
		int NextTimeToRun
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes the component with a data file.
		/// </summary>
		/// <param name="dataFile">
		/// Path to the file with initialization data.
		/// </param>
		void Initialize(string dataFile);

		//---------------------------------------------------------------------

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
		void AgeCohorts(IEnumerable<ActiveSite> sites,
		                int                     currentTime);

		//---------------------------------------------------------------------

		/// <summary>
		/// Computes the shade at certain specified sites.
		/// </summary>
		/// <param name="sites">
		/// The sites where shade is to be computed.
		/// </param>
		void ComputeShade(IEnumerable<ActiveSite> sites);

		//---------------------------------------------------------------------

		/// <summary>
		/// Reproduces plants (trees) at certain specified sites.
		/// </summary>
		/// <param name="sites">
		/// The sites where plant reproduction should be done.
		/// </param>
		void ReproducePlants(IEnumerable<ActiveSite> sites);
	}
}

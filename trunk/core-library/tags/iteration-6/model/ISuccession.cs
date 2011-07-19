using Landis.Landscape;
using Landis.Species;
using System.Collections.Generic;

namespace Landis
{
	/// <summary>
	/// Interface between the model's main module and succession components.
	/// </summary>
	public interface ISuccession
	{
		/// <summary>
		/// The component's timestep (years).
		/// </summary>
		int Timestep
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The method for dispersing seeds to a site.
		/// </summary>
		DisperseSeedMethod DisperseSeed
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes an active site.
		/// </summary>
		/// <param name="site">
		/// The site to initialize.
		/// </param>
		/// <param name="initialSiteClass">
		/// The initial site class.
		/// </param>
		void InitializeSite(ActiveSite                site,
		                    SiteInitialization.IClass initialSiteClass);

		//---------------------------------------------------------------------

		/// <summary>
		/// Advances the age of all the cohorts at a site.
		/// </summary>
		/// <param name="site">
		/// The site whose cohorts are to be aged.
		/// </param>
		/// <param name="deltaTime">
		/// The amount of time to advance the cohorts' ages by (years).
		/// </param>
		void AgeCohorts(ActiveSite site,
		                int        deltaTime);

		//---------------------------------------------------------------------

		/// <summary>
		/// Computes the shade at a site.
		/// </summary>
		/// <param name="site">
		/// The site where the shade is to be computed.
		/// </param>
		/// <returns>
		/// A value between 0 and 5.
		/// </returns>
		byte ComputeShade(ActiveSite site);
	}
}

using Edu.Wisc.Forest.Flel.Grids;

using System.Collections.Generic;

namespace Landis.Landscape
{
	public interface ILandscape
		: IGrid, IEnumerable<MutableActiveSite>
	{
		/// <summary>
		/// The number of active sites on a landscape.
		/// </summary>
		int ActiveSiteCount
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The number of inactive sites on a landscape.
		/// </summary>
		int InactiveSiteCount
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The first inactive site in row-major order on the landscape.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">
		/// The number of inactive sites is 0.
		/// </exception>
		Location FirstInactiveSite
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The data index shared by all the inactive sites on the landscape.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">
		/// The number of inactive sites is 0.
		/// </exception>
		uint InactiveSiteDataIndex
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The number of sites (active and inactive) on a landscape.
		/// </summary>
		int SiteCount
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets an active site on the landscape.
		/// </summary>
		/// <param name="location">
		/// the site's location
		/// </param>
		/// <returns>
		/// null if the location is not on the landscape.
		/// </returns>
		ActiveSite this[Location location]
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets an active site on the landscape.
		/// </summary>
		/// <param name="row">
		/// the row where the site is located
		/// </param>
		/// <param name="column">
		/// the column where the site is located
		/// </param>
		/// <returns>
		/// null if the location is not on the landscape.
		/// </returns>
		ActiveSite this[uint row,
                        uint column]
		{
			get;
		}

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets an enumerable collection of all the active sites.
        /// </summary>
        IEnumerable<MutableActiveSite> ActiveSites
        {
        	get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets an enumerable collection of all the sites.
        /// </summary>
        IEnumerable<Site> AllSites
        {
        	get;
        }

		//---------------------------------------------------------------------

		/// <summary>
		/// Is a location valid for a landscape?
		/// </summary>
		bool IsValid(Location location);

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets a site on the landscape.
		/// </summary>
		/// <param name="location">
		/// the site's location
		/// </param>
		/// <returns>
		/// null if the location is not on the landscape.
		/// </returns>
		Site GetSite(Location location);

        //---------------------------------------------------------------------

		/// <summary>
		/// Gets an active site on the landscape.
		/// </summary>
		/// <param name="row">
		/// the row where the site is located
		/// </param>
		/// <param name="column">
		/// the column where the site is located
		/// </param>
		/// <returns>
		/// null if the location is not on the landscape.
		/// </returns>
		Site GetSite(uint row,
                     uint column);

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets a site on the landscape.
		/// </summary>
		/// <param name="location">
		/// The site's location
		/// </param>
		/// <param name="site">
		/// The object which will be assigned the requested site.  If this
		/// parameter is null and the location is valid, then a new instance
		/// will be created and assigned to the parameter.
		/// </param>
		/// <returns>
		/// true if the location is on the landscape, and the information
		/// about the requested site was assigned to the site parameter.
		/// false if the location is not valid (in which case, the site
		/// parameter is unchanged).
		/// </returns>
		bool GetSite(Location        location,
		             ref MutableSite site);

		//---------------------------------------------------------------------

		/// <summary>
		/// Creates a new site variable for a landscape.
		/// </summary>
		/// <param name="mode">
		/// Indicates whether inactives sites share a common value or have
		/// distinct values.
		/// </param>
		ISiteVar<T> NewSiteVar<T>(InactiveSiteMode mode);

		//---------------------------------------------------------------------

		/// <summary>
		/// Creates a new site variable for a landscape.  The inactive sites
		/// share a common value.
		/// </summary>
		ISiteVar<T> NewSiteVar<T>();
	}
}

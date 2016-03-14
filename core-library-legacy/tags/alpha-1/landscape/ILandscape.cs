using System.Collections.Generic;

namespace Landis.Landscape
{
	public interface ILandscape
		: IGrid, IEnumerable<ActiveSite>
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
        IEnumerable<ActiveSite> ActiveSites
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

using System.Collections.Generic;

namespace Landis.Landscape
{
	public interface ILandscape
		: IGrid, IEnumerable<ActiveSite>
	{
            //!<  The number of active sites on landscape.
		uint ActiveSiteCount
		{
			get;
		}

		//---------------------------------------------------------------------

            //!<  \brief  Is a location on the landscape?
		bool IsValid(Location location);

		//---------------------------------------------------------------------

            //!<  \brief  Get an active site on the landscape.
            //!
            //! \param location -- the site's location
            //!
            //! \returns  A null pointer (i.e., 0) if the location is not on
            //!           the landscape.
		ActiveSite this[Location location]
		{
			get;
		}

		//---------------------------------------------------------------------

            //!<  \brief  Get an active site on the landscape.
            //!
            //! \param row     The row containing the site.
            //! \param column  The column containing the site.
            //!
            //! \returns  A null pointer (i.e., 0) if the location is not on
            //!           the landscape.

		ActiveSite this[uint row,
                        uint column]
		{
			get;
		}

		//---------------------------------------------------------------------

            //!<  \brief  Get a site on the landscape.
            //!
            //! \param location  The site's location.
            //!
            //! \returns  A null pointer (i.e., 0) if the location is not on
            //!           the landscape.
		Site GetSite(Location location);

        //---------------------------------------------------------------------

            //!<  \brief  Get a site on the landscape.
            //!
            //! \param row     The row containing the site.
            //! \param column  The column containing the site.
            //!
            //! \returns  A null pointer (i.e., 0) if the location is not on
            //!           the landscape.
		Site GetSite(uint row,
                     uint column);

		//---------------------------------------------------------------------

		void Add(ISiteVariable variable);

        //---------------------------------------------------------------------

        IEnumerable<Site> AllSites
        {
        	get;
        }

        //---------------------------------------------------------------------

/*
        SiteIterator begin() const;
            //!<  \brief  Get a site iterator that points to the first site
            //!           on the landscape in row-major order.

        SiteIterator end() const;
            //!<  \brief  Get a site iterator that points just after the last
            //!           site on the landscape in row-major order.

        ActiveSiteIterator begin_active() const;
            //!<  \brief  Get an active-site iterator that points to the first
            //!           active site on the landscape in row-major order.

        ActiveSiteIterator end_active() const;
            //!<  \brief  Get an active-site iterator that points just after
            //!           the last active site on the landscape in row-major
            //!           order.

        void add_site_var(const std::string & name,
                          SiteVarData *       data);
            //!<  \brief  Add a site variable.
            //!
            //! \param name  The name of the site variable.
            //! \param data  The variable's data.
            //!
            //! Note:  The landscape takes ownership of the data, and is
            //! responsible for its deallocation.
            //!
            //! \pre  get_site_var(\a name) == 0

        SiteVarData * get_site_var(const std::string & name) const;
            //!<  \brief  Get the site data for a site variable.
            //!
            //! \param name  The name of the site variable
            //!
            //! \returns 0 if the landscape does not have a site variable with
            //!          the specified name.

        ~Landscape();
            //<!  \brief  Destroy a landscape, deallocating the memory for
            //!           its site variables.
*/
	}
}

// Copyright 2007 University of Wisconsin
// Author: James Domingo, UW-Madison, Forest Landscape Ecology Lab

using System.Collections.Generic;
using Wisc.Flel.GeospatialModeling.Grids;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

using Dimensions = Wisc.Flel.GeospatialModeling.Landscapes.DualScale.Dimensions;
using Location = Wisc.Flel.GeospatialModeling.Landscapes.DualScale.Location;

namespace Landis.Test.Util
{
    /// <summary>
    /// A mock landscape for test purposes.
    /// </summary>
    public class MockLandscape
        : Grid, ILandscape
    {
        public int ActiveSiteCount;
        public int InactiveSiteCount;
        public Location FirstInactiveSite;
        public uint InactiveSiteDataIndex;
        public int BlockSize;
        public int SitesPerBlock;
        public IEnumerable<ActiveSite> ActiveSites;
        public IEnumerable<Site> AllSites;

        //---------------------------------------------------------------------

        public MockLandscape(int rows,
                             int columns)
            : base(rows, columns)
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The dimensions of a landscape.
        /// </summary>
        Dimensions ILandscape.Dimensions
        {
            get {
                return Dimensions;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The number of active sites on a landscape.
        /// </summary>
        int ILandscape.ActiveSiteCount
        {
            get {
                return ActiveSiteCount;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The number of inactive sites on a landscape.
        /// </summary>
        int ILandscape.InactiveSiteCount
        {
            get {
                return InactiveSiteCount;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The first inactive site in row-major order on the landscape.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// The number of inactive sites is 0.
        /// </exception>
        Location ILandscape.FirstInactiveSite
        {
            get {
                return FirstInactiveSite;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The data index shared by all the inactive sites on the landscape.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">
        /// The number of inactive sites is 0.
        /// </exception>
        uint ILandscape.InactiveSiteDataIndex
        {
            get {
                return InactiveSiteDataIndex;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The number of sites (active and inactive) on a landscape.
        /// </summary>
        int ILandscape.SiteCount
        {
            get {
                return (int) Count;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The size of a block of sites on the landscape.  The length of a
        /// block's side in number of sites.
        /// </summary>
        int ILandscape.BlockSize
        {
            get {
                return BlockSize;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The number of sites in a block on the landscape.
        /// </summary>
        int ILandscape.SitesPerBlock
        {
            get {
                return SitesPerBlock;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets an active site on the landscape.
        /// </summary>
        /// <param name="location">
        /// the site's location
        /// </param>
        /// <returns>
        /// a false site if the location is not on the landscape.
        /// </returns>
        public virtual ActiveSite this[Location location]
        {
            get {
                return new ActiveSite();
            }
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
        /// a false if the location is not on the landscape.
        /// </returns>
        public virtual ActiveSite this[int row,
                                       int column]
        {
            get {
                return new ActiveSite();
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets an enumerable collection of all the active sites.
        /// </summary>
        IEnumerable<ActiveSite> ILandscape.ActiveSites
        {
            get {
                return ActiveSites;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets an enumerable collection of all the sites.
        /// </summary>
        IEnumerable<Site> ILandscape.AllSites
        {
            get {
                return AllSites;
            }
        }

        //---------------------------------------------------------------------

        public virtual IEnumerator<ActiveSite> GetEnumerator()
        {
            return null;
        }

        //---------------------------------------------------------------------

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Is a location valid for a landscape?
        /// </summary>
        public virtual bool IsValid(Location location)
        {
            return false;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets a site on the landscape.
        /// </summary>
        /// <param name="location">
        /// the site's location
        /// </param>
        /// <returns>
        /// a false site if the location is not on the landscape.
        /// </returns>
        public virtual Site GetSite(Location location)
        {
            return new Site();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets a site on the landscape.
        /// </summary>
        /// <param name="row">
        /// the row where the site is located
        /// </param>
        /// <param name="column">
        /// the column where the site is located
        /// </param>
        /// <returns>
        /// a false if the location is not on the landscape.
        /// </returns>
        public virtual Site GetSite(int row,
                                    int column)
        {
            return new Site();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a new site variable for a landscape.
        /// </summary>
        /// <param name="mode">
        /// Indicates whether inactives sites share a common value or have
        /// distinct values.
        /// </param>
        public virtual ISiteVar<T> NewSiteVar<T>(InactiveSiteMode mode)
        {
            return null;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a new site variable for a landscape.  The inactive sites
        /// share a common value.
        /// </summary>
        public virtual ISiteVar<T> NewSiteVar<T>()
        {
            return null;
        }
    }
}

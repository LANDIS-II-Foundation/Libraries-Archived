using Edu.Wisc.Forest.Flel.Grids;

using System.Collections;
using System.Collections.Generic;

namespace Landis.Landscape
{
	public class Landscape
		: Grid, ILandscape, IEnumerable<Site>
	{
		private ActiveSiteMap activeSiteMap;
		private int inactiveSiteCount;

		//---------------------------------------------------------------------

		public int ActiveSiteCount
		{
			get {
				return (int) activeSiteMap.Count;
			}
		}

		//---------------------------------------------------------------------

		public int InactiveSiteCount
		{
			get {
				return inactiveSiteCount;
			}
		}

		//---------------------------------------------------------------------

		public Location FirstInactiveSite
		{
			get {
				if (activeSiteMap.FirstInactive != null)
					return activeSiteMap.FirstInactive.Location;
				throw new System.InvalidOperationException("No inactive sites");
			}
		}

		//---------------------------------------------------------------------

		public uint InactiveSiteDataIndex
		{
			get {
				if (inactiveSiteCount == 0)
					throw new System.InvalidOperationException("No inactive sites");
				return ActiveSiteMap.InactiveSiteDataIndex;
			}
		}

		//---------------------------------------------------------------------

		public int SiteCount
		{
			get {
				return (int) Count;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance using an input grid of active sites.
		/// </summary>
		/// <param name="activeSites">
		/// A grid that indicates which sites are active.
		/// </param>
		public Landscape(IInputGrid<bool> activeSites)
			: base(activeSites.Dimensions)
		{
			Initialize(activeSites);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance using an indexable grid of active sites.
		/// </summary>
		/// <param name="activeSites">
		/// A grid that indicates which sites are active.
		/// </param>
		public Landscape(IIndexableGrid<bool> activeSites)
			: base(activeSites.Dimensions)
		{
			Initialize(new InputGrid<bool>(activeSites));
		}

		//---------------------------------------------------------------------

		private void Initialize(IInputGrid<bool> activeSites)
		{
			if (Count > int.MaxValue) {
				string mesg = string.Format("Landscape dimensions are too big; maximum # of sites = {0:#,###}",
				                            int.MaxValue);
				throw new System.ApplicationException(mesg);
			}
			activeSiteMap = new ActiveSiteMap(activeSites);
			activeSites.Close();
			inactiveSiteCount = SiteCount - (int) activeSiteMap.Count;
		}

        //---------------------------------------------------------------------

        public ActiveSite this[Location location]
        {
        	get {
        		if (! IsValid(location))
        			return null;
	       		uint index = activeSiteMap[location];
	       		if (index == ActiveSiteMap.InactiveSiteDataIndex)
					return null;
	       		else
	       			return new ImmutableActiveSite(this, location, index);
        	}
        }

        //---------------------------------------------------------------------

        public ActiveSite this[uint row,
                               uint column]
        {
        	get {
        		return this[new Location(row, column)];
        	}
        }

        //---------------------------------------------------------------------

        public IEnumerable<MutableActiveSite> ActiveSites
        {
        	get {
        		return this;
        	}
        }

        //---------------------------------------------------------------------

		public IEnumerator<MutableActiveSite> GetEnumerator()
		{
			return GetActiveSiteEnumerator();
		}

        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
        	return GetActiveSiteEnumerator();
        }

        //---------------------------------------------------------------------

        public ActiveSiteEnumerator GetActiveSiteEnumerator()
        {
			return new ActiveSiteEnumerator(this, activeSiteMap);
        }

        //---------------------------------------------------------------------

        public IEnumerable<Site> AllSites
        {
        	get {
        		return this;
        	}
        }

        //---------------------------------------------------------------------

        IEnumerator<Site> IEnumerable<Site>.GetEnumerator()
        {
        	return GetSiteEnumerator();
        }

        //---------------------------------------------------------------------

        public SiteEnumerator GetSiteEnumerator()
        {
			return new SiteEnumerator(this);
        }

        //---------------------------------------------------------------------

		public bool IsValid(Location location)
		{
			return (1 <= location.Row)&& (location.Row <= Rows) &&
				   (1 <= location.Column) && (location.Column <= Columns);
		}

        //---------------------------------------------------------------------

		public Site GetSite(Location location)
		{
			if (! IsValid(location))
				return null;
       		uint index = activeSiteMap[location];
       		if (index == ActiveSiteMap.InactiveSiteDataIndex)
				return new ImmutableInactiveSite(this, location, index);
       		else
       			return new ImmutableActiveSite(this, location, index);
		}

        //---------------------------------------------------------------------

		public Site GetSite(uint row,
	                     	uint column)
		{
           	return GetSite(new Location(row, column));
		}

        //---------------------------------------------------------------------

		public bool GetSite(Location        location,
		                    ref MutableSite site)
        {
        	if (IsValid(location)) {
        		uint index = activeSiteMap[location];
        		bool isActive = (index != ActiveSiteMap.InactiveSiteDataIndex);
        		if (site == null)
        			site = new MutableSite(this, location, isActive, index);
        		else
        			site.SetAll(this, location, isActive, index);
	        	return true;
        	}
       		return false;
        }

        //---------------------------------------------------------------------

        public ISiteVar<T> NewSiteVar<T>(InactiveSiteMode mode)
        {
        	if (mode == InactiveSiteMode.Share1Value)
				return new SiteVarShare<T>(this);
        	else
        		return new SiteVarDistinct<T>(this);
        }

        //---------------------------------------------------------------------

        public ISiteVar<T> NewSiteVar<T>()
        {
        	return NewSiteVar<T>(InactiveSiteMode.Share1Value);
        }
	}
}

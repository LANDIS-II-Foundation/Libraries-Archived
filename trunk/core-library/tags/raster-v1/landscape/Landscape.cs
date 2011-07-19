using System.Collections.Generic;

namespace Landis.Landscape
{
	public class Landscape
		: Grid, ILandscape
	{
		private ActiveSiteMap activeSiteMap;
		private int inactiveSiteCount;
		private int inactiveSiteDataIndex;

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
			if (inactiveSiteCount > 0)
				inactiveSiteDataIndex = (int) activeSiteMap.Count;
		}

        //---------------------------------------------------------------------

        public ActiveSite this[Location location]
        {
        	get {
        		uint index = activeSiteMap[location];
        		if (index == activeSiteMap.Count)
        			return null;
        		else
        			return new ActiveSite(this, location, index);
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

        public IEnumerable<ActiveSite> ActiveSites
        {
        	get {
        		return this;
        	}
        }

        //---------------------------------------------------------------------

        public IEnumerator<ActiveSite> GetEnumerator()
        {
        	foreach (ActiveSiteMap.Entry entry in activeSiteMap) {
        		yield return new ActiveSite(this, entry.Location,
        		                            entry.Index);
        	}
        }

        //---------------------------------------------------------------------

        public IEnumerable<Site> AllSites
        {
        	get {
        		for (uint row = 1; row <= Rows; ++row)
        			for (uint column = 1; column <= Columns; ++column) {
        				yield return GetSite(row, column);
        		}
        	}
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
			ActiveSite activeSite = this[location];
			if (activeSite != null)
				return activeSite;
			else
				return new InactiveSite(this, location, (uint) inactiveSiteDataIndex);
		}

        //---------------------------------------------------------------------

		public Site GetSite(uint row,
	                     	uint column)
		{
           	return GetSite(new Location(row, column));
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

        //---------------------------------------------------------------------
#if FOO
        public void Add(ISiteVariable variable)
        {
        	ISiteVarWithData existingVar;
        	if (siteVars.TryGetValue(variable.Name, out existingVar)) {
        		if (variable.DataType == existingVar.DataType) {
        			ISiteVarWithData var = (ISiteVarWithData) variable;
        			var.ShareData(existingVar);
        		}
        		else
        			throw new System.ApplicationException("type mismatch");
        	}
       		else {
       			ISiteVarWithData var = (ISiteVarWithData) variable;
				var.AllocateData(siteCountForVars);
				siteVars[var.Name] = var;
			}
		}
#endif
	}
}

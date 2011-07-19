using System.Collections.Generic;

namespace Landis.Landscape
{
	public class Landscape
		: Grid, ILandscape
	{
		private ActiveSiteMap activeSiteMap;
		private uint siteCountForVars;
		private uint inactiveSiteDataIndex;
		private Dictionary<string, ISiteVarWithData> siteVars;

		//---------------------------------------------------------------------

		public Landscape(IIndexableGrid<bool> activeSites)
			: base(activeSites.Dimensions)
		{
			activeSiteMap = new ActiveSiteMap(activeSites);
			if (activeSites.Count == activeSiteMap.Count)
				//	All active sites, no inactive sites
				siteCountForVars = activeSiteMap.Count;
			else {
				siteCountForVars = activeSiteMap.Count + 1;
				inactiveSiteDataIndex = siteCountForVars - 1;
			}
			siteVars = new Dictionary<string, ISiteVarWithData>();
		}

        //---------------------------------------------------------------------

		public uint ActiveSiteCount
		{
        	get {
        		return activeSiteMap.Count;
        	}
		}

        //---------------------------------------------------------------------

            //!<  \brief  Is a location on the landscape?
		public bool IsValid(Location location)
		{
			return (1 <= location.Row)&& (location.Row <= Rows) &&
				   (1 <= location.Column) && (location.Column <= Columns);
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

		public Site GetSite(Location location)
		{
			ActiveSite activeSite = this[location];
			if (activeSite != null)
				return activeSite;
			else
				return new InactiveSite(this, location, inactiveSiteDataIndex);
		}

        //---------------------------------------------------------------------

            //!<  \brief  Get a site on the landscape.
            //!
            //! \param row     The row containing the site.
            //! \param column  The column containing the site.
            //!
            //! \returns  A null pointer (i.e., 0) if the location is not on
            //!           the landscape.
		public Site GetSite(uint row,
	                     	uint column)
		{
           	return GetSite(new Location(row, column));
		}

        //---------------------------------------------------------------------

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
	}
}

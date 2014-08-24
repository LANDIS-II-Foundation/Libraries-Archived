// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

using Landis.SpatialModeling;
using System.Collections.Generic;

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// A site-selection method that harvests all the sites in a stand.
    /// </summary>
    public class CompleteStand
        : ISiteSelector
    {
        private double areaSelected;

        //---------------------------------------------------------------------

        public CompleteStand()
        {
        }

        //---------------------------------------------------------------------

        double ISiteSelector.AreaSelected
        {
            get {
                return areaSelected;
            }
        }
		
        //---------------------------------------------------------------------
        //mark the whole area selected as harvested
        IEnumerable<ActiveSite> ISiteSelector.SelectSites(Stand stand)
        {
            areaSelected = stand.ActiveArea;
            stand.MarkAsHarvested();
			//mark this stand's event id
			stand.EventId = PlugIn.EventId;
			
			//increment global event id number
			PlugIn.EventId++;
			
            return stand;
        }
		
    }
}

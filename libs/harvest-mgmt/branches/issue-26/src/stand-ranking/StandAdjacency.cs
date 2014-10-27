// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using System.Collections.Generic;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A ranking requirement which requires a stand 's neighbors to be no less than
    /// the minimum age to be eligible for ranking.
    /// </summary>
    public class StandAdjacency
        : IRequirement
    {
	
		private ushort time;
		private string type;
		private ushort set_aside;

        //---------------------------------------------------------------------

        public StandAdjacency(ushort time, string type, ushort set_aside)
        {
			this.time = time;
			this.type = type;
			this.set_aside = set_aside;
        }
		
        //---------------------------------------------------------------------
		
		public ushort SetAside {
			get {
				return set_aside;
			}
		}

        //---------------------------------------------------------------------

        bool IRequirement.MetBy(Stand stand)
        {
			//Model.Core.UI.WriteLine("checking stand {0}", stand.MapCode);
			//get list of neighboring stands (must cast from enum)
			List<Stand> neighbor_stands = new List<Stand>();
			neighbor_stands = (List<Stand>) stand.Neighbors;
			//add ma_neighbors to this list as well, to check with all the neighboring stands that are in a different management area
			foreach (Stand n_stand in stand.MaNeighbors) {
				neighbor_stands.Add(n_stand);
			}
			//loop through neighbor stands (including ma_neighbors), if one is too young then return false
			foreach (Stand n_stand in neighbor_stands) {
				//if n_stand is too young, return false (breaking out of loop as soon as one neighbor fails)
				if (type == "StandAge") {
					if (n_stand.Age < time) {
						return false;
					}
				}
				//if type was mintimesincelastharvest
				else {
					if (n_stand.TimeSinceLastHarvested < time) {
						//Model.Core.UI.WriteLine("stand {0} NOT ranked.", stand.MapCode);
						return false;
					}
				}
			}
			return true;
        }
    }
}
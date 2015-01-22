// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using System.Collections.Generic;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A base class that represents a method for computing stand rankings.
    /// </summary>
    public abstract class StandRankingMethod
        : IStandRankingMethod
    {
        private List<IRequirement> requirements;

        //---------------------------------------------------------------------

        protected StandRankingMethod()
        {
			requirements = new List<IRequirement>();
        }
		
		//---------------------------------------------------------------------
		
		/// <summary>
		/// Show list of requirements
		/// </summary>
		
		public List<IRequirement> Requirements {
			get {
				return requirements;
			}
		}

        //---------------------------------------------------------------------

        /// <summary>
        /// Computes the rank for a stand.
        /// </summary>
        protected abstract double ComputeRank(Stand stand, int i);

        //---------------------------------------------------------------------

        protected virtual void InitializeForRanking(List<Stand> stands, int standCount)
        {
        }

        //---------------------------------------------------------------------

        void IStandRankingMethod.AddRequirement(IRequirement requirement)
        {
            requirements.Add(requirement);
        }

        //---------------------------------------------------------------------

        void IStandRankingMethod.RankStands(List<Stand> stands, StandRanking[] rankings) 
        {
            InitializeForRanking(stands, stands.Count);
            for (int i = 0; i < stands.Count; i++) {
                Stand stand = stands[i];
                double rank = 0;
                if (! stand.IsSetAside) {
                    //check if stand meets all the ranking requirements
                    bool meetsAllRequirements = true;
                    foreach (IRequirement requirement in requirements) {
                        if (! requirement.MetBy(stand)) {
                            meetsAllRequirements = false;
							//set stand rank to 0
							rankings[i].Rank = 0;
                            break;
                        }
                    }
					
                    //if the stand meets all the requirements and is not set-aside,, get its rank
                    if (meetsAllRequirements) {
                        rank = ComputeRank(stand, i);
                    }
                    //otherwise, rank it 0 (so it will not be harvested.)
                    else {
                        rank = 0;
                        //Model.Core.UI.WriteLine("   Stand {0} did not meet its requirements.", stand.MapCode);
                    }
                }
				else {
					rankings[i].Rank = 0;
				}
                rankings[i].Stand = stand;
                rankings[i].Rank = rank;
                //assign rank to stand
				//Model.Core.UI.WriteLine("   Stand {0} rank = {1}.", rankings[i].Stand.MapCode, rankings[i].Rank);
            }
        }
    }
}
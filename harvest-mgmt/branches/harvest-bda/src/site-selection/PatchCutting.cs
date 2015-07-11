// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Edu.Wisc.Forest.Flel.Util;
using Landis.SpatialModeling;
using System.Collections;
using System.Collections.Generic;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A site-selection method that selects small non-contiguous collections
    /// of sites within a stand.
    /// </summary>
    public class PatchCutting
        : StandSpreading, ISiteSelector, IEnumerable<ActiveSite>
    {

        private Stand stand;         // stand to work in
        private double percent;      // percent of stand to harvest
        private double patch_size;   // harvest patch sizes
        private double areaSelected; // total area selected
        private string priority;     // patch cutting priority (optional)
        
        //collect all 8 relative neighbor locations in array
        private RelativeLocation[] all_neighbor_locations = new RelativeLocation[]
        {
                new RelativeLocation(-1,0),
                new RelativeLocation(1,0),
                new RelativeLocation(0,-1),
                new RelativeLocation(0,1),
                new RelativeLocation(-1,-1),
                new RelativeLocation(-1,1),
                new RelativeLocation(1,-1),
                new RelativeLocation(1,1)
        };
        
        public static void ValidatePercentage(InputValue<Percentage> percentage)
        {
            if (percentage.Actual < 0 || percentage.Actual > 1.0)
                throw new InputValueException(percentage.String,
                                              percentage.String + " is not between 0% and 100%");
        }

        //---------------------------------------------------------------------

        public static void ValidateSize(InputValue<double> size)
        {
            if (size.Actual < 0.0)
                throw new InputValueException(size.String,
                    "Patch size cannot be negative");
        }

        private static class PatchCutPriority {
            //Names for each acceptable patch cut priority
            public const string PatchSize = "PatchSize";    //preserve 1 cell buffer between patches; Disregard PercentCut minimum
            public const string PercentCut = "PercentCut";  //no buffer between patches
            public const string NoPriority = "NoPriority";  //Patch size and percent cut are equal
        }
        
        //constructor
        public PatchCutting(Percentage percentage, double size, string priority) {
            this.percent = (double) percentage;
            this.patch_size = size;
            if (string.IsNullOrEmpty(priority)) {
                this.priority = PatchCutPriority.NoPriority;
            }
            else {
                this.priority = priority;
            }
        }
        
        //---------------------------------------------------------------------

        double ISiteSelector.AreaSelected
        {
            get {
                return areaSelected;
            }
        }

        //---------------------------------------------------------------------

        IEnumerable<ActiveSite> ISiteSelector.SelectSites(Stand stand)
        {
            this.stand = stand;
            return this;
        }        

        //---------------------------------------------------------------------

        IEnumerator<ActiveSite> IEnumerable<ActiveSite>.GetEnumerator() {
            
            // initialize and declare variables
            areaSelected = 0;
            double patchAreaSelected = 0;
            List<ActiveSite> sites = stand.GetActiveSites();                   // this stand's sites
            Queue<ActiveSite> sitesToHarvest = new Queue<ActiveSite>();  // for harvesting
            Queue<ActiveSite> sitesToConsider = new Queue<ActiveSite>(); // sites to harvest
            Queue<ActiveSite> sitesConsidered = new Queue<ActiveSite>(); // all considered
            ActiveSite crntSite;
            int random;
            double standTargetArea;

            //get a random site from the stand
            //random = (int) (Model.Core.GenerateUniform() * 
            //    (stand.SiteCount - 1));
            
            //get a random active site from the stand
            random = (int) (Model.Core.GenerateUniform() * (sites.Count - 1));
            crntSite = sites[random];

            //while (!(((Site)crntSite).IsActive) && sites.Count > 0) {
            //while (sites.Count > 0) {
            //    //Model.Core.UI.WriteLine("   ERROR:  Non-active site included in stand {0}.", stand.MapCode);
            //    sitesConsidered.Enqueue(crntSite);
            //    sites.Remove(crntSite);
            //    random = (int)(Model.Core.GenerateUniform() * (sites.Count - 1));
            //    crntSite = sites[random];
            //} // while (!(((Site)crntSite).IsActive) && sites.Count > 0)

            //put initial pivot site on queue
            sitesToConsider.Enqueue(crntSite);
            sites.Remove(crntSite);
            
            standTargetArea = Model.Core.CellArea * stand.SiteCount * percent;
            
            // loop through stand, harvesting patches of size patch_size at a time
            while (areaSelected < standTargetArea && sites.Count > 0) 
            {
                while (patchAreaSelected < patch_size && 
                    areaSelected < standTargetArea && 
                    sites.Count > 0) 
                {
                    //loop through the site's neighbors enqueueing them too
                    foreach (RelativeLocation loc in all_neighbor_locations) {
                        
                        // get a neighbor site
                        Site tempSite = crntSite.GetNeighbor(loc);
                        if(tempSite != null && tempSite.IsActive)
                        {
                        
                            ActiveSite nbrSite = (ActiveSite) tempSite;

                            if (sites.Contains(nbrSite))
                            {
                                //get a neighbor site (if it's non-null and active)
                                if (!sitesToConsider.Contains(nbrSite) && 
                                !sitesConsidered.Contains(nbrSite)) 
                                {
                                    //then enqueue the neighbor
                                    sitesToConsider.Enqueue(nbrSite);
                                }
                                //Always remove the site if it's in the sites list.
                                sites.Remove(nbrSite);
                            }
                        }
                    } 

                    //check if there's anything left on the queue
                    if (sitesToConsider.Count > 1) {
                        ActiveSite crntConsideredSite = sitesToConsider.Dequeue();
                        if (SiteVars.TimeSinceLastDamage(crntConsideredSite) >= 
                            stand.MinTimeSinceDamage) {

                            // now after looping through all of the current 
                            // site's neighbors dequeue the current site and 
                            // put it on the sitesToHarvest queue (used later)
                            sitesToHarvest.Enqueue(crntConsideredSite);
                        
                            // increment area selected and total_areaSelected
                            patchAreaSelected += Model.Core.CellArea;
                            areaSelected += Model.Core.CellArea;
                        }

                        // Whether harvestable or not, it has been considered
                        sitesConsidered.Enqueue(crntConsideredSite);
                        //and set the new crntSite to the head of the queue (by peeking)
                        crntSite = sitesToConsider.Peek();
                    
                    } else if (sitesToConsider.Count == 1) { //get another site from the queue
                    
                        crntSite = sitesToConsider.Peek();

                        ActiveSite crntConsideredSite = sitesToConsider.Dequeue();
                        if (SiteVars.TimeSinceLastDamage(crntConsideredSite) >=
                            stand.MinTimeSinceDamage) {
                            
                            sitesToHarvest.Enqueue(crntConsideredSite);

                            //increment area selected and total_areaSelected
                            patchAreaSelected += Model.Core.CellArea;
                            areaSelected += Model.Core.CellArea;
                        }

                        // Whether harvestable or not, it has been considered
                        sitesConsidered.Enqueue(crntConsideredSite);

                    } else {
                    // else just break out- if it's not big enough it's not big enough
                      //Model.Core.UI.WriteLine("patch isn't big enough ({0}), must BREAK.", areaSelected);
                        break;
                    } // if (sitesToConsider.Count > 1)

                } // while (patchAreaSelected < patch_size && 

                //Model.Core.UI.WriteLine("Done with a patch.");

                //If priority is PercentCut, put the unused sitesToConsider back 
                //into the sites list before clearing sitesToConsider
                if (priority == PatchCutPriority.PercentCut && sitesToConsider.Count > 0)
                {
                    ActiveSite[] remainingSitesToConsider = sitesToConsider.ToArray();
                    sites.AddRange(remainingSitesToConsider);
                }
                //clear the sitesToConsider queue to get rid of old sites
                sitesToConsider.Clear();
                // get a new random site to start at (one that hasn't been 
                // put on the sitesConsidered queue yet)
                // only allow a site-count # of tries
                bool found_site = false;
                int tries = 0;
                while (!found_site && tries < sites.Count) 
                {

                    //increment the number of tries
                    tries++;
                    random = (int) (Model.Core.GenerateUniform() * (sites.Count - 1));
                    crntSite = sites[random];

                    // Get an active site
                    //while (!(((Site)crntSite).IsActive) && sites.Count > 0) {
                    //while (sites.Count > 0) {
                        //Model.Core.UI.WriteLine("   ERROR:  Non-active site included in stand {0}.", stand.MapCode);
                    //    sitesConsidered.Enqueue(crntSite);
                    //    sites.Remove(crntSite);
                    //    random = (int)(Model.Core.GenerateUniform() *
                    //        (sites.Count - 1));
                    //    crntSite = sites[random];
                    //} // while (!(((Site)crntSite).IsActive) && sites.Count > 0)

                    if (sites.Count <= 0)
                        break;
                    
                    //if it's not on the sitesConsidered queue already
                    if (!sitesConsidered.Contains(crntSite)) 
                    {
                        // now put this site on for consideration (which will 
                        // later be put onto the sitesToHarvest queue).
                        sitesToConsider.Enqueue(crntSite);
                        sites.Remove(crntSite);
                        found_site = true;
                    }
                } // while (!found_site && tries < sites.Count)

                //if the site isn't found (prev loop was broken because 
                // of too many tries) then break from this stand entirely
                if (!found_site) {
                    break;
                }
                //reset areaSelected = 0 to start over
                patchAreaSelected = 0;
                //Model.Core.UI.WriteLine("areaSelected = {0}", areaSelected);
            } // while (areaSelected < standTargetArea && sites.Count > 0)

            // if the stand met the criteria for the harvest, mark it
            // as harvested otherwise add the prescription name to the
            // stands rejected prescription list
            // if priority = PatchSize that overrides targetArea as priority for the harvest

            if (areaSelected >= standTargetArea || priority == PatchCutPriority.PatchSize) {
                stand.MarkAsHarvested();
                stand.EventId = EventId.MakeNewId();
            } else {
                //Model.Core.UI.WriteLine("Rejecting stand {0} for prescription {1}",stand.MapCode, stand.PrescriptionName);
                stand.RejectPrescriptionName(stand.PrescriptionName);
            }

            // if harvest criteria met (includes priority), yield the sites, else, don't
            if (areaSelected >= standTargetArea || priority == PatchCutPriority.PatchSize)
            {
                while (sitesToHarvest.Count > 0) {
                    yield return sitesToHarvest.Dequeue();
                }
            }

        } // IEnumerator<ActiveSite> IEnumerable<ActiveSite>.GetEnumerator()

        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<ActiveSite>) this).GetEnumerator();
        }
        
        //---------------------------------------------------------------------
    }
}

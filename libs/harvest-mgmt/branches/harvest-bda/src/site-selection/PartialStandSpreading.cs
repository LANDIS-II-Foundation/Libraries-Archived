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
    /// A site-selection method that harvests site-by-site until a target
    /// size is reached.
    /// </summary>
    public class PartialStandSpreading
        : StandSpreading, ISiteSelector, IEnumerable<ActiveSite> {
        private Stand initialStand;
        private double minTargetSize;
        private double maxTargetSize;
        private double areaSelected;

        private Queue<ActiveSite> harvestableSites;  // Sites to harvest
        Queue<Stand> standsToHarvest;                // Stands to harvest
        Queue<double>standsToHarvestRankings;        // Stands to harvest rankings
        List<Stand> standsToReject;                  // Stands to mark as rejected
        private int minTimeSinceDamage;              // From prescription

        //collect all 8 relative neighbor locations in array
        public static RelativeLocation[] all_neighbor_locations = new RelativeLocation[]
        {
            //define 8 neighboring locations
            new RelativeLocation(-1, 0),
            new RelativeLocation( 1, 0),
            new RelativeLocation( 0, -1),
            new RelativeLocation( 0, 1),
            new RelativeLocation(-1, -1),
            new RelativeLocation(-1, 1),
            new RelativeLocation(1, -1),
            new RelativeLocation(1, 1)
        };

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        /// <param name="minTargetSize">
        /// The min size (area) to harvest.  Units: hectares.
        /// </param>
        /// <param name="maxTargetSize">
        /// The max size (area) to harvest.  Units: hectares
        /// </param>
        public PartialStandSpreading(double minTargetSize, double maxTargetSize) {
            this.minTargetSize = minTargetSize;
            this.maxTargetSize = maxTargetSize;
        }

        //---------------------------------------------------------------------

        double ISiteSelector.AreaSelected {
            get {
                return areaSelected;
            }
        }

        //---------------------------------------------------------------------

        IEnumerable<ActiveSite> ISiteSelector.SelectSites(Stand stand) {
            initialStand = stand;
            return this;
        }

        //---------------------------------------------------------------------

        IEnumerator<ActiveSite> IEnumerable<ActiveSite>.GetEnumerator() {

            // harvestable sites are thos sites that will be harvested if
            // the minTargetSize is reached
            //
            // standsToHarvest are the stands from which harvesting will
            // be done if minTargetSize is reached.
            //
            // standsToHarvestRankings holds the rankings of the stands
            // we will be harvesting.
            //
            // standsToReject are those stands immediately that
            // are not harvestable and that will be marked
            // as rejected for the current prescription name

            harvestableSites = new Queue<ActiveSite>();
            areaSelected = 0;
            standsToHarvest = new Queue<Stand>();
            standsToHarvestRankings = new Queue<double>();
            standsToReject = new List<Stand>();
            string prescriptionName = initialStand.PrescriptionName;
            Prescription lastPrescription = initialStand.LastPrescription;
            minTimeSinceDamage = initialStand.MinTimeSinceDamage;
            this.HarvestedNeighbors.Clear();

            // Attempt to do the harvest
            if (SpreadFromStand(initialStand)) {
                int eventId = EventId.MakeNewId();

                // loop through all harvestable stands and update
                // appropriate items
                foreach (Stand standToHarvest in standsToHarvest) {
                    standToHarvest.MarkAsHarvested();
                    standToHarvest.EventId = eventId;
                    standToHarvest.PrescriptionName = prescriptionName;
                    standToHarvest.LastPrescription = lastPrescription;
                    standToHarvest.MinTimeSinceDamage = minTimeSinceDamage;
                    standToHarvest.HarvestedRank = standsToHarvestRankings.Dequeue();
                    if(!(standToHarvest==initialStand))
                        this.HarvestedNeighbors.Add(standToHarvest);
                } // foreach(Stand standToHarvest in standsToHarvest)

            } else {

                // if this set of stands is not harvestable by this prescription
                // mark them all as such using the prescriptionName.

                foreach (Stand standToReject in standsToHarvest) {
                    //Model.Core.UI.WriteLine("Rejecting stand {0} for prescription {1}",standToReject.MapCode, prescriptionName);
                    standToReject.RejectPrescriptionName(prescriptionName);
                    standToReject.HarvestedRank = standsToHarvestRankings.Dequeue();
                } // foreach(Stand standToReject in standsToHarvest)

            } // if(SpreadFromStand(initialStand)) ... else

            // mark all rejected stands as rejected for this
            // prescription name

            foreach (Stand standToReject in standsToReject) {
                //Model.Core.UI.WriteLine("Rejecting stand {0} for prescription {1}",standToReject.MapCode, prescriptionName);
                standToReject.RejectPrescriptionName(prescriptionName);
            }

            // If what was found is enough to harvest, yield it
            if (harvestableSites.Count >= minTargetSize) {
                while (harvestableSites.Count > 0) {
                    yield return harvestableSites.Dequeue();
                }
            }

        } // IEnumerator<ActiveSite> IEnumerable<ActiveSite>.GetEnumerator()

        private static ActiveSite GetNeighboringSite(List<Stand> harvestedNeighbors, Stand neighborStand) 
        {
            // get a shared-edge site from any one of the previously harvested neighboring stands
            // tjs - changed to allow a null return. Sometimes there are not adjacent sites
            ActiveSite returnSite;

            foreach (Site current_site in neighborStand) 
            {
                //check if one of its neighbors is on the edge of the initialStand
                foreach (RelativeLocation relloc in all_neighbor_locations) 
                {

                    //if it's a valid site and is on the edge
                    if (current_site.GetNeighbor(relloc) != null && current_site.GetNeighbor(relloc).IsActive) 
                    {
                        foreach (Stand stand in harvestedNeighbors) 
                        {
                            if (SiteVars.Stand[current_site.GetNeighbor(relloc)] == stand)
                            {
                                returnSite = (ActiveSite) current_site;
                                return returnSite;
                            }
                        } 
                    } 
                }

            } 

            return new ActiveSite();         
        } 

        //--------------------------------------------------------------

        // For the starting stand do partial stand spreading until either
        // we run out of stands or we have our target area
        private bool SpreadFromStand(Stand startingStand) {

            List<Stand> standsConsidered = new List<Stand>();
            // a list of every stand we have thought about considering
            // used to prevent considering a stand more than once
            List<Stand> standsToConsiderAll = new List<Stand>();
            List<StandRanking> standsToConsiderRankings = new List<StandRanking>();
            bool rtrnVal = false;
            Stand crntStand;
            double crntRank;
            ActiveSite startingSite;// = null;

            // If we have a valid starting stand, put it on the list to
            // consider
            if(startingStand != null && !startingStand.IsSetAside) 
            {
                standsToConsiderRankings.Insert(0,GetRanking(startingStand));
                standsToConsiderAll.Add(startingStand);
            }

            while (standsToConsiderRankings.Count > 0 &&
                standsToConsiderRankings[0].Rank > 0 &&
                areaSelected < maxTargetSize) 
                {

                // Get the stand to work with for this loop iteration
                crntStand = standsToConsiderRankings[0].Stand;
                crntRank = standsToConsiderRankings[0].Rank;
                standsToConsiderRankings.RemoveAt(0);

                // If the stand is not set aside, Get the starting site
                if (!crntStand.IsSetAside) 
                {
                    // first stand starts at a random site, subsequent
                    // stands start at an adjoining site
                    if (standsConsidered.Count == 0) 
                    {
                        startingSite = crntStand.GetRandomActiveSite;
                    } else {
                        startingSite = GetNeighboringSite(standsConsidered, crntStand);
                        if (startingSite == false) 
                        {
                            standsToReject.Add(crntStand);
                            continue;
                        }
                    }
                } else {
                    // if the stand is set aside, it doesn't get processed
                    // and its neighbors don't go on the stand list
                    standsToReject.Add(crntStand);
                    continue;
                } 

                // Enqueue the eligible sites and put the stand
                // on the appropriate queue(s)
                if (EnqueueEligibleSites(startingSite, crntStand)) 
                {
                    standsToHarvest.Enqueue(crntStand);
                    standsToHarvestRankings.Enqueue(crntRank);
                } else {
                    standsToReject.Add(crntStand);
                }

                standsConsidered.Add(crntStand);

                if (areaSelected < maxTargetSize) {

                    // Get the neighbors and put them on the
                    // standsToConsider queue

                    foreach (Stand neighbor in crntStand.Neighbors) 
                    {
                        if(!standsConsidered.Contains(neighbor) &&
                            !standsToConsiderAll.Contains(neighbor) &&
                            !neighbor.Harvested) 
                        {
                            StandRanking neighborRanking = GetRanking(neighbor);
                            standsToConsiderAll.Add(neighbor);
                            if (neighborRanking.Rank <= 0) 
                            {
                                continue;
                            }

                            int i;
                            for (i = 0; i < standsToConsiderRankings.Count; i++) 
                            {
                                if (standsToConsiderRankings[i].Rank < neighborRanking.Rank)
                                    break;
                            }

                            standsToConsiderRankings.Insert(i, neighborRanking);

                        } 
                    } 

                } 

            } 

            // If we found enough to meet our goal, return true,
            // otherwise return the default of false.
            if (areaSelected >= minTargetSize)
                rtrnVal = true;

            return rtrnVal;
        } 

        //--------------------------------------------------------------

        // For the current stand, enqueue all the eligible sites onto
        // the harvestableSites queue
        private bool EnqueueEligibleSites(ActiveSite startingSite, Stand crntStand) 
        {
            Queue<ActiveSite> sitesConsidered = new Queue<ActiveSite>();
            Queue<ActiveSite> sitesToConsider = new Queue<ActiveSite>();
            bool rtrnVal = false;

            ActiveSite crntSite = startingSite;

            //The following case could happen if prevent establishment
            //generates empty stands.
            if (crntStand.GetActiveSites().Count <= 0)  
                return false;

            if (crntSite != null)
                sitesToConsider.Enqueue(crntSite);

            while (sitesToConsider.Count > 0 && areaSelected < maxTargetSize) 
            {

                // Get the site to work with for this loop iteration
                crntSite = sitesToConsider.Dequeue();

                // Enqueue and increment area if sight is harvestable
                sitesConsidered.Enqueue(crntSite);
                if (SiteVars.TimeSinceLastDamage(crntSite) >= minTimeSinceDamage) 
                {
                    harvestableSites.Enqueue(crntSite);
                    areaSelected += Model.Core.CellArea;
                    rtrnVal = true;
                }

                // Put those neighbors on the sightsToConsider queue
                foreach (RelativeLocation loc in all_neighbor_locations) 
                {
                    // get a neighbor site
                    Site tempSite = crntSite.GetNeighbor(loc);
                    if(tempSite != null && tempSite.IsActive)
                    {
                    //get a neighbor site (if it's active and non-null)
                    //if (crntSite.GetNeighbor(loc) != null && crntSite.GetNeighbor(loc).IsActive) 
                    //{
                        ActiveSite neighborSite = (ActiveSite) tempSite; // (ActiveSite)crntSite.GetNeighbor(loc);

                        // check if in the same stand and management area
                        // and if it has not been looked at
                        if (SiteVars.Stand[neighborSite] == SiteVars.Stand[crntSite]
                        && SiteVars.ManagementArea[neighborSite] == SiteVars.ManagementArea[crntSite]
                        && !sitesConsidered.Contains(neighborSite)
                        && !sitesToConsider.Contains(neighborSite)) 
                        {

                             sitesToConsider.Enqueue(neighborSite);

                        } 
                    } 
                } 

            } 

            return rtrnVal;

        } 


        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<ActiveSite>)this).GetEnumerator();
        } // IEnumerator IEnumerable.GetEnumerator() {


    } //     public class PartialStandSpreading
} // namespace Landis.Library.HarvestManagement

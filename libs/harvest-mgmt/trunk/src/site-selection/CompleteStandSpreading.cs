// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Edu.Wisc.Forest.Flel.Util;
using Landis.SpatialModeling;
using System.Collections;
using System.Collections.Generic;

namespace Landis.Library.HarvestManagement {
    /// <summary>
    /// A site-selection method that harvests complete stands until a target
    /// size is reached.
    /// </summary>
    public class CompleteStandSpreading
        : StandSpreading, ISiteSelector, IEnumerable<ActiveSite>
    {
        private Stand initialStand;
        private double minTargetSize;
        private double maxTargetSize;
        private double areaSelected;
        private Queue<ActiveSite> harvestableSites;  // Sites to harvest
        Queue<Stand> standsToHarvest;                // Stands to harvest
        Queue<double> standsToHarvestRankings;        // Stands to harvest rankings
        List<Stand> standsToReject;                  // Stands to mark as rejected
        private int minTimeSinceDamage;              // From prescription

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

        public CompleteStandSpreading(double minTargetSize, double maxTargetSize) {
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
                    if (!(standToHarvest == initialStand))
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

        //---------------------------------------------------------------------

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

            // If we have a valid starting stand, put it on the list to
            // consider
            if(startingStand != null && !startingStand.IsSetAside) {
                standsToConsiderRankings.Insert(0,GetRanking(startingStand));
                standsToConsiderAll.Add(startingStand);
            }

            // Miranda and Scheller testing methods to make repeat harvest work with stand spreading.
            //if (startingStand != null && startingStand.IsSetAside)
            //    if( startingStand.LastPrescription.
            //    //if (startingStand.LastPrescription.SiteSelectionMethod.ToString() == "Landis.Harvest.CompleteStandSpreading")
            //    {
            //        standsToConsiderRankings.Insert(0, GetRanking(startingStand));
            //        standsToConsiderAll.Add(startingStand);
            //        return true;
            //    }


            while (standsToConsiderRankings.Count > 0 &&
                standsToConsiderRankings[0].Rank > 0 &&
                areaSelected < maxTargetSize) {

                // Get the stand to work with for this loop iteration
                crntStand = standsToConsiderRankings[0].Stand;
                crntRank = standsToConsiderRankings[0].Rank;
                standsToConsiderRankings.RemoveAt(0);

                // If the stand is set aside, it doesn't get processed
                if (crntStand.IsSetAside)
                    continue;

                // Enqueue the eligible sites and put the stand
                // on the appropriate queue(s)
                if (EnqueueEligibleSites(crntStand)) {
                    standsToHarvest.Enqueue(crntStand);
                    standsToHarvestRankings.Enqueue(crntRank);
                } else {
                    standsToReject.Add(crntStand);
                }

                standsConsidered.Add(crntStand);

                if (areaSelected < maxTargetSize) {

                    // Get the neighbors and put them on the
                    // standsToConsider queue

                    foreach (Stand neighbor in crntStand.Neighbors) {
                        if(!standsConsidered.Contains(neighbor) &&
                            !standsToConsiderAll.Contains(neighbor) &&
                            !neighbor.Harvested) {

                            StandRanking neighborRanking = GetRanking(neighbor);
                            standsToConsiderAll.Add(neighbor);
                            if (neighborRanking.Rank <= 0) {
                                continue;
                            }
                        
                            int i;
                            for (i = 0; i < standsToConsiderRankings.Count; i++) {
                                if (standsToConsiderRankings[i].Rank < neighborRanking.Rank)
                                    break;
                            }

                            standsToConsiderRankings.Insert(i, neighborRanking);

                        } // if(!standsConsidered.Contains(neighbor)

                    } // foreach (Stand neighbor in crntStand.Neighbors)

                } // if(areaSelected >= maxTargetSize)

            } // while (standsToConsider.Count > 0 && ..

            // If we found enough to meet our goal, return true,
            // otherwise return the default of false.
            if (areaSelected >= minTargetSize)
                rtrnVal = true;

            return rtrnVal;
        } // private bool spreadFromStand() {

        //--------------------------------------------------------------
        // For the current stand, enqueue all the eligible sites onto
        // the harvestableSites queue
        private bool EnqueueEligibleSites(Stand crntStand) {
            bool rtrnVal = false;

            int
                sitesHarvested = 0,
                sitesChecked = 0;

            foreach (ActiveSite crntSite in crntStand) {
                sitesChecked++;
                if (SiteVars.TimeSinceLastDamage(crntSite) >= minTimeSinceDamage) {
                    sitesHarvested++;
                    harvestableSites.Enqueue(crntSite);
                    areaSelected += Model.Core.CellArea;
                    rtrnVal = true;
                } 
            } 

            return rtrnVal;

        } // private void EnqueueEligibleSites() {

        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable<ActiveSite>)this).GetEnumerator();
        } // IEnumerator IEnumerable.GetEnumerator() {


    } //     public class PartialStandSpreading
} // namespace Landis.Library.HarvestManagement

// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Edu.Wisc.Forest.Flel.Util;
using Landis.Library.SiteHarvest;
using Landis.SpatialModeling;
using System.Collections;
using System.Collections.Generic;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A stand is a collection of sites and represent typical or average
    /// forest management block sizes.
    /// </summary>
    public class Stand
        : IEnumerable<ActiveSite>
    {
        private uint mapCode;
        private List<Location> siteLocations;
        private double activeArea;
        private ManagementArea mgmtArea;
        private bool harvested;
        private List<Stand> neighbors;
        private List<Stand> ma_neighbors;   //list for neighbors in different management areas
        private ushort age;
        private int yearAgeComputed;
        private int setAsideUntil;
        private int timeLastHarvested;
        // for log, and for marking rejected prescriptions
        private string prescriptionName;
        private Prescription lastPrescription;
        private int event_id; // for log
        private double rank;  // for log
        //harvested_rank, used in log
        private double harvested_rank; // for log

        // tjs 2009.02.07 - Set by prescription to prevent recently
        // damaged sites from being harvested
        private int minTimeSinceDamage;

        // tjs 2009.02.22 - Keep track of which prescriptions
        // will not work with this stand. Used to prevent multiple
        // attempts by a prescription to harvest a stand that
        // will not meet the prescription's requirements. The
        // prescription name is put on the list
        List<string> rejectedPrescriptionNames;
        public double LastAreaHarvested;

        //---------------------------------------------------------------------

        /// <summary>
        /// The code that designates which sites are in the stand in the stand
        /// input map.
        /// </summary>
        public uint MapCode
        {
            get {
                return mapCode;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// All the site locations in the stand, as specified in the stand map.
        /// </summary>
        public List<Location> AllLocations { get; private set; }

        //---------------------------------------------------------------------

        /// <summar>
        /// The list of locations in this stand where a site's land use allows
        /// harvesting at the current timestep.
        /// </summary>
        public List<Location> SiteLocations {
            get {
                return siteLocations;
            }
        }

        //---------------------------------------------------------------------

        public int SiteCount
        {
            get {
                return siteLocations.Count;
            }
        }

        //---------------------------------------------------------------------

        public List<ActiveSite> GetActiveSites() {
            List<ActiveSite> sites = new List<ActiveSite>();
            foreach (ActiveSite site in this) {
                sites.Add(site);
            }
            return sites;
        }

        //---------------------------------------------------------------------

        public double ActiveArea
        {
            get {
                return activeArea;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The management area that the stand belongs to.
        /// </summary>
        public ManagementArea ManagementArea
        {
            get {
                return mgmtArea;
            }
            internal set {
                mgmtArea = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The stand's age, which is the mean of the oldest cohort on each
        /// site within the stand.
        /// </summary>
        public ushort Age
        {
            get {
                if (yearAgeComputed != Model.Core.CurrentTime)
                {
                    age = ComputeAge();
                    yearAgeComputed = Model.Core.CurrentTime;
                }
                return age;
            }
        }


        //---------------------------------------------------------------------

        /// <summary>
        /// Set's or returns this stand's rank.
        /// </summary>
        public double Rank {
            get {
                return this.rank;
            }

            set {
                this.rank = value;
            }
        }


        //---------------------------------------------------------------------

        /// <summary>
        /// Has the stand been harvested during the current timestep?
        /// </summary>
        public bool Harvested
        {
            get {
                return harvested;
            }
        }

        /// <summary>
        /// Sets or returns the rank at which this stand was last harvested
        /// </summary>
        public double HarvestedRank {
            get {
                return this.harvested_rank;
            }

            set {
                this.harvested_rank = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Return the time this stand was last harvested.
        /// </summary>
        public int TimeLastHarvested {
            get {
                return timeLastHarvested;
            }
            set {
                timeLastHarvested = value;
           }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Set by prescription to prevent recently
        /// damaged sites from being harvested
        /// </summary>
        public int MinTimeSinceDamage {
            get {
                return minTimeSinceDamage;
            }
            set {
                minTimeSinceDamage = value;
            }
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Return the time SINCE this stand was last harvested.
        /// </summary>
        public int TimeSinceLastHarvested {
            get {
                return Model.Core.CurrentTime - timeLastHarvested;
            }

        }

        //---------------------------------------------------------------------

        public IEnumerable<Stand> Neighbors
        {
            get {
                return neighbors;
            }
        }

        //---------------------------------------------------------------------

        public IEnumerable<Stand> MaNeighbors
        {
            get {
                return ma_neighbors;
            }
        }

        //---------------------------------------------------------------------

        public IEnumerable<string> RejectedPrescriptionNames {
            get {
                return rejectedPrescriptionNames;
            }
        }

        //---------------------------------------------------------------------

        public bool IsRejectedPrescriptionName(string name) {
            bool
                isRejected = false;
            if(rejectedPrescriptionNames.Contains(name))
                isRejected = true;
            return isRejected;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Has the stand been set aside by a repeat harvest? (or a stand adjacency constraint)
        /// </summary>
        public bool IsSetAside
        {
            get {

                bool ret = (Model.Core.CurrentTime <= setAsideUntil);
                return ret;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Returns the name of the prescription which damaged this stand
        /// </summary>
        public string PrescriptionName {
            get {
                return prescriptionName;
            }

            set {
                prescriptionName = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Returns the prescription which damaged this stand
        /// </summary>
        public Prescription LastPrescription {
            get {
                return lastPrescription;
            }

            set {
                lastPrescription = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Returns the event-id of this harvest
        /// </summary>
        public int EventId {
            get {
                return event_id;
            }

            set {
                event_id = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The # of cohorts cut in the stand during its most recent harvest.
        /// </summary>
        public CohortCounts DamageTable { get; private set; }

        //---------------------------------------------------------------------

        /// <summary>
        /// A new Stand Object, given a map code
        /// </summary>
        public Stand(uint mapCode)
        {
            this.mapCode = mapCode;
            this.AllLocations = new List<Location>();
            this.siteLocations = new List<Location>();
            this.activeArea = Model.Core.CellArea;
            this.mgmtArea = null;
            this.neighbors = new List<Stand>();
            this.ma_neighbors = new List<Stand>();      //new list for neighbors not in this management area
            this.yearAgeComputed = Model.Core.StartTime;
            this.setAsideUntil = Model.Core.StartTime;
            this.timeLastHarvested = -1;
            this.DamageTable = new CohortCounts();
            this.rejectedPrescriptionNames = new List<string>();
        }

        //---------------------------------------------------------------------

        private static RelativeLocation neighborAbove = new RelativeLocation(-1, 0);
        private static RelativeLocation neighborLeft  = new RelativeLocation( 0, -1);
        private static RelativeLocation[] neighborsAboveAndLeft = new RelativeLocation[]{ neighborAbove, neighborLeft };

        //---------------------------------------------------------------------
         public void Add(ActiveSite site) 
         {

            AllLocations.Add(site.Location);
            this.activeArea = AllLocations.Count * Model.Core.CellArea;
            //set site var
            SiteVars.Stand[site] = this;

            //loop- really just 2 locations, relative (-1, 0) and relative (0, -1)
            foreach (RelativeLocation neighbor_loc in neighborsAboveAndLeft) {
                //check this site for neighbors that are different
                Site neighbor = site.GetNeighbor(neighbor_loc);
                if (neighbor != null && neighbor.IsActive) {
                    //declare a stand with this site as its index.
                    Stand neighbor_stand = SiteVars.Stand[neighbor];
                    //check for non-null stand
                    //also, only allow stands in same management area to be called neighbors
                    if (neighbor_stand != null && this.ManagementArea == neighbor_stand.ManagementArea) {
                        //if neighbor_stand is different than this stand, then it is a true
                        //neighbor.  add it as a neighbor and add 'this' as a neighbor of that.
                        if (this != neighbor_stand) {
                            //add neighbor_stand as a neighboring stand to this
                            AddNeighbor(neighbor_stand);
                            //add this as a neighboring stand to neighbor_stand
                            neighbor_stand.AddNeighbor(this);
                        }
                    }
                    //take into account other management areas just for stand-adjacency issue
                    else if (neighbor_stand != null && this.ManagementArea != neighbor_stand.ManagementArea) {
                        if (this != neighbor_stand) {
                            //add neighbor_stand as a ma-neighboring stand to this
                            AddMaNeighbor(neighbor_stand);
                            //add this as a ma-neighbor stand to neighbor_stand
                            neighbor_stand.AddMaNeighbor(this);
                        }
                    }
                }
            }
        }
        //---------------------------------------------------------------------

        public ActiveSite GetRandomActiveSite {
            get
            {
                if(siteLocations == null || siteLocations.Count == 0)
                    return new ActiveSite(); 

                int random = (int)(Model.Core.GenerateUniform() * (siteLocations.Count - 1));
                if(random < 0 || random > siteLocations.Count - 1)
                    return new ActiveSite(); 
                return Model.Core.Landscape[siteLocations[random]];
            }
        }

        //---------------------------------------------------------------------

        public void DelistActiveSite(ActiveSite site) {
            SiteVars.LandUseAllowHarvest[site] = false;
        } 

        //---------------------------------------------------------------------

        protected void AddNeighbor(Stand neighbor)
        {
            Require.ArgumentNotNull(neighbor);
            if (! neighbors.Contains(neighbor))
                neighbors.Add(neighbor);
        }

        //---------------------------------------------------------------------

        protected void AddMaNeighbor(Stand neighbor) {
            Require.ArgumentNotNull(neighbor);
            if (! ma_neighbors.Contains(neighbor)) {
                ma_neighbors.Add(neighbor);
            }
        }

        //---------------------------------------------------------------------

        public ushort ComputeAge()
       {
            double total = 0.0;
            foreach (ActiveSite site in this) {
                total += (double) SiteVars.GetMaxAge(site);
            }
            return (ushort) (total / (double) siteLocations.Count);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the stand for the current timestep by resetting certain
        /// harvest-related properties.
        /// </summary>
        public void InitializeForHarvesting()
        {
            harvested = false;
            rejectedPrescriptionNames.Clear();
            LastAreaHarvested = 0.0;
            siteLocations.Clear();
            foreach (Location location in AllLocations)
            {
                ActiveSite site = Model.Core.Landscape[location];
                SiteVars.CohortsDamaged[site] = 0;
                if (SiteVars.LandUseAllowHarvest[site])
                    siteLocations.Add(location);
            }
            activeArea = siteLocations.Count * Model.Core.CellArea;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Marks a stand as harvested by setting its timeLastHarvested
        /// and harvested as true
        /// </summary>
        public void MarkAsHarvested()
        {
            //reset timeLastHarvested to current time
            this.timeLastHarvested = Model.Core.CurrentTime;

            //mark stand as harvested
            harvested = true;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Adds prescription name to rejectedPrescriptionNames
        /// sets prescription name to empty string
        /// </summary>
        public void RejectPrescriptionName(string name) {
            if(!rejectedPrescriptionNames.Contains(name))
                rejectedPrescriptionNames.Add(name);
            prescriptionName = "";
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Sets the stand aside until a later time for a repeat harvest.
        /// </summary>
        /// <param name="year">
        /// The calendar year until which the stand should be stand aside.
        /// </param>
        public void SetAsideUntil(int year) {
            setAsideUntil = year;
        }

        /// <summary>
        /// Update the damage_table for this stand
        /// </summary>
        /// <param name="siteCounts">
        /// The number of cohorts cut for each species at the site that was
        /// just harvested.
        /// </param>
        public void UpdateDamageTable(CohortCounts siteCounts)
        {
            DamageTable.IncrementCounts(siteCounts);
        }

        /// <summary>
        ///Clear the damage table of all data.
        /// </summary>
        public void ClearDamageTable() {
            DamageTable.Reset();
        }


        //---------------------------------------------------------------------

        IEnumerator<ActiveSite> IEnumerable<ActiveSite>.GetEnumerator()
        {
            foreach (Location location in siteLocations)
               yield return Model.Core.Landscape[location];
        }

        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ActiveSite>)this).GetEnumerator();
        }
    }
} // namespace Landis.Extensions.Harvest

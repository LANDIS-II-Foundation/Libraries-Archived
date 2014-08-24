// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

using Edu.Wisc.Forest.Flel.Util;
using Landis.SpatialModeling;
using System.Collections;
using System.Collections.Generic;

namespace Landis.Extension.BaseHarvest
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
        //dictionary for keeping number of damaged sites.
        // key = species, value = number of occurrences
        private Dictionary<string, int> damage_table;

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

        /// <summar>
        /// The list of locations in this stand at which there exist a site.
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
                if (yearAgeComputed != PlugIn.ModelCore.CurrentTime)
                {
                    age = ComputeAge();
                    yearAgeComputed = PlugIn.ModelCore.CurrentTime;
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
                return PlugIn.ModelCore.CurrentTime - timeLastHarvested;
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

                bool ret = (PlugIn.ModelCore.CurrentTime <= setAsideUntil);
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
        public Dictionary<string, int> DamageTable {
            get {
                return damage_table;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// A new Stand Object, given a map code
        /// </summary>
        public Stand(uint mapCode)
        {
            this.mapCode = mapCode;
            this.siteLocations = new List<Location>();
            this.activeArea = PlugIn.ModelCore.CellArea;
            this.mgmtArea = null;
            this.neighbors = new List<Stand>();
            this.ma_neighbors = new List<Stand>();      //new list for neighbors not in this management area
            this.yearAgeComputed = PlugIn.ModelCore.StartTime;
            this.setAsideUntil = PlugIn.ModelCore.StartTime;
            this.timeLastHarvested = -1;
            //initialize damage_table dictionary
            damage_table = new Dictionary<string, int>();
            this.rejectedPrescriptionNames = new List<string>();
        }

        //---------------------------------------------------------------------

        private static RelativeLocation neighborAbove = new RelativeLocation(-1, 0);
        private static RelativeLocation neighborLeft  = new RelativeLocation( 0, -1);
        private static RelativeLocation[] neighborsAboveAndLeft = new RelativeLocation[]{ neighborAbove, neighborLeft };

        //---------------------------------------------------------------------
         public void Add(ActiveSite site) 
         {

            siteLocations.Add(site.Location);
            this.activeArea = siteLocations.Count * PlugIn.ModelCore.CellArea;
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

                int random = (int)(PlugIn.ModelCore.GenerateUniform() * (siteLocations.Count - 1));
                if(random < 0 || random > siteLocations.Count - 1)
                    return new ActiveSite(); 
                return PlugIn.ModelCore.Landscape[siteLocations[random]];
            }
        }

        //---------------------------------------------------------------------

        public void DelistActiveSite(ActiveSite site) {
            siteLocations.Remove(site.Location);
            this.activeArea = siteLocations.Count * PlugIn.ModelCore.CellArea;
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
            foreach(Site site in this)
                SiteVars.CohortsDamaged[site] = 0;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Marks a stand as harvested by setting its timeLastHarvested
        /// and harvested as true
        /// </summary>
        public void MarkAsHarvested()
        {
            //reset timeLastHarvested to current time
            this.timeLastHarvested = PlugIn.ModelCore.CurrentTime;

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
        public void UpdateDamageTable(string species) {
            try {
                //add this species to the dictionary, with initial value = 1
                damage_table.Add(species, 1);
            }
            //if an ArguementException is caught, increment this key's value
            catch (System.ArgumentException) {
                damage_table[species]++;
            }
        }

        /// <summary>
        ///Clear the damage table of all data.
        /// </summary>
        public void ClearDamageTable() {
            damage_table.Clear();
        }


        //---------------------------------------------------------------------

        IEnumerator<ActiveSite> IEnumerable<ActiveSite>.GetEnumerator()
        {
            foreach (Location location in siteLocations)
               yield return PlugIn.ModelCore.Landscape[location];
        }

        //---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ActiveSite>)this).GetEnumerator();
        }
    }
} // namespace Landis.Extensions.Harvest

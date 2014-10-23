// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;
using System.Reflection;

using log4net;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// Management area is a collection of stands to which specific harvesting
    /// prescriptions are applied.
    /// </summary>
    public class ManagementArea
    {
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly bool isDebugEnabled = log.IsDebugEnabled;

        private uint mapCode;
        private List<Stand> stands;
        private double area;
        private List<AppliedPrescription> prescriptions;
        private bool onMap;

        //---------------------------------------------------------------------

        /// <summary>
        /// The code that represents the area in the management area input map.
        /// </summary>
        public uint MapCode
        {
            get {
                return mapCode;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The number of stands in the management area.
        /// </summary>
        public int StandCount
        {
            get {
                return stands.Count;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The area covered by the management area (units: hectares).
        /// </summary>
        public double Area
        {
            get {
                return area;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The list of prescriptions
        /// </summary>
        public List<AppliedPrescription> Prescriptions
        {
            get {
                return prescriptions;
            }
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Was the management area's map code used in the map of management
        /// areas?
        /// </summary>
        public bool OnMap
        {
            get {
                return onMap;
            }

            set {
                onMap = value;
            }
        }

        //---------------------------------------------------------------------

        public ManagementArea(ushort mapCode)
        {
            this.mapCode = mapCode;
            stands = new List<Stand>();

            prescriptions = new List<AppliedPrescription>();
            onMap = false;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Has a particular prescription been applied to the managment area?
        /// </summary>
        public bool IsApplied(string prescriptionName, int beginTime, int endTime)
        {
            //Model.Core.UI.WriteLine("prescriptionName = {0} beginTime = {1} endTime = {2}");
            //loop through prescriptions already applied to this management area
            //looking for one that matches this exactly.
            foreach (AppliedPrescription appliedPrescription in prescriptions) {
                //if this prescription matches
                if (appliedPrescription.Prescription.Name == prescriptionName && 
                    appliedPrescription.BeginTime == beginTime &&
                    appliedPrescription.EndTime == endTime) {
                    return true;
                }
            }
            //otherwise this exact prescription has not yet been applied
            return false;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Adds a prescription to be applied to the management area.
        /// </summary>
        public void ApplyPrescription(Prescription prescription,
                                      Percentage   percentageToHarvest,
                                      int          startTime,
                                      int          endTime)
        {

            // tjs - 2008.12.17 - reversing if and else if so that it checks for
            // SingleRepeatHarvest. This is done because SingleRepeatHarvest
            // is a descendent of RepeatHarvest, so the RepeatHarvest condition
            // is true if prescription is SingleRepeatHarvest

            if(prescription is SingleRepeatHarvest)
            {
                AppliedRepeatHarvest appy = new AppliedRepeatHarvest((SingleRepeatHarvest) prescription,
                    percentageToHarvest,
                    startTime,
                    endTime);
                prescriptions.Add(appy);
            }
            else if (prescription is RepeatHarvest)
            {
                AppliedRepeatHarvest appy = new AppliedRepeatHarvest((RepeatHarvest)prescription, percentageToHarvest, startTime, endTime);
                prescriptions.Add(appy);
            }
            else
                prescriptions.Add(new AppliedPrescription(prescription,
                                                      percentageToHarvest,
                                                      startTime,
                                                      endTime));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Adds a stand to the management area.
        /// </summary>
        public void Add(Stand stand)
        {
            stands.Add(stand);
            area += stand.ActiveArea;
            //Model.Core.UI.WriteLine("ma {0} now has area {1}", mapCode, area);
            stand.ManagementArea = this;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Finish initializing the management area after reading the stand
        /// map.
        /// </summary>
        /// <remarks>
        /// This phase of initialization includes computing the total area for
        /// the management area, and finishing the initialization of its
        /// applied prescriptions.
        /// </remarks>
        public void FinishInitialization()
        {
            //  Update the total area of the management area after adding all
            //  its stands.
            area = 0;
            foreach (Stand stand in stands) {
                area += stand.ActiveArea;
                stand.TimeLastHarvested = -1 * stand.ComputeAge();

            }

            foreach (AppliedPrescription prescription in prescriptions)
                prescription.FinishInitialization(StandCount, area);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Harvest the area's stands according to its prescriptions.
        /// </summary>
        public void HarvestStands()
        {
            //Model.Core.UI.WriteLine("BaseHarvest: ManagementArea.cs: HarvestStands: Harvesting management area {0} ...", mapCode);

            //initialize each stand for harvesting (setting harvested = false)
            foreach (Stand stand in stands) {
                stand.InitializeForHarvesting();
            }

            //  Determine which are prescriptions are active.
            List<AppliedPrescription> activePrescriptions = new List<AppliedPrescription>();
            foreach (AppliedPrescription prescription in prescriptions) {

                // Decide whether or not to apply prescription

                // tjs 2009.01.10 - prescription application
                // This has been modified so that if the prescription is AppliedRepeatHarvest
                // AND has not been harvested once, it is set to harvest. If it has been set
                // to harvest the first time, it will automatically be harvested by the logic
                // having to do with the setAside member variable (see AppliedRepeatHarvest.cs)

                bool applyPrescription = false;

                if (prescription.BeginTime <= Model.Core.CurrentTime &&
                    prescription.EndTime >= Model.Core.CurrentTime)
                {
                    if (!(prescription is AppliedRepeatHarvest))
                    {
                        applyPrescription = true;
                    }
                    else if (prescription is AppliedRepeatHarvest &&
                        Model.Core.CurrentTime > 0 &&
                        !((AppliedRepeatHarvest)prescription).HasBeenHarvested)
                    {
                        applyPrescription = true;
                        ((AppliedRepeatHarvest)prescription).HasBeenHarvested = true;
                    }
                    
                } // if(prescription.BeginTime <= Model.Core.CurrentTime...

                if(applyPrescription)
                {
                    //Model.Core.UI.WriteLine("   Applying Prescription: {0}  Model.Core.CurrentTime: {1}", prescription.Prescription.Name, Model.Core.CurrentTime);
                    
                    if (isDebugEnabled)
                        log.DebugFormat("  Initializing prescription {0} ...", prescription.Prescription.Name);
                        
                    //Model.Core.UI.WriteLine("   Initializing prescription {0} ...", prescription.Prescription.Name);
                    
                    //set harvesting areas, rank stands (by user choice method)
                    prescription.InitializeForHarvest(stands);
                    
                    if (prescription.AnyUnharvestedStandsRankedAbove0) {
                        //Model.Core.UI.WriteLine("   Adding {0}", prescription.Prescription.Name);
                        foreach (StandRanking sr in prescription.Rankings) {
                            //Model.Core.UI.WriteLine("   Stand {0} ranked {1}", sr.Stand.MapCode, sr.Rank);
                        } 
                        activePrescriptions.Add(prescription);
                    }
                } 
            } 

            if (isDebugEnabled) {
                Model.Core.UI.WriteLine("   Number of active prescriptions: {0}", activePrescriptions.Count);
                for (int i = 0; i < activePrescriptions.Count; i++)
                    Model.Core.UI.WriteLine("    {0})  {1}", i + 1, activePrescriptions[i].Prescription.Name);
            }

            foreach (AppliedPrescription prescription in prescriptions) {
                //Model.Core.UI.WriteLine("      Looping through prescriptions... {0}.", prescription.Prescription.Name);
            
                if (prescription is AppliedRepeatHarvest) 
                {
                    //prescription.Prescription.SiteSelectionMethod = new CompleteStand();
                    //Model.Core.UI.WriteLine("      Attempting to Re-Harvest {0}.", prescription.Prescription.Name);
                    ((AppliedRepeatHarvest) prescription).HarvestReservedStands();
                }
            } 

            //  Loop while there are still active prescriptions that haven't
            //  reached their target harvest areas and that still have
            //  at least one unharvested stand ranked above 0.

            while (activePrescriptions.Count > 0) {
                double[] endProbability = new double[activePrescriptions.Count + 1];

                //  Assign a part of the probability interval [0, 1) to each 
                //  prescription based on how the ratio of the area remaining to
                //  be harvested to the total area to be harvested
                double ratioTotal = 0.0;
                
                foreach (AppliedPrescription prescription in activePrescriptions) {
                    ratioTotal += prescription.AreaRemainingRatio;
                }
                
                if (ratioTotal > 0) {
                    for (int i = 0; i < activePrescriptions.Count; ++i) {
                        AppliedPrescription prescription = activePrescriptions[i];
                        //first prescription, start at 0
                        if (i == 0) {
                            endProbability[i] = prescription.AreaRemainingRatio / ratioTotal;
                        }

                        //last prescription, end at 1.0                    
                        else if (i == activePrescriptions.Count - 1) {
                            endProbability[i] = 1.0;
                        }                    

                        //
                        else {
                            double startProbability = endProbability[i - 1];
                            double intervalWidth = prescription.AreaRemainingRatio / ratioTotal;
                            endProbability[i] = startProbability + intervalWidth;

                        }

                    } // for (int i = 0; i < activePrescriptions.Count; ++i)

                    //  Randomly select one of the active prescriptions and harvest the stand ranked highest by that prescription.
                    AppliedPrescription selectedPrescription = null;

                    double randomNum = Model.Core.GenerateUniform();
                    for (int i = 0; i < activePrescriptions.Count; ++i) {
                        if (randomNum < endProbability[i]) {
                            selectedPrescription = activePrescriptions[i];
                            //Model.Core.UI.WriteLine("\nSELECTED PRESCRIPTION = {0}\n", selectedPrescription.Prescription.Name);
                            break;
                        }
                    }
                    
                    //actually harvest the stands: starting with highest ranked

                    selectedPrescription.HarvestHighestRankedStand();
                    
                    //Model.Core.UI.WriteLine("\nSELECTED PRESCRIPTION = {0}\n", selectedPrescription.Prescription.Name);
                    
                    Stand stand = selectedPrescription.HighestRankedStand;

                    if (stand != null) {
                        //if there was a stand-adjacency constraint on this stand, enforce:
                        foreach (IRequirement r in selectedPrescription.Prescription.StandRankingMethod.Requirements) {
                            //look for stand-adacency constraint in list r ranking methods
                            if (r.ToString() == "Landis.Harvest.StandAdjacency") {
                                StandAdjacency sa = (StandAdjacency) r;
                                //set-aside every stand in this stand's neighbor-list for the specified number of years
                                
                                //IF siteselection = some type of spreading, freeze the spread-list of neighbors
                                if (selectedPrescription.Prescription.SiteSelectionMethod.ToString() == "Landis.Harvest.CompleteStandSpreading"                                 
                                    || selectedPrescription.Prescription.SiteSelectionMethod.ToString() == "Landis.Harvest.PartialStandSpreading") {
                                    
                                    //freeze every stand in the neighbor list
                                    StandSpreading ss = (StandSpreading) selectedPrescription.Prescription.SiteSelectionMethod;
                                    
                                    //if it's spreading, go through the UnharvestedNeighbors list that was built during the site-selection spread
                                    foreach (Stand n_stand in ss.UnharvestedNeighbors) {
                                            //Model.Core.UI.WriteLine("SPREAD setting aside {0}", n_stand.MapCode);
                                        n_stand.SetAsideUntil(Model.Core.CurrentTime + sa.SetAside);
                                    }
                                }
                                else {
                                    //if it's not a spreading, just take all of the stand's neighbors
                                    foreach (Stand n_stand in stand.Neighbors) {
                                            //Model.Core.UI.WriteLine("NON-SPREAD setting aside {0}", n_stand.MapCode);
                                        n_stand.SetAsideUntil(Model.Core.CurrentTime + sa.SetAside);
                                    }
                                }
                                //found and implemented the stand adjacency, so break out of the requirements list
                                break;
                            }
                        }
                    }
                    else {
                        //Model.Core.UI.WriteLine("returned a null stand");
                    }
                    
                    //  Check each prescription to see if there's at least one  unharvested stand that the prescription ranks higher than  0.  
                    // The list is traversed in reverse order, so that the removal of items doesn't mess up the traversal.
                    for (int i = activePrescriptions.Count - 1; i >= 0; --i) {
                        if (! activePrescriptions[i].AnyUnharvestedStandsRankedAbove0) {
                            //Model.Core.UI.WriteLine("removing1 {0}", activePrescriptions[i].Prescription.Name);
                            activePrescriptions.RemoveAt(i);
                        }
                    }
                    
                }
                else {
                    for (int i = activePrescriptions.Count - 1; i >= 0; --i) {
                        //Model.Core.UI.WriteLine("removing2 {0}", activePrescriptions[i].Prescription.Name);
                        activePrescriptions.RemoveAt(i);
                    }
                }
            }  //endwhile
        }

        //---------------------------------------------------------------------

        public IEnumerator<Stand> GetEnumerator()
        {
            foreach (Stand stand in stands)
                yield return stand;
        }
    }
}
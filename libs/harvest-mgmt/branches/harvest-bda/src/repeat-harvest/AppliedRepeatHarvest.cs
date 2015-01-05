// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Edu.Wisc.Forest.Flel.Util;

using System;
using System.Collections.Generic;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// The application of a repeat-harvest to a management area.
    /// </summary>
    public class AppliedRepeatHarvest
        : AppliedPrescription
    {
        private delegate void SetAsideMethod(Stand stand);

        //---------------------------------------------------------------------

        private RepeatHarvest repeatHarvest;
        private bool isMultipleRepeatHarvest;
        private SetAsideMethod setAside;
        // tjs 2009.01.09
        private bool hasBeenHarvested;
        //  The queue is in the chronological order.
        public Queue<ReservedStand> reservedStands;

        //---------------------------------------------------------------------

        public AppliedRepeatHarvest(RepeatHarvest  repeatHarvest,
                                    Percentage percentageToHarvest,
                                    Percentage percentStandsToHarvest,
                                    int            beginTime,
                                    int            endTime)
            : base(repeatHarvest,
                   percentageToHarvest,
                   percentStandsToHarvest,
                   beginTime,
                   endTime)
        {
            this.repeatHarvest = repeatHarvest;
            // tjs 2009.01.09
            hasBeenHarvested = false;
            if (repeatHarvest is SingleRepeatHarvest) {
                isMultipleRepeatHarvest = false;
                setAside = SetAsideForSingleHarvest;
            }
            else {
                isMultipleRepeatHarvest = true;
                setAside = SetAsideForMultipleHarvests;
            }
            this.reservedStands = new Queue<ReservedStand>();
        }

        //---------------------------------------------------------------------

        // <summary>
        // Has this ever been harvested - tjs 2009.01.09
        // </summary>
        public bool HasBeenHarvested
        {
            get
            {
                return hasBeenHarvested;
            }
            set
            {
                hasBeenHarvested = value;
            }
        }
        // <summary>
        // Time interval for repeat harvest - tjs 2008.12.17
        // </summary>
        public int Interval
        {
            get
            {
                return repeatHarvest.Interval;
            }
        }
        // <summary>
        // Whether the prescription is a mutliple repeat harvest.
        // </summary>
        public bool IsMultipleRepeatHarvest
        {
            get {
                return isMultipleRepeatHarvest;
            }
        }

 

        //---------------------------------------------------------------------

        /// <summary>
        /// Sets a stand aside for a single additional harvest.
        /// </summary>
        public void SetAsideForSingleHarvest(Stand stand)
        {
            stand.SetAsideUntil(Math.Min(Model.Core.CurrentTime + repeatHarvest.Interval,
                                         EndTime));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Sets a stand aside for multiple additional harvests.
        /// </summary>
        public void SetAsideForMultipleHarvests(Stand stand)
        {
            stand.SetAsideUntil(EndTime);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Harvests the highest-ranked stand which hasn't been harvested yet
        /// during the current timestep.
        /// </summary>
        public override void HarvestHighestRankedStand()
        {
        
            base.HarvestHighestRankedStand();


            foreach (Stand stand in repeatHarvest.HarvestedStands)
            {
                if (!stand.IsSetAside)
                {
                    setAside(stand);
                    ScheduleNextHarvest(stand);
                }
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Schedules the next harvest for a stand that's been set aside
        /// (reserved).
        /// </summary>
        protected void ScheduleNextHarvest(Stand stand)
        {
            int nextTimeToHarvest = Model.Core.CurrentTime + repeatHarvest.Interval;
            if (nextTimeToHarvest <= EndTime)
                reservedStands.Enqueue(new ReservedStand(stand, nextTimeToHarvest));
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Harvests the stands that have repeat harvests scheduled for the
        /// current time step.
        /// </summary>
        public void HarvestReservedStands()
        {
            while (reservedStands.Count > 0 &&
                   reservedStands.Peek().NextTimeToHarvest <= Model.Core.CurrentTime) {
                //Stand stand = reservedStands.Dequeue().Stand;
                Stand stand = reservedStands.Peek().Stand;
                
                repeatHarvest.Harvest(stand);

                stand = reservedStands.Dequeue().Stand;
                
                if (isMultipleRepeatHarvest)
                    ScheduleNextHarvest(stand);
            }
        }
    }
}

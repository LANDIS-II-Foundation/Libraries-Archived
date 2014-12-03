// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using System.Collections.Generic;   

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A ranking requirement which requires a stand be no more than a certain
    /// maximum age to be eligible for ranking.
    /// </summary>
    public class RegulateAgesRank
        : StandRankingMethod
    {

        //age_array keeps all of the unique ages
        private int [] age_array;
        //freq_array is parallel to age_array, and keeps the frequency of stands
        //with that age.
        private double [] freq_array;
        //keep age count, made global for use in ComputeRank
        int age_count = 0;        
        
        //---------------------------------------------------------------------
        /// <summary>
        /// Initialize ranking of stands.  assigns age frequencies for this
        /// ranking method to use in ComputeRank.
        /// </summary>
        protected override void InitializeForRanking(List<Stand> stands, int standCount)
        {            
            //initialize the parallel arrays to size of stand collection
            age_array = new int[stands.Count];
            
            //count = number of ages that have been filled
            age_count = 0;
            freq_array = new double[stands.Count];
            
            //assigned tells whether the age has been assigned a spot in the array yet
            bool assigned = false;
            
            //j indexes both arrays (to keep them parallel)
            int j = 0;
            //loop through each stand:
            foreach (Stand stand in stands) {
                //reset assigned to false because we don't know if this stand's age
                //is currently in the age_array or not
                assigned = false;
                
                //before adding a stand's age to a new element of the age_array
                //check to see if it is already in the array
                for (j = 0; j < age_count; j++) {
                    if (stand.Age == age_array[j]) {
                        assigned = true;
                        //increment frequency counter on this element
                        freq_array[j]++;
                        //found a match so break out of loop
                        break;
                    }
                }
                //if after looping through the age array this stand's age has
                //not been found, then it has never been assigned a spot in the
                //age_array.
                if (!assigned) {
                    //give this age to the array
                    age_array[j] = stand.Age;
                    //increment age_count to say that another spot in the array has
                    //been filled (a new age appeared).
                    age_count++;
                    //set frequency of this age = 1 (because this is the only
                    //appearance of that age in the age_array)
                    freq_array[j] = 1;
                    //Model.Core.UI.WriteLine("not assigned.  element {0} gets {1}", j, stand.Age);
                }
            }
            
            //after done summing the frequencies, divide each element by the stand count
            //to get their frequency percentage
            for (j = 0; j < age_count; j++) {
                freq_array[j] = freq_array[j] / (double) stands.Count;
            }
        }

        //---------------------------------------------------------------------
        public RegulateAgesRank()
        {
        }

        //---------------------------------------------------------------------
        /// <summary>
        /// Computes the rank for a stand.
        /// </summary>
        /// <remarks>
        /// Stands are ranked by age class so that over time harvesting will
        /// result in an even distribution of sites by age classes.
        /// </remarks>
        protected override double ComputeRank(Stand stand, int i)
        {
            double freq = 0.0;
            //find where the stands of this age are in the array
            //and return that frequency from the freq_array
            int k = 0;
            for (k = 0; k < age_count; k++) {
                if (stand.Age == age_array[k]) {
                    freq = freq_array[k];
                    //Model.Core.UI.WriteLine("stand {0}.\nreturning age = {1} and freq = {2}", stand.MapCode, age_array[k], freq);
                    //break loop when freq for this stand has been found
                    break;
                }
            }
            //rank = freq * e^(age / 10) <-- using age / 10 to get a smaller number
            return freq * (System.Math.Exp(stand.Age / 10));

        }
    }
}
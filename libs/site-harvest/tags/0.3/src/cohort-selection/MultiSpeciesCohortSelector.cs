// Copyright 2005 University of Wisconsin

using Landis.Core;
using Landis.Library.AgeOnlyCohorts;
using System.Collections.Generic;

namespace Landis.Library.Harvest
{
    /// <summary>
    /// Removes certain cohorts of one or more species from a site.
    /// </summary>
    public class MultiSpeciesCohortSelector
        : ICohortSelector
    {
        private Dictionary<ISpecies, SelectCohorts.Method> selectionMethods;

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets or sets the selection method for a species.
        /// </summary>
        /// <remarks>
        /// When getting the selection method, if a species has none, null is
        /// returned.
        /// </remarks>
        public SelectCohorts.Method this[ISpecies species]
        {
            get {
                SelectCohorts.Method method;
                selectionMethods.TryGetValue(species, out method);
                return method;
            }

            set {
                selectionMethods[species] = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        public MultiSpeciesCohortSelector()
        {
            selectionMethods = new Dictionary<ISpecies, SelectCohorts.Method>();
        }

        //---------------------------------------------------------------------

    	/// <summary>
    	/// Selects which of a species' cohorts are harvested.
    	/// </summary>
    	public void Harvest(ISpeciesCohorts         cohorts,
                            ISpeciesCohortBoolArray isHarvested)
    	{
    	    SelectCohorts.Method selectionMethod;
    	    if (selectionMethods.TryGetValue(cohorts.Species, out selectionMethod))
    	        selectionMethod(cohorts, isHarvested);
    	}
    }
}

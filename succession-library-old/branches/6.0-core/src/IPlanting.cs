//using Landis.Species;
using System.Collections.Generic;
using Wisc.Flel.GeospatialModeling.Landscapes;

namespace Landis.Library.Succession
{
    /// <summary>
    /// The form of reproduction that represents the planting of trees after
    /// a harvest.
    /// </summary>
    public interface IPlanting
        : IFormOfReproduction
    {
        /// <summary>
        /// Schedules one or more species to be planted during the reproduction
        /// phase of succession.
        /// </summary>
        /// <param name="speciesList">
        /// The list of species to be planted.
        /// </param>
        /// <param name="site">
        /// The site where the species are to be planted.
        /// </param>
        void Schedule(Planting.SpeciesList speciesList,
                      ActiveSite           site);
    }
}

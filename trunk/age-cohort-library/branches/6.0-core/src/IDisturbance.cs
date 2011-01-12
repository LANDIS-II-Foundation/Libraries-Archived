//using Landis.PlugIns;
using Landis.Core;
//using Wisc.Flel.GeospatialModeling.Landscapes;
using Landis.SpatialModeling;
using Landis.SpatialModeling.CoreServices;

namespace Landis.Library.AgeOnlyCohorts
{
    /// <summary>
    /// A disturbance that damages cohorts.
    /// </summary>
    public interface IDisturbance
    {
        /// <summary>
        /// The disturbance's type.
        /// </summary>
        ExtensionType Type
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The current site that the disturbance is damaging.
        /// </summary>
        ActiveSite CurrentSite
        {
            get;
        }
    }
}

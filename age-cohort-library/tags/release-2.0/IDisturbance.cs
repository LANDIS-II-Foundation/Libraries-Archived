using Landis.Landscape;
using Landis.PlugIns;

namespace Landis.AgeCohort
{
    /// <summary>
    /// A disturbance that damages cohorts.
    /// </summary>
    public interface IDisturbance
    {
        /// <summary>
        /// The disturbance's type.
        /// </summary>
        PlugInType Type
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

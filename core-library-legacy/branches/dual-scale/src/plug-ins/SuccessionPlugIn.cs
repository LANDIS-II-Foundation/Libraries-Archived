//  Author: Jimm Domingo, UW-Madison, FLEL

namespace Landis.PlugIns
{
    /// <summary>
    /// Base class for succession plug-ins.
    /// </summary>
    public abstract class SuccessionPlugIn
        : PlugIn
    {
        private Cohorts.TypeIndependent.ILandscapeCohorts cohorts;

        //---------------------------------------------------------------------

        /// <summary>
        /// The cohorts for all the sites on the landscape.
        /// </summary>
        public Cohorts.TypeIndependent.ILandscapeCohorts Cohorts
        {
            get {
                return cohorts;
            }

            protected set {
                cohorts = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        protected SuccessionPlugIn(string name)
            : base(name, new PlugInType("succession"))
        {
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the site cohorts for the active sites.
        /// </summary>
        /// <param name="initialCommunities">
        /// Path to the file with initial communities' definitions.
        /// </param>
        /// <param name="initialCommunitiesMap">
        /// Path to the raster file showing where the initial communities are.
        /// </param>
        public abstract void InitializeSites(string initialCommunities,
                                             string initialCommunitiesMap);
    }
}

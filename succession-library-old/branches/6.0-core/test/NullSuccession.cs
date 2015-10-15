using Landis.InitialCommunities;
using System.Collections.Generic;
using Wisc.Flel.GeospatialModeling.Landscapes;

namespace Landis.Test.Succession
{
    //  A null implementation of the succession plug-in for testing.
    public class NullSuccession
        : Landis.Succession.PlugIn
    {
        public NullSuccession()
            : base("Null Succession")
        {
        }

        //---------------------------------------------------------------------

        protected NullSuccession(string name)
            : base(name)
        {
        }

        //---------------------------------------------------------------------

        public override void Initialize(string        dataFile,
                                        PlugIns.ICore modelCore)
        {
        }

        //---------------------------------------------------------------------

        protected override void InitializeSite(ActiveSite site,
                                               ICommunity initialCommunity)
        {
        }

        //---------------------------------------------------------------------

        protected override void AgeCohorts(ActiveSite site,
                                           ushort     years,
                                           int?       successionTimestep)
        {
        }

        //---------------------------------------------------------------------

        public override byte ComputeShade(ActiveSite site)
        {
            return 0;
        }

        //---------------------------------------------------------------------

        public new void ReproduceCohorts(IEnumerable<ActiveSite> sites)
        {
        }
    }
}

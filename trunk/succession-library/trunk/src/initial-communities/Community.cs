using Landis.Library.AgeOnlyCohorts;
using System.Collections.Generic;

namespace Landis.Library.InitialCommunities
{
    public class Community
        : ICommunity
    {
        private ushort mapCode;
        //private ISiteCohorts cohorts;
        private List<ISpeciesCohorts> cohorts;

        //---------------------------------------------------------------------

        public ushort MapCode
        {
            get {
                return mapCode;
            }
        }

        //---------------------------------------------------------------------

        public List<ISpeciesCohorts> Cohorts//ISiteCohorts Cohorts
        {
            get {
                return cohorts;
            }
        }

        //---------------------------------------------------------------------

        public Community(ushort                 mapCode,
            List<ISpeciesCohorts> cohorts)
                         //ISiteCohorts cohorts)
        {
            this.mapCode = mapCode;
            this.cohorts = cohorts;
        }
    }
}

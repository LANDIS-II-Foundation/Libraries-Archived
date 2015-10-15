using Landis.Library.BaseCohorts;

namespace Landis.Library.InitialCommunities
{
    public class Community
        : ICommunity
    {
        private ushort mapCode;
        private ISiteCohorts cohorts;

        //---------------------------------------------------------------------

        public ushort MapCode
        {
            get {
                return mapCode;
            }
        }

        //---------------------------------------------------------------------

        public ISiteCohorts Cohorts
        {
            get {
                return cohorts;
            }
        }

        //---------------------------------------------------------------------

        public Community(ushort                 mapCode,
                         ISiteCohorts cohorts)
        {
            this.mapCode = mapCode;
            this.cohorts = cohorts;
        }
    }
}

namespace Landis.InitialCommunities
{
    public class Community
        : ICommunity
    {
        private ushort mapCode;
        private AgeCohort.ISiteCohorts cohorts;

        //---------------------------------------------------------------------

        public ushort MapCode
        {
            get {
                return mapCode;
            }
        }

        //---------------------------------------------------------------------

        public AgeCohort.ISiteCohorts Cohorts
        {
            get {
                return cohorts;
            }
        }

        //---------------------------------------------------------------------

        public Community(ushort                 mapCode,
                         AgeCohort.ISiteCohorts cohorts)
        {
            this.mapCode = mapCode;
            this.cohorts = cohorts;
        }
    }
}

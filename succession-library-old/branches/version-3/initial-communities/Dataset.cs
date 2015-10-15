using System.Collections.Generic;

namespace Landis.InitialCommunities
{
    public class Dataset
        : IDataset
    {
        private List<ICommunity> communities;

        //---------------------------------------------------------------------

        public Dataset()
        {
            communities = new List<ICommunity>();
        }

        //---------------------------------------------------------------------

        public void Add(ICommunity community)
        {
            communities.Add(community);
        }

        //---------------------------------------------------------------------

        public ICommunity Find(ushort mapCode)
        {
            foreach (ICommunity community in communities)
                if (community.MapCode == mapCode)
                    return community;
            return null;
        }
    }
}

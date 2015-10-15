using System.Collections.Generic;

namespace Landis.Library.InitialCommunities
{
    public class Dataset
        : IDataset
    {
        private Dictionary<uint, ICommunity> communities;

        //---------------------------------------------------------------------

        public Dataset()
        {
            communities = new Dictionary<uint, ICommunity>();
        }

        //---------------------------------------------------------------------

        public void Add(ICommunity community)
        {
            communities.Add(community.MapCode, community);
        }

        //---------------------------------------------------------------------

        public ICommunity Find(uint mapCode)
        {
            if (communities.ContainsKey(mapCode))
            {
                return communities[mapCode];
            }
            return null;
        }
    }
}

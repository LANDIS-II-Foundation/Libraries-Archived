// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

namespace Landis.Extension.BaseHarvest
{
    /// <summary>
    /// A ranking requirement which requires a stand's neighbors be at 
    /// least a certain minimum age to be eligible for ranking.
    /// </summary>
    public class MinTimeSinceLastHarvest
        : IRequirement
    {
        private ushort minTime;

        //---------------------------------------------------------------------

        public MinTimeSinceLastHarvest(ushort time)
        {
            minTime = time;
        }

        //---------------------------------------------------------------------

        //require that the stand wait a certain minimum time before being
        //eligible for harvesting again.
        bool IRequirement.MetBy(Stand stand)
        {
            //PlugIn.ModelCore.UI.WriteLine("stand {0} TimeLastHarvested = {1}", stand.MapCode, stand.TimeLastHarvested);
            int time_since = PlugIn.ModelCore.CurrentTime - stand.TimeLastHarvested;
            //PlugIn.ModelCore.UI.WriteLine("time_since stand {0} was harvested = {1}\n", stand.MapCode, time_since);
            return time_since >= minTime;
        }
        
    }
}
using Landis.SpatialModeling;
namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A ranking requirement which requires the average time to next BDA disturbance
    /// to be less than or equal to the input presalvage years
    /// </summary>
    public class Presalvage
        : IRequirement
    {
        private ushort presalvYears;

        //---------------------------------------------------------------------

        public Presalvage(ushort years)
        {
            presalvYears = years;
        }

        //---------------------------------------------------------------------

        bool IRequirement.MetBy(Stand stand)
        {
            int sumTimeOfNext = 0;
            int siteCount = 0;
            if (SiteVars.NextBDA == null)
            {
                return false;
            }
            else
            {
                foreach (ActiveSite site in stand)
                {
                    int timeOfNext = SiteVars.NextBDA[site];
                    siteCount += 1;
                    sumTimeOfNext += timeOfNext;
                }

                double avgTimeOfNext = (double)sumTimeOfNext / (double)siteCount;

                return avgTimeOfNext <= (Model.Core.CurrentTime + presalvYears);

            }
        }
    }
}

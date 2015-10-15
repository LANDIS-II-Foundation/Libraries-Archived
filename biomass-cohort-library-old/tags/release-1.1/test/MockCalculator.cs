using Edu.Wisc.Forest.Flel.Util;
using Landis.Biomass;
using Landis.Landscape;

namespace Landis.Test.Biomass
{
    public class MockCalculator
        : ICalculator
    {
        public int CountCalled;
        public int Change;
        public Percentage NonWoodyPercentage;
        public int Mortality;

        //---------------------------------------------------------------------

        public int MortalityWithoutLeafLitter
        {
            get {
                return Mortality;
            }
        }

        //---------------------------------------------------------------------

        public MockCalculator()
        {
        }

        //---------------------------------------------------------------------

        public int ComputeChange(ICohort    cohort,
                                 ActiveSite site,
                                 int        siteBiomass,
                                 int        prevYearSiteMortality)
        {
            CountCalled++;
            return Change;
        }

        //---------------------------------------------------------------------

        public Percentage ComputeNonWoodyPercentage(ICohort    cohort,
                                                    ActiveSite site)
        {
            return NonWoodyPercentage;
        }
    }
}

using Landis.Biomass;
using Landis.Landscape;

namespace Landis.Test.Biomass
{
	public class MockCalculator
	    : ICalculator
	{
		public int CountCalled;
		public int Change;

		//---------------------------------------------------------------------

		public int MortalityWithoutLeafLitter
		{
		    get {
		        return 0;
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
	}
}

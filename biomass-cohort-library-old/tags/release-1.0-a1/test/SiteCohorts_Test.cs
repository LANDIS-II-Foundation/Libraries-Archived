using Edu.Wisc.Forest.Flel.Util;

using Landis.Biomass;
using Landis.Cohorts;
using Landis.Landscape;
using Landis.Species;

using NUnit.Framework;

using System.Collections.Generic;

namespace Landis.Test.Biomass
{
	[TestFixture]
	public class SiteCohorts_Test
	{
		private ISpecies abiebals;
		private ActiveSite activeSite;
		private MockCalculator mockCalculator;
		private const int successionTimestep = 10;
		private Dictionary<ISpecies, ushort[]> expectedCohorts;

		//---------------------------------------------------------------------

		[TestFixtureSetUp]
		public void Init()
		{
			abiebals = Data.Species["abiebals"];

			bool[,] grid = new bool[,]{ {true} };
			DataGrid<bool> dataGrid = new DataGrid<bool>(grid);
			ILandscape landscape = new Landscape.Landscape(dataGrid);
			activeSite = landscape[1,1];

			mockCalculator = new MockCalculator();
			Landis.Biomass.Cohorts.Initialize(successionTimestep,
			                                  this.DeathNotExpected,
			                                  mockCalculator);

			expectedCohorts = new Dictionary<ISpecies, ushort[]>();
		}

		//---------------------------------------------------------------------

		public void DeathNotExpected(ICohort    cohort,
		                             ActiveSite site)
		{
		    Assert.Fail("A cohort died unexpectedly");
		}

		//---------------------------------------------------------------------

		[Test]
		public void NoCohorts_Grow()
		{
		    SiteCohorts cohorts = new SiteCohorts();
		    mockCalculator.CountCalled = 0;
		    cohorts.Grow(5, activeSite, false);
		    Assert.AreEqual(0, mockCalculator.CountCalled);
		}

		//---------------------------------------------------------------------

		[Test]
		public void SingleCohort()
		{
		    SiteCohorts cohorts = new SiteCohorts();
		    const ushort initialBiomass = 300;
		    cohorts.AddNewCohort(abiebals, initialBiomass);

		    expectedCohorts.Clear();
		    expectedCohorts[abiebals] = new ushort[] { 1, initialBiomass };
		    Util.CheckCohorts(expectedCohorts, cohorts);
		}

		//---------------------------------------------------------------------

		[Test]
		public void CombineYoungCohorts()
		{
		    SiteCohorts cohorts = new SiteCohorts();
		    ushort[] initialBiomass = new ushort[] { 300, 700 };
		    cohorts.AddNewCohort(abiebals, initialBiomass[0]);

		    //  Grow 1st cohort for 4 years, adding 10 to its biomass per year
		    mockCalculator.CountCalled = 0;
		    mockCalculator.Change = 10;
		    cohorts.Grow(4, activeSite, false);

		    Assert.AreEqual(4, mockCalculator.CountCalled);

		    expectedCohorts.Clear();
		    expectedCohorts[abiebals] = new ushort[] {
		        5, (ushort) (300 + 4 * mockCalculator.Change)
		    };
		    Util.CheckCohorts(expectedCohorts, cohorts);

		    //  Add 2nd cohort and then grow both cohorts 6 more years up to
		    //  a succession timestep
		    cohorts.AddNewCohort(abiebals, initialBiomass[1]);
     	    mockCalculator.CountCalled = 0;
		    cohorts.Grow(6, activeSite, true);

		    //  ComputeChange for both cohorts for 5 years, then combine them,
		    //  and then one time for the combined cohort
		    Assert.AreEqual(5 * 2 + 1, mockCalculator.CountCalled);

		    expectedCohorts.Clear();
		    expectedCohorts[abiebals] = new ushort[] {
		        successionTimestep,
		        (ushort)
		            (300 + (4 + 5) * mockCalculator.Change // first cohort before combining
		             + 700 + 5 * mockCalculator.Change     // 2nd cohort before combining
		             + mockCalculator.Change)              // growth after combining
		    };
		    Util.CheckCohorts(expectedCohorts, cohorts);
		}
	}
}

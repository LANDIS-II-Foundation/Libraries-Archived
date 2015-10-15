using Edu.Wisc.Forest.Flel.Grids;
using Edu.Wisc.Forest.Flel.Util;

using Landis.Biomass;
using Landis.Landscape;
using Landis.Species;

using NUnit.Framework;

namespace Landis.Test.Biomass
{
    [TestFixture]
    public class CohortDefoliation_Test
    {
        private ISpecies betualle;
        private ICohort myCohort;
        private ActiveSite myActiveSite;
        private const int mySiteBiomass = 8053;
        private bool myComputeCalled;
        private const double myComputeResult = 0.767;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            betualle = Data.Species["betualle"];

            const ushort age = 100;
            const ushort biomass = 500;
            CohortData data = new CohortData(age, biomass);
            myCohort = new Cohort(betualle, data);

            bool[,] grid = new bool[,]{ {true} };
            DataGrid<bool> dataGrid = new DataGrid<bool>(grid);
            ILandscape landscape = new Landscape.Landscape(dataGrid);
            myActiveSite = landscape[1,1];
        }

        //---------------------------------------------------------------------

        [Test]
        public void DefaultIs0()
        {
            Assert.AreEqual(0.0, CohortDefoliation.Compute(null, null, 0));
        }

        //---------------------------------------------------------------------

        public double MyCompute(ICohort    cohort,
                                ActiveSite site,
                                int        siteBiomass)
        {
            Assert.AreEqual(myCohort, cohort);
            Assert.AreEqual(myActiveSite, site);
            Assert.AreEqual(mySiteBiomass, siteBiomass);
            myComputeCalled = true;
            return myComputeResult;
        }

        //---------------------------------------------------------------------

        [Test]
        public void LocalComputeMethod()
        {
            CohortDefoliation.Delegates.Compute computeMethod = CohortDefoliation.Compute;
            try {
                CohortDefoliation.Compute = MyCompute;
                myComputeCalled = false;
                Assert.AreEqual(myComputeResult, CohortDefoliation.Compute(myCohort,
                                                                           myActiveSite,
                                                                           mySiteBiomass));
                Assert.IsTrue(myComputeCalled);
            }
            finally {
                // Restore the compute delegate.
                CohortDefoliation.Compute = computeMethod;
            }
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void NullCompute()
        {
            CohortDefoliation.Delegates.Compute computeMethod = CohortDefoliation.Compute;
            try {
                CohortDefoliation.Compute = null;
            }
            finally {
                // If the expected exception didn't happen, make sure to
                // restore the delegate.
                if (CohortDefoliation.Compute == null)
                    CohortDefoliation.Compute = computeMethod;
            }
        }
    }
}

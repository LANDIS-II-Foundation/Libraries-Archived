using Landis.Ecoregions;
using Landis.Species;
using Landis.Succession;
using NUnit.Framework;
using System.Collections.Generic;
using Wisc.Flel.GeospatialModeling.Grids;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

using Location = Wisc.Flel.GeospatialModeling.Landscapes.DualScale.Location;

namespace Landis.Test.Succession
{
    [TestFixture]
    public class Reproduction_Test
    {
        const int blockSize = 2;
        private ILandscape landscape;
        private SimpleCore core;

        private static ActiveSite expectedSite;

        private static List<ISpecies> expectedSpecies;
        private static List<ISpecies> speciesThatReproduce;

        private static List<ISpecies> actualSpecies_SeedingAlg;
        private static List<ISpecies> actualSpecies_AddNewCohort;

        //---------------------------------------------------------------------

        public static bool MySeedingAlgorithm(ISpecies   species,
                                              ActiveSite site)
        {
            Assert.IsTrue(expectedSpecies.Contains(species));
            Assert.AreEqual(expectedSite, site);
            actualSpecies_SeedingAlg.Add(species);
            return speciesThatReproduce.Contains(species);
        }

        //---------------------------------------------------------------------

        public static void MyAddNewCohort(ISpecies   species,
                                          ActiveSite site)
        {
            Assert.IsTrue(speciesThatReproduce.Contains(species));
            Assert.AreEqual(expectedSite, site);
            actualSpecies_AddNewCohort.Add(species);
        }

        //---------------------------------------------------------------------

        //  A succession plug-in for the sole purpose of passing a mock core
        //  into the succession library for testing the Reproduction class.
        public class MockSuccession
            : NullSuccession
        {
            public MockSuccession(PlugIns.ICore modelCore)
                : base("MockSuccession")
            {
                //  Initialize the succession library with the core.  The other
                //  3 parameters are passed to Reproduction's Initialize method
                //  but we want to use a mock seeding algorithm.  Since the
                //  base class' Initialize method uses an enumerated type to
                //  specify the seeding algorithm, we just pass in an arbitrary
                //  value, and then call Reproduction's Initialize method
                //  directly with the mock seeding algorithm.
                Initialize(modelCore,
                           null,  // establishment probabilities
                           SeedingAlgorithms.NoDispersal,
                           null); // AddNewCohort delegate
                Reproduction.Initialize(CreateEstablishProbabilities(modelCore),
                                        MySeedingAlgorithm,
                                        MyAddNewCohort);
            }
        }

        //---------------------------------------------------------------------

        //  Create a 2-D array of establish probabilities that are all 1.0
        private static double[,] CreateEstablishProbabilities(PlugIns.ICore modelCore)
        {
            double[,] estabProbs = new double[modelCore.Ecoregions.Count,
                                              modelCore.Species.Count];
            foreach(IEcoregion ecoregion in modelCore.Ecoregions) {
                foreach (ISpecies species in modelCore.Species)
                    estabProbs[ecoregion.Index, species.Index] = 1.0;
            }
            return estabProbs;
        }

        //---------------------------------------------------------------------

        private static Species.IDataset CreateSpeciesDataset()
        {
            List<Species.IParameters> parms = new List<Species.IParameters>();
            for (int i = 1; i <= 10; i++) {
                parms.Add(new Species.Parameters(
                                    string.Format("species {0}", i), // Name
                                    i * 100,                         // Longevity
                                    25,                              // Sexual Maturity
                                    5,                               // Shade Tolerance
                                    1,                               // Fire Tolerance
                                    130,                             // Effective Seed Dispersal Dist
                                    160,                             // Maximum   Seed Dispersal Dist
                                    0.0f,                            // Vegetative Reprod Prob
                                    0,                               // Min Sprout Age
                                    0,                               // Max Sprout Age
                                    PostFireRegeneration.None ));
            }
            return new Species.Dataset(parms);
        }

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            core = new SimpleCore();
            core.Species = CreateSpeciesDataset();

            List<Ecoregions.IParameters> ecoregionParms = new List<Ecoregions.IParameters>();
            ecoregionParms.Add(new Ecoregions.Parameters("Ecoregion A", // Name
                                                         "ecoregion A", // Description
                                                         1,             // Map Code,
                                                         true));        // Is Active?
            ecoregionParms.Add(new Ecoregions.Parameters("Ecoregion D", // Name
                                                         "ecoregion D", // Description
                                                         2,             // Map Code,
                                                         true));        // Is Active?
            core.Ecoregions = new Ecoregions.Dataset(ecoregionParms);

            int A =  core.Ecoregions[0].MapCode;
            int D =  core.Ecoregions[1].MapCode;
            int i = -1;   // inactive ecoregion
            DataGrid<EcoregionCode> ecoregionGrid = new DataGrid<EcoregionCode>(
                Data.MakeEcoregionCodes(new int[,]{
                    //1 2
                    { A,A }, // 1
                    { A,A }, // 2

                    { A,A }, // 3
                    { A,D }, // 4

                    { i,D }, // 5
                    { D,i }  // 6
                }));

            landscape = new Landscape(ecoregionGrid, blockSize);
            core.Landscape = landscape;

            core.Ecoregion = landscape.NewSiteVar<IEcoregion>();
            foreach (ActiveSite activeSite in landscape) {
                ushort mapCode = (ushort) (ecoregionGrid[activeSite.Location]);
                core.Ecoregion[activeSite] = core.Ecoregions.Find(mapCode);
            }

            //  Reproduction.Initialize method accesses the succession
            //  plug-in's via the age-cohort interfaces.  So to avoid an
            //  exception, we just create an empty landscape cohorts object.
            core.SuccessionCohorts = new AgeCohort.LandscapeCohorts(null);

            MockSuccession mockSuccessionPlugIn = new MockSuccession(core);

            expectedSpecies = new List<ISpecies>(core.Species.Count);
            speciesThatReproduce = new List<ISpecies>(core.Species.Count);

            actualSpecies_SeedingAlg = new List<ISpecies>(core.Species.Count);
            actualSpecies_AddNewCohort = new List<ISpecies>(core.Species.Count);
        }

        //---------------------------------------------------------------------

        private void SetList(List<ISpecies>    speciesList,
                             params ISpecies[] speciesToAdd)
        {
            speciesList.Clear();
            if (speciesToAdd != null)
                foreach (ISpecies species in speciesToAdd)
                    speciesList.Add(species);
        }

        //---------------------------------------------------------------------

        [Test]
        public void JustSeeding()
        {
            // For readability
            Species.IDataset spp = core.Species;

            foreach (ActiveSite activeSite in landscape) {
                expectedSite = activeSite;
                actualSpecies_SeedingAlg.Clear();
                actualSpecies_AddNewCohort.Clear();

                if (activeSite.SharesData) {
                    if (activeSite.LocationInBlock == new Location(1,1)) {
                        SetList(expectedSpecies, spp[0], spp[1], spp[2], spp[3], spp[4],
                                                 spp[5], spp[6], spp[7], spp[8], spp[9]);
                        SetList(speciesThatReproduce, spp[2], spp[5], spp[7]);
                    }
                    else if (activeSite.LocationInBlock == new Location(1,2)) {
                        SetList(expectedSpecies, spp[0], spp[1], spp[3], spp[4], spp[6], spp[8], spp[9]);
                        SetList(speciesThatReproduce, spp[0], spp[4]);
                    }
                    else if (activeSite.LocationInBlock == new Location(2,1)) {
                        SetList(expectedSpecies, spp[1], spp[3], spp[6], spp[8], spp[9]);
                        SetList(speciesThatReproduce, spp[1], spp[6], spp[8]);
                    }
                    else {
                        Assert.AreEqual(new Location(2,2), activeSite.LocationInBlock);
                        SetList(expectedSpecies, spp[3], spp[9]);
                        SetList(speciesThatReproduce, spp[3]);
                    }
                }
                else {
                    // Since planting, serotiny and resprouting aren't enabled,
                    // the seeding algorithm to be called with all species.
                    SetList(expectedSpecies, spp[0], spp[1], spp[2], spp[3], spp[4],
                                             spp[5], spp[6], spp[7], spp[8], spp[9]);
                    SetList(speciesThatReproduce, spp[3], spp[6], spp[9]);
                }
                Reproduction.Do(activeSite);

                Assert.AreEqual(expectedSpecies, actualSpecies_SeedingAlg);
                Assert.AreEqual(speciesThatReproduce, actualSpecies_AddNewCohort);
            }
        }

        //---------------------------------------------------------------------

        [Test]
        public void Planting()
        {
            // For readability
            Species.IDataset spp = core.Species;

            ISpecies[] speciesToPlant = new ISpecies[]{ spp[1], spp[4], spp[8] };
            Planting.SpeciesList plantingSpeciesList = new Planting.SpeciesList(speciesToPlant, core.Species);

            foreach (ActiveSite activeSite in landscape) {
                Reproduction.ScheduleForPlanting(plantingSpeciesList, activeSite);
            }
            
            // Don't expected seeding algorithm to be called since each active
            // set will be planted
            expectedSpecies.Clear();

            foreach (ActiveSite activeSite in landscape) {
                expectedSite = activeSite;
                actualSpecies_SeedingAlg.Clear();
                actualSpecies_AddNewCohort.Clear();

                if (activeSite.SharesData) {
                    if (activeSite.LocationInBlock == new Location(1,1)) {
                        SetList(speciesThatReproduce, speciesToPlant);
                    }
                    else {
                        // At all other active sites in the block, nothing else
                        // should reproduce (because first site in block had
                        // planting occur)
                        SetList(speciesThatReproduce);
                    }
                }
                else {
                    SetList(speciesThatReproduce, speciesToPlant);
                }
                Reproduction.Do(activeSite);

                Assert.AreEqual(expectedSpecies, actualSpecies_SeedingAlg);
                Assert.AreEqual(speciesThatReproduce, actualSpecies_AddNewCohort);
            }
        }
    }
}

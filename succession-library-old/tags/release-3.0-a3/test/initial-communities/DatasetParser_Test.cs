using Edu.Wisc.Forest.Flel.Util;
using Edu.Wisc.Forest.Flel.Util.PlugIns;
using Landis.AgeCohort;
using Landis.InitialCommunities;
using NUnit.Framework;
using System.Collections.Generic;

namespace Landis.Test.Succession.InitialCommunities
{
    [TestFixture]
    public class DatasetParser_Test
    {
        private Species.ISpecies oak;
        private DatasetParser parser;
        private LineReader reader;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            List<Species.IParameters> speciesParms = new List<Species.IParameters>();
            speciesParms.Add(new Species.Parameters("oak",
                                                    400,    // longevity
                                                    0,      // maturity
                                                    0,      // shadeTolerance
                                                    0,      // fireTolerance
                                                    0,      // effectiveSeedDist
                                                    0,      // maxSeedDist
                                                    0,      // vegReprodProb
                                                    0,      // minSproutAge
                                                    0,      // maxSproutAge
                                                    Species.PostFireRegeneration.None));
            speciesParms.Add(new Species.Parameters("pine",
                                                    200,    // longevity
                                                    0,      // maturity
                                                    0,      // shadeTolerance
                                                    0,      // fireTolerance
                                                    0,      // effectiveSeedDist
                                                    0,      // maxSeedDist
                                                    0,      // vegReprodProb
                                                    0,      // minSproutAge
                                                    0,      // maxSproutAge
                                                    Species.PostFireRegeneration.Serotiny));
            Species.IDataset speciesDataset = new Species.Dataset(speciesParms);
            oak = speciesDataset["oak"];

            parser = new DatasetParser(10, speciesDataset);
        }

        //---------------------------------------------------------------------

        private FileLineReader OpenFile(string filename)
        {
            return Landis.Data.OpenTextFile(Data.MakeInputPath(filename));
        }

        //---------------------------------------------------------------------

        private void TryParse(string filename,
                              int    errorLineNum)
        {
            try {
                reader = OpenFile(filename);
                IDataset dataset = parser.Parse(reader);
            }
            catch (System.Exception e) {
                Data.Output.WriteLine();
                Data.Output.WriteLine(e.Message.Replace(Data.Directory, Data.DirPlaceholder));
                LineReaderException lrExc = e as LineReaderException;
                if (lrExc != null)
                    Assert.AreEqual(errorLineNum, lrExc.LineNumber);
                throw;
            }
            finally {
                reader.Close();
            }
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void Empty()
        {
            TryParse("empty.txt", LineReader.EndOfInput);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LandisData_WrongName()
        {
            TryParse("LandisData-WrongName.txt", 3);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LandisData_NoValue()
        {
            TryParse("LandisData-NoValue.txt", 3);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LandisData_MissingQuote()
        {
            TryParse("LandisData-MissingQuote.txt", 3);
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(LineReaderException))]
        public void LandisData_WrongValue()
        {
            TryParse("LandisData-WrongValue.txt", 3);
        }

        //---------------------------------------------------------------------

        [Test]
        public void Communities_Timestep10()
        {
            reader = OpenFile("Communities.txt");
            IDataset dataset = parser.Parse(reader);

            ICommunity community = dataset.Find(100);
            Assert.IsNotNull(community);

            ISpeciesCohorts oakCohorts = community.Cohorts[oak];
            Assert.AreEqual(5, oakCohorts.Count);

            List<ushort> actualAges = new List<ushort>(oakCohorts.Count);
            foreach (ICohort cohort in oakCohorts)
                actualAges.Add(cohort.Age);
            ushort[] expectedAges = new ushort[] { 150, 100, 30, 20, 10 };
            AssertAreEqual(expectedAges, actualAges.ToArray());
        }

        //---------------------------------------------------------------------

        private void AssertAreEqual(ushort[] expected,
                                    ushort[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);
            for (int i = 0; i < expected.Length; i++)
                Assert.AreEqual(expected[i], actual[i]);
        }
    }
}

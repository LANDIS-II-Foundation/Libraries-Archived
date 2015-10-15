using Edu.Wisc.Forest.Flel.Util;
using Landis.Succession;
using NUnit.Framework;

namespace Landis.Test.Succession
{
    [TestFixture]
    public class SeedingAlgorithmsUtil_Test
    {
        private InputVar<SeedingAlgorithms> seedAlgVar;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            SeedingAlgorithmsUtil.RegisterForInputValues();
            seedAlgVar = new InputVar<SeedingAlgorithms>("SeedingAlgorithm");
        }

        //---------------------------------------------------------------------

        private void PrintInputVarException(string input)
        {
            try {
                StringReader reader = new StringReader(input);
                seedAlgVar.ReadValue(reader);
            }
            catch (InputVariableException exc) {
                Data.Output.WriteLine(exc.Message);
                throw exc;
            }
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(InputVariableException))]
        public void EmptyString()
        {
            PrintInputVarException("");
        }

        //---------------------------------------------------------------------

        [Test]
        [ExpectedException(typeof(InputVariableException))]
        public void InvalidAlgorithm()
        {
            PrintInputVarException(" Fred'sAlgorithm ");
        }

        //---------------------------------------------------------------------

        private void ReadInputVar(string input)
        {
            StringReader reader = new StringReader(input);
            seedAlgVar.ReadValue(reader);
        }

        //---------------------------------------------------------------------

        [Test]
        public void NoDispersal()
        {
            ReadInputVar(" NoDispersal ");
            Assert.AreEqual(SeedingAlgorithms.NoDispersal, seedAlgVar.Value.Actual);
        }

        //---------------------------------------------------------------------

        [Test]
        public void UniversalDispersal()
        {
            ReadInputVar(" \t UniversalDispersal\n");
            Assert.AreEqual(SeedingAlgorithms.UniversalDispersal, seedAlgVar.Value.Actual);
        }

        //---------------------------------------------------------------------

        [Test]
        public void WardSeedDispersal()
        {
            ReadInputVar(" WardSeedDispersal");
            Assert.AreEqual(SeedingAlgorithms.WardSeedDispersal, seedAlgVar.Value.Actual);
        }
    }
}

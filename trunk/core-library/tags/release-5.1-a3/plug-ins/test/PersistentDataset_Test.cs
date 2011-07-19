//using Edu.Wisc.Forest.Flel.Util;
using Landis.PlugIns;
using NUnit.Framework;
//using System.Collections.Generic;
//using System.IO;

namespace Landis.Test.PlugIns
{
    [TestFixture]
    public class PersistentDataset_Test
    {
        [Test]
        public void OnePlugIn()
        {
            PersistentDataset dataset = new PersistentDataset();

            PlugInInfo fooPlugIn;
            fooPlugIn = new PlugInInfo("Foo Plug-in",
                                       new PlugInType("succession"),
                                       "Org.Bar.Foo.PlugIn,Org.Bar.Foo");
            dataset.PlugIns.Add(new PersistentDataset.PlugInInfo(fooPlugIn));

            string path = Data.MakeOutputPath("OnePlugIn.xml");
            dataset.Save(path);

            PersistentDataset dataset2;
            dataset2 = PersistentDataset.Load(path);

            Assert.AreEqual(dataset.PlugIns.Count, dataset2.PlugIns.Count);
//            AssertAreEqual(dataset, dataset2);
        }
/*
        //---------------------------------------------------------------------

        private void AssertAreEqual(PersistentDriverDataset expected,
                                    PersistentDriverDataset actual)
        {
            Assert.IsNotNull(actual);
            AssertAreEqual(expected.Drivers, actual.Drivers);
            AssertAreEqual(expected.Formats, actual.Formats);
        }

        //---------------------------------------------------------------------

        private void AssertAreEqual(List<PersistentDriverDataset.DriverInfo> expected,
                                    List<PersistentDriverDataset.DriverInfo> actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
                AssertAreEqual(expected[i], actual[i]);
        }

        //---------------------------------------------------------------------

        private void AssertAreEqual(PersistentDriverDataset.DriverInfo expected,
                                    PersistentDriverDataset.DriverInfo actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.ImplementationName, actual.ImplementationName);
            AssertAreEqual(expected.Formats, actual.Formats);
        }

        //---------------------------------------------------------------------

        private void AssertAreEqual(List<PersistentDriverDataset.FormatAccess> expected,
                                    List<PersistentDriverDataset.FormatAccess> actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
                AssertAreEqual(expected[i], actual[i]);
        }

        //---------------------------------------------------------------------

        private void AssertAreEqual(PersistentDriverDataset.FormatAccess expected,
                                    PersistentDriverDataset.FormatAccess actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Format, actual.Format);
            Assert.AreEqual(expected.Access, actual.Access);
        }

        //---------------------------------------------------------------------

        private void AssertAreEqual(List<PersistentDriverDataset.FormatDrivers> expected,
                                    List<PersistentDriverDataset.FormatDrivers> actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Count, actual.Count);
            for (int i = 0; i < expected.Count; i++)
                AssertAreEqual(expected[i], actual[i]);
        }

        //---------------------------------------------------------------------

        private void AssertAreEqual(PersistentDriverDataset.FormatDrivers expected,
                                    PersistentDriverDataset.FormatDrivers actual)
        {
            Assert.IsNotNull(actual);
            Assert.AreEqual(expected.Format, actual.Format);
            Assert.AreEqual(expected.Drivers.Count, actual.Drivers.Count);
            for (int i = 0; i < expected.Drivers.Count; i++)
                Assert.AreEqual(expected.Drivers[i], actual.Drivers[i]);
        }
*/
    }
}

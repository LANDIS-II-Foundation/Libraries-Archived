using Edu.Wisc.Forest.Flel.Util;
using Landis.RasterIO;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Landis.Test.RasterIO
{
    [TestFixture]
    public class PersistentDriverDataset_Test
    {
        [Test]
        public void OneDriver()
        {
            PersistentDriverDataset dataset = new PersistentDriverDataset();

            string fooExt = ".foo";

            PersistentDriverDataset.DriverInfo fooDriver;
            fooDriver = new PersistentDriverDataset.DriverInfo(
                                            "Foo Driver",
                                            "Com.Acme.Foo.Driver,Com.Acme.Foo"
            );
            fooDriver.AddFormat(fooExt, FileAccess.ReadWrite);
            dataset.Drivers.Add(fooDriver);

            PersistentDriverDataset.FormatDrivers fooFormat;
            fooFormat = new PersistentDriverDataset.FormatDrivers(fooExt);
            fooFormat.Drivers.Add(fooDriver.Name);
            dataset.Formats.Add(fooFormat);

            string path = Data.MakeOutputPath("OneDriver.xml");
            dataset.Save(path);

            PersistentDriverDataset dataset2;
            dataset2 = PersistentDriverDataset.Load(path);

            AssertAreEqual(dataset, dataset2);
        }

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
    }
}

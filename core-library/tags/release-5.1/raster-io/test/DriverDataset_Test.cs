using Edu.Wisc.Forest.Flel.Util;
using Landis.RasterIO;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;

namespace Landis.Test.RasterIO
{
    [TestFixture]
    public class DriverDataset_Test
    {
        [Test]
        public void Erdas74()
        {
            string path = Data.MakeInputPath("Erdas74.xml");
            DriverDataset dataset = new DriverDataset(path);

            IList<DriverInfo> gisDrivers = dataset.GetDrivers(".gis");
            Assert.IsNotNull(gisDrivers);
            Assert.AreEqual(1, gisDrivers.Count);
            AssertIsErdas74Driver(gisDrivers[0]);

            IList<DriverInfo> lanDrivers = dataset.GetDrivers(".lan");
            Assert.IsNotNull(lanDrivers);
            Assert.AreEqual(1, lanDrivers.Count);
            AssertIsErdas74Driver(lanDrivers[0]);
        }

        //---------------------------------------------------------------------

        private void AssertIsErdas74Driver(DriverInfo driver)
        {
            Assert.IsNotNull(driver);
            Assert.AreEqual("Erdas 7.4", driver.Name);
            Assert.AreEqual("Landis.RasterIO.Drivers.Erdas74.Driver,Landis.RasterIO.Drivers.Erdas74",
                            driver.ImplementationName);
            Assert.AreEqual(FileAccess.ReadWrite, driver[".gis"]);
            Assert.AreEqual(FileAccess.ReadWrite, driver[".lan"]);
        }
    }
}

using System.Collections.Generic;
using System.IO;

namespace Landis.RasterIO
{
    /// <summary>
    /// A collection of information about installed raster drivers.
    /// </summary>
    public class DriverDataset
    {
        private string path;
        private List<DriverInfo> drivers;
        private Dictionary<string, List<DriverInfo>> formats;

        //---------------------------------------------------------------------

        public DriverDataset(string path)
        {
            this.path = path;
            PersistentDriverDataset dataset = PersistentDriverDataset.Load(path);

            drivers = new List<DriverInfo>();
            foreach (PersistentDriverDataset.DriverInfo info in dataset.Drivers) {
                //  Create a dictionary from list of formats
                Dictionary<string, FileAccess> formats;
                formats = new Dictionary<string, FileAccess>();
                foreach (PersistentDriverDataset.FormatAccess formatAccess in info.Formats)
                    formats[formatAccess.Format] = formatAccess.Access;

                drivers.Add(new DriverInfo(info.Name,
                                           info.ImplementationName,
                                           formats));
            }

            this.formats = new Dictionary<string, List<DriverInfo>>();
            foreach (PersistentDriverDataset.FormatDrivers formatDrivers in dataset.Formats) {
                List<DriverInfo> driverList = new List<DriverInfo>();
                foreach (string driverName in formatDrivers.Drivers) {
                    DriverInfo driverInfo = null;
                    foreach (DriverInfo info in this.drivers)
                        if (info.Name == driverName) {
                            driverInfo = info;
                            break;
                        }
                    if (driverInfo == null)
                        throw new System.ApplicationException(string.Format("Unknown raster driver: \"{0}\"", driverName));
                    driverList.Add(driverInfo);
                }
                formats[formatDrivers.Format] = driverList;
            }
        }

        //---------------------------------------------------------------------

        public IList<DriverInfo> GetDrivers(string format)
        {
            List<DriverInfo> drivers;
            formats.TryGetValue(format, out drivers);
            return drivers;
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;

namespace Landis.RasterIO
{
    /// <summary>
    /// Information about a raster driver.
    /// </summary>
    public class DriverInfo
        : Edu.Wisc.Forest.Flel.Util.PlugIns.Info
    {
        IDictionary<string, FileAccess> formats;

        //---------------------------------------------------------------------

        public DriverInfo(string                          name,
                          string                          implementationName,
                          IDictionary<string, FileAccess> formats)
            : base(name, typeof(IDriver), implementationName)
        {
            if (formats == null)
                throw new ArgumentNullException("formats argument is null");

            this.formats = formats;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the file access that the driver supports for a particular
        /// format.
        /// </summary>
        public FileAccess this[string format]
        {
            get {
                FileAccess fileAccess;
                formats.TryGetValue(format, out fileAccess);
                return fileAccess;
            }
        }
    }
}

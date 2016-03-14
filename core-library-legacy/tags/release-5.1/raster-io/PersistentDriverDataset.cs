using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Landis.RasterIO
{
    /// <summary>
    /// A persistent collection of information about installed raster drivers.
    /// </summary>
    [XmlRoot("RasterDriverDataset")]
    public class PersistentDriverDataset
    {
        /// <summary>
        /// The access to a particular raster format that a driver provides.
        /// </summary>
        public class FormatAccess
        {
            /// <summary>
            /// The format.
            /// </summary>
            [XmlAttribute("Extension")]
            public string Format;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// The type of access provided (read, write, read-write).
            /// </summary>
            [XmlAttribute]
            public FileAccess Access;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// Initializes a new instance.
            /// </summary>
            public FormatAccess()
            {
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// Initializes a new instance.
            /// </summary>
            public FormatAccess(string     format,
                                FileAccess access)
            {
                Format = format;
                Access = access;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Information about a raster driver.
        /// </summary>
        public class DriverInfo
        {
            /// <summary>
            /// The driver's name.
            /// </summary>
            [XmlAttribute]
            public string Name;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// The AssemblyQualifiedName of the class that implements the
            /// driver.
            /// </summary>
            public string ImplementationName;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// The list of formats supported by the driver.
            /// </summary>
            [XmlArrayItem("Format")]
            public List<FormatAccess> Formats;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// Initializes a new instance.
            /// </summary>
            public DriverInfo()
            {
                Formats = new List<FormatAccess>();
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// Initializes a new instance.
            /// </summary>
            public DriverInfo(string name,
                              string implementationName)
                : this()
            {
                Name = name;
                ImplementationName = implementationName;
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// Adds a format to the end of the list of formats supported by
            /// the driver.
            /// </summary>
            public void AddFormat(string     format,
                                  FileAccess access)
            {
                Formats.Add(new FormatAccess(format, access));
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The list of drivers that provide access to a raster format.
        /// </summary>
        public class FormatDrivers
        {
            /// <summary>
            /// The filename extension associated with the format.
            /// </summary>
            [XmlAttribute("Extension")]
            public string Format;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// The drivers that provide access to the raster format.
            /// </summary>
            [XmlArrayItem("Driver")]
            public List<string> Drivers;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// Initializes a new instance with an empty list of drivers.
            /// </summary>
            public FormatDrivers()
            {
                Drivers = new List<string>();
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// Initializes a new instance with an empty list of drivers.
            /// </summary>
            public FormatDrivers(string format)
                : this()
            {
                Format = format;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The drivers in the collection.
        /// </summary>
        [XmlArrayItem("Driver")]
        public List<DriverInfo> Drivers;

        //---------------------------------------------------------------------

        /// <summary>
        /// The formats in the collection, and the drivers that provide access
        /// to each format.
        /// </summary>
        [XmlArrayItem("Format")]
        public List<FormatDrivers> Formats;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance with empty lists of drivers and formats.
        /// </summary>
        public PersistentDriverDataset()
        {
            Drivers = new List<DriverInfo>();
            Formats = new List<FormatDrivers>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Loads a driver dataset from a file.
        /// </summary>
        public static PersistentDriverDataset Load(string path)
        {
            PersistentDriverDataset dataset;
            using (TextReader reader = new StreamReader(path)) {
                XmlSerializer serializer = new XmlSerializer(typeof(PersistentDriverDataset));
                dataset = (PersistentDriverDataset) serializer.Deserialize(reader);
            }
            return dataset;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Saves the driver dataset to a file.
        /// </summary>
        public void Save(string path)
        {
            using (TextWriter writer = new StreamWriter(path)) {
                XmlSerializer serializer = new XmlSerializer(typeof(PersistentDriverDataset));
                serializer.Serialize(writer, this);
            }
        }
    }
}

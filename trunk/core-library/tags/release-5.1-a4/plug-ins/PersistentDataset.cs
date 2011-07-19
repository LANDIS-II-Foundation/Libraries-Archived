using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace Landis.PlugIns
{
    /// <summary>
    /// A persistent collection of information about installed plug-ins.
    /// </summary>
    [XmlRoot("PlugInDataset")]
    public class PersistentDataset
    {
        /// <summary>
        /// Information about a plug-in.
        /// </summary>
        public class PlugInInfo
        {
            /// <summary>
            /// The plug-in's name.
            /// </summary>
            [XmlAttribute]
            public string Name;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// The plug-in's type.
            /// </summary>
            [XmlAttribute("Type")]
            public string TypeName;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// The partial AssemblyQualifiedName of the class that implements
            /// the plug-in.
            /// </summary>
            public string ImplementationName;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// Initializes a new instance with all empty fields.
            /// </summary>
            public PlugInInfo()
            {
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            /// <summary>
            /// Initializes a new instance from a plug-in's information.
            /// </summary>
            public PlugInInfo(Landis.PlugIns.PlugInInfo info)
            {
                Name = info.Name;
                TypeName = info.PlugInType.Name;
                ImplementationName = info.ImplementationName;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The plug-ins in the collection.
        /// </summary>
        [XmlArrayItem("PlugIn")]
        public List<PlugInInfo> PlugIns;

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance with an empty list of plug-ins.
        /// </summary>
        public PersistentDataset()
        {
            PlugIns = new List<PlugInInfo>();
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Loads a plug-in information dataset from a file.
        /// </summary>
        public static PersistentDataset Load(string path)
        {
            PersistentDataset dataset;
            using (TextReader reader = new StreamReader(path)) {
                XmlSerializer serializer = new XmlSerializer(typeof(PersistentDataset));
                dataset = (PersistentDataset) serializer.Deserialize(reader);
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
                XmlSerializer serializer = new XmlSerializer(typeof(PersistentDataset));
                serializer.Serialize(writer, this);
            }
        }
    }
}

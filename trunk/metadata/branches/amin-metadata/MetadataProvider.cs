using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
//using System.Data.DataSetExtensions;
using System.Data.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Xml;
//using System.Dynamic;
//using Microsoft.CSharp.RuntimeBinder;

namespace Landis.Library.Metadata
{
    public class MetadataProvider
    {
        private XmlDocument doc = new XmlDocument();
        //private ExtensionMetadata extensionMetadata;// = new ExtensionMetadata();
        private IMetadata metadata; 

        public MetadataProvider()
        {
            //List<OutputMetadata> lst = new List<OutputMetadata>();
            //lst.All<OutputMetadata>()
            //IEnumerable<OutputMetadata> q = from l in lst select l;

            //System.Data.datase
            //OutputMetadata om = new OutputMetadata();
            //om.GetType().Attributes();
            //dynamic contact = new ExpandoObject();
            //contact.Name = "";

            
            //IAppDomainSetup want To ceate as mataDataTable and use newRow() that returns a dynamic object of the type which is determined by the defined metaDataFields
        }

        public MetadataProvider(IMetadata metadata)
        {
            this.metadata = metadata;
        }


        public XmlNode GetMetadataXmlNode()
        {
            return this.metadata.Get_XmlNode(doc);
        }
        public String GetMetadataString()
        {
            return this.metadata.Get_XmlNode(doc).OuterXml;
        }
        public void WriteMetadataToXMLFile(string metadataFolderPath, string folderName, string fileName)
        {

            if (!System.IO.Directory.Exists(metadataFolderPath))
                System.IO.Directory.CreateDirectory(metadataFolderPath);

            if (!System.IO.Directory.Exists(metadataFolderPath +"\\"+folderName))
                System.IO.Directory.CreateDirectory(metadataFolderPath + "\\" + folderName);
            System.IO.StreamWriter file = new System.IO.StreamWriter(metadataFolderPath + "\\" + folderName +"\\"+ fileName + ".xml", false);
            //string strMetadata = GetMetadataString();
            file.WriteLine(GetMetadataString());
            file.Close();
            file.Dispose();

        }
    }
}

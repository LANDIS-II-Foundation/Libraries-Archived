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
        XmlNode metadataNode;
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
            metadataNode = doc.CreateElement("landisMetadata");
            doc.AppendChild(metadataNode);
            this.metadata = metadata;
        }


        public XmlNode GetMetadataXmlNode()
        {
            metadataNode.AppendChild(this.metadata.Get_XmlNode(doc));
            return metadataNode;
        }
        public String GetMetadataString()
        {
            return this.metadata.Get_XmlNode(doc).OuterXml;
        }
        public void WriteMetadataToXMLFile(string metadataFolderPath, string folderName, string fileName)
        {

            if (!System.IO.Directory.Exists(metadataFolderPath))
                System.IO.Directory.CreateDirectory(metadataFolderPath);

            if (!System.IO.Directory.Exists(metadataFolderPath + "\\" + folderName))
                System.IO.Directory.CreateDirectory(metadataFolderPath + "\\" + folderName);
            System.IO.StreamWriter file;

            try
            {
                foreach(OutputMetadata om in ((ExtensionMetadata)metadata).OutputMetadatas.Where(p=>p.Type == OutputType.Table))
                {
                    XmlDocument outDoc = new XmlDocument();
                    XmlNode outputMetadataNode = outDoc.CreateElement("landisMetadata");
                    XmlNode outputNode = outDoc.CreateElement("output");
                    outputMetadataNode.AppendChild(outputNode);

                    XmlNode extensionNode = outDoc.CreateElement("extension");

                    XmlAttribute outputExtNameAt = outDoc.CreateAttribute("name");
                    outputExtNameAt.Value = ((ExtensionMetadata)metadata).Name;
                    extensionNode.Attributes.Append(outputExtNameAt);

                    XmlAttribute pathAt = outDoc.CreateAttribute("metadataFilePath");
                    pathAt.Value = fileName + ".xml";
                    extensionNode.Attributes.Append(pathAt);
                    outputNode.AppendChild(extensionNode);

                    XmlNode fieldsNode = om.Get_Fields_XmlNode(outDoc);
                    outputNode.AppendChild(fieldsNode);

                   
                    file = new System.IO.StreamWriter(metadataFolderPath + "\\" + folderName + "\\" + om.Name + "_Metadata.xml", false);
                    //string strMetadata = GetMetadataString();
                    file.WriteLine(outputMetadataNode.OuterXml);
                    file.Close();
                    file.Dispose();

                    om.MetadataFilePath = metadataFolderPath + "\\" + folderName + "\\" + om.Name + "_Metadata.xml";
                }
            }
            catch(InvalidCastException ex)
            {
                //string message = 
                throw new ApplicationException(String.Format("Error generating metadata: {0}.", ex.ToString()));
            }


            
            file = new System.IO.StreamWriter(metadataFolderPath + "\\" + folderName + "\\" + fileName + ".xml", false);
            //string strMetadata = GetMetadataString();
            XmlNode metadataNode = doc.CreateElement("landisMetadata");
            metadataNode.AppendChild(((ExtensionMetadata)metadata).Get_XmlNode(doc));
            file.WriteLine(metadataNode.OuterXml);
            file.Close();
            file.Dispose();


        }
    }
}

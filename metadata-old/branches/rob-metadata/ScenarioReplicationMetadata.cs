using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Xml;

namespace Landis.Library.Metadata
{
    public class ScenarioReplicationMetadata: IMetadata
    { 
        public string FolderName {get; set;}
        public int TimeMin {get; set;}
        public int TimeMax {get; set;}
        //public int TimeInterval {get; set;} 
        public float RasterOutCellArea {get; set;}
        public string ProjectionFilePath {get; set;}  // Need to fix LSML first.

        public XmlNode Get_XmlNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement("scenario-replication");

            //XmlAttribute folderAtt = doc.CreateAttribute("folderName");
            //folderAtt.Value = this.FolderName;
            //node.Attributes.Append(folderAtt);

            XmlAttribute timeMinAtt = doc.CreateAttribute("timeMin");
            timeMinAtt.Value = this.TimeMin.ToString();
            node.Attributes.Append(timeMinAtt);

            XmlAttribute timeMaxAtt = doc.CreateAttribute("timeMax");
            timeMaxAtt.Value = this.TimeMax.ToString();
            node.Attributes.Append(timeMaxAtt);

            //XmlAttribute timeIntervalAtt = doc.CreateAttribute("timeInterval");
            //timeIntervalAtt.Value = this.TimeInterval.ToString();
            //node.Attributes.Append(timeIntervalAtt);

            XmlAttribute rasterOutCellSizeAtt = doc.CreateAttribute("rasterOutCellArea");
            rasterOutCellSizeAtt.Value = this.RasterOutCellArea.ToString();
            node.Attributes.Append(rasterOutCellSizeAtt);

            //XmlAttribute speciesFileAtt = doc.CreateAttribute("projectionFilePath");
            //speciesFileAtt.Value = this.ProjectionFilePath;
            //node.Attributes.Append(speciesFileAtt);

            //XmlAttribute MetadataFileAtt = doc.CreateAttribute("MetadataFile");
            //MetadataFileAtt.Value = this.MetadataFile;
            //node.Attributes.Append(MetadataFileAtt);

            //node.AppendChild(Get_Fields_XmlNode(doc));

            return node;

            
        }
    }
}

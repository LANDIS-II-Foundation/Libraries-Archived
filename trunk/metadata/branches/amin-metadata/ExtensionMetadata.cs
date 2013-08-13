﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Xml;

namespace Landis.Library.Metadata
{
    public class ExtensionMetadata: IMetadata
    {
        public string Name { get; set; }
        public int TimeInterval { get; set; }  
        public ScenarioReplicationMetadata ScenarioReplicationMetadata { get; set; }
        public List<OutputMetadata> OutputMetadatas { get; set; }
                   
        
        public ExtensionMetadata()
        {
            ScenarioReplicationMetadata = new ScenarioReplicationMetadata();
            OutputMetadatas = new List<OutputMetadata>();

        }

        public XmlNode Get_XmlNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement("extension");

            XmlAttribute nameAtt = doc.CreateAttribute("name");
            nameAtt.Value = this.Name;
            node.Attributes.Append(nameAtt);

            XmlAttribute timeIntervalAtt = doc.CreateAttribute("timeInterval");
            timeIntervalAtt.Value = this.TimeInterval.ToString();
            node.Attributes.Append(timeIntervalAtt);

            //XmlNode srNode = doc.CreateElement("scenario-replication");
            XmlNode outsColl = doc.CreateElement("outputs");
            foreach (OutputMetadata om in OutputMetadatas)
                outsColl.AppendChild(om.Get_XmlNode(doc));
            node.AppendChild(ScenarioReplicationMetadata.Get_XmlNode(doc));
            node.AppendChild(outsColl);
            return node;
        }

       
    }
}

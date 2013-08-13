﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Xml;

namespace Landis.Library.Metadata
{
    public class OutputMetadata: IMetadata
    { 
        public OutputType Type{get; set;}
        public string Name { get; set; } // e.g."Century Succession monthly by Ecoregion" 
        public string FilePath { get; set; } // e.g. "Century-sucession-monthly-log.csv" 
        public string MetadataFilePath { get; set; }
        public List<FieldMetadata> Fields { get; set; }

        public MapDataType? Map_DataType { get; set; } //set null for table type
        public string Map_Unit { get; set; } //set null for table type


        public OutputMetadata()
        {
            Fields = new List<FieldMetadata>();
        }



        public void RetriveFields(Type dataObjectType)
        {
            //var dataObject = Activator.CreateInstance<T>();
            var tpDataObject = dataObjectType;// dataObject.GetType();
            Fields.Clear();
            foreach (var property in tpDataObject.GetProperties())
            {
                var attributes = property.GetCustomAttributes(typeof(DataFieldAttribute), true);
                if (attributes.Length > 0)
                {
                    if (null != attributes)
                    {
                        if (property.CanRead)
                        {
                            Fields.Add(new FieldMetadata { Name = property.Name, Unit = ((DataFieldAttribute)attributes[0]).Unit, Desc = ((DataFieldAttribute)attributes[0]).Desc });
                        }
                    }
                }
                else 
                {
                    throw new ApplicationException("Error in OutputMetadata Retriving Fields: No DataFieldAttribute coud be found in th provide object type.");
                }
            }
        }


        public XmlNode Get_XmlNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement("Output");
            
            XmlAttribute typeAtt = doc.CreateAttribute("type");
            typeAtt.Value = this.Type.ToString();
            node.Attributes.Append(typeAtt);

            XmlAttribute nameAtt = doc.CreateAttribute("name");
            nameAtt.Value = this.Name;
            node.Attributes.Append(nameAtt);

            XmlAttribute fileAtt = doc.CreateAttribute("file");
            fileAtt.Value = this.FilePath;
            node.Attributes.Append(fileAtt);

            if(this.Type == OutputType.Table)
            {
                XmlAttribute MetadataFileAtt = doc.CreateAttribute("MetadataFile");
                MetadataFileAtt.Value = this.MetadataFilePath;
                node.Attributes.Append(MetadataFileAtt);
            }
            else if (this.Type == OutputType.Map)
            {
                XmlAttribute map_initAtt = doc.CreateAttribute("unit");
                map_initAtt.Value = this.MetadataFilePath;
                node.Attributes.Append(map_initAtt);

                XmlAttribute map_DataTypeAtt = doc.CreateAttribute("dataType");
                map_DataTypeAtt.Value = this.Map_DataType.ToString();
                node.Attributes.Append(map_DataTypeAtt);
            }
           
            //It is commented so it does not append fiels to the extension xml node
            //node.AppendChild(Get_Fields_XmlNode(doc));

            return node;
            
            //XmlElement element = new XmlElement();
            //element.Attributes.Append(new XmlAttribute());
        }

        public XmlNode Get_Fields_XmlNode(XmlDocument doc)
        {
            XmlNode nodeColl = doc.CreateElement("fields");

            foreach (FieldMetadata fld in Fields)
            {
                nodeColl.AppendChild(fld.Get_XmlNode(doc));
            }

            return nodeColl;
        }
    }
}


//using (var stringwriter = new stringwriter())
//using (var xmltextwriter = xmlwriter.create(stringwriter))
//{
//    xmldoc.writeto(xmltextwriter);
//    xmltextwriter.flush();
//    return stringwriter.getstringbuilder().tostring();
//}
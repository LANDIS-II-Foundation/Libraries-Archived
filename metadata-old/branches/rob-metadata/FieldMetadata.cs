using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Xml;

namespace Landis.Library.Metadata
{
    public class FieldMetadata: IMetadata
    {
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Unit { get; set; }
        public string Format { get; set; }

        public XmlNode Get_XmlNode(XmlDocument doc)
        {
            XmlNode node = doc.CreateElement("field");

            XmlAttribute typeAtt = doc.CreateAttribute("name");
            typeAtt.Value = this.Name;
            node.Attributes.Append(typeAtt);

            XmlAttribute discAtt = doc.CreateAttribute("description");
            discAtt.Value = this.Desc;
            node.Attributes.Append(discAtt);

            if (this.Unit != null)
            {
                XmlAttribute unitAtt = doc.CreateAttribute("unit");
                unitAtt.Value = this.Unit;
                node.Attributes.Append(unitAtt);
            }

            if (this.Format != null)
            {
                XmlAttribute formatAt = doc.CreateAttribute("format");
                formatAt.Value = this.Format;
                node.Attributes.Append(formatAt);
            }

            return node;
        }
    }
}

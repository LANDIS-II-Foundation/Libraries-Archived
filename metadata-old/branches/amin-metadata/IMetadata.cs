using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;
using System.Xml;

namespace Landis.Library.Metadata
{
    public interface IMetadata
    {
        XmlNode Get_XmlNode(XmlDocument doc);

    }
}

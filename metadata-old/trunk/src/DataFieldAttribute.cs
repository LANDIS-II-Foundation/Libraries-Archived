using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.Threading.Tasks;

namespace Landis.Library.Metadata
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataFieldAttribute : Attribute 
    { 
        public string Unit; 
        public string Desc;
        public string Format;
        public bool SppList;
        public bool ColumnList;
    }

}

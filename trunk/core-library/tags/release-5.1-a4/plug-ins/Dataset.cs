using System.Collections.Generic;

namespace Landis.PlugIns
{
    /// <summary>
    /// A collection of information about installed plug-ins.
    /// </summary>
    public class Dataset
        : IDataset
    {
        private string path;
        private List<PlugInInfo> plugIns;

        //---------------------------------------------------------------------

        public Dataset(string path)
        {
            this.path = path;
            PersistentDataset dataset = PersistentDataset.Load(path);

            plugIns = new List<PlugInInfo>();
            foreach (PersistentDataset.PlugInInfo info in dataset.PlugIns) {
                plugIns.Add(new PlugInInfo(info.Name,
                                           new PlugInType(info.TypeName),
                                           info.ImplementationName));
            }
        }

        //---------------------------------------------------------------------

        public int Count
        {
            get {
                return plugIns.Count;
            }
        }

        //---------------------------------------------------------------------

        public PlugInInfo this[string name]
        {
            get {
                foreach (PlugInInfo info in plugIns)
                    if (info.Name == name)
                        return info;
                return null;
            }
        }
    }
}

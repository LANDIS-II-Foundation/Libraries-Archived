using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace Landis.Library.Metadata
{
    public class MetadataTable <T> where T: new()
    {
        private DataTable tbl = new DataTable();
        private List<T> list = new List<T>();
        private string filePath;

        public List<T> ObjectsList { get { return this.list; } }
        public string FilePath { get { return this.filePath; } }

        //------
        public MetadataTable(string filePath)
        {
            this.filePath = filePath;
            this.tbl.SetColumns<T>();
            this.tbl.WriteToFile(this.filePath, false);
        }
        //

        //------
        public void AddObject(T obj)
        {
            this.list.Add(obj);
        }
        
        //------
        public T GetObjectAt(int index)
        {
            return this.list[index];
        }

        //------
        public void RemoveObjectAt(int index)
        {
            this.list.RemoveAt(index);
        }

        //------
        public void RemoveObject(T obj)
        {
            this.list.Remove(obj);
        }

        //------
        public void WriteToFile()
        {
            this.tbl.AppendDataObjects(this.list);
            this.tbl.WriteToFile(this.filePath, true);
        }

        //------
        public void Clear()
        {
            ObjectsList.Clear();
            tbl.Clear();
        }

        

    }
}

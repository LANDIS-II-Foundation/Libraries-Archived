using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Linq;
using System.Text;
using Landis.Core;
//using System.Threading.Tasks;

namespace Landis.Library.Metadata
{
    public static class DataTableExtensions //<T> where T : new()
    {
        private static System.IO.StreamWriter file;

        
        /// <summary>
        /// Set DataFieldAttributes of the given Type to the columns of the DataTable.
        /// This should be called befor adding data to the data table.
        /// </summary>
        /// <typeparam name="T"></typeparam>ok
        /// 
        /// <param name="tbl"></param>
        public static void SetColumns<T>(this DataTable tbl) where T : new()
        { 
            var dataObject = Activator.CreateInstance<T>();
            var tpDataObject = dataObject.GetType();
            tbl.Rows.Clear();
            tbl.Columns.Clear();

            foreach (var property in tpDataObject.GetProperties())
            {
                var attributes = property.GetCustomAttributes(typeof(DataFieldAttribute), true);
                if (null != attributes && attributes.Length > 0)
                {
                    if (property.CanRead)
                    {
                        bool sppString = ((DataFieldAttribute)attributes[0]).SppList;
                        bool columnList = ((DataFieldAttribute)attributes[0]).ColumnList;
                        if (sppString)
                        {
                            //ExtensionMetadata.ModelCore.UI.WriteLine("   Adding column headers for Species ...");
                            foreach (ISpecies species in ExtensionMetadata.ModelCore.Species)
                            {
                                //ExtensionMetadata.ModelCore.UI.WriteLine("   Adding column header for {0} ...", species.Name);
                                tbl.Columns.Add(String.Format(property.Name + species.Name), typeof(double)); //property.PropertyType);
                            }
                        }
                        else if (columnList)
                        {
                            foreach (String columnName in ExtensionMetadata.ColumnNames)// int i = 0; i < ExtensionMetadata.ColumnNames.Length; i++)
                            {
                                //if (ExtensionMetadata.ColumnNames[i].Trim() == "")
                                //    break;
                                //tbl.Columns.Add(String.Format(property.Name + ExtensionMetadata.ColumnNames(i)), typeof(double)); //property.PropertyType);
                                tbl.Columns.Add(String.Format(property.Name + columnName), typeof(double)); //property.PropertyType);
                            }
                        }
                        else
                        {
                            tbl.Columns.Add(property.Name, property.PropertyType);
                        }
                    }
                }
            }
        }

        //------
        public static T GetDataObjectAt<T>(this DataTable tbl, int index) where T : new()
        {
            return GetDataObject<T>(tbl.Rows[index]);
        }

        //------
        public static T GetDataObject<T>(this DataRow dataRow) where T: new()
        {
            var dataObject = Activator.CreateInstance<T>();
            var tpDataObject = dataObject.GetType();

            foreach (var property in tpDataObject.GetProperties())
            {
                var attributes = property.GetCustomAttributes(typeof(DataFieldAttribute), true);
                if (null != attributes && attributes.Length > 0)
                {
                    if (property.CanWrite)
                    {
                        DataColumn clm = dataRow.Table.Columns[property.Name];
                        if (null != clm)
                        {
                            object value = dataRow[clm];
                            property.SetValue(dataObject, (value == DBNull.Value)?null:value, null);
                        }
                    }
                }
            }
            return dataObject;
        }

        //------
        public static DataRow GetDataRow(object dataObject, DataTable tbl)
        {
            var tpDataObject = dataObject.GetType();

            DataRow dataRow = tbl.NewRow();
            foreach (var property in tpDataObject.GetProperties())
            {
                var attributes = property.GetCustomAttributes(typeof(DataFieldAttribute), true);
                if (null != attributes && attributes.Length > 0)
                {
                    if (property.CanRead)
                    {
                        object value = property.GetValue(dataObject, null);
                        DataColumn clm = tbl.Columns.Add(property.Name, property.PropertyType);
                        dataRow[clm] = value;
                    }
                }
            }
            return dataRow;
        }

        //------
        public static void AppendDataObjects(this DataTable tbl, IEnumerable dataObjects)
        {
            foreach (object obj in dataObjects)
            {
                AddDataObject(tbl, obj);
            }
            tbl.AcceptChanges();
            //return tbl;
        }

        ////------
        //public static void SaveDataObjectsIntoTable(this DataTable tbl, IEnumerable dataObjects)
        //{
        //    tbl.Rows.Clear();
        //    AppendDataObjects(tbl, dataObjects);
        //}

        //------
        // Function adds a row of data to a data table.
        public static void AddDataObject(this DataTable tbl, object dataObject)
        {
            if (tbl.Columns.Count == 0)
                throw new ApplicationException("Error in adding DataObject/s into the table: No culomn has been defined in the table. Call SetColumns() function befor adding DataObject to the table.");

            var tpDataObject = dataObject.GetType();

            DataRow dataRow = tbl.NewRow();
            foreach (var property in tpDataObject.GetProperties())
            {
                var attributes = property.GetCustomAttributes(typeof(DataFieldAttribute), true);
                if (null != attributes && attributes.Length > 0)
                {
                    if (property.CanRead)
                    {
                        bool sppString = ((DataFieldAttribute)attributes[0]).SppList;
                        bool columnList = ((DataFieldAttribute)attributes[0]).ColumnList;
                        if (sppString)
                        {

                            double[] sppValue = (double[])property.GetValue(dataObject, null);
                            foreach (ISpecies species in ExtensionMetadata.ModelCore.Species)
                            {
                                DataColumn clm = tbl.Columns[(property.Name + species.Name)];
                                string format = ((DataFieldAttribute)attributes[0]).Format;
                                dataRow[clm] = format == null ? sppValue[species.Index].ToString() : string.Format("{0:" + format + "}", sppValue[species.Index].ToString());
                            }
                        }
                        else if (columnList)
                        {
                            double[] columnValue = (double[])property.GetValue(dataObject, null);
                            int i = 0;
                            foreach (String columnName in ExtensionMetadata.ColumnNames)
                                //for (int i = 0; i < ExtensionMetadata.ColumnNames.Length; i++)
                            {
                                //DataColumn clm = tbl.Columns[(property.Name + ExtensionMetadata.ColumnNames[i])];
                                DataColumn clm = tbl.Columns[(property.Name + columnName)];
                                string format = ((DataFieldAttribute)attributes[0]).Format;
                                dataRow[clm] = format == null ? columnValue[i].ToString() : string.Format("{0:" + format + "}", columnValue[i].ToString());
                                i++;
                            }
                        }
                        else
                        {
                            object value = property.GetValue(dataObject, null);
                            DataColumn clm = tbl.Columns[property.Name];//, property.PropertyType);
                            string format = ((DataFieldAttribute)attributes[0]).Format;
                            dataRow[clm] = format == null ? value : string.Format("{0:" + format + "}", value);
                        }
                    }
                }
            }
            tbl.Rows.Add(dataRow);
            tbl.AcceptChanges();

        }

        //------
        public static void WriteToFile(this DataTable tbl, string filePath, bool append)
        {

            //System.IO.StreamWriter file;
  

            StringBuilder strb = new StringBuilder();
            if (!append)
            {
                try
                {
                    file = new System.IO.StreamWriter(filePath, append);
                    //file = Landis.Data.CreateTextFile(filePath);
                }
                catch (Exception err)
                {
                    string mesg = string.Format("{0}", err.Message);
                    throw new System.ApplicationException(mesg);
                }
                file.AutoFlush = true;

                
                foreach (DataColumn col in tbl.Columns)
                {
                    strb.AppendFormat("{0},", col.ColumnName);
                }
                file.WriteLine(strb.ToString());
                //file.Close();
                //file.Dispose();
            }
            else
            {
                //file = Landis.Data.OpenTextFile(filePath);
                file = new System.IO.StreamWriter(filePath, append);
                foreach (DataRow dr in tbl.Rows)
                {
                    strb = new StringBuilder();
                    foreach (DataColumn col in tbl.Columns)
                    {
                        strb.AppendFormat("{0}, ", dr[col].ToString());
                    }
                    file.WriteLine(strb.ToString());
                }
            }
            file.Close();
            file.Dispose();
        }


        //------




        //public DataTable GetDataTable(this IEnumerable<object> dataObjects)
        //{
        //    DataTable tbl = new DataTable();
        //    foreach (object obj in dataObjects)
        //    {
        //        tbl.Rows.Add(GetDataRow(obj));
        //    }
        //    tbl.AcceptChanges();
        //    return tbl;
        //}

        //------
        //public DataTable GetDataTable(this IEnumerable<object> dataObjects)
        //{
        //    DataTable tbl = new DataTable();
        //    foreach (object obj in dataObjects)
        //    {
        //        tbl.Rows.Add(this.GetDataRow(obj));
        //    }
        //    tbl.AcceptChanges();
        //    return tbl;
        //}

        //------
        //public List<FieldMetadata> GetFieldMetadatas()
        //{
        //    return this.fieldMetadatas;
        //}



        //public static DataTable GetDataTable(this object dataObject)
        //{
        //    var tpDataObject = dataObject.GetType();

        //    DataTable tbl = new DataTable();
        //    DataRow dataRow = tbl.NewRow();
        //    foreach (var property in tpDataObject.GetProperties())
        //    {
        //        var attributes = property.GetCustomAttributes(typeof(DataFieldAttribute), true);
        //        if (null != attributes && attributes.Length > 0)
        //        {
        //            if (property.CanRead)
        //            {
        //                object value = property.GetValue(dataObject, null);
        //                DataColumn clm = tbl.Columns.Add(property.Name, property.PropertyType);
        //                dataRow[clm] = value;
        //            }
        //        }
        //    }

        //    tbl.Rows.Add(dataRow);
        //    tbl.AcceptChanges();
        //    return tbl;
        //}

    }

}






/*


public static class DataObjectExtensions {
 * 
	public static T ToDataObject<T>( this DataRow dataRow ) where T : new() {
		var dataObject = Activator.CreateInstance<T>();
		var tpDataObject = dataObject.GetType();

		foreach ( var property in tpDataObject.GetProperties() ) {
			var attributes = property.GetCustomAttributes( typeof( DataColumnAttribute ), true );
			if ( null != attributes && attributes.Length > 0 ) {
				if ( property.CanWrite ) {
					DataColumn clm = dataRow.Table.Columns[property.Name];
					if ( null != clm ) {
						object value = dataRow[clm];
						property.SetValue( dataObject, value, null );
					}
				}
			}
		}

		return dataObject;
	}
	public static DataTable ToDataTable( this object dataObject ) {
		var tpDataObject = dataObject.GetType();

		DataTable tbl = new DataTable();
		DataRow dataRow = tbl.NewRow();
		foreach ( var property in tpDataObject.GetProperties() ) {
			var attributes = property.GetCustomAttributes( typeof( DataColumnAttribute ), true );
			if ( null != attributes && attributes.Length> 0 ) {
				if ( property.CanRead ) {
					object value = property.GetValue( dataObject, null );
					DataColumn clm = tbl.Columns.Add( property.Name, property.PropertyType );
					dataRow[clm] = value;
				}
			}
		}

		tbl.Rows.Add( dataRow );
		tbl.AcceptChanges();
		return tbl;
	}
}
 *
 * 
*/
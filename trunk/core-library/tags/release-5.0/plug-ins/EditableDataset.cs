using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Landis.PlugIns
{
	/// <summary>
	/// An edtiable dataset of plug-ins.
	/// </summary>
	public class EditableDataset
		: IDataset
	{
		/// <summary>
		/// Creates a new empty editable dataset.
		/// </summary>
		public static EditableDataset Create()
		{
			return new EditableDataset();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Loads an editable dataset from a file.
		/// </summary>
		/// <param name="path">
		/// The path to the file containing the dataset
		/// </param>
		public static EditableDataset Load(string path)
		{
			return new EditableDataset(path);
		}

		//---------------------------------------------------------------------

		private string path;
		private List<DatasetEntry> entries;
		private Dictionary<string, List<string>> referencedLibs;

		//---------------------------------------------------------------------

		/// <summary>
		/// The path of the file from which the dataset was loaded.  If the
		/// dataset was not loaded from a file, this property is null.
		/// </summary>
		public string Path
		{
			get {
				return path;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The number of plug-ins in the dataset.
		/// </summary>
		public int Count
		{
			get {
				return entries.Count;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The plug-in at a given index in the dataset.
		/// </summary>
		/// <exception cref="System.IndexOutOfRange">
		/// index is less than 0 or equal to or greater than Count.
		/// </exception>
		public IDatasetEntry this[int index]
		{
			get {
				return entries[index];
			}
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// Initializes a new instance with no entries.
		/// </summary>
		internal EditableDataset()
		{
			this.path = null;
			this.entries = new List<DatasetEntry>();
			this.referencedLibs = new Dictionary<string, List<string>>();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance by loading plug-in entries from a file.
		/// </summary>
		internal EditableDataset(string path)
		{
			this.path = path;
			using (InputBinaryFile file = new InputBinaryFile(path, Database.BinaryFileIdentifier)) {
				this.entries = file.Deserialize<List<DatasetEntry>>();
				this.referencedLibs = file.Deserialize<Dictionary<string, List<string>>>();
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Finds a plug-in by its name.
		/// </summary>
		public IDatasetEntry Find(string name)
		{
			foreach (IDatasetEntry entry in entries) {
				if (entry.Name == name)
					return entry;
			}
			return null;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Adds a new entry to the dataset.
		/// </summary>
		/// <exception cref="System.ArgumentNullException">
		/// entry is null.
		/// </exception>
		/// <exception cref="System.ApplicationException">
		/// There is already an entry in the dataset with the same name.
		/// </exception>
		public void Add(IDatasetEntry entry)
		{
			Require.ArgumentNotNull(entry);
			IDatasetEntry foundEntry = Find(entry.Name);
			if (foundEntry != null) {
				throw new System.ApplicationException(string.Format("The plug-in dataset already has an entry with the name {0}", entry.Name));
			}

			entries.Add(new DatasetEntry(entry));
			foreach (string lib in entry.ReferencedAssemblies) {
				List<string> referencedBy;
				if (! referencedLibs.TryGetValue(lib, out referencedBy)) {
					referencedBy = new List<string>();
					referencedLibs[lib] = referencedBy;
				}
				referencedBy.Add(entry.AssemblyName);
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Removes the entry with the particular name.
		/// </summary>
		/// <returns>
		/// null if there is not entry with the given name.  Otherwise, the
		/// entry that was removed.
		/// </returns>
		public IDatasetEntry Remove(string name)
		{
			Require.ArgumentNotNull(name);
			for (int i = 0; i < entries.Count; ++i) {
				if (entries[i].Name == name) {
					IDatasetEntry entry = entries[i];
					entries.RemoveAt(i);

					foreach (string lib in entry.ReferencedAssemblies) {
						referencedLibs[lib].Remove(entry.AssemblyName);
						if (referencedLibs[lib].Count == 0)
							referencedLibs.Remove(lib);
					}

					return entry;
				}
			}
			return null;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Determines if a library is referenced by any of the entries
		/// </summary>
		public bool ReferencedByEntries(string library)
		{
			Require.ArgumentNotNull(library);
			return referencedLibs.ContainsKey(library);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Determines which libraries in a given list are referenced by the
		/// entries.
		/// </summary>
		public IList<string> ReferencedByEntries(IList<string> libs)
		{
			List<string> libsReferencedByEntries = new List<string>();
			foreach (string lib in libs) {
				if (referencedLibs.ContainsKey(lib))
					libsReferencedByEntries.Add(lib);
			}
			return libsReferencedByEntries;
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// Saves the dataset's entries to the file from which the dataset
		/// was loaded.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">
		/// The dataset was not loaded from a file; rather it was created
		/// dynamically.  To save such a dataset, use the SaveAs method.
		/// </exception>
		public void Save()
		{
			if (path == null)
				throw new System.InvalidOperationException("Dataset was not loaded from a file, so it has no path");
			SaveAs(path);
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// Saves the dataset's entries to a file.
		/// </summary>
		public void SaveAs(string path)
		{
			Require.ArgumentNotNull(path);
			using (OutputBinaryFile file = new OutputBinaryFile(path, Database.BinaryFileIdentifier)) {
				file.Serialize(entries);
				file.Serialize(referencedLibs);
			}
		}
	}
}

using System.IO;

namespace Landis.PlugIns.Admin
{
	/// <summary>
	/// Methods for accessing the plug-ins dataset.
	/// </summary>
	public static class Dataset
	{
		private static string path;
		private static bool datasetExists;

		//---------------------------------------------------------------------

		static Dataset()
		{
			path = Database.DefaultPath.Replace("plug-ins_database.txt", "extensions.landis");
			datasetExists = File.Exists(path);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Loads the plug-ins dataset if its file exists.
		/// </summary>
		/// <returns>
		/// null if the dataset file does not exist.
		/// </returns>
		public static EditableDataset LoadIfExists()
		{
			if (datasetExists)
				return EditableDataset.Load(path);
			else
				return null;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Loads the plug-ins dataset if its file exists; otherwise it creates
		/// an empty dataset.
		/// </summary>
		/// <returns>
		/// null if the dataset file does not exist.
		/// </returns>
		public static EditableDataset LoadOrCreate()
		{
			if (! datasetExists) {
				//	Create an empty dataset.
				EditableDataset.Create().SaveAs(path);
			}
			return EditableDataset.Load(path);
		}
	}
}

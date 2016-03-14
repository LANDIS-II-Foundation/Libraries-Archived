namespace Landis.PlugIns
{
	/// <summary>
	/// A dataset of plug-ins.
	/// </summary>
	public interface IDataset
	{
		/// <summary>
		/// The path of the file from which the dataset was loaded.  If the
		/// dataset was not loaded from a file, this property is null.
		/// </summary>
		string Path
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The number of plug-ins in the dataset.
		/// </summary>
		int Count
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The plug-in at a given index in the dataset.
		/// </summary>
		/// <exception cref="System.IndexOutOfRange">
		/// index is less than 0 or equal to or greater than Count.
		/// </exception>
		IDatasetEntry this[int index]
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Finds a plug-in by its name.
		/// </summary>
		IDatasetEntry Find(string name);
	}
}

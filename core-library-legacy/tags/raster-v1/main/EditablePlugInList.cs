using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis
{
	/// <summary>
	/// Editable list of information about a group of plug-ins.
	/// </summary>
	/// <remarks>
	/// The group may be empty, i.e., contain no plug-ins.
	///
	/// The IsComplete property is true only if the IsComplete property is true
	/// for each of the plug-ins in the list.
	/// </remarks>
	public class EditablePlugInList<T>
		: IEditablePlugInList<T>
		where T : Edu.Wisc.Forest.Flel.Util.PlugIns.IPlugIn
	{
		private List<IEditablePlugIn<T>> plugIns;

		//---------------------------------------------------------------------

		/// <summary>
		/// The number of plug-ins in the list.
		/// </summary>
		public int Count
		{
			get {
				return plugIns.Count;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Accesses a plug-in by its index in the list.
		/// </summary>
		public IEditablePlugIn<T> this[int index]
		{
			get {
				return plugIns[index];
			}
		}

		//---------------------------------------------------------------------

		public EditablePlugInList()
		{
			plugIns = new List<IEditablePlugIn<T>>();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Inserts a plug-in into the list.
		/// </summary>
		/// <param name="index">
		/// Index where to insert the new plug-in.  Valid values are between 0
		/// and Count.  Inserting at index Count adds the new plug-in to the
		/// end of the list.
		/// </param>
		/// <exception cref="System.IndexOutOfRangeException">
		/// index is less than 0 or greater than Count.
		/// </exception>
		/// <exception cref="System.ArgumentNullException">
		/// plugIn is null.
		/// </exception>
		public void InsertAt(int                index,
		                     IEditablePlugIn<T> plugIn)
		{
			if (index < 0 || index > Count)
				throw new System.IndexOutOfRangeException();
			if (plugIn == null)
				throw new System.ArgumentNullException();
			if (index == Count)
				plugIns.Add(plugIn);
			else
				plugIns[index] = plugIn;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Removes a plug-in from the list.
		/// </summary>
		/// <exception cref="System.IndexOutOfRangeException">
		/// index is less than 0 or equal to or greater than Count.
		/// </exception>
		public void RemoveAt(int index)
		{
			if (index < 0 || index >= Count)
				throw new System.IndexOutOfRangeException();
			plugIns.RemoveAt(index);
		}

		//---------------------------------------------------------------------

		public bool IsComplete
		{
			get {
				foreach (IEditablePlugIn<T> plugIn in plugIns)
					if (! plugIn.IsComplete)
						return false;
				return true;
			}
		}

		//---------------------------------------------------------------------

		public IPlugIn[] GetComplete()
		{
			if (IsComplete) {
				IPlugIn[] completePlugIns = new IPlugIn[plugIns.Count];
				foreach (int index in Indexes.Of(plugIns))
					completePlugIns[index] = plugIns[index].GetComplete();
				return completePlugIns;
			}
			else
				return null;
		}
	}
}

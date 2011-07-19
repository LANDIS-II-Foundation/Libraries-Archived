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
	public interface IEditablePlugInList<T>
		: IEditable< IPlugIn[] >
		where T : Edu.Wisc.Forest.Flel.Util.PlugIns.IPlugIn
	{
		/// <summary>
		/// The number of plug-ins in the list.
		/// </summary>
		int Count
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Accesses a plug-in by its index in the list.
		/// </summary>
		IEditablePlugIn<T> this[int index]
		{
			get;
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
		void InsertAt(int                index,
		              IEditablePlugIn<T> plugIn);

		//---------------------------------------------------------------------

		/// <summary>
		/// Removes a plug-in from the list.
		/// </summary>
		/// <exception cref="System.IndexOutOfRangeException">
		/// index is less than 0 or equal to or greater than Count.
		/// </exception>
		void RemoveAt(int index);
	}
}

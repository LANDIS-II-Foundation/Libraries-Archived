using System.Collections.Generic;

namespace Landis.Species
{
	/// <summary>
	/// Editable dataset of species parameters.
	/// </summary>
	public class EditableDataset
		: List<IEditableParameters>, IEditableDataset
	{
		/// <summary>
		/// Is each set of parameters in the dataset complete?
		/// </summary>
		public bool IsComplete
		{
			get {
				foreach (IEditableParameters parameters in this)
					if (! parameters.IsComplete)
						return false;
				return true;
			}
		}

		//---------------------------------------------------------------------

		public new IEditableParameters this[int index]
		{
			get {
				return base[index];
			}

			set {
				if (value == null)
					RemoveAt(index);
				else
					base[index] = value;
			}
		}

		//---------------------------------------------------------------------

		public IEditableParameters this[string name]
		{
			get {
				int index = IndexOf(name);
				if (index >= 0)
					return this[index];
				else
					return null;
			}

			set {
				int index = IndexOf(name);
				if (index >= 0) {
					if (value != null)
						this[index] = value;
					else
						RemoveAt(index);
				}
				else {
					if (value != null)
						Add(value);
				}
			}
		}

		//---------------------------------------------------------------------

		public EditableDataset()
		{
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Gets the index of a species parameters in the dataset.
		/// </summary>
		/// <returns>
		/// -1 if the species is not in the dataset.
		/// </returns>
		public int IndexOf(string name)
		{
			for (int index = 0; index < Count; ++index)
				if (this[index].Name == name)
					return index;
			return -1;
		}

		//---------------------------------------------------------------------

		public IDataset GetComplete()
		{
			if (IsComplete) {
				List<ISpecies> species = new List<ISpecies>(Count);
				for (int index = 0; index < Count; ++index) {
					species.Add(new Species(index, this[index].GetComplete()));
				}
				return new Dataset(species);
			}
			else
				return null;
		}
	}
}

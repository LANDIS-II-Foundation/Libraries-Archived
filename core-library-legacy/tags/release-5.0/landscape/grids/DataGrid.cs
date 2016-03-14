using System.Collections;
using System.Collections.Generic;

namespace Landis.Landscape
{
	public class DataGrid<T>
		: GridWithType, IIndexableGrid<T>, IEnumerableGrid<T>
	{
		private T[,] data;

		//---------------------------------------------------------------------

		public DataGrid(GridDimensions dimensions)
			: base(dimensions, typeof(T))
		{
			if (Count > 0) {
				this.data = new T[dimensions.Rows, dimensions.Columns];
			}
		}

		//---------------------------------------------------------------------

		public DataGrid(uint rows,
		                uint columns)
			: base(rows, columns, typeof(T))
		{
			if (Count > 0) {
				this.data = new T[rows, columns];
			}
		}

		//---------------------------------------------------------------------

		public DataGrid(T[,] data)
			: base((uint)data.GetLength(0), (uint)data.GetLength(1),
			       typeof(T))
		{
			if (Count > 0) {
				this.data = (T[,]) data.Clone();
			}
		}

		//---------------------------------------------------------------------

		public T this [uint row,
		               uint column]
		{
			get {
				return this[new Location(row, column)];
			}
			set {
				this[new Location(row, column)] = value;
			}
		}

		//---------------------------------------------------------------------

		public T this [Location location]
		{
			get {
				MustBeValid(location);
				return data[location.Row-1, location.Column-1];
			}
			set {
				MustBeValid(location);
				data[location.Row-1, location.Column-1] = value;
			}
		}

		//---------------------------------------------------------------------

		private void MustBeValid(Location location)
		{
			if (location.Row < 1 || location.Row > Rows
			    || location.Column < 1 || location.Column > Columns)
				throw new System.IndexOutOfRangeException();
		}

		//---------------------------------------------------------------------

		public IEnumerator<T> GetEnumerator()
		{
			for (uint row = 0; row < Rows; ++row) {
				for (uint column = 0; column < Columns; ++column) {
					yield return data[row, column];
				}
			}
		}

		//---------------------------------------------------------------------

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}

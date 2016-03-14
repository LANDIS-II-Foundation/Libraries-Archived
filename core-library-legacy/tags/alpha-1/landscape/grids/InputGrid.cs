using System.Collections.Generic;

namespace Landis.Landscape
{
	/// <summary>
	/// An input grid with data values.
	/// </summary>
	public class InputGrid<T>
		: GridWithType, IInputGrid<T>
	{
		private IIndexableGrid<T> data;
		private Location currentLocation;
		private bool disposed = false;

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance using an indexable data grid.
		/// </summary>
		public InputGrid(IIndexableGrid<T> dataGrid)
			: base(dataGrid.Dimensions, typeof(T))
		{
			this.data = dataGrid;
			//  Initialize current location such that RowMajor.Next will return
			//  the upper left location (1,1).
			this.currentLocation = new Location(1, 0);
		}

		//---------------------------------------------------------------------

		public T ReadValue()
		{
			if (disposed)
				throw new System.InvalidOperationException("Object has been disposed.");
			currentLocation = RowMajor.Next(currentLocation, Columns);
			if (currentLocation.Row > Rows)
				throw new System.IO.EndOfStreamException();
			return data[currentLocation];
		}

		//---------------------------------------------------------------------

		public void Close()
		{
			Dispose();
		}

		//---------------------------------------------------------------------

		public void Dispose()
		{
			disposed = true;
		}
	}
}

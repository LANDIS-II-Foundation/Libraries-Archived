namespace Landis.Landscape
{
	/// <summary>
	/// An input grid of data values that are read from the upper-left corner
	/// to the lower-right corner in row-major order.
	/// </summary>
	public interface IInputGrid<T>
		: IGrid, System.IDisposable
	{
		/// <summary>
		/// Reads the next data value from the grid.
		/// </summary>
		/// <exception cref="System.IO.EndOfStreamException">
		/// Thrown when there are no more data values left to read.
		/// </exception>
		T ReadValue();

		//---------------------------------------------------------------------

		/// <summary>
		/// Closes the input grid.
		/// </summary>
		void Close();
	}
}

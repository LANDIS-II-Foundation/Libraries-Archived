namespace Landis.Landscape
{
	/// <summary>
	/// Methods for traversing locations on a landscape in row-major order.
	/// </summary>
	public static class RowMajor
	{
        //!<  \brief  Get the next location in row-major order.
    	public static Location Next(Location location,
                  					uint     columns)
		{
			if (location.Column < columns) {
				return new Location(location.Row, location.Column + 1);
			}
			else {
				return new Location(location.Row + 1, 1);
			}
		}
	}
}

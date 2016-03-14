namespace Gov.Fgdc.Csdgm
{
	/// <summary>
	/// Northern-most coordinate of the limit of coverage expressed in
	/// latitude.
	/// </summary>
	/// <remarks>
	/// Section 1.5.1.3
	/// Type: real
	/// Domain: -90.0 <= North Bounding Coordinate <= 90.0;
	/// North Bounding Coordinate >= South Bounding Coordinate
	/// Short Name: northbc
	/// </remarks>
	public static class NorthBoundingCoordinate
	{
		public const string Name = "North Bounding Coordinate";
	}
}

namespace Landis.Landscape
{
	/// <summary>
	/// Represents a variable with values for the sites on a landscape.
	/// </summary>
	public interface ISiteVariable
	{
		/// <summary>
		/// The data type of the values.
		/// </summary>
		System.Type DataType
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Indicates whether the inactive sites share a common value or have
		/// distinct values.
		/// </summary>
		InactiveSiteMode Mode
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The landscape that the site variable was created for.
		/// </summary>
		ILandscape Landscape
		{
			get;
		}
	}
}

namespace Landis.Landscape
{
	/// <summary>
	/// Represents a variable with values for the sites on a landscape.
	/// </summary>
	public interface ISiteVar<T>
		: ISiteVariable
	{
		/// <summary>
		/// Gets or sets the value for a particular site.
		/// </summary>
		T this[Site site]
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Sets all the values for the active sites to the same value.
		/// </summary>
		T ActiveSiteValues
		{
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Sets all the values for the inactive sites to the same value.
		/// </summary>
		T InactiveSiteValues
		{
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Sets all the values for both active and inactive sites to the same
		/// value.
		/// </summary>
		T SiteValues
		{
			set;
		}
	}
}

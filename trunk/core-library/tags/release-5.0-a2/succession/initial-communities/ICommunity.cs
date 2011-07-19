namespace Landis.InitialCommunities
{
	/// <summary>
	/// An initial community.
	/// </summary>
	public interface ICommunity
	{
		/// <summary>
		/// The code that represents the community on maps.
		/// </summary>
		byte MapCode
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The site cohorts in the community.
		/// </summary>
		ISiteCohorts<AgeOnly.ICohort> Cohorts
		{
			get;
		}
	}
}

namespace Landis.SiteInitialization
{
	public interface IClass
	{
		/// <summary>
		/// The class' id number.
		/// </summary>
		int IdNumber
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The initial cohorts at the sites in the class.
		/// </summary>
		ISiteCohorts SiteCohorts
		{
			get;
		}
	}
}

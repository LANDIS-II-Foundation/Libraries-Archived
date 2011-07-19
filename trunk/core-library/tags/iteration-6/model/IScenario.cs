namespace Landis
{
	/// <summary>
	/// A model scenario.
	/// </summary>
	public interface IScenario
	{
		/// <summary>
		/// The input files with initial site classes.
		/// </summary>
		SiteInitialization.InputFiles InitialSiteClasses
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The succession plug-in to use for the scenario.
		/// </summary>
		PlugIn.Info Succession
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The disturbance plug-ins to use for the scenario.
		/// </summary>
		PlugIn.Info[] Disturbances
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Are disturbance run is random order?
		/// </summary>
		bool DisturbancesRandomOrder
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// How long the scenario runs (years).
		/// </summary>
		int Duration
		{
			get;
		}
	}
}

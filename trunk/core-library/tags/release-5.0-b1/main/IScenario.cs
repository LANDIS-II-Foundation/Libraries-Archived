namespace Landis
{
	/// <summary>
	/// A model scenario.
	/// </summary>
	public interface IScenario
	{
		/// <summary>
		/// How long the scenario runs (years).
		/// </summary>
		int Duration
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the file with species parameters.
		/// </summary>
		string Species
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the file with ecoregion definitions.
		/// </summary>
		string Ecoregions
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the raster file showing where the ecoregions are.
		/// </summary>
		string EcoregionsMap
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The length of a cell's side (meters).  Optional; used only if
		/// ecoregion map does not specify cell length in its metadata.
		/// </summary>
		float? CellLength
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the file with the initial communities' definitions.
		/// </summary>
		string InitialCommunities
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the raster file showing where the initial communities are.
		/// </summary>
		string InitialCommunitiesMap
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The succession plug-in to use for the scenario.
		/// </summary>
		IPlugIn Succession
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The disturbance plug-ins to use for the scenario.
		/// </summary>
		IPlugIn[] Disturbances
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
		/// The output plug-ins to use for the scenario.
		/// </summary>
		IPlugIn[] Outputs
		{
			get;
		}
	}
}

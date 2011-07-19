using Edu.Wisc.Forest.Flel.Util;

namespace Landis
{
	/// <summary>
	/// An editable model scenario.
	/// </summary>
	public interface IEditableScenario
		: IEditable<IScenario>
	{
		/// <summary>
		/// How long the scenario runs (years).
		/// </summary>
		InputValue<int> Duration
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the file with species parameters.
		/// </summary>
		InputValue<string> Species
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the file with ecoregion definitions.
		/// </summary>
		InputValue<string> Ecoregions
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the raster file showing where the ecoregions are.
		/// </summary>
		InputValue<string> EcoregionsMap
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The length of a cell's side (meters).  Optional; used only if
		/// ecoregion map does not specify cell length in its metadata.
		/// </summary>
		InputValue<float> CellLength
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the file with the initial communities' definitions.
		/// </summary>
		InputValue<string> InitialCommunities
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the raster file showing where the initial communities are.
		/// </summary>
		InputValue<string> InitialCommunitiesMap
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The succession plug-in to use for the scenario.
		/// </summary>
		IEditablePlugIn<PlugIns.ISuccession> Succession
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The disturbance plug-ins to use for the scenario.
		/// </summary>
		IEditablePlugInList<PlugIns.IDisturbance> Disturbances
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Are disturbance run is random order?
		/// </summary>
		InputValue<bool> DisturbancesRandomOrder
		{
			get;
			set;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The output plug-ins to use for the scenario.
		/// </summary>
		IEditablePlugInList<PlugIns.IOutput> Outputs
		{
			get;
		}
	}
}

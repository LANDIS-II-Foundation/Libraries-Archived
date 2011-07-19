namespace Landis
{
	/// <summary>
	/// A model scenario.
	/// </summary>
	public class Scenario
		: IScenario
	{
		private int duration;
		private string species;
		private string ecoregions;
		private string ecoregionsMap;
		private float? cellLength;
		private string initCommunities;
		private string communitiesMap;
		private IPlugIn succession;
		private IPlugIn[] disturbances;
		private bool disturbRandom;
		private IPlugIn[] outputs;

		//---------------------------------------------------------------------

		/// <summary>
		/// How long the scenario runs (years).
		/// </summary>
		public int Duration
		{
			get {
				return duration;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the file with species parameters.
		/// </summary>
		public string Species
		{
			get {
				return species;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the file with ecoregion definitions.
		/// </summary>
		public string Ecoregions
		{
			get {
				return ecoregions;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the raster file showing where the ecoregions are.
		/// </summary>
		public string EcoregionsMap
		{
			get {
				return ecoregionsMap;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The length of a cell's side (meters).  Optional; used only if
		/// ecoregion map does not specify cell length in its metadata.
		/// </summary>
		public float? CellLength
		{
			get {
				return cellLength;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the file with the initial communities' definitions.
		/// </summary>
		public string InitialCommunities
		{
			get {
				return initCommunities;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the raster file showing where the initial communities are.
		/// </summary>
		public string InitialCommunitiesMap
		{
			get {
				return communitiesMap;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The succession plug-in to use for the scenario.
		/// </summary>
		public IPlugIn Succession
		{
			get {
				return succession;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The disturbance plug-ins to use for the scenario.
		/// </summary>
		public IPlugIn[] Disturbances
		{
			get {
				return disturbances;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Are disturbance run is random order?
		/// </summary>
		public bool DisturbancesRandomOrder
		{
			get {
				return disturbRandom;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The output plug-ins to use for the scenario.
		/// </summary>
		public IPlugIn[] Outputs
		{
			get {
				return outputs;
			}
		}

		//---------------------------------------------------------------------

		public Scenario(int       duration,
		                string    species,
		                string    ecoregions,
		                string    ecoregionsMap,
		                float?    cellLength,
		                string    initCommunities,
		                string    communitiesMap,
		                IPlugIn   succession,
		                IPlugIn[] disturbances,
		                bool      disturbRandom,
		                IPlugIn[] outputs)
		{
			this.duration        = duration;
			this.species         = species;
			this.ecoregions      = ecoregions;
			this.ecoregionsMap   = ecoregionsMap;
			this.cellLength      = cellLength;
			this.initCommunities = initCommunities;
			this.communitiesMap  = communitiesMap;
			this.succession      = succession;
			this.disturbances    = disturbances;
			this.disturbRandom   = disturbRandom;
			this.outputs         = outputs;
		}
	}
}

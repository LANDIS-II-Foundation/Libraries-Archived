using Edu.Wisc.Forest.Flel.Util;
using Landis.PlugIns;

namespace Landis
{
	/// <summary>
	/// An editable model scenario.
	/// </summary>
	public class EditableScenario
		: IEditableScenario
	{
		private InputValue<int> duration;
		private InputValue<string> species;
		private InputValue<string> ecoregions;
		private InputValue<string> ecoregionsMap;
		private InputValue<float> cellLength;
		private InputValue<string> initCommunities;
		private InputValue<string> communitiesMap;
		private EditablePlugIn<ISuccession> succession;
		private EditablePlugInList<IDisturbance> disturbances;
		private EditablePlugInList<IOutput> outputs;
		private InputValue<bool> disturbRandom;
		private InputValue<uint> seed;

		//---------------------------------------------------------------------

		/// <summary>
		/// How long the scenario runs (years).
		/// </summary>
		public InputValue<int> Duration
		{
			get {
				return duration;
			}

			set {
				if (value != null) {
					if (value.Actual < 0)
						throw new InputValueException(value.String,
						                              "Value must be = or > 0");
				}
				duration = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the file with species parameters.
		/// </summary>
		public InputValue<string> Species
		{
			get {
				return species;
			}

			set {
				if (value != null) {
					ValidatePath(value.Actual);
				}
				species = value;
			}
		}

		//---------------------------------------------------------------------

		private void ValidatePath(string path)
		{
			if (string.IsNullOrEmpty(path))
				throw new InputValueException();
			if (path.Trim(null).Length == 0)
				throw new InputValueException(path,
				                              "\"{0}\" is not a valid path.",
				                              path);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the file with ecoregion definitions.
		/// </summary>
		public InputValue<string> Ecoregions
		{
			get {
				return ecoregions;
			}

			set {
				if (value != null) {
					ValidatePath(value.Actual);
				}
				ecoregions = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the raster file showing where the ecoregions are.
		/// </summary>
		public InputValue<string> EcoregionsMap
		{
			get {
				return ecoregionsMap;
			}

			set {
				if (value != null) {
					ValidatePath(value.Actual);
				}
				ecoregionsMap = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The length of a cell's side (meters).  Optional; used only if
		/// ecoregion map does not specify cell length in its metadata.
		/// </summary>
		public InputValue<float> CellLength
		{
			get {
				return cellLength;
			}
			set {
				if (value != null) {
					if (value.Actual <= 0.0)
						throw new InputValueException(value.String,
						                              "Value must be > 0");
				}
				cellLength = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the file with the initial communities' definitions.
		/// </summary>
		public InputValue<string> InitialCommunities
		{
			get {
				return initCommunities;
			}

			set {
				if (value != null) {
					ValidatePath(value.Actual);
				}
				initCommunities = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Path to the raster file showing where the initial communities are.
		/// </summary>
		public InputValue<string> InitialCommunitiesMap
		{
			get {
				return communitiesMap;
			}

			set {
				if (value != null) {
					ValidatePath(value.Actual);
				}
				communitiesMap = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The succession plug-in to use for the scenario.
		/// </summary>
		public IEditablePlugIn<ISuccession> Succession
		{
			get {
				return succession;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The disturbance plug-ins to use for the scenario.
		/// </summary>
		public IEditablePlugInList<PlugIns.IDisturbance> Disturbances
		{
			get {
				return disturbances;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Are disturbance run is random order?
		/// </summary>
		public InputValue<bool> DisturbancesRandomOrder
		{
			get {
				return disturbRandom;
			}

			set {
				disturbRandom = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The output plug-ins to use for the scenario.
		/// </summary>
		public IEditablePlugInList<PlugIns.IOutput> Outputs
		{
			get {
				return outputs;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The seed for the random number generator.
		/// </summary>
		public InputValue<uint> RandomNumberSeed
		{
			get {
				return seed;
			}

			set {
				if (value != null)
					if (value.Actual == 0)
						throw new InputValueException(value.String, "Value must be > 0.");
				seed = value;
			}
		}

		//---------------------------------------------------------------------

		public EditableScenario()
		{
			this.succession = new EditablePlugIn<ISuccession>();
			this.disturbances = new EditablePlugInList<IDisturbance>();
			this.outputs = new EditablePlugInList<IOutput>();
		}

		//---------------------------------------------------------------------

		public bool IsComplete
		{
			get {
				// DisturbanceRandomOrder isn't checked because it has a
				// default value; see GetComplete() method.  Also, CellLength
				// isn't checked because it's optional.
				foreach (object inputValue in new object[]{ duration,
				         	                                species,
				         	                                ecoregions,
				         	                                ecoregionsMap,
				         	                                initCommunities,
				         	                                communitiesMap})
					if (inputValue == null)
						return false;
				if (succession.IsComplete && disturbances.IsComplete &&
				    	outputs.IsComplete && outputs.Count > 0)
					return true;
				return false;
			}
		}

		//---------------------------------------------------------------------

		public IScenario GetComplete()
		{
			if (IsComplete) {
				return new Scenario(duration.Actual,
				                    species.Actual,
				                    ecoregions.Actual,
				                    ecoregionsMap.Actual,
				                    cellLength == null ? (float?) null
				                                       : cellLength.Actual,
				                    initCommunities.Actual,
				                    communitiesMap.Actual,
				                    succession.GetComplete(),
				                    disturbances.GetComplete(),
				                    disturbRandom == null ? false
				                                          : disturbRandom.Actual,
				                    outputs.GetComplete(),
				                    seed == null ? (uint?) null
				                                 : seed.Actual);
			}
			else
				return null;
		}
	}
}

using Landis.Util;

namespace Landis.Species
{
	/// <summary>
	/// The parameters for a single tree species that can be edited.
	/// </summary>
	public class EditableParameters
		: IEditableParameters
	{
		private InputValue<string> name;
		private InputValue<int> longevity;
		private InputValue<int> maturity;
		private InputValue<byte> shadeTolerance;
		private InputValue<byte> fireTolerance;
		private InputValue<byte> windTolerance;
		private InputValue<int> effectiveSeedDist;
		private InputValue<int> maxSeedDist;
		private InputValue<float> vegReprodProb;
		private InputValue<int> maxSproutAge;

		//---------------------------------------------------------------------

		/// <summary>
		/// Name
		/// </summary>
		public InputValue<string> Name
		{
			get {
				return name;
			}

			set {
				name = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Longevity (years)
		/// </summary>
		public InputValue<int> Longevity
		{
			get {
				return longevity;
			}

			set {
				if (value != null) {
					if (maturity != null) {
						if (value.Actual < maturity)
							throw new InputValueException(value.String,
							                              "Longevity must be \u2265 sexual maturity");
					}
					if (maxSproutAge != null) {
						if (value.Actual < maxSproutAge)
							throw new InputValueException(value.String,
							                              "Longevity must be \u2265 maximum sprouting age");
					}
					if (maturity == null && maxSproutAge == null) {
						if (value.Actual < 0)
							throw new InputValueException(value.String,
							                              "Longevity must be \u2265 0");
					}
				}
				longevity = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Age of sexual maturity (years)
		/// </summary>
		public InputValue<int> Maturity
		{
			get {
				return maturity;
			}

			set {
				if (value != null) {
					if (longevity != null) {
						if (value.Actual > longevity)
							throw new InputValueException(value.String,
							                              "Sexual maturity must be \u2264 longevity");
					}
					if (value.Actual < 0)
						throw new InputValueException(value.String,
						                              "Sexual maturity must be \u2265 0");
				}
				maturity = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Shade tolerance class (1-5)
		/// </summary>
		public InputValue<byte> ShadeTolerance
		{
			get {
				return shadeTolerance;
			}

			set {
				if (value != null)
					ValidateTolerance(value);
				shadeTolerance = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Fire tolerance class (1-5)
		/// </summary>
		public InputValue<byte> FireTolerance
		{
			get {
				return fireTolerance;
			}

			set {
				if (value != null)
					ValidateTolerance(value);
				fireTolerance = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Windthrow tolerance class (1-5)
		/// </summary>
		public InputValue<byte> WindTolerance
		{
			get {
				return windTolerance;
			}

			set {
				if (value != null)
					ValidateTolerance(value);
				windTolerance = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Effective seed dispersal distance (m?)
		/// </summary>
		public InputValue<int> EffectiveSeedDist
		{
			get {
				return effectiveSeedDist;
			}

			set {
				if (value != null) {
					if (maxSeedDist != null) {
						if (value.Actual > maxSeedDist.Actual)
							throw new InputValueException(value.String,
							                              "Effective seed distance must be \u2264 maximum seed distance");
					}
					else {
						if (value.Actual < 0)
							throw new InputValueException(value.String,
							                              "Effective seed distance must be \u2265 0");
					}
				}
				effectiveSeedDist = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Maximum seed dispersal distance (m?)
		/// </summary>
		public InputValue<int> MaxSeedDist
		{
			get {
				return maxSeedDist;
			}

			set {
				if (value != null) {
					if (effectiveSeedDist != null) {
						if (value.Actual < effectiveSeedDist.Actual)
							throw new InputValueException(value.String,
							                              "Maximum seed distance must be \u2265 effective seed distance");
					}
					else {
						if (value.Actual < 0)
							throw new InputValueException(value.String,
							                              "Maximum seed distance must be \u2265 0");
					}
				}
				maxSeedDist = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Vegetative reproduction probability
		/// </summary>
		public InputValue<float> VegReprodProb
		{
			get {
				return vegReprodProb;
			}

			set {
				if (value != null) {
					if (value.Actual < 0.0 || value.Actual > 1.0)
						throw new InputValueException(value.String,
						                              value.String + " is not between 0 and 1.0");
				}
				vegReprodProb = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Maximum age for sprouting (years)?
		/// </summary>
		public InputValue<int> MaxSproutAge
		{
			get {
				return maxSproutAge;
			}

			set {
				if (value != null) {
					if (longevity != null) {
						if (value.Actual > longevity)
							throw new InputValueException(value.String,
							                              "Maximum sprouting age must be \u2264 longevity");
					}
					if (value.Actual < 0)
						throw new InputValueException(value.String,
						                              "Maximum sprouting age must be \u2265 0");
				}
				maxSproutAge = value;
			}
		}

		//---------------------------------------------------------------------

		private void ValidateTolerance(InputValue<byte> tolerance)
		{
			if (1 <= tolerance.Actual && tolerance.Actual <= 5)
				return;
			throw new InputValueException(tolerance.String,
			                              tolerance.String + " is not between 1 and 5");
		}

		//---------------------------------------------------------------------

		public bool IsComplete
		{
			get {
				object[] parameters = new object[]{
					name, longevity, maturity,
					shadeTolerance, fireTolerance, windTolerance,
					effectiveSeedDist, maxSeedDist,
					vegReprodProb, maxSproutAge};
				foreach (object parameter in parameters)
					if (parameter == null)
						return false;
				return true;
			}
		}

		//---------------------------------------------------------------------

		public IParameters GetComplete()
		{
			if (this.IsComplete)
				return new Parameters(name.Actual,
				                      longevity.Actual,
				                      maturity.Actual,
				                      shadeTolerance.Actual,
				                      fireTolerance.Actual,
				                      windTolerance.Actual,
				                      effectiveSeedDist.Actual,
				                      maxSeedDist.Actual,
				                      vegReprodProb.Actual,
				                      maxSproutAge.Actual);
			else
				return null;
		}
	}
}

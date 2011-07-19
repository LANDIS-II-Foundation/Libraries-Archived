using Landis.Util;

namespace Landis.Species
{
	/// <summary>
	/// The parameters for a single tree species.
	/// </summary>
	public class Parameters
		: IParameters
	{
		private string name;
		private int longevity;
		private int maturity;
		private byte shadeTolerance;
		private byte fireTolerance;
		private byte windTolerance;
		private int effectiveSeedDist;
		private int maxSeedDist;
		private float vegReprodProb;
		private int maxSproutAge;

		//---------------------------------------------------------------------

		public string Name
		{
			get {
				return name;
			}
		}

		//---------------------------------------------------------------------

		public int Longevity
		{
			get {
				return longevity;
			}
		}

		//---------------------------------------------------------------------

		public int Maturity
		{
			get {
				return maturity;
			}
		}

		//---------------------------------------------------------------------

		public byte ShadeTolerance
		{
			get {
				return shadeTolerance;
			}
		}

		//---------------------------------------------------------------------

		public byte FireTolerance
		{
			get {
				return fireTolerance;
			}
		}

		//---------------------------------------------------------------------

		public byte WindTolerance
		{
			get {
				return windTolerance;
			}
		}

		//---------------------------------------------------------------------

		public int EffectiveSeedDist
		{
			get {
				return effectiveSeedDist;
			}
		}

		//---------------------------------------------------------------------

		public int MaxSeedDist
		{
			get {
				return maxSeedDist;
			}
		}

		//---------------------------------------------------------------------

		public float VegReprodProb
		{
			get {
				return vegReprodProb;
			}
		}

		//---------------------------------------------------------------------

		public int MaxSproutAge
		{
			get {
				return maxSproutAge;
			}
		}

		//---------------------------------------------------------------------

		public Parameters(string name,
		                  int longevity,
		                  int maturity,
		                  byte shadeTolerance,
		                  byte fireTolerance,
		                  byte windTolerance,
		                  int effectiveSeedDist,
		                  int maxSeedDist,
		                  float vegReprodProb,
		                  int maxSproutAge)
		{
			this.name = name;
			this.longevity = longevity;
			this.maturity = maturity;
			this.shadeTolerance = shadeTolerance;
			this.fireTolerance = fireTolerance;
			this.windTolerance = windTolerance;
			this.effectiveSeedDist = effectiveSeedDist;
			this.maxSeedDist = maxSeedDist;
			this.vegReprodProb = vegReprodProb;
			this.maxSproutAge = maxSproutAge;
		}

		//---------------------------------------------------------------------

		public Parameters(IParameters parameters)
		{
			name              = parameters.Name;
			longevity         = parameters.Longevity;
			maturity          = parameters.Maturity;
			shadeTolerance    = parameters.ShadeTolerance;
			fireTolerance     = parameters.FireTolerance;
			windTolerance     = parameters.WindTolerance;
			effectiveSeedDist = parameters.EffectiveSeedDist;
			maxSeedDist       = parameters.MaxSeedDist;
			vegReprodProb     = parameters.VegReprodProb;
			maxSproutAge      = parameters.MaxSproutAge;
		}
	}
}

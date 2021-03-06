namespace Landis.Util
{
	/// <summary>
	/// Methods for random-number generation.
	/// </summary>
	public static class Random
	{
		private static jp.takel.PseudoRandom.MersenneTwister generator;

		//---------------------------------------------------------------------

		static Random()
		{
			generator = new jp.takel.PseudoRandom.MersenneTwister();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Generates a random number with a uniform distribution between
		/// 0.0 and 1.0.
		/// </summary>
		public static double GenerateUniform()
		{
			return generator.NextDouble();
		}
	}
}

using Flel = Edu.Wisc.Forest.Flel;
using System.Collections.Generic;

namespace Landis.Util
{
	/// <summary>
	/// Methods for random-number generation.
	/// </summary>
	public static class Random
	{
		private static jp.takel.PseudoRandom.MersenneTwister generator;

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes the random-number generator with a specific seed.
		/// </summary>
		public static void Initialize(uint seed)
		{
			generator = new jp.takel.PseudoRandom.MersenneTwister(seed);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Generates a non-zero random seed using the system clock.
		/// </summary>
		public static uint GenerateSeed()
		{
			System.Random rng = new System.Random();
			int seed;
			do {
				seed = rng.Next(int.MinValue, int.MaxValue);
			} while (seed == 0);
			return (uint) seed;
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

		//---------------------------------------------------------------------

		/// <summary>
		/// Generates a random number with an exponential distribution.
		/// </summary>
		/// <param name="mean">
		/// The mean of the exponential distribution.
		/// </param>
		/// <exception cref="System.ArgumentException">
		/// mean is zero or negative; it must be positive.
		/// </exception>
		public static double GenerateExponential(double mean)
		{
			if (mean <= 0.0)
				throw new System.ArgumentException("mean must be positive");
			return -mean * System.Math.Log(1 - generator.NextDouble());
		}

		//---------------------------------------------------------------------

		///	<summary>
		/// Shuffles the items in a list.
		/// </summary>
		/// <param name="list">
		/// The list to be shuffled.
		/// </param>
		public static void Shuffle<T>(IList<T> list)
		{
			if (list == null || list.Count <= 1)
				return;
			IList<T> shuffledList = Flel.Util.List.Shuffle(list, generator);
			for (int i = 0; i < list.Count; i++)
				list[i] = shuffledList[i];
		}
	}
}

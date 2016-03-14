using Edu.Wisc.Forest.Flel.Util;
using System.Collections.Generic;

namespace Landis.DualScale
{
    /// <summary>
    /// Methods for applying majority rule to fine-scale input maps.
    /// </summary>
    public static class MajorityRule
    {
        /// <summary>
        /// Selects a map code from a dictionary of codes and their counts
        /// in a broad-scale block.
        /// </summary>
        public static ushort SelectMapCode(IDictionary<ushort, int> codeCounts)
        {
            Require.ArgumentNotNull(codeCounts);
            if (codeCounts.Count == 0)
                throw new System.ArgumentException("# of code counts = 0; Must be = or > 1");

            IList<ushort> mostCommonCodes = new List<ushort>();
            int maxCount = 0;
            foreach (KeyValuePair<ushort, int> pair in codeCounts) {
                if (pair.Value > maxCount) {
                    maxCount = pair.Value;
                    mostCommonCodes.Clear();
                    mostCommonCodes.Add(pair.Key);
                }
                else if (pair.Value == maxCount) {
                    mostCommonCodes.Add(pair.Key);
                }
            }
            if (mostCommonCodes.Count == 1)
                return mostCommonCodes[0];
            else {
                int selectedIndex = RandomlySelectBetween(0, mostCommonCodes.Count - 1);
                return mostCommonCodes[selectedIndex];
            }
        }

        //---------------------------------------------------------------------

        public class Delegates
        {
            public delegate int RandomBetween(int low,
                                              int high);
        }

        //---------------------------------------------------------------------

        private static Delegates.RandomBetween randomBetweenMethod;

        //---------------------------------------------------------------------

        static MajorityRule()
        {
            randomBetweenMethod = Default.RandomBetween;
        }

        //---------------------------------------------------------------------

        public static Delegates.RandomBetween RandomlySelectBetween
        {
            get {
                return randomBetweenMethod;
            }

            set {
                if (value == null)
                    throw new System.ArgumentNullException();
                randomBetweenMethod = value;
            }
        }

        //---------------------------------------------------------------------

        public static class Default
        {
            public static int RandomBetween(int low,
                                            int high)
            {
                if (high < low)
                    throw new System.ArgumentException("high is < low");
                if (low == high)
                    return low;
                return low + (int) (System.Math.Floor(Landis.Util.Random.GenerateUniform() * (high - low + 1)));
            }
        }
    }
}

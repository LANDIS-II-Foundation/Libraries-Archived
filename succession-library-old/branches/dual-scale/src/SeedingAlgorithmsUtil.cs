using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Succession
{
    /// <summary>
    /// Utility methods for the enumerated type SeedingAlgorithms.
    /// </summary>
    public static class SeedingAlgorithmsUtil
    {
        /// <summary>
        /// Parses a word into a SeedingAlgorithm.
        /// </summary>
        /// <exception cref="System.FormatException">
        /// The word doesn't match any of these: "NoDispersal",
        /// "UniversalDispersal", "WardSeedDispersal".
        /// </exception>
        public static SeedingAlgorithms Parse(string word)
        {
            if (word == "NoDispersal")
                return SeedingAlgorithms.NoDispersal;
            else if (word == "UniversalDispersal")
                return SeedingAlgorithms.UniversalDispersal;
            else if (word == "WardSeedDispersal")
                return SeedingAlgorithms.WardSeedDispersal;
            throw new System.FormatException("Valid algorithms: NoDispersal, UniversalDispersal, WardSeedDispersal");
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Registers the appropriate method for reading input values of the
        /// type SeedingAlgorithms.
        /// </summary>
        public static void RegisterForInputValues()
        {
            Type.SetDescription<SeedingAlgorithms>("seeding algorithm");
            InputValues.Register<SeedingAlgorithms>(Parse);
        }
    }
}

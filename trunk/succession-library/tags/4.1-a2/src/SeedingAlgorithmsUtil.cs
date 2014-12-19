using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Library.Succession
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
        /// "UniversalDispersal", "WardSeedDispersal", "DemographicSeeding".
        /// </exception>
        public static SeedingAlgorithms Parse(string word)
        {
            if (word == "NoDispersal")
                return SeedingAlgorithms.NoDispersal;
            else if (word == "UniversalDispersal")
                return SeedingAlgorithms.UniversalDispersal;
            else if (word == "WardSeedDispersal")
                return SeedingAlgorithms.WardSeedDispersal;
            else if (word == "DemographicSeeding")
                return SeedingAlgorithms.DemographicSeeding;
            throw new System.FormatException("Valid algorithms: NoDispersal, UniversalDispersal, WardSeedDispersal, DemographicSeeding");
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

        //---------------------------------------------------------------------

        /// <summary>
        /// Gets the seeding algorithm identified by a particular value of the
        /// SeedingAlgorithms type.
        /// </summary>
        public static SeedingAlgorithm GetAlgorithm(SeedingAlgorithms seedAlg,
                                                    int               successionTimestep)
        {
            SeedingAlgorithm algorithm;
            switch (seedAlg)
            {
                case SeedingAlgorithms.NoDispersal:
                    algorithm = NoDispersal.Algorithm;
                    break;

                case SeedingAlgorithms.UniversalDispersal:
                    algorithm = UniversalDispersal.Algorithm;
                    break;

                case SeedingAlgorithms.WardSeedDispersal:
                    algorithm = WardSeedDispersal.Algorithm;
                    break;

                case SeedingAlgorithms.DemographicSeeding:
                    algorithm = new DemographicSeeding.Algorithm(successionTimestep).DoesSpeciesSeedSite;
                    break;

                default:
                    throw new System.ArgumentException(string.Format("Unknown seeding algorithm: {0}", seedAlg));
            }
            return algorithm;
        }
    }
}

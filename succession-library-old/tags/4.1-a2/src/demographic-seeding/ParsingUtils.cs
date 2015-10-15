using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Library.Succession.DemographicSeeding
{
    /// <summary>
    /// Utility methods for parsing parameter values.
    /// </summary>
    public static class ParsingUtils
    {
        /// <summary>
        /// Parses a word into a dispersal kernel.
        /// </summary>
        /// <exception cref="System.FormatException">
        /// The word doesn't match any of these: "DoubleExponential" or
        /// "2Dt".
        /// </exception>
        public static Seed_Dispersal.Dispersal_Model ParseKernel(string word)
        {
            if (word == "DoubleExponential")
                return Seed_Dispersal.Dispersal_Model.DOUBLE_EXPONENTIAL;
            else if (word == "2Dt")
                return Seed_Dispersal.Dispersal_Model.TWODT;
            throw new System.FormatException("Valid kernels: DoubleExponential, 2Dt");
        }

        /// <summary>
        /// Parses a word into a seed production model.
        /// </summary>
        /// <exception cref="System.FormatException">
        /// The word doesn't match any of these: "Fixed" or "Uniform".
        /// </exception>
        public static Seed_Dispersal.Seed_Model ParseProduction(string word)
        {
            if (word == "Fixed")
                return Seed_Dispersal.Seed_Model.FIXED;
            else if (word == "Uniform")
                return Seed_Dispersal.Seed_Model.UNIFORM;
            throw new System.FormatException("Valid production models: Fixed, Uniform");
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Registers the appropriate method for reading input values of the
        /// Seed_Disperal.Dispersal_Model and Seed_Model types.
        /// </summary>
        public static void RegisterForInputValues()
        {
            Type.SetDescription<Seed_Dispersal.Dispersal_Model>("dispersal kernel");
            InputValues.Register<Seed_Dispersal.Dispersal_Model>(ParseKernel);

            Type.SetDescription<Seed_Dispersal.Seed_Model>("seed production model");
            InputValues.Register<Seed_Dispersal.Seed_Model>(ParseProduction);
        }
    }
}

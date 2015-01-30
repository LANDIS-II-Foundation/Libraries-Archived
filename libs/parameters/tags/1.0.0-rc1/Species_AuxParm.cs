// Copyright 2005 University of Wisconsin

using Landis.Core;

namespace Landis.Library.Parameters.Species
{
    /// <summary>
    /// An auxiliary parameter for species.
    /// </summary>
    public class AuxParm<T>
    {
        private T[] values;

        //---------------------------------------------------------------------

        ///<Summary>
        /// Gets a species specific value
        ///</Summary>
        public T this[ISpecies species]
        {
            get {
                return values[species.Index];
            }

            set {
                values[species.Index] = value;
            }
        }
        //---------------------------------------------------------------------

        ///<Summary>
        /// Initializes a species-specific parameter
        ///</Summary>
        public AuxParm(ISpeciesDataset species)
        {
            values = new T[species.Count];
        }
    }
}

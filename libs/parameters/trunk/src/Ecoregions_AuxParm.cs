// Copyright 2005 University of Wisconsin

using Landis.Core;

namespace Landis.Library.Parameters.Ecoregions
{
    /// <summary>
    /// An auxiliary parameter for ecoregions.
    /// </summary>
    public class AuxParm<T>
    {
        private T[] values;

        public T[] Values
        {
            get
            {
                return values;
            }
        }
        //---------------------------------------------------------------------

        ///<Summary>
        /// Gets an ecoregion-specific value
        ///</Summary>
        public T this[IEcoregion ecoregion]
        {
            get {
                return values[ecoregion.Index];
            }

            set {
                values[ecoregion.Index] = value;
            }
        }

        //---------------------------------------------------------------------

        ///<Summary>
        /// Initializes an ecoregion-specific value
        ///</Summary>
        public AuxParm(IEcoregionDataset ecoregions)
        {
            values = new T[ecoregions.Count];
        }
    }
}

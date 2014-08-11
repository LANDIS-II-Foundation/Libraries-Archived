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

        //---------------------------------------------------------------------

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

        public AuxParm(IEcoregionDataset ecoregions)
        {
            values = new T[ecoregions.Count];
        }
    }
}

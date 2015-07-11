using Landis.Species;
using System.Collections.Generic;
using Wisc.Flel.GeospatialModeling.Grids;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

namespace Landis.Test.AgeCohort
{
    public static class Data
    {
        private static Species.Dataset dataset;

        //---------------------------------------------------------------------

        public static Species.Dataset Species
        {
            get {
                if (dataset == null)
                    CreateDataset();
                return dataset;
            }
        }

        //---------------------------------------------------------------------

        private static void CreateDataset()
        {
            List<Species.IParameters> parms = new List<Species.IParameters>();
                                            //                      Sexual    Shade  Fire  Seed Disperal Dist  Vegetative   Sprout Age  Post-Fire
                                            // Name      Longevity  Maturity  Tol.   Tol.  Effective  Maximum  Reprod Prob  Min   Max   Regen
                                            // ----      ---------  --------  -----  ----  ---------  -------  -----------  ----------  ---------
            parms.Add(new Species.Parameters("abiebals",  200,         25,      5,    1,     130,       160,       0.0f,      0,     0,  PostFireRegeneration.None ));
            parms.Add(new Species.Parameters("betualle",  300,         40,      4,    2,     100,       400,       0.1f,     10,   180,  PostFireRegeneration.Resprout ));
            parms.Add(new Species.Parameters("poputrem",  100,         20,      1,    1,    1000,      5000,       0.9f,     10,   100,  PostFireRegeneration.Serotiny ));

            dataset = new Species.Dataset(parms);
        }

        //---------------------------------------------------------------------

        public static ILandscape Make1by1Landscape()
        {
            EcoregionCode[,] grid = new EcoregionCode[,]{ {new EcoregionCode(1, true)} };
            DataGrid<EcoregionCode> dataGrid = new DataGrid<EcoregionCode>(grid);
            return new Landscape(dataGrid, 1);
        }
    }
}

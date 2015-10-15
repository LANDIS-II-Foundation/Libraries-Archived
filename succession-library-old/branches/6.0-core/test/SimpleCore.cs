using Wisc.Flel.GeospatialModeling.Landscapes;
using Wisc.Flel.GeospatialModeling.RasterIO;

using GridDimensions = Wisc.Flel.GeospatialModeling.Grids.Dimensions;

namespace Landis.Test.Succession
{
    /// <summary>
    /// Simple implementation of the core for test purposes.
    /// </summary>
    public class SimpleCore
        : PlugIns.ICore
    {
        public Species.IDataset Species;
        public Ecoregions.IDataset Ecoregions;
        public ISiteVar<Ecoregions.IEcoregion> Ecoregion;
        public ILandscape Landscape;
        public IMetadata LandscapeMapMetadata;
        public float CellLength;
        public float CellArea;
        public int StartTime;
        public int EndTime;
        public int CurrentTime;
        public int TimeSinceStart;
        public Cohorts.TypeIndependent.ILandscapeCohorts SuccessionCohorts;

        //---------------------------------------------------------------------

        Species.IDataset PlugIns.ICore.Species
        {
            get {
                return Species;
            }
        }

        //---------------------------------------------------------------------

        Ecoregions.IDataset PlugIns.ICore.Ecoregions
        {
            get {
                return Ecoregions;
            }
        }

        //---------------------------------------------------------------------

        ISiteVar<Ecoregions.IEcoregion> PlugIns.ICore.Ecoregion
        {
            get {
                return Ecoregion;
            }
        }

        //---------------------------------------------------------------------

        ILandscape PlugIns.ICore.Landscape
        {
            get {
                return Landscape;
            }
        }

        //---------------------------------------------------------------------

        IMetadata PlugIns.ICore.LandscapeMapMetadata
        {
            get {
                return LandscapeMapMetadata;
            }
        }

        //---------------------------------------------------------------------

        float PlugIns.ICore.CellLength
        {
            get {
                return CellLength;
            }
        }

        //---------------------------------------------------------------------

        float PlugIns.ICore.CellArea
        {
            get {
                return CellArea;
            }
        }

        //---------------------------------------------------------------------

        int PlugIns.ICore.StartTime
        {
            get {
                return StartTime;
            }
        }

        //---------------------------------------------------------------------

        int PlugIns.ICore.EndTime
        {
            get {
                return EndTime;
            }
        }

        //---------------------------------------------------------------------

        int PlugIns.ICore.CurrentTime
        {
            get {
                return CurrentTime;
            }
        }

        //---------------------------------------------------------------------

        int PlugIns.ICore.TimeSinceStart
        {
            get {
                return TimeSinceStart;
            }
        }

        //---------------------------------------------------------------------

        Cohorts.TypeIndependent.ILandscapeCohorts PlugIns.ICore.SuccessionCohorts
        {
            get {
                return SuccessionCohorts;
            }
        }

        //---------------------------------------------------------------------

        public virtual IInputRaster<TPixel> OpenRaster<TPixel>(string path)
             where TPixel : IPixel, new()
        {
            return null;
        }

        //---------------------------------------------------------------------

        public virtual IOutputRaster<TPixel> CreateRaster<TPixel>(string         path,
                                                                  GridDimensions dimensions,
                                                                  IMetadata      metadata)
             where TPixel : IPixel, new()
        {
            return null;
        }

        //---------------------------------------------------------------------

        void PlugIns.ICore.RegisterSiteVar(ISiteVariable siteVar,
                                           string        name)
        {
        }

        //---------------------------------------------------------------------

        ISiteVar<T> PlugIns.ICore.GetSiteVar<T>(string name)
        {
            return null;
        }
    }
}

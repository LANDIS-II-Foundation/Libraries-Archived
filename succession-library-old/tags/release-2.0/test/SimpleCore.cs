using Edu.Wisc.Forest.Flel.Grids;
using Landis.Landscape;
using Landis.RasterIO;

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
        public Landscape.ISiteVar<Ecoregions.IEcoregion> Ecoregion;
        public Landscape.ILandscape Landscape;
        public RasterIO.IMetadata LandscapeMapMetadata;
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

        Landscape.ISiteVar<Ecoregions.IEcoregion> PlugIns.ICore.Ecoregion
        {
            get {
                return Ecoregion;
            }
        }

        //---------------------------------------------------------------------

        Landscape.ILandscape PlugIns.ICore.Landscape
        {
            get {
                return Landscape;
            }
        }

        //---------------------------------------------------------------------

        RasterIO.IMetadata PlugIns.ICore.LandscapeMapMetadata
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

        public virtual IOutputRaster<TPixel> CreateRaster<TPixel>(string     path,
                                                                  Dimensions dimensions,
                                                                  IMetadata  metadata)
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

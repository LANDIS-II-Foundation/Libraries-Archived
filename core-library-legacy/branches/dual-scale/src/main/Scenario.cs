namespace Landis
{
    /// <summary>
    /// A model scenario.
    /// </summary>
    public class Scenario
    {
        private int startTime;
        private int endTime;
        private string species;
        private string ecoregions;
        private string ecoregionsMap;
        private float? cellLength;
        private int? blockSize;
        private string initCommunities;
        private string communitiesMap;
        private PlugInAndInitFile succession;
        private PlugInAndInitFile[] disturbances;
        private bool disturbRandom;
        private PlugInAndInitFile[] otherPlugIns;
        private uint? seed;

        //---------------------------------------------------------------------

        /// <summary>
        /// The calendar year that the scenario starts from (represents time
        /// step 0).
        /// </summary>
        public int StartTime
        {
            get {
                return startTime;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The calendar year that the scenario ends at.
        /// </summary>
        public int EndTime
        {
            get {
                return endTime;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the file with species parameters.
        /// </summary>
        public string Species
        {
            get {
                return species;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the file with ecoregion definitions.
        /// </summary>
        public string Ecoregions
        {
            get {
                return ecoregions;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the raster file showing where the ecoregions are.
        /// </summary>
        public string EcoregionsMap
        {
            get {
                return ecoregionsMap;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The length of a cell's side (meters).  Optional; used only if
        /// ecoregion map does not specify cell length in its metadata.
        /// </summary>
        public float? CellLength
        {
            get {
                return cellLength;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The size of a broad-scale block (site).
        /// </summary>
        /// <remarks>
        /// The size is expressed in the number of fine-scale sites along an
        /// edge of the block.  For example, a block size of 3 means a broad-
        /// scale site contains 9 (3-by-3) fine-scale sites.
        /// <para>
        /// Optional.  Defaults to 1 (single-scale).
        /// </remarks>
        public int? BlockSize
        {
            get {
                return blockSize;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the file with the initial communities' definitions.
        /// </summary>
        public string InitialCommunities
        {
            get {
                return initCommunities;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the raster file showing where the initial communities are.
        /// </summary>
        public string InitialCommunitiesMap
        {
            get {
                return communitiesMap;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The succession plug-in to use for the scenario.
        /// </summary>
        public PlugInAndInitFile Succession
        {
            get {
                return succession;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The disturbance plug-ins to use for the scenario.
        /// </summary>
        public PlugInAndInitFile[] Disturbances
        {
            get {
                return disturbances;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Are disturbance run is random order?
        /// </summary>
        public bool DisturbancesRandomOrder
        {
            get {
                return disturbRandom;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The other plug-ins to use for the scenario (for example: output,
        /// metapopulation).
        /// </summary>
        public PlugInAndInitFile[] OtherPlugIns
        {
            get {
                return otherPlugIns;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// The seed for the random number generator.
        /// </summary>
        public uint? RandomNumberSeed
        {
            get {
                return seed;
            }
        }

        //---------------------------------------------------------------------

        public Scenario(int                 startTime,
                        int                 endTime,
                        string              species,
                        string              ecoregions,
                        string              ecoregionsMap,
                        float?              cellLength,
                        int?                blockSize,
                        string              initCommunities,
                        string              communitiesMap,
                        PlugInAndInitFile   succession,
                        PlugInAndInitFile[] disturbances,
                        bool                disturbRandom,
                        PlugInAndInitFile[] otherPlugIns,
                        uint?               seed)
        {
            this.startTime       = startTime;
            this.endTime         = endTime;
            this.species         = species;
            this.ecoregions      = ecoregions;
            this.ecoregionsMap   = ecoregionsMap;
            this.cellLength      = cellLength;
            this.blockSize       = blockSize;
            this.initCommunities = initCommunities;
            this.communitiesMap  = communitiesMap;
            this.succession      = succession;
            this.disturbances    = disturbances;
            this.disturbRandom   = disturbRandom;
            this.otherPlugIns    = otherPlugIns;
            this.seed            = seed;
        }
    }
}

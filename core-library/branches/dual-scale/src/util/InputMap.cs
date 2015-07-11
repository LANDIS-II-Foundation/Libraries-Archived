using System.Collections.Generic;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;
using Wisc.Flel.GeospatialModeling.RasterIO;

namespace Landis.DualScale
{
    /// <summary>
    /// Methods for working with fine-scale input maps.
    /// </summary>
    public static class InputMap
    {
        /// <summary>
        /// Delegates related to input maps.
        /// </summary>
        public static class Delegates
        {
            /// <summary>
            /// A method for initializing data for a particular active site
            /// based on the associated map code in an input map.
            /// <summary>
            public delegate void InitializeSite(ActiveSite activeSite,
                                                ushort     mapCode);
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Reads a fine-scale input map using majority rule.
        /// </summary>
        /// <param name="map">
        /// The input map to read.
        /// </param>
        /// <param name="landscape">
        /// The landscape that the input map is associated with.
        /// </param>
        /// <param name="initSiteMethod">
        /// The method that's called to process a map code for a site.
        /// </param>
        public static void ReadWithMajorityRule<TPixel>(IInputRaster<TPixel>     map,
                                                        ILandscape               landscape,
                                                        Delegates.InitializeSite initSiteMethod)
            where TPixel : SingleBandPixel<ushort>, new()
        {
            BlockRowBuffer< IDictionary<ushort, int> > codeCountBuffer = GetCodeCountBuffer(landscape);
            Location lowerRight = new Location(landscape.BlockSize,
                                               landscape.BlockSize);
            foreach (Site site in landscape.AllSites) {
                TPixel pixel = map.ReadPixel();
                if (site.IsActive) {
                    ActiveSite activeSite = (ActiveSite) site;
                    ushort mapCode = pixel.Band0;
                    if (activeSite.SharesData) {
                        // Increment the count for the current map code.
                        Location blockLocation = activeSite.BroadScaleLocation;
                        IDictionary<ushort, int> codeCounts = codeCountBuffer[blockLocation.Column];
                        int count;
                        codeCounts.TryGetValue(mapCode, out count);
                        codeCounts[mapCode] = count + 1;

                        if (activeSite.LocationInBlock == lowerRight) {
                            // Last site in the block, so select one of
                            // map codes by majority rule.
                            ushort selectedMapCode = MajorityRule.SelectMapCode(codeCounts);
                            initSiteMethod(activeSite, selectedMapCode);
                            codeCounts.Clear();
                        }
                    }
                    else {
                        // Active site has a unique data index
                        initSiteMethod(activeSite, mapCode);
                    }
                }
            }
        }

        //---------------------------------------------------------------------

        public static BlockRowBuffer< IDictionary<ushort, int> > GetCodeCountBuffer(ILandscape landscape)
        {
            BlockRowBuffer< IDictionary<ushort, int> > buffer =
                new BlockRowBuffer< IDictionary<ushort, int> >(landscape);
            for (int col = 1; col <= buffer.Columns; col++)
                buffer[col] = new Dictionary<ushort, int>();
            return buffer;
        }
    }
}

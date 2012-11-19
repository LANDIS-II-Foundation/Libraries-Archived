using Landis.Core;
using Landis.SpatialModeling;
using System.Collections.Generic;

namespace Landis.Library.LandUses
{
    /// <summary>
    /// Represents the current land use at a site.
    /// </summary>
    public class LandUse
    {
        /// <summary>
        /// Name of the land use.
        /// </summary>
        public string Name { get; private set;  }

        /// <summary>
        /// Does the land use allow harvesting?
        /// </summary>
        public bool AllowsHarvest { get; private set; }

        /// <summary>
        /// Site variable with each site's current land use.
        /// </summary>
        public static ISiteVar<LandUse> SiteVar { get; private set; }

        public LandUse(string name,
                       bool allowsHarvest)
        {
            Name = name;
            AllowsHarvest = allowsHarvest;
        }

        private static IList<LandUse> landUses;

        public static void Initialize(ICore modelCore)
        {
            string path = "land-uses.txt";
            modelCore.UI.WriteLine("Reading land uses from \"{0}\"...", path);
            Parser parser = new Parser();
            landUses = Data.Load<IList<LandUse>>(path, parser);

            SiteVar = modelCore.Landscape.NewSiteVar<LandUse>();
            // Initialize all the actives to the first land-use in table.
            SiteVar.ActiveSiteValues = landUses[0];
        }
    }
}

// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Landis.SpatialModeling;
using System.Collections.Generic;
using System.IO;

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// Utility methods for stands.
    /// </summary>
    public static class Stands
    {
        /// <summary>
        /// Reads the input map of stands.
        /// </summary>
        /// <param name="path">
        /// Path to the map.
        /// </param>

        public static void ReadMap(string path) {
            Stand stand;
            Dictionary<uint, Stand> stands = new Dictionary<uint, Stand>();

            IInputRaster<UIntPixel> map;

            try
            {
                map = Model.Core.OpenRaster<UIntPixel>(path);
            }
            catch (FileNotFoundException)
            {
                string mesg = string.Format("Error: The file {0} does not exist", path);
                throw new System.ApplicationException(mesg);
            }

            if (map.Dimensions != Model.Core.Landscape.Dimensions)
            {
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }

            using (map) {
                
                UIntPixel pixel = map.BufferPixel;
                foreach (Site site in Model.Core.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    uint mapCode = pixel.MapCode.Value;
                    if (site.IsActive && SiteVars.ManagementArea[site] != null)
                    {
                        if (stands.TryGetValue(mapCode, out stand)) {
                            //if the stand is already in the dictionary, check if it is in the same management area.
                            //if it's not in the same MA, throw exception.
                            if (SiteVars.ManagementArea[site] != stand.ManagementArea) {
                                string mesg = string.Format("Stand {0} is in management areas {1} and {2}",
                                    stand.MapCode,
                                    stand.ManagementArea.MapCode,
                                    SiteVars.ManagementArea[site].MapCode);
                                throw new System.ApplicationException(mesg);
                            }
                        
                        }
                        //valid site location which has not been keyed by the dictionary.
                        else {
                            //assign stand (trygetvalue set it to null when it wasn't found in the dictionary)
                            stand = new Stand(mapCode);
                            //add this stand to the correct management area (pointed to by the site)
                            SiteVars.ManagementArea[site].Add(stand);
                            stands[mapCode] = stand;
                        }                        
                        //add this site to this stand
                        stand.Add((ActiveSite) site);
                    }
                }

            }
        }
    }
}
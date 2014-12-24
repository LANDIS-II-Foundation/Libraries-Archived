// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Landis.SpatialModeling;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// Utility methods for management areas.
    /// </summary>
    public static class ManagementAreas
    {
        /// <summary>
        /// Reads the input map of management areas.
        /// </summary>
        /// <param name="path">
        /// Path to the map.
        /// </param>
        /// <param name="managementAreas">
        /// Management areas that have prescriptions applied to them.
        /// </param>
        public static void ReadMap(string                 path,
                                   IManagementAreaDataset managementAreas)
        {

            IInputRaster<UIntPixel> map;

            try {
                map = Model.Core.OpenRaster<UIntPixel>(path);
            }
            catch (FileNotFoundException) {
                string mesg = string.Format("Error: The file {0} does not exist", path);
                throw new System.ApplicationException(mesg);
            }

            if (map.Dimensions != Model.Core.Landscape.Dimensions)
            {
                string mesg = string.Format("Error: The input map {0} does not have the same dimension (row, column) as the ecoregions map", path);
                throw new System.ApplicationException(mesg);
            }

            List<uint> inactiveMgmtAreas = new List<uint>();

            using (map) {
                UIntPixel pixel = map.BufferPixel;
                foreach (Site site in Model.Core.Landscape.AllSites)
                {
                    map.ReadBufferPixel();
                    if (site.IsActive)
                    {
                        uint mapCode = pixel.MapCode.Value;
                        ManagementArea mgmtArea = managementAreas.Find(mapCode);
                        if (mgmtArea == null) {
                            if (! inactiveMgmtAreas.Contains(mapCode))
                                inactiveMgmtAreas.Add(mapCode);
                        }
                        else {
                            mgmtArea.OnMap = true;
                            SiteVars.ManagementArea[site] = mgmtArea;
                        }
                    }
                }
            }

            // Inform user about non-active areas: those that don't have any
            // applied prescriptions.
            if (inactiveMgmtAreas.Count > 0) {
                Model.Core.UI.WriteLine("   Inactive management areas: {0}",
                             MapCodesToString(inactiveMgmtAreas));
            }
        }

        //---------------------------------------------------------------------

        public static string MapCodesToString(List<uint> mapCodes)
        {
            if (mapCodes == null || mapCodes.Count == 0)
                return "";

            mapCodes.Sort();
            List<Range> ranges = new List<Range>();
            Range currentRange = new Range(mapCodes[0]);
            for (int i = 1; i < mapCodes.Count; i++) {
                uint mapCode = mapCodes[i];
                if (currentRange.End + 1 == mapCode)
                    currentRange.End = mapCode;
                else {
                    ranges.Add(currentRange);
                    currentRange = new Range(mapCode);
                }
            }
            ranges.Add(currentRange);

            StringBuilder listAsText = new StringBuilder(ranges[0].ToString());
            for (int i = 1; i < ranges.Count; i++) {
                listAsText.AppendFormat(", {0}", ranges[i]);
            }
            return listAsText.ToString();
        }

        //---------------------------------------------------------------------

        private struct Range
        {
            public uint Start;
            public uint End;

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            public Range(uint mapCode)
            {
                Start = mapCode;
                End = mapCode;
            }

            //- - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

            public override string ToString()
            {
                if (Start == End)
                    return Start.ToString();
                if (Start + 1 == End)
                    return string.Format("{0}, {1}", Start, End);
                return string.Format("{0}-{1}", Start, End);
            }
        }
    }
}

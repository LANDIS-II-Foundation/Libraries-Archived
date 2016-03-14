// Copyright 2007 University of Wisconsin
// Author: James Domingo, UW-Madison, FLEL

using Landis.DualScale;
using NUnit.Framework;
using System.Collections.Generic;
using Wisc.Flel.GeospatialModeling.Grids;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;
using Wisc.Flel.GeospatialModeling.RasterIO;

using Location = Wisc.Flel.GeospatialModeling.Landscapes.DualScale.Location;

namespace Landis.Test.Util
{
	[TestFixture]
	public class InputMap_Test
	{
        public class MapPixel
            : SingleBandPixel<ushort>
        {
        }

        //---------------------------------------------------------------------

        // Sample ecoregion map in Figure 3 of dual-scale document
        private DataGrid<EcoregionCode> Figure3Ecoregions;
        private ILandscape landscape;
        private MockInputRaster<MapPixel> inputMap;
        private Dictionary<Location, int> dataSharingBlocks;
        private int[,] expectedSiteOrder;
        private int actualSiteOrder;
        private MajorityRule.Delegates.RandomBetween originalRandomBetween;

        //---------------------------------------------------------------------

        [TestFixtureSetUp]
        public void Init()
        {
            const int G = 33; // green ecoregion
            const int T = 55; // tan ecoregion
            const int w = -1; // white ecoregion
            const int b = -4; // blue ecoregion
            Figure3Ecoregions = new DataGrid<EcoregionCode>(
                Data.MakeEcoregionCodes(new int[,]{
                    //1 2 3   4 5 6   7 8 9  101112
                    { G,G,G,  G,G,G,  b,b,b,  b,b,b }, // 1
                    { G,G,G,  G,G,G,  b,b,b,  b,b,b }, // 2
                    { G,G,G,  G,G,G,  b,b,b,  b,b,b }, // 3

                    { G,G,G,  G,G,G,  b,b,b,  T,T,T }, // 4
                    { G,G,G,  G,G,G,  T,T,T,  T,T,T }, // 5
                    { G,G,G,  G,T,T,  T,T,T,  T,T,T }, // 6

                    { w,w,w,  T,T,T,  T,T,T,  b,b,b }, // 7
                    { w,w,w,  T,T,T,  T,T,b,  b,b,b }, // 8
                    { w,w,w,  T,T,T,  T,b,b,  b,b,b }  // 9
                }));
            landscape = new Landscape(Figure3Ecoregions, 3);

            ushort[,] mapCodes = new ushort[,]{
                    //  1   2   3     4   5   6     7   8   9    10  11  12
                    {   1,  2,  3,    4,  4,  4,  107,108,109,  110,111,112 }, // 1
                    {   1,  2,  3,    4,  5,  5,  207,208,209,  210,211,212 }, // 2
                    {   1,  2,  3,    5,  5,  5,  307,308,309,  310,311,312 }, // 3

                    {   0,  0,  0,  404,405,406,  407,408,409,    7,  8,  8 }, // 4
                    {   0,  0,  0,  504,505,506,  507,508,509,    7,  9,  8 }, // 5
                    {   0,  0,  0,  604,605,606,  607,608,609,    7,  7,  8 }, // 6

                    { 701,702,703,   21, 22, 23,  707,708,709,  710,711,712 }, // 7
                    { 801,802,803,   24, 25, 26,  807,808,809,  810,811,812 }, // 8
                    { 901,902,903,   27, 28, 28,  907,908,909,  910,911,912 }, // 9
                };
            inputMap = new MockInputRaster<MapPixel>(mapCodes);
            dataSharingBlocks = new Dictionary<Location, int>();
            dataSharingBlocks[ new Location(1, 1) ] = -2; // high parameter to RandomBetween = 2
            dataSharingBlocks[ new Location(1, 2) ] = 5;  // expected map code
            dataSharingBlocks[ new Location(2, 1) ] = 0;  // expected map code
            dataSharingBlocks[ new Location(2, 4) ] = -1; // high parameter to RandomBetween = 1
            dataSharingBlocks[ new Location(3, 2) ] = 28; // expected map code

            expectedSiteOrder = new int[,]{
                    //  1   2   3     4   5   6     7   8   9    10  11  12
                    {  -1, -1, -1,   -1, -1, -1,   -2, -2, -2,   -2, -2, -2 }, // 1
                    {  -1, -1, -1,   -1, -1, -1,   -2, -2, -2,   -2, -2, -2 }, // 2
                    {  -1, -1,  1,   -1, -1,  2,   -2, -2, -2,   -2, -2, -2 }, // 3

                    {  -1, -1, -1,    3,  4,  5,   -2, -2, -2,   -1, -1, -1 }, // 4
                    {  -1, -1, -1,    6,  7,  8,    9, 10, 11,   -1, -1, -1 }, // 5
                    {  -1, -1, 12,   13, 14, 15,   16, 17, 18,   -1, -1, 19 }, // 6

                    {  -2, -2, -2,   -1, -1, -1,   20, 21, 22,   -2, -2, -2 }, // 7
                    {  -2, -2, -2,   -1, -1, -1,   23, 24, -2,   -2, -2, -2 }, // 8
                    {  -2, -2, -2,   -1, -1, 25,   26, -2, -2,   -2, -2, -2 }, // 9
            };
            actualSiteOrder = 0;

            originalRandomBetween = MajorityRule.RandomlySelectBetween;
            MajorityRule.RandomlySelectBetween = RandomBetween;
        }

		//---------------------------------------------------------------------

        public void InitializeSite(ActiveSite activeSite,
                                   ushort     mapCode)
        {
            actualSiteOrder++;
            Assert.AreEqual(expectedSiteOrder[activeSite.Location.Row - 1,
                                              activeSite.Location.Column - 1],
                            actualSiteOrder);
            if (activeSite.SharesData) {
                int expectedMapCode;
                Assert.IsTrue(dataSharingBlocks.TryGetValue(activeSite.BroadScaleLocation,
                                                            out expectedMapCode));
                if (expectedMapCode >= 0)
                    Assert.AreEqual(expectedMapCode, mapCode);
            }
            else {
                int expectedMapCode = activeSite.Location.Row * 100 + activeSite.Location.Column;
                Assert.AreEqual(expectedMapCode, mapCode);
            }
        }

		//---------------------------------------------------------------------

        public int RandomBetween(int low,
                                 int high)
        {
            Assert.AreEqual(0, low);
            int expectedHighNegated;
            ActiveSite currentActiveSite = landscape[inputMap.CurrentPixelLocation];
            Assert.IsTrue(dataSharingBlocks.TryGetValue(currentActiveSite.BroadScaleLocation,
                                                        out expectedHighNegated));
            Assert.AreEqual(-expectedHighNegated, high);
            return low;
        }

		//---------------------------------------------------------------------

		[Test]
		public void ReadWithMajorityRule()
		{
            InputMap.ReadWithMajorityRule(inputMap, landscape, InitializeSite);

            int maxSiteOrder = 0;
            for (int r = 0; r < expectedSiteOrder.GetLength(0); r++) {
                for (int c = 0; c < expectedSiteOrder.GetLength(1); c++) {
                    maxSiteOrder = System.Math.Max(maxSiteOrder, expectedSiteOrder[r,c]);
                }
            }
            Assert.AreEqual(maxSiteOrder, actualSiteOrder);
		}

		//---------------------------------------------------------------------

		[TestFixtureTearDown]
		public void TearDown()
		{
            MajorityRule.RandomlySelectBetween = originalRandomBetween;
		}
    }
}

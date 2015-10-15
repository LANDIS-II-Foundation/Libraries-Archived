using Edu.Wisc.Forest.Flel.Util;
using NUnit.Framework;
using Wisc.Flel.GeospatialModeling.Landscapes.DualScale;

namespace Landis.Test.Succession
{
    [TestFixture]
    public class DisturbedSiteEnumerator_Test
    {
        [Test]
        public void MixedLandscape()
        {
            ISiteVar<bool> disturbed = DisturbedSites.MixedLandscape.NewSiteVar<bool>();
            foreach (Location loc in DisturbedSites.Locations) {
                ActiveSite site = DisturbedSites.MixedLandscape[loc];
                Assert.IsNotNull(site);
                disturbed[site] = true;
            }

            DisturbedSiteEnumerator disturbedSites;
            disturbedSites = new DisturbedSiteEnumerator(DisturbedSites.MixedLandscape, disturbed);
            int count = 0;
            foreach (ActiveSite site in disturbedSites) {
                count++;
                Assert.IsTrue(count <= DisturbedSites.Locations.Length);
                Assert.AreEqual(DisturbedSites.Locations[count-1], site.Location);
            }
            Assert.AreEqual(count, DisturbedSites.Locations.Length);
        }
    }
}

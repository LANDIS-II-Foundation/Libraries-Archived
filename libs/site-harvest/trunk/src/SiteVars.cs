// This file is part of the Harvest library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest/trunk/

using Landis.Core;
using Landis.Library.AgeOnlyCohorts;
using Landis.SpatialModeling;

namespace Landis.Library.Harvest
{
    /// <summary>
    /// The library's site variables.
    /// </summary>
    public static class SiteVars
    {
        /// <summary>
        /// The site variable with cohorts (accessed as age-only cohorts).
        /// </summary>
        public static ISiteVar<ISiteCohorts> Cohorts { get; private set; }

        /// <summary>
        /// The management area for each site.
        /// </summary>
        public static ISiteVar<ManagementArea> ManagementArea { get; private set; }

        /// <summary>
        /// The stand that each site belongs to.
        /// </summary>
        public static ISiteVar<Stand> Stand { get; private set; }

        /// <summary>
        /// The prescription that was applied to a site during the current timestep.
        /// </summary>
        public static ISiteVar<Prescription> Prescription { get; private set; }

        /// <summary>
        /// The name of the prescription applied to a site during the current timestep.
        /// </summary>
        public static ISiteVar<string> PrescriptionName { get; private set; }

        /// <summary>
        /// The number of cohorts damaged at a site during the current timestep.
        /// </summary>
        public static ISiteVar<int> CohortsDamaged { get; private set; }

        /// <summary>
        /// The number of years since that last harvest event at a site.
        /// </summary>
        public static ISiteVar<int> TimeOfLastEvent { get; private set; }

        /// <summary>
        /// The number of years since the last fire event at a site.
        /// </summary>
        public static ISiteVar<int> TimeOfLastFire { get; private set; }

        /// <summary>
        /// The number of years since the last wind event at a site.
        /// </summary>
        public static ISiteVar<int> TimeOfLastWind { get; private set; }

        /// <summary>
        /// The CFS fuel type of a site.
        /// </summary>
        public static ISiteVar<int> CFSFuelType { get; private set; }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes the library's site variables.
        /// </summary>
        public static void Initialize()
        {
            Cohorts = Model.Core.GetSiteVar<ISiteCohorts>("Succession.AgeCohorts");

            ManagementArea   = Model.Core.Landscape.NewSiteVar<ManagementArea>();
            Stand            = Model.Core.Landscape.NewSiteVar<Stand>();
            Prescription     = Model.Core.Landscape.NewSiteVar<Prescription>();
            PrescriptionName = Model.Core.Landscape.NewSiteVar<string>();
            CohortsDamaged   = Model.Core.Landscape.NewSiteVar<int>();
            TimeOfLastEvent  = Model.Core.Landscape.NewSiteVar<int>();

            Model.Core.RegisterSiteVar(SiteVars.PrescriptionName, "Harvest.PrescriptionName");
            Model.Core.RegisterSiteVar(SiteVars.TimeOfLastEvent, "Harvest.TimeOfLastEvent");
            Model.Core.RegisterSiteVar(SiteVars.CohortsDamaged, "Harvest.CohortsDamaged");

            SiteVars.TimeOfLastEvent.ActiveSiteValues = -100;
            SiteVars.Prescription.SiteValues = null;

            TimeOfLastFire = Model.Core.GetSiteVar<int>("Fire.TimeOfLastEvent");
            TimeOfLastWind = Model.Core.GetSiteVar<int>("Wind.TimeOfLastEvent");
            CFSFuelType    = Model.Core.GetSiteVar<int>("Fuels.CFSFuelType");
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// (Why did someone write this method?  Why fetch a site variable from
        /// the core's registry at the start of each timestep?  Left it here
        /// because it was in Base Extension; really need to re-assess this
        /// some day...)
        /// </summary>
        public static void ReInitialize()
        {
            TimeOfLastFire = Model.Core.GetSiteVar<int>("Fire.TimeOfLastEvent");
            TimeOfLastWind = Model.Core.GetSiteVar<int>("Wind.TimeOfLastEvent");
            CFSFuelType = Model.Core.GetSiteVar<int>("Fuels.CFSFuelType");
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Get the age of the oldest cohort at a site. 
        /// </summary>
        public static int GetMaxAge(ActiveSite site)
        {
            ushort max = 0;

            foreach (ISpeciesCohorts speciesCohorts in Cohorts[site])
            {
                foreach (ICohort cohort in speciesCohorts)
                {
                    if (cohort.Age > max)
                        max = cohort.Age;
                }
            }
            return max;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Compute the number of years since the site was last damaged.
        /// </summary>
        public static int TimeSinceLastDamage(ActiveSite site)
        {
            int lastDamageTime = -100;

            if (SiteVars.TimeOfLastEvent[(Site)site] > lastDamageTime)
                lastDamageTime = SiteVars.TimeOfLastEvent[(Site)site];

            if (SiteVars.TimeOfLastFire != null)
                if (SiteVars.TimeOfLastFire[(Site)site] > lastDamageTime && SiteVars.TimeOfLastFire[(Site)site] > 0)
                    lastDamageTime = SiteVars.TimeOfLastFire[(Site)site];

            if (SiteVars.TimeOfLastWind != null)
                if (SiteVars.TimeOfLastWind[(Site)site] > lastDamageTime && SiteVars.TimeOfLastWind[(Site)site] > 0)
                    lastDamageTime = SiteVars.TimeOfLastWind[(Site)site];

            return Model.Core.CurrentTime - lastDamageTime;
        }
    }
}

// This file is part of the Base Harvest extension for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/trunk/base-harvest/trunk/

using Landis.SpatialModeling;
using Landis.Library.AgeOnlyCohorts;

namespace Landis.Extension.BaseHarvest
{
    public class SiteVars
    {
        private static ISiteVar<int> timeOfLastEvent;
        private static ISiteVar<ManagementArea> mgmtAreas;
        private static ISiteVar<Stand> stand;
        private static ISiteVar<Prescription> prescription;
        private static ISiteVar<string> prescription_name;
        private static ISiteVar<int> cohortsDamaged;
        private static ISiteVar<int> timeOfLastFire;
        private static ISiteVar<int> timeOfLastWind;
        private static ISiteVar<ISiteCohorts> cohorts;
        private static ISiteVar<int> cfsFuelType;

        //---------------------------------------------------------------------

        public static void Initialize()
        {
            timeOfLastEvent         = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            mgmtAreas               = PlugIn.ModelCore.Landscape.NewSiteVar<ManagementArea>();
            stand                   = PlugIn.ModelCore.Landscape.NewSiteVar<Stand>();
            prescription            = PlugIn.ModelCore.Landscape.NewSiteVar<Prescription>();
            prescription_name       = PlugIn.ModelCore.Landscape.NewSiteVar<string>();
            cohortsDamaged          = PlugIn.ModelCore.Landscape.NewSiteVar<int>();
            timeOfLastFire          = PlugIn.ModelCore.GetSiteVar<int>("Fire.TimeOfLastEvent");
            timeOfLastWind          = PlugIn.ModelCore.GetSiteVar<int>("Wind.TimeOfLastEvent");
            cfsFuelType             = PlugIn.ModelCore.GetSiteVar<int>("Fuels.CFSFuelType");

            PlugIn.ModelCore.RegisterSiteVar(SiteVars.PrescriptionName, "Harvest.PrescriptionName");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.TimeOfLastEvent, "Harvest.TimeOfLastEvent");
            PlugIn.ModelCore.RegisterSiteVar(SiteVars.CohortsDamaged, "Harvest.CohortsDamaged");

            SiteVars.TimeOfLastEvent.ActiveSiteValues = -100;
            SiteVars.Prescription.SiteValues = null;

            cohorts = PlugIn.ModelCore.GetSiteVar<ISiteCohorts>("Succession.AgeCohorts");
        
        }

        public static void ReInitialize()
        {
            timeOfLastFire = PlugIn.ModelCore.GetSiteVar<int>("Fire.TimeOfLastEvent");
            timeOfLastWind = PlugIn.ModelCore.GetSiteVar<int>("Wind.TimeOfLastEvent");
            cfsFuelType = PlugIn.ModelCore.GetSiteVar<int>("Fuels.CFSFuelType");
            
            //if (SiteVars.CFSFuelType == null)
            //    throw new System.ApplicationException("Error: CFS Fuel Type NOT Initialized.  Louise is making me crazy.  Fuel extension MUST be active.");
        }
        //---------------------------------------------------------------------
        public static ISiteVar<ISiteCohorts> Cohorts
        {
            get
            {
                return cohorts;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<ManagementArea> ManagementArea
        {
            get {
                return mgmtAreas;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<Stand> Stand
        {
            get {
                return stand;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<Prescription> Prescription
        {
            get {
                return prescription;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<string> PrescriptionName 
        {
            get {
                return prescription_name;
            }
        }

        //---------------------------------------------------------------------

        public static ISiteVar<int> TimeOfLastEvent 
        {
            get {
                return timeOfLastEvent;
            }
        }

        //---------------------------------------------------------------------
        public static ISiteVar<int> TimeOfLastFire
        {
            get
            {
                return timeOfLastFire;
            }
        }

        //---------------------------------------------------------------------
        public static ISiteVar<int> TimeOfLastWind
        {
            get
            {
                return timeOfLastWind;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<int> CFSFuelType
        {
            get
            {
                return cfsFuelType;
            }
        }
        //---------------------------------------------------------------------

        public static ISiteVar<int> CohortsDamaged
        {
            get {
                return cohortsDamaged;
            }
            set {
                cohortsDamaged = value;
            }
        }

        //---------------------------------------------------------------------
        public static int GetMaxAge(ActiveSite site)
        {
            if (SiteVars.Cohorts[site] == null)
            {
                PlugIn.ModelCore.UI.WriteLine("Cohort are null.  Why?");
                return 0;
            }
            ushort max = 0;

            foreach (ISpeciesCohorts speciesCohorts in SiteVars.Cohorts[site])
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
        public static int TimeSinceLastDamage(ActiveSite site)
        {

            int lastDamageTime = -100;

            if(SiteVars.TimeOfLastEvent[(Site) site] > lastDamageTime)
                lastDamageTime = SiteVars.TimeOfLastEvent[(Site)site];

            if (SiteVars.TimeOfLastFire != null)
                if (SiteVars.TimeOfLastFire[(Site)site] > lastDamageTime && SiteVars.TimeOfLastFire[(Site) site] > 0)
                    lastDamageTime = SiteVars.TimeOfLastFire[(Site)site];

            if (SiteVars.TimeOfLastWind != null)
                if (SiteVars.TimeOfLastWind[(Site)site] > lastDamageTime && SiteVars.TimeOfLastWind[(Site) site] > 0)
                    lastDamageTime = SiteVars.TimeOfLastWind[(Site)site];

            return PlugIn.ModelCore.CurrentTime - lastDamageTime;
        }

    }
}

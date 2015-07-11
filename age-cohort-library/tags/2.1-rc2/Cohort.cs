//using Landis.Cohorts.TypeIndependent;
//using TypeIndependent = Landis.Cohorts.TypeIndependent;
using Landis.Core;
using Landis.SpatialModeling;

namespace Landis.Library.AgeOnlyCohorts
{
    /// <summary>
    /// A species cohort with only age information.
    /// </summary>
    public class Cohort
        : ICohort
    {
        private ISpecies species;
        private ushort age;

        //---------------------------------------------------------------------

        public ISpecies Species
        {
            get {
                return species;
            }
        }

        //---------------------------------------------------------------------

        public ushort Age
        {
            get {
                return age;
            }
        }


        //---------------------------------------------------------------------

        public Cohort(ISpecies species,
                      ushort   age)
        {
            this.species = species;
            this.age = age;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Occurs when a cohort dies either due to senescence or disturbances.
        /// </summary>
        public static event DeathEventHandler<DeathEventArgs> DeathEvent;

        //---------------------------------------------------------------------

        /// <summary>
        /// Raises a Cohort.DeathEvent.
        /// </summary>
        public static void Died(object     sender,
                                ICohort    cohort,
                                ActiveSite site,
                                ExtensionType disturbanceType)
        {
            if (DeathEvent != null)
                DeathEvent(sender, new DeathEventArgs(cohort, site, disturbanceType));
        }
    }
}

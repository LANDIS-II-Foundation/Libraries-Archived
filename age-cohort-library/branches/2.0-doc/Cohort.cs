using Landis.Cohorts.TypeIndependent;
using TypeIndependent = Landis.Cohorts.TypeIndependent;
using Landis.Landscape;
using Landis.PlugIns;
using Landis.Species;

namespace Landis.AgeCohort
{
    /// <summary>
    /// A species cohort with only age information.
    /// </summary>
    public class Cohort
        : ICohort, TypeIndependent.ICohort
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

        public static readonly CohortAttribute AgeAttribute = new CohortAttribute("Age");
        public static readonly CohortAttribute[] Attributes = new CohortAttribute[]{ AgeAttribute };

        //---------------------------------------------------------------------

        object TypeIndependent.ICohort.this[CohortAttribute attribute]
        {
            get {
                if (attribute == AgeAttribute)
                    return age;
                return null;
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
        public static event Cohorts.DeathEventHandler<DeathEventArgs> DeathEvent;

        //---------------------------------------------------------------------

        /// <summary>
        /// Raises a Cohort.DeathEvent.
        /// </summary>
        public static void Died(object     sender,
                                ICohort    cohort,
                                ActiveSite site,
                                PlugInType disturbanceType)
        {
            if (DeathEvent != null)
                DeathEvent(sender, new DeathEventArgs(cohort, site, disturbanceType));
        }
    }
}

using Landis.Cohorts.TypeIndependent;
using TypeIndependent = Landis.Cohorts.TypeIndependent;
using Landis.Landscape;
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
		/// A method that is called for a cohort and its site.
		/// </summary>
		public delegate void SiteMethod(ICohort    cohort,
		                                ActiveSite site);

		//---------------------------------------------------------------------

		/// <summary>
		/// The method that is called when a cohort dies.
		/// </summary>
		public static SiteMethod Death = DefaultDeath;

		//---------------------------------------------------------------------

		public static void DefaultDeath(ICohort    cohort,
		                                ActiveSite site)
		{
			//  Do nothing.
		}
	}
}

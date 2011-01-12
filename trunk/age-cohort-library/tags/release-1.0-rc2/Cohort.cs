using Landis.Landscape;
using Landis.Species;

namespace Landis.AgeCohort
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
		/// A method that is called for a cohort and its site.
		/// </summary>
		public delegate void SiteMethod(ICohort    cohort,
		                                ActiveSite site);

		//---------------------------------------------------------------------

		/// <summary>
		/// The method that is called when a cohort dies due to senescence.
		/// </summary>
		public static SiteMethod SenescenceDeath = DefaultSenescenceDeath;

		//---------------------------------------------------------------------

		public static void DefaultSenescenceDeath(ICohort    cohort,
		                                          ActiveSite site)
		{
			//  Do nothing.
		}
	}
}

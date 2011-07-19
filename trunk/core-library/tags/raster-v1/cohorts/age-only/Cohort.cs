using Landis.Landscape;
using Landis.Species;

namespace Landis.AgeOnly
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
		/// The method that is called when a cohort dies.
		/// </summary>
		public static DeathMethod<ICohort> Died = DefaultDiedMethod;

		//---------------------------------------------------------------------

		public static void DefaultDiedMethod(ICohort    cohort,
		                                     ActiveSite site)
		{
			//  Do nothing.
		}
	}
}

using Landis.Cohorts;
using Landis.Landscape;
using Landis.Species;
using System.Collections;
using System.Collections.Generic;

namespace Landis.AgeCohort
{
	public class SiteCohorts
		: ISiteCohorts<ICohort>, IEnumerable<ISpeciesCohorts<ICohort>>
	{
		private List<SpeciesCohorts> cohorts;

		//---------------------------------------------------------------------

		public ISpeciesCohorts<ICohort> this[ISpecies species]
		{
			get {
				for (int i = 0; i < cohorts.Count; i++) {
					SpeciesCohorts speciesCohorts = cohorts[i];
					if (speciesCohorts.Species == species)
						return speciesCohorts;
				}
				return null;
			}
		}

		//---------------------------------------------------------------------

		public SiteCohorts()
		{
			this.cohorts = new List<SpeciesCohorts>();
		}

		//---------------------------------------------------------------------

		public SiteCohorts(IEnumerable<ISpeciesCohorts<ICohort>> cohorts)
		{
			this.cohorts = new List<SpeciesCohorts>();
			foreach (ISpeciesCohorts<ICohort> speciesCohorts in cohorts) {
				this.cohorts.Add(new SpeciesCohorts(speciesCohorts));
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Grows all the cohorts by advancing their ages.
		/// </summary>
		/// <param name="years">
		/// The number of years to advance each cohort's age.
		/// </param>
		/// <param name="site">
		/// The site where the cohorts are located.
		/// </param>
		/// <param name="successionTimestep">
		/// Indicates whether the current timestep is a succession timestep.
		/// If so, then all young cohorts (i.e., those whose ages are less than
		/// or equal to the succession timestep are combined into a single
		/// cohort whose age is the succession timestep.
		/// </param>
		public void Grow(ushort        years,
		                 ActiveSite    site,
		                 int?          successionTimestep)
		{
			//  Go through list of species cohorts from back to front so that
			//	a removal does not mess up the loop.
			for (int i = cohorts.Count - 1; i >= 0; i--) {
				cohorts[i].Grow(years, site, successionTimestep);
				if (cohorts[i].Count == 0)
					cohorts.RemoveAt(i);
			}
		}

		//---------------------------------------------------------------------

		public void Remove(SelectMethod<ICohort> selectMethod)
		{
			//  Go through list of species cohorts from back to front so that
			//	a removal does not mess up the loop.
			for (int i = cohorts.Count - 1; i >= 0; i--) {
				cohorts[i].Remove(selectMethod);
				if (cohorts[i].Count == 0)
					cohorts.RemoveAt(i);
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Adds a new cohort for a particular species.
		/// </summary>
		public void AddNewCohort(ISpecies species)
		{
			for (int i = 0; i < cohorts.Count; i++) {
				SpeciesCohorts speciesCohorts = cohorts[i];
				if (speciesCohorts.Species == species) {
					speciesCohorts.AddNewCohort();
					return;
				}
			}

			//  Species not present at the site.
			cohorts.Add(new SpeciesCohorts(species));
		}

		//---------------------------------------------------------------------

		public bool IsMaturePresent(ISpecies species)
		{
			for (int i = 0; i < cohorts.Count; i++) {
				SpeciesCohorts speciesCohorts = cohorts[i];
				if (speciesCohorts.Species == species) {
					return speciesCohorts.IsMaturePresent;
				}
			}
			return false;
		}

		//---------------------------------------------------------------------

		public IEnumerator<ISpeciesCohorts<ICohort>> GetEnumerator()
		{
			foreach (ISpeciesCohorts<ICohort> speciesCohorts in cohorts)
				yield return speciesCohorts;
		}
 
		//---------------------------------------------------------------------

        IEnumerator IEnumerable.GetEnumerator()
        {
        	return GetEnumerator();
        }
	}
}

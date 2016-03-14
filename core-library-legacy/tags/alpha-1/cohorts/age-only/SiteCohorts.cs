using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;
using Landis.Species;
using System.Collections.Generic;

namespace Landis.AgeOnly
{
	public class SiteCohorts
		: ISiteCohorts<ICohort>, IEnumerable<ISpeciesCohorts<ICohort>>, IEnumerable<ISpecies>
	{
		private List<ISpeciesCohorts<ICohort>> cohorts;

		//---------------------------------------------------------------------

		public IEnumerable<ISpeciesCohorts<ICohort>> BySpecies
		{
			get {
				return this;
			}
		}

		//---------------------------------------------------------------------

		IEnumerator<ISpeciesCohorts<ICohort>> IEnumerable<ISpeciesCohorts<ICohort>>.GetEnumerator()
		{
			foreach (ISpeciesCohorts<ICohort> speciesCohorts in cohorts)
				yield return speciesCohorts;
		}

		//---------------------------------------------------------------------

		public IEnumerable<ISpecies> SpeciesPresent
		{
			get {
				return this;
			}
		}

		//---------------------------------------------------------------------

		IEnumerator<ISpecies> IEnumerable<ISpecies>.GetEnumerator()
		{
			foreach (SpeciesCohorts speciesCohorts in cohorts)
				yield return speciesCohorts.Species;
		}

		//---------------------------------------------------------------------

		public ISpeciesCohorts<ICohort> this[ISpecies species]
		{
			get {
				foreach (SpeciesCohorts speciesCohorts in cohorts)
					if (speciesCohorts.Species == species)
						return speciesCohorts;
				return null;
			}
		}

		//---------------------------------------------------------------------

		public SiteCohorts()
		{
			this.cohorts = new List<ISpeciesCohorts<ICohort>>();
		}

		//---------------------------------------------------------------------

		public SiteCohorts(IEnumerable<ISpeciesCohorts<ICohort>> cohorts)
		{
			this.cohorts = new List<ISpeciesCohorts<ICohort>>();
			foreach (ISpeciesCohorts<ICohort> speciesCohorts in cohorts) {
				this.cohorts.Add(new SpeciesCohorts(speciesCohorts));
			}
		}

		//---------------------------------------------------------------------

		public void Grow(ushort     years,
		                 ActiveSite site,
		                 int?       successionTimestep)
		{
			//  Go through list of species cohorts from back to front so that
			//	a removal does not mess up the loop.
			foreach (int i in Indexes.Of(cohorts).Reverse) {
				cohorts[i].Grow(years, site, successionTimestep);
				if (cohorts[i].Count == 0)
					cohorts.RemoveAt(i);
			}
		}

		//---------------------------------------------------------------------

		public void Remove(SelectMethod<ICohort> selectMethod,
		                   ActiveSite            site)
		{
			//  Go through list of species cohorts from back to front so that
			//	a removal does not mess up the loop.
			foreach (int i in Indexes.Of(cohorts).Reverse) {
				cohorts[i].Remove(selectMethod, site);
				if (cohorts[i].Count == 0)
					cohorts.RemoveAt(i);
			}
		}

		//---------------------------------------------------------------------

		public void AddNewCohort(ISpecies species)
		{
			foreach (ISpeciesCohorts<ICohort> speciesCohorts in cohorts) {
				if (speciesCohorts.Species == species) {
					speciesCohorts.AddNewCohort();
					return;
				}
			}

			//  Species not present at the site.
			cohorts.Add(new SpeciesCohorts(species));
		}

		//---------------------------------------------------------------------

		public IEnumerator<ICohort> GetEnumerator()
		{
			foreach (ISpeciesCohorts<ICohort> speciesCohorts in cohorts)
				foreach (Cohort cohort in speciesCohorts)
					yield return cohort;
		}
	}
}

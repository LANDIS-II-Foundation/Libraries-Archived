using Edu.Wisc.Forest.Flel.Util;
using Landis.Landscape;
using Landis.Species;
using System.Collections.Generic;

namespace Landis.AgeOnly
{
	/// <summary>
	/// The cohorts for a particular species at a site.
	/// </summary>
	public class SpeciesCohorts
		: ISpeciesCohorts<ICohort>, IEnumerable<ushort>
	{
		private ISpecies species;
		private List<ushort> ages;

		//---------------------------------------------------------------------

		public int Count
		{
			get {
				return ages.Count;
			}
		}

		//---------------------------------------------------------------------

		public ISpecies Species
		{
			get {
				return species;
			}
		}

		//---------------------------------------------------------------------

		public IEnumerable<ushort> Ages
		{
			get {
				return this;
			}
		}

		//---------------------------------------------------------------------

		IEnumerator<ushort> IEnumerable<ushort>.GetEnumerator()
		{
			foreach (ushort age in ages)
				yield return age;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance with one young cohort (age = 1).
		/// </summary>
		public SpeciesCohorts(ISpecies species)
		{
			this.species = species;
			this.ages = new List<ushort>();
			AddNewCohort();
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance with a list of ages.
		/// </summary>
		public SpeciesCohorts(ISpecies     species,
		                      List<ushort> ages)
		{
			this.species = species;
			this.ages = new List<ushort>(ages);
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Initializes a new instance by copying a set of species cohorts.
		/// </summary>
		public SpeciesCohorts(ISpeciesCohorts<ICohort> cohorts)
		{
			this.species = cohorts.Species;
			this.ages = new List<ushort>();
			foreach (ushort age in cohorts.Ages)
				this.ages.Add(age);
		}

		//---------------------------------------------------------------------

		public void AddNewCohort()
		{
			this.ages.Add(1);
		}

		//---------------------------------------------------------------------

		public void Grow(ushort     years,
		                 ActiveSite site,
		                 int?       successionTimestep)
		{
			//  Update ages
			foreach (int i in Indexes.Of(ages)) {
				if (successionTimestep.HasValue && (ages[i] < successionTimestep.Value))
					// Young cohort
					ages[i] = (ushort) successionTimestep.Value;
				else
					ages[i] += years;
			}

			//  Combine young cohorts if succession timestep
			if (successionTimestep.HasValue) {
				//  Go backwards through list of ages, so the removal of an age
				//	doesn't mess up the loop.
				bool left1YoungCohort = false;
				foreach (int i in Indexes.Of(ages).Reverse) {
					if (ages[i] == successionTimestep.Value) {
						// Young cohort
						if (left1YoungCohort)
							ages.RemoveAt(i);
						else
							left1YoungCohort = true;
					}
				}
			}

			//	Now go through ages and check for senescence.  Again go
			//  backwards through the list, so the removal of an age
			//	doesn't mess up the loop.
			foreach (int i in Indexes.Of(ages).Reverse) {
				if (ages[i] > species.Longevity) {
					ushort age = ages[i];
					ages.RemoveAt(i);
					Cohort.Died(new Cohort(species, age), site);
				}
			}
		}

		//---------------------------------------------------------------------

		public void Remove(SelectMethod<ICohort> selectMethod,
		                   ActiveSite            site)
		{
			//  Go backwards through list of ages, so the removal of an age
			//	doesn't mess up the loop.
			foreach (int i in Indexes.Of(ages).Reverse) {
				ICohort cohort = new Cohort(species, ages[i]);
				if (selectMethod(cohort)) {
					ages.RemoveAt(i);
					Cohort.Died(cohort, site);
				}
			}
		}

		//---------------------------------------------------------------------

		public IEnumerator<ICohort> GetEnumerator()
		{
			foreach (ushort age in ages)
				yield return new Cohort(species, age);
		}
	}
}

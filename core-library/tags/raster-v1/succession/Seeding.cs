using Landis.Landscape;
using Landis.Species;
using Landis.Util;

using System.Collections.Generic;

namespace Landis.Succession
{
	public class Seeding
		: ISeeding
	{
		private SeedDisperal.Method seedDispersalMethod;
		private List<RelativeLocation>[] neighborhoods;

		//---------------------------------------------------------------------

		public Seeding(SeedDisperal.Method seedDispersal)
		{
			this.seedDispersalMethod = seedDispersal;

			//  Initialize neighborhoods for each species
			neighborhoods = new List<RelativeLocation>[Model.Species.Count];
			foreach (ISpecies species in Model.Species) {
				List<RelativeLocation> neighborhood = new List<RelativeLocation>();
				neighborhoods[species.Index] = neighborhood;
				// using species.MaxSeed, determine the list of relative
				// locations that represents the species' seeding neighborhood.
				// Need to use Model.CellLength to do computation.
				//
				// Possible enhancement.  Rather than store list of relative
				// locations, perhaps a row offset, and a range of column offsets.
				// For example, row offset = -6 (6 rows up), column offsets from
				// -11 to 11 (the site 11 columns to left & 6 rows up to the
				// site 11 columns to right & 6 rows up).  Actually if the
				// start & end column offsets are the same (only differ in sign),
				// could just store start column offset (end offset = -start offset).
				// Enhancement saves space, but would need to alter the foreach
				// loop in the Seeds method.
			}
		}

		//---------------------------------------------------------------------

		public bool Seeds(ISpecies   species,
		                  ActiveSite site)
		{
			if (! Reproduction.SufficientLight(species, site) ||
			    ! Reproduction.Establish(species, site))
				return false;

			double randomNum = Random.GenerateUniform();
			foreach (RelativeLocation relLoc in neighborhoods[species.Index]) {
				ActiveSite neighbor = site.GetNeighbor(relLoc) as ActiveSite;
				if (neighbor != null) {
					float probability = seedDispersalMethod(species, neighbor, site);
					if (randomNum < probability)
						return true;
				}
			}
			return false;
		}
	}
}

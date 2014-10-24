// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

using Landis.Library.SiteHarvest;
using System;
using System.Collections.Generic;

namespace Landis.Library.HarvestManagement {
	/// <summary>
	/// a struct for the inclusion-rule type
	/// to hold all parameters
	/// </summary>
	public struct InclusionRule {
		private string inclusion_type;
		private AgeRange age_range;
		private double percentOfCells;
		private List<string> species_list;
		private List<int> species_index_list;
		
		//---------------------------------------------------------------------
		
		/// <summary>
		/// constructor, assign members of the stucture
		/// </summary>
		public InclusionRule(string inclusion_type,
	                            AgeRange age_range,
	                            string temp_percent,
	                            List<string> species_list) {

			//assign members of the struct		
			this.inclusion_type = inclusion_type;
			this.age_range = age_range;
			//check for type 'percentage' by looking for the % character
			string [] split = temp_percent.Split(new char [] {'%'});
			//try to make a percentage.  if this doesn't work then check for keyword 'highest'
			try {
				percentOfCells = ((double) Convert.ToInt32(split[0])) / 100;
			}
			catch (Exception) {
				//and set percentOfCells to -1 (the flag for InclusionRequirement to handle)
				percentOfCells = -1;
			}
			this.species_list = species_list;
			//get the species index list using species name
			this.species_index_list = new List<int>();
			foreach (string species in species_list) {
                if (Model.Core.Species[species] != null)
                {
                    this.species_index_list.Add(Model.Core.Species[species].Index);
				}				
			}
			//Model.Core.UI.WriteLine("species index = {0}", this.species_index);
		}
		
		//---------------------------------------------------------------------
		
		/// <summary>
		/// return inclusion type
		/// </summary>
		
		public string InclusionType {
			get {
				return this.inclusion_type;
			}
		}

		//---------------------------------------------------------------------
		
		/// <summary>
		/// return age_range
		/// </summary>
		
		public AgeRange RuleAgeRange {
			get {
				return this.age_range;
			}
		}
		
		//---------------------------------------------------------------------
		
		/// <summary>
		/// return percentOfCells
		/// </summary>
		
		public double PercentOfCells {
			get {
				return this.percentOfCells;
			}
		}
		
		//---------------------------------------------------------------------
		
		/// <summary>
		/// return species list
		/// </summary>
		
		public List<string> SpeciesList {
			get {
				return this.species_list;
			}
		}
		
		//---------------------------------------------------------------------
		
		/// <summary>
		/// return species index list
		/// </summary>
		
		public List<int> SpeciesIndexList {
			get {
				return species_index_list;
			}
		}
	}	
}
//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Library.BiomassCohortsPnET
{
    /// <summary>
    /// A species cohort with biomass information.
    /// </summary>
    public class Cohort : Landis.Library.BiomassCohorts.Cohort, Landis.Library.BiomassCohorts.ICohort
    {
        
        float fol;
        float wood;
        float root;
        float nsc;
        float folshed;

        public Cohort(ISpecies species, int biomass, ushort age) : base(species, new BiomassCohorts.CohortData(age,biomass))
        { 
        
        }
       
        public float FolShed
        {
            get
            {
                return folshed;
            }
            set
            {
                folshed = value;
            }
        }
       
        public new int Biomass
        {
            get
            {
                return (int)(wood + fol);
            }
        }
        public float Fol
        {
            get
            {
                return fol;
            }
            set
            {
                fol = value;
            }
        }
        public float Wood
        {
            get
            {
                return wood;
            }
            set
            {
                wood = value;
            }
        }

        public float Root
        {
            get
            {
                return root;
            }
            set
            {
                root = value;
            }
        }
         
        public new int ComputeNonWoodyBiomass(ActiveSite site)
        {
            return (int)(Fol + Root);
        }
        
        
        public float NSC
        {
            get
            {
                return nsc;
            }
            set
            {
                nsc = value;
            }
        }
        
      
         
    }
}

using Landis.Core;
using Landis.SpatialModeling;
using Landis.Library.InitialCommunities;
using Landis.Library.BiomassCohortsPnET;
using System.Collections.Generic;
using Edu.Wisc.Forest.Flel.Util;
using System;
using System.Linq;

namespace Landis.Library.BiomassCohortsPnET
{
    public class PnETSpecies  
    {
        public Landis.Core.ISpecies species;
        public readonly float CFracBiomass;
        public readonly float SLWDel;
        public readonly float SLWmax;
        public readonly float FracFol;
        public readonly float DNSC;
        public readonly float FracBelowG;
        public readonly float TOroot;
        public readonly float TOwood;
        public readonly float TOfol;
        public readonly float PsnAgeRed;
        public readonly float BFolResp;
        public readonly float KWdLit;
        public readonly float FrActWd;
        public readonly float InitialNSC;
        public readonly bool PreventEstablishment;
        public readonly float EstRad;
        public readonly float EstMoist;
        public readonly float PsnTMin;
        public readonly float HalfSat;
        public readonly float FolLignin;
        public readonly float K;
         
        public readonly int H2;
        public readonly int H3;
        public readonly int H4;
        public readonly float FolN;
        public readonly float MaintResp;
        public readonly float PsnTOpt;
        public readonly float DVPD1;
        public readonly float DVPD2;

        public readonly float WUEcnst;
        public readonly float AmaxA;
        public readonly float AmaxB;
        public readonly float Q10;
                     
        public Landis.Core.ISpecies Species
        {
            get
            {
                return species;
            }
        }
         

        public float VegReprodProb 
        {
            get
            {
                return species.VegReprodProb;
            }
        }
        public byte ShadeTolerance
        {
            get
            {
                return species.ShadeTolerance;
            }
        }
        public int EffectiveSeedDist
        {
            get
            {
                return species.EffectiveSeedDist;
            }
        }
         
        public byte FireTolerance
        {
            get
            {
                return species.FireTolerance;
            }
        }
        public int Longevity
        {
            get
            {
                return species.Longevity;
            }
        }
        public int Maturity
        {
            get
            {
                return species.Maturity;
            }
        }
        public int MaxSeedDist
        {
            get
            {
                return species.MaxSeedDist;
            }
        }
        public int MaxSproutAge
        {
            get
            {
                return species.MaxSproutAge;
            }
        }
        public int MinSproutAge
        {
            get
            {
                return species.MinSproutAge;
            }
        }
        public string Name
        {
            get
            {
                return species.Name;
            }
        }
        public int Index
        {
            get
            {
                return species.Index;
            }
        }
        public PostFireRegeneration PostFireRegeneration
        {
            get
            {
                return species.PostFireRegeneration;
            }
        }
        public static List<string> ParameterNames
        {
            get
            {
                return typeof(PnETSpecies).GetFields().Select(x => x.Name).ToList();
            }
        }
       
        public PnETSpecies(ISpecies species, 
                       float FolN,
                        float MaintResp,
                        float PsnTOpt,
                        float DVPD1,
                        float DVPD2,
                        float WUEcnst,
                        float AmaxA,
                        float AmaxB,
                        float Q10,
                       float CFracBiomass, 
                       float SLWDel, 
                       float SLWmax, 
                       float FracFol, 
                       float DNSC, 
                       float FracBelowG,
                       float TOroot,
                       float TOwood,
                       float TOfol,
                       float PsnAgeRed,
                       float BFolResp,
                       float KWdLit,
                       float FrActWd,
                       float InitialNSC,
                       bool PreventEstablishment,
                       float EstRad,
                       float EstMoist,
                       float PsnTMin,
                       float HalfSat,
                       float FolLignin,
                       float K,
                       int H2,
                       int H3,
                       int H4
                     )
        {
            
            this.species = species;
             this.FolN = FolN;
            this.MaintResp =MaintResp ;
            this.PsnTOpt =PsnTOpt;
            this.DVPD1 =DVPD1;
            this.DVPD2 =DVPD2;
            this.WUEcnst =WUEcnst ;
            this.AmaxA =AmaxA ;
            this.AmaxB = AmaxB;
            this.Q10 =Q10;
            this.CFracBiomass=CFracBiomass;
            this.SLWDel = SLWDel;
            this.SLWmax = SLWmax;
            this.FracFol = FracFol;
            this.DNSC =DNSC;
            this.FracBelowG = FracBelowG;
            this.TOroot = TOroot;
            this.TOwood = TOwood;
            this.TOfol = TOfol;
            this.PsnAgeRed = PsnAgeRed;
            this.BFolResp = BFolResp;
            this.KWdLit = KWdLit;
            this.FrActWd = FrActWd;
            this.InitialNSC = InitialNSC;
            this.PreventEstablishment = PreventEstablishment;
            this.EstRad = EstRad;
            this.EstMoist = EstMoist;
            this.PsnTMin = PsnTMin;
            this.HalfSat = HalfSat;
            this.FolLignin = FolLignin;
            this.K = K;
            this.H2 = H2;
            this.H3 = H3;
            this.H4 = H4;
             
        }
    }
}

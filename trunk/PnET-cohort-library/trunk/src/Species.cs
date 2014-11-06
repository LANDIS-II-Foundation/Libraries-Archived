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
    public class Species :  ISpecies
    {
        private Landis.Core.ISpecies species;
         
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
        
        public float CFracBiomass { get; private set; }
        public float SLWDel { get; private set; }
        public float SLWmax { get; private set; }
        public float FracFol { get; private set; }
        public float DNSC { get; private set; }
        public float FracBelowG { get; private set; }
        public float TOroot { get; private set; }
        public float TOwood { get; private set; }
        public float TOfol { get; private set; }
        public float PsnAgeRed { get; private set; }
        public float BFolResp { get; private set; }
        public float KWdLit { get; private set; }
        public float FrActWd { get; private set; }
        public float InitialNSC { get; private set; }
        public bool PreventEstablishment { get; private set; }
        public float EstRad { get; private set; }
        public float EstMoist { get; private set; }
        public float PsnTMin { get; private set; }
        public float HalfSat { get; private set; }
        public float FolLignin { get; private set; }
        public float K { get; private set; }
        public int H2 { get; private set; }
        public int H3 { get; private set; }
        public int H4 { get; private set; }
        
        
        public Species(ISpecies species, 
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

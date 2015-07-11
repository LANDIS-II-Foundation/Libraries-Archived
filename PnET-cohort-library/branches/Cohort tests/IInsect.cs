//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

using Edu.Wisc.Forest.Flel.Util;
using Landis.SpatialModeling;
using Landis.Core;
using System.Collections.Generic;

namespace Landis.Extension.Insects
{
    /// <summary>
    /// Parameters for the extension.
    /// </summary>
    public interface IInsect
    {
        string Name {get;set;}
        double MeanDuration {get;set;}
        int StdDevDuration {get;set;}
        int MeanTimeBetweenOutbreaks {get;set;}
        int StdDevTimeBetweenOutbreaks {get;set;}
        int NeighborhoodDistance {get;set;}

        double InitialPatchShapeCalibrator {get;set;}
        double InitialPatchOutbreakSensitivity { get; set; }
        DistributionType InitialPatchDistr { get; set; }
        double InitialPatchValue1 {get;set;}
        double InitialPatchValue2 {get;set;}

        int OutbreakStartYear {get;set;}
        int OutbreakStopYear {get;set;}
        int MortalityYear {get; set;}
        
        // ARJAN
        Landis.Library.Biomass.Species.AuxParm<int> Susceptibility { get; set; }
        Landis.Library.Biomass.Species.AuxParm<double> GrowthReduceSlope { get; set; }
        Landis.Library.Biomass.Species.AuxParm<double> GrowthReduceIntercept { get; set; }
        Landis.Library.Biomass.Species.AuxParm<double> MortalitySlope { get; set; }
        Landis.Library.Biomass.Species.AuxParm<double> MortalityIntercept { get; set; }
        
        List<ISusceptible> SusceptibleTable{get;set;}
        IEnumerable<RelativeLocation> Neighbors{get;set;}

        ISiteVar<bool> Disturbed{get;set;}
        ISiteVar<Dictionary<int,double[]>> HostDefoliationByYear{get;set;}
        ISiteVar<double> LastYearDefoliation{get;set;}
        ISiteVar<double> ThisYearDefoliation{get;set;}
        ISiteVar<double> NeighborhoodDefoliation{ get; set; }
        ISiteVar<int> BiomassRemoved { get; set; }
        ISiteVar<double> InitialOutbreakProb { get; set; }

        bool ActiveOutbreak{get;set;}
        //BRM
        int InitialSites { get; set; }
        int LastStartYear { get; set; }
        int LastStopYear { get; set; }
        int LastBioRemoved { get; set; }
        string AnnMort { get; set; }


    }
}

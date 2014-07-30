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

        List<ISppParameters> SppTable{get;set;}
        List<ISusceptible> SusceptibleTable{get;set;}
        IEnumerable<RelativeLocation> Neighbors{get;set;}

        //ISiteVar<byte> Severity{get;set;}
        ISiteVar<bool> Disturbed{get;set;}
        ISiteVar<Dictionary<int,double[]>> HostDefoliationByYear{get;set;}
        ISiteVar<double> LastYearDefoliation{get;set;}
        ISiteVar<double> ThisYearDefoliation{get;set;}
        ISiteVar<double> NeighborhoodDefoliation{ get; set; }

        bool ActiveOutbreak{get;set;}
        //BRM
        int InitialSites { get; set; }
        int LastStartYear { get; set; }
        int LastStopYear { get; set; }
        int LastBioRemoved { get; set; }
        string AnnMort { get; set; }


    }
}

namespace Landis.Extension.Insects
{
    /// <summary>
    /// Parameters for the plug-in.
    /// </summary>
    public class InsectParameters
        : IInsect
    {
        private string name;
        private double meanDuration;
        private int stdDevDuration;
        private int meanTimeBetweenOutbreaks;
        private int stdDevTimeBetweenOutbreaks;
        private int neighborhoodDistance;

        private double initialPatchShapeCalibrator;
        private double initialPatchOutbreakSensitivity;
        private DistributionType initialPatchDistr;
        private double initialPatchValue1;
        private double initialPatchValue2;

        private int outbreakStartYear;
        private int outbreakStopYear;
        private int mortalityYear;

        // BRM
        private int initialSites;
        private int lastStartYear;
        private int lastStopYear;
        private int lastBioRemoved;

        private bool activeOutbreak;

        private List<ISppParameters> sppTable;
        private List<ISusceptible> susceptibleTable;
        IEnumerable<RelativeLocation> neighbors;

        private ISiteVar<Dictionary<int, double[]>> hostDefoliationByYear;
        private ISiteVar<bool> disturbed;
        private ISiteVar<double> lastYearDefoliation;
        private ISiteVar<double> thisYearDefoliation;
        private ISiteVar<double> neighborhoodDefoliation;

        private string annMort;

        //---------------------------------------------------------------------

        public string Name
        {
            get {
                return name;
            }
            set {
                name = value;
            }
        }

        //---------------------------------------------------------------------
        public double MeanDuration
        {
            get {
                return meanDuration;
            }
            set {
                if (value <= 0)
                    throw new InputValueException(value.ToString(), "Value must be  > 0.");
                meanDuration = value;
            }
        }
        //---------------------------------------------------------------------
        public int StdDevDuration
        {
            get {
                return stdDevDuration;
            }
            set {
                if (value < 0)
                        throw new InputValueException(value.ToString(),
                                                      "Value must be  > 0.");
                 stdDevDuration = value;
            }
        }
        //---------------------------------------------------------------------
        public int MeanTimeBetweenOutbreaks
        {
            get {
                return meanTimeBetweenOutbreaks;
            }
            set {
                if (value <= 0)
                        throw new InputValueException(value.ToString(), "Value must be  > 0.");
                 meanTimeBetweenOutbreaks = value;
            }
        }
        //---------------------------------------------------------------------
        public int StdDevTimeBetweenOutbreaks
        {
            get {
                return stdDevTimeBetweenOutbreaks;
            }
            set {
                if (value < 0)
                        throw new InputValueException(value.ToString(), "Value must be  > 0.");
                 stdDevTimeBetweenOutbreaks = value;
            }
        }

        //---------------------------------------------------------------------
        public int NeighborhoodDistance
        {
            get {
                return neighborhoodDistance;
            }
            set {
                if (value < 0)
                        throw new InputValueException(value.ToString(), "Value must be  > or = 0.");
                 neighborhoodDistance = value;
            }
        }
        //---------------------------------------------------------------------
        public double InitialPatchShapeCalibrator
        {
            get {
                return initialPatchShapeCalibrator;
            }
            set {
                 if (value > 1.0 || value < 0.0)
                        throw new InputValueException(value.ToString(), "Value must be  < 1.0 and > 0.0.");
                 initialPatchShapeCalibrator = value;
            }
        }
        //---------------------------------------------------------------------
        public double InitialPatchOutbreakSensitivity
        {
            get
            {
                return initialPatchOutbreakSensitivity;
            }
            set
            {
                if (value <= 0)
                    throw new InputValueException(value.ToString(), "Value must be  > 0.");
                initialPatchOutbreakSensitivity = value;
            }
        }
        //---------------------------------------------------------------------
        public DistributionType InitialPatchDistr
        {
            get {
                return initialPatchDistr;
            }
            set {
                 initialPatchDistr = value;
            }
        }
        //---------------------------------------------------------------------
        public double InitialPatchValue1
        {
            get {
                return initialPatchValue1;
            }
            set {
                 if (value <= 0)
                        throw new InputValueException(value.ToString(), "Value must be  > 0.");
                 initialPatchValue1 = value;
            }
        }
        //---------------------------------------------------------------------
        public double InitialPatchValue2
        {
            get {
                return initialPatchValue2;
            }
            set {
                 if (value <= 0)
                        throw new InputValueException(value.ToString(),
                                                      "Value must be  > 0.");
                 initialPatchValue2 = value;
            }
        }
        //---------------------------------------------------------------------
        public int OutbreakStartYear
        {
            get {
                return outbreakStartYear;
            }
            set {
                outbreakStartYear = value;
            }
        }

        //---------------------------------------------------------------------
        public int OutbreakStopYear
        {
            get {
                return outbreakStopYear;
            }
            set {
                outbreakStopYear = value;
            }
        }
        //---------------------------------------------------------------------
        public int MortalityYear
        {
            get {
                return mortalityYear;
            }
            set {
                mortalityYear = value;
            }
        }
        //---------------------------------------------------------------------
        public bool ActiveOutbreak
        {
            get {
                return activeOutbreak;
            }
            set {
                activeOutbreak = value;
            }
        }
        //---------------------------------------------------------------------
        public List<ISppParameters> SppTable
        {
            get {
                return sppTable;
            }
            set {
                sppTable = value;
            }
        }
        //---------------------------------------------------------------------
        public List<ISusceptible> SusceptibleTable
        {
            get {
                return susceptibleTable;
            }
            set {
                susceptibleTable = value;
            }
        }

        //---------------------------------------------------------------------
        public IEnumerable<RelativeLocation> Neighbors
        {
            get {
                return neighbors;
            }
            set {
                neighbors = value;
            }
        }

        //---------------------------------------------------------------------
        public ISiteVar<Dictionary<int, double[]>> HostDefoliationByYear
        {
            get {
                return hostDefoliationByYear;
            }
            set {
                hostDefoliationByYear = value;
            }
        }
        //---------------------------------------------------------------------
        public ISiteVar<bool> Disturbed
        {
            get {
                return disturbed;
            }
            set {
                disturbed = value;
            }
        }
        //---------------------------------------------------------------------
        public ISiteVar<double> LastYearDefoliation
        {
            get {
                return lastYearDefoliation;
            }
            set {
                lastYearDefoliation = value;
            }
        }
        //---------------------------------------------------------------------
        public ISiteVar<double> ThisYearDefoliation
        {
            get {
                return thisYearDefoliation;
            }
            set {
                thisYearDefoliation = value;
            }
        }
        //---------------------------------------------------------------------
        public ISiteVar<double> NeighborhoodDefoliation
        {
            get {
                return neighborhoodDefoliation;
            }
            set
            {
                neighborhoodDefoliation = value;
            }
        }

        //---------------------------------------------------------------------
        public int InitialSites
        {
            get
            {
                return initialSites;
            }
            set
            {
                initialSites = value;
            }
        }
        //---------------------------------------------------------------------
        public int LastStartYear
        {
            get
            {
                return lastStartYear;
            }
            set
            {
                lastStartYear = value;
            }
        }
        //---------------------------------------------------------------------
        public int LastStopYear
        {
            get
            {
                return lastStopYear;
            }
            set
            {
                lastStopYear = value;
            }
        }
        //---------------------------------------------------------------------
        public int LastBioRemoved
        {
            get
            {
                return lastBioRemoved;
            }
            set
            {
                lastBioRemoved = value;
            }
        }
        //---------------------------------------------------------------------
        public string AnnMort
        {
            get
            {
                return annMort;
            }
            set
            {
                if (value != "Annual" && value != "7Year")
                    throw new InputValueException(value.ToString(),
                                                  "Value must be  either Annual or 7Year.");
                annMort = value;
            }
        }
        //---------------------------------------------------------------------
        public InsectParameters(int sppCount)
        {
            sppTable = new List<ISppParameters>(sppCount);
            susceptibleTable = new List<ISusceptible>();
            neighbors = new List<RelativeLocation>();

            hostDefoliationByYear = PlugIn.ModelCore.Landscape.NewSiteVar<Dictionary<int, double[]>>();
            disturbed = PlugIn.ModelCore.Landscape.NewSiteVar<bool>();
            lastYearDefoliation = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            thisYearDefoliation = PlugIn.ModelCore.Landscape.NewSiteVar<double>();
            neighborhoodDefoliation = PlugIn.ModelCore.Landscape.NewSiteVar<double>();

            outbreakStopYear = 0;  //default = beginning of simulation
            outbreakStartYear = 0;  //default = beginning of simulation
            mortalityYear = 0;  //default = beginning of simulation
            activeOutbreak = false;
            initialSites = 0;
            lastStartYear = 0;
            lastStopYear = 0;
            lastBioRemoved = 0;
            annMort = "";
            
            //Initialize outbreaks:
            foreach (ActiveSite site in PlugIn.ModelCore.Landscape)
            {
                hostDefoliationByYear[site] = new Dictionary<int, double[]>();
            }
        }
    }
}

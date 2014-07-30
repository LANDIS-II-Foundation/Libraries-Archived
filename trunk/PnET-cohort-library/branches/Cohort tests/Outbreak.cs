//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

using Landis.SpatialModeling;
//using Landis.Extension.Succession.Biomass;
using Landis.Core;
using Landis.Library.BiomassCohorts;
using System.Collections.Generic;
using System;


namespace Landis.Extension.Insects
{
    public class Outbreak

    {

        private IInsect outbreakParms;

        //collect all 8 relative neighbor locations in array
        private static RelativeLocation[] all_neighbor_locations = new RelativeLocation[]
        {
                new RelativeLocation(-1,0),
                new RelativeLocation(1,0),
                new RelativeLocation(0,-1),
                new RelativeLocation(0,1),
                //new RelativeLocation(-1,-1),
                //new RelativeLocation(-1,1),
                //new RelativeLocation(1,-1),
                //new RelativeLocation(1,1)
        };

        //---------------------------------------------------------------------
        // Outbreak Constructor
        public Outbreak(IInsect insect)
        {
            this.outbreakParms = insect;
        }


        //---------------------------------------------------------------------
        ///<summary>
        // Go through all active sites and damage them.  Mortality should occur the year FOLLOWING an active year.
        ///</summary>
        public static void Mortality(IInsect insect)
        {
            
            //PlugIn.ModelCore.UI.WriteLine("   {0} mortality.  StartYear={1}, StopYear={2}, CurrentYear={3}.", insect.Name, insect.OutbreakStartYear, insect.OutbreakStopYear, PlugIn.ModelCore.CurrentTime);

            foreach (ActiveSite site in PlugIn.ModelCore.Landscape) 
            {
                PartialDisturbance.ReduceCohortBiomass(site);
                    
                if (insect.BiomassRemoved[site] > 0) 
                {
                    //PlugIn.ModelCore.UI.WriteLine("  Biomass removed at {0}/{1}: {2}.", site.Location.Row, site.Location.Column, SiteVars.BiomassRemoved[site]);
                    SiteVars.TimeOfLastEvent[site] = PlugIn.ModelCore.CurrentTime;
                } 
            }
        }


        //---------------------------------------------------------------------
        // Initialize landscape with patches of defoliation during the first year
        public static void InitializeDefoliationPatches(IInsect insect)
        {

            PlugIn.ModelCore.UI.WriteLine("   Initializing Defoliation Patches... ");   
            insect.InitialOutbreakProb.ActiveSiteValues = 0.0;
            insect.Disturbed.ActiveSiteValues = false;
            
            foreach(ActiveSite site in PlugIn.ModelCore.Landscape)
            {
            
                double suscIndexSum = 0.0;
                double sumBio = 0.0;
                double protectBiomass = 0.0;

                Landis.Library.BiomassCohorts.ISiteCohorts siteCohorts = SiteVars.Cohorts[site];

                foreach (ISpeciesCohorts speciesCohorts in siteCohorts)
                {
                    foreach (ICohort cohort in speciesCohorts) 
                    {
                        int sppSuscIndex = insect.Susceptibility[cohort.Species];
                        if (sppSuscIndex == 4)
                        {
                            protectBiomass += cohort.Biomass;
                            sppSuscIndex = 3;
                        }
                        suscIndexSum += cohort.Biomass * (sppSuscIndex);
                        sumBio += cohort.Biomass;
                    }
                }
                
                
                // If no biomass, no chance of defoliation, go to the next site.
                if(suscIndexSum <= 0 || sumBio <=0)
                {
                    insect.InitialOutbreakProb[site] = 0.0;
                    continue;
                }
                
                int siteSuscIndex = (int) Math.Round(suscIndexSum /sumBio) - 1;
                double protectProp = protectBiomass / sumBio;
                
                if (siteSuscIndex > 2.0 || siteSuscIndex < 0)
                {
                    PlugIn.ModelCore.UI.WriteLine("SuscIndex < 0 || > 2.  Site R/C={0}/{1},suscIndex={2},suscIndexSum={3},sumBio={4}.", site.Location.Row, site.Location.Column, siteSuscIndex,suscIndexSum,sumBio);
                    throw new ApplicationException("Error: SuscIndex is not between 2.0 and 0.0");
                }
                // Assume that there are no neighbors whatsoever:
                DistributionType dist = insect.SusceptibleTable[siteSuscIndex].Distribution_80.Name;


                //PlugIn.ModelCore.UI.WriteLine("suscIndex={0},suscIndexSum={1},cohortBiomass={2}.", suscIndex,suscIndexSum,sumBio);
                double value1 = insect.SusceptibleTable[siteSuscIndex].Distribution_80.Value1;
                double value2 = insect.SusceptibleTable[siteSuscIndex].Distribution_80.Value2;

                double probability = Distribution.GenerateRandomNum(dist, value1, value2);
                
                // Account for protective effect of Susc Class 4
                if (protectProp > 0)
                {
                    double slope = 1.0;
                    double protectReduce = 1- (protectProp * slope);
                    if (protectReduce > 1)
                        protectReduce = 1;
                    if (protectReduce < 0)
                        protectReduce = 0;
                    probability = probability * protectReduce;
                }
                if(probability > 1.0 || probability < 0)
                {
                    PlugIn.ModelCore.UI.WriteLine("Initial Defoliation Probility < 0 || > 1.  Site R/C={0}/{1}.", site.Location.Row, site.Location.Column);
                    throw new ApplicationException("Error: Probability is not between 1.0 and 0.0");
                }
                
                insect.InitialOutbreakProb[site] = probability; // This probability reflects the protective effects
                //PlugIn.ModelCore.UI.WriteLine("Susceptiblity index={0}.  Outbreak Probability={1:0.00}.  R/C={2}/{3}.", suscIndex, probability, site.Location.Row, site.Location.Column);
            }

            foreach(ActiveSite site in PlugIn.ModelCore.Landscape)
            {

                //get a random site from the stand
                double randomNum = PlugIn.ModelCore.GenerateUniform();
                double randomNum2 = PlugIn.ModelCore.GenerateUniform();
                
                //Create random variability in outbreak area within a simulation so outbreaks are more variable.
                double initialAreaCalibratorRandomNum = (randomNum2 - 0.5) * insect.InitialPatchOutbreakSensitivity / 2;

                //Start spreading!
                if (randomNum < insect.InitialOutbreakProb[site] * (insect.InitialPatchOutbreakSensitivity + initialAreaCalibratorRandomNum))  
                //if(randomNum < SiteVars.InitialOutbreakProb[site] * insect.InitialPatchOutbreakSensitivity)  
                {
            
                    //start with this site (if it's active)
                    ActiveSite currentSite = site;           
            
                    //queue to hold sites to defoliate
                    Queue<ActiveSite> sitesToConsider = new Queue<ActiveSite>();
            
                    //put initial site on queue
                    sitesToConsider.Enqueue(currentSite);
            
                    DistributionType dist = insect.InitialPatchDistr;
                    double targetArea = Distribution.GenerateRandomNum(dist, insect.InitialPatchValue1, insect.InitialPatchValue2);
                    
                    //PlugIn.ModelCore.UI.WriteLine("  Target Patch Area={0:0.0}.", targetArea);
                    double areaSelected = 0.0;
            
                    //loop through stand, defoliating patches of size target area
                    while (sitesToConsider.Count > 0 && areaSelected < targetArea) 
                    {

                        currentSite = sitesToConsider.Dequeue();
                    
                        // Because this is the first year, neighborhood defoliaiton is given a value.
                        // The value is used in Defoliate.DefoliateCohort()
                        insect.NeighborhoodDefoliation[currentSite] = insect.InitialOutbreakProb[currentSite];
                        areaSelected += PlugIn.ModelCore.CellArea;
                        insect.Disturbed[currentSite] = true;

                        //Next, add site's neighbors to the list of
                        //sites to consider.  
                        //loop through the site's neighbors enqueueing all the good ones.

                        //double maxNeighborProb = 0.0;
                        //Site maxNeighbor = currentSite;
                        //bool foundNewNeighbor = false;

                        foreach (RelativeLocation loc in all_neighbor_locations) 
                        {
                            Site neighbor = currentSite.GetNeighbor(loc);

                            //get a neighbor site (if it's non-null and active)
                            if (neighbor != null 
                                && neighbor.IsActive  
                                && !sitesToConsider.Contains((ActiveSite) neighbor)
                                && !insect.Disturbed[neighbor]) 
                            {
                                //insect.Disturbed[currentSite] = true;
                                randomNum = PlugIn.ModelCore.GenerateUniform();

                                /*if (SiteVars.InitialOutbreakProb[neighbor] > maxNeighborProb)
                                {
                                    maxNeighbor = currentSite.GetNeighbor(loc);
                                    maxNeighborProb = SiteVars.InitialOutbreakProb[neighbor];
                                    foundNewNeighbor = true;
                                }*/
                                
                                //check if it's a valid neighbor:
                                if (insect.InitialOutbreakProb[neighbor] * insect.InitialPatchShapeCalibrator > randomNum)
                                {
                                    sitesToConsider.Enqueue((ActiveSite) neighbor);
                                }
                            }
                        }

                        //if(foundNewNeighbor)
                        //    sitesToConsider.Enqueue((ActiveSite) maxNeighbor);

                    } 
                    
                    //PlugIn.ModelCore.UI.WriteLine("   Initial Patch Area Selected={0:0.0}.", areaSelected);
                } 

            } 
        } 
    }
    
}


//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

//using Landis.Extension.Succession.Biomass;
using Landis.Core;
using Landis.SpatialModeling;
using System.Collections.Generic;
using System;


namespace Landis.Extension.Insects
{
    public class Defoliate
    {

        private static IEnumerable<IInsect> manyInsect;
        //---------------------------------------------------------------------

        public static void Initialize(IInputParameters parameters)
        {
            manyInsect = parameters.ManyInsect;

            // Assign the method below to the CohortDefoliation delegate in
            // biomass-cohorts/Biomass.CohortDefoliation.cs
            Landis.Library.Biomass.CohortDefoliation.Compute = Landis.Extension.Insects.Defoliate.DefoliateCohort;
            //Landis.Library.LeafBiomassCohorts.CohortDefoliation.Compute = Landis.Extension.Insects.Defoliate.DefoliateCohort;

        }


        //---------------------------------------------------------------------
        // This method replaces the delegate method.  It is called every year when
        // ACT_ANPP is calculated, for each cohort.  Therefore, this method is operating at
        // an ANNUAL time step and separate from the normal extension time step.

        public static double DefoliateCohort(Landis.Library.BiomassCohorts.ICohort cohort, ActiveSite site, int siteBiomass)
        {
            // This maintains backwards compatibility with succession versions that don't use Biomass Library
            // but the functions must be sure to provide siteBiomass not cohortBiomass
            double defoliation = Landis.Extension.Insects.Defoliate.DefoliateCohort(site, cohort.Species, cohort.Biomass, siteBiomass);
            return defoliation;
           
        }
        

        //---------------------------------------------------------------------
        // This method replaces the delegate method.  It is called every year when
        // ACT_ANPP is calculated, for each cohort.  Therefore, this method is operating at
        // an ANNUAL time step and separate from the normal extension time step.

        public static double DefoliateCohort(ActiveSite site, ISpecies species, int cohortBiomass, int siteBiomass)
        {
            //PlugIn.ModelCore.UI.WriteLine("   Calculating insect defoliation...");

            double totalDefoliation = 0.0;  // Cohort total defoliation

            foreach (IInsect insect in manyInsect)
            {
                if (!insect.ActiveOutbreak)
                    continue;
                double defoliation = 0.0;

                // Calculate biomass proportion of "protective" species
                double protectBiomass = 0;
                foreach (Landis.Library.BiomassCohorts.ISpeciesCohorts spp in SiteVars.Cohorts[site])
                {
                    foreach (Landis.Library.BiomassCohorts.ICohort spp_cohort in spp)
                    {
                        if (insect.Susceptibility[spp_cohort.Species] == 4)
                            protectBiomass += spp_cohort.Biomass;
                    }
                }
                double protectProp = protectBiomass / (double)siteBiomass;

                int suscIndex = insect.Susceptibility[species] - 1;


                if (suscIndex < 0) suscIndex = 0;
                if (suscIndex > 2) suscIndex = 2;

                // Get the Neighborhood GrowthReduction Density
                double meanNeighborhoodDefoliation = 0.0;
                int neighborCnt = 0;

                // If it is the first year, the neighborhood growth reduction
                // will have been initialized in Outbreak.InitializeDefoliationPatches

                if (insect.NeighborhoodDefoliation[site] > 0)
                {
                    //PlugIn.ModelCore.UI.WriteLine("   First Year of Defoliation:  Using initial patch defo={0:0.00}.", SiteVars.NeighborhoodDefoliation[site]);
                    meanNeighborhoodDefoliation = insect.NeighborhoodDefoliation[site];
                }

                // If not the first year, calculate mean neighborhood defoliation based on the
                // previous year.
                else
                {
                    double sumNeighborhoodDefoliation = 0.0;

                    //PlugIn.ModelCore.UI.WriteLine("Look at the Neighbors... ");
                    foreach (RelativeLocation relativeLoc in insect.Neighbors)
                    {
                        Site neighbor = site.GetNeighbor(relativeLoc);
                        if (neighbor != null && neighbor.IsActive)
                        {
                            neighborCnt++;

                            // The previous year...
                            //if(SiteVars.DefoliationByYear[neighbor].ContainsKey(PlugIn.ModelCore.CurrentTime - 1))
                            //    sumNeighborhoodDefoliation += SiteVars.DefoliationByYear[neighbor][PlugIn.ModelCore.CurrentTime - 1];

                            // BUG - This does not sum values in the nbrhd, just uses the last nbr value
                            //sumNeighborhoodDefoliation = Math.Min(1.0, insect.LastYearDefoliation[neighbor]);
                            // BUG FIX - BRM
                            sumNeighborhoodDefoliation += Math.Min(1.0, insect.LastYearDefoliation[neighbor]);
                        }
                    }

                    if (neighborCnt > 0.0)
                        meanNeighborhoodDefoliation = sumNeighborhoodDefoliation / (double)neighborCnt;

                }  //endif

                if (meanNeighborhoodDefoliation > 1.0 || meanNeighborhoodDefoliation < 0)
                {
                    PlugIn.ModelCore.UI.WriteLine("MeanNeighborhoodDefoliation={0}; NeighborCnt={1}.", meanNeighborhoodDefoliation, neighborCnt);
                    throw new ApplicationException("Error: Mean Neighborhood GrowthReduction is not between 1.0 and 0.0");
                }

                // First assume that there are no neighbors whatsoever:
                DistributionType dist = insect.SusceptibleTable[suscIndex].Distribution_0.Name;
                double value1 = insect.SusceptibleTable[suscIndex].Distribution_0.Value1;
                double value2 = insect.SusceptibleTable[suscIndex].Distribution_0.Value2;

                if (meanNeighborhoodDefoliation <= 1.0 && meanNeighborhoodDefoliation >= 0.8)
                {
                    dist = insect.SusceptibleTable[suscIndex].Distribution_80.Name;
                    value1 = insect.SusceptibleTable[suscIndex].Distribution_80.Value1;
                    value2 = insect.SusceptibleTable[suscIndex].Distribution_80.Value2;
                }
                else if (meanNeighborhoodDefoliation < 0.8 && meanNeighborhoodDefoliation >= 0.6)
                {
                    dist = insect.SusceptibleTable[suscIndex].Distribution_60.Name;
                    value1 = insect.SusceptibleTable[suscIndex].Distribution_60.Value1;
                    value2 = insect.SusceptibleTable[suscIndex].Distribution_60.Value2;
                }
                else if (meanNeighborhoodDefoliation < 0.6 && meanNeighborhoodDefoliation >= 0.4)
                {
                    dist = insect.SusceptibleTable[suscIndex].Distribution_40.Name;
                    value1 = insect.SusceptibleTable[suscIndex].Distribution_40.Value1;
                    value2 = insect.SusceptibleTable[suscIndex].Distribution_40.Value2;
                }
                else if (meanNeighborhoodDefoliation < 0.4 && meanNeighborhoodDefoliation >= 0.2)
                {
                    dist = insect.SusceptibleTable[suscIndex].Distribution_20.Name;
                    value1 = insect.SusceptibleTable[suscIndex].Distribution_20.Value1;
                    value2 = insect.SusceptibleTable[suscIndex].Distribution_20.Value2;
                }
                else if (meanNeighborhoodDefoliation < 0.2 && meanNeighborhoodDefoliation >= 0.0)
                {
                    dist = insect.SusceptibleTable[suscIndex].Distribution_0.Name;
                    value1 = insect.SusceptibleTable[suscIndex].Distribution_0.Value1;
                    value2 = insect.SusceptibleTable[suscIndex].Distribution_0.Value2;
                }

                // Next, ensure that all cohorts of the same susceptibility class
                // receive the same level of defoliation.

                if (insect.HostDefoliationByYear[site].ContainsKey(PlugIn.ModelCore.CurrentTime))
                {
                    if (insect.HostDefoliationByYear[site][PlugIn.ModelCore.CurrentTime][suscIndex] <= 0.00000000)
                    {
                        defoliation = Distribution.GenerateRandomNum(dist, value1, value2);
                        // Account for protective effect of Susc Class 4
                        if (protectProp > 0)
                        {
                            double slope = 1.0;
                            double protectReduce = 1 - (protectProp * slope);
                            if (protectReduce > 1)
                                protectReduce = 1;
                            if (protectReduce < 0)
                                protectReduce = 0;
                            defoliation = defoliation * protectReduce;
                        }
                        insect.HostDefoliationByYear[site][PlugIn.ModelCore.CurrentTime][suscIndex] = defoliation;
                    }
                    else
                    {
                        defoliation = insect.HostDefoliationByYear[site][PlugIn.ModelCore.CurrentTime][suscIndex];
                    }
                }
                else
                {
                    insect.HostDefoliationByYear[site].Add(PlugIn.ModelCore.CurrentTime, new Double[3] { 0.0, 0.0, 0.0 });
                    defoliation = Distribution.GenerateRandomNum(dist, value1, value2);
                    //if (meanNeighborhoodDefoliation <= 0.0 && defoliation > 0.0)
                    //    PlugIn.ModelCore.UI.WriteLine("THAT'S WEIRD!!  meanNeighborhoodDefoliation = {0}, defoliation={1}.", meanNeighborhoodDefoliation, defoliation);
                    // Account for protective effect of Susc Class 4
                    if (protectProp > 0)
                    {
                        double slope = 1.0;
                        double protectReduce = 1 - (protectProp * slope);
                        if (protectReduce > 1)
                            protectReduce = 1;
                        if (protectReduce < 0)
                            protectReduce = 0;
                        defoliation = defoliation * protectReduce;
                    }
                    insect.HostDefoliationByYear[site][PlugIn.ModelCore.CurrentTime][suscIndex] = defoliation;
                }


                // Alternatively, allow defoliation to vary even among cohorts and species with
                // the same susceptibility.


                if (defoliation > 1.0 || defoliation < 0)
                {
                    PlugIn.ModelCore.UI.WriteLine("DEFOLIATION TOO BIG or SMALL:  {0}, {1}, {2}, {3}.", dist, value1, value2, defoliation);
                    throw new ApplicationException("Error: New defoliation is not between 1.0 and 0.0");
                }

                //PlugIn.ModelCore.UI.WriteLine("Cohort age={0}, species={1}, suscIndex={2}, defoliation={3}.", cohort.Age, cohort.Species.Name, (suscIndex -1), defoliation);

                // BUG - This is summed proportions across all cohorts (exceeds 1.0)
                //insect.ThisYearDefoliation[site] += defoliation;

                // BUG FIX - Weight site-level defoliation by cohort biomass
                double weightedDefoliation = (defoliation * Math.Min(1.0, (double)cohortBiomass/(double) siteBiomass));
                //PlugIn.ModelCore.UI.WriteLine("Cohort age={0}, species={1}, suscIndex={2}, cohortDefoliation={3}, weightedDefolation={4}.", cohort.Age, cohort.Species.Name, (suscIndex+1), defoliation, weightedDefoliation);
                insect.ThisYearDefoliation[site] += weightedDefoliation;


                totalDefoliation += defoliation;  // Cohort total defoliation (summed across insects)
            }

            if (totalDefoliation > 1.0)  // Cannot exceed 100% defoliation
                totalDefoliation = 1.0;

            if (totalDefoliation > 1.1 || totalDefoliation < 0)
            {
                PlugIn.ModelCore.UI.WriteLine("Cohort Total Defoliation = {0:0.00}.  Site R/C={1}/{2}.", totalDefoliation, site.Location.Row, site.Location.Column);
                throw new ApplicationException("Error: Total Defoliation is not between 1.1 and 0.0");
            }


            return totalDefoliation;  // Cohort total defoliation proportion (summed across insects)

        }


        // An option to use if cohort biomass is not available, but weighting is equal among all cohorts
        /*public static double DefoliateCohort(ActiveSite site, ISpecies species)
        {
             

            //PlugIn.ModelCore.UI.WriteLine("   Calculating insect defoliation...");

            double totalDefoliation = 0.0;  // Cohort total defoliation proportion
            
            // Count the number of cohorts on site for defoliation weighting
            int cohortCount  = 0;

            foreach (ISpeciesCohorts spp in SiteVars.Cohorts[site])
            {
                foreach (ICohort spp_cohort in spp)
                {
                    cohortCount += 1;
                }
            }
           // Calculate protective weighting here ??

            foreach(IInsect insect in manyInsect)
            {
                if(!insect.ActiveOutbreak)
                    continue;
                double defoliation = 0.0;

                double totalBiomass = 0;
                double protectBiomass = 0;
                foreach (ISpeciesCohorts spp in SiteVars.Cohorts[site])
                {
                    foreach (ICohort spp_cohort in spp)
                    {
                        if (insect.Susceptibility[spp_cohort.Species] == 4)
                            protectBiomass += spp_cohort.Biomass;
                        totalBiomass += spp_cohort.Biomass;
                    }
                }
                double protectProp = protectBiomass / totalBiomass;
                insect.ProtectProp[site] = protectProp;

                int suscIndex = insect.Susceptibility[species] - 1;

                if (suscIndex < 0) suscIndex = 0;
                if (suscIndex > 2) suscIndex = 2;

                // Get the Neighborhood GrowthReduction Density
                double meanNeighborhoodDefoliation = 0.0;
                int neighborCnt = 0;

                // If it is the first year, the neighborhood growth reduction
                // will have been initialized in Outbreak.InitializeDefoliationPatches

                if(insect.NeighborhoodDefoliation[site] > 0)
                {
                    //PlugIn.ModelCore.UI.WriteLine("   First Year of Defoliation:  Using initial patch defo={0:0.00}.", SiteVars.NeighborhoodDefoliation[site]);
                    meanNeighborhoodDefoliation = insect.NeighborhoodDefoliation[site];
                }

                // If not the first year, calculate mean neighborhood defoliation based on the
                // previous year.
                else
                {
                    double sumNeighborhoodDefoliation = 0.0;

                    //PlugIn.ModelCore.UI.WriteLine("Look at the Neighbors... ");
                    foreach (RelativeLocation relativeLoc in insect.Neighbors)
                    {
                        Site neighbor = site.GetNeighbor(relativeLoc);
                        if (neighbor != null && neighbor.IsActive)
                        {
                            neighborCnt++;

                            // The previous year...
                            //if(SiteVars.DefoliationByYear[neighbor].ContainsKey(PlugIn.ModelCore.CurrentTime - 1))
                            //    sumNeighborhoodDefoliation += SiteVars.DefoliationByYear[neighbor][PlugIn.ModelCore.CurrentTime - 1];
                            
                            // BUG - This does not sum values in the nbrhd, just uses the last nbr value
                            //sumNeighborhoodDefoliation = Math.Min(1.0, insect.LastYearDefoliation[neighbor]);
                            // BUG FIX - BRM
                            sumNeighborhoodDefoliation += Math.Min(1.0, insect.LastYearDefoliation[neighbor]);
                        }
                    }

                    if(neighborCnt > 0.0)
                        meanNeighborhoodDefoliation = sumNeighborhoodDefoliation / (double) neighborCnt;

                }  //endif

                if(meanNeighborhoodDefoliation > 1.0 || meanNeighborhoodDefoliation < 0)
                {
                    PlugIn.ModelCore.UI.WriteLine("MeanNeighborhoodDefoliation={0}; NeighborCnt={1}.", meanNeighborhoodDefoliation, neighborCnt);
                    throw new ApplicationException("Error: Mean Neighborhood GrowthReduction is not between 1.0 and 0.0");
                }

                // First assume that there are no neighbors whatsoever:
                DistributionType dist = insect.SusceptibleTable[suscIndex].Distribution_0.Name;
                double value1 = insect.SusceptibleTable[suscIndex].Distribution_0.Value1;
                double value2 = insect.SusceptibleTable[suscIndex].Distribution_0.Value2;

                if(meanNeighborhoodDefoliation <= 1.0 && meanNeighborhoodDefoliation >= 0.8)
                {
                    dist = insect.SusceptibleTable[suscIndex].Distribution_80.Name;
                    value1 = insect.SusceptibleTable[suscIndex].Distribution_80.Value1;
                    value2 = insect.SusceptibleTable[suscIndex].Distribution_80.Value2;
                }
                else if(meanNeighborhoodDefoliation < 0.8 && meanNeighborhoodDefoliation >= 0.6)
                {
                    dist = insect.SusceptibleTable[suscIndex].Distribution_60.Name;
                    value1 = insect.SusceptibleTable[suscIndex].Distribution_60.Value1;
                    value2 = insect.SusceptibleTable[suscIndex].Distribution_60.Value2;
                }
                else if(meanNeighborhoodDefoliation < 0.6 && meanNeighborhoodDefoliation >= 0.4)
                {
                    dist = insect.SusceptibleTable[suscIndex].Distribution_40.Name;
                    value1 = insect.SusceptibleTable[suscIndex].Distribution_40.Value1;
                    value2 = insect.SusceptibleTable[suscIndex].Distribution_40.Value2;
                }
                else if(meanNeighborhoodDefoliation < 0.4 && meanNeighborhoodDefoliation >= 0.2)
                {
                    dist = insect.SusceptibleTable[suscIndex].Distribution_20.Name;
                    value1 = insect.SusceptibleTable[suscIndex].Distribution_20.Value1;
                    value2 = insect.SusceptibleTable[suscIndex].Distribution_20.Value2;
                }
                else if(meanNeighborhoodDefoliation < 0.2 && meanNeighborhoodDefoliation >= 0.0)
                {
                    dist = insect.SusceptibleTable[suscIndex].Distribution_0.Name;
                    value1 = insect.SusceptibleTable[suscIndex].Distribution_0.Value1;
                    value2 = insect.SusceptibleTable[suscIndex].Distribution_0.Value2;
                }

                // Next, ensure that all cohorts of the same susceptibility class
                // receive the same level of defoliation.

                if(insect.HostDefoliationByYear[site].ContainsKey(PlugIn.ModelCore.CurrentTime))
                {
                    if (insect.HostDefoliationByYear[site][PlugIn.ModelCore.CurrentTime][suscIndex] <= 0.00000000)
                    {
                        defoliation = Distribution.GenerateRandomNum(dist, value1, value2);
                        // Adjust for protection
                        insect.HostDefoliationByYear[site][PlugIn.ModelCore.CurrentTime][suscIndex] = defoliation;
                    }
                    else
                    {
                        defoliation = insect.HostDefoliationByYear[site][PlugIn.ModelCore.CurrentTime][suscIndex];
                    }
                }
                else
                {
                    insect.HostDefoliationByYear[site].Add(PlugIn.ModelCore.CurrentTime, new Double[3]{0.0, 0.0, 0.0});
                    defoliation = Distribution.GenerateRandomNum(dist, value1, value2);
                    //if (meanNeighborhoodDefoliation <= 0.0 && defoliation > 0.0)
                    //    PlugIn.ModelCore.UI.WriteLine("THAT'S WEIRD!!  meanNeighborhoodDefoliation = {0}, defoliation={1}.", meanNeighborhoodDefoliation, defoliation);
                    // Adjust for protection
                    insect.HostDefoliationByYear[site][PlugIn.ModelCore.CurrentTime][suscIndex] = defoliation;
                }


                // Alternatively, allow defoliation to vary even among cohorts and species with
                // the same susceptibility.

              
                if(defoliation > 1.0 || defoliation < 0)
                {
                    PlugIn.ModelCore.UI.WriteLine("DEFOLIATION TOO BIG or SMALL:  {0}, {1}, {2}, {3}.", dist, value1, value2, defoliation);
                    throw new ApplicationException("Error: New defoliation is not between 1.0 and 0.0");
                }

                //PlugIn.ModelCore.UI.WriteLine("Cohort age={0}, species={1}, suscIndex={2}, defoliation={3}.", cohort.Age, cohort.Species.Name, (suscIndex -1), defoliation);

                //BUG - This is summed proportions across all cohorts (exceeds 1.0)
                //insect.ThisYearDefoliation[site] += defoliation;

                //BUG FIX - weight all cohorts equally
                double weightedDefoliation = (defoliation * Math.Min(1.0, (double) 1 / (double) cohortCount));
                //PlugIn.ModelCore.UI.WriteLine("Cohort age={0}, species={1}, suscIndex={2}, cohortDefoliation={3}, weightedDefolation={4}.", cohort.Age, cohort.Species.Name, (suscIndex+1), defoliation, weightedDefoliation);
                //insect.ThisYearDefoliation[site] += weightedDefoliation;

                totalDefoliation += defoliation;  // Cohort defoliation proportion summed across insects
            }

            if(totalDefoliation > 1.0)  // Cannot exceed 100% defoliation
                totalDefoliation = 1.0;

            if(totalDefoliation > 1.1 || totalDefoliation < 0)
            {
                PlugIn.ModelCore.UI.WriteLine("Cohort Total Defoliation = {0:0.00}.  Site R/C={1}/{2}.", totalDefoliation, site.Location.Row, site.Location.Column);
                throw new ApplicationException("Error: Total Defoliation is not between 1.1 and 0.0");
            }


            return totalDefoliation;

        }
         * */


    }

}

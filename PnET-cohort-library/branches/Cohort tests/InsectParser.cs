//  Copyright 2006-2011 University of Wisconsin, Portland State University
//  Authors:  Jane Foster, Robert M. Scheller

using Edu.Wisc.Forest.Flel.Util;
using Landis.Core;
using System.Collections.Generic;
using System.Text;
 

namespace Landis.Extension.Insects
{
    /// <summary>
    /// A parser that reads the extension parameters from text input.
    /// </summary>
    public class InsectParser
        : TextParser<IInsect>
    {
        //public static Species.IDataset SpeciesDataset = PlugIn.ModelCore.Species;

        //---------------------------------------------------------------------
        public InsectParser()
        {
            RegisterForInputValues();
        }

        //---------------------------------------------------------------------

        public override string LandisDataValue
        {
            get
            {
                return PlugIn.ExtensionName;
            }
        }
        private bool TrySet(string ReadLabel, string VariableLabel, InputValue<float> value, float mininput, float maxinput, ISpecies species, Landis.Library.Biomass.Species.AuxParm<float> SpcVar)
        {
            // VariableLabel has to occur in headers
            //AssureVariableHeaderIsThere(VariableLabel);
            if (System.String.Compare(ReadLabel, VariableLabel, System.StringComparison.OrdinalIgnoreCase) == 0)
            {
                SpcVar[species] = Landis.Library.Biomass.CheckParms.CheckBiomassParm(value, mininput, maxinput);
                return true;
            }
            return false;
        }
        protected override IInsect Parse()
        {

            InputVar<string> landisData = new InputVar<string>("LandisData");
            ReadVar(landisData);
            if (landisData.Value.Actual != "InsectDefoliator")
                throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", "InsectDefoliator");

            Insect insect = new Insect(PlugIn.ModelCore.Species.Count);

            InputVar<string> insectName = new InputVar<string>("InsectName");
            ReadVar(insectName);
            insect.Name = insectName.Value;

            InputVar<double> mD = new InputVar<double>("MeanDuration");
            ReadVar(mD);
            insect.MeanDuration = mD.Value;

            InputVar<int> mTBO = new InputVar<int>("MeanTimeBetweenOutbreaks");
            ReadVar(mTBO);
            insect.MeanTimeBetweenOutbreaks = mTBO.Value;

            InputVar<int> sdTBO = new InputVar<int>("StdDevTimeBetweenOutbreaks");
            ReadVar(sdTBO);
            insect.StdDevTimeBetweenOutbreaks = sdTBO.Value;

            InputVar<int> nhs = new InputVar<int>("NeighborhoodSize");
            ReadVar(nhs);
            insect.NeighborhoodDistance = nhs.Value;

            InputVar<double> ipsc = new InputVar<double>("InitialPatchShapeCalibrator");
            ReadVar(ipsc);
            insect.InitialPatchShapeCalibrator = ipsc.Value;

            InputVar<double> ipnc = new InputVar<double>("InitialPatchOutbreakSensitivity");
            ReadVar(ipnc);
            insect.InitialPatchOutbreakSensitivity = ipnc.Value;

            InputVar<DistributionType> ipdt = new InputVar<DistributionType>("InitialPatchDistribution");
            ReadVar(ipdt);
            insect.InitialPatchDistr = ipdt.Value;

            InputVar<double> ipv1 = new InputVar<double>("InitialPatchValue1");
            ReadVar(ipv1);
            insect.InitialPatchValue1 = ipv1.Value;

            InputVar<double> ipv2 = new InputVar<double>("InitialPatchValue2");
            ReadVar(ipv2);
            insect.InitialPatchValue2 = ipv2.Value;

            //--------- Read In Species Table ---------------------------------------
            PlugIn.ModelCore.UI.WriteLine("   Begin parsing SPECIES table.");

            ReadName("SpeciesParameters");

            InputVar<string> annMort = new InputVar<string>("MortalityEstimate");
            ReadVar(annMort);
            insect.AnnMort = annMort.Value;

            InputVar<string> sppName = new InputVar<string>("Species");
            InputVar<int> susc = new InputVar<int>("Species Susceptibility");
            InputVar<double> grs = new InputVar<double>("Growth Reduction Slope");
            InputVar<double> gri = new InputVar<double>("Growth Reduction Intercept");
            InputVar<double> msl = new InputVar<double>("Mortality Slope");
            InputVar<double> min = new InputVar<double>("Mortality Intercept");
            Dictionary <string, int> lineNumbers = new Dictionary<string, int>();

            const string Susceptiblities = "Susceptibilities";

            Landis.Library.Biomass.Species.AuxParm<bool> AccountedFor = new Library.Biomass.Species.AuxParm<bool>(PlugIn.ModelCore.Species);

            while (! AtEndOfInput && CurrentName != Susceptiblities) {
                StringReader currentLine = new StringReader(CurrentLine);


                ReadValue(sppName, currentLine);
                ISpecies species = PlugIn.ModelCore.Species[sppName.Value.Actual];

                if (species == null)
                    throw new InputValueException(sppName.Value.String,
                                                  "{0} is not a species name.",
                                                  sppName.Value.String);
                int lineNumber;
                if (lineNumbers.TryGetValue(species.Name, out lineNumber))
                    throw new InputValueException(sppName.Value.String,
                                                  "The species {0} was previously used on line {1} for insect "+ insect.Name,
                                                  sppName.Value.String, lineNumber);
                else
                    lineNumbers[species.Name] = LineNumber;

                ReadValue(susc, currentLine);
                insect.Susceptibility[species] = Landis.Library.Biomass.CheckParms.CheckBiomassParm(susc.Value, 1, 4);

                ReadValue(grs, currentLine);
                insect.GrowthReduceSlope[species] = Landis.Library.Biomass.CheckParms.CheckBiomassParm(grs.Value, double.MinValue, 0); 
                
                ReadValue(gri, currentLine);
                insect.GrowthReduceIntercept[species] = Landis.Library.Biomass.CheckParms.CheckBiomassParm(gri.Value, 0, double.MaxValue); 
              
                ReadValue(msl, currentLine);
                insect.MortalitySlope[species] = Landis.Library.Biomass.CheckParms.CheckBiomassParm(msl.Value, 0, double.MaxValue); 

                ReadValue(min, currentLine);
                insect.MortalityIntercept[species] = Landis.Library.Biomass.CheckParms.CheckBiomassParm(min.Value, 0, double.MaxValue);

                AccountedFor[species] = true;

                CheckNoDataAfter("the " + min.Name + " column",
                                 currentLine);

                GetNextLine();
            }
            foreach (ISpecies species in PlugIn.ModelCore.Species)
            {
                if (AccountedFor[species] == false) throw new System.Exception(species.Name + " is not parameterized for " + insect.Name);
            
            }

            //  Read table of Susceptibilities.
            //  Susceptibilities are in decreasing order.
            ReadName(Susceptiblities);
            const string MapNames  = "MapNames";

            InputVar<byte> number = new InputVar<byte>("Susceptibility Number");
            InputVar<DistributionType> dt = new InputVar<DistributionType>("Distribution");
            InputVar<double> v1 = new InputVar<double>("Distribution Value 1");
            InputVar<double> v2 = new InputVar<double>("Distribution Value 2");

            byte previousNumber = 0;

            while (! AtEndOfInput && CurrentName != MapNames)
            {
                StringReader currentLine = new StringReader(CurrentLine);

                ISusceptible susceptible = new Susceptible();

                insect.SusceptibleTable.Add(susceptible);

                ReadValue(number, currentLine);
                susceptible.Number = number.Value;

                //  Check that the current severity's number is 1 less than
                //  the previous number (numbers are must be in decreasing
                //  order).
                if (number.Value.Actual != previousNumber + 1)
                    throw new InputValueException(number.Value.String,
                                                  "Expected the severity number {0}",
                                                  previousNumber + 1);
                previousNumber = number.Value.Actual;

                IDistribution distribution = new Distribution();

                ReadValue(dt, currentLine);
                distribution.Name = dt.Value;

                ReadValue(v1, currentLine);
                distribution.Value1 = v1.Value;

                ReadValue(v2, currentLine);
                distribution.Value2 = v2.Value;

                susceptible.Distribution_80 = distribution; //.GetComplete();

                distribution = new Distribution();

                ReadValue(dt, currentLine);
                distribution.Name = dt.Value;

                ReadValue(v1, currentLine);
                distribution.Value1 = v1.Value;

                ReadValue(v2, currentLine);
                distribution.Value2 = v2.Value;

                susceptible.Distribution_60 = distribution; //.GetComplete();

                distribution = new Distribution();

                ReadValue(dt, currentLine);
                distribution.Name = dt.Value;

                ReadValue(v1, currentLine);
                distribution.Value1 = v1.Value;

                ReadValue(v2, currentLine);
                distribution.Value2 = v2.Value;

                susceptible.Distribution_40 = distribution; //.GetComplete();

                distribution = new Distribution();

                ReadValue(dt, currentLine);
                distribution.Name = dt.Value;

                ReadValue(v1, currentLine);
                distribution.Value1 = v1.Value;

                ReadValue(v2, currentLine);
                distribution.Value2 = v2.Value;

                susceptible.Distribution_20 = distribution; //.GetComplete();

                distribution = new Distribution();

                ReadValue(dt, currentLine);
                distribution.Name = dt.Value;

                ReadValue(v1, currentLine);
                distribution.Value1 = v1.Value;

                ReadValue(v2, currentLine);
                distribution.Value2 = v2.Value;

                susceptible.Distribution_0 = distribution; //.GetComplete();

                CheckNoDataAfter("the " + v2.Name + " column",
                                 currentLine);
                GetNextLine();
            }
            if (insect.SusceptibleTable.Count == 0)
                throw NewParseException("No susceptibilities defined.");

            return insect;//.GetComplete();

        }
        //---------------------------------------------------------------------

        private void CheckForRepeatedName(string      name,
                                          string      description,
                                          Dictionary<string, int> lineNumbers)
        {
            int lineNumber;
            if (lineNumbers.TryGetValue(name, out lineNumber))
                throw new InputValueException(name,
                                              "The {0} {1} was previously used on line {2}",
                                              description, name, lineNumber);
            lineNumbers[name] = LineNumber;
        }

        public static DistributionType DTParse(string word)
        {
            if (word == "Gamma")
                return DistributionType.Gamma;
            else if (word == "Beta")
                return DistributionType.Beta;
            else if (word == "Weibull")
                return DistributionType.Weibull;
            else
                throw new System.FormatException("Valid Distribution Types: Gamma, Beta, Weibull.");
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Registers the appropriate method for reading input values.
        /// </summary>
        public static void RegisterForInputValues()
        {
            Type.SetDescription<DistributionType>("Distribution Type");
            InputValues.Register<DistributionType>(DTParse);
        }

    }
}

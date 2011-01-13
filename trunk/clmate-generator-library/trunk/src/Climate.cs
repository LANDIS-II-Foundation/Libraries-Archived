//  Copyright 2009-2010 Portland State University, Conservation Biology Institute
//  Authors:  Robert M. Scheller

using Landis.Core;
using System.Collections.Generic;
using System.IO;
using System;

namespace Landis.Library.Climate
{

    public class Climate
    {
        private static Dictionary<int, IClimateRecord[,]> allData;
        private static IClimateRecord[,] timestepData;
        private static ICore modelCore;
        
        public Climate()
        {
        }

        //---------------------------------------------------------------------

        public static ICore ModelCore
        {
            get
            {
                return modelCore;
            }
        }

        public static Dictionary<int, IClimateRecord[,]> AllData
        {
            get {
                return allData;
            }
        }
        //---------------------------------------------------------------------
        public static IClimateRecord[,] TimestepData
        {
            get {
                return timestepData;
            }
            set {
                timestepData = value;
            }
        }
        
        public static void Write(IEcoregionDataset ecoregionDataset) 
        {
            foreach(IEcoregion ecoregion in ecoregionDataset)
            {
                for(int i = 0; i < 12; i++)
                {
                    ModelCore.Log.WriteLine("Eco={0}, Month={1}, AvgMinTemp={2:0.0}, AvgMaxTemp={3:0.0}, StdDevTemp={4:0.0}, AvgPpt={5:0.0}, StdDevPpt={6:0.0}.",
                        ecoregion.Index, i+1, 
                        TimestepData[ecoregion.Index,i].AvgMinTemp,
                        TimestepData[ecoregion.Index,i].AvgMaxTemp,
                        TimestepData[ecoregion.Index,i].StdDevTemp,
                        TimestepData[ecoregion.Index,i].AvgPpt,
                        TimestepData[ecoregion.Index,i].StdDevPpt
                        );
                }
            }
            
        }
        //---------------------------------------------------------------------
        public static void Initialize(string filename, IEcoregionDataset ecoregionDataset, bool writeOutput, ICore mCore) 
        {
            modelCore = mCore;
            ModelCore.Log.WriteLine("Loading weather data from file \"{0}\" ...", filename);
            ClimateParser parser = new ClimateParser(ecoregionDataset);
            allData = ModelCore.Load<Dictionary<int, IClimateRecord[,]>>(filename, parser);
            modelCore = mCore;
            
            timestepData = allData[0];
            
            if(writeOutput)
                Write(ecoregionDataset);

        }
    }

}

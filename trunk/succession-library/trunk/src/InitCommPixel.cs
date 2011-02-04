//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo, Srinivas S.

using Landis.SpatialModeling;

namespace Landis.Library.Succession
{
    public class InitCommPixel : Pixel
    {
        public Band<int> MapCode  = "The numeric code for each initial community";

        public InitCommPixel() 
        {
            SetBands(MapCode);
        }
    }
}

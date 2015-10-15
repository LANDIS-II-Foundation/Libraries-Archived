using Wisc.Flel.GeospatialModeling.Landscapes;

namespace Landis.Library.Succession
{
        public class RelativeLocationWeighted
        {
            private RelativeLocation location;
            private double weight;

            //---------------------------------------------------------------------
            public RelativeLocation Location
            {
                get {
                    return location;
                }
                set {
                    location = value;
                }
            }
            
            public double Weight
            {
                get {
                    return weight;
                }
                set {
                    weight = value;
                }
            }
        
            public RelativeLocationWeighted (RelativeLocation location, double weight)
            {
                this.location = location;
                this.weight = weight;
            }
        }
}


namespace Landis.Raster.Erdas74
{
    /// <summary>
    /// Projections is used to map ERDAS projection names to ERDAS
    /// projection numbers and vice versa.
    /// </summary>
    static class Projections
    {
        class Pair
        {
            int i;
            string s;
            
            public Pair(int i, string s)
            {
                this.i = i;
                this.s = s;
            }
            
            public int Index
            {
                get { return this.i; }
            }
            
            public string String
            {
                get { return this.s; }
            }
        }
        
        static Pair[] pairs;
        
        /// <summary>
        /// Build the mappings between projection names and projection numbers
        /// </summary>
        static Projections()
        {
            pairs = new Pair[] {
                new Pair(0,"Geographic"),
                new Pair(1,"UTM"),
                new Pair(2,"State Plane"),
                new Pair(3,"Albers Conical Equal Area"),
                new Pair(4,"Lambert Conformal Conic"),
                new Pair(5,"Mercator"),
                new Pair(6,"Polar Sterographic"),
                new Pair(7,"Polyconic"),
                new Pair(8,"Equidistant Conic"),
                new Pair(9,"Transverse Mercator"),
                new Pair(10,"Stereographic"),
                new Pair(11,"Lambert Azimuthal Equal Area"),
                new Pair(12,"Azimuthal Equidistant"),
                new Pair(13,"Gnomonic"),
                new Pair(14,"Orthographic"),
                new Pair(15,"General Vertical Near-Side Perspective"),
                new Pair(16,"Sinusoidal"),
                new Pair(17,"Equirectangular"),
                new Pair(18,"Miller Cylindrical"),
                new Pair(19,"Van Der Grinten"),
                new Pair(20,"Oblique Mercator"),
                new Pair(99,"None")
            };
        }
        
        /// <summary>
        /// Find the number ERDAS associates with a given projection name
        /// </summary>
        static public int find(string projectionName)
        {
            for (int i = 0; i < pairs.Length; i++)
                if (pairs[i].String.Equals(projectionName))
                    return pairs[i].Index;
            return -1;
        }
        
        /// <summary>
        /// Find the name ERDAS associates with a given projection number
        /// </summary>
        static public string find(int projectionNum)
        {
            for (int i = 0; i < pairs.Length; i++)
                if (pairs[i].Index == projectionNum)
                    return pairs[i].String;
            return null;
        }
    }
    
}
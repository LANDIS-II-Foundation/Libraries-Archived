// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// A collection of parameters for computing the economic ranks of species.
    /// </summary>
    public class FireRiskTable
    {
        private FireRiskParameters[] parameters;

        //---------------------------------------------------------------------

        public FireRiskParameters this[int fuelTypeIndex]
        {
            get {
                return parameters[fuelTypeIndex];
            }

            set {
                parameters[fuelTypeIndex] = value;
            }
        }

        //---------------------------------------------------------------------

        public FireRiskTable()
        {
            parameters = new FireRiskParameters[150];  //up to 150 fuel types
            //foreach (FireRiskParameters fireRiskParm in parameters)
            //{
            //    fireRiskParm 
            //}
        }
    }
}

// This file is part of the Harvest Management library for LANDIS-II.
// For copyright and licensing information, see the NOTICE and LICENSE
// files in this project's top-level directory, and at:
//   http://landis-extensions.googlecode.com/svn/libs/harvest-mgmt/trunk/

namespace Landis.Library.HarvestManagement
{
    /// <summary>
    /// The parameters for harvest.
    /// </summary>
    public interface IInputParameters
    {
        /// <summary>
        /// Timestep (years)
        /// </summary>
        int Timestep
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the map of management areas.
        /// </summary>
        string ManagementAreaMap
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Management areas that prescriptions have been applied to.
        /// </summary>
        ManagementAreaDataset ManagementAreas
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path to the map of stands.
        /// </summary>
        string StandMap
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Template for pathnames for prescription maps.
        /// </summary>
        string PrescriptionMapNames
        {
            get;
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Path for the log file with information about harvest events.
        /// </summary>
        string EventLog
        {
            get;
        }
        //---------------------------------------------------------------------

        /// <summary>
        /// Path for the summary log file with information about harvest events.
        /// </summary>
        string SummaryLog
        {
            get;
        }
    }
}

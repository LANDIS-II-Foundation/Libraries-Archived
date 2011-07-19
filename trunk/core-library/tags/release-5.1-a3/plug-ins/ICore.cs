//  Author: Jimm Domingo, UW-Madison, FLEL

using Landis.Cohorts;
using Landis.Ecoregions;
using Landis.Landscape;
using Landis.RasterIO;
using Landis.Species;

namespace Landis.PlugIns
{
    /// <summary>
    /// Interface to the core framework for plug-ins.
    /// </summary>
	public interface ICore
	    : IRasterFactory
	{
	    /// <summary>
	    /// The dataset of species parameters for the scenario.
	    /// </summary>
		Species.IDataset Species
		{
			get;
		}

		//---------------------------------------------------------------------

	    /// <summary>
	    /// The dataset of ecoregion parameters for the scenario.
	    /// </summary>
		Ecoregions.IDataset Ecoregions
		{
			get;
		}

		//---------------------------------------------------------------------

	    /// <summary>
	    /// The ecoregion for each site on the landscape.
	    /// </summary>
		ISiteVar<IEcoregion> Ecoregion
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The landscape for the scenario.
		/// </summary>
		Landscape.ILandscape Landscape
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The map metadata for the landscape.
		/// </summary>
		RasterIO.IMetadata LandscapeMapMetadata
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The length of a side of a cell (a site on landscape).
		/// </summary>
		/// <remarks>
		/// Units: meters
		/// </remarks>
		float CellLength
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The area of a cell (a site on landscape).
		/// </summary>
		/// <remarks>
		/// Units: hectares
		/// </remarks>
		float CellArea
		{
			get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The calendar year that the scenario starts from.
		/// </summary>
		/// <remarks>
		/// This year represents time step 0, so the first year in the scenario
		/// is this year plus 1.
		/// </remarks>
		int StartTime
		{
		    get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The last calendar year in the scenario.
		/// </summary>
		int EndTime
		{
		    get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The calendar year in the current time step.
		/// </summary>
		int CurrentTime
		{
		    get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The number of years from StartTime to CurrentTime.
		/// </summary>
		int TimeSinceStart
		{
		    get;
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// The succession plug-in's cohorts for the landscape.
		/// </summary>
		Cohorts.TypeIndependent.ILandscapeCohorts SuccessionCohorts
		{
		    get;
		}
	}
}

namespace Landis.Library.Cohort.AgeOnly
{
    /// <summary>
    /// The cohorts for all the sites in the landscape.
    /// </summary>
    public interface ILandscapeCohorts
        : Cohorts.ILandscapeCohorts<ISiteCohorts>
    {
    }
}

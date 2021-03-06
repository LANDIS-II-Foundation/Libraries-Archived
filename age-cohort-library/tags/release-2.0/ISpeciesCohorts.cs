namespace Landis.AgeCohort
{
    /// <summary>
    /// The age cohorts for a particular species at a site.
    /// </summary>
    public interface ISpeciesCohorts
        : Cohorts.ISpeciesCohorts<ICohort>
    {
    }
}

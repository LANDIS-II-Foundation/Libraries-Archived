
using Landis.Core;
using System.Collections.Generic;

namespace Landis.Library.AgeOnlyCohorts
{
    /// <summary>
    /// The age cohorts for a particular species at a site.
    /// </summary>
    public interface ISpeciesCohorts
        : Landis.Library.Cohorts.ISpeciesCohorts<ICohort>
    {
    }
}

namespace Landis.Cohorts.TypeIndependent
{
    /// <summary>
    /// Information about a cohort's attribute.
    /// </summary>
    //  Implementation note: Rather than represent an attribute directly with
    //  a string, this class is used so that equality tests are faster because
    //  they are object-reference equality tests.
    //
    public class CohortAttribute
    {
        private string name;

        //---------------------------------------------------------------------

        /// <summary>
        /// The attribute's name.
        /// </summary>
        public string Name
        {
            get {
                return name;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Initializes a new instance.
        /// </summary>
        public CohortAttribute(string name)
        {
            this.name = name;
        }

        //---------------------------------------------------------------------

        public override string ToString()
        {
            return name;
        }
    }
}

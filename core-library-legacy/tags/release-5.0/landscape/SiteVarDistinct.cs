namespace Landis.Landscape
{
	/// <summary>
	/// A site variable where the inactive sites have distinct value.
	/// </summary>
	public class SiteVarDistinct<T>
		: SiteVar<T>, ISiteVar<T>
	{
		private T[,] values;

		//---------------------------------------------------------------------

		public InactiveSiteMode Mode
		{
			get {
				return InactiveSiteMode.DistinctValues;
			}
		}

		//---------------------------------------------------------------------

		public T this[Site site]
		{
			get {
				Validate(site);
				return values[site.Location.Row-1, site.Location.Column-1];
			}

			set {
				Validate(site);
				values[site.Location.Row-1, site.Location.Column-1] = value;
			}
		}

		//---------------------------------------------------------------------

		public T ActiveSiteValues
		{
			set {
				foreach (ActiveSite site in Landscape.ActiveSites)
					this[site] = value;
			}
		}

		//---------------------------------------------------------------------

		public T InactiveSiteValues
		{
			set {
				foreach (Site site in Landscape.AllSites)
					if (! site.IsActive)
						this[site] = value;
			}
		}

		//---------------------------------------------------------------------

		public T SiteValues
		{
			set {
				foreach (Site site in Landscape.AllSites)
					this[site] = value;
			}
		}

		//---------------------------------------------------------------------

		public SiteVarDistinct(ILandscape landscape)
			: base(landscape)
		{
			values = new T[landscape.Rows, landscape.Columns];
		}
	}
}

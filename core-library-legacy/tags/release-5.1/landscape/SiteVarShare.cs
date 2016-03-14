namespace Landis.Landscape
{
	/// <summary>
	/// A site variable where the inactive sites share a common value.
	/// </summary>
	public class SiteVarShare<T>
		: SiteVar<T>, ISiteVar<T>
	{
		private T[] values;

		//---------------------------------------------------------------------

		public InactiveSiteMode Mode
		{
			get {
				return InactiveSiteMode.Share1Value;
			}
		}

		//---------------------------------------------------------------------

		public T this[Site site]
		{
			get {
				Validate(site);
				return values[site.DataIndex];
			}

			set {
				Validate(site);
				values[site.DataIndex] = value;
			}
		}

		//---------------------------------------------------------------------

		public T ActiveSiteValues
		{
			set {
				for (int i = 1; i <= Landscape.ActiveSiteCount; i++)
					values[i] = value;
			}
		}

		//---------------------------------------------------------------------

		/// <summary>
		/// Sets all the values for the inactive sites to the same value.
		/// </summary>
		public T InactiveSiteValues
		{
			set {
				if (Landscape.InactiveSiteCount > 0)
					values[Landscape.InactiveSiteDataIndex] = value;
			}
		}

		//---------------------------------------------------------------------

		public T SiteValues
		{
			set {
				for (int i = 0; i < values.Length; i++)
					values[i] = value;
			}
		}

		//---------------------------------------------------------------------

		public SiteVarShare(ILandscape landscape)
			: base(landscape)
		{
			values = new T[landscape.ActiveSiteCount + 1];
		}
	}
}

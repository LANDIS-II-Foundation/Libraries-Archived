using System.Diagnostics;

namespace Landis.Landscape
{
	public class SiteVariable<T>
		: ISiteVarWithData
	{
		private string name;
		private System.Type dataType;
		private ILandscape landscape;
		private T[] data;

		//---------------------------------------------------------------------

		public string Name
		{
			get {
				return name;
			}
		}

		//---------------------------------------------------------------------

		public System.Type DataType
		{
			get {
				return dataType;
			}
		}

		//---------------------------------------------------------------------

		public ILandscape Landscape
		{
			get {
				return landscape;
			}
		}

		//---------------------------------------------------------------------

		public SiteVariable(string name)
		{
			this.name = name;
			this.dataType = typeof(T);
			this.data = null;
			this.landscape = null;
		}

		//---------------------------------------------------------------------

		public T this[Site site]
		{
			get {
				Debug.Assert( data != null );
				return data[site.DataIndex];
			}
			set {
				Debug.Assert( data != null );
				data[site.DataIndex] = value;
			}
		}

		//---------------------------------------------------------------------

		void ISiteVarWithData.AllocateData(uint siteCount)
		{
			Debug.Assert( data == null );
			data = new T[siteCount];
		}

		//---------------------------------------------------------------------

		void ISiteVarWithData.ShareData(ISiteVarWithData variable)
		{
			SiteVariable<T> var = (SiteVariable<T>) variable;
			Debug.Assert( var.data != null );
			data = var.data;
		}
	}
}

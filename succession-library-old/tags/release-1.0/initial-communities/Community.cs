using Landis.Cohorts;

namespace Landis.InitialCommunities
{
	public class Community
		: ICommunity
	{
		private ushort mapCode;
		private ISiteCohorts<AgeCohort.ICohort> cohorts;

		//---------------------------------------------------------------------

		public ushort MapCode
		{
			get {
				return mapCode;
			}
		}

		//---------------------------------------------------------------------

		public ISiteCohorts<AgeCohort.ICohort> Cohorts
		{
			get {
				return cohorts;
			}
		}

		//---------------------------------------------------------------------

		public Community(ushort                          mapCode,
		                 ISiteCohorts<AgeCohort.ICohort> cohorts)
		{
			this.mapCode = mapCode;
			this.cohorts = cohorts;
		}
	}
}

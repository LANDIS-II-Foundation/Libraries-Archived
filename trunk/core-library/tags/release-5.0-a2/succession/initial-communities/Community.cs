namespace Landis.InitialCommunities
{
	public class Community
		: ICommunity
	{
		private byte mapCode;
		private ISiteCohorts<AgeOnly.ICohort> cohorts;

		//---------------------------------------------------------------------

		public byte MapCode
		{
			get {
				return mapCode;
			}
		}

		//---------------------------------------------------------------------

		public ISiteCohorts<AgeOnly.ICohort> Cohorts
		{
			get {
				return cohorts;
			}
		}

		//---------------------------------------------------------------------

		public Community(byte                          mapCode,
		                 ISiteCohorts<AgeOnly.ICohort> cohorts)
		{
			this.mapCode = mapCode;
			this.cohorts = cohorts;
		}
	}
}

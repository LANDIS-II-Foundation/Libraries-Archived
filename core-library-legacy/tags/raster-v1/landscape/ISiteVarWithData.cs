namespace Landis.Landscape
{
	internal interface ISiteVarWithData
		: ISiteVariable
	{
		//---------------------------------------------------------------------

		void AllocateData(uint siteCount);

		//---------------------------------------------------------------------

		void ShareData(ISiteVarWithData variable);
	}
}

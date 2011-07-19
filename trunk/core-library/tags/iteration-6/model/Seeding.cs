using Landis.Landscape;

namespace Landis
{
	public class Seeding
		: IReproduction
	{
		private DisperseSeedMethod disperseSeedMethod;

		//---------------------------------------------------------------------

		public Seeding(DisperseSeedMethod disperseSeed)
		{
			this.disperseSeedMethod = disperseSeed;
		}

		//---------------------------------------------------------------------

		public void Do(ActiveSite site)
		{
			// TODO
		}
	}
}
